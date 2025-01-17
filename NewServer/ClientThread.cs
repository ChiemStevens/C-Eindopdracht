﻿using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;

namespace Server
{
    class ClientThread
    {
        private TcpClient client;
        private NetworkStream networkStream;
        private Room room;
        private System.Timers.Timer timer;

        private Thread runningThread;

        private bool threadRunning;
        private byte[] buffer;
        private string name;
        private bool recievedResponse;

        private const int BYTE_SIZE = 1024;

        public ClientThread(TcpClient client, Room room)
        {
            this.client = client;
            this.room = room;
            this.networkStream = client.GetStream();
            this.threadRunning = false;
            this.recievedResponse = true;
            this.buffer = new byte[BYTE_SIZE];

            SetTimer();
        }

        public void StartClientThread()
        {
            this.threadRunning = true;
            this.runningThread = new Thread(Run);
            this.runningThread.IsBackground = true;
            this.runningThread.Start();
        }

        public void StopClientThread()
        {
            this.threadRunning = false;
            this.runningThread.Abort();
        }

        /// <summary>
        /// This timer is set every 10 seconds, it pings the client if it is still here
        /// If there is no respone within the next 10 seconds, the client is disconnected
        /// </summary>
        private void SetTimer()
        {
            // Create a timer with a 10 second interval.
            timer = new System.Timers.Timer(10000);
            // Hook up the Elapsed event for the timer. 
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if(recievedResponse)
            {
                Console.WriteLine("Ping!");
                recievedResponse = false;
                this.SendMessage(new Message(MessageTypes.Ping, ""));
            }
            else
            {
                Console.WriteLine("No response, Disconnect");
                Console.WriteLine("Client: {0} disconnected", name);
                this.StopClientThread();
                this.room.DisconnectClient(this);
            }
        }

        /// <summary>
        /// Method used for sending multiple messages at once.
        /// </summary>
        /// <param name="messages"></param>
        public void SendMultiMessage(List<Message> messages)
        {
            string json = "";
            foreach(Message message in messages)
            {
                json += JsonConvert.SerializeObject((message)) + Util.END_MESSAGE_KEY;
            }

            byte[] messageBytes = Encoding.Unicode.GetBytes(json);
            this.networkStream.Write(messageBytes, 0, messageBytes.Length);
        }

        /// <summary>
        /// Send a message to the client
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(Message message)
        {
            string toSend = JsonConvert.SerializeObject((message)) + Util.END_MESSAGE_KEY;
            byte[] messageBytes = Encoding.Unicode.GetBytes(toSend);
            this.networkStream.Write(messageBytes, 0, messageBytes.Length);
        }

        public void JoinRoom(Room room)
        {
            this.room = room;
        }

        private void Run()
        {
            while (threadRunning)
            {
                try
                {
                    this.networkStream.Read(buffer, 0, buffer.Length);

                    string wholePacket = Encoding.Unicode.GetString(buffer);
                    string stringMessage = wholePacket.Replace("\0", "");

                    Console.WriteLine(stringMessage);

                    //Split the messages till the End key. this leaves us with a string array containing all the json messages that can be convert to objects.
                    string[] messages = stringMessage.Split(new string[] { Util.END_MESSAGE_KEY }, StringSplitOptions.None);


                    for(int i = 0; i < messages.Length; i++)
                    {
                        try
                        {
                            if (messages[i] == "")
                                continue;

                            Message message = JsonConvert.DeserializeObject<Message>(messages[i]);

                            switch (message.Type)
                            {
                                case MessageTypes.JoinRoom:
                                    JoinOtherRoom(message);
                                    break;
                                case MessageTypes.LeaveRoom:
                                    this.LeaveRoom(JsonConvert.DeserializeObject<RoomModel>(message.Data));
                                    break;
                                case MessageTypes.SendDrawing:
                                    Console.WriteLine(JsonConvert.SerializeObject(message));
                                    this.room.SendToAllClientsInRoom(message);
                                    break;
                                case MessageTypes.SendUsername:
                                    this.name = JsonConvert.DeserializeObject<ClientModel>(message.Data).Name;
                                    break;
                                case MessageTypes.StartGame:
                                    this.room.StartGame();
                                    break;
                                case MessageTypes.GuessWord:
                                    this.room.GuessWord(JsonConvert.DeserializeObject<GuessModel>(message.Data).Word, this);
                                    break;
                                case MessageTypes.UsernameCheck:
                                    this.room.CheckUsernameServer(JsonConvert.DeserializeObject<ClientModel>(message.Data).Name, this);
                                    break;
                                case MessageTypes.Pong:
                                    this.recievedResponse = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (JsonException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    //Clear the buffer
                    buffer = new byte[BYTE_SIZE];
                }
                catch (IOException ex)
                {
                    this.timer.Stop();
                    this.timer = null;
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Client: {0} disconnected", name);
                    this.room.DisconnectClient(this);
                    this.StopClientThread();
                    return;
                }
                catch(ThreadAbortException ex)
                {
                    return;
                }
            }
        }

        private void LeaveRoom(RoomModel roomModel)
        {
            if(this.room.Name != "hub")
            {
                if (this.room.Name == roomModel.Name)
                {
                    this.room.LeaveRoom(this);
                }
            }
        }

        private void JoinOtherRoom(Message message)
        {
            RoomModel roomModel = JsonConvert.DeserializeObject<RoomModel>(message.Data);
            this.room.JoinOtherRoom(this, roomModel.Name);
        }

        public string Name { get { return this.name; } }
    }
}

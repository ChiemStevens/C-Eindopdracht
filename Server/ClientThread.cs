﻿using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class ClientThread
    {
        private TcpClient client;
        private NetworkStream networkStream;
        private Room room;

        private Thread runningThread;

        private bool threadRunning;
        private byte[] buffer;
        private string name;

        private const int BYTE_SIZE = 1024;

        public ClientThread(TcpClient client, Room room)
        {
            this.client = client;
            this.room = room;
            this.networkStream = client.GetStream();
            this.threadRunning = false;
            this.buffer = new byte[BYTE_SIZE];
        }

        public void StartClientThread()
        {
            this.threadRunning = true;
            this.runningThread = new Thread(Run);
            this.runningThread.Start();
        }

        public void StopClientThread()
        {
            this.threadRunning = false;
        }

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
                    string[] messages = stringMessage.Split(new string[] { Util.END_MESSAGE_KEY }, StringSplitOptions.None);


                    for(int i = 0; i < messages.Length; i++)
                    {
                        try
                        {
                            if (messages[i] == "")
                                continue;

                            Message message = JsonConvert.DeserializeObject<Message>(messages[i]);
                            Console.WriteLine(message.Type);

                            switch (message.Type)
                            {
                                case MessageTypes.JoinRoom:
                                    JoinOtherRoom(message);
                                    break;
                                case MessageTypes.LeaveRoom:
                                    this.LeaveRoom(JsonConvert.DeserializeObject<RoomModel>(message.Data));
                                    break;
                                case MessageTypes.SendDrawing:
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
                    Console.WriteLine(ex.Message);
                    this.StopClientThread();
                    return;
                }
            }
        }

        private void LeaveRoom(RoomModel roomModel)
        {
            if(this.room.Name == roomModel.Name)
            {
                this.room.LeaveRoom(this);
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

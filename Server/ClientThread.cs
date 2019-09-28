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

        public void SendMessage(Message message)
        {
            byte[] messageBytes = Encoding.Unicode.GetBytes(Message.GetJsonString(message));
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

                    string stringMessage = Encoding.Unicode.GetString(buffer);
                    stringMessage = stringMessage.Replace("\0", "");

                    try
                    {
                        Message message = JsonConvert.DeserializeObject<Message>(stringMessage);
                        Console.WriteLine(message.Type);

                        switch (message.Type)
                        {
                            case MessageTypes.Authorize:
                                break;
                            case MessageTypes.Inform:
                                break;
                            case MessageTypes.JoinRoom:
                                JoinOtherRoom(message);
                                break;
                            case MessageTypes.LeaveRoom:
                                break;
                            case MessageTypes.SendDrawing:
                                this.room.SendToAllClientsInRoom(message);
                                break;
                            default:
                                break;
                        }
                    }
                    catch(JsonException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    

                    //Clear the buffer
                    for(int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = 0;
                    }
                }
                catch (IOException ex)
                {
                    this.StopClientThread();
                    return;
                }
            }
        }

        private void JoinOtherRoom(Message message)
        {
            RoomModel roomModel = JsonConvert.DeserializeObject<RoomModel>(message.Data);
            this.room.JoinOtherRoom(this, roomModel.Name);
        }
    }
}
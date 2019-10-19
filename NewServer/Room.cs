﻿using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Room
    {
        private Server server;
        private List<ClientThread> clients;
        private string roomname;

        private ClientThread host;
        private ClientThread drawer;
        private GameHandler gameHandler;

        private int currentDrawer; 

        public Room(Server server, string roomname)
        {
            this.server = server;
            this.roomname = roomname;
            this.gameHandler = new GameHandler(3, this);
            clients = new List<ClientThread>();
            currentDrawer = 0;
        }

        public void JoinOtherRoom(ClientThread clientThread, string roomname)
        {
            if(server.RoomExists(roomname))
            {
                server.JoinRoom(clientThread, this, roomname);
            }
            else
            {
                server.CreateRoom(clientThread, this, roomname);
            }
        }

        public void DisconnectClient(ClientThread clientThread)
        {
            RemoveClient(clientThread);
        }

        public void SendToAllClientsInRoom(Message message)
        {
            foreach(ClientThread client in clients)
            {
                client.SendMessage(message);
            }
        }

        public void SendToAllClientsInRoom(List<Message> messages)
        {
            foreach (ClientThread client in clients)
            {
                client.SendMultiMessage(messages);
            }
        }

        public void CheckUsernameServer(string username, ClientThread client)
        {
            bool isValid = this.server.CheckUsername(username);

            if(isValid)
            {
                client.SendMessage(new Message(MessageTypes.UsernameCheck, JsonConvert.SerializeObject(new ClientModel(username, true))));
            }
            else
            {
                client.SendMessage(new Message(MessageTypes.UsernameCheck, JsonConvert.SerializeObject(new ClientModel(username, false))));
            }
        }

        public bool CheckUsername(string username)
        {
            foreach(ClientThread client in clients)
            {
                if (username == client.Name)
                    return false;
            }

            return true;
        }

        public void AddClient(ClientThread clientThread)
        {
            clientThread.JoinRoom(this);
            clients.Add(clientThread);
            SendToAllClientsInRoom(new Message(MessageTypes.JoinRoom, JsonConvert.SerializeObject(new RoomModel(this.Name, this.clients.Count))));
            SendToAllClientsInRoom(new Message(MessageTypes.Inform, JsonConvert.SerializeObject(new GuessModel(clientThread.Name + " joined the room"))));

            if (this.Name.ToLower() != "hub")
            {
                if (clients.Count == 1)
                {
                    this.MakeHost(clientThread);
                }
            }
        }

        public void RemoveClient(ClientThread clientThread)
        {
            clients.Remove(clientThread);

            if(clientThread == host)
            {
                if(clients.Count > 0)
                {
                    this.MakeHost(clients[0]);
                }
                else
                {
                    this.DestroyRoom();
                    return;
                }
            }

            if(clientThread == drawer)
            {
                if(clients.Count == 1)
                {
                    gameHandler.EndGame();
                }
                else
                {
                    gameHandler.NewRound();
                }
            }

            this.SendToAllClientsInRoom(new Message(MessageTypes.Inform, JsonConvert.SerializeObject(new GuessModel(clientThread.Name + " left the room"))));
        }

        public void LeaveRoom(ClientThread clientThread)
        {
            server.JoinRoom(clientThread, this, "hub");
            SendToAllClientsInRoom(new Message(MessageTypes.JoinRoom, JsonConvert.SerializeObject(new RoomModel(this.Name, this.clients.Count))));
        }

        public void NextDrawer()
        {
            currentDrawer++;
            if (currentDrawer >= clients.Count)
            {
                currentDrawer = 0;
            }
            Console.WriteLine("New Drawer: " + clients[currentDrawer].Name);
            this.SendToAllClientsInRoom(new Message(MessageTypes.NewDrawer, JsonConvert.SerializeObject(new ClientModel(clients[currentDrawer].Name, true))));
        }

        public void GuessWord(string word, ClientThread client)
        {
            bool guessed = this.gameHandler.GuessWord(word, client.Name);  
        }

        public async void StartGame()
        {
            if(this.clients.Count > 1)
            {
                await this.gameHandler.StartGame(clients);
                this.drawer = clients[0];
                this.SendToAllClientsInRoom(new Message(MessageTypes.NewDrawer, JsonConvert.SerializeObject(new ClientModel(drawer.Name, true))));
                this.SendToAllClientsInRoom(new Message(MessageTypes.GuessWord, JsonConvert.SerializeObject(new GuessModel(this.gameHandler.Word))));
                this.SendToAllClientsInRoom(new Message(MessageTypes.StartGame, JsonConvert.SerializeObject(new GameModel(gameHandler.Word.Length, 1))));
            }
        }

        private void MakeHost(ClientThread clientThread)
        {
            this.host = clientThread;
            clientThread.SendMessage(new Message(MessageTypes.NewHost, JsonConvert.SerializeObject(new ClientModel(clientThread.Name, true))));
        }

        private void DestroyRoom()
        {
            if(this.Name.ToLower() != "hub")
            {
                if (clients.Count <= 0)
                {
                   
                    server.DestroyRoom(this);
                }
            }
        }

        private void SetDrawer(ClientThread clientThread)
        {
            this.drawer = clientThread;
        }

        public string Name { get { return roomname; } }
    }
}

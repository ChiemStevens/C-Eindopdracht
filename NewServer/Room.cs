using Newtonsoft.Json;
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

        /// <summary>
        /// Makes a client join an other room
        /// </summary>
        /// <param name="clientThread"></param>
        /// <param name="roomname"></param>
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

        /// <summary>
        /// Remove a client from the server and this room.
        /// </summary>
        /// <param name="clientThread"></param>
        public void DisconnectClient(ClientThread clientThread)
        {
            RemoveClient(clientThread);
        }

        /// <summary>
        /// Sends a message to all clients in this room.
        /// </summary>
        /// <param name="message"></param>
        public void SendToAllClientsInRoom(Message message)
        {
            foreach(ClientThread client in clients)
            {
                client.SendMessage(message);
            }
        }

        /// <summary>
        /// Send multiple messages to all clients in thie room
        /// </summary>
        /// <param name="messages"></param>
        public void SendToAllClientsInRoom(List<Message> messages)
        {
            foreach (ClientThread client in clients)
            {
                client.SendMultiMessage(messages);
            }
        }

        /// <summary>
        /// Check if the entered username is available for use.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="client"></param>
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

        /// <summary>
        /// Returns true if the username is not yet picked, false if username already exists
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool CheckUsername(string username)
        {
            foreach(ClientThread client in clients)
            {
                if (username == client.Name)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Adds a client to this room.
        /// </summary>
        /// <param name="clientThread"></param>
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

        /// <summary>
        /// Remove a client from this room, also checks if he is a drawer or a host.
        /// </summary>
        /// <param name="clientThread"></param>
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

        /// <summary>
        /// Leave this room and returns to the hub.
        /// </summary>
        /// <param name="clientThread"></param>
        public void LeaveRoom(ClientThread clientThread)
        {
            server.JoinRoom(clientThread, this, "hub");
            SendToAllClientsInRoom(new Message(MessageTypes.JoinRoom, JsonConvert.SerializeObject(new RoomModel(this.Name, this.clients.Count))));
        }

        /// <summary>
        /// choice the next drawer.
        /// </summary>
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

        /// <summary>
        /// An attempt to guess the word.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="client"></param>
        public void GuessWord(string word, ClientThread client)
        {
            this.gameHandler.GuessWord(word, client.Name);  
        }

        /// <summary>
        /// Start the game, can only happen if there are more than 2 people in the room.
        /// </summary>
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

        /// <summary>
        /// Make the entered clientthread the host.
        /// </summary>
        /// <param name="clientThread"></param>
        private void MakeHost(ClientThread clientThread)
        {
            this.host = clientThread;
            clientThread.SendMessage(new Message(MessageTypes.NewHost, JsonConvert.SerializeObject(new ClientModel(clientThread.Name, true))));
        }

        /// <summary>
        /// Destory this room, cannot be called if this room name is hub.
        /// </summary>
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

        /// <summary>
        /// Set the new drawer to a clientThread
        /// </summary>
        /// <param name="clientThread"></param>
        private void SetDrawer(ClientThread clientThread)
        {
            this.drawer = clientThread;
        }

        public string Name { get { return roomname; } }
    }
}

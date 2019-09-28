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

        public Room(Server server, string roomname)
        {
            this.server = server;
            this.roomname = roomname;
            clients = new List<ClientThread>();
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

        public void SendToAllClientsInRoom(Message message)
        {
            foreach(ClientThread client in clients)
            {
                client.SendMessage(message);
            }
        }

        public void AddClient(ClientThread clientThread)
        {
            clients.Add(clientThread);

            if(clients.Count == 1)
            {
                host = clientThread;
            }
        }

        public void RemoveClient(ClientThread clientThread)
        {
            clients.Remove(clientThread);

            if(clientThread == host)
            {
                if(clients.Count > 0)
                {
                    host = clients[0];
                }
                else
                {
                    this.DestroyRoom();
                }
            }
        }

        private void DestroyRoom()
        {
            if(clients.Count <= 0)
            {
                server.DestroyRoom(this);
            }
        }

        public string Name { get { return roomname; } }
    }
}

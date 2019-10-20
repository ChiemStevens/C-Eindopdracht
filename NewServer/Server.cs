using Shared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Server
    {
        private IPEndPoint iPEndPoint;
        private TcpListener listener;
        private List<Room> rooms = new List<Room>();
        private List<Room> roomsToDestory = new List<Room>();
        private Room hub;

        public Server()
        {
            this.iPEndPoint = new IPEndPoint(IPAddress.Any, 12242);
            this.listener = new TcpListener(iPEndPoint);
            this.listener.Start();

            Console.WriteLine(@"Started listening requests at: {0}:{1}", iPEndPoint.Address, iPEndPoint.Port);

            hub = new Room(this, "hub");
            rooms.Add(hub);
        }

        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            while (true)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                ClientThread clientThread = new ClientThread(tcpClient, hub);
                clientThread.StartClientThread();
                hub.AddClient(clientThread);
            }
        }

        /// <summary>
        /// Send a message to the whole server
        /// </summary>
        /// <param name="message"></param>
        public void SendToWholeServer(Message message)
        {
            foreach(Room room in rooms)
            {
                room.SendToAllClientsInRoom(message);
            }
        }

        /// <summary>
        /// Check if a room exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RoomExists(string name)
        {
            foreach (Room room in rooms)
            {
                if (room.Name.ToLower() == name.ToLower())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if a username exists
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool CheckUsername(string username)
        {
            foreach(Room room in rooms)
            {
                bool validUsername = room.CheckUsername(username);

                if (!validUsername)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Makes a clientThread join another room
        /// </summary>
        /// <param name="clientThread"></param>
        /// <param name="currentRoom"></param>
        /// <param name="name"></param>
        public void JoinRoom(ClientThread clientThread, Room currentRoom, string name)
        {
            if(RoomExists(name))
            {
                foreach(Room room in rooms)
                {
                    if (room.Name.ToLower() == name.ToLower())
                    {
                        currentRoom.RemoveClient(clientThread);
                        room.AddClient(clientThread);
                    }
                }

                DestoryRooms();
            }
        }

        /// <summary>
        /// Create a new room, only happens if the room does not already exists.
        /// </summary>
        /// <param name="clientThread"></param>
        /// <param name="currentRoom"></param>
        /// <param name="name"></param>
        public void CreateRoom(ClientThread clientThread, Room currentRoom, string name)
        {
            if(!RoomExists(name))
            {
                currentRoom.RemoveClient(clientThread);
                Room room = new Room(this, name);
                room.AddClient(clientThread);
                rooms.Add(room);
                clientThread.JoinRoom(room);
            }
        }

        /// <summary>
        /// Destroy a room, only when the room name is not hub
        /// </summary>
        /// <param name="room"></param>
        public void DestroyRoom(Room room)
        {
            if(room.Name.ToLower() != "hub")
            {
                if (RoomExists(room.Name))
                {
                    roomsToDestory.Add(room);
                }
            }
        }

        private void DestoryRooms()
        {
            foreach(Room room in roomsToDestory)
            {
                rooms.Remove(room);
            }
        }
    }
}

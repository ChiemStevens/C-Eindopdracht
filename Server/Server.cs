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
            this. iPEndPoint = new IPEndPoint(IPAddress.Loopback, 1234);
            this.listener = new TcpListener(iPEndPoint);
            this.listener.Start();

            Console.WriteLine(@"Started listening requests at: {0}:{1}", iPEndPoint.Address, iPEndPoint.Port);

            hub = new Room(this, "hub");
            rooms.Add(hub);
        }

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

        public void SendToWholeServer(Message message)
        {
            foreach(Room room in rooms)
            {
                room.SendToAllClientsInRoom(message);
            }
        }

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

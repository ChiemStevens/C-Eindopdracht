using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class ClientHandler
    {
        private static ClientHandler instance;

        private string name;
        private bool isHost;
        private string roomName;
        private int roomSize;

        private ClientHandler()
        {
            isHost = false;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public void SetRoomname(string name)
        {
            this.roomName = name;
            DrawHandler.GetInstance().SetRoomNameLabel(name);
        }

        public void SetRoomSize(int roomSize)
        {
            this.roomSize = roomSize;
            DrawHandler.GetInstance().SetRoomSizeLabel(roomSize);
        }

        public void SetHost(ClientModel clientModel)
        {
            if(this.name == clientModel.Name)
            {
                this.isHost = true;
                DrawHandler.GetInstance().ShowHostGrid();
            }
            else
            {
                this.isHost = false;
                DrawHandler.GetInstance().HideHostGrid();
            }
        }

        public static ClientHandler GetInstance()
        {
            if(instance == null)
            {
                instance = new ClientHandler();
            }
            return instance;
        }

        public string Name { get { return this.name; } }

        public bool IsHost { get { return this.isHost; } }

        public string Roomname { get { return this.roomName; } }
    }
}

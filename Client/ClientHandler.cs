using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    /// <summary>
    /// Class that handles all client specific things
    /// CanDraw, IsHost and communication with the DrawHandler class
    /// </summary>
    class ClientHandler
    {
        private static ClientHandler instance;

        private string name;
        private bool isHost;
        private string roomName;
        private int roomSize;
        private int wordSize;

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

        public void SetWordSize(int wordSize)
        {
            this.wordSize = wordSize;
            DrawHandler.GetInstance().SetWordSizeLabel(wordSize);
            DrawHandler.GetInstance().ShowWordGrid();
        }

        public void SetRoundLabel(int rounds)
        {
            DrawHandler.GetInstance().SetRoundsLabel(rounds);
        }

        public void SetWord(string word)
        {
            DrawHandler.GetInstance().SetWord(word);
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

        public void ShowWinners(EndGameModel endGameModel)
        {
            DrawHandler.GetInstance().ShowWinners(endGameModel);
        }

        public void CheckUsername(bool validName)
        {
            if(validName)
            {
                DrawHandler.GetInstance().SetUsername();
            }
            else
            {
                DrawHandler.GetInstance().WrongUsername();
            }
        }

        public void LeaveRoom()
        {
            this.isHost = false;
            DrawHandler.GetInstance().HideHostGrid();
            DrawHandler.GetInstance().HideDrawGrid();
            DrawHandler.GetInstance().HideWordGrid();
            DrawHandler.GetInstance().CanDraw = false;
        }

        public void EndGame()
        {
            DrawHandler.GetInstance().HideWordGrid();
            DrawHandler.GetInstance().HideDrawGrid();
            if(this.isHost)
            {
                DrawHandler.GetInstance().ShowHostGrid();
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

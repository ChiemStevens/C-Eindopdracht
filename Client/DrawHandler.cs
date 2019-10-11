using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Shared;

namespace Client
{
    class DrawHandler
    {
        private static DrawHandler instance;

        private MainWindow mainWindow;
        private bool initialized;
        private bool canDraw;
        private Color color;

        private DrawHandler()
        {
            canDraw = false;
            initialized = false;
            color = Colors.Black;
        }

        public void Initialize(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            initialized = true;
        }

        public void SetRoomNameLabel(string roomname)
        {
            mainWindow.SetRoomnameLabel(roomname);
        }

        public void SetRoomSizeLabel(int roomSize)
        {
            mainWindow.SetRoomsizeLabel(roomSize);
        }

        public void SetRoundsLabel(int rounds)
        {
            mainWindow.SetRoundLabel(rounds);
        }

        public void SetWordSizeLabel(int wordSize)
        {
            if(!this.canDraw)
            {
                mainWindow.SetWordSizeLabel(wordSize);
            }
        }

        public void SetWord(string word)
        {
            if(this.canDraw)
            {
                mainWindow.SetWord(word);
            }
        }

        public void SetColor(Color color)
        {
            if(this.canDraw)
            {
                this.color = color;
            }
        }

        public void CheckDrawer(ClientModel client)
        {
            if(initialized)
            {
                if (client.Name == ClientHandler.GetInstance().Name)
                {
                    this.canDraw = true;
                    this.mainWindow.ShowDrawGrid();
                }
                else
                {
                    this.canDraw = false;
                    this.mainWindow.HideDrawGrid();
                }
            }
        }

        public void DrawLine(DrawPoint drawPoint)
        {
            if(initialized)
            {
                mainWindow.DrawLine(drawPoint);
            }
        }

        public void ShowHostGrid()
        {
            if(initialized)
            {
                mainWindow.ShowHostGrid();
            }
        }

        public void HideHostGrid()
        {
            if(initialized)
            {
                mainWindow.HideHostGrid();
            }
        }

        public void ShowWordGrid()
        {
            if (initialized)
            {
                mainWindow.ShowWordGrid();
            }
        }

        public void HideWordGrid()
        {
            if (initialized)
            {
                mainWindow.HideWordGrid();
            }
        }

        public void WriteMessage(string text)
        {
            if(initialized)
            {
                mainWindow.WriteChatMessage(text);
            }
        }

        public static DrawHandler GetInstance()
        {
            if(instance == null)
            {
                instance = new DrawHandler();
            }
            return instance;
        }

        public bool CanDraw { get { return canDraw; } }

        public Color Color { get { return color; } }
    }
}

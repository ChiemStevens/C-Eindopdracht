using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Client
{
    class DrawHandler
    {
        private static DrawHandler instance;

        private MainWindow mainWindow;
        private bool initialized;
        private bool canDraw;

        private DrawHandler()
        {
            canDraw = false;
            initialized = false;
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

        public void SetWordSizeLabel(int wordSize)
        {
            mainWindow.SetWordSizeLabel(wordSize);
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

        public static DrawHandler GetInstance()
        {
            if(instance == null)
            {
                instance = new DrawHandler();
            }
            return instance;
        }

        public bool CanDraw { get { return canDraw; } }
    }
}

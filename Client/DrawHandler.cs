﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Shared;

namespace Client
{
    /// <summary>
    /// Handles all GUI related tasks. Has an connection with the MainWindow class.
    /// </summary>
    class DrawHandler
    {
        private static DrawHandler instance;

        private MainWindow mainWindow;
        private bool initialized;
        private bool canDraw;
        private Color color;
        private double lineThickness;

        private DrawHandler()
        {
            canDraw = false;
            initialized = false;
            color = Colors.Black;
            LineThickness = 1;
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

        public void ClearCanvas()
        {
            mainWindow.ClearCanvas();
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

        public void ShowNoConnection()
        {
            if(initialized)
            {
                mainWindow.ShowNoConnection();
            }
        }

        public void HideNoConnection()
        {
            if(initialized)
            {
                mainWindow.HideNoConnection();
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

        public void HideWinners()
        {
            if(initialized)
            {
                mainWindow.HidewinningGrid();
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

        public void ShowDrawGrid()
        {
            this.mainWindow.ShowDrawGrid();
        }

        public void HideDrawGrid()
        {
            this.mainWindow.HideDrawGrid();
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

        public void ShowWinners(EndGameModel endGameModel)
        {
            mainWindow.FillWiningGrid(endGameModel);
            mainWindow.ShowWinningGrid();
        }

        public void WrongUsername()
        {
            if(initialized)
            {
                mainWindow.WrongUsername();
            }
        }

        public void SetUsername()
        {
            if(initialized)
            {
                mainWindow.SetUsername();
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

        public bool CanDraw { get { return canDraw; } set { canDraw = value; } }

        public Color Color { get { return color; } }

        public double LineThickness { get { return lineThickness; } set { lineThickness = value; } }
    }
}

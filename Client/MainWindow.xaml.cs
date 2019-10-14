using Newtonsoft.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Shared;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point currentPoint = new Point();
        private Connector connector;

        public MainWindow()
        {
            InitializeComponent();

            HideHostGrid();
            ToolGrid.Visibility = Visibility.Hidden;
            DrawGrid.Visibility = Visibility.Hidden;
            GridWord.Visibility = Visibility.Hidden;
            winningGrid.Visibility = Visibility.Hidden;
            gridNoConnection.Visibility = Visibility.Hidden;
            lblWrongUsername.Visibility = Visibility.Hidden;

            pencilThickness.Items.Add("1");
            pencilThickness.Items.Add("2");
            pencilThickness.Items.Add("3");
            pencilThickness.Items.Add("4");
            pencilThickness.Items.Add("5");
            pencilThickness.Items.Add("6");
            pencilThickness.Items.Add("7");
            pencilThickness.Items.Add("8");
            pencilThickness.SelectedItem = "1";

            DrawHandler.GetInstance().Initialize(this);
            this.connector = new Connector();
        }

        private void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(DrawHandler.GetInstance().CanDraw)
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                    currentPoint = e.GetPosition(this);
            }
        }

        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(DrawHandler.GetInstance().CanDraw)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Line line = new Line();

                    SolidColorBrush brush = new SolidColorBrush();
                    brush.Color = DrawHandler.GetInstance().Color;
                    line.Stroke = brush;

                    line.StrokeThickness = DrawHandler.GetInstance().LineThickness;
                    line.X1 = currentPoint.X;
                    line.Y1 = currentPoint.Y;
                    line.X2 = e.GetPosition(this).X;
                    line.Y2 = e.GetPosition(this).Y;

                    currentPoint = e.GetPosition(this);

                    DrawPoint drawpoint = DrawPoint.CreatePointFromLine(line, DrawHandler.GetInstance().Color);
                    this.connector.SendDrawPoint(drawpoint);

                    paintSurface.Children.Add(line);
                }
            }
        }

        public void DrawLine(DrawPoint drawPoint)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                Line line = new Line();
                SolidColorBrush brush = new SolidColorBrush();

                brush.Color = drawPoint.Color;
                line.Stroke = brush;

                line.StrokeThickness = drawPoint.Thickness;
                line.X1 = drawPoint.X1;
                line.X2 = drawPoint.X2;
                line.Y1 = drawPoint.Y1;
                line.Y2 = drawPoint.Y2;

                paintSurface.Children.Add(line);
            }));
        }

        public void ClearCanvas()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                paintSurface.Children.Clear();
            }));
        }

        public void ShowHostGrid()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                hostGrid.Visibility = Visibility.Visible;
            }));
        }

        public void HideHostGrid()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                hostGrid.Visibility = Visibility.Hidden;
            }));
        }

        public void ShowDrawGrid()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                chat.IsEnabled = false;
                DrawGrid.Visibility = Visibility.Visible;
            }));
        }

        public void HideDrawGrid()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                chat.IsEnabled = true;
                DrawGrid.Visibility = Visibility.Hidden;
            }));
        }

        public void ShowWordGrid()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                GridWord.Visibility = Visibility.Visible;
            }));
        }

        public void HideWordGrid()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                GridWord.Visibility = Visibility.Hidden;
            }));
        }

        public void SetRoomnameLabel(string name)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                currentRoom.Content = "Current room: " + name;
            }));
        }

        public void SetRoomsizeLabel(int size)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                players.Content = "Players: " + size + "/8";
            }));
        }

        public void SetWordSizeLabel(int wordSize)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                lblWord.Content = "";
                for(int i = 0; i <= wordSize; i++)
                {
                    lblWord.Content += "_ ";
                }
            }));
        }

        public void SetWord(string word)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                lblWord.Content = word;
            }));
        }

        public void WriteChatMessage(string text)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                chatBox.Text += text + "\r\n";
            }));
        }

        public void SetRoundLabel(int currentRound)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                lblRounds.Content = "Ronde: " + currentRound + "/ 3"; 
            }));
        }

        public void ShowWinningGrid()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                winningGrid.Visibility = Visibility.Visible;
            }));
        }

        public void HidewinningGrid()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                winningGrid.Visibility = Visibility.Hidden;
            }));
        }

        public void ShowNoConnection()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                gridNoConnection.Visibility = Visibility.Visible;
                StartGrid.Visibility = Visibility.Hidden;
            }));
        }

        public void HideNoConnection()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                gridNoConnection.Visibility = Visibility.Hidden;
                StartGrid.Visibility = Visibility.Visible;
            }));
        }

        public void SetUsername()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                connector.SendUserName(txtUsername.Text);
                StartGrid.Visibility = Visibility.Hidden;
                ToolGrid.Visibility = Visibility.Visible;
            }));
        }

        public void WrongUsername()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                lblWrongUsername.Visibility = Visibility.Visible;
            }));
        }

        public void FillWiningGrid(EndGameModel endGameModel)
        {
            int counter = 1;
            Dictionary<string, int> sortedDictionary = endGameModel.Winners.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            foreach (KeyValuePair<string, int> item in sortedDictionary)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    switch(counter)
                    {
                        case 1:
                            lblFirst.Content = string.Format("1st: {0} {1} points", item.Key, item.Value);
                            break;
                        case 2:
                            lblSecond.Content = string.Format("2nd: {0} {1} points", item.Key, item.Value);
                            break;
                        case 3:
                            lblThid.Content = string.Format("3rd: {0} {1} points", item.Key, item.Value);
                            break;
                        case 4:
                            lblFourth.Content = string.Format("4rt: {0} {1} points", item.Key, item.Value);
                            break;
                        case 5:
                            lblFifth.Content = string.Format("5th: {0} {1} points", item.Key, item.Value);
                            break;
                        case 6:
                            lblSixth.Content = string.Format("6th: {0} {1} points", item.Key, item.Value);
                            break;
                        case 7:
                            lblSeventh.Content = string.Format("7th: {0} {1} points", item.Key, item.Value);
                            break;
                        case 8:
                            lblEigth.Content = string.Format("8th: {0} {1} points", item.Key, item.Value);
                            break;
                        default:
                            break;
                    }

                    counter++;
                }));
            }

            if(counter < 8)
            {
                for (int i = counter; i <= 8; i++)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        switch (i)
                        {
                            case 1:
                                lblFirst.Content = "";
                                break;
                            case 2:
                                lblSecond.Content = "";
                                break;
                            case 3:
                                lblThid.Content = "";
                                break;
                            case 4:
                                lblFourth.Content = "";
                                break;
                            case 5:
                                lblFifth.Content = "";
                                break;
                            case 6:
                                lblSixth.Content = "";
                                break;
                            case 7:
                                lblSeventh.Content = "";
                                break;
                            case 8:
                                lblEigth.Content = "";
                                break;
                            default:
                                break;
                        }
                    }));
                }
            }
        }

        private void Btn_JoinRoom_Click(object sender, RoutedEventArgs e)
        {
            this.connector.SendRoomName(txtRoomName.Text);
        }

        private void btnEnterGame_Click(object sender, RoutedEventArgs e)
        {
            ClientHandler.GetInstance().SetName(txtUsername.Text);
            this.connector.SendCheckUsername(txtUsername.Text);
        }

        private void btnColor_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush buttonBrush = (SolidColorBrush) (sender as Button).Background;
            DrawHandler.GetInstance().SetColor(buttonBrush.Color);
        }

        private void btn_Leaveroom_Click(object sender, RoutedEventArgs e)
        {
            this.connector.LeaveRoom();
        }

        private void btn_StartGame_Click(object sender, RoutedEventArgs e)
        {
            this.connector.StartGame();
        }

        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            SendChatMessage();
        }

        private void chat_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                SendChatMessage();
            }
        } 

        private void SendChatMessage()
        {
            this.connector.SendGuessModel(chat.Text);
            chat.Text = "";
        }

        private void pencilThickness_Selected(object sender, RoutedEventArgs e)
        {
            int lineThickness = int.Parse(pencilThickness.SelectedIndex.ToString());
            if (lineThickness == 0)
                lineThickness = 1;

            DrawHandler.GetInstance().LineThickness = lineThickness;
        }
    }
}


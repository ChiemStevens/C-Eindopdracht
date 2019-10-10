using Newtonsoft.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Shared;
using System;

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

            pencilThickness.Items.Add("1");
            pencilThickness.Items.Add("2");
            pencilThickness.Items.Add("3");
            pencilThickness.Items.Add("4");
            pencilThickness.SelectedItem = "1";

            this.connector = new Connector();

            DrawHandler.GetInstance().Initialize(this);

            HideHostGrid();
            ToolGrid.Visibility = Visibility.Hidden;
            DrawGrid.Visibility = Visibility.Hidden;
            GridWord.Visibility = Visibility.Hidden;
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

                    SolidColorBrush redBrush = new SolidColorBrush();
                    redBrush.Color = Colors.Black;
                    line.Stroke = redBrush;

                    line.StrokeThickness = 1;
                    line.X1 = currentPoint.X;
                    line.Y1 = currentPoint.Y;
                    line.X2 = e.GetPosition(this).X;
                    line.Y2 = e.GetPosition(this).Y;

                    currentPoint = e.GetPosition(this);

                    DrawPoint drawpoint = DrawPoint.CreatePointFromLine(line, Colors.Black);
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
                DrawGrid.Visibility = Visibility.Visible;
            }));
        }

        public void HideDrawGrid()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
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

        private void Btn_JoinRoom_Click(object sender, RoutedEventArgs e)
        {
            this.connector.SendRoomName(txtRoomName.Text);
        }

        private void btnEnterGame_Click(object sender, RoutedEventArgs e)
        {
            ClientHandler.GetInstance().SetName(txtUsername.Text);
            connector.SendUserName(txtUsername.Text);
            StartGrid.Visibility = Visibility.Hidden;
            ToolGrid.Visibility = Visibility.Visible;
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
    }
}


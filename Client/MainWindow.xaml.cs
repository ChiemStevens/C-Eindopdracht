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
            this.connector = new Connector(this);
        }

        private void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                currentPoint = e.GetPosition(this);
        }

        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
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

        private void Btn_JoinRoom_Click(object sender, RoutedEventArgs e)
        {
            this.connector.SendRoomName(txtRoomName.Text);
        }
    }
}


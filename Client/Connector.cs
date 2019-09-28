using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Connector
    {
        private TcpClient client;
        private NetworkStream stream;

        private MainWindow tempMainWindow;

        /// <summary>
        /// Makes connection to the draw server and saves the connection in the client and stream variable. 
        /// </summary>
        public  Connector(MainWindow mainWindow)
        {
            this.tempMainWindow = mainWindow;

            client = new System.Net.Sockets.TcpClient("127.0.0.1", 1234); // Create a new connection
            stream = client.GetStream();

            Message message = new Message(MessageTypes.Inform, "Hello There!");
            sendMessage(message);

            StartReading();
        }

        /// <summary>
        /// Send a drawpoint to the server. It contains the position, color and thickness of a line
        /// The server will send this to other clients to show them the drawing of the current selected drawer. 
        /// </summary>
        /// <param name="drawPoint"></param>
        public void SendDrawPoint(DrawPoint drawPoint)
        {
            this.sendMessage(new Message(MessageTypes.SendDrawing, JsonConvert.SerializeObject(drawPoint)));
        }

        public void SendRoomName(string roomname)
        {
            this.sendMessage(new Message(MessageTypes.JoinRoom, JsonConvert.SerializeObject(new RoomModel(roomname))));
        }

        /// <summary>
        /// Start reading messages from the server on a new thread. 
        /// </summary>
        private void StartReading()
        {
            new Thread(() =>
            {
                this.ReadMessage();
            }).Start();
        }

        /// <summary>
        /// Read a message from the stream
        /// </summary>
        private void ReadMessage()
        {
            byte[] buffer = new byte[1024];
            stream.Read(buffer, 0, buffer.Length);
            Message message = JsonConvert.DeserializeObject<Message>(Encoding.Unicode.GetString(buffer));

            if(message.Type == MessageTypes.SendDrawing)
            {
                DrawPoint drawPoint = JsonConvert.DeserializeObject<DrawPoint>(message.Data);
                this.tempMainWindow.DrawLine(drawPoint);
                //Console.WriteLine(Message.GetJsonString(message));
            }

            this.ReadMessage();
        }

        /// <summary>
        /// Send a message to the server. Messagebytes is a string converted to a byte array. 
        /// </summary>
        /// <param name="messageBytes"></param>
        private void sendMessage(Message message)
        {
            byte[] messageBytes = Encoding.Unicode.GetBytes(Message.GetJsonString(message));

            try
            {
                stream.Write(messageBytes, 0, messageBytes.Length); // Write the bytes
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}

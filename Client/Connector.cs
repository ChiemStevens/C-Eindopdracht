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
        private string firstHalfFromBuffer;
        private string secondHalfFromBuffer;
        private bool incompleteMessage;

        /// <summary>
        /// Makes connection to the draw server and saves the connection in the client and stream variable. 
        /// </summary>
        public Connector()
        {
            try
            {
                //client = new System.Net.Sockets.TcpClient("86.82.166.205", 12242); // Create a new connection
                client = new System.Net.Sockets.TcpClient("127.0.0.1", 12242); // Create a new connection
                stream = client.GetStream();

                Message message = new Message(MessageTypes.Inform, "Hello There!");
                sendMessage(message);

                StartReading();
            }
            catch(Exception ex)
            {
                DrawHandler.GetInstance().ShowNoConnection();
            }
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
            this.sendMessage(new Message(MessageTypes.JoinRoom, JsonConvert.SerializeObject(new RoomModel(roomname, 0))));
        }

        public void LeaveRoom()
        {
            ClientHandler.GetInstance().LeaveRoom();
            this.sendMessage(new Message(MessageTypes.LeaveRoom, JsonConvert.SerializeObject(new RoomModel(ClientHandler.GetInstance().Roomname, 0))));
        }

        public void SendUserName(string username)
        {
            this.sendMessage(new Message(MessageTypes.SendUsername, JsonConvert.SerializeObject(new ClientModel(username, true))));
        }

        public void SendGuessModel(string guessedWord)
        {
            this.sendMessage(new Message(MessageTypes.GuessWord, JsonConvert.SerializeObject(new GuessModel(guessedWord))));
        }

        public void SendCheckUsername(string username)
        {
            this.sendMessage(new Message(MessageTypes.UsernameCheck, JsonConvert.SerializeObject(new ClientModel(username, false))));
        }

        public void StartGame()
        {
            this.sendMessage(new Message(MessageTypes.StartGame, ""));
        }

        /// <summary>
        /// Start reading messages from the server on a new thread. 
        /// </summary>
        private void StartReading()
        {
            Thread t = new Thread(ReadMessage);
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// Read a message from the stream
        /// </summary>
        private void ReadMessage()
        {
            byte[] buffer = new byte[1024];
            stream.Read(buffer, 0, buffer.Length);

            string wholePacket = Encoding.Unicode.GetString(buffer);
            string stringMessage = wholePacket.Replace("\0", "");
            string[] messages = stringMessage.Split(new string[] { Util.END_MESSAGE_KEY }, StringSplitOptions.None);

            int length = messages.Length;

            if (incompleteMessage)
            {
                secondHalfFromBuffer = messages[0];
                messages[0] = firstHalfFromBuffer + secondHalfFromBuffer;
            }

            if (!messages[messages.Length - 1].Contains(Util.END_MESSAGE_KEY))
            {
                length -= 1;
                firstHalfFromBuffer = messages[messages.Length - 1];
                incompleteMessage = true;
            }
            

            for (int i = 0; i < length; i++)
            {
                if (messages[i] == "")
                    continue;

                HandleMessage(messages[i]);
            }

            this.ReadMessage();
        }

        private void HandleMessage(string stringMessage)
        {

            Message message = JsonConvert.DeserializeObject<Message>(stringMessage);

            switch (message.Type)
            {
                case MessageTypes.Inform:
                    GuessModel guessModel = JsonConvert.DeserializeObject<GuessModel>(message.Data);
                    DrawHandler.GetInstance().WriteMessage(guessModel.Word);
                    break;
                case MessageTypes.SendDrawing:
                    DrawPoint drawPoint = JsonConvert.DeserializeObject<DrawPoint>(message.Data);
                    DrawHandler.GetInstance().DrawLine(drawPoint);
                    break;
                case MessageTypes.NewDrawer:
                    DrawHandler.GetInstance().CheckDrawer(JsonConvert.DeserializeObject<ClientModel>(message.Data));
                    break;
                case MessageTypes.NewHost:
                    ClientHandler.GetInstance().SetHost(JsonConvert.DeserializeObject<ClientModel>(message.Data));
                    break;
                case MessageTypes.JoinRoom:
                    RoomModel room = JsonConvert.DeserializeObject<RoomModel>(message.Data);
                    ClientHandler.GetInstance().SetRoomname(room.Name);
                    ClientHandler.GetInstance().SetRoomSize(room.AmountOfPlayers);
                    break;
                case MessageTypes.StartGame:
                    GameModel gameModel = JsonConvert.DeserializeObject<GameModel>(message.Data);
                    ClientHandler.GetInstance().SetWordSize(gameModel.LengthOfWord);
                    ClientHandler.GetInstance().SetRoundLabel(gameModel.CurrentRound);
                    DrawHandler.GetInstance().HideHostGrid();
                    DrawHandler.GetInstance().HideWinners();
                    break;
                case MessageTypes.EndGame:
                    EndGameModel endGameModel = JsonConvert.DeserializeObject<EndGameModel>(message.Data);
                    ClientHandler.GetInstance().ShowWinners(endGameModel);
                    ClientHandler.GetInstance().EndGame();
                    break;
                case MessageTypes.GuessWord:
                    ClientHandler.GetInstance().SetWord(JsonConvert.DeserializeObject<GuessModel>(message.Data).Word);
                    break;
                case MessageTypes.NewRound:
                    gameModel = JsonConvert.DeserializeObject<GameModel>(message.Data);
                    ClientHandler.GetInstance().SetWordSize(gameModel.LengthOfWord);
                    ClientHandler.GetInstance().SetRoundLabel(gameModel.CurrentRound);
                    DrawHandler.GetInstance().ClearCanvas();
                    break;
                case MessageTypes.UsernameCheck:
                    bool validName = JsonConvert.DeserializeObject<ClientModel>(message.Data).ValidName;
                    ClientHandler.GetInstance().CheckUsername(validName);
                    break;
                case MessageTypes.Ping:
                    this.sendMessage(new Message(MessageTypes.Pong, ""));
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Send a message to the server.
        /// </summary>
        private void sendMessage(Message message)
        {
            string toSend = JsonConvert.SerializeObject(message) + Util.END_MESSAGE_KEY;
            byte[] messageBytes = Encoding.Unicode.GetBytes(toSend);

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

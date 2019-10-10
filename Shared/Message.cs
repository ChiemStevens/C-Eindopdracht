using Newtonsoft.Json;
using System;

namespace Shared
{
    public class Message
    {
        private MessageTypes type;
        private string data;

        public Message(MessageTypes type, string data)
        {
            this.type = type;
            this.data = data;
        }

        public static string GetJsonString(Message message)
        {
            return JsonConvert.SerializeObject(message);
        }


        public MessageTypes Type
        {
            get { return type; }
        }

        public string Data
        {
            get { return data; }
        }
    }

    public enum MessageTypes
    {
        Authorize,
        Inform,
        SendDrawing,
        JoinRoom,
        LeaveRoom,
        NewDrawer,
        NewHost,
        SendUsername,
        AmountOfPlayers,
        StartGame
    }
}

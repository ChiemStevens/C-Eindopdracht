using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class GameHandler
    {
        private bool started;
        private string word;
        private Dictionary<string, int> usersAndPoints;

        public GameHandler()
        {
            this.started = false;
            this.word = "";
            this.usersAndPoints = new Dictionary<string, int>();
        }

        public void StartGame(List<ClientThread> clients)
        {
            if(!this.started)
            {
                this.started = true;
                foreach(ClientThread client in clients)
                {
                    usersAndPoints.Add(client.Name, 0);
                }
            }
        }
    }
}

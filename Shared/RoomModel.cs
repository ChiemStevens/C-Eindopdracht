using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class RoomModel
    {
        private string name;
        private int amountOfPlayers;

        public RoomModel(string name, int amountOfPlayers)
        {
            this.name = name;
            this.amountOfPlayers = amountOfPlayers;
        }

        public string Name { get { return name; } set { name = value; } }

        public int AmountOfPlayers { get { return amountOfPlayers; } set { amountOfPlayers = value; } }
    }
}

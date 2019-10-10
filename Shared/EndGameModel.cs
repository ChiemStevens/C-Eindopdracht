using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class EndGameModel
    {
        private Dictionary<string, int> winners;

        public EndGameModel(Dictionary<string, int> winners)
        {
            this.winners = winners;
        }

        public Dictionary<string, int> Winners { get { return this.winners; } }
    }
}

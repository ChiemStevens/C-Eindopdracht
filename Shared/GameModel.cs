using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class GameModel
    {
        private int lengthOfWord;
        private int currentRound;

        public GameModel(int lengthOfWord, int currentRound)
        {
            this.lengthOfWord = lengthOfWord;
            this.currentRound = currentRound;
        }

        public int LengthOfWord { get { return this.lengthOfWord; } }

        public int CurrentRound { get { return this.currentRound; } }
    }
}

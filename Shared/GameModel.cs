using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class GameModel
    {
        private int lengthOfWord;

        public GameModel(int lengthOfWord)
        {
            this.lengthOfWord = lengthOfWord;
        }

        public int LengthOfWord { get { return this.lengthOfWord; } }
    }
}

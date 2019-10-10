using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class GuessModel
    {
        private string word;

        public GuessModel(string word)
        {
            this.word = word;
        }

        public string Word { get { return this.word; } }
    }
}

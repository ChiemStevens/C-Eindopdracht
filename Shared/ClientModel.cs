using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class ClientModel
    {
        private string name;

        public ClientModel(string name)
        {
            this.name = name;
        }

        public string Name { get { return this.name; } set { this.name = value; } }
    }
}

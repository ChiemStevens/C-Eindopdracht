using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class ClientModel
    {
        private string name;
        private bool validName;

        public ClientModel(string name, bool validName)
        {
            this.name = name;
            this.validName = validName;
        }

        public string Name { get { return this.name; } set { this.name = value; } }

        public bool ValidName { get { return this.validName; } set { this.validName = value; } }
    }
}

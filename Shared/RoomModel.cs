using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class RoomModel
    {
        private string name;

        public RoomModel(string name)
        {
            this.name = name;
        }

        public string Name { get { return name; } set { name = value; } }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    public class Skin
    {
        public string Name { get; set; }
        public Dictionary<string, string> Settings { get; set; } 
        public Skin(string name, Dictionary<string, string> settings)
        {
            Name = name;
            Settings = settings;
        }
    }
}

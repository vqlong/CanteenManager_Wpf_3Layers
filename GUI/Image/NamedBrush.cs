using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GUI
{
    public class NamedBrush
    {
        public string Name { get; }
        public Brush Brush { get; }
        public NamedBrush(string name, Brush brush)
        {
            Name = name;
            Brush = brush;
        }
        public override string ToString() => Name;
    }
}

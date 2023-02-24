using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaction logic for wLogin1.xaml
    /// </summary>
    public partial class wLogin1 : Window
    {
        public wLogin1()
        {
            InitializeComponent();
            
            var uri = new Uri("pack://application:,,,/PresentationFramework.Luna;component/themes/Luna.NormalColor.xaml");
            var dictionary = new ResourceDictionary { Source = uri };
            foreach (var key in dictionary.Keys)
            {
                //Lấy ra style cho scrollbar và thêm vào resource của window
                if (dictionary[key] is Style style && style.TargetType == typeof(ScrollBar))
                {
                    Resources.Add(key, style);
                    break;
                }
            }
        }

    }
}

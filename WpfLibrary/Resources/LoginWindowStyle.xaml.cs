using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfLibrary.Resources
{
    public partial class LoginWindowStyle : System.Windows.ResourceDictionary
    {
        public LoginWindowStyle()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.Close();
        }

        private void btnMax_Click(object sender, RoutedEventArgs e)
        {

            var window = (Window)((FrameworkElement)sender).TemplatedParent;

            //if (window.ResizeMode == ResizeMode.CanMinimize) return;

            if (window.WindowState == WindowState.Maximized)
            {
                window.WindowState = WindowState.Normal;
                //((Button)sender).Content = "🗖";
            }
            else if (window.WindowState == WindowState.Normal)
            {
                window.WindowState = WindowState.Maximized;
                //((Button)sender).Content = "🗗";
            }
        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.WindowState = WindowState.Minimized;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfLibrary.UserControls;

namespace GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static Mutex mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            mutex = new Mutex(true, "thisapplicationonlyrunswithonlyoneinstance", out bool createdNew);
            if(createdNew == false)
            {
                DialogBox.Show("Một phiên bản của chương trình vẫn đang chạy.", "Thông báo", DialogBoxButton.OK, DialogBoxIcon.Information);
                Shutdown();
            }

            //BUS.Config.RegisterEntity();
            BUS.Config.RegisterSQLite();

            //Chọn đường dẫn cho thư mục chứa file log
            log4net.GlobalContext.Properties["ApplicationPath"] = Environment.CurrentDirectory;
            //Hiện tên DataProvider trong message log
            log4net.GlobalContext.Properties["DataProvider"] = BUS.Config.DataProvider.Name;

            //Cấu hình đặt trong file App.config
            //[assembly: log4net.Config.XmlConfigurator(Watch=true)]
            //log4net.Config.XmlConfigurator.Configure();

            //Cấu hình đặt trong file log4net.config
            //[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

            base.OnStartup(e);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            DialogBox.Show(e.Exception.Message);
            e.Handled = true;
        }
    }
}

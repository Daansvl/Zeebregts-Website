using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MandagenRegistratie.controls;
using ZeebregtsLogic;
using MandagenRegistratie.controls.Navigatie;
using System.Net;

namespace MandagenRegistratie
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void iInitializeComponent()
        {
            //Program vmv = new Program();
            //vmv.Show();
            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] address = hostEntry.AddressList;

            string endpoint = "";
            for (int i = 0; i < address.Count(); i++)
            {

                if (address.GetValue(i).ToString().StartsWith("192.160"))
                {
                    endpoint = "net.p2p://" + address.GetValue(i).ToString() + ":8089/MyChatServer/Chat";
                    break;
                }

            }

            Logging log = new Logging();
            log.Log(endpoint, "Start applicatie");
            

            PageContainer vmv2 = new PageContainer(endpoint);


            // 


            MainMenu mm = new MainMenu();

            vmv2.LoadControlDefault(mm, true);

            vmv2.Show();


            Global.useWeekviewLeesstand = true;

            log.Log("Applicatie gestart");


            MandagenRegistratie.controls.Login.Edit edit = new controls.Login.Edit();
            vmv2.inloggenalscontrol = edit;

            if (Rechten.IsAdmin)
            {
                ApplicationState.SetValue(ApplicationVariables.blnIsAdmin, true);
            }

        }

        // give the mutex a  unique name

        private string MutexName = "##||" + ConfigurationManager.AppSettings["WindowTitle"].ToString() + "||##";

        // declare the mutex
        private readonly Mutex _mutex;
        // overload the constructor
        bool createdNew;
        public App()
        {
            // overloaded mutex constructor which outs a boolean
            // telling if the mutex is new or not.
            // see http://msdn.microsoft.com/en-us/library/System.Threading.Mutex.aspx
            _mutex = new Mutex(true, MutexName, out createdNew);
            if (!createdNew)
            {
                // if the mutex already exists, notify and quit
                MessageBox.Show("Het programma is al geopend. Dit proces wordt nu afgesloten.");
                Application.Current.Shutdown(0);
            }
        }

        [STAThread]
        public static void Main()
        {
            App app = new App();
            app.iInitializeComponent();
            //app.MainWindow.WindowState = WindowState.Maximized;
            app.MainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            app.MainWindow.Left = 0;
            app.MainWindow.Top = 0;
            app.MainWindow.Width = 700;
            app.MainWindow.Height = 300;
            app.Run();
        }
    }
}

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



            PageContainer vmv2 = new PageContainer(endpoint);


            MainMenu mm = new MainMenu();
            
            vmv2.LoadControl(mm,true);

            vmv2.Show();
        }

        [STAThread]
        public static void Main()
        {
            App app = new App();
            app.iInitializeComponent();
            app.Run();
        }
    }
}

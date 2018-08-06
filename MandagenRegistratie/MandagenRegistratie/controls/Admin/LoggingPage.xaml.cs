using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Admin
{
    /// <summary>
    /// Interaction logic for Comparison.xaml
    /// </summary>
    public partial class LoggingPage : MenuControl
    {
        public LoggingPage()
        {
            InitializeComponent();

            this.OkClick += Load;
            this.PageOKButtonText = "Refresh";
            this.PageTitle = "Logboek";

        }

        public void Load()
        {
            try
            {

                dbRepository dbrep = new dbRepository();
                dgLogboek.ItemsSource = dbrep.datacontext.Loggens.OrderByDescending(l => l.Datum).Take(500);
                dgLogboek.Width = 700;

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.StackTrace);
            }
        }


    }
}

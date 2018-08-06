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

namespace MandagenRegistratie
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            

            string strWindowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            string[] usernameArray = strWindowsIdentity.Split('\\');
            string strUsername = usernameArray[usernameArray.Length - 1];

            dbRepository dbRep = new dbRepository();
            List<Project> testing = dbRep.GetProjects();

        }

        public void MouseOverEvent(object sender, MouseEventArgs e)
        {


        }

        string strOriginalValue;

        private void Button_MouseMove_1(object sender, MouseEventArgs e)
        {
            strOriginalValue = txtResult.Text;
            txtResult.Text = "yesssssss";
        }

        private void Button_MouseLeave_1(object sender, MouseEventArgs e)
        {
            txtResult.Text = "testtt";
        }
    }
}

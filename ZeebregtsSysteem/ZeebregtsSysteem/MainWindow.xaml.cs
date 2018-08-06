using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace ZeebregtsSysteem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            zeebregtsCs.Menu m = new zeebregtsCs.Menu();
            m.Show();


            this.Hide();

        }


        private void btnMandagenRegistratie_Click(object sender, RoutedEventArgs e)
        {
            //PageContainer p = new PageContainer();
            //p.LoadControl();
            //p.Show();

            //this.Hide();

        }

        private void btnMainApplication_Click(object sender, RoutedEventArgs e)
        {
            //zeebregtsCs.Menu m = new zeebregtsCs.Menu();
            //m.Show();

            //this.Hide();

        }



        int intShiftCount = 0;

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                intShiftCount += 1;

            }

            if (intShiftCount == 3)
            {
                this.Show();

            }
        }

        private void btnSQL2Excel_Click(object sender, RoutedEventArgs e)
        {
            //Excel2SqlContainer.Form1 sql2excelscreen = new Excel2SqlContainer.Form1();

            //sql2excelscreen.Show();

        }


    }
}

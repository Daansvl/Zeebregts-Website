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
using MandagenRegistratie.controls;
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Vakmannen.Overzicht
{
    /// <summary>
    /// Interaction logic for ContactenOverview.xaml
    /// </summary>
    public partial class ContactenOverview : MenuControl
    {
        public ContactenOverview()
        {

            //TerugButtonText = "BACK2";
            GereedButtonText = "wijzigen2";

            InitializeComponent();

            //TerugButtonText = "BACK2";
            GereedButtonText = "wijzigen2";

            ZoekResultaten();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ZoekResultaten();
        }

        private void ZoekResultaten()
        {
            if (txtSearch != null)
            {
                dbOriginalRepository dbrep = new dbOriginalRepository();
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    dgVakmannen.ItemsSource = dbrep.GetContacten();
                }
                else
                {
                    dgVakmannen.ItemsSource = dgVakmannen.ItemsSource = dbrep.GetContacten(txtSearch.Text.Split(' '));
                }
            }
        }

    }
}

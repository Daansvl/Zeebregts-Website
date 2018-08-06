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
using MandagenRegistratieDomain;

namespace MandagenRegistratie.controls.Projecten.Overzicht
{
    /// <summary>
    /// Interaction logic for ProjectenOverview.xaml
    /// </summary>
    public partial class ProjectenOverview : MenuControl
    {
        public ProjectenOverview()
        {
            InitializeComponent();

            ZoekResultaten();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ZoekResultaten();
        }

        public void ZoekResultaten()
        {
            if (txtSearch != null)
            {
                dbOriginalRepository dbrep = new dbOriginalRepository();
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    dgVakmannen.ItemsSource = dbrep.GetProjects();
                }
                else
                {
                    dgVakmannen.ItemsSource = dgVakmannen.ItemsSource = dbrep.GetProjects(txtSearch.Text.Split(' '));
                }
            }
        }

    }
}

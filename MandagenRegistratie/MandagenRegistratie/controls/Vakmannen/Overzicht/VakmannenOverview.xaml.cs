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
using System.ComponentModel;
using MandagenRegistratie.controls.Vakmannen.Dagview;
using MandagenRegistratie.controls.Vakmannen.Detail;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Vakmannen.Overzicht
{

    /// <summary>
    /// Interaction logic for VakmannenOverview.xaml
    /// </summary>
    public partial class VakmannenOverview : MenuControl
    {

        public VakmannenOverview()
        {

            InitializeComponent();

            #region Pagina specifieke informatie
            PageTitle = "Vakmannen overzicht";
            PageSubtitle = "Zoek op deze pagina naar vakmannen. Klik op 'Toon alle contacten' om ook in het originele systeem te zoeken";
            GereedButtonVisibility = System.Windows.Visibility.Collapsed;
            #endregion

            this.Gereed += VakmannenOverview_Gereed;

            ZoekResultaten();

        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here

            //this.Dispatcher.Invoke((Action)(() =>
            //{
            //    ZoekResultaten();
            //}));

        }

        private void worker_RunWorkerCompleted(object sender,
                                               RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
        }

        void VakmannenOverview_Gereed()
        {
            System.Windows.MessageBox.Show("nieuwe vakman toevoegen");
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (cbAlleContacten.IsChecked == true)
            {
                ZoekResultatenOrigineel();
            }
            else
            {
                ZoekResultaten();
            }

        }

        private void ZoekResultaten()
        {
            if (txtSearch != null)
            {
                dbRepository dbrep = new dbRepository();
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    if ((bool)cbMijnProjecten.IsChecked)
                    {
                        dgVakmannen.ItemsSource = dbrep.GetVakmannenAll().Where(p => p.ProjectleiderId == ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider)).ToList();
                    }
                    else
                    {
                        dgVakmannen.ItemsSource = dbrep.GetVakmannenAll();
                    }
                }
                else
                {
                    if ((bool)cbMijnProjecten.IsChecked)
                    {
                        dgVakmannen.ItemsSource = dgVakmannen.ItemsSource = dbrep.GetVakmannen(txtSearch.Text.Split(' ')).Where(p => p.ProjectleiderId == ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider)).ToList();
                    }
                    else
                    {
                        dgVakmannen.ItemsSource = dgVakmannen.ItemsSource = dbrep.GetVakmannen(txtSearch.Text.Split(' '));
                    }
                }

                // set de focus op de zoek textbox
                txtSearch.Focus();

            }


        }

        private void ZoekResultatenOrigineel()
        {
            if (txtSearch != null)
            {
                dbOriginalRepository dbrep = new dbOriginalRepository();
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    dgContacten.ItemsSource = dbrep.GetContacten();
                }
                else
                {
                    dgContacten.ItemsSource = dgVakmannen.ItemsSource = dbrep.GetContacten(txtSearch.Text.Split(' '));
                }
            }
        }

        private void cbMijnProjecten_Checked(object sender, RoutedEventArgs e)
        {

            if (cbMijnProjecten.IsChecked == true)
            {
                cbAlleContacten.IsChecked = false;
                dgContacten.Visibility = System.Windows.Visibility.Collapsed;

                dgVakmannen.Visibility = System.Windows.Visibility.Visible;
            }

            ZoekResultaten();
        }

        private void cbAlleContacten_Checked(object sender, RoutedEventArgs e)
        {
            if (cbAlleContacten.IsChecked == true)
            {
                cbMijnProjecten.IsChecked = false;
                dgVakmannen.Visibility = System.Windows.Visibility.Collapsed;

                dgContacten.Visibility = System.Windows.Visibility.Visible;
                ZoekResultatenOrigineel();
            }
            else
            {
                dgVakmannen.Visibility = System.Windows.Visibility.Visible;
                dgContacten.Visibility = System.Windows.Visibility.Collapsed;
                ZoekResultaten();
            }

        }

        private void dgVakmannen_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Vakman vm = (Vakman)dgVakmannen.SelectedItem;
            ApplicationState.SetValue("intVakmanId", vm.VakmanId);
            ApplicationState.SetValue("dtSelectedDay", DateTime.Now);

            // create the page and load all values
            VakmanDetailView vdv = new VakmanDetailView();

            // load the page into the contentcontrol
            GoToPage(vdv);

        }
    }
}

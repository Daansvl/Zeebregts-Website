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
using System.ComponentModel;
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Projecten.Overzicht
{
    /// <summary>
    /// Interaction logic for Projecten.xaml
    /// </summary>
    public partial class Projecten : MenuControl
    {
        public Projecten()
        {
            InitializeComponent();

            #region Pagina specifieke informatie
            PageTitle = "Projecten overzicht";
            PageSubtitle = "Zoek op deze pagina naar projecten. Klik op 'Toon alle projecten' om ook in het originele systeem te zoeken";
            GereedButtonVisibility = System.Windows.Visibility.Collapsed;
            #endregion

            // default settings
            cbMijnProjecten.IsChecked = true;
            
            ZoekResultaten();

            dgProjecten.SelectionMode = DataGridSelectionMode.Single;

        }

        void VakmannenOverview_Gereed()
        {
            System.Windows.MessageBox.Show("nieuwe vakman toevoegen");
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cbAlleProjecten.IsChecked == true)
            {
                ZoekResultatenOrigineel();
            }
            else
            {
                ZoekResultaten();
            }
        }

        /// <summary>
        /// Zoek resultaten in de mandagenregistratie projecten
        /// </summary>
        public void ZoekResultaten()
        {
            if (txtSearch != null)
            {
                dbRepository dbrep = new dbRepository();
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    if ((bool)cbMijnProjecten.IsChecked)
                    {
                        dgProjecten.ItemsSource = dbrep.GetProjects().Where(p => p.ProjectleiderId == ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider)).ToList();
                    }
                    else
                    {
                        dgProjecten.ItemsSource = dbrep.GetProjects();
                    }
                }
                else
                {
                    if ((bool)cbMijnProjecten.IsChecked)
                    {
                        dgProjecten.ItemsSource = dbrep.GetProjects(txtSearch.Text.Split(' ')).Where(p => p.ProjectleiderId == ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider)).ToList();
                    }
                    else
                    {
                        dgProjecten.ItemsSource = dbrep.GetProjects(txtSearch.Text.Split(' '));
                    }
                }
            }
        }

        public void ZoekResultatenOrigineel()
        {
            if (txtSearch != null)
            {
                dbOriginalRepository dbrep = new dbOriginalRepository();
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    dgProjectenOrigineel.ItemsSource = dbrep.GetProjects();
                }
                else
                {
                    dgProjectenOrigineel.ItemsSource = dbrep.GetProjects(txtSearch.Text.Split(' '));
                }
            }
        }


        private void cbMijnProjecten_Checked(object sender, RoutedEventArgs e)
        {
            if (cbMijnProjecten.IsChecked == true)
            {
                cbAlleProjecten.IsChecked = false;
                dgProjectenOrigineel.Visibility = System.Windows.Visibility.Collapsed;

                dgProjecten.Visibility = System.Windows.Visibility.Visible;
            }

            ZoekResultaten();
        }

        private void cbAlleProjecten_Checked(object sender, RoutedEventArgs e)
        {
            if (cbAlleProjecten.IsChecked == true)
            {
                cbMijnProjecten.IsChecked = false;
                dgProjecten.Visibility = System.Windows.Visibility.Collapsed;
                dgProjectenOrigineel.Visibility = System.Windows.Visibility.Visible;

                ZoekResultatenOrigineel();
            }
            else
            {
                dgProjecten.Visibility = System.Windows.Visibility.Visible;
                dgProjectenOrigineel.Visibility = System.Windows.Visibility.Collapsed;
                ZoekResultaten();
            }
        }

        private void dgProjectenOrigineel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MandagenRegistratieDomain.dbo_project project = ((MandagenRegistratieDomain.dbo_project)dgProjectenOrigineel.SelectedItem);
            MessageBox.Show(project.naam_project);
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {


                if (cbAlleProjecten.IsChecked == true)
                {
                    if (Keyboard.FocusedElement.GetType() != typeof(DataGridCell))
                    {
                        dgProjectenOrigineel.Focus();
                        dgProjectenOrigineel.SelectedIndex = 0;
                        //dgVakmannenOrigineel.CurrentCell = (DataGridCellInfo)dgVakmannenOrigineel.SelectedItem;
                    }
                }
                else
                {
                    //if (((System.Type)Keyboard.FocusedElement) != dgVakmannen.SelectedCells[0].GetType())
                    if (Keyboard.FocusedElement.GetType() != typeof(DataGridCell))
                    {
                        dgProjecten.Focus();
                        dgProjecten.SelectedIndex = 0;
                        dgProjecten.CurrentCell = dgProjecten.SelectedCells[0];
                    }

                }
            }
            else if (e.Key == Key.Up && dgProjecten.CurrentCell == dgProjecten.SelectedCells[0])
            {
                txtSearch.Focus();

            }

        }

    }
}

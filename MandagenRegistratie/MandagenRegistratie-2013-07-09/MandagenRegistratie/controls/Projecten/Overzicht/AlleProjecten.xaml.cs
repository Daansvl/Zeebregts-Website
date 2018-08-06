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
using MandagenRegistratie.controls.Vakmannen.Dagview;

namespace MandagenRegistratie.controls.Projecten.Overzicht
{
    /// <summary>
    /// Interaction logic for AlleProjecten.xaml
    /// </summary>
    public partial class AlleProjecten : MenuControl
    {

        public FunctieOK OkFunctie;
        public List<vwProject> listVwProjecten = new List<vwProject>();

        public AlleProjecten()
        {
            InitializeComponent();

            #region Pagina specifieke informatie
            PageTitle = "Project toevoegen";
            PageSubtitle = "Aan mijn projectlijst";
            PageGereedButtonVisibility = System.Windows.Visibility.Hidden;
            //PageOKButtonText = "TOEVOEGEN";
            #endregion

            //this.Reloaded += ZoekResultaten;
            this.OkClick += ToevoegenAanProjecten;
            this.Reloaded += ZoekResultatenOrigineel;

            // default settings
            txtSearch.Focus();
            
            //ZoekResultatenOrigineel();

            Loaded += AlleProjecten_Loaded;
        }

        public void AlleProjecten_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(txtSearch);
            txtSearch.Focus();
        }


        void VakmannenOverview_Gereed()
        {
            System.Windows.MessageBox.Show("nieuwe vakman toevoegen");
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ZoekResultatenOrigineel();
        }

        public void ZoekResultatenOrigineel()
        {
            if (txtSearch != null)
            {
                dbRepository dbrep = new dbRepository();
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    dgProjectenOrigineel.ItemsSource = dbrep.GetVwProjectsAll().Where(p => !listVwProjecten.Any(v => v.ProjectIdOrigineel == p.project_ID));
                }
                else
                {
                    dgProjectenOrigineel.ItemsSource = dbrep.GetVwProjectsAll(txtSearch.Text.Split(' ')).Where(p => !listVwProjecten.Any(v => v.ProjectIdOrigineel == p.project_ID));
                }
            }
        }


        private void dgProjectenOrigineel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ToevoegenAanProjecten();
        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (Keyboard.FocusedElement.GetType() != typeof(DataGridCell))
                {
                    dgProjectenOrigineel.Focus();
                    dgProjectenOrigineel.SelectedIndex = 0;
                    dgProjectenOrigineel.CurrentCell = dgProjectenOrigineel.SelectedCells[0];
                    dgProjectenOrigineel.SelectedItem = dgProjectenOrigineel.Items[0];
                }

            }
            // == dgProjecten.SelectedCells[0]
            else if (e.Key == Key.Up && dgProjectenOrigineel.SelectedIndex == 0)
            {
                txtSearch.Focus();
            }
            else if (e.Key == Key.Enter)
            {
                dgProjectenOrigineel_MouseDoubleClick(dgProjectenOrigineel, null);
            }


        }

        public void ToevoegenAanProjecten()
        {

            dbRepository dbrep = new dbRepository();
            dbOriginalRepository dbOriginalRep = new dbOriginalRepository();

            MandagenRegistratieDomain.vwProjectAll dboproject = ((MandagenRegistratieDomain.vwProjectAll)dgProjectenOrigineel.SelectedItem);
            int intProject = -1;

            if (dboproject != null)
            {
                // check of het project al bestaat in MDR
                MandagenRegistratieDomain.Project project = dbrep.GetProjectByProjectIdOrigineel(dboproject.project_ID);

                if (project == null)
                {
                    // bestond nog niet, dus toevoegen
                    MandagenRegistratieDomain.Project projectNew = new MandagenRegistratieDomain.Project();

                    // overige info van het project invullen
                    projectNew.Actief = true;
                    projectNew.ProjectIdOrigineel = dboproject.project_ID;
                    projectNew.Naam = dboproject.naam_project;

                    // mutatiedatum = now
                    projectNew.Mutatiedatum = DateTime.Now;

                    projectNew.Postcode = "";
                    projectNew.Huisnummer = "";
                    projectNew.Adres = "";

                    // projectleiderID is de huidig ingelogde
                    projectNew.ProjectleiderId = ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider);

                    // voeg toe aan database
                    intProject = dbrep.InsertProject(projectNew);
                }

                // bewaar info in 'sessie'
                ApplicationState.SetValue(ApplicationVariables.intProjectId, intProject);

                DateTime nu = DateTime.Now;
                //ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, new DateTime(nu.Year, nu.Month, nu.Day, 0, 0, 0));

                PageGoBack();
            }
            else
            {

                //MessageBox.Show("Selecteer aub een project, of klik op annulleer");
            }

        }


    }
}

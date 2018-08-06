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

namespace MandagenRegistratie.controls.Projecten.Lijst
{
    /// <summary>
    /// Interaction logic for AlleProjecten.xaml
    /// </summary>
    public partial class AlleProjecten : MenuControl
    {

        public FunctieOK OkFunctie;
        public List<vwProject> listVwProjecten = new List<vwProject>();
        public bool SubRoute;

        public AlleProjecten(bool blnSubRoute)
        {
            InitializeComponent();

            #region Pagina specifieke informatie
            PageTitle = "Nieuw project";
            PageSubtitle = "Voeg project toe aan projectlijst en toon projectgegevens";
            PageBackButtonText = "ANNULEER";
            PageOKButtonText = "TOEVOEGEN";

            //if (Rechten.IsProjectleider)
            //{
            //    PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    PageGereedButtonVisibility = System.Windows.Visibility.Hidden;
            //}

            PageGereedButtonVisibility = System.Windows.Visibility.Hidden;

            //PageOKButtonText = "TOEVOEGEN";
            #endregion

            //this.Reloaded += ZoekResultaten;
            this.OkClick += ToevoegenAanProjecten;
            this.Reloaded += ZoekResultatenOrigineel;
            
            CustomActieClick += ToevoegenAanProjecten;

            // default settings
            txtSearch.Focus();
            
            //ZoekResultatenOrigineel();
            SubRoute = blnSubRoute;

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
            Mouse.OverrideCursor = Cursors.Wait;

            if (txtSearch != null)
            {
                dbRepository dbrep = new dbRepository();
                List<vwProjectAll> listProjecten = new List<vwProjectAll>();

                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    listProjecten = dbrep.GetVwProjectsAllRemainder().Where(p => !listVwProjecten.Any(v => v.ProjectNrOrigineel == p.ProjectNrOrigineel)).OrderBy(pv => pv.Bedrijfsnaam).ThenBy(pv => pv.naam_project).ToList();

                    dgProjectenOrigineel.ItemsSource = listProjecten;
                }
                else
                {
                    listProjecten = dbrep.GetVwProjectsAll(txtSearch.Text.Split(' ')).Where(p => !listVwProjecten.Any(v => v.ProjectNrOrigineel == p.ProjectNrOrigineel)).OrderBy(pv => pv.Bedrijfsnaam).ThenBy(pv => pv.naam_project).ToList();

                    dgProjectenOrigineel.ItemsSource = listProjecten;

                    // probeer de eerste te selecteren
                    vwProjectAll firstItem = listProjecten.FirstOrDefault();
                    if (firstItem != null)
                    {
                        SelectItem((int)firstItem.ProjectNrOrigineel);
                    }

                }
            }

            Keyboard.Focus(txtSearch);

            Mouse.OverrideCursor = null;

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
            try
            {
                if (dgProjectenOrigineel.SelectedItem != null)
                {
                    dbRepository dbrep = new dbRepository();
                    dbOriginalRepository dbOriginalRep = new dbOriginalRepository();

                    // blijkbaar bestaat er de mogelijkheid dat er zojuist een project is toegevoegd aan de ZeebregtsDb
                    // vandaar voor de zekerheid opnieuw de ZeebregtsDb Cachen

                    ApplicationState.SetValue(ApplicationVariables.listMDRPersoons, dbOriginalRep.datacontext.MDRpersoons.ToList());
                    ApplicationState.SetValue(ApplicationVariables.listMDRProjecten, dbOriginalRep.datacontext.MDRprojects.ToList());





                    MandagenRegistratieDomain.vwProjectAll dboproject = ((MandagenRegistratieDomain.vwProjectAll)dgProjectenOrigineel.SelectedItem);
                    int intProject = -1;

                    if (dboproject != null)
                    {
                        // check of het project al bestaat in MDR
                        MandagenRegistratieDomain.Project project = dbrep.GetProjectByProjectNrOrigineel((int)dboproject.ProjectNrOrigineel);

                        if (project == null)
                        {
                            // bestond nog niet, dus toevoegen
                            MandagenRegistratieDomain.Project projectNew = new MandagenRegistratieDomain.Project();

                            // overige info van het project invullen
                            // weggehaald
                            // projectNew.ProjectIdOrigineel = dboproject.project_ID;
                            // vervangen door
                            projectNew.ProjectNr = dboproject.ProjectNrOrigineel;

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

                        // WEGGEHAALD VOOR TABBLADENVIEW
                        if (SubRoute)
                        {
                            PageGoBack();

                        }
                        else
                        {
                            MDRproject projectorigineel = dbOriginalRep.GetProject((int)dboproject.ProjectNrOrigineel);
                            Project pv = new Project(projectorigineel);
                            pv.SelectTab(1);

                            // load the page into the contentcontrol
                            PageGoBack(pv);

                        }


                        //Intro intro = Tools.FindVisualParent<Intro>(this);
                        //intro.OkClick -= ToevoegenAanProjecten;
                        //// intro.CustomActieClick
                        //intro.LoadLijst();
                    }
                    else
                    {

                        //MessageBox.Show("Selecteer aub een project, of klik op annulleer");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Er is een onbekende fout opgetreden, error #801");
            }


        }

        private void txtSearch_PreviewKeyUp_1(object sender, KeyEventArgs e)
        {
            ZoekResultatenOrigineel();
        }

        private void SelectItem(int projectNr)
        {

            int count = 0;

            foreach (vwProjectAll projectItem in dgProjectenOrigineel.Items)
            {
                if (projectNr == projectItem.ProjectNrOrigineel)
                {
                    dgProjectenOrigineel.Focus();
                    //dgProjecten.SelectedIndex = 0;
                    //dgProjecten.CurrentCell = dgProjecten.SelectedCells[0];
                    //dgProjecten.SelectedItem = dgProjecten.Items[0];


                    dgProjectenOrigineel.SelectedIndex = count;
                    dgProjectenOrigineel.CurrentCell = dgProjectenOrigineel.SelectedCells[0];
                    dgProjectenOrigineel.SelectedItem = projectItem;

                    break;
                }
                count++;
            }

        }

        private void MenuControl_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            CheckLayout();
        }

        public void CheckLayout()
        {
            if (Window.GetWindow(this) != null)
            {
                this.Height = Window.GetWindow(this).ActualHeight - 164;
            }
        }



    }
}

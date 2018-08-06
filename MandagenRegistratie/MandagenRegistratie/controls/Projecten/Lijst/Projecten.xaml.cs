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
    /// Interaction logic for Projecten.xaml
    /// </summary>
    public partial class Projecten : MenuControl
    {


        public FunctieOK OkFunctie;
        public FunctieCustomActie CustomActieFunctie;

        public List<MandagenRegistratieDomain.Project> listProjecten = new List<MandagenRegistratieDomain.Project>();
        public List<MandagenRegistratieDomain.vwProject> listVwProjecten = new List<MandagenRegistratieDomain.vwProject>();
        public bool filterProjectleiderId = false;

        public bool blnRememberSelectedDay = false;
        public bool SubRoute;

        public Projecten(bool blnSubRoute)
        {
            InitializeComponent();

            Loaded += Projecten_Loaded;

            #region Pagina specifieke informatie
            PageTitle = "Projectlijst"; // pageTitle;
            PageSubtitle = "Toon projecturen op maandag week " + Tools.GetWeekNumber(DateTime.Now).ToString();

            if (Rechten.IsProjectleider)
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Hidden;
            }

            if (blnSubRoute)
            {
                PageBackButtonText = "ANNULEER";
            }
            else
            {
                PageBackButtonText = "TERUG";
            }

            PageOKButtonText = "NIEUW PROJECT";

            OkClick += VakmannenOverview_Gereed;
            CustomActieClick += Selectproject;
            Reloaded += LoadView;
            #endregion

            SubRoute = blnSubRoute;


        }

        private void dgProjecten_LostFocus(object sender, RoutedEventArgs e)
        {
            //((SolidColorBrush)dgProjecten.Resources["SelectionColorKey"]).Color = Colors.DarkBlue;
        }

        private void dgProjecten_GotFocus(object sender, RoutedEventArgs e)
        {
            //((SolidColorBrush)dgProjecten.Resources["SelectionColorKey"]).Color = Colors.DarkBlue;
        }

        private void dgProjecten_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //((SolidColorBrush)dgProjecten.Resources["SelectionColorKey"]).Color = Colors.DarkBlue;


            //foreach (DataGridRow dr in dgProjecten.RowStyle .SelectedItems
        }

        public void Projecten_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationState.GetValue<int>(ApplicationVariables.intProjectId) > 0)
            {
                SelectItem(ApplicationState.GetValue<int>(ApplicationVariables.intProjectId));

            }

            Keyboard.Focus(txtSearch);
            txtSearch.Focus();


        }

        public void LoadView()
        {


            dgProjecten.SelectionMode = DataGridSelectionMode.Single;



            // default settings
            // txtSearch.Focusable = true;
            // txtSearch.Focus();
            // FocusManager.SetFocusedElement(dpProjecten, txtSearch);

            // Keyboard.Focus(txtSearch);

            ZoekResultaten();


        }

        public void VakmannenOverview_Gereed()
        {
            // TODO: Checken --> HIER KOM JE NIET MEER IN

            AlleProjecten ap = new AlleProjecten(SubRoute);
            ap.listVwProjecten = listVwProjecten;
            ap.PageReloaded();

            // WEGGEHAALD VOOR TABBLADENVIEW
            PageGoToPage(ap);

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (cbAlleProjecten.IsChecked == true)
            //{
            //    ZoekResultatenOrigineel();
            //}
            //else
            //{

            txtSearch.Focus();
            ZoekResultaten();

            //}
        }



        /// <summary>
        /// Zoek resultaten in de mandagenregistratie projecten
        /// </summary>
        public void ZoekResultaten()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            // zet de geselecteerde dag op de eerste dag van de week (van de datum die momenteel geselecteerd is/was)
            if (!blnRememberSelectedDay)
            {
                ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, Tools.CalculateWeekstart(DateTime.Now));
            }
            // ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, Tools.CalculateWeekstart(ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay)));

            if (txtSearch != null)
            {
                dbRepository dbrep = new dbRepository();
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    //if ((bool)cbMijnProjecten.IsChecked)
                    //{
                    //    dgProjecten.ItemsSource = dbrep.GetProjects().Where(p => p.ProjectleiderId == ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider)).ToList();
                    //}
                    //else
                    //{
                    if (filterProjectleiderId)
                    {
                        listVwProjecten = dbrep.GetViewProjects().Where(v => !listProjecten.Any(lp => v.ProjectId == lp.ProjectId) && v.ProjectleiderId == ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider)).OrderBy(pv => pv.Bedrijfsnaam).ThenBy(pv => pv.ZProjectNaam).ToList();
                    }
                    else
                    {
                        listVwProjecten = dbrep.GetViewProjects().Where(v => !listProjecten.Any(lp => v.ProjectId == lp.ProjectId)).OrderBy(pv => pv.Bedrijfsnaam).ThenBy(pv => pv.ZProjectNaam).ToList();
                    }

                    dgProjecten.ItemsSource = listVwProjecten;
                    //}
                }
                else
                {
                    //if ((bool)cbMijnProjecten.IsChecked)
                    //{
                    //    dgProjecten.ItemsSource = dbrep.GetProjects(txtSearch.Text.Split(' ')).Where(p => p.ProjectleiderId == ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider)).ToList();
                    //}
                    //else
                    //{
                    if (filterProjectleiderId)
                    {
                        listVwProjecten = dbrep.GetViewProjects(txtSearch.Text.Split(' ')).Where(v => !listProjecten.Any(lp => v.ProjectId == lp.ProjectId) && v.ProjectleiderId == ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider)).OrderBy(pv => pv.Bedrijfsnaam).ThenBy(pv => pv.ZProjectNaam).ToList();
                    }
                    else
                    {
                        listVwProjecten = dbrep.GetViewProjects(txtSearch.Text.Split(' ')).Where(v => !listProjecten.Any(lp => v.ProjectId == lp.ProjectId)).OrderBy(pv => pv.Bedrijfsnaam).ThenBy(pv => pv.ZProjectNaam).ToList();
                    }
                    
                    dgProjecten.ItemsSource = listVwProjecten;

                    // probeer de eerste te selecteren
                    vwProject firstItem = listVwProjecten.FirstOrDefault();
                    if(firstItem != null)
                    {
                        SelectItem(firstItem.ProjectId);
                    }

                    //}
                }
            }

            Keyboard.Focus(txtSearch);

            Mouse.OverrideCursor = null;
        }


        private void SelectItem(int projectId)
        {

            int count = 0;

            foreach (vwProject vwproject in dgProjecten.Items)
            {
                if (projectId == vwproject.ProjectId)
                {
                    dgProjecten.Focus();
                    //dgProjecten.SelectedIndex = 0;
                    //dgProjecten.CurrentCell = dgProjecten.SelectedCells[0];
                    //dgProjecten.SelectedItem = dgProjecten.Items[0];


                    dgProjecten.SelectedIndex = count;
                    dgProjecten.CurrentCell = dgProjecten.SelectedCells[0];
                    dgProjecten.SelectedItem = vwproject;

                    break;
                }
                count++;
            }

        }

        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {

                if (Keyboard.FocusedElement.GetType() != typeof(DataGridCell))
                {
                    dgProjecten.Focus();
                    dgProjecten.SelectedIndex = 0;
                    dgProjecten.CurrentCell = dgProjecten.SelectedCells[0];
                    dgProjecten.SelectedItem = dgProjecten.Items[0];
                    //dgProjecten.UpdateLayout();

                }

            }
            // == dgProjecten.SelectedCells[0]
            else if (e.Key == Key.Up && dgProjecten.SelectedIndex == 0)
            {
                txtSearch.Focus();
            }
            else if (e.Key == Key.Enter)
            {
                PageCustomActie();
                //CustomActieFunctie();
            }

        }

        public void dgProjecten_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PageCustomActie();
            //Selectproject();

        }

        public void Selectproject()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {

                ApplicationState.SetValue(ApplicationVariables.intVakmanViewModus, 1);

                dbRepository dbrep = new dbRepository();
                dbOriginalRepository dbOriginalRep = new dbOriginalRepository();

                // haal het geselecteerde project op
                MandagenRegistratieDomain.Project project = dbrep.GetProject(((MandagenRegistratieDomain.vwProject)dgProjecten.SelectedItem).ProjectId);


                // bewaar info in 'sessie'
                ApplicationState.SetValue(ApplicationVariables.intProjectId, project.ProjectId);

                DateTime nu = DateTime.Now;
                //ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, new DateTime(nu.Year, nu.Month, nu.Day, 0, 0, 0));
                MDRpersoon projectleider = dbOriginalRep.GetContact(dbrep.GetProjectleider(project.ProjectleiderId).ContactIdOrigineel);
                MDRproject projectorigineel = dbOriginalRep.GetProject((int)project.ProjectNr);

                // create the page and load all values

                string strProjectleiderNaam = string.Empty;
                if (projectleider != null)
                {
                    strProjectleiderNaam = (projectleider.voornaam + " " + projectleider.tussenvoegsel + " " + projectleider.achternaam).ToStringTrimmed();
                }
                else
                {
                    strProjectleiderNaam = "";
                }


                Project pv = new Project(projectorigineel);

                // load the page into the contentcontrol
                PageGoToPage(pv);

            }
            catch(Exception ex)
            {
                if (dgProjecten.Items.Count == 0)
                {
                    MessageBox.Show("Je zoekopdracht geeft geen resultaat, probeer een andere zoekterm.");
                }
                else if (dgProjecten.Items.Count == 1)
                {
                    dgProjecten.SelectedIndex = 0;

                    // gevaarlijk, kan oneindige loop in werking zetten!
                    Selectproject();

                }
                else
                {
                    MessageBox.Show("Selecteer een project in de lijst en druk daarna op enter.");
                }

            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

        }

        public void dgProjecten_MouseDoubleClickForVakman(object sender, MouseButtonEventArgs e)
        {
            ToevoegenAanVakman();
        }

        public void ToevoegenAanVakman()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            Logging log = new Logging();
            //log.Log("ToevoegenAanVakman()");

            try
            {
                dbRepository dbrep = new dbRepository();
                // haal het geselecteerde project op
                MandagenRegistratieDomain.Project project = dbrep.GetProject(((MandagenRegistratieDomain.vwProject)dgProjecten.SelectedItem).ProjectId);

                if (project != null)
                {
                    VakmanDagView vdv = new VakmanDagView();
                    vdv.ProjectToevoegen(project, ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId), ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider));

                    // zorgen dat de pagina's op andere schermen verversen

                    List<int> vakmanIds = new List<int>();
                    vakmanIds.Add(ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId));

                    PageChannelMessage("vakmandagview", Tools.CreateChannelMessage(vakmanIds, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay), ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay).AddDays(1)));

                    PageGoBack();
                }
                else
                {

                    //MessageBox.Show("Selecteer aub een project, of klik op annulleer");
                }

            }
            catch
            {
                MessageBox.Show("Er is een onbekende fout opgetreden, error #901");
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

            //log.Log("EINDE ToevoegenAanVakman()");


        }

        private void txtSearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            ZoekResultaten();
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

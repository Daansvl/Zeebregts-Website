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
    /// Interaction logic for Projecten.xaml
    /// </summary>
    public partial class Projecten : MenuControl
    {


        public FunctieOK OkFunctie;
        public FunctieCustomActie CustomActieFunctie;

        public List<MandagenRegistratieDomain.Project> listProjecten = new List<MandagenRegistratieDomain.Project>();
        public List<MandagenRegistratieDomain.vwProject> listVwProjecten = new List<MandagenRegistratieDomain.vwProject>();
        public bool filterProjectleiderId = false;

        public Projecten()
        {
            InitializeComponent();

            Loaded += Projecten_Loaded;

        }

        public void Projecten_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(txtSearch);
            txtSearch.Focus();

            if (ApplicationState.GetValue<int>(ApplicationVariables.intProjectId) > 0)
            {
                SelectItem(ApplicationState.GetValue<int>(ApplicationVariables.intProjectId));

            }

        }

        public void LoadView()
        {

            #region Pagina specifieke informatie
            PageTitle = "Projecten overzicht";
            PageSubtitle = "";
            PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            #endregion

            this.Reloaded += ZoekResultaten;
            this.OkClick += VakmannenOverview_Gereed;
            this.CustomActieClick += Selectproject;

            PageOKButtonText = "Nieuw Project";
            PageSubtitle = "";

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
            AlleProjecten ap = new AlleProjecten();
            //ap.PageReloaded();
            ap.listVwProjecten = listVwProjecten;
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
                        listVwProjecten = dbrep.GetViewProjects().Where(v => !listProjecten.Any(lp => v.ProjectId == lp.ProjectId) && v.ProjectleiderId == ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider)).ToList();
                    }
                    else
                    {
                        listVwProjecten = dbrep.GetViewProjects().Where(v => !listProjecten.Any(lp => v.ProjectId == lp.ProjectId)).ToList();
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
                        listVwProjecten = dbrep.GetViewProjects(txtSearch.Text.Split(' ')).Where(v => !listProjecten.Any(lp => v.ProjectId == lp.ProjectId) && v.ProjectleiderId == ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider)).ToList();
                    }
                    else
                    {
                        listVwProjecten = dbrep.GetViewProjects(txtSearch.Text.Split(' ')).Where(v => !listProjecten.Any(lp => v.ProjectId == lp.ProjectId)).ToList();
                    }
                    
                    dgProjecten.ItemsSource = listVwProjecten;
                    //}
                }
            }

            Mouse.OverrideCursor = null;
        }

        //public void ZoekResultatenOrigineel()
        //{
        //    if (txtSearch != null)
        //    {
        //        dbOriginalRepository dbrep = new dbOriginalRepository();
        //        if (string.IsNullOrEmpty(txtSearch.Text))
        //        {
        //            dgProjectenOrigineel.ItemsSource = dbrep.GetProjects();
        //        }
        //        else
        //        {
        //            dgProjectenOrigineel.ItemsSource = dbrep.GetProjects(txtSearch.Text.Split(' '));
        //        }
        //    }
        //}


        //private void cbMijnProjecten_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (cbMijnProjecten.IsChecked == true)
        //    {
        //        cbAlleProjecten.IsChecked = false;
        //        dgProjectenOrigineel.Visibility = System.Windows.Visibility.Collapsed;

        //        dgProjecten.Visibility = System.Windows.Visibility.Visible;
        //    }

        //    ZoekResultaten();
        //}

        //private void cbAlleProjecten_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (cbAlleProjecten.IsChecked == true)
        //    {
        //        cbMijnProjecten.IsChecked = false;
        //        dgProjecten.Visibility = System.Windows.Visibility.Collapsed;
        //        dgProjectenOrigineel.Visibility = System.Windows.Visibility.Visible;

        //        ZoekResultatenOrigineel();
        //    }
        //    else
        //    {
        //        dgProjecten.Visibility = System.Windows.Visibility.Visible;
        //        dgProjectenOrigineel.Visibility = System.Windows.Visibility.Collapsed;
        //        ZoekResultaten();
        //    }
        //}

        //private void dgProjectenOrigineel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{

        //    bool IsNewProject = false;

        //    dbRepository dbrep = new dbRepository();
        //    dbOriginalRepository dbOriginalRep = new dbOriginalRepository();

        //    MandagenRegistratieDomain.dbo_project dboproject = ((MandagenRegistratieDomain.dbo_project)dgProjectenOrigineel.SelectedItem);

        //    // check of het project al bestaat in MDR
        //    MandagenRegistratieDomain.Project project = dbrep.GetProjectByProjectIdOrigineel(dboproject.project_ID);

        //    if (project == null)
        //    {
        //        // bestond nog niet, dus toevoegen
        //        IsNewProject = true;

        //        MandagenRegistratieDomain.Project projectNew = new MandagenRegistratieDomain.Project();

        //        // overige info van het project invullen
        //        projectNew.Actief = true;
        //        projectNew.ProjectIdOrigineel = dboproject.project_ID;
        //        projectNew.Naam = dboproject.naam_project;

        //        // mutatiedatum = now
        //        projectNew.Mutatiedatum = DateTime.Now;

        //        projectNew.Postcode = "";
        //        projectNew.Huisnummer = "";
        //        projectNew.Adres = "";

        //        // projectleiderID is de huidig ingelogde
        //        projectNew.ProjectleiderId = ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider);

        //        // voeg toe aan database
        //        project = dbrep.InsertProject(projectNew);
        //    }

        //    // bewaar info in 'sessie'
        //    ApplicationState.SetValue(ApplicationVariables.intProjectId, project.ProjectId);

        //    DateTime nu = DateTime.Now;
        //    ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, new DateTime(nu.Year,nu.Month,nu.Day,0,0,0));

        //    // create the page and load all values
        //    Project pv = new Project();

        //    // load the page into the contentcontrol
        //    PageGoToPage(pv);

        //    if (IsNewProject)
        //    {
        //        pv.Okay();
        //    }





        //}

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

            dbRepository dbrep = new dbRepository();
            dbOriginalRepository dbOriginalRep = new dbOriginalRepository();

            // haal het geselecteerde project op
            MandagenRegistratieDomain.Project project = dbrep.GetProject(((MandagenRegistratieDomain.vwProject)dgProjecten.SelectedItem).ProjectId);


            // bewaar info in 'sessie'
            ApplicationState.SetValue(ApplicationVariables.intProjectId, project.ProjectId);

            DateTime nu = DateTime.Now;
            //ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, new DateTime(nu.Year, nu.Month, nu.Day, 0, 0, 0));
            persoon projectleider = dbOriginalRep.GetContact(dbrep.GetProjectleider(project.ProjectleiderId).ContactIdOrigineel);
            project projectorigineel = dbOriginalRep.GetProject(project.ProjectIdOrigineel);

            // create the page and load all values
            Project pv = new Project(projectorigineel.naam_project + " - ID " + projectorigineel.project_NR, projectleider.voornaam + " " + projectleider.tussenvoegsel + " " + projectleider.achternaam);

            // load the page into the contentcontrol
            PageGoToPage(pv);

            Mouse.OverrideCursor = null;


        }

        public void dgProjecten_MouseDoubleClickForVakman(object sender, MouseButtonEventArgs e)
        {
            ToevoegenAanVakman();
        }

        public void ToevoegenAanVakman()
        {
            Mouse.OverrideCursor = Cursors.Wait;

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

            Mouse.OverrideCursor = null;

        }

    }
}

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
using ZeebregtsLogic;
using MandagenRegistratieDomain;
using MandagenRegistratie.controls.Projecten.Dagview;

namespace MandagenRegistratie.controls.Projecten.Lijst
{
    /// <summary>
    /// Interaction logic for Project.xaml
    /// </summary>
    public partial class Project : MenuControl
    {

        public class cPeriode
        {
            public string periodeNaam { get; set; }
            public string periodeVan { get; set; }
            public string periodeTot { get; set; }
            public string periodeProjectleider { get; set; }
        }


        public Project(MDRproject pr)
        {
            InitializeComponent();
            #region Pagina specifieke informatie
            PageTitle = "Project " + pr.project_NR; // pageTitle;
            PageSubtitle = "Gegevens van " + pr.naam_project;
            if (Rechten.IsProjectleider)
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Hidden;
            }
            
            PageOKButtonText = "WIJZIG";
            #endregion

            // load info op pagina
            //this.Load();

            Loaded += Project_Loaded;

            OkClick += Okay;
            Reloaded += Load;
//#if DEBUG
//#else
            ChannelMessage += ReceiveChannelMessage;

//#endif

            // load info op pagina
            this.Load();

            //// create the page and load all values
            //VakmanDagView vdv = new VakmanDagView();
            //vdv.LoadVakmanDagView(true);
            //vdv.LoadWeekInfo();

            //tabPlanning.Content = vdv;
        }

        public void SelectTab(int intSelectedTab)
        {
                tabControl.SelectedIndex = intSelectedTab;
        }


        public void Project_Loaded(object sender, RoutedEventArgs e)
        {

            PageContainer pc = (PageContainer)Tools.FindVisualParent<Window>(this);
            if (Rechten.IsProjectleider)
            {
                pc.SetVisibilityGereedButton(System.Windows.Visibility.Visible);
            }
            else
            {
                pc.SetVisibilityGereedButton(System.Windows.Visibility.Hidden);
            }

            if (tabDetails.IsSelected)
            {
                if (Rechten.IsProjectleider)
                {
                    pc.SetLabelSubtitleGegevens(true);
                }
                else
                {
                    pc.SetLabelSubtitleGegevens(false);
                }
            }
            else
            {
                if (Rechten.IsProjectleider)
                {
                    pc.SetLabelSubtitlePlanning(true);
                }
                else
                {
                    pc.SetLabelSubtitlePlanning(false);
                }
            }
        }



        int intProjectId = -1;
        DateTime dtSelectedDay = DateTime.MinValue;

        /// <summary>
        /// Voert de OKAY functie uit
        /// </summary>
        public void Okay()
        {

            ProjectEdit pe = new ProjectEdit(PageSubtitle);
            pe.Load();

            PageGoToPage(pe);

        }

        /// <summary>
        /// Voert de OKAY functie uit
        /// </summary>
        public void ReceiveChannelMessage(string sender, string message)
        {
            ((ProjectDagView)tabPlanning.Content).ReceiveChannelMessage(sender, message);
        }


        public void Load()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                int projectId = ApplicationState.GetValue<int>(ApplicationVariables.intProjectId);
                int projectleiderId = ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).ProjectleiderId;

                dbRepository dbrep = new dbRepository();

                if (intProjectId == -1)
                {
                    intProjectId = ApplicationState.GetValue<int>(ApplicationVariables.intProjectId);
                }

                //if (dtSelectedDay == DateTime.MinValue)
                //{
                dtSelectedDay = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
                //}

                MandagenRegistratieDomain.Project project = dbrep.GetProject(intProjectId);

                dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
                MDRpersoon persoon = dbrepOriginal.GetContact(dbrep.GetProjectleider(project.ProjectleiderId).ContactIdOrigineel);
                MDRproject dboproject = dbrepOriginal.GetProject((int)project.ProjectNr);


                MandagenRegistratieDomain.MDRadressen dboadres = null;

                if (dboproject.adres_id_bouw != null)
                {
                    dboadres = dbrepOriginal.datacontext.MDRadressens.Where(a => a.adres_id == dboproject.adres_id_bouw).FirstOrDefault();

                }
                else if (dboproject.adres_id_factuur != null)
                {
                    dboadres = dbrepOriginal.datacontext.MDRadressens.Where(a => a.adres_id == dboproject.adres_id_factuur).FirstOrDefault();
                }

                MDRbedrijf bedrijfzdb = dbrepOriginal.datacontext.MDRbedrijfs.Where(b => b.bedrijf_nr == dboproject.opdrachtgeverZEEBREGTS_nr).FirstOrDefault();

                if (bedrijfzdb != null)
                {
                    lblOpdrachtgever.Content = bedrijfzdb.naam;
                }

                lblProjectNr.Content = project.ProjectNr.ToString(); // dbrepOriginal.GetProject((int)project.ProjectNr).project_NR.ToString();

                lblProjectnaam.Content = dboproject.naam_project;

                if (persoon != null)
                {
                    lblProjectleider.Content = (persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam).ToStringTrimmed();
                }
                else
                {
                    lblProjectleider.Content = "";
                }

                lblProjectcodeExtern.Content = dboproject.aannemer_projectnummer;
                lblProjectContractnummer.Content = dboproject.aannemer_contractnummer;
                lblProjectNacalculatiecode.Content = dboproject.nacalculatiecode;

                bool showLblReferentieOpdrachtgever = false;

                if (string.IsNullOrWhiteSpace(dboproject.aannemer_projectnummer))
                {
                    lblProjectcodeExtern.Visibility = System.Windows.Visibility.Collapsed;
                    lblProjectcodeExtern2.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    lblProjectcodeExtern.Visibility = System.Windows.Visibility.Visible;
                    lblProjectcodeExtern2.Visibility = System.Windows.Visibility.Visible;
                    showLblReferentieOpdrachtgever = true;
                }

                if (string.IsNullOrWhiteSpace(dboproject.aannemer_contractnummer))
                {
                    lblProjectContractnummer.Visibility = System.Windows.Visibility.Collapsed;
                    lblProjectContractnummer2.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    lblProjectContractnummer.Visibility = System.Windows.Visibility.Visible;
                    lblProjectContractnummer2.Visibility = System.Windows.Visibility.Visible;
                    showLblReferentieOpdrachtgever = true;
                }

                if (string.IsNullOrWhiteSpace(dboproject.nacalculatiecode))
                {
                    lblProjectNacalculatiecode.Visibility = System.Windows.Visibility.Collapsed;
                    lblProjectNacalculatiecode2.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    lblProjectNacalculatiecode.Visibility = System.Windows.Visibility.Visible;
                    lblProjectNacalculatiecode2.Visibility = System.Windows.Visibility.Visible;
                    showLblReferentieOpdrachtgever = true;
                }

                if (showLblReferentieOpdrachtgever)
                {
                    lblReferentieOpdrachtgever.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    lblReferentieOpdrachtgever.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (dboadres != null)
                {
                    lblPostcode.Content = (dboadres.postcode_cijfers.HasValue ? dboadres.postcode_cijfers.Value.ToString() + dboadres.postcode_letters : "") + ", " + dboadres.plaats;

                    string postcodeTemp = (dboadres.postcode_cijfers.HasValue ? dboadres.postcode_cijfers.Value.ToString() + dboadres.postcode_letters : "");

                    lblPostcode.Content = postcodeTemp + (string.IsNullOrWhiteSpace(dboadres.plaats) || string.IsNullOrWhiteSpace(postcodeTemp) ? "" : ", ") + dboadres.plaats;

                    
                    //lblHuisnummer.Content = dboadres.huis_postbus_nummer + dboadres.huisnummer_toevoeging;
                    lblAdres.Content = dboadres.straat_1 + " " + dboadres.huis_postbus_nummer + dboadres.huisnummer_toevoeging;
                    //lblPlaats.Content = dboadres.plaats;
                    lblLand.Content = dboadres.land;

                    bool showLblAdresProject = false;

                    if (string.IsNullOrWhiteSpace(lblPostcode.Content.ToString()))
                    {
                        lblPostcode.Visibility = System.Windows.Visibility.Collapsed;
                        lblPostcode2.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {

                        if (string.IsNullOrWhiteSpace(postcodeTemp))
                        {
                            lblPostcode2.Content = "Plaats";
                        }
                        else if (string.IsNullOrWhiteSpace(dboadres.plaats))
                        {
                            lblPostcode2.Content = "Postcode";
                        }
                        else
                        {
                            lblPostcode2.Content = "Postcode, plaats";
                        }

                        lblPostcode.Visibility = System.Windows.Visibility.Visible;
                        lblPostcode2.Visibility = System.Windows.Visibility.Visible;
                        showLblAdresProject = true;
                    }

                    //if (string.IsNullOrWhiteSpace(lblHuisnummer.Content.ToString()))
                    //{
                    //    lblHuisnummer.Visibility = System.Windows.Visibility.Collapsed;
                    //    lblHuisnummer2.Visibility = System.Windows.Visibility.Collapsed;
                    //}
                    //else
                    //{
                    //    lblHuisnummer.Visibility = System.Windows.Visibility.Visible;
                    //    lblHuisnummer2.Visibility = System.Windows.Visibility.Visible;
                    //    showLblAdresProject = true;
                    //}

                    if (string.IsNullOrWhiteSpace(lblAdres.Content.ToString()))
                    {
                        lblAdres.Visibility = System.Windows.Visibility.Collapsed;
                        lblAdres2.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        lblAdres.Visibility = System.Windows.Visibility.Visible;
                        lblAdres2.Visibility = System.Windows.Visibility.Visible;
                        showLblAdresProject = true;
                    }

                    //if (string.IsNullOrWhiteSpace(lblPlaats.Content.ToString()))
                    //{
                    //    lblPlaats.Visibility = System.Windows.Visibility.Collapsed;
                    //    lblPlaats2.Visibility = System.Windows.Visibility.Collapsed;
                    //}
                    //else
                    //{
                    //    lblPlaats.Visibility = System.Windows.Visibility.Visible;
                    //    lblPlaats2.Visibility = System.Windows.Visibility.Visible;
                    //    showLblAdresProject = true;
                    //}

                    if (string.IsNullOrWhiteSpace(lblLand.Content.ToString()))
                    {
                        lblLand.Visibility = System.Windows.Visibility.Collapsed;
                        lblLand2.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        lblLand.Visibility = System.Windows.Visibility.Visible;
                        lblLand2.Visibility = System.Windows.Visibility.Visible;
                        showLblAdresProject = true;
                    }

                    if (showLblAdresProject)
                    {
                        lblAdresProject.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        lblAdresProject.Visibility = System.Windows.Visibility.Collapsed;
                    }

                }


                ProjectDagView pdv = new ProjectDagView();
                pdv.objProject = project;
                pdv.LoadVakmanDagView(true, intProjectId, dtSelectedDay);
                pdv.LoadWeekInfo();


                //pdv.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

                tabPlanning.Content = pdv;

                // set periodes
                List<cPeriode> listPeriodes = new List<cPeriode>();

                List<Mandagen> listMandagen = dbrep.datacontext.Mandagens.Where(m => m.ProjectId == intProjectId).ToList();
                List<Gebruiker> listProjectleiders2 = dbrep.GetProjectleiders();

                // set huidige projectleider
                lblHuidigeProjectEigenaar.Content = listProjectleiders2.Where(p => p.ProjectleiderId == project.ProjectleiderId).FirstOrDefault().Gebruikersnaam;


                cPeriode cp = new cPeriode();
                int previousProjectleiderId = 0;
                int counter = 0;

                List<Mandagen> listMandagenByDate = listMandagen.Where(m => m.Begintijd != m.Eindtijd).OrderBy(m => m.Begintijd).ToList();
                foreach (Mandagen mandag in listMandagenByDate)
                {
                    // count the loop
                    counter++;

                    int currentProjectleiderId = mandag.ProjectleiderId;
                    //alleen de eerste keer startdatum standaard instellen
                    if (previousProjectleiderId == 0)
                    {
                        cp.periodeVan = mandag.Begintijd.ToString("dd-MM-yyyy");
                    }


                    // als de huidige projectleider een andere is dan de vorige
                    // dan vorige opslaan, en verder gaan met een nieuwe
                    if (currentProjectleiderId != previousProjectleiderId && previousProjectleiderId != 0)
                    {
                        listPeriodes.Add(cp);
                        cp = new cPeriode();
                        cp.periodeVan = mandag.Begintijd.ToString("dd-MM-yyyy");
                        cp.periodeTot = mandag.Eindtijd.ToString("dd-MM-yyyy");
                        cp.periodeProjectleider = listProjectleiders2.FirstOrDefault(pl => pl.ProjectleiderId == mandag.ProjectleiderId).Gebruikersnaam;
                    }
                    // anders alleen verder gaan met de einddatum
                    else
                    {
                        cp.periodeTot = mandag.Eindtijd.ToString("dd-MM-yyyy");
                        cp.periodeProjectleider = listProjectleiders2.FirstOrDefault(pl => pl.ProjectleiderId == mandag.ProjectleiderId).Gebruikersnaam;
                    }


                    // als het de laatste is, dan sowieso opslaaan

                    if (listMandagenByDate.Count == counter)
                    {
                        listPeriodes.Add(cp);
                    }

                    // sla de previousProjectleiderID op
                    previousProjectleiderId = mandag.ProjectleiderId;

                }


                //foreach (Gebruiker pl in listProjectleiders2.Where(p => listMandagen.Any(m => m.ProjectleiderId == p.ProjectleiderId)))
                //{
                //    cPeriode cp = new cPeriode();
                //    cp.periodeVan = listMandagen.Where(m=>m.ProjectleiderId == pl.ProjectleiderId).OrderBy(m => m.Begintijd).FirstOrDefault().Begintijd.ToString("dd-MM-yyyy");
                //    cp.periodeTot = listMandagen.Where(m => m.ProjectleiderId == pl.ProjectleiderId).OrderByDescending(m => m.Eindtijd).FirstOrDefault().Begintijd.ToString("dd-MM-yyyy");
                //    cp.periodeProjectleider = pl.Gebruikersnaam;

                //    listPeriodes.Add(cp);
                //}

                itemsPeriodes.ItemsSource = listPeriodes;

                //// TODO: testen, null waarde ? why?
                //PageContainer pc = (PageContainer)Tools.FindVisualParent<Window>(this);
                //pc.SetLabelSubtitlePlanning();

            }
            catch
            {
                MessageBox.Show("Er is een onbekende fout opgetreden, error #501");
            }

            Mouse.OverrideCursor = null;

        }


        private void MenuControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Window.GetWindow(this) != null)
            {
                //tabControl.Height = Window.GetWindow(this).ActualHeight - 110;
                tabControl.Width = Window.GetWindow(this).ActualWidth - 20;
            }

        }

        private void tabDetails_GotFocus(object sender, RoutedEventArgs e)
        {
            PageContainer pc = (PageContainer)Tools.FindVisualParent<Window>(this);
            if (Rechten.IsProjectleider)
            {
                pc.SetLabelSubtitleGegevens(true);
            }
            else
            {
                pc.SetLabelSubtitleGegevens(false);
            }

        }

        private void tabPlanning_GotFocus(object sender, RoutedEventArgs e)
        {
            PageContainer pc = (PageContainer)Tools.FindVisualParent<Window>(this);
            if (Rechten.IsProjectleider)
            {
                pc.SetLabelSubtitlePlanning(true);
            }
            else
            {
                pc.SetLabelSubtitlePlanning(false);
            }
        }


    }
}

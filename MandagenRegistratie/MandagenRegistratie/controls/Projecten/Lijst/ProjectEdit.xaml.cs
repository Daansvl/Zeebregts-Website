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
using System.Text.RegularExpressions;
using MandagenRegistratie.tools;

namespace MandagenRegistratie.controls.Projecten.Lijst
{
    /// <summary>
    /// Interaction logic for ProjectEdit.xaml
    /// </summary>
    public partial class ProjectEdit : MenuControl
    {
        bool ChangeAdres = true;
        string AdresLookup = "";

        DateTime dtFirstPossibleDate;
        DateTime dtLastPossibleDate;
        DateTime dtSelectedDate = DateTime.MinValue;
        DateTime dtSelectedDateEnd = DateTime.MinValue;

        public class cPeriode
        {
            public DateTime periodeStart { get; set; }
            public DateTime periodeStop { get; set; }
            public string periodeNaam { get; set; }
            public string periodeVan { get; set; }
            public string periodeTot { get; set; }
            public string periodeProjectleider { get; set; }
        }


        public ProjectEdit(string pageSubTitle)
        {
            InitializeComponent();
            #region Pagina specifieke informatie
            PageTitle = "Wijzigen";
            PageSubtitle = pageSubTitle + " wijzigen";

            if (Rechten.IsProjectleider)
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Hidden;
            }

            PageOKButtonText = "OPSLAAN";
            PageBackButtonText = "ANNULEER";
            #endregion

            // load info op pagina
            Load();

            this.OkClick += Save;
            this.Reloaded += Load;
        }

        public void Save()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                int projectId = ApplicationState.GetValue<int>(ApplicationVariables.intProjectId);
                int projectleiderId = ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).ProjectleiderId;

                dbRepository dbrep = new dbRepository();
                dbOriginalRepository dbrepOriginal = new dbOriginalRepository();

                MandagenRegistratieDomain.Project project = dbrep.datacontext.Projects.Where(p => p.ProjectId == projectId).FirstOrDefault();
                MandagenRegistratieDomain.MDRproject dboproject = dbrepOriginal.datacontext.MDRprojects.Where(p => p.project_NR == project.ProjectNr).FirstOrDefault();
                MandagenRegistratieDomain.MDRadressen dboadres = dbrepOriginal.datacontext.MDRadressens.Where(a => a.adres_id == dboproject.adres_id_bouw).FirstOrDefault();

                project.Naam = txtProjectnaam.Text;
                project.Postcode = txtPostcodeCijfers.Text; // +txtPostcodeLetters.Text;
                project.Huisnummer = txtHuisnummer.Text;
                project.Adres = txtAdres.Text;

                // loggen dat het gewijzigd is
                Logging logging = new Logging();

                // projecteigenaarschap wijzigen, if wanted
                if ((int)cbProjectleiders2.SelectedValue != projectleiderId && projectleiderId == project.ProjectleiderId)
                {
                    // waarschuwen en alleen doorvoeren als akkoord
                    MessageBoxResult mr = MessageBox.Show("Na wijziging eigenaarschap kunt u geen aanpassingen meer doen!", "Let op!", MessageBoxButton.OKCancel);
                    if (mr == MessageBoxResult.OK)
                    {

                        string logregel1 = string.Empty;
                        logregel1 += "Projectleider overdracht|Van:" + project.ProjectleiderId + "," + dbrep.GetProjectleider(project.ProjectleiderId).Gebruikersnaam;
                        logregel1 += "|Naar:" + cbProjectleiders2.SelectedValue.ToString() + "," + dbrep.GetProjectleider((int)cbProjectleiders2.SelectedValue).Gebruikersnaam;
                        logging.Log(logregel1);

                        project.ProjectleiderId = (int)cbProjectleiders2.SelectedValue;

                    }
                }

                // alleen eigen mandagenrecords wijzigen, if wanted
                if (dtSelectedDate != DateTime.MinValue && dtSelectedDateEnd != DateTime.MinValue)
                {

                    // voeg 24 uur aan de einddatum toe
                    dtSelectedDateEnd = dtSelectedDateEnd.AddHours(24);

                    string logregel1 = string.Empty;
                    logregel1 += "Uren overdracht";
                    logregel1 += "|Aan:" + cbProjectleiders.SelectedValue.ToString() + "," + dbrep.GetProjectleider((int)cbProjectleiders.SelectedValue).Gebruikersnaam;
                    logregel1 += "|Periode:" + dtSelectedDate.ToString("yyyy-MM-dd") + "-" + dtSelectedDateEnd.ToString("yyyy-MM-dd");


                    List<Mandagen> listMandagen = dbrep.datacontext.Mandagens.Where(m => m.ProjectId == projectId).ToList();
                    string urengewijzigd = Functies.CalculateUrenExact(listMandagen.Where(m => m.Begintijd >= dtSelectedDate && m.Eindtijd <= dtSelectedDateEnd && m.ProjectleiderId == projectleiderId).ToList());

                    if (dtSelectedDateEnd >= dtSelectedDate)
                    {
                        foreach (Mandagen mandag in dbrep.datacontext.Mandagens.Where(m => m.ProjectId == projectId && m.Begintijd >= dtSelectedDate && m.Eindtijd <= dtSelectedDateEnd && m.ProjectleiderId == projectleiderId))
                        {
                            mandag.ProjectleiderId = (int)cbProjectleiders.SelectedValue;
                        }
                    }

                    logregel1 += "|Uren gewijzigd:" + (string.IsNullOrEmpty(urengewijzigd) ? "0" : urengewijzigd);

                    logging.Log(logregel1);
                }




                //dbrep.SaveProject(project);
                dbrep.datacontext.SubmitChanges();

                if (dboadres != null)
                {
                    // postcode
                    if (!string.IsNullOrWhiteSpace(txtPostcodeCijfers.Text))
                    {
                        try
                        {
                            if (txtPostcodeCijfers.Text.Length >= 6)
                            {
                                dboadres.postcode_cijfers = Convert.ToInt32(txtPostcodeCijfers.Text.Substring(0, 4));
                                dboadres.postcode_letters = txtPostcodeCijfers.Text.Substring(4).Trim();
                            }
                            else
                            {
                                MessageBox.Show("Postcode heeft een onjuiste format. Gegevens worden opgeslagen zonder de postcode");
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Postcode heeft een onjuiste format. Gegevens worden opgeslagen zonder de postcode");
                        }
                    }
                    else
                    {
                        dboadres.postcode_cijfers = null;
                        dboadres.postcode_letters = string.Empty;
                    }

                    dboadres.straat_1 = txtAdres.Text;
                    dboadres.huis_postbus_nummer = txtHuisnummer.Text;
                    dboadres.plaats = txtPlaats.Text;
                    dboadres.land = txtLand.Text;
                }

                if (dboproject != null)
                {
                    dboproject.aannemer_projectnummer = txtProjectcodeExtern.Text;
                    dboproject.aannemer_contractnummer = txtProjectContractnummer.Text;
                    dboproject.nacalculatiecode = txtProjectNacalculatiecode.Text;
                }

                dbrepOriginal.datacontext.SubmitChanges();



                // na het saven terug in history
                PageGoBack();

                Mouse.OverrideCursor = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        /// <summary>
        /// 
        /// </summary>
        public void Load()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            int projectId = ApplicationState.GetValue<int>(ApplicationVariables.intProjectId);
            int projectleiderId = ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).ProjectleiderId;

            dbRepository dbrep = new dbRepository();

            MandagenRegistratieDomain.Project project = dbrep.GetProject(ApplicationState.GetValue<int>(ApplicationVariables.intProjectId));

            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
            MDRpersoon persoon = dbrepOriginal.GetContact(dbrep.GetProjectleider(project.ProjectleiderId).ContactIdOrigineel);

            MandagenRegistratieDomain.MDRproject dboproject = dbrepOriginal.datacontext.MDRprojects.Where(p => p.project_NR == project.ProjectNr).FirstOrDefault();
            MandagenRegistratieDomain.MDRadressen dboadres = dbrepOriginal.datacontext.MDRadressens.Where(a => a.adres_id == dboproject.adres_id_bouw).FirstOrDefault();


            lblProjectNr.Content = project.ProjectNr.ToString(); // dbrepOriginal.GetProject((int)project.ProjectNr).project_NR.ToString();


            if (dboadres != null)
            {
                txtPostcodeCijfers.Text = (dboadres.postcode_cijfers.HasValue ? dboadres.postcode_cijfers.Value.ToString() : "") + dboadres.postcode_letters;
                //txtPostcodeLetters.Text = ;
                txtHuisnummer.Text = dboadres.huis_postbus_nummer + dboadres.huisnummer_toevoeging;
                txtAdres.Text = dboadres.straat_1;
                if (dboadres.straat_1.Length - txtHuisnummer.Text.Length > 0)
                {
                    AdresLookup = dboadres.straat_1.Substring(0, dboadres.straat_1.Length - txtHuisnummer.Text.Length).Trim();
                }
                else
                {
                    AdresLookup = "";
                }

                txtPlaats.Text = dboadres.plaats;
                txtLand.Text = dboadres.land;
            }

            txtProjectnaam.Text = dboproject.naam_project;

            if (persoon != null)
            {
                lblProjectleider.Content = (persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam).ToStringTrimmed();
            }
            else
            {
                lblProjectleider.Content = "";
            }

            txtProjectcodeExtern.Text = dboproject.aannemer_projectnummer;
            txtProjectContractnummer.Text = dboproject.aannemer_contractnummer ;
            txtProjectNacalculatiecode.Text = dboproject.nacalculatiecode;


            // set dropdownlist
            List<Gebruiker> listProjectleiders = new List<Gebruiker>();
            listProjectleiders = dbrep.GetProjectleiders().Where(g => g.IsProjectleider).ToList();

            cbProjectleiders.ItemsSource = listProjectleiders;
            cbProjectleiders.DisplayMemberPath = "Gebruikersnaam";
            cbProjectleiders.SelectedValuePath = "ProjectleiderId";

            cbProjectleiders.SelectedValue = ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).ProjectleiderId;

            // set projectleiders voor eigenaarschap
            cbProjectleiders2.ItemsSource = listProjectleiders;
            cbProjectleiders2.DisplayMemberPath = "Gebruikersnaam";
            cbProjectleiders2.SelectedValuePath = "ProjectleiderId";

            cbProjectleiders2.SelectedValue = project.ProjectleiderId;


            // set zichtbaarheid
            if (project.ProjectleiderId == projectleiderId)
            {
                cbProjectleiders2.IsEnabled = true;
            }


            // set periodes
            List<cPeriode> listPeriodes = new List<cPeriode>();

            List<Mandagen> listMandagen = dbrep.datacontext.Mandagens.Where(m => m.ProjectId == projectId).ToList();
            List<Gebruiker> listProjectleiders2 = dbrep.GetProjectleiders();

            // set huidige projectleider
            lblHuidigeProjectEigenaar.Content = listProjectleiders2.Where(p => p.ProjectleiderId == project.ProjectleiderId).FirstOrDefault().Gebruikersnaam;

            // default het scherm onzichtbaar maken
            // wpUrenOverdragen.Visibility = System.Windows.Visibility.Collapsed;

            // greyen in plaats van onzichtbaar maken
            lblBeginDatum.IsEnabled = false;
            cbProjectleiders.IsEnabled = false;
            // standaard zo
            imgCalendarOn.Visibility = System.Windows.Visibility.Collapsed;
            imgCalendar.Visibility = System.Windows.Visibility.Visible;


            if (listMandagen.Where(m => m.Begintijd != m.Eindtijd).Count() > 0)
            {
            }

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
                    cp.periodeStart = mandag.Begintijd;
                }


                // als de huidige projectleider een andere is dan de vorige
                // dan vorige opslaan, en verder gaan met een nieuwe
                if (currentProjectleiderId != previousProjectleiderId && previousProjectleiderId != 0)
                {
                    listPeriodes.Add(cp);
                    cp = new cPeriode();
                    cp.periodeVan = mandag.Begintijd.ToString("dd-MM-yyyy");
                    cp.periodeStart = mandag.Begintijd;
                    cp.periodeTot = mandag.Eindtijd.ToString("dd-MM-yyyy");
                    cp.periodeProjectleider = listProjectleiders2.FirstOrDefault(pl => pl.ProjectleiderId == mandag.ProjectleiderId).Gebruikersnaam;
                    cp.periodeStop = mandag.Eindtijd;
                }
                // anders alleen verder gaan met de einddatum
                else
                {
                    cp.periodeTot = mandag.Eindtijd.ToString("dd-MM-yyyy");
                    cp.periodeProjectleider = listProjectleiders2.FirstOrDefault(pl => pl.ProjectleiderId == mandag.ProjectleiderId).Gebruikersnaam;
                    cp.periodeStop = mandag.Eindtijd;
                }


                // als het de laatste is, dan sowieso opslaaan

                if (listMandagenByDate.Count == counter)
                {
                    listPeriodes.Add(cp);
                }

                // sla de previousProjectleiderID op
                previousProjectleiderId = mandag.ProjectleiderId;

                // als ik een periode heb, kan ik uren overzetten, anders heeft dat scherm uberhaupt geen nut
                if (mandag.ProjectleiderId == projectleiderId)
                {
                    wpUrenOverdragen.Visibility = System.Windows.Visibility.Visible;

                    // niet meer ge-greyd
                    lblBeginDatum.IsEnabled = true;
                    cbProjectleiders.IsEnabled = true;
                    // calendar plaatjes verwisselen
                    imgCalendarOn.Visibility = System.Windows.Visibility.Visible;
                    imgCalendar.Visibility = System.Windows.Visibility.Collapsed;

                    dtFirstPossibleDate = listMandagen.Where(m => m.ProjectleiderId == projectleiderId && m.Begintijd != m.Eindtijd).OrderBy(m => m.Begintijd).FirstOrDefault().Begintijd;
                    dtLastPossibleDate = listMandagen.Where(m => m.ProjectleiderId == projectleiderId && m.Begintijd != m.Eindtijd).OrderByDescending(m => m.Eindtijd).FirstOrDefault().Eindtijd;

                    // reset selected date on Load
                    dtSelectedDate = dtFirstPossibleDate;
                    dpBeginDatum.SelectedDate = dtSelectedDate;
                    lblBeginDatum.Content = dtSelectedDate.ToString("dd-MM-yyyy");

                }

            }

            //foreach (Gebruiker pl in listProjectleiders2.Where(p => listMandagen.Any(m => m.ProjectleiderId == p.ProjectleiderId)))
            //{
            //    cPeriode cp = new cPeriode();
            //    cp.periodeVan = listMandagen.Where(m => m.ProjectleiderId == pl.ProjectleiderId && m.Begintijd != m.Eindtijd).OrderBy(m => m.Begintijd).FirstOrDefault().Begintijd.ToString("dd-MM-yyyy");
            //    cp.periodeTot = listMandagen.Where(m => m.ProjectleiderId == pl.ProjectleiderId && m.Begintijd != m.Eindtijd).OrderByDescending(m => m.Eindtijd).FirstOrDefault().Eindtijd.ToString("dd-MM-yyyy");
            //    cp.periodeProjectleider = pl.Gebruikersnaam;
            //    cp.periodeStart = listMandagen.Where(m => m.ProjectleiderId == pl.ProjectleiderId && m.Begintijd != m.Eindtijd).OrderBy(m => m.Begintijd).FirstOrDefault().Begintijd;
            //    cp.periodeStop = listMandagen.Where(m => m.ProjectleiderId == pl.ProjectleiderId && m.Begintijd != m.Eindtijd).OrderByDescending(m => m.Eindtijd).FirstOrDefault().Eindtijd;

            //    listPeriodes.Add(cp);

            //    // als ik een periode heb, kan ik uren overzetten, anders heeft dat scherm uberhaupt geen nut
            //    if (pl.ProjectleiderId == projectleiderId)
            //    {
            //        wpUrenOverdragen.Visibility = System.Windows.Visibility.Visible;

            //        // niet meer ge-greyd
            //        lblBeginDatum.IsEnabled = true;
            //        cbProjectleiders.IsEnabled = true;
            //        // calendar plaatjes verwisselen
            //        imgCalendarOn.Visibility = System.Windows.Visibility.Visible;
            //        imgCalendar.Visibility = System.Windows.Visibility.Collapsed;

            //        dtFirstPossibleDate = listMandagen.Where(m => m.ProjectleiderId == pl.ProjectleiderId && m.Begintijd != m.Eindtijd).OrderBy(m => m.Begintijd).FirstOrDefault().Begintijd;
            //        dtLastPossibleDate = listMandagen.Where(m => m.ProjectleiderId == pl.ProjectleiderId && m.Begintijd != m.Eindtijd).OrderByDescending(m => m.Eindtijd).FirstOrDefault().Eindtijd;

            //        // reset selected date on Load
            //        dtSelectedDate = dtFirstPossibleDate;
            //        dpBeginDatum.SelectedDate = dtSelectedDate;
            //        lblBeginDatum.Content = dtSelectedDate.ToString("dd-MM-yyyy");

            //    }
            //}



            itemsPeriodes.ItemsSource = listPeriodes.OrderBy(p=> p.periodeStart).ThenBy(p => p.periodeStop).ToList();


            Mouse.OverrideCursor = null;


        }

        public void AutoVulAdres(object sender, KeyEventArgs e)
        {
            txtPostcodeCijfers.Text = txtPostcodeCijfers.Text.ToUpper();
            txtPostcodeCijfers.CaretIndex = txtPostcodeCijfers.Text.Length;

            string _postcode = txtPostcodeCijfers.Text; //+ txtPostcodeLetters.Text;

            Tools tool = new Tools();

            var regx = new Regex(@"(\d{4}\s*)(\D{2})");
            var match = regx.Match(_postcode);

            if (match.Success)
            {
                APIResponse adresComplete = tool.ZoekOpPostcodeComplete(_postcode);
                if (adresComplete != null)
                {

                    string straat = adresComplete.results[0].street;

                    txtAdres.Text = straat;
                    txtPlaats.Text = adresComplete.results[0].city;
                    txtLand.Text = "Nederland";

                    AdresLookup = straat;
                }

                ChangeAdres = true;

            }
        }

        public void AdresAanvullen(object sender, KeyEventArgs e)
        {
            string huisnummer = txtHuisnummer.Text;

            if (ChangeAdres)
            {
                txtAdres.Text = AdresLookup + " " + huisnummer;
            }
        }

        public void AdresWijzigen(object sender, KeyEventArgs e)
        {
            // stops automatic changing of the adres
            if (e.Key != Key.Tab)
            {
                ChangeAdres = false;
            }
        }

        private void dpBeginDatum_SelectedDateChanged_1(object sender, RoutedEventArgs e)
        {
            dtSelectedDate = (DateTime)dpBeginDatum.SelectedDate;

            if (dtSelectedDate < dtFirstPossibleDate)
            {
                dtSelectedDate = dtFirstPossibleDate;
            }
            else if (dtSelectedDate > dtLastPossibleDate)
            {
                dtSelectedDate = dtLastPossibleDate;
            }

            dtSelectedDateEnd = dtLastPossibleDate;

            lblBeginDatum.Content = dtSelectedDate.ToString("dd-MM-yyyy");
            lblEindDatum.Content = dtSelectedDateEnd.ToString("dd-MM-yyyy");

            
            // terug invouwen
            lblBeginDatumOpvulling.Visibility = System.Windows.Visibility.Collapsed;
            dpBeginDatum.Visibility = System.Windows.Visibility.Collapsed;

            // tot en met datum nu ook invullen
            lblTotenMet.Visibility = System.Windows.Visibility.Visible;
            lblEindDatum.Visibility = System.Windows.Visibility.Visible;
            dpEinddatumButton.Visibility = System.Windows.Visibility.Visible;
        }

        private void dpEindDatum_SelectedDateChanged_1(object sender, RoutedEventArgs e)
        {
            dtSelectedDateEnd = ((DateTime)dpEindDatum.SelectedDate);

            if (dtSelectedDateEnd < dtSelectedDate)
            {
                dtSelectedDateEnd = dtSelectedDate;
            }

            lblEindDatum.Content = dtSelectedDateEnd.ToString("dd-MM-yyyy");

            // terug invouwen
            lblTotenMetOpvulling.Visibility = System.Windows.Visibility.Collapsed;
            dpEindDatum.Visibility = System.Windows.Visibility.Collapsed;

        }

        private void lblBeginDatum_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            // begindatum weergeven

            if (lblBeginDatumOpvulling.Visibility == System.Windows.Visibility.Visible)
            {
                lblBeginDatumOpvulling.Visibility = System.Windows.Visibility.Collapsed;
                dpBeginDatum.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                lblBeginDatumOpvulling.Visibility = System.Windows.Visibility.Visible;
                dpBeginDatum.Visibility = System.Windows.Visibility.Visible;

                // bij openklikken altijd de geselecteerde datum tonen in de datepicker
                if (dtSelectedDate != DateTime.MinValue)
                {
                    dpBeginDatum.DisplayDate = Convert.ToDateTime(lblBeginDatum.Content.ToString()); // dtSelectedDate;
                }
            }
        }

        private void lblEindDatum_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (lblTotenMetOpvulling.Visibility == System.Windows.Visibility.Visible)
            {
                lblTotenMetOpvulling.Visibility = System.Windows.Visibility.Collapsed;
                dpEindDatum.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                // einddatum weergeven
                lblTotenMetOpvulling.Visibility = System.Windows.Visibility.Visible;
                dpEindDatum.Visibility = System.Windows.Visibility.Visible;

                // bij openklikken altijd de geselecteerde datum tonen in de datepicker
                if (dtSelectedDateEnd != DateTime.MinValue)
                {
                    dpEindDatum.DisplayDate = Convert.ToDateTime(lblEindDatum.Content.ToString()); // dtSelectedDateEnd;
                }
            }
        }



    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeebregtsLogic;
using System.Configuration;
using System.Windows.Documents;

namespace MDR2PDF
{
    public partial class USMarioStart : WFControl
    {

        public string pubOMGEVING = (ConfigurationManager.AppSettings["WindowTitle"].ToString().ToUpper().Contains("DEV"))
            ? "DEV" :
            (ConfigurationManager.AppSettings["WindowTitle"].ToString().ToUpper().Contains("BETA"))
            ? "BETA" :
            "LIVE";

        public List<classLijstgegevens> pubLIJSTEN = new List<classLijstgegevens>();
        public dbLPdfOutputDataContext dbLijst = new dbLPdfOutputDataContext("MandagenRegistratieConnectionStringLIVE");

        public string pubConnectionString = "MandagenRegistratieConnectionStringLIVE";

        public bool GEBRUIKER = false;
        public bool SUBLIJSTOPNIEUWKIEZEN = true;


        public MarioPDFSettings PDFSETTTINGS = new MarioPDFSettings();

        static string xHtmlFolder = ConfigurationManager.AppSettings["htmlFolder"].ToString();
        public EvoTools ET = new EvoTools(xHtmlFolder);

        public USMarioStart()
        {
            InitializeComponent();

            // JURACI Coomunicatie met 'The APP'
            this.PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            this.PageGereedButtonText = "Bevestig NUNUNU";
            this.OkClick += BevestigFunctie; // Zodoende naar Pagina 2 te gaan
            this.PageTitle += "PDF gebeuren van Marino";
            this.PageSubtitle += "Jaja, we kunnen ook best met subtiteels werken!";


            // Zet instellingen knopje aan of uit (haal tevens pdfsettings.gebruiker op: Huidige windows-user of via Mandagen UserID)
            btnSettings.Visible = WindowsLogonIsManager();

            PDFSETTTINGS.Omgeving = pubOMGEVING;
            ApplicationState.SetValue(GlobaleVars.strApplicationState, PDFSETTTINGS);

            // Bouw de lijst voor de eerste keer op
            ET.Log("1e aanroep Bouwlijst");
            BouwLijst();
            ET.Log("1e aanroep Bouwlijst");
        }

        private void HalloUser()
        {
            label1.Visible = true;
            label1.Text = string.Format("Welkom {0}{1}", PDFSETTTINGS.Gebruiker.IsManager ? "Manager " : "", PDFSETTTINGS.Gebruiker.Gebruikersnaam);
            txbAdminInfo.Text = string.Format("{0} - {1}", PDFSETTTINGS.Omgeving, PDFSETTTINGS.Gebruiker.Gebruikersnaam);
            txbAdminInfo.Visible = true;
        }

        private bool WindowsLogonIsManager()
        {
            ET.Log("getUserLogon");
            string stringmd;
            switch (pubOMGEVING)
            {
                case "BETA":
                    stringmd = "MandagenRegistratieConnectionStringBETA";
                    break;
                case "DEV":
                    stringmd = "MandagenRegistratieConnectionStringDEV";
                    break;
                default: //"LIVE":
                    stringmd = "MandagenRegistratieConnectionStringLIVE";
                    break;
            }

            pubConnectionString = ConfigurationManager.ConnectionStrings[stringmd] !=null ? ConfigurationManager.ConnectionStrings[stringmd].ConnectionString : "";

            // Bepaal ingelogde gebruiker
            dbLMandagenDataContext dm = new dbLMandagenDataContext(pubConnectionString);
            ET.Log("Gebruik security string: "+ pubConnectionString );
            string windowsentity = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            Gebruiker Geb = new Gebruiker();
            bool IsAdmin = false;
            // GebruikersID indien doorgelogd door Mandagen!
            int Ui = ApplicationState.GetValue<int>(ApplicationVariables.intGebruikerId); 
            try
            {
                Geb = dm.Gebruikers.Where(x => x.Windowsidentity.ToUpper() == windowsentity.ToUpper()).FirstOrDefault();
                IsAdmin = Geb.IsAdministrator;
                // Haal nu de gegevens van degene als wie er is doorgelogt
                if (Ui > 0)
                    Geb = dm.Gebruikers.Where(x => x.GebruikerId == Ui).FirstOrDefault();
            }
            catch (Exception e)
            {
                MessageBox.Show("Uw Login gegevens zijn niet gevonden, bent u wel een gerigistreerd gebruiker?? \n" + e.ToString());
                return false;
            }
            PDFSETTTINGS.Gebruiker = Geb;

            // Lijsten opnieuw inlezen uit Database
            Load_Lijsten();

            return IsAdmin;
        }

        public void BouwLijst()
        {
            ET.Log("BouwLijst");

            // Onthoud vorige lijst-keuze
            int SelectedList = Math.Max( lbLijsten.SelectedIndex,0);

            ET.Log("BouwLijst 126");
            if (!PDFSETTTINGS.Gebruiker.CanPrint)
            {
                ET.Log("BouwLijst 129");
                lbLijsten.Items.Clear();
                ET.Log("BouwLijst 131");
                txbOmschrijving.Clear();
                ET.Log("Lijsten geleegd");
                return;
            }

            ET.Log("BouwLijst 137");
            // Maak nu de lijst opnieuw, rekening houdende met ingelogde gebruiker
            lbLijsten.Items.Clear();
            ET.Log("BouwLijst 140");
            
            foreach (classLijstgegevens Lijst in pubLIJSTEN)
            {
                // Indien een van de sublijsten van toepassing is, dan hoofdlijst tonen
                if ( (Lijst.SubLijst.Select(x => x.Managers).Contains(true) && PDFSETTTINGS.Gebruiker.IsManager) ||
                    (Lijst.SubLijst.Select(x => x.ProjectLeiders).Contains(true) && PDFSETTTINGS.Gebruiker.IsProjectleider) ||
                    (Lijst.SubLijst.Select(x => x.Managers).Contains(true) && PDFSETTTINGS.Gebruiker.IsAdministrator)
                    )
                    lbLijsten.Items.Add(Lijst.Titel);
            }
            // Als bij opnieuw opbouwen minder lijsten dan voorheen dan standaard bovenste

            ET.Log("BouwLijst 152");
            if (SelectedList > lbLijsten.Items.Count - 1)
                SelectedList = 0;
            ET.Log("BouwLijst 155");
            if (lbLijsten.Items.Count > 0) lbLijsten.SetSelected(SelectedList, true); // Standaard bovenste positie
            // Pas lijstgrootte aan
            ET.Log("BouwLijst 158");
            lbLijsten.AutoSize = false;
            lbLijsten.AutoSize = true;
        }



        /// <summary>
        /// Functie om naar Pagina 2 te gaan (vanuit Juraci Mandagen-Knop)
        /// </summary>
        public void BevestigFunctie()
        {
            ET.Log("BevestigFunctie");

            if (SubcodeNogNietBekend())
                return;

            if (PDFSETTTINGS.Gebruiker.CanPrint)
            {
                // Allereerst: (voor de zekerheid nogmaals) verzamelde gegevens naar ApplicationState copieren
                ApplicationState.SetValue(GlobaleVars.strApplicationState, PDFSETTTINGS);

                USMario pagina2 = new USMario();
                pagina2.PageTitle = "Rapport";
                pagina2.PageSubtitle = PDFSETTTINGS.Lijstgegevens.Titel;
                
                // probeer eens wat uit om Vertikaal alignment uit te zetten
                pagina2.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                pagina2.Top = 10;


                //MessageBox.Show(PDFSETTTINGS.Lijstgegevens.Titel + "/" + PDFSETTTINGS.Lijstgegevens.Code);

                //test 2014-06-23
                //this.Cursor = Cursors.WaitCursor;
                pagina2.Bevestig(PDFSETTTINGS);
                PageGoToPage(pagina2);
                //test 2014-06-23
                //this.Cursor = Cursors.Default;
                
                // Reset gekozen sublijst zodat bij terugkomst opnieuw sublijst moet worden gekozen
                //PDFSETTTINGS.Lijstgegevens.Code = ""; // Dit werkt niet omdat dit blijkbaar een verwijzing is vanuit pagina2
                SUBLIJSTOPNIEUWKIEZEN = true;
           }
            else MessageBox.Show("U heeft niet genoeg rechten om verder te gaan \nPretiige dag verder");
        }

        private bool SubcodeNogNietBekend()
        {
            // Laat de gebruiker eerst een sublijst uitzoeken! (indien nodig)
            if (PDFSETTTINGS.Lijstgegevens.SubLijst.Count == 1)
            {
                PDFSETTTINGS.Lijstgegevens.Code = PDFSETTTINGS.Lijstgegevens.SubLijst[0].SubCode;
            }
            else
            {
                if (PDFSETTTINGS.Lijstgegevens.Code == "" || SUBLIJSTOPNIEUWKIEZEN)
                {
                    contextMenuStrip1.Items.Clear();
                    contextMenuStrip1.Items.Add("kies één van onderstaande sublijsten");
                    contextMenuStrip1.Items.Add("");
                    foreach (var sl in PDFSETTTINGS.Lijstgegevens.SubLijst)
                    {
                        contextMenuStrip1.Items.Add(sl.SubTitel);
                    }
                    contextMenuStrip1.Show(Control.MousePosition.X - contextMenuStrip1.Width + 20, Control.MousePosition.Y - 10);
                    return true;
                }
            }
            return false;
        }

        public void lbLijsten_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbLijsten.SelectedIndex < 0) return;
            PDFSETTTINGS.Lijstgegevens.Titel = lbLijsten.SelectedItem.ToString();
            PDFSETTTINGS.Lijstgegevens.Code = "";
            
            // Voeg sublijstgegevens toe aan PDFSETTINGS
            PDFSETTTINGS.Lijstgegevens.SubLijst = new List<classSubLijstGegevens>();

            foreach (classLijstgegevens lg in pubLIJSTEN)
            {
                if (lg.Titel == PDFSETTTINGS.Lijstgegevens.Titel)
                {
                    for (int i = 0; i < lg.SubLijst.Count; i++)
                    {
                        if (PDFSETTTINGS.Gebruiker.IsAdministrator ||
                            (PDFSETTTINGS.Gebruiker.IsManager && lg.SubLijst[i].Managers) ||
                            (PDFSETTTINGS.Gebruiker.IsProjectleider && lg.SubLijst[i].ProjectLeiders)
                            )
                            PDFSETTTINGS.Lijstgegevens.SubLijst.Add(lg.SubLijst[i]);
                    }
                    break;
                }
            }
        }


        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            pnlSettings.Visible = false;
            if (comboBox1.SelectedIndex == 1)
                pubOMGEVING = "DEV";
            else if (comboBox1.SelectedIndex == 2)
                pubOMGEVING = "LIVE";
            else
                pubOMGEVING = "BETA";
            
            // Load mieuwe settings
            WindowsLogonIsManager();

            // Wijzig ApplicationState
            PDFSETTTINGS.Omgeving = pubOMGEVING;
            HalloUser();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            pnlSettings.Visible = !pnlSettings.Visible;
            //
            // Zorg ervoor dat in AssemblyInfo.cs het volgende staat: 
            //
            // [assembly: AssemblyVersion("1.0.*")] // important: use wildcard for build and revision numbers!
            //
            var version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            var buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(
            TimeSpan.TicksPerDay * version.Build + // days since 1 January 2000
            TimeSpan.TicksPerSecond * 2 * version.Revision)); // seconds since midnight, (multiply by 2 to get original)

            // Erg leuk allemaal maar dan geeft ie UTC en bovendien vind ik onderstaande gewoon robuuster, dus:
            string w_file = "MDR Output PDF.EXE";
            string w_directory = System.IO.Directory.GetCurrentDirectory();
            DateTime c3 = System.IO.File.GetLastWriteTime(System.IO.Path.Combine(w_directory, w_file));

            label3.Text = "Versie " + c3.ToString("yyyy-MM-dd HH:mm:ss")
            ;

        }

        private void pnlSettings_MouseMove(object sender, MouseEventArgs e)
        {
            return;
            Point P = new Point();
            P = pnlSettings.PointToScreen(P);
            if (MousePosition.X > P.X + pnlSettings.Width - 5 || MousePosition.Y > P.Y + pnlSettings.Height - 5)
                pnlSettings.Visible = false;
        }

        private void cbbGebruikers_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoginAls();
        }

        private void LoginAls()
        {
            if (cbbGebruikers.SelectedIndex < 0)
                return;
            pnlSettings.Visible = false;
            try
            {
                int g = Convert.ToInt16(cbbGebruikers.SelectedItem.ToString().Substring(0, cbbGebruikers.SelectedItem.ToString().IndexOf(" ")));
                dbLMandagenDataContext dm = new dbLMandagenDataContext(pubConnectionString);

                PDFSETTTINGS.Gebruiker = dm.Gebruikers.Where(x => x.GebruikerId == g).First();

                // Nieuwe user melden aan Juraci
                ApplicationState.SetValue(GlobaleVars.strApplicationState, PDFSETTTINGS);
                ApplicationState.SetValue(ApplicationVariables.intGebruikerId, PDFSETTTINGS.Gebruiker.GebruikerId);

                HalloUser();
                ET.Log("2e aanroep Bouwlijst");
                BouwLijst();
                ET.Log("2e aanroep Bouwlijst");

                // dit zet Juraci hier neer op 4 mei 2015
            }
            catch { }
        }

        private void cbbGebruikers_DropDown(object sender, EventArgs e)
        {
            try
            {
                dbLMandagenDataContext dm = new dbLMandagenDataContext(pubConnectionString);
                List<string> gebruikers = dm.Gebruikers.Select(x => 
                    x.GebruikerId.ToString() + " " + 
                    x.Gebruikersnaam + " (" + 
                    (x.CanPrint ? "Canprint" : "NOprint") +
                    (x.IsAdministrator ? ", +Admin" : "") +
                    (x.IsManager ? ", +Mngr" : "") +
                    (x.IsProjectleider ? ", +PrLdr" : "") +
                    ")"
                    ).ToList();

                cbbGebruikers.Items.Clear();
                foreach (string gebruiker in gebruikers)
                    cbbGebruikers.Items.Add(gebruiker);
            }
            catch { }

            // Pas breedte aan
            MszTools.AdjustWidthComboBox_DropDown(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lbLijsten.AutoSize=true;
        }

        private void pnlSettings_MouseLeave(object sender, EventArgs e)
        {
            if (gbManager.ClientRectangle.Contains(gbManager.PointToClient(Control.MousePosition)))
                return;            
            pnlSettings.Visible = false;
        }

        private void listBoxAlgemeeen_MouseHover(object sender, MouseEventArgs e)
        {
            Point point = ((ListBox)sender).PointToClient(Cursor.Position);
            int index = ((ListBox)sender).IndexFromPoint(point);
            if (index >= 0) //;//&& index != ((ListBox)sender).SelectedIndex)
            {
                //  ((ListBox)sender).SetSelected(index, true);
                // Kijk, nu gaan we vanzelf de module changed-selected-item in

                // NENENE we gaan niet de selectie zelf aanpassen! maar alleen de tooltip !!!!
                string NieuweTekst = "FOUT!!!";
                foreach (classLijstgegevens lg in pubLIJSTEN)
                {
                    if (lg.Titel == ((ListBox)sender).Items[index].ToString())
                    {
                        NieuweTekst="";
                        for (int i = 0; i < lg.SubLijst.Count; i++)
                        {
                            if (PDFSETTTINGS.Gebruiker.IsAdministrator ||
                                (lg.SubLijst[i].Managers == true && PDFSETTTINGS.Gebruiker.IsManager) ||
                                (lg.SubLijst[i].ProjectLeiders == true && PDFSETTTINGS.Gebruiker.IsProjectleider)
                                )
                                NieuweTekst += "\u2022" + lg.SubLijst[i].SubTitel + "\n";

                            //if (lg.SubLijst[i].Managers == false || PDFSETTTINGS.Gebruiker.IsAdministrator || PDFSETTTINGS.Gebruiker.IsManager)
                            //    NieuweTekst += "\u2022" + lg.SubLijst[i].SubTitel + "\n";
                        }
                    }
                }

                // Pas tooltip aan (indien nodig)
                if (toolTip1.GetToolTip((ListBox)sender) != NieuweTekst)
                    toolTip1.SetToolTip(((ListBox)sender), NieuweTekst);
            }
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var subtitels = PDFSETTTINGS.Lijstgegevens.SubLijst.Select(x => x.SubTitel).ToList();
            if (subtitels.Contains(e.ClickedItem.ToString()))
            {
                for (int i = 0; i < subtitels.Count; i++)
                {
                    if (subtitels[i] == e.ClickedItem.ToString())
                    {
                        PDFSETTTINGS.Lijstgegevens.Code = PDFSETTTINGS.Lijstgegevens.SubLijst[i].SubCode;
                        SUBLIJSTOPNIEUWKIEZEN = false;
                        break;
                    }
                }
                if (PDFSETTTINGS.Lijstgegevens.Code != "")
                    BevestigFunctie();
            }
        }

        private void btnLijsten_Click(object sender, EventArgs e)
        {
            gbLijst.Visible = true;
            gbLijst.BringToFront();
            Load_Lijsten();
            dataGridViewLijsten.Columns[0].Visible = false;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            //Load_Lijsten();
            gbLijst.Visible = false;
            pnlSettings.Visible = false;
        }

        private void Load_Lijsten()
        {
            ET.Log("LOAD_LIJSTEN");
            string connectiestring = pubConnectionString.Replace("MandagenRegistratie", "PdfOutput");
            ET.Log(connectiestring);
            dbLijst = new dbLPdfOutputDataContext(connectiestring);
            BindingSource b = new BindingSource();
            b.DataSource = from eq in dbLijst.Lijstens
                           orderby eq.SubCode
                           select eq;

            dataGridViewLijsten.DataSource = b;
            // Maak kolom 'Subcode' readonly!
            dataGridViewLijsten.Columns[2].DefaultCellStyle.ForeColor = Color.Gray;
            try { dataGridViewLijsten.Columns[2].Width = 20; } catch { }
            for (int i = 0; i < dataGridViewLijsten.RowCount - 1; i++)
            {
                dataGridViewLijsten[2, i].ReadOnly = true;
            }
            ET.Log("Lijsten LOADED");

            pubLIJSTEN.Clear();
            var _lijsten = dbLijst.Lijstens.OrderBy(x => x.ID).ToList();
            classLijstgegevens lijstje; classSubLijstGegevens sublijst;

            // Voor iedere hoofdlijst
            foreach (string hoofdlijst in _lijsten.Select(x => x.Titel).Distinct())
            {
                ET.Log(hoofdlijst);
                lijstje = new classLijstgegevens();
                lijstje.Titel = hoofdlijst;

                // voor iedere sublijst
                foreach (var _sublijst in _lijsten.Where(x => x.Titel == hoofdlijst).ToList())
                {
                    sublijst = new classSubLijstGegevens();
                    sublijst.SubCode = _sublijst.SubCode;
                    sublijst.SubTitel = _sublijst.SubTitel;
                    sublijst.Managers = _sublijst.Manager;
                    sublijst.ProjectLeiders = _sublijst.ProjectLeider;
                    lijstje.SubLijst.Add(sublijst);
                }
                pubLIJSTEN.Add(lijstje);
            }
            ET.Log("3e aanroep Bouwlijst");
            BouwLijst();
            ET.Log("3e aanroep Bouwlijst");
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            dataGridViewLijsten.EndEdit();
            dbLijst.SubmitChanges();
            Load_Lijsten();


            gbLijst.Visible = false;
            pnlSettings.Visible = false;

        }


    } //
} //

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
using MDR2PDF; // dbLMandagen, ZeebregtsDB
//using Excel = Microsoft.Office.Interop.Excel;
using System.Globalization; // weeknummer
using System.IO;
using System.Configuration; // Fileinfo



namespace MDR2PDF
{

    public partial class USMario : WFControl
    {
        // Algemene public variabelen
        Color ROOD = Color.Red;
        Font FROOD;
        Font FNORMAAL;

        public DateTime EVENWACHTEN;
        public List<string> OLD_SearchstringProjecten = new List<string> { "", "" };
        

        public class EasySettingClass
        {
            public List<string> PeriodeBoxOpties = new List<string>{"jaar", "kwartaal", "maand", "week", "dag","datum", "keuze"};
            public int PeriodeBox = 0; // Default
        }
        public EasySettingClass EasySettings = new EasySettingClass();

        public class PdfLVItems
        {
            public int key;
            public string text;
        }

        // PdfOutput PoortViews
        public List<PoortViewMandagen> pdfMandagen = new List<PoortViewMandagen>();
        public List<PoortViewMandagenNoRel> pdfMandagenNoRel = new List<PoortViewMandagenNoRel>();
        public List<PoortViewProject> pdfProject = new List<PoortViewProject>();
        public List<PoortViewVakman> pdfVakman = new List<PoortViewVakman>();
        public List<PoortViewVakmanNoDefault> pdfVakmanNoDefault = new List<PoortViewVakmanNoDefault>();
        public List<PoortViewBedrijf> pdfBedrijf = new List<PoortViewBedrijf>();
        //public List<relViewVakmandefault> pdfrelVakmanDefault = new List<relViewVakmandefault>();
        //public List<relVakmandefault> pdfrelVakmanDefault = new List<relVakmandefault>();

        // Om snel te kunnen zoeken door de mandagrecords gebruik ik deze lijst; 
        // die is een stuk kleiner dan alle mandagen terwijl voor selectiebox-doeleinden alleen de 
        // unieke combinaties Vakman/project/relaties van belang zijn
        // Werken met deze lijst maakt het verschil tussen 'even moeten wachten' of DIRECT het resultaat zien bij aan of uitvinken van selectie-checkboxjes
        // Als je een jaar of wat aan mandagen selecteert is het nog redelijk te doen. Zonder deze lijst zou het dan NIET te doen zijn!
        public class UniekeMandagCombi
        {
            public int VakmanId;
            public int ProjectId;
            public int ArbeidsrelatieVW;
            public int ContractVW;
            public int FunctieVW;
            public int KetenpartnerVW;
            public int NietBeschikbaarVW;
            public int ProjectleiderId;
        }
        public List<UniekeMandagCombi> pdfUniekeMandagenDezePeriode = new List<UniekeMandagCombi>();


        // PdfOutput Tabellen
        public List<allArbeidsRelaty> pdfAllArbeidsrelatie = new List<allArbeidsRelaty>();
        public List<allContract> pdfAllContract = new List<allContract>();
        public List<allFuncty> pdfAllFunctie = new List<allFuncty>();
        // public List<allKetenpartner> pdfAllKetenpartner = new List<allKetenpartner>();
        // Gebruik voortaan de poortview zodat je de echte bedrijfsnamen gebruikt
        public List<PoortViewKetenpartner> pdfPoortViewKetenpartner = new List<PoortViewKetenpartner>();
        public List<allNietBeschikbaar> pdfAllNietbeschikbaar = new List<allNietBeschikbaar>();
        public List<PoortViewRegistratiebevoegd> pdfAllRegistratiebevoegd = new List<PoortViewRegistratiebevoegd>();

        public class ListViewSelecties
        {
            public const int AlleAangevinkte = 0;
            public const int AlleAangevinkteBehalveGrijs = 1;
            public const int AlleNietAangevinke = 2;
        }

        public MarioPDFSettings PDFSETTINGS = new MarioPDFSettings();
        public bool BACKGROUNDFINNISCHED = false;
        //public List<VakmanRelatiesView> VAKMANNENRELATIELIJST = new List<VakmanRelatiesView>();
        public List<vwVakmanRelaty> VAKMANNENRELATIELIJST = new List<vwVakmanRelaty>();
        public List<vwVakmanRelaty> pVAKMANNENRELATIELIJST = new List<vwVakmanRelaty>();

        public class MANDAGREGISTER
        {
            public int VakmanID = new int();
            public int ProjectID = new int();
            public int ProjectNR = new int();
            public string ProjectNaam = string.Empty;
            public int ProjectleiderID = new int();
            public string ProjectLeiderNaam = string.Empty;
            public string NietBeschikbaar = string.Empty;
        }
        public List<MANDAGREGISTER> pMANDAGREGISTER = new List<MANDAGREGISTER>();

        public string SublijstCode = string.Empty;
        public bool BSN_Tonen = false;

        public class cPROJECT
        {
            public int ProjectID;
            public int ProjectNR;
            public string ProjectNaam;
            public DateTime Min = DateTime.MinValue;
            public DateTime Max = DateTime.MaxValue;
        }
        List<cPROJECT> PROJECTEN = new List<cPROJECT>();

        // Publieke lijst van Vakmannen/Ketenpartners
        public class VakmanBedrijf
        {
            public int VakmanID = 0;
            public int VakmanSoort = -1;
            public int Bedrijfsnummer = 0;
            public string Bedrijfsnaam = string.Empty;
        }
        public List<VakmanBedrijf> VAKMANNENBEDRIJF = new List<VakmanBedrijf>();
        //public List<int> EXTERNEVAKMANIDs = new List<int>();

        // Te gebruiken om de totale periode van projecten en vakmannen op te slaan
        // Slechts éénmaal updaten na overgang van omgeving (gebeurt in Backgroundproces)
        public class ShowAllMinMax
        {
            public int ID = 0;
            public DateTime Min = DateTime.MinValue;
            public DateTime Max = DateTime.MaxValue;
        }
        public List<ShowAllMinMax> VMinMax = new List<ShowAllMinMax>();
        public List<ShowAllMinMax> PMinMax = new List<ShowAllMinMax>();

        // Om te kunnen schakeneln van ShowWeek Aan/uit willen we de oorspronkelijke
        // periode onthouden (De periode vóórdat er automatisch wordt uitgebreid naar ShowWeek)
        public DateTime ShowWeekStart;
        public DateTime ShowWeekEinde;
        public List<ListViewItem> SHowWeekSelectedVakmannen = new List<ListViewItem>();
        public List<ListViewItem> SHowWeekSelectedProjecten = new List<ListViewItem>();


        public class JoinedVakmangegevens
        {
            public Vakman MandagenReg;
            public persoon Zeebregts;
            public string VolledigeNaam;
            public DateTime ShowAllStart;
            public DateTime ShowAllEinde;
        }

        // Deze lijst bevat de (gecombineerde) Vakmangegevens van ALLE VAKMANNEN
        // Die binnen de geselecteerde periode gewerkt hebben.
        // Deze lijst wordt alléén opgebouwd als de PERIODE wijzigt!!!!
        public List<JoinedVakmangegevens> aVAKMANNEN = new List<JoinedVakmangegevens>();

        public class xMANDAG // Very eXtened version of Mandag
        {
            public Mandagen Mandag;
            public JoinedVakmangegevens Vakman;
            public Gebruiker Gebruiker;
            public int Dienstbetrekking; // 1, 3, 5 of 7 ofwel Intern, ZZP, Utzendkracht of Extern
            public Decimal LoonKosten = 0.00M;
        }

        public List<xMANDAG> xMANDAGEN = new List<xMANDAG>(); // aMANDAGEN eXtended (met alle andere gegevens er bij!!!)
        public List<xMANDAG> selxMANDAGEN = new List<xMANDAG>(); // Gebruiker deze list om door te sturen naar betreffende lijsten!

        public class xxMANDAG // NIEUWE eXXtended version of Mandag; nu met gebruik van VakmanRelatietabel en verdere onzin weggelaten
        {
            // Gegevens rechtstreeks uit Mandag
            public int ProjectId;
            public int VakmanId;
            public int ProjectleiderId;
            public DateTime Begintijd;
            public DateTime Eindtijd;
            public int Uren;
            public int Minuten;
            // Extra gegevens
            public vwVakmanRelaty VAKMANRELATIE = new vwVakmanRelaty();
            public cPROJECT PROJECT = new cPROJECT();
            public Decimal LoonKosten = 0.00M;
        }
        public List<xxMANDAG> xxMandagen = new List<xxMANDAG>();

        // Array inclusief loonkosten
        public class xMANDAGplus
        {
            public Mandagen Mandag;
            public JoinedVakmangegevens Vakman;
            public Gebruiker Gebruiker;
            public int Dienstbetrekking; // 1, 3, 5 of 7 ofwel Intern, ZZP, Utzendkracht of Extern
            public Double dagtarief;
            public double Dagloon;
            public double Uurloon;
        }
        public List<xMANDAGplus> selxMANDAGENplus = new List<xMANDAGplus>();

        static string xHtmlFolder = ConfigurationManager.AppSettings["htmlFolder"].ToString();
        public EvoTools ET = new EvoTools(xHtmlFolder);

        // Dit is een slimme!
        // Waneer pSTARTDATUM waar dan ook wordt gewijzigd, dan wordt dus de set-tak hieronder aangeroepen
        // Zodoende wordt bij elke wijziging van pSTARTDATUM of pEINDDATUM variabelen opnieuw ingelezen uit SQL
        // en de labels goed gezet. slimmmm
        DateTime _theVariable = new DateTime();
        public bool pubUpdaten = true; // Als je direct na het setten van Startdatum toch ook Einddatum gaat setten hoef je pas dan te updaten!
        public DateTime NULLDATUM = new DateTime(2001,1,1);
        public string pubNaamPeriodeSelector; // Naam van de periode-radiobutton welke gebruikt is voor de geselecteerde periode

        public DateTime pSTARTDATUM
        {
            get { return _theVariable; }
            set 
            {
                DateTime OldValue = _theVariable;
                _theVariable = value;
                ShowWeekStart = _theVariable; // Onthoud Allereeerst de door de gebruiker bedoelde datum

                if (cbShowAllWeek.Checked)
                {
                    // Indien nodig, (dat weeet de gebruiker in principe helemaal niet) uitbreiden naar start van de week!
                    if (_theVariable != Weekstart(_theVariable))
                        // Note: ShowWeek veranderd hierdoor niet, die is al gezet op de door de verstuurder 'bedoelde' datum
                        _theVariable = Weekstart(_theVariable);
                }
                if (_theVariable != OldValue)
                {
                    if (pSTARTDATUM <= NULLDATUM)
                    {
                        lblVan.Text = "Geen datum geselecteerd";
                        lblVanWeek.Text = "";
                        pEINDEDATUM = pSTARTDATUM;
                        lblTot.Text = "";
                        lblTotWeek.Text = "";
                        // Nieuw: Leeg alle lijsten en zo
                        return;
                    }

                    lblVan.Text = String.Format("{0:D}", pSTARTDATUM);
                    lblVanWeek.Text = "Week " + weekNumber(pSTARTDATUM);

                    /* Dwing af dat deze labels refrechen */
                    lblVan.Invalidate();
                    lblVan.Update();
                    lblVanWeek.Invalidate();
                    lblVanWeek.Update();

                    if (pubUpdaten && BACKGROUNDFINNISCHED)
                    {
                        WijzigPeriode();
                    }
                }
                pubUpdaten = true; // Zodat je nooit kan vergeten om het weer op true te zetten!!!
            }
        }
        DateTime _theVariable2 = new DateTime(2012, 1, 1);
        public DateTime pEINDEDATUM
        {
            get { return _theVariable2; }
            set
            {
                DateTime OldValue = _theVariable2;
                _theVariable2 = value;
                ShowWeekEinde = _theVariable2; // Onthoud ALTIJD de Origine datum die door verstuurder bedoelt was
                if (cbShowAllWeek.Checked)
                {
                    // Indien nodig, uitbreiden naar eind vd week
                    // Note: ShowWeek veranderd hierdoor niet, die is al gezet op de door de verstuurder 'bedoelde' datum
                    if (_theVariable2 != Weekstart(_theVariable2).AddDays(6))
                        _theVariable2 = Weekstart(_theVariable2).AddDays(6);
                }
                if (_theVariable2 != OldValue)
                {
                    lblTot.Text = String.Format("{0:D}", pEINDEDATUM);
                    lblTotWeek.Text = "Week " + weekNumber(pEINDEDATUM) ;

                    /* Dwing af dat deze labels refrechen */
                    lblTot.Invalidate();
                    lblTot.Update();
                    lblTotWeek.Invalidate();
                    lblTotWeek.Update();

                    if (pSTARTDATUM > NULLDATUM) // Anders heeft het toch gen enkele zin!
                    {
                        WijzigPeriode();
                    }
                }
            }
        }

        public string xZeebregtsDBConnectionString = "";//= ConfigurationManager.ConnectionStrings["ZeebregtsDBConnectionString"].ConnectionString;
        public string xMandagenRegistratieConnectionString = ""; // = ConfigurationManager.ConnectionStrings["MandagenRegistratieConnectionString"].ConnectionString;
        public string xPdfOutputConnectieString = "";

        public List<string> pl_MaandStrings = new List<string>(new string[] { "Dummy", "januari", "februari", "maart", "april", "mei", "juni", "juli", "augustus", "september", "oktober", "november", "december" });

        public string PLogFile;

        public USMario()
        {
            // Doe hier iets aan de lelijke WPF styles
            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new frmAddress());

            //EvoTools ET = new EvoTools();
            ET.Log("Start UsMario");
            try
            {
                ET.Log("Start UsMario1");
                InitializeComponent();
            }
            catch  (Exception e )
            {
                MessageBox.Show("Programma kon niet goed opstarten: \n"+e.ToString() );
            }

            ET.Log("OK button = PDF Maken");

            // JURACI: Koppeling met knoppen van Mandagenregistratie
            this.OkClick += OKButton;
            this.PageGereedButtonText = "PDF maken";
            this.PageGereedButtonVisibility = System.Windows.Visibility.Visible;

            ET.Log("OK button = PDF Maken done");

            FROOD = new System.Drawing.Font(
                lvContracten.Font.Name
                , lvContracten.Font.Size
                , System.Drawing.FontStyle.Italic);
            FNORMAAL = new System.Drawing.Font(
                lvContracten.Font.Name
                , lvContracten.Font.Size
                , System.Drawing.FontStyle.Regular);
        }



        /// <summary>
        /// Deze functie wordt uitgevoerd als je bij MANDAGEN op 'Maak PDF' klikt!
        /// </summary>
        public void OKButton()
        {
            ET.Log("Juraci drukt op MAAK PDF");

            // Check of we ruim NA EVENWACHTEN zitten!
            if (EVENWACHTEN.AddMilliseconds(500) > DateTime.Now)
            {
                //MessageBox.Show("haha trappen wij niet in in dat gedubbelklik mister!");
                return;
            }

            EVENWACHTEN = DateTime.Now.AddSeconds(2);

            /*
            // Als je weer een derde pagina zou willen maken is daar hier de plaats voor
            // Hieronder hoe dat dan zou moeten
            USMario2 pagina2 = new USMario2();
            PageGoToPage(pagina2);
            //Nu doen we dat dus nietVan Tot: Dit is nu feitelijk de eindpagina, we gaan nu de PDF maken
             * */

            // Maak de juiste lijst!

            // Selecteer de PdfMandagen die van toepassing zijn!

            ET.Log("Juraci drukt op MAAK PDF - Waitcursor");
            
            // test 2014-06-23
            //this.Cursor = Cursors.WaitCursor;
            this.Cursor = Cursors.WaitCursor;
            Cursor.Current = Cursors.WaitCursor;

            ET.Log("Bevestig");

            ET.Log("Juraci drukt op MAAK PDF Connectiestring: " + xPdfOutputConnectieString);
            dbLPdfOutputDataContext dp = new dbLPdfOutputDataContext(xPdfOutputConnectieString);
            ET.Log("Juraci drukt op MAAK PDF -Zo, dat is gelukt");


            // Eerst eventjes een en ander vernieuwen
            // Vernieuw enige zaken eventjes:
            // Get all Poortviews
            pdfBedrijf = dp.PoortViewBedrijfs.ToList();
            pdfMandagen = dp.PoortViewMandagens.ToList();
            pdfProject = dp.PoortViewProjects.ToList();
            pdfVakman = dp.PoortViewVakmans.ToList();
            //pdfrelVakmanDefault = dp.relVakmandefaults.ToList();
            // Fijn


            // Selecteer in één grote select rechtstreeks vanuit de Database (Gaat veeeel sneller dan uit eigen interne linq list)
            ET.Log("Juraci drukt op MAAK PDF - Select Mandagen");
            var pdfGeselecteerdeMandagen = dp.PoortViewMandagens.Where
                (x =>

                // Arbeidsrelatis
                (
                    lvArbeidsrelaties.CheckedItems.Count == lvArbeidsrelaties.Items.Count
                    ? true 
                    : GetIDs(lvArbeidsrelaties, 0).Contains(x.ArbeidsrelatieVW)
                ) &&

                // Contracten
                (
                    lvContracten.CheckedItems.Count == lvContracten.Items.Count
                    ? true 
                    :GetIDs(lvContracten, 0).Contains(x.ContractVW) 
                ) &&

                // Functies
                (
                    lvFuncties.CheckedItems.Count == lvFuncties.Items.Count
                    ? true 
                    : GetIDs(lvFuncties, 0).Contains(x.FunctieVW) 
                ) &&

                // Ketenpartners
                (
                    lvKetenpartners.CheckedItems.Count == lvKetenpartners.Items.Count
                    ? true 
                    : GetIDs(lvKetenpartners, 0).Contains(x.KetenpartnerVW) 
                ) &&

                // NietBeschikbaar
                (
                    lvNB.CheckedItems.Count == lvNB.Items.Count
                    ? true 
                    : GetIDs(lvNB, 0).Contains(x.NietBeschikbaarVW) 
                ) &&

                // Registratiebevoegden (projectleiders)
                (
                    lvRegistratiebevoegd.CheckedItems.Count == lvRegistratiebevoegd.Items.Count
                    ? true 
                    : GetIDs(lvRegistratiebevoegd, 0).Contains(x.ProjectleiderId) 
                ) &&

                // ShowAll //
                (cbShowAllVan.Checked ? true : x.Begintijd >= pSTARTDATUM) &&
                (cbShowAllTot.Checked ? true : x.Begintijd <= pEINDEDATUM) &&

                
                // Projecten OF Vakmannan
                ( 
                    (
                    (tabControlProjecten.SelectedIndex == 0) && GetIDs(lvProjecten, 1).Contains(x.ProjectId)
                    ) ||
                    (
                    (tabControlProjecten.SelectedIndex == 1) && GetIDs(lvVakmannen, 1).Contains(x.VakmanId)
                    )
                )

                ).ToList();

            ET.Log("Juraci drukt op MAAK PDF - Mandagen selected");
            ET.Log("Juraci drukt op MAAK PDF - Mandagen aantal:" + pdfGeselecteerdeMandagen.Count.ToString());

            if (pdfGeselecteerdeMandagen.Count == 0)
            {
                if ((tabControlProjecten.SelectedIndex == 0) && GetIDs(lvProjecten, 1).Count == 0)
                    MessageBox.Show("U heeft geen enkel (geldig) project geselecteerd");
                else if ((tabControlProjecten.SelectedIndex == 1) && GetIDs(lvVakmannen, 1).Count == 0)
                    MessageBox.Show("U heeft geen enkele (geldige) vakman geselecteerd");
                else
                    MessageBox.Show("Geen enkele mandag voldoet aan de gekozen criteria. Pas de selecties aan");
                this.Cursor = Cursors.Default;
                Cursor.Current = Cursors.Default;
                return;
            }

            // Controleer of er niet-geselecteerde vakmannen in de lijst voorkomen
            pdfMandagenNoRel = dp.PoortViewMandagenNoRels.ToList();
            Controleer_VakmanRelaties(dp.PoortViewMandagenNoRels.ToList(), pdfGeselecteerdeMandagen, dp.PoortViewVakmanNoDefaults.ToList());

            ET.Log("Juraci drukt op MAAK PDF - Mandagen aantal:" + pdfGeselecteerdeMandagen.Count.ToString());

            DateTime mindag = pdfGeselecteerdeMandagen.Select(x => x.Begintijd).Min().Date;//  selxMANDAGEN.Select(x => x.Mandag.Begintijd).Min().Date;
            DateTime maxdag = pdfGeselecteerdeMandagen.Select(x => x.Begintijd).Max().Date;//  selxMANDAGEN.Select(x => x.Mandag.Eindtijd).Max().Date;

            // Controleer bij de '8 serie dat de combinatie Productie = Intern (1), Contract = Per meter (2) NIET voorkomt!
            var _InternPerMeter = pdfGeselecteerdeMandagen.Where(x => x.ProductieVW == 1 && x.ContractVW ==2).Select(x => x.VakmanId).Distinct().ToList();
            if (_InternPerMeter.Count() > 0)
            {
                string melding = "Er zijn vakmannen met Productie = Intern / Contract = Per meter. \n Dit mag niet voorkomen, de lijst is dus NIET volledig!";
                melding += "\n Het gaat om de volgende vakmannen:";
                foreach (int FouteVakmanID in _InternPerMeter)
                {
                    var _FouteMan = pdfVakman.Where(x => x.VakmanId == FouteVakmanID).Select(x => x.Naam).FirstOrDefault();
                    if (_FouteMan == null || _FouteMan == string.Empty)
                        melding += "\nOnbekende vakman met vakmanID = " + FouteVakmanID.ToString();
                    else
                        melding += "\n" + _FouteMan;
                }
                MessageBox.Show(melding);
            }
            switch (SublijstCode.ToLower())
            {
                case "1a":
                    Lijst1a LIJST1a = new Lijst1a();
                    LIJST1a.Maak_Lijst(pSTARTDATUM, pEINDEDATUM, pdfGeselecteerdeMandagen, pdfAllArbeidsrelatie, pdfAllRegistratiebevoegd, pdfProject, pdfBedrijf, pdfVakman, LijstGegevens());
                    LIJST1a.Dispose();
                    break;
                case "1b":
                case "1bbsn":
                    // Bij Lijst 1b gaat het alleen om Externen, dus controleer of er sprake is van mandagrecords met externen
                    //int Extern = pdfAllContract.Where(y => y.ContractNaam.ToLower().Contains("extern")).Select(x => x.ContractID).FirstOrDefault();
                    //var pdfGeselecteerdeMandagen_Externen = pdfGeselecteerdeMandagen.Where(x => x.ContractVW == Extern).ToList();
                    //if (pdfGeselecteerdeMandagen_Externen.Count == 0)
                    //{
                    //    MessageBox.Show("U heeft geen Externen geselecteerd, Weeklijst Externen heeft geen zin");
                    //    break;
                    //}
                    _1b_WeeklijstExternen Lijst1b = new _1b_WeeklijstExternen(SublijstCode);
                    Lijst1b.Maak_Lijst(pSTARTDATUM, pEINDEDATUM, pdfGeselecteerdeMandagen, pdfAllArbeidsrelatie, pdfAllRegistratiebevoegd, pdfProject, pdfBedrijf, pdfVakman, LijstGegevens(), false);
                    Lijst1b.Dispose();
                    break;
                case "1c": // We gebruiken de standaard lijst1b, maar dan met uitgebreide cakman-nfo (naam, adres etc) in vooraf-tabel
                case "1cbsn":
                case "1czap":
                    _1b_WeeklijstExternen Lijst1c = new _1b_WeeklijstExternen(SublijstCode);
                    Lijst1c.Maak_Lijst(pSTARTDATUM, pEINDEDATUM, pdfGeselecteerdeMandagen, pdfAllArbeidsrelatie, pdfAllRegistratiebevoegd, pdfProject, pdfBedrijf, pdfVakman, LijstGegevens(),true);
                    Lijst1c.Dispose();
                    break;
                case "8a":
                    _8a_pr_wk_pr_prd Lijst8a = new _8a_pr_wk_pr_prd();
                    Lijst8a.Maak_Lijst(mindag, maxdag, pdfGeselecteerdeMandagen, pdfAllContract, LijstGegevens());
                    Lijst8a.Dispose();
                    break;
                case "8b":
                    try {
                        _8b_pr_wk_pr_utvrdr_pr_prd Lijst8b = new _8b_pr_wk_pr_utvrdr_pr_prd();
                        Lijst8b.Maak_Lijst(mindag, maxdag, pdfGeselecteerdeMandagen, pdfAllRegistratiebevoegd, pdfAllContract, LijstGegevens());
                        Lijst8b.Dispose();
                        } catch (Exception e)
                        {
                            MessageBox.Show("Er is iets misgegaan in module 8b. Info voor Marino: Current Dir = [" + Environment.CurrentDirectory + "] message = " +  e.Message);
                        }
                    break;
                case "8c":
                    _8c_LK_pr_wk_pr_project_pr_prd Lijst8c = new _8c_LK_pr_wk_pr_project_pr_prd();
                    Lijst8c.Maak_Lijst(mindag, maxdag, pdfGeselecteerdeMandagen, pdfProject, pdfAllContract, LijstOpties(), pSTARTDATUM, pEINDEDATUM, LijstGegevens());
                    Lijst8c.Dispose();
                    break;
                case "8d":
                    _8d_pr_wk_pr_vakman_pr_prd Lijst8d = new _8d_pr_wk_pr_vakman_pr_prd();
                    Lijst8d.Maak_Lijst(mindag, maxdag, pdfGeselecteerdeMandagen, pdfVakman, pdfAllContract, LijstOpties(), pSTARTDATUM, pEINDEDATUM, LijstGegevens());
                    Lijst8d.Dispose();
                    break;
                case "8e":
                    _8e_pr_wk_pr_ketenp_pr_prd Lijst8e = new _8e_pr_wk_pr_ketenp_pr_prd();
                    Lijst8e.Maak_Lijst(mindag, maxdag, pdfGeselecteerdeMandagen, pdfPoortViewKetenpartner , pdfAllContract, LijstOpties(), pSTARTDATUM, pEINDEDATUM, LijstGegevens());
                    Lijst8e.Dispose();
                    break;
                //case "4":
                //    if (lblSubform.Text.Contains("NAW")) // oftewel: lijst4b!
                //    {
                //        _4b_vkm_pr_prd Lijst4b = new _4b_vkm_pr_prd();
                //        Lijst4b.Maak_Lijst(pSTARTDATUM, pEINDEDATUM, selxMANDAGEN, cbClipboard.Checked);
                //        Lijst4b.Dispose();
                //    }
                //    else
                //    {
                //        _4_Intrn_vkm_pr_prd Lijst4 = new _4_Intrn_vkm_pr_prd();
                //        Lijst4.Maak_Lijst(pSTARTDATUM, pEINDEDATUM, selxMANDAGEN);
                //        Lijst4.Dispose();
                //    }
                //    break;
                //case "5":
                //    _5_prjctn_pr_utvrdr_pr_prd Lijst5 = new _5_prjctn_pr_utvrdr_pr_prd();
                //    Lijst5.Maak_Lijst(pSTARTDATUM, pEINDEDATUM, selxMANDAGEN);
                //    Lijst5.Dispose();
                //    break;
                default:
                    MessageBox.Show(string.Format("Onbekende Lijst '{0}' (lijstcode {1}) \nControleer uw instellingen in tabel 'Lijsten'",LijstGegevens().LijstNaam, SublijstCode));
                    break;
            }

            this.Cursor = Cursors.Default;
            Cursor.Current = Cursors.Default;
            //EVENWACHTEN = DateTime.Now;
        }

        private EvoTools.HeaderFooter LijstGegevens()
        {
            EvoTools.HeaderFooter LG = new EvoTools.HeaderFooter();

            // LijstNaam
            // 2014-11-28 RFC: Pak liever de hoofdtitel
            //LG.LijstNaam = this.PageSubtitle ?? this.PageTitle;
            //LG.LijstNaam = this.PageTitle;
            LG.LijstNaam = PDFSETTINGS.Lijstgegevens.Titel;
            LG.LijstCode = "d" + DateTime.Now.ToString("yyMMdd") + "t" + DateTime.Now.ToString("HHmmss") + "r" + SublijstCode;

            // Periode
            LG.UI_Startdatum = pSTARTDATUM;
            LG.UI_Einddatum = pEINDEDATUM;

            string Settings = string.Empty;

            // projecten of Vakmannen Selectie
            if (tabControlProjecten.SelectedTab.Text == "Projecten")
            {
                Settings += string.Format("{0} projecten geselecteerd", lvProjecten.CheckedItems.Count);
                if (lvProjecten.CheckedItems.Count == pdfUniekeMandagenDezePeriode.Select(x => x.ProjectId).Distinct().Count()) // lvProjecten.Items.Count)
                    Settings += " (allen aangevinkt)";
                if (lvProjecten.CheckedItems.Count < 4)
                {
                    string before = " (";
                    foreach (ListViewItem LI in lvProjecten.CheckedItems)
                    { Settings += before + LI.SubItems[1].Text; before = ", "; }
                    Settings += ")";
                }
            }
            else
            {
                Settings += string.Format("{0} {1} geselecteerd", lvVakmannen.CheckedItems.Count
                    , lvVakmannen.CheckedItems.Count == 1 ? "vakman" : "vakmannen");
                if (lvVakmannen.CheckedItems.Count == pdfUniekeMandagenDezePeriode.Select(x => x.VakmanId).Distinct().Count())//  lvVakmannen.Items.Count)
                    Settings += " (allen aangevinkt)";
                if (lvVakmannen.CheckedItems.Count < 4)
                {
                    string before = " (";
                    foreach (ListViewItem LI in lvVakmannen.CheckedItems)
                    { Settings += before + LI.SubItems[0].Text; before = ", "; }
                    Settings += ")";
                }
            }
            if (Settings.Length > 0)
                LG.UISettings.Add(new List<string> { "Selectie:", Settings });

            return LG;
        }

        private List<string> LijstOpties()
        {
            List<string> lijstopties = new List<string>();
            string opties = "";

            // Instellingen
            //if (cbFriendlyUserInterface.Checked)
            //    opties += "Friendly User interface ";
            if (cbShowAllWeek.Checked)
                opties += "+'Hele week' ";
            //if (cbShowAllPeriode.Checked)
            //    opties += "+'Toon de hele periode' ";
            if (cbShowAllTot.Checked && cbShowAllVan.Checked)
            {
                opties += "+'Toon de hele periode' ";
            }
            else
            {
                if (cbShowAllTot.Checked)
                    opties += "+'Toon ook eerdere periode' ";
                if (cbShowAllVan.Checked)
                    opties += "+'Toon ook latere periode' ";
            };

            lijstopties.Add(opties);
            opties = "";

            // projecten of Vakmannen
            if (opties.Length > 1)
                opties += "<br>";
            if (tabControlProjecten.SelectedTab.Text == "Projecten")
            {
                opties += string.Format("{0} projecten geselecteerd", lvProjecten.CheckedItems.Count);
                if (lvProjecten.CheckedItems.Count == lvProjecten.Items.Count)
                    opties += " (allen aangevinkt)";
                if (lvProjecten.CheckedItems.Count < 4)
                {
                    string before = " (";
                    foreach (ListViewItem LI in lvProjecten.CheckedItems)
                    { opties += before + LI.SubItems[1].Text; before = ", "; }
                    opties += ")";
                }
            }
            else
            {
                opties += string.Format("{0} {1} geselecteerd", lvVakmannen.CheckedItems.Count
                    , lvVakmannen.CheckedItems.Count == 1 ? "vakman" : "vakmannen");
                if (lvVakmannen.CheckedItems.Count == lvVakmannen.Items.Count)
                    opties += " (allen aangevinkt)";
                if (lvVakmannen.CheckedItems.Count < 4)
                {
                    string before = " (";
                    foreach (ListViewItem LI in lvVakmannen.CheckedItems)
                    { opties += before + LI.SubItems[0].Text; before = ", "; }
                    opties += ")";
                }
            }

            lijstopties.Add(opties);
            return lijstopties;
        }

        private void EasySet()
        {
            // Picturebox
            HideAll.Visible = true;
            HideAll.Show();


            //tlpRechts.Visible = false;
            //tlpHoofd.Visible = true;
            //tlpLinks.SuspendLayout();


            ET.Log("EasySet Start");
            if (SublijstCode == string.Empty)
                return;

            Easy easy = new Easy();
            string gebruiker = EasyUser.SelectedItem.ToString(); //(cbProjectLeider.Checked ? "Gebruiker" : "Admin");
            try
            {
                dbLPdfOutputDataContext pd = new dbLPdfOutputDataContext(xPdfOutputConnectieString);
                easy = pd.Easies.Where(x => x.Lijstcode == SublijstCode && x.Gebruiker == gebruiker).FirstOrDefault();
            } catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            //tlpHoofd.SuspendLayout();

            // Per lijst aangeven welke instellingen geldig zijn (Sla UsrFriendly over; die wordt bij binnenkomst bepaald!)
            foreach (Control X in gbInstellingen.Controls)
                //if (!X.Name.ToUpper().Contains("FRIENDLY"))
                    X.Visible = false;

            // Maak Sublijsten aan
            foreach (Control rb in gbLijsten.Controls)
                rb.Visible = false;

            // Ovrige velden initialiseren
            tbSearch.Visible = false;
            tbSearchClick.Visible = true;
            tbSearch.Text = "";
            picStopSearch.Visible = false;


            ET.Log("EasySet Maak lijsten");
            int sublijst = 0;
            foreach (RadioButton rb in gbLijsten.Controls.Cast<RadioButton>().OrderBy(c => c.TabIndex))
            {
                if (sublijst < PDFSETTINGS.Lijstgegevens.SubLijst.Count)
                {
                    //if (sublijst == 0)
                    //if (PDFSETTINGS.Lijstgegevens.Code == PDFSETTINGS.Lijstgegevens.SubLijst[sublijst].SubCode)
                    if (SublijstCode == PDFSETTINGS.Lijstgegevens.SubLijst[sublijst].SubCode)
                        rb.Checked = true;
                    rb.Text = PDFSETTINGS.Lijstgegevens.SubLijst[sublijst++].SubTitel;//[sublijst++];
                    // Wordwrapping if nessecary (quick but durty method!!!!)
                    if (rb.Text.Length > 40)
                    {
                        int nl = rb.Text.IndexOf(" ", 30);
                        rb.Text = rb.Text.Substring(0, nl) + "\n" + rb.Text.Substring(nl + 1);
                    }
                    rb.Visible = true;
                }
                else
                    break;
            }

            // Indien 'Only UserFriendly settings' dan alleen onderstaande uitvoeren
            string LC = SublijstCode;// PDFSETTINGS.Lijstgegevens.Code;

            // EASY SETTINGS TOEPASSEN
            if (easy == null)
            {
                MessageBox.Show(string.Format("Geen easy settings gevonden voor lijst {0} met gebruiker {1}", SublijstCode, gebruiker));
                // Standaard settings gebruiken
                easy = new Easy();
                easy.Periode = "DEFAULT";
                easy.PV = 'P';
                easy.SelectAll_ArbeidRel = true;
                easy.SelectAll_Besch= true;
                easy.SelectAll_Contract= true;
                easy.SelectAll_Functies = true;
                easy.SelectAll_KetenP = true;
                easy.SelectAll_RegBev = true;
                EasyME.Checked = false;
            }

            // Hele week
            if (true)
            {
                cbShowAllWeek.Visible = true;
                if (easy.ShowAll_Week)
                    cbShowAllWeek.Checked = true;
                else
                    cbShowAllWeek.Checked = false;
            }

            // UserFriendly
            //cbFriendlyUserInterface.Visible = (PDFSETTINGS.Gebruiker.IsAdministrator);
            llbEasy.Visible = (PDFSETTINGS.Gebruiker.IsAdministrator);


            //SHOWALL
            cbShowAllVan.Checked = !cbShowAllVan.Checked; // reset
            cbShowAllVan.Checked = easy.ShowAllVan_PV;
            cbShowAllTot.Checked = easy.ShowAllTot_PV;


            tlpHoofd.Visible = true; 
            // PERIODE
            DateTime NU = DateTime.Now.Date;
            // reset datum
            //pSETDATA(new DateTime(1963, 07, 28), new DateTime(1963, 07, 28));
            // Reset huidige periodecheck (zorg dat willekeurige check anders gezet wordt)
            rbWeek.Checked = !rbWeek.Checked;
            switch (easy.Periode.Trim())
            {
                case "Jaar":
                    rbJaar.Checked = true;
                    cbbJaar.SelectedIndex = -1;
                    cbbJaar.SelectedIndex = Math.Max(0,Math.Min(cbbJaar.Items.Count - 1,(easy.PeriodeOffset + PeriodeSelectieNU[0]))); // Offset tov 'NU'
                    break;
                case "Kwartaal":
                    rbKwartaal.Checked = true;
                    cbbKwartaal.SelectedIndex = -1;
                    cbbKwartaal.SelectedIndex = Math.Max(0,Math.Min(cbbKwartaal.Items.Count - 1,(easy.PeriodeOffset + PeriodeSelectieNU[1]))); // Offset tov 'NU';
                    break;
                case "Maand":
                    rbMaand.Checked = true;
                    cbbMaand.SelectedIndex = -1;
                    cbbMaand.SelectedIndex = Math.Max(0,Math.Min(cbbMaand.Items.Count - 1,(easy.PeriodeOffset + PeriodeSelectieNU[2]))); // Offset tov 'NU';
                    break;
                case "Week":
                    rbWeek.Checked = true;
                    cbbWeek.SelectedIndex = -1;
                    cbbWeek.SelectedIndex = Math.Max(0,Math.Min(cbbWeek.Items.Count - 1,(easy.PeriodeOffset + PeriodeSelectieNU[3]))); // Offset tov 'NU';
                    break;
                case "Dag":
                    rbDag.Checked = true;
                    cbbDag.SelectedIndex = -1;
                    cbbDag.SelectedIndex = Math.Max(0,Math.Min(cbbDag.Items.Count - 1,(easy.PeriodeOffset + PeriodeSelectieNU[4]))); // Offset tov 'NU';
                    break;
                case "Datum":
                    rbDatum.Checked = true;
                    pSTARTDATUM = Convert.ToDateTime(easy.Van);
                    pEINDEDATUM = Convert.ToDateTime(easy.Tot);
                    break;
                case "Periode":
                    rbAnders.Checked = true;
                    pSTARTDATUM = Convert.ToDateTime(easy.Van);
                    pEINDEDATUM = Convert.ToDateTime(easy.Tot);
                    break;
                default:
                    rbMaand.Checked = true;
                    int i = cbbMaand.SelectedIndex;
                    cbbMaand.SelectedIndex = -1;
                    cbbMaand.SelectedIndex = i;
                    break;
            }

            // Vanaf nu weer visible, dat werkt redelijk goed
            //tlpHoofd.Visible = true; 
            //tlpHoofd.Visible = false; 
            tlpLinks.ResumeLayout();

            // CONTRACT
            if (!(cbAllContracten.Checked = easy.SelectAll_Contract))
                SetEasySubselecties(
                    easy.Selecties_Contract,
                    lvContracten,
                    pdfAllContract.Where(x => SelectieList(easy.Selecties_Contract).Contains(x.ContractID)).Select(x => new PdfLVItems { key = x.ContractID, text = x.ContractNaam }).ToList()
                    );


            // ARBEIDSRELATIES
            if (!(cbAllArbeidsrelaties.Checked = easy.SelectAll_ArbeidRel))
                SetEasySubselecties(
                    easy.Selecties_ArbeidRel,
                    lvArbeidsrelaties,
                    pdfAllArbeidsrelatie.Where(x => SelectieList(easy.Selecties_ArbeidRel).Contains(x.ArbeidsRelatieID)).Select(x => new PdfLVItems { key = x.ArbeidsRelatieID, text = x.ArbeidsRelatieNaam }).ToList()
                    );

            // KETENPARTNER
            if (!(cbAllKetenpartners.Checked = easy.SelectAll_KetenP))
                SetEasySubselecties(
                    easy.Selecties_KetenP,
                    lvKetenpartners,
                    pdfPoortViewKetenpartner.Where(x => SelectieList(easy.Selecties_KetenP).Contains(x.KetenpartnerID)).Select(x => new PdfLVItems { key = x.KetenpartnerID, text = x.Bedrijfsnaam }).ToList()
                    );

            // FUNCTIES
            if (!(cbAllFuncties.Checked = easy.SelectAll_Functies))
                SetEasySubselecties(
                    easy.Selecties_Functies,
                    lvFuncties,
                    pdfAllFunctie.Where(x => SelectieList(easy.Selecties_Functies).Contains(x.FunctieID)).Select(x => new PdfLVItems { key = x.FunctieID, text = x.FuncieOmschrijving}).ToList()
                    );

            // BESCHIKBAAR
            if (!(cbAllNietBeschikbaar.Checked = easy.SelectAll_Besch))
                SetEasySubselecties(
                    easy.Selecties_Besch,
                    lvNB,
                    pdfAllNietbeschikbaar.Where(x => SelectieList(easy.Selecties_Besch).Contains(x.NBID)).Select(x => new PdfLVItems { key = x.NBID, text = x.NBOmschrijving}).ToList()
                    );


            // set EASYME en EASYUSER
            // Indien ME = true, dan huidige gebruiker toevoegen aan selectie
            EasyME.Checked = (easy.Selecties_RegBev != null && SelectieList(easy.Selecties_RegBev).Contains(0));
            if (EasyME.Checked)
            {
                easy.Selecties_RegBev += "," + PDFSETTINGS.Gebruiker.ProjectleiderId.ToString();
            }

            tlpHoofd.Visible = true;

            // REGISTRATIEBEVOEGD (uitvoerders)
            cbAllUitvoerders.Checked = easy.SelectAll_RegBev;
            if (!(cbAllUitvoerders.Checked = easy.SelectAll_RegBev))
                SetEasySubselecties(
                    easy.Selecties_RegBev,
                    lvRegistratiebevoegd,
                    pdfAllRegistratiebevoegd.Where(x => SelectieList(easy.Selecties_RegBev).Contains(x.ProjectleiderId ?? 0)).Select(x => new PdfLVItems { key = (x.ProjectleiderId ?? 0), text = x.Gebruikersnaam}).ToList()
                    );

            //PROJECTEN of VAKMANNEN
            //GetUniekeMandagenDezePeriode();
            //VulProjectenOfVakmannenList();
            if (easy.PV == 'P')
            {
                tabControlProjecten.SelectedIndex = 0;
                cbAllProjecten.Checked = easy.SelectAll_PV;
                SetEasySubselecties(
                    easy.Selecties_PV,
                    lvProjecten,
                    new List<PdfLVItems>()
                   );
            }
            else
            {
                tabControlProjecten.SelectedIndex = 1;
                cbAllVakmannen.Checked = easy.SelectAll_PV;
                SetEasySubselecties(
                    easy.Selecties_PV,
                    lvVakmannen,
                    new List<PdfLVItems>()
                   );
            }
            // Zorg er voor dat alle vakmannen geselecteerd zijn indien vinkje aanstond
            if (easy.SelectAll_PV)
                    CheckOrUncheck_Lijst(easy.PV == 'P' ? cbAllProjecten.Name : cbAllVakmannen.Name );

            
            // Instellingen mooi positionren
            ControlsUitlijnen(gbLijsten);
            ControlsUitlijnen(gbInstellingen);

            // Easy link iets meer naar links zodat ie netjes uitlijnt met de checkboxen
            llbEasy.Location = new Point(17, llbEasy.Location.Y);

            // subselcties grijs maken
            Maak_ListboxItems_Grijs();

            tlpLinks.Visible = true;
            tlpRechts.Visible = true;
            //tlpHoofd.ResumeLayout();

            //tlpHoofd.ResumeLayout();
            this.Cursor = Cursors.Default;

            HideAll.Visible = false;

            return;
        }

        private List<int> SelectieList(string SelectieString)
        {
            // Ga robuust om met selectiestring
            if (SelectieString == string.Empty || SelectieString == null) 
                return new List<int> { };

            SelectieString = SelectieString.Replace(" ","");
            if (SelectieString.Last() == ',')
                SelectieString = SelectieString.Substring(0, SelectieString.Length - 1);
            SelectieString = SelectieString.Replace(",,", ",");

            try
            {
                return SelectieString.Split(',').Select(s => Convert.ToInt32(s.Trim())).ToList();
            }catch(Exception e)
            {
                MessageBox.Show("Selectiestring "+ SelectieString + "voldoet niet aan minimale eisen");
                return new List<int> { };
            }
        }

        private void SetEasySubselecties(string Lijst, ListView lvORG, List<PdfLVItems> lvSelectON)
        {
            //if (Lijst == string.Empty) return;

            try
            {
                //var SELECTEDVALUESINT = Selecties.Split(',').Select(s => Convert.ToInt64(s.Trim())).ToList();

                if (!(lvORG.Name.Contains("listview"))) // ListviewProjecten en ListviewVakmannen overslaan!
                {
                    Add_Red_Items(lvORG, lvSelectON);
                    adjustListSize(lvORG);
                }

                var AANVINKEN = SelectieList(Lijst);

                // Aan te vinken selecties aanvinken
                int ID = lvORG.Columns.Count - 1; // Laatste subitem bevat het ID van de vakman
                foreach (ListViewItem lvItem in lvORG.Items)
                {
                    lvItem.Checked = (AANVINKEN.Contains(Convert.ToInt32(lvItem.SubItems[ID].Text)));
                        //BOXWAARDEN.Contains(Convert.ToInt32(lvItem.SubItems[1].ToString()));
                }

            }catch
            { return; }
        }

        private void ControlsUitlijnen(GroupBox GB)
        {
            Point P = new Point(); // pnlInstellingen.Location;
            P.X = 20; P.Y = 30;
            foreach (Control C in GB.Controls.Cast<Control>().OrderBy(c=> c.TabIndex))
                if (C.Visible)//&& !C.Name.ToUpper().Contains("FRIENDLY"))
                {
                    C.Location = P;
                    P.Y += 10 + C.Height; // hou rekening met de hoogte (bv bij 2 regeels hoger dan bij 1!
                }
        }

        private Boolean UserInterface_Settings()
        {
            ET.Log("UserInterface_Settings");

            string windowsentity = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            ET.Log("UserInterface_Settings1a : "+ xMandagenRegistratieConnectionString );

            if (xMandagenRegistratieConnectionString == null || xMandagenRegistratieConnectionString.Length < 10)
                xMandagenRegistratieConnectionString = ConfigurationManager.ConnectionStrings["MandagenRegistratieConnectionStringLIVE"].ConnectionString;

            Gebruiker geb = new Gebruiker();
            try
            {
                    dbLMandagenDataContext dm = new dbLMandagenDataContext(xMandagenRegistratieConnectionString);
                    geb = dm.Gebruikers.Where(x => x.Windowsidentity.ToUpper() == windowsentity.ToUpper()).FirstOrDefault();
            }
            catch (Exception e)
            {
                ShowException("Userinterface_Settings: user " + windowsentity.ToUpper(),e);
                return false;
            }

            if (geb == null || !geb.CanPrint || !(geb.IsProjectleider || geb.IsManager))
            {
                string Reden = "U bent geen Projectleider of Manager";
                if (geb == null)
                    Reden += windowsentity + " is geen geregistreerde gebruiker";
                MessageBox.Show("U bent niet bevoegd om van deze tool gebruik te makan \rReden:\r"+ Reden);
                return false;
            }

            return true;
        }

        private void ShowException(string opmerking, Exception e)
        {
            MessageBox.Show(string.Format("Foutmelding ({0}), result: {1}\n----\n{2}",opmerking,e.HResult,e.ToString()));
        }

        /// <summary>
        /// Haal Namen van Uitvoerders op en pas Datum-Pick-velden aan aan omgeving
        /// </summary>
        private void initDatumBoxenEnUitvoerdersLijst()
        {
            //if (cbbJaar.Items.Count > 0 && pSTARTDATUM <= NULLDATUM) // Indin reeds gevuld; overslaan!
            //    return;
            ET.Log("initDatumBoxenEnUitvoerdersLijst: "+ xMandagenRegistratieConnectionString);

            //MessageBox.Show("Ophalen Uitvoerders");
            // LET OP: DE listviews moeten nu ook geleegd worden!!!
            // want we willen nu alles initialiseren
            lvProjecten.Items.Clear();
            lvVakmannen.Items.Clear();
            pubNaamPeriodeSelector = "";

            //
            //
            // Vul alle datum-pick comboboxen met geldige data; gesorteerd van boven naar beneden!
            //

            ET.Log("goto: Data_Pick_Boxen");
            //Vul_Data_Pick_Boxen(db.Mandagens.Where(x => x.Begintijd != x.Eindtijd && x.Status == true).Select(x => x.Begintijd.Date).Distinct().ToList());
            Vul_Data_Pick_Boxen(pdfMandagen.Select(x => x.Begintijd.Date).Distinct().ToList()); //2014-04-29
        }

        public List<int> PeriodeSelectieNU = new List<int> {1,1,1,1,1 }; // periodekeuze-posities van juidig {Jaar, Kwartaal, Maand, Week, Dag}
        private void Vul_Data_Pick_Boxen(List<DateTime> _Dagen)
        {
            PeriodeSelectieNU = new List<int> { 0,0,0,0,0 }; // periodekeuze-posities van juidig {Jaar, Kwartaal, Maand, Week, Dag}
            pubNaamPeriodeSelector = "";
            // Jaar-box
            var Jaren = _Dagen.Select(x => x.Year).Distinct().ToList();

            cbbJaar.Items.Clear();
            foreach (int jaar in Jaren.OrderByDescending(x=>x))
            {
                cbbJaar.Items.Add(jaar.ToString());
                // Bepaal positie van 'vandaag' (nodig om later easysave stand tov 'NU' te bepalen)
                if (DateTime.Now.Year <= jaar)
                    PeriodeSelectieNU[0] = cbbJaar.Items.Count - 1;

                // Zet standaard jaar-keuze op huidig jaar
                if (jaar < DateTime.Now.Year)
                {
                    int i = cbbJaar.Items.Count - 2;
                    if (i > -1 && cbbJaar.SelectedIndex == -1)
                    {
                        cbbJaar.SelectedIndex = i;
                        cbbJaar.SelectedItem = cbbJaar.Items[i];//.Items.Count - 1;
                    }
                }
            }
            //cbbJaar.SelectedIndex = -1;

            // maanden + kwartalen
            var Maanden = _Dagen.Select(x => new { x.Year, x.Month }).Distinct().OrderByDescending(x => x.Year).ThenByDescending (x => x.Month).ToList();
            cbbMaand.Items.Clear();
            cbbKwartaal.Items.Clear();

            foreach (var jaarmaand in Maanden)
            {
                cbbMaand.Items.Add(string.Format("{0} - {1}", jaarmaand.Year, pl_MaandStrings[jaarmaand.Month]));
                // Zet standaard maand-keuze op huidige maand
                if (new DateTime(jaarmaand.Year,jaarmaand.Month,1) < new DateTime(DateTime.Now.Year, DateTime.Now.Month,1))
                {
                    int i = cbbMaand.Items.Count - 2;
                    if (i > -1 && cbbMaand.SelectedIndex == -1)
                    {
                        cbbMaand.SelectedIndex = i;
                        cbbMaand.SelectedItem = cbbMaand.Items[i];//.Items.Count - 1;
                    }
                }
                // Bepaal positie van 'vandaag' (nodig om later easysave stand tov 'NU' te bepalen)
                if (DateTime.Now.Year <= jaarmaand.Year && DateTime.Now.Month <= jaarmaand.Month)
                    PeriodeSelectieNU[2] = cbbMaand.Items.Count - 1;
                // Bepaal meteen het bijbehorende kwartaal
                int kwartaalnr = (jaarmaand.Month-1)/3 + 1;
                string kwartaal = string.Format("{0} - kwartaal {1}", jaarmaand.Year, kwartaalnr);
                if (!cbbKwartaal.Items.Contains(kwartaal))
                {
                    cbbKwartaal.Items.Add(kwartaal);
                    // Bepaal positie van 'vandaag' (nodig om later easysave stand tov 'NU' te bepalen)
                    if (DateTime.Now.Year <= jaarmaand.Year && DateTime.Now.Month <= jaarmaand.Month)
                        PeriodeSelectieNU[1] = cbbKwartaal.Items.Count - 1;

                    // Zet standaard kwartaal-keuze op huidig kwartaal
                    if (new DateTime(jaarmaand.Year, jaarmaand.Month, 1) < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1) && kwartaalnr != ((DateTime.Now.Month-1)/3 + 1))
                    {
                        int i = cbbKwartaal.Items.Count - 2;
                        if (i > -1 && cbbKwartaal.SelectedIndex == -1)
                        {
                            cbbKwartaal.SelectedIndex = i;
                            cbbKwartaal.SelectedItem = cbbKwartaal.Items[i];//.Items.Count - 1;
                        }
                    }

                }
            }
            //cbbKwartaal.SelectedIndex = -1;
            //cbbJaar.SelectedItem = -1;

            // Weken en dagen
            cbbWeek.Items.Clear();
            cbbDag.Items.Clear();

            //var __DagenOrderTest = _Dagen.Where(x => x >= new DateTime(1015, 07, 01) && x <= new DateTime(2015, 07, 31)).Union(_Dagen);
            foreach (DateTime dag in _Dagen.OrderByDescending(x=>x))
            {
                cbbDag.Items.Add(string.Format("{0:yyyy-MM-dd}",dag));

                // Zet standaard dag keuze op huidige datum
                if (dag.Date < DateTime.Now.Date)
                {
                    int i = cbbDag.Items.Count - 2;
                    if (i > -1 && cbbDag.SelectedIndex == -1)
                    {
                        cbbDag.SelectedIndex = i;
                        cbbDag.SelectedItem = cbbDag.Items[i];//.Items.Count - 1;
                    }
                }
                int weeknr = weekNumber(dag);
                // Bepaal positie van 'vandaag' (nodig om later easysave stand tov 'NU' te bepalen)
                if (DateTime.Now.Date <= dag.Date)
                    PeriodeSelectieNU[4] = cbbDag.Items.Count - 1;
                // Let op! als weeknummer bij een ander jaar hoort (bv 2014-12-31 is week 1 van 2015!)
                int jaar = dag.Year ;
                if (weeknr < 3 && dag.Month == 12) jaar++;
                // Maar ook andersom; zo kan 1 januari bij week 52 van het vorige jaar horen!
                if (weeknr > 50 && dag.Month == 1) jaar--;
                string week = string.Format("{0} - week {1}", jaar, weeknr);
                if (!cbbWeek.Items.Contains(week))
                {
                    cbbWeek.Items.Add(week);
                    if (DateTime.Now.Date <= dag.Date)
                        PeriodeSelectieNU[3] = cbbWeek.Items.Count - 1;

                    // Zet standaard weekkeuze op huidige week
                    if (dag.Date < DateTime.Now.Date && weekNumber(dag) != weekNumber(DateTime.Now))
                    {
                        int i = cbbWeek.Items.Count - 2;
                        if (i > -1 && cbbWeek.SelectedIndex == -1)
                        {
                            cbbWeek.SelectedIndex = i;
                            cbbWeek.SelectedItem = cbbWeek.Items[i];//.Items.Count - 1;
                        }
                    }
                }
            }
            //cbbWeek.SelectedItem = -1;
            //cbbWeek.SelectedIndex = -1;
        }

        internal void IinitializeConfigSettings(string Env )
        {
            // Nieuwe data inlezen
            initDatumBoxenEnUitvoerdersLijst();
            EERSTE_KEER = true;
        }

        private void Vul_Alle_SelectieListBoxen()
        {

            // SCREENTEST
            HideAll.Visible = true;

            // 2014-04-29 Misschien moeten we onderstaande 6 variabelen PUBLIC maken en alleen bij datumchang inladen!
            //var _contracten = pdfUniekeMandagenDezePeriode.Select(x => x.ContractVW).Distinct().ToList();
            List<PdfLVItems> _contracten = pdfAllContract
                .Where(c => pdfUniekeMandagenDezePeriode.Select(x => x.ContractVW).Distinct().ToList().Contains(c.ContractID))
                .Select(c => new PdfLVItems
                {
                    key = c.ContractID,
                    text = c.ContractNaam
                }).ToList();            

            var _arbeidsrelaties = pdfAllArbeidsrelatie.Where(x => pdfUniekeMandagenDezePeriode.Select(y => y.ArbeidsrelatieVW).Contains(x.ArbeidsRelatieID)).OrderBy(x => x.SorteerVolgorde).Select(x => x.ArbeidsRelatieID).ToList();
            var _ketenpartners = pdfPoortViewKetenpartner.Where(x => pdfUniekeMandagenDezePeriode.Select(y => y.KetenpartnerVW).Contains(x.KetenpartnerID)).OrderBy(x => x.Sorteervolgorde).Select(x => x.KetenpartnerID).ToList();
            var _functies = pdfAllFunctie.Where(x => pdfUniekeMandagenDezePeriode.Select(y => y.FunctieVW).Contains(x.FunctieID)).OrderBy(x => x.SorteerVolgorde).Select(x => x.FunctieID).ToList();
            var _registratiebevoegden = pdfUniekeMandagenDezePeriode.Select(x => x.ProjectleiderId).Distinct().ToList();
            var _NietBeschikbaar = pdfAllNietbeschikbaar.Where(x => pdfUniekeMandagenDezePeriode.Select(y => y.NietBeschikbaarVW).Contains(x.NBID)).OrderBy(x => x.SorteerVolgorde).Select(x => x.NBID).ToList();

            // vul de listboxen
            // 2014-04-29 Misschien moeten we layout-drawing even uit zetten!!

            tlpHoofd.SuspendLayout();

            if (EERSTE_KEER)
            { 
                lvContracten.Items.Clear();
                lvArbeidsrelaties.Items.Clear();
                lvFuncties.Items.Clear();
                lvNB.Items.Clear();
                lvRegistratiebevoegd.Items.Clear();
                lvKetenpartners.Items.Clear();
                lvProjecten.Items.Clear();
                lvVakmannen.Items.Clear();
            }

            // Contracten
            List<int> huidige_selectieIDs = (cbAllContracten.Checked && 
                lvContracten.Items.Count == lvContracten.CheckedItems.Count) ? new List<int>() : GetIDs(lvContracten, 0);
            List<PdfLVItems> Huidige_CheckedItems = pdfAllContract
                .Where(c => huidige_selectieIDs.Contains(c.ContractID))
                .Select(c => new PdfLVItems 
                             { 
                                 key = c.ContractID,
                                 text = c.ContractNaam
                             }).ToList();
            lvContracten.Items.Clear();
            foreach (PdfLVItems _contract in _contracten)
            {
                ListViewItem lvitem = new ListViewItem(new[] { _contract.text, _contract.key.ToString()});
                lvitem.Checked = (huidige_selectieIDs.Contains(_contract.key) || EERSTE_KEER || cbAllContracten.Checked);
                lvContracten.Items.Add(lvitem);
            }
            // Vul aan met vorige geselecteerde items (Rood)
            Add_Red_Items(lvContracten, Huidige_CheckedItems);
            //if (EERSTE_KEER || lvContracten.Items.Count == lvContracten.CheckedItems.Count) cbAllContracten.Checked = true; //else cbAllContracten.Checked = false;

            // Arbeidsrelaties
            huidige_selectieIDs = (cbAllArbeidsrelaties.Checked &&
                lvArbeidsrelaties.Items.Count == lvArbeidsrelaties.CheckedItems.Count) ?  new List<int>() : GetIDs(lvArbeidsrelaties,0);
            Huidige_CheckedItems = pdfAllArbeidsrelatie.Distinct()
                            .Where(c => huidige_selectieIDs.Contains(c.ArbeidsRelatieID))
                            .Select(c => new PdfLVItems
                            {
                                key = c.ArbeidsRelatieID,
                                text = c.ArbeidsRelatieNaam
                            }).ToList();            
            lvArbeidsrelaties.Items.Clear();
            foreach (int _key in _arbeidsrelaties)
            {
                ListViewItem lvitem = new ListViewItem(new[] { 
                        pdfAllArbeidsrelatie.Where(x => x.ArbeidsRelatieID == _key).Select(y => y.ArbeidsRelatieNaam).FirstOrDefault() ,
                        _key.ToString()});
                lvitem.Checked = (huidige_selectieIDs.Contains(_key) || EERSTE_KEER || cbAllArbeidsrelaties.Checked);
                lvArbeidsrelaties.Items.Add(lvitem);
            }
            // Vul aan met vorige geselecteerde items (Rood)
            Add_Red_Items(lvArbeidsrelaties, Huidige_CheckedItems);
            //if (EERSTE_KEER || lvArbeidsrelaties.Items.Count == lvArbeidsrelaties.CheckedItems.Count) cbAllArbeidsrelaties.Checked = true; //else cbAllArbeidsrelaties.Checked = false;


            // Ketenpartners
            huidige_selectieIDs = (cbAllKetenpartners.Checked  &&
                lvKetenpartners.Items.Count == lvKetenpartners.CheckedItems.Count) ? new List<int>()  : GetIDs(lvKetenpartners, 0);
            Huidige_CheckedItems = pdfPoortViewKetenpartner.Distinct()
                            .Where(c => huidige_selectieIDs.Contains(c.KetenpartnerID))
                            .Select(c => new PdfLVItems
                            {
                                key = c.KetenpartnerID,
                                text = c.Bedrijfsnaam 
                            }).ToList();
            lvKetenpartners.Items.Clear();

            foreach (int _key in _ketenpartners)
            {
                ListViewItem lvitem = new ListViewItem(new[] { 
                        pdfPoortViewKetenpartner.Where(x => x.KetenpartnerID == _key).Select(y => y.Bedrijfsnaam).FirstOrDefault() ,
                        _key.ToString()});
                lvitem.Checked = (huidige_selectieIDs.Contains(_key) || EERSTE_KEER || cbAllKetenpartners.Checked);
                lvKetenpartners.Items.Add(lvitem);
            }
            Add_Red_Items(lvKetenpartners, Huidige_CheckedItems);
            //if (EERSTE_KEER || lvKetenpartners.Items.Count == lvKetenpartners.CheckedItems.Count) cbAllKetenpartners.Checked = true; 
            
            // Functies
            bool ALLES_SELECTEREN = cbAllFuncties.Checked && lvFuncties.Items.Count == lvFuncties.CheckedItems.Count;
            huidige_selectieIDs = (ALLES_SELECTEREN) ? new List<int>() : GetIDs(lvFuncties, 0);
            Huidige_CheckedItems = pdfAllFunctie.Distinct()
                            .Where(c => huidige_selectieIDs.Contains(c.FunctieID))
                            .Select(c => new PdfLVItems
                            {
                                key = c.FunctieID,
                                text = c.FuncieOmschrijving
                            }).ToList();
            lvFuncties.Items.Clear();
            foreach (int _key in _functies)
            {
                ListViewItem lvitem = new ListViewItem(new[] { 
                        pdfAllFunctie.Where(x => x.FunctieID == _key).Select(y => y.FuncieOmschrijving).FirstOrDefault(),
                        _key.ToString()});
                lvitem.Checked = (huidige_selectieIDs.Contains(_key) || EERSTE_KEER || ALLES_SELECTEREN);
                lvFuncties.Items.Add(lvitem);
            }
            Add_Red_Items(lvFuncties, Huidige_CheckedItems);
            //if (EERSTE_KEER || lvFuncties.Items.Count == lvFuncties.CheckedItems.Count) cbAllFuncties.Checked = true; 

            // Registratiebevoegd
            huidige_selectieIDs = (cbAllUitvoerders.Checked &&
                lvRegistratiebevoegd.Items.Count == lvRegistratiebevoegd.CheckedItems.Count) ? new List<int>() : GetIDs(lvRegistratiebevoegd,0);
            Huidige_CheckedItems = pdfAllRegistratiebevoegd.Distinct()
                            .Where(c => huidige_selectieIDs.Contains(c.ProjectleiderId ?? -1))
                            .Select(c => new PdfLVItems
                            {
                                key = c.ProjectleiderId ?? -1,
                                text = c.Gebruikersnaam
                            }).ToList();
            lvRegistratiebevoegd.Items.Clear();
            foreach (int _key in _registratiebevoegden)
            {
                ListViewItem lvitem = new ListViewItem(new[] { 
                        pdfAllRegistratiebevoegd.Where(x => x.ProjectleiderId == _key).Select(y => y.Gebruikersnaam).FirstOrDefault() ?? "? ID="+_key.ToString(),
                        _key.ToString()});
                lvitem.Checked = (huidige_selectieIDs.Contains(_key) || EERSTE_KEER || cbAllUitvoerders.Checked);
                lvRegistratiebevoegd.Items.Add(lvitem);
            }
            Add_Red_Items(lvRegistratiebevoegd, Huidige_CheckedItems);
            //if (EERSTE_KEER || lvRegistratiebevoegd.Items.Count == lvRegistratiebevoegd.CheckedItems.Count) cbAllUitvoerders.Checked = true;

            // Niet Beschikbaar
            huidige_selectieIDs = (cbAllNietBeschikbaar.Checked &&
                lvNB.Items.Count == lvNB.CheckedItems.Count) ? new List<int>() : GetIDs(lvNB,0);
            Huidige_CheckedItems = pdfAllNietbeschikbaar.Distinct()
                            .Where(c => huidige_selectieIDs.Contains(c.NBID))
                            .Select(c => new PdfLVItems
                            {
                                key = c.NBID,
                                text = c.NBOmschrijving
                            }).ToList();
            lvNB.Items.Clear();
            foreach (int _key in _NietBeschikbaar)
            {
                ListViewItem lvitem = new ListViewItem(new[] { 
                        pdfAllNietbeschikbaar.Where(x => x.NBID == _key).Select(y => y.NBOmschrijving).FirstOrDefault() ,
                        _key.ToString()});
                lvitem.Checked = (huidige_selectieIDs.Contains(_key) || EERSTE_KEER || cbAllNietBeschikbaar.Checked);
                lvNB.Items.Add(lvitem);
            }
            Add_Red_Items(lvNB, Huidige_CheckedItems);
            //if (EERSTE_KEER || lvNB.Items.Count == lvNB.CheckedItems.Count) cbAllNietBeschikbaar.Checked = true;


            // Projecten of Vakmannen
            VulProjectenOfVakmannenList();

            // Pas de hoogte van de listboxen aan
            adjustListSize(lvContracten);
            adjustListSize(lvArbeidsrelaties);
            adjustListSize(lvKetenpartners);
            adjustListSize(lvFuncties);
            adjustListSize(lvRegistratiebevoegd);
            adjustListSize(lvNB);

            Maak_ListboxItems_Grijs();
            EERSTE_KEER = false;

            tlpHoofd.ResumeLayout();

            // SCREENTEST
            HideAll.Visible = false;

        }

        private void VulProjectenOfVakmannenList()
        {
            if (tabControlProjecten.SelectedIndex == 0) // Projecten
            {
                var _Projecten = pdfProject.Where(x => pdfUniekeMandagenDezePeriode.Select(y => y.ProjectId).Contains(x.ProjectId)).OrderByDescending(x => x.ProjectId).ToList();
                List<int> huidige_selectieIDs = (cbAllProjecten.Checked &&
                    lvProjecten.Items.Count == lvProjecten.CheckedItems.Count) ? new List<int>() : GetIDs(lvProjecten, 0);
                var Huidige_CheckedItems_PV = pdfProject.Distinct()
                                .Where(c => huidige_selectieIDs.Contains(c.ProjectId))
                                .Select(c => new PoortViewProject
                                {
                                    ProjectNR = c.ProjectNR,
                                    Naam = c.Naam,
                                    FirstDate = c.FirstDate,
                                    LastDate = c.LastDate,
                                    ProjectId = c.ProjectId
                                }).ToList();
                lvProjecten.Items.Clear();
                foreach (PoortViewProject _Projectje in _Projecten.OrderByDescending(x => x.ProjectNR))
                {
                    // Als project niet voldoet aan zoekveld en ook niet geselecteerd was, dan overslaan!
                    if (tbSearch.Visible && tbSearch.Text.Length > 0)
                        if (!(huidige_selectieIDs.Contains(_Projectje.ProjectId)))
                            if (!((_Projectje.ProjectNR.ToString() + _Projectje.Naam.ToUpper()).Contains(tbSearch.Text.ToUpper())))
                                continue;

                    string NRstring = _Projectje.ProjectNR.ToString();//  .Zeebregts.project_NR.ToString();
                    while (NRstring.Length < 4)
                        NRstring = " " + NRstring;
                    // Truukje, zodat deze kolom niet als getallen-kolom wordt geinterpreteerd 
                    // (heb ik alleen last van op de server of vanuit MDR gedraaid)
                    // In dat geval gaat ie deze kolom nl auto-resizen
                    NRstring += "\u0000";

                    ListViewItem lvitem = new ListViewItem(new[] { 
                    NRstring ,
                    _Projectje.Naam,
                    string.Format("{0:d-M-yy}",_Projectje.FirstDate),
                    string.Format("{0:d-M-yy}",_Projectje.LastDate),
                    _Projectje.ProjectId.ToString()
                });
                    lvitem.Checked = (huidige_selectieIDs.Contains(_Projectje.ProjectId) || EERSTE_KEER || cbAllProjecten.Checked);

                    lvProjecten.Items.Add(lvitem);
                }

                lvProjecten.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                lvProjecten.Columns[1].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                //listViewProjecten.Columns[0].Width = 17;
                lvProjecten.Columns[1].Width = 450 - lvProjecten.Columns[0].Width;

                Add_Red_Items_to_ProjectList(lvProjecten, Huidige_CheckedItems_PV);
            }
            else
            {
                // Vakmannen
                var _Vakmannen = pdfVakman.Where(x => pdfUniekeMandagenDezePeriode.Select(y => y.VakmanId).Contains(x.VakmanId)).OrderBy(x => x.Naam).ToList();
                List<int> huidige_selectieIDs = (cbAllVakmannen.Checked &&
                    lvVakmannen.Items.Count == lvVakmannen.CheckedItems.Count) ? new List<int>() : GetIDs(lvVakmannen, 0);
                var Huidige_CheckedItems_Vakmannen = pdfVakman.Distinct()
                                .Where(c => huidige_selectieIDs.Contains(c.VakmanId))
                                .Select(c => new PoortViewVakman
                                {
                                    Naam = c.Naam,
                                    bedrijf_nr = c.bedrijf_nr,
                                    FirstDate = c.FirstDate,
                                    LastDate = c.LastDate,
                                    VakmanId = c.VakmanId,
                                }).ToList();
                lvVakmannen.Items.Clear();
                foreach (PoortViewVakman _Vakman in _Vakmannen.OrderBy(x => x.Naam))
                {
                    // Als Vakman niet voldoet aan zoekveld en ook niet geselecteerd was, dan overslaan!
                    if (tbSearch.Visible && tbSearch.Text.Length > 0)
                        if (!(huidige_selectieIDs.Contains(_Vakman.VakmanId)))
                            if (!(      (_Vakman.Naam.ToUpper() + 
                                            (pdfBedrijf.Where(y => y.bedrijf_nr == _Vakman.bedrijf_nr).Select(x => x.naam).FirstOrDefault() ?? "onbekend").ToUpper()
                                        ).Contains(tbSearch.Text.ToUpper())
                                 )) 
                                continue;
                    ListViewItem lvitem = new ListViewItem(new[] { 
                    _Vakman.Naam,
                    pdfBedrijf.Where( y => y.bedrijf_nr == _Vakman.bedrijf_nr).Select(x => x.naam).FirstOrDefault(),
                    string.Format("{0:d-M-yy}",_Vakman.FirstDate),
                    string.Format("{0:d-M-yy}",_Vakman.LastDate),
                    _Vakman.VakmanId.ToString()
                });
                    lvitem.Checked = (huidige_selectieIDs.Contains(_Vakman.VakmanId) || EERSTE_KEER || cbAllVakmannen.Checked);

                    lvVakmannen.Items.Add(lvitem);
                }
                Add_Red_Items_to_VakmanList(lvVakmannen, Huidige_CheckedItems_Vakmannen);
            }
        }

        private void Add_Red_Items(ListView LV, List<PdfLVItems> thisList)
        {
            var _Keys = GetIDs(LV, 3);

            foreach (var thisItem in thisList)
            {
                if (!(_Keys.Contains(thisItem.key)))
                // Voeg item toe, maar dan in het rood
                {
                    ListViewItem lvitem = new ListViewItem(new[] { thisItem.text, thisItem.key.ToString() });
                    lvitem.Checked = true;
                    lvitem.ForeColor = ROOD;
                    lvitem.Font = FROOD;
                    LV.Items.Add(lvitem);
                }
            }
        }

        /// <summary>
        /// Add red items (versie voor Projectlijst)
        /// </summary>
        /// <param name="LV"></param>
        /// <param name="thisList"></param>
        private void Add_Red_Items_to_ProjectList(ListView LV, List<PoortViewProject> thisList)
        {
            var _Keys = GetIDs(LV, 3);

            foreach (var thisItem in thisList)
            {
                if (!(_Keys.Contains(thisItem.ProjectId)))
                // Voeg item toe, maar dan in het rood
                {
                    ListViewItem lvitem = new ListViewItem(new[] { thisItem.ProjectNR.ToString(), thisItem.Naam, 
                        string.Format("{0:d-M-yy}",thisItem.FirstDate),
                        string.Format("{0:d-M-yy}",thisItem.LastDate),
                        thisItem.ProjectId.ToString() 
                    });
                    lvitem.Checked = true;
                    lvitem.ForeColor = ROOD;
                    lvitem.Font = FROOD;
                    LV.Items.Add(lvitem);
                }
            }
        }

        /// <summary>
        /// Add red items (versie voor Vakmannen Lijst)
        /// </summary>
        /// <param name="LV"></param>
        /// <param name="thisList"></param>
        private void Add_Red_Items_to_VakmanList(ListView LV, List<PoortViewVakman> thisList)
        {
            var _Keys = GetIDs(LV, 3);

            foreach (var thisItem in thisList)
            {
                if (!(_Keys.Contains(thisItem.VakmanId)))
                // Voeg item toe, maar dan in het rood
                {
                    ListViewItem lvitem = new ListViewItem(new[] { thisItem.VakmanId.ToString(), thisItem.Naam, 
                        string.Format("{0:d-M-yy}",thisItem.FirstDate),
                        string.Format("{0:d-M-yy}",thisItem.LastDate),
                        thisItem.VakmanId.ToString() 
                    });
                    lvitem.Checked = true;
                    lvitem.ForeColor = ROOD;
                    lvitem.Font = FROOD;
                    LV.Items.Add(lvitem);
                }
            }
        }


        public class functie
        {
            public int VakmanID = new int();
            public string Functie = string.Empty;
        }
        public List <functie> FUNCTIELIJST = new List<functie>();
            
        public class nietbeschikb
        {
            public int ProjectID = new int();
            public string Naam = string.Empty;
        }
        public List<nietbeschikb> NIETBESCHIKBAAR= new List<nietbeschikb>();
        private bool EERSTE_KEER;


        /// <summary>
        /// Bepaal start van de week (maandag) van een bepaalde datum
        /// </summary>
        /// <param name="Datum"></param>
        /// <returns></returns>
        private DateTime Weekstart(DateTime Datum)
        {
            int dag = (int)Datum.DayOfWeek - 1;
            if (dag == -1) dag = 6;
            Datum = Datum.AddDays(dag * -1);
            return Datum;
        }

        private DateTime Bepaal1eMaandagvan(int jaar)
        {
            //pStartDatum = monthCalendarOneWeek.SelectionRange.Start;
            DateTime Startdag = new DateTime(jaar, 1, 1);
            int DagVanDeWeek = (int)Startdag.DayOfWeek;
            int dag = (int)Startdag.DayOfWeek - 1;
            if (dag > 3) // Kijk, 1-januari op een vrijdag, zaterdag of zondag valt, dan moeten we een week verder zijn
                dag -= 7;
            return Startdag.AddDays(dag * -1);
        }

        int weekNumber(DateTime fromdate, bool InclYear)
        {
            return fromdate.Year * 10000 + weekNumber(fromdate);
        }

        private int weekNumber(DateTime fromDate)
        {
            // Geleend van http://codebetter.com/petervanooijen/2005/09/26/iso-weeknumbers-of-a-date-a-c-implementation/

            // Get jan 1st of the year
            DateTime startOfYear = fromDate.AddDays(-fromDate.Day + 1).AddMonths(-fromDate.Month + 1);
            // Get dec 31st of the year
            DateTime endOfYear = startOfYear.AddYears(1).AddDays(-1);
            // ISO 8601 weeks start with Monday 
            // The first week of a year includes the first Thursday 
            // DayOfWeek returns 0 for sunday up to 6 for saterday
            int[] iso8601Correction = { 6, 7, 8, 9, 10, 4, 5 };
            int nds = fromDate.Subtract(startOfYear).Days + iso8601Correction[(int)startOfYear.DayOfWeek];
            int wk = nds / 7;
            switch (wk)
            {
                case 0:
                    // Return weeknumber of dec 31st of the previous year
                    return weekNumber(startOfYear.AddDays(-1));
                case 53:
                    // If dec 31st falls before thursday it is week 01 of next year
                    if (endOfYear.DayOfWeek < DayOfWeek.Thursday)
                        return 1;
                    else
                        return wk;
                default: return wk;
            }
        }

        private void CheckOrUncheck_Lijst(string Lijst)
        {
            switch (Lijst)
            {
                case "cbAllVakmannen": 
                        if (!cbAllVakmannen.Checked) lv_removeAllRed(lvVakmannen);
                        foreach (ListViewItem vakmannetje in lvVakmannen.Items)
                        {
                            vakmannetje.Checked = cbAllVakmannen.Checked;
                        }
                        break;
                case "cbAllProjecten":
                        if (!cbAllProjecten.Checked) lv_removeAllRed(lvProjecten);
                        foreach (ListViewItem projectje in lvProjecten.Items)
                        {
                            projectje.Checked = cbAllProjecten.Checked;
                        }
                        break;
                case "cbAllUitvoerders":
                    if (!cbAllUitvoerders.Checked) lv_removeAllRed(lvRegistratiebevoegd);
                    for (int i = 0; i < lvRegistratiebevoegd.Items.Count ; i++)
                    {
                        lvRegistratiebevoegd.Items[i].Checked = cbAllUitvoerders.Checked;
                    }
                        break;
                case "cbAllArbeidsrelaties":
                    if (!cbAllArbeidsrelaties.Checked) lv_removeAllRed(lvArbeidsrelaties);
                    for (int i = 0; i < lvArbeidsrelaties.Items.Count; i++)
                    {
                        lvArbeidsrelaties.Items[i].Checked = cbAllArbeidsrelaties.Checked;
                    }
                    break;
                case "cbAllKetenpartners":
                    if (!cbAllKetenpartners.Checked) lv_removeAllRed(lvKetenpartners);
                    for (int i = 0; i < lvKetenpartners.Items.Count; i++)
                    {
                        lvKetenpartners.Items[i].Checked = cbAllKetenpartners.Checked;
                    }
                    break;
                case "cbAllFuncties":
                    if (!cbAllFuncties.Checked) lv_removeAllRed(lvFuncties);
                    for (int i = 0; i < lvFuncties.Items.Count; i++)
                    {
                        lvFuncties.Items[i].Checked = cbAllFuncties.Checked;
                    }
                    break;
                case "cbAllNietBeschikbaar":
                    if (!cbAllNietBeschikbaar.Checked) lv_removeAllRed(lvNB);
                    for (int i = 0; i < lvNB.Items.Count; i++)
                    {
                        lvNB.Items[i].Checked = cbAllNietBeschikbaar.Checked;
                    }
                    break;
                case "cbAllContracten":
                    if (!cbAllContracten.Checked) lv_removeAllRed(lvContracten);
                    for (int i = 0; i < lvContracten.Items.Count; i++)
                    {
                        lvContracten.Items[i].Checked = cbAllContracten.Checked;
                    }
                    break;
                default:
                    MessageBox.Show("Foute aanroep module 'CheckOrUncheck_Lijst', geef '" + Lijst + "' door aan Marino");
                    break;
            }
            
        }

        private void WijzigPeriode()
        {
            // SCREEN TEST
            HideAll.Visible = true;

            GetUniekeMandagenDezePeriode();
            Vul_Alle_SelectieListBoxen();

            // SCREEN TEST
            HideAll.Visible = false;

            // Onthoud welke periode-selector verantwoordelijk is voor deze periode-wijziging
            foreach (var Rbutton in tlp2col.Controls.OfType<RadioButton>())
            {
                if (Rbutton.Checked)
                {
                    pubNaamPeriodeSelector = Rbutton.Name;
                    continue;
                }
            }

            this.Cursor = Cursors.Default;
            Cursor.Current = Cursors.Default;


        }

        /// <summary>
        /// Wijzig de pSTARTDATUM en pEINDDATUM
        /// Hou daarbij rekening dat de nieuwe data niet 2x achter elkaar opnieuw wordt ingelezem
        /// </summary>
        /// <param name="_pstartdatum"></param>
        /// <param name="_peindedatum"></param>
        private void pSETDATA(DateTime _pstartdatum, DateTime _peindedatum)
        {
            // verberg opbouwen van scherm indien niet reeds verborgen
            bool Unhide = true;
            if (Unhide = !HideAll.Visible)
            {
                HideAll.Visible = true;
                HideAll.Show();
            }
            
            // Als de Einddatum niet verandert, dan bij (eventuel) wijzigen van de startdatum updaten
            // Als einddatum wijzigt, dan hoef je bij wijzigen van startdatum niet te updaten, dat gebeurt dan bij
            // wijzigen van pEINDEDATUM
            pubUpdaten = (pEINDEDATUM == _peindedatum);
            if (pubUpdaten && cbShowAllWeek.Checked && _peindedatum.DayOfWeek != DayOfWeek.Sunday)
                pubUpdaten = false;
            pSTARTDATUM = _pstartdatum;
            pEINDEDATUM = _peindedatum;
            pubUpdaten = true; // Tja, je weet maar nooit

            // En verlaat het gebied om de muisbewegingen af te vangen!!!
            tlpHoofd.Focus();

            // Maak schernm weer zichtbaar indien door mij onzichtbaar gemaakt
            if (Unhide)
                HideAll.Visible = false;
        }


        private void monthCalendarStart_DateChanged(object sender, DateRangeEventArgs e)
        {
            return;
            // Check of huidgieg geselecteerde datum zichtbaar is binnen het huidige display van de datumpicker
            if (pSTARTDATUM < monthCalendarStart.GetDisplayRange(true).Start ||
                pSTARTDATUM > monthCalendarStart.GetDisplayRange(true).End)
            {
                // Pas datum van datumpicker aan zodat dezelfde dag binnen de geselecteerde maand getoond wordt
                if (pSTARTDATUM > NULLDATUM)
                {
                    pSTARTDATUM = NieuweMaand(pSTARTDATUM, monthCalendarStart.GetDisplayRange(true).Start);
                    monthCalendarStart.SelectionStart = pSTARTDATUM;
                }
                // Indien je hierboven de startdatum wijzigt wordt deze procedure aangeroepen en VISIBLE uitgezet. Zet dus gewoon weer aan haha
                monthCalendarStart.Visible = true;
                // Recalculate Vakmannen en Projecten
                //GetAllProjectenAndVakmannen();
                return;
            }
            else
                    monthCalendarStart.Visible = false;


            pSTARTDATUM = monthCalendarStart.SelectionStart;
            monthCalendarEind.MinDate = monthCalendarStart.SelectionStart;
            if (pEINDEDATUM < monthCalendarEind.MinDate)
                pEINDEDATUM = monthCalendarEind.SelectionStart;
            //WijzigPeriode();
        }

        private DateTime NieuweMaand(DateTime pStartDatum, DateTime ToDate)
        {
            if (pStartDatum.Date < ToDate.Date)
                while (pStartDatum.Month != ToDate.Month) pStartDatum = pStartDatum.AddMonths(1);
            else if (pStartDatum.Date > ToDate.Date)
                while (pStartDatum.Month != ToDate.Month) pStartDatum = pStartDatum.AddMonths(-1);
            return pStartDatum;
        }

        private void monthCalendarEind_DateChanged(object sender, DateRangeEventArgs e)
        {
            return;
            // Check of huidgieg geselecteerde datum zichtbaar is binnen het huidige display van de datumpicker
            if (pEINDEDATUM  < monthCalendarEind.GetDisplayRange(true).Start ||
                pEINDEDATUM > monthCalendarEind.GetDisplayRange(true).End)
            {
                // Pas datum van datumpicker aan zodat dezelfde dag binnen de geselecteerde maand getoond wordt
                pEINDEDATUM = NieuweMaand(pEINDEDATUM, monthCalendarEind.GetDisplayRange(true).Start);
                //pEindDatum = new DateTime(monthCalendarEind.GetDisplayRange(true).Start.Year,
                //    monthCalendarEind.GetDisplayRange(true).Start.Month, pStartDatum.Day);
                if (pEINDEDATUM >= monthCalendarEind.MinDate )
                    monthCalendarEind.SelectionStart = pEINDEDATUM;


                // Indien je hierboven de startdatum wijzigt wordt deze procedure aangeroepen en VISIBLE uitgezet. Zet dus gewoon weer aan haha
                monthCalendarEind.Visible = true;
                return;
            } else
                monthCalendarEind.Visible = false;

            pEINDEDATUM = monthCalendarEind.SelectionStart;
            //WijzigPeriode();
        }

        /// <summary>
        /// Return ID's (laatste kolom) van een listview
        /// </summary>
        /// <param name="Lijst">De listview</param>
        /// <param name="GetAll">0 = Alle aangevinkte, 1 = Aangevinkte met zwarte kleur, 2 = Niet-aangevinkte, 3 (default) = Alle ID's</param>
        /// <returns></returns>
        private List<int> GetIDs(ListView Lijst, int GetAll)
        {
            // Bepaal laatste kolomnr van de lijst want daar staat altijd het ID dat we willen hebben ;)
            int IDkolom = Lijst.Columns.Count - 1;

            switch (GetAll)
            {
                case ListViewSelecties.AlleAangevinkte:
                    return Lijst.CheckedItems.Cast<ListViewItem>()
                        .Select(x => x.SubItems[IDkolom].Text.ToInt()).ToList();
                case ListViewSelecties.AlleAangevinkteBehalveGrijs:
                    return Lijst.CheckedItems.Cast<ListViewItem>()
                            .Where(x => x.ForeColor == Color.Black)
                            .Select(x => x.SubItems[IDkolom].Text.ToInt()).ToList();
                case ListViewSelecties.AlleNietAangevinke:
                    return Lijst.Items.Cast<ListViewItem>()
                        .Where(x => x.Checked == false)
                        .Select(x => x.SubItems[IDkolom].Text.ToInt()).ToList();
                default:
                    return Lijst.Items.Cast<ListViewItem>()
                        .Select(x => x.SubItems[IDkolom].Text.ToInt()).ToList();
            }
        }

        private void Fill_DatumListview()
        {
            var alleTijden = pdfMandagen.Select(x => x.Begintijd.Date).Distinct().ToList();
            //lvDatum.Items.Clear();
            foreach (var xxx in alleTijden)
                lvDatum.Items.Add(xxx.ToShortDateString());
            adjustListSize(lvDatum);
        }
        private void button9_Click(object sender, EventArgs e)
        {
            int vertScrollWidth = SystemInformation.VerticalScrollBarWidth;
            tlpHoofd.Padding = new Padding(0, 0, vertScrollWidth, 0);
            //tlpHoofd.AutoScroll = true;
            //tlpHoofd.AutoSize = true;
            //tlpHoofd.Dock = DockStyle.Fill;
            tlpHoofd.ResumeLayout();
            tlpHoofd.Refresh();
            

            return;

            Scrolbars();
            //return;
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;
            int lw = tlpHoofd.Width;
            int lh = tlpHoofd.Height;
            int fw = ClientSize.Width;
            int fh = ClientRectangle.Height;
            // Dit geeft geloof ik een foutmelding, dus:
            int ffw = 800;
            int ffh = 600;
            try
            {
                ffw = Form.ActiveForm.Width;
                ffh = Form.ActiveForm.Height  - 33;;
                MessageBox.Show(string.Format("Schermbreedte: {0}, Hoogte: {1} \n Tabel: {2}/{3}\n Form: {4}/{5}", w, h, lw, lh, ffw, ffh));
            }
            catch (Exception eee)
            {
                try
                {
                    ffw = Convert.ToInt32(Tools.FindVisualParent<PageContainer>(((WFControl)(this)).HHH).ActualWidth);
                    ffh = Convert.ToInt32(Tools.FindVisualParent<PageContainer>(((WFControl)(this)).HHH).ActualHeight) - 133;
                    MessageBox.Show(string.Format("Schermbreedte MDR: {0}, Hoogte: {1} \n Tabel: {2}/{3}\n Form: {4}/{5}", w, h, lw, lh, ffw, ffh));
                }
                catch
                {
                    MessageBox.Show("Schermgegevens niet te bepalen vanuit MDR; we gaan uit van schermgrootte 800x600");
                }
            } 


            tlpHoofd.AutoScroll = true;
            tlpHoofd.AutoSize = false;
            tlpHoofd.MaximumSize = new System.Drawing.Size(ffw - 75, ffh);
            tlpHoofd.MinimumSize = new System.Drawing.Size(ffw - 75, ffh);
            tlpHoofd.Refresh();
            return;

            SetScreenScrollbars();

            return;

            ControlsUitlijnen(gbPeriode);
            adjustListSize(lvContracten);
            adjustListSize(lvArbeidsrelaties);
            adjustListSize(lvKetenpartners);
            adjustListSize(lvFuncties);
            adjustListSize(lvRegistratiebevoegd);
            adjustListSize(lvNB);




            lvDatum.Visible = !lvDatum.Visible;
            Fill_DatumListview();
            return;

            lvDatum.Location = cbbJaar.Location;
            
            
            

            return;

            Graphics g;

            g = this.CreateGraphics();

            Pen myPen = new Pen(Color.Red);
            myPen.Width = 30;
            g.DrawLine(myPen, 30, 30, 45, 65);

            g.DrawLine(myPen, 1, 1, 45, 65);
           
            
            
            ControlsUitlijnen(gbPeriode);
            adjustListSize(lvContracten);
            adjustListSize(lvArbeidsrelaties);
            adjustListSize(lvKetenpartners);
            adjustListSize(lvFuncties);
            adjustListSize(lvRegistratiebevoegd);
            adjustListSize(lvNB);


            //pictureBox1.BackColor = Color.Red;
            HideAll.Location = new Point(0, 0);
            HideAll.Width = this.ParentForm.Width;
            HideAll.Height = this.ParentForm.Height;
            //tlp2col.ResumeLayout();
            tlpHoofd.ResumeLayout();
            //tlpLinks.ResumeLayout();
            //tlpRechts.ResumeLayout();

            label2.Text = lvFuncties.Columns[0].Width.ToString();
            //lvFuncties.Columns[0].AutoResize(0);// = true;
            lvFuncties.AutoResizeColumn(0,ColumnHeaderAutoResizeStyle.ColumnContent);//  autoSizeColumnMode);
            label2.Text += "," + lvFuncties.Columns[0].Width.ToString();
            lvArbeidsrelaties.SelectedIndices.Clear();
            //label13.Focus();
            return;
            backgroundWorker1.RunWorkerAsync();
            return;
        }

        private Point LocationOnClient(Control c)
        {
            Point retval = new Point(0, 0);
            for (; c.Parent != null; c = c.Parent)
            { retval.Offset(c.Location); }
            return retval;
        }

        private void Scrolbars()
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;
            int lw = tlpHoofd.Width;
            int lh = tlpHoofd.Height;
            int fw = ClientSize.Width;
            int fh = ClientRectangle.Height;
            // Dit geeft geloof ik een foutmelding, dus:
            int ffw = 800;
            int ffh = 600;

            try
            {
                ffw = Form.ActiveForm.Width;
                ffh = Form.ActiveForm.Height - 33; ;
                MessageBox.Show(string.Format("Schermbreedte: {0}, Hoogte: {1} \n Tabel: {2}/{3}\n Form: {4}/{5}", w, h, lw, lh, ffw, ffh));
            }
            catch (Exception eee)
            {
                try
                {
                    ffw = Convert.ToInt32(Tools.FindVisualParent<PageContainer>(((WFControl)(this)).HHH).ActualWidth);
                    ffh = Convert.ToInt32(Tools.FindVisualParent<PageContainer>(((WFControl)(this)).HHH).ActualHeight) - 133;
                    MessageBox.Show(string.Format("Schermbreedte MDR: {0}, Hoogte: {1} \n Tabel: {2}/{3}\n Form: {4}/{5}", w, h, lw, lh, ffw, ffh));
                }
                catch
                {
                    //MessageBox.Show("Schermgegevens niet te bepalen vanuit MDR; we doen niets");
                    return;
                }
            }


            tlpHoofd.AutoScroll = true;
            tlpHoofd.AutoSize = false;
            tlpHoofd.MaximumSize = new System.Drawing.Size(ffw - 75, ffh);
            tlpHoofd.MinimumSize = new System.Drawing.Size(ffw - 75, ffh);
            //tlpHoofd.
            tlpHoofd.Refresh();
            MessageBox.Show("CHANGED!");
        }
        
        public void SetScreenScrollbars()
        {
            return; // 2014-10-26
            //Scrolbars();
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;

            // get position of tlpHoofd 
            // stomme truuk: Vanuit MDR levert dit een NULL waarde (foutmelding), dus dan zetten we het handmatig op 133
            int linksboven = 133;
            try
            {
                linksboven = tlpHoofd.FindForm().PointToClient(tlpHoofd.Parent.PointToScreen(tlpHoofd.Location)).Y;
            }
            catch (Exception e)
            { linksboven = 133; }

            // Het past allemaal al, niets doen
            if (tlpHoofd.Height + linksboven < h)
                return;


            //int sh = Screen.PrimaryScreen.Bounds.Size.Height;

            tlpHoofd.Visible = false;
            // Originele settings: 964; 592
            tlpHoofd.MinimumSize = new System.Drawing.Size(970, h - 100 - linksboven);
            //tlpHoofd.Size = new System.Drawing.Size(964, 592);
            //tlpHoofd.Size = new System.Drawing.Size(950, 592);
            tlpHoofd.MaximumSize = new System.Drawing.Size(w - 100, h - 100 - linksboven);
            tlpHoofd.AutoScroll = true;
            //tlpHoofd.Height = h - 100;
            tlpHoofd.Dock = DockStyle.None;
            tlpHoofd.AutoSize = true;
            //MessageBox.Show(m);
            tlpHoofd.Visible = true;

            MessageBox.Show("Sherm aangepast");
        }


        private void tabControlProjecten_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Zoekscherm mag gewist worden (doe maar alsof je op stopsearch klikt) maar onthoud wat er stond
            OLD_SearchstringProjecten[((TabControl)sender).SelectedIndex * -1 + 1] = tbSearch.Text; // 0 wordt 1, 1 wordt 0
            if (tbSearch.Visible || tbSearch.TextLength > 0)
            {
                tbSearch.TextChanged -= tbSearch_TextChanged;
                picStopSearch_Click(sender, e);
                tbSearch.TextChanged += tbSearch_TextChanged;
            }
            if (OLD_SearchstringProjecten[((TabControl)sender).SelectedIndex].Length > 0)
            {
                tbSearch.TextChanged -= tbSearch_TextChanged;
                tbSearch.Text = OLD_SearchstringProjecten[((TabControl)sender).SelectedIndex];
                tbSearch.Visible = true;
                picStopSearch.Visible = true;
                tbSearch.TextChanged += tbSearch_TextChanged;
            }
            // Let op: als showall aanstaat dan moeten de subselecties worden aangepast, dus opnieuw opbouwen!
            if (cbShowAllVan.Checked || cbShowAllTot.Checked)
            {
                WijzigPeriode();
            }
            else
            {
                // Projecten en of vakmannen handmatig vullen
                // Let op: Listboxen moeten nu weer grijs gemaakt worden want andere selectie is van toepassing
                VulProjectenOfVakmannenList();
                Maak_ListboxItems_Grijs();
            }
        }

        /// <summary>
        /// Nieuwe gegevens ophalen uit SQL en opslaan in lokale variabelen voor intern gebruik
        /// Tevens lijsten bijwerken; Vergeet eerdere selectie in de VakmannenLijst en de ProjectenLijst
        /// 
        /// Let op: Deze Procedure wordt aangeroepem door Wijzig_Periode, maar ook als je van Tabblad wisselt (althans, als ShowAll aanstaat)
        /// </summary>
        private void GetUniekeMandagenDezePeriode()
        {

            
            // TESTTEST hou eventjes bij hoe vaak de projectenlijst opnieuw wordt opgebouwd
            lblDatabase.Text = (Convert.ToInt16(lblDatabase.Text) + 1).ToString();


            //if (!BACKGROUNDFINNISCHED)
            //{
            //    ET.Log("GetAllProjectenAndVakmannen wordt direct afgebroken omdat het backgroundproces nog niet beindigd is!");
            //    return;
            //}


            // Wat is er aan de hand: Bij het setten van de einddatum pEINDDATUM, wat in het begin gebeurd,
            // vindt pEINDDATUM het nodig dat alle projecten en vakmannen opnieuw geladen worden. 
            // Misschien terecht, wie zal het zeggen, maar iig resulteert het hier toch in een catch() aangezien de connectie-string nog niet gevuld is.
            // Tja, dan kunnen we daar net zo goed van tevoren op anticiperen en meteen al returnen toch?.
            if (xMandagenRegistratieConnectionString.Length < 10)
                return;

            this.Cursor = Cursors.WaitCursor;
            Cursor.Current = Cursors.WaitCursor;

            // Dit is puur even om TEMP de juiste structuur te geven
            var TEMP = pdfMandagen.Where(x => false)
                .Select(x => new { x.VakmanId, x.ProjectId, x.ArbeidsrelatieVW, x.ContractVW, x.FunctieVW, x.KetenpartnerVW, x.NietBeschikbaarVW, x.ProjectleiderId }).Distinct().ToList();
            dbLPdfOutputDataContext dp = new dbLPdfOutputDataContext(xPdfOutputConnectieString);

            // Bepaal1eMaandagvan projecten / Vakmannen BINNEN DEZE PERIODE

            // Indien Showall aanstaat, dan willen we straks van de projecten of vakmannen die BINNEN DEZE periode voorkomen de gehele periode selecteren
            List<int> SHowAlls = new List<int>();
            if (cbShowAllVan.Checked || cbShowAllTot.Checked)
            {
                if (tabControlProjecten.SelectedIndex == 0)
                    SHowAlls = dp.PoortViewMandagens.Where(x => x.Begintijd >= pSTARTDATUM && x.Begintijd <= pEINDEDATUM).Select(x => x.ProjectId).Distinct().ToList();
                else
                    SHowAlls = dp.PoortViewMandagens.Where(x => x.Begintijd >= pSTARTDATUM && x.Begintijd <= pEINDEDATUM).Select(x => x.VakmanId).Distinct().ToList();
            }


            if (cbShowAllVan.Checked && cbShowAllTot.Checked)
            {
                TEMP = dp.PoortViewMandagens.Where
                (
                    (x =>
                        (x.Begintijd.Date >= pSTARTDATUM && x.Begintijd.Date <= pEINDEDATUM) ||
                        (tabControlProjecten.SelectedIndex == 0 && SHowAlls.Contains(x.ProjectId)) ||
                        (tabControlProjecten.SelectedIndex == 1 && SHowAlls.Contains(x.VakmanId))
                    )
                ).Select(x => new { x.VakmanId, x.ProjectId, x.ArbeidsrelatieVW, x.ContractVW, x.FunctieVW, x.KetenpartnerVW, x.NietBeschikbaarVW, x.ProjectleiderId }).Distinct().ToList();
            } else
            if (cbShowAllVan.Checked)
            {
                TEMP = dp.PoortViewMandagens.Where
                (
                    (x =>
                        (x.Begintijd.Date >= pSTARTDATUM && x.Begintijd.Date <= pEINDEDATUM) ||
                        (tabControlProjecten.SelectedIndex == 0 && x.Begintijd < pSTARTDATUM && SHowAlls.Contains(x.ProjectId)) ||
                        (tabControlProjecten.SelectedIndex == 1 && x.Begintijd < pSTARTDATUM && SHowAlls.Contains(x.VakmanId))
                    )
                ).Select(x => new { x.VakmanId, x.ProjectId, x.ArbeidsrelatieVW, x.ContractVW, x.FunctieVW, x.KetenpartnerVW, x.NietBeschikbaarVW, x.ProjectleiderId }).Distinct().ToList();
            }
            else
            if (cbShowAllTot.Checked)
            {
                TEMP = dp.PoortViewMandagens.Where
                (
                    (x =>
                        (x.Begintijd.Date >= pSTARTDATUM && x.Begintijd.Date <= pEINDEDATUM) ||
                        (tabControlProjecten.SelectedIndex == 0 && x.Begintijd > pSTARTDATUM && SHowAlls.Contains(x.ProjectId)) ||
                        (tabControlProjecten.SelectedIndex == 1 && x.Begintijd > pSTARTDATUM && SHowAlls.Contains(x.VakmanId))
                    )
                ).Select(x => new { x.VakmanId, x.ProjectId, x.ArbeidsrelatieVW, x.ContractVW, x.FunctieVW, x.KetenpartnerVW, x.NietBeschikbaarVW, x.ProjectleiderId }).Distinct().ToList();
            }
            else
            {
                TEMP = pdfMandagen.Where(x => x.Begintijd.Date >= pSTARTDATUM && x.Begintijd.Date <= pEINDEDATUM)
                    .Select(x => new { x.VakmanId, x.ProjectId, x.ArbeidsrelatieVW, x.ContractVW, x.FunctieVW, x.KetenpartnerVW, x.NietBeschikbaarVW, x.ProjectleiderId }).Distinct().ToList();
            }
            /// XXXX (2014-07-14)

            pdfUniekeMandagenDezePeriode.Clear();
            foreach (var X in TEMP)
            {
                UniekeMandagCombi CC = new UniekeMandagCombi();
                CC.VakmanId = X.VakmanId;
                CC.ProjectId = X.ProjectId;

                CC.ArbeidsrelatieVW = X.ArbeidsrelatieVW;
                CC.ContractVW = X.ContractVW;
                CC.FunctieVW = X.FunctieVW;
                CC.KetenpartnerVW = X.KetenpartnerVW;
                CC.NietBeschikbaarVW = X.NietBeschikbaarVW;
                CC.ProjectleiderId = X.ProjectleiderId;

                pdfUniekeMandagenDezePeriode.Add(CC);
            }


            // En nu mag je eigenlijk al gewoon returnen hoor je!
            return; // 2014-04-29 Okay, doen we!
            /* 2014-04-29  EINDE FUNCTIE*/
        }

        private void monthCalendarStart_DateSelected(object sender, DateRangeEventArgs e)
        {
            monthCalendarStart.Visible = false;
            pSTARTDATUM = monthCalendarStart.SelectionStart.Date;
        }

        private void monthCalendarEind_DateSelected(object sender, DateRangeEventArgs e)
        {
            monthCalendarEind.Visible = false;
            pEINDEDATUM = monthCalendarEind.SelectionStart.Date;
        }

        private void buttonVan_Click(object sender, EventArgs e)
        {
            if (pSTARTDATUM > NULLDATUM)
                monthCalendarStart.SelectionStart = pSTARTDATUM;
            ShowCalendar(sender, monthCalendarStart);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try // Houd rekening met eventuele mindate die nog verkeerd staat
            {
                monthCalendarEind.MinDate = pSTARTDATUM;
                monthCalendarEind.SelectionStart = pEINDEDATUM;
            }
            catch { }
            ShowCalendar(sender, monthCalendarEind);
        }

        private void monthCalendarStart_MouseLeave(object sender, EventArgs e)
        {
            monthCalendarStart.Visible = false;
            tlpHoofd.Focus();
        }

        private void monthCalendarEind_MouseLeave(object sender, EventArgs e)
        {
            monthCalendarEind.Visible = false;
        }

        private void rbJaar_CheckedChanged(object sender, EventArgs e)
        {
            rbPeriodeClick(sender);
        }

        private void rbPeriodeClick(object sender)
        {

            //DATUMTEST
            //DATUMTEST

            RadioButton RB = (RadioButton) sender;

            tlpHoofd.SuspendLayout();

            foreach (Control TB in tlp2col.Controls)
            {
                if (TB.Visible == true)
                {
                    if (TB is ComboBox)
                        TB.Visible = false;
                    if (TB is Button)
                        TB.Visible = false;
                }
            }

            buttonDatum.Visible = false;
            buttonVan.Visible = false;
            buttonTot.Visible = false;
            monthCalendarDatum.Visible = false;
            monthCalendarEind.Visible = false;
            monthCalendarStart.Visible = false;

            switch (RB.Name.ToLower() )// Text.ToLower())
            {
                case "rbjaar":
                    cbbJaar.Visible = true;
                    if (RB.Name != pubNaamPeriodeSelector)
                    {
                        //cbbJaar.SelectedItem = null;
                        cbbJaar.Text = "- Selecteer - ";
                    }
                    //DATUMTEST
                    var alleTijden = pdfMandagen.Select(x => x.Begintijd.Year.ToString()).Distinct().ToList().OrderByDescending(x => x);
                    lvDatum.Items.Clear();
                    foreach (var xxx in alleTijden)
                        lvDatum.Items.Add(xxx);
                    //if (lvDatum.Items.Count > 0)  adjustListSize(lvDatum);
                    //DATUMTEST
    
                    break;
                case "rbkwartaal":
                    cbbKwartaal.Visible = true;
                    if (RB.Name != pubNaamPeriodeSelector)
                    {
                        //cbbKwartaal.SelectedItem = null;
                        cbbKwartaal.Text = "- Selecteer - ";
                    }
                    break;
                case "rbmaand":
                    cbbMaand.Visible = true;
                    if (RB.Name != pubNaamPeriodeSelector)
                    {
                        //cbbMaand.SelectedItem = null;
                        cbbMaand.Text = "- Selecteer - ";
                    }
                    //DATUMTEST
                    alleTijden = pdfMandagen.Select(x => x.Begintijd.Month.ToString()).Distinct().ToList().OrderByDescending(x => x);
                    lvDatum.Items.Clear();
                    foreach (var xxx in alleTijden)
                        lvDatum.Items.Add(xxx);
                    //if (lvDatum.Items.Count > 0)  adjustListSize(lvDatum);
                    //DATUMTEST
                    break;
                case "rbweek":
                    cbbWeek .Visible = true;
                    if (RB.Name != pubNaamPeriodeSelector)
                    {
                        //cbbWeek.SelectedItem = null;
                        cbbWeek.Text = "- Selecteer - ";
                    }
                    //DATUMTEST
                    alleTijden = pdfMandagen.Select(x => ET.weeknummerNEW(x.Begintijd,true)).Distinct().ToList().OrderByDescending(x => x);
                    lvDatum.Items.Clear();
                    foreach (var xxx in alleTijden)
                        lvDatum.Items.Add(xxx);
                    //if (lvDatum.Items.Count > 0)  adjustListSize(lvDatum);
                    //DATUMTEST
                    break;
                case "rbdag":
                    cbbDag.Visible = true;
                    if (RB.Name != pubNaamPeriodeSelector)
                    {
                        //cbbDag.SelectedItem = null;
                        cbbDag.Text = "- Selecteer - ";
                    }
                    break;
                case "rbdatum":
                    buttonDatum.Visible = true;
                    break;
                case "rbanders":
                    buttonVan.Visible = true;
                    buttonTot.Visible = true;
                    break;
            }

            tlpHoofd.ResumeLayout();
        }

        private void cbbJaar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbJaar.SelectedIndex == -1) return;

            DateTime gekozenstart = new DateTime(cbbJaar.Text.ToInt(), 1, 1);
            DateTime gekozeneinde = new DateTime(cbbJaar.Text.ToInt(), 12, 31);

            // Set de periode op de gekozen maand
            pSETDATA(gekozenstart, gekozeneinde);
        }

        private void rbKwartaal_CheckedChanged(object sender, EventArgs e)
        {
            rbPeriodeClick(sender);
        }

        private void cbbKwartaal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbKwartaal.SelectedIndex == -1) return;

            // Listitem in de vorm "2014 - kwartaal 1"
            string item = cbbKwartaal.SelectedItem.ToString();
            int jaar = Convert.ToInt16(item.Substring(0, item.IndexOf(" ")));
            int kwartaal = Convert.ToInt16(item.Substring(item.IndexOf("aal ") + 4));

            DateTime gekozenstart = new DateTime(jaar, (kwartaal - 1) * 3 + 1, 1);
            DateTime gekozeneinde = gekozenstart.AddMonths(3).AddDays(-1);

            //if (cbShowWeek.Checked)
            //{
            //    // Sla eerst de 'eigenlijke' datum-selectie op
            //    ShowWeekStart = gekozenstart;
            //    ShowWeekEinde = gekozeneinde;
            //    // Breid nu uit naar de hele week
            //    gekozenstart = Weekstart(gekozenstart);
            //    gekozeneinde = Weekstart(gekozeneinde).AddDays(6);

            //}

            pSETDATA(gekozenstart, gekozeneinde);
            
        }

        private void rbMaand_CheckedChanged(object sender, EventArgs e)
        {
            rbPeriodeClick(sender);
        }

        private void rbWeek_CheckedChanged(object sender, EventArgs e)
        {
            rbPeriodeClick(sender);
        }

        private void rbDag_CheckedChanged(object sender, EventArgs e)
        {
            rbPeriodeClick(sender);
        }

        private void cbbMaand_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbMaand.SelectedIndex == -1) return;

            string item = cbbMaand.Items[cbbMaand.SelectedIndex].ToString();
            //
            // Listbox-item is in de vorm: "2014 - januari"
            //

            int jaar = Convert.ToInt16(item.Substring(0,item.IndexOf(" - "))); // 1e deel bevat dus het jaartal
            string m = item.Substring(item.IndexOf(" - ") + 3); // laatste deel bevat de maand - als tekst
            int maand = pl_MaandStrings.IndexOf(m); // zoek bijbehorend maandnummer op in de (publieke) lijst van maandstrings

            DateTime gekozenstart = new DateTime(jaar, maand, 1);
            DateTime gekozeneinde = gekozenstart.AddMonths(1).AddDays(-1);

            //if (cbShowWeek.Checked)
            //{
            //    // Sla eerst de 'eigenlijke' datum-selectie op
            //    ShowWeekStart = gekozenstart;
            //    ShowWeekEinde = gekozeneinde;
            //    // Breid nu uit naar de hele week
            //    gekozenstart = Weekstart(gekozenstart);
            //    gekozeneinde = Weekstart(gekozeneinde).AddDays(6);

            //}

            // Set de periode op de gekozen maand
            pSETDATA(gekozenstart, gekozeneinde);
        }

        private void rbAnders_CheckedChanged(object sender, EventArgs e)
        {
            rbPeriodeClick(sender);
        }

        private void cbbWeek_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbWeek.SelectedIndex == -1) return;

            // Listitem in de vorm "2014 - week 43"
            string item = cbbWeek.SelectedItem.ToString();
            int jaar = Convert.ToInt16(item.Substring(0, item.IndexOf(" ")));
            int week = Convert.ToInt16(item.Substring(item.IndexOf("eek ") + 4));

            DateTime startdag = Bepaal1eMaandagvan(jaar);
            startdag = startdag.AddDays(7 * (week-1));

            pSETDATA(startdag, startdag.AddDays(6));
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            // TESTTESTTEST plaats vakmannen in Memory zodat je ze kunt plakken in Excel
            string MEM = "";
            // Kolomnamen
            for (int s = 1; s < lvVakmannen.Columns.Count; s++)
            {
                MEM += (s == 1 ? "" : "\t") +  lvVakmannen.Columns[s].Text;
            }
            MEM += System.Environment.NewLine;
            foreach (ListViewItem Li in lvVakmannen.Items)
            {
                for (int s = 1; s < Li.SubItems.Count ; s++)
                {
                    MEM += (s == 1 ? "" : "\t") + Li.SubItems[s].Text;
                }
                MEM += System.Environment.NewLine;
            }
            Clipboard.SetText(MEM);
        }

        private void cbbDag_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbDag.SelectedIndex == -1) return;

            // Listitem in de vorm "2014-12-25"
            string item = cbbDag.SelectedItem.ToString();

            pSETDATA(Convert.ToDateTime(item), Convert.ToDateTime(item));
        }

        /// <summary>
        /// Hier komen we binnen vanuit Scherm1 (Zowel via Juraci MDR Mandagen als Via eigen Knop)
        /// </summary>
        /// <param name="_lijst"></param>
        /// <param name="_Omschrijving"></param>
        public void Bevestig(MarioPDFSettings _PDFSETTINGS)
        {
            try
            {
                HideAll.Location = new Point(0, 0);
                HideAll.Width = tlpHoofd.Width + 40;
                HideAll.Height = 600;
                HideAll.Visible = true;
                HideAll.Show();
                HideAll.Refresh();
                HideAll.ResumeLayout();
            }
            catch
            {
                MessageBox.Show("Kleine fout");
            }


            //test 2014-06-23
            this.Cursor = Cursors.WaitCursor;
            Cursor.Current = Cursors.WaitCursor;
            //tlpHoofd.SuspendLayout();
            tlpHoofd.Visible = false;
            HideAll.Refresh();

            ET.Log("Bevestig");

            ET.Log("*** Applicationstate variabelen ophalen ***");
            //PDFSETTINGS = ApplicationState.GetValue<MarioPDFSettings>(GlobaleVars.strApplicationState);
            // Ik weet niet zeker of het uitmaakt, maar op deze manier lijkt me veiliger
            PDFSETTINGS = new MarioPDFSettings();
            PDFSETTINGS = _PDFSETTINGS;
            SublijstCode = PDFSETTINGS.Lijstgegevens.Code;

            //MessageBox.Show(PDFSETTINGS.Lijstgegevens.Titel + "/" + PDFSETTINGS.Lijstgegevens.Code);
            pubNaamPeriodeSelector = ""; // Bij start willen we toch niet dat er al en voorgeselecteerde peropde aanstaat!
            // 'Reset voorselectie
            this.rbAnders.Checked = true;
            //2014-05-19 Niet doen, zet aan of uit in Lijst Layout Settings
            rbWeek.Checked = true;

            EnvironmentSettings(PDFSETTINGS.Omgeving);
            // Zou hierdoor n problem ontstaan?
            // Eens eventjes testen met BACKGROUNDFINNISHED;
            BACKGROUNDFINNISCHED = false;
            ET.Log("*** Start BackgroundWorker ***");
            // backgroundWorker1.RunWorkerAsync();
            // Jammer, het lijkt in productiee steeds mis te gaan dus niks geen backgroundproces gebeuren meeer!
            BackgroundProces();
            ET.Log("*** Finisched BackgroundWorker ***");
            IinitializeConfigSettings(PDFSETTINGS.Omgeving);

            bool IsAdmin = WGebruikerIsAdmin();

            //gbEasy.Visible = IsAdmin;
            EasyUser.SelectedIndex = (PDFSETTINGS.Gebruiker.IsManager ? 0 : 1);

            // Eenmalig (Per nieuwe Lijst of nieuwe Omgeving) 
            // * Start en Einddata van projecten/Vakmannen ophalen en opslaan in publieke lijsten VminMax en PminMax
            // * Datum boxen met geldige data vullen
            pSTARTDATUM = NULLDATUM;

            // Clear datum-selection
            pSTARTDATUM = NULLDATUM;
            if (!(UserInterface_Settings())) return;
            EasySet();
            ET.Log("RETURN from Bevestig");

            //test 2014-06-23
            this.Cursor = Cursors.Default;
            Cursor.Current = Cursors.Default;

            // Set huidig moment: NU ben ik klaar met alles en mag de fucking OK knop eigenlijk pas gaan werken
            EVENWACHTEN = DateTime.Now;

            SetScreenScrollbars();
            tlpHoofd.Visible = true;
            //Scrolbars();
        }

        private bool WGebruikerIsAdmin()
        {
            dbLMandagenDataContext dm = new dbLMandagenDataContext(xMandagenRegistratieConnectionString);
            string windowsentity = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            Gebruiker Geb = new Gebruiker();
            bool IsAdmin = false;
            try
            {
                Geb = dm.Gebruikers.Where(x => x.Windowsidentity.ToUpper() == windowsentity.ToUpper()).FirstOrDefault();
                IsAdmin = Geb.IsAdministrator;
            }
            catch (Exception e)
            {
                MessageBox.Show("Uw Login gegevens zijn niet gevonden, bent u wel een gerigistreerd gebruiker?? \n" + e.ToString());
                return false;
            }
            return IsAdmin;
        }

        private void EnvironmentSettings(string Env)
        {
            ET.Log("EnvironmentSettings: " + Env);
            ET.Log("IinitializeConfigSettings");
            string stringzb = "ZeebregtsDBConnectionStringLIVE";
            string stringmd = "MandagenRegistratieConnectionStringLIVE";

            switch (Env)
            {
                case "BETA":
                    stringzb = "ZeebregtsDBConnectionStringBETA";
                    stringmd = "MandagenRegistratieConnectionStringBETA";
                    break;
                case "DEV":
                    stringzb = "ZeebregtsDBConnectionStringDEV";
                    stringmd = "MandagenRegistratieConnectionStringDEV";
                    break;
                default: //"LIVE":
                    stringzb = "ZeebregtsDBConnectionStringLIVE";
                    stringmd = "MandagenRegistratieConnectionStringLIVE";
                    break;
            }

            xZeebregtsDBConnectionString = ConfigurationManager.ConnectionStrings[stringzb].ConnectionString;
            xMandagenRegistratieConnectionString = ConfigurationManager.ConnectionStrings[stringmd].ConnectionString;
            xPdfOutputConnectieString = xMandagenRegistratieConnectionString.Replace("MandagenRegistratie", "PdfOutput");

            // Voor gebruik in andere modules
            Class1.Globals.ZeebregtsDBConnectionString = xZeebregtsDBConnectionString;
            Class1.Globals.MandagenRegistratieConnectionString = xMandagenRegistratieConnectionString;

            //IinitializeConfigSettings(Omgeving);
        }

        private void rbDatum_CheckedChanged(object sender, EventArgs e)
        {
            rbPeriodeClick(sender);
        }

        private void buttonDatum_Click(object sender, EventArgs e)
        {
            ShowCalendar(sender, monthCalendarDatum);
        }

        /// <summary>
        /// Make control visible at mouseposition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="_monthCalendar"></param>
        private void ShowCalendar(Object sender, Control _monthCalendar)
        {
            _monthCalendar.Visible = true;
            Point xx = new Point();
            xx = this.PointToClient(Control.MousePosition);
            xx.X += 10;
            xx.Y -= (_monthCalendar.Height/3*2);
            _monthCalendar.Location = xx;
            _monthCalendar.BringToFront();
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            pSETDATA( monthCalendarDatum.SelectionStart.Date, monthCalendarDatum.SelectionStart.Date);
            monthCalendarDatum.Visible = false;
        }

        private void monthCalendarDatum_MouseLeave(object sender, EventArgs e)
        {
            monthCalendarDatum.Visible = false;
        }

        //TESTTESTTEST 
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //BackgroundProces();
        }

        private void BackgroundProces()
        {

            if (xMandagenRegistratieConnectionString == null || xMandagenRegistratieConnectionString.Length < 10)
            {
                MessageBox.Show("He vervelend, de connectiestring is niet geladen. Dit bericht is bestemd voor Marino");
                xMandagenRegistratieConnectionString = ConfigurationManager.ConnectionStrings["MandagenRegistratieConnectionStringLIVE"].ConnectionString;
            }

            // Nieuwe stijl; we weken met PdfOutput Database (met eigen poortview gebeuren)
            dbLPdfOutputDataContext dp = new dbLPdfOutputDataContext(xPdfOutputConnectieString);
            
            // Get all eigen tabellen
            pdfAllArbeidsrelatie = dp.allArbeidsRelaties.ToList();
            pdfAllContract = dp.allContracts.ToList();
            pdfAllFunctie = dp.allFuncties.ToList();
            pdfPoortViewKetenpartner = dp.PoortViewKetenpartners.ToList();
            pdfAllNietbeschikbaar = dp.allNietBeschikbaars.ToList();
            pdfAllRegistratiebevoegd = dp.PoortViewRegistratiebevoegds.ToList();
            //pdfAllLanden = dp.allLandens.ToList();

            // Get all Poortviews
            pdfBedrijf = dp.PoortViewBedrijfs.ToList();
            pdfMandagen = dp.PoortViewMandagens.ToList();
            pdfProject = dp.PoortViewProjects.ToList();
            pdfVakman = dp.PoortViewVakmans.ToList();
            //pdfrelVakmanDefault = dp.relViewVakmandefaults.ToList();
            //pdfrelVakmanDefault = dp.relVakmandefaults.ToList();


            //
            //
            //  EN JAJAJAJ STRAKS KUNNEN WE HIER RETURNEN; ALLEEN Nog maar de pdfViews !!!!!
            //  2014-04-30 (Geplande datum) - STOP HIER
            BACKGROUNDFINNISCHED = true;
            return;
            //
            //

        }

        private void Controleer_VakmanRelaties( List<PoortViewMandagenNoRel> pdfMandagenNoRel, List<PoortViewMandagen> pdfGeselecteerdeMandagen, List<PoortViewVakmanNoDefault> pdfVakmanNoDefault)
        {
            if (pdfMandagenNoRel.Count == 0) return;
            var _prIDs = pdfGeselecteerdeMandagen.Select(x => x.ProjectId).Distinct().ToList();
            var _vakmIDs = pdfMandagenNoRel.Where(x => _prIDs.Contains(x.ProjectId )).Select(x => x.VakmanId).Distinct().ToList();
            if (_vakmIDs.Count() == 0) return;


            string msg = "Er zijn vakmannen zonder Relatie!\n";
            msg += "Deze vakmannen worden niet meegenomen in de afgedrukte rapporten\n";
            msg += "\nHet gaat om de volgende vakmannen:\n\n";
            string msg2 = "";
            foreach (int _VakmanID in _vakmIDs)
            {
                var vakman = pdfVakmanNoDefault.Where(x => x.VakmanId == _VakmanID).FirstOrDefault();
                
                if (vakman == null || vakman.Naam == null)
                    msg += string.Format("{0}\n","Onbekende vakman met ID: " + _VakmanID.ToString());
                else
                    msg2 += ", " + vakman.Naam.ToString();
            }
            if (!(msg2 == "")) msg += "\n en nog de volgende VakmanID's waarvan alle gevens onbekend zijn: " + msg2;
            MessageBox.Show(msg, "Vakmanrelaties moet bijgewerkt worden");
        }


        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //progressBar1.Value = e.ProgressPercentage;
            //PURGEME
            switch (e.ProgressPercentage)
            {
                case 100:
                    break;
            }
            //ET.Log(string.Format("Background procees reeported {0}%",e.ProgressPercentage.ToString()));
        }


        private void cbShowWeek_CheckedChanged(object sender, EventArgs e)
        {
            ShowWeekAanUit(cbShowAllWeek.Checked, pSTARTDATUM, pEINDEDATUM);
        }

        /// <summary>
        /// Breid de selectieperiode uit naar hele weken of Restore naar vorige Periode
        /// </summary>
        /// <param name="ZetWeekAan">true = Uitbreidn naar Hele weken, False = Restore naar vorige periode</param>
        private void ShowWeekAanUit(bool ZetWeekAan, DateTime _BeginDatum, DateTime EindDatum)
        {
            if (_BeginDatum <= NULLDATUM) return; // Startsetting, doe niets
            if (ZetWeekAan) // Breid periode uit naar hele weken en onthoud hudige selectie
            {
                // Breid Periode uit naar hele Weken (Alleen indien nodig natuurlijk)
                //if (pSTARTDATUM != Weekstart(pSTARTDATUM) || pEINDEDATUM != pSTARTDATUM.AddDays(6))
                if (pSTARTDATUM.DayOfWeek != DayOfWeek.Monday || pEINDEDATUM.DayOfWeek != DayOfWeek.Sunday)
                {
                    // Huidige geselecteerde vakmannen en Projecten opslaan
                    ShowWeekBackupCheckedItems(true);
                    pSETDATA(pSTARTDATUM, pEINDEDATUM);

                    // Alls is nu gereset, dus geselecteerde Projecten en/of Vakmannen weer selecteren in de lijst
                    // Of toevoegen indien niet meer aanwezig in de huidige lijst!
                    ShowWeekBackupCheckedItems(false);
                }
            }
            else // Week wordt uitgezet
            {
                // Hadden we al een Backup?
                ShowWeekBackupCheckedItems(true);

                // Ga maar fijn weerterug naar oorspronkelijk geselecteerde boolshit
                pSETDATA(ShowWeekStart, ShowWeekEinde);

                // restore geselecteerde items
                ShowWeekBackupCheckedItems(false);
            }
        }

        /// <summary>
        /// Backup or Restore Checked Projecten and Vakmannen
        /// </summary>
        /// <param name="Backup">true = Backup, False = Restore</param>
        private void ShowWeekBackupCheckedItems(bool Backup)
        {
            // TEST onsderstaand GELUL is volgens mij nu helemaal totally niet meer nodig
            // aangezien we nu de oude selecties toch al onthouden!!!! (checkbox cbLock)
            return;
            if (Backup)
            {
                SHowWeekSelectedProjecten.Clear(); SHowWeekSelectedVakmannen.Clear();
                foreach (ListViewItem P in lvProjecten.CheckedItems)
                    SHowWeekSelectedProjecten.Add(P);
                foreach (ListViewItem V in lvVakmannen.CheckedItems)
                    SHowWeekSelectedVakmannen.Add(V);
            }
            else // (Restore)
            {
                // restore Projecten
                int LastColumn = lvProjecten.Columns.Count - 1; // Hier staat het ID
                foreach (ListViewItem Pmem in SHowWeekSelectedProjecten)
                {
                    bool aanwezig = false;
                    foreach (ListViewItem Plist in lvProjecten.Items)
                    {
                        if (Pmem.SubItems[LastColumn].Text == Plist.SubItems[LastColumn].Text)
                        {
                            aanwezig = true;
                            Plist.Checked = true;
                            // Neeneenee, hou de kleur op wat ie was
                            //Plist.ForeColor = Color.Black;
                            continue;
                        }
                    }
                    if (!aanwezig)
                    {
                        Pmem.Checked = true;
                        Pmem.ForeColor = Color.DarkGray;
                        lvProjecten.Items.Add(Pmem);
                    }
                }
                // restore Vakmannen
                LastColumn = lvVakmannen.Columns.Count - 1; // Hier staat het ID
                foreach (ListViewItem Vmem in SHowWeekSelectedVakmannen)
                {
                    bool aanwezig = false;
                    foreach (ListViewItem Vlist in lvVakmannen.Items)
                    {
                        if (Vmem.SubItems[LastColumn].Text == Vlist.SubItems[LastColumn].Text)
                        {
                            aanwezig = true;
                            Vlist.Checked = true;
                            // Neeneenee, hou de kleur op wat ie was
                            //Vlist.ForeColor = Color.Black;
                            continue;
                        }
                    }
                    if (!aanwezig)
                    {
                        Vmem.Checked = true;
                        Vmem.ForeColor = Color.DarkGray;
                        lvVakmannen.Items.Add(Vmem);
                    }
                }
            }

        }

        
        private void cbbMaand_DropDown(object sender, EventArgs e)
        {
        // http://www.codeproject.com/Articles/5801/Adjust-combo-box-drop-down-list-width-to-longest-s
        //public void AdjustWidthComboBox_DropDown(object sender, System.EventArgs e)
        //{
            ComboBox senderComboBox = (ComboBox)sender;
            int width = senderComboBox.DropDownWidth;
            Graphics g = senderComboBox.CreateGraphics();
            Font font = senderComboBox.Font;
            int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                ? SystemInformation.VerticalScrollBarWidth : 0;

            int newWidth;
            foreach (string s in ((ComboBox)sender).Items)
            {
                newWidth = (int)g.MeasureString(s, font).Width
                    + vertScrollBarWidth;
                if (width < newWidth)
                {
                    width = newWidth;
                }
            }
            senderComboBox.DropDownWidth = width;

            if (cbbMaand.SelectedItem != null)
                cbbMaand.Text = cbbMaand.SelectedItem.ToString();

            //}
        }


        private void cbbJaar_DropDown(object sender, EventArgs e)
        {
            // Pas breedte aan
            MszTools.AdjustWidthComboBox_DropDown(sender, e);
            if (cbbJaar.SelectedItem != null)
                cbbJaar.Text = cbbJaar.SelectedItem.ToString();
        }

        private void cbbWeek_DropDown(object sender, EventArgs e)
        {
            // Pas breedte aan
            MszTools.AdjustWidthComboBox_DropDown(sender, e);
            if (cbbWeek.SelectedItem != null)
                cbbWeek.Text = cbbWeek.SelectedItem.ToString();

        }

        private void monthCalendarDatum_MouseHover(object sender, EventArgs e)
        {
            //Visible = true;
        }

        private void cbbKwartaal_DropDown(object sender, EventArgs e)
        {
            // Pas breedte aan
            MszTools.AdjustWidthComboBox_DropDown(sender, e);
            if (cbbKwartaal.SelectedItem != null)
                cbbKwartaal.Text = cbbKwartaal.SelectedItem.ToString();
        }

        private void cbbDag_DropDown(object sender, EventArgs e)
        {
            // Pas breedte aan
            MszTools.AdjustWidthComboBox_DropDown(sender, e);
            if (cbbDag.SelectedItem != null)
                cbbDag.Text = cbbDag.SelectedItem.ToString();
        }

        private static void adjustListSize(ListView lst)
        {
            // Hoogte aanpassen
            int h = (ListBox.DefaultItemHeight + 4) * Math.Min(lst.Items.Count, 5) + 6;
            lst.Height = h + lst.Height - lst.ClientSize.Height;
            // Breedte van de eerste kolom aanpassen
            lst.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);//  autoSizeColumnMode);

        }
        
        private static void adjustListSize(ListBox lst)
        {
            int h = lst.ItemHeight * Math.Min( lst.Items.Count, 5);
            lst.Height = h + lst.Height - lst.ClientSize.Height;
        } 

        private void timer1_Tick(object sender, EventArgs e)
        {
            Point xx = new Point();
            string x = this.Name;
//            xx.Offset(pnDatePickers.PointToClient();
            xx = MousePosition;
            xx = this.PointToClient(Control.MousePosition);
        }

        private void LijstChange(object sender, EventArgs e)
        {
            LijstChange(sender);
        }

        /// <summary>
        /// Als de lijstkeuze wijzigt, verander dan de sublijst-titel
        /// </summary>
        /// <param name="sender"></param>
        private void LijstChange(object sender)
        {
            this.PageSubtitle = ((RadioButton) sender).Text;
            this.PageSubtitle = this.PageSubtitle.Replace('\n', ' ');
            lblSubform.Text = this.PageSubtitle;
            
            SublijstCode = string.Empty;
            for (int i = 0; i < PDFSETTINGS.Lijstgegevens.SubLijst.Count; i++)
            {
                if (PDFSETTINGS.Lijstgegevens.SubLijst[i].SubTitel == lblSubform.Text)
                {
                    SublijstCode = PDFSETTINGS.Lijstgegevens.SubLijst[i].SubCode;
                    BSN_Tonen = PDFSETTINGS.Lijstgegevens.SubLijst[i].SubTitel.Contains("BSN");
                }
            }


            // Bij Lijst4 inclusief NAW optie 'Copy to Clipboard god instellen)
            if (PDFSETTINGS.Lijstgegevens.Code == "4")
            {
                //cbClipboard.Visible =  (lblSubform.Text.Contains("NAW") && PDFSETTINGS.Lijstgegevens.Code == "4")      ; //rbLijst1.Checked;
                cbClipboard.Enabled = (lblSubform.Text.Contains("NAW") && PDFSETTINGS.Lijstgegevens.Code == "4"); //rbLijst1.Checked;
                if (!cbClipboard.Enabled)
                    cbClipboard.Checked = false;
                ControlsUitlijnen(gbInstellingen);
            }

        }

        private void listViewProjecten_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            // Als het projct grijs is en uitgevinkt wordt; dan ook maargelijk verwijderen!
            if (e.Item.Checked == false &&
                e.Item.ForeColor != Color.Black)
            {
                e.Item.Remove();
            }

            // (2014-06-27) Alles geselecteerd aanpassen (Als zoekterm is ingevuld, dan nooit alles selecteren aanzetten)
            if (lvProjecten.Focused)
                if (tbSearch.TextLength > 0)
                {
                    if (lvProjecten.CheckedItems.Count != lvProjecten.Items.Count)
                        cbAllProjecten.Checked = false;
                }
                else
                    cbAllProjecten.Checked = (lvProjecten.CheckedItems.Count == lvProjecten.Items.Count);

        }

        private void listViewVakmannen_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            // Als de vakman grijs is en uitgevinkt wordt; dan ook maargelijk verwijderen!
            if (e.Item.Checked == false &&
                e.Item.ForeColor != Color.Black)
            {
                e.Item.Remove();
            }

            // (2014-06-27) Alles geselecteerd aanpassen (Als zoekterm is ingevuld, dan nooit alles selecteren aanzetten)
            if (lvVakmannen.Focused)
            {
                if (tbSearch.TextLength > 0)
                {
                    if (lvVakmannen.CheckedItems.Count != lvVakmannen.Items.Count)
                        cbAllVakmannen.Checked = false;
                }
                else
                    cbAllVakmannen.Checked = (lvVakmannen.CheckedItems.Count == lvVakmannen.Items.Count);
            }
        }


        private void listViewProjecten_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvProjecten.SelectedItems.Count > 0)
                lvProjecten.SelectedItems[0].Checked = !lvProjecten.SelectedItems[0].Checked;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Okay, de backgroundworkr is finished dus nu kunnn we de boxjes echt vullen!
            //ET.Log(string.Format("Background procees reported 'Finished'. BACKGROUNDFINISHED value = {0}",BACKGROUNDFINNISCHED.ToString()));
            BACKGROUNDFINNISCHED = true;

            // Indien Userfriendly-setting een startdatum hebben gezet dan kloppn de ggevens waarschijnlijk niet
            // dus even opnieuw datum forceren!
            if (pSTARTDATUM > DateTime.MinValue)
            {
                DateTime SetDateAgain = pSTARTDATUM;
                pubUpdaten = false; pSTARTDATUM = pSTARTDATUM.AddDays(-1);
                pSTARTDATUM = SetDateAgain;
            }
            //Vul_Alle_SelectieListBoxen();
        }

        private void cbShowAllTot_CheckedChanged(object sender, EventArgs e)
        {
            //ShowOrHideShowAlls();
            /// XXXX (2014-07-14)
            //pSETDATA(pSTARTDATUM, pEINDEDATUM);
            if (cbShowAllTot.Focused)
            {
                this.Cursor = Cursors.WaitCursor;
                Cursor.Current = Cursors.WaitCursor;
                WijzigPeriode();
                this.Cursor = Cursors.Default;
                Cursor.Current = Cursors.Default;
            }
            /// XXXX (2014-07-14)
        }

        private void cbShowAllVanaf_CheckedChanged(object sender, EventArgs e)
        {
            //ShowOrHideShowAlls();
            /// XXXX (2014-07-14)
            //pSETDATA(pSTARTDATUM.AddDays(1), pEINDEDATUM);
            if (cbShowAllVan.Focused)
            {
                //HideAll.Visible = true;
                //tlpHoofd.Enabled = false;
                //tlpHoofd.Refresh();
                //HideAll.Refresh();
                //Application.DoEvents();
                this.Cursor = Cursors.WaitCursor;
                Cursor.Current = Cursors.WaitCursor;
                WijzigPeriode();
                this.Cursor = Cursors.Default;
                Cursor.Current = Cursors.Default;
                //HideAll.Visible = false;
                //tlpHoofd.Visible = true;
                //tlpHoofd.Enabled = true ;

            }
            /// XXXX (2014-07-14)
    
        }

private void listViewVakmannen_MouseHover(object sender, EventArgs e)
{
    if (!tbSearch.Focused)
    lvVakmannen.Focus();
}

private void listViewVakmannen_MouseLeave(object sender, EventArgs e)
{
    if (!tbSearch.Focused)
        tabControlProjecten.Focus();
}

private void tabControlProjecten_MouseHover(object sender, EventArgs e)
{
}


/// <summary>
/// Remove all red items from Listview
/// </summary>
/// <param name="sender"></param>
private void lv_removeAllRed(ListView sender)
{
    foreach (ListViewItem LI in sender.Items)
    {
        if (LI.ForeColor == ROOD)
            LI.Remove();
    }
}


private void lv_Cheked(object sender, ItemCheckedEventArgs e)
{

    if (((ListView)sender).Focused)
    {
        if (e.Item.ForeColor == ROOD && !e.Item.Checked)
            e.Item.Remove();

        Maak_ListboxItems_Grijs();//(sender);

        // En nu, á lá EXCEL select-all aan of uitzetten
        switch (((ListView)sender).Name)
        {
            case "lvContracten":
                cbAllContracten.CheckedChanged -= new EventHandler(this.cbAll_CheckedChanged);
                cbAllContracten.Checked = (lvContracten.CheckedItems.Count == lvContracten.Items.Count);
                cbAllContracten.CheckedChanged += new EventHandler(this.cbAll_CheckedChanged);
                break;
            case "lvRegistratiebevoegd":
                cbAllUitvoerders.CheckedChanged -= new EventHandler(this.cbAll_CheckedChanged);
                cbAllUitvoerders.Checked = (lvRegistratiebevoegd.CheckedItems.Count == lvRegistratiebevoegd.Items.Count);
                cbAllUitvoerders.CheckedChanged += new EventHandler(this.cbAll_CheckedChanged);
                break;
            case "lvArbeidsrelaties":
                cbAllArbeidsrelaties.CheckedChanged -= new EventHandler(this.cbAll_CheckedChanged);
                cbAllArbeidsrelaties.Checked = (lvArbeidsrelaties.CheckedItems.Count == lvArbeidsrelaties.Items.Count);
                cbAllArbeidsrelaties.CheckedChanged += new EventHandler(this.cbAll_CheckedChanged);
                break;
            case "lvKetenpartners":
                cbAllKetenpartners.CheckedChanged -= new EventHandler(this.cbAll_CheckedChanged);
                cbAllKetenpartners.Checked = (lvKetenpartners.CheckedItems.Count == lvKetenpartners.Items.Count);
                cbAllKetenpartners.CheckedChanged += new EventHandler(this.cbAll_CheckedChanged);
                break;
            case "lvFuncties":
                cbAllFuncties.CheckedChanged -= new EventHandler(this.cbAll_CheckedChanged);
                cbAllFuncties.Checked = (lvFuncties.CheckedItems.Count == lvFuncties.Items.Count);
                cbAllFuncties.CheckedChanged += new EventHandler(this.cbAll_CheckedChanged);
                break;
            case "lvNB":
                cbAllNietBeschikbaar.CheckedChanged -= new EventHandler(this.cbAll_CheckedChanged);
                cbAllNietBeschikbaar.Checked = (lvNB.CheckedItems.Count == lvNB.Items.Count);
                cbAllNietBeschikbaar.CheckedChanged += new EventHandler(this.cbAll_CheckedChanged);
                break;
            case "lvProjecten":
                cbAllProjecten.CheckedChanged -= new EventHandler(this.cbAll_CheckedChanged);
                // let op, als Search-text is ingevuld en alle projecten zijn geselecteerd, dan niet persé Alles Selecteren aanvinken
                if (tbSearch.Text.Length == 0)
                    cbAllProjecten.Checked = (lvProjecten.CheckedItems.Count == lvProjecten.Items.Count);
                else
                    cbAllProjecten.Checked = (lvProjecten.CheckedItems.Count == lvProjecten.Items.Count
                        && lvProjecten.Items.Count == pdfUniekeMandagenDezePeriode.Select(x => x.ProjectId).Distinct().ToList().Count );
                cbAllProjecten.CheckedChanged += new EventHandler(this.cbAll_CheckedChanged);
                break;
            case "lvVakmannen":
                cbAllVakmannen.CheckedChanged -= new EventHandler(this.cbAll_CheckedChanged);
                // let op, als Search-text is ingevuld en alle projecten zijn geselecteerd, dan niet persé Alles Selecteren aanvinken
                if (tbSearch.Text.Length == 0)
                    cbAllVakmannen.Checked = (lvVakmannen.CheckedItems.Count == lvVakmannen.Items.Count);
                else
                    cbAllVakmannen.Checked = (lvVakmannen.CheckedItems.Count == lvVakmannen.Items.Count
                        && lvVakmannen.Items.Count  == pdfUniekeMandagenDezePeriode.Select(x => x.VakmanId).Distinct().ToList().Count );
                cbAllVakmannen.CheckedChanged += new EventHandler(this.cbAll_CheckedChanged);
                break;
        }
    }
}


/// <summary>
/// Maak optoes in listboxen grijs indien ze op basis VAN DE ANDERE aangevinkte listboxjes niet gekozen kunnen worden
/// </summary>
private void Maak_ListboxItems_Grijs()//(object sender)
{
    Color Aanwezig = Color.Black;//.Green;
    Color NietAanwezig = Color.DarkGray;//.Red;
    // Bepaal of de ProjectenLijst actief is (P = true) of de Vakmannenlijst (P = False)
    bool Projecten = (tabControlProjecten.SelectedIndex == 0 ? true : false);

    List<int> MogelijkeIDs = new List<int>();

    // Get selected ID's from all checkboxen
    List<int> _sel_arbeidsr = GetIDs(lvArbeidsrelaties,0);
    List<int> _sel_contract = GetIDs(lvContracten,0);
    List<int> _sel_functies = GetIDs(lvFuncties,0);
    List<int> _sel_ketenptr = GetIDs(lvKetenpartners,0);
    List<int> _sel_nietbscb = GetIDs(lvNB,0);
    List<int> _sel_regbevgd = GetIDs(lvRegistratiebevoegd,0);
    List<int> _sel_PV = Projecten ? GetIDs(lvProjecten, 0) : GetIDs(lvVakmannen,0);

    // Contracten Checkbox
    {
        MogelijkeIDs = pdfUniekeMandagenDezePeriode.Where
            (x =>
            _sel_arbeidsr.Contains(x.ArbeidsrelatieVW) &&
            //_sel_contract.Contains(x.ContractVW) &&
            _sel_functies.Contains(x.FunctieVW) &&
            _sel_ketenptr.Contains(x.KetenpartnerVW) &&
            _sel_nietbscb.Contains(x.NietBeschikbaarVW) &&
            _sel_regbevgd.Contains(x.ProjectleiderId) &&
            Projecten ? _sel_PV.Contains(x.ProjectId) : _sel_PV.Contains(x.VakmanId)
            ).Select(x => 
                x.ContractVW).Distinct().ToList();

        foreach (ListViewItem LI in lvContracten.Items)
        {
            if (MogelijkeIDs.Contains(LI.SubItems[1].Text.ToInt()))
                LI.ForeColor = Aanwezig;
            else
                if (LI.ForeColor != ROOD)
                    LI.ForeColor = NietAanwezig;
        }
    }

    // Arbeidsrelaties checkbox
    {
        MogelijkeIDs = pdfUniekeMandagenDezePeriode.Where
            (x =>
            //_sel_arbeidsr.Contains(x.ArbeidsrelatieVW) &&
            _sel_contract.Contains(x.ContractVW) &&
            _sel_functies.Contains(x.FunctieVW) &&
            _sel_ketenptr.Contains(x.KetenpartnerVW) &&
            _sel_nietbscb.Contains(x.NietBeschikbaarVW) &&
            _sel_regbevgd.Contains(x.ProjectleiderId) &&
            Projecten ? _sel_PV.Contains(x.ProjectId) : _sel_PV.Contains(x.VakmanId)
            ).Select(x =>
                x.ArbeidsrelatieVW).Distinct().ToList();

        foreach (ListViewItem LI in lvArbeidsrelaties.Items)
        {
            if (MogelijkeIDs.Contains(LI.SubItems[1].Text.ToInt()))
                LI.ForeColor = Aanwezig;
            else
                if (LI.ForeColor != ROOD)
                    LI.ForeColor = NietAanwezig;
        }
    }

    // Ketenpartners
    {
        MogelijkeIDs = pdfUniekeMandagenDezePeriode.Where
            (x =>
            _sel_arbeidsr.Contains(x.ArbeidsrelatieVW) &&
            _sel_contract.Contains(x.ContractVW) &&
            _sel_functies.Contains(x.FunctieVW) &&
            //_sel_ketenptr.Contains(x.KetenpartnerVW) &&
            _sel_nietbscb.Contains(x.NietBeschikbaarVW) &&
            _sel_regbevgd.Contains(x.ProjectleiderId) &&
            Projecten ? _sel_PV.Contains(x.ProjectId) : _sel_PV.Contains(x.VakmanId)
            ).Select(x =>
                x.KetenpartnerVW).Distinct().ToList();
        foreach (ListViewItem LI in lvKetenpartners.Items)
        {
            if (MogelijkeIDs.Contains(LI.SubItems[1].Text.ToInt()))
                LI.ForeColor = Aanwezig;
            else
                if (LI.ForeColor != ROOD)
                    LI.ForeColor = NietAanwezig;
        }
    }

    // Functies
    {
        MogelijkeIDs = pdfUniekeMandagenDezePeriode.Where
            (x =>
            _sel_arbeidsr.Contains(x.ArbeidsrelatieVW) &&
            _sel_contract.Contains(x.ContractVW) &&
            //_sel_functies.Contains(x.FunctieVW) &&
            _sel_ketenptr.Contains(x.KetenpartnerVW) &&
            _sel_nietbscb.Contains(x.NietBeschikbaarVW) &&
            _sel_regbevgd.Contains(x.ProjectleiderId) &&
            Projecten ? _sel_PV.Contains(x.ProjectId) : _sel_PV.Contains(x.VakmanId)
            ).Select(x =>
                x.FunctieVW).Distinct().ToList();

        foreach (ListViewItem LI in lvFuncties.Items)
        {
            if (MogelijkeIDs.Contains(LI.SubItems[1].Text.ToInt()))
                LI.ForeColor = Aanwezig;
            else
                if (LI.ForeColor != ROOD)
                    LI.ForeColor = NietAanwezig;
        }
    }

    // Registratiebevoegd
    {
        MogelijkeIDs = pdfUniekeMandagenDezePeriode.Where
            (x =>
            _sel_arbeidsr.Contains(x.ArbeidsrelatieVW) &&
            _sel_contract.Contains(x.ContractVW) &&
            _sel_functies.Contains(x.FunctieVW) &&
            _sel_ketenptr.Contains(x.KetenpartnerVW) &&
            _sel_nietbscb.Contains(x.NietBeschikbaarVW) //&&
            //_sel_regbevgd.Contains(x.ProjectleiderId) &&
            && Projecten ? _sel_PV.Contains(x.ProjectId) : _sel_PV.Contains(x.VakmanId)
            ).Select(x =>
                x.ProjectleiderId).Distinct().ToList();

        foreach (ListViewItem LI in lvRegistratiebevoegd.Items)
        {
            if (MogelijkeIDs.Contains(LI.SubItems[1].Text.ToInt()))
                LI.ForeColor = Aanwezig;
            else
                if (LI.ForeColor != ROOD)
                    LI.ForeColor = NietAanwezig;
        }
    }

    // Niet beschikbaar
    {
        MogelijkeIDs = pdfUniekeMandagenDezePeriode.Where
            (x =>
            _sel_arbeidsr.Contains(x.ArbeidsrelatieVW) &&
            _sel_contract.Contains(x.ContractVW) &&
            _sel_functies.Contains(x.FunctieVW) &&
            _sel_ketenptr.Contains(x.KetenpartnerVW) &&
            //_sel_nietbscb.Contains(x.NietBeschikbaarVW) &&
            _sel_regbevgd.Contains(x.ProjectleiderId) &&
            Projecten ? _sel_PV.Contains(x.ProjectId) : _sel_PV.Contains(x.VakmanId)
            ).Select(x =>
                x.NietBeschikbaarVW).Distinct().ToList();

        foreach (ListViewItem LI in lvNB.Items)
        {
            if (MogelijkeIDs.Contains(LI.SubItems[1].Text.ToInt()))
                LI.ForeColor = Aanwezig;
            else
                if (LI.ForeColor != ROOD)
                    LI.ForeColor = NietAanwezig;
        }
    }

    // Projecten of Vakmannen
    if (Projecten)
    // Projecten
    {
        MogelijkeIDs = pdfUniekeMandagenDezePeriode.Where
            (x =>
            _sel_arbeidsr.Contains(x.ArbeidsrelatieVW) &&
            _sel_contract.Contains(x.ContractVW) &&
            _sel_functies.Contains(x.FunctieVW) &&
            _sel_ketenptr.Contains(x.KetenpartnerVW) &&
            _sel_nietbscb.Contains(x.NietBeschikbaarVW) &&
            _sel_regbevgd.Contains(x.ProjectleiderId)
            ).Select(x =>
                x.ProjectId).Distinct().ToList();
        foreach (ListViewItem LI in lvProjecten.Items)
        {
            if (MogelijkeIDs.Contains(LI.SubItems[4].Text.ToInt()))
                LI.ForeColor = Aanwezig;
            else
                if (LI.ForeColor != ROOD)
                    LI.ForeColor = NietAanwezig;
        }
    }
    else
    // VAKMANNEN
    {
        MogelijkeIDs = pdfUniekeMandagenDezePeriode.Where
            (x =>
            _sel_arbeidsr.Contains(x.ArbeidsrelatieVW) &&
            _sel_contract.Contains(x.ContractVW) &&
            _sel_functies.Contains(x.FunctieVW) &&
            _sel_ketenptr.Contains(x.KetenpartnerVW) &&
            _sel_nietbscb.Contains(x.NietBeschikbaarVW) &&
            _sel_regbevgd.Contains(x.ProjectleiderId)
            ).Select(x =>
                x.VakmanId).Distinct().ToList();
        foreach (ListViewItem LI in lvVakmannen.Items)
        {
            if (MogelijkeIDs.Contains(LI.SubItems[4].Text.ToInt()))
                LI.ForeColor = Aanwezig;
            else
                if (LI.ForeColor != ROOD)
                    LI.ForeColor = NietAanwezig;
        }
    }


} //


private void cb_MouseLeave(object sender, EventArgs e)
{
    tlpHoofd.Focus();
}

/// <summary>
/// Algemene Module:
/// Check or Uncheck de onderliggende Lijst
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
private void cbAll_CheckedChanged(object sender, EventArgs e)
{
    CheckOrUncheck_Lijst(((CheckBox)sender).Name);
    if (((CheckBox)sender).Focused) Maak_ListboxItems_Grijs();//(lvContracten);
}

/// <summary>
/// Algemene Module:
/// Met rechts klikken van Check-ALL Up wordt gebruikt om de checkbox uit te zonder verdere gevolgen
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
private void cbAll_MouseUp(object sender, MouseEventArgs e)
{
    if (e.Button == MouseButtons.Right && ((CheckBox)sender).Checked)
    {
        switch (((CheckBox) sender).Name)
        {
            case "cbAllContracten":
                cbAllContracten.CheckedChanged -= new EventHandler(this.cbAll_CheckedChanged);
                cbAllContracten.Checked = !cbAllContracten.Checked;
                cbAllContracten.CheckedChanged += new EventHandler(this.cbAll_CheckedChanged);
                break;
            case "cbAllUitvoerders":
                cbAllUitvoerders.CheckedChanged -= new EventHandler(this.cbAll_CheckedChanged);
                cbAllUitvoerders.Checked = !cbAllUitvoerders.Checked;
                cbAllUitvoerders.CheckedChanged += new EventHandler(this.cbAll_CheckedChanged);
                break;
            case "cbAllArbeidsrelaties":
                cbAllArbeidsrelaties.CheckedChanged -= new EventHandler(this.cbAll_CheckedChanged);
                cbAllArbeidsrelaties.Checked = !cbAllArbeidsrelaties.Checked;
                cbAllArbeidsrelaties.CheckedChanged += new EventHandler(this.cbAll_CheckedChanged);
                break;
            case "cbAllKetenpartners":
                cbAllKetenpartners.CheckedChanged -= new EventHandler(this.cbAll_CheckedChanged);
                cbAllKetenpartners.Checked = !cbAllKetenpartners.Checked;
                cbAllKetenpartners.CheckedChanged += new EventHandler(this.cbAll_CheckedChanged);
                break;
            case "cbAllFuncties":
                cbAllFuncties.CheckedChanged -= new EventHandler(this.cbAll_CheckedChanged);
                cbAllFuncties.Checked = !cbAllFuncties.Checked;
                cbAllFuncties.CheckedChanged += new EventHandler(this.cbAll_CheckedChanged);
                break;
            case "cbAllNietBeschikbaar":
                cbAllNietBeschikbaar.CheckedChanged -= new EventHandler(this.cbAll_CheckedChanged);
                cbAllNietBeschikbaar.Checked = !cbAllNietBeschikbaar.Checked;
                cbAllNietBeschikbaar.CheckedChanged += new EventHandler(this.cbAll_CheckedChanged);
                break;
            case "cbAllProjecten":
                cbAllProjecten.CheckedChanged -= new EventHandler(this.cbAll_CheckedChanged);
                cbAllProjecten.Checked = !cbAllProjecten.Checked;
                cbAllProjecten.CheckedChanged += new EventHandler(this.cbAll_CheckedChanged);
                break;
            case "cbAllVakmannen":
                cbAllVakmannen.CheckedChanged -= new EventHandler(this.cbAll_CheckedChanged);
                cbAllVakmannen.Checked = !cbAllVakmannen.Checked;
                cbAllVakmannen.CheckedChanged += new EventHandler(this.cbAll_CheckedChanged);
                break;
            default:
                MessageBox.Show("Foute aanroep module 'cbAll_MouseUP', geef '" + ((CheckBox) sender).Name + "' door aan Marino");
                break;
        }
    }
}


/// <summary>
/// Save All settings
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
        
private void EasySave_Click(object sender, EventArgs e)
{

    Easy easy= new Easy();
    easy.Lijstcode = SublijstCode;
    //easy.Gebruiker = (PDFSETTINGS.Gebruiker.IsAdministrator ? "Admin" : "Gebruiker");
    easy.Gebruiker = EasyUser.SelectedItem.ToString(); //(cbProjectLeider.Checked ? "Gebruiker" : "Admin");


    // PERIODE (Relatief tov huidige tijd behalve Datum en Periode)
    DateTime NU =  DateTime.Now.Date;
    if (cbbJaar.Visible)
    {
        easy.Periode = "Jaar";
        easy.PeriodeOffset = cbbJaar.SelectedIndex - PeriodeSelectieNU[0]; // Offset tov 'NU'
    }
    else
        if (cbbKwartaal.Visible)
        {
            easy.Periode = "Kwartaal";
            easy.PeriodeOffset = cbbKwartaal.SelectedIndex - PeriodeSelectieNU[1]; // Offset tov 'NU'
        }
        else
            if (cbbMaand.Visible)
            {
                easy.Periode = "Maand";
                easy.PeriodeOffset = cbbMaand.SelectedIndex - PeriodeSelectieNU[2]; // Offset tov 'NU'
            }
            else
                if (cbbWeek.Visible)
                {
                    easy.Periode = "Week";
                    easy.PeriodeOffset = cbbWeek.SelectedIndex - PeriodeSelectieNU[3]; // Offset tov 'NU'
                }
                else
                    if (cbbDag.Visible)
                    {
                        easy.Periode = "Dag";
                        easy.PeriodeOffset = cbbDag.SelectedIndex - PeriodeSelectieNU[4]; // Offset tov 'NU'
                    }
                    else
                        // Zowel Datum als Periode: Huidige selectie overnemen, dus NIET relatief tov nu.
                        if (rbDatum.Checked)
                            {
                                easy.Periode = "Datum";
                                easy.Van = pSTARTDATUM.ToShortDateString();
                                easy.Tot = pEINDEDATUM.ToShortDateString();
                            }
                            else
                                if (rbAnders.Checked)
                                {
                                    easy.Periode = "Periode";
                                    easy.Van = pSTARTDATUM.ToShortDateString();
                                    easy.Tot = pEINDEDATUM.ToShortDateString();
                                }
    // CONTRACT
    if (cbAllContracten.Checked)
        easy.SelectAll_Contract = true;
    else easy.Selecties_Contract = string.Join(",", GetIDs(lvContracten, 0));

    //REGISTRATIEBEVOEGD
    if (cbAllUitvoerders.Checked)
        easy.SelectAll_RegBev = true;
    else
    {
        // Als ME cheked, dan Alleen ingelogde gebruiker selecteren (is relatief, dus registratiebevoegde = '0';
        //easy.Selecties_RegBev = (EasyME.Checked ? "0" : string.Join(",", GetIDs(lvRegistratiebevoegd, 0)));
        easy.Selecties_RegBev =
            (EasyME.Checked ? "0" : "");
        if (GetIDs(lvRegistratiebevoegd, 0).Count > 1)
            easy.Selecties_RegBev += ", " + string.Join(",", GetIDs(lvRegistratiebevoegd, 0));

        //if (cbProjectLeider.Checked && GetIDs(lvRegistratiebevoegd, 0).Count == 1)
        //{
        //    DialogResult answer = MessageBox.Show("Indien Registratiebevoegde niet aanwezig, dan Allen (Yes) or Niets (NO) selecteren?","Registratiebevoegde",MessageBoxButtons.YesNo);
        //    if (answer == DialogResult.Yes) easy.RegBev_Afwezig_AllenOrNone = 'A'; else easy.RegBev_Afwezig_AllenOrNone = 'N';
        //};
    }

    //ARBEIDSRELATIES
    if (cbAllArbeidsrelaties.Checked)
        easy.SelectAll_ArbeidRel = true;
    else easy.Selecties_ArbeidRel = string.Join(",", GetIDs(lvArbeidsrelaties, 0));

    //KETENPARTNERS
    if (cbAllKetenpartners.Checked)
        easy.SelectAll_KetenP = true;
    else easy.Selecties_KetenP= string.Join(",", GetIDs(lvKetenpartners, 0));

    //FUNCTIES
    if (cbAllFuncties.Checked)
        easy.SelectAll_Functies = true;
    else easy.Selecties_Functies = string.Join(",", GetIDs(lvFuncties, 0));

    //BESCHIKBAAR
    if (cbAllNietBeschikbaar.Checked)
        easy.SelectAll_Besch = true;
    else easy.Selecties_Besch = string.Join(",", GetIDs(lvNB, 0));

    //PROJECTEN/VAKMANNEN
    if (cbShowAllVan.Checked) easy.ShowAllVan_PV = true;
    if (cbShowAllTot.Checked) easy.ShowAllTot_PV = true;
    easy.SelectAll_PV = false;
    if (tabControlProjecten.SelectedIndex == 0)
    {
        easy.PV = 'P';
        if (cbAllProjecten.Checked)
            easy.SelectAll_PV = true;
        else
            easy.Selecties_PV = string.Join(",", GetIDs(lvProjecten, 0));
    }
    else
    {
        easy.PV = 'V';
        if (cbAllVakmannen.Checked)
            easy.SelectAll_PV = true;
        else
            easy.Selecties_PV = string.Join(",", GetIDs(lvVakmannen, 0));
    }

    //INSTELLINGEN
    if (cbShowAllWeek.Checked) easy.ShowAll_Week = true;


    // Save all settings
    try
    {
        using (dbLPdfOutputDataContext myDb = new dbLPdfOutputDataContext(xPdfOutputConnectieString))
        {
            var xxx = myDb.Easies.Where(x => x.Lijstcode == easy.Lijstcode && x.Gebruiker == easy.Gebruiker).FirstOrDefault();
            if (xxx != null)
            {
                myDb.Easies.DeleteOnSubmit(xxx);
                myDb.SubmitChanges();
            }
            myDb.Easies.InsertOnSubmit(easy);
            myDb.SubmitChanges();
            MessageBox.Show("Gegegevens zijn opgeslagen");

        }
    }
    catch (Exception dsds)
    {
        MessageBox.Show("Er is iets fout gegaan!\n\n"+dsds.ToString());
    }

}

private void EasyLoad_Click(object sender, EventArgs e)
{
    EasySet();
}

private void gbEasy_MouseHover(object sender, EventArgs e)
{
    //gbEasy.BringToFront();
    //gbEasy.Focus();
    //foreach (Control x in gbEasy.Controls)
    //    x.Visible = true;
}

private void gbEasy_Leave(object sender, EventArgs e)
{
    gbEasy.SendToBack();
    gbEasy.Visible = false;
    //foreach (Control x in gbEasy.Controls)
    //    x.Visible = false;
}

private void EasyUser_SelectedIndexChanged(object sender, EventArgs e)
{
}

private void listViewProjecten_MouseHover(object sender, EventArgs e)
{
    if (!tbSearch.Focused)
        lvProjecten.Focus();
}

private void listViewProjecten_MouseLeave(object sender, EventArgs e)
{
    if (!tbSearch.Focused)
        tabControlProjecten.Focus();
}

private void llbEasy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
{
    gbEasy.Visible = true;
    gbEasy.BringToFront();
    ShowCalendar(sender, gbEasy);
    gbEasy.Focus();

    //gbEasy.Location = llbEasy.Location;
}

private void lv_Clear_Selection(object sender, EventArgs e)
{
    ListView LV = ((ListView)sender);
    if (LV.SelectedIndices.Count == 0)return;
    int i = LV.SelectedIndices[0];
    LV.Items[i].Checked = !LV.Items[i].Checked;
    LV.SelectedIndices.Clear();
    //((ListView)sender).SelectedIndices.Clear();

}


private void picStopSearch_Click(object sender, EventArgs e)
{
    tbSearch.Visible = false;
    tbSearchClick.Visible = true;
    tbSearch.Text = "";
    picStopSearch.Visible = false;
}

private void tbSearch_TextChanged(object sender, EventArgs e)
{
    if (tbSearch.Visible && tbSearch.Text.Length > 0)
    {
        if (!picStopSearch.Visible)
        {
            picStopSearch.Visible = true;
            picStopSearch.BringToFront();
        }
    }
    
    VulProjectenOfVakmannenList();
    Maak_ListboxItems_Grijs(); // Ook grijsmaken, want VulProjectenOfVakmannen vult altijd met zwart
}

private void tbSearchClick_Click(object sender, EventArgs e)
{
    tbSearchClick.Visible = false;
    tbSearch.Visible = true;
    tbSearch.Focus();
}

private void tbSearch_MouseLeave(object sender, EventArgs e)
{
    return;
    if (tbSearch.Text.Length == 0)
    {
        tbSearch.Visible = false;
        tbSearchClick.Visible = true;
        picStopSearch.Visible = false;
    }
}

private void tbSearch_Leave(object sender, EventArgs e)
{
    if (tbSearch.Text.Length == 0)
    {
        tbSearch.Visible = false;
        tbSearchClick.Visible = true;
        picStopSearch.Visible = false;
    }
}

private void lvDatum_SelectedIndexChanged(object sender, EventArgs e)
{

}


private void timer2_Tick(object sender, EventArgs e)
{
    
    timer2.Enabled = false;
}
    } //public partial class USMario
} // namespace MDR2Excel

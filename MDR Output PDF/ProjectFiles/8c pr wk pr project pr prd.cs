using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using EvoPdf;
using System.Globalization;

namespace MDR2PDF
{
    public partial class _8c_pr_wk_pr_project_pr_prd : UserControl
    {
        public EvoTools ET = new EvoTools();
        public int Aantal_Regels_per_Pagina = -1; // Pagina Totaalregel uit indien '-1'
        public int[] PaginaTotalen = new int[8];

        public string[] NewRecordNames = new string[12];
        public string[] NewRecordValues = new string[12];

        // EVO HTML Variabelen
        static string pathcontainer = System.IO.Path.Combine(Environment.CurrentDirectory, "html/Lijst8b/Container.html");
        static string pathhtmlpage = System.IO.Path.Combine(Environment.CurrentDirectory, "html/lijst8b/VakmannenOverzicht.html");
        static string pathhtmlrecord = System.IO.Path.Combine(Environment.CurrentDirectory, "html/Lijst8b/VakmannenOverzichtItem.html");

        static string container = File.ReadAllText(pathcontainer);
        static string htmlpage = File.ReadAllText(pathhtmlpage);
        static string htmlrecord = File.ReadAllText(pathhtmlrecord);

        public string htmlBody = string.Empty;
        public string title = htmlrecord;

        public string records = string.Empty;
        public string newRecord = htmlrecord;

        public int RegelsOpDezePagina = 0;

        public _8c_pr_wk_pr_project_pr_prd()
        {
            InitializeComponent();
        }


        internal void Maak_Lijst(DateTime pStartDatum, DateTime pEindDatum, List<USMario.xMANDAG> selxMANDAGEN, List<USMario.JoinedProjectGegevens> _Projecten)
        {
            // Projectgegevens

            //
            // Deel periode in x-aantal weken in
            //

            // Week van Startdatum begin op maandag
            List<DateTime> WeekStart = new List<DateTime>();
            List<DateTime> WeekEinde = new List<DateTime>();
            // Trek DayofWeek van startdatum af om de maandag van die week te bepalen
            DateTime Maandag = pStartDatum.AddDays((1 - ((int)pStartDatum.DayOfWeek == 0 ? 7 : (int)pStartDatum.DayOfWeek)));
            DateTime PeriodeStart = Maandag;
            int StartWeekNr = new GregorianCalendar(GregorianCalendarTypes.Localized).GetWeekOfYear(PeriodeStart, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            do
            {
                // Voorlopig nog even geen 'ShowAll', dus WeekStart niet eerder dan startdatum
                if (Maandag < pStartDatum)
                    WeekStart.Add(pStartDatum);
                else
                    WeekStart.Add(Maandag);

                Maandag = Maandag.AddDays(7);

                // Voorlopig nog even geen 'ShowAll', dus WeekEinde niet later dan einddatum
                if (Maandag > pEindDatum)
                    WeekEinde.Add(pEindDatum.AddDays(1));
                else
                    WeekEinde.Add(Maandag);
            } while (Maandag <= pEindDatum);
            DateTime PeriodeEinde = Maandag;

            // NIEUW: lijst met vakmannen die op één dag gedeeld worden over verschillende projecten
            // mandagen per dag persoon per project
            var _MandagenPerDagPerPersoonPerProject = selxMANDAGEN
                .Select(x => new
                {
                    Datum = x.Mandag.Begintijd.Date,
                    VakmanID = x.Mandag.VakmanId,
                    CodeDienstbetrekking = x.Dienstbetrekking,
                    PLID = x.Mandag.ProjectId
                }
                    )
                .Distinct()
                .ToList();
            // Idem, maar nu group by { vakman, count(aantal verschillende projecten)"} 
            var _MandagenPerDag_PerVakman_AantalProjecten = _MandagenPerDagPerPersoonPerProject.Select(x => x)
                .GroupBy(x => new { x.Datum, x.VakmanID, x.CodeDienstbetrekking })
                .Select(x => new { x.Key, Aantal = x.Count() })
                .ToList();
            // Pak alleen die mandagen die op één over meer dan 1 project lopen
            _MandagenPerDag_PerVakman_AantalProjecten = _MandagenPerDag_PerVakman_AantalProjecten.Where(x => x.Aantal > 1).ToList();

            // En dan als laatst: alle losse mandaggegevens waarvan je de key hierboven bepaalt hebt. is het nog enigszinds duidelijk?
            var _MeerDan1ProjectenPerVakmanPerDag =
                (from x in _MandagenPerDagPerPersoonPerProject
                 join y in _MandagenPerDag_PerVakman_AantalProjecten
                 on new { x.Datum, x.VakmanID } equals new { y.Key.Datum, y.Key.VakmanID }
                 select x).ToList();

            // Eerst even Totaal alle vakmannen tesamen
            //List<char> Dienstbetrekkingscode = new List<int>("1,3,U,A".Split(',').Select(n => char.Parse(n)).ToList());
            double DagenT, UrenT;
            double DagenD = 0;
            double UrenD = 0;

            string[] RegelVars = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

            // ****************************************************************************************************
            //
            // Start opbouwen van de totaalregels
            // Per 'DO'
            //  Bereken het totaal van de betreffende periode;
            //  Berken per project het totaal van de betreffende periode
            // 
            // We beginnen met Totallperiode van periodestart tot periodeeind
            // vlak voor de 'WHILE' starten we met de 1e weekperiode te selecteren, daarna alle andere weekperiodes
            //
            // ****************************************************************************************************

            // Startsetting zodat we de gehele periode pakken
            DateTime SelectieStart = PeriodeStart;
            DateTime SelectieEinde = PeriodeEinde;
            string regelSamenvatting = "Totaal alle vakmannen";
            // WeekIndex gebruiken we pas nadat de 1e lus is doorlopen
            int WeekIndex = 0;
            do // Start met totaal-periode; Neem daarna alle weekperiodes
            {
                UrenT = 0; DagenT = 0;
                // Bereken voor elke dienstbetrekking het totaal over deze periode
                for (int c = -1; c <= 7; c += 2)
                {
                    // Uren
                    UrenD = selxMANDAGEN.Where(x =>
                        x.Mandag.Begintijd >= SelectieStart &&
                        x.Mandag.Eindtijd < SelectieEinde &&
                        (x.Dienstbetrekking == c || c == -1) // bij c = -1, alles tellen, anders selecteren op Code c
                        ).Sum(x => x.Mandag.Uren) * 60;
                    UrenD += selxMANDAGEN.Where(x =>
                        x.Mandag.Begintijd >= SelectieStart &&
                        x.Mandag.Eindtijd < SelectieEinde &&
                        (x.Dienstbetrekking == c || c == -1) // bij c = -1, alles tellen, anders selecteren op Code c
                        ).Sum(x => x.Mandag.Minuten);
                    UrenD /= 60.0;
                    // Dagen
                    DagenD = selxMANDAGEN.Where(x =>
                        x.Mandag.Begintijd >= SelectieStart &&
                        x.Mandag.Eindtijd < SelectieEinde &&
                        (x.Dienstbetrekking == c || c == -1) // bij c = -1, alles tellen, anders selecteren op Code c
                        ).Select(y => new { y.Vakman.MandagenReg.VakmanId, y.Mandag.Begintijd.Date }).Distinct().Count();
                    if (c == -1) // Totaal van alle codes
                    {
                        RegelVars[9] = (UrenD == 0) ? "&nbsp" : Math.Ceiling(UrenD).ToString();//ET.DecimaalUitijnen(UrenD, 1);
                        RegelVars[10] = (DagenD == 0) ? "&nbsp" : Math.Ceiling(DagenD).ToString(); //ET.DecimaalUitijnen(DagenD, 1);
                        RegelVars[0] = regelSamenvatting;

                    }
                    else // Totaal per dienstpetrekking (1 = Intern, 3 = ZZP, 5 = uitzendkracht, 7 = Extern)
                    {
                        RegelVars[c] = (UrenD == 0) ? "&nbsp" : Math.Ceiling(UrenD).ToString(); //ET.DecimaalUitijnen(UrenD, 1);
                        RegelVars[c + 1] = (DagenD == 0) ? "&nbsp" : Math.Ceiling(DagenD).ToString(); //ET.DecimaalUitijnen(DagenD, 1);
                    }
                }
                records += "<b>" + ET.HtmlFormat(htmlrecord, RegelVars) + "</b>";

                // Bereken nu PER PROJECT voor elke dienstbetrekking het totaal over deze periode
                foreach (int ProjectID in selxMANDAGEN
                    .OrderBy(x => x.Mandag.ProjectId)
                    .Select(x => x.Mandag.ProjectId).Distinct().ToList())
                {
                    UrenT = 0; DagenT = 0;
                    for (int c = 1; c <= 7; c += 2)
                    {
                        // Uren
                        UrenD = selxMANDAGEN.Where(x =>
                            x.Mandag.Begintijd >= SelectieStart &&
                            x.Mandag.Eindtijd < SelectieEinde &&
                            x.Mandag.ProjectId == ProjectID &&
                            x.Dienstbetrekking == c
                            ).Sum(x => x.Mandag.Uren) * 60;
                        UrenD += selxMANDAGEN.Where(x =>
                            x.Mandag.Begintijd >= SelectieStart &&
                            x.Mandag.Eindtijd < SelectieEinde &&
                            x.Mandag.ProjectId == ProjectID &&
                            x.Dienstbetrekking == c
                            ).Sum(x => x.Mandag.Minuten);
                        UrenD /= 60.0;
                        DagenD = selxMANDAGEN.Where(x =>
                            x.Mandag.Begintijd >= SelectieStart &&
                            x.Mandag.Eindtijd < SelectieEinde &&
                            x.Mandag.ProjectId == ProjectID &&
                            (x.Dienstbetrekking == c || c == -1) // bij c = -1, alles tellen, anders selecteren op Code c
                            ).Select(y => new { y.Vakman.MandagenReg.VakmanId, y.Mandag.Begintijd.Date }).Distinct().Count();
                        if (c != -1) // Totaal van alle codes
                        // Totaal per dienstpetrekking (1 = Intern, 3 = ZZP, 5 = uitzendkracht, 7 = Extern)
                        {
                            // Check of er dagen zijn die ook bij een andere vakman geteld zijn
                            foreach (var d in _MeerDan1ProjectenPerVakmanPerDag
                                .Where(x =>
                                    x.Datum >= SelectieStart &&
                                    x.Datum < SelectieEinde &&
                                    x.CodeDienstbetrekking == c &&
                                    x.PLID == ProjectID)
                                    )
                            {
                                var meerdere_dagen = _MeerDan1ProjectenPerVakmanPerDag
                                    .Where(y =>
                                        d.Datum == y.Datum &&
                                        d.VakmanID == y.VakmanID
                                        ).Count();
                                DagenD -= 1;
                                DagenD += 1.0 / meerdere_dagen;
                            }

                            // Totaal van dienstbetrekking invullen
                            RegelVars[c] = (UrenD == 0) ? "&nbsp" : Math.Ceiling(UrenD).ToString(); //ET.DecimaalUitijnen(UrenD, 1);
                            RegelVars[c + 1] = (DagenD == 0) ? "&nbsp" : Math.Ceiling(DagenD).ToString(); //ET.DecimaalUitijnen(DagenD, 1);
                            UrenT += UrenD;
                            DagenT += DagenD;
                        }
                    }

                    // RegelBegin en RegelTotalen toevoegen
                    RegelVars[0] = "<div style=\"float: left;width : 5%;margin :1px\"></div>" +
                        //_gebruikers.Where(x => x.ProjectleiderId == ProjectID).Select(x => x.Gebruikersnaam).First();
                        _Projecten.Where(x => x.MandagenReg.ProjectId == ProjectID).Select(x => x.Zeebregts.naam_project).First();
                    RegelVars[9] = (UrenT == 0) ? "&nbsp" : Math.Ceiling(UrenT).ToString(); //ET.DecimaalUitijnen(UrenT, 1);
                    RegelVars[10] = (DagenT == 0) ? "&nbsp" : Math.Ceiling(DagenT).ToString(); //ET.DecimaalUitijnen(DagenT, 1);

                    // Alle waarden zijn gevuld; Voeg regel toe aan HTML pagina
                    // NIEUW als er geen uren staan voor deze vakman, dan ook geen regel aan besteden dus alleen tonen als er uren gemaakt zijn
                    if (UrenT > 0)
                        records += ET.HtmlFormat(htmlrecord, RegelVars);

                }

                // bij de 1e lus was de periode ingesteld op totaalperiode, vanaf nu pakken we de weekperiodes
                if (WeekIndex < WeekStart.Count())
                {
                    SelectieStart = WeekStart[WeekIndex];
                    SelectieEinde = WeekEinde[WeekIndex];
                    regelSamenvatting = "Week: " + new GregorianCalendar(GregorianCalendarTypes.Localized).GetWeekOfYear(WeekStart[WeekIndex], CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                }
            } while (++WeekIndex <= WeekStart.Count());


            // Eco PDF eenmaken, vullen en tonen

            PdfConverter pdfConverter = new PdfConverter();
            pdfConverter.LicenseKey = "B4mYiJubiJiInIaYiJuZhpmahpGRkZE=";
            pdfConverter.LicenseKey = "EpyMnY6OnYyPjJ2Jk42djoyTjI+ThISEhA==";
            ET.AddHeaderElements(pdfConverter);


            string newPage = htmlpage;
            //newPage = newPage.Replace("[%vantot%]", PeriodeStart.ToShortDateString() + " / " + PeriodeEinde.ToShortDateString());
            newPage = newPage.Replace("[%vantot%]", WeekStart[0].ToShortDateString() + " / " + WeekEinde[WeekEinde.Count - 1].ToShortDateString());
            newPage = newPage.Replace("[%periode%]", "Week " +
                new GregorianCalendar(GregorianCalendarTypes.Localized).GetWeekOfYear(PeriodeStart, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
                + " tot en met week " +
                new GregorianCalendar(GregorianCalendarTypes.Localized).GetWeekOfYear(PeriodeEinde.AddDays(-1), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
                );


            // HTML regels toevoegen aan pagina
            newPage = newPage.Replace("[%OverzichtItems%]", records);
            htmlBody += newPage;

            // HTML Totaalplaatje genereren
            string fullHtml = container.Replace("[%Overzicht%]", htmlBody);

            // HTML: PDF creeren
            string outFilePath = System.IO.Path.Combine(Environment.CurrentDirectory, "ConvertHtmlString-" + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Millisecond + ".pdf");
            pdfConverter.SavePdfFromHtmlStringToFile(fullHtml, outFilePath);

            System.Diagnostics.Process.Start(outFilePath);
        }

    } //
} //

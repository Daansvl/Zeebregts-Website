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
using System.Configuration;

namespace MDR2PDF
{


    public partial class _8c_LK_pr_wk_pr_project_pr_prd : UserControl
    {
        static string xHtmlFolder = ConfigurationManager.AppSettings["htmlFolder"].ToString();
        public EvoTools ET = new EvoTools(xHtmlFolder);

        public int Aantal_Regels_per_Pagina = -1; // Pagina Totaalregel uit indien '-1'
        public int[] PaginaTotalen = new int[8];

        static string pathtabel = System.IO.Path.Combine(xHtmlFolder, "html/Lijst8/tabel.htm");
        static string pathtabel_subregel = System.IO.Path.Combine(xHtmlFolder, "html/Lijst8/tabel_subregel.htm");
        static string pathtabel_totaalregel = System.IO.Path.Combine(xHtmlFolder, "html/Lijst8/tabel_totaalregel.htm");
        static string pathlogo = System.IO.Path.Combine(xHtmlFolder, "html/logo.png");

        public string htmlTabel = File.ReadAllText(pathtabel); //(public wordt steeds opnieuw gelezen!)
        public string htmlTabelSubRegel = File.ReadAllText(pathtabel_subregel); //Static wordt eenmalig gelezen!
        public string htmlTabelTotaalRegel = File.ReadAllText(pathtabel_totaalregel);

        public string records = string.Empty;

        public _8c_LK_pr_wk_pr_project_pr_prd()
        {
            InitializeComponent();
            // Splits de eerste cel in tweeen zodat je 'nr' en 'Projectomschrijving' in een aparte kolom kunt plaatsen/uitlijnen
            htmlTabelSubRegel = htmlTabelSubRegel.Replace("{omschrijving}",
                "<table border = \"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td width=\"25px\" valign=\"top\">{nr}</td><td style=\"padding-left:20px;padding-right:7px\">{omschrijving}</td></tr></table>");
            //table border="0" cellpadding="0" cellspacing="0"
        }



        /// <summary>
        /// Re-order de gevonden totalen van dienstbetrekkingen 
        /// in de juiste formulier-volgorde
        /// EN maak er meteen afgeronde strings van
        /// </summary>
        /// <param name="DienstBetrekkingT"></param>
        /// <returns></returns>
        private string[] TotalenStringxx(string[] S, double[] DienstBetrekkingT)
        {

            // Totaal (Alleen indien nog niet van tevoren al berekend: Dat gebeurt bij het totaaloverzicht binnen de loop)
            if (S[2] == "")
            {
                S[2] = Math.Ceiling(DienstBetrekkingT[3] + DienstBetrekkingT[5] + DienstBetrekkingT[7]).ToString(); // was 1 + 3 + 5
                S[3] = Math.Ceiling(DienstBetrekkingT[4] + DienstBetrekkingT[6] + DienstBetrekkingT[8]).ToString(); // was 2 + 4 + 6
            }

            // (Extern) Per meter
            S[4] = Math.Ceiling(DienstBetrekkingT[3]).ToString();  //[1]).ToString();
            S[5] = Math.Ceiling(DienstBetrekkingT[4]).ToString();  //[2]).ToString();

            // Per Meter 
            S[6] = Math.Ceiling(DienstBetrekkingT[5]).ToString();  // was 3
            S[7] = Math.Ceiling(DienstBetrekkingT[6]).ToString();  // was 4

            // Per Uur
            S[8] = Math.Ceiling(DienstBetrekkingT[7]).ToString(); // was 5
            S[9] = Math.Ceiling(DienstBetrekkingT[8]).ToString(); // was 6
            //S[10] = Math.Ceiling(DienstBetrekkingT[10]).ToString() ;
            S[10] = ET.EuroPresentatie(DienstBetrekkingT[10], 2);

            for (int i = 4; i <= 9; i++)
            {
                if (S[i] == "0")
                    S[i] = "";
            }

            return S;
        }

        internal void Maak_Lijst(DateTime pStartDatum, DateTime pEindDatum, List<PoortViewMandagen> selxMANDAGEN, List<PoortViewProject> _Projecten, List<allContract> Contracten, List<string> _opties, DateTime UI_Startdatum, DateTime UI_Einddatum, EvoTools.HeaderFooter LijstGegevens)
        {
            // Bepaal aantal bij elkaar te houden regels
            int RowSpan = ET.GetRowSpan(htmlTabelTotaalRegel);
            int Rows;

            // Start en Einddata van projecten
            var _VanTotDatums =
                selxMANDAGEN.Select(x => new { x.ProjectId, Datum = x.Begintijd }).Distinct();

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

            // Okay we doen het nu helemaal fucking anders!
            // Bereken per Vakman hoeveel uren hij op die dag heeft gemaakt, hoeveel uren hij op voor een project heeft gemaakt
            // en bepaal het dagdeel (percentage) wat dan kan worden toegekend aan dat project
            var VakmanUrenPerDag = selxMANDAGEN
                .GroupBy(a => new { Datum = a.Begintijd.Date, a.VakmanId })
                .Select(a => new { a.Key.Datum, a.Key.VakmanId , Uren = a.Sum(x => x.Uren * 60 + x.Minuten) / 60.0,  Projecten = a.Count() })
                .OrderBy(a => a.Datum ).ThenBy(a => a.VakmanId )
                .ToList();
            var VakmanUrenPerDagPerProjectPerProdContr = selxMANDAGEN
                .GroupBy(a => new { Datum = a.Begintijd.Date, a.VakmanId, a.ProjectId, a.ProductieVW, a.ContractVW  })
                .Select(a => new {a.Key.ProjectId ,a.Key.Datum, a.Key.VakmanId, a.Key.ProductieVW, a.Key.ContractVW , Uren = a.Sum(x => x.Uren * 60 + x.Minuten) / 60.0 })
                //.OrderBy(a => a.Name2.Datum )
                .ToList();
            var VakmanDagdelenPerDagPerProjectPerProdContr = (from a in VakmanUrenPerDagPerProjectPerProdContr
                        join b in VakmanUrenPerDag
                        on new { a.Datum, a.VakmanId } equals new { b.Datum, b.VakmanId }
                        select new { a, UrenAlleProjecten = b.Uren, DagDeel = a.Uren / b.Uren })
                        .OrderBy(x => x.a.Datum)
                        .ToList()
                        .OrderBy(x => x.a.Datum);

            // Eerst even Totaal alle vakmannen tesamen
            //List<char> Dienstbetrekkingscode = new List<int>("1,3,U,A".Split(',').Select(n => char.Parse(n)).ToList());
            double DagenT, UrenT;
            double DagenD = 0;
            double UrenD = 0;

            string[] RegelStrings = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "" };
            double[] DienstBetrekkingT = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

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
            string regelSamenvatting = "Totaal";
            // WeekIndex gebruiken we pas nadat de 1e lus is doorlopen
            int WeekIndex = 0; string mindag = "", maxdag = ""; 
            Boolean DIKKESTREEP = false;
            do // Start met totaal-periode; Neem daarna alle weekperiodes
            {
                UrenT = 0; DagenT = 0;
                // Bereken voor elke dienstbetrekking het totaal over deze periode
                var _AllProducties = (new List<int> { 1, 2});
                var _AllContracten = (Contracten.Select(x => x.ContractID).ToList()).OrderBy(x => x);
                string TotaalRegel = htmlTabelTotaalRegel;

                // Voeg extra dikke lijn toe na tonen van totaalgegevens (Alleen eerste keer!)
                if (DIKKESTREEP)
                {
                    TotaalRegel = TotaalRegel.Replace("<!--LINE-->", "<tr><td colspan=\"14\" style=\"width:100%; border-bottom:1px solid black\"></td></tr>");
                    DIKKESTREEP = false;
                }

                foreach (int p in _AllProducties)
                {
                    foreach (int c in _AllContracten)
                    {
                        if (p == 1 && c == 2) break; // Intern / Per Meter is niet toegestaan
                        // Uren
                        UrenD = (selxMANDAGEN.Where(x =>
                            x.Begintijd >= SelectieStart &&
                            x.Begintijd < SelectieEinde &&
                            (x.ContractVW == c && x.ProductieVW == p)
                            ).Sum(x => x.Uren * 60 + x.Minuten)) / 60.0;
                        UrenT += UrenD;

                        DagenD = VakmanDagdelenPerDagPerProjectPerProdContr
                            .Where(x =>
                                x.a.Datum >= SelectieStart &&
                                x.a.Datum < SelectieEinde &&
                                x.a.ContractVW == c && x.a.ProductieVW == p)
                            .Select(x => x.DagDeel).Sum();
                        DagenT += DagenD;

                        TotaalRegel = TotaalRegel.Replace("{pcu" + p.ToString() + c.ToString() + "}", DagUurToString(UrenD));
                        TotaalRegel = TotaalRegel.Replace("{pcd" + p.ToString() + c.ToString() + "}", DagUurToString(DagenD));
                    } // Next Contract
                } // Next Productie

                TotaalRegel = TotaalRegel.Replace("{omschrijving}", regelSamenvatting);
                TotaalRegel = TotaalRegel.Replace("{tu}", DagUurToString(UrenT));
                TotaalRegel = TotaalRegel.Replace("{td}", DagUurToString(DagenT));


                // loonkosten
                Double LK = Convert.ToDouble(
                    selxMANDAGEN.Where(x =>
                    x.Begintijd >= SelectieStart &&
                    x.Begintijd < SelectieEinde &&
                    x.ProductieVW == 1 &&
                    x.ContractVW == 1 // Moet 'Per uur' zijn anders is het onzin
                    ).Sum(x => x.Loonkosten)
                    );
                TotaalRegel = TotaalRegel.Replace("{KPi}", ET.EuroPresentatie(LK, 2));

                LK = Convert.ToDouble(
                    selxMANDAGEN.Where(x =>
                    x.Begintijd >= SelectieStart &&
                    x.Begintijd < SelectieEinde &&
                    x.ProductieVW == 2 &&
                    x.ContractVW == 1 // Moet 'Per uur' zijn anders is het onzin
                    ).Sum(x => x.Loonkosten)
                    );
                TotaalRegel = TotaalRegel.Replace("{KPe}", ET.EuroPresentatie(LK, 2));

                // Periode toevoegen
                if (UrenT > 0)
                {
                    mindag = _VanTotDatums.Where(x =>
                            x.Datum >= SelectieStart &&
                            x.Datum < SelectieEinde
                            ).Select(x => x.Datum).Min().ToShortDateString();
                    maxdag = _VanTotDatums.Where(x =>
                            x.Datum >= SelectieStart &&
                            x.Datum < SelectieEinde
                            ).Select(x => x.Datum).Max().ToShortDateString();
                }
                TotaalRegel = TotaalRegel.Replace("{van}", mindag);
                TotaalRegel = TotaalRegel.Replace("{tot}", maxdag);

                // Voeg Totaalregel toe en maak nieuwe tabelregel
                records += TotaalRegel; Rows = 2;


                // Bereken nu PER PROJECT voor elke dienstbetrekking het totaal over deze periode
                int regelnummer = 0;
                foreach (int ProjectID in selxMANDAGEN
                    .OrderBy(x => x.project_NR)
                    .Select(x => x.ProjectId).Distinct().ToList())
                {
                    UrenT = 0; DagenT = 0; mindag = ""; maxdag = "";

                    // Steeds per 3 regels van kleur wisselen
                    TotaalRegel = htmlTabelSubRegel.Replace("ProjectRegel", "ProjectRegelColor" + ((regelnummer++ % 6) / 3).ToString());
                    
                    foreach (int p in _AllProducties)
                    {
                        foreach (int c in _AllContracten)
                        {
                            if (p == 1 && c == 2) break; // Intern / Per Meter is niet toegestaan
                            // Uren
                            UrenD = selxMANDAGEN.Where(x =>
                                x.Begintijd >= SelectieStart &&
                                x.Begintijd < SelectieEinde &&
                                x.ProjectId == ProjectID &&
                                (x.ContractVW == c) && (x.ProductieVW == p)
                                ).Sum(x => x.Uren * 60 + x.Minuten) / 60.0;
                            UrenT += UrenD;

                            DagenD = VakmanDagdelenPerDagPerProjectPerProdContr
                                .Where(x =>
                                    x.a.Datum >= SelectieStart &&
                                    x.a.Datum < SelectieEinde &&
                                    x.a.ProjectId == ProjectID &&
                                    x.a.ContractVW == c && x.a.ProductieVW == p)
                                .Select(x => x.DagDeel).Sum();
                            DagenT += DagenD;

                            // Totaal van geselecteeerd Productie/Contract invullen
                            TotaalRegel = TotaalRegel.Replace("{pcu" + p.ToString() + c.ToString() + "}", DagUurToString(UrenD));
                            TotaalRegel = TotaalRegel.Replace("{pcd" + p.ToString() + c.ToString() + "}", DagUurToString(DagenD));

                        } // Next Contract
                    } // Next Productie

                    // LoonKosten
                    LK = Convert.ToDouble(selxMANDAGEN.Where(x =>
                        x.Begintijd >= SelectieStart &&
                        x.Begintijd < SelectieEinde &&
                        x.ProjectId == ProjectID &&
                        x.ProductieVW == 1 &&
                        x.ContractVW == 1 // Moet 'Per uur' zijn anders is het onzin
                        ).Sum(x => x.Loonkosten));
                    TotaalRegel = TotaalRegel.Replace("{KPi}", ET.EuroPresentatie(LK, 2));

                    LK = Convert.ToDouble(selxMANDAGEN.Where(x =>
                        x.Begintijd >= SelectieStart &&
                        x.Begintijd < SelectieEinde &&
                        x.ProjectId == ProjectID &&
                        x.ProductieVW == 2 &&
                        x.ContractVW == 1 // Moet 'Per uur' zijn anders is het onzin
                        ).Sum(x => x.Loonkosten));
                    TotaalRegel = TotaalRegel.Replace("{KPe}", ET.EuroPresentatie(LK, 2));

                    // RegelBegin en RegelTotalen toevoegen
                    TotaalRegel = TotaalRegel.Replace("{nr}",
                        _Projecten.Where(x => x.ProjectId == ProjectID).Select(x => x.ProjectNR).FirstOrDefault().ToString() ?? "<Onbekend project>");
                    TotaalRegel = TotaalRegel.Replace("{omschrijving}",
                        _Projecten.Where(x => x.ProjectId == ProjectID).Select(x => x.Naam).FirstOrDefault() ?? "<No Name>");
                    TotaalRegel = TotaalRegel.Replace("{tu}", DagUurToString(UrenT));
                    TotaalRegel = TotaalRegel.Replace("{td}", DagUurToString(DagenT));

                    // Alle waarden zijn gevuld; Voeg regel toe aan HTML pagina
                    // NIEUW als er geen uren staan voor deze vakman, dan ook geen regel aan besteden dus alleen tonen als er uren gemaakt zijn

                    if (UrenT > 0)
                    {
                        // Periode toevoegen
                        mindag = _VanTotDatums.Where(x =>
                                x.Datum >= SelectieStart &&
                                x.Datum < SelectieEinde &&
                                x.ProjectId == ProjectID
                                ).Select(x => x.Datum).Min().ToShortDateString();
                        maxdag = _VanTotDatums.Where(x =>
                                x.Datum >= SelectieStart &&
                                x.Datum < SelectieEinde &&
                                x.ProjectId == ProjectID
                                ).Select(x => x.Datum).Max().ToShortDateString();

                        // Steeds per 3 regels van kleur wisselen
                        //TotaalRegel = htmlTabelSubRegel.Replace("ProjectRegel", "ProjectRegelColor" + ((regelnummer++ % 6) / 3).ToString());
                        //records += ET.HtmlFormat(TotaalRegel, TotalenStringxx(RegelStrings, DienstBetrekkingT)); Rows++;
                    }
                    TotaalRegel = TotaalRegel.Replace("{van}", mindag);
                    TotaalRegel = TotaalRegel.Replace("{tot}", maxdag);

                    // Alle waarden zijn gevuld; Voeg regel toe aan HTML pagina
                    if (UrenT > 0)
                    {
                        records += TotaalRegel; Rows++;
                    }

                } // Next Project

                // Nodig ivm eerste paar subregels bij kopregel op één pagina houden: 
                // Pas Rowspan aan indien er minder subregels bij elkaar te houden zijn dan in de rowspan is aangegeven
                if (Rows < RowSpan)
                    records = ET.SetRowSpan(records, RowSpan, Rows);

                // bij de 1e lus was de periode ingesteld op totaalperiode, vanaf nu pakken we de weekperiodes
                if (WeekIndex < WeekStart.Count())
                {
                    SelectieStart = WeekStart[WeekIndex];
                    SelectieEinde = WeekEinde[WeekIndex];
                    if (regelSamenvatting.ToLower().Contains("totaal")) DIKKESTREEP = true;
                    regelSamenvatting = "Week " + ET.Weeknummer(WeekStart[WeekIndex], true);
                }
            } while (++WeekIndex <= WeekStart.Count());


            // Eco PDF eenmaken, vullen en tonen
            string FilledTable = htmlTabel.Replace("{tabelregels}", records);
            ET.MakePdf(FilledTable, LijstGegevens);

        }

        private string DagUurToString(double Getal)
        {
            return (Getal == 0) ? "&nbsp" : Math.Ceiling(Getal).ToString();
        }
    } //

} //


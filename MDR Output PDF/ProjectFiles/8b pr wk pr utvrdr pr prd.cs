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
    public partial class _8b_pr_wk_pr_utvrdr_pr_prd : UserControl
    {
        static string xHtmlFolder = ConfigurationManager.AppSettings["htmlFolder"].ToString();
        public EvoTools ET = new EvoTools(xHtmlFolder);

        static string pathtabel = System.IO.Path.Combine(xHtmlFolder, "html/Lijst8/tabel.htm");
        static string pathtabel_subregel = System.IO.Path.Combine(xHtmlFolder, "html/Lijst8/tabel_subregel.htm");
        static string pathtabel_totaalregel = System.IO.Path.Combine(xHtmlFolder, "html/Lijst8/tabel_totaalregel.htm");
        static string pathlogo = System.IO.Path.Combine(xHtmlFolder, "html/logo.png");

        public string htmlTabel = File.ReadAllText(pathtabel); //(public wordt steeds opnieuw gelezen!)
        static string htmlTabelSubRegel = File.ReadAllText(pathtabel_subregel); //Static wordt eenmalig gelezen!
        static string htmlTabelTotaalRegel = File.ReadAllText(pathtabel_totaalregel);

        public string records = string.Empty;

        public _8b_pr_wk_pr_utvrdr_pr_prd()
        {
            InitializeComponent();
            // Verander het tabelkopje even, hier kan beter Uitvoerders staan ipv Projecten
            htmlTabel = htmlTabel.Replace("Projecten", "Uitvoerders");
        }

        internal void Maak_Lijst(DateTime pStartDatum, DateTime pEindDatum, List<PoortViewMandagen> selxMANDAGEN, List<PoortViewRegistratiebevoegd> _gebruikers, List<allContract> Contracten, EvoTools.HeaderFooter LijstGegevens)
        {
            // Bepaal aantal bij elkaar te houden regels
            int RowSpan = ET.GetRowSpan(htmlTabelTotaalRegel);
            int Rows;

            ET.Log("Start van Maak Lijst 8b");
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
                .Select(a => new { a.Key.Datum, a.Key.VakmanId, Uren = a.Sum(x => x.Uren * 60 + x.Minuten) / 60.0, Projecten = a.Count() })
                .OrderBy(a => a.Datum).ThenBy(a => a.VakmanId)
                .ToList();
            var VakmanUrenPerDagPerProjectLeiderPerProdContr = selxMANDAGEN
                .GroupBy(a => new { Datum = a.Begintijd.Date, a.VakmanId, a.ProjectleiderId , a.ProductieVW, a.ContractVW })
                .Select(a => new { a.Key.ProjectleiderId, a.Key.Datum, a.Key.VakmanId, a.Key.ProductieVW, a.Key.ContractVW, Uren = a.Sum(x => x.Uren * 60 + x.Minuten) / 60.0 })
                //.OrderBy(a => a.Name2.Datum )
                .ToList();
            var VakmanDagdelenPerDagPerProjectPerProdContr = (from a in VakmanUrenPerDagPerProjectLeiderPerProdContr
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

            // ****************************************************************************************************
            //
            // Start opbouwen van de totaalregels
            // Per 'DO'
            //  Bereken het totaal van de betreffende periode;
            //  Berken per vakman het totaal van de betreffende periode
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
            int WeekIndex = 0; string mindag = "", maxdag = ""; bool DIKKESTREEP = false;
            do // Start met totaal-periode; Neem daarna alle weekperiodes
            {
                UrenT = 0; DagenT = 0;
                // Bereken voor elke dienstbetrekking het totaal over deze periode
                //for (int c = -1; c <= 7; c += 2)
                var _AllContracten = (Contracten.Select(x => x.ContractID).ToList()).OrderBy(x => x);
                var _AllProducties = new List<int> { 1, 2 };
                string TotaalRegel = htmlTabelTotaalRegel;
                // Voeg extra dikke lijn toe na tonen van totaalgegevens (Alleen eerste keer!)
                if (DIKKESTREEP)
                {
                    TotaalRegel = TotaalRegel.Replace("<!--LINE-->", "<tr><td colspan=\"14\" style=\"width:100%; border-bottom:1px solid black\"></td></tr>");
                    DIKKESTREEP = false;
                }

                foreach (int prod in _AllProducties)
                {
                    foreach (int contr in _AllContracten)
                    {
                        if (prod == 1 && contr == 2) break; // Intern, Per meter komt niet voor (wordt op gecontroleerd bij de invoer)
                        // Uren
                        UrenD = selxMANDAGEN.Where(x =>
                            x.Begintijd >= SelectieStart &&
                            x.Begintijd < SelectieEinde &&
                            (x.ContractVW == contr && x.ProductieVW == prod)
                            ).Sum(x => x.Uren) * 60;
                        UrenD += selxMANDAGEN.Where(x =>
                            x.Begintijd >= SelectieStart &&
                            x.Begintijd < SelectieEinde &&
                            (x.ContractVW == contr && x.ProductieVW == prod)
                            ).Sum(x => x.Minuten);
                        UrenD /= 60.0;
                        UrenT += UrenD;
                        // Dagen
                        DagenD = selxMANDAGEN.Where(x =>
                            x.Begintijd >= SelectieStart &&
                            x.Begintijd < SelectieEinde &&
                            (x.ContractVW == contr && x.ProductieVW == prod)
                            ).Select(y => new { y.VakmanId, y.Begintijd.Date }).Distinct().Count();
                        DagenT += DagenD;

                        TotaalRegel = TotaalRegel.Replace("{pcu" + prod.ToString() + contr.ToString() + "}", DagUurToString(UrenD));
                        TotaalRegel = TotaalRegel.Replace("{pcd" + prod.ToString() + contr.ToString() + "}", DagUurToString(DagenD));

                    } // NBext C
                } // Next P

                // Regel verder invullen ---
                //---------------------- ---

                // min en maxdatum
                if (UrenD > 0)
                {
                    mindag = selxMANDAGEN.Where(x => x.Begintijd >= SelectieStart && x.Begintijd < SelectieEinde).Select(x => x.Begintijd).Min().ToString("d-M-yyy");
                    maxdag = selxMANDAGEN.Where(x => x.Begintijd >= SelectieStart && x.Begintijd < SelectieEinde).Select(x => x.Begintijd).Max().ToString("d-M-yyy");
                }
                TotaalRegel = TotaalRegel.Replace("{van}", mindag);
                TotaalRegel = TotaalRegel.Replace("{tot}", maxdag);

                // Omschrijving en Totaaluren
                TotaalRegel = TotaalRegel.Replace("{omschrijving}", regelSamenvatting);
                TotaalRegel = TotaalRegel.Replace("{tu}", DagUurToString(UrenT));
                TotaalRegel = TotaalRegel.Replace("{td}", DagUurToString(DagenT));

                // Loonkosten
                var LK = Convert.ToDouble
                    (selxMANDAGEN.Where(x =>
                        x.Begintijd >= SelectieStart &&
                        x.Begintijd < SelectieEinde &&
                        x.ProductieVW == 1 &&
                        x.ContractVW == 1 // Moet 'Per uur' zijn anders is het onzin
                    ).Sum(x => x.Loonkosten));
                TotaalRegel = TotaalRegel.Replace("{KPi}", ET.EuroPresentatie(LK, 2));

                LK = Convert.ToDouble
                    (selxMANDAGEN.Where(x =>
                        x.Begintijd >= SelectieStart &&
                        x.Begintijd < SelectieEinde &&
                        x.ProductieVW == 2 &&
                        x.ContractVW == 1 // Moet 'Per uur' zijn anders is het onzin
                    ).Sum(x => x.Loonkosten));
                TotaalRegel = TotaalRegel.Replace("{KPe}", ET.EuroPresentatie(LK, 2));

                //records += "<b>" + ET.HtmlFormat(htmlrecord, RegelVars) + "</b>";
                records += TotaalRegel; Rows = 2;

                // Bereken nu PER PROJECTLEIDER voor elke dienstbetrekking het totaal over deze periode
                List<int> _ProjectleiderIDs = selxMANDAGEN.Select(x => x.ProjectleiderId).Distinct().ToList();
                var _Projectleiders = _gebruikers.Where(x => _ProjectleiderIDs.Contains(x.ProjectleiderId ?? 0)).Select(x => new { x.ProjectleiderId, x.Gebruikersnaam }).Distinct().ToList();
                int regelnummer = 0;
                var projectleiders = selxMANDAGEN.Where(x =>
                            x.Begintijd >= SelectieStart &&
                            x.Begintijd < SelectieEinde).Select(x => x.ProjectleiderId).Distinct();

                foreach (var Projectleider in _Projectleiders.Where(x => projectleiders.Contains(x.ProjectleiderId ?? 0))
                    .OrderBy(x => x.Gebruikersnaam))
                {
                    UrenT = 0; DagenT = 0; mindag = ""; maxdag = "";

                    // Steeds per 3 regels van kleur wisselen
                    TotaalRegel = htmlTabelSubRegel.Replace("ProjectRegel", "ProjectRegelColor" + ((regelnummer++ % 6) / 3).ToString());
                    foreach (int prod in _AllProducties)
                    {
                        foreach (int contr in _AllContracten)
                        {
                            if(prod == 1 && contr == 2) break; // Intern, Per meter doen we niet aan
                            // Uren
                            UrenD = selxMANDAGEN.Where(x =>
                                x.Begintijd >= SelectieStart &&
                                x.Begintijd < SelectieEinde &&
                                x.ProjectleiderId == Projectleider.ProjectleiderId &&
                                x.ContractVW == contr && x.ProductieVW  == prod
                                ).Sum(x => x.Uren) * 60;
                            UrenD += selxMANDAGEN.Where(x =>
                                x.Begintijd >= SelectieStart &&
                                x.Begintijd < SelectieEinde &&
                                x.ProjectleiderId == Projectleider.ProjectleiderId &&
                                x.ContractVW == contr && x.ProductieVW == prod
                                ).Sum(x => x.Minuten);
                            UrenD /= 60.0;
                            UrenT += UrenD;

                            // Dagen (Als percentage 'Dagdeel')
                            DagenD = VakmanDagdelenPerDagPerProjectPerProdContr
                                .Where(x =>
                                    x.a.Datum >= SelectieStart &&
                                    x.a.Datum < SelectieEinde &&
                                    x.a.ProjectleiderId == Projectleider.ProjectleiderId &&
                                    x.a.ContractVW == contr && x.a.ProductieVW == prod)
                                .Select(x => x.DagDeel).Sum();
                            DagenT += DagenD;

                            // Totaal van geselecteeerd Productie/Contract invullen
                            TotaalRegel = TotaalRegel.Replace("{pcu" + prod.ToString() + contr.ToString() + "}", DagUurToString(UrenD));
                            TotaalRegel = TotaalRegel.Replace("{pcd" + prod.ToString() + contr.ToString() + "}", DagUurToString(DagenD));

                        } // Next contract
                    } // Next Productie

                    TotaalRegel = TotaalRegel.Replace("{omschrijving}",
                        Projectleider.Gebruikersnaam
                        );
                    TotaalRegel = TotaalRegel.Replace("{tu}", DagUurToString(UrenT));
                    TotaalRegel = TotaalRegel.Replace("{td}", DagUurToString(DagenT));

                    // min en maxdatum
                    if (UrenT > 0)
                    {
                        mindag = selxMANDAGEN.Where(x => x.Begintijd >= SelectieStart && x.Begintijd < SelectieEinde && x.ProjectleiderId == Projectleider.ProjectleiderId).Select(x => x.Begintijd).Min().ToString("d-M-yyy");
                        maxdag = selxMANDAGEN.Where(x => x.Begintijd >= SelectieStart && x.Begintijd < SelectieEinde && x.ProjectleiderId == Projectleider.ProjectleiderId).Select(x => x.Begintijd).Max().ToString("d-M-yyy");
                    }
                    TotaalRegel = TotaalRegel.Replace("{van}", mindag);
                    TotaalRegel = TotaalRegel.Replace("{tot}", maxdag);

                    // Loonkosten
                    LK = Convert.ToDouble
                        (selxMANDAGEN.Where(x =>
                            x.Begintijd >= SelectieStart &&
                            x.Begintijd < SelectieEinde &&
                            x.ProjectleiderId == Projectleider.ProjectleiderId &&
                            x.ProductieVW == 1 &&
                            x.ContractVW == 1 // Moet 'Per uur' zijn anders is het onzin
                        ).Sum(x => x.Loonkosten));
                    TotaalRegel = TotaalRegel.Replace("{KPi}", ET.EuroPresentatie(LK, 2));

                    LK = Convert.ToDouble
                        (selxMANDAGEN.Where(x =>
                            x.Begintijd >= SelectieStart &&
                            x.Begintijd < SelectieEinde &&
                            x.ProjectleiderId == Projectleider.ProjectleiderId &&
                            x.ProductieVW == 2 &&
                            x.ContractVW == 1 // Moet 'Per uur' zijn anders is het onzin
                        ).Sum(x => x.Loonkosten));
                    TotaalRegel = TotaalRegel.Replace("{KPe}", ET.EuroPresentatie(LK, 2));


                    // Alle waarden zijn gevuld; Voeg regel toe aan HTML pagina
                    // NIEUW als er geen uren staan voor deze vakman, dan ook geen regel aan besteden dus alleen tonen als er uren gemaakt zijn
                    if (UrenT > 0 || LK > 0)
                    //records += ET.HtmlFormat(htmlrecord, RegelVars);
                    {
                        records += TotaalRegel; Rows++;
                    }

                }

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
                    regelSamenvatting = string.Format("Week {0}", ET.Weeknummer(SelectieStart, true));
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

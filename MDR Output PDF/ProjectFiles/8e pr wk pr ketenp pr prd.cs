using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Configuration;

namespace MDR2PDF
{
    public partial class _8e_pr_wk_pr_ketenp_pr_prd : UserControl
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

        public _8e_pr_wk_pr_ketenp_pr_prd()
        {
            InitializeComponent();
            // Verander het tabelkopje even, hier kan beter Vakmannen staan ipv Projecten
            htmlTabel = htmlTabel.Replace("Projecten", "Ketenpartners");
        }

        internal void Maak_Lijst(DateTime pStartDatum, DateTime pEindDatum, List<PoortViewMandagen> selxMANDAGEN, List<PoortViewKetenpartner> Ketenpartners, List<allContract> Contracten, List<string> _opties, DateTime UI_Startdatum, DateTime UI_Einddatum, EvoTools.HeaderFooter LijstGegevens)
        {
            // Start en Einddata van projecten
            var _VanTotDatums =
                selxMANDAGEN.Select(x => new { x.VakmanId, Datum = x.Begintijd }).Distinct();

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
                if (Maandag < pStartDatum)
                    WeekStart.Add(pStartDatum);
                else
                    WeekStart.Add(Maandag);

                Maandag = Maandag.AddDays(7);

                if (Maandag > pEindDatum)
                    WeekEinde.Add(pEindDatum.AddDays(1));
                else
                    WeekEinde.Add(Maandag);
            } while (Maandag <= pEindDatum);
            DateTime PeriodeEinde = Maandag;

            double DagenT, UrenT;
            double DagenD = 0;
            double UrenD = 0;

            // Bepaal aantal bij elkaar te houden regels
            int RowSpan = ET.GetRowSpan(htmlTabelTotaalRegel);
            int Rows;

            // ****************************************************************************************************
            //
            // Start opbouwen van de totaalregels
            // Per 'DO'
            //  Bereken het totaal van de betreffende periode;
            //  Berken per Ketenpartner het totaal van de betreffende periode
            // 
            // We beginnen met Totaalperiode van periodestart tot periodeeind
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
                UrenT = 0; DagenT = 0; mindag = ""; maxdag = "";
                string TotaalRegel = htmlTabelTotaalRegel;
                // Voeg extra dikke lijn toe na tonen van totaalgegevens (Alleen eerste keer!)
                if (DIKKESTREEP)
                {
                    TotaalRegel = TotaalRegel.Replace("<!--LINE-->", "<tr><td colspan=\"14\" style=\"width:100%; border-bottom:1px solid black\"></td></tr>");
                    DIKKESTREEP = false;
                }

                // Bereken voor alle 3 mogelijkheden het totaal over deze periode
                foreach (int prod in new List<int> { 1, 2 }) // 1 = Intern, 2 = Extern
                {
                    foreach (int contr in new List<int> { 1, 2 }) //1 = Per uur, 2 = Per meter
                    {
                        if (prod == 1 && contr == 2) break; // Intern, Per meter doet niet mee
                        UrenD = (selxMANDAGEN.Where(x => x.Begintijd >= SelectieStart && x.Begintijd < SelectieEinde &&
                                x.ProductieVW == prod && x.ContractVW == contr 
                                ).Sum(x => x.Uren * 60 + x.Minuten)) / 60.0;
                        DagenD = selxMANDAGEN.Where(x => x.Begintijd >= SelectieStart && x.Begintijd < SelectieEinde &&
                                x.ProductieVW == prod && x.ContractVW == contr
                                ).Select(y => new { y.VakmanId, y.Begintijd.Date }).Distinct().Count();
                        UrenT += UrenD; DagenT += DagenD;

                        TotaalRegel = TotaalRegel.Replace("{pcu" + prod.ToString() + contr.ToString() + "}", DagUurToString(UrenD));
                        TotaalRegel = TotaalRegel.Replace("{pcd" + prod.ToString() + contr.ToString() + "}", DagUurToString(DagenD));
                    } // Next contr
                } // Next prod
                TotaalRegel = TotaalRegel.Replace("{omschrijving}", regelSamenvatting);
                TotaalRegel = TotaalRegel.Replace("{tu}", DagUurToString(UrenT));
                TotaalRegel = TotaalRegel.Replace("{td}", DagUurToString(DagenT));

                // min en maxdatum
                if (UrenT > 0)
                {
                    mindag = selxMANDAGEN.Where(x => x.Begintijd >= SelectieStart && x.Begintijd < SelectieEinde).Select(x => x.Begintijd).Min().ToString("d-M-yyy");
                    maxdag = selxMANDAGEN.Where(x => x.Begintijd >= SelectieStart && x.Begintijd < SelectieEinde).Select(x => x.Begintijd).Max().ToString("d-M-yyy");
                }
                TotaalRegel = TotaalRegel.Replace("{van}", mindag);
                TotaalRegel = TotaalRegel.Replace("{tot}", maxdag);

                // loonkosten Intern
                var KostPrijs = Convert.ToDouble(
                    selxMANDAGEN.Where(x =>
                        x.Begintijd >= SelectieStart &&
                        x.Begintijd < SelectieEinde &&
                        x.ProductieVW == 1 &&
                        x.ContractVW == 1 )// Moet 'Per uur' zijn anders is het onzin)
                    .Sum(x => x.Loonkosten));
                TotaalRegel = TotaalRegel.Replace("{KPi}", ET.EuroPresentatie(KostPrijs, 2));

                //Loonkosten Extern
                KostPrijs = Convert.ToDouble(
                    selxMANDAGEN.Where(x =>
                        x.Begintijd >= SelectieStart &&
                        x.Begintijd < SelectieEinde &&
                        x.ProductieVW == 2 &&
                        x.ContractVW == 1 ) // Moet 'Per uur' zijn anders is het onzin)
                    .Sum(x => x.Loonkosten));
                TotaalRegel = TotaalRegel.Replace("{KPe}", ET.EuroPresentatie(KostPrijs, 2));

                // Regel compleet. Row toevoegen aan tabelregels
                records += TotaalRegel; Rows = 2;

                // Bereken nu PER VAKMAN voor elke dienstbetrekking het totaal over deze periode
                var _KetenPartnerIDs = selxMANDAGEN
                    .Where(x => x.Begintijd >= SelectieStart &&
                            x.Begintijd < SelectieEinde)
                    .OrderBy(x => x.KetenpartnerVW)
                    .Select(x => x.KetenpartnerVW).Distinct().ToList();
                var _KetenPartners = Ketenpartners.Where(x => _KetenPartnerIDs.Contains(x.KetenpartnerID));

                int regelnummer = 0;
                foreach (var Ketenpartner in _KetenPartners
                    //.OrderBy(x => x.Sorteervolgorde).ThenBy(x => x.Bedrijfsnaam )
                    .OrderBy(x => x.Bedrijfsnaam)
                    )
                {
                    UrenT = 0; DagenT = 0;
                    TotaalRegel = htmlTabelSubRegel;
                    // Steeds per 3 regels van kleur wisselen
                    TotaalRegel = htmlTabelSubRegel.Replace("ProjectRegel", "ProjectRegelColor" + ((regelnummer++ % 6) / 3).ToString());

                    for (int prod = 1; prod <= 2; prod++ )
                    {
                        for (int contr = 1; contr <=2; contr++)
                        {
                            if (prod == 1 && contr == 2) // Intern per meter mag niet voorkomen!
                                break;
                            UrenD = (selxMANDAGEN.Where(x => x.Begintijd >= SelectieStart && x.Begintijd < SelectieEinde &&
                                    x.KetenpartnerVW == Ketenpartner.KetenpartnerID && x.ProductieVW == prod && x.ContractVW == contr
                                    ).Sum(x => x.Uren * 60 + x.Minuten)) / 60.0;
                            UrenT += UrenD;
                            DagenD = selxMANDAGEN.Where(x => x.Begintijd >= SelectieStart && x.Begintijd < SelectieEinde &&
                                    x.KetenpartnerVW == Ketenpartner.KetenpartnerID && x.ProductieVW == prod && x.ContractVW == contr
                                    ).Select(y => new { y.VakmanId, y.Begintijd.Date }).Distinct().Count();
                            DagenT += DagenD;

                            // Totaal van dienstbetrekking invullen
                            TotaalRegel = TotaalRegel.Replace("{pcu" + prod.ToString() + contr.ToString() + "}", DagUurToString(UrenD));
                            TotaalRegel = TotaalRegel.Replace("{pcd" + prod.ToString() + contr.ToString() + "}", DagUurToString(DagenD));
                        }
                    }

                    if (Ketenpartner.Bedrijfsnaam  == null )
                        TotaalRegel = TotaalRegel.Replace("{omschrijving}", "Onbekende Ketenpartner met ID = " + Ketenpartner.KetenpartnerID.ToString());
                    else
                        TotaalRegel = TotaalRegel.Replace("{omschrijving}", Ketenpartner.Bedrijfsnaam);
                    
                    TotaalRegel = TotaalRegel.Replace("{tu}", DagUurToString(UrenT));
                    TotaalRegel = TotaalRegel.Replace("{td}", DagUurToString(DagenT));

                    // loonkosten Intern
                    KostPrijs = Convert.ToDouble(
                        selxMANDAGEN.Where(x =>
                            x.Begintijd >= SelectieStart &&
                            x.Begintijd < SelectieEinde &&
                            x.KetenpartnerVW == Ketenpartner.KetenpartnerID &&
                            x.ProductieVW == 1 &&
                            x.ContractVW == 1 )// Moet 'Per uur' zijn anders is het onzin)
                            .Sum(x => x.Loonkosten));
                    TotaalRegel = TotaalRegel.Replace("{KPi}", ET.EuroPresentatie(KostPrijs, 2));

                    // Loonkosten Extern (Let op: Vakman kan gedeelte van de tijd zowel intern als extern geweest zijn
                    KostPrijs = Convert.ToDouble(
                        selxMANDAGEN.Where(x =>
                            x.Begintijd >= SelectieStart &&
                            x.Begintijd < SelectieEinde &&
                            x.KetenpartnerVW == Ketenpartner.KetenpartnerID &&
                            x.ProductieVW == 2 &&
                            x.ContractVW == 1 ) // Moet 'Per uur' zijn anders is het onzin)
                            .Sum(x => x.Loonkosten));

                    TotaalRegel = TotaalRegel.Replace("{KPe}", ET.EuroPresentatie(KostPrijs, 2));

                    // Alle waarden zijn gevuld; Voeg regel toe aan HTML pagina
                    // (Als er geen uren staan voor deze vakman, dan ook geen regel aan besteden dus alleen tonen als er uren gemaakt zijn)
                    if (UrenT > 0)
                    {
                        // min en maxdatum
                        if (UrenT > 0)
                        {
                            mindag = selxMANDAGEN.Where(x => x.Begintijd >= SelectieStart && x.Begintijd < SelectieEinde && x.KetenpartnerVW == Ketenpartner.KetenpartnerID ).Select(x => x.Begintijd).Min().ToString("d-M-yyy");
                            maxdag = selxMANDAGEN.Where(x => x.Begintijd >= SelectieStart && x.Begintijd < SelectieEinde && x.KetenpartnerVW == Ketenpartner.KetenpartnerID ).Select(x => x.Begintijd).Max().ToString("d-M-yyy");
                        }
                        TotaalRegel = TotaalRegel.Replace("{van}", mindag);
                        TotaalRegel = TotaalRegel.Replace("{tot}", maxdag);

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

    }//
}

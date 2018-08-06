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
    public partial class _8a_pr_wk_pr_prd : UserControl
    {

        static string xHtmlFolder = ConfigurationManager.AppSettings["htmlFolder"].ToString();
        public EvoTools ET = new EvoTools(xHtmlFolder);

        static string pathtabel = System.IO.Path.Combine(xHtmlFolder, "html/Lijst8/tabel.htm");
        static string pathtabel_weekregel = System.IO.Path.Combine(xHtmlFolder, "html/Lijst8/tabel_subregel.htm");
        static string pathtabel_totaalregel = System.IO.Path.Combine(xHtmlFolder, "html/Lijst8/tabel_totaalregel.htm");
        static string pathlogo = System.IO.Path.Combine(xHtmlFolder, "html/logo.png");

        public string htmlTabel = File.ReadAllText(pathtabel); //(public wordt steeds opnieuw gelezen!)
        public string htmlTabelWeekRegel = File.ReadAllText(pathtabel_weekregel); //Static wordt eenmalig gelezen!
        public string htmlTabelTotaalRegel = File.ReadAllText(pathtabel_totaalregel);

        public string records = string.Empty;

        public _8a_pr_wk_pr_prd()

        {
            InitializeComponent();
            // Verander het tabelkopje even, hier kan beter Periodes staan ipv Projecten
            htmlTabel = htmlTabel.Replace("Projecten", "Periodes");
        }


        internal void Maak_Lijst(DateTime pStartDatum, DateTime pEindDatum, List<PoortViewMandagen> selxMANDAGEN, List<allContract> Contracten, EvoTools.HeaderFooter LijstGegevens)
        {
            // Bepaal aantal bij elkaar te houden regels
            int RowSpan = ET.GetRowSpan(htmlTabelTotaalRegel);
            int Rows;

            //
            // Deel periode in x-aantal weken in
            //

            // Week van Startdatum begin op maandag
            List<DateTime> WeekStart = new List<DateTime>();
            List<DateTime> WeekEinde = new List<DateTime>();
            // Trek DayofWeek van startdatum af om de maandag van die week te bepalen
            DateTime Maandag = pStartDatum.AddDays((1 - ((int)pStartDatum.DayOfWeek == 0 ? 7 : (int)pStartDatum.DayOfWeek)));
            DateTime PeriodeStart = Maandag;
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

            // Eerst even Totaal alle vakmannen
            double UrenT = 0;
            double DagenT = 0;
            double DagenD = 0;
            double UrenD = 0;

            // Bereken voor elke dienstbetrekking het totaal
            var _AllContracten = (Contracten.Select(x => x.ContractID).ToList()).OrderBy(x => x);
            string TotaalRegel = htmlTabelTotaalRegel;
            foreach (int prod in new List<int> { 1, 2 })
            {
                foreach (int contr in _AllContracten)
                {
                    // Uren
                    UrenD = selxMANDAGEN.Where(x =>
                        x.Begintijd >= PeriodeStart &&
                        x.Begintijd < PeriodeEinde &&
                        x.ContractVW == contr && x.ProductieVW == prod
                        ).Sum(x => x.Uren) * 60;
                    UrenD += selxMANDAGEN.Where(x =>
                        x.Begintijd >= PeriodeStart &&
                        x.Begintijd < PeriodeEinde &&
                        x.ContractVW == contr && x.ProductieVW == prod
                        ).Sum(x => x.Minuten);
                    UrenD /= 60.0;
                    UrenT += UrenD;

                    // Dagen
                    DagenD = selxMANDAGEN.Where(x =>
                        x.Begintijd >= PeriodeStart &&
                        x.Begintijd < PeriodeEinde &&
                        x.ContractVW == contr && x.ProductieVW == prod
                        ).Select(y => new { y.VakmanId, y.Begintijd.Date }).Distinct().Count();
                    DagenT += DagenD;

                    // Totaal van dienstbetrekking invullen
                    TotaalRegel = TotaalRegel.Replace("{pcu" + prod.ToString() + contr.ToString() + "}", DagUurToString(UrenD));
                    TotaalRegel = TotaalRegel.Replace("{pcd" + prod.ToString() + contr.ToString() + "}", DagUurToString(DagenD));
                }
            }

            //Datums
            string mindag = string.Empty, maxdag = string.Empty;

            if (UrenT > 0)
            {
                mindag = selxMANDAGEN.Select(x => x.Begintijd).Min().ToString("d-M-yyy");
                maxdag = selxMANDAGEN.Select(x => x.Begintijd).Max().ToString("d-M-yyy");
            }

            TotaalRegel = TotaalRegel.Replace("{omschrijving}", "Totaal");
            TotaalRegel = TotaalRegel.Replace("{tu}", DagUurToString(UrenT));
            TotaalRegel = TotaalRegel.Replace("{td}", DagUurToString(DagenT));
            TotaalRegel = TotaalRegel.Replace("{van}", mindag);
            TotaalRegel = TotaalRegel.Replace("{tot}", maxdag);

            // loonkosten
            var KostPrijs = Convert.ToDouble(
                selxMANDAGEN.Where(x => x.ProductieVW == 1 && x.ContractVW == 1).Sum(x => x.Loonkosten));
            TotaalRegel = TotaalRegel.Replace("{KPi}", ET.EuroPresentatie(KostPrijs,2));

            KostPrijs = Convert.ToDouble(
                selxMANDAGEN.Where(x => x.ProductieVW == 2 && x.ContractVW == 1).Sum(x => x.Loonkosten));
            TotaalRegel = TotaalRegel.Replace("{KPe}", ET.EuroPresentatie(KostPrijs, 2));

            // Regel compleet. Row toevoegen aan tabelregels
            records += TotaalRegel; Rows = 2;

            // Bepaal voor elke week de totalen
            for (int i = 0; i < WeekStart.Count(); i++)
            {
                DateTime _StartTijd = WeekStart[i];
                DateTime _EindeTijd = WeekEinde[i];

                DagenD = 0;
                UrenD = 0;
                UrenT = 0;
                DagenT = 0;

                // Steeds per 3 regels van kleur wisselen
                TotaalRegel = htmlTabelWeekRegel.Replace("ProjectRegel", "ProjectRegelColor" + ((i % 6)/3).ToString());
                foreach (int prod in new List<int> { 1, 2 })
                {
                    foreach (int contr in _AllContracten)
                    {
                        UrenD = selxMANDAGEN.Where(x =>
                            x.Begintijd >= _StartTijd &&
                            x.Begintijd < _EindeTijd &&
                            x.ContractVW == contr && x.ProductieVW == prod
                            ).Sum(x => x.Uren) * 60;
                        UrenD += selxMANDAGEN.Where(x =>
                            x.Begintijd >= _StartTijd &&
                            x.Begintijd < _EindeTijd &&
                            x.ContractVW == contr && x.ProductieVW == prod
                            ).Sum(x => x.Minuten);
                        UrenD /= 60.0;
                        UrenT += UrenD;

                        // Dagen
                        DagenD = selxMANDAGEN.Where(x =>
                            x.Begintijd >= _StartTijd &&
                            x.Begintijd < _EindeTijd &&
                            x.ContractVW == contr && x.ProductieVW == prod
                            ).Select(y => new { y.VakmanId, y.Begintijd.Date }).Distinct().Count();
                        DagenT += DagenD;

                        // Totaal van dienstbetrekking invullen
                        TotaalRegel = TotaalRegel.Replace("{pcu" + prod.ToString() + contr.ToString() + "}", DagUurToString(UrenD));
                        TotaalRegel = TotaalRegel.Replace("{pcd" + prod.ToString() + contr.ToString() + "}", DagUurToString(DagenD));
                    } // Next contr
                } // Nect prod

                TotaalRegel = TotaalRegel.Replace("{omschrijving}", "Week " + ET.Weeknummer(_StartTijd,true));
                TotaalRegel = TotaalRegel.Replace("{tu}", DagUurToString(UrenT));
                TotaalRegel = TotaalRegel.Replace("{td}", DagUurToString(DagenT));

                //Datums
                if (UrenT > 0)
                {
                    mindag = selxMANDAGEN.Where(x => x.Begintijd >= _StartTijd && x.Begintijd < _EindeTijd).Select(x => x.Begintijd).Min().ToString("d-M-yyy");
                    maxdag = selxMANDAGEN.Where(x => x.Begintijd >= _StartTijd && x.Begintijd < _EindeTijd).Select(x => x.Begintijd).Max().ToString("d-M-yyy");
                }
                else
                {
                    mindag = string.Empty;
                    maxdag = string.Empty;
                }


                TotaalRegel = TotaalRegel.Replace("{van}" , mindag);
                TotaalRegel = TotaalRegel.Replace("{tot}",  maxdag);

                // loonkosten
                KostPrijs = Convert.ToDouble
                    (
                        selxMANDAGEN.Where(x =>
                            x.Begintijd >= _StartTijd &&
                            x.Begintijd < _EindeTijd &&
                            x.ProductieVW ==1 &&
                            x.ContractVW == 1 ) // Moet 'Per uur' zijn anders is het onzin)
                        .Sum(x => x.Loonkosten)
                    );
                TotaalRegel = TotaalRegel.Replace("{KPi}", ET.EuroPresentatie(KostPrijs, 2));

                KostPrijs = Convert.ToDouble
                    (
                        selxMANDAGEN.Where(x =>
                            x.Begintijd >= _StartTijd &&
                            x.Begintijd < _EindeTijd &&
                            x.ProductieVW == 2 &&
                            x.ContractVW == 1 ) // Moet 'Per uur' zijn anders is het onzin)
                        .Sum(x => x.Loonkosten)
                    );
                TotaalRegel = TotaalRegel.Replace("{KPe}", ET.EuroPresentatie(KostPrijs, 2));

                // Regel compleet. Row toevoegen aan tabelregels
                records += TotaalRegel; Rows++;
            
            }

            // Nodig ivm eerste paar subregels bij kopregel op één pagina houden: 
            // Pas Rowspan aan indien er minder subregels bij elkaar te houden zijn dan in de rowspan is aangegeven
            if (Rows < RowSpan)
                records = ET.SetRowSpan(records, RowSpan, Rows);

            // Eddy Ready Go.
            // We hebben nu feitelijk alle regels gevuld, laten we de HTMl pagina opmaken!

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

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
using System.Globalization;
using EvoPdf;
using System.Configuration;

namespace MDR2PDF
{
    public partial class _1c_WeeklijstExternenBAM : UserControl
    {
        public _1c_WeeklijstExternenBAM()
        {
            InitializeComponent();
            htmlBody = VerwijderLabel(htmlcontainer, "Tabelregels");
            HTMLTotaal = VerwijderLabel(htmlcontainer, "Projecttabel");
            ProjectTabel = VerwijderLabel(GetLabel(htmlcontainer, "Projecttabel"), "Tabelregels");
            TotaalRegel = GetLabel(htmlcontainer, "Totaalregel");
            DetailRegel = GetLabel(htmlcontainer, "Detailregel");
        }

        static string xHtmlFolder = ConfigurationManager.AppSettings["htmlFolder"].ToString();
        public EvoTools ET = new EvoTools(xHtmlFolder);

        public int Aantal_Regels_per_Pagina = -1; // Pagina Totaalregel uit indien '-1'
        public int[] PaginaTotalen = new int[8];

        // EVO HTML Variabelen
        static string pathcontainer = System.IO.Path.Combine(xHtmlFolder, "html/Lijst1/Lijst1c.html");

        public string htmlcontainer = File.ReadAllText(pathcontainer);

        public string htmlBody;
        public string HTMLTotaal;
        public string ProjectTabel;
        public string TotaalRegel;
        public string DetailRegel;
        public int Volgnummer;
        public int Detailregels = 0;
        public int DetailregelsLeft = 0; // Aantal regels dat er nog afgedrukt zullen worden


        private static string VerwijderLabel(string htmlcontainer, string Label)
        {
            int P1 = htmlcontainer.ToLower().IndexOf("<!-- "+ Label.ToLower());
            int P2 = htmlcontainer.ToLower().IndexOf("<!-- /"+ Label.ToLower());
            if (P1 * P2 < 0)
            {
                MessageBox.Show("Label '" + Label + "' niet gevonden");
                return "";
            }
            string s1 = htmlcontainer.Substring(0, P1);
            string S2 = htmlcontainer.Substring(P2);
            return s1 + S2;
        }

        private static string GetLabel(string htmlcontainer, string Label)
        {
            int P1 = htmlcontainer.ToLower().IndexOf("<!-- " + Label.ToLower());
            int P2 = htmlcontainer.ToLower().IndexOf("<!-- /" + Label.ToLower());
            if (P1 * P2 < 0)
            {
                MessageBox.Show("Label '" + Label + "' niet gevonden");
                return "";
            }
            P1 += Label.Length + "<!--__-->".Length;
            return htmlcontainer.Substring(P1, P2 - P1);
        }

        public string records = string.Empty;

        public int RegelsOpDezePagina = 0;

        public List<DateTime> WeekStart = new List<DateTime>();
        public List<DateTime> WeekEinde = new List<DateTime>();

        /// <summary>
        /// Deel het project in in perioden van weken waarbij aaneengesloten lege weken bij elkaar samengevoegd worden.
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="Maandag"></param>
        /// <param name="selxMANDAGEN"></param>
        /// <param name="pEindDatum"></param>
        internal void Bepaal_Projectweken(int ProjectId, DateTime Maandag, List<PoortViewMandagen> selxMANDAGEN, DateTime pEindDatum )
        {
            WeekStart.Clear();
            WeekEinde.Clear();
            do
            {
                WeekStart.Add(Maandag);
                DateTime NextMaandag = Maandag.AddDays(7);
                while (selxMANDAGEN.Where(x => 
                                        x.Begintijd >= Maandag && x.Begintijd < NextMaandag.AddDays(7) &&
                                        x.ProjectId == ProjectId).Count() == 0 && 
                                        NextMaandag <= pEindDatum)
                    NextMaandag = NextMaandag.AddDays(7);
                WeekEinde.Add(NextMaandag);
                Maandag = NextMaandag;
            } while (Maandag <= pEindDatum);// pAllMaxDatum);// pEindDatum);
        }

        internal void Maak_Lijst(DateTime pStartDatum, DateTime pEindDatum, List<PoortViewMandagen> selxMANDAGEN, List<allArbeidsRelaty> _AllArbeidsrelaties, List<PoortViewRegistratiebevoegd> _AllProjectleiders, List<PoortViewProject> _AllProjecten, List<PoortViewBedrijf> _AllBedrijven, List<PoortViewVakman> _AllVakmannen, EvoTools.HeaderFooter LijstGegevens)
        {

            string tabellen = string.Empty;
            // Bepaal start-maandag
            // Let op: Pak de HELE periode, niet slechts de keuze in de UI (Met showall wordt de periode automatisch uitgebreid!)
            DateTime pAllMinDatum = selxMANDAGEN.Select(x => x.Begintijd.Date).Min(); if (pStartDatum < pAllMinDatum) pAllMinDatum = pStartDatum;
            DateTime pAllMaxDatum = selxMANDAGEN.Select(x => x.Begintijd.Date).Max(); if (pEindDatum > pAllMaxDatum) pAllMaxDatum = pEindDatum;

            // Bepaal de maandag van de startweek
            //DateTime Maandag = pAllMinDatum;
            // We willen gewoon alle weken zien, ook de weken voorafgaande aan de eerste week met echte data dus pak pStartdatum
            DateTime Maandag = pAllMinDatum;
            while (Maandag.DayOfWeek != DayOfWeek.Monday)
                Maandag = Maandag.AddDays(-1);

            /*
             * Verzamel nu per project, per week de nodige data
             * */

            // Per Project
            string tabel = string.Empty;
            records = string.Empty;
            var ProjectIDs = selxMANDAGEN
                .OrderBy(x => x.project_NR)
                .Select(x => x.project_NR)
                .Distinct().ToList();
            bool FirstPage = true;
            foreach (var DitProject in _AllProjecten.Where(y => ProjectIDs.Contains(y.ProjectNR)))
            {
                // Bepaal weken voor dit project
                Bepaal_Projectweken(DitProject.ProjectId, Maandag, selxMANDAGEN, pAllMaxDatum);

                // Bereken nu alvast totaal aantal detailregels voor dit project
                DetailregelsLeft = selxMANDAGEN.Where(x =>
                    x.ProjectId == DitProject.ProjectId)
                    .Select(x => new { VK = x.VakmanId, Week = ET.weeknummerNEW(x.Begintijd,false)}).Distinct().ToList().Count();

                // Algemene Projectgegevens (Nieuwe tabel)
                tabel = ProjectTabel;
                if (FirstPage)
                {
                    tabel = tabel.Replace("TableOnNewPage", "TableOnSamePage");
                    FirstPage = false;
                }
                tabel = tabel.Replace("{Bestandscode}", LijstGegevens.LijstCode);
                string WeekS = ET.weeknummerNEW(pAllMinDatum,true);
                string WeekE = ET.weeknummerNEW(pAllMaxDatum, true);
                tabel = tabel.Replace("{Week}", WeekS + (WeekS == WeekE ? "" : " tot en met " + WeekE));
                tabel = tabel.Replace("{Periode}", string.Format("{0} - {1}",pAllMinDatum.ToLongDateString(), pAllMaxDatum.ToLongDateString()));
                tabel = tabel.Replace("{Projectnummer}",DitProject.ProjectNR.ToString());
                string __Opdrachtgever = (from og in _AllBedrijven
                                            where og.bedrijf_nr == DitProject.BedrijfVW
                                            select og.naam).FirstOrDefault() ?? "{Onbekend}";
                // Opdrachtger / Aannemer (aannemer = 'Select AannwmerVW, anders gelijk aan opdrachtgever)
                tabel = tabel.Replace("{Opdrachtgever}", __Opdrachtgever);
                if (DitProject.AannemerVW == DitProject.BedrijfVW || DitProject.AannemerVW == null)
                    tabel = tabel.Replace("{Aannemer}", __Opdrachtgever);
                else
                    tabel = tabel.Replace("{Aannemer}", (from og in _AllBedrijven
                                                         where og.bedrijf_nr == DitProject.AannemerVW
                                                         select og.naam).FirstOrDefault() ?? "{Onbekend}");

                tabel = tabel.Replace("{Projectnaam}", DitProject.Naam);
                tabel = tabel.Replace("{Projectlocatie}", DitProject.plaats);
                tabel = tabel.Replace("{Projectstraat}", DitProject.bouw_straat);

                var UitvoerdersIDsOpDitPorject = selxMANDAGEN.Where(x =>
                    x.project_NR == DitProject.ProjectNR)
                    .Select(x => x.ProjectleiderId).Distinct().ToList();
                var UitvoerdersOpDitPorject = selxMANDAGEN.Where(x =>
                    x.ProjectId == DitProject.ProjectNR)
                    .Select(x => x.ProjectleiderId).Distinct().ToList();
                string Uitvoerders = string.Empty;
                foreach (var uitv in _AllProjectleiders.Where(x => UitvoerdersIDsOpDitPorject.Contains(x.ProjectleiderId ?? -1)))
                {
                    Uitvoerders += (Uitvoerders == string.Empty ? "" : " / ");
                    Uitvoerders += uitv.Gebruikersnaam;
                }
                tabel = tabel.Replace("{Projectleider}", Uitvoerders);


                // ProjectTotaal hele periode (en gedetailleerd per vakman)
                records = string.Empty;

                // NIEUW voer totaalperiode-overzicht alleen uit als de periode meer dan twee weken beslaat. 
                // Anders heeft totaaltelling zo weing nut!
                DateTime mindag = selxMANDAGEN.Where(x => x.ProjectId == DitProject.ProjectId).Select(x => x.Begintijd).Min();
                DateTime maxdag = selxMANDAGEN.Where(x => x.ProjectId == DitProject.ProjectId).Select(x => x.Begintijd).Max();
                if (ET.weeknummerNEW(mindag, true) != ET.weeknummerNEW(maxdag, true))
                {
                    records += vul_TotaalRegel(selxMANDAGEN, DitProject);
                    // En nu uitgesplitst per vakman
                    foreach (var DezeVakman in selxMANDAGEN.Where(x =>
                        x.ProjectId == DitProject.ProjectId)
                        .Select(x => new
                        {
                            x.VakmanId,
                            _Ketenpartner = (x.KetenpartnerVW == 1 ? _AllArbeidsrelaties.Where(z => z.ArbeidsRelatieID == x.ArbeidsrelatieVW).Select(z => z.ArbeidsRelatieNaam).FirstOrDefault() : "Extern"),
                            _Vakman = _AllVakmannen.Where(y => y.VakmanId == x.VakmanId).FirstOrDefault()
                        }).Distinct().ToList()
                        .OrderBy(x => x._Vakman.Naam))
                    {
                        records += Vul_DetailRegel(selxMANDAGEN, DitProject, DezeVakman._Ketenpartner, DezeVakman._Vakman);
                    }
                }


                // ProjectTotaal per week (en detailregels uitgesplitst per Vakman)
                for (int i = 0; i < WeekStart.Count(); i++)
                {
                    Volgnummer = 1;
                    DateTime _StartTijd = WeekStart[i]; DateTime _EindeTijd = WeekEinde[i];
                    records += Vul_TotaalRegel(selxMANDAGEN, DitProject, _StartTijd, _EindeTijd);

                    // En nu uitgesplitst per vakman
                    foreach (var DezeVakman in selxMANDAGEN.Where(x =>
                        x.Begintijd >= _StartTijd &&
                        x.Begintijd < _EindeTijd &&
                        x.ProjectId == DitProject.ProjectId)
                        .Select(x => new
                        {
                            x.VakmanId,
                            _Ketenpartner = (x.KetenpartnerVW == 1 ? _AllArbeidsrelaties.Where(z => z.ArbeidsRelatieID == x.ArbeidsrelatieVW).Select(z => z.ArbeidsRelatieNaam).FirstOrDefault(): "Extern"),
                            _Vakman = _AllVakmannen.Where(y => y.VakmanId == x.VakmanId).FirstOrDefault()
                        }).Distinct().ToList()
                        .OrderBy(x => x._Vakman.Naam))
                    {
                        records += Vul_DetailRegel(selxMANDAGEN, DitProject, DezeVakman._Ketenpartner, DezeVakman._Vakman, _StartTijd, _EindeTijd);
                    }
                } // Week
                tabel = tabel.Replace("<!-- /TABELREGELS -->", records);
                tabellen += tabel;
            } // Project


            
            // Maak PDF aan volgens NIEUWE METHODE
            string test = HTMLTotaal.Replace("<!-- /PROJECTTABEL -->", tabellen);
            ET.MakePdf(test, LijstGegevens, false, 0);

            //ET.MakePdf(HTMLTotaal.Replace("<!-- /TABELREGELS -->", records), LijstGegevens, false);
        }


        /// <summary>
        /// Vul detailregel (Alle 'maandagen' bij elkaar opgeteld voor de hele periode)
        /// </summary>
        /// <param name="selxMANDAGEN"></param>
        /// <param name="DitProject"></param>
        /// <param name="_ArbeidsRelatieVW"></param>
        /// <param name="poortViewVakman"></param>
        /// <param name="_AllArbeidsrelaties"></param>
        private string Vul_DetailRegel(List<PoortViewMandagen> selxMANDAGEN, PoortViewProject DitProject, string _Ketenpartner, PoortViewVakman poortViewVakman)
        {
            // Om de 3 regels de kleur van de kleur van de detailregel omwisselen
            string regel = DetailRegel.Replace("Detailregel", "DetailregelColor" + ((Detailregels++ % 6) / 3).ToString());

            regel = regel.Replace("{Relatie}", _Ketenpartner);
            regel = regel.Replace("{BSN}", poortViewVakman.BSNLandcode + " " + poortViewVakman.Bsn);
            regel = regel.Replace("{Naam}", poortViewVakman.Naam);
            regel = regel.Replace("{Nr}", (Volgnummer++).ToString());
            DateTime EenWillekeurigeMaandag = new DateTime(2013, 10, 21).Date;

            int TOTAAL = 0;
            for (int Weekdag = 0; Weekdag <= 6; Weekdag++)
            {
                var DagNR = EenWillekeurigeMaandag.AddDays(Weekdag).DayOfWeek;
                int totaal = selxMANDAGEN
                    .Where(x =>
                        x.ProjectId == DitProject.ProjectId &&
                        x.VakmanId == poortViewVakman.VakmanId &&
                        x.Begintijd.DayOfWeek == DagNR)
                    .Select(x => x.Uren * 60 + x.Minuten).Sum();
                TOTAAL += totaal;
                regel = regel.Replace("{" + DagNR.ToString() + "}",Uurformaat(totaal));
            }
            return (TOTAAL > 0 ? regel.Replace("{Totaal}",Uurformaat(TOTAAL)) : "");
        }

        /// <summary>
        /// Vul Detailregel (Losse dagen binnen één week)
        /// </summary>
        /// <param name="selxMANDAGEN"></param>
        /// <param name="DitProject"></param>
        /// <param name="_ArbeidsRelatieVW"></param>
        /// <param name="poortViewVakman"></param>
        /// <param name="_AllArbeidsrelaties"></param>
        /// <param name="pStartDatum"></param>
        /// <param name="pEindDatum"></param>
        private string Vul_DetailRegel(List<PoortViewMandagen> selxMANDAGEN, PoortViewProject DitProject, string _Ketenpartner ,PoortViewVakman poortViewVakman, DateTime pStartDatum,DateTime pEindDatum)
        {
            // Om de 3 regels de kleur van de kleur van de detailregel omwisselen
            string regel = DetailRegel.Replace("Detailregel", "DetailregelColor" + ((Detailregels++ % 6) / 3).ToString());
            if (DetailregelsLeft-- == 3) // Neem nu extra grote rowspan om vorige rowspans waar deze rij in zit soort van te 'overrulen'
                regel = regel.Replace("</tr>", "<td rowspan=\"100\" style=\"page-break-inside:avoid\" > </td></tr>");

            regel = regel.Replace("{Relatie}", _Ketenpartner );
            regel = regel.Replace("{BSN}", poortViewVakman.BSNLandcode + " " + poortViewVakman.Bsn);
            regel = regel.Replace("{Naam}", poortViewVakman.Naam);

            int TOTAAL = 0;
            for (DateTime dag = pStartDatum ; dag <= pEindDatum; dag = dag.AddDays(1))
            {
                var TEST = selxMANDAGEN
                    .Where(x =>
                        x.ProjectId == DitProject.ProjectId &&
                        x.VakmanId == poortViewVakman.VakmanId &&
                        x.Begintijd.Day == dag.Day).ToList();
                int totaal = selxMANDAGEN
                    .Where(x =>
                        x.ProjectId == DitProject.ProjectId &&
                        x.VakmanId == poortViewVakman.VakmanId &&
                        x.Begintijd.Date == dag.Date)
                    .Select(x => x.Uren * 60 + x.Minuten).Sum();
                TOTAAL += totaal;
                regel = regel.Replace("{" + dag.DayOfWeek + "}", Uurformaat(totaal));
            }
            return TOTAAL > 0 ? (regel.Replace("{Totaal}", Uurformaat(TOTAAL)) ) : "";
        }


        /// <summary>
        /// Vul Totaalregel (Alle 'maandagen' bij elkaar opgeteld voor de hele periode)
        /// </summary>
        /// <param name="selxMANDAGEN"></param>
        /// <param name="DitProject"></param>
        private string vul_TotaalRegel(List<PoortViewMandagen> selxMANDAGEN, PoortViewProject DitProject)
        {
            string regel = TotaalRegel.Replace("{Periode}","Totaal");
            int TOTAAL = 0;
            Detailregels = 0;

            DateTime EenWillekeurigeMaandag = new DateTime(2013, 10, 21).Date;

            for (int Weekdag = 0; Weekdag <= 6; Weekdag++)
            {
                var DagNR = EenWillekeurigeMaandag.AddDays(Weekdag).DayOfWeek;
                int totaal = selxMANDAGEN
                    .Where(x => x.Begintijd.DayOfWeek == DagNR && x.ProjectId == DitProject.ProjectId)
                    .Select(x => x.Uren * 60 + x.Minuten).Sum();
                TOTAAL += totaal;
                regel = regel.Replace("{" + DagNR.ToString() + "}", Uurformaat(totaal));
            }

            if (TOTAAL > 0)
                return regel.Replace("{Totaal}",Uurformaat(TOTAAL));
            else
                return "";
        }

        /// <summary>
        /// Vul Totaalregel (alle afzonderlijke dagen binnen de gegeven periode van een week)
        /// </summary>
        /// <param name="selxMANDAGEN"></param>
        /// <param name="DitProject"></param>
        /// <param name="_StartTijd"></param>
        /// <param name="_EindeTijd"></param>
        private string Vul_TotaalRegel(List<PoortViewMandagen> selxMANDAGEN, PoortViewProject DitProject, DateTime _StartTijd, DateTime _EindeTijd)
        {
            string regel = TotaalRegel;
            if (_StartTijd == _EindeTijd.AddDays(-7))
                regel = regel.Replace("{Periode}", "Week "+ ET.weeknummerNEW(_StartTijd,true) + "");
            else
                if (_StartTijd.AddDays(14) == _EindeTijd)
                    regel = regel.Replace("{Periode}", "Week " + ET.weeknummerNEW(_StartTijd, true) + " en week " + ET.weeknummerNEW(_EindeTijd.AddDays(-7), true));
                else
                    regel = regel.Replace("{Periode}", "Week " + ET.weeknummerNEW(_StartTijd, true) + " t/m week " + ET.weeknummerNEW(_EindeTijd.AddDays(-7), true));

            int TOTAAL = 0;
            Detailregels = 0; 
            
            for (DateTime dag = _StartTijd; dag < _EindeTijd; dag = dag.AddDays(1))
            {
                var TEST = selxMANDAGEN
                    .Where(x => x.Begintijd.Date == dag.Date && x.ProjectId == DitProject.ProjectId)
                    .ToList();
                int totaal = selxMANDAGEN
                    .Where(x => x.Begintijd.Date  == dag.Date && x.ProjectId == DitProject.ProjectId)
                    .Select(x => x.Uren * 60 + x.Minuten).Sum();
                TOTAAL += totaal;
                regel = regel.Replace("{" + dag.DayOfWeek + "}", Uurformaat(totaal));
            }
            //return (TOTAAL> 0 ? regel.Replace("{Totaal}", Uurformaat(TOTAAL)) : "");
            // Return totaalregel altijd, dus ook bij 0-totaal
            return regel.Replace("{Totaal}", Uurformaat(TOTAAL));
        }

        private string Uurformaat(int totaal)
        {
            int UrenT = totaal / 60;
            int MinutenT = totaal - (UrenT * 60);

            if (UrenT == 0 && MinutenT == 0)
                return "";

            string min = MinutenT.ToString();
            return UrenT.ToString() + ":" + (min.Length == 1 ? "0" + min : min);
        }
    } //
} //


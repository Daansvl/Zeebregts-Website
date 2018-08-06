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
    public partial class Lijst1a : UserControl
    {
        static string xHtmlFolder = ConfigurationManager.AppSettings["htmlFolder"].ToString();
        public EvoTools ET = new EvoTools(xHtmlFolder);

        public int Aantal_Regels_per_Pagina = -1; // Pagina Totaalregel uit indien '-1'
        public int[] PaginaTotalen = new int[8];

        // EVO HTML Variabelen
        static string pathcontainer = System.IO.Path.Combine(xHtmlFolder, "html/Lijst1a/Container.html");
        static string pathhtmlpage = System.IO.Path.Combine(xHtmlFolder, "html/lijst1a/ProjectTabel.html");
        static string pathhtmlrecord = System.IO.Path.Combine(xHtmlFolder, "html/Lijst1a/ProjectRegel.html");

        static string htmlcontainer = File.ReadAllText(pathcontainer);
        static string htmltabel = File.ReadAllText(pathhtmlpage);
        static string htmltabelrow = File.ReadAllText(pathhtmlrecord);

        public string htmlBody = string.Empty;

        public string records = string.Empty;

        public int RegelsOpDezePagina = 0;

        public Lijst1a()
        {
            InitializeComponent();
        }

        internal void Maak_Lijst(DateTime pStartDatum, DateTime pEindDatum, List<PoortViewMandagen> selxMANDAGEN, List<allArbeidsRelaty> _AllArbeidsrelaties, List<PoortViewRegistratiebevoegd> _AllProjectleiders, List<PoortViewProject> _AllProjecten, List<PoortViewBedrijf> _AllBedrijven, List<PoortViewVakman> _AllVakmannen, EvoTools.HeaderFooter LijstGegevens)
        {
            // Start met allereerste pagina
            htmlBody = string.Empty;
            records = string.Empty;

            //
            // Deel periode in x-aantal weken in
            //

            // Week van Startdatum begin op maandag
            List<DateTime> WeekStart = new List<DateTime>();
            List<DateTime> WeekEinde = new List<DateTime>();
           
            // Trek DayofWeek van startdatum af om de maandag van die week te bepalen
            // Let op: Pak de HELE periode, niet slechts de keuze in de UI (Met showall wordt de periode automatisch uitgebreid!)
            DateTime pAllMinDatum = selxMANDAGEN.Select(x => x.Begintijd).Min();
            DateTime pAllMaxDatum = selxMANDAGEN.Select(x => x.Begintijd).Max();

            // Bepaal de maandag van de startweek
            DateTime Maandag = pAllMinDatum;
            while (Maandag.DayOfWeek != DayOfWeek.Monday)
                Maandag = Maandag.AddDays(-1);
            //DateTime PeriodeStart = Maandag;
            do
            {
                WeekStart.Add(Maandag);
                Maandag = Maandag.AddDays(7);
                WeekEinde.Add(Maandag);
            } while (Maandag <= pAllMaxDatum);// pEindDatum);
            //DateTime PeriodeEinde = Maandag;

            // Weekstart en einde kunnen nooit vóór of na totaalplaatje liggen!
            // Let daar op, want misschien is niet een gehele week geselecteerd, dan kan dit voorkomen. Pas periode's dan aan!
            //if (WeekStart[0] < pStartDatum)
            //    WeekStart[0] = pStartDatum;
            //if (WeekEinde[WeekEinde.Count - 1] > pEindDatum)
            //    WeekEinde[WeekEinde.Count - 1] = pEindDatum.AddDays(1);

            // Eerst even Totaal alle vakmannen
            double UrenT = 0;
            double UrenV = 0;
            int Vakmannen = 0;
            int VakmannenT = 0;

            string[] RegelVars = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

            // Bereken voor elke dienstbetrekking het totaal (per dag!!!)
            htmlBody = htmlcontainer;
            DayOfWeek DagvdWeek = new DayOfWeek();
            DateTime EenWillekeurigeMaandag = new DateTime(2013, 10, 21).Date;
            for (int Weekdag = 0; Weekdag <= 6; Weekdag++)
            {
                //int DagvdWeek = Weekdag == 0 ? 7 : Weekdag; // Dagvdweek begint nu van 7 (maandag), dan  
                DagvdWeek = EenWillekeurigeMaandag.AddDays(Weekdag).DayOfWeek; // Zo starten we dus bij Maandag en gaan door tot Zondag
                allArbeidsRelaty Allen = new allArbeidsRelaty();
                Allen.ArbeidsRelatieID = 0;
                Allen.ArbeidsRelatieNaam = "Select ALL";
                Allen.SorteerVolgorde = -1;
                _AllArbeidsrelaties.Add(Allen);
                //for (int c = -1; c <= 7; c = c + 2)
                foreach (var Arbeidsrelatie in _AllArbeidsrelaties)
                {
                    // Uren
                    UrenV = selxMANDAGEN.Where(x =>
                        (x.ArbeidsrelatieVW == Arbeidsrelatie.ArbeidsRelatieID || Arbeidsrelatie.ArbeidsRelatieID == -1)
                        && x.Begintijd.DayOfWeek == DagvdWeek
                        ).Sum(x => x.Uren) * 60;
                    UrenV += selxMANDAGEN.Where(x =>
                        (x.ArbeidsrelatieVW == Arbeidsrelatie.ArbeidsRelatieID || Arbeidsrelatie.ArbeidsRelatieID == -1)
                        && x.Begintijd.DayOfWeek == DagvdWeek
                        ).Sum(x => x.Minuten);
                    UrenV /= 60.0;
                    // Plaats ureen in HTML voorblad
                    htmlBody = htmlBody.Replace("{" + Weekdag.ToString() + "u" + Arbeidsrelatie.ArbeidsRelatieID.ToString() + "}"
                        , (UrenV == 0) ? "" : UrenV.ToString());

                    // Dagen
                    Vakmannen = selxMANDAGEN.Where(x =>
                        (x.ArbeidsrelatieVW == Arbeidsrelatie.ArbeidsRelatieID || Arbeidsrelatie.ArbeidsRelatieID == -1)
                        && x.Begintijd.DayOfWeek == DagvdWeek
                        )
                        // Pak liever versch. vakmannen per dag ipv versch aantal vakmnnen
                        //.Select(x => x.Mandag.VakmanId).Distinct().Count();
                        .Select(x => new { x.VakmanId, x.Begintijd.Date }).Distinct().Count();
                    //if (Arbeidsrelatie.ArbeidsRelatieID == -1)
                    //    RegelVars[Arbeidsrelatie.ArbeidsRelatieID + 2] = (Vakmannen == 0) ? "nvt" : Vakmannen.ToString();
                    //else
                        // Plaats vakmandagen in HTML voorblad
                        htmlBody = htmlBody.Replace("{" + Weekdag.ToString() + "d" + Arbeidsrelatie.ArbeidsRelatieID.ToString() + "}"
                            , (UrenV == 0) ? "" : Vakmannen.ToString());

                    // 'Halve vakmannen (vakmannen die op dezelfde dag op meerdere projecten zaten
                    // ??? hoeft hier volegsn mij geen rekening mee gehouden te worden; in het totaalplaatje zit dat al 'verrekend'
                }
            }

            // Zeker weten dat alle mogelijke {xxx} variabelen verwijderd zijn uit voorblad:
            for (int d = 0; d <= 6; d++)
            {
                for (int i = 0; i <= 4; i++)
                {
                    htmlBody = htmlBody.Replace("{" + d.ToString() + "d" + i.ToString() + "}", "");
                    htmlBody = htmlBody.Replace("{" + d.ToString() + "u" + i.ToString() + "}", "");
                }
            }



            // Periode
            htmlBody = htmlBody.Replace("[%periode%]",
                string.Format("{0:dd-MM-yyyy} t/m {1:dd-MM-yyyy}", pStartDatum, pEindDatum)); // Periode

            /*
             * Okay we hebben het totaal van alles, fijn.
             * nu dan per week:
             * (Week-tabblad maken op nieuw pagina met totalen)
             *  per Project
             *  (Project-tabblad maken op nieuwe pagina)
             *      Per Vakman
             *          Per dag van de week
             *          Gewerkte uren van deze valkman
             *      (Regel met alle uren van de week toevoegen aan tabel)
             * */


            // Bepaal voor elke week de totalen
            string Tabellen = string.Empty;
            for (int i = 0; i < WeekStart.Count(); i++)
            {
                DateTime _StartTijd = WeekStart[i];
                DateTime _EindeTijd = WeekEinde[i];

                UrenV = 0;
                UrenT = 0;

                Array.Clear(RegelVars, 0, 15);

                var ProjectIDsDezeWeek = selxMANDAGEN.Where(x =>
                    x.Begintijd >= _StartTijd &&
                    x.Begintijd < _EindeTijd)
                    .OrderBy(x => x.project_NR)
                    .Select(x => x.project_NR)
                    .Distinct().ToList();
                // Per project
                foreach (var DitProject in  _AllProjecten.Where(y => ProjectIDsDezeWeek.Contains(y.ProjectNR)))
                {
                    // Start tabel
                    records = string.Empty;
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


                    string NieuwProjectTabel = ET.ReplStr(htmltabel, new string[] {
                        DitProject.ProjectNR.ToString(),
                        (from og in _AllBedrijven
                                            where og.bedrijf_nr == DitProject.BedrijfVW
                                            select og.naam).FirstOrDefault() ?? "<Onbekend>",
                        DitProject.Naam,
                        DitProject.plaats,
                        DitProject.bouw_straat,
                        Uitvoerders,
                        CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(_StartTijd, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString(),
                        _StartTijd.ToShortDateString()  + " t/m " + _EindeTijd.AddDays(-1).ToShortDateString()
                    }, 8);

                    //Per Vakman
                    //foreach (var DezeVakman in selxMANDAGEN.Where(x =>
                    //    x.Begintijd >= _StartTijd &&
                    //    x.Begintijd < _EindeTijd &&
                    //    x.ProjectId == DitProject.ProjectId)
                    //    .OrderBy(x => x.ArbeidsrelatieVW) //.ThenBy(x => x.VolledigeNaam)
                    //    .Select(x => new { x.VakmanId, x.ArbeidsrelatieVW}).Distinct().ToList())
                    foreach (var DezeVakman in selxMANDAGEN.Where(x =>
                        x.Begintijd >= _StartTijd &&
                        x.Begintijd < _EindeTijd &&
                        x.ProjectId == DitProject.ProjectId)
                        .Select(x => new { x.VakmanId, x.ArbeidsrelatieVW, 
                            _Vakman = _AllVakmannen.Where(y => y.VakmanId == x.VakmanId).FirstOrDefault()
                        }).Distinct().ToList()
                        .OrderBy(x => x.ArbeidsrelatieVW).ThenBy(x => x._Vakman.Naam)) //.ThenBy(x => x.VolledigeNaam)
                    {
                        Array.Clear(RegelVars, 0, 17);
                        string Arbeidsrelatie = _AllArbeidsrelaties.Where(x => x.ArbeidsRelatieID == DezeVakman.ArbeidsrelatieVW).Select(x => x.ArbeidsRelatieNaam).FirstOrDefault();
                        //PoortViewVakman _Vakman = _AllVakmannen.Where(x => x.VakmanId == DezeVakman.VakmanId).FirstOrDefault();
                        RegelVars[0] = Arbeidsrelatie + // Moet zijn: Werkelijke waarde 
                            (Arbeidsrelatie.ToLower() == "intern" ?
                            " " + DezeVakman.VakmanId.ToString() : "");

                        RegelVars[1] = DezeVakman._Vakman.Bsn;// _Vakman.Bsn;

                        RegelVars[2] = DezeVakman._Vakman.Naam;

                        // Regelvar[3] geeft uren van Maandag... ect tm regelvar[9] (zondag)
                        // Bepaal startpositie afhankelijk van de dag (if maandag, then startpositie = 3)
                        int regelDagvdWeekPositie = 2 + (
                            ((int)_StartTijd.DayOfWeek == 0) ? 7 : (int)_StartTijd.DayOfWeek // .Dayofweek loopt van 0 (zonadg) tot Zaterdag (6)
                            );
                        // MSZ: 2014-1-22 Hey dit is fout, het moet < zijn en niet <=
                        //for (DateTime dag = _StartTijd.Date; dag <= _EindeTijd; dag = dag.AddDays(1))
                        for (DateTime dag = _StartTijd.Date; dag < _EindeTijd; dag = dag.AddDays(1))
                        {
                            string project = DitProject.Naam;
                            UrenV = selxMANDAGEN.Where(x =>
                                x.Begintijd >= dag &&
                                x.Begintijd < dag.AddDays(1) &&
                                x.Begintijd.Date == dag &&
                                x.ProjectId == DitProject.ProjectId &&
                                x.VakmanId == DezeVakman.VakmanId)
                                .Select(x => x.Uren* 60 + x.Minuten).Sum() / 60.0;
                            //UrenV += selxMANDAGEN.Where(x =>
                            //    x.Mandag.Begintijd >= dag &&
                            //    x.Mandag.Begintijd < dag.AddDays(1) &&
                            //    x.Mandag.Begintijd.Date == dag &&
                            //    x.Mandag.ProjectId == DitProject.Project.MandagenReg.ProjectId &&
                            //    x.Mandag.VakmanId == DezeVakman.Vakman.MandagenReg.VakmanId)
                            //    .Select(x => x.Mandag.Minuten).Sum();
                            //UrenV /= 60.0;

                            RegelVars[regelDagvdWeekPositie++] = (UrenV == 0) ? "" : UrenV.ToString();
                        } // Next Dag van de Week

                        // Voeg nieuwe tabel-regel toe
                        records += ET.ReplStr(htmltabelrow, RegelVars, 17);

                    } // Next Vakman

                    NieuwProjectTabel = NieuwProjectTabel.Replace("[%Regels%]", records);
                    Tabellen += NieuwProjectTabel;

                } // Next Project

            } // Next Week

            // Eddy Ready Go.
            // We hebben nu feitelijk alle regels gevuld, laten we de HTMl pagina opmaken!


            // NIEUWE METHODE:
            string HtmLtOTAAL = htmlBody.Replace("[%Overzicht%]", Tabellen);
            ET.MakePdf(HtmLtOTAAL, LijstGegevens,true);
            return;


            //
            // Open Evo gebeuren
            //

            PdfConverter pdfConverter = new PdfConverter();
            pdfConverter.LicenseKey = "B4mYiJubiJiInIaYiJuZhpmahpGRkZE=";
            pdfConverter.LicenseKey = "EpyMnY6OnYyPjJ2Jk42djoyTjI+ThISEhA==";

            // Linker en rechter-marge instellen want het document was net te breed
            pdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Landscape;
            pdfConverter.PdfDocumentOptions.LeftMargin = 10;
            pdfConverter.PdfDocumentOptions.RightMargin = 10;
            pdfConverter.PdfDocumentOptions.TopMargin = 10;
            pdfConverter.PdfDocumentOptions.BottomMargin = 10;
            pdfConverter.PdfDocumentOptions.FitWidth = true; // Default
            pdfConverter.PdfDocumentOptions.AutoSizePdfPage = true; //Default

            ET.AddHeaderElements(pdfConverter);


            // HTML Totaalplaatje genereren
            //string fullHtml = htmlcontainer.Replace("[%Overzicht%]", htmlBody);
            string fullHtml = htmlBody.Replace("[%Overzicht%]", Tabellen);
            // HTML: PDF creeren
            string outFilePath = System.IO.Path.Combine(xHtmlFolder, "ConvertHtmlString-" + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Millisecond + ".pdf");
            pdfConverter.SavePdfFromHtmlStringToFile(fullHtml, outFilePath);

            System.Diagnostics.Process.Start(outFilePath);

        }


    }//
}//

using EvoPdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDR2PDF
{

    public class EvoTools
    {
        // EVO HTML Variabelen
        static string pathcontainer = "html/Algemeen/container.html"; // System.IO.Path.Combine(Environment.CurrentDirectory, "html/Algemeen/container.html");
        static string pathcontainerNIB = "html/Algemeen/containerNoInfoBlock.html"; // System.IO.Path.Combine(Environment.CurrentDirectory, "html/Algemeen/containerNoInfoBlock.html");
        static string pathfooter = "html/Algemeen/footer.html"; // System.IO.Path.Combine(Environment.CurrentDirectory, "html/Algemeen/footer.html");
        static string pathstylesheet = "html/Algemeen/stylesheet.css"; // System.IO.Path.Combine(Environment.CurrentDirectory, "html/Algemeen/stylesheet.css");

        static string htmlContainer = string.Empty; // File.ReadAllText(pathcontainer);
        static string htmlContainerNoInfoBlock = string.Empty; // File.ReadAllText(pathcontainerNIB);
        static string htmlFooter = string.Empty; // File.ReadAllText(pathfooter);
        static string htmlStyleSheet = string.Empty; // File.ReadAllText(pathstylesheet);

        static string xHtmlFolder = ConfigurationManager.AppSettings["htmlFolder"].ToString();

        public EvoTools(string htmlFolder)
        {
            htmlContainer = File.ReadAllText(System.IO.Path.Combine(htmlFolder, pathcontainer));
            htmlContainerNoInfoBlock = File.ReadAllText(System.IO.Path.Combine(htmlFolder, pathcontainerNIB));
            htmlFooter = File.ReadAllText(System.IO.Path.Combine(htmlFolder, pathfooter));
            htmlStyleSheet = File.ReadAllText(System.IO.Path.Combine(htmlFolder, pathstylesheet));
        }


        public string htmlBody = string.Empty;

        public class HeaderFooter
        {
            public string LijstNaam = string.Empty;
            public string LijstCode = string.Empty;
            public string Periode = string.Empty;
            public DateTime UI_Startdatum;
            public DateTime UI_Einddatum;
            public List<List<string>> UISettings = new List<List<string>>();
        }

        /// <summary>
        /// Samenstelling volledige naam zonder extra spaties indien tussenvoegsel ontbreekt
        /// </summary>
        /// <param name="voornaam">Voornaam</param>
        /// <param name="tussenvoegsel">tussenvoegsel</param>
        /// <param name="achternaam">Achternaam</param>
        /// <returns></returns>
        public string VolledigeNaam(string voornaam, string tussenvoegsel, string achternaam)
        {
            return voornaam + (tussenvoegsel == null ? " " : " " + tussenvoegsel + " ") + achternaam;
        }

      
        /// <summary>
        /// Geef string van een double. 
        /// - Afronden naarboven;
        /// - Indien 0 dan spatie
        /// </summary>
        /// <param name="UrenT"></param>
        /// <returns></returns>
        internal string ToIntSTR(double Getal)
        {
            return Math.Ceiling(Getal) == 0 ? "&nbsp" : Math.Ceiling(Getal).ToString();
        }


        /// <summary>
        /// Algemene HTML functie á lá string.format !!!!!
        /// </summary>
        /// <param name="NieuweRegel">Input String met variabelen aangegeven als {0} {1} etc</param>
        /// <param name="VariabeleValues">string[] met inhoud variabelen</param>
        /// <param name="Aantal_Values">"Aantal variabelen"</param>
        /// <returns></returns>
        public string ReplStr(string NieuweRegel, string[] VariabeleValues, int Aantal_Values)
        {
            for (int i = 0; i < Aantal_Values; i++)
            {
                NieuweRegel = NieuweRegel.Replace("{" + i.ToString() + "}", VariabeleValues[i] ?? "&nbsp");
            }
            return NieuweRegel;
        }

        public string ReplStr(string NieuweRegel, double[] VariabeleValues, int Aantal_Values)
        {
            for (int i = 0; i < Aantal_Values; i++)
            {
                NieuweRegel = NieuweRegel.Replace("{" + i.ToString() + "}", VariabeleValues[i].ToString() ?? "&nbsp");
            }
            return NieuweRegel;
        }

        public List<String> VAKMANSOORTEN = new List<string> { "1", "Intern", "3", "ZZP", "5", "Uitzendkracht", "7", "Extern", "9", "Uitvoerder" };

        internal int VakmanSoort(Boolean Intern, bool? ZZP, int VakmanID , int? BedrijfNR)
        {
            // Ivm juiste volgorde indien vakmannen gesorteerd moeten worden
            // Beginnen we bij 1 en verhogen steeds met 2 omdat dan standaard HTML-lijst makkelijker te vullen is
            //
            // standaard HTML Lijst is namelijk in de vorm 
            // "Naam: {0}    Interne Uren/Dagen: {1}/{2}    ZZP uren/dagen: {3}/{4}   etcetc"
            //
            // 1 = Intern
            // 3 = ZZP
            // 5 = Uitzendkracht
            // 7 = Extern
            // 9 = Uitvoerder (Alleen Eddy en Marc)

            //if (VakmanID == 147 || VakmanID == 148)
            //    return 9; // UTA, Werkvoorbreider, Overhead
            if (Intern == true)
                return 5;
            if (ZZP == true)
                return 3; // ZZP
            //if (Uitzendkracht == true)
            //    return 5; // Uitzendkracht
            if (BedrijfNR == 1)
                return 1; // Intern
            //return 7; // Extern
            return 1; // 2014-03-25 Extern wordt voortaan Intern
        }

        internal void AddHeaderElementsLijst5(PdfConverter pdfConverter, string HTMLInHeader)
        {

            pdfConverter.PdfDocumentOptions.ShowHeader = true;

            //write the page number
            //TextElement HeaderText = new TextElement(0, pdfConverter.PdfFooterOptions.FooterHeight  - 15,
            //            "Pagina nummer &p; van &P; ",
            //            //new System.Drawing.Font(new System.Drawing.FontFamily(System.Drawing.Text.GenericFontFamilies.SansSerif),// "Times New Roman"),
            //            new System.Drawing.Font(new System.Drawing.FontFamily("Calibri"),// "Nog steeds Times New Roman!!!"),
            //            8, System.Drawing.GraphicsUnit.Point));
            TextElement HeaderText = new TextElement(0, pdfConverter.PdfHeaderOptions.HeaderHeight - 15,
                        "Pagina nummer &p; van &P; ",
                //new System.Drawing.Font(new System.Drawing.FontFamily(System.Drawing.Text.GenericFontFamilies.SansSerif),// "Times New Roman"),
                        new System.Drawing.Font(new System.Drawing.FontFamily("Calibri"),// "Nog steeds Times New Roman!!!"),
                        8, System.Drawing.GraphicsUnit.Point));
            HeaderText.EmbedSysFont = true;
            HeaderText.TextAlign = HorizontalTextAlign.Right;
            pdfConverter.PdfHeaderOptions.AddElement(HeaderText);

            HtmlToPdfElement headerHtml = new HtmlToPdfElement(0, 0, 0, pdfConverter.PdfHeaderOptions.HeaderHeight,
            HTMLInHeader, null, 1024, 0);
            headerHtml.FitHeight = true;
            headerHtml.EmbedFonts = true; //new  cbEmbedFonts.Checked;
            pdfConverter.PdfHeaderOptions.AddElement(headerHtml);
        }


        public void AddHeaderElements(PdfConverter pdfConverter)
        {
            AddHeaderElements(pdfConverter, htmlFooter);
        }

        public void AddHeaderElements(PdfConverter pdfConverter, string htmlFooter)
        {
            pdfConverter.PdfDocumentOptions.LeftMargin = 30;
            pdfConverter.PdfDocumentOptions.RightMargin = 30;
            pdfConverter.PdfDocumentOptions.TopMargin = 30;
            pdfConverter.PdfDocumentOptions.BottomMargin = 30;

            // handle the PrepareRenderPdfPageEvent to set the header and footer visibility on different pages
            pdfConverter.PrepareRenderPdfPageEvent += new PrepareRenderPdfPageDelegate(pdfConverter_PrepareRenderPdfPageEvent);

            pdfConverter.PdfDocumentOptions.ShowHeader = true;
            pdfConverter.PdfDocumentOptions.ShowFooter = true;
            pdfConverter.PdfHeaderOptions.HeaderHeight = 50;
            pdfConverter.PdfFooterOptions.FooterHeight = 60;

            // Logo linksBoven
            HtmlToPdfElement HtmlLogo  = new HtmlToPdfElement(5, 0, 0, 0,
                    "<img alt=\"\" src=\"http://www.zeebregts.nl/images/logo.png\" style=\"margin-bottom:15px\"/>", null , 1024,0);
            HtmlLogo.FitHeight = true;
            HtmlLogo.EmbedFonts = true;
            pdfConverter.PdfHeaderOptions.AddElement(HtmlLogo);

            // Aantal Pagina's Rechtsboven
            TextElement HeaderText = new TextElement(0, 10, "&p; van &P;  ", new System.Drawing.Font((new System.Drawing.FontFamily("Calibri")),8,System.Drawing.GraphicsUnit.Point)); //x
            HeaderText.EmbedSysFont = true;
            HeaderText.TextAlign = HorizontalTextAlign.Right;
            //HeaderText.TextFont.Size = 10;
            pdfConverter.PdfHeaderOptions.AddElement(HeaderText);

            // Footer rechtsonder (Alleen op eerste pagina)
            //if (pdfConverter.PdfDocumentOptions.w

            // Dit is echt debiel, maar ik moet gewoon handmatig de breedte aanpassen bij Landscape !!!
            if (pdfConverter.PdfDocumentOptions.PdfPageOrientation == PdfPageOrientation.Landscape)
                htmlFooter = htmlFooter.Replace("100%", "36.2cm");
            HtmlToPdfElement HtmlFooter = new HtmlToPdfElement(0, 0,0 , 0, htmlFooter, null ,1024, 0);
            //HtmlToPdfElement HtmlFooter = new HtmlToPdfElement(htmlFooter);
            HtmlFooter.FitHeight = true;
            HtmlFooter.EmbedFonts = true;
            HtmlFooter.FitWidth = true;
            HtmlFooter.StretchToFit = true;
            pdfConverter.PdfFooterOptions.AddElement(HtmlFooter);
        }


        /// <summary>
        /// PDF Event Hendler om header alleen op 1e pagina en footer juist alleen op volgende pagina's te tonen
        /// </summary>
        /// <param name="eventParams"></param>
        void pdfConverter_PrepareRenderPdfPageEvent(PrepareRenderPdfPageParams eventParams)
        {
            // set the footer invisibility after 1st page
            if (eventParams.PageNumber > 1)
                eventParams.Page.ShowFooter = false;
        }

        public void Log(string p, bool Nieuw)
        {
            //return;
            try // Als loggen niet lukt, geen probleem van maken
            {
                string xWeekLijst = ConfigurationManager.AppSettings["WeekLijst"].ToString();
                string PLogFile = Application.StartupPath + "\\" + Path.GetFileNameWithoutExtension(xWeekLijst) + ".log";
                using (System.IO.StreamWriter w = File.AppendText(PLogFile))
                {
                    //if (Nieuw)
                    //    File.Create(PLogFile);
                    w.WriteLine("{0:yyyy-MM-dd\tHH:mm:ss.fff\tssfff} \t {1}", DateTime.Now.Subtract(PProgramStartTijd), p);
                }
            }
            catch (Exception e) { MessageBox.Show(string.Format("Loggen van melding {0} mislukt. Reden: \n{1}", p, e.ToString())); }
        }

        public void Log(string p)
        {
            return;
            Log(p, false);
        }

        public TimeSpan PProgramStartTijd { get; set; }

        internal string HtmlFormat(string htmlrecord,string[] RegelVars)
        {
            int i = 0;
            foreach (string RegelVar in RegelVars)
            {
                htmlrecord = htmlrecord.Replace("{"+ i.ToString() + "}", RegelVar) ;
                if (RegelVar == null)
                    return htmlrecord;
                i++;
            }
            return htmlrecord;
        }



        internal string EuroPresentatie(double Getal, int Decimalen)
        {
            //double euros = Math.Ceiling(p);
            double euros = Getal;
            if (euros == 0)
                return "";
            switch (Decimalen)
            {
                case 1:
                    return string.Format("{0:c1}", euros); // c0 geeft aan: Currency dus € en 0 decimalen
                case 2:
                    return string.Format("{0:c2}", euros); // c0 geeft aan: Currency dus € en 0 decimalen
                default:
                    return string.Format("{0:c0}", euros); // c0 geeft aan: Currency dus € en 0 decimalen
            }
        }


        /// <summary>
        /// Maak PDF van html invoer
        /// </summary>
        /// <param name="htmlTabel">HTML invoer</param>
        /// <param name="LijstHeader">Lijstgegevens (Naam, subcode etc)</param>
        /// <param name="Landscape">Landscape (default = false)</param>
        public void MakePdf(string htmlTabel, HeaderFooter LijstHeader, bool Landscape)
        {
            MakePdf(htmlTabel, LijstHeader, Landscape, 1);
        }


        /// <summary>
        /// Maak PDF van html invoer
        /// </summary>
        /// <param name="htmlTabel">HTML invoer</param>
        /// <param name="LijstHeader">Lijstgegevens (Naam, subcode etc)</param>
        /// <param name="Landscape">Landscape (default = false)</param>
        /// <param name="Headernr">Header nummer (default = 1)</param>
        public void MakePdf(string htmlTabel, HeaderFooter LijstHeader, bool Landscape, int Headernr)
        {
            MakePdf(htmlTabel, LijstHeader, htmlFooter, Landscape, Headernr);
        }

        /// <summary>
        /// Maak PDF van html invoer
        /// </summary>
        /// <param name="htmlTabel">HTML invoer</param>
        /// <param name="LijstHeader">Lijstgegevens (Naam, subcode etc)</param>
        /// <param name="HtmlFooter">Aangepaste Footer in HTML code</param>
        /// <param name="Landscape">Landscape (default = false)</param>
        /// <param name="Headernr">Header nummer (default = 1)</param>
        public void MakePdf(string htmlTabel, HeaderFooter LijstHeader, string LijstFooter, bool Landscape, int Headernr)
        {
            PdfConverter pdfConverter = new PdfConverter();
            pdfConverter.LicenseKey = "B4mYiJubiJiInIaYiJuZhpmahpGRkZE=";
            pdfConverter.LicenseKey = "EpyMnY6OnYyPjJ2Jk42djoyTjI+ThISEhA==";

            if (Landscape)
                pdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Landscape;

            // HTML Totaalplaatje genereren
            string fullHtml = string.Empty;
            switch (Headernr)
            {
                case 0: // No Header
                    fullHtml = htmlContainerNoInfoBlock;
                    break;
                default:
                    fullHtml = htmlContainer;
                    //fullHtml = fullHtml.Replace("logo.png", pathlogo);
                    fullHtml = fullHtml.Replace("{lijst}", LijstHeader.LijstNaam);
                    fullHtml = fullHtml.Replace("{bestandscode}", LijstHeader.LijstCode);//"Bezetting: per project per week");
                    fullHtml = fullHtml.Replace("{periode}", LijstHeader.UI_Startdatum.ToLongDateString() + " - " + LijstHeader.UI_Einddatum.ToLongDateString());
                    for (int c = 0; c <= 5; c++)
                    {
                        fullHtml = fullHtml.Replace(
                            "{l" + c.ToString() + "}", (c < LijstHeader.UISettings.Count ? LijstHeader.UISettings[c][0] : ""));
                        fullHtml = fullHtml.Replace(
                            "{r" + c.ToString() + "}", (c < LijstHeader.UISettings.Count ? LijstHeader.UISettings[c][1] : ""));
                    }
                    break;
            }
            fullHtml = fullHtml.Replace("/*stylesheet*/", htmlStyleSheet);
            fullHtml = fullHtml.Replace("{tabellen}", htmlTabel);

            // Header en footer toevoegen
            AddHeaderElements(pdfConverter, LijstFooter);

            // HTML: PDF creeren
            string outFilePath = System.IO.Path.Combine(xHtmlFolder, LijstHeader.LijstCode + ".pdf");

            // PDF maken
            //pdfConverter.SavePdfFromHtmlStringToFile(fullHtml, outFilePath);// xHtmlFolder);

            // TEST JURACI

            Document pdfDocument = pdfConverter.GetPdfDocumentObjectFromHtmlString(fullHtml);
            AlternatieveHeader(pdfDocument, pdfConverter, LijstHeader.LijstCode);// + " " + LijstHeader.LijstNaam);

            pdfDocument.Save(outFilePath);
            System.Diagnostics.Process.Start(outFilePath);

            // Oude files opruimen
            string dirName = System.IO.Path.GetDirectoryName(outFilePath);
            try
            {
                Directory.GetFiles(dirName)
                         .Select(f => new FileInfo(f))
                         .Where(f =>
                             f.LastAccessTime < DateTime.Now.AddDays(-31) &&
                             f.Name.Length == 21 &&
                             f.Name.Substring(0, 1) == "d" &&
                             f.Name.Substring(7, 1) == "t" &&
                             f.Name.Substring(14, 1) == "r" &&
                             f.Name.Substring(17) == ".pdf" &&
                             f.Name.Substring(f.Name.Length - 4) == ".pdf"
                              )
                         .ToList()
                         .ForEach(f => f.Delete());
            }
            catch { }
        }

        public void MakePdf(string htmlTabel, HeaderFooter LijstHeader)
        {
            MakePdf(htmlTabel, LijstHeader, false);
        }

        private void AlternatieveHeader(Document pdfDocument, PdfConverter pdfConverter, string LijstCode)
        {
            if (pdfDocument.Pages.Count >= 2)
            {
                // get the alternate header and footer width and height
                // the width is given by the PDF page width
                float altHeaderFooterWidth = pdfDocument.Pages[0].ClientRectangle.Width;
                // the height is the same with the document header height from the PdfConverter object
                float altHeaderHeight = pdfConverter.PdfHeaderOptions.HeaderHeight;
                float altFooterHeight = pdfConverter.PdfFooterOptions.FooterHeight;

                // create the alternate header template
                Template altHeaderTemplate = pdfDocument.Templates.AddNewTemplate(altHeaderFooterWidth, altHeaderHeight);

                // add page numbering to the left of the header
                PdfFont pageNumberFont = pdfDocument.Fonts.Add(new Font(new FontFamily("calibri"),
                            8, GraphicsUnit.Point));
                TextElement pageNumbering = new TextElement(0, 10, "&p; van &P;  ", new System.Drawing.Font((new System.Drawing.FontFamily("Calibri")), 8, System.Drawing.GraphicsUnit.Point)); //x
                pageNumbering.EmbedSysFont = true;
                pageNumbering.TextAlign = HorizontalTextAlign.Right;
                TextElement HeaderTekstCodeLinks = new TextElement(5, 10, LijstCode, pageNumberFont);
                HeaderTekstCodeLinks.EmbedSysFont = true;
                HeaderTekstCodeLinks.TextAlign = HorizontalTextAlign.Left;

                
                //TextElement pageNumbering = new TextElement(10, 10, "Page &p; of &P;", pageNumberFont, Color.Blue);

                altHeaderTemplate.AddElement(pageNumbering);
                altHeaderTemplate.AddElement(HeaderTekstCodeLinks);

                for (int pageIndex = 1; pageIndex < pdfDocument.Pages.Count; pageIndex ++)
                {
                    PdfPage pdfPage = pdfDocument.Pages[pageIndex];

                    pdfPage.Header = altHeaderTemplate;
                    //pdfPage.Footer = altFooterTemplate;
                }
            }
        }

        /// <summary>
        /// Toont het weeknummer als string
        /// </summary>
        /// <param name="SelectieStart">Datum waarvan de week getoont moet worden</param>
        /// <param name="ShowYear">Toont Weeknummer / jaar</param>
        /// <returns></returns>
        internal string Weeknummer(DateTime SelectieStart, bool ShowYear)
        {
            // Pak de donderdag van de week (Dat is bepalend voor het jaar, dus of het gaat om week 53 of week 1)
            if (SelectieStart.DayOfWeek != DayOfWeek.Thursday)
                SelectieStart = SelectieStart.AddDays(DayOfWeek.Thursday - SelectieStart.DayOfWeek);

            if (ShowYear) return
                new GregorianCalendar(GregorianCalendarTypes.Localized).GetWeekOfYear(SelectieStart.AddDays(3), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) +
                 " / " + SelectieStart.AddDays(3).Year.ToString();
            else return
                new GregorianCalendar(GregorianCalendarTypes.Localized).GetWeekOfYear(SelectieStart.AddDays(3), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString();

        }

        internal string weeknummerNEW (DateTime fromDate, bool ShowYear)
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
                    return weeknummerNEW(startOfYear.AddDays(-1), ShowYear);
                case 53:
                    // If dec 31st falls before thursday it is week 01 of next year
                    if (endOfYear.DayOfWeek < DayOfWeek.Thursday)
                        return "1" + (ShowYear ? " / " + fromDate.AddDays(3).Year.ToString() : "");
                    else
                        return wk.ToString() + (ShowYear ? " / " + fromDate.AddDays(3).Year.ToString() : "");
                default: return wk.ToString() + (ShowYear ? " / " + fromDate.AddDays(3).Year.ToString() : "");
            }
        }


        internal int GetRowSpan(string htmlTabelTotaalRegel)
        {
            int i = htmlTabelTotaalRegel.IndexOf("rowspan");
            i = htmlTabelTotaalRegel.IndexOf('"',i);
            string rowspan = htmlTabelTotaalRegel.Substring(i, 3);
            rowspan = rowspan.Replace('"', ' ');
            i = Convert.ToInt16(rowspan);
            return i;
        }

        internal string SetRowSpan(string records, int RowSpan, int Rows)
        {
            int start = records.LastIndexOf("\"" + RowSpan + "\"");
            if (start > 0 ) 
                records = records.Remove(start, 3).Insert(start, "\"" + Rows + "\"");
            return records;
        }
    } //
} //

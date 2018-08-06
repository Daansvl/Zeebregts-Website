using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EvoPdf;
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Printen
{
    /// <summary>
    /// Interaction logic for ProjectleidersOverzicht.xaml
    /// </summary>
    public partial class Intro : MenuControl
    {

        public Intro()
        {
            InitializeComponent();
            #region Pagina specifieke informatie
            PageTitle = "Instellingen";
            PageSubtitle = "Gebruikers";

            if (Rechten.IsAdmin || ApplicationState.GetValue<bool>(ApplicationVariables.blnControlMDR))
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Hidden;
            }


            PageBackButtonText = "TERUG";
            PageOKButtonText = "WIJZIGEN";
            #endregion

            //this.OkClick += Wijzigen;

        }

        private void btnVakmanOverzicht_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                //string pathcontainer = System.IO.Path.Combine(xHtmlFolder, "html/Container.html");
                //string pathhtmlpage = System.IO.Path.Combine(xHtmlFolder, "html/VakmannenOverzicht.html");
                //string pathhtmlrecord = System.IO.Path.Combine(xHtmlFolder, "html/VakmannenOverzichtItem.html");

                //string container = File.ReadAllText(pathcontainer);
                //string htmlpage = File.ReadAllText(pathhtmlpage);
                //string htmlrecord = File.ReadAllText(pathhtmlrecord);

                //string htmlBody = string.Empty;

                //dbRepository dbrep = new dbRepository();
                //dbOriginalRepository dbrepOriginal = new dbOriginalRepository();

                //List<vwVakman> listVakmannen = dbrep.GetVakmannenAll(DateTime.Now.AddDays(-30),DateTime.Now);
                //List<Mandagen> listMandagen = dbrep.datacontext.Mandagens.ToList();
                //List<MDRpersoon> listPersoons = dbrepOriginal.datacontext.MDRpersoons.ToList();

                //foreach (vwVakman vakman in listVakmannen)
                //{
                //    string newPage = htmlpage;

                //    string records = string.Empty;
                //    MDRpersoon pers = listPersoons.Where(p => p.persoon_ID == vakman.ContactIdOrigineel).FirstOrDefault();
                //    if (pers != null)
                //    {
                //        newPage = newPage.Replace("[%Naam%]", (pers.voornaam + " " + pers.tussenvoegsel + " " + pers.achternaam)).ToStringTrimmed();
                //    }
                //    else
                //    {
                //        newPage = newPage.Replace("[%Naam%]", "Onbekende naam");
                //    }

                //    foreach (Mandagen mandag in listMandagen.Where(m => m.VakmanId == vakman.VakmanId).Take(20))
                //    {
                //        string newRecord = htmlrecord;
                //        string newItem = mandag.Begintijd.DayOfWeek.ToString();
                //        string newItem2 = mandag.Begintijd.ToString("MM-dd");
                //        string newItem3 = mandag.Uren.ToString() + ":" + mandag.Minuten;

                //        newRecord = newRecord.Replace("[%Item%]", newItem);
                //        newRecord = newRecord.Replace("[%Item2%]", newItem2);
                //        newRecord = newRecord.Replace("[%Item3%]", newItem3);

                //        records += newRecord;

                //    }

                //    newPage = newPage.Replace("[%OverzichtItems%]", records);

                //    htmlBody += newPage;
                //}

                //string fullHtml = container.Replace("[%Overzicht%]", htmlBody);

                //PdfConverter pdfConverter = new PdfConverter();

                //pdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Landscape;


                //// set the license key
                //pdfConverter.LicenseKey = "B4mYiJubiJiInIaYiJuZhpmahpGRkZE=";

                //// save the PDF bytes in a file on disk
                //string outFilePath = System.IO.Path.Combine(xHtmlFolder, "ConvertHtmlString-" + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "-" + DateTime.Now.Millisecond + ".pdf");

                //pdfConverter.SavePdfFromHtmlStringToFile(fullHtml, outFilePath);

                //MessageBoxResult dr = System.Windows.MessageBox.Show("Open the rendered file in an external viewer?", "Open Rendered File", System.Windows.MessageBoxButton.YesNo);

                //if (dr == MessageBoxResult.Yes)
                //{
                //    System.Diagnostics.Process.Start(outFilePath);
                //}
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return;
            }
            finally
            {
            }


        }




    }
}

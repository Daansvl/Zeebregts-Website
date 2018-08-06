using eBrochure_zeebregts.Classes;
using eBrochure_zeebregts.ExpertControls;
using eBrochure_zeebregts.ExpertControls.Models;
using ImageTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace eBrochure_zeebregts.Controls
{
    public partial class ExpertUitvoerPrijs : UserControl
    {
        private int ArticleRowCounter;
        private double TotaalPrijs;
        private ExtendedImage exImg;
        private Dictionary<string, double> HeleDozenLijst = new Dictionary<string, double>();
        
        public ExpertUitvoerPrijs()
        {
            InitializeComponent();
     
        }

      
        public void SetPaginaNR(int totalpages, int tegelpages)
        {
            paginaNummerTB.Text = "Pagina " + (tegelpages + 1) + " van " + totalpages;
        }
        
        public ExtendedImage GetImg(bool saveneeded, DateTime printDatum)
        {
            GenerateReport(saveneeded, printDatum);
            LayoutRoot.Measure(new Size(797, 1123));
            LayoutRoot.Arrange(new Rect(0.0, 0.0, 797, 1123));
            LayoutRoot.UpdateLayout();
           
             //while (!imageloaded)
            //{
            //    System.Threading.Thread.Sleep(500);
            //}
            var wb = new WriteableBitmap(2391, 3369);
            wb.Render(LayoutRoot, new ScaleTransform { ScaleX = 3, ScaleY = 3 });
            wb.Invalidate();
            ExtendedImage myImage = wb.ToImage();
            exImg = myImage;
            return exImg;
        }
        private void AddArticle(string text, double price, bool newline)
        {
            ContentGrid.RowDefinitions.Add(new RowDefinition());


            var tbO = new TextBlock
            {
                FontFamily = new FontFamily("Lucida Grande"),
                FontSize = 10,
                FontWeight = price < 0 ? FontWeights.Bold : FontWeights.Normal,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Margin = new Thickness(45, 0, 0, 0),
                Text = text
            };
            tbO.SetValue(Grid.ColumnProperty, 0);
            tbO.SetValue(Grid.RowProperty, ArticleRowCounter);
            ContentGrid.Children.Add(tbO);
            var tbP = new TextBlock
            {
                FontFamily = new FontFamily("Lucida Grande"),
                FontSize = 10,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Margin = new Thickness(0, 0, 45, 0),
                Text = price >= 0 ? "€" + String.Format("{0:0.00}", price) : ""
            };
            tbP.SetValue(Grid.ColumnProperty, 1);
            tbP.SetValue(Grid.RowProperty, ArticleRowCounter);
            ContentGrid.Children.Add(tbP);

            ArticleRowCounter++;

            if (newline)
            {
                ContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(15) });
                ArticleRowCounter++;
            }

            if (text != "subtotaal afname hele dozen")
            {
                TotaalPrijs += price >= 0 ? price : 0;
            }
           
        }
        private void GenerateReport(bool saveneeded, DateTime printDatum)
        {
            //var bitmapImage = new BitmapImage();
            
            //bitmapImage.UriSource = new Uri("Images/logo-payoff-online.jpg",UriKind.Relative);
            byte[] bytes;
            // Get an Image Stream
            using (var ms = ImageTools.Helpers.Extensions.GetLocalResourceStream(new Uri("Images/logo-payoff-online.jpg",UriKind.Relative)))
            {
                // reset the stream pointer to the beginning
                ms.Seek(0, 0);
                //read the stream into a byte array
                bytes = new byte[ms.Length];
                ms.Read(bytes, 0, bytes.Length);
            }
            //data now holds the bytes of the image
            if (bytes != null)
            {
                using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
                {
                    BitmapImage im = new BitmapImage();
                    im.SetSource(ms);
                    var BlueprintImg = new Image();
                    BlueprintImg.Source = im;
                    BlueprintImg.Width = 266;
                    HeaderGrid.Children.Add(BlueprintImg);
                    BlueprintImg.SetValue(Grid.ColumnProperty, 0);
                    BlueprintImg.SetValue(Grid.RowSpanProperty, 2);
                    BlueprintImg.SetValue(Grid.RowProperty, 0);
                    BlueprintImg.Margin = new Thickness(45,63,15,0);
                    BlueprintImg.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    BlueprintImg.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                }
            }
         
            //Header
            var ctx = Acumulator.Instance().ctx;
            var inst = Acumulator.Instance();

            //	TBKlantnaam.Text += Acumulator.Instance().GebruikersNaam;
            //	TBDatum.Text += DateTime.Now.ToShortDateString(); 
            var instapChecklist = new Dictionary<String, int>();
            HeleDozenLijst.Clear();
            MakeHeader();
            MakeFooter(saveneeded, printDatum);
            ///
            if (inst.OTracker != null && inst.OTracker.offerteRuimte_ != null)
            {

                foreach (Ruimte r in inst.OTracker.offerteRuimte_.Children)
                {

                    foreach (var p in r.GekozenTegels)
                    {
                        if (instapChecklist.ContainsKey(p.ID))
                        {
                            instapChecklist[p.ID] += 1;
                        }
                        else
                        {
                            instapChecklist.Add(p.ID, 1);
                        }
                    }
                    foreach (var p in r.GekozenProfielen)
                    {
                        if (instapChecklist.ContainsKey(p.ProfielID))
                        {
                            instapChecklist[p.ProfielID] += 1;
                        }
                        else
                        {
                            instapChecklist.Add(p.ProfielID, 1);
                        }
                    }
                }

              
                double totaalprijs = 0;
                foreach (Ruimte r in inst.OTracker.offerteRuimte_.Children)
                {
                    
                    if (r.GekozenPakket != null)
                    {
                        AddArticle(inst.MaakStartZin(r.Omschrijving), -1, true);
                        AddArticle(inst.MaakStartZin("Pakket: " + r.GekozenPakket.Omschrijving), r.GekozenPakket.PrijsHuidig, true);
                    }
                    foreach (OptieKeuze op in r.GekozenOpties)
                    {
                        op.laad_verder();
                        var dorpelcat = (from dc in ctx.SubCatPerRuimteDeels
                                         where dc.R_NR == op.OptieID && dc.SCB_NR == "SCB13"
                                         select dc.R_NR).FirstOrDefault();

                        

                        var tmptotPrijs = 0.0;
                        if (r.GekozenPakket != null)
                        {

                            tmptotPrijs += op.getMeerprijs(ctx, r.GekozenPakket.PakketPrijsgroep_NR);

                        }
                        else
                        {
                            var basispakket = (from rpt in ctx.RuimtesPerTypes
                                               where rpt.R_NR == inst.BB.HuidigRuimte.RuimteID
                                               select rpt.BasisPakket_NR).FirstOrDefault();
                            var optiebasisprijs = 0.0;
                            if (basispakket != null)
                            {
                                var basispakprijs = (from pr in ctx.MeerPrijzenRuimteOpties
                                                     where pr.R_NR == op.OptieID && pr.PP_NR == (from p in ctx.Pakkettens where p.P_ID == basispakket select p.PP_NR).FirstOrDefault()
                                                     select pr.meerprijs).FirstOrDefault();
                                if (basispakprijs != null)
                                {
                                    optiebasisprijs = inst.BerekenEindPrijs((double)basispakprijs);
                                }
                            }
                            if (optiebasisprijs == 0)
                            {
                                optiebasisprijs = op.BasisPrijs;
                            }
                            tmptotPrijs = optiebasisprijs;
                        }
                        AddArticle(inst.MaakStartZin(op.Omschrijving), tmptotPrijs, false);
                        totaalprijs += tmptotPrijs;
                        if (dorpelcat != null && r.GekozenTegels.Count > 0)
                        {
                            if (!inst.ProjFase.FilterDorpels)
                            {
                                var oms = r.GekozenTegels.Where(x => x.Omschrijving.Contains('€')).FirstOrDefault();
                                if (oms != null)
                                {
                                    var parts = oms.Omschrijving.Split('€');
                                    if (parts[1] != "0,00")
                                    {
                                        AddArticle("Meerprijs Dorpel", double.Parse(parts[1]), false);
                                        totaalprijs += double.Parse(parts[1]);
                                    }
                                }
                            }
                        }
                    }
                    foreach (var exmtrs in r.AanvullendeMeters)
                    {
                        if (!exmtrs.IsBasis)
                        {
                            exmtrs.ProdSoort = exmtrs.LinkedSubCat != null && (exmtrs.LinkedSubCat == "SCB13" || exmtrs.LinkedSubCat == "SCB18") ? ProductSoort.Dorpel : exmtrs.ProdSoort;

                            var eenheidsText = exmtrs.ProdSoort == ProductSoort.HoekProfiel ? "m x €" : exmtrs.ProdSoort == ProductSoort.Dorpel ? "st x €" : "m² x €";
                            var accText = (
                                            exmtrs.LinkedProduct != null ?
                                            exmtrs.LinkedProduct.Omschrijving :
                                            (
                                                exmtrs.LinkedHoekProf != null ?
                                                exmtrs.LinkedHoekProf.Omschrijving : ""
                                            )
                                          )
                                          + " (" + exmtrs.Meters + eenheidsText + exmtrs.Meerprijs.ToString() + ")";
                            var accPrijs = exmtrs.Meters * exmtrs.Meerprijs;
                            if (exmtrs.LinkedProduct != null)
                            {
                                ExpertMethods.instapPrijsCalc(exmtrs.LinkedProduct.ID, exmtrs.LinkedProduct.Omschrijving, instapChecklist, exmtrs.LinkedProduct.VerpakkingsToeslag,HeleDozenLijst);
                            }
                            else if (exmtrs.LinkedHoekProf != null)
                            {
                                ExpertMethods.instapPrijsCalc(exmtrs.LinkedHoekProf.ProfielID, exmtrs.LinkedHoekProf.Omschrijving, instapChecklist, exmtrs.LinkedHoekProf.VerpakkingsToeslag,HeleDozenLijst);
                            }
                            totaalprijs += accPrijs;
                            AddArticle(accText, accPrijs, false);
                        }
                    }
                    AddArticle("", -1, true);
                }
                //afname van hele dozen
                AddArticle("Afname Hele Dozen", -1, true);
                double dozenprijs = 0.0;
                foreach (var hd in HeleDozenLijst)
                {
                    AddArticle(hd.Key, hd.Value, false);
                    dozenprijs += hd.Value;
                }
                AddArticle("", -1, true);
                AddArticle("subtotaal afname hele dozen", dozenprijs, true);
                totaalprijs += dozenprijs;
                //
                //

                MakeTotals();
            }

        }
        private void MakeHeader()
        {
            var ctx = Acumulator.Instance().ctx;
            
            var kavelinfo = (from k in ctx.Bouwnummers
                             where k.B_ID == Acumulator.Instance().Bouwnr
                             select k.Omschrijving).FirstOrDefault().ToString();
            var completeBouwnummer = (from k in ctx.Bouwnummers
                                      where k.B_ID == Acumulator.Instance().Bouwnr
                                      select k).FirstOrDefault();
            var projinf = (from p in ctx.PRojects
                           where p.PR_ID == completeBouwnummer.PR_NR
                           select p.Omschrijving).FirstOrDefault();
            if (completeBouwnummer.KlantNaam != null && completeBouwnummer.KlantNaam != "")
            {
                klantnaamTB.Text = completeBouwnummer.KlantNaam;
            }
            if (completeBouwnummer.adres_straat != null && completeBouwnummer.adres_straat != "")
            {
                addresStTB.Text = completeBouwnummer.adres_straat;
                
            }
            if (completeBouwnummer.adres_plaats != null && completeBouwnummer.adres_plaats != "")
            {
                addresPostTB.Text = completeBouwnummer.adres_plaats;
            }
            if (completeBouwnummer.Telefoon1 != null && completeBouwnummer.Telefoon1 != "")
            {
                telnrsTB.Text = completeBouwnummer.Telefoon1;
            }
            else { telnrsTB.Text = ""; }
            if (completeBouwnummer.Telefoon2 != null && completeBouwnummer.Telefoon2.Length > 1)
            {
                telnrsTB.Text += " / " + completeBouwnummer.Telefoon2;
            }
            if (completeBouwnummer.email != null && completeBouwnummer.email != "")
            {
                emailTB.Text = completeBouwnummer.email;
            }
            ProjectNameTB.Text = projinf;
            BouwnummerTB.Text = completeBouwnummer.Omschrijving;
            if (Acumulator.Instance().HuidigGebruiker.Rol == UserRole.Adviseur)
            {
                AdvieseurTB.Text = Acumulator.Instance().HuidigGebruiker.GebruikersNaam;
            }
            else
            {
                AdvieseurTB.Text = "";
            }

        }
        private void MakeTotals()
        {
           
            var korting = Acumulator.Instance().ProjFase.Korting;
            //hier korting verrekenen
            var kortHolder = Acumulator.Instance().OTracker.offerteRuimte_.Korting;
            if (kortHolder != null)
            {
                korting = Acumulator.Instance().OTracker.offerteRuimte_.Korting.KortingBedrag;
            }
            ////////////////////////
            if (korting > 0)
            {

                subtotalTB.Visibility = Visibility.Visible;
                subtotalPrijsTB.Visibility = Visibility.Visible;
                kortingTB.Visibility = Visibility.Visible;
                kortingPrijsTB.Visibility = Visibility.Visible;

                subtotalPrijsTB.Text = "€" + String.Format("{0:0.00}", TotaalPrijs);
                
                var kortt = Math.Round(korting, 2);
                kortingPrijsTB.Text = "- €" + String.Format("{0:0.00}", (kortt));
                TotaalPrijs -= kortt;
            }

            totaalPrijsTB.Text = "€" + String.Format("{0:0.00}", TotaalPrijs);
        }
        private void MakeFooter(bool saveneeded, DateTime printDatum)
        {
            var ctx = Acumulator.Instance().ctx;
            var completeBouwnummer = (from k in ctx.Bouwnummers
                                      where k.B_ID == Acumulator.Instance().Bouwnr
                                      select k).FirstOrDefault();
            printDatumTB.Text = "Printdatum: " + printDatum.ToString("dd-MM-yyyy HH:mm:ss");// +(DateTime.Now+Acumulator.Instance().serverTimeDiff).ToString("dd-MM-yyyy HH:mm:ss");
            naamTB.Text = completeBouwnummer.KlantNaam;
            var versienr = "";
            if (saveneeded)
            {
                if (Acumulator.Instance().oOL != null)
                { versienr = "versie: " + (Acumulator.Instance().oOL.VersieFull + 1).ToString() + ".0"; }
                else
                {
                    versienr = "versie: 1.0";
                }
                //
                infoVersieTB.Text = "Prijsrapportage " + versienr;
                infoDatumTB.Text = printDatum.ToString("dd-MM-yyyy"); ;
                infoTijdTB.Text = printDatum.ToString("HH:mm:ss");

            }
            else
            {
                if (Acumulator.Instance().oOL != null) { versienr = "versie: " + Acumulator.Instance().oOL.VersieFull.ToString() + ".0"; } else { versienr = "versie: 1.0"; }
                if (Acumulator.Instance().OfferteDatum != null && Acumulator.Instance().OfferteDatum.Year > 2000)
                {

                    infoVersieTB.Text = "Prijsrapportage " + versienr;
                    infoDatumTB.Text = Acumulator.Instance().OfferteDatum.ToString("dd-MM-yyyy");
                    infoTijdTB.Text = Acumulator.Instance().OfferteDatum.ToString("HH:mm:ss");
                }
                else
                {
                    infoVersieTB.Text = "Prijsrapportage" + versienr;
                    infoDatumTB.Text = printDatum.ToString("dd-MM-yyyy"); ;
                    infoTijdTB.Text = printDatum.ToString("HH:mm:ss");
                }
            }
        
        }

        bool imageloaded = false;
     

       
    }
}

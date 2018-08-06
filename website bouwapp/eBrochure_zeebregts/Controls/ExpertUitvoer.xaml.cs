using eBrochure_zeebregts.UitvoerClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace eBrochure_zeebregts.Controls
{
    public partial class ExpertUitvoer : UserControl
    {
        private int ArticleRowCounter;
        
        public ExpertUitvoer()
        {
            InitializeComponent();
        }

        public void SetPaginaNR(int totalpages, int tegelpages)
        {
            paginaNummerTB.Text = "Pagina " + (tegelpages + 1) + " van " + totalpages;
        }

        public void InitNewPage(bool saveneeded, DateTime printDatum)
        {
            byte[] bytes;
            // Get an Image Stream
            using (var ms = ImageTools.Helpers.Extensions.GetLocalResourceStream(new Uri("Images/logo-payoff-online.jpg", UriKind.Relative)))
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
                    BlueprintImg.Margin = new Thickness(45, 63, 15, 0);
                    BlueprintImg.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    BlueprintImg.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                }
            }

            //Header
            var ctx = Acumulator.Instance().ctx;
            var inst = Acumulator.Instance();
            MakeHeader();
            MakeFooter(saveneeded, printDatum);
          
        }
        public bool InitNewRuimte(int ruimteCntr,Ruimte r)
        {
            var ctx = Acumulator.Instance().ctx;
            var nabew = r.qryNabewerkingen(ctx);
            ArticleRowCounter++; 
            ContentGrid.RowDefinitions.Add(new RowDefinition{ Height = GridLength.Auto });
            var secndlinetext = "";
            if (nabew.Count == 0)
            {
                return false;
                //skip if empty
                //secndlinetext = "Geen tegelwerk";
                //MessageBox.Show("geentegelwerk");
            }
            else
            {

            var tbP = new TextBlock // product text
            {
                FontFamily = new FontFamily("Lucida Grande"),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Margin = new Thickness(45,15, 0, 15),
                Text = Acumulator.Instance().MaakStartZin(r.Omschrijving)
            };
            tbP.SetValue(Grid.ColumnProperty, 0);
            tbP.SetValue(Grid.RowProperty, ArticleRowCounter);
            ContentGrid.Children.Add(tbP);

            
           
            
                //tegel hoogte
                var tegelh = (from h in r.Children
                              where h.GetType() == typeof(Classes.SubCatPerRuimteDeel)
                              && (h as Classes.SubCatPerRuimteDeel).SubCatNR == "SCB4"
                              select (h as Classes.SubCatPerRuimteDeel).Meters).Sum();
                if (r.PlafondHoogte == tegelh)
                {
                    secndlinetext = "Betegelen tot plafond";
                }
                else if (tegelh > 0)
                {
                    secndlinetext = "Betegelen tot " + tegelh + "mm";
                }
                else
                {
                    var ruimtenaam = (from br in ctx.Bron_Ruimtes
                                      where br.R_NR == r.RuimteID
                                      select br.RuimteNaam).FirstOrDefault();
                    secndlinetext = "Betegelen " + ruimtenaam;
                }
                ArticleRowCounter++;
            ContentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            var tb0 = new TextBlock
            {
                FontFamily = new FontFamily("Lucida Grande"),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 45, 0),
                Text = "Info"

            };
            tb0.SetValue(Grid.ColumnProperty, 1);
            tb0.SetValue(Grid.RowProperty, ArticleRowCounter);
            ContentGrid.Children.Add(tb0);

            }
            
            ArticleRowCounter++;
            ContentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            var tb1 = new TextBlock // product text
            {
                FontFamily = new FontFamily("Lucida Grande"),
                FontSize = 12,
                //FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 45, 0),
                Text =secndlinetext ,
            };
            tb1.SetValue(Grid.ColumnProperty, 1);
            tb1.SetValue(Grid.RowProperty, ArticleRowCounter);
            ContentGrid.Children.Add(tb1);

            ArticleRowCounter++;
            return true;
        }
        public bool AddPaginaBlok(PaginaBlok pb)
        {
            
            for (int i = 0; i < pb.TextRegels.Count; i++)
            {

                var newRow = new RowDefinition { Height = GridLength.Auto };
               
              
                var tbO = new TextBlock // kantlijn text
                {
                    FontFamily = new FontFamily("Lucida Grande"),
                    FontSize = 12,
                    FontWeight = FontWeights.Normal,
                    TextWrapping = TextWrapping.NoWrap,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    VerticalAlignment = System.Windows.VerticalAlignment.Top,
                    Margin = new Thickness(45, 0, 0, 0),
                    Text = pb.KantlijnRegels[i]
                };
                tbO.SetValue(Grid.ColumnProperty, 0);
                tbO.SetValue(Grid.RowProperty, ArticleRowCounter);
                
                var tbP = new TextBlock // product text
                {
                    FontFamily = new FontFamily("Lucida Grande"),
                    FontSize = 12,
                    TextWrapping = TextWrapping.Wrap,
                    FontWeight = pb.BoolRegels[i] == true ? FontWeights.Bold : FontWeights.Normal,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    VerticalAlignment = System.Windows.VerticalAlignment.Top,
                    Margin = new Thickness(0, 0, 45, 0),
                    Text = pb.TextRegels[i]
                };
                tbP.SetValue(Grid.ColumnProperty, 1);
                tbP.SetValue(Grid.RowProperty, ArticleRowCounter);


                ContentGrid.RowDefinitions.Add(newRow);
                ContentGrid.Children.Add(tbP);
                ContentGrid.Children.Add(tbO);
                ArticleRowCounter++;
                
            }
            var retval = true; 
            if(ArticleRowCounter > 24)
            { retval = false;
            //add extra empty row
            var tbP = new TextBlock // product text
            {
                FontFamily = new FontFamily("Lucida Grande"),
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap,
                FontWeight =  FontWeights.Normal,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 45, 0),
                Text = Environment.NewLine
            };
            var newRow = new RowDefinition { Height = GridLength.Auto };
               
            tbP.SetValue(Grid.ColumnProperty, 1);
            tbP.SetValue(Grid.RowProperty, ArticleRowCounter);
            ContentGrid.RowDefinitions.Add(newRow);
            ContentGrid.Children.Add(tbP);
            ArticleRowCounter++;
            }
            return retval;
        }
        
        public void AddBluePrint(Image blueprint,string oms)
        {
            TitelTb.Text = "Tekeningen";
            ContentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            blueprint.SetValue(Grid.ColumnProperty, 0);
            blueprint.SetValue(Grid.ColumnSpanProperty, 2);
            blueprint.SetValue(Grid.RowProperty, 0);
            ContentGrid.Children.Add(blueprint);
           

            AdvieseurTB.Text = oms;
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

    }
}

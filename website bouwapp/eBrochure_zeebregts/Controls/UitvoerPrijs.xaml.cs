using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using ImageTools;
using eBrochure_zeebregts.Classes;
using System.Windows;
using System.Windows.Media.Imaging;
using eBrochure_zeebregts.ExpertControls.Models;
using System.Collections.Generic;

namespace eBrochure_zeebregts.Controls
{
	public partial class UitvoerPrijs : UserControl
	{
		private ExtendedImage exImg;
        public UitvoerPrijs()
		{
			InitializeComponent();
		}
        public void SetPaginaNR(int totalpages, int tegelpages)
        {
            Footerpaginanr.Text = "Pagina " + (tegelpages + 1) + " van " + totalpages;
        }
        private void SetAdresBox(UitvoerPrijs Page)
        {
            var ctx = Acumulator.Instance().ctx;
            var completeBouwnummer = (from k in ctx.Bouwnummers
                                      where k.B_ID == Acumulator.Instance().Bouwnr
                                      select k).FirstOrDefault();
            int regelcounter = 0; // 1e regel start op canvas hoogte 140
            if (completeBouwnummer.KlantNaam != null && completeBouwnummer.KlantNaam != "")
            {
                regelcounter=1;
                Page.HeaderKlantnaam.Text += completeBouwnummer.KlantNaam;
                Canvas.SetTop(Page.HeaderKlantnaam, 140);
            }
            if (completeBouwnummer.adres_straat != null && completeBouwnummer.adres_straat != "")
            {
                regelcounter++;
                Page.HeaderadresST.Text += completeBouwnummer.adres_straat;
                Canvas.SetTop(Page.HeaderadresST, 125 + (regelcounter * 15));
            }
            if (completeBouwnummer.adres_plaats != null && completeBouwnummer.adres_plaats != "")
            {
                regelcounter++;
                Page.HeaderadresPP.Text += completeBouwnummer.adres_plaats;
                Canvas.SetTop(Page.HeaderadresPP, 125 + (regelcounter * 15));
            }
            if (completeBouwnummer.Telefoon1 != null && completeBouwnummer.Telefoon1 != "")
            {
                regelcounter++;
                Page.HeaderTel1_2.Text += completeBouwnummer.Telefoon1;
                Canvas.SetTop(Page.HeaderTel1_2, 125 + (regelcounter * 15));
            }
            if (completeBouwnummer.Telefoon2 != null && completeBouwnummer.Telefoon2.Length > 1)
            {
                Page.HeaderTel1_2.Text += " / " + completeBouwnummer.Telefoon2;
            }
            if (completeBouwnummer.email != null && completeBouwnummer.email != "")
            {
                regelcounter++;
                Page.HeaderEmail.Text += completeBouwnummer.email;
                Canvas.SetTop(Page.HeaderEmail, 125 + (regelcounter * 15));
            }
            
        }
        private void InitNewPage(bool saveneeded,DateTime printDatum)
        {
           // page_nr++;

            var Page = this;
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
            Page.HeaderProjInfo.Text = projinf;
            Page.TBBouwnummer.Text = completeBouwnummer.Omschrijving;
            SetAdresBox(this);
          /*  Page.HeaderKlantnaam.Text += completeBouwnummer.KlantNaam;
            Page.HeaderadresST.Text += completeBouwnummer.adres_straat;
            Page.HeaderadresPP.Text += completeBouwnummer.adres_plaats;
            Page.HeaderTel1_2.Text += completeBouwnummer.Telefoon1;
            if (completeBouwnummer.Telefoon2 != null && completeBouwnummer.Telefoon2.Length > 1)
            {
                Page.HeaderTel1_2.Text += " / " + completeBouwnummer.Telefoon2;
            }
            Page.HeaderEmail.Text += completeBouwnummer.email;*/
            Page.FooterPrintDatum.Text = "Printdatum: " + printDatum.ToString("dd-MM-yyyy HH:mm:ss");// +(DateTime.Now+Acumulator.Instance().serverTimeDiff).ToString("dd-MM-yyyy HH:mm:ss");
            if (Acumulator.Instance().HuidigGebruiker.Rol == UserRole.Adviseur)
            {
                Page.HeaderAdviseur.Text += Acumulator.Instance().HuidigGebruiker.GebruikersNaam;
            }
            else
            {
                Page.HeaderAdviseur.Text = "";
            }
            var versienr = "";
///////////////////////////
            if (saveneeded)
            {
                if (Acumulator.Instance().oOL != null)
                { versienr = "versie: " + (Acumulator.Instance().oOL.VersieFull + 1).ToString() + ".0"; }
                else
                {
                    versienr = "versie: 1.0";
                }
                //
                Page.FooterInfoVersie.Text = "Prijsrapportage " + versienr;
                Page.FooterInfoDatum.Text =printDatum.ToString("dd-MM-yyyy"); ;
                Page.FooterInfoTijd.Text = printDatum.ToString("HH:mm:ss");

            }
            else
            {
                if (Acumulator.Instance().oOL != null) { versienr = "versie: " + Acumulator.Instance().oOL.VersieFull.ToString() + ".0"; } else { versienr = "versie: 1.0"; }
                if (Acumulator.Instance().OfferteDatum != null && Acumulator.Instance().OfferteDatum.Year > 2000)
                {

                    Page.FooterInfoVersie.Text = "Prijsrapportage " + versienr;
                    Page.FooterInfoDatum.Text = Acumulator.Instance().OfferteDatum.ToString("dd-MM-yyyy");
                    Page.FooterInfoTijd.Text = Acumulator.Instance().OfferteDatum.ToString("HH:mm:ss");
                }
                else
                {
                    Page.FooterInfoVersie.Text = "Prijsrapportage" + versienr;
                    Page.FooterInfoDatum.Text = printDatum.ToString("dd-MM-yyyy"); ;
                    Page.FooterInfoTijd.Text = printDatum.ToString("HH:mm:ss");
                }
            }
            /////////////////////////////
            //klant info, tel, email, etc
            Page.FooterNaam.Text += completeBouwnummer.KlantNaam;
            //pagina nummering toevoegen!!
       //     Page.Footerpaginanr.Text = "Pagina " + page_nr + " van " + totalPages;
           
        }
      
		private void GenerateReport(bool saveneeded,DateTime printDatum)
		{
			//Header
			var ctx = Acumulator.Instance().ctx;
			
		//	TBKlantnaam.Text += Acumulator.Instance().GebruikersNaam;
		//	TBDatum.Text += DateTime.Now.ToShortDateString(); 
            var instapChecklist = new Dictionary<String,int>();
            HeleDozenLijst.Clear();
            InitNewPage(saveneeded,printDatum);
            ///
			int h_cntr = 215;
			var TBoptie = new TextBlock();
			TBoptie.FontFamily = new FontFamily("Lucida Grande");
			TBoptie.FontSize = 12;
			var TBprijs = new TextBlock();
			TBprijs.FontFamily = new FontFamily("Lucida Grande");
			TBprijs.FontSize = 12;
            TBprijs.TextAlignment = TextAlignment.Right;
			LayoutRoot.Children.Add(TBoptie);
			Canvas.SetLeft(TBoptie, 50);
			Canvas.SetTop(TBoptie, h_cntr);
			LayoutRoot.Children.Add(TBprijs);
			Canvas.SetLeft(TBprijs, 690);
			Canvas.SetTop(TBprijs, h_cntr);
			if (Acumulator.Instance().OTracker != null && Acumulator.Instance().OTracker.offerteRuimte_ != null)
			{

                foreach (Ruimte r in Acumulator.Instance().OTracker.offerteRuimte_.Children)
                {

                    foreach (var p in r.GekozenTegels)
                    {
                        if (instapChecklist.ContainsKey(p.ID))
                        {
                            instapChecklist[p.ID]+=1;
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
				
				//tmpBon.AddArticle(bouwnr, 0, false);
				TBoptie.Text += Environment.NewLine;
				TBprijs.Text += Environment.NewLine;
				h_cntr += 16;
				double totaalprijs = 0;
				foreach (Ruimte r in Acumulator.Instance().OTracker.offerteRuimte_.Children)
				{
                    //tmpBon.AddArticle(r.Omschrijving, 0, true);
					TBoptie.Text +=Acumulator.Instance().MaakStartZin( r.Omschrijving) + Environment.NewLine;
					TBprijs.Text += Environment.NewLine;
					h_cntr += 16;
					if (r.GekozenPakket != null)
					{
						//tmpBon.AddArticle(r.GekozenPakket.Omschrijving, r.GekozenPakket.PrijsHuidig, true);
                        TBoptie.Text += Acumulator.Instance().MaakStartZin("Pakket: " + r.GekozenPakket.Omschrijving) + Environment.NewLine;
						TBprijs.Text += "€" + String.Format("{0:0.00}", r.GekozenPakket.PrijsHuidig) + Environment.NewLine;
						totaalprijs += r.GekozenPakket.PrijsHuidig;
						h_cntr += 16;
						
					}
					foreach (OptieKeuze op in r.GekozenOpties)
					{
						op.laad_verder();
                        var dorpelcat = (from dc in Acumulator.Instance().ctx.SubCatPerRuimteDeels
                                         where dc.R_NR == op.OptieID && dc.SCB_NR == "SCB13"
                                         select dc.R_NR).FirstOrDefault();
                       
						//tmpBon.AddArticle(op.Omschrijving, op.BasisPrijs, false);
                        TBoptie.Text += Acumulator.Instance().MaakStartZin(op.Omschrijving) + Environment.NewLine;


                        var tmptotPrijs = 0.0;
                        if (r.GekozenPakket != null)
                        {
                            TBprijs.Text += "€" + String.Format("{0:0.00}", op.getMeerprijs(Acumulator.Instance().ctx, r.GekozenPakket.PakketPrijsgroep_NR)) + Environment.NewLine;
                           // totaalprijs += op.getMeerprijs(Acumulator.Instance().ctx, r.GekozenPakket.PakketPrijsgroep_NR);
                            tmptotPrijs += op.getMeerprijs(Acumulator.Instance().ctx, r.GekozenPakket.PakketPrijsgroep_NR);
                        
                        }
                        else
                        {
                            //TBprijs.Text += "€" + String.Format("{0:0.00}",0) + Environment.NewLine;
                            var basispakket = (from rpt in ctx.RuimtesPerTypes
                                               where rpt.R_NR == Acumulator.Instance().BB.HuidigRuimte.RuimteID
                                               select rpt.BasisPakket_NR).FirstOrDefault();
                            var optiebasisprijs = 0.0;
                            if (basispakket != null)
                            {
                                var basispakprijs = (from pr in ctx.MeerPrijzenRuimteOpties
                                                     where pr.R_NR == op.OptieID && pr.PP_NR == (from p in ctx.Pakkettens where p.P_ID == basispakket select p.PP_NR).FirstOrDefault()
                                                     select pr.meerprijs).FirstOrDefault();
                                if (basispakprijs != null)
                                {
                                    optiebasisprijs = Acumulator.Instance().BerekenEindPrijs((double)basispakprijs);
                                }
                            }
                            if (optiebasisprijs == 0)
                            {
                                optiebasisprijs = op.BasisPrijs;
                            }
                            tmptotPrijs = optiebasisprijs;
                            TBprijs.Text += "€" + String.Format("{0:0.00}",optiebasisprijs) + Environment.NewLine;
                            
                        }
                        totaalprijs += tmptotPrijs;
						h_cntr += 16;
                        if (dorpelcat != null && r.GekozenTegels.Count > 0)
                        {
                            if (!Acumulator.Instance().ProjFase.FilterDorpels)
                            {
                                var oms = r.GekozenTegels.Where(x => x.Omschrijving.Contains('€')).FirstOrDefault();
                                if (oms != null)
                                {
                                    var parts = oms.Omschrijving.Split('€');
                                    if (parts[1] != "0,00")
                                    {
                                        // tmpBon.AddArticle("Meerprijs Dorpel", double.Parse(parts[1]), false);
                                        TBoptie.Text += "Meerprijs Dorpel: " + Environment.NewLine;
                                        TBprijs.Text += "€" + String.Format("{0:0.00}", double.Parse(parts[1])) + Environment.NewLine;
                                        totaalprijs += double.Parse(parts[1]);
                                        h_cntr += 16;
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
                                          + " (" + exmtrs.Meters + eenheidsText + exmtrs.Meerprijs.ToString() + ")";/* +" + €" +
                                             String.Format("{0:0.00}", 
                                             (
                                                exmtrs.LinkedProduct != null ?
                                                   instapPrijsCalc(exmtrs.LinkedProduct.ID,exmtrs.LinkedProduct.Omschrijving,instapChecklist, exmtrs.LinkedProduct.VerpakkingsToeslag) :
                                                   exmtrs.LinkedHoekProf != null ?
                                                   instapPrijsCalc(exmtrs.LinkedHoekProf.ProfielID,exmtrs.LinkedHoekProf.Omschrijving,instapChecklist,exmtrs.LinkedHoekProf.VerpakkingsToeslag) : 0.0
                                             )
                                          ) 
                                          + " instapprijs";*/
                            


                            var accPrijs = exmtrs.Meters * exmtrs.Meerprijs;
                            if(exmtrs.LinkedProduct != null)
                            {
                                instapPrijsCalc(exmtrs.LinkedProduct.ID, exmtrs.LinkedProduct.Omschrijving, instapChecklist, exmtrs.LinkedProduct.VerpakkingsToeslag) ;
                            }
                            else if (exmtrs.LinkedHoekProf != null)
                            {
                                instapPrijsCalc(exmtrs.LinkedHoekProf.ProfielID, exmtrs.LinkedHoekProf.Omschrijving, instapChecklist, exmtrs.LinkedHoekProf.VerpakkingsToeslag);
                            }
                            totaalprijs += accPrijs;
                            //  tmpBon.AddArticle(accText, accPrijs, false);
                            TBoptie.Text += accText + Environment.NewLine;
                            TBprijs.Text += "€" + String.Format("{0:0.00}", accPrijs) + Environment.NewLine;
                        }
                    }
					TBoptie.Text += Environment.NewLine;
					TBprijs.Text += Environment.NewLine;
					h_cntr += 16;
                    h_cntr += 16;
                }
               //afname van hele dozen
                TBoptie.Text += "Afname Hele Dozen" + Environment.NewLine;
                TBprijs.Text += Environment.NewLine;
                h_cntr += 16;
                double dozenprijs = 0.0;
                foreach (var hd in HeleDozenLijst)
                {
                    TBoptie.Text += hd.Key +Environment.NewLine;
                    TBprijs.Text += "€" + String.Format("{0:0.00}", hd.Value) + Environment.NewLine;
                    dozenprijs += hd.Value;
                    h_cntr += 16;
                }
                TBoptie.Text += Environment.NewLine;
                TBprijs.Text += Environment.NewLine;
                h_cntr += 16;
                TBoptie.Text += "subtotaal afname hele dozen " +Environment.NewLine;
                TBprijs.Text += "€" + String.Format("{0:0.00}", dozenprijs) + Environment.NewLine;
                h_cntr += 16;
                totaalprijs += dozenprijs;
                //

                TBoptie.Text += Environment.NewLine;
                TBprijs.Text += Environment.NewLine;
                h_cntr += 16;
                System.Windows.Shapes.Rectangle rect2 = new System.Windows.Shapes.Rectangle();
                rect2.Fill = new SolidColorBrush(Colors.Black);
                rect2.Width = 715;
                rect2.Height = 1;
                LayoutRoot.Children.Add(rect2);
                Canvas.SetLeft(rect2, 40);
                Canvas.SetTop(rect2, h_cntr + 80);
                h_cntr += 80;
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
                    TBoptie.Text += Environment.NewLine;
                    TBprijs.Text += Environment.NewLine;
                    h_cntr += 16;
                    TBoptie.Text += "Subtotaal ";
                    TBprijs.Text += "€" + String.Format("{0:0.00}", totaalprijs);
                    TBoptie.Text += Environment.NewLine;
                    TBprijs.Text += Environment.NewLine;
                    h_cntr += 16;
                    TBoptie.Text += "Korting";// " + 100 * korting + "%";
                    TBoptie.Text += Environment.NewLine;

                   var kortt = Math.Round(korting, 2);
                    TBprijs.Text += "- €" + String.Format("{0:0.00}", (kortt));
                    TBprijs.Text += Environment.NewLine;
                    totaalprijs -= kortt;
                    h_cntr += 16;
                }
                TBoptie.Text += Environment.NewLine;
                TBprijs.Text += Environment.NewLine;
                h_cntr += 16;
				//this.Setprijs(tmpBon.GetTotal());
			
				var TBtotaal = new TextBlock();
				TBtotaal.FontFamily = new FontFamily("Lucida Grande");
				TBtotaal.FontSize = 14;
                TBtotaal.FontWeight = FontWeights.Bold;
                TBtotaal.Text = "Totaal: incl. BTW";

                LayoutRoot.Children.Add(TBtotaal);
                Canvas.SetLeft(TBtotaal, 50);
                Canvas.SetTop(TBtotaal, h_cntr + 20); 


                var TBtotaalPrijs = new TextBlock();
                TBtotaalPrijs.FontFamily = new FontFamily("Lucida Grande");
                TBtotaalPrijs.FontSize = 12;
                TBtotaalPrijs.TextAlignment = TextAlignment.Right;
                TBtotaalPrijs.FontWeight = FontWeights.Bold;
                TBtotaalPrijs.Text = "€" + String.Format("{0:0.00}", totaalprijs);
                LayoutRoot.Children.Add(TBtotaalPrijs);
                Canvas.SetLeft(TBtotaalPrijs,690);
                Canvas.SetTop(TBtotaalPrijs, h_cntr + 20);
				

			}

		}
        private Dictionary<string, double> HeleDozenLijst = new Dictionary<string, double>();
        private double instapPrijsCalc(string id,string oms,Dictionary<string,int> checklist,double prijs)
        {
            var retval = 0.0;
            if(checklist.ContainsKey(id) == true)
            {
                checklist[id] += 1;
            }
            else
            {
                retval = prijs;
                checklist.Add(id, 1);
                HeleDozenLijst.Add(oms, prijs);
            }
            return retval;
        }
		public ExtendedImage GetImg(bool saveneeded, DateTime printDatum)
		{
			GenerateReport(saveneeded,printDatum);
            var wb = new WriteableBitmap(LayoutRoot, new ScaleTransform() { ScaleX = 3, ScaleY = 3 });
            ExtendedImage myImage = wb.ToImage();
			exImg = myImage;
			return exImg;
		}
	}
}

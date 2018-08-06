using System;
using System.Windows.Controls;
using System.Windows.Media;
using ImageTools;
using System.Collections.Generic;
using eBrochure_zeebregts.Controls;
using System.Linq;
using eBrochure_zeebregts.Classes;
using System.Windows;
using System.Windows.Media.Imaging;
using eBrochure_zeebregts.Web;
using System.Net;
using System.IO.IsolatedStorage;
using System.IO;
using System.Collections.ObjectModel;
using eBrochure_zeebregts.UitvoerClasses;
using eBrochure_zeebregts.ExpertControls.Models;
using eBrochure_zeebregts.ExpertControls;


namespace eBrochure_zeebregts.Helpers
{
	public class UitvoerCreator
	{
        private int page_nr = 0;
        public int totalPages = 0;
        public int pagebreaks;
		public List<ExtendedImage> MaakTegelUitvoer(bool saveneeded,DateTime printDatum)
		{
            if (saveneeded)
            {
                // Acumulator.Instance().oOL.Versie += 1;
                // Acumulator.Instance().OfferteDatum = (DateTime.Now + Acumulator.Instance().serverTimeDiff);
            }
			var ImgPages = new List<ExtendedImage>();
            int cntr = 0;
            var wblist = new List<WriteableBitmap>();
            foreach (ExpertUitvoer u in MaakCanvasTegelRaport(saveneeded,printDatum))
            {
                cntr++;

                u.LayoutRoot.Measure(new Size(797, 1123));
                u.LayoutRoot.Arrange(new Rect(0.0, 0.0, 797, 1123));
                u.LayoutRoot.UpdateLayout();
                var wb = new WriteableBitmap(2391, 3369);
                wb.Render(u.LayoutRoot, new ScaleTransform { ScaleX = 3, ScaleY = 3 });
                wb.Invalidate();
                wblist.Add(wb);

              
			}
            foreach (var wb in wblist)
            {
                ExtendedImage myImage = wb.ToImage();
                ImgPages.Add(myImage);
            }

            wblist.Clear();
            ImgPages.Clear();
            foreach (ExpertUitvoer u in MaakCanvasTegelRaport(saveneeded, printDatum))
            {
                cntr++;

                u.LayoutRoot.Measure(new Size(797, 1123));
                u.LayoutRoot.Arrange(new Rect(0.0, 0.0, 797, 1123));
                u.LayoutRoot.UpdateLayout();
                var wb = new WriteableBitmap(2391, 3369);
                wb.Render(u.LayoutRoot, new ScaleTransform { ScaleX = 3, ScaleY = 3 });
                wb.Invalidate();
                wblist.Add(wb);


            }
            foreach (var wb in wblist)
            {
                ExtendedImage myImage = wb.ToImage();
                ImgPages.Add(myImage);
            }
            return ImgPages;
		}
      
        public List<ExtendedImage> MaakPlattegrondUitvoer(bool saveneeded,DateTime printDatum)
        {
            var ImgPages = new List<ExtendedImage>();
            int cntr = 0;
            var wblist = new List<WriteableBitmap>();
            var tmpPgnNR = page_nr;
            foreach (ExpertUitvoer u in MaakCanvasPlattegrond(saveneeded, printDatum))
            {
                cntr++;

                u.LayoutRoot.Measure(new Size(797, 1123));
                u.LayoutRoot.Arrange(new Rect(0.0, 0.0, 797, 1123));
                u.LayoutRoot.UpdateLayout();
                var wb = new WriteableBitmap(2391, 3369);
                wb.Render(u.LayoutRoot, new ScaleTransform { ScaleX = 3, ScaleY = 3 });
                wb.Invalidate();
                wblist.Add(wb);


            }
            foreach (var wb in wblist)
            {
                ExtendedImage myImage = wb.ToImage();
                ImgPages.Add(myImage);
            }

            wblist.Clear();
            ImgPages.Clear();

            page_nr = tmpPgnNR;
            foreach (ExpertUitvoer u in MaakCanvasPlattegrond(saveneeded, printDatum))
            {
                cntr++;

                u.LayoutRoot.Measure(new Size(797, 1123));
                u.LayoutRoot.Arrange(new Rect(0.0, 0.0, 797, 1123));
                u.LayoutRoot.UpdateLayout();
                var wb = new WriteableBitmap(2391, 3369);
                wb.Render(u.LayoutRoot, new ScaleTransform { ScaleX = 3, ScaleY = 3 });
                wb.Invalidate();
                wblist.Add(wb);


            }
            foreach (var wb in wblist)
            {
                ExtendedImage myImage = wb.ToImage();
                ImgPages.Add(myImage);
            }
            return ImgPages;
        }
        private List<ExpertUitvoer> MaakCanvasPlattegrond(bool saveneeded,DateTime printDatum)
        {
            var Pages = new List<ExpertUitvoer>();
            var ctx = Acumulator.Instance().ctx;
           
            foreach (Ruimte R in Acumulator.Instance().OTracker.offerteRuimte_.Children.Where(x=>(x as Ruimte).GekozenPakket != null))
            {
                R.SwitchBluePrint();
               byte[] bytes = null;
               //var ruimtesetkey = "";
               //var bpPath = "";
               var waitcntr = 0;
               while (Acumulator.Instance().bluePrintManager.GetBlueprintStatus(R.RuimteID) == BluePrintStatus.Downloading && waitcntr < 150)
               {
                   System.Threading.Thread.Sleep(200);
                   waitcntr++;
               }
             
               if (Acumulator.Instance().bluePrintManager.GetBlueprintStatus(R.RuimteID) == BluePrintStatus.Complete)
               {
                   bytes = Acumulator.Instance().bluePrintManager.getBlueprintData(R.RuimteID, false);
                   if (bytes != null)
                   {
                       var np = new ExpertUitvoer();
                       np.InitNewPage(saveneeded,printDatum);
                       page_nr++;
                       np.SetPaginaNR(totalPages, page_nr);
                       Pages.Add(np);
                       
                       using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
                       {
                           BitmapImage im = new BitmapImage();
                           im.SetSource(ms);
                           var BlueprintImg = new Image();
                           BlueprintImg.Source = im;
                           BlueprintImg.Width = 707;
                           BlueprintImg.Height = 750;
                           //function to add blueprint
                           var roms = R.Omschrijving + (R.spiegel ? "(Gespiegeld)" : "");
                           np.AddBluePrint(BlueprintImg,roms);
                         //  Pages.Last().LayoutRoot.Children.Add(BlueprintImg);
                           
                           //tie into previous function?
                          // Pages.Last().SetTitle("Tekeningen", roms);
                           //Canvas.SetLeft(BlueprintImg, 45);
                           //Canvas.SetTop(BlueprintImg, 218);
                       }
                   }
               }
               else
               {
                   //show no img available
                   var np = new ExpertUitvoer();
                   np.InitNewPage(saveneeded, printDatum);
                   np.SetPaginaNR(totalPages, page_nr);
                   Pages.Add(np);
                   var warningMsg = new TextBlock()
                   {
                       Text = "Plattegrond is tijdenlijk niet beschikbaar.\nDeze is via email opvraagbaar bij zeebregts.",
                       FontFamily = new FontFamily("Lucida Grande"),
                       FontSize = 20,
                       Margin = new Thickness(5),

                   };
                   //Pages.Last().LayoutRoot.Children.Add(warningMsg);
                   var roms = R.Omschrijving + (R.spiegel ? "(Gespiegeld)" : "");
                   //Pages.Last().SetTitle("Tekeningen", roms);
                   //Canvas.SetLeft(warningMsg, 45);
                   //Canvas.SetTop(warningMsg, 218);

               }
              
            }
            return Pages;
        }
        public void MaakPrijsUitvoer()
		{

		}
		private List<String> CalculateTegelPages()
		{
			var PageBreaks = new List<String>();
			var H_Totaal = 0;
            var counter = 0;
			foreach (Ruimte r in Acumulator.Instance().OTracker.offerteRuimte_.Children.Where(x=>(x as Ruimte).GekozenPakket != null))
			{
                counter++;
				var H_ruimte = 135;
				H_ruimte += 60 * r.GekozenTegels.Count;
				H_ruimte += 15 * r.GekozenProfielen.Count;
				var vlakOptieCnt = r.GekozenOpties.Where(x => x.OptieSoort == "OT3").Count();
				if (vlakOptieCnt >= 2)
				{
					vlakOptieCnt+=1;
					H_ruimte += 15 * vlakOptieCnt;
				}
                if (r.GekozenAccenten.Count() > 0)
                {
                    H_ruimte += 30 * r.GekozenAccenten.Count();
                    H_ruimte += 15;
                }
                if (r.AanvullendeMeters.Count() > 0)
                {
                    H_ruimte += 45 * (r.AanvullendeMeters.Count() - r.GekozenTegels.Count - r.GekozenProfielen.Count);
                }
				H_Totaal += H_ruimte;
                
				if (H_Totaal > 590)
				{
					PageBreaks.Add(r.RuimteID);
					H_Totaal = H_ruimte;
				}
			}
            totalPages = PageBreaks.Count + 1+(counter +1) + 1;// +counter;
            pagebreaks = PageBreaks.Count;
            pagebreaks += (Acumulator.Instance().OTracker.offerteRuimte_.Children.Count(x=>(x as Ruimte).GekozenPakket != null) -1);
			return PageBreaks;
		}
        private void SetAdresBox(Uitvoer Page)
        {
            var ctx = Acumulator.Instance().ctx;
            var completeBouwnummer = (from k in ctx.Bouwnummers
                                      where k.B_ID == Acumulator.Instance().Bouwnr
                                      select k).FirstOrDefault();
            int regelcounter = 0; // 1e regel start op canvas hoogte 140
            if (completeBouwnummer.KlantNaam != null && completeBouwnummer.KlantNaam != "")
            {
                regelcounter = 1;
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
        private Uitvoer InitNewPage(bool saveneeded,DateTime printDatum)
		{
            page_nr++;
    
			var Page = new Uitvoer();
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
			Page.HeaderProjInfo.Text += projinf;
			Page.TBBouwnummer.Text += completeBouwnummer.Omschrijving;
            SetAdresBox(Page);
		
            if (Acumulator.Instance().HuidigGebruiker.Rol == UserRole.Adviseur)
			{
				Page.HeaderAdviseur.Text += Acumulator.Instance().HuidigGebruiker.GebruikersNaam;
			}
			else
			{
				Page.HeaderAdviseur.Text = "";
			}
            var versienr = "";
            if (saveneeded)
            {
                if (Acumulator.Instance().oOL != null)
                { versienr = "versie: " + (Acumulator.Instance().oOL.VersieFull +1).ToString()+".0"; }
                else
                {
                    versienr = "versie: 1.0";
                }
                //
               Page.FooterInfoVersie.Text = "Offerte " + versienr;
               Page.FooterInfoDatum.Text = printDatum.ToString("dd-MM-yyyy"); ;
               Page.FooterInfoTijd.Text = printDatum.ToString("HH:mm:ss");
            
            }
            else
            {
                if (Acumulator.Instance().oOL != null) { versienr = "versie: " + Acumulator.Instance().oOL.VersieFull.ToString()+".0"; } else { versienr = "versie: 1.0"; }
                if (Acumulator.Instance().OfferteDatum != null && Acumulator.Instance().OfferteDatum.Year > 2000)
                {

                    Page.FooterInfoVersie.Text = "Offerte " + versienr;
                    Page.FooterInfoDatum.Text = Acumulator.Instance().OfferteDatum.ToString("dd-MM-yyyy");
                    Page.FooterInfoTijd.Text = Acumulator.Instance().OfferteDatum.ToString("HH:mm:ss");
                }
                else
                {
                    Page.FooterInfoVersie.Text = "Offerte" + versienr;
                    Page.FooterInfoDatum.Text = printDatum.ToString("dd-MM-yyyy"); ;
                    Page.FooterInfoTijd.Text = printDatum.ToString("HH:mm:ss");
                }
            }

			//klant info, tel, email, etc
            Page.FooterNaam.Text += completeBouwnummer.KlantNaam;
			//pagina nummering toevoegen!!
            Page.Footerpaginanr.Text = "Pagina " + page_nr + " van " + totalPages;
			return Page;
		}
		private void CalculatePrijsPages()
		{

		}

        public List<ExpertUitvoer> MaakCanvasTegelRaport(bool saveneeded, DateTime printDatum)
        {
            page_nr = -1;
            Acumulator.Instance().PrevPrintDatum = printDatum;
            var ctx = Acumulator.Instance().ctx;
            var PageBreaks = CalculateTegelPages();
            var Pages = new List<ExpertUitvoer>();
            //Ruimtes
            int ruimteCntr = 0;
          

            foreach (Ruimte r in Acumulator.Instance().OTracker.offerteRuimte_.Children)
            {
                ruimteCntr++;
                ExpertUitvoer np;
                 var newpage = new ExpertUitvoer();
                 newpage.InitNewPage(saveneeded, printDatum);
                    Pages.Add(newpage);
                    page_nr++;
                    newpage.SetPaginaNR(totalPages, page_nr);
                
                
               if(newpage != null)
               {
                   if (newpage.InitNewRuimte(ruimteCntr, r))
                   {
                       var custsrt = new UitvoerComparer<IOfferte>();
                       r.GekozenTegels.Sort(custsrt);

                       if (!newpage.AddPaginaBlok(MakeTegelBlok(r, "HC1")))
                       {
                           newpage = new ExpertUitvoer();
                           newpage.InitNewPage(saveneeded, printDatum);
                           Pages.Add(newpage);
                           page_nr++;
                           newpage.SetPaginaNR(totalPages, page_nr);
                       }
                       if (!newpage.AddPaginaBlok(MakeAccentBlok(r, "HC1")))
                       {
                           newpage = new ExpertUitvoer();
                           newpage.InitNewPage(saveneeded, printDatum);
                           Pages.Add(newpage);
                           page_nr++;
                           newpage.SetPaginaNR(totalPages, page_nr);
                       }
                       if (!newpage.AddPaginaBlok(MakeHoekProfielBlok(r)))
                       {
                           newpage = new ExpertUitvoer();
                           newpage.InitNewPage(saveneeded, printDatum);
                           Pages.Add(newpage);
                           page_nr++;
                           newpage.SetPaginaNR(totalPages, page_nr);
                       }
                       if (!newpage.AddPaginaBlok(MakeTegelBlok(r, "HC2")))
                       {
                           newpage = new ExpertUitvoer();
                           newpage.InitNewPage(saveneeded, printDatum);
                           Pages.Add(newpage);
                           page_nr++;
                           newpage.SetPaginaNR(totalPages, page_nr);
                       }
                       if (!newpage.AddPaginaBlok(MakeAccentBlok(r, "HC2")))
                       {
                           newpage = new ExpertUitvoer();
                           newpage.InitNewPage(saveneeded, printDatum);
                           Pages.Add(newpage);
                           page_nr++;
                           newpage.SetPaginaNR(totalPages, page_nr);
                       }
                       if (!newpage.AddPaginaBlok(MakeDorpelBlok(r)))
                       {
                           newpage = new ExpertUitvoer();
                           newpage.InitNewPage(saveneeded, printDatum);
                           Pages.Add(newpage);
                           page_nr++;
                           newpage.SetPaginaNR(totalPages, page_nr);
                       }
                   }
                   else
                   {
                       Pages.Remove(newpage);
                   }
               }
                
                
            }
            return Pages;
        }
       
        #region Page_SubSection_Functions

        private PaginaBlok MakeTegelBlok(Ruimte r, string MainCatNr)
        {
            var pBlok = new PaginaBlok(15);
            var ctx = Acumulator.Instance().ctx;
            var nabew = r.qryNabewerkingen(ctx);
            
            ///////////////////////////////
            foreach (Product P in r.GekozenTegels)
            {
                if (P.LinkedMainCat == MainCatNr)
                {
                    if (P.Lengte <= 0 && P.Breedte <= 0)
                    { continue; }

                    var scb = (from sc in ctx.SubCatPerPakkets
                               where sc.SCBP_ID == P.LinkedSubCat
                               select sc.SCB_NR).FirstOrDefault().ToString();

                    //var bew = r.GekozenBewerkingen.Where(b => b.LinkedSubcatNr == scb).ToList();
                    var bew = r.GekozenBewerkingen.Where(b =>b.Kenmerk != null && b.Kenmerk.StartsWith(P.ID + scb)).ToList();
                    //var prodtoon = false;
                    if (bew.Count > 0)
                    {
                        //prodtoon = true;
                        var scpp = nabew.Where(n => n.ID == P.LinkedSubCat).FirstOrDefault() as Classes.SubCatPerPakket;
                        if (scpp.TotaalMeters > 0)
                        {
                            if (scpp == null){continue;}

                            var oms = (from c in ctx.Categorieëns
                                   join sbct in ctx.SubCats on c.C_ID equals sbct.C_NR
                                   where sbct.SCB_ID == scpp.SubCatNR
                                   select c.Omschrijving).FirstOrDefault();
                            pBlok.AddRegel(Environment.NewLine, Environment.NewLine,false);
                            pBlok.AddRegel(null, oms, true);
                            var meters = scpp.TotaalMeters;
                            var eenheid = (from e in ctx.SubCats
                                           where e.SCB_ID == scpp.SubCatNR
                                           select e.eenheidMETERS).FirstOrDefault();

                            pBlok.AddRegel(Math.Round(meters, 2) + eenheid, P.productcode.TrimStart(' ') + " " + P.Kleur + " " + P.Breedte.ToString().Substring(0, 2) + "x" + P.Lengte.ToString().Substring(0, 2) + "cm", false);

                            string lijm = "", verwerking = "", plaatsing = "";
                            foreach (var b in bew)
                            {
                                switch (b.BewerkingCat_NR)
                                {
                                    case "NC1":
                                        lijm = b.TextVoorZin;
                                        break;
                                    case "NC2":
                                        verwerking = b.Omschrijving;
                                        break;
                                    case "NC3":
                                        plaatsing = b.TextVoorZin;
                                        break;
                                }
                            }
                            string zin = "";
                            if (lijm == "" && verwerking == "")
                            {
                                zin = plaatsing;
                            }
                            else
                            {
                                zin = verwerking + " verwerken met " + lijm + " voeg";
                            }
                            if (!String.IsNullOrEmpty(zin))
                            {
                                pBlok.AddRegel(null, zin, false);
                            }
                           // pBlok.AddRegel(null, null, false);
                        }
                        else
                        {
                            var vervangTegel = r.AanvullendeMeters.FirstOrDefault(am => am.LinkedSubCat == scb);
                            if (vervangTegel != null)
                            {
                                var lP = vervangTegel.LinkedProduct;
                                var oms = (from c in ctx.Categorieëns
                                           join sbct in ctx.SubCats on c.C_ID equals sbct.C_NR
                                           where sbct.SCB_ID == scb
                                           select c.Omschrijving).FirstOrDefault();

                                pBlok.AddRegel(null, oms,true);
                                var meters = vervangTegel.Meters;
                                var eenheid = (from e in ctx.SubCats
                                               where e.SCB_ID == scpp.SubCatNR
                                               select e.eenheidMETERS).FirstOrDefault();

                                pBlok.AddRegel(Math.Round(meters, 2) + eenheid, lP.productcode.TrimStart(' ') + " " + lP.Kleur + " " + lP.Breedte.ToString().Substring(0, 2) + "x" + lP.Lengte.ToString().Substring(0, 2) + "cm", false);
                                
                                string lijm = "", verwerking = "", plaatsing = "";
                                var bew_a = r.GekozenBewerkingen.Where(x => x.NabewerkingSetNr == vervangTegel.NabewerkingSetNR);
                                foreach (var b in bew_a)
                                {
                                    switch (b.BewerkingCat_NR)
                                    {
                                        case "NC1":
                                            lijm = b.TextVoorZin;
                                            break;
                                        case "NC2":
                                            verwerking = b.Omschrijving;
                                            break;
                                        case "NC3":
                                            plaatsing = b.TextVoorZin;
                                            break;
                                    }

                                }
                                string zin = "";
                                if (lijm == "" && verwerking == "")
                                {
                                    zin = plaatsing;
                                }
                                else
                                {
                                    zin = verwerking + " verwerken met " + lijm + " voeg";
                                }
                                if(!String.IsNullOrEmpty(zin))
                                { 
                                    pBlok.AddRegel(null, zin, false);
                                }
                                if (!String.IsNullOrEmpty(vervangTegel.Opmerking))
                                {
                                    pBlok.AddRegel(null, vervangTegel.Opmerking, false);
                                }

                             //   pBlok.AddRegel(Environment.NewLine, Environment.NewLine, false);

                            }
                        }
                    }
                }
            }
            //////////////////////////////
            return pBlok;
        }

        private PaginaBlok MakeDorpelBlok(Ruimte r)
        {
            var pBlok = new PaginaBlok(15);
            pBlok.LinkedRuimteId = r.RuimteID;

            var nabew = r.qryNabewerkingen(Acumulator.Instance().ctx);
            var hasdorp = false;
            foreach (var pDorp in r.GekozenTegels)
            {
                var cat = (from pd in Acumulator.Instance().ctx.Productens
                           where pd.PD_ID == pDorp.ID
                           select pd.PC_NR).FirstOrDefault();
                if (int.Parse(cat) == 3)
                {
                    hasdorp = true;
                    //pBlok.AddRegel(Environment.NewLine, Environment.NewLine, false);
                    //pBlok.AddRegel(null, "Dorpel", true);
                    //var scppd = nabew.Where(n => n.ID == pDorp.LinkedSubCat).FirstOrDefault() as Classes.SubCatPerPakket;
                    //if (scppd != null)
                    //{
                    //    pBlok.AddRegel(scppd.TotaalMeters + "st", pDorp.productcode.TrimStart(' ') + " " + pDorp.Kleur, false);
                    //    pBlok.AddRegel(null, "Verwerken ter plaatse van de douchevloer", false);
                    //}
                }
            }
            if(hasdorp == true)
            { 
                pBlok.AddRegel(Environment.NewLine, Environment.NewLine, false);
                pBlok.AddRegel(null, "Dorpel", true);
            }
            
            foreach(var exDorp in r.AanvullendeMeters.Where(e=>e.ProdSoort == ProductSoort.Dorpel))
            {
               // pBlok.AddRegel(Environment.NewLine, Environment.NewLine, false);
                pBlok.AddRegel(exDorp.Meters + "st", exDorp.LinkedProduct.productcode.TrimStart(' ') + " " + exDorp.LinkedProduct.Kleur, false);
                if(String.IsNullOrEmpty(exDorp.Opmerking))
                {
                   // pBlok.AddRegel(Environment.NewLine, Environment.NewLine, false);
                    pBlok.AddRegel(null, "Verwerken ter plaatse van de douchevloer", false);

                }
                else
                {
                    //pBlok.AddRegel(Environment.NewLine, Environment.NewLine, false);
                    pBlok.AddRegel(null, exDorp.Opmerking, false);
                }
                pBlok.AddRegel(Environment.NewLine, Environment.NewLine, false);
               }
            return pBlok;

        }

        private PaginaBlok MakeHoekProfielBlok(Ruimte r)
        {
            var pBlok = new PaginaBlok(15);
            pBlok.LinkedRuimteId = r.RuimteID;
            pBlok.AddRegel(Environment.NewLine, Environment.NewLine, false);
            pBlok.AddRegel(null, "Hoekprofiel", true);
            var basisRegelFinished = false;
            List<int> accIndexDone = new List<int>();
            var nabew = r.qryNabewerkingen(Acumulator.Instance().ctx);
            foreach (var hkprof in r.GekozenProfielen)
            {

                var scppd = nabew.Where(n => n.ID == hkprof.LinkedSubCat).FirstOrDefault() as Classes.SubCatPerPakket;
                if (scppd != null)
                {
                    
                    if (!basisRegelFinished)
                    {
                        var accHprof = r.AanvullendeMeters.FirstOrDefault(x => x.ProdSoort == ExpertControls.Models.ProductSoort.HoekProfiel &&
                                                           x.LinkedHoekProf.ProfielID == hkprof.ProfielID &&
                                                           x.RegelIndex == 0);
                        if (accHprof != null) // basis vervangen/aangepast in accent pagina
                        {
                            pBlok.AddRegel(accHprof.Meters + "m", accHprof.LinkedHoekProf.Omschrijving, false);
                            if (!String.IsNullOrEmpty(accHprof.Opmerking))
                            {
                                pBlok.AddRegel(null, accHprof.Opmerking, false);
                            }
                            
                            accIndexDone.Add(accHprof.RegelIndex);
                        }
                        else//basis nog standaard
                        {
                            
                            pBlok.AddRegel(/*hkprof.Meters*/scppd.TotaalMeters + "m", hkprof.Omschrijving, false);
                        }
                        basisRegelFinished = true;
                    }
                    else
                    {
                        var anvHprof = r.AanvullendeMeters.FirstOrDefault(x => x.ProdSoort == ProductSoort.HoekProfiel &&
                                                                              x.LinkedHoekProf.ProfielID == hkprof.ProfielID &&
                                                                              !accIndexDone.Contains(x.RegelIndex));
                        if (anvHprof != null)
                        {
                            pBlok.AddRegel(anvHprof.Meters + "m", anvHprof.LinkedHoekProf.Omschrijving, false);
                            if (!String.IsNullOrEmpty(anvHprof.Opmerking))
                            {
                                pBlok.AddRegel(null, anvHprof.Opmerking, false);
                            }
                            accIndexDone.Add(anvHprof.RegelIndex);
                            pBlok.AddRegel(Environment.NewLine, Environment.NewLine, false);
                        }
                    }
                }
            }
            pBlok.AddRegel(Environment.NewLine, Environment.NewLine, false);
            return pBlok;
        }

        private PaginaBlok MakeAccentBlok(Ruimte r, string MainCatNr)
        {
            var pBlok = new PaginaBlok(15);
            ///////////////
            var ctx = Acumulator.Instance().ctx;
            var nabew = r.qryNabewerkingen(Acumulator.Instance().ctx);
            foreach (var ac in r.GekozenAccenten.Where(x => x.LinkedSubCat.StartsWith("Sub4Accent")))
            {
                if(MainCatNr == ac.LinkedMainCat)
                {

                    if (r.AanvullendeMeters.Count(x => x.LinkedProduct != null && x.LinkedProduct.ID == ac.ID && x.IsBasis == false) > 0)
                    {
                        var scb_ax_s = (from sc in ctx.SubCatPerPakkets
                                        where sc.SCBP_ID == ac.LinkedSubCat
                                        select sc.SCB_NR).FirstOrDefault();//

                        var scb_ax = "";
                        var foo = "";

                        if (scb_ax_s != null)
                        {
                            scb_ax = scb_ax_s.ToString();
                            foo = ac.LinkedSubCat;
                        }
                        else
                        {
                            scb_ax = ac.LinkedSubCat.StartsWith("Sub4Accent") ? ac.LinkedSubCat : "Sub4Accent" + Ruimte.GenerateScbNr4Accent(ac.LinkedSubCat).SCB_ID;
                            foo = scb_ax;

                        }
                        //var bew_ax = r.GekozenBewerkingen.Where(b => b.LinkedSubcatNr == scb_ax).ToList();
                        var bew_ax = r.GekozenBewerkingen.Where(b =>b.Kenmerk != null && b.Kenmerk.StartsWith(ac.ID + NabewerkingHandler.TranslateSubCat(scb_ax))).ToList();
                        if (bew_ax.Count == 0)
                        {
                            bew_ax = r.GekozenBewerkingen.Where(b => b.LinkedSubcatNr == NabewerkingHandler.TranslateSubCat(scb_ax)).ToList();
                        }
                        if (bew_ax.Count > 0)
                        {
                            var scpp_ax = nabew.Where(n => n.ID == foo).FirstOrDefault() as Classes.SubCatPerPakket;
                            var vExM = r.AanvullendeMeters.FirstOrDefault(x => x.LinkedProduct != null && x.LinkedProduct.ID == ac.ID);

                            var vAccTgl = vExM.LinkedProduct;
                            string oms_ax = "";
                            var eenheid_ax = "m²";
                            if (scpp_ax != null)
                            {
                                oms_ax = (from c in ctx.Categorieëns
                                          join sbct in ctx.SubCats on c.C_ID equals sbct.C_NR
                                          where sbct.SCB_ID == scpp_ax.SubCatNR
                                          select c.Omschrijving).FirstOrDefault();

                                eenheid_ax = (from e in ctx.SubCats
                                              where e.SCB_ID == scpp_ax.SubCatNR
                                              select e.eenheidMETERS).FirstOrDefault();
                            }

                            if (oms_ax.ToLower().StartsWith("accent") == false)
                            {
                                pBlok.AddRegel(Environment.NewLine, Environment.NewLine, false);
                                pBlok.AddRegel(null, oms_ax,true);
                            }
                            var meters_ax = vExM.Meters;
                            pBlok.AddRegel(Math.Round(meters_ax, 2) + eenheid_ax, vAccTgl.productcode.TrimStart(' ') + " " + vAccTgl.Kleur + " " + vAccTgl.Breedte.ToString().Substring(0, 2) + "x" + vAccTgl.Lengte.ToString().Substring(0, 2) + "cm", false);
                            string lijm_ax = "", verwerking_ax = "";

                            foreach (var b in bew_ax.Where(x => x.NabewerkingSetNr == vExM.NabewerkingSetNR))
                            {
                                if (lijm_ax == "")
                                    lijm_ax = b.TextVoorZin;
                                else
                                    verwerking_ax = b.Omschrijving;
                            }
                            if(String.IsNullOrEmpty(lijm_ax))
                            {
                                var b = bew_ax.Where(x => x.BewerkingCat_NR == "NC1").FirstOrDefault();
                                if (b != null)
                                {
                                    lijm_ax = b.TextVoorZin;
                                }
                            }
                            if (String.IsNullOrEmpty(verwerking_ax))
                            {
                                var b = bew_ax.Where(x => x.BewerkingCat_NR != "NC1").FirstOrDefault();
                                if (b != null)
                                {
                                    verwerking_ax = b.Omschrijving;
                                }
                            }
                            string zin_ax = verwerking_ax + " verwerken met " + lijm_ax + " voeg";

                            pBlok.AddRegel(null, zin_ax, false);
                            var OpmText = vExM.Opmerking;
                            if (OpmText!=null && OpmText.Count() > 45)
                            {
                                var za = OpmText.Substring(0, OpmText.IndexOf(' ', 45));
                                var zb = OpmText.Substring(za.Length + 1, OpmText.Length - za.Length-1);
                                if (!String.IsNullOrEmpty(za))
                                {
                                    pBlok.AddRegel(null, za, false);
                                }
                                if (!String.IsNullOrEmpty(zb))
                                {
                                    pBlok.AddRegel(null, zb, false);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(vExM.Opmerking))
                                {
                                    pBlok.AddRegel(null, vExM.Opmerking, false);
                                }
                            }
                            
                        }
                    }
                }
            }
            //////////////
            return pBlok;
        }

        #endregion

    }
}

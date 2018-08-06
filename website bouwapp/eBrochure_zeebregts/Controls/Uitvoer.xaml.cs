using System.Windows;
using System.Windows.Controls;

namespace eBrochure_zeebregts.Controls
{
	public partial class Uitvoer : UserControl
	{
		public Uitvoer()
		{
			InitializeComponent();
		}
        public void SetTitle(string title,string roms)
        {
            TbTitle.Text = title;
            HeaderAdviseur.Text = roms;
        }
		//private ExtendedImage exImg;
		public void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			
			
		}
		private void GenerateReport()
		{
			//Header
		/*	var ctx = Acumulator.Instance().ctx;
			var kavelinfo = (from k in ctx.Bouwnummers
							 where k.B_ID == Acumulator.Instance().Bouwnr
							 select k.Omschrijving).FirstOrDefault().ToString();
			TBBouwnummer.Text += kavelinfo;
			TBKlantnaam.Text += Acumulator.Instance().GebruikersNaam;
			TBDatum.Text += DateTime.Now.ToShortDateString(); 
			//Ruimtes
			int ruimteCntr = 0;
			int h_cntr=170;
			foreach (Ruimte r in Acumulator.Instance().OTracker.offerteRuimte_.Children)
			{
				ruimteCntr++;
				h_cntr += 15;
				TextBlock TB1 = new TextBlock();
				TB1.FontFamily = new FontFamily("Lucida Grande");
				TB1.FontSize = 14;
				TB1.Text = ruimteCntr + ") " + r.Omschrijving;
				//Border B1 = new Border();
				//B1.BorderThickness = new Thickness(2);
			//	B1.BorderBrush = new SolidColorBrush(Colors.Black);
				System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
				rect.Fill = new SolidColorBrush(Colors.Black);
				rect.Width = 500;
				rect.Height = 1;
				LayoutRoot.Children.Add(TB1);
				Canvas.SetLeft(TB1, 50);
				Canvas.SetTop(TB1, h_cntr);
				h_cntr += 15;
				LayoutRoot.Children.Add(rect);
				Canvas.SetLeft(rect, 50);
				Canvas.SetTop(rect, h_cntr+10);
				h_cntr += 15;
				var TB2 = new TextBlock();
				TB2.FontFamily = new FontFamily("Lucida Grande");
				TB2.FontSize = 12;
				LayoutRoot.Children.Add(TB2);
				Canvas.SetLeft(TB2,115);
				Canvas.SetTop(TB2,h_cntr);
				var TB3 = new TextBlock();
				TB3.FontFamily = new FontFamily("Lucida Grande");
				TB3.FontSize = 12;
				LayoutRoot.Children.Add(TB3);
				Canvas.SetLeft(TB3, 50);
				Canvas.SetTop(TB3,h_cntr);
				
				var nabew = r.qryNabewerkingen(ctx);

				var accentDone = new List<string>();
				var tegelh = (from h in r.Children
							  where h.GetType() == typeof(SubCatPerRuimteDeel)
							  && (h as SubCatPerRuimteDeel).SubCatNR == "SCB4"
							  select (h as SubCatPerRuimteDeel).Meters).FirstOrDefault();
				if (r.PlafondHoogte == tegelh)
				{
					TB2.Text += "Betegelen tot plafond" + Environment.NewLine;
				}
				else
				{
					TB2.Text += "Betegelen tot " + tegelh + "mm" + Environment.NewLine;
				}
				TB3.Text += Environment.NewLine;
				h_cntr += 15;
				TB2.Text += Environment.NewLine;
				TB3.Text += Environment.NewLine;
				h_cntr += 15;
				foreach (Product P in r.GekozenTegels)
				{

					var scb = (from sc in ctx.SubCatPerPakkets
							   where sc.SCBP_ID == P.LinkedSubCat
							   select sc.SCB_NR).FirstOrDefault().ToString();

					var bew = r.GekozenBewerkingen.Where(b => b.LinkedSubcatNr == scb).ToList();
					if (bew.Count > 0)
					{
						var scpp = nabew.Where(n => n.ID == P.LinkedSubCat).FirstOrDefault() as SubCatPerPakket;
						var oms = (from c in ctx.Categorieëns
								   join sbct in ctx.SubCats on c.C_ID equals sbct.C_NR
								   where sbct.SCB_ID == scpp.SubCatNR
								   select c.Omschrijving).FirstOrDefault();

						//TB4.Text =;

						TB2.Text += oms + Environment.NewLine;
						TB3.Text += Environment.NewLine;
						h_cntr += 15;
						var meters = scpp.TotaalMeters;
						var eenheid = (from e in ctx.SubCats
									   where e.SCB_ID == scpp.SubCatNR
									   select e.eenheidMETERS).FirstOrDefault();
						TB3.Text += Math.Round(meters, 2) + eenheid;
						TB2.Text += P.productcode.TrimStart(' ') + " " +P.Kleur+" "+ P.Breedte.ToString().Substring(0, 2) + "x" + P.Lengte.ToString().Substring(0, 2) + "cm" +Environment.NewLine;
						TB3.Text += Environment.NewLine;
						h_cntr += 15;
						string lijm = "", verwerking = "";
						foreach (var b in bew)
						{
							if (lijm == "")
								lijm = b.TextVoorZin;
							else
								verwerking = b.Omschrijving;
						}
						string zin = verwerking + " verlijmen met " + lijm + " voeg";
						TB2.Text += zin;
						//TB3.Text += Environment.NewLine;
						//h_cntr += 15;


						// hier hoekprofiel basis
						var main_c = (from sc in ctx.SubCats
									 join scp in ctx.SubCatPerPakkets on sc.SCB_ID equals scp.SCB_NR
									 where scp.SCBP_ID == P.LinkedSubCat
									 select sc.C_NR).FirstOrDefault();
						bool welhoek = false;
						foreach(HoekProfiel hkprof in r.GekozenProfielen)
						{
							var hoek_c = (from sc in ctx.SubCats
										 join scp in ctx.SubCatPerPakkets on sc.SCB_ID equals scp.SCB_NR
										 where scp.SCBP_ID == hkprof.LinkedSubCat
										 select sc.C_NR).FirstOrDefault();
							if(main_c == hoek_c)
							{
								welhoek = true;
								TB2.Text += " en hoekprofiel:" + Environment.NewLine;
								TB3.Text += Environment.NewLine;
								h_cntr += 15;
								TB3.Text += hkprof.Meters + "m" + Environment.NewLine;
								TB2.Text += hkprof.Omschrijving + Environment.NewLine;
								h_cntr += 15;
								TB2.Text += Environment.NewLine;
								TB3.Text += Environment.NewLine;
								h_cntr += 15;
							}
						}
						if (!welhoek)
						{
							TB3.Text += Environment.NewLine + Environment.NewLine;
							TB2.Text += Environment.NewLine + Environment.NewLine;
							h_cntr += 30;
						}
						//
						var ac = (from a in r.GekozenAccenten
								  where a.LinkedMainCat == P.LinkedMainCat
								  select a).FirstOrDefault();
						if (ac != null)
						{
							var scb_a = (from sc in ctx.SubCatPerPakkets
										 where sc.SCBP_ID == ac.LinkedSubCat
										 select sc.SCB_NR).FirstOrDefault().ToString();
							var bew_a = r.GekozenBewerkingen.Where(b => b.LinkedSubcatNr == scb_a).ToList();
							if (bew_a.Count > 0)
							{
								var scpp_a = nabew.Where(n => n.ID == ac.LinkedSubCat).FirstOrDefault() as SubCatPerPakket;
								var oms_a = (from c in ctx.Categorieëns
											 join sbct in ctx.SubCats on c.C_ID equals sbct.C_NR
											 where sbct.SCB_ID == scpp_a.SubCatNR
											 select c.Omschrijving).FirstOrDefault();
								
								
								TB2.Text += oms_a + Environment.NewLine;
								TB3.Text += Environment.NewLine;
								h_cntr += 15;
									var meters_a = scpp_a.TotaalMeters;
								var eenheid_a = (from e in ctx.SubCats
											   where e.SCB_ID == scpp_a.SubCatNR
											   select e.eenheidMETERS).FirstOrDefault();
								TB2.Text += ac.productcode.TrimStart(' ') + " " +ac.Kleur + " "+ ac.Breedte.ToString().Substring(0, 2) + "x" + ac.Lengte.ToString().Substring(0, 2) + "cm" +Environment.NewLine;
								TB3.Text += Math.Round(meters_a, 2) + eenheid_a + Environment.NewLine;
								h_cntr += 15;
								string lijm_a = "", verwerking_a = "";
								foreach (var b in bew_a)
								{
									if (lijm_a == "")
										lijm_a = b.TextVoorZin;
									else
										verwerking_a = b.Omschrijving;
								}
								string zin_a = verwerking_a + " verlijmen met " + lijm_a + " voeg";
								TB2.Text += zin_a;
								//TB3.Text += Environment.NewLine;
								//h_cntr += 15;
								//hier hoekprofiel accent 
								var ac_main_c = (from sc in ctx.SubCats
											  join scp in ctx.SubCatPerPakkets on sc.SCB_ID equals scp.SCB_NR
											  where scp.SCBP_ID == ac.LinkedSubCat
											  select sc.C_NR).FirstOrDefault();
								bool welhoek_a = false;
								foreach (HoekProfiel hkprof in r.GekozenProfielen)
								{
									var ac_hoek_c = (from sc in ctx.SubCats
												  join scp in ctx.SubCatPerPakkets on sc.SCB_ID equals scp.SCB_NR
												  where scp.SCBP_ID == hkprof.LinkedSubCat
												  select sc.C_NR).FirstOrDefault();
									if (ac_main_c == ac_hoek_c)
									{
										welhoek_a = true;
										TB2.Text += " en hoekprofiel:" + Environment.NewLine;
										TB3.Text += Environment.NewLine;
										h_cntr += 15;
										TB3.Text += hkprof.Meters + "m" + Environment.NewLine;
										TB2.Text += hkprof.Omschrijving + Environment.NewLine;
										h_cntr += 15;
										TB2.Text += Environment.NewLine;
										TB3.Text += Environment.NewLine;
										h_cntr += 15;
									}
								}
								if (!welhoek_a)
								{
									TB3.Text += Environment.NewLine + Environment.NewLine;
									TB2.Text += Environment.NewLine + Environment.NewLine;
									h_cntr += 15;
								}
								//

								var wanden = (from w in r.GekozenOpties
											 where w.OptieSoort == "OT3" && w.OptType == OptieType.Resulting
											 select w).ToList();
								if (wanden.Count > 0)
								{
									TB3.Text += Environment.NewLine;
									TB2.Text += "De Accenttegels worden toegepast op de volgende wanden:" + Environment.NewLine;
									h_cntr += 15;
									foreach (OptieKeuze w in wanden)
									{
										
										TB3.Text += Environment.NewLine;
										TB2.Text += w.Omschrijving + Environment.NewLine;
										h_cntr += 15;
									}
									TB3.Text += Environment.NewLine;
									TB2.Text += Environment.NewLine;
									h_cntr += 15;
								}
								accentDone.Add(ac.ID);
							

							}
						}
					}
				}
				foreach (var ac in r.GekozenAccenten)
				{
					if (!accentDone.Contains(ac.ID))
					{
						var scb_ax = (from sc in ctx.SubCatPerPakkets
									  where sc.SCBP_ID == ac.LinkedSubCat
									  select sc.SCB_NR).FirstOrDefault().ToString();
						var bew_ax = r.GekozenBewerkingen.Where(b => b.LinkedSubcatNr == scb_ax).ToList();
						if (bew_ax.Count > 0)
						{
							var scpp_ax = nabew.Where(n => n.ID == ac.LinkedSubCat).FirstOrDefault() as SubCatPerPakket;
							var oms_ax = (from c in ctx.Categorieëns
										  join sbct in ctx.SubCats on c.C_ID equals sbct.C_NR
										  where sbct.SCB_ID == scpp_ax.SubCatNR
										  select c.Omschrijving).FirstOrDefault();


							TB2.Text += oms_ax + Environment.NewLine;
							TB3.Text += Environment.NewLine;
							h_cntr += 15;
							var meters_ax = scpp_ax.TotaalMeters;
							var eenheid_ax = (from e in ctx.SubCats
											  where e.SCB_ID == scpp_ax.SubCatNR
											  select e.eenheidMETERS).FirstOrDefault();
							TB2.Text += ac.productcode.TrimStart(' ')+" "+ ac.Kleur+ " " + ac.Breedte.ToString().Substring(0, 2) + "x" + ac.Lengte.ToString().Substring(0, 2) + "cm" +Environment.NewLine;
							TB3.Text += Math.Round(meters_ax, 2) + eenheid_ax + Environment.NewLine;
							h_cntr += 15;
							string lijm_ax = "", verwerking_ax = "";
							foreach (var b in bew_ax)
							{
								if (lijm_ax == "")
									lijm_ax = b.TextVoorZin;
								else
									verwerking_ax = b.Omschrijving;
							}
							string zin_ax = verwerking_ax + " verlijmen met " + lijm_ax + " voeg";
							TB2.Text += zin_ax;
							var ac_main_c = (from sc in ctx.SubCats
											 join scp in ctx.SubCatPerPakkets on sc.SCB_ID equals scp.SCB_NR
											 where scp.SCBP_ID == ac.LinkedSubCat
											 select sc.C_NR).FirstOrDefault();
							bool welhoek_a = false;
							foreach (HoekProfiel hkprof in r.GekozenProfielen)
							{
								var ac_hoek_c = (from sc in ctx.SubCats
												 join scp in ctx.SubCatPerPakkets on sc.SCB_ID equals scp.SCB_NR
												 where scp.SCBP_ID == hkprof.LinkedSubCat
												 select sc.C_NR).FirstOrDefault();
								if (ac_main_c == ac_hoek_c)
								{
									welhoek_a = true;
									TB2.Text += " en hoekprofiel:" + Environment.NewLine;
									TB3.Text += Environment.NewLine;
									h_cntr += 15;
									TB3.Text += hkprof.Meters + "m" + Environment.NewLine;
									TB2.Text += hkprof.Omschrijving + Environment.NewLine;
									h_cntr += 15;
									TB2.Text += Environment.NewLine;
									TB3.Text += Environment.NewLine;
									h_cntr += 15;
								}
							}
							if (!welhoek_a)
							{
								TB3.Text += Environment.NewLine + Environment.NewLine;
								TB2.Text += Environment.NewLine + Environment.NewLine;
								h_cntr += 15;
							}
							
							TB2.Text += Environment.NewLine;
							TB3.Text += Environment.NewLine;
							h_cntr += 15;

						}
					}
				}
				TB2.Text += Environment.NewLine;
				TB3.Text += Environment.NewLine;
				h_cntr += 15;
				TB2.Text += Environment.NewLine;
				TB3.Text += Environment.NewLine;
				h_cntr += 15;
			}
			LayoutRoot.Height = h_cntr;*/
		}
		private void Image_Loaded(object sender, RoutedEventArgs e)
		{
			
		}
	}
}

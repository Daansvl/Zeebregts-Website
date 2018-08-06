using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using eBrochure_zeebregts.Classes;
using System.Windows.Data;
using System.Globalization;
using eBrochure_zeebregts.Web.Services;
using System.Collections.ObjectModel;
using eBrochure_zeebregts.ExpertControls;
using eBrochure_zeebregts.ExpertControls.UiElements;

namespace eBrochure_zeebregts.KeuzeControls
{
	public enum SubType
	{
		product,
		optie,
		tegel,
		nabewerking,
		info,
		hoekprofiel
	}
	public enum MainCatType
	{
		normaal,
		accent,
		verwerking
	}
	
	public partial class SubCatkControl : UserControl, IBaseControl
	{
		public List<OptieKeuze> LinkedOptie = new List<OptieKeuze>();
		private KeuzeList KList_vlakken;
		private KeuzeList Klist_opties;
		private Samevatlijst samenvatList;
		private IOfferte InputList;
		public MainCatType HoofdCatType
		{  get; private set; }
		public SubType sType;
		private Ruimte _ruimte;
		public SubPage SParent;
        public string hoofdcatnummer;
		public SubCatkControl(string hcnr, MainCatType mct)
		{
			InitializeComponent();
			
			HoofdCatType = mct;
			if (hcnr != null)
			{
				hoofdcatnummer = hcnr;
			}
			if (HoofdCatType == MainCatType.accent)
			{

			}
			else
			{
				AccentJaNee.Visibility = Visibility.Collapsed;
			}
			
		}
		public void InitSubs(IOfferte L, string type, Ruimte R, SubPage sp,string SubcatTitel)
		{
			SParent = sp;
			_ruimte = R;
			InputList = L;
			switch (type)
			{
				case "product":
					MetersTB.Width = 0;
					profielMetersTB.Width = 0;
					sType = SubType.product;
					mkProduct(L,R,SubcatTitel);
					break;
				case "optie":
					sType = SubType.optie;
					//mkOptie(L);
					break;
				case "tegel":
					sType = SubType.tegel;
					//mkTegel(L);
					break;
				case "nabewerking":
					sType = SubType.nabewerking;

					//old style 
                    //mkBewerking(L, R);


                    //new style
                  //  MakeBewerkingMain(L, R);
					break;
				case "info":
					MetersTB.Width = 0;
					profielMetersTB.Width = 0;
					sType = SubType.info;
					SubpTitel.Text = "";
					break;
				case "hoekprofiel":
					MetersTB.Width = 0;
					profielMetersTB.Width = 0;
					sType = SubType.hoekprofiel;
					mkHoekprofiel(L, R, SubcatTitel);
					break;

				
			}
		}
        public void InitNabew(List<NabewerkingUiRegel> regels, Ruimte R, SubPage sp, string SubcatTitel)
        {
            SParent = sp;
            _ruimte = R;
            sType = SubType.nabewerking;
            MetersTB.Width = 0;
            profielMetersTB.Width = 0;
            subinfoTB.Text = "";
            foreach (var r in regels)
            {
                SubCatPanel.Children.Add(new NabewerkingKeuzeControl(r,R));
            }

        }
		public void setTitle(string title)
		{
			SubpTitel.Text = title;
		}
		private void mkHoekprofiel(IOfferte hoekp, Ruimte r, string subcattitle)
		{
			//
			bool disableall = false;
			subinfoTB.Text = "";
			SubCatPanel.Orientation = Orientation.Horizontal;
		//	SubCatPanel.Children.Add(new TextBlock() { Text = subcattitle, FontFamily = new FontFamily("Lucida Grande"), FontSize = 14, Width = 350, TextAlignment = TextAlignment.Left });
			WrapPanel wp = new WrapPanel();
			var gpname = r.RandomGenerator.Next();
			var lasttype = "Recht";
			foreach (HoekProfiel h in hoekp.Children)
			{
				var kleurSetNR = (from kt in Acumulator.Instance().ctx.KleurHoekKleurCombinaties
								  where kt.Kleur_NR == r.GekozenTegels.Where(t => t.LinkedMainCat == hoofdcatnummer).FirstOrDefault().KleurCode
								  select kt.KleurCombinatieSet_NR);
				List<string> AllowedColors = new List<string>();
				foreach (int klsnr in kleurSetNR)
				{
					var goodcol = (from gk in Acumulator.Instance().ctx.KleurCombinatieSetOpbouws
								   where gk.KCS_NR == klsnr
								   select gk.HoekKleur_NR).ToList();
					AllowedColors.AddRange(goodcol);
				}
				if (AllowedColors.Contains(h.KleurCode))
				{
					var tmp = new ProductKControl();
					tmp.InitHoek(h);
					tmp.janeeRdBtn.Checked += new RoutedEventHandler(janeeRdBtn_Checked);
					tmp.janeeRdBtn.GroupName = gpname.ToString();
					if (lasttype != h.ProfielType)
					{
						var tb = new TextBlock() { Text= "", FontFamily = new FontFamily("Lucida Grande"), FontSize = 12, Width = 550, TextAlignment = TextAlignment.Left, Margin = new Thickness(20, 25, 0, 0) };
						wp.Children.Add(tb);
					}
					wp.Children.Add(tmp);
					lasttype = h.ProfielType;
					//herinvoer>>>
					if (r.GekozenProfielen.Where(p => p.ProfielID == h.ProfielID).FirstOrDefault() != null)
					{
						tmp.janeeRdBtn.IsChecked = true;
						disableall = true;
						//wijzigbutton visable
						Acumulator.Instance().BB.ShowWijzigBtn(Acumulator.Instance().BB.IsCurrentComplete());
					}
				}
			}
			bool matchwel = false;
			foreach (HoekProfiel hp in r.GekozenProfielen)
			{
				var mprof = (from h in hoekp.Children
								   where (h as HoekProfiel).ProfielID == hp.ProfielID
								   select h).FirstOrDefault();
				if (mprof != null)
				{
					foreach (ProductKControl pkc in wp.Children.Where(x => x.GetType() == typeof(ProductKControl)))
					{
						if (pkc.hoekprofiel.ProfielID == hp.ProfielID && pkc.janeeRdBtn.IsChecked == true)
						{
							matchwel = true;
						}
					}

				}
				else
				{
					//matchwel = true;
				}
			}
			if (!Acumulator.Instance().BB.IsCurrentComplete() && ! disableall && r.GekozenProfielen.Count > 0  && matchwel)
			{
				foreach (ProductKControl p in wp.Children.Where(x => x.GetType() == typeof(ProductKControl)))
					{
						if (p.hoekprofiel.ProfielType != r.GekozenProfielen.First().ProfielType)
						{
							p.IsEnabled = false;
						}
					}
			}
			wp.Tag = "";
			SubCatPanel.Children.Add(wp);
			if ((disableall ||Acumulator.Instance().BB.IsCurrentComplete() ) &&matchwel )
			{
				foreach (WrapPanel WP in SubCatPanel.Children.Where(x => x.GetType() == typeof(WrapPanel)))
				{
					foreach (ProductKControl p in WP.Children.Where(x => x.GetType() == typeof(ProductKControl)))
					{
						p.IsEnabled = !Acumulator.Instance().BB.IsCurrentComplete();
						SParent.MatchProfiel(this, p.hoekprofiel.ProfielType);
					}
				}
				
			}
		}
		private void mkBewerking(IOfferte nabew, Ruimte r)
		{	//subinfoTB.Text = "";
			//MetersTB.Text = "";
            initializing = Acumulator.Instance().BB.IsCurrentComplete();
			bool basisinfoset = false;
			
            if (r.GekozenTegels != null && subinfoTB.Text.Length < 1)
			{
				if ((nabew as SubCatPerPakket).SubCatNR != "SCB3" &&
                    (nabew as SubCatPerPakket).SubCatNR != "SCB10" &&
                    (nabew as SubCatPerPakket).SubCatNR != "SCB12" &&
                    (nabew as SubCatPerPakket).SubCatNR != "SCB17")
				{
					var prod = (r.GekozenTegels.Where(p => p.LinkedSubCat == (nabew as SubCatPerPakket).ID).FirstOrDefault() as Product);
					if (prod != null)
					{
						var prodinfo = prod.Omschrijving.Split('€')[0];
						subinfoTB.Text = prodinfo;
						basisinfoset = true;
						tegelTTB.Visibility = Visibility.Visible;
					}
				}
				var meters = (nabew as SubCatPerPakket).TotaalMeters;
				var eenheid = (from e in Acumulator.Instance().ctx.SubCats
							   where e.SCB_ID == (nabew as SubCatPerPakket).SubCatNR
							   select e.eenheidMETERS).FirstOrDefault();
				MetersTB.Text = Math.Round(meters, 2) + eenheid;
			}

            var nabewerkingDone = false;
			if (r.GekozenAccenten != null && !basisinfoset && subinfoTB.Text.Length < 1)
			{
                if ((nabew as SubCatPerPakket).SubCatNR != "SCB3" &&
                    (nabew as SubCatPerPakket).SubCatNR != "SCB10" &&
                    (nabew as SubCatPerPakket).SubCatNR != "SCB12" &&
                    (nabew as SubCatPerPakket).SubCatNR != "SCB17")
				{
                    //////////
                    var nbSets = (from n in nabew.Children
                                 where n.GetType() == typeof(Nabewerking)
                                 select (n as Nabewerking).NabewerkingSetNr).Distinct().ToList<string>();
                    ///////////
                
                   var prod = r.GekozenAccenten.Where(p => p.LinkedSubCat == (nabew as SubCatPerPakket).ID).ToList<Product>();
                   tegelTTB.Text = Environment.NewLine;
                   tegelTTB.Visibility = Visibility.Visible;
                   subinfoTB.Text = "";
                   MetersTB.Text = "";
                    if (prod != null && r.AanvullendeMeters.Count > 0)
                    {
                        var sortedProds = new Dictionary<Product, ExtraMeters>();
                        foreach(var p in prod)
                        {
                            switch (r.AanvullendeMeters.FirstOrDefault().ProdSoort)
                            {
                                case ExpertControls.Models.ProductSoort.Tegel:
                                case ExpertControls.Models.ProductSoort.Dorpel:
                                    var val = r.AanvullendeMeters.FirstOrDefault(x =>x.LinkedProduct != null && x.LinkedProduct.ID == p.ID);
                                    if (val != null)
                                    {
                                        sortedProds.Add(p, val);
                                    }
                                    break;
                                case ExpertControls.Models.ProductSoort.HoekProfiel:
                                    var valH = r.AanvullendeMeters.FirstOrDefault(x =>x.LinkedHoekProf != null && x.LinkedHoekProf.ProfielID == p.ID);
                                    if (valH != null)
                                    {
                                        sortedProds.Add(p, valH);
                                    }
                                    break;
                            }
                           
                        }
                        if(sortedProds.Count == prod.Count)
                        {
                        var sProds = (from pair in sortedProds
                                      orderby pair.Value.NabewerkingSetNR ascending
                                      select pair).ToList();

                        for (int i = 0; i < sProds.Count(); i++)
                        {
                            /// make dynamic tb's
                            var stack = new StackPanel
                            {
                                Orientation = Orientation.Horizontal
                            };
                            var mTB = new TextBlock
                            {
                                FontFamily = new FontFamily("Lucida Grande"),
                                FontSize = 12,
                                Width = 90,
                                Margin = new Thickness(-90, 0, 0, 0)
                            };
                            var siTB = new TextBlock
                            {
                                FontFamily = new FontFamily("Lucida Grande"),
                                FontSize = 12
                            };
                            stack.Children.Add(mTB);
                            stack.Children.Add(siTB);

                            SubCatPanel.Children.Add(stack);

                            var prodinfo = sProds[i].Key.Omschrijving;
                            siTB.Text += prodinfo;
                            var exM = sProds[i].Value;//r.AanvullendeMeters.FirstOrDefault(x => x.LinkedProduct.ID == sProds[i].Key.ID);
                            if (exM != null)
                            {
                                var meters = exM.Meters;
                                var eenheid = (from e in Acumulator.Instance().ctx.SubCats
                                               where e.SCB_ID == (nabew as SubCatPerPakket).SubCatNR
                                               select e.eenheidMETERS).FirstOrDefault();
                                mTB.Text += Math.Round(meters, 2) + eenheid;
                            }
                            if (i == sProds.Count() - 1 || exM.NabewerkingSetNR != (sProds[i + 1].Value).NabewerkingSetNR)
                            {
                                //nabewerkingen insert hier
                                maakBewerkingOptieLists(nabew, r, new List<string> { exM.NabewerkingSetNR });
                                nabewerkingDone = true;
                            }
                        }

                        }

                    }
                    
				}
                //var meters = (nabew as SubCatPerPakket).TotaalMeters;
                //var eenheid = (from e in Acumulator.Instance().ctx.SubCats
                //               where e.SCB_ID == (nabew as SubCatPerPakket).SubCatNR
                //               select e.eenheidMETERS).FirstOrDefault();
                //MetersTB.Text = Math.Round(meters, 2) + eenheid;
			}
			if ((nabew as SubCatPerPakket).SubCatNR == "SCB4")
			{
				MetersTB.Text = "";
				var hoogte = (nabew as SubCatPerPakket).TotaalMeters;
				var plafond = (from b in Acumulator.Instance().ctx.Bron_Ruimtes
							   where b.R_NR == r.RuimteID
							   select b.PLAFONDHOOGTE).FirstOrDefault();
				var eenheid = (from e in Acumulator.Instance().ctx.SubCats
							   where e.SCB_ID == (nabew as SubCatPerPakket).SubCatNR
							   select e.eenheidMETERS).FirstOrDefault();
				if (plafond <= hoogte)
				{
					subinfoTB.Text = "Betegelen tot plafond: hoogte " + plafond + eenheid;
				}
				else
				{
					subinfoTB.Text = "Betegelen tot " + hoogte + eenheid;
				}
			}
			if (subinfoTB.Text.Length > 0)
			{
				subinfoTB.Visibility = Visibility.Visible;
			}
			else
			{
				subinfoTB.Visibility = Visibility.Collapsed;
			}
            if (!nabewerkingDone)
            {
                maakBewerkingOptieLists(nabew, r,null);
            }
			if (Acumulator.Instance().BB.IsCurrentComplete())
			{
				AccentJaNee.IsEnabled = false;
			/*	var brush = new SolidColorBrush(Colors.LightGray);
				profielMetersTB.Foreground = brush;
				profielsubinfoTB.Foreground = brush;
				profielTTB.Foreground = brush;
				MetersTB.Foreground = brush;
				subinfoTB.Foreground = brush;
				tegelTTB.Foreground = brush;*/

				foreach (var control in SubCatPanel.Children)
				{
					ProductKControl IP = control as ProductKControl;
					if (IP != null)
					{
						IP.IsEnabled = false;
					}
					else
					{
						WrapPanel WP = control as WrapPanel;
						if (WP != null)
						{
							foreach (ProductKControl PK in WP.Children.Where(x => x.GetType() == typeof(ProductKControl)))
							{
								PK.IsEnabled = false;
							}
							foreach (Control C in WP.Children.Where(x => x.GetType() == typeof(Control)))
							{
							//	C.IsEnabled = false;
							}
						}
						else
						{
							Control c = control as Control;
							if (c != null)
							{
							//	c.IsEnabled = false;
							}
							else
							{
								TextBlock TB = control as TextBlock;
								if (TB != null)
								{
								//	TB.Foreground = new SolidColorBrush(Colors.LightGray);
								}
							}
						}
					}
				}
			}
            initializing = false;
		}
        private void maakBewerkingOptieLists(IOfferte nabew, Ruimte r,List<string> validSetNrs)
        {
            if (nabew.Children.Count > 0 && nabew.Children.First() != null )
            {
                var hoekpL = nabew.Children.Where(h => h.GetType() == typeof(HoekProfiel)).ToList();
                var voegL = nabew.Children.Where(v => v.GetType() == typeof(Nabewerking) &&
                                                (v as Nabewerking).BewerkingCat_NR == "NC1" &&
                                                (validSetNrs != null ? 
                                                validSetNrs.Contains((v as Nabewerking).NabewerkingSetNr) : true)).ToList();
                var verwL = nabew.Children.Where(vw => vw.GetType() == typeof(Nabewerking) &&
                                                (vw as Nabewerking).BewerkingCat_NR == "NC2" &&
                                                (validSetNrs != null ? 
                                                validSetNrs.Contains((vw as Nabewerking).NabewerkingSetNr) : true)).ToList();
                var plaatsL = nabew.Children.Where(p => p.GetType() == typeof(Nabewerking) &&
                                                  (p as Nabewerking).BewerkingCat_NR == "NC3" &&
                                                  (validSetNrs != null ?
                                                  validSetNrs.Contains((p as Nabewerking).NabewerkingSetNr) : true)).ToList();
                if (hoekpL.Count > 0)
                {
                    foreach (HoekProfiel hp in hoekpL)
                    {
                        profielMetersTB.Text = hp.Meters + "m";
                        profielsubinfoTB.Text = hp.Omschrijving;
                        profielsubinfoTB.Visibility = Visibility.Visible;
                        profielTTB.Visibility = Visibility.Visible;
                    }
                }
                if (voegL.Count > 0)
                {
                    var KLV = new KeuzeList(r, "bewerking", "Voegkleur");
                    KLV.MotherPnl.Margin = new Thickness(0);
                    KLV.InitList(voegL, true);
                    KLV.SetLinkedSControl(this);
                    SubCatPanel.Children.Add(KLV);
                    if (r.GekozenBewerkingen.Count > 0)
                    {
                        Acumulator.Instance().BB.ShowWijzigBtn(true);
                        KLV.ReloadKeuzes();
                    }
                }
                if (verwL.Count > 0)
                {
                    var KLVW = new KeuzeList(r, "bewerking", "Verwerking tegel");
                    KLVW.MotherPnl.Margin = new Thickness(0);
                    KLVW.InitList(verwL, true);
                    KLVW.SetLinkedSControl(this);
                    SubCatPanel.Children.Add(KLVW);
                    if (r.GekozenBewerkingen.Count > 0)
                    {
                        Acumulator.Instance().BB.ShowWijzigBtn(true);
                        KLVW.ReloadKeuzes();
                    }
                }
                if (plaatsL.Count > 0)
                {
                    var KLP = new KeuzeList(r, "bewerking", "Plaatsing");
                    KLP.MotherPnl.Margin = new Thickness(0);
                    KLP.InitList(plaatsL, true);
                    KLP.SetLinkedSControl(this);
                    SubCatPanel.Children.Add(KLP);
                    if (r.GekozenBewerkingen.Count > 0)
                    {
                        Acumulator.Instance().BB.ShowWijzigBtn(true);
                        KLP.ReloadKeuzes();
                    }
                }
                SubCatPanel.Children.Add(new TextBlock { Text = "" + Environment.NewLine });
            }
        }
		public bool initializing;
        private string GetSubCatPOptie(eBrochureDomainContext ctx, String subperpak)
        {
            var rs_nrs = (from ro in ctx.RuimteOpbouws
                          where ro.R_NR == _ruimte.RuimteID
                          select ro.RS_NR).Distinct().ToList();
            var R_ids = (from r in ctx.RuimteOpbouws
                         where rs_nrs.Contains(r.RS_NR) /*&& !foo.Contains(r.R_NR)*/
                         select r.R_NR).Distinct().ToList();
            var inscpr = (from s in ctx.SubCatPerRuimteDeels
                          where R_ids.Contains(s.R_NR)
                          select s.R_NR).Distinct().ToList();
            var onafhankelijk = (from soro in ctx.StandaardOnafhandelijkeRuimteOpbouws
                                 join sro in ctx.StandaardRuimteOpbouws on soro.SRO_NR equals sro.SRO_ID
                                 where R_ids.Contains(sro.R_NR)
                                 select sro.R_NR).Distinct().ToList();
            var retval1 = "";
            var retval2 = "";
           //////////////////////////////////////////////
             var   oid =(from opt in onafhankelijk
                            join ot in ctx.RuimteDelens on opt equals ot.R_ID
                            where ot.OT_NR == "OT3"
                            select opt).ToList();
                var dldl = (from rs in ctx.RuimteOpbouws
                            where oid.Contains(rs.R_NR) && rs_nrs.Contains(rs.RS_NR)
                            select new KeyValuePair<string, string>(rs.RS_NR, rs.R_NR)).Distinct().ToList();

                var accentsc_o = (from ac in ctx.SubCatPerRuimteDeels
                                  join sc in ctx.SubCats on ac.SCB_NR equals sc.SCB_ID
                                  join c in ctx.Categorieëns on sc.C_NR equals c.C_ID
                                  where (c.C_ID == "C8" || c.C_ID == "C6" || c.C_ID == "C7") && R_ids.Contains(ac.R_NR) && ac.meters > 0 && ac.SCB_NR != "SCB3" && ac.SCB_NR != "SCB12" && ac.SCB_NR != "SCB10" && ac.SCB_NR != "SCB17" && ac.SCB_NR != "SCB5"
                                  select ac.R_NR).Distinct().ToList();

                List<KeyValuePair<string, string>> OptiesPerSubcat = new List<KeyValuePair<string, string>>();
                foreach (var x in dldl)
                {
                    var omfg = (from ropb in ctx.RuimteOpbouws
                                where ropb.RS_NR == x.Key && accentsc_o.Contains(ropb.R_NR)
                                select ropb.R_NR).FirstOrDefault();
                    var optiesubcat = (from sc in ctx.SubCatPerRuimteDeels
                                       join sca in ctx.SubCats on sc.SCB_NR equals sca.SCB_ID
                                       join c in ctx.Categorieëns on sca.C_NR equals c.C_ID
                                       where (c.C_ID == "C8" || c.C_ID == "C6" || c.C_ID == "C7") && sc.R_NR == omfg && sc.meters > 0 && sc.SCB_NR != "SCB3" && sc.SCB_NR != "SCB12" && sc.SCB_NR != "SCB10" && sc.SCB_NR != "SCB17" && sc.SCB_NR != "SCB5"
                                       select sc.SCB_NR).FirstOrDefault();
                    //x.Value = optie r_nr optiesubcat = subcat bij de optie
                    OptiesPerSubcat.Add(new KeyValuePair<string, string>(optiesubcat, x.Value));

                }

                var subcat = (from scpp in ctx.SubCatPerPakkets
                              where scpp.SCBP_ID == subperpak
                              select scpp.SCB_NR).FirstOrDefault();
                var gekoptiesIDs = (from go in _ruimte.GekozenOpties
                                    select go.OptieID).ToList();

             foreach (var k in OptiesPerSubcat)
                {
                    if(gekoptiesIDs.Contains(k.Value))
                    {
                        retval1 = k.Value;
                    }
                }
             if (retval1 == "" || retval1 == null)
             {
                 retval1 = OptiesPerSubcat.Where(x => x.Key == subcat).FirstOrDefault().Value;
             }
            //////////////////////////////////////////////
             var oid2 = (from x in R_ids
                       where !inscpr.Contains(x)
                       select x).Distinct().ToList();

                var dldl2 = (from rs in ctx.RuimteOpbouws
                            where oid2.Contains(rs.R_NR) && rs_nrs.Contains(rs.RS_NR)
                            select new KeyValuePair<string, string>(rs.RS_NR, rs.R_NR)).Distinct().ToList();

                var accentsc_o2 = (from ac in ctx.SubCatPerRuimteDeels
                                  join sc in ctx.SubCats on ac.SCB_NR equals sc.SCB_ID
                                  join c in ctx.Categorieëns on sc.C_NR equals c.C_ID
                                   where (c.C_ID == "C8" || c.C_ID == "C6" || c.C_ID == "C7") && R_ids.Contains(ac.R_NR) && ac.meters > 0 && ac.SCB_NR != "SCB3" && ac.SCB_NR != "SCB12" && ac.SCB_NR != "SCB10" && ac.SCB_NR != "SCB17" && ac.SCB_NR != "SCB5"
                                  select ac.R_NR).Distinct().ToList();

                List<KeyValuePair<string, string>> OptiesPerSubcat2 = new List<KeyValuePair<string, string>>();
                foreach (var x in dldl2)
                {
                    var omfg2 = (from ropb in ctx.RuimteOpbouws
                                where ropb.RS_NR == x.Key && accentsc_o2.Contains(ropb.R_NR)
                                select ropb.R_NR).FirstOrDefault();
                    var optiesubcat2 = (from sc in ctx.SubCatPerRuimteDeels
                                       join sca in ctx.SubCats on sc.SCB_NR equals sca.SCB_ID
                                       join c in ctx.Categorieëns on sca.C_NR equals c.C_ID
                                        where (c.C_ID == "C8" || c.C_ID == "C6" || c.C_ID == "C7") && sc.R_NR == omfg2 && sc.meters > 0 && sc.SCB_NR != "SCB3" && sc.SCB_NR != "SCB12" && sc.SCB_NR != "SCB10" && sc.SCB_NR != "SCB17" && sc.SCB_NR != "SCB5"
                                       select sc.SCB_NR).FirstOrDefault();
                    //x.Value = optie r_nr optiesubcat = subcat bij de optie
                    OptiesPerSubcat2.Add(new KeyValuePair<string, string>(optiesubcat2, x.Value));

                }

                var subcat2 = (from scpp in ctx.SubCatPerPakkets
                              where scpp.SCBP_ID == subperpak
                              select scpp.SCB_NR).FirstOrDefault();
                retval2 = OptiesPerSubcat2.Where(x => x.Key == subcat2).FirstOrDefault().Value;
            //////////////////////////////////
                if (retval1 != null && retval1 != "")
                    return retval1;
                else
                    return retval2;
        }
        private void ReturnsOptie(List<string> oid)
        {

        }

        private bool filterdorpel(string optieid,string productid)
        {
            var dorpel = (from d in Acumulator.Instance().ctx.DorpelOptieCombos
                          where d.Optie_NR == optieid && d.Product_NR == productid
                          select d.DorpelOptieCombo_ID).FirstOrDefault();
            if (dorpel < 1)
            {
                return true;
            }
            else
            {
                return false;
            }
            
            
        }
        private void mkProduct(IOfferte prods, Ruimte r,string subcattile)
		{
			initializing = Acumulator.Instance().BB.IsCurrentComplete();
			subinfoTB.Text = "";
			SubCatPanel.Orientation = Orientation.Horizontal;
			bool disableall = false;
			SubCatPanel.Children.Add(new TextBlock() { Text = subcattile,FontFamily=new FontFamily("Lucida Grande"), FontSize = 14,Width = 550 ,TextAlignment= TextAlignment.Left, Margin=new Thickness(70,0,0,0)  });
			WrapPanel wp = new WrapPanel();
			
			foreach(Product p in prods.Children)
			{
				var tmp = new ProductKControl();
				tmp.InitProduct(p);
				tmp.janeeRdBtn.Checked += new RoutedEventHandler(janeeRdBtn_Checked);
				wp.Children.Add(tmp);
				if (r.GekozenTegels.Where(t => t.ID == p.ID).FirstOrDefault() != null && HoofdCatType != MainCatType.accent)
				{
					tmp.janeeRdBtn.IsChecked = true;
					disableall = true;
					
				}
				else if (r.GekozenAccenten.Where(t => t.ID == p.ID).FirstOrDefault() != null && HoofdCatType == MainCatType.accent)
				{
					tmp.janeeRdBtn.IsChecked = true;
					disableall = true;
					
				}
				
				
			}
			wp.Tag = "";
			wp.Margin = new Thickness(0, 0, 0, 50);
			SubCatPanel.Children.Add(wp);
            //Dorpel filter temp hier !!!
            Stack<ProductKControl> todeldorpel = new Stack<ProductKControl>();
            foreach (ProductKControl pk in wp.Children.Where(x => x.GetType() == typeof(ProductKControl)))
            {
                

                if (pk.product.LinkedMainCat == "HC6" && Acumulator.Instance().ProjFase.FilterDorpels)
                {
                    var dorpelopties = (from doc in Acumulator.Instance().ctx.DorpelOptieCombos
                                        select doc.Optie_NR).ToList();

                    foreach (OptieKeuze o in r.GekozenOpties.Where(o => (o as OptieKeuze).OptieSoort == "OT1"))
                    {
                        if (dorpelopties.Contains(o.OptieID))
                        {
                            if (filterdorpel(o.OptieID, pk.product.ID))
                            {
                                todeldorpel.Push(pk);
                            }
                        }
                    }
                }
                if(Acumulator.Instance().Projectnr == "PR4" && pk.product.ID != "PD120")//VERY DIRTY!!
                {
                    todeldorpel.Push(pk);
                }
                 if(Acumulator.Instance().Projectnr == "PR5" && pk.product.ID != "PD169")
                 {
                     todeldorpel.Push(pk);
                 }
            }
            if (todeldorpel.Count <= wp.Children.Count - 1)
            {
                while (todeldorpel.Count > 0)
                {
                    var p = todeldorpel.Pop();
                    wp.Children.Remove(p);
                }
            }

			if (HoofdCatType == MainCatType.accent)
			{
				Stack<ProductKControl> todel = new Stack<ProductKControl>();
				foreach (ProductKControl pk in wp.Children.Where(x => x.GetType() == typeof(ProductKControl)))
				{
					var kleur = (from t in r.GekozenTegels
								 select t.Kleur).ToList();
					if (!kleur.Contains(pk.product.Kleur) && pk.product.ID !="0")
					{
						todel.Push(pk);
					}
                  
				}
				if (todel.Count < wp.Children.Count - 1)
				{
					while (todel.Count > 0)
					{
						var p = todel.Pop();
						wp.Children.Remove(p);
					}
				}
				var pro =prods.Children.First() as Product;
				var lsc = pro.LinkedSubCat;
				var pd_set = (from scpp in Acumulator.Instance().ctx.SubCatPerPakkets
							  where scpp.SCBP_ID == lsc
							  select scpp.PD_SET_NR).FirstOrDefault().ToString();
			/*	var optID = (from opc in Acumulator.Instance().ctx.OptieProductCombos
							 where opc.PD_SET_NR == pd_set
							 select opc.R_NR).ToList<string>();
			*/
                var optID = GetSubCatPOptie(Acumulator.Instance().ctx, lsc);
                var optie = (from o in Acumulator.Instance().ctx.RuimteDelens
							 where optID==o.R_ID
							 select new OptieKeuze(o.R_ID, o.Omschrijving) { OptType = OptieType.Determinating, PakketPg = r.GekozenPakket.PakketPrijsgroep_NR, OptieSoort=o.OT_NR }).ToList<OptieKeuze>();
				foreach (OptieKeuze o in optie)
				{
					o.MeerprijsInPakket(Acumulator.Instance().ctx);
				}
				LinkedOptie = optie;
				
			
				OptieTB.Text = "Basis ongewijzigd";// optie.Omschrijving + ": €" + optie.BasisPrijs;
				//AccentJaNee.GroupName = hoofdcatnummer;
				if (disableall == false)
				{
					foreach (ProductKControl pk in wp.Children.Where(x => x.GetType() == typeof(ProductKControl)))
					{
						if (pk.product.ID == "0")
						{
							pk.janeeRdBtn.IsChecked = true;
						}
					}
				}
				string kltitle = "";
				if(LinkedOptie.Count > 1)
				{
					kltitle = "Kies hier uw mogelijkheden.";
				}
				else
				{
					kltitle = "Instap prijs";
				}
				KeuzeList kl1 = new KeuzeList(r, "Opties_accent", kltitle);
				List<IOfferte> optlist=new List<IOfferte>();
				optlist.AddRange(LinkedOptie);
				kl1.InitList(optlist, true);
				kl1.NoMarkup();
				kl1.loadprices(lsc,this,r);
				kl1.Margin = new Thickness(60, 0, 0, 50);
				SubCatPanel.Children.Add(kl1);
				Klist_opties = kl1;
				var gekoptlst = new List<OptieKeuze>();
				var std_ids = (from sro in Acumulator.Instance().ctx.StandaardRuimteOpbouws select sro.R_NR).ToList<string>();
				gekoptlst.AddRange(r.GekozenOpties.Where(o=>std_ids.Contains(o.OptieID) && !LinkedOptie.Select(x=>x.OptieID).ToList().Contains(o.OptieID) && o.OptieSoort != "OT3" ));
				if(LinkedOptie.Count > 0 &&!gekoptlst.Contains(LinkedOptie.First()))
				{
					gekoptlst.Add(LinkedOptie.First());
				}
				var newoptielist = new List<IOfferte>();
				newoptielist.AddRange(r.GetVervolgOpties(Acumulator.Instance().ctx, gekoptlst));
				if (newoptielist.Count > 0)
				{
					var kl = new KeuzeList(r, "Opties_accent", "Kies hier uw vlakken");
					kl.InitList(newoptielist, false);
					kl.NoMarkup();
					kl.loadprices(lsc,this,r);
					//	(kl.ListBox.Items.First() as CustomListItem).Gekozen = !disableall;
					//(kl.ListBox.Items.First() as CustomListItem).Beschikbaar = false;

					KList_vlakken = kl;
					kl.Margin = new Thickness(60, 0, 0, 50);
					SubCatPanel.Children.Add(kl);
				}
				if (samenvatList != null)
				{
					SubCatPanel.Children.Remove(samenvatList);
					samenvatList = null;
				}
				samenvatList = new Samevatlijst();
				UpdateSummaryM();
				
				if (!disableall)
				{
					if (KList_vlakken != null)
					{
						KList_vlakken.Visibility = Visibility.Collapsed;
					}
					if (Klist_opties != null)
					{
						Klist_opties.Visibility = Visibility.Collapsed;
					}
					if (samenvatList != null)
					{
						samenvatList.Visibility = Visibility.Collapsed;
					}
				}
				else
				{
					if (KList_vlakken != null)
					{
						KList_vlakken.ReloadKeuzes();
					}
					if (Klist_opties != null)
					{
						Klist_opties.ReloadKeuzes();
					}
				}
				SParent.AccentGuard(this.hoofdcatnummer, Acumulator.Instance().BB.IsCurrentComplete());
				samenvatList.Margin = new Thickness(0, 0, 0, 50);
				SubCatPanel.Children.Add(samenvatList);



			}
			
		

			if (Acumulator.Instance().BB.IsCurrentComplete())
			{
				Acumulator.Instance().BB.ShowWijzigBtn(true);
				AccentJaNee.IsEnabled = false;
				foreach (var control in SubCatPanel.Children)
				{
					ProductKControl IP = control as ProductKControl;
					if (IP != null)
					{
						IP.IsEnabled = false;
					}
					else
					{
						WrapPanel WP = control as WrapPanel;
						if (WP != null)
						{
							foreach (ProductKControl PK in WP.Children.Where(x => x.GetType() == typeof(ProductKControl)))
							{
								PK.IsEnabled = false;
							}
							foreach (Control C in WP.Children.Where(x => x.GetType() == typeof(Control)))
							{
								//C.IsEnabled = false;
							}
						}
						else
						{
							Control c = control as Control;
							if (c != null)
							{
								//c.IsEnabled = false;
							}
							else
							{
								TextBlock TB = control as TextBlock;
								if (TB != null)
								{
									//TB.Foreground =new SolidColorBrush(Colors.LightGray);
								}
							}
						}
					}
				}

				/*foreach (WrapPanel WP in SubCatPanel.Children.Where(x => x.GetType() == typeof(WrapPanel)))
				{
					foreach (ProductKControl p in WP.Children.Where(x=>x.GetType()==typeof(ProductKControl)))
					{
						p.IsEnabled = false; 
					}
				}*/
			}
			
		}
		public void UpdateSummaryM()
		{
			var tmpprodlst = new List<Product>();
			var tmpdict = new Dictionary<string, Product>();
			//Basis
			var saldi = _ruimte.GetSaldoMetersAccent();
			var p_base = _ruimte.GekozenTegels.Where(x => x.LinkedMainCat == hoofdcatnummer).FirstOrDefault() as Product;
			if (p_base != null)
			{
				var scb_m = (from sc in Acumulator.Instance().ctx.SubCats
							 join scp in Acumulator.Instance().ctx.SubCatPerPakkets on sc.SCB_ID equals scp.SCB_NR
							 where scp.SCBP_ID == p_base.LinkedSubCat
							 select sc.SCB_ID).FirstOrDefault();

				var meters = saldi[scb_m];
				//var meters = (InputList as SubCatPerPakket).TotaalMeters;
				var eenheid = (from e in Acumulator.Instance().ctx.SubCats
							   where e.SCB_ID == scb_m
							   select e.eenheidMETERS).FirstOrDefault();
				if (KList_vlakken != null)
				{
					meters = meters + KList_vlakken.UpdateSumary();
					var mindif = KList_vlakken.ListBox.Items.Count / 2 * 0.01;
					if (meters < mindif)
					{ meters = 0; }
				}
				else
				{
					var scbnr = (from scp in Acumulator.Instance().ctx.SubCatPerPakkets
								 where scp.SCBP_ID == p_base.LinkedSubCat
								 select scp.SCB_NR).FirstOrDefault().ToString();
					meters = meters + (from scr in Acumulator.Instance().ctx.SubCatPerRuimteDeels
									   where scr.SCB_NR == scbnr && scr.R_NR == LinkedOptie.First().OptieID
									   select (double)scr.meters).FirstOrDefault();
				}
				var m_base = Math.Round(meters, 2) + eenheid;
				//Upgrades
				Dictionary<Product, string> p_upgrades = new Dictionary<Product, string>();
				foreach (WrapPanel wp in SubCatPanel.Children.Where(x => x.GetType() == typeof(WrapPanel)))
				{
					foreach (ProductKControl pk in wp.Children.Where(x => x.GetType() == typeof(ProductKControl)))
					{
						if (pk.janeeRdBtn.IsChecked == true)
						{
							var scbnr = (from scp in Acumulator.Instance().ctx.SubCatPerPakkets
										 where scp.SCBP_ID == p_base.LinkedSubCat
										 select scp.SCB_NR).FirstOrDefault().ToString();

							double delta_m = 0;
							if (KList_vlakken != null)
							{
								delta_m = KList_vlakken.UpdateSumary();
							}
							else
							{
								delta_m = (from scr in Acumulator.Instance().ctx.SubCatPerRuimteDeels
										   where scr.SCB_NR == scbnr && scr.R_NR == LinkedOptie.First().OptieID
										   select (double)scr.meters).FirstOrDefault();
							}
							var eenheid_u = (from e in Acumulator.Instance().ctx.SubCats
											 where e.SCB_ID == (InputList as SubCatPerPakket).SubCatNR
											 select e.eenheidMETERS).FirstOrDefault();
							delta_m = Math.Abs(delta_m);
							if (meters == 0)
							{
								delta_m = saldi[scb_m];
							}
							var m_upgrade = Math.Round(delta_m, 2) + eenheid_u;

							p_upgrades.Add(pk.product, m_upgrade);
						}
					}

				}
				samenvatList.Initlist(p_base, m_base, p_upgrades);
			}
			else
			{
				this.Visibility = Visibility.Collapsed;
			}


		}
		public void WijzigPressed()
		{
			initializing = false;
			AccentJaNee.IsEnabled = true;
			var brush = new SolidColorBrush(Colors.Black);
			profielMetersTB.Foreground = brush;
			profielsubinfoTB.Foreground = brush;
			profielTTB.Foreground = brush;
			MetersTB.Foreground = brush;
			subinfoTB.Foreground = brush;
			tegelTTB.Foreground = brush;
			if (sType == SubType.hoekprofiel)
			{
				foreach (WrapPanel wp in SubCatPanel.Children.Where(x => x.GetType() == typeof(WrapPanel)))
				{
					foreach (ProductKControl pk in wp.Children.Where(x => x.GetType() == typeof(ProductKControl)))
					{
						if (SParent.SubContPanel.Children.First() == this)
						{
							pk.IsEnabled = true;
							
						}
						else if (pk.hoekprofiel.ProfielType == _ruimte.GekozenProfielen.First().ProfielType)
						{
							pk.IsEnabled = true;
						}
					}
				}
			}
			else
			{
				foreach (var control in SubCatPanel.Children)
				{
					IBaseControl IB = control as IBaseControl;
					if (IB != null)
					{
						IB.WijzigPressed();
					}
					else
					{
						WrapPanel WP = control as WrapPanel;
						if (WP != null)
						{
							foreach (IBaseControl IBC in WP.Children)
							{
								IBC.WijzigPressed();
							}
						}
						else
						{
							Control C = control as Control;
							if (C != null)
							{
								C.IsEnabled = true;
							}
							else
							{
								TextBlock TB = control as TextBlock;
								if (TB != null)
								{
									TB.Foreground = new SolidColorBrush(Colors.Black);
								}
							}
						}
					}
				}
				SParent.AccentGuard(this.hoofdcatnummer, false);
			}
		
			/*switch (sType)
			{
				case SubType.nabewerking:
					foreach (IBaseControl BC in SubCatPanel.Children)
					{
						BC.WijzigPressed();
					}
					break;
				case SubType.product:
					foreach (WrapPanel wp in SubCatPanel.Children.Where(x => x.GetType() == typeof(WrapPanel)))
					{
						foreach (ProductKControl pk in wp.Children.Where(x=>x.GetType()==typeof(ProductKControl)))
						{ 
							
							pk.IsEnabled = true;
						}
					}
					break;
			}*/
			
			
		}
		public void Clear4Submit()
		{
			if (_ruimte != null)
			{
				switch (sType)
				{
					case SubType.product:
						if (HoofdCatType == MainCatType.normaal)
						{
							_ruimte.GekozenTegels.Clear();
						}
						else if (HoofdCatType == MainCatType.accent)
						{
							_ruimte.GekozenAccenten.Clear();
							var optex = (from o in _ruimte.GekozenOpties
										 where o.OptieSoort == "OT3"
										 select o).ToList();
							foreach (var op in optex)
							{
								var otd = (from o in _ruimte.GekozenOpties
										   where o.OptieID == op.OptieID
										   select o).FirstOrDefault();
								_ruimte.GekozenOpties.Remove(otd);
							}
						}
						break;
					case SubType.hoekprofiel:
						{
							_ruimte.GekozenProfielen.Clear();
						}
						break;
					case SubType.nabewerking:
						_ruimte.GekozenBewerkingen.Clear();
						break;
				}
			}
			
			
		}
		public bool SubmitPressed()
		{
			bool retval = false;
			if (sType == SubType.product)
			{
				
				if (HoofdCatType == MainCatType.normaal)
				{
					//_ruimte.GekozenTegels.Clear();
					foreach (WrapPanel wb in SubCatPanel.Children.Where(x => x.GetType() == typeof(WrapPanel)))
					{
						foreach (ProductKControl pkc in wb.Children.Where(x => x.GetType() == typeof(ProductKControl)))
						{
							if (pkc.janeeRdBtn.IsChecked == true)
							{
								_ruimte.GekozenTegels.Add(pkc.product);
								retval = true;
							}
						}
					}
				}
				else if (HoofdCatType == MainCatType.accent)
				{
					//_ruimte.GekozenAccenten.Clear();
					foreach (WrapPanel wb in SubCatPanel.Children.Where(x => x.GetType() == typeof(WrapPanel)))
					{
						foreach (ProductKControl pkc in wb.Children.Where(x => x.GetType() == typeof(ProductKControl)))
						{
							if (pkc.janeeRdBtn.IsChecked == true)
							{
								if (pkc.product.ID != "0")
								{
									_ruimte.GekozenAccenten.Add(pkc.product);
								}
								retval = true;
							}
						}
					}
					foreach (KeuzeList kl in SubCatPanel.Children.Where(x => x.GetType() == typeof(KeuzeList)))
					{
						if (kl.Visibility == Visibility.Visible)
						{
							if (kl.Gekozen() < 1 && kl.Gekozen()!= kl.ListBox.Items.Count)
							{
								return false;
							}
							if (kl.SubmitPressed())
							{
								retval = true;
							}
						}
						else
						{
							foreach (var item in kl.Inputlist)
							{
								var opt = item as OptieKeuze;
								if (opt != null)
								{
									_ruimte.GekozenOpties.Remove(opt);
								}
							}
						}
					}
				}
				
				
				
			}
			else if (sType == SubType.hoekprofiel)
			{
				//_ruimte.GekozenProfielen.Clear();
				var cnt = SubCatPanel.Children.Where(x => x.GetType() == typeof(WrapPanel)).Count();
				foreach (WrapPanel wp in SubCatPanel.Children.Where(x => x.GetType() == typeof(WrapPanel)))
				{
					foreach (ProductKControl pk in wp.Children.Where(x => x.GetType() == typeof(ProductKControl)))
					{
						if (pk.janeeRdBtn.IsChecked == true)
						{
							_ruimte.GekozenProfielen.Add(pk.hoekprofiel);
							cnt--;
						}
					}
				}
				if (cnt == 0)
				{
					retval = true;
				}
			}
			else if (sType == SubType.nabewerking)
			{
				retval = true;
				foreach (KeuzeList kl in SubCatPanel.Children.Where(x => x.GetType() == typeof(KeuzeList)))
				{
					if (!kl.SubmitPressed())
					{
						retval = false;
					}
				}
                foreach (NabewerkingKeuzeControl nkc in SubCatPanel.Children.OfType<NabewerkingKeuzeControl>())
                {
                    nkc.SubmitPressed();
                }
			}
			else if (sType != SubType.info)
			{
				retval = true;
				foreach (IBaseControl cntrl in SubCatPanel.Children.Where(x => x.GetType() == typeof(IBaseControl)))
				{
					if (!cntrl.SubmitPressed())
					{
						retval = false;
					}
				}
			}
			else
			{
				retval = true;
			}
			return retval;
		}
		void janeeRdBtn_Checked(object sender, RoutedEventArgs e)
		{
			RadioButton prodctrl = sender as RadioButton;
			WrapPanel parent = ((prodctrl.Parent as Grid).Parent as ProductKControl).Parent as WrapPanel;
			if (sType == SubType.product)
			{
				foreach (ProductKControl pk in parent.Children.Where(x => x.GetType() == typeof(ProductKControl)))
				{
					if (pk.janeeRdBtn != prodctrl)
					{
						pk.janeeRdBtn.IsChecked = false;
					}
					else if (((prodctrl.Parent as Grid).Parent as ProductKControl).product.ID != "0")
					{
						if (!SParent.Matching)
						{
							SParent.MatchKeuze(this, pk.product.Kleur,false);
						}
					}

				}
				if (((prodctrl.Parent as Grid).Parent as ProductKControl).product.ID != "0")
				{
					AccentJaNee.IsChecked = false;
					
					if (KList_vlakken != null)
					{
						KList_vlakken.Visibility = Visibility.Visible;
					}
					if (Klist_opties != null)
					{
						Klist_opties.Visibility = Visibility.Visible;
					}
					if (samenvatList != null)
					{
						UpdateSummaryM();
						samenvatList.Visibility = Visibility.Visible;
					}
				}
				else
				{
					AccentJaNee.IsChecked = true;
					if (KList_vlakken != null)
					{
						KList_vlakken.Visibility = Visibility.Collapsed;
					}
					if (Klist_opties != null)
					{
						Klist_opties.Visibility = Visibility.Collapsed;
					}
					if (samenvatList != null)
					{
						samenvatList.Visibility = Visibility.Collapsed;
					}
				}
			}
			else if (sType == SubType.hoekprofiel)
			{
				if (!SParent.Matching)
				{

					SParent.MatchProfiel(this, ((prodctrl.Parent as Grid).Parent as ProductKControl).hoekprofiel.ProfielType);
				}
			}
		}

		private void prijsBtn_Click(object sender, RoutedEventArgs e)
		{

		}

		private void InfoBtn_Click(object sender, RoutedEventArgs e)
		{

		}

		private void AccentJaNee_Checked(object sender, RoutedEventArgs e)
		{
			/*if (KList != null)
			{
				KList.Visibility = Visibility.Collapsed;
			}*/
			SParent.AccentGuard(this.hoofdcatnummer, initializing);
			foreach (WrapPanel wp in SubCatPanel.Children.Where(x => x.GetType() == typeof(WrapPanel)))
			{
				foreach (ProductKControl pk in wp.Children.Where(x => x.GetType() == typeof(ProductKControl)))
				{
					if (pk.product.ID == "0")
					{
						pk.janeeRdBtn.IsChecked = true;
						AccentJaNee.IsChecked = true;
					}
				}
			}
		}

		private void AccentJaNee_Unchecked(object sender, RoutedEventArgs e)
		{
			/*if (KList != null)
			{
				KList.Visibility = Visibility.Visible;
			}*/
			SParent.AccentGuard(this.hoofdcatnummer, initializing);
			foreach (WrapPanel wp in SubCatPanel.Children.Where(x => x.GetType() == typeof(WrapPanel)))
			{
				foreach (ProductKControl pk in wp.Children.Where(x => x.GetType() == typeof(ProductKControl)))
				{
					pk.IsEnabled = true;
					
				}
			}
        }


#region bewerkingnewstyle

//        //vars
//        public ObservableCollection<NabewerkingOptieHolder> OptieHoldersCollection { get; set; }
//        public ObservableCollection<NabewerkingUiRegel> UiRegelCollection { get; set; }
//        //vars

//        //hoofd functie voor nabewerking pagina (called per sub cat)
//        private void MakeBewerkingMain(IOfferte nabew, Ruimte r)
//        {
//            UiRegelCollection = new ObservableCollection<NabewerkingUiRegel>();
                
//            var prodrefs = Acumulator.Instance().ctx.Productens;

//            foreach (var product in r.GekozenTegels)
//            {
//                var prodcat = (from pr in prodrefs
//                               where pr.PD_ID == product.ID
//                               select pr.PC_NR).FirstOrDefault().ToString();

//                if (!String.IsNullOrEmpty(prodcat))
//                {
//                    switch (prodcat)
//                    {
//                        case "1"://tegel
//                            var accentMatchT = r.AanvullendeMeters.Where(x => (x as ExtraMeters).ProdSoort == ExpertControls.Models.ProductSoort.Tegel
//                                                                        && (x as ExtraMeters).LinkedProduct == product).FirstOrDefault();
//                            BewerkingTegelRegel(nabew, product,accentMatchT);
//                            break;
//                        case "2"://profiel
//                           //geen hoekprofielen hier
//                           //BewerkingHoekprofielRegel(nabew, xxxx);
//                            break;
//                        case "3"://dorpel
//                            var accentMatchD = r.AanvullendeMeters.Where(x => (x as ExtraMeters).ProdSoort == ExpertControls.Models.ProductSoort.Dorpel
//                                                                       && (x as ExtraMeters).LinkedProduct == product).FirstOrDefault();
//                            BewerkingDorpelRegel(nabew, product,accentMatchD);
//                            break;
//                    }
//                }

//            }

//            foreach (var accent in r.GekozenAccenten)
//            {
//                var prodcat = (from pr in prodrefs
//                               where pr.PD_ID == accent.ID
//                               select pr.PC_NR).FirstOrDefault().ToString();

            
//                if (!String.IsNullOrEmpty(prodcat))
//                {
//                    switch (prodcat)
//                    {
//                        case "1"://tegel
//                            var accentMatchT = r.AanvullendeMeters.Where(x => (x as ExtraMeters).ProdSoort == ExpertControls.Models.ProductSoort.Tegel
//                   && (x as ExtraMeters).LinkedProduct.ID == accent.ID).FirstOrDefault();
//                            BewerkingTegelRegelAccent(nabew, accent, accentMatchT);
//                            break;
//                        case "2"://profiel
//                            //geen hoekprofiel hier >> profiel != product
//                            break;
//                        case "3"://dorpel
//                            var accentMatchD = r.AanvullendeMeters.Where(x => (x as ExtraMeters).ProdSoort == ExpertControls.Models.ProductSoort.Dorpel
//                   && (x as ExtraMeters).LinkedProduct.ID == accent.ID).FirstOrDefault();
//                            BewerkingDorpelRegelAccent(nabew, accent,accentMatchD);
//                            break;
//                    }
//                }
//            }

//            foreach (var profiel in r.GekozenProfielen)
//            {
//                var accentMatch = r.AanvullendeMeters.Where(x => (x as ExtraMeters).ProdSoort == ExpertControls.Models.ProductSoort.HoekProfiel 
//                    && (x as ExtraMeters).LinkedHoekProf == profiel).FirstOrDefault();
//                if (accentMatch != null)
//                {
//                    BewerkingHoekprofielRegelAccent(nabew, profiel,accentMatch);
//                }
//                else
//                {
//                    BewerkingHoekprofielRegel(nabew, profiel,accentMatch);
//                }
//            }


//            if (UiRegelCollection.Count() > 0)
//            {
//                MessageBox.Show("break here");
//            }
//        }

//        //product regels
//        //tegel regels
//        //normaal
//        private void BewerkingTegelRegel(IOfferte nabew,Product tegel,ExtraMeters accentInfo)
//        {
//            var nRegel = new NabewerkingUiRegel
//            {
//                Meters = accentInfo != null ? accentInfo.Meters : (nabew as SubCatPerPakket).TotaalMeters,
//                Eenheid = GetEenheid(tegel.LinkedSubCat),
//                InfoText = tegel.Omschrijving.Split('€')[0]
//            };
//            UiRegelCollection.Add(nRegel);
//        }

//        //accent
//        private void BewerkingTegelRegelAccent(IOfferte nabew,Product accentTegel,ExtraMeters accentInfo)
//        {
//            var nRegel = new NabewerkingUiRegel
//            {
//                Meters = accentInfo.Meters,
//                Eenheid = GetEenheid(accentTegel.LinkedSubCat),
//                InfoText = accentTegel.Omschrijving.Split('€')[0]
//            };
//            UiRegelCollection.Add(nRegel);
//        }

//        //hoekprofiel regels
//        //normaal
//        private void BewerkingHoekprofielRegel(IOfferte nabew, HoekProfiel profiel,ExtraMeters accentInfo)
//        {
//            var nRegel = new NabewerkingUiRegel
//            {
//                Meters = accentInfo != null ? accentInfo.Meters : (nabew as SubCatPerPakket).TotaalMeters,
//                Eenheid = GetEenheid(profiel.LinkedSubCat),
//                InfoText = profiel.Omschrijving.Split('€')[0]
//            };
//            UiRegelCollection.Add(nRegel);
//        }

//        //accent
//        private void BewerkingHoekprofielRegelAccent(IOfferte nabew, HoekProfiel accentProfiel,ExtraMeters accentInfo)
//        {
//            var nRegel = new NabewerkingUiRegel
//            {
//                Meters = accentInfo.Meters,
//                Eenheid = (from e in Acumulator.Instance().ctx.SubCats
//                           where e.SCB_ID == (from scb in Acumulator.Instance().ctx.SubCatPerPakkets
//                                              where scb.SCBP_ID == accentProfiel.LinkedSubCat
//                                              select scb.SCB_NR).FirstOrDefault() 
//                           select e.eenheidMETERS).FirstOrDefault(),
//                InfoText = accentProfiel.Omschrijving.Split('€')[0]

//            };
//            UiRegelCollection.Add(nRegel);
//        }

//        //dorpel regels
//        //normaal
//        private void BewerkingDorpelRegel(IOfferte nabew,Product dorpel,ExtraMeters accentInfo)
//        {
//            var nRegel = new NabewerkingUiRegel
//            {
//                Meters = accentInfo != null ? accentInfo.Meters : (nabew as SubCatPerPakket).TotaalMeters,
//                Eenheid =GetEenheid(dorpel.LinkedSubCat),
//                InfoText = dorpel.Omschrijving.Split('€')[0]

//            };
//            UiRegelCollection.Add(nRegel);
//        }

//        //accent
//        private void BewerkingDorpelRegelAccent(IOfferte nabew,Product accentDorpel,ExtraMeters accentInfo)
//        {
           
//            var nRegel = new NabewerkingUiRegel
//            {
//                Meters = accentInfo.Meters,
//                Eenheid = GetEenheid(accentInfo.LinkedSubCat),
//                InfoText = accentDorpel.Omschrijving.Split('€')[0]

//            };
//            UiRegelCollection.Add(nRegel);
//        }

//        //helper methods
//        private static string GetEenheid(string inputLinkedSubCat)
//        {
//             var linkedSubCat = inputLinkedSubCat.StartsWith("Sub4Accent") ? inputLinkedSubCat.Substring(10) : inputLinkedSubCat;
            
//            var eenheid = (from e in Acumulator.Instance().ctx.SubCats
//                           where e.SCB_ID ==(from scb in Acumulator.Instance().ctx.SubCatPerPakkets
//                                             where scb.SCBP_ID == linkedSubCat
//                                              select scb.SCB_NR).FirstOrDefault() 
//                           select e.eenheidMETERS).FirstOrDefault();

//            if(string.IsNullOrEmpty(eenheid))
//            {
//                eenheid = (from e in Acumulator.Instance().ctx.SubCats
//                           where e.SCB_ID == linkedSubCat
//                           select e.eenheidMETERS).FirstOrDefault();
//            }

//            return eenheid;
//        }
//        //nabewerking collector
//        private void AddNabewerking()
//        {
            
//        }

//        public void GroupNabewerkingOptiesProducten()
//        {
//            var GroupIterator = 0;
//            foreach (var holder in OptieHoldersCollection)
//            {
//                holder.GroupNr = GroupIterator;
//                foreach (var comp in OptieHoldersCollection.Where(x => x != holder))
//                {
//                    foreach (var set in holder.NabewerkingSets)
//                    {
//                        var match = comp.NabewerkingSets.Where(x => x.NabewerkingSetNr == set.NabewerkingSetNr).FirstOrDefault();
//                        if (match != null)
//                        {
//                            comp.GroupNr = holder.GroupNr;
//                        }

//                    }
//                }
//                GroupIterator++;
//            }
//        }


#endregion

    }
	public class VisLenConvertor : IValueConverter
	{
		public object Convert(
			object value,
			Type targetType,
			object parameter,
			CultureInfo culture)
			{
				int textlen = (int)value;
				if (textlen > 0)
				{
					return Visibility.Visible;
				}
				else
				{
					return Visibility.Collapsed;
				}
			
			}

		public object ConvertBack(
			object value,
			Type targetType,
			object parameter,
			CultureInfo culture)
			{
				Visibility visi = (Visibility)value;
				return (visi == Visibility.Visible);
			}
	}



}

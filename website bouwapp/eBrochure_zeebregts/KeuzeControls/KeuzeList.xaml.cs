using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using eBrochure_zeebregts.Classes;
using eBrochure_zeebregts.Web.Services;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Media;

namespace eBrochure_zeebregts.KeuzeControls
{
	public partial class KeuzeList :  UserControl, IBaseControl
	{
		
		private bool _singleKeuze;
		public SubCatkControl LinkedSControl;
		private PakketKeuze Pakket_Old;
		private bool clearpakketflag = false;
		private Ruimte RuimteHuidig;
		private string KeuzeSoort;
		public List<IOfferte> Inputlist;
		private List<CustomListItem> Disabled = new List<CustomListItem>();
		public KeuzeList(Ruimte R, string keuzesoort,string title)
		{
			InitializeComponent();
			RuimteHuidig = R;
			KeuzeSoort = keuzesoort;
			if (KeuzeSoort == "profiel" || KeuzeSoort == "bewerking")
			{
				OptBorder.BorderThickness = new Thickness(0);
			}
			if (title != null)
			{
				KLtile.Text = title;
			}
			
		}

		private void save_Keuze_Single()
		{
			switch (KeuzeSoort)
			{
				case "Pakket":
					CustomListItem si = ListBox.Items.Where(it => (it as CustomListItem).Gekozen == true).FirstOrDefault() as CustomListItem;
					PakketKeuze PakK = RuimteHuidig.Children.Where(pk => pk.GetType() == typeof(PakketKeuze) && (pk as PakketKeuze).Pakket_ID == si.Id).FirstOrDefault() as PakketKeuze;
					RuimteHuidig.GekozenPakket = PakK;
					if (Pakket_Old != null && PakK.Pakket_ID != Pakket_Old.Pakket_ID)
					{
						RuimteHuidig.GekozenBewerkingen.Clear();
						RuimteHuidig.GekozenTegels.Clear();
						RuimteHuidig.GekozenAccenten.Clear();
						RuimteHuidig.GekozenProfielen.Clear();
					}

					break;
				case "Opties":
					//var ok = Inputlist.Where(I => (I as OptieKeuze).OptieID == (ListBox.Items.Where(p => (p as CustomListItem).Gekozen == true) as CustomListItem).Id) as OptieKeuze;
					foreach (CustomListItem cli in ListBox.Items)
					{
						var ok = (from k in Inputlist where (k as OptieKeuze).OptieID == cli.Id select k).FirstOrDefault() as OptieKeuze;
						if (cli.Gekozen == true && !RuimteHuidig.GekozenOpties.Contains(ok))
						{
							RuimteHuidig.GekozenOpties.Add(ok);
							//RuimteHuidig.updateSubcats(ok);
						}
						else if (cli.Gekozen == false && RuimteHuidig.GekozenOpties.Contains(ok))
						{
							RuimteHuidig.GekozenOpties.Remove(ok);
							//RuimteHuidig.updateSubcats(null);
						}
					}
					break;
				case "bewerking":
					var sliTag = (ListBox.Items.Where(s => (s as CustomListItem).Gekozen == true).FirstOrDefault() as CustomListItem).Id;
					var nb = Inputlist.Where(I => (I as Nabewerking).Nabewerking_ID == sliTag).FirstOrDefault() as Nabewerking;
					//var nb = Inputlist.Where(I => (I as Nabewerking).Nabewerking_ID == (ListBox.Items.Where(p => (p as SingleListItem).IsChecked == true) as SingleListItem).Tag) as Nabewerking;
					RuimteHuidig.GekozenBewerkingen.Add(nb);
					break;
				case "profiel":
					var slitag = (ListBox.Items.Where(s => (s as CustomListItem).Gekozen == true).FirstOrDefault() as CustomListItem).Id;
					var pfl = Inputlist.Where(I => (I as Product).ID == slitag).FirstOrDefault() as Product;
					//var pfl = Inputlist.Where(I => (I as Product).ID == (ListBox.Items.Where(p => (p as SingleListItem).IsChecked == true) as SingleListItem).Tag) as Product;
					RuimteHuidig.GekozenTegels.Add(pfl);
					break;
				case "Opties_accent":
					OptieKeuze o_a;
					foreach (CustomListItem cli in ListBox.Items)
					{
						o_a = (from k in Inputlist where (k as OptieKeuze).OptieID == cli.Id select k).FirstOrDefault() as OptieKeuze;
						if (cli.Gekozen == true)
						{
							if (!RuimteHuidig.GekozenOpties.Contains(o_a))
							{
								RuimteHuidig.GekozenOpties.Add(o_a);
								//RuimteHuidig.updateSubcats(o_a);
							}
						}
						else if(RuimteHuidig.GekozenOpties.Contains(o_a))
						{
							RuimteHuidig.GekozenOpties.Remove(o_a);
							//RuimteHuidig.updateSubcats(null);
						}
					}
					
					break;
			}
			RuimteHuidig.updateSubcats();
		}
		private void save_Keuze_Multi()
		{
			switch (KeuzeSoort)
			{
				case "Opties":
					RuimteHuidig.GekozenOpties.Clear();
					RuimteHuidig.GekozenPakket = null;
					RuimteHuidig.updateSubcats();
					RuimteHuidig.FilterPakketten();
                    
					foreach (CustomListItem MLI in ListBox.Items)
					{
							var ok = (from k in Inputlist where (k as OptieKeuze).OptieID == MLI.Id select k).FirstOrDefault() as OptieKeuze;
							if (ok != null)
							{
								if (MLI.Gekozen == true && !RuimteHuidig.GekozenOpties.Contains(ok))
								{
									RuimteHuidig.GekozenOpties.Add(ok);
									//RuimteHuidig.updateSubcats(ok);
								}
								//else if (MLI.Gekozen == false && RuimteHuidig.GekozenOpties.Contains(ok))
								//{
								//    RuimteHuidig.GekozenOpties.Remove(ok);
								//}
							}
							
						
					}
					
					break;
				case "Opties_accent":
					foreach (CustomListItem cli in ListBox.Items)
					{
						var ok = (from k in Inputlist where (k as OptieKeuze).OptieID == cli.Id select k).FirstOrDefault() as OptieKeuze;
						if (ok != null)
						{
							if (cli.Gekozen == true &&!RuimteHuidig.GekozenOpties.Contains(ok))
							{		
								RuimteHuidig.GekozenOpties.Add(ok);
								//RuimteHuidig.updateSubcats(ok);
							}
							else if (cli.Gekozen == false && RuimteHuidig.GekozenOpties.Contains(ok))
							{
								RuimteHuidig.GekozenOpties.Remove(ok);
							}
						}
					}
					break;
			}

            RuimteHuidig.SwitchBluePrint();
			RuimteHuidig.updateSubcats();
		}
		public void Clear4Submit()
		{ }
		public bool SubmitPressed()
		{
			 if (clearpakketflag)
			{
				RuimteHuidig.PakketOud = RuimteHuidig.GekozenPakket;
				RuimteHuidig.GekozenPakket = null;
				clearpakketflag = false;
			}
			bool retval = false;
			if (_singleKeuze)
			{
				if (Gekozen() > 0)
				{
					save_Keuze_Single();
					ListBox.IsEnabled = false;
					retval = true;
				}
				else
				{
					retval = false;
				}
			}
			else
			{
				save_Keuze_Multi();
				ListBox.IsEnabled = false;
				retval = true;
			
			
			}
			RuimteHuidig.FilterPakketten();
			
			return retval;
		}
		public void WijzigPressed()
		{
			if (KeuzeSoort == "Opties")
			{
				clearpakketflag = true;
				Acumulator.Instance().bluePrintManager.SetHuidigRuimteSetNr(Acumulator.Instance().BB.HuidigRuimte.RuimteID, "Basis");
				Acumulator.Instance().InfoBar.SetRuimteID(Acumulator.Instance().BB.HuidigRuimte.RuimteID);
				//Acumulator.Instance().SetHuidigRuimteSetKey(Acumulator.Instance().BB.HuidigRuimte.RuimteID, "Basis");
             //   var path = Acumulator.Instance().TekeningBijRuimte[Acumulator.Instance().BB.HuidigRuimte.RuimteID]["Basis"];
               // Acumulator.Instance().InfoBar.LoadImg(path);
			}
			this.IsEnabled = true;
			ReEnable();
		}
		public void ReloadDetail()
		{
			foreach (CustomListItem item in ListBox.Items)
			{
				item.Beschikbaar = false;
			}
			ListBox.IsEnabled = true;
			var detopt = RuimteHuidig.Children.Where(x => x.GetType() == typeof(OptieKeuze) && (x as OptieKeuze).OptType == OptieType.Resulting).ToList();
			if (detopt.Count > 0)
			{
				InitList(detopt, false);
			}
		
			
			
			
			
		}
		public int Gekozen()
		{
			int keuzecntr = 0;
			foreach (CustomListItem item in ListBox.Items)
				{
					if (item.Gekozen == true)
					{
						keuzecntr++;
					}
				}
			return keuzecntr;
		}
		public void InitList(List<IOfferte> L, bool single)
		{
			if (Inputlist != null)
			{
				foreach (var o in L)
				{
					var oudeK = Inputlist.Where(i => i.GetType() == typeof(OptieKeuze) && (i as OptieKeuze).OptieID == (o as OptieKeuze).OptieID).FirstOrDefault();
					Inputlist.Remove(oudeK);
				}
				Inputlist.AddRange(L);
			}
			else
			{
				Inputlist = L;
			}
			_singleKeuze = single;
			//single switched soort item
			if (KeuzeSoort == "Pakket")
			{
				GetBasisPrijs();
			}
	
			string gpnm = RuimteHuidig.RandomGenerator.Next(99999).ToString();
			if (int.Parse(gpnm) == RuimteHuidig.lastRandom)
			{
				gpnm = RuimteHuidig.RandomGenerator.Next(99999).ToString();
			}
			RuimteHuidig.lastRandom = int.Parse(gpnm);
			
			string Prijsgroep = "";
			foreach (var item in L)
			{
				if (single)
				{
					if (KeuzeSoort == "Pakket")
						{
						   getpakketprijs(item as PakketKeuze, Acumulator.Instance().ctx);
						}
					var Cli = mkListItem_single(item, Acumulator.Instance().ctx, gpnm);
					
					if (KeuzeSoort == "Pakket")
					{
						var pg = (item as PakketKeuze).PakketPrijsgroep_NR;
						if (pg == Prijsgroep)
						{
							Cli.SetBorders(false, false, true);
							
						}
						else
						{
							Cli.SetBorders(true, true, false);
						}
					
						Prijsgroep = pg;
					}
					ListBox.Items.Add(Cli);
				}
				else
				{
					gpnm = RuimteHuidig.RandomGenerator.Next(99999).ToString();
					if (int.Parse(gpnm) == RuimteHuidig.lastRandom)
					{
						gpnm = RuimteHuidig.RandomGenerator.Next(99999).ToString();
					}
					RuimteHuidig.lastRandom = int.Parse(gpnm);
					ListBox.Items.Add(mkListItem_multi(item, Acumulator.Instance().ctx,gpnm));
				}
			}
			if (KeuzeSoort == "Pakket")
			{
				(ListBox.Items.Last() as CustomListItem).BottomLine = true;
				(ListBox.Items.First() as CustomListItem).HorizontalLine = false;
			}
			if (ListBox.Items.Count == 1 && single)
			{
				(ListBox.Items[0] as CustomListItem).Gekozen = true;
			}
			
		}
		private double BasisPrijs = 0;
		private void GetBasisPrijs()
		{
			var bprijzen = from o in RuimteHuidig.GekozenOpties
						   select o.BasisPrijs;
			foreach (double bmp in bprijzen)
			{
				BasisPrijs += bmp;
			}
			//BasisPrijs = Acumulator.Instance().BerekenEindPrijs(BasisPrijs);
		}
		private void getpakketprijs(PakketKeuze pk, eBrochureDomainContext ctx)
		{
			if (pk != null)
			{
				double PakketPrijs = 0;
				var prijzen = from mpro in ctx.MeerPrijzenRuimteOpties
							  where mpro.PP_NR == pk.PakketPrijsgroep_NR && RuimteHuidig.RuimteID==mpro.R_NR
							  select mpro.meerprijs;
				foreach (decimal mp in prijzen)
				{
					PakketPrijs += (double)mp;
				}
				PakketPrijs = Acumulator.Instance().BerekenEindPrijs(PakketPrijs);
				pk.PrijsHuidig = PakketPrijs;
			}

		}
		public void ReloadKeuzes()
		{
			switch (KeuzeSoort)
			{
				case "Opties_accent":
				case "Opties":
					if (RuimteHuidig.GekozenOpties.Count > 0)
					{
						Acumulator.Instance().BB.ShowWijzigBtn(true);
						var opties = (from ok in RuimteHuidig.GekozenOpties
									  select (ok as OptieKeuze).OptieID).ToList();
						if (_singleKeuze)
						{
							foreach (CustomListItem si in ListBox.Items)
							{
								if (opties.Contains(si.Id))
								{
									si.Gekozen = true;
								}
								if (Acumulator.Instance().BB.IsCurrentComplete())
								{
									si.Beschikbaar = false;
								}
							}
						}
						else
						{
							foreach (CustomListItem mi in ListBox.Items)
							{
								if (opties.Contains(mi.Id))
								{
									mi.Gekozen = true;
								}
								
							}
							foreach (CustomListItem mi in ListBox.Items)
							{
								if (Acumulator.Instance().BB.IsCurrentComplete())
								{
									mi.Beschikbaar = false;
								}
								else
								{
									/*var enab = (from o in RuimteHuidig.Children
												where o.GetType() == typeof(OptieKeuze)
												&& (o as OptieKeuze).OptType == OptieType.Resulting
												select (o as OptieKeuze).OptieID).ToList();
									if (!enab.Contains(mi.Id))
									{
										mi.Beschikbaar = false;
									}*/
								}
							}
						}

					}
					else
					{
						if (KeuzeSoort == "Opties")
						{
							foreach (CustomListItem cli in ListBox.Items)
							{
								cli.Beschikbaar = false;
							}
						}
						//Acumulator.Instance().BB.ShowWijzigBtn(false);
					}
					break;
				
				case "Pakket":
					Pakket_Old = null;
					if (RuimteHuidig.GekozenPakket != null)
					{
						Pakket_Old = RuimteHuidig.GekozenPakket;
					}
					else if (RuimteHuidig.PakketOud != null)
					{
						Pakket_Old = RuimteHuidig.PakketOud;
					}
					if (Pakket_Old != null)
					{
						Acumulator.Instance().BB.ShowWijzigBtn(true);
						foreach (CustomListItem sli in ListBox.Items)
						{
							if (sli.Id.ToString() == Pakket_Old.Pakket_ID && sli.Beschikbaar)
							{
								sli.Gekozen = true;
							}
							if (Acumulator.Instance().BB.IsCurrentComplete())
							{
								sli.Beschikbaar = false;
							}
						}
					}
					break;
				case "profiel":
					var profielsc = (from scpp in Acumulator.Instance().ctx.SubCatPerPakkets
									 join sc in Acumulator.Instance().ctx.SubCats on scpp.SCB_NR equals sc.SCB_ID
									 where sc.SCB_ID == "SCB3" || sc.SCB_ID == "SCB10" || sc.SCB_ID == "SCB12"
									select scpp.SCBP_ID).ToList();
					var prof = RuimteHuidig.GekozenTegels.Where(p => profielsc.Contains(p.LinkedSubCat)).FirstOrDefault() as Product;
					if (prof != null)
					{
						foreach (CustomListItem si in ListBox.Items)
						{
							if (si.Id.ToString() == prof.ID)
							{
								si.Gekozen = true;
							}
							if (Acumulator.Instance().BB.IsCurrentComplete())
							{
								si.Beschikbaar = false;
							}
						}
					}
					break;
				case "bewerking":
					if (RuimteHuidig.GekozenBewerkingen.Count > 0)
					{
						var bewIds = (from nb in RuimteHuidig.GekozenBewerkingen
									  select nb.Nabewerking_ID).ToList();
						foreach (CustomListItem sli in ListBox.Items)
						{
							if (bewIds.Contains(sli.Id))
							{
								sli.Gekozen = true;
							}
							if (Acumulator.Instance().BB.IsCurrentComplete())
							{
								sli.Beschikbaar = false;
							}
						}
					}
					break;
			}
		}
		public void ReEnable()
		{
			foreach (CustomListItem x in ListBox.Items)
				{
					if (!Disabled.Contains(x))
					{
						x.Beschikbaar = true;
					}
				}
		}
		private CustomListItem mkListItem_single(IOfferte item, eBrochureDomainContext ctx, string grpnm)
		{
			//invulling aan listItem geven, wss 
			var optie = item as OptieKeuze;
			if (optie != null)
			{
				var sli = new CustomListItem(optie.Omschrijving, optie.OptieID, item.IsEnabled, true, this, grpnm);
				//var sli = new SingleListItem(optie.Omschrijving + "   -   €"+ String.Format("{0:0.00}",optie.BasisPrijs), optie.OptieID, item.IsEnabled);
				if (!sli.Beschikbaar)
				{
					Disabled.Add(sli);
				}
				return sli;
			}
			var ruimte = item as RuimteOfferte;
			if (ruimte != null)
			{
				var sli = new CustomListItem(ruimte.Omschrijving, ruimte.Omschrijving, item.IsEnabled, true, this, grpnm);
				//var sli = new SingleListItem(ruimte.Omschrijving, ruimte.Omschrijving, item.IsEnabled);
				if (!sli.Beschikbaar)
				{
					Disabled.Add(sli);
				}
				return sli;
			}
			var pakket = item as PakketKeuze;
			if (pakket != null)
			{
				var sli = new CustomListItem(pakket.Omschrijving, pakket.Pakket_ID, pakket.IsEnabled, true, this, grpnm);
				var oli = new List<OpbouwItem>();
				oli.Add(new OpbouwItem("pakket "+pakket.Omschrijving.ToLower(), "€"+string.Format("{0:0.00}",pakket.PrijsHuidig),""));
				foreach (OptieKeuze ok in RuimteHuidig.GekozenOpties)
				{
					if (ok.OptieSoort != "OT3")
					{
						oli.Add(new OpbouwItem(ok.Omschrijving, "€" + string.Format("{0:0.00}", (ok.getMeerprijs(Acumulator.Instance().ctx, pakket.PakketPrijsgroep_NR))), ""));
					}
				}
				if (oli.Count > 0)
				{
					sli.Opbouw = oli;
				}
				//var sli = new SingleListItem(pakket.Omschrijving + "   -   €"+ String.Format("{0:0.00}",pakket.PrijsHuidig), pakket.Pakket_ID, item.IsEnabled);
				if (!sli.Beschikbaar)
				{
					Disabled.Add(sli);
				}
				
				return sli;
			}
			var bewerking = item as Nabewerking;
			if(bewerking != null)
			{
				var sli = new CustomListItem(bewerking.Omschrijving, bewerking.Nabewerking_ID, true, true, this, grpnm);
				//var sli =  new SingleListItem(bewerking.Omschrijving, bewerking.Nabewerking_ID, true);
				if (!sli.Beschikbaar)
				{
					Disabled.Add(sli);
				}
				return sli;
			}
			var product = item as Product;
			if (product != null)
			{
				var sli = new CustomListItem(product.Omschrijving, product.ID, true, true, this, grpnm);
				//var sli = new SingleListItem(product.Omschrijving, product.ID, true);
				if (!sli.Beschikbaar)
				{
					Disabled.Add(sli);
				}
				return sli;
			}
			return null;
		}
		private CustomListItem mkListItem_multi(IOfferte item, eBrochureDomainContext ctx, string gpnm)
		{
			//invulling aan listItem geven, wss 
			var optie = item as OptieKeuze;
			if (optie != null)
			{
				var mli = new CustomListItem(optie.Omschrijving, optie.OptieID, item.IsEnabled, false,this,gpnm);
				//var mli = new MultiListItem(optie.Omschrijving + "   -   €" + String.Format("{0:0.00}",optie.BasisPrijs), optie.OptieID, item.IsEnabled);
				//mli.Checked += new RoutedEventHandler(mli_Checked);
				//mli.Unchecked += new RoutedEventHandler(mli_Unchecked);
				var opb = new List<OpbouwItem>();
                var basispakket = (from rpt in ctx.RuimtesPerTypes
                                   where rpt.R_NR == RuimteHuidig.RuimteID
                                   select rpt.BasisPakket_NR).FirstOrDefault();
                var optiebasisprijs = 0.0;
                if (basispakket != null)
                {
                    var basispakprijs = (from pr in ctx.MeerPrijzenRuimteOpties
                                         where pr.R_NR == optie.OptieID && pr.PP_NR == (from p in ctx.Pakkettens where p.P_ID == basispakket select p.PP_NR).FirstOrDefault()
                                         select pr.meerprijs).FirstOrDefault();
                    if (basispakprijs != null)
                    {
                       optiebasisprijs =  Acumulator.Instance().BerekenEindPrijs((double)basispakprijs);
                    }
                }
                if (optiebasisprijs == 0)
                {
                    optiebasisprijs = optie.BasisPrijs;
                }
				opb.Add(new OpbouwItem("", "€" + String.Format("{0:0.00}", optiebasisprijs), "vanaf") { TextStijl = FontStyles.Italic });
				
				mli.Opbouw = opb;
				// add excludes
				var exc = from e in ctx.OngeldigeCombinatieOpbouws
						  where e.R_NR == optie.OptieID
						  select e.OC_NR;
				foreach (string s in exc)
				{
					var foo = (from ex in ctx.OngeldigeCombinatieOpbouws
							   where ex.OC_NR == s && ex.R_NR != optie.OptieID
							   select ex.R_NR).ToList<string>();
					foreach (string bar in foo)
					{
						mli.Excludes.Add(bar);
					}
				}
                ///////////////////
                var verplichtesetid = (from vcs in ctx.VerplichteCombinatieOpbouws
                                       where vcs.R_NR == optie.OptieID
                                       select vcs.VC_NR).FirstOrDefault();
                var combis = (from vco in ctx.VerplichteCombinatieOpbouws
                              where vco.VC_NR == verplichtesetid && vco.R_NR != optie.OptieID
                              select vco.R_NR).ToList();
                if (combis.Count > 0)
                {
                    foreach (string c in combis)
                    {
                        mli.Combines.Add(c);
                    }
                }
                ///////////////////
				var grayed = (from r in ctx.MeervoudigeCombinatiesOpbouws
							 where r.R_NR == optie.OptieID
							 select r.IsAfhankelijk).FirstOrDefault();
				var mcset = (from mc in ctx.MeervoudigeCombinatiesOpbouws
							where mc.R_NR == optie.OptieID
							select mc.MC_NR).ToList();
				var mcrids = (from mr in ctx.MeervoudigeCombinatiesOpbouws
							 where mcset.Contains(mr.MC_NR)
							 select mr.R_NR).ToList();
				int cntr = 0;
				foreach (OptieKeuze ok in Inputlist)
				{
					if (mcrids.Contains(ok.OptieID))
					{ cntr++; }
				}
				if (cntr == mcrids.Count && grayed!=null)
				{
					mli.Beschikbaar = !(bool)grayed;
				}
				//add unlocks
				var multi = from m in ctx.MeervoudigeCombinatiesOpbouws
							where m.R_NR == optie.OptieID && m.IsAfhankelijk == false
							select m.MC_NR;
				foreach (string s in multi)
				{
					var foo = (from mu in ctx.MeervoudigeCombinatiesOpbouws
							   where mu.MC_NR == s && mu.R_NR != optie.OptieID
							   select mu.R_NR).ToList();
					foreach (string bar in foo)
					{
						mli.Unlocks.Add(bar);
					}

				}
				if (!mli.Beschikbaar)
				{
					Disabled.Add(mli);
				}
				return mli;
			}
			var ruimte = item as RuimteOfferte;
			if (ruimte != null)
			{
				var mli = new CustomListItem(ruimte.Omschrijving, ruimte.Omschrijving, item.IsEnabled, false,this,gpnm);
				//var sli = new MultiListItem(ruimte.Omschrijving, ruimte.Omschrijving, item.IsEnabled);
				if (!mli.Beschikbaar)
				{
					Disabled.Add(mli);
				}
				//add excludes
				return mli;
			}
			return null;
		}

		public void Cli_Checked(CustomListItem cli)
		{
			var items = from i in ListBox.Items
						where cli.Excludes.Contains((i as CustomListItem).Id)
						select i;
				foreach (CustomListItem mi in items)
				{
					mi.Beschikbaar = false;
					Disabled.Add(mi);
				}
				var uitems = from u in ListBox.Items
							 where cli.Unlocks.Contains((u as CustomListItem).Id)
							 select u;
				foreach (CustomListItem mi in uitems)
				{
					mi.Beschikbaar = true;
					if (Disabled.Contains(mi))
					{
						Disabled.Remove(mi);
					}
				}
                ///////////////
                var combis = from i in ListBox.Items
                             where cli.Combines.Contains((i as CustomListItem).Id)
                             select i;
                foreach (CustomListItem ci in combis)
                {
                    if (ci.Gekozen == false)
                    {
                        ci.Gekozen = true;
                    }
                }
            //////////////////
				if (KeuzeSoort == "Opties_accent" && LinkedSControl !=null)
				{
					LinkedSControl.UpdateSummaryM();
				}
				if (KeuzeSoort == "bewerking" /*&& KLtile.Text == "Voegkleur"*/)
				{
					bool voegkl = KLtile.Text == "Voegkleur";
					if (LinkedSControl != null)
					{
						LinkedSControl.SParent.MatchKeuze(LinkedSControl, cli.Id,voegkl);
					}
                    
				}
		}
		public void Cli_Unchecked(CustomListItem cli)
		{
			var items = from i in ListBox.Items
						where cli.Excludes.Contains((i as CustomListItem).Id)
						select i;
			foreach (CustomListItem mi in items)
            {
                var ooknie = (from i in ListBox.Items
                              where (i as CustomListItem).Excludes.Contains(mi.Id) && (i as CustomListItem).Gekozen == true
                              select i).FirstOrDefault();
                if (ooknie == null)
                {

                    mi.Beschikbaar = true;
                    if (Disabled.Contains(mi))
                    {
                        Disabled.Remove(mi);
                    }
                }
			}
            ///////////////
            var combis = from i in ListBox.Items
                         where cli.Combines.Contains((i as CustomListItem).Id)
                         select i;
            foreach (CustomListItem ci in combis)
            {
                if (ci.Gekozen == true)
                {
                    ci.Gekozen = false;
                }
            }
            //////////////////
			var uitems = from u in ListBox.Items
						 where cli.Unlocks.Contains((u as CustomListItem).Id)
						 select u;
			foreach (CustomListItem mi in uitems)
			{
				mi.Gekozen = false;
				mi.Beschikbaar = false;
				Disabled.Add(mi);
			}
			if (KeuzeSoort == "Opties_accent" && LinkedSControl != null)
			{
				LinkedSControl.UpdateSummaryM();
			}
		}
		
	
		
		public void NoMarkup()
		{
			OptBorder.Visibility = Visibility.Collapsed;
			foreach (CustomListItem ci in ListBox.Items)
			{
				ci.SetBorders(false, false, false);
			}
		}
		public double UpdateSumary()
		{
			double tmpmtrs = 0;
			foreach (CustomListItem cli in ListBox.Items.Where(x=>x.GetType() == typeof(CustomListItem) && (x as CustomListItem).Gekozen == true))
			{
				var optie = Inputlist.Where(x => x.GetType() == typeof(OptieKeuze) && (x as OptieKeuze).OptieID == cli.Id).FirstOrDefault() as OptieKeuze;
				
				var meters = (from scpr in Acumulator.Instance().ctx.SubCatPerRuimteDeels
							  where scpr.R_NR == optie.OptieID && (scpr.SCB_NR != "SCB3" && scpr.SCB_NR != "SCB10" && scpr.SCB_NR != "SCB12") && scpr.meters < 0
							  select scpr.meters).FirstOrDefault();
				tmpmtrs += (double)meters;
			}

			return tmpmtrs;
		}
		public void SetLinkedSControl(SubCatkControl parent)
		{
			LinkedSControl = parent;
		}
		public void loadprices(string subcatppNR, SubCatkControl parent,Ruimte r)
		{
			LinkedSControl = parent;
			
			foreach (CustomListItem cli in ListBox.Items)
			{
				var optie = Inputlist.Where(x=>x.GetType() == typeof(OptieKeuze) && (x as OptieKeuze).OptieID == cli.Id).FirstOrDefault() as OptieKeuze;
				var subcatNR = (from scpp in Acumulator.Instance().ctx.SubCatPerPakkets
							   where scpp.SCBP_ID == subcatppNR
							   select scpp.SCB_NR).FirstOrDefault().ToString();
				var meters = (from scpr in Acumulator.Instance().ctx.SubCatPerRuimteDeels
							  join scp in Acumulator.Instance().ctx.SubCats on scpr.SCB_NR equals scp.SCB_ID
							  where scpr.SCB_NR == subcatNR && scpr.R_NR == optie.OptieID && scp.C_NR != "C8"
							  select scpr.meters).FirstOrDefault().ToString();
				if (meters.Length > 0)
				{
					meters += (from sc in Acumulator.Instance().ctx.SubCats
							   where sc.SCB_ID == subcatNR
							   select sc.eenheidMETERS).FirstOrDefault().ToString();
				}
				var oli = new List<OpbouwItem>();
				oli.Add(new OpbouwItem("","€"+string.Format("{0:0.00}",optie.getMeerprijs(Acumulator.Instance().ctx,r.GekozenPakket.PakketPrijsgroep_NR)),meters));
				cli.Opbouw = oli;
			}
		}

        private void ListBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var itm = ListBox.SelectedItem as CustomListItem;
            if (itm != null && itm.Beschikbaar)
                {
                    itm.Gekozen = itm.IsEnkel == true ? true : !itm.Gekozen;
                }
                e.Handled = true;
        }
	}
	public class CustomListItem:INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private KeuzeList Papa;
		private string _groupname;
		public string GroupName
		{ get { return _groupname; } set { _groupname = value; OnPropertyChanged("GroupName"); } }
		private bool _gekozen;
		public bool Gekozen
		{ 
			get
			{ 
				return _gekozen; 
			} 
			set 
			{
				_gekozen = value;
				OnPropertyChanged("Gekozen");
				if (value)
					{
						Papa.Cli_Checked(this);
					}
					else
					{
						Papa.Cli_Unchecked(this);
					}
			}
		}
		private bool _isenkel;
		public bool IsEnkel
		{ get { return _isenkel; } set{_isenkel=value;OnPropertyChanged("IsEnkel"); }}
		private bool _beschikbaar;
		public bool Beschikbaar
		{ get { return _beschikbaar; } set { _beschikbaar = value; OnPropertyChanged("Beschikbaar"); } }
		private bool _btnsnotinheader;
		public bool BtnsNotInHeader
		{ get { return _btnsnotinheader; } set { _btnsnotinheader = value; OnPropertyChanged("BtnsNotInHeader"); } }
		private string _naam;
		public string Naam
		{ get { return _naam; } set { _naam = value; OnPropertyChanged("Naam"); } }
		private List<OpbouwItem> _opbouw;
		public List<OpbouwItem> Opbouw
		{ get { return _opbouw; } set { _opbouw = value; OnPropertyChanged("Opbouw"); } }
		private string _id;
		public string Id
		{ get { return _id; } set { _id = value; OnPropertyChanged("Id"); } }
		private bool _horizontalline;
		public bool HorizontalLine
		{ get { return _horizontalline; } set { _horizontalline = value; OnPropertyChanged("HorizontalLine"); } }
		private bool _verticalline;
		public bool VerticalLine
		{ get { return _verticalline; } set { _verticalline = value; OnPropertyChanged("VerticalLine"); } }
		private bool _bottomline;
		public bool BottomLine
		{ get { return _bottomline; } set { _bottomline = value; OnPropertyChanged("BottomLine"); } }
		public List<string> Excludes = new List<string>();
		public List<string> Unlocks = new List<string>();
        public List<string> Combines = new List<string>();
		public CustomListItem(string naam,string id, bool enabl, bool single,KeuzeList pa, string grpnm)
		{
			Naam = naam;
			Id = id;
			Beschikbaar = enabl;
			IsEnkel = single;
			BtnsNotInHeader = true;
			Papa = pa;
			GroupName = grpnm;
			SetBorders(false, false, false);
		}
		public void SetBorders(bool horizontal,bool vertical, bool secondofmulti)
		{
			HorizontalLine = horizontal;
			VerticalLine = vertical;
			
			if (secondofmulti)
			{
				Opbouw.Clear();
			}
		}
		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
	}
	public class OpbouwItem:INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private FontStyle _textstijl;
		public FontStyle TextStijl
		{ get { return _textstijl; } set { _textstijl = value; OnPropertyChanged("TextStijl"); } }
		private string _omschrijving;
		public string Omschrijving
		{ get { return _omschrijving; } set { _omschrijving = value; OnPropertyChanged("Omschrijving"); } }
		private string _prijs;
		public string Prijs
		{ get { return _prijs; } set { _prijs = value; OnPropertyChanged("Prijs"); } }
		private string _meters;
		public string Meters
		{ get { return _meters; } set { _meters = value; OnPropertyChanged("Meters"); } }

		public OpbouwItem(string oms, string prijs, string meters)
		{
			Omschrijving = oms;
			Prijs = prijs;
			Meters = meters;
		}

		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
	}
	public class VisibilityConverter : IValueConverter
	{
		public object Convert(
			object value,
			Type targetType,
			object parameter,
			CultureInfo culture)
		{
			bool visibility = (bool)value;
			return visibility ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(
			object value,
			Type targetType,
			object parameter,
			CultureInfo culture)
		{
			Visibility visibility = (Visibility)value;
			return (visibility == Visibility.Visible);
		}
	}
	public class InvVisibilityConverter : IValueConverter
	{
		public object Convert(
			object value,
			Type targetType,
			object parameter,
			CultureInfo culture)
		{
			bool visibility = !(bool)value;
			return visibility ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(
			object value,
			Type targetType,
			object parameter,
			CultureInfo culture)
		{
			Visibility visibility = (Visibility)value;
			return !(visibility == Visibility.Visible);
		}
	}
}

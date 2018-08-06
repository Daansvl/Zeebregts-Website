using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using eBrochure_zeebregts.KeuzeControls;

namespace eBrochure_zeebregts
{
	public partial class SubPage : UserControl,IBaseControl
	{
		public bool Matching = false;
		public SubCatkControl firstchoice;
		public SubPage()
		{
			InitializeComponent();
		}
		public String Titel { get { return SubpTitel.Text; } set { SubpTitel.Text = value; } }
		public void Addcontrol(UserControl subcont)
		{
			SubContPanel.Children.Clear();
			SubContPanel.Children.Add(subcont);
		}
		public void Clear4Submit()
		{ }
		public bool SubmitPressed()
		{
			bool retval = true;
			foreach (var x in SubContPanel.Children)
			{
				IBaseControl ibc = x as IBaseControl;
				if (ibc != null)
				{
					if (!ibc.SubmitPressed())
					{
						retval = false;
					}
				}
			}
			return retval; 
		}
		public void MatchProfiel(SubCatkControl cntrl, string matchvalue)
		{
			if (firstchoice == null)
			{
				firstchoice = cntrl;
			}
			if (firstchoice == cntrl)
			{
				Matching = true;
				foreach (SubCatkControl sck in SubContPanel.Children)
				{
					if (sck != cntrl)
					{
						foreach (WrapPanel wp in sck.SubCatPanel.Children.Where(x => x.GetType() == typeof(WrapPanel)))
						{
							foreach (ProductKControl pc in wp.Children.Where(x => x.GetType() == typeof(ProductKControl)))
							{
								if (pc.hoekprofiel.ProfielType != matchvalue)
								{
									pc.IsEnabled = false;
									if (pc.janeeRdBtn.IsChecked == true)
									{
										pc.janeeRdBtn.IsChecked = false;
									}
								}
								else
								{
									pc.IsEnabled = true;
								}
							}
						}
					}
				}
			}
			Matching = false;
		}
		public void MatchKeuze(SubCatkControl cntrl, string matchvalue, bool voegkleur)
		{
           
			Matching = true;
			switch (cntrl.HoofdCatType)
			{
				case MainCatType.normaal:
					foreach (SubCatkControl sck in SubContPanel.Children)
					{
						if (sck != cntrl)
						{
							switch (cntrl.sType)
							{
								case SubType.product:
									foreach (WrapPanel wp in sck.SubCatPanel.Children.Where(x => x.GetType() == typeof(WrapPanel)))
									{
										foreach (ProductKControl pc in wp.Children.Where(x => x.GetType() == typeof(ProductKControl)))
										{
											if (pc.product.Kleur == matchvalue)
											{
												pc.janeeRdBtn.IsChecked = true;
											}
										}
									}
									break;
								case SubType.nabewerking:
									
									break;
							}
						}
					}
					break;
				case MainCatType.accent:
					foreach(SubCatkControl sck in SubContPanel.Children)
					{
						if (sck != cntrl)
						{
							if (sck.hoofdcatnummer == cntrl.hoofdcatnummer)
							{
								//geen accent
								foreach (WrapPanel wp in sck.SubCatPanel.Children.Where(x => x.GetType() == typeof(WrapPanel)))
								{
									var noselct = wp.Children.Where(x => x.GetType() == typeof(ProductKControl) && (x as ProductKControl).product.ID == "0").FirstOrDefault() as ProductKControl;
									noselct.janeeRdBtn.IsChecked = true;
									
								}
							}
						}
					}
					foreach (WrapPanel wp in cntrl.SubCatPanel.Children.Where(x => x.GetType() == typeof(WrapPanel)))
					{
						foreach (ProductKControl pc in wp.Children.Where(x => x.GetType() == typeof(ProductKControl)))
						{
							if (pc.product.Kleur == matchvalue)
							{
								pc.janeeRdBtn.IsChecked = true;
							}
						}

					}
					break;
				case MainCatType.verwerking:
                  	foreach (SubCatkControl sck in SubContPanel.Children.Where(x => x.GetType() == typeof(SubCatkControl)))
					{
						if (sck != cntrl)
						{
							if (sck.hoofdcatnummer == cntrl.hoofdcatnummer || voegkleur )
							{
								foreach (KeuzeList kl in sck.SubCatPanel.Children.Where(x => x.GetType() == typeof(KeuzeList)))
								{
									var tomatch = kl.ListBox.Items.Where(I => (I as CustomListItem).Id == matchvalue).FirstOrDefault() as CustomListItem;
									if (tomatch != null)
									{
                                       if (tomatch.Gekozen == false)
										{
											tomatch.Gekozen = true;
										}
									}
								}
							}
						}
						
					}
					break;
			}
			Matching = false;
		}
       
		public void AccentGuard(string hcatnr, bool rdonly)
		{
			Dictionary<SubCatkControl,bool> GeenAcc = new Dictionary<SubCatkControl,bool>();
			foreach (SubCatkControl sckc in SubContPanel.Children.Where(x=>x.GetType() == typeof(SubCatkControl)&& (x as SubCatkControl).hoofdcatnummer == hcatnr))
			{
				if (sckc.AccentJaNee.IsChecked == true)
				{
					GeenAcc.Add(sckc,true);
				}
				else
				{
					GeenAcc.Add(sckc,false);
				}
			}
			int cntr = 0;
			foreach (bool b in GeenAcc.Values)
			{
				if (b)
				{
					cntr++;
				}
			}
			bool AllOpen = false;
			if (cntr == GeenAcc.Count)
			{
				AllOpen = true;
			}
			if (!rdonly)
			{
				foreach (SubCatkControl sckc in SubContPanel.Children.Where(x => x.GetType() == typeof(SubCatkControl) && (x as SubCatkControl).hoofdcatnummer == hcatnr))
				{
					foreach (WrapPanel wp in sckc.SubCatPanel.Children.Where(x => x.GetType() == typeof(WrapPanel)))
					{
						foreach (ProductKControl pc in wp.Children.Where(x => x.GetType() == typeof(ProductKControl)))
						{
							if (AllOpen || GeenAcc[sckc] == false)
							{
								pc.IsEnabled = true;
							}
							else
							{
								pc.IsEnabled = false;
							}
						}
					}
				}
			}
		}
		public void WijzigPressed()
		{
			Acumulator.Instance().BB.InvalidateNaWijzig();
			foreach (IBaseControl BC in SubContPanel.Children)
			{
				BC.WijzigPressed();
			}
		}
		public void setInfoBtn(DetailsWindow dw)
		{
			//InfoBtn.Tag = dw;
		}

		private void InfoBtn_Click(object sender, RoutedEventArgs e)
		{
			if ((sender as Button).Tag != null)
			{
				((sender as Button).Tag as DetailsWindow).Show();
			}
		}
	}
}

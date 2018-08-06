using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.ObjectModel;
using eBrochure_zeebregts.Classes;

namespace eBrochure_zeebregts.KeuzeControls
{
	public partial class Samevatlijst : UserControl,IBaseControl
	{
		private ObservableCollection<SumaryItem> Upgrades;
		private ObservableCollection<SumaryItem> Basis;
		public Samevatlijst()
		{
			InitializeComponent();
		}
		public bool SubmitPressed()
		{
			return true;
		}
		public void Clear4Submit()
		{
		}
		public void WijzigPressed()
		{
			this.IsEnabled = true;
		}
		public void Initlist(Product basis, string basisMeters, Dictionary<Product, string> upgrades)
		{
			Basis = null;
			Basis = new ObservableCollection<SumaryItem>();
			Basis.Add(MakeSumaryItem(basis, basisMeters));
			BasisList.ItemsSource = Basis;
			Upgrades = null;
			Upgrades = MakeUpgrades(upgrades);
			UpgradeList.ItemsSource = Upgrades;
			
			if (!basisMeters.StartsWith("0m"))
			{
				ActieTB.Text = "gecombineerd met:";
			}
			else
			{
				ActieTB.Text = "vervangen door:";
			}
		}
		private SumaryItem MakeSumaryItem(Product p,string m)
		{
			var pinfo = new List<string>();
			pinfo.Add(p.productcode.TrimStart(' ') + " " + p.Breedte.ToString().Substring(0, 2) + "x" + p.Lengte.ToString().Substring(0, 2) + "cm");
			var soort = (from sc in Acumulator.Instance().ctx.SubCats
						join scp in Acumulator.Instance().ctx.SubCatPerPakkets on sc.SCB_ID equals scp.SCB_NR
						where scp.SCBP_ID == p.LinkedSubCat
						select sc.Omschrijving).FirstOrDefault();
			pinfo.Add(soort);
			pinfo.Add(p.Kleur.TrimStart(' '));
			var si = new SumaryItem(m, pinfo);

			return si;

		}
		private ObservableCollection<SumaryItem> MakeUpgrades(Dictionary<Product, string> upg)
		{
			ObservableCollection<SumaryItem> col = new ObservableCollection<SumaryItem>();
			foreach (var p in upg.Keys)
			{
				col.Add(MakeSumaryItem(p,upg[p]));
			}
			return col;
		}
		
	}
	public class SumaryItem:INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private string _meters;
		private Visibility _mvis;
		public Visibility Mvis
		{get{return _mvis;}set{_mvis=value; OnPropertyChanged("Mvis");}}
		public string Meters
		{ get { return _meters; } set { _meters = value; OnPropertyChanged("Meters"); if (_meters.StartsWith("0m")) { Mvis = Visibility.Collapsed; } else { Mvis = Visibility.Visible; } } }
		private List<string> _product;
		public List<string> Product
		{ get { return _product; } set { _product = value; OnPropertyChanged("Product"); } }

		public SumaryItem(string meters, List<String> prodinfo)
		{
			Meters = meters;
			Product = prodinfo;
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
}

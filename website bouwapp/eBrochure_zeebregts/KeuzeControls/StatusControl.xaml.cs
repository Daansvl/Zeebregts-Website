using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using eBrochure_zeebregts.Classes;
using System.Collections.Generic;
using eBrochure_zeebregts.ExpertControls;

namespace eBrochure_zeebregts.KeuzeControls
{
	public partial class StatusControl : UserControl,IBaseControl
	{
		public StatusControl()
		{
			InitializeComponent();
		}
		private string _path;
        private bool empty = true;
		private string ruimteID;

        public ExpertReceipt Bon
        {
            get;
            set;
        }
		public void LoadImg(string path)
		{
            empty = false;
			_path = path;
            if (path != null)
            {
                if (path.StartsWith(@"/"))
                {
                    path = path.TrimStart('/');
                }
                string apath = "http://mybouwapp.nl/Images/Blueprints/" + path;
                var u = new Uri(apath, UriKind.Absolute);
            }
			//miniblueprint.Source = new BitmapImage(u);
		}
		public void SetRuimteID(string r_id) {
			empty = false;
			ruimteID = r_id;
		}
		public void Setprijs(string prijs)
		{
            empty = false;
            totaalprijs.Text = prijs;
            totaalprijs.Visibility = Visibility.Visible;
		}
		public void UpdateInfo()
		{
            empty = false;
			if (this.Visibility != Visibility.Visible)
			{
				this.Visibility = Visibility.Visible;
			}
            Bon = MakeBon();

		}
        private Dictionary<string, double> HeleDozenLijst = new Dictionary<string, double>();
        private Dictionary<String, int> instapChecklist = new Dictionary<string, int>();
        private void GetInstapPrijzen()
        {
            HeleDozenLijst.Clear();
            instapChecklist.Clear();
            foreach (Ruimte r in Acumulator.Instance().OTracker.offerteRuimte_.Children)
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
        }

      
        private ExpertReceipt MakeBon()
		{
            var ctx = Acumulator.Instance().ctx;
			var tmpBon = new ExpertReceipt();
			if (Acumulator.Instance().OTracker != null && Acumulator.Instance().OTracker.offerteRuimte_ != null)
			{
                GetInstapPrijzen();
				tmpBon.Clear();
				var bouwnr = (from b in Acumulator.Instance().ctx.Bouwnummers
							 where b.B_ID == Acumulator.Instance().Bouwnr
							 select b.Omschrijving).FirstOrDefault();
				tmpBon.AddArticle(bouwnr,-1,true);
				foreach (Ruimte r in Acumulator.Instance().OTracker.offerteRuimte_.Children)
				{

					tmpBon.AddArticle(r.Omschrijving, -1, true);
					if (r.GekozenPakket != null)
					{
						tmpBon.AddArticle("Pakket: " + r.GekozenPakket.Omschrijving, r.GekozenPakket.PrijsHuidig, false);
					}
					foreach (OptieKeuze op in r.GekozenOpties)
					{
						op.laad_verder();
                        var dorpelcat = (from dc in Acumulator.Instance().ctx.SubCatPerRuimteDeels
                                         where dc.R_NR == op.OptieID && dc.SCB_NR == "SCB13"
                                         select dc.R_NR).FirstOrDefault();
                       
						var prijs = 0.0;
						if (r.GekozenPakket != null)
						{
							prijs = op.getMeerprijs(Acumulator.Instance().ctx,(r.GekozenPakket as PakketKeuze).PakketPrijsgroep_NR);
						}
						else
						{
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
                            prijs = optiebasisprijs;
						}
						tmpBon.AddArticle(op.Omschrijving,prijs , false);
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
                                        tmpBon.AddArticle("Meerprijs Dorpel", double.Parse(parts[1]), false);
                                    }
                                }
                            }
                        }
					}
                    foreach (var exmtrs in r.AanvullendeMeters)
                    {
                        if (!exmtrs.IsBasis)
                        {
                            var accText = exmtrs.LinkedProduct !=null ?  exmtrs.LinkedProduct.Omschrijving : exmtrs.LinkedHoekProf != null ? exmtrs.LinkedHoekProf.Omschrijving : "";// + ": " + exmtrs.Meters + " meter voor €" + exmtrs.Meerprijs.ToString() + " per meter";
                            var accPrijs = exmtrs.Meters * exmtrs.Meerprijs;// +(exmtrs.LinkedProduct != null ? exmtrs.LinkedProduct.VerpakkingsToeslag : exmtrs.LinkedHoekProf != null ? exmtrs.LinkedHoekProf.VerpakkingsToeslag : 0.0);//0.0;
                            tmpBon.AddArticle(accText, accPrijs, false);
                        }
                            if(exmtrs.LinkedProduct != null)
                            {
                              ExpertMethods.instapPrijsCalc(exmtrs.LinkedProduct.ID, exmtrs.LinkedProduct.Omschrijving, instapChecklist, exmtrs.LinkedProduct.VerpakkingsToeslag,HeleDozenLijst) ;
                            }
                            else if (exmtrs.LinkedHoekProf != null)
                            {
                                ExpertMethods.instapPrijsCalc(exmtrs.LinkedHoekProf.ProfielID, exmtrs.LinkedHoekProf.Omschrijving, instapChecklist, exmtrs.LinkedHoekProf.VerpakkingsToeslag,HeleDozenLijst);
                            }
                    }

					tmpBon.AddArticle("", -1, true);
				}
                var oruim = Acumulator.Instance().OTracker.offerteRuimte_;

                //instap prijzen aka afname hele dozen
                tmpBon.AddArticle("Afname hele dozen",-1, true);

                double dozenprijs = 0.0;
                foreach (var hd in HeleDozenLijst)
                {
                    tmpBon.AddArticle(hd.Key, hd.Value, false);
                    dozenprijs += hd.Value;
                }


                //if(oruim.Korting != null && oruim.Korting.KortingBedrag > 0.0)
                //{
                //    var kort = oruim.Korting.KortingBedrag * -1;
                //    tmpBon.AddArticle("Korting", kort,true );
                //}

				this.Setprijs(tmpBon.GetTotal());
			}
			return tmpBon;
		}
		private void showBonBtn_Click(object sender, RoutedEventArgs e)
		{
            if (!empty)
            {
                var bigbon = new DetailsWindow();
                Bon = MakeBon();
                bigbon.Width = Bon.Width + 25;
                bigbon.Height = Bon.Height + 25;
                bigbon.LoadContent(Bon);
                bigbon.Show();
            }
		}

		private void BlueprintBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!empty)
            {
                var bigblue = new DetailsWindow();
                bigblue.Width = 725;
                bigblue.Height = 525;

				if (ruimteID == null) {
				}
				else {
					var blue = new BluePrintControl();
                    var data = Acumulator.Instance().bluePrintManager.getBlueprintData(ruimteID, false);
					if (data != null)
						blue.LoadImg(data);
					bigblue.LoadContent(blue);
				}
				bigblue.Show();

				/*
                if (_path == null)
                {
                    foreach (var bytes in Acumulator.Instance().Blueprints.Values)
                    {
                        var blue = new BluePrintControl();
                        blue.LoadImg(bytes);
                        bigblue.LoadContent(blue);
                    }
                }
                else
                {
                    var blue = new BluePrintControl();
                    if (Acumulator.Instance().Blueprints.ContainsKey(_path))
                    {
                        blue.LoadImg(Acumulator.Instance().Blueprints[_path]);
                    }
                    else
                    {
                        //blue.LoadImg(_path);
                    }
                    bigblue.LoadContent(blue);
                }
                bigblue.Show();*/
            }
		}
        
		public void Clear4Submit()
		{
			//miniblueprint.Source = null;
            _path = null;
            empty = true;
			totaalprijs.Text = "€0,00";
            //showBonBtn.Content = "";
		}
		public bool SubmitPressed()
		{
			return true;
		}
		public void WijzigPressed()
		{
		}
	}
}

using System.Linq;
using System.Net;
using System;
using System.IO;
using System.Collections.Generic;

namespace eBrochure_zeebregts.Classes
{
	public class OfferteTracker : IOfferteTracker
	{
		//int stagecntr = 0;
	    public bool loaded = false;
		private void InitData()
		{
			var ctx = Acumulator.Instance().ctx;
            try
            {
                Acumulator.Instance().BusyBee.IsBusy = true;
                Acumulator.Instance().BusyBee.BusyContent = "Laden initialiseren...";
                ctx.Load(ctx.GetHoofdCategorieënQuery()).Completed += (sender00, args00) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 1%";
                ctx.Load(ctx.GetHoekProfielTypesQuery()).Completed += (sender01, args01) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 2%";
                ctx.Load(ctx.GetBouwnummersQuery()).Completed += (sender0, args0) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 3%";
                ctx.Load(ctx.GetOptieProductCombosQuery()).Completed += (sender02, args02) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 4%";
                ctx.Load(ctx.GetSubCatPerRuimteDeelQuery()).Completed += (sender1, args1) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 5%";
                ctx.Load(ctx.GetKleurHoekKleurCombinatiesQuery()).Completed += (sender03, args03) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 6%";
                ctx.Load(ctx.GetKleurCombinatieSetOpbouwQuery()).Completed += (sender04, args04) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 7%";
                ctx.Load(ctx.GetOpgeslagenOfferteQuery(Acumulator.Instance().Bouwnr)).Completed += (sender05, args05) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 8%";
                ctx.Load(ctx.GetMeerPrijzenRuimteOptieQuery()).Completed += (sender2, args2) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens Laden 9%";
                ctx.Load(ctx.GetVerplichteCombinatieOpbouwQuery()).Completed += (sender07, args07) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 12%";
                ctx.Load(ctx.GetSubCatPerPakketQuery()).Completed += (sender3, args3) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 16%";
                ctx.Load(ctx.GetRuimteOpbouwQuery()).Completed += (sender4, args4) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 20%";
                ctx.Load(ctx.GetRuimteDelenQuery()).Completed += (sender5, args5) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 24%";
                ctx.Load(ctx.GetStandaardOnafhandelijkeRuimteOpbouwQuery()).Completed += (sender6, args6) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 28%";
                ctx.Load(ctx.GetStandaardRuimteOpbouwQuery()).Completed += (sender7, args7) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 32%";
                ctx.Load(ctx.GetProductSetsQuery()).Completed += (sender8, args8) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 36%";
                ctx.Load(ctx.GetProductSetOpbouwQuery()).Completed += (sender9, args9) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 40%";
                ctx.Load(ctx.GetProductenQuery()).Completed += (sender10, args10) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 44%";
                ctx.Load(ctx.GetProductCategoriënQuery()).Completed += (sender11, args11) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 48%";
                ctx.Load(ctx.GetPakketGroepOpbouwQuery()).Completed += (sender12, args12) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 52%";
                ctx.Load(ctx.GetPakkettenQuery()).Completed += (sender13, args13) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 56%";
                ctx.Load(ctx.GetSubCatPerRuimteDeelQuery()).Completed += (sender14, args14) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 60%";
                ctx.Load(ctx.GetSubCatsQuery()).Completed += (sender15, args15) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 64%";
                ctx.Load(ctx.GetRuimtesPerTypeQuery()).Completed += (sender16, args16) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 68%";
                ctx.Load(ctx.GetBron_RuimtesQuery()).Completed += (sender17, args17) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 72%";
                ctx.Load(ctx.GetOngeldigeCombinatieOpbouwQuery()).Completed += (sender18, args18) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 76%";
                ctx.Load(ctx.GetMeervoudigeCombinatiesOpbouwQuery()).Completed += (sender19, args19) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 80%";
                ctx.Load(ctx.GetKleurenQuery()).Completed += (sender20, args20) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 84%";
                ctx.Load(ctx.GetBouwTypesQuery()).Completed += (sender21, args21) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 88%";
                ctx.Load(ctx.GetCategorieënQuery()).Completed += (sender22, args22) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 92%";
                ctx.Load(ctx.GetNabewerkingSetCombinatiesQuery()).Completed += (sender23, args23) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 96%";
                ctx.Load(ctx.GetNabewerkingSetOpbouwQuery()).Completed += (sender24, args24) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 99%";
                ctx.Load(ctx.GetNabewerkingenQuery()).Completed += (sender25, args25) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 99,1%";
                ctx.Load(ctx.GetNabewerkingSetsQuery()).Completed += (sender26, args26) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 99,2%";
                ctx.Load(ctx.GetPRojectQuery()).Completed += (sender27, args27) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 99,3%";
                ctx.Load(ctx.GetFaseQuery()).Completed += (sender28, args28) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 99,4%";
                ctx.Load(ctx.GetMerknamenQuery()).Completed += (sender29, args29) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 99,5%";
                ctx.Load(ctx.GetDorpelOptieCombosQuery()).Completed += (sender30, args30) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 99,6%";
                ctx.Load(ctx.GetRuimteSQuery()).Completed += (sender31, args31) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 99,7%";
                ctx.Load(ctx.GetMeterPrijsPerProjectsQuery()).Completed += (sender32, args32) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 99,8%";
                ctx.Load(ctx.GetInstapPrijzenQuery(Acumulator.Instance().Projectnr)).Completed += (sender33,args33)=>
                {
                ctx.Load(ctx.GetAccentSetOpbouwQuery()).Completed += (sender99,args99) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 99,9%";
                ctx.Load(ctx.GetVrijAccentDataQuery(Acumulator.Instance().Projectnr)).Completed += (sender34,args34) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 99,91%";
                ctx.Load(ctx.GetAccentProjectCombinatiesQuery()).Completed += (sender35,args35) =>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 99,93%";
                ctx.Load(ctx.GetBouwnummerOptieGroepenQuery()).Completed += (sender37,args37)=>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 99,94%";
                ctx.Load(ctx.GetBouwnummerOptieGroepOpbouwQuery()).Completed += (sender38,args38)=>
                {
                Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 99,94%";
                ctx.Load(ctx.GetBouwnummerOptieFilterCombinateiesQuery()).Completed += (sender39,args39)=>
                {
                    Acumulator.Instance().BusyBee.BusyContent = "Gegevens laden 100%";
                    Acumulator.Instance().SetFase();
                    ctx.GetServerTime(opp => { saveDateDif(opp.Value); }, null);
                    BouwOfferte();
                    Acumulator.Instance().BusyBee.IsBusy = false;
                    Acumulator.Instance().BB.load_tree();
                    if (loaded)
                    {
                        Acumulator.Instance().BB.CompleteAfterLoad(null);
                        Acumulator.Instance().BB.ShowHideAllMarks(false, null);
                        LogHelper.SendLog("Save Loaded", LogType.activity);
                    }
                    Acumulator.Instance().bluePrintManager.GetAllBlueprints();
                    //GetBlueprints();
                };};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};};
            }
            catch (Exception exp)
            {
                Acumulator.Instance().BusyBee.IsBusy = false;
                // Acumulator.Instance().StartPagina.RecoverError();

            }
		}
		protected override IOfferte BouwOfferte()
		{
			Acumulator.Instance().BusyBee.BusyContent = "Opbouwen OfferteBoom - Basis";
			Acumulator.Instance().BusyBee.IsBusy = true;
			offerteBasis_ = new BaseOfferte();
			Acumulator.Instance().BusyBee.BusyContent = "Opbouwen OfferteBoom - Kavel";
			offerteKavel_ = new KavelOfferte();
			offerteBasis_.Add(offerteKavel_);
			//load existing save
			var ctx = Acumulator.Instance().ctx;
			RuimteOfferte Loaded_ro = null;
			if (ctx.OpgeslagenOffertes != null)
			{
				var maxdate = (from oo in ctx.OpgeslagenOffertes
							   where oo.Bouwnummer_NR == Acumulator.Instance().Bouwnr
							   select oo.timestamp).Max();
				LogHelper.SendLog("Save Found", LogType.status);
				var ofxml = (from oo in ctx.OpgeslagenOffertes
							 where oo.Bouwnummer_NR == Acumulator.Instance().Bouwnr && oo.timestamp == maxdate
							 select oo.Xml_Value).FirstOrDefault();
				if (ofxml != null)
				{
                    Acumulator.Instance().SavedXml = ofxml;
					SaveLoadXML slx = new SaveLoadXML();
					Loaded_ro = slx.LoadOfferte(ofxml);
					Acumulator.Instance().OfferteDatum = (System.DateTime)maxdate;
				}
				
				
			}
			Acumulator.Instance().BusyBee.BusyContent = "Opbouwen OfferteBoom - Ruimtes";
			if (Loaded_ro != null)
			{
				LogHelper.SendLog("Xml Parse Succes", LogType.status);
				loaded = true;

				//offerteRuimte_ = Loaded_ro;
                IntegrateSave(Loaded_ro);

			}
			else
			{
				offerteRuimte_ = new RuimteOfferte();
			}
			offerteKavel_.Add(offerteRuimte_);
			Acumulator.Instance().BusyBee.IsBusy = false;
			return offerteBasis_;
		}
        private void IntegrateSave(RuimteOfferte save)
        {
            offerteRuimte_ = new RuimteOfferte();
            if (save != null)
            {
                foreach (var r in offerteRuimte_.Children)
                {
                    var ruimte = r as Ruimte;
                    if (ruimte != null)
                    {
                        var saved_ruimte = save.Children.Where(x => (x as Ruimte).RuimteID == ruimte.RuimteID).FirstOrDefault() as Ruimte;
                        if (saved_ruimte != null)
                        {
                            ruimte.GekozenAccenten = CheckProductsExists(saved_ruimte.GekozenAccenten, save.LoadedCompleet);
                            ruimte.GekozenBewerkingen = CheckBewerkingenExists(saved_ruimte.GekozenBewerkingen, save.LoadedCompleet);
                            ruimte.GekozenOpties = CheckOptiesExists(saved_ruimte.GekozenOpties, save.LoadedCompleet);
                            ruimte.GekozenPakket = CheckPakkettenExists(saved_ruimte.GekozenPakket, save.LoadedCompleet);
                            ruimte.PakketOud = ruimte.GekozenPakket;
                            ruimte.GekozenProfielen = CheckProfielenExists(saved_ruimte.GekozenProfielen, save.LoadedCompleet);
                            ruimte.GekozenTegels = CheckProductsExists(saved_ruimte.GekozenTegels, save.LoadedCompleet);
                            ruimte.AanvullendeMeters = CheckMetersExists(saved_ruimte.AanvullendeMeters, save.LoadedCompleet);
                        }
                    }
                }
                offerteRuimte_.Korting = save.Korting;
                if (save.LoadedCompleet != null)
                {
                    offerteRuimte_.LoadedCompleet = save.LoadedCompleet;
                }
            }
        }
        private List<Product> CheckProductsExists(List<Product> save_prods, List<string> loadcompl)
        {
            var ctx = Acumulator.Instance().ctx;
            if(save_prods == null)
            {
                return new List<Product>();
            }
            var savedIDs = save_prods.Select(x => x.ID).ToList();
            
            var validProds = (from p in ctx.Productens
                              where savedIDs.Contains(p.PD_ID)
                              select p.PD_ID).ToList();
            if (validProds != null)
            {
                if (validProds.Distinct().Count() != savedIDs.Distinct().Count())
                {
                    var todel = new Queue<Product>();
                    foreach (var prod in save_prods)
                    {
                        if (!validProds.Contains(prod.ID))
                        {
                            //notify save not complete
                            loadcompl.Clear();
                            todel.Enqueue(prod);
                        }
                    }
                    while (todel.Count > 0)
                    {
                        save_prods.Remove(todel.Dequeue());
                    }
                }
                return save_prods;
            }
            else
            {//notify save not complete
                loadcompl.Clear();
                return new List<Product>();
            }
            
        }
        private List<Nabewerking> CheckBewerkingenExists(List<Nabewerking> save_nabew, List<string> loadcompl)
        {
            var ctx = Acumulator.Instance().ctx;
            if(save_nabew == null)
            {
                return new List<Nabewerking>();
            }
            var saveIDs = save_nabew.Select(x => x.Nabewerking_ID).ToList();

            var validNabew = (from nb in ctx.Nabewerkingens
                              where saveIDs.Contains(nb.N_ID)
                              select nb.N_ID).ToList();

            if (validNabew != null)
            {

                if (validNabew != null && validNabew.Distinct().Count() != saveIDs.Distinct().Count())
                {
                    var todel = new Queue<Nabewerking>();
                    foreach (var nabew in save_nabew)
                    {
                        if (!validNabew.Contains(nabew.Nabewerking_ID))
                        {
                            //notify save not complete
                            loadcompl.Clear();
                            todel.Enqueue(nabew);
                        }
                    }
                    while (todel.Count > 0)
                    {
                        save_nabew.Remove(todel.Dequeue());
                    }
                }
                return save_nabew;
            }
            else
            {
                //notify save not complete
                loadcompl.Clear();
                return new List<Nabewerking>();
            }

        }
        private List<OptieKeuze> CheckOptiesExists(List<OptieKeuze> save_opties, List<string> loadcompl)
        {
            var ctx = Acumulator.Instance().ctx;
            if(save_opties == null)
            {
                return new List<OptieKeuze>();
            }
            var saveIDs = save_opties.Select(x => x.OptieID).ToList();

            var validOpt = (from opt in ctx.RuimteDelens
                            where saveIDs.Contains(opt.R_ID)
                            select opt.R_ID).ToList();
            if (validOpt != null)
            {
                if (validOpt != null && validOpt.Distinct().Count() != saveIDs.Distinct().Count())
                {
                    var todel = new Queue<OptieKeuze>();
                    foreach (var opt in save_opties)
                    {
                        if (!validOpt.Contains(opt.OptieID))
                        {
                            //notify save not complete
                            loadcompl.Clear();
                            todel.Enqueue(opt);
                        }
                    }
                    while (todel.Count > 0)
                    {
                        save_opties.Remove(todel.Dequeue());
                    }
                }

                return save_opties;
            }
            else
            {
                //notify save not complete
                loadcompl.Clear();
                return new List<OptieKeuze>();
            }
        }
        private PakketKeuze CheckPakkettenExists(PakketKeuze save_pakketn, List<string> loadcompl)
        {
            var ctx = Acumulator.Instance().ctx;
            if (save_pakketn != null)
            {
                var validpak = (from pak in ctx.Pakkettens
                                where save_pakketn.Pakket_ID == pak.P_ID
                                select pak.P_ID).ToList();
                if (validpak != null)
                {
                    return save_pakketn;
                }
                else
                {
                    //notify save not complete
                    loadcompl.Clear();
                    return null;
                }
            }
            else
            {
                return null;
            }

        }
        private List<HoekProfiel> CheckProfielenExists(List<HoekProfiel> save_profielen, List<string> loadcompl)
        {
            var ctx = Acumulator.Instance().ctx;
            if(save_profielen == null)
            {
                return new List<HoekProfiel>();
            }
            var saveIDs = save_profielen.Select(x => x.ProfielID).ToList();

            var validProf = (from prof in ctx.Productens
                             where saveIDs.Contains(prof.PD_ID)
                             select prof.PD_ID).ToList();

            if (validProf != null)
            {
                if (validProf.Distinct().Count() != saveIDs.Distinct().Count())
                {
                    var todel = new Queue<HoekProfiel>();
                    foreach (var prof in save_profielen)
                    {
                        if (!validProf.Contains(prof.ProfielID))
                        {
                            //notify save not complete
                            loadcompl.Clear();
                            todel.Enqueue(prof);
                        }
                    }
                    while (todel.Count > 0)
                    {
                        save_profielen.Remove(todel.Dequeue());
                    }
                }

                return save_profielen;
            }
            else
            {
                //notify save not complete
                loadcompl.Clear();
                return new List<HoekProfiel>();
            }
        }
        private List<ExtraMeters> CheckMetersExists(List<ExtraMeters> save_aanvMeters, List<string> loadcompl)
        {
            var ctx = Acumulator.Instance().ctx;
            if (save_aanvMeters == null)
            {
                return new List<ExtraMeters>();
            }
            var saveIDs = save_aanvMeters.Select(x =>x.LinkedProduct != null ? x.LinkedProduct.ID :  x.LinkedHoekProf.ProfielID).ToList();
            var validProds = (from prods in ctx.Productens
                              where saveIDs.Contains(prods.PD_ID)
                              select prods.PD_ID).ToList();
            if (validProds != null)
            {
                if (validProds.Distinct().Count() != saveIDs.Distinct().Count())
                {
                    var todel = new Queue<ExtraMeters>();
                    foreach (var prod in save_aanvMeters)
                    {
                        if (!validProds.Contains(prod.LinkedProduct.ID))
                        {
                            loadcompl.Clear();
                            todel.Enqueue(prod);
                        }
                    }
                    while (todel.Count > 0)
                    {
                        save_aanvMeters.Remove(todel.Dequeue());
                    }
                }
                return save_aanvMeters;
            }
            else
            {
                loadcompl.Clear();
                return new List<ExtraMeters>();
            }
        }

        private void saveDateDif(DateTime dt)
        {
            var dateDiff = dt - DateTime.Now;
            Acumulator.Instance().serverTimeDiff = dateDiff;
        }
       /* private void GetBlueprints()
        {
         //   Acumulator.Instance().Blueprints.Clear();
            foreach (Ruimte r in this.offerteRuimte_.Children)
            {
                string path = Acumulator.Instance().GetPathGespiegeld(r.RuimteID);
               
                if (path != null && path != "")
                {
                    string apath = "https://mybouwapp.nl/Images/Blueprints/" + path;
                  

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apath);
                    
                    request.UseDefaultCredentials = true;
                    Acumulator.Instance().updateDownActive(r.RuimteID, true);
                    request.BeginGetResponse(new AsyncCallback(RequestReady), request);

                }
            }
        }*/
        //private bool lockacumu = false;
        /*private void RequestReady(IAsyncResult asyncResult)
        {
            var req = asyncResult.AsyncState as HttpWebRequest;
            try
            {
                using (WebResponse wrs = req.EndGetResponse(asyncResult))
                {
                    var path = wrs.ResponseUri.ToString().Replace("https://mybouwapp.nl/Images/Blueprints/", "");
                    var foo = wrs.GetResponseStream();

                    var bar = foo.CloneToMemoryStream();
                    var foobar = bar.ToArray();
                    while (lockacumu)
                    { }
                    lockacumu = true;
                  
                    Acumulator.Instance().BlueprintRequestReady(path, foobar, null);
                    lockacumu = false;
                }
            }
            catch (Exception e)
            {
                var err = e;
                var emsg = e.Message;
            }
        }
*/
		public override void NieuwOfferte()
		{
			Acumulator.Instance().OTracker = this;
			offerteKavel_ = null;
			offerteBasis_ = null;
			offerteRuimte_ = null;
			InitData();
		}
	}
}

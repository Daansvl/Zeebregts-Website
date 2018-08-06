using System;
using System.Windows.Controls;
using eBrochure_zeebregts.Web.Services;
using eBrochure_zeebregts.KeuzeControls;
using eBrochure_zeebregts.Helpers;
using System.Windows.Media.Imaging;
using eBrochure_zeebregts.Classes;
using System.Linq;
using System.Collections.Generic;
using PdfSharp.Pdf;


namespace eBrochure_zeebregts
{
	public enum OptieType { Unknown, Determinating, Resulting, Independant, Detail }
	public sealed class Acumulator
	{
		const double indexering = 1.02;
		const double opslag = 1.222222222222222;
		const double btw = 1.21;
        public StartPagina StartPagina;
        private BluePrintManager _bluePrintManager = new BluePrintManager();
        public BluePrintManager bluePrintManager
        { get { return _bluePrintManager; } }
        public string MaakStartZin(string zin)
        {


            var l_zin = zin.ToLower();
            var ar_zin = l_zin.ToCharArray();
            var H_letter = ar_zin[0].ToString().ToUpper();
            ar_zin[0] = H_letter.ToCharArray()[0];

            var Brands = (from b in ctx.Merknamens
                          select b.Merknaam).ToList();
            string CapZin = new string(ar_zin);
            var words = CapZin.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                foreach (string b in Brands)
                {
                    if (b.Length == words[i].Length)
                    {
                        var matchcntr = 0;
                        for (int p = 0; p < b.Length; p++)
                        {
                            if (b[p] == words[i][p])
                            { matchcntr++; }
                        }
                        if (matchcntr >= b.Length - 1)
                        {
                            words[i] = b;
                        }
                    }
                }
            }
            var result = "";
            for (int q = 0; q < words.Length; q++)
            {
                result += words[q] + " ";
            }
            result.TrimEnd(' ');

            if (result.Length >120)
            {
                var length = result.Length;
                var subsend = result.Substring(length / 2);
                var substart = result.Substring(0, length / 2);
                var indx = subsend.IndexOf(' ');
                subsend = subsend.Insert(indx, Environment.NewLine);
                result = substart + subsend;

            }

            return result;
        }
		public double BerekenEindPrijs(double basisprijs)
		{
			double result = basisprijs;
			if (ProjFase != null)
			{
				double foo = basisprijs * ProjFase.Indexering * ProjFase.Opslag * ProjFase.BTW;
				double bar = (foo / ProjFase.AfrondingBoven) + 0.5;
				double foobar = Math.Round(bar, 0);
				result = foobar * ProjFase.AfrondingBoven;
				result = Math.Round(result, 2);
			}
			else
			{
				double foo = basisprijs * indexering * opslag * btw;
				double bar = (foo / 0.05) + 0.5;
				double foobar = Math.Round(bar, 0);
				result = foobar * 0.05;
				result = Math.Round(result, 2);
			}

            if (!ProjFase.ToonNegatievePrijzen)
            {
                if (result < 0)
                {
                    result = 0;
                }
            }
			return result;
			//=AFRONDEN(O31*(1+ind)*ops*(1+btwnew)/0,05+0,5;0)*0,05
		}
		public ProjectFase ProjFase;
		public void SetFase()
		{
			if (Bouwnr != null && Bouwnr != "")
			{
				ProjectFase pf = new ProjectFase(Bouwnr, ctx);
                ProjFase = pf;
			}
		}
		public DateTime OfferteDatum
		{ get; set; }
        public DateTime PrevPrintDatum
        {
            get;
            set;
        }
        public StatusControl InfoBar
		{ get; set; }
		public BusyIndicator BusyBee
		{
			get;
			set;
		}
		public eBrochureDomainContext ctx
		{
			get;set;
		}
		public string GebruikersNaam
		{ get; set; }
        private string _savexml;
        public string SavedXml
        { get { return _savexml; } set { _savexml = value; } }
        public OpgeslagenOfferteLean oOL;
        public TimeSpan serverTimeDiff;
		private static Acumulator _instance
		{ get; set; }
		public string Projectnr
		{ get; set; }
        private string _bouwnr;
        public string Bouwnr
        { get { return _bouwnr; } set { _bouwnr = value; var prnr = (from pr in ctx.Bouwnummers where pr.B_ID == value select pr.PR_NR).FirstOrDefault(); if (prnr != null) { Projectnr = prnr; } } }
		public TvNode SelectedTVItem
		{
			get;
			set;
		}
		public string Pakket
		{ get; set; }
		public string Type
		{ get; set; }
		public IOfferteTracker OTracker
		{
			get;
			set;
		}
        public UitvoerPlaceHolder uitvoerPlaceHolder
        { get; set; }
		public BrochureBasis BB
		{ get; set; }
		public LoggedInUser HuidigGebruiker;
    /*
		private Dictionary<string, byte[]> _blueprints = new Dictionary<string, byte[]>();
        public Dictionary<string,byte[]> Blueprints
        {
            get
            {
                if (_blueprints == null)
                {
                    _blueprints = new Dictionary<string, byte[]>();
                }
                return _blueprints;
            }
            set
            {
                _blueprints = value; 
            }
        }
        private Dictionary <string,Dictionary<string,string>>_tekeningbijruimte = new Dictionary<string,Dictionary<string,string>>();
        //ruimteID,dict<ruimteset,path>
        public Dictionary<string, Dictionary<string, string>> TekeningBijRuimte
        { get { return _tekeningbijruimte; } set { _tekeningbijruimte = value; } }
	 */

        private static int _nodeCounter;
        public static int GetNodeID()
        {
            _nodeCounter++;
            return _nodeCounter;
        }
        public string ClientIP;
        public bool ImgMutex = false;
       
		/*
		private Dictionary<string,string> _huidigruimtesetkey = new Dictionary<string,string>();
        public Dictionary<string, string> HuidigRuimteSetKey
        { get { return _huidigruimtesetkey; } set { _huidigruimtesetkey = value; } }
		 * */
        private Dictionary<string, byte[]> _productplaatjes = new Dictionary<string,byte[]>();
        public Dictionary<string, byte[]> ProductPlaatjes
        { get { return _productplaatjes; } set { _productplaatjes = value; } }
        private Dictionary<string, bool> _ruimteSpiegel = new Dictionary<string, bool>();
        public Dictionary<string, bool> ruimteSpiegel
        { get { return _ruimteSpiegel; } set { _ruimteSpiegel = value; } }
        public static Acumulator Instance()
		{
			if (_instance == null)
			{
				_instance = new Acumulator();
			}
			return _instance;
		}
      /* 
		public void ClearBouwnummer()
        {
            HuidigRuimteSetKey.Clear();
        }
	    */
        public void SetProductPlaatje(string productID, byte[] data)
        {
            if (ProductPlaatjes.ContainsKey(productID))
            {
                ProductPlaatjes[productID] = data;
            }
            else
            {
                ProductPlaatjes.Add(productID, data);
            }
        }
		/*
        public void SetHuidigRuimteSetKey(string ruimteid, string ruimtesetkey)
        {
            if (HuidigRuimteSetKey.ContainsKey(ruimteid))
            {
                HuidigRuimteSetKey[ruimteid] = ruimtesetkey;
            }
            else
            {
                HuidigRuimteSetKey.Add(ruimteid, ruimtesetkey);
            }
        }
     /*   public void BlueprintRequestReady(string path,byte[] data,string RuimteS_NR)
        {
            var ValidR_ids = (from r in OTracker.offerteRuimte_.Children
                             where r.GetType() == typeof(Ruimte)
                             select (r as Ruimte).RuimteID).ToList();
            // Let op, als meerdere ruimtes in het zelfde bouwnummer de zelfde tekening gebruiken gaat dit fout!!!
            String R_id;
          //replacement
            R_id = (from br in Acumulator.Instance().ctx.Bron_Ruimtes
                    where br.tekeningPath_Spiegel == path && ValidR_ids.Contains(br.R_NR)
                    select br.R_NR).FirstOrDefault();
            if (R_id == null)
            {
                R_id = (from br in Acumulator.Instance().ctx.Bron_Ruimtes
                        where br.tekeningPath == path && ValidR_ids.Contains(br.R_NR)
                        select br.R_NR).FirstOrDefault();
            }
            //
            if (R_id == null)
            {
                R_id = BB.HuidigRuimte.RuimteID;
            }
            updateDownActive(R_id, false);
            var RuimteSetKey= "";
            if(RuimteS_NR == null)
            {
                RuimteSetKey = "Basis";
            }
            else
            {
                RuimteSetKey = RuimteS_NR;
            }
            SetHuidigRuimteSetKey(R_id, RuimteSetKey);

            if (TekeningBijRuimte.ContainsKey(R_id))
            {
                TekeningBijRuimte[R_id].Add(RuimteSetKey, path);
            }
            else
            {
                var dict = new Dictionary<string, string>();
                dict.Add(RuimteSetKey, path);
                TekeningBijRuimte.Add(R_id,dict);
            }
            
            if (Blueprints.ContainsKey(path))
            {
                Blueprints[path] = data;
            }
            else
            {
                Blueprints.Add(path, data);
            }

        }*/
        public string GetPathGespiegeld(string ruimteID)
        {
            string path;
           var spiegeld = (from rpt in ctx.RuimtesPerTypes
                        where rpt.R_NR == ruimteID && rpt.T_NR == Type
                        select rpt.Spiegel).FirstOrDefault();
           if (!ruimteSpiegel.ContainsKey(ruimteID))
           {
               var sp = spiegeld ?? false;
               ruimteSpiegel.Add(ruimteID, sp);
           }
            if (spiegeld != null && spiegeld == true)
            {
                path = (from br in ctx.Bron_Ruimtes
                        where br.R_NR == ruimteID
                        select br.tekeningPath_Spiegel).FirstOrDefault();
            }
            else
            {
                path = (from br in ctx.Bron_Ruimtes
                        where br.R_NR == ruimteID
                        select br.tekeningPath).FirstOrDefault();
            }
            return path;
        }
        private Dictionary<string, int> _downloadsactive = new Dictionary<string,int>();
        public int Donwloadsactive
        { 
            get 
            {
                int cntr = 0;
                foreach (var dl in _downloadsactive)
                {
                    cntr += dl.Value;
                }
                return cntr;
            }
        }
        public FinalStage FSdelegate;
        public void clearDownloadsActive()
        {
            _downloadsactive.Clear();
        }
        public void updateDownActive(string r_nr, bool starting)
        {
            if(starting)
            {
                if (_downloadsactive.ContainsKey(r_nr))
                {
                    _downloadsactive[r_nr]++;
                }
                else
                {
                    _downloadsactive.Add(r_nr,1);
                }
            }
            else
            {
                _downloadsactive[r_nr]--;
            }


            if (FSdelegate != null)
            { 
                FSdelegate.downloadChanged(_downloadsactive);
            }
        }
		public string lastGeneratedXml { get; set; }
	}

	public enum UserRole{ Geen,Admin,Adviseur, Demo,Bewoner,Showroom}
	public class LoggedInUser
	{
        private int _id;
        public int ID
        {
            get {return _id;}
            set { _id = value; }
        }
		private string _naam;
		public string GebruikersNaam
		{ get { return _naam; } set { _naam = value; } }
		private UserRole _rol;
		public UserRole Rol
		{ get { return _rol; } set { _rol = value; } }

	}
	public static class extensions
	{
		public static byte[] ToByteArray(this WriteableBitmap bmp)
		{
			int[] p = bmp.Pixels;
			int len = p.Length * 4;
			byte[] result = new byte[len]; // ARGB
			Buffer.BlockCopy(p, 0, result, 0, len);
			return result;
		}
	}
    public class UitvoerPlaceHolder
    {
        public UitvoerCreator uitvoerCreator
        { get; set; }
        public PdfDocument Pdfdoc
        { get; set; }

        
    }
}






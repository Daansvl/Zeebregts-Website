using System;
using System.Linq;
using System.Collections.Generic;
using eBrochure_zeebregts.Web.Services;
using eBrochure_zeebregts.Classes;
using eBrochure_zeebregts.Helpers;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Windows;
using System.Net;
using System.IO;
using eBrochure_zeebregts.ExpertControls.Models;

namespace eBrochure_zeebregts
{
	public class Ruimte : IOfferte, IXmlSerializable
	{
		public XmlSchema GetSchema() { return null; }
		public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("RuimteID", RuimteID);
            writer.WriteAttributeString("Omschrijving", Omschrijving);
            writer.WriteAttributeString("PlafondHoogte", PlafondHoogte.ToString());

            if (GekozenPakket != null)
            {
                writer.WriteStartElement("GekozenPakket");
                GekozenPakket.WriteXml(writer);
                writer.WriteEndElement();
            }

            /*if (PakketOud != null)
            {
                writer.WriteStartElement("PakketOud");
                PakketOud.WriteXml(writer);
                writer.WriteEndElement();
            }*/

            writer.WriteStartElement("GekozenOpties");
            foreach (OptieKeuze ok in GekozenOpties)
            {
                writer.WriteStartElement("Optie");
                ok.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("GekozenTegels");
            foreach (Product p in GekozenTegels)
            {
                writer.WriteStartElement("Tegel");
                p.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("GekozenBewerkingen");
            foreach (Nabewerking nb in GekozenBewerkingen)
            {
                writer.WriteStartElement("Bewerking");
                nb.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("GekozenAccenten");
            foreach (Product p in GekozenAccenten)
            {
                writer.WriteStartElement("Accent");
                p.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("GekozenProfielen");
            foreach (HoekProfiel hp in GekozenProfielen)
            {
                writer.WriteStartElement("Profiel");
                hp.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("AanvullendeMeters");
            foreach (ExtraMeters em in AanvullendeMeters)
            {
                writer.WriteStartElement("ExtraMeters");
                em.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("ChildrenSubPerR");
            foreach (SubCatPerRuimteDeel scpr in Children.Where(x => x.GetType() == typeof(SubCatPerRuimteDeel)))
            {
                writer.WriteStartElement("SubPerR");
                scpr.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("ChildrenSubPerP");
            foreach (SubCatPerPakket scpp in Children.Where(x => x.GetType() == typeof(SubCatPerPakket)))
            {
                writer.WriteStartElement("SubPerP");
                scpp.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("ChildrenPakK");
            foreach (PakketKeuze pk in Children.Where(x => x.GetType() == typeof(PakketKeuze)))
            {
                writer.WriteStartElement("PakketK");
                pk.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("ChildrenOptieK");
            foreach (OptieKeuze ok in Children.Where(x => x.GetType() == typeof(OptieKeuze)))
            {
                writer.WriteStartElement("OptieK");
                ok.WriteXml(writer);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            //writer.WriteEndElement();

        }
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			RuimteID = reader.GetAttribute("RuimteID");
			Omschrijving = reader.GetAttribute("Omschrijving");
			PlafondHoogte = int.Parse(reader.GetAttribute("PlafondHoogte"));

			reader.Read();
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "GekozenPakket")
			{
				while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "GekozenPakket")
				{
					PakketKeuze p = new PakketKeuze();
					p.ReadXml(reader);
					p.GetPrijs(this.RuimteID, Acumulator.Instance().ctx);
					GekozenPakket = p;
					reader.Read();
				}
				if (reader.LocalName == "GekozenPakket")
				{
					reader.ReadEndElement();
				}
			}
			//reader.Read();
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "PakketOud")
			{
				while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "PakketOud")
				{
					PakketKeuze p = new PakketKeuze();
					p.ReadXml(reader);
					PakketOud = p;
					reader.Read();
				}
				if (reader.LocalName == "PakketOud")
				{
					reader.ReadEndElement();
				}
			}
			//
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "GekozenOpties")
			{
				reader.Read();
				while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Optie")
				{
					OptieKeuze o = new OptieKeuze();
					o.ReadXml(reader);
					GekozenOpties.Add(o);
					reader.Read();
				}
				if (reader.LocalName == "GekozenOpties")
				{
					reader.ReadEndElement();
				}
			}
			//
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "GekozenTegels")
			{
				reader.Read();
				while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Tegel")
				{
					Product p = new Product();
					p.ReadXml(reader);
					GekozenTegels.Add(p);
					reader.Read();
				}
				if (reader.LocalName == "GekozenTegels")
				{
					reader.ReadEndElement();
				}
			}
			//
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "GekozenBewerkingen")
			{
				reader.Read();
				while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Bewerking")
				{
					Nabewerking nb = new Nabewerking();
					nb.ReadXml(reader);
					GekozenBewerkingen.Add(nb);
					reader.Read();
				}
				if (reader.LocalName == "GekozenBewerkingen")
				{
					reader.ReadEndElement();
				}
			}
			//
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "GekozenAccenten")
			{
				reader.Read();
				while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Accent")
				{
					Product p = new Product();
					p.ReadXml(reader);
					GekozenAccenten.Add(p);
					reader.Read();
				}
				if (reader.LocalName == "GekozenAccenten")
				{
					reader.ReadEndElement();
				}
			}
			//
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "GekozenProfielen")
			{
				reader.Read();
				while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Profiel")
				{
					HoekProfiel hp = new HoekProfiel();
					hp.ReadXml(reader);
					GekozenProfielen.Add(hp);
					reader.Read();
				}
				if (reader.LocalName == "GekozenProfielen")
				{
					reader.ReadEndElement();
				}
			}

            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "AanvullendeMeters")
            {
                reader.Read();
                while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ExtraMeters")
                {
                    var exMtr = new ExtraMeters();
                    exMtr.ReadXml(reader);
                    AanvullendeMeters.Add(exMtr);
                    reader.Read();
                }
                if (reader.LocalName == "AanvullendeMeters")
                {
                    reader.ReadEndElement();
                }
            }

			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ChildrenSubPerR")
			{
				reader.Read();
				while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "SubPerR")
				{
					SubCatPerRuimteDeel srd = new SubCatPerRuimteDeel();
					srd.ReadXml(reader);
					Children.Add(srd);
					reader.Read();
				}
				if (reader.LocalName == "ChildrenSubPerR")
				{
					reader.ReadEndElement();
				}
			}
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ChildrenSubPerP")
			{
				reader.Read();
				while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "SubPerP")
				{
					SubCatPerPakket spk = new SubCatPerPakket();
					spk.ReadXml(reader);
					Children.Add(spk);
					reader.Read();
				}
				if (reader.LocalName == "ChildrenSubPerP")
				{
					reader.ReadEndElement();
				}
			}
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ChildrenPakK")
			{
				reader.Read();
				while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "PakketK")
				{
					PakketKeuze pk = new PakketKeuze();
					pk.ReadXml(reader);
					Children.Add(pk);
					reader.Read();
				}
				if (reader.LocalName == "ChildrenPakK")
				{
					reader.ReadEndElement();
				}
			}
			if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "ChildrenOptieK")
			{
				reader.Read();
				while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "OptieK")
				{
					OptieKeuze ok = new OptieKeuze();
					ok.ReadXml(reader);
					Children.Add(ok);
					reader.Read();
				}
				if (reader.LocalName == "ChildrenOptieK")
				{
					reader.ReadEndElement();
				}
			}
				//qryPakketten(Acumulator.Instance().ctx);
			//	qryGetRuimteOpties(Acumulator.Instance().ctx);
		//		qryGetSubCategories(Acumulator.Instance().ctx);
			
		}

		public int lastRandom;
		public Random RandomGenerator = new Random();
		public string RuimteID
		{ get; set; }
		public string Omschrijving
		{ get; set; }
		public int PlafondHoogte
		{ get; set; }
		private PakketKeuze _gekPakt;
		public PakketKeuze GekozenPakket
		{
			get { return _gekPakt; } 
			set
			{
				_gekPakt = value;
				if (value != null)
				{
					
					//qrySubCatPakketten(Acumulator.Instance().ctx, _gekPakt);
				}
			}
		}
		public PakketKeuze PakketOud;
		private List<OptieKeuze> gekozenopties = new List<OptieKeuze>();
		public List<OptieKeuze> GekozenOpties
		{ get { return gekozenopties; } set { gekozenopties = value; } }
		private List<Product> _gekozenTegls = new List<Product>();
		public List<Product> GekozenTegels
		{
			get { return _gekozenTegls; }
			set { _gekozenTegls = value; }
		}
		private List<Nabewerking> _gekozenbewerkingen = new List<Nabewerking>();
		public List<Nabewerking> GekozenBewerkingen
		{ get { return _gekozenbewerkingen; }
			set { _gekozenbewerkingen = value; }
		}
		private List<HoekProfiel> _gekProfl = new List<HoekProfiel>();
		public List<HoekProfiel> GekozenProfielen
		{ get { return _gekProfl; } set { _gekProfl = value; } }
		private List<Product> _gekAccnt = new List<Product>();
		public List<Product> GekozenAccenten
		{ get { return _gekAccnt; } set { _gekAccnt = value; } }
        private List<ExtraMeters> _anvMeters = new List<ExtraMeters>();
        public List<ExtraMeters> AanvullendeMeters
        {
            get { return _anvMeters; }
            set { _anvMeters = value; }
        }
		public Ruimte()
		{
		}
		public Ruimte(string rid, string omsch, int ph)
		{
			RuimteID = rid;
			Omschrijving = omsch;
			PlafondHoogte = ph;


			qryPakketten(Acumulator.Instance().ctx);
			qryGetRuimteOpties(Acumulator.Instance().ctx);
			qryGetSubCategories(Acumulator.Instance().ctx);
			
		}



		public void updateSubcats()
		{
			Children.RemoveAll(sc => sc.GetType() == typeof(SubCatPerPakket) || sc.GetType() == typeof(SubCatPerRuimteDeel));
			qryGetSubCategories(Acumulator.Instance().ctx);
			foreach (OptieKeuze opk in gekozenopties)
			{
				addSubcats(opk);
			}
		}
		private void addSubcats(OptieKeuze ok)
		{
			if (ok != null)
			{
				foreach (var sc in ok.Children)
				{
					Add(sc);
				}
			}
		}
 
        public Dictionary<string, double> GetSaldoMetersHoek()
        {
            updateSubcats();
            Dictionary<string, double> saldi = new Dictionary<string, double>();
          
            List<string> excludes = new List<string>();
           
            foreach (SubCatPerRuimteDeel scprd in Children.Where(c => c.GetType() == typeof(SubCatPerRuimteDeel) /*&& !(excludes.Contains((c as SubCatPerRuimteDeel).SubCatNR))*/))
            {
                if (scprd.SubCatNR == "SCB3" || scprd.SubCatNR == "SCB10" || scprd.SubCatNR == "SCB12" || scprd.SubCatNR == "SCB17")
                {
                    if (saldi.ContainsKey(scprd.SubCatNR))
                    {
                        saldi[scprd.SubCatNR] += scprd.Meters;
                    }
                    else
                    {
                        saldi.Add(scprd.SubCatNR, scprd.Meters);
                    }
                }
            }

            return saldi;
        }
		public Dictionary<string, double> GetSaldoMetersAccent()
		{
			var optex = (from o in gekozenopties
						 where o.OptieSoort == "OT3"
						 select o).ToList();
			foreach (var op in optex)
			{
				var otd = (from o in gekozenopties
						   where o.OptieID == op.OptieID
						   select o).FirstOrDefault();
				gekozenopties.Remove(otd);
			}
			updateSubcats();
			Dictionary<string, double> saldi = new Dictionary<string, double>();
			foreach (SubCatPerRuimteDeel scprd in Children.Where(c => c.GetType() == typeof(SubCatPerRuimteDeel) /*&& !(excludes.Contains((c as SubCatPerRuimteDeel).SubCatNR))*/))
			{
				if (saldi.ContainsKey(scprd.SubCatNR))
				{
					saldi[scprd.SubCatNR] += scprd.Meters;
				}
				else
				{
					saldi.Add(scprd.SubCatNR, scprd.Meters);
				}
			}
			gekozenopties.AddRange(optex);
			updateSubcats();
			return saldi;
		}
		public Dictionary<string, double> GetSaldoMeters()
		{
			updateSubcats();
			Dictionary<string, double> saldi = new Dictionary<string, double>();
		
			List<string> excludes = new List<string>();
		
			foreach (SubCatPerRuimteDeel scprd in Children.Where(c => c.GetType() == typeof(SubCatPerRuimteDeel)))
			{
				if (saldi.ContainsKey(scprd.SubCatNR))
					{
						saldi[scprd.SubCatNR] += scprd.Meters;
					}
					else
					{
						saldi.Add(scprd.SubCatNR, scprd.Meters);
					}
			}

			return saldi;
		}
        public Dictionary<string, double> GetMetersAdjusted(Dictionary<string, double> oldSaldo)
        {

            foreach (var exM in AanvullendeMeters)
            {
                var tmpSld = oldSaldo[exM.LinkedSubCat];

                if (!exM.IsBasis)
                {
                    oldSaldo[exM.LinkedSubCat] -= exM.Meters;
                }
            }

            
            return oldSaldo;
        }
        
        public BluePrintStatus SwitchBluePrint()
        {
            BluePrintStatus retval = BluePrintStatus.Idle;
            var ctx = Acumulator.Instance().ctx;
            var onaf = (from rd in ctx.RuimteDelens
                        join sro in ctx.StandaardRuimteOpbouws on rd.R_ID equals sro.R_NR
                        join soro in ctx.StandaardOnafhandelijkeRuimteOpbouws on sro.SRO_ID equals soro.SRO_NR
                        select rd.R_ID).ToList();
            var gekR_ids = (from go in GekozenOpties
                            where !onaf.Contains(go.OptieID) && (go.OptieSoort == "OT1"|| go.OptieSoort == "OT2" || go.OptieSoort == "OT5")
                            select go.OptieID).ToList();
            gekR_ids.Add(RuimteID);
            var rs_sets = new Dictionary<string, List<string>>();
            foreach (var rid in gekR_ids)
            {
                var Rs_nrs = (from rs in ctx.RuimteOpbouws
                              where rs.R_NR == rid
                              select rs.RS_NR).ToList();
                rs_sets.Add(rid, Rs_nrs);
            }
       // method as of Juraci
            var allruimteSets = (from set in ctx.RuimteS
                                select set.RS_ID).ToList();
            for (int i = allruimteSets.Count - 1; i >= 0; i--)
            {
                var rs_rids_count = (from rsid in ctx.RuimteOpbouws
                               where rsid.RS_NR == allruimteSets[i]
                               select rsid.R_NR).ToList().Count;
                if (gekR_ids.Count != rs_rids_count)
                {
                    allruimteSets.RemoveAt(i);

                }
                else
                {
                    bool match = true;
                    foreach (var oset in rs_sets)
                    {
                        if (!oset.Value.Contains(allruimteSets[i]))
                        {
                            match = false;
                            break;
                        }
                    }
                    if (!match)
                    {
                        allruimteSets.RemoveAt(i);
                    }
                }
            }
            ///
          //  MessageBox.Show("done: " + allruimteSets.Count + "  "+allruimteSets.First());
            if (allruimteSets.Count == 1 && !RuimteID.StartsWith("937"))
            {
				Acumulator.Instance().bluePrintManager.SetHuidigRuimteSetNr(RuimteID, allruimteSets.First());
                retval = Acumulator.Instance().bluePrintManager.GetBlueprintStatus(RuimteID); 
				if ( retval == BluePrintStatus.Complete) {
					
				}
				/*  if (Acumulator.Instance().TekeningBijRuimte.ContainsKey(RuimteID) && Acumulator.Instance().TekeningBijRuimte[RuimteID].ContainsKey(allruimteSets.First()) && (Acumulator.Instance().Blueprints.ContainsKey( Acumulator.Instance().TekeningBijRuimte[RuimteID][allruimteSets.First()])))
                {
                    Acumulator.Instance().InfoBar.LoadImg(Acumulator.Instance().TekeningBijRuimte[RuimteID][allruimteSets.First()]);
                    Acumulator.Instance().SetHuidigRuimteSetKey(RuimteID, allruimteSets.First());
                }*/
                else
                {

                    RuimteSetKey = allruimteSets.First();
                    var path = "";
                /*    var spiegeld = (from bnr in ctx.Bouwnummers
                                    where bnr.B_ID == Acumulator.Instance().Bouwnr
                                    select bnr.Gespiegeld).FirstOrDefault();*/
                    var spiegeld = (from rpt in ctx.RuimtesPerTypes
                                    where rpt.R_NR == RuimteID && rpt.T_NR == Acumulator.Instance().Type
                                    select rpt.Spiegel).FirstOrDefault();
                    if (spiegeld != null && spiegeld == true)
                    {
                        path = (from rs in ctx.RuimteS
                                where rs.RS_ID == allruimteSets.First()
                                select rs.BlueprintPath_Gespiegeld).FirstOrDefault();
                    }
                    else
                    {
                        path = (from rs in ctx.RuimteS
                                where rs.RS_ID == allruimteSets.First()
                                select rs.BlueprintPath).FirstOrDefault();
                    }
                    if (path != null && path != "")
                    {
                        string apath = "http://mybouwapp.nl/Images/Blueprints" + path;
                        Acumulator.Instance().bluePrintManager.DownloadBlueprint(path, RuimteID, RuimteSetKey);
                        retval = BluePrintStatus.Downloading;
                        /*   HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apath);

                           request.UseDefaultCredentials = true;
                           Acumulator.Instance().updateDownActive(this.RuimteID, true);
                           request.BeginGetResponse(new AsyncCallback(RequestReady), request);*/

                    }
                    else
                    {
                        //geen blueprint beschikbaar
                        retval = BluePrintStatus.Unavailable;
                    }
                }
            }
            else
            {
				Acumulator.Instance().bluePrintManager.SetHuidigRuimteSetNr(RuimteID, "Basis");
				Acumulator.Instance().InfoBar.SetRuimteID(RuimteID);
               //Acumulator.Instance().InfoBar.LoadImg(Acumulator.Instance().TekeningBijRuimte[RuimteID]["Basis"]);
            }
            return retval;
        }
        private string RuimteSetKey;
        public bool spiegel;
        /*private void RequestReady(IAsyncResult asyncResult)
        {
            var req = asyncResult.AsyncState as HttpWebRequest;

            try
            {
                using (WebResponse wrs = req.EndGetResponse(asyncResult))
                {
                    var path = (from br in Acumulator.Instance().ctx.Bron_Ruimtes
                                where br.R_NR == RuimteID
                                select br.tekeningPath).FirstOrDefault();

                    var rpath = "";
                
                    var spiegeld = (from rpt in Acumulator.Instance().ctx.RuimtesPerTypes
                                    where rpt.R_NR == RuimteID && rpt.T_NR == Acumulator.Instance().Type
                                    select rpt.Spiegel).FirstOrDefault();
                    spiegel = spiegeld ?? false;
                    if (spiegeld != null && spiegeld == true)
                    {
                        rpath = (from rs in Acumulator.Instance().ctx.RuimteS
                                 where rs.RS_ID == RuimteSetKey
                                 select rs.BlueprintPath_Gespiegeld).FirstOrDefault();
                    }
                    else
                    {
                        rpath = (from rs in Acumulator.Instance().ctx.RuimteS
                                 where rs.RS_ID == RuimteSetKey
                                 select rs.BlueprintPath).FirstOrDefault();
                    }
                    var foo = wrs.GetResponseStream();

                    var bar = foo.CloneToMemoryStream();
                    var foobar = bar.ToArray();
                   
                    Acumulator.Instance().BlueprintRequestReady(rpath, foobar, RuimteSetKey);
                    Acumulator.Instance().InfoBar.LoadImg(rpath);
                 UIThread.Invoke(()=>  Acumulator.Instance().BB.ReloadImage());
                }
            }
            catch (Exception e)
            {
                var err = e;
                var emsg = e.Message;
            }
           
           
        }
      */
		public void FilterPakketten()
		{
			//huidige pakketten
			try
			{
				var pakketten = from p in Children
								where p.GetType() == typeof(PakketKeuze)
								select p;
				//get subcats + meters Keuzes
				var saldi = GetSaldoMeters();
				List<string> benodigde_catt = new List<string>();
				foreach (string cat in saldi.Keys)
				{
					if (cat != "SCB4")
					{
						if (saldi[cat] > 0)
						{
							benodigde_catt.Add(cat);
						}
					}
				}
				foreach (PakketKeuze pk in pakketten)
				{
					var foo = pk.Children.Where(s => s.GetType() == typeof(SubCatPerPakket));//.Select(c => (c as SubCatPerPakket).SubCatNR).ToList();
					var bar = (from f in foo
							   select (f as SubCatPerPakket).SubCatNR).ToList();
					//var x =bar.Intersect(benodigde_catt);
					var catinpak = from pc in bar
								   join bc in benodigde_catt on pc equals bc
								   select bc;
					IOfferte toremove = Children.Where(p => p.GetType() == typeof(PakketKeuze) && (p as PakketKeuze).Pakket_ID == pk.Pakket_ID).FirstOrDefault();
					if (benodigde_catt.Count > catinpak.Count() || catinpak.Count() > benodigde_catt.Count)
					{
                        toremove.IsEnabled = false;
					}
					else
					{
						toremove.IsEnabled = true;
					}
				}
			}
			catch (Exception e)
			{
				FilterPakketten();
			}

			


		}
		public override TvNode MakeTree(TvNode TreeNode)
		{

			List<TvNode> Childs = new List<TvNode>();
			//
            TvNode thisnode = new TvNode(Omschrijving,RuimteID, this.GetType().Name, Childs, TreeNode);
			/*if (Children.Where(c => c.GetType() == typeof(OptieKeuze) && ((c as OptieKeuze).OptieSoort == "OT1" || (c as OptieKeuze).OptieSoort == "OT2") && ((c as OptieKeuze).OptType == OptieType.Determinating || (c as OptieKeuze).OptType == OptieType.Independant)).Count() > 0)
			{
				var node = new TvNode("Opties", "RuimteOpties_pre", null, thisnode);
				node.Enabled = false;
				Childs.Add(node);
			}*/
            var anode = new TvNode("Opties", RuimteID, "RuimteOpties_pre", null, thisnode);
            anode.Enabled = false;
            Childs.Add(anode);
            var bpnode = new TvNode("Plategrond", RuimteID, "BluePrintPage", null, thisnode);
			bpnode.Enabled = true;
			Childs.Add(bpnode);
			if (Children.Where(c => c.GetType() == typeof(PakketKeuze)).Count() > 0)
			{
                var node = new TvNode("Pakket", RuimteID, "PakketKeuze", null, thisnode);
				node.Enabled = false;
				Childs.Add(node);
			}
			if (Children.Where(c => c.GetType() == typeof(SubCatPerRuimteDeel)).Count() > 0)
			{
                var node = new TvNode("Tegels", RuimteID, "SubCategoriën", null, thisnode);
				node.Enabled = false;
				Childs.Add(node);

                node = new TvNode("Hoekprofielen", RuimteID, "SubCategoriën_hoek", null, thisnode);
                node.Enabled = false;
                Childs.Add(node);

                node = new TvNode("Accent Tegels", RuimteID, "SubCategoriën_accent", null, thisnode);
				node.Enabled = false;
				Childs.Add(node);

                node = new TvNode("Verwerking", RuimteID, "SubCategoriën_na", null, thisnode);
				node.Enabled = false;
				Childs.Add(node);
			}
			thisnode.Children = Childs;
			
		
			return thisnode;
		}
		public bool LoadOptieSet(KeuzeControls.KeuzeList keuzelist)
		{
			return qryGetRuimteOptiesFase2(Acumulator.Instance().ctx,keuzelist);
		}
		private bool CanShowSubCat(string SubCatNR, Dictionary<string, double> saldi, double maxdif)
		{
			var ctx = Acumulator.Instance().ctx;
			var retval = false;
            var HC_NR = (from sc in ctx.SubCats
						 join c in ctx.Categorieëns on sc.C_NR equals c.C_ID
						 join hc in ctx.HoofdCategorieëns on c.HC_NR equals hc.HC_ID
						 where sc.SCB_ID == SubCatNR
						 select hc.HC_ID).FirstOrDefault();
            if (HC_NR != null)
            {
                var C_NRs = (from c in ctx.Categorieëns
                             where c.HC_NR == HC_NR
                             select c.C_ID).ToList();
                var SC_NRs = (from sc in ctx.SubCats
                              where C_NRs.Contains(sc.C_NR) && (sc.SCB_ID != "SCB3" && sc.SCB_ID != "SCB10" && sc.SCB_ID != "SCB12" && sc.SCB_ID != "SCB17" /* && sc.SCB_ID != "SCB11"*/)
                              select sc.SCB_ID).ToList();
                var Meters = from m in saldi
                             where SC_NRs.Contains(m.Key)
                             select m.Value;
                var saldo = 0.0;
                foreach (var x in Meters)
                {
                    saldo += x;
                }
                if (saldi[SubCatNR] == 0)
                {
                    return false;
                }
                if (saldo > 0 && saldo >= maxdif)
                    retval = true;
                else
                    retval = false;
            }
			return retval;
		}

        public bool IsAccentNewStyle()
        {
            return Acumulator.Instance().ctx.AccentSetProjectCombinaties.FirstOrDefault(x=>x.Project_NR == Acumulator.Instance().Projectnr) != null;
        }

        public Dictionary<string, AdvancedTegelSubCatInfo> qryNewSubCatAccent(eBrochureDomainContext ctx, PakketKeuze pk)
        {
            Dictionary<string, AdvancedTegelSubCatInfo> resultaat = new Dictionary<string, AdvancedTegelSubCatInfo>();
            var R_ids = (from ok in GekozenOpties
                         where ok.OptieSoort == "OT1" || ok.OptieSoort == "OT2" || ok.OptieSoort == "OT5"
                         orderby ok.OptieID
                         select ok.OptieID).ToList();
            var m_R_ids = (from o in GekozenOpties
                           select o.OptieID).ToList();
            m_R_ids.Add(this.RuimteID);
            R_ids.Add(this.RuimteID);
            var Keuzes = (from srd in ctx.SubCatPerRuimteDeels
                          where R_ids.Contains(srd.R_NR)
                          orderby srd.SCB_NR
                          select srd.SCB_NR).Distinct().ToList();


            var subcats = (from scp in ctx.SubCatPerPakkets
                           join scb in ctx.SubCats on scp.SCB_NR equals scb.SCB_ID
                           where scp.P_NR == pk.Pakket_ID && Keuzes.Contains(scp.SCB_NR) /*&& (scp.SCB_NR != "SCB3" && scp.SCB_NR != "SCB10" && scp.SCB_NR != "SCB12")*/ /* && scp.SCB_NR != "SCB11"*/
                           orderby scb.volgorde
                           select new SubCatPerPakket() { PakketNR = scp.P_NR, SubCatNR = scp.SCB_NR, ID = scp.SCBP_ID, ProductSetNR = scp.PD_SET_NR, CategorieNR = scb.C_NR });
            var saldi = GetSaldoMeters();
            foreach (SubCatPerPakket subc in subcats)
            {
                var minmeters = (double)(from srd in ctx.SubCatPerRuimteDeels
                                         where m_R_ids.Contains(srd.R_NR) && srd.SCB_NR == subc.SubCatNR
                                         select Math.Abs((double)srd.meters)).Min();
                var maxdif = (double)(from srd in ctx.SubCatPerRuimteDeels
                                      where m_R_ids.Contains(srd.R_NR) && srd.SCB_NR == subc.SubCatNR
                                      select srd).Count();
                maxdif = maxdif / 2 * 0.01;

                if (CanShowSubCat(subc.SubCatNR, saldi, maxdif))//saldi[subc.SubCatNR] > 0 && saldi[subc.SubCatNR] >= maxdif
                {

                    var gekTegel = Acumulator.Instance().BB.HuidigRuimte.GekozenTegels.FirstOrDefault(x => (from scpp in ctx.SubCatPerPakkets
                                                                                                            where scpp.SCBP_ID == x.LinkedSubCat
                                                                                                            select scpp.SCB_NR).FirstOrDefault() == subc.SubCatNR);
                                                                                                               
                    if (gekTegel != null)
                    {
                        var newModel = AdvancedTegelSubCatInfo.GetNewAccentModel(subc, gekTegel, saldi[subc.SubCatNR]);
                            //newModel = UpdateModel(newModel);
                        resultaat.Add(subc.SubCatNR, newModel);
                    }                                                                                      
                    else//maybe hoekprofiel
                    {
                       var gekProfiel = Acumulator.Instance().BB.HuidigRuimte.GekozenProfielen.FirstOrDefault(x => (from scpp in ctx.SubCatPerPakkets
                                                                                                               where scpp.SCBP_ID == x.LinkedSubCat
                                                                                                               select scpp.SCB_NR).FirstOrDefault() == subc.SubCatNR);
                       if (gekProfiel != null)
                       {
                           var newModel = AdvancedTegelSubCatInfo.GetNewAccentModel(subc, gekProfiel, saldi[subc.SubCatNR]);
                           resultaat.Add(subc.SubCatNR, newModel);
                       }
                    }
                    
                    
                }
            }

            return resultaat;
        }
        private AdvancedTegelSubCatInfo UpdateModel(AdvancedTegelSubCatInfo inputModel)
        {
            foreach (var aanv in AanvullendeMeters)
            {
                foreach (var rgl in inputModel.Regels)
                {
                    if (rgl.GekozenTegel.ProductId == aanv.LinkedProduct.ID)
                    {
                        rgl.Opmerking = aanv.Opmerking;
                        rgl.VervangInfo.Opmerking = aanv.Opmerking;
                    }
                }
            }
            return inputModel;
        }
        public List<SubCatPerPakket> qrySubCatAccent(eBrochureDomainContext ctx, PakketKeuze pk)
		{
			List<SubCatPerPakket> resultaat = new List<SubCatPerPakket>();

			var rs_nrs = (from ro in ctx.RuimteOpbouws
						  where ro.R_NR == RuimteID
						  select ro.RS_NR).Distinct().ToList();
            var ongeldigeoptieset = (from o in GekozenOpties
                                     join oo in ctx.OngeldigeCombinatieOpbouws on o.OptieID equals oo.R_NR
                                     select oo.OC_NR).ToList();
            var GekOpties = (from go in GekozenOpties
                            select go.OptieID).ToList();
            var foo = (from bar in ctx.OngeldigeCombinatieOpbouws
                       where ongeldigeoptieset.Contains(bar.OC_NR) && !GekOpties.Contains(bar.R_NR)
					   select bar.R_NR).ToList();
            var ot5filter = (from r in Children.Where(x=>x.GetType() == typeof(OptieKeuze))
                             where (r as OptieKeuze).OptieSoort == "OT5"
                             select (r as OptieKeuze).OptieID).ToList();
            foo.AddRange(ot5filter);
			var R_ids = (from r in ctx.RuimteOpbouws
						 where rs_nrs.Contains(r.RS_NR)&& !foo.Contains(r.R_NR)
						 select r.R_NR).Distinct().ToList();
        	var accentsc = (from ac in ctx.SubCatPerRuimteDeels
							join sc in ctx.SubCats on ac.SCB_NR equals sc.SCB_ID
							join c in ctx.Categorieëns on sc.C_NR equals c.C_ID
							where (c.C_ID == "C8" || c.C_ID == "C6" || c.C_ID == "C7") && R_ids.Contains(ac.R_NR) && ac.meters > 0 && ac.SCB_NR != "SCB3" && ac.SCB_NR != "SCB12" && ac.SCB_NR != "SCB10" && ac.SCB_NR!="SCB5"
							select ac.SCB_NR).Distinct().ToList();
       	
			var subcats = (from scp in ctx.SubCatPerPakkets
						   join scb in ctx.SubCats on scp.SCB_NR equals scb.SCB_ID
						   where scp.P_NR == GekozenPakket.Pakket_ID && accentsc.Contains(scp.SCB_NR)
						   orderby scb.volgorde
						   select new SubCatPerPakket() {PakketNR = scp.P_NR, SubCatNR = scp.SCB_NR, ID=scp.SCBP_ID, ProductSetNR = scp.PD_SET_NR,CategorieNR=scb.C_NR });
			//var saldi = GetSaldoMeters();
			foreach (SubCatPerPakket subc in subcats)
			{
			//	subc.TotaalMeters = saldi[subc.SubCatNR];
				var producten = from prdct in ctx.Productens
								join pso in ctx.ProductSetOpbouws on prdct.PD_ID equals pso.PD_NR
								join pst in ctx.ProductSets on pso.PD_SET_NR equals pst.PD_SET_ID
								join pc in ctx.ProductCategoriëns on prdct.PC_NR equals pc.PC_ID
								where pst.PD_SET_ID == subc.ProductSetNR
								select new Product()
								{
									ID = prdct.PD_ID,
									volgorde = (int)prdct.volgorde,
									VerpakkingsToeslag = (double)prdct.verpakkingstoeslag,
									LinkedSubCat = subc.ID,
									LinkedMainCat = (from hc in ctx.HoofdCategorieëns
													 join c in ctx.Categorieëns on hc.HC_ID equals c.HC_NR
													 join sc in ctx.SubCats on c.C_ID equals sc.C_NR
													 where sc.SCB_ID == subc.SubCatNR
													 select hc.HC_ID).FirstOrDefault(),
									Omschrijving = prdct.Omschrijving,
									Kleur = (from kleur in ctx.Kleurens
												where kleur.K_ID == prdct.PKC_NR
												select kleur.Omschrijving).FirstOrDefault().ToString(),
									kleurVolgorde = (from kleur in ctx.Kleurens
													 where kleur.K_ID == prdct.PKC_NR
													 select (int)kleur.volgorde).FirstOrDefault(),
													 KleurCode = prdct.PKC_NR,
									Breedte = (int)prdct.breedte,
									Lengte = (int)prdct.lengte,
									ImgPath = prdct.ImagePath,
									productcode = prdct.productcode
								};
				foreach (Product p in producten)
				{
					subc.Children.Add(p);
				}
				var tmpP = new Product()
				{
					ID = "0",
					volgorde = 9999,
					VerpakkingsToeslag = 0,
					LinkedSubCat = subc.ID,
					Omschrijving = "Geen Accent",
					Kleur = "Geen Accent",
					kleurVolgorde = 9999,
					Breedte = 9999,
					Lengte = 9999,
					ImgPath = "NothingSign.png",
					productcode = ""
				};
				subc.Children.Add(tmpP);
				Helpers.CustomComparer<IOfferte> comp = new Helpers.CustomComparer<IOfferte>();
				subc.Children.Sort(comp);
				resultaat.Add(subc);
			}
			
			return resultaat;

		}
		public List<SubCatPerPakket> qrySubCatPakketten(eBrochureDomainContext ctx, PakketKeuze pk)
		{
			List<SubCatPerPakket> resultaat = new List<SubCatPerPakket>();
			var R_ids = (from ok in GekozenOpties
						 where ok.OptieSoort == "OT1"|| ok.OptieSoort == "OT2" || ok.OptieSoort == "OT5"
						 orderby ok.OptieID
						select ok.OptieID).ToList();
			var m_R_ids = (from o in GekozenOpties
							   select o.OptieID).ToList();
			m_R_ids.Add(this.RuimteID);
			R_ids.Add(this.RuimteID);
			var Keuzes = (from srd in ctx.SubCatPerRuimteDeels
						 where R_ids.Contains(srd.R_NR)
						 orderby srd.SCB_NR
						 select srd.SCB_NR).Distinct().ToList();
						 
						 
			var subcats = (from scp in ctx.SubCatPerPakkets
						   join scb in ctx.SubCats on scp.SCB_NR equals scb.SCB_ID
						   where scp.P_NR == pk.Pakket_ID && Keuzes.Contains(scp.SCB_NR) && (scp.SCB_NR != "SCB3" && scp.SCB_NR != "SCB10" && scp.SCB_NR != "SCB12" /* && scp.SCB_NR != "SCB11"*/)
						   orderby scb.volgorde
						   select new SubCatPerPakket() { PakketNR = scp.P_NR, SubCatNR = scp.SCB_NR, ID = scp.SCBP_ID, ProductSetNR = scp.PD_SET_NR, CategorieNR=scb.C_NR });
			var saldi = GetSaldoMeters();
			foreach (SubCatPerPakket subc in subcats)
			{
				var minmeters = (double)(from srd in ctx.SubCatPerRuimteDeels
											 where m_R_ids.Contains(srd.R_NR) && srd.SCB_NR == subc.SubCatNR
											 select Math.Abs((double)srd.meters)).Min();
				var maxdif = (double)(from srd in ctx.SubCatPerRuimteDeels
							  where m_R_ids.Contains(srd.R_NR) && srd.SCB_NR == subc.SubCatNR
							  select srd).Count();
				maxdif = maxdif / 2 * 0.01;

				if (CanShowSubCat(subc.SubCatNR,saldi,maxdif))//saldi[subc.SubCatNR] > 0 && saldi[subc.SubCatNR] >= maxdif
				{
                    var producten = from prdct in ctx.Productens
                                    join pso in ctx.ProductSetOpbouws on prdct.PD_ID equals pso.PD_NR
                                    join pst in ctx.ProductSets on pso.PD_SET_NR equals pst.PD_SET_ID
                                    join pc in ctx.ProductCategoriëns on prdct.PC_NR equals pc.PC_ID
                                    where pst.PD_SET_ID == subc.ProductSetNR
                                    select new Product()
									{
										ID = prdct.PD_ID,
										volgorde = (int)prdct.volgorde,
										VerpakkingsToeslag = (double)prdct.verpakkingstoeslag,
										LinkedSubCat = subc.ID,
										LinkedMainCat = (from hc in ctx.HoofdCategorieëns
															 join c in ctx.Categorieëns on hc.HC_ID equals c.HC_NR
															 join sc in ctx.SubCats on c.C_ID equals sc.C_NR
															 where sc.SCB_ID == subc.SubCatNR
															 select hc.HC_ID).FirstOrDefault(),
										Omschrijving = prdct.Omschrijving,
										Kleur = (from kleur in ctx.Kleurens
													where kleur.K_ID == prdct.PKC_NR
													select kleur.Omschrijving).FirstOrDefault().ToString(),
										kleurVolgorde = (from kleur in ctx.Kleurens
														 where kleur.K_ID == prdct.PKC_NR
														 select (int)kleur.volgorde).FirstOrDefault(),
														 KleurCode = prdct.PKC_NR,
										Breedte = (int)prdct.breedte,
										Lengte = (int)prdct.lengte,
										ImgPath = prdct.ImagePath,
										productcode = prdct.productcode
									};
					foreach (var p in producten)
					{
						subc.Children.Add(p);
					}
					Helpers.CustomComparer<IOfferte> comp = new Helpers.CustomComparer<IOfferte>();
					subc.Children.Sort(comp);
					resultaat.Add(subc);
				}
			}
			return resultaat;
		}
		public List<SubCatPerPakket> qryNabewerkingen(eBrochureDomainContext ctx)
		{
            List<SubCatPerPakket> results = new List<SubCatPerPakket>();
                
            if (GekozenPakket != null)
            {
               var R_ids = (from ok in GekozenOpties
                             orderby ok.OptieID
                             select ok.OptieID).ToList();
                R_ids.Add(this.RuimteID);
                var Keuzes = (from srd in ctx.SubCatPerRuimteDeels
                              where R_ids.Contains(srd.R_NR) && srd.SCB_NR != "SCB6" && srd.SCB_NR != "SCB7"
                              orderby srd.SCB_NR
                              select srd.SCB_NR).Distinct().ToList();


                var subcats = (from scp in ctx.SubCatPerPakkets
                               join scb in ctx.SubCats on scp.SCB_NR equals scb.SCB_ID
                               where scp.P_NR == GekozenPakket.Pakket_ID && Keuzes.Contains(scp.SCB_NR)
                               orderby scb.volgorde
                               select new SubCatPerPakket() { PakketNR = scp.P_NR, SubCatNR = scp.SCB_NR, ID = scp.SCBP_ID, ProductSetNR = scp.PD_SET_NR, CategorieNR = scb.C_NR });
                //get all meters
                 
                 var saldi = GetMetersAdjusted(GetSaldoMeters());

                foreach (SubCatPerPakket subc in subcats)
                {
                    subc.TotaalMeters = saldi[subc.SubCatNR];
                    var minmeters = (double)(from srd in ctx.SubCatPerRuimteDeels
                                             where R_ids.Contains(srd.R_NR) && srd.SCB_NR == subc.SubCatNR
                                             select Math.Abs((double)srd.meters)).Min();

                    if (saldi[subc.SubCatNR] > 0 )
                    {
                        if (subc.SubCatNR == "SCB3" || subc.SubCatNR == "SCB10" || subc.SubCatNR == "SCB12" || subc.SubCatNR == "SCB17")
                        {
                            var prof = GekozenProfielen.Where(p => p.LinkedSubCat == subc.ID).FirstOrDefault();
                            if (prof != null)
                            {
                                subc.Children.Add(prof);
                                results.Add(subc);
                            }
                        }
                        else
                        {
                            var bewerking = from bwrk in ctx.Nabewerkingens
                                            join nbso in ctx.NabewerkingSetOpbouws on bwrk.N_ID equals nbso.N_NR
                                            join nbst in ctx.NabewerkingSets on nbso.NS_NR equals nbst.NS_ID
                                            join nbsc in ctx.NabewerkingSetCombinaties on nbst.NS_ID equals nbsc.NS_NR
                                            where nbsc.SCBP_NR == subc.ID
                                            orderby bwrk.volgorde
                                            select new Nabewerking()
                                            {
                                                Nabewerking_ID = bwrk.N_ID,
                                                LinkedSubcatNr = subc.SubCatNR,
                                                BewerkingCat_NR = bwrk.NC_NR,
                                                Omschrijving = bwrk.omschrijving,
                                                volgorde = (int)bwrk.volgorde,
                                                TextVoorZin = bwrk.TextVoorZin
                                            };
                            foreach (Nabewerking n in bewerking)
                            {
                                subc.Children.Add(n);
                            }
                            results.Add(subc);
                        }

                    }
                }
                if (Keuzes.Contains("SCB4"))
                {
                    var thsub = new SubCatPerPakket()
                    {
                        PakketNR = GekozenPakket.Pakket_ID,
                        SubCatNR = "SCB4",
                        ID = "SCBP0",
                        CategorieNR = "C1",
                        ProductSetNR = "PD SET 0"
                    };
                    var hoogbasis = (from hb in ctx.SubCatPerRuimteDeels
                                     where hb.R_NR == this.RuimteID && hb.SCB_NR == "SCB4"
                                     select hb.meters).FirstOrDefault();
                    var hoogopties = (from ho in ctx.SubCatPerRuimteDeels
                                      join go in GekozenOpties on ho.R_NR equals go.OptieID
                                      where ho.SCB_NR == "SCB4" && ho.meters > 0
                                      select ho.meters).ToList();

                    var hoogteOpties = 0.0;
                    foreach (var x in hoogopties)
                    {
                        hoogteOpties += (double)x;
                    }
                    thsub.TotaalMeters = (double)hoogbasis;
                    if (hoogopties != null)
                    {
                        thsub.TotaalMeters +=hoogteOpties;
                    }
                    results.Add(thsub);
                }
                //vrije accenten
                if (AanvullendeMeters.Count > 0)
                {
                    foreach (var exMtr in AanvullendeMeters)
                    {
                        var scb = GenerateScbNr4Accent(exMtr.LinkedSubCat);
                        if (exMtr.IsBasis)
                        {
                            var cat = results.FirstOrDefault(x => x.SubCatNR == exMtr.LinkedSubCat);
                            if (cat != null)
                            {
                                cat.TotaalMeters = exMtr.Meters;
                            }
                        }
                        else
                        {
                            var scppID = "Sub4Accent" + scb.SCB_ID;
                            if (exMtr.RegelIndex == 0)
                            {
                                var cat = results.FirstOrDefault(x => x.SubCatNR == exMtr.LinkedSubCat);
                                if (cat != null)
                                {
                                    cat.TotaalMeters = 0;
                                }
                                scppID = exMtr.LinkedSubCat;
                            }
                            var curSubc = results.FirstOrDefault(x => x.SubCatNR == scb.SCB_ID);
                            if (curSubc == null)
                            {
                                curSubc = new SubCatPerPakket
                                {
                                    PakketNR = GekozenPakket.Pakket_ID,
                                    SubCatNR = scb.SCB_ID,
                                    ID = scppID,
                                    ProductSetNR = "",
                                    CategorieNR = scb.C_NR,
                                    TotaalMeters = GetAccMeters(AanvullendeMeters)
                                };
                                results.Add(curSubc);
                            }

                            var bewerkingAcc = (from bewerk in ctx.Nabewerkingens
                                                join nbso in ctx.NabewerkingSetOpbouws on bewerk.N_ID equals nbso.N_NR
                                                join nbst in ctx.NabewerkingSets on nbso.NS_NR equals nbst.NS_ID
                                                where nbst.NS_ID == exMtr.NabewerkingSetNR
                                                select new Nabewerking
                                                {
                                                    Nabewerking_ID = bewerk.N_ID,
                                                    LinkedSubcatNr = "Sub4Accent" + scb.SCB_ID,//exMtr.LinkedSubCat,
                                                    BewerkingCat_NR = bewerk.NC_NR,
                                                    Omschrijving = bewerk.omschrijving,
                                                    volgorde = (int)bewerk.volgorde,
                                                    TextVoorZin = bewerk.TextVoorZin,
                                                    NabewerkingSetNr = exMtr.NabewerkingSetNR

                                                });
                            foreach (Nabewerking n in bewerkingAcc)
                            {
                                if (curSubc.Children.FirstOrDefault(x => x.GetType() == typeof(Nabewerking) &&
                                                                   (x as Nabewerking).Nabewerking_ID == n.Nabewerking_ID &&
                                                                   (x as Nabewerking).NabewerkingSetNr == n.NabewerkingSetNr) == null)
                                {
                                    curSubc.Children.Add(n);
                                }

                            }
                            //  results.Add(subc);   
                        }
                                            
                    }
                   
                                          
                }
            }
                return results;
            
		}
        private double GetAccMeters(List<ExtraMeters> mtrs)
        {
            var cumuMeters = 0.0;
            foreach (var m in mtrs)
            {
                if (!m.IsBasis)
                {
                    cumuMeters += m.Meters;
                }
            }
            return cumuMeters;
        }
        public static Web.SubCats GenerateScbNr4Accent(string SCBNR)
        {
            var ctx = Acumulator.Instance().ctx;
            var scbNr2Get = "";
            //MessageBox.Show(SCBNR);
            switch (SCBNR)
            {
                case "SCB1":
                    scbNr2Get = "SCB15";
                    break;
                case "SCB2":
                    scbNr2Get = "SCB16";
                    break;
                case "SCB3":
                case "SCB10":
                case "SCB12":
                    scbNr2Get = "SCB17";
                    break;
                case "SCB13":
                    scbNr2Get = "SCB18";
                    break;
            }
            var acSCB = (from scb in ctx.SubCats
                         where scb.SCB_ID == scbNr2Get
                         select scb).FirstOrDefault()
                         ;
            return acSCB;
        }
		public List<SubCatPerPakket> qryHoekprofielen(eBrochureDomainContext ctx, PakketKeuze pk)
		{
			List<SubCatPerPakket> Results = new List<SubCatPerPakket>();
			var R_ids = (from ok in GekozenOpties
						 orderby ok.OptieID
						 select ok.OptieID).ToList();
			R_ids.Add(this.RuimteID);
			var Keuzes = (from srd in ctx.SubCatPerRuimteDeels
						  where R_ids.Contains(srd.R_NR)
						  orderby srd.SCB_NR
						  select srd.SCB_NR).Distinct().ToList();


			var subcats = (from scp in ctx.SubCatPerPakkets
						   join scb in ctx.SubCats on scp.SCB_NR equals scb.SCB_ID
						   where scp.P_NR == GekozenPakket.Pakket_ID && Keuzes.Contains(scp.SCB_NR)
						   orderby scb.volgorde
						   select new SubCatPerPakket() { PakketNR = scp.P_NR, SubCatNR = scp.SCB_NR, ID = scp.SCBP_ID, ProductSetNR = scp.PD_SET_NR, CategorieNR=scb.C_NR });
			//get all meters
			var saldi = GetSaldoMeters();
			foreach (SubCatPerPakket subc in subcats)
			{
				subc.TotaalMeters = saldi[subc.SubCatNR];
                var rsset = (from scprd in ctx.SubCatPerRuimteDeels
                            where R_ids.Contains(scprd.R_NR) && scprd.SCB_NR == subc.SubCatNR && scprd.R_NR != this.RuimteID
                            select scprd.R_NR).FirstOrDefault();
                var rids = (from rop in ctx.RuimteOpbouws
                            where rop.RS_NR == (from rs in ctx.RuimteOpbouws
                                                where rs.R_NR == rsset
                                                select rs.RS_NR).FirstOrDefault()
                            select rop.R_NR).ToList();
                double minmeters;
                if (rids.Count > 0)
                {
                     minmeters = (double)(from srd in ctx.SubCatPerRuimteDeels
                                             where rids.Contains(srd.R_NR) && srd.SCB_NR == subc.SubCatNR
                                             select Math.Abs((double)srd.meters)).Min();
                }
                else
                {
                     minmeters = (double)(from srd in ctx.SubCatPerRuimteDeels
                                             where R_ids.Contains(srd.R_NR) && srd.SCB_NR == subc.SubCatNR
                                             select Math.Abs((double)srd.meters)).Min();
                }

				if (saldi[subc.SubCatNR] > 0 && saldi[subc.SubCatNR] >= Math.Abs(minmeters))
				{
					if (subc.SubCatNR == "SCB3" || subc.SubCatNR == "SCB10" || subc.SubCatNR == "SCB12")
					{
						var hoekprofielen = from prdct in ctx.Productens
										join pso in ctx.ProductSetOpbouws on prdct.PD_ID equals pso.PD_NR
										join pst in ctx.ProductSets on pso.PD_SET_NR equals pst.PD_SET_ID
										join pc in ctx.ProductCategoriëns on prdct.PC_NR equals pc.PC_ID
										where pst.PD_SET_ID == subc.ProductSetNR
										select new HoekProfiel(prdct.PD_ID,prdct.Omschrijving)
										{
											Meters = saldi[subc.SubCatNR],
											ProfielImg = prdct.ImagePath,
											volgorde = (int)prdct.volgorde,
											VerpakkingsToeslag = (double)prdct.verpakkingstoeslag,
											LinkedSubCat = subc.ID,
											Kleur = (from kleur in ctx.Kleurens
													 where kleur.K_ID == prdct.PKC_NR
													 select kleur.Omschrijving).FirstOrDefault().ToString(),
													 KleurVolgorde = (int)(from kleur in ctx.Kleurens
																	  where kleur.K_ID == prdct.PKC_NR
																	  select kleur.volgorde).FirstOrDefault(),
											ProfielType = (from hpt in ctx.HoekProfielTypes
														   where hpt.PD_NR == prdct.PD_ID
														   select hpt.SoortProfiel).FirstOrDefault(),
														   KleurCode = prdct.PKC_NR
											//soort!!
										};
						foreach (HoekProfiel h in hoekprofielen)
						{
							subc.Children.Add(h);
						}
						var comp1 = new Helpers.CustomComparer<IOfferte>();
						subc.Children.Sort(comp1);
						Results.Add(subc);
					}
				}
				
			}
			return Results;
		}
		private void qryPakketten(eBrochureDomainContext ctx)
		{
			var projnr = Acumulator.Instance().Projectnr;
			if (projnr != null && projnr != String.Empty)
			{

                //debug segment
                //var pakketten = (from p in ctx.Pakkettens
                //                 select p).ToList();
                //var pgos = (from pn in ctx.PakketGroepOpbouws
                //            select pn).ToList();
                //var pgns = (from pgn in ctx.Bron_Ruimtes
                //           select pgn).ToList();

                //var stap1 = (from p in pakketten
                //             join pn in pgos on p.P_ID equals pn.P_NR
                //             select new { p.P_ID, pn.PGO_ID,pn.PG_NR }).ToList();

                //var stap2 = (from p in pakketten
                //             join pn in pgos on p.P_ID equals pn.P_NR
                //             join pgn in pgns on pn.PG_NR equals pgn.PG_NR
                             
                //             select new { p.P_ID,pn.P_NR,pn.PG_NR,pgn.BR_ID}
                //             ).ToList();
                //end debug segment

				var pnrs = (from p in ctx.Pakkettens
                            join pn in ctx.PakketGroepOpbouws on p.P_ID equals pn.P_NR
						   join pgn in ctx.Bron_Ruimtes on pn.PG_NR equals pgn.PG_NR
						   where pgn.R_NR == this.RuimteID
						   select new PakketKeuze(p.P_ID,p.PP_NR,pn.Omschrijving));
				/*var pakketten = (from p in ctx.Pakkettens
								 join ppr in ctx.PakketGroepOpbouws on p.P_ID equals ppr.P_NR
								 where pnrs.Contains(p.P_ID)
								 select new PakketKeuze(p.P_ID,  p.PP_NR, ppr.Omschrijving ));*/
                
				foreach (var x in pnrs)
				{
					Add(x);
				}
			}
		}
		private void qryGetRuimteOpties(eBrochureDomainContext ctx)
		{

			//standaard ruimte keuzes
			var r_opb = from ro in ctx.RuimteOpbouws
						where ro.R_NR == RuimteID
						select ro.RS_NR;
			
			var std_r_opb = (from ro2 in ctx.RuimteOpbouws
							join sr in ctx.StandaardRuimteOpbouws on ro2.R_NR equals sr.R_NR
							where r_opb.Contains(ro2.RS_NR) && ro2.R_NR != RuimteID
							select ro2.R_NR).Distinct();
            var r_opt =  new List<Web.RuimteDelen>();
            foreach (var x in std_r_opb)
            {
                var bla = (from rd in ctx.RuimteDelens
                           where rd.R_ID == x
                           select new Web.RuimteDelen() { Omschrijving = rd.Omschrijving, R_ID = rd.R_ID, Omschrijving_Prijs = rd.Omschrijving_Prijs, OT_NR = rd.OT_NR }).FirstOrDefault();
               r_opt.Add(bla);
            }
		/*	var r_opt = (from rd in ctx.RuimteDelens
						where std_r_opb.Contains(rd.R_ID)
						select rd).ToList();*/
			var post_opt = from o in r_opt
						   where !(from p in ctx.StandaardOnafhandelijkeRuimteOpbouws
								   join sro in ctx.StandaardRuimteOpbouws on p.SRO_NR equals sro.SRO_ID
								   select sro.R_NR).Contains(o.R_ID)
						   select o;
			var pre_opt = from o in r_opt
						  where (from p in ctx.StandaardOnafhandelijkeRuimteOpbouws
									  join sro in ctx.StandaardRuimteOpbouws on p.SRO_NR equals sro.SRO_ID
								   select sro.R_NR).Contains(o.R_ID)
						  select o;
			var indie = from op in pre_opt
						where (	   from sr in ctx.StandaardRuimteOpbouws
								   join i in ctx.StandaardOnafhandelijkeRuimteOpbouws on sr.SRO_ID equals i.SRO_NR
								   select sr.R_NR).Contains(op.R_ID)
						select op.R_ID;
			foreach (var x in pre_opt)
			{
				if (indie.Contains(x.R_ID))
				{
					Add(new OptieKeuze(x.R_ID, x.Omschrijving)
					{
						OptType = OptieType.Independant,
						OptieSoort = x.OT_NR
					});
				}
				else
				{
					Add(new OptieKeuze(x.R_ID, x.Omschrijving)
					{
						OptType = OptieType.Determinating,
						OptieSoort = x.OT_NR
					});
				}
				
			}
			foreach (var x in post_opt)
			{
				Add(new OptieKeuze(x.R_ID, x.Omschrijving)
					{
						OptType = OptieType.Determinating,
						OptieSoort = x.OT_NR			
					});
			}
			//bezig met standaard. check op pagina bij submitBtn. Dan laden opties voor gekozen configuratie
			//var r_opties = from rd in ctx.RuimteDelens
			//               where (from ro2 in ctx.RuimteOpbouws
			//                      where (from ro in ctx.RuimteOpbouws
			//                             where ro.R_NR == RuimteID
			//                             select ro.RS_NR).Contains(ro2.RS_NR)
			//                      select ro2.R_NR).Contains(rd.R_ID) && rd.R_ID != RuimteID
			//               select new OptieKeuze(rd.R_ID, rd.Omschrijving);
			//foreach (var x in r_opties)
			//	{
			//		Add(x);
			//	}
			
		}
		public List<IOfferte> GetVervolgOpties(eBrochureDomainContext ctx, List<OptieKeuze> bekend)
		{
			List<IOfferte> Results = new List<IOfferte>();
			var opties = bekend.Where(b => b.OptType != OptieType.Independant && b.OptType != OptieType.Resulting);
			var optiesIDs = (from gekop in opties
						   select gekop.OptieID).ToList<string>();
			optiesIDs.Add(RuimteID);
			var optiesets = (from os in ctx.RuimteOpbouws
							 where optiesIDs.Contains(os.R_NR)
							 select os.RS_NR).Distinct();
			string set_ID = String.Empty;
			foreach (var rsnr in optiesets)
			{
				var foo = from r in ctx.RuimteOpbouws
						  join st in ctx.StandaardRuimteOpbouws on r.R_NR equals st.R_NR
						  where r.RS_NR == rsnr && !(from i in ctx.StandaardOnafhandelijkeRuimteOpbouws
													select i.SRO_NR).Contains(st.SRO_ID)
						  select r.R_NR;
				var bar = optiesIDs.Intersect(foo).ToList();
				if (optiesIDs.Count == bar.Count && bar.Count == foo.Count())
				{
					set_ID = rsnr;
				}
			}
			var xtra_opt = from rop in ctx.RuimteOpbouws
						   where rop.RS_NR == set_ID && !optiesIDs.Contains(rop.R_NR)
						   select rop.R_NR;
			var resulting_opties = from rd in ctx.RuimteDelens
						 where xtra_opt.Contains(rd.R_ID) && rd.R_ID != RuimteID
						 select rd;
			foreach (var x in resulting_opties)
			{
				Results.Add(new OptieKeuze(x.R_ID, x.Omschrijving)
				{
					OptType = OptieType.Resulting,
					standaard = false,
					OptieSoort = x.OT_NR
				});
			}
			return Results;
		}
		private bool qryGetRuimteOptiesFase2(eBrochureDomainContext ctx, KeuzeControls.KeuzeList keuzelist)
		{
			bool  retval = false;
			var o = GekozenOpties.Where(op => (op as OptieKeuze).OptType != OptieType.Independant && (op as OptieKeuze).OptType != OptieType.Resulting);
			var optiesIDs = (from gekop in o
							 select gekop.OptieID).ToList<string>();
			optiesIDs.Add(RuimteID);
			// get resulting opties van keuzes
			var optiesets = (from os in ctx.RuimteOpbouws
							where optiesIDs.Contains(os.R_NR)
							select os.RS_NR).Distinct();
			string set_ID = String.Empty;
			foreach (var rsnr in optiesets)
			{
				var foo = from r in ctx.RuimteOpbouws
						  join st in ctx.StandaardRuimteOpbouws on r.R_NR equals st.R_NR
						  where r.RS_NR == rsnr
						  select r.R_NR;
				var bar = optiesIDs.Intersect(foo).ToList();
				if (optiesIDs.Count == bar.Count && bar.Count == foo.Count())
				{
					set_ID = rsnr;
				}
			}
		
		// haal uitgebreidere opties bij de set	  
			var xtra_opt = from rop in ctx.RuimteOpbouws
						   where rop.RS_NR == set_ID && !optiesIDs.Contains(rop.R_NR)
						   select rop.R_NR;
			var opties = from rd in ctx.RuimteDelens
						 where xtra_opt.Contains(rd.R_ID) && rd.R_ID != RuimteID
						 select rd;
			//remove existing
			var res = from r in Children
					  where r.GetType() == typeof(OptieKeuze) && (r as OptieKeuze).OptType == OptieType.Resulting
					  select r as OptieKeuze;
			Stack<OptieKeuze> todel = new Stack<OptieKeuze>();
			foreach (var x in res)
			{
				todel.Push(x);
			}
			while (todel.Count >0)
			{
				Children.Remove(todel.Pop());
			}
			foreach (var x in opties)
			{
				retval = true;
				Add(new OptieKeuze(x.R_ID, x.Omschrijving)
					{
						OptType = OptieType.Resulting,
						standaard = false
					});
			}
			keuzelist.ReloadDetail();
			return retval;
		}
		private void qryGetSubCategories(eBrochureDomainContext ctx)
		{
			
			var subs = (from spr in ctx.SubCatPerRuimteDeels
						where spr.R_NR == RuimteID
						select spr);
			Dictionary<string, double> saldi_sub = new Dictionary<string, double>();
			foreach (var s in subs)
			{
				if (saldi_sub.ContainsKey(s.SCB_NR))
				{
					saldi_sub[s.SCB_NR] += (double)s.meters;
				}
				else
				{
					saldi_sub.Add(s.SCB_NR,(double)s.meters);
				}
			}
			var s_sub = (from spr in ctx.SubCatPerRuimteDeels
						join scb in ctx.SubCats on spr.SCB_NR equals scb.SCB_ID
						where spr.R_NR == RuimteID//saldi_sub.ContainsKey(scb.SCB_ID)
						select new SubCatPerRuimteDeel()
 						{
 							ID= spr.SCPR_ID,
 							SubCatNR = spr.SCB_NR,
 							RuimteDeelNR = spr.R_NR,
 							Meters = saldi_sub[scb.SCB_ID],
 							Omschrijving = scb.Omschrijving
 						});


			 foreach (var x in s_sub)
				{

					Add(x);
				}
			

		}
		
		
	}
}

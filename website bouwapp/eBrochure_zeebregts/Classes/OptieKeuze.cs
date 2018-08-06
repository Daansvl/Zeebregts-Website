using System;
using System.Linq;
using eBrochure_zeebregts.Web.Services;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace eBrochure_zeebregts.Classes
{
	public class OptieKeuze : IOfferte, IXmlSerializable
	{
		public XmlSchema GetSchema() { return null; }
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("OptieID", OptieID);
			writer.WriteAttributeString("Omschrijving", Omschrijving);
			writer.WriteAttributeString("standaard", standaard.ToString());
			writer.WriteAttributeString("BasisPrijs", BasisPrijs.ToString());
			writer.WriteAttributeString("PakketPg", PakketPg);
			writer.WriteAttributeString("OptieSoort", OptieSoort);
			writer.WriteAttributeString("OptType", OptType.ToString());



		}
		public void ReadXml(XmlReader reader)
		{
			OptieID = reader.GetAttribute("OptieID");
			Omschrijving = reader.GetAttribute("Omschrijving");
			standaard = bool.Parse(reader.GetAttribute("standaard"));
			BasisPrijs = double.Parse(reader.GetAttribute("BasisPrijs"));
			PakketPg = reader.GetAttribute("PakketPg");
			OptieSoort = reader.GetAttribute("OptieSoort");
			OptType = (OptieType)Enum.Parse(typeof(OptieType), reader.GetAttribute("OptType"),true);
			qrySubCat(Acumulator.Instance().ctx);
		}

		private OptieType opttype;
		public OptieType OptType
		{ 
			get { return opttype; }
			set {
					opttype = value;
					laad_verder();
					
				}
		}
		public string OptieID
		{ get;  set; }
		public string Omschrijving
		{ get;  set; }
		public bool standaard
		{ get; set; }
		public double BasisPrijs
		{ get;  set; }
		public string PakketPg
		{ get;  set; }
		public string OptieSoort
		{ get; set; }

		public OptieKeuze() { init(); }
		public OptieKeuze(string id, string omsch)
		{
			OptieID = id;
			Omschrijving = omsch;
			init();
			
		}
		private void init()
		{
			eBrochureDomainContext ctx = Acumulator.Instance().ctx;
			var ppgnrs = from mpro in ctx.MeerPrijzenRuimteOpties
						 where mpro.R_NR == OptieID && mpro.meerprijs > 0
						 select mpro;
			string Pakpg = "";
			double prijs = 9999999;
			foreach (var mpro in ppgnrs)
			{
				if ((double)mpro.meerprijs < prijs)
				{
					prijs = (double)mpro.meerprijs;
					Pakpg = mpro.PP_NR;
				}
			}
			PakketPg = Pakpg;
			OptType = OptieType.Unknown;
			qrySubCat(Acumulator.Instance().ctx);
			
		}
		public void laad_verder()
		{
				MeerprijsInPakket(Acumulator.Instance().ctx);
				
		}
		public double getMeerprijs(eBrochureDomainContext ctx, string pakketprijs_nr)
		{
			var mp = (from m in ctx.MeerPrijzenRuimteOpties
					  where m.R_NR == OptieID && m.PP_NR == pakketprijs_nr
					  select m.meerprijs).FirstOrDefault();
			double prijs = 0;
			if (mp != null)
			{
				prijs = Acumulator.Instance().BerekenEindPrijs((double)mp);
               
			}
			return prijs;
		}
		public void MeerprijsInPakket(eBrochureDomainContext ctx)
		{
			
				
				var mp = from m in ctx.MeerPrijzenRuimteOpties
						 where m.R_NR == OptieID && m.PP_NR == PakketPg
						 select m.meerprijs;
					foreach (var x in mp)
					{
						BasisPrijs = (double)x;
					}
					BasisPrijs = Acumulator.Instance().BerekenEindPrijs(BasisPrijs);
		}
		public void qrySubCat(eBrochureDomainContext ctx)
		{
			
			
			var subcats = (from sc in ctx.SubCatPerRuimteDeels
							   where sc.R_NR == OptieID
							   select new SubCatPerRuimteDeel()
							   {
								   ID = sc.SCPR_ID,
								   RuimteDeelNR = sc.R_NR,
								   SubCatNR = sc.SCB_NR,
								   Meters = (double)sc.meters
							   });
				foreach (var x in subcats)
				{
					Add(x);
				}
		}
		

	}
}

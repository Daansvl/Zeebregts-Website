using System.Linq;
using eBrochure_zeebregts.Web.Services;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace eBrochure_zeebregts.Classes
{
	public class PakketKeuze : IOfferte, IXmlSerializable
	{
		public XmlSchema GetSchema(){return null;}
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("Pakket_ID", Pakket_ID);
			writer.WriteAttributeString("PakketPrijsgroep_NR", PakketPrijsgroep_NR);
			writer.WriteAttributeString("Omschrijving", Omschrijving);
		}
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			Pakket_ID = reader.GetAttribute("Pakket_ID");
			PakketPrijsgroep_NR = reader.GetAttribute("PakketPrijsgroep_NR");
			Omschrijving = reader.GetAttribute("Omschrijving");
			Getsubs(Acumulator.Instance().ctx);
		}
		public double PrijsHuidig
		{ get; set; }

		public PakketKeuze() { }
		public PakketKeuze(string id, string pg, string oms)
		{
			Pakket_ID = id;
			PakketPrijsgroep_NR = pg;
			Omschrijving = oms;
			Getsubs(Acumulator.Instance().ctx);
        }
		public string Pakket_ID
		{ get; set; }
        public string PakketPrijsgroep_NR
		{ get; set; }
		public string Omschrijving
		{ get; set; }
		public void GetPrijs(string rid,eBrochureDomainContext ctx)
		{
				double PakketPrijs = 0;
				var prijzen = from mpro in ctx.MeerPrijzenRuimteOpties
							  where mpro.PP_NR == PakketPrijsgroep_NR && rid == mpro.R_NR
							  select mpro.meerprijs;
				foreach (decimal mp in prijzen)
				{
					PakketPrijs += (double)mp;
				}
				PakketPrijs = Acumulator.Instance().BerekenEindPrijs(PakketPrijs);
				PrijsHuidig = PakketPrijs;
		}
		private void Getsubs(eBrochureDomainContext ctx)
		{
			var subs = from scp in ctx.SubCatPerPakkets
					   where scp.P_NR == Pakket_ID
					   select new SubCatPerPakket() { PakketNR = scp.P_NR, SubCatNR = scp.SCB_NR, ID = scp.SCBP_ID, ProductSetNR = scp.PD_SET_NR };
			foreach (var x in subs)
			{
				Add(x);
			}
		}

	}
}

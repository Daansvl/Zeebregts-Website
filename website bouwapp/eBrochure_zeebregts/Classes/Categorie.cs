using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Windows;

namespace eBrochure_zeebregts.Classes
{
	public class Categorie : IOfferte
	{
		public string ID
		{ get; set; }
		public string Omschrijving
		{ get; set; }
		public int Volgorde
		{ get; set; }

		public Categorie()
		{
		}

		
	}
	public class SubCat : Categorie
	{
		public string CategorieNR
		{ get; set; }
		public string EenheidMeters
		{ get; set; }
		public SubCat()
		{

		}
	}
	public class SubCatPerRuimteDeel : SubCat, IXmlSerializable
	{
		public XmlSchema GetSchema() { return null; }
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("RuimteDeelNR", RuimteDeelNR);
			writer.WriteAttributeString("SubCatNR", SubCatNR);
			writer.WriteAttributeString("Meters", Meters.ToString());
			//
			writer.WriteAttributeString("CategorieNR", CategorieNR);
			writer.WriteAttributeString("EenheidMeters", EenheidMeters);
			//
			writer.WriteAttributeString("ID", ID);
			writer.WriteAttributeString("Omschrijving", Omschrijving);
			writer.WriteAttributeString("Volgorde", Volgorde.ToString());
		}
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			RuimteDeelNR = reader.GetAttribute("RuimteDeelNR");
			SubCatNR = reader.GetAttribute("SubCatNR");
			double d;
			
			if (double.TryParse(reader.GetAttribute("Meters"),out d))
			{
				Meters = d;
			}
			//

			CategorieNR = reader.GetAttribute("CategorieNR");
			EenheidMeters = reader.GetAttribute("EenheidMeters");
			ID = reader.GetAttribute("ID");
			Omschrijving = reader.GetAttribute("Omschrijving");
			int i;
			if (int.TryParse(reader.GetAttribute("Volgorde"),out i))
			{
				Volgorde = i;
			}
		}
		public string RuimteDeelNR
		{ get; set; }
		public string SubCatNR
		{ get; set; }
		public double Meters
		{ get; set; }
		public SubCatPerRuimteDeel()
		{

		}
		
	}
	public class SubCatPerPakket : SubCat, IXmlSerializable
	{
		public XmlSchema GetSchema() { return null; }
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("PakketNR", PakketNR);
			writer.WriteAttributeString("SubCatNR", SubCatNR);
			writer.WriteAttributeString("Meerprijs", Meerprijs.ToString());
			writer.WriteAttributeString("VerpakkingsToeslag", VerpakkingsToeslag.ToString());
			writer.WriteAttributeString("TotaalMeters", TotaalMeters.ToString());
			//
			writer.WriteAttributeString("CategorieNR", CategorieNR);
			writer.WriteElementString("EenheidMeters", EenheidMeters);
			//
			writer.WriteAttributeString("ID", ID);
			writer.WriteAttributeString("Omschrijving", Omschrijving);
			writer.WriteAttributeString("Volgorde", Volgorde.ToString());
		}
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			PakketNR = reader.GetAttribute("PakketNR");
			SubCatNR = reader.GetAttribute("SubCatNR");
			Meerprijs = double.Parse(reader.GetAttribute("Meerprijs"));
			VerpakkingsToeslag = double.Parse(reader.GetAttribute("VerpakkingsToeslag"));
			TotaalMeters = double.Parse(reader.GetAttribute("TotaalMeters"));
			//
			CategorieNR = reader.GetAttribute("CategorieNR");
			EenheidMeters = reader.GetAttribute("EenheidMeters");
			ID = reader.GetAttribute("ID");
			Omschrijving = reader.GetAttribute("Omschrijving");
			Volgorde = int.Parse(reader.GetAttribute("Volgorde"));
		}
		public string PakketNR
		{ get; set; }
        private string subcatnr;
		public string SubCatNR
        { 
            get 
            {
                return subcatnr; 
            }
            set
            {
                subcatnr = value;
                if (subcatnr.StartsWith("SCBP"))
                {
                    LogHelper.SendLog("Base Class subcatnr not good: " + subcatnr, LogType.error);
                    //MessageBox.Show("HERE IS A PROBLEM"); 
                }
            }
        }
		public string ProductSetNR
		{ get; set; }
		public double Meerprijs
		{ get; set; }
		public double VerpakkingsToeslag
		{ get; set; }
		public SubCatPerPakket()
		{
		}
		public double TotaalMeters
		{ get; set; }
		
	}
}

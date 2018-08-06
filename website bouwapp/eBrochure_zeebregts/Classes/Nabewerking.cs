using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace eBrochure_zeebregts.Classes
{
	public class Nabewerking : IOfferte, IXmlSerializable
	{
		public XmlSchema GetSchema() { return null; }
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("Nabewerking_ID", Nabewerking_ID);
			writer.WriteAttributeString("BewerkingCat_NR", BewerkingCat_NR);
			writer.WriteAttributeString("Omschrijving", Omschrijving);
			writer.WriteAttributeString("volgorde",volgorde.ToString());
			writer.WriteAttributeString("TextVoorZin", TextVoorZin);
			writer.WriteAttributeString("LinkedSubcatNr", LinkedSubcatNr);
            writer.WriteAttributeString("NabewerkingSetNr", NabewerkingSetNr);
            writer.WriteAttributeString("Kenmerk", Kenmerk);
		}
		public void ReadXml(XmlReader reader)
		{
			Nabewerking_ID = reader.GetAttribute("Nabewerking_ID");
			BewerkingCat_NR = reader.GetAttribute("BewerkingCat_NR");
			Omschrijving = reader.GetAttribute("Omschrijving");
			volgorde = int.Parse(reader.GetAttribute("volgorde"));
			TextVoorZin = reader.GetAttribute("TextVoorZin");
			LinkedSubcatNr = reader.GetAttribute("LinkedSubcatNr");
            NabewerkingSetNr = reader.GetAttribute("NabewerkingSetNr");
            Kenmerk = reader.GetAttribute("Kenmerk");
		}

		public string Nabewerking_ID
		{ get; set; }
		public string BewerkingCat_NR
		{ get; set; }
		public string Omschrijving
		{ get; set; }
		public int volgorde
		{ get; set; }
		public string TextVoorZin
		{ get; set; }
		public string LinkedSubcatNr
		{
			get;
			set;
		}
        public string NabewerkingSetNr { get; set; }


        public string Kenmerk { get; set; }
	}
}

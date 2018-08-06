using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using eBrochure_zeebregts.ExpertControls;

namespace eBrochure_zeebregts.Classes
{
	public class HoekProfiel : IOfferte, IXmlSerializable
	{
		public XmlSchema GetSchema() { return null; }
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("ProfielType", ProfielType);
			writer.WriteAttributeString("ProfielImg", ProfielImg);
			writer.WriteAttributeString("Omschrijving", Omschrijving);
			writer.WriteAttributeString("ProfielID", ProfielID);
			writer.WriteAttributeString("LinkedSubCat", LinkedSubCat);
			writer.WriteAttributeString("Meters", Meters.ToString());
			writer.WriteAttributeString("volgorde", volgorde.ToString());
			writer.WriteAttributeString("VerpakkingsToeslag", VerpakkingsToeslag.ToString());
			writer.WriteAttributeString("KleurCode", KleurCode);
			writer.WriteAttributeString("Kleur", Kleur);
			writer.WriteAttributeString("KleurVolgorde", KleurVolgorde.ToString());
            writer.WriteAttributeString("Lengte", Lengte.ToString());
            writer.WriteAttributeString("Breedte", Breedte.ToString());
            writer.WriteAttributeString("IsExpertMode", IsExpertMode.ToString());

		}
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			ProfielType = reader.GetAttribute("ProfielType");
			ProfielImg = reader.GetAttribute("ProfielImg");
			Omschrijving = reader.GetAttribute("Omschrijving");
			ProfielID = reader.GetAttribute("ProfielID");
			LinkedSubCat = reader.GetAttribute("LinkedSubCat");
            //var tmpMtrs = 0.0;
            //double.TryParse(reader.GetAttribute("Meters"),out tmpMtrs);
            Meters = ExpertMethods.CultureDeliminatorConverter(reader.GetAttribute("Meters"));
            var tmpVlg = 0;
            int.TryParse(reader.GetAttribute("volgorde"),out tmpVlg);
            volgorde = tmpVlg;
            //var tmpVerpToes = 0.0;
            //double.TryParse(reader.GetAttribute("VerpakkingsToeslag"),out tmpVerpToes);
            VerpakkingsToeslag = ExpertMethods.CultureDeliminatorConverter(reader.GetAttribute("VerpakkingsToeslag"));
			KleurCode = reader.GetAttribute("KleurCode");
			Kleur = reader.GetAttribute("Kleur");
            var tmpKlVlg = 0;
            int.TryParse(reader.GetAttribute("KleurVolgorde"),out tmpKlVlg);
            KleurVolgorde = tmpKlVlg;

            int lenOut;
            if (int.TryParse(reader.GetAttribute("Lengte"), out lenOut))
            {
                Lengte = lenOut;
            }
            int breOut;
            if (int.TryParse(reader.GetAttribute("Breedte"), out breOut))
            {
                Breedte = breOut;
            }
            bool expmd;
            if(bool.TryParse(reader.GetAttribute("IsExpertMode"),out expmd))
            {
                IsExpertMode = expmd;
            }
		}


        public string ProfielID
        { get; set; }

        public string Omschrijving
        { get; set; }

        public int Lengte
        { get; set; }

        public int Breedte
        { get; set; }

        public string Kleur
        { get; set; }

        public string KleurCode
        { get; set; }

        public int KleurVolgorde
        { get; set; }

        public string ProfielImg
        { get; set; }

        public string LinkedSubCat
        { get; set; }

        public int volgorde
        { get; set; }
        
        public double VerpakkingsToeslag
        { get; set; }
		
		
        
        
        public string ProfielType
		{get;set;}
		
        public double Meters
		{ get; set; }

        public bool IsExpertMode { get; set; }
       
       
        
		public HoekProfiel() { }
		public HoekProfiel(string id, string oms)
		{
			ProfielID = id;
			Omschrijving = oms;
		}

	}
}

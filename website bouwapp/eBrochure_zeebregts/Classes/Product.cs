using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using eBrochure_zeebregts.ExpertControls;

namespace eBrochure_zeebregts.Classes
{
	public class Product : IOfferte, IXmlSerializable
	{
		public XmlSchema GetSchema() { return null; }
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("ID", ID);
			writer.WriteAttributeString("volgorde", volgorde.ToString());
			writer.WriteAttributeString("Omschrijving", Omschrijving);
			writer.WriteAttributeString("LinkedMainCat", LinkedMainCat);
			writer.WriteAttributeString("LinkedSubCat", LinkedSubCat);
			writer.WriteAttributeString("Prijs", Prijs.ToString());
			writer.WriteAttributeString("VerpakkingsToeslag", VerpakkingsToeslag.ToString());
			writer.WriteAttributeString("Lengte", Lengte.ToString());
			writer.WriteAttributeString("Breedte", Breedte.ToString());
			writer.WriteAttributeString("ImgPath", ImgPath);
			writer.WriteAttributeString("KleurCode", KleurCode);
			writer.WriteAttributeString("Kleur", Kleur);
			writer.WriteAttributeString("Naam", Naam);
			writer.WriteAttributeString("productcode", productcode);
			writer.WriteAttributeString("kleurvolgorde", kleurVolgorde.ToString());

		}
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			ID = reader.GetAttribute("ID");
            var tmpvolgorde = 0;
            int.TryParse(reader.GetAttribute("volgorde"), out tmpvolgorde);
            volgorde = tmpvolgorde;
			Omschrijving = reader.GetAttribute("Omschrijving");
			LinkedMainCat = reader.GetAttribute("LinkedMainCat");
			LinkedSubCat = reader.GetAttribute("LinkedSubCat");
            //var tmpPrijs = 0.0;
            //double.TryParse(reader.GetAttribute("Prijs"),out tmpPrijs);
            Prijs = ExpertMethods.CultureDeliminatorConverter(reader.GetAttribute("Prijs"));
            //var tmpToeslag = 0.0;
            //double.TryParse(reader.GetAttribute("VerpakkingsToeslag"),out tmpToeslag);
            VerpakkingsToeslag = ExpertMethods.CultureDeliminatorConverter(reader.GetAttribute("VerpakkingsToeslag"));
            var tmpLengt = 0;
            int.TryParse(reader.GetAttribute("Lengte"),out tmpLengt);
            Lengte = tmpLengt;
            var tmpBreedt = 0;
            int.TryParse(reader.GetAttribute("Breedte"),out tmpBreedt);
            Breedte = tmpBreedt;
			ImgPath = reader.GetAttribute("ImgPath");
			KleurCode = reader.GetAttribute("KleurCode");
			Kleur = reader.GetAttribute("Kleur");
			Naam = reader.GetAttribute("Naam");
			productcode = reader.GetAttribute("productcode");
            var tmpKlVlg = 0;
            int.TryParse(reader.GetAttribute("kleurvolgorde"),out tmpKlVlg);
            kleurVolgorde = tmpKlVlg;
		}

		
        public string ID
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
        
        public int kleurVolgorde
        { get; set; }
		
        public string ImgPath
        { get; set; }

        public string LinkedSubCat
        { get; set; }

        public int volgorde
        { get; set; }

        public double VerpakkingsToeslag
        { get; set; }
		








        public string LinkedMainCat
		{ get; set; }
       
        public double Prijs
		{ get; set; }
		
        public string Naam
		{ get; set; }
       
        public string productcode
		{ get; set; }
		
       
        
        
        public Product()
		{
			//initialiser
		}
		
	}
}

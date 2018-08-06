using eBrochure_zeebregts.ExpertControls;
using eBrochure_zeebregts.ExpertControls.Models;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace eBrochure_zeebregts.Classes
{
    public class ExtraMeters : IOfferte, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Meters", Meters.ToString());
            writer.WriteAttributeString("RegelIndex", RegelIndex.ToString());
            writer.WriteAttributeString("Opmerking", Opmerking);
            writer.WriteAttributeString("LinkedSubCat", LinkedSubCat);
            writer.WriteAttributeString("NabewerkingSetNR", NabewerkingSetNR);
            writer.WriteAttributeString("IsBasis", IsBasis.ToString());
            writer.WriteAttributeString("Meerprijs", Meerprijs.ToString());
            writer.WriteAttributeString("ProdSoort", ((int)ProdSoort).ToString());
            if (LinkedProduct != null)
            {
                writer.WriteStartElement("LinkedProduct");
                LinkedProduct.WriteXml(writer); 
                writer.WriteEndElement();
            }
           
            
            if (LinkedHoekProf != null)
            {
                writer.WriteStartElement("LinkedHoekProf");
                LinkedHoekProf.WriteXml(writer);
                writer.WriteEndElement();

            }

            

        }
        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            //double oM;
            //double.TryParse(reader.GetAttribute("Meters"), out oM);
            Meters = ExpertMethods.CultureDeliminatorConverter(reader.GetAttribute("Meters"));
            RegelIndex = int.Parse(reader.GetAttribute("RegelIndex"));
            Opmerking = reader.GetAttribute("Opmerking");
            LinkedSubCat = reader.GetAttribute("LinkedSubCat");
            NabewerkingSetNR = reader.GetAttribute("NabewerkingSetNR");
            bool isb;
            bool.TryParse(reader.GetAttribute("IsBasis"), out isb);
            IsBasis = isb;
            //double oMp;
            //double.TryParse(reader.GetAttribute("Meerprijs"), out oMp);
            //Meerprijs = oMp;
            Meerprijs = ExpertMethods.CultureDeliminatorConverter(reader.GetAttribute("Meerprijs"));
            int prodEnum;
            if (int.TryParse(reader.GetAttribute("ProdSoort"), out prodEnum))
            {
                ProdSoort = (ProductSoort)prodEnum;
            }
            reader.Read();
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "LinkedProduct")
            {
                while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "LinkedProduct")
                {
                    Product p = new Product();
                    p.ReadXml(reader);
                    LinkedProduct = p;
                    reader.Read();
                }
                if (reader.LocalName == "LinkedProduct")
                {
                    reader.ReadEndElement();
                }
            }

            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "LinkedHoekProf")
            {
                while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "LinkedHoekProf")
                {
                    HoekProfiel h = new HoekProfiel();
                    h.ReadXml(reader);
                    LinkedHoekProf = h;
                    reader.Read();
                }
                if (reader.LocalName == "LinkedHoekProf")
                {
                    reader.ReadEndElement();
                }
            }

        }
        public double Meters { get; set; }
        public int RegelIndex { get; set; }
        public string Opmerking { get; set; }
        public string LinkedSubCat { get; set; }
        public string NabewerkingSetNR { get; set; }
        public bool IsBasis { get; set; }
        public double Meerprijs { get; set; }
        public Product LinkedProduct { get; set; }
        public HoekProfiel LinkedHoekProf { get; set; }
        public ProductSoort ProdSoort { get; set; }
    }
}

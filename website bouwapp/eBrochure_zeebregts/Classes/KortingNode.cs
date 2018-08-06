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
using System.Xml.Serialization;

namespace eBrochure_zeebregts.Classes
{
    public class KortingNode : IOfferte, IXmlSerializable
    {

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            //AuthDate = null;
            Omschrijving = null;
            KortingBedrag = 0.0;

            reader.MoveToContent();
          


            var datestring = reader.GetAttribute("AuthDate");
            DateTime outdatetime;
            DateTime.TryParse(datestring, out outdatetime);
            if(outdatetime != null)
            {
                AuthDate = outdatetime;
            }
            //var date = reader.ReadElementContentAsDateTime();
            //AuthDate = date;
            
            Omschrijving = reader.GetAttribute("Omschrijving");
            
            double kn;
            var kortstring =reader.GetAttribute("KortingBedrag"); 
            kortstring = kortstring.Replace(",",System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            kortstring = kortstring.Replace(".",System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            double.TryParse(kortstring,out kn);
            if(kn != null)
            {
                KortingBedrag = kn;
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartAttribute("AuthDate");
            writer.WriteValue(AuthDate);
            writer.WriteEndAttribute();

            writer.WriteAttributeString("Omschrijving",Omschrijving);
            writer.WriteAttributeString("KortingBedrag",KortingBedrag.ToString());

        }


        public DateTime AuthDate
        { get; set; }
        public string Omschrijving
        { get; set; }
        public double KortingBedrag
        {get;set;}
    }
}

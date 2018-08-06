using System;
using System.Xml.Serialization;
using System.IO;
using eBrochure_zeebregts.Web;
using System.Linq;
using eBrochure_zeebregts.Web.Services;

namespace eBrochure_zeebregts.Classes
{
	public class SaveLoadXML
	{
		private Type[] typez = new Type[] { typeof(Ruimte), typeof(OptieKeuze), typeof(PakketKeuze), typeof(Product), typeof(HoekProfiel), typeof(Nabewerking), typeof(SubCatPerPakket), typeof(SubCatPerRuimteDeel) };
			
		public string SerializeOfferte(RuimteOfferte ro)
		{
			XmlSerializer xser = new XmlSerializer(ro.GetType(), typez);
			StringWriter sw = new StringWriter();
			var cntr = 0;
			Start:
			if (cntr < 3) {
				cntr++;
				try {
					xser.Serialize(sw, ro);
				}
				catch (Exception e) {
					LogHelper.SendLog(e.Message, LogType.error);
					goto Start;
				}
			}
			return sw.ToString();
		}
		private RuimteOfferte DeSerializeOfferte(string saved_xml)
		{
			XmlSerializer xdser = new XmlSerializer(typeof(RuimteOfferte), typez);
			StringReader sr = new StringReader(saved_xml);
			return (RuimteOfferte)xdser.Deserialize(sr);

		}
		public RuimteOfferte LoadOfferte(string xml)
		{
			return DeSerializeOfferte(xml);
		}
		public void SaveOfferte(RuimteOfferte ro,bool volledig,KeuzeControls.FinalStage FinSta)
		{
			var xml = SerializeOfferte(ro);
			//check xml vs saved xml using xml diff

			var ctx = Acumulator.Instance().ctx;
			OpgeslagenOffertes oo = new OpgeslagenOffertes();
            oo.timestamp = FinSta.PrintDatum;
            Acumulator.Instance().OfferteDatum = FinSta.PrintDatum;
            CompleteSaveOfferte(oo,xml,volledig,FinSta);
			//oo.timestamp = DateTime.Now;
           // Acumulator.Instance().OfferteDatum = DateTime.Now;
		
		}
        private void CompleteSaveOfferte(OpgeslagenOffertes oo, string xml,bool volledig,KeuzeControls.FinalStage FinSta)
        {
            var ctx = Acumulator.Instance().ctx;
            oo.Bouwnummer_NR = Acumulator.Instance().Bouwnr;
            oo.Xml_Value = xml;
            oo.Gebruiker = Acumulator.Instance().HuidigGebruiker.GebruikersNaam;
            oo.Volledig = volledig;
            oo.Gebruiker_ID = Acumulator.Instance().HuidigGebruiker.ID;
            oo.Prijs = Acumulator.Instance().InfoBar.totaalprijs.Text;
            ctx.OpgeslagenOffertes.Add(oo);
           
            Acumulator.Instance().SavedXml = xml;

            var oOL = new OpgeslagenOfferteLean();
            oOL.gebruiker = Acumulator.Instance().HuidigGebruiker.GebruikersNaam;
            oOL.Datum = (DateTime)oo.timestamp;
            oOL.B_ID = Acumulator.Instance().Bouwnr;
            oOL.Prijs = oo.Prijs;
            if (volledig)
            {
                if (Acumulator.Instance().oOL != null)
                {
                    oOL.VersieFull = Acumulator.Instance().oOL.VersieFull + 1;
                }
                else
                {
                    oOL.VersieFull = 1;

                }
                oOL.VersiePartial = 0;
                RemovePartialSaves();
            }
            else
            {
                if (Acumulator.Instance().oOL != null)
                {
                    oOL.VersiePartial = Acumulator.Instance().oOL.VersiePartial + 1;
                    oOL.VersieFull = Acumulator.Instance().oOL.VersieFull;
                }
                else
                {
                    oOL.VersiePartial = 1;
                    oOL.VersieFull = 0;
                }
                ctx.SubmitChanges();
            }
            Acumulator.Instance().oOL = oOL;
            FinSta.CurrentStatusBox.SetDataSaved();
            FinSta.CurrentStatusBox.Visibility = System.Windows.Visibility.Visible;
        }
        public void RemoveSave()
        {
            
            var ctx = Acumulator.Instance().ctx;
                ctx.Load(ctx.GetOpgeslagenOfferteByIdQuery(Acumulator.Instance().Bouwnr)).Completed += (sender, args) =>
                {
                    var saves = (from s in ctx.OpgeslagenOffertes
                                 orderby s.timestamp
                                select s);
                    while (saves.Count() >0)
                    {
                        if (saves.Last().Volledig == true)
                        {
                            ArchiefOffertes ao = new ArchiefOffertes();
                            ao.Bouwnummer_NR = saves.Last().Bouwnummer_NR;
                            ao.GebruikerID_Delete = Acumulator.Instance().HuidigGebruiker.ID;
                            ao.GebruikerID_Save = saves.Last().Gebruiker_ID;
                            ao.timestamp_save = saves.Last().timestamp;
                            ao.timestamp_remove = DateTime.Now;
                            ao.Xml_Value = saves.Last().Xml_Value;
                            ao.Versie = saves.Count().ToString();
                            if (saves.Last().Prijs != null)
                            {
                                ao.Prijs = saves.Last().Prijs;
                            }
                            ctx.ArchiefOffertes.Add(ao);
                        }
                        ctx.OpgeslagenOffertes.Remove(saves.Last());
                    
                    }
                    ctx.SubmitChanges();
                    
                    Acumulator.Instance().SavedXml = null;
                    Acumulator.Instance().oOL = null;
                };
            
        }
        public void RemovePartialSaves()
        {
            var ctx = Acumulator.Instance().ctx;
            ctx.Load(ctx.GetOpgeslagenOfferteByIdQuery(Acumulator.Instance().Bouwnr)).Completed += (sender, args) =>
            {
                var saves = (from s in ctx.OpgeslagenOffertes
                             where s.Volledig == false
                             select s);
                while (saves.Count() > 0)
                {
                    ctx.OpgeslagenOffertes.Remove(saves.Last());

                }
               ctx.SubmitChanges();
            };
            
        }
        
	}
}

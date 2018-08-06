using System;
using System.Linq;
using System.Collections.Generic;
using eBrochure_zeebregts.Web.Services;
using eBrochure_zeebregts.Helpers;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using eBrochure_zeebregts.Classes;

namespace eBrochure_zeebregts
{
	public class RuimteOfferte : IOfferte, IXmlSerializable
	{

		public XmlSchema GetSchema()
		{
			return null;
		}
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("Omschrijving", Omschrijving);
			foreach (Ruimte r in Children)
			{
				writer.WriteStartElement("Ruimte");
				r.WriteXml(writer);
				writer.WriteEndElement();
			}
			
			var list = Acumulator.Instance().BB.MakeCompleteList();
			if(list.Count > 0)
			{
				writer.WriteStartElement("CompleetLijst");
				int cntr = 0;
				foreach (string c in list)
				{
					writer.WriteAttributeString("C"+cntr.ToString(), c);
					cntr++;
				}
				writer.WriteAttributeString("Count",cntr.ToString());
				writer.WriteEndElement();
			}
            if(Korting != null)
            {
                writer.WriteStartElement("KortingInfo");
                Korting.WriteXml(writer);
                writer.WriteEndElement();
            }


		}
		public void ReadXml(XmlReader reader)
		{
			Children.Clear();
			reader.MoveToContent();
			Omschrijving = reader.GetAttribute("Omschrijving");

				reader.Read();
				if(reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Ruimte")
				{
					while(reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Ruimte")
					{
						Ruimte r = new Ruimte();
						r.ReadXml(reader);
						Children.Add(r);
						reader.Read();
					}
				} 
				if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "CompleetLijst")
				{
					List<string> tmplist = new List<string>();
					int cntr = int.Parse(reader.GetAttribute("Count"));
					for (int i = 0; i < cntr; i++)
					{
						tmplist.Add(reader.GetAttribute("C"+i.ToString()));
					}
					LoadedCompleet = tmplist;
					//if (reader.LocalName == "CompleetLijst")
					//{
					//    reader.ReadEndElement();
					//}
				}
                reader.Read();
            if(reader.MoveToContent()==XmlNodeType.Element && reader.LocalName == "KortingInfo")
            {
                var kn = new KortingNode();
                kn.ReadXml(reader);
                Korting = kn;
                reader.Read();
            }

			
		}
		public List<string> LoadedCompleet;
		public string Omschrijving
		{ get; set; }
        public KortingNode Korting
        { get; set; }
		public RuimteOfferte()
		{
			var tp = Acumulator.Instance().Type;
			if (tp != null)
			{

				var oms = (from bt in Acumulator.Instance().ctx.bouwTypes
						   where bt.T_ID == tp
						   select bt.Soort).FirstOrDefault().ToString();
					Omschrijving =oms;

					qryRuimtes(Acumulator.Instance().Type, Acumulator.Instance().ctx);
					//Acumulator.Instance().BB.reload_tree();
					//Acumulator.Instance().OTracker.VerderBouwen();
				
			}
		}
	
		private void qryRuimtes(string t_id, eBrochureDomainContext ctx)
		{
			//
			Children.Clear();
			if (t_id != null && t_id != String.Empty)
			{

                var rooms = (from r in ctx.RuimteDelens
                             join rpt in ctx.RuimtesPerTypes on r.R_ID equals rpt.R_NR
                             join br in ctx.Bron_Ruimtes on rpt.R_NR equals br.R_NR
                             where rpt.T_NR == t_id
                            // select r.R_ID).ToList();
							 select new Ruimte(r.R_ID, r.Omschrijving, (int)br.PLAFONDHOOGTE));

				foreach (var x in rooms)
					{
						Add(x);
					}
			}
		}
		public override TvNode MakeTree(TvNode TreeNode)
		{
			List<TvNode> Childs = new List<TvNode>();
			var bnr = "";
			if (Acumulator.Instance().Bouwnr != null)
			{
				bnr = (from b in Acumulator.Instance().ctx.Bouwnummers
					   where b.B_ID == Acumulator.Instance().Bouwnr
					   select b.Omschrijving).FirstOrDefault();
			}
			var thisnode = new TvNode("Overzicht: "+bnr,"ID"+Acumulator.GetNodeID(), this.GetType().Name, Childs, TreeNode);
			foreach (IOfferte of in Children)
			{
				Childs.Add(of.MakeTree(thisnode));
			}
			thisnode.Children = Childs;
			return thisnode;
		}
		public override void Add(IOfferte Off)
		{
			Children.Add(Off);
		}
		public override IOfferte Get(int OffId)
		{
			return Children[OffId];
		}
		public override void remove(IOfferte _Off)
		{
			Children.Remove(_Off);
		}
		

	}

}

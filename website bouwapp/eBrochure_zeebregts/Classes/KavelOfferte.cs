using System;
using System.Linq;
using System.Collections.Generic;
using eBrochure_zeebregts.Web.Services;
using eBrochure_zeebregts.Helpers;

namespace eBrochure_zeebregts
{
	public class KavelOfferte : IOfferte
	{
		public KavelOfferte()
		{
			BouwNummer = Acumulator.Instance().Bouwnr;
			Type = Acumulator.Instance().Type = qryType(Acumulator.Instance().ctx);
		}
		public string BouwNummer
		{
			get;
			private set;
		}
		public string Type
		{
			get;
			private set;
		}
		private string qryType(eBrochureDomainContext ctx)
		{
			if (BouwNummer != null)
			{
				string s = String.Empty;
				var type = (from t in ctx.Bouwnummers
							where t.B_ID == BouwNummer
							select t.T_NR);
				foreach (var item in type)
				{
					s = item;
				}
				return s;
			}
			else
			{
				return String.Empty;
			}
		}
		public override TvNode MakeTree(TvNode TreeNode)
		{
			List<TvNode> Childs = new List<TvNode>();
			foreach (IOfferte of in Children)
			{
				Childs.Add(of.MakeTree(TreeNode));
			}
			Childs.Add(new TvNode("Tot slot","ID"+Acumulator.GetNodeID(), "FinalStage",null,null));
			//TreeNode.Children.AddRange(Childs);
			Acumulator.Instance().BB.treeView1.ItemsSource = Childs;
			return null;
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

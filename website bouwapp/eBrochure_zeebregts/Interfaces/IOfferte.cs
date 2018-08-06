using System.Collections.Generic;
using eBrochure_zeebregts.Helpers;

namespace eBrochure_zeebregts
{
	public abstract class IOfferte
	{
		public List<IOfferte> Children = new List<IOfferte>();
		
		public bool IsEnabled = true;
	

		public virtual void Add(IOfferte Off)
		{
			Children.Add(Off);
		}
		public virtual IOfferte Get(int OffId)
		{
			return Children[OffId];
		}
		public virtual void remove(IOfferte _Off)
		{
			Children.Remove(_Off);
		}

		public virtual TvNode MakeTree(TvNode TreeCollection)
		{
			
			List<TvNode> Childs = new List<TvNode>();
			foreach (IOfferte of in Children)
			{
				Childs.Add(of.MakeTree(null));
			}
			var thisnode = new TvNode(this.GetType().Name,"ID"+Acumulator.GetNodeID(), this.GetType().Name,Childs,TreeCollection);
			return thisnode;
		}
	}
}

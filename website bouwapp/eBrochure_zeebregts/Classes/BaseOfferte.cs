using eBrochure_zeebregts.Helpers;

namespace eBrochure_zeebregts
{
	public class BaseOfferte : IOfferte
	{
		// List<IOfferte> Kavels = new List<IOfferte>();
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
		public override TvNode MakeTree(TvNode TreeNode)
		{
			//var thisnode = new TreeViewItem() { Header = this.GetType().Name, Tag = this.GetType().Name, Visibility= Visibility.Collapsed };
			foreach (IOfferte of in Children)
			{
				of.MakeTree(TreeNode);
			}
			return null;
		}


	}
}

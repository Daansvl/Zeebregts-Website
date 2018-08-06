using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.ComponentModel;

namespace eBrochure_zeebregts.Helpers
{
    public enum TvNodeStatus { Compleet, InvalidCompleet, Incompleet }
	public class TvNode:INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;
		public TvNode(string naam,string id, string typehandle, List<TvNode> children, TvNode parent)
		{
			NodeNaam = naam;
            NodeID = id;
			TypeHandle = typehandle;
			Compleet = false;
            Status = TvNodeStatus.Incompleet;
			Children = children;
			Parent = parent;
			Enabled = true;
			
			
		}
		public void ShowCompleet()
		{
            if (Status == TvNodeStatus.Compleet)
            {
                var reluri = new Uri("/Images/checkmark.png", UriKind.Relative);
                var absuri = new Uri("http://mybouwapp.nl/Images/UIplaatjes/checkmark.png", UriKind.Absolute);
                StatusImg = new BitmapImage(reluri);
                Enabled = true;
            }
            else if (Status == TvNodeStatus.InvalidCompleet)
            {
                var reluri = new Uri("/Images/GrayCheckmark.png", UriKind.Relative);
                StatusImg = new BitmapImage(reluri);
                Enabled = true;
            }
            else
            {
                var reluri = new Uri("/Images/cross.png", UriKind.Relative);
                var absuri = new Uri("http://mybouwapp.nl/Images/UIplaatjes/cross.png", UriKind.Absolute);
                StatusImg = new BitmapImage(reluri);
            }
		}
		public void HideCompleet()
		{
			StatusImg = null;
			if (Children == null && Parent != null)
			{
				int indx = Parent.Children.IndexOf(this);
				if (indx == 0 || Parent.Children[indx - 1].Status == TvNodeStatus.Compleet || Parent.Children[indx - 1].Status == TvNodeStatus.InvalidCompleet)
				{
					Enabled = true;
				}
				else
				{
					Enabled = false;
				}
			}
		}
		private string _typehandle;
		public string TypeHandle { get { return _typehandle; } set { _typehandle = value; OnPropertyChanged("TypeHandle"); } }
		private List<TvNode> _children;
		public List<TvNode> Children { get { return _children; } set { _children = value;  OnPropertyChanged("Children"); } }
		private string _nodenaam;
		public string NodeNaam { get { return _nodenaam; } set { _nodenaam = value; OnPropertyChanged("NodeNaam"); } }
        private string _nodeID;
        public string NodeID { get { return _nodeID; } set { _nodeID = value; OnPropertyChanged("NodeID"); } }
        private ImageSource _statusImg;
		public ImageSource StatusImg { get { return _statusImg; } set { _statusImg = value; OnPropertyChanged("StatusImg"); } }
		private bool _compleet;
		public bool Compleet { get { return _compleet; } set { _compleet = value; OnPropertyChanged("Compleet"); } }
        private TvNodeStatus _status;
        public TvNodeStatus Status { get { return _status; } set { _status = value; OnPropertyChanged("Status"); } }
		private TvNode _parent;
		public TvNode Parent { get { return _parent; } set {_parent = value; OnPropertyChanged("Parent"); } }
		private bool _enabled;
		public bool Enabled
		{ get { return _enabled; } set { _enabled = value; OnPropertyChanged("Enabled"); } }
		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}

using System;
using System.Windows;
using System.Windows.Controls;

namespace eBrochure_zeebregts.KeuzeControls
{
	public partial class DetailsWindow : ChildWindow
	{
		public DetailsWindow()
		{
			InitializeComponent();
		}
		public void LoadContent(UIElement control)
		{
			ContentPanel.Children.Add(control);
		}
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			Application.Current.RootVisual.SetValue(Control.IsEnabledProperty, true);
		}

        private void ScrollViewer_LostFocus(object sender, RoutedEventArgs e)
        {
           // MessageBox.Show("Lost scroll focus");
        }
		
	}
}


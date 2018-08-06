using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;

namespace eBrochure_zeebregts.Controls
{
	public partial class Page1 : Page
	{
		public Page1()
		{
			InitializeComponent();
		}

		// Executes when the user navigates to this page.
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
		}
		private void Convert2Img()
		{
			WriteableBitmap bitmap = new WriteableBitmap(480, 800);
			bitmap.Render(LayoutRoot, null);
			bitmap.Invalidate();
		}
	}
}

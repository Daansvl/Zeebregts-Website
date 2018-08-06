using System.Windows.Controls;

namespace eBrochure_zeebregts
{
	public abstract partial class BaseControl : UserControl
	{
		public BaseControl()
		{
			InitializeComponent();
		}
		public abstract bool SubmitPressed();
		
	}
}

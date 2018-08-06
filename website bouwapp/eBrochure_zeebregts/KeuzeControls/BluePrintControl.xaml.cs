using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;

namespace eBrochure_zeebregts.KeuzeControls
{
	public partial class BluePrintControl : UserControl, IBaseControl
	{
		public BluePrintControl()
		{
			InitializeComponent();
			
		}
        public void LoadImg(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                BitmapImage im = new BitmapImage();
                im.SetSource(ms);
                BlueprintIMG.Source = im;
            }
          
        }
		public void LoadImg(string path)
		{
			string apath = "http://mybouwapp.nl/Images/Blueprints/" + path;
			var u = new Uri(apath, UriKind.Absolute);
			BlueprintIMG.Source = new BitmapImage(u);
		}
		public void Clear4Submit()
		{
		}
		public bool SubmitPressed()
		{
			return true;
		}
		public void WijzigPressed()
		{
		}
	}
}

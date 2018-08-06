using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using eBrochure_zeebregts.Classes;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Globalization;
using System.Net;
using System.IO;

namespace eBrochure_zeebregts.KeuzeControls
{
	public partial class ProductKControl : UserControl, IBaseControl
	{
		public Product product;
		public HoekProfiel hoekprofiel;
		public ProductKControl()
		{
			InitializeComponent();
		}
		public void InitProduct(Product p)
		{
			product = p;
            if (p.Breedte < 3000 && p.Breedte > 1 && p.Lengte < 3000 && p.Lengte > 1)
            {
                productNR.Text = p.productcode.TrimStart(' ') + " " + p.Breedte.ToString().Substring(0, 2) + "x" + p.Lengte.ToString().Substring(0, 2) + "cm";
            }
            else
            {
                productNR.Text = p.productcode.TrimStart(' ');
            }
            productKleur.Text = p.Kleur.TrimStart(' ');
            if (p.Omschrijving.Contains('€') && !Acumulator.Instance().ProjFase.FilterDorpels)
            {
                var oms_parts = p.Omschrijving.Split('€');
                productKleur.Text += Environment.NewLine + "Let op: Meerprijs van €" + oms_parts[1];
            }
			
			var soort = (from sc in Acumulator.Instance().ctx.SubCats
						 join scp in Acumulator.Instance().ctx.SubCatPerPakkets on sc.SCB_ID equals scp.SCB_NR
						 where scp.SCBP_ID == p.LinkedSubCat
						 select sc.Omschrijving).FirstOrDefault();
			soortInfo.Text = soort;
			string path = "default image";
			if (p.ImgPath != null)
			{
				path = "http://mybouwapp.nl/Images/ProductImgStorage/" + p.ImgPath;
				//this.LoadImage(new Uri(path, UriKind.Absolute));
                GetImageStart(path);
			}
            
					
		}
		public void InitHoek(HoekProfiel h)
		{
			hoekprofiel = h;
			productNR.Text = hoekprofiel.Omschrijving;
			productKleur.Text = "";// hoekprofiel.Kleur.TrimStart(' ');
			/*string path = "default image";
			if (hoekprofiel.ProfielImg != null)
			{
				path = "https://mybouwapp.nl/Images/ProductImgStorage/" + hoekprofiel.ProfielImg;
				LoadImageHoek(new Uri(path, UriKind.Absolute));
			}*/
		}
		public void Clear4Submit()
		{ }
		public bool SubmitPressed()
		{
			return true;
		}
		public void WijzigPressed()
		{
			this.IsEnabled = true;
		}
		public void LoadImageHoek(Uri uri)
		{
			productIMG.Source = new BitmapImage(uri);
		}
		public void LoadImage(byte[] Bi)
        {
            BitmapImage im = new BitmapImage();
             using (MemoryStream ms = new MemoryStream(Bi, 0, Bi.Length))
                    {
                        
                         im.SetSource(ms);
                         productIMG.Source = im;
                    }
		
//			productIMG.RenderTransformOrigin = new Point(0, 1);
			double scale;
			if (product.Lengte > 3000 || product.Breedte > 3000)
			{
				scale = 0.2;
				this.Visibility = Visibility.Collapsed;
			}
			else
			{
				int breedte = product.Breedte / 10;
				int lengte = product.Lengte / 10;

                var osb = (from p in Acumulator.Instance().ctx.Productens
                           where p.PD_ID == product.ID
                           select p.OverrideScaleBreedte).FirstOrDefault();
                var osl = (from p in Acumulator.Instance().ctx.Productens
                           where p.PD_ID == product.ID
                           select p.OverrideScaleLengte).FirstOrDefault();
                if (osb != null)
                {
                    breedte = (int)osb / 10;
                }
                if (osl != null)
                {
                    lengte = (int)osl / 10;
                }

				int mfactor = 60;
				if (lengte >= 60 || breedte >= 60)
				{
					scale = 1;
				}
				else
				{
					if (lengte > breedte)
					{
						scale = ((double)(lengte * 2) + mfactor) / (120 + mfactor);
					}
					else
					{
						scale = ((double)(breedte * 2) + mfactor) / (120 + mfactor);
					}
				}
			}
            productIMG.Width = productIMG.Width * scale;
            productIMG.Height = productIMG.Height * scale;
			//ScaleTransformer1.ScaleX = scale;
			//ScaleTransformer1.ScaleY = scale;
            UpdateLayout();
		}
        public void LoadImage(Uri uri)
		{
			


			BitmapImage Bi = new BitmapImage(uri);
		    productIMG.Source = Bi;
		
//			productIMG.RenderTransformOrigin = new Point(0, 1);
			double scale;
			if (product.Lengte > 3000 || product.Breedte > 3000)
			{
				scale = 0.2;
				this.Visibility = Visibility.Collapsed;
			}
			else
			{
				int breedte = product.Breedte / 10;
				int lengte = product.Lengte / 10;
				
				int mfactor = 60;
				if (lengte >= 60 || breedte >= 60)
				{
					scale = 1;
				}
				else
				{
					if (lengte > breedte)
					{
						scale = ((double)(lengte * 2) + mfactor) / (120 + mfactor);
					}
					else
					{
						scale = ((double)(breedte * 2) + mfactor) / (120 + mfactor);
					}
				}
			}
			
			//ScaleTransform zoomtegel = new ScaleTransform();
			ScaleTransformer1.ScaleX = scale;
			ScaleTransformer1.ScaleY = scale;
			//productIMG.RenderTransform = zoomtegel;
			
			


			

			
			
			 
		}

        private void GetImageStart(string path)
        {
            if (Acumulator.Instance().ProductPlaatjes.ContainsKey(product.ID))
            {
                LoadImage(Acumulator.Instance().ProductPlaatjes[product.ID]);
            }
            else
            {
                if (path != null && path != "")
                {

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);

                    request.UseDefaultCredentials = true;
                    request.BeginGetResponse(new AsyncCallback(RequestReady), request);
                }
            }
            
        }
        private void RequestReady(IAsyncResult asyncResult)
        {
            var req = asyncResult.AsyncState as HttpWebRequest;
            try
            {
                using (WebResponse wrs = req.EndGetResponse(asyncResult))
                {
                    var foo = wrs.GetResponseStream();
                     var bar = foo.CloneToMemoryStream();
                    var foobar = bar.ToArray();
                    Acumulator.Instance().SetProductPlaatje(product.ID, foobar);
                     
                  UIThread.Invoke(()=> LoadImage(foobar));
                    
                }
            }
            catch (Exception e)
            {
                var err = e;
                var emsg = e.Message;
            }
            Acumulator.Instance().ImgMutex = false;
        }

        private void ProductCntrl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            janeeRdBtn.IsChecked = true;
        }
	}
	public class EnabledConvertor : IValueConverter
	{
		public object Convert(
			object value,
			Type targetType,
			object parameter,
			CultureInfo culture)
			{
				bool enabled = (bool)value;
				return enabled ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.LightGray);
			}

		public object ConvertBack(
			object value,
			Type targetType,
			object parameter,
			CultureInfo culture)
			{
				SolidColorBrush brush = (SolidColorBrush)value;
				return (brush.Color == Colors.Black);
			}
	}
}

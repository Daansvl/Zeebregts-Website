using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;

namespace eBrochure_zeebregts.KeuzeControls
{
	public partial class Receipt : UserControl
	{
		private double TotaalPrijs;
        private double kortingprijs;
		public Receipt()
		{
			InitializeComponent();
		}
		public void Clear()
		{
			TBoptie.Text = "";
			TBprijs.Text = "";
			TotaalPrijs = 0.00;
			CalculateTotal();
		}
		public void AddArticle(string name, double price, bool newline)
		{
           

			TBoptie.Text += name + Environment.NewLine;

			if (price != 0)
			{
				TBprijs.Text += "€"+String.Format("{0:0.00}",price) + Environment.NewLine;
			}
			else
			{
				TBprijs.Text +=  Environment.NewLine; 
			}
			TotaalPrijs += price;
			
			if(newline)
			{
				TBoptie.Text += Environment.NewLine;
				TBprijs.Text += Environment.NewLine;
			}
          
			CalculateTotal();
			
		}

        

        public double GetTotalD()
        {
            return TotaalPrijs;
        }
		public string GetTotal()
		{
			return "€" + String.Format("{0:0.00}",(TotaalPrijs));
		}
		private void CalculateTotal()
		{
            var korting = Acumulator.Instance().ProjFase.Korting;
            if (korting > 0)
            {
                TBsubtotaal.Text = "Subtotaal: ";
                TBsubtotaalprijs.Text = "€" + String.Format("{0:0.00}", TotaalPrijs) + Environment.NewLine;
                TBkorting.Text = "Korting " +100 * korting + "%" + Environment.NewLine;
                kortingprijs = (TotaalPrijs * korting);
                var foo = (kortingprijs / 0.05) + 0.5;
                var bar = Math.Round(foo, 0);
                var foobar = bar * 0.05;
                kortingprijs = Math.Round(foobar, 2);
                TBkortingprijs.Text = "- €" + String.Format("{0:0.00}", kortingprijs);
                TBtotalPrijs.Text = "€" + String.Format("{0:0.00}", TotaalPrijs - kortingprijs);
            }
            else
            {
                TBtotalPrijs.Text = "€" + String.Format("{0:0.00}", TotaalPrijs);
            }
			
		}
	}
}

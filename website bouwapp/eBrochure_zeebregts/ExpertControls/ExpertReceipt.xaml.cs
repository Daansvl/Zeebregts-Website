using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace eBrochure_zeebregts.ExpertControls
{
    public partial class ExpertReceipt : UserControl
    {
        public ExpertReceipt()
        {
            InitializeComponent();
        }
        private double TotaalPrijs;
        private double kortingprijs = 0.0;
        private int ArticleRowCounter = 0;

        public void Clear()
        {
            ArticleGrid.Children.Clear();
            ArticleGrid.RowDefinitions.Clear();
            

          
            TotaalPrijs = 0.00;
            CalculateTotal();
        }

        public void AddArticle(string text,double price,bool newline)
        {
            ArticleGrid.RowDefinitions.Add(new RowDefinition());
           
            
            var tbO = new TextBlock
            {
                FontFamily = new FontFamily("Courier New"),
                FontSize = 16,
                FontWeight = price < 0 ? FontWeights.Bold : FontWeights.Normal,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Margin = new Thickness(0, 5, 0,0),
                Text = text
            };
            tbO.SetValue(Grid.ColumnProperty, 0);
            tbO.SetValue(Grid.RowProperty, ArticleRowCounter);
            ArticleGrid.Children.Add(tbO);
            var tbP = new TextBlock
            {
                FontFamily = new FontFamily("Courier New"),
                FontSize = 16,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                Margin = new Thickness(20, 5, 0, 0),
                Text = price >= 0 ? "€" + String.Format("{0:0.00}", price) : ""
            };
            tbP.SetValue(Grid.ColumnProperty, 1);
            tbP.SetValue(Grid.RowProperty, ArticleRowCounter);
            ArticleGrid.Children.Add(tbP);

            ArticleRowCounter++;

            if(newline)
            {
                ArticleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(15) });
                ArticleRowCounter++;
            }

            
            TotaalPrijs += price >=0 ? price:0;
            CalculateTotal();
        }
        public double GetTotalD()
        {
            return TotaalPrijs;
        }
        public string GetTotal()
        {
            return "€" + String.Format("{0:0.00}", (TotaalPrijs - kortingprijs));
        }
        private void CalculateTotal()
        {
            var korting = Acumulator.Instance().ProjFase.Korting;
            if (korting > 0)
            {
                TBsubtotaal.Text = "Subtotaal: ";
                TBsubtotaalprijs.Text = "€" + String.Format("{0:0.00}", TotaalPrijs) + Environment.NewLine;
                TBkorting.Text = "Korting " + 100 * korting + "%" + Environment.NewLine;
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

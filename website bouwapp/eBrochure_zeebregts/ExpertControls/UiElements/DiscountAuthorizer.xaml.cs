using eBrochure_zeebregts.KeuzeControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace eBrochure_zeebregts.ExpertControls.UiElements
{
    public partial class DiscountAuthorizer : UserControl,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private bool authorized;
        public bool Authorized
        {
            get { return authorized; }
            set { authorized = value; OnPropertyChanged("Authorized"); }
        }

        private double oldprice;
        public double OldPrice
        {
            get { return oldprice; }
            set { oldprice = value; OnPropertyChanged("OldPrice"); }
        }

        private double newprice;
        public double NewPrice
        {
            get { return newprice; }
            set { newprice = value; OnPropertyChanged("NewPrice"); }
        }

        private double baseprice;
        public double BasePrice
        {
            get { return baseprice; }
            set { baseprice = value; OnPropertyChanged("BasePrice"); }
        }
        
        private double oldbaseprice;
        public double OldBasePrice
        {
            get { return oldbaseprice; }
            set { oldbaseprice = value; OnPropertyChanged("OldBasePrice"); }
        }

        private double kortingbedrag;
        public double KortingBedrag
        {
            get { return kortingbedrag; }
            set { kortingbedrag = value; OnPropertyChanged("KortingBedrag"); }
        }
        private double oldkortingbedrag;
        public double OldKortingBedrag
        {
            get { return oldkortingbedrag; }
            set { oldkortingbedrag = value; OnPropertyChanged("OldKortingBedrag"); }
        }

        private DetailsWindow Parent;

        public DiscountAuthorizer(double totalprice, double oldPrice,double oldkorting,DetailsWindow parent)
        {
            InitializeComponent();
            BasePrice = totalprice + oldkorting;
            OldKortingBedrag = oldkorting;
            OldPrice = oldPrice - OldKortingBedrag;
            
            OldBasePrice = OldPrice+ OldKortingBedrag;
            Parent = parent;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(PwdBox.Password == "1337")
            {
                Authorized = true;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            KortingBedrag = KortingInv.doubleValue;
            NewPrice = BasePrice - KortingBedrag;
            //double kort;
            //if(Double.TryParse(KortingInv.Text,out kort))
            //{
            //    KortingBedrag = kort;
            //    NewPrice = BasePrice - KortingBedrag;
            //    //KortingText.Text = "€ " + kort.ToString();
            //}
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Acumulator.Instance().OTracker.offerteRuimte_.Korting = new Classes.KortingNode
            {
                 AuthDate = DateTime.Now,
                 KortingBedrag = KortingBedrag,
                 Omschrijving = "Korting"
            };
            Parent.Close();   
        }
    }

    public class AuthVisConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value == true ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MoneyConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return  "€ " + value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



}

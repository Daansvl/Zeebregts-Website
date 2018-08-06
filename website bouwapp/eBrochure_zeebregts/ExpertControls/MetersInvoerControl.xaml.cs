using eBrochure_zeebregts.ExpertControls.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System;
using System.Net;
using System.IO;
using eBrochure_zeebregts.Classes;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Globalization;

namespace eBrochure_zeebregts.ExpertControls
{
    public partial class MetersInvoerControl : UserControl, INotifyPropertyChanged
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

        #region routed events
        public delegate void SubmitMetersEventHandler(object sender, SubmitMetersEventArgs e);
        public event SubmitMetersEventHandler SubmitMeters;
        public delegate void SubmitWijzigEventHandler(object sender, SubmitWijzigEventArgs e);
        public event SubmitWijzigEventHandler SubmitWijzig;
        public delegate void SubmitDeleteEventHandler(object sender, SubmitDeleteEventArgs e);
        public event SubmitDeleteEventHandler SubmitDelete;
        #endregion

        
        #region dependency props
        public StatusType Status
        {
            get { return (StatusType)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(StatusType), typeof(MetersInvoerControl), new PropertyMetadata(null));

        
        public double RemainingBasisMeters
        {
            get { return (double)GetValue(RemainingBasisMetersProperty); }
            set { SetValue(RemainingBasisMetersProperty, value); }
        }

        public static readonly DependencyProperty RemainingBasisMetersProperty =
            DependencyProperty.Register("RemainingBasisMeters", typeof(double), typeof(MetersInvoerControl), new PropertyMetadata(null));




        public bool ReadOnly
        {
            get { return (bool)GetValue(ReadOnlyProperty); }
            set { SetValue(ReadOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReadOnlyProperty =
            DependencyProperty.Register("ReadOnly", typeof(bool), typeof(MetersInvoerControl), new PropertyMetadata(null));


        
        #endregion
        
        public string TotaalPrijs
        {
            get
            {
                return ((AdvancedVervangRegel)DataContext).GekozenTegel != null ?
                    "Subtotaal prijs: €" + (((AdvancedVervangRegel)DataContext).GekozenTegel.PrijsPerMeter * ((AdvancedVervangRegel)DataContext).VervangInfo.GekozenMeters /*+ ((AdvancedVervangRegel)DataContext).GekozenTegel.InstapPrijs*/).ToString("F2") :
                    "";
            }
        }

        public string MeterPrijsText
        {
            get
            {
                return ((AdvancedVervangRegel)DataContext).GekozenTegel != null ? ((AdvancedVervangRegel)DataContext).VervangInfo.GekozenMeters.ToString() + " " +
                ((AdvancedVervangRegel)DataContext).Eenheid
                    +" voor €" +
                ((AdvancedVervangRegel)DataContext).GekozenTegel.PrijsPerMeter.ToString() +
                " per " + ((AdvancedVervangRegel)DataContext).Eenheid + ". " +
                TotaalPrijs:
                "";
            }
        }

        public string InstapPrijsTekst
        {
            get
            {
                return ((AdvancedVervangRegel)DataContext).GekozenTegel != null && ((AdvancedVervangRegel)DataContext).GekozenTegel.InstapPrijs > 0.0 ?
                "Instap prijs is €" + ((AdvancedVervangRegel)DataContext).GekozenTegel.InstapPrijs :
                "";
            }
        }
      
        public MetersInvoerControl()
        {
            InitializeComponent();
            SearchImg.Source = new BitmapImage(new Uri("http://mybouwapp.nl/Images/UIplaatjes/search-icon.jpg", UriKind.Absolute));
           
        }

        private void WijzigButton_Click(object sender, RoutedEventArgs e)
        {
            ((AdvancedVervangRegel)DataContext).VervangInfo.Wijzigstand = true;
            OnPropertyChanged("MeterPrijsText");
            OnPropertyChanged("InstapPrijsTekst");
            var wArgs = new SubmitWijzigEventArgs(((AdvancedVervangRegel)DataContext).VervangInfo.Wijzigstand);
            SubmitWijzig(this, wArgs);
            ((AdvancedVervangRegel)DataContext).SavedTegel = ((AdvancedVervangRegel)DataContext).GekozenTegel;
            ((AdvancedVervangRegel)DataContext).SavedOpmerking = ((AdvancedVervangRegel)DataContext).Opmerking;

            decimalInput.doubleValue = ((AdvancedVervangRegel)DataContext).VervangInfo.GekozenMeters;
            foreach (var tgl in TegelListBox.Items)
            {
                var ctgl = tgl as AdvancedTegelInfo;
                if (ctgl.IsGekozen == true)
                {
                    var indexOfSelect = TegelListBox.Items.IndexOf(tgl);
                    int scrollOffset = 0;
                    if (((TegelListBox.Items.Count - 1)-indexOfSelect) > 4)
                    {
                        if (indexOfSelect >= 4)
                        { scrollOffset = 3; }
                        else
                        {
                            scrollOffset = 3 - indexOfSelect;
                        }
                    }
                    var scrollval = indexOfSelect + scrollOffset - 1;
                    TegelListBox.ScrollIntoView(TegelListBox.Items[scrollval]);
                }
            }
            
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ClearSearch();
            var AvR = (AdvancedVervangRegel)DataContext;
            foreach (var tgl in TegelListBox.Items)
            {
                var tegel = tgl as AdvancedTegelInfo;
                if (tegel.IsGekozen)
                {
                    TegelListBox.SelectedItem = tgl;
                }
            }
           
            if (TegelListBox.SelectedItem != null)
            {
                var meters = AvR.VervangInfo.GekozenMeters;
                var eArgs = new SubmitMetersEventArgs(meters);
                SubmitMeters(this, eArgs);
                if (meters <= 0)
                {
                    MessageBox.Show("Te weinig meters opgegeven. Geef meer dan 0 meters op of verwijder deze regel");
                }
                else if (RemainingBasisMeters > meters || (RemainingBasisMeters == meters && AvR.Status == StatusType.Basis))
                {
                    AvR.VervangInfo.Wijzigstand = false;
                    OnPropertyChanged("TotaalPrijs");
                    OnPropertyChanged("MeterPrijsText");
                    OnPropertyChanged("InstapPrijsTekst");
                    SubmitMeters(this, eArgs);
                }
                else
                {
                    MessageBox.Show("Te veel meters opgegeven, maximaal beschikbaar is minder dan " + RemainingBasisMeters + " meter.\n Tip: U kunt de basistegel vervangen door die regel te wijzigen");
                }
            }
            else
            {
                MessageBox.Show("Geen accent tegel gekozen. \n\rDruk op het kruisje om accent te annuleren");
            }
            var wArgs = new SubmitWijzigEventArgs(((AdvancedVervangRegel)DataContext).VervangInfo.Wijzigstand);
            SubmitWijzig(this, wArgs);
        
        }

        private BackgroundWorker metersTimer;
        private void MetersTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (metersTimer == null)
            {
                metersTimer = new BackgroundWorker();
                metersTimer.DoWork += metersTimer_DoWork;
                metersTimer.RunWorkerCompleted += metersTimer_RunWorkerCompleted;
                metersTimer.WorkerSupportsCancellation = true;
            }

            if (metersTimer != null && metersTimer.IsBusy)
            {
                metersTimer.CancelAsync();
            }
            while (metersTimer.IsBusy)
            {
               
            }
            metersTimer.RunWorkerAsync(sender as TextBox);
 
           //var tb = sender as TextBox;
           
        }

        void metersTimer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                return;
            }

            var tb = e.Result as TextBox;
            if (tb != null)
            {
                var gekMtrs = ((AdvancedVervangRegel)DataContext).VervangInfo.GekozenMeters;
                tb.Text = tb.Text.Replace(",", ".");
                var splitDot = tb.Text.Split('.');
                var correctedMeters = 0.0;

                double.TryParse(tb.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out correctedMeters);

                
                if (correctedMeters > 0 /*double.TryParse(tb.Text, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out gekMtrs)*/)
                {
                    gekMtrs = Math.Round(correctedMeters, 2);
                    if (gekMtrs > 0)
                    {
                        ((AdvancedVervangRegel)DataContext).VervangInfo.GekozenMeters = gekMtrs;
                    }
                }
                
                OnPropertyChanged("TotaalPrijs");
                OnPropertyChanged("MeterPrijsText");
                OnPropertyChanged("InstapPrijsTekst");

                tb.Select(tb.Text.Length, 0);
            }

        }

        void metersTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            var iterations = 0;
            while (!metersTimer.CancellationPending && iterations < 30)
            {
                Thread.Sleep(100);
                iterations++;
            }
            if (metersTimer.CancellationPending)
            {
                e.Cancel = true;
            }
            e.Result = e.Argument;
        }

       

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach(var tegel in ((ListBox)sender).Items)
            {
                  if(((ListBox)sender).SelectedItem!= null && tegel == ((ListBox)sender).SelectedItem)
                  {
                      var luckyTegel = ((ListBox)sender).SelectedItem as AdvancedTegelInfo;
                      luckyTegel.IsGekozen = true;
                      ((AdvancedVervangRegel)DataContext).GekozenTegel = luckyTegel;
                  }
                  else
                  {
                      ((AdvancedTegelInfo)tegel).IsGekozen = false;
                  }
            }
            OnPropertyChanged("TotaalPrijs");
            OnPropertyChanged("MeterPrijsText");
            OnPropertyChanged("InstapPrijsTekst");
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = (sender as TextBox).Text;
            TegelListBox.ScrollIntoView(TegelListBox.Items[0]);
            foreach (var tegel in TegelListBox.Items)
            {
                var sTegel = tegel as AdvancedTegelInfo;
                if (sTegel.ProductId.Contains(searchText) ||
                    (sTegel.TegelDetails != null && sTegel.TegelDetails.Contains(searchText)) ||
                    (sTegel.TegelOmschrijving != null && sTegel.TegelOmschrijving.Contains(searchText)))
                {
                    sTegel.Filtered = false;
                }
                else
                {
                    sTegel.Filtered = true;
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

            var tegelinfo = ((RadioButton)sender).DataContext as AdvancedTegelInfo;
            

            foreach (var tegel in TegelListBox.Items)
            {
                var typedTegel = tegel as AdvancedTegelInfo;
                if (typedTegel != null)
                {
                    if (typedTegel == tegelinfo)
                    {
                        TegelListBox.SelectedItem = tegel;
                    }
                    else
                    {
                        typedTegel.IsGekozen = false;
                    }
                }

            }
            OnPropertyChanged("TotaalPrijs");
            OnPropertyChanged("MeterPrijsText");
            OnPropertyChanged("InstapPrijsTekst");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var avr = ((AdvancedVervangRegel)DataContext);
            if (avr.SavedTegel == null)
            {
                var dArgs = new SubmitDeleteEventArgs(true);
                SubmitDelete(this, dArgs);
            }
            else
            {
                avr.GekozenTegel = avr.SavedTegel;
                avr.VervangInfo.GekozenMeters = avr.VervangInfo.SavedMeters;
                avr.VervangInfo.Tegels = avr.VervangInfo.SavedTegels;
                avr.Opmerking = avr.SavedOpmerking;
                ((AdvancedVervangRegel)DataContext).VervangInfo.Wijzigstand = false;
                var wArgs = new SubmitWijzigEventArgs(((AdvancedVervangRegel)DataContext).VervangInfo.Wijzigstand);
                SubmitWijzig(this, wArgs);
            }
            
        }

        private void ClearSearch()
        {
            SearchTb.Text = "";
        }

        private void doubleInput_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (metersTimer == null)
            {
                metersTimer = new BackgroundWorker();
                metersTimer.DoWork += metersTimer_DoWork;
                metersTimer.RunWorkerCompleted += metersTimer_RunWorkerCompleted;
                metersTimer.WorkerSupportsCancellation = true;
            }

            if (metersTimer != null && metersTimer.IsBusy)
            {
                metersTimer.CancelAsync();
            }
            while (metersTimer.IsBusy)
            {

            }
            metersTimer.RunWorkerAsync(sender as TextBox);
 
        }
    
    }

    public class SubmitMetersEventArgs:EventArgs
    {
        private double meters;
        public double Meters
        {
            get { return meters; }
        }

        public SubmitMetersEventArgs(double meters)
        {
            this.meters = meters;
        }
    }
    public class SubmitWijzigEventArgs : EventArgs
    {
        private bool wijzig;
        public bool Wijzig
        {
            get { return wijzig; }
        }

        public SubmitWijzigEventArgs(bool wijzig)
        {
            this.wijzig = wijzig;
        }
    }
    public class SubmitDeleteEventArgs : EventArgs
    {
        private bool delete;
        public bool Delete
        {
            get { return delete; }
        }

        public SubmitDeleteEventArgs(bool delete)
        {
            this.delete = delete;
        }
    }

    public class MeterInputEnableConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var status = (StatusType)value;
            var result = true;
            if (value != null)
            {
                result = status != StatusType.Basis;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ReadOnlyVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value == (bool)parameter ? Visibility.Visible : Visibility.Collapsed; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class WijzigVisibilityConverter:IValueConverter
    {
        
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value == (bool)parameter ? Visibility.Visible : Visibility.Collapsed;            
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
    public class ImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var imgPath = (string)value;
            if (imgPath != null)
            {
                var path = "http://mybouwapp.nl/Images/ProductImgStorage/" + imgPath;
                try
                {
                    return new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
                }
                catch { }

            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class OpmerkingConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return String.IsNullOrEmpty(value as string) ? "" : "Opmerking: " + value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return String.IsNullOrEmpty((string)value) ? "" : value.ToString().Replace("Opmerking: ", "");
        }
    }
    public class FilterdVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BorderHideConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 0 : 4;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

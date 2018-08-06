using eBrochure_zeebregts.Classes;
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
    public partial class NabewerkingKeuzeControl : UserControl,INotifyPropertyChanged,IBaseControl
    {
        private bool wijzigstand;
        public bool Wijzigstand
        {
            get { return wijzigstand; }
            set { wijzigstand = value; OnPropertyChanged("Wijzigstand"); }
        }
        private NabewerkingUiRegel dataRegel;
        public NabewerkingUiRegel DataRegel
        { get { return dataRegel; } set { dataRegel = value; OnPropertyChanged("DataRegel"); } }

        private Ruimte ruimte;
        public NabewerkingKeuzeControl(NabewerkingUiRegel dr,Ruimte r)
        {
            DataRegel = dr;
            DataContext = DataRegel;
            ruimte = r;
            InitializeComponent();

            loadSave();
           
        }

        private void loadSave()
        {
            Wijzigstand = false;
            if (dataRegel.ProductId != null)
            {
                var bew = ruimte.GekozenBewerkingen.Where(x =>x.Kenmerk != null && x.Kenmerk.StartsWith(dataRegel.ProductId + dataRegel.SubCat)).ToList();
                foreach (var nb in bew)
                {
                    if (nb.BewerkingCat_NR == "NC1")
                    {
                        VoegCbb.SelectedValue = nb.Nabewerking_ID;
                    }
                    else
                    {
                        VerwerkingCbb.SelectedValue = nb.Nabewerking_ID;
                    }
                }

                if (bew == null || bew.Count == 0)
                {
                    Wijzigstand = true;

                }
                else
                {
                    var iscurcompl = Acumulator.Instance().BB.IsCurrentComplete();
                    wijzigstand = !iscurcompl;
                    Acumulator.Instance().BB.ShowWijzigBtn(iscurcompl);
                }
            }
            else
            {//tegelhoogte etc
                wijzigstand = true;
            }
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public bool SubmitPressed()
        {
            var result = true;

            var selectedNbVoeg = VoegCbb.SelectedItem as Nabewerking;
            if (selectedNbVoeg != null)
            {
                ruimte.GekozenBewerkingen.Add(selectedNbVoeg);
            }
            else
            {
                result = false;
            }
            
            var selectedNbVerwerk = VerwerkingCbb.SelectedItem as Nabewerking;
            if (selectedNbVerwerk != null)
            {
                ruimte.GekozenBewerkingen.Add(selectedNbVerwerk);
            }
            else
            {
                result = false;
            }
            if (result)
            {
                Wijzigstand = false;
            }
            return result;
        }

        public void Clear4Submit()
        {
            ruimte.GekozenBewerkingen.Clear();
        }

        public void WijzigPressed()
        {
            Wijzigstand = true;
        }
    }

    public class VerwerkVisConv : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class WijzigstanOffVisConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value == true ? new GridLength(0) : new GridLength(30);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class WijzigstandOnVisConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value == true ? new GridLength(30) : new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

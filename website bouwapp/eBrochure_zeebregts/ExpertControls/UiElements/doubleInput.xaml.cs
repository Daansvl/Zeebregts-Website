using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace eBrochure_zeebregts.ExpertControls.UiElements
{
    public partial class doubleInput : UserControl,INotifyPropertyChanged
    {


        
        private string intval;
        public string intVal
        {
            get { return intval; }
            set
            {
                intval = value;
                OnPropertyChanged("intVal");
                updateValue();
            }
        }

        private string decival;
        public string deciVal
        {
            get { return decival; }
            set
            { 
                decival = value;
                OnPropertyChanged("deciVal");
                updateValue();
            }
        }

        public double doubleValue
        {
            get { return (double)GetValue(doubleValueProperty); }
            set
            {
                SetValue(doubleValueProperty, value);
                OnPropertyChanged("doubleValue");
                OndoubleValueChanged();
            }
        }

        // Using a DependencyProperty as the backing store for doubleValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty doubleValueProperty =
            DependencyProperty.Register("doubleValue", typeof(double), typeof(doubleInput), new PropertyMetadata(0.0));

        private void OndoubleValueChanged()
        {
            var delim = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var delimitersplit = (doubleValue.ToString("0.00")).Split(delim.ToCharArray());
            if (delimitersplit.Count() > 1)
            {
                intVal = delimitersplit[0];
                deciVal = delimitersplit[1];
            }
            //var commasplit = (doubleValue.ToString("0.00")).Split(',');
            //if (commasplit.Count() > 1)
            //{
            //    intVal = commasplit[0];
            //    deciVal = commasplit[1];
            //}
            //else
            //{
            //    var pointsplit = (doubleValue.ToString("0.00")).Split('.');
            //    if (pointsplit.Count() > 1)
            //    {
            //        intVal = pointsplit[0];
            //        deciVal = pointsplit[1];
            //    }
            //}

        }

       
       

        public doubleInput()
        {
            InitializeComponent();
        }
        
        private void updateValue()
        {
            string number = intVal + System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator + deciVal;
            var doubleNumber = 0.0;
            if(Double.TryParse(number,out doubleNumber))
            {
                SetValue(doubleValueProperty,doubleNumber);
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

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.NumPad0:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                case Key.Tab:
                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }
    }
}

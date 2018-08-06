using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MandagenRegistratie.controls
{
    /// <summary>
    /// Interaction logic for RangeSlider.xaml
    /// </summary>
    public partial class RangeSlider : UserControl
    {

        public RangeSlider()
        {
            InitializeComponent();

            this.Loaded += Slider_Loaded;
        }

        void Slider_Loaded(object sender, RoutedEventArgs e)
        {
            LowerSlider.ValueChanged += LowerSlider_ValueChanged;
            UpperSlider.ValueChanged += UpperSlider_ValueChanged;
        }

        private void LowerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpperSlider.Value = Math.Max(UpperSlider.Value, LowerSlider.Value);

            SetSelectedArea();
        }

        private void SetSelectedArea()
        {
            LowerSlider.SelectionStart = LowerSlider.Value;
            LowerSlider.SelectionEnd = UpperSlider.Value;
        }

        private void UpperSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LowerSlider.Value = Math.Min(UpperSlider.Value, LowerSlider.Value);

            SetSelectedArea();
        }

        public double mMinimum
        {
            get { return (double)GetValue(mMinimumProperty); }
            set { SetValue(mMinimumProperty, value); }
        }

        public static readonly DependencyProperty mMinimumProperty =
            DependencyProperty.Register("mMinimum", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d));

        public double mLowerValue
        {
            get { return (double)GetValue(mLowerValueProperty); }
            set { SetValue(mLowerValueProperty, value); }
        }

        public static readonly DependencyProperty mLowerValueProperty =
            DependencyProperty.Register("mLowerValue", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d));

        public double mUpperValue
        {
            get { return (double)GetValue(mUpperValueProperty); }
            set { SetValue(mUpperValueProperty, value); }
        }

        public static readonly DependencyProperty mUpperValueProperty =
            DependencyProperty.Register("mUpperValue", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d));

        public double mMaximum
        {
            get { return (double)GetValue(mMaximumProperty); }
            set { SetValue(mMaximumProperty, value); }
        }

        public static readonly DependencyProperty mMaximumProperty =
            DependencyProperty.Register("mMaximum", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(1d));



    }
}

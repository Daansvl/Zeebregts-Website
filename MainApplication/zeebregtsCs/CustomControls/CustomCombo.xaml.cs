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

namespace zeebregtsCs.CustomControls
{
    /// <summary>
    /// Interaction logic for CustomCombo.xaml
    /// </summary>
    public partial class CustomCombo 
    {
        public CustomCombo()
        {
            InitializeComponent();
            Loaded += CustomCombo_Loaded;
        }

        void CustomCombo_Loaded(object sender, RoutedEventArgs e)
        {
            var ct = Template;
            var border = ct.FindName("Border", this) as Border;

            if (border != null)
            {
                border.Background = Background;

                // In the case of bound property
                BindingExpression be = GetBindingExpression(BackgroundProperty);
                if (be != null)
                {
                    border.SetBinding(Border.BackgroundProperty, be.ParentBindingBase);
                }
            }
        }
        public void SetBorder(bool valid)
        {
            if (valid)
            {
                SetValue(Border.BackgroundProperty, Brushes.White);
            }
            else
            {
                SetValue(Border.BackgroundProperty, Brushes.Crimson);
            }
        }
    }
}

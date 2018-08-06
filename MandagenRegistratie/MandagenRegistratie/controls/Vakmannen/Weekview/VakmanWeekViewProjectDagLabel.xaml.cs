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
using MandagenRegistratieDomain;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace MandagenRegistratie.controls.Vakmannen.Weekview
{
    /// <summary>
    /// Interaction logic for VakmanDagViewProjectUurLabel.xaml
    /// </summary>
    public partial class VakmanWeekViewProjectDagLabel : UserControl
    {
        public string Dag;
        public bool IsSelected;

        public VakmanWeekViewProjectDagLabel()
        {
            InitializeComponent();
        }

        public void Load()
        {

            lblDag.Content = Dag;

            if (IsSelected)
            {

                spDagLabel.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));

            }
            else
            {
                spDagLabel.Background = new SolidColorBrush(Colors.White);
            }


        }

    }
}


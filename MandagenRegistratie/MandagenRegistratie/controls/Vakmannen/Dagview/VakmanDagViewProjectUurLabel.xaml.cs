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

namespace MandagenRegistratie.controls.Vakmannen.Dagview
{
    /// <summary>
    /// Interaction logic for VakmanDagViewProjectUurLabel.xaml
    /// </summary>
    public partial class VakmanDagViewProjectUurLabel : UserControl
    {
        public int Uur;

        public VakmanDagViewProjectUurLabel()
        {
            InitializeComponent();
        }

    }
}


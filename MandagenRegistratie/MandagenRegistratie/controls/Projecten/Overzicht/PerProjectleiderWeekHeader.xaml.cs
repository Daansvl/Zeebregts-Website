using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MandagenRegistratie.controls.Projecten.Overzicht
{
    /// <summary>
    /// Interaction logic for PerProjectleiderWeekHeader.xaml
    /// </summary>
    public partial class PerProjectleiderWeekHeader : UserControl
    {
        public bool IsEven { get; set; }
        public DateTime dtDag { get; set; }

        public PerProjectleiderWeekHeader()
        {
            InitializeComponent();
        }

        public string GetDagTooltip(DateTime dag)
        {
            return dag.ToString("dd-MM-yyyy");
        }

        public void Load()
        {

            // datums onder tooltip
            lblMa.ToolTip = GetDagTooltip(dtDag);
            lblDi.ToolTip = GetDagTooltip(dtDag.AddDays(1));
            lblWo.ToolTip = GetDagTooltip(dtDag.AddDays(2));
            lblDo.ToolTip = GetDagTooltip(dtDag.AddDays(3));
            lblVr.ToolTip = GetDagTooltip(dtDag.AddDays(4));
            lblZa.ToolTip = GetDagTooltip(dtDag.AddDays(5));
            lblZo.ToolTip = GetDagTooltip(dtDag.AddDays(6));

            if (IsEven)
            {
                bbDivider.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
            }
            else
            {
                bbDivider.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5D9F1"));
            }
        }
    }
}

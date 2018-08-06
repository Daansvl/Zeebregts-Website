using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDR2PDF
{
    public partial class MszTools : UserControl
    {
        public MszTools()
        {
            InitializeComponent();
        }
        // http://www.codeproject.com/Articles/5801/Adjust-combo-box-drop-down-list-width-to-longest-s

        internal static void AdjustWidthComboBox_DropDown(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            int width = senderComboBox.DropDownWidth;
            Graphics g = senderComboBox.CreateGraphics();
            Font font = senderComboBox.Font;
            int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                ? SystemInformation.VerticalScrollBarWidth : 0;

            int newWidth;
            foreach (var s in ((ComboBox)sender).Items)
            {
                string ss = s.ToString();
                newWidth = (int)g.MeasureString(ss, font).Width
                    + vertScrollBarWidth;
                if (width < newWidth)
                {
                    width = newWidth;
                }
            }
            senderComboBox.DropDownWidth = width;
        }
    }
}

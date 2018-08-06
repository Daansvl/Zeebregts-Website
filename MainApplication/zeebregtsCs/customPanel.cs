using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace zeebregtsCs
{
    public partial class customPanel : Panel
    {
        public customPanel()
        {
            InitializeComponent();
        }
         
        protected override System.Drawing.Point ScrollToControl(System.Windows.Forms.Control activeControl)
        {   
            // Returning the current location prevents the panel from
            // scrolling to the active control when the panel loses and regains focus
            return this.DisplayRectangle.Location;
        }

    }
}

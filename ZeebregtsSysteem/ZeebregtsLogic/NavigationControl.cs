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
using System.ComponentModel;

namespace ZeebregtsLogic
{
    public class NavigationControl : UserControl
    {
        public delegate void FunctieOK();
        public event FunctieOK OkClick; // de gereed functie

        public delegate void FunctieLoad();
        public event FunctieLoad Reloaded;



        public NavigationControl()
        {
        }

        /// <summary>
        /// Kan aangeroepen worden om de OK-functie van de control aan te roepen
        /// triggert het 
        /// </summary>
        public void PageOK()
        {
            OkClick();
        }


        /// <summary>
        /// Kan aangeropen worden om de Load-functie van de control aan te roepen
        /// triggert het 
        /// </summary>
        public void PageReloaded()
        {
            Reloaded();
        }


        /// <summary>
        /// Ga naar deze pagina
        /// </summary>
        /// <param name="control"></param>
        public void PageGoToPage(Control control)
        {
            // load the page into the contentcontrol
            Tools.FindVisualParent<PageContainer>(this).LoadControl(control, false);
        }

        /// <summary>
        /// Ga naar deze pagina
        /// </summary>
        /// <param name="control"></param>
        public void PageGoToPage(System.Windows.Forms.Control control)
        {
            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).LoadControl(control);
            Tools.FindVisualParent<PageContainer>(this).LoadControl(control);
        }



    }
}

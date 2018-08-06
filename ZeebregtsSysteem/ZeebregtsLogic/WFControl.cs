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
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace ZeebregtsLogic
{

    public class WFControl : System.Windows.Forms.UserControl
    {
        public delegate void FunctieOK();
        public event FunctieOK OkClick; // de gereed functie

        // DEFAULT VALUES
        public Visibility PageGereedButtonVisibility = Visibility.Visible;
        public string PageTitle = "Titel van de pagina";
        public string PageSubtitle = "Subtitel van de pagina";
        public string PageGereedButtonText = "OK";
        public string PageTerugButtonText = "TERUG";

        public WFControl()
        {

        }

        /// <summary>
        /// Kan aangeropen worden om de OK-functie van de control aan te roepen
        /// triggert het 
        /// </summary>
        public void PageOK()
        {
            OkClick();
        }

        /// <summary>
        /// HACK, I have to save the host to be able to return to its parent later
        /// </summary>
        public System.Windows.Forms.Integration.WindowsFormsHost HHH;

        ///// <summary>
        ///// Ga naar deze pagina
        ///// </summary>
        ///// <param name="control"></param>
        //public void PageGoToPage(System.Windows.Controls.Control control)
        //{
        //    // load the page into the contentcontrol

        //    ((PageContainer)((StackPanel)HHH.Parent).Parent).LoadControl(control, false);
        //}

        ///// <summary>
        ///// Ga naar deze pagina
        ///// </summary>
        ///// <param name="control"></param>
        //public void PageGoToPage(System.Windows.Forms.Control control)
        //{
        //    // load the page into the contentcontrol

        //    ((PageContainer)((StackPanel)HHH.Parent).Parent).LoadControl(control);
        //}


        /// <summary>
        /// Ga naar deze pagina
        /// </summary>
        /// <param name="control"></param>
        public void PageGoToPage(System.Windows.Controls.Control control)
        {
            // load the page into the contentcontrol
            Tools.FindVisualParent<PageContainer>(HHH).LoadControl(control, false);
        }

        /// <summary>
        /// Ga naar deze pagina
        /// </summary>
        /// <param name="control"></param>
        public void PageGoToPage(System.Windows.Forms.Control control)
        {
            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).LoadControl(control);
            Tools.FindVisualParent<PageContainer>(HHH).LoadControl(control);
        }

        public void PageGoBack()
        {
            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).GoBack();
            Tools.FindVisualParent<PageContainer>(HHH).GoBack();

        }

        public void PageGoBack(MenuControl otherPage)
        {
            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).GoBack();
            //Tools.FindVisualParent<PageContainer>(this).GoBack();

            Tools.FindVisualParent<PageContainer>(HHH).LoadControl(otherPage, true);

        }

        //public void PageGoBack()
        //{
        //    // load the page into the contentcontrol
        //    ((PageContainer)((StackPanel)HHH.Parent).Parent).GoBack();

        //}

        //public void PageGoBack(MenuControl otherPage)
        //{
        //    // load the page into the contentcontrol
        //    //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).GoBack();
        //    //Tools.FindVisualParent<PageContainer>(this).GoBack();

        //    ((PageContainer)((StackPanel)HHH.Parent).Parent).LoadControl(otherPage, true);

        //}


        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WFControl
            // 
            this.Name = "WFControl";
            //this.Size = new System.Drawing.Size(1050, 690);
            this.AutoSize = false;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.ResumeLayout(false);

        }

    }
}

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
using MandagenRegistratieLogic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace MandagenRegistratieLogic
{

    public class WFControl : System.Windows.Forms.UserControl
    {
        public delegate void FunctieGereed();
        public event FunctieGereed Gereed; // de gereed functie

        // DEFAULT VALUES
        public Visibility GereedButtonVisibility = Visibility.Visible;
        public string PageTitle = "Bel de programmeur om ook deze pagina van een leuke titel te voorzien :-) 0646408407";
        public string GereedButtonText = "OK";
        public string TerugButtonText = "TERUG";

        public enum TerugNiveau
        {
            EentjeTerug = 0,
            TweeTerug = 1,
            DrieTerug = 2
        }

        public TerugNiveau TerugNaar = TerugNiveau.EentjeTerug;


        public WFControl()
        {

        }

        /// <summary>
        /// Kan aangeropen worden om de OK-functie van de control aan te roepen
        /// triggert het 
        /// </summary>
        public void OK()
        {
            Gereed();
        }

        /// <summary>
        /// HACK, I have to save the host to be able to return to its parent later
        /// </summary>
        public System.Windows.Forms.Integration.WindowsFormsHost HHH;

        /// <summary>
        /// Ga naar deze pagina
        /// </summary>
        /// <param name="control"></param>
        public void GoToPage(System.Windows.Controls.Control control)
        {
            // load the page into the contentcontrol

            ((PageContainer)((StackPanel)HHH.Parent).Parent).LoadControl(control);
        }

        /// <summary>
        /// Ga naar deze pagina
        /// </summary>
        /// <param name="control"></param>
        public void GoToPage(System.Windows.Forms.Control control)
        {
            // load the page into the contentcontrol

            ((PageContainer)((StackPanel)HHH.Parent).Parent).LoadControl(control);
        }


        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WFControl
            // 
            this.Name = "WFControl";
            this.Size = new System.Drawing.Size(943, 478);
            this.ResumeLayout(false);

        }

    }
}

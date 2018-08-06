using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Projecten.Overzicht
{
    public partial class TEST : WFControl
    {
        public TEST()
        {
            InitializeComponent();

            Gereed += KlaarMetDePagina;
            TerugButtonText = "TERUG TEST";
            GereedButtonText = "KLAAR MET DE PAGINA";

            TerugNaar = TerugNiveau.EentjeTerug;
            
        }

        private void KlaarMetDePagina()
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {
            // load the page into the contentcontrol

            TEST test = new TEST();
            GoToPage(test);


        }


    }
}

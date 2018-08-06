using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeebregtsLogic;

namespace MDR2PDF
{
    public partial class StartScherm : Form
    {
        //public static string xWeekLijst = "";//@ConfigurationManager.AppSettings["Weeklijst"];

        public static string xWeeklijst = "";//@ConfigurationManager.AppSettings["Weeklijst"];
        public USMarioStart Pagina1 = new USMarioStart();
        public USMario Pagina2 = new USMario();

        public string _pubOKButton;
        public string pubOKButton 
        {   get { return btVerder.Text;}
            set { btVerder.Text = pubOKButton; }
        }

        public StartScherm()
        {
            xWeeklijst = ConfigurationManager.AppSettings["Weeklijst"];
            InitializeComponent();

            // Open de eerste Pagina
            OpenPagina(1);

        }

        private void OpenPagina(int p)
        {
            switch (p)
            {
                case 1: // Initialize
                    panel1.Controls.Clear();
                    panel1.Controls.Add(Pagina1);
                    // Set scrollbar minsize zodat hele tabel te zien is
                    this.AutoScrollMinSize = new Size(Pagina1.Width - 20, Pagina1.Height + 200);

                    Point L = new Point();
                    L.X = panel1.Width /2 - Pagina1.Width/2;
                    Pagina1.Location = L;
                    Pagina1.BouwLijst();
                    // Reset gekozen lijst
                    Pagina1.PDFSETTTINGS.Lijstgegevens.Code = "";
                    ApplicationState.SetValue(GlobaleVars.strApplicationState, Pagina1.PDFSETTTINGS);
                    break;
                case 2: // Bevestig
                    if (panel1.Controls[0].Name.ToUpper() == "USMARIO")
                    { // We zitten al op Pagina 2, dus deze knop betekent nu 'PDF afdrukken'
                        Pagina2.OKButton();
                    }
                    else
                    { // Ga naar Pagina2

                        if (SublijstNogNietBekend())
                            return;
                        
                        panel1.Controls.Clear();
                        panel1.Controls.Add(Pagina2);
                        Pagina2.PageTitle = Pagina1.PDFSETTTINGS.Lijstgegevens.Titel;//.LijstTitel;
                        if (Pagina1.PDFSETTTINGS.Lijstgegevens.SubLijst.Count == 1)
                            Pagina2.PageSubtitle = Pagina1.PDFSETTTINGS.Lijstgegevens.SubLijst[0].SubTitel;//.LijstSubtitel + "TESTTEST";
                        else
                            //Pagina2.PageSubtitle = "";
                            //Pagina2.PageSubtitle = Pagina1.PDFSETTTINGS.Lijstgegevens.SubLijst[Pagina1.PDFSETTTINGS.Lijstgegevens.SublijstKeuze].SubTitel;
                            Pagina2.PageSubtitle = "Dit gebruiken we volgens mij helemaal niet meer!";
                        Pagina2.Bevestig(Pagina1.PDFSETTTINGS);
                    }
                    Pagina2.PageSubtitle = Pagina1.PDFSETTTINGS.Lijstgegevens.SubLijst.Where(x => x.SubCode == Pagina1.PDFSETTTINGS.Lijstgegevens.Code).Select(z => z.SubTitel).FirstOrDefault(); // [Pagina1.PDFSETTTINGS.Lijstgegevens.SublijstKeuze].SubTitel;
                    // Set scrollbar minsize zodat hele tabel te zien is
                    this.AutoScrollMinSize = new Size(Pagina2.Width - 20, Pagina2.Height - 20);

                    //Pagina2.scrollbarsnew();
                    break;
                default:
                    break;
            }
        }

        private void btnTerug_Click(object sender, EventArgs e)
        {
            OpenPagina(1);
            //string test = Pagina2.pubX;
        }

        public void btVerder_Click(object sender, EventArgs e)
        {
            if (this.Cursor == Cursors.WaitCursor)
                return;

            if (Pagina1.PDFSETTTINGS.Gebruiker.CanPrint)
            {
                //this.Cursor = Cursors.WaitCursor;
                OpenPagina(2);
                //this.Cursor = Cursors.Default;
            }
            else
                MessageBox.Show("U heeft niet voldoende rechten om verder te gaan");
            
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var subtitels = Pagina1.PDFSETTTINGS.Lijstgegevens.SubLijst.Select(x => x.SubTitel).ToList();
            if (subtitels.Contains(e.ClickedItem.ToString()))
            {
                for (int i = 0; i < subtitels.Count; i++)
                {
                    if (subtitels[i] == e.ClickedItem.ToString())
                    {
                        Pagina1.PDFSETTTINGS.Lijstgegevens.Code = Pagina1.PDFSETTTINGS.Lijstgegevens.SubLijst[i].SubCode;
                        break;
                    }
                }
                if (Pagina1.PDFSETTTINGS.Lijstgegevens.Code != "")
                    OpenPagina(2);
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        public bool SublijstNogNietBekend ()
        { 
            // Laat de gebruiker eerst een sublijst uitzoeken! (indien nodig)
            if (Pagina1.PDFSETTTINGS.Lijstgegevens.SubLijst.Count == 1)
            {
                Pagina1.PDFSETTTINGS.Lijstgegevens.Code = Pagina1.PDFSETTTINGS.Lijstgegevens.SubLijst[0].SubCode;
            }
            else
            {
                if (Pagina1.PDFSETTTINGS.Lijstgegevens.Code == "")
                {
                    contextMenuStrip1.Items.Clear();
                    contextMenuStrip1.Items.Add("kies één van onderstaande sublijsten");
                    contextMenuStrip1.Items.Add("");
                    foreach (var sl in Pagina1.PDFSETTTINGS.Lijstgegevens.SubLijst)
                    {
                        contextMenuStrip1.Items.Add(sl.SubTitel);
                    }

                    contextMenuStrip1.Show(Control.MousePosition.X - contextMenuStrip1.Width+20, Control.MousePosition.Y-10);
                    return true;
                }
            }
            return false;
        }

        private void StartScherm_ResizeEnd(object sender, EventArgs e)
        {

        }
    }//
}//

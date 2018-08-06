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
using MandagenRegistratie.controls;
using MandagenRegistratie.controls.Vakmannen.Dagview;
using ZeebregtsLogic;
using System.Reflection;
using MandagenRegistratieDomain;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web;

namespace MandagenRegistratie.controls.Vakmannen.Detail
{
    /// <summary>
    /// Interaction logic for VakmanDetailEdit.xaml
    /// </summary>
    public partial class VakmanDetailEdit : MenuControl
    {
        bool ChangeAdres = true;
        string AdresLookup = "";

        public VakmanDetailEdit(string pageTitle)
        {
            InitializeComponent();

            #region Pagina specifieke informatie
            PageTitle = pageTitle + " - edit";
            PageSubtitle = "Detail info van de vakman wijzigen";
            PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            PageOKButtonText = "OPSLAAN";
            PageBackButtonText = "ANNULEER";
            #endregion

            // load info op pagina
            Load();
            Init();

            this.OkClick += Save;
            this.Reloaded += Load;
        }

        public void Save()
        {
            vwVakman vm = ApplicationState.GetValue<vwVakman>(ApplicationVariables.objVakman);
            dbRepository dbrep = new dbRepository();
                
            Vakman vakman = dbrep.GetVakman(vm.VakmanId);

            vakman.Bsn = txtBsn.Text;
            vakman.Postcode = txtPostcode.Text;
            vakman.Huisnummer = txtHuisnummer.Text;
            vakman.Adres = txtAdres.Text;
            //vakman.Ophaalpostcode = listTextboxes[3].Text;
            //vakman.Ophaalhuisnummer = listTextboxes[4].Text;
            //vakman.Ophaaladres = listTextboxes[5].Text;
            vakman.Ma = Convert.ToInt32(txtMa.Text);
            vakman.Di = Convert.ToInt32(txtDi.Text);
            vakman.Wo = Convert.ToInt32(txtWo.Text);
            vakman.Do = Convert.ToInt32(txtDo.Text);
            vakman.Vr = Convert.ToInt32(txtVr.Text);
            vakman.Za = Convert.ToInt32(txtZa.Text);
            vakman.Zo = Convert.ToInt32(txtZo.Text);
            vakman.Werkweek = Convert.ToInt32(lblVakmanWerkweek.Content);

            string strUur = ddlDefaultBeginuur.Text;
            vakman.DefaultBeginuur = strUur.ToInt();

            string strMinuut = ddlDefaultBeginminuut.Text;
            vakman.DefaultBeginminuut = strMinuut.ToInt();

            dbrep.datacontext.SubmitChanges();

            //ApplicationState.SetValue(ApplicationVariables.objVakman, vakman);
            ApplicationState.SetValue(ApplicationVariables.intVakmanId, vakman.VakmanId);

            // na het saven terug in history
            PageGoBack();

        }

        public void Init()
        {
            // event handlers toewijzen
            txtPostcode.KeyUp += new KeyEventHandler(AutoVulAdres);
            txtHuisnummer.KeyUp += new KeyEventHandler(AdresAanvullen);
            txtAdres.KeyUp += new KeyEventHandler(AdresWijzigen);

            txtMa.KeyUp += new KeyEventHandler(WerkweekUrenBerekenen);
            txtDi.KeyUp += new KeyEventHandler(WerkweekUrenBerekenen);
            txtWo.KeyUp += new KeyEventHandler(WerkweekUrenBerekenen);
            txtDo.KeyUp += new KeyEventHandler(WerkweekUrenBerekenen);
            txtVr.KeyUp += new KeyEventHandler(WerkweekUrenBerekenen);
            txtZa.KeyUp += new KeyEventHandler(WerkweekUrenBerekenen);
            txtZo.KeyUp += new KeyEventHandler(WerkweekUrenBerekenen);



        }


        /// <summary>
        /// 
        /// </summary>
        public void Load()
        {

            Vakman vm = ApplicationState.GetValue<Vakman>(ApplicationVariables.objVakman);

            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
            persoon persoon = dbrepOriginal.GetContact(vm.ContactIdOrigineel);

            lblVakmanId.Content = vm.ContactIdOrigineel.ToString();
            lblVakmanNaam.Content = persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam;
            txtBsn.Text = vm.Bsn;
            txtPostcode.Text =  vm.Postcode;
            txtHuisnummer.Text = vm.Huisnummer;
            txtAdres.Text = vm.Adres;

            AdresLookup = vm.Adres.Substring(0,vm.Adres.Length - vm.Huisnummer.Length).Trim();

            txtMa.Text = vm.Ma.ToString();
            txtDi.Text = vm.Di.ToString();
            txtWo.Text = vm.Wo.ToString();
            txtDo.Text = vm.Do.ToString();
            txtVr.Text = vm.Vr.ToString();
            txtZa.Text = vm.Za.ToString();
            txtZo.Text = vm.Zo.ToString();

            ddlDefaultBeginuur.Text = ((int)vm.DefaultBeginuur).ToString("D2");
            ddlDefaultBeginminuut.Text = ((int)vm.DefaultBeginminuut).ToString("D2");

            lblVakmanWerkweek.Content = vm.Werkweek.ToString();              
            


        }




        /// <summary>
        /// Voeg dit item toe aan het panel
        /// </summary>
        /// <param name="label"></param>
        /// <param name="waarde"></param>
        public void AddLabelToPanel(string label, string waarde)
        {

            Label lbl = new Label();
            lbl.Content = label;
            lbl.Width = 200;
            lbl.Background = Brushes.Bisque;

            Label lbl2 = new Label();
            lbl2.Content = waarde;
            lbl2.Width = 400;


            wpDetailsEdit.Children.Add(lbl);
            wpDetailsEdit.Children.Add(lbl2);

        }

        /// <summary>
        /// Voeg dit item toe aan het panel
        /// </summary>
        /// <param name="label"></param>
        /// <param name="waarde"></param>
        public void AddTextboxToPanel(string id, string label, string waarde)
        {

            Label lbl = new Label();
            lbl.Content = label;
            lbl.Width = 200;
            lbl.Background = Brushes.Bisque;

            TextBox txtbox = new TextBox();
            txtbox.Text = waarde;
            txtbox.Width = 400;
            txtbox.Name = id;

            if (id == "txtPostcode")
            {
                txtbox.KeyUp += new KeyEventHandler(AutoVulAdres);
            }

            if (id == "txtHuisnummer")
            {
                txtbox.KeyUp += new KeyEventHandler(AdresAanvullen);
            }

            if (id == "txtAdres")
            {
                txtbox.KeyUp += new KeyEventHandler(AdresWijzigen);
            }

            wpDetailsEdit.Children.Add(lbl);
            wpDetailsEdit.Children.Add(txtbox);

        }

        public void AutoVulAdres(object sender, KeyEventArgs e)
        {
            string _postcode = ((TextBox)sender).Text;
            Tools tool = new Tools();

            var regx = new Regex(@"(\d{4}\s*)(\D{2})");
            var match = regx.Match(_postcode);

            if (match.Success)
            {
                string straat = tool.ZoekOpPostcode(_postcode);

                foreach(Control control in wpDetailsEdit.Children)
                {
                    if(control.Name == "txtAdres")
                    {
                        ((TextBox)control).Text = straat;
                        AdresLookup = straat;
                        ChangeAdres = true;
                    }
                }
            }
        }

        public void AdresAanvullen(object sender, KeyEventArgs e)
        {
            string huisnummer = ((TextBox)sender).Text;

            foreach (Control control in wpDetailsEdit.Children)
            {
                if (control.Name == "txtAdres")
                {
                    if (ChangeAdres)
                    {
                        ((TextBox)control).Text = AdresLookup + " " + huisnummer;
                    }
                }
            }
        }

        public void AdresWijzigen(object sender, KeyEventArgs e)
        {
            // stops automatic changing of the adres
            if (e.Key != Key.Tab)
            {
                ChangeAdres = false;
            }
        }

        /// <summary>
        /// Voeg dit item toe aan het panel
        /// </summary>
        /// <param name="label"></param>
        /// <param name="waarde"></param>
        public void AddLabelToPanel(string label)
        {

            Label lbl = new Label();
            lbl.Content = label;
            lbl.Width = 600;
            lbl.FontWeight = FontWeights.Bold;

            wpDetailsEdit.Children.Add(lbl);

        }






        public void WerkweekUrenBerekenen(object sender, KeyEventArgs e)
        {
            int totaal = txtMa.Text.ToInt() + txtDi.Text.ToInt() + txtWo.Text.ToInt() + txtDo.Text.ToInt() + txtVr.Text.ToInt() + txtZa.Text.ToInt() + txtZo.Text.ToInt();

            lblVakmanWerkweek.Content = totaal.ToString();
        }
    }
}

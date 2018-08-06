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
using ZeebregtsLogic;
using MandagenRegistratieDomain;
using System.Text.RegularExpressions;

namespace MandagenRegistratie.controls.Projecten.Overzicht
{
    /// <summary>
    /// Interaction logic for ProjectEdit.xaml
    /// </summary>
    public partial class ProjectEdit : MenuControl
    {
        bool ChangeAdres = true;
        string AdresLookup = "";

        public ProjectEdit(string pageTitle)
        {
            InitializeComponent();
            #region Pagina specifieke informatie
            PageTitle = pageTitle + " - edit";
            PageSubtitle = "Detail info van het project wijzigen";
            PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            PageOKButtonText = "OPSLAAN";
            PageBackButtonText = "ANNULEER";
            #endregion

            // load info op pagina
            Load();

            this.OkClick += Save;
            this.Reloaded += Load;
        }

        public void Save()
        {
            int projectId = ApplicationState.GetValue<int>(ApplicationVariables.intProjectId);

            dbRepository dbrep = new dbRepository();
            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();

            MandagenRegistratieDomain.Project project = dbrep.GetProject(projectId);
            MandagenRegistratieDomain.project dboproject = dbrepOriginal.datacontext.projects.Where(p => p.project_ID == project.ProjectIdOrigineel).FirstOrDefault();
            MandagenRegistratieDomain.adressen dboadres = dbrepOriginal.datacontext.adressens.Where(a => a.adres_id == dboproject.adres_id_bouw).FirstOrDefault();

            project.Naam = txtProjectnaam.Text;
            project.Actief = cbActief.IsChecked == true;
            project.Postcode = txtPostcodeCijfers.Text + txtPostcodeLetters.Text;
            project.Huisnummer = txtHuisnummer.Text;
            project.Adres = txtAdres.Text;

            //dbrep.SaveProject(project);
            dbrep.datacontext.SubmitChanges();

            if (txtPostcodeCijfers.Text != "")
            {
                dboadres.postcode_cijfers = Convert.ToInt32(txtPostcodeCijfers.Text);
            }

            dboadres.postcode_letters = txtPostcodeLetters.Text;
            dboadres.straat_1 = txtAdres.Text;
            dboadres.huis_postbus_nummer = txtHuisnummer.Text;

            dbrepOriginal.datacontext.SubmitChanges();

            // na het saven terug in history
            PageGoBack();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Load()
        {
            dbRepository dbrep = new dbRepository();

            MandagenRegistratieDomain.Project project = dbrep.GetProject(ApplicationState.GetValue<int>(ApplicationVariables.intProjectId));

            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
            persoon persoon = dbrepOriginal.GetContact(dbrep.GetProjectleider(project.ProjectleiderId).ContactIdOrigineel);

            MandagenRegistratieDomain.project dboproject = dbrepOriginal.datacontext.projects.Where(p => p.project_ID == project.ProjectIdOrigineel).FirstOrDefault();
            MandagenRegistratieDomain.adressen dboadres = dbrepOriginal.datacontext.adressens.Where(a => a.adres_id == dboproject.adres_id_bouw).FirstOrDefault();


            lblProjectNr.Content = dbrepOriginal.GetProject(project.ProjectIdOrigineel).project_NR.ToString();

            cbActief.IsChecked = project.Actief == true;

            if (dboadres != null)
            {
                txtPostcodeCijfers.Text = dboadres.postcode_cijfers.HasValue ? dboadres.postcode_cijfers.Value.ToString() : "";
                txtPostcodeLetters.Text = dboadres.postcode_letters;
                txtHuisnummer.Text = dboadres.huis_postbus_nummer + dboadres.huisnummer_toevoeging;
                txtAdres.Text = dboadres.straat_1;
                AdresLookup = dboadres.straat_1.Substring(0, dboadres.straat_1.Length - txtHuisnummer.Text.Length).Trim();
            }

            txtProjectnaam.Text = dboproject.naam_project;
            lblProjectleider.Content = persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam;

        }

        public void AutoVulAdres(object sender, KeyEventArgs e)
        {
            string _postcode = txtPostcodeCijfers.Text + txtPostcodeLetters.Text;

            Tools tool = new Tools();

            var regx = new Regex(@"(\d{4}\s*)(\D{2})");
            var match = regx.Match(_postcode);

            if (match.Success)
            {
                string straat = tool.ZoekOpPostcode(_postcode);

                txtAdres.Text = straat;
                AdresLookup = straat;
                ChangeAdres = true;

            }
        }

        public void AdresAanvullen(object sender, KeyEventArgs e)
        {
            string huisnummer = txtHuisnummer.Text;

            if (ChangeAdres)
            {
                txtAdres.Text = AdresLookup + " " + huisnummer;
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
            txtbox.Margin = new Thickness(6, 0, 0, 0);
            txtbox.Width = 400;
            txtbox.Name = id;

            wpDetailsEdit.Children.Add(lbl);
            wpDetailsEdit.Children.Add(txtbox);

        }

        /// <summary>
        /// Voeg dit item toe aan het panel
        /// </summary>
        /// <param name="label"></param>
        /// <param name="waarde"></param>
        public void AddCheckboxToPanel(string id, string label, bool waarde)
        {

            Label lbl = new Label();
            lbl.Content = label;
            lbl.Width = 200;
            lbl.Background = Brushes.Bisque;

            CheckBox chbox = new CheckBox();
            chbox.IsChecked = waarde;
            chbox.Margin = new Thickness(6, 6, 0, 0);
            chbox.Width = 400;
            chbox.Name = id;

            wpDetailsEdit.Children.Add(lbl);
            wpDetailsEdit.Children.Add(chbox);

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

    }
}

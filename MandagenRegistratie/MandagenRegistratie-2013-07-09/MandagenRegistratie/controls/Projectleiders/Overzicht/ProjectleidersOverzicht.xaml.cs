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
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Projectleiders.Overzicht
{
    /// <summary>
    /// Interaction logic for ProjectleidersOverzicht.xaml
    /// </summary>
    public partial class ProjectleidersOverzicht : MenuControl
    {
        public ProjectleidersOverzicht()
        {
            InitializeComponent();
            #region Pagina specifieke informatie
            PageTitle = "Projectleiders overzicht";
            PageSubtitle = "Verander van ingelogde projectleider om te kunnen testen als iemand anders";
            PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            PageBackButtonText = "ANNULEER";
            PageOKButtonText = "INLOGGEN ALS..";
            #endregion

            this.Reloaded += Load;
            this.OkClick += Inloggen;

            Load();
        }

        class PL
        {
            public PL()
            {

            }


            public string Naam { get; set; }
            public int ProjectleiderId { get; set; }
            public int ContactIdOrigineel { get; set; }
        }


        /// 
        /// </summary>
        public void Load()
        {

            // intialize
            dbRepository dbrep = new dbRepository();
            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();

            List<PL> listPL = new List<PL>();

            foreach(Projectleider plitem in dbrep.GetProjectleiders())
            {
                persoon tt = dbrepOriginal.GetContact(plitem.ContactIdOrigineel);

                PL pl = new PL();
                pl.Naam = tt.voornaam + " " + tt.tussenvoegsel + " " + tt.achternaam;
                pl.ProjectleiderId = plitem.ProjectleiderId;
                pl.ContactIdOrigineel = plitem.ContactIdOrigineel;

                listPL.Add(pl);
            }

            cbProjectleiders.ItemsSource = listPL;
            cbProjectleiders.DisplayMemberPath = "Naam";
            cbProjectleiders.SelectedValuePath = "ProjectleiderId";

            cbProjectleiders.SelectedValue = ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider);


        }


        public void Inloggen()
        {
            ApplicationState.SetValue(ApplicationVariables.intProjectleider, (int)cbProjectleiders.SelectedValue);

            // na het saven terug in history
            PageGoBack();

        }
    }
}

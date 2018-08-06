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
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Vakmannen.Overzicht
{
    /// <summary>
    /// Interaction logic for ContactenOverview.xaml
    /// </summary>
    public partial class ContactenOverview : MenuControl
    {
        public static int ZeebregtsBedrijfId = 1;
        public List<vwVakman> listViewVakmannen = new List<vwVakman>();

        public ContactenOverview()
        {

            InitializeComponent();
            PageGereedButtonVisibility = System.Windows.Visibility.Hidden;
            PageTitle = "Vakman toevoegen";
            PageSubtitle = "Aan vakmannenlijst";

            //TerugButtonText = "BACK2";
            //PageOKButtonText = "Nieuwe vakman";
            OkClick += Toevoegen;
            Reloaded += ZoekResultaten;

            //ZoekResultaten();
            Loaded += ContactenOverview_Loaded;
        }

        public void ContactenOverview_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(txtSearch);
            txtSearch.Focus();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ZoekResultaten();
        }

        private void ZoekResultaten()
        {
            if (txtSearch != null)
            {
                dbOriginalRepository dbrep = new dbOriginalRepository();
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    dgContacten.ItemsSource = dbrep.GetContacten().Where(c => !listViewVakmannen.Any(v => c.persoon_ID == v.ContactIdOrigineel));
                }
                else
                {
                    dgContacten.ItemsSource = dbrep.GetContacten(txtSearch.Text.Split(' ')).Where(c => !listViewVakmannen.Any(v => c.persoon_ID == v.ContactIdOrigineel));
                }
            }
        }


        private void Toevoegen()
        {

            bool IsNewVakman = false;

            dbRepository dbrep = new dbRepository();
            dbOriginalRepository dbOriginalRep = new dbOriginalRepository();

            persoon persoon = (persoon)dgContacten.SelectedItem;


            // check of de vakman al bestaat in MDR
            Vakman vakman = dbrep.GetVakmanByOriginalId(persoon.persoon_ID);
            if (vakman == null)
            {
                // bestond nog niet, dus toevoegen
                IsNewVakman = true;

                Vakman vakmanNew = new Vakman();

                // check of er een bedrijf aan gekoppeld moet worden
                if (persoon.bedrijf_nr != null)
                {
                    // haal het originele bedrijf op
                    bedrijf dbobedrijf = dbOriginalRep.GetBedrijf((int)persoon.bedrijf_nr);

                    // toevoegen indien het niet bestaat
                    Bedrijf bedrijf = dbrep.GetBedrijf(dbobedrijf.bedrijf_ID);
                    if (bedrijf == null)
                    {
                        Bedrijf bedrijfNew = new Bedrijf();
                        bedrijfNew.BedrijfIdOrigineel = dbobedrijf.bedrijf_ID;
                        bedrijf = dbrep.InsertBedrijf(bedrijfNew);
                    }

                    if (dbobedrijf.bedrijf_ID == ZeebregtsBedrijfId)
                    {
                        vakmanNew.Intern = true;
                    }
                    else
                    {
                        vakmanNew.Intern = false;
                    }

                    vakmanNew.BedrijfId = bedrijf.BedrijfId;

                }

                // overige info van de vakman invullen
                vakmanNew.Actief = true;
                vakmanNew.ContactIdOrigineel = persoon.persoon_ID;
                vakmanNew.ProjectleiderId = ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider);
                vakmanNew.Bsn = persoon.voornaam + " " + (String.IsNullOrEmpty(persoon.tussenvoegsel) ? "" : persoon.tussenvoegsel + " ") + persoon.achternaam;
                vakmanNew.Postcode = "";
                vakmanNew.Huisnummer = "";
                vakmanNew.Adres = "";
                vakmanNew.Ma = 8;
                vakmanNew.Di = 8;
                vakmanNew.Wo = 8;
                vakmanNew.Do = 8;
                vakmanNew.Vr = 8;
                vakmanNew.Za = 0;
                vakmanNew.Zo = 0;
                vakmanNew.Werkweek = 40;
                vakmanNew.DefaultBeginuur = 0;
                vakmanNew.DefaultBeginminuut = 0;

                // voeg toe aan database
                vakman = dbrep.InsertVakman(vakmanNew);
            }

            // bewaar info in 'sessie'
            //ApplicationState.SetValue(ApplicationVariables.objVakman, vakman);
            ApplicationState.SetValue(ApplicationVariables.intVakmanId, vakman.VakmanId);

            DateTime nu = DateTime.Now;
            //ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, new DateTime(nu.Year, nu.Month, nu.Day, 0, 0, 0));

            PageGoBack();

        }

        private void dgContacten_DoubleClick(object sender, MouseEventArgs e)
        {
            Toevoegen();
        }

        private void DockPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (Keyboard.FocusedElement.GetType() != typeof(DataGridCell))
                {
                    dgContacten.Focus();
                    dgContacten.SelectedIndex = 0;
                    dgContacten.CurrentCell = dgContacten.SelectedCells[0];
                    dgContacten.SelectedItem = dgContacten.Items[0];
                }

            }
            // == dgProjecten.SelectedCells[0]
            else if (e.Key == Key.Up && dgContacten.SelectedIndex == 0)
            {
                txtSearch.Focus();
            }
            else if (e.Key == Key.Enter)
            {
                dgContacten_DoubleClick(dgContacten, null);
            }

        }

    }
}

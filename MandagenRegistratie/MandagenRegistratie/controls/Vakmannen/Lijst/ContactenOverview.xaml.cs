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

namespace MandagenRegistratie.controls.Vakmannen.Lijst
{
    /// <summary>
    /// Interaction logic for ContactenOverview.xaml
    /// </summary>
    public partial class ContactenOverview : MenuControl
    {
        public static int ZeebregtsBedrijfId = 1;
        public List<vwVakman> listViewVakmannen = new List<vwVakman>();
        public bool SubRoute;

        public ContactenOverview(bool blnSubRoute)
        {

            InitializeComponent();
            PageGereedButtonVisibility = System.Windows.Visibility.Hidden;
            PageTitle = "Nieuwe vakman";
            PageSubtitle = "Voeg vakman toe aan vakmanlijst en toon vakmangegevens";

            PageBackButtonText = "ANNULEER";
            PageOKButtonText = "TOEVOEGEN";
            OkClick += Toevoegen;
            Reloaded += ZoekResultaten;

            
            //ZoekResultaten();
            Loaded += ContactenOverview_Loaded;

            SubRoute = blnSubRoute;

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

        public void ZoekResultaten()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            dbRepository dbrep = new dbRepository();
            if (dbrep.IsStandalone())
            {
                dgContacten.IsReadOnly = false;
                dgContacten.CanUserAddRows = true;
            }



            if (txtSearch != null)
            {
                List<vwContactAll> listPersonen = new List<vwContactAll>();

                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    listPersonen = dbrep.GetContacten().Where(c => !listViewVakmannen.Any(v => c.persoon_ID == v.ContactIdOrigineel)).OrderBy(vv => vv.bedrijf_nr).ThenBy(vv => vv.voornaam).ThenBy(vv => vv.achternaam).ToList();
                    dgContacten.ItemsSource = listPersonen;
                }
                else
                {
                    listPersonen = dbrep.GetContacten(txtSearch.Text.Split(' ')).Where(c => !listViewVakmannen.Any(v => c.persoon_ID == v.ContactIdOrigineel)).OrderBy(vv => vv.bedrijf_nr).ThenBy(vv => vv.voornaam).ThenBy(vv => vv.achternaam).ToList();
                    dgContacten.ItemsSource = listPersonen;

                    // probeer de eerste te selecteren
                    vwContactAll firstItem = listPersonen.FirstOrDefault();
                    if (firstItem != null)
                    {
                        SelectItem(firstItem.persoon_ID);
                    }

                }


            }

            Keyboard.Focus(txtSearch);

            Mouse.OverrideCursor = null;

        }

        public string ToonNaam(MDRpersoon objPersoon)
        {
            return objPersoon.voornaam + " " + (string.IsNullOrWhiteSpace(objPersoon.tussenvoegsel) ? "" : objPersoon.tussenvoegsel + " ") + objPersoon.achternaam;
        }

        private void Toevoegen()
        {
            try
            {
                if (dgContacten.SelectedItem != null)
                {
                    bool IsNewVakman = false;

                    dbRepository dbrep = new dbRepository();
                    dbOriginalRepository dbOriginalRep = new dbOriginalRepository();

                    // blijkbaar bestaat er de mogelijkheid dat er zojuist een contact is toegevoegd aan de ZeebregtsDb
                    // vandaar voor de zekerheid opnieuw de ZeebregtsDb Cachen

                    ApplicationState.SetValue(ApplicationVariables.listMDRPersoons, dbOriginalRep.datacontext.MDRpersoons.ToList());
                    ApplicationState.SetValue(ApplicationVariables.listMDRProjecten, dbOriginalRep.datacontext.MDRprojects.ToList());


                    vwContactAll persoon = (vwContactAll)dgContacten.SelectedItem;

                    if (persoon.persoon_nr == null && dbrep.IsStandalone())
                    {
                        // via stored procedure de mdrpersoon toevoegen aan de database in tabel MDRpersoon
                        // en het ID wat terugkomt in persoon.persoon_ID stoppen
                        persoon.persoon_ID = dbrep.InsertMDRPersoon("nieuwe persoon");

                    }

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
                            MDRbedrijf dbobedrijf = dbOriginalRep.GetBedrijf((int)persoon.bedrijf_nr);

                            // nieuw, in deze tabel opslaan
                            vakmanNew.BedrijfIdOrigineel = dbobedrijf.bedrijf_ID;

                            if (dbobedrijf.bedrijf_ID == ZeebregtsBedrijfId)
                            {
                                vakmanNew.Intern = true;
                            }
                            else
                            {
                                vakmanNew.Intern = false;
                            }

                        }

                        // overige info van de vakman invullen
                        vakmanNew.ContactIdOrigineel = persoon.persoon_ID;
                        vakmanNew.ProjectleiderId = ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider);
                        vakmanNew.Bsn = ""; // persoon.voornaam + " " + (String.IsNullOrWhiteSpace(persoon.tussenvoegsel) ? "" : persoon.tussenvoegsel + " ") + persoon.achternaam;
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

                    if (SubRoute)
                    {
                        PageGoBack();
                    }
                    else
                    {
                        dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
                        MDRpersoon objPersoon = dbrepOriginal.GetContact(persoon.persoon_ID);

                        // create the page and load all values
                        Vakmannen.Detail.VakmanDetailView vdv = new Vakmannen.Detail.VakmanDetailView(objPersoon);
                        vdv.SelectTab(1);

                        // load the page into the contentcontrol
                        PageGoBack(vdv);


                    }
                }
            }
            catch
            {
                MessageBox.Show("Er is een onbekende fout opgetreden, error #701");
            }
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

        private void txtSearch_PreviewKeyUp_1(object sender, KeyEventArgs e)
        {
            ZoekResultaten();
        }

        private void SelectItem(int persoonId)
        {

            try
            {
                int count = 0;

                foreach (vwContactAll vwPersoon in dgContacten.Items)
                {
                    if (persoonId == vwPersoon.persoon_ID)
                    {
                        dgContacten.Focus();
                        //dgProjecten.SelectedIndex = 0;
                        //dgProjecten.CurrentCell = dgProjecten.SelectedCells[0];
                        //dgProjecten.SelectedItem = dgProjecten.Items[0];


                        dgContacten.SelectedIndex = count;
                        dgContacten.CurrentCell = dgContacten.SelectedCells[0];
                        dgContacten.SelectedItem = vwPersoon;

                        break;
                    }
                    count++;
                }
            }
            catch
            {
                MessageBox.Show("Er is een onbekende fout opgetreden, error #702");
            }

        }

    }
}

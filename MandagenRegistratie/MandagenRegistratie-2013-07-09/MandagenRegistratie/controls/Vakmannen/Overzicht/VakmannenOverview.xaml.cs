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
using System.ComponentModel;
using MandagenRegistratie.controls.Vakmannen.Dagview;
using MandagenRegistratie.controls.Vakmannen.Detail;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Vakmannen.Overzicht
{

    /// <summary>
    /// Interaction logic for VakmannenOverview.xaml
    /// </summary>
    public partial class VakmannenOverview : MenuControl
    {

        public static int ZeebregtsBedrijfId = 1;
        public List<int> listVakmannen = new List<int>();
        public List<vwVakman> listViewVakmannen = new List<vwVakman>();

        public VakmannenOverview()
        {

            InitializeComponent();

            #region Pagina specifieke informatie
            PageTitle = "Vakmannen overzicht";
            //PageSubtitle = "Zoek op deze pagina naar vakmannen. Klik op 'Toon alle contacten' om ook in het originele systeem te zoeken";
            PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            PageOKButtonText = "Nieuwe vakman";
            #endregion

            this.OkClick += Okay;
            this.Reloaded += ZoekResultaten;
            this.CustomActieClick += SelectVakman;

            Loaded += VakmannenOverview_Loaded;
        }


        private void SelectItem(int vakmanId)
        {

            int count = 0;

            foreach (vwVakman vwVakman in dgVakmannen.Items)
            {
                if (vakmanId == vwVakman.VakmanId)
                {
                    dgVakmannen.Focus();
                    //dgProjecten.SelectedIndex = 0;
                    //dgProjecten.CurrentCell = dgProjecten.SelectedCells[0];
                    //dgProjecten.SelectedItem = dgProjecten.Items[0];


                    dgVakmannen.SelectedIndex = count;
                    dgVakmannen.CurrentCell = dgVakmannen.SelectedCells[0];
                    dgVakmannen.SelectedItem = vwVakman;

                    break;
                }
                count++;
            }

        }

        public void VakmannenOverview_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(txtSearch);
            txtSearch.Focus();

            if (ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId) > 0)
            {
                SelectItem(ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId));
            }

        }

        public void LoadView()
        {
            ZoekResultaten();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here

            //this.Dispatcher.Invoke((Action)(() =>
            //{
            //    ZoekResultaten();
            //}));

        }

        private void worker_RunWorkerCompleted(object sender,
                                               RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
        }

        public void Okay()
        {
            ContactenOverview co = new ContactenOverview();
            co.listViewVakmannen = listViewVakmannen;
            //co.PageReloaded();
            PageGoToPage(co);

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

                ZoekResultaten();

        }

        private void ZoekResultaten()
        {
            if (txtSearch != null)
            {
                dbRepository dbrep = new dbRepository();
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    listViewVakmannen = dbrep.GetViewVakmannenAll().Where(v => !listVakmannen.Contains(v.VakmanId)).ToList();
                    dgVakmannen.ItemsSource = listViewVakmannen;
                }
                else
                {
                    listViewVakmannen = dbrep.GetViewVakmannen(txtSearch.Text.Split(' ')).Where(v => !listVakmannen.Contains(v.VakmanId)).ToList();
                    dgVakmannen.ItemsSource = listViewVakmannen;
                }

                // set de focus op de zoek textbox
                txtSearch.Focus();

            }


        }


        public void dgVakmannen_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PageCustomActie();

            //SelectVakman();
        }

        public void SelectVakman()
        {
            if (dgVakmannen.SelectedIndex >= 0)
            {
                dbRepository dbrep = new dbRepository();


                Vakman vm = dbrep.GetVakman(((vwVakman)dgVakmannen.SelectedItem).VakmanId);

                //ApplicationState.SetValue(ApplicationVariables.objVakman, vm);
                ApplicationState.SetValue(ApplicationVariables.intVakmanId, vm.VakmanId);


                dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
                persoon persoon = dbrepOriginal.GetContact(vm.ContactIdOrigineel);

                // create the page and load all values
                Vakmannen.Detail.VakmanDetailView vdv = new Vakmannen.Detail.VakmanDetailView(persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam);

                // load the page into the contentcontrol
                PageGoToPage(vdv);
            }

        }

        public void dgVakmannen_MouseDoubleClickForProject(object sender, MouseButtonEventArgs e)
        {
            ToevoegenAanProject();
        }

        public void ToevoegenAanProject()
        {
            dbRepository dbrep = new dbRepository();

            // haal het geselecteerde project op
            MandagenRegistratieDomain.Vakman vakman = dbrep.GetVakman(((MandagenRegistratieDomain.vwVakman)dgVakmannen.SelectedItem).VakmanId);
            MandagenRegistratieDomain.Project project = dbrep.GetProject(ApplicationState.GetValue<int>(ApplicationVariables.intProjectId));

            if (vakman != null)
            {
                VakmanDagView vdv = new VakmanDagView();
                vdv.ProjectToevoegen(project, vakman.VakmanId, ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider));

                // zorgen dat de pagina's op andere schermen verversen

                List<int> vakmanIds = new List<int>();

                foreach (vwVakman vm in dbrep.GetVakmannenByProjectId(project.ProjectId, Tools.CalculateWeekstart(ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay))))
                {
                    vakmanIds.Add(vm.VakmanId);
                }


                PageChannelMessage("projectdagview", Tools.CreateChannelMessage(vakmanIds, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay), ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay).AddDays(1)));


                PageGoBack();





            }
            else
            {

                //MessageBox.Show("Selecteer aub een vakman, of klik op annulleer");
            }

        }

        private void StackPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (Keyboard.FocusedElement.GetType() != typeof(DataGridCell))
                {
                    dgVakmannen.Focus();
                    dgVakmannen.SelectedIndex = 0;
                    dgVakmannen.CurrentCell = dgVakmannen.SelectedCells[0];
                    dgVakmannen.SelectedItem = dgVakmannen.Items[0];
                }

            }
            // == dgProjecten.SelectedCells[0]
            else if (e.Key == Key.Up && dgVakmannen.SelectedIndex == 0)
            {
                txtSearch.Focus();
            }
            else if (e.Key == Key.Enter)
            {
                PageCustomActie();
                //dgVakmannen_MouseDoubleClick(dgVakmannen, null);
            }

        }


    }
}

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

namespace MandagenRegistratie.controls.Vakmannen.Lijst
{

    /// <summary>
    /// Interaction logic for VakmannenOverview.xaml
    /// </summary>
    public partial class VakmannenOverview : MenuControl
    {

        public static int ZeebregtsBedrijfId = 1;
        public List<int> listVakmannen = new List<int>();
        public List<vwVakman> listViewVakmannen = new List<vwVakman>();

        public bool blnRememberSelectedDay = false;
        public bool SubRoute;

        public VakmannenOverview(bool blnSubRoute)
        {

            InitializeComponent();

            #region Pagina specifieke informatie
            PageTitle = "Vakmannenlijst";
            PageSubtitle = "Toon vakmanuren op maandag week " + Tools.GetWeekNumber(DateTime.Now).ToString();

            if (Rechten.IsProjectleider)
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Hidden;
            }


            if (blnSubRoute)
            {
                PageBackButtonText = "ANNULEER";
            }
            else
            {
                PageBackButtonText = "TERUG";
            }


            PageOKButtonText = "NIEUWE VAKMAN";

            #endregion

            //dgVakmannen.EnableColumnVirtualization = true;
            //dgVakmannen.EnableRowVirtualization = true;

            this.OkClick += Okay;
            this.Reloaded += ZoekResultaten;
            this.CustomActieClick += SelectVakman;
            this.ChannelMessage += VakmannenOverview_ChannelMessage;

            Loaded += VakmannenOverview_Loaded;

            SubRoute = blnSubRoute;

        }

        private void VakmannenOverview_ChannelMessage(string sender, string message)
        {
            // throw new NotImplementedException();
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

            if (ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId) > 0)
            {
                SelectItem(ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId));
            }

            Keyboard.Focus(txtSearch);
            txtSearch.Focus();

        }

        public void LoadView()
        {
            dgVakmannen.EnableColumnVirtualization = true;
            dgVakmannen.EnableRowVirtualization = true;

            // TODO, test: asynchronous uitvoeren...
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                ZoekResultaten();
            }));

            //ZoekResultaten();
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
            ContactenOverview co = new ContactenOverview(SubRoute);
            co.listViewVakmannen = listViewVakmannen;

            co.ZoekResultaten();

            //co.PageReloaded();
            PageGoToPage(co);

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

                ZoekResultaten();

        }

        private void ZoekResultaten()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            // zet de geselecteerde dag op de eerste dag van de week (van de datum die momenteel geselecteerd is/was)
            if (!blnRememberSelectedDay)
            {
                ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, Tools.CalculateWeekstart(DateTime.Now));
            }

            // ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, Tools.CalculateWeekstart(ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay)));

            if (txtSearch != null)
            {
                dbRepository dbrep = new dbRepository();
                if (string.IsNullOrEmpty(txtSearch.Text))
                {
                    listViewVakmannen = dbrep.GetViewVakmannenAll().Where(v => !listVakmannen.Contains(v.VakmanId)).OrderBy(vv => vv.bedrijfnaam).ThenBy(vv => vv.voornaam).ThenBy(vv => vv.achternaam).ToList();
                    dgVakmannen.ItemsSource = listViewVakmannen;

                    dgVakmannen.InvalidateMeasure();
                    dgVakmannen.InvalidateArrange();
                }
                else
                {
                    listViewVakmannen = dbrep.GetViewVakmannen(txtSearch.Text.Split(' ')).Where(v => !listVakmannen.Contains(v.VakmanId)).OrderBy(vv => vv.bedrijfnaam).ThenBy(vv => vv.voornaam).ThenBy(vv => vv.achternaam).ToList();
                    dgVakmannen.ItemsSource = listViewVakmannen;
                    dgVakmannen.InvalidateMeasure();
                    dgVakmannen.InvalidateArrange();
                    // probeer de eerste te selecteren
                    vwVakman firstItem = listViewVakmannen.FirstOrDefault();
                    if (firstItem != null)
                    {
                        SelectItem(firstItem.VakmanId);
                    }

                }

                // set de focus op de zoek textbox
                txtSearch.Focus();

            }

            Keyboard.Focus(txtSearch);

            Mouse.OverrideCursor = null;

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

                ApplicationState.SetValue(ApplicationVariables.intVakmanViewModus, 1);

                dbRepository dbrep = new dbRepository();


                Vakman vm = dbrep.GetVakman(((vwVakman)dgVakmannen.SelectedItem).VakmanId);

                //ApplicationState.SetValue(ApplicationVariables.objVakman, vm);
                ApplicationState.SetValue(ApplicationVariables.intVakmanId, vm.VakmanId);


                dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
                MDRpersoon persoon = dbrepOriginal.GetContact(vm.ContactIdOrigineel);

                // create the page and load all values
                Vakmannen.Detail.VakmanDetailView vdv = new Vakmannen.Detail.VakmanDetailView(persoon);

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

            Logging log = new Logging();
            //log.Log("ToevoegenAanProject()");



            // haal het geselecteerde project op
            MandagenRegistratieDomain.Vakman vakman = dbrep.GetVakman(((MandagenRegistratieDomain.vwVakman)dgVakmannen.SelectedItem).VakmanId);
            MandagenRegistratieDomain.Project project = dbrep.GetProject(ApplicationState.GetValue<int>(ApplicationVariables.intProjectId));

            if (vakman != null)
            {
                VakmanDagView vdv = new VakmanDagView();
                vdv.ProjectToevoegen(project, vakman.VakmanId, ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider));


                if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsAdding) == null)
                {
                    ApplicationState.SetValue(ApplicationVariables.listSelectedVakmanIdsAdding, new List<int>());
                }

                // voorkom duplicate entries
                if (!ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsAdding).Contains(vakman.VakmanId))
                {
                    ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsAdding).Add(vakman.VakmanId);
                }



                // zorgen dat de pagina's op andere schermen verversen

                List<int> vakmanIds = new List<int>();

                foreach (vwVakman vm in dbrep.GetVakmannenByProjectId(project.ProjectId, Tools.CalculateWeekstart(ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay))))
                {
                    vakmanIds.Add(vm.VakmanId);
                }


                PageChannelMessage("projectdagview", Tools.CreateChannelMessage(vakmanIds, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay), ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay).AddDays(1)));


                PageGoBack();



                //log.Log("EINDE: ToevoegenAanProject()");


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

        private void txtSearch_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            ZoekResultaten();
        }

        private void MenuControl_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            if (Window.GetWindow(this) != null)
            {
                dgVakmannen.Width = Window.GetWindow(this).ActualWidth - 10;
            }

        }


    }
}

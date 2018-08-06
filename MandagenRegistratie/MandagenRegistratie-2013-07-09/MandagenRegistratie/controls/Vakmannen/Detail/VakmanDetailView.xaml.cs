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

namespace MandagenRegistratie.controls.Vakmannen.Detail
{
    /// <summary>
    /// Interaction logic for VakmanDetailView.xaml
    /// </summary>
    public partial class VakmanDetailView : MenuControl
    {
        public VakmanDetailView(string pageTitle)
        {
            InitializeComponent();

            #region Pagina specifieke informatie
            PageTitle = "Vakmanview";
            PageSubtitle = "Planning van " + pageTitle;
            PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            PageOKButtonText = "WIJZIG";
            #endregion

            // load info op pagina
            this.Load();

            OkClick += Okay;
            Reloaded += Load;
//#if DEBUG
//#else

            ChannelMessage += ReceiveChannelMessage;
//#endif

        }

        /// <summary>
        /// Voert de OKAY functie uit
        /// </summary>
        public void Okay()
        {

            VakmanDetailEdit vde = new VakmanDetailEdit(PageTitle);
            vde.Load();

            PageGoToPage(vde);

        }

        /// <summary>
        /// Voert de OKAY functie uit
        /// </summary>
        public void ReceiveChannelMessage(string sender, string message)
        {
            ((VakmanDagView)tabPlanning.Content).ReceiveChannelMessage(sender, message);
        }


        Vakman objVakman = null;
        DateTime dtSelectedDay = DateTime.MinValue;

        public void Load()
        {
            dbRepository dbrep = new dbRepository();

            if(objVakman == null)
            {
                objVakman = dbrep.GetVakman(ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId));
            }

            //if (dtSelectedDay == DateTime.MinValue)
            //{
                dtSelectedDay = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
            //}

            Vakman vm = objVakman;

            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
            persoon persoon = dbrepOriginal.GetContact(vm.ContactIdOrigineel);

            lblVakmanId.Content = vm.ContactIdOrigineel.ToString();

            bedrijf bedrijfzdb = dbrepOriginal.datacontext.bedrijfs.Where(b => b.bedrijf_nr == persoon.bedrijf_nr).FirstOrDefault();

            if (bedrijfzdb != null)
            {
                lblWerkgever.Content = bedrijfzdb.naam;
            }



            lblVakmanNaam.Content = persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam;
            txtBsn.Content = vm.Bsn;
            txtPostcode.Content = vm.Postcode;
            txtHuisnummer.Content = vm.Huisnummer;
            txtAdres.Content = vm.Adres;

            txtMa.Content = vm.Ma.ToString();
            txtDi.Content = vm.Di.ToString();
            txtWo.Content = vm.Wo.ToString();
            txtDo.Content = vm.Do.ToString();
            txtVr.Content = vm.Vr.ToString();
            txtZa.Content = vm.Za.ToString();
            txtZo.Content = vm.Zo.ToString();

            ddlDefaultBeginuur.Content = ((int)vm.DefaultBeginuur).ToString("D2");
            ddlDefaultBeginminuut.Content = ((int)vm.DefaultBeginminuut).ToString("D2");

            lblVakmanWerkweek.Content = vm.Werkweek.ToString();              
            
            // create the page and load all values
            VakmanDagView vdv = new VakmanDagView();
            vdv.LoadVakmanDagView(true, vm.VakmanId, dtSelectedDay);
            vdv.LoadWeekInfo();

            tabPlanning.Content = vdv;

                

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


            wpDetails.Children.Add(lbl);
            wpDetails.Children.Add(lbl2);

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

            wpDetails.Children.Add(lbl);

        }

        private void MenuControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Window.GetWindow(this) != null)
            {
                //tabControl.Height = Window.GetWindow(this).ActualHeight - 110;
                //tabControl.Height = Window.GetWindow(this).ActualHeight - 120;
                tabControl.Width = Window.GetWindow(this).ActualWidth;
            }

        }

        private void tabPlanning_GotFocus(object sender, RoutedEventArgs e)
        {
            PageContainer pc = (PageContainer)Tools.FindVisualParent<Window>(this);
            pc.SetLabelSubtitlePlanning();
        }

        private void tabDetails_GotFocus(object sender, RoutedEventArgs e)
        {
            PageContainer pc = (PageContainer)Tools.FindVisualParent<Window>(this);
            pc.SetLabelSubtitleGegevens();
        }

    }
}

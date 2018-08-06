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
        public VakmanDetailView(MDRpersoon pp)
        {
            InitializeComponent();

            #region Pagina specifieke informatie
            PageTitle = "Vakman " + ToonNaam(pp); // pp.persoon_nr;
            PageSubtitle = "Gegevens van " + ToonNaam(pp);

            if (Rechten.IsProjectleider)
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Hidden;
            }

            PageOKButtonText = "WIJZIG";
            #endregion

            //MessageBox.Show("#0");

            // load info op pagina
            this.Load();

            OkClick += Okay;
            Reloaded -= Load;
            Reloaded += Load;
            //#if DEBUG
//#else
            Loaded += Vakman_Loaded;

            ChannelMessage += ReceiveChannelMessage;
//#endif

        }

        public void SelectTab(int intSelectedTab)
        {

             tabControl.SelectedIndex = intSelectedTab;

        }

        public void Vakman_Loaded(object sender, RoutedEventArgs e)
        {

            if (tabDetails.IsSelected)
            {
                PageContainer pc = (PageContainer)Tools.FindVisualParent<Window>(this);
                if (Rechten.IsProjectleider)
                {
                    pc.SetLabelSubtitleGegevens(true);
                }
                else
                {
                    pc.SetLabelSubtitleGegevens(false);
                }
            }
            else
            {
                PageContainer pc = (PageContainer)Tools.FindVisualParent<Window>(this);
                if (Rechten.IsProjectleider)
                {
                    pc.SetLabelSubtitlePlanning(true);
                }
                else
                {
                    pc.SetLabelSubtitlePlanning(false);
                }
            }

        }


        /// <summary>
        /// Voert de OKAY functie uit
        /// </summary>
        public void Okay()
        {

            VakmanDetailEdit vde = new VakmanDetailEdit(PageSubtitle);
            vde.Load();

            PageGoToPage(vde);

        }

        public string ToonNaam(MDRpersoon objPersoon)
        {
            return (objPersoon.voornaam + " " +  objPersoon.tussenvoegsel + " " + objPersoon.achternaam).ToStringTrimmed();
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

            //MessageBox.Show("#1");
            Logging log = new Logging();
            //log.Log("Vakmandetailview: Load()");

            dbRepository dbrep = new dbRepository();
            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();

            //if(objVakman == null)
            //{

            objVakman = dbrep.GetVakman(ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId));

            //}

            if (objVakman.ZZP == true)
            {
                lbWerkrelatie.Content = "Bedrijfsnaam:";
                lblWerkrelatie.Content = "ZZP'er";
            }
            else
            {
                lbWerkrelatie.Content = "Werkgever:";
                lblWerkrelatie.Content = "In loondienst";
            }

            //if (dtSelectedDay == DateTime.MinValue)
            //{
            dtSelectedDay = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
            //}

            Vakman vm = objVakman;

            MDRpersoon persoon = dbrepOriginal.GetContact(vm.ContactIdOrigineel);

            if (persoon != null)
            {
                txtPostcode.Content = vm.Postcode + (string.IsNullOrWhiteSpace(vm.Plaats) || string.IsNullOrWhiteSpace(vm.Postcode) ? "" : ", ") + vm.Plaats;

                txtAdres.Content = vm.Adres + " " + vm.Huisnummer;
                //txtPlaats.Content = vm.Plaats;
                txtLand.Content = vm.Land;

                bool showWoonadresLabel = false;

                if (string.IsNullOrWhiteSpace(vm.Postcode) && string.IsNullOrWhiteSpace(vm.Plaats))
                {
                    lblPostcode.Visibility = System.Windows.Visibility.Collapsed;
                    txtPostcode.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    lblPostcode.Visibility = System.Windows.Visibility.Visible;
                    txtPostcode.Visibility = System.Windows.Visibility.Visible;
                    showWoonadresLabel = true;

                    if (string.IsNullOrWhiteSpace(vm.Postcode))
                    {
                        lblPostcode.Content = "Plaats";
                    }
                    else if (string.IsNullOrWhiteSpace(vm.Plaats))
                    {
                        lblPostcode.Content = "Postcode";
                    }
                    else
                    {
                        lblPostcode.Content = "Postcode, plaats";
                    }
                }

                if (string.IsNullOrWhiteSpace(vm.Adres))
                {
                    lblAdres.Visibility = System.Windows.Visibility.Collapsed;
                    txtAdres.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    lblAdres.Visibility = System.Windows.Visibility.Visible;
                    txtAdres.Visibility = System.Windows.Visibility.Visible;
                    showWoonadresLabel = true;
                }

                //if (string.IsNullOrWhiteSpace(vm.Plaats))
                //{
                //    lblPlaats.Visibility = System.Windows.Visibility.Collapsed;
                //    txtPlaats.Visibility = System.Windows.Visibility.Collapsed;
                //}
                //else
                //{
                //    lblPlaats.Visibility = System.Windows.Visibility.Visible;
                //    txtPlaats.Visibility = System.Windows.Visibility.Visible;
                //    showWoonadresLabel = true;
                //}


                if (string.IsNullOrWhiteSpace(vm.Land))
                {
                    lblLand.Visibility = System.Windows.Visibility.Collapsed;
                    txtLand.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    lblLand.Visibility = System.Windows.Visibility.Visible;
                    txtLand.Visibility = System.Windows.Visibility.Visible;
                    showWoonadresLabel = true;
                }

                if (showWoonadresLabel)
                {
                    lblWoonadres.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    lblWoonadres.Visibility = System.Windows.Visibility.Collapsed;
                }


                if (persoon.geboortedatum != null)
                {
                    txtGebdatum.Content = ((DateTime)persoon.geboortedatum).ToString("dd-MM-yyyy");
                    lblGebdatum.Visibility = System.Windows.Visibility.Visible;
                    txtGebdatum.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    lblGebdatum.Visibility = System.Windows.Visibility.Collapsed;
                    txtGebdatum.Visibility = System.Windows.Visibility.Collapsed;
                }


                if (string.IsNullOrWhiteSpace(persoon.telefoon_nr_1))
                {
                    lblTelefoonPrive1.Visibility = System.Windows.Visibility.Collapsed;
                    txtTelefoonPrive1.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    lblTelefoonPrive1.Visibility = System.Windows.Visibility.Visible;
                    txtTelefoonPrive1.Visibility = System.Windows.Visibility.Visible;
                }

                if (string.IsNullOrWhiteSpace(persoon.telefoon_nr_2))
                {
                    lblTelefoonPrive2.Visibility = System.Windows.Visibility.Collapsed;
                    txtTelefoonPrive2.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    lblTelefoonPrive2.Visibility = System.Windows.Visibility.Visible;
                    txtTelefoonPrive2.Visibility = System.Windows.Visibility.Visible;
                }

                if (string.IsNullOrWhiteSpace(persoon.telefoon_nr_3))
                {
                    lblTelefoonPrive3.Visibility = System.Windows.Visibility.Collapsed;
                    txtTelefoonPrive3.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    lblTelefoonPrive3.Visibility = System.Windows.Visibility.Visible;
                    txtTelefoonPrive3.Visibility = System.Windows.Visibility.Visible;
                }

                txtTelefoonPrive1.Content = persoon.telefoon_nr_1;
                txtTelefoonPrive2.Content = persoon.telefoon_nr_2;
                txtTelefoonPrive3.Content = persoon.telefoon_nr_3;

                if (string.IsNullOrWhiteSpace(persoon.zaemail))
                {
                    lblEmail.Visibility = System.Windows.Visibility.Collapsed;
                    txtEmail.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    lblEmail.Visibility = System.Windows.Visibility.Visible;
                    txtEmail.Visibility = System.Windows.Visibility.Visible;
                }

                TextBlock email = new TextBlock();
                email.Text = persoon.zaemail;
                txtEmail.Content = email;
                

                lblVakmanId.Content = persoon.persoon_nr == null ? "" : persoon.persoon_nr.ToString();

                MDRbedrijf bedrijfzdb = dbrepOriginal.datacontext.MDRbedrijfs.Where(b => b.bedrijf_nr == persoon.bedrijf_nr).FirstOrDefault();

                if (bedrijfzdb != null)
                {
                    lblWerkgever.Content = bedrijfzdb.naam;
                }


                if (string.IsNullOrWhiteSpace(vm.Bsn))
                {
                    lblBsn.Visibility = System.Windows.Visibility.Collapsed;
                    txtBsn.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    lblBsn.Visibility = System.Windows.Visibility.Visible;
                    txtBsn.Visibility = System.Windows.Visibility.Visible;
                }
                txtBsn.Content = vm.Bsn;


                lblVakmanNaam.Content = ToonNaam(persoon);

                txtMa.Content = vm.Ma.ToString().ToTime();
                txtDi.Content = vm.Di.ToString().ToTime();
                txtWo.Content = vm.Wo.ToString().ToTime();
                txtDo.Content = vm.Do.ToString().ToTime();
                txtVr.Content = vm.Vr.ToString().ToTime();
                txtZa.Content = vm.Za.ToString().ToTime();
                txtZo.Content = vm.Zo.ToString().ToTime();

                lblIsChauffeur.Content = vm.IsChauffeur ? "Ja" : "Nee";
                lblIsBijrijder.Content = vm.IsBijrijder ? "Ja" : "Nee";

                if (vm.IsChauffeur)
                {
                    wpChauffeur.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    wpChauffeur.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (vm.IsBijrijder)
                {
                    wpBijrijder.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    wpBijrijder.Visibility = System.Windows.Visibility.Collapsed;
                }


                txtChMa.Content = vm.IsChauffeurMa ? "Ja" : "Nee";
                txtChDi.Content = vm.IsChauffeurDi ? "Ja" : "Nee";
                txtChWo.Content = vm.IsChauffeurWo ? "Ja" : "Nee";
                txtChDo.Content = vm.IsChauffeurDo ? "Ja" : "Nee";
                txtChVr.Content = vm.IsChauffeurVr ? "Ja" : "Nee";
                txtChZa.Content = vm.IsChauffeurZa ? "Ja" : "Nee";
                txtChZo.Content = vm.IsChauffeurZo ? "Ja" : "Nee";


                txtBrMa.Content = vm.IsBijrijderMa ? "Ja" : "Nee";
                txtBrDi.Content = vm.IsBijrijderDi ? "Ja" : "Nee";
                txtBrWo.Content = vm.IsBijrijderWo ? "Ja" : "Nee";
                txtBrDo.Content = vm.IsBijrijderDo ? "Ja" : "Nee";
                txtBrVr.Content = vm.IsBijrijderVr ? "Ja" : "Nee";
                txtBrZa.Content = vm.IsBijrijderZa ? "Ja" : "Nee";
                txtBrZo.Content = vm.IsBijrijderZo ? "Ja" : "Nee";



                //ddlDefaultBeginuur.Content = ((int)vm.DefaultBeginuur).ToString("D2");
                //ddlDefaultBeginminuut.Content = ((int)vm.DefaultBeginminuut).ToString("D2");

                lblVakmanWerkweek.Content = "Uren (" + vm.Werkweek.ToString().ToTime() + "):";

                // create the page and load all values
                VakmanDagView vdv = new VakmanDagView();
                vdv.LoadVakmanDagView(true, vm.VakmanId, dtSelectedDay);
                vdv.LoadWeekInfo();

                tabPlanning.Content = vdv;


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
                tabControl.Width = Window.GetWindow(this).ActualWidth - 20;

            }

        }

        private void tabPlanning_GotFocus(object sender, RoutedEventArgs e)
        {
            PageContainer pc = (PageContainer)Tools.FindVisualParent<Window>(this);
            if (Rechten.IsProjectleider)
            {
                pc.SetLabelSubtitlePlanning(true);
            }
            else
            {
                pc.SetLabelSubtitlePlanning(false);
            }
        }

        private void tabDetails_GotFocus(object sender, RoutedEventArgs e)
        {
            PageContainer pc = (PageContainer)Tools.FindVisualParent<Window>(this);
            if (Rechten.IsProjectleider)
            {
                pc.SetLabelSubtitleGegevens(true);
            }
            else
            {
                pc.SetLabelSubtitleGegevens(false);
            }
        }

    }
}

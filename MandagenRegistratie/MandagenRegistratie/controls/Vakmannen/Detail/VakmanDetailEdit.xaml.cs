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

        public VakmanDetailEdit(string pageSubTitle)
        {
            InitializeComponent();

            #region Pagina specifieke informatie
            PageTitle = "Wijzigen";
            PageSubtitle = pageSubTitle + " wijzigen";

            if (Rechten.IsProjectleider)
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Hidden;
            }

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
            try
            {
                dbRepository dbrep = new dbRepository();
                dbOriginalRepository dbrepOriginal = new dbOriginalRepository();

                Vakman vakman = dbrep.GetVakman(ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId));
                MDRpersoon persoon = dbrepOriginal.GetContact(vakman.ContactIdOrigineel);

                // alleen saven als het ingevuld is
                if (Tools.IsValidBsn(txtBsn.Text))
                {
                    if (txtBsn.Text.Length == 8)
                    {
                        vakman.Bsn = "0" + txtBsn.Text;
                    }
                    else
                    {
                        vakman.Bsn = txtBsn.Text;
                    }
                }
                else
                {
                    vakman.Bsn = txtBsn.Text;
                }

                persoon.zaemail = txtEmail.Text;

                try
                {
                    int iDag = -1;
                    int iMaand = -1;
                    int iJaar = -1;


                    if (!string.IsNullOrWhiteSpace(txtDag.Text))
                    {
                        try
                        {
                            iDag = Convert.ToInt32(txtDag.Text);
                        }
                        catch { }
                    }

                    if (!string.IsNullOrWhiteSpace(txtMaand.Text))
                    {
                        try
                        {
                            iMaand = Convert.ToInt32(txtMaand.Text);
                        }
                        catch { }
                    }

                    if (!string.IsNullOrWhiteSpace(txtJaar.Text))
                    {
                        try
                        {
                            iJaar = Convert.ToInt32(txtJaar.Text);
                        }
                        catch { }
                    }

                    if (iJaar > 1900 && iMaand > 0 && iDag > 0)
                    {
                        try
                        {
                            DateTime gebdat = new DateTime(iJaar, iMaand, iDag);
                            persoon.geboortedatum = gebdat;
                        }
                        catch { }

                    }


                
                }
                catch{}

                dbrepOriginal.datacontext.SubmitChanges();


                vakman.Postcode = txtPostcode.Text;
                vakman.Huisnummer = txtHuisnummer.Text;
                vakman.Adres = txtAdres.Text;
                vakman.Plaats = txtPlaats.Text;
                vakman.Land = txtLand.Text;

                //vakman.Ophaalpostcode = listTextboxes[3].Text;
                //vakman.Ophaalhuisnummer = listTextboxes[4].Text;
                //vakman.Ophaaladres = listTextboxes[5].Text;
                vakman.Ma = Convert.ToDecimal(cbMaUur.SelectedValue.ToString().HourToTimeDouble() + cbMaMin.SelectedValue.ToString().MinuteToTimeDouble());
                vakman.Di = Convert.ToDecimal(cbDiUur.SelectedValue.ToString().HourToTimeDouble() + cbDiMin.SelectedValue.ToString().MinuteToTimeDouble());
                vakman.Wo = Convert.ToDecimal(cbWoUur.SelectedValue.ToString().HourToTimeDouble() + cbWoMin.SelectedValue.ToString().MinuteToTimeDouble());
                vakman.Do = Convert.ToDecimal(cbDoUur.SelectedValue.ToString().HourToTimeDouble() + cbDoMin.SelectedValue.ToString().MinuteToTimeDouble());
                vakman.Vr = Convert.ToDecimal(cbVrUur.SelectedValue.ToString().HourToTimeDouble() + cbVrMin.SelectedValue.ToString().MinuteToTimeDouble());
                vakman.Za = Convert.ToDecimal(cbZaUur.SelectedValue.ToString().HourToTimeDouble() + cbZaMin.SelectedValue.ToString().MinuteToTimeDouble());
                vakman.Zo = Convert.ToDecimal(cbZoUur.SelectedValue.ToString().HourToTimeDouble() + cbZoMin.SelectedValue.ToString().MinuteToTimeDouble());

                vakman.Werkweek = vakman.Ma + vakman.Di + vakman.Wo + vakman.Do + vakman.Vr + vakman.Za + vakman.Zo;


                vakman.IsChauffeurMa = cbChauffeurMa.IsChecked == true && cbChauffeur.IsChecked == true;
                vakman.IsChauffeurDi = cbChauffeurDi.IsChecked == true && cbChauffeur.IsChecked == true;
                vakman.IsChauffeurWo = cbChauffeurWo.IsChecked == true && cbChauffeur.IsChecked == true;
                vakman.IsChauffeurDo = cbChauffeurDo.IsChecked == true && cbChauffeur.IsChecked == true;
                vakman.IsChauffeurVr = cbChauffeurVr.IsChecked == true && cbChauffeur.IsChecked == true;
                vakman.IsChauffeurZa = cbChauffeurZa.IsChecked == true && cbChauffeur.IsChecked == true;
                vakman.IsChauffeurZo = cbChauffeurZo.IsChecked == true && cbChauffeur.IsChecked == true;

                vakman.IsChauffeur = (bool)cbChauffeur.IsChecked;

                vakman.IsBijrijderMa = cbBijrijderMa.IsChecked == true && cbBijrijdersregistratie.IsChecked == true;
                vakman.IsBijrijderDi = cbBijrijderDi.IsChecked == true && cbBijrijdersregistratie.IsChecked == true;
                vakman.IsBijrijderWo = cbBijrijderWo.IsChecked == true && cbBijrijdersregistratie.IsChecked == true;
                vakman.IsBijrijderDo = cbBijrijderDo.IsChecked == true && cbBijrijdersregistratie.IsChecked == true;
                vakman.IsBijrijderVr = cbBijrijderVr.IsChecked == true && cbBijrijdersregistratie.IsChecked == true;
                vakman.IsBijrijderZa = cbBijrijderZa.IsChecked == true && cbBijrijdersregistratie.IsChecked == true;
                vakman.IsBijrijderZo = cbBijrijderZo.IsChecked == true && cbBijrijdersregistratie.IsChecked == true;

                vakman.IsBijrijder = (bool)cbBijrijdersregistratie.IsChecked;


                vakman.ZZP = cbWerkrelatie.SelectedIndex == 1;

                //string strUur = ddlDefaultBeginuur.Text;
                //vakman.DefaultBeginuur = strUur.ToInt();

                //string strMinuut = ddlDefaultBeginminuut.Text;
                //vakman.DefaultBeginminuut = strMinuut.ToInt();

                dbrep.datacontext.SubmitChanges();

                //ApplicationState.SetValue(ApplicationVariables.objVakman, vakman);
                ApplicationState.SetValue(ApplicationVariables.intVakmanId, vakman.VakmanId);

                // na het saven terug in history
                PageGoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Init()
        {

            string[] hours = new string[] { "00u", "01u", "02u", "03u", "04u", "05u", "06u", "07u", "08u", "09u", "10u", "11u", "12u", "13u", "14u", "15u", "16u", "17u", "18u", "19u", "20u" };
            string[] minutes = new string[] { "00m", "15m", "30m", "45m" };

            cbMaUur.ItemsSource = hours;
            cbDiUur.ItemsSource = hours;
            cbWoUur.ItemsSource = hours;
            cbDoUur.ItemsSource = hours;
            cbVrUur.ItemsSource = hours;
            cbZaUur.ItemsSource = hours;
            cbZoUur.ItemsSource = hours;

            cbMaMin.ItemsSource = minutes;
            cbDiMin.ItemsSource = minutes;
            cbWoMin.ItemsSource = minutes;
            cbDoMin.ItemsSource = minutes;
            cbVrMin.ItemsSource = minutes;
            cbZaMin.ItemsSource = minutes;
            cbZoMin.ItemsSource = minutes;

            cbMaUur.SelectionChanged += new SelectionChangedEventHandler(WerkweekUrenBerekenen);
            cbDiUur.SelectionChanged += new SelectionChangedEventHandler(WerkweekUrenBerekenen);
            cbWoUur.SelectionChanged += new SelectionChangedEventHandler(WerkweekUrenBerekenen);
            cbDoUur.SelectionChanged += new SelectionChangedEventHandler(WerkweekUrenBerekenen);
            cbVrUur.SelectionChanged += new SelectionChangedEventHandler(WerkweekUrenBerekenen);
            cbZaUur.SelectionChanged += new SelectionChangedEventHandler(WerkweekUrenBerekenen);
            cbZoUur.SelectionChanged += new SelectionChangedEventHandler(WerkweekUrenBerekenen);

            cbMaMin.SelectionChanged += new SelectionChangedEventHandler(WerkweekUrenBerekenen);
            cbDiMin.SelectionChanged += new SelectionChangedEventHandler(WerkweekUrenBerekenen);
            cbWoMin.SelectionChanged += new SelectionChangedEventHandler(WerkweekUrenBerekenen);
            cbDoMin.SelectionChanged += new SelectionChangedEventHandler(WerkweekUrenBerekenen);
            cbVrMin.SelectionChanged += new SelectionChangedEventHandler(WerkweekUrenBerekenen);
            cbZaMin.SelectionChanged += new SelectionChangedEventHandler(WerkweekUrenBerekenen);
            cbZoMin.SelectionChanged += new SelectionChangedEventHandler(WerkweekUrenBerekenen);



        }


        public string ToonNaam(MDRpersoon objPersoon)
        {
            return objPersoon.voornaam + " " + (string.IsNullOrWhiteSpace(objPersoon.tussenvoegsel) ? "" : objPersoon.tussenvoegsel + " ") + objPersoon.achternaam;
        }


        /// <summary>
        /// 
        /// </summary>
        public void Load()
        {

            Mouse.OverrideCursor = Cursors.Wait;


            dbRepository dbrep = new dbRepository();
            Vakman vm = dbrep.GetVakman(ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId));

            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
            MDRpersoon persoon = dbrepOriginal.GetContact(vm.ContactIdOrigineel);
            
            txtPostcode.Text = vm.Postcode;
            txtAdres.Text = vm.Adres;
            txtPlaats.Text = vm.Plaats;
            txtLand.Text = vm.Land;
            txtHuisnummer.Text = vm.Huisnummer;

            if (persoon != null)
            {
                if (vm.ZZP == true)
                {
                    lbWerkrelatie.Content = "Bedrijfsnaam:";
                    cbWerkrelatie.SelectedIndex = 1;
                }
                else
                {
                    lbWerkrelatie.Content = "Werkgever:";
                    cbWerkrelatie.SelectedIndex = 0;
                }


                if (persoon.bedrijf_nr != null)
                {
                    MDRbedrijf b = dbrepOriginal.GetBedrijf((int)persoon.bedrijf_nr);
                    if (b != null)
                    {
                        lblWerkgever.Content = b.naam;
                    }
                }

                lblVakmanId.Content = persoon.persoon_nr == null ? "" : persoon.persoon_nr.ToString();

                lblVakmanNaam.Content = ToonNaam(persoon);
                txtBsn.Text = vm.Bsn;
                //txtPostcode.Text = vm.Postcode;
                //txtHuisnummer.Text = vm.Huisnummer;
                //txtAdres.Text = vm.Adres;

                AdresLookup = vm.Adres.Substring(0, vm.Adres.Length - vm.Huisnummer.Length).Trim();

                cbMaUur.SelectedValue = vm.Ma.ToString().ToHour();
                cbDiUur.SelectedValue = vm.Di.ToString().ToHour();
                cbWoUur.SelectedValue = vm.Wo.ToString().ToHour();
                cbDoUur.SelectedValue = vm.Do.ToString().ToHour();
                cbVrUur.SelectedValue = vm.Vr.ToString().ToHour();
                cbZaUur.SelectedValue = vm.Za.ToString().ToHour();
                cbZoUur.SelectedValue = vm.Zo.ToString().ToHour();

                cbMaMin.SelectedValue = vm.Ma.ToString().ToMinute();
                cbDiMin.SelectedValue = vm.Di.ToString().ToMinute();
                cbWoMin.SelectedValue = vm.Wo.ToString().ToMinute();
                cbDoMin.SelectedValue = vm.Do.ToString().ToMinute();
                cbVrMin.SelectedValue = vm.Vr.ToString().ToMinute();
                cbZaMin.SelectedValue = vm.Za.ToString().ToMinute();
                cbZoMin.SelectedValue = vm.Zo.ToString().ToMinute();

                //ddlDefaultBeginuur.Text = ((int)vm.DefaultBeginuur).ToString("D2");
                //ddlDefaultBeginminuut.Text = ((int)vm.DefaultBeginminuut).ToString("D2");

                WerkweekUrenBerekenen(null, null);


                if (persoon.geboortedatum != null)
                {
                    txtDag.Text = ((DateTime)persoon.geboortedatum).ToString("dd");
                    txtMaand.Text = ((DateTime)persoon.geboortedatum).ToString("MM");
                    txtJaar.Text = ((DateTime)persoon.geboortedatum).ToString("yyyy");
                    dpGebdatum.SelectedDate = persoon.geboortedatum;
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

                txtEmail.Text = persoon.zaemail;


                cbChauffeur.IsChecked = vm.IsChauffeur;
                cbBijrijdersregistratie.IsChecked = vm.IsBijrijder;

                cbChauffeurMa.IsChecked = vm.IsChauffeurMa;
                cbChauffeurDi.IsChecked = vm.IsChauffeurDi;
                cbChauffeurWo.IsChecked = vm.IsChauffeurWo;
                cbChauffeurDo.IsChecked = vm.IsChauffeurDo;
                cbChauffeurVr.IsChecked = vm.IsChauffeurVr;
                cbChauffeurZa.IsChecked = vm.IsChauffeurZa;
                cbChauffeurZo.IsChecked = vm.IsChauffeurZo;

                cbBijrijderMa.IsChecked = vm.IsBijrijderMa;
                cbBijrijderDi.IsChecked = vm.IsBijrijderDi;
                cbBijrijderWo.IsChecked = vm.IsBijrijderWo;
                cbBijrijderDo.IsChecked = vm.IsBijrijderDo;
                cbBijrijderVr.IsChecked = vm.IsBijrijderVr;
                cbBijrijderZa.IsChecked = vm.IsBijrijderZa;
                cbBijrijderZo.IsChecked = vm.IsBijrijderZo;

                if (!vm.IsChauffeur)
                {
                    wpChauffeur.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (!vm.IsBijrijder)
                {
                    wpBijrijder.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

            Mouse.OverrideCursor = null;

        }


        public void AutoVulAdres(object sender, KeyEventArgs e)
        {
            txtPostcode.Text = txtPostcode.Text.ToUpper();
            txtPostcode.CaretIndex = txtPostcode.Text.Length;

            string _postcode = ((TextBox)sender).Text;
            Tools tool = new Tools();

            var regx = new Regex(@"(\d{4}\s*)(\D{2})");
            var match = regx.Match(_postcode);

            if (match.Success)
            {
                APIResponse adresComplete = tool.ZoekOpPostcodeComplete(_postcode);
                if (adresComplete != null)
                {
                    string straat = adresComplete.results[0].street;

                    txtAdres.Text = straat;
                    txtPlaats.Text = adresComplete.results[0].city;
                    txtLand.Text = "Nederland";

                    AdresLookup = straat;
                }

                ChangeAdres = true;
            }
        }

        public void AdresAanvullen(object sender, KeyEventArgs e)
        {
            string huisnummer = ((TextBox)sender).Text;

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
        public void AddLabelToPanel(string label)
        {

            Label lbl = new Label();
            lbl.Content = label;
            lbl.Width = 600;
            lbl.FontWeight = FontWeights.Bold;

            wpDetailsEdit.Children.Add(lbl);

        }






        public void WerkweekUrenBerekenen(object sender, SelectionChangedEventArgs e)
        {

            double totaal = cbMaUur.SelectedValue.ToString().HourToTimeDouble()
                + cbDiUur.SelectedValue.ToString().HourToTimeDouble()
                + cbWoUur.SelectedValue.ToString().HourToTimeDouble()
                + cbDoUur.SelectedValue.ToString().HourToTimeDouble()
                + cbVrUur.SelectedValue.ToString().HourToTimeDouble()
                + cbZaUur.SelectedValue.ToString().HourToTimeDouble()
                + cbZoUur.SelectedValue.ToString().HourToTimeDouble()

                + cbMaMin.SelectedValue.ToString().MinuteToTimeDouble()
                + cbDiMin.SelectedValue.ToString().MinuteToTimeDouble()
                + cbWoMin.SelectedValue.ToString().MinuteToTimeDouble()
                + cbDoMin.SelectedValue.ToString().MinuteToTimeDouble()
                + cbVrMin.SelectedValue.ToString().MinuteToTimeDouble()
                + cbZaMin.SelectedValue.ToString().MinuteToTimeDouble()
                + cbZoMin.SelectedValue.ToString().MinuteToTimeDouble();

            //lblVakmanWerkweek.Content = totaal.ToString().ToTime();

            lblUren.Content = "Uren (" + totaal.ToString().ToTime().Split(':')[0].ToString() + "):";
            lblMinuten.Content = "Minuten (" + totaal.ToString().ToTime().Split(':')[1].ToString() + "):";

            //if (totaal == 0)
            //{
            //    cbChauffeur.IsChecked = false;
            //    cbChauffeur.IsEnabled = false;
            //}
            //else
            //{
            //    cbChauffeur.IsEnabled = true;
            //}

            if (cbMaUur.SelectedIndex == 0 && cbMaMin.SelectedIndex == 0)
            {
                cbChauffeurMa.IsChecked = false;
                cbChauffeurMa.IsEnabled = false;
                cbBijrijderMa.IsChecked = false;
                cbBijrijderMa.IsEnabled = false;

                cbDefaulturenMa.IsChecked = false;
            }
            else
            {
                cbDefaulturenMa.IsChecked = true;

                cbChauffeurMa.IsEnabled = true;
                cbBijrijderMa.IsEnabled = true;
            }

            if (cbDiUur.SelectedIndex == 0 && cbDiMin.SelectedIndex == 0)
            {
                cbChauffeurDi.IsChecked = false;
                cbChauffeurDi.IsEnabled = false;
                cbBijrijderDi.IsChecked = false;
                cbBijrijderDi.IsEnabled = false;

                cbDefaulturenDi.IsChecked = false;
            }
            else
            {
                cbDefaulturenDi.IsChecked = true;

                cbChauffeurDi.IsEnabled = true;
                cbBijrijderDi.IsEnabled = true;
            }

            if (cbWoUur.SelectedIndex == 0 && cbWoMin.SelectedIndex == 0)
            {
                cbChauffeurWo.IsChecked = false;
                cbChauffeurWo.IsEnabled = false;
                cbBijrijderWo.IsChecked = false;
                cbBijrijderWo.IsEnabled = false;

                cbDefaulturenWo.IsChecked = false;
            }
            else
            {
                cbDefaulturenWo.IsChecked = true;

                cbChauffeurWo.IsEnabled = true;
                cbBijrijderWo.IsEnabled = true;
            }

            if (cbDoUur.SelectedIndex == 0 && cbDoMin.SelectedIndex == 0)
            {
                cbChauffeurDo.IsChecked = false;
                cbChauffeurDo.IsEnabled = false;
                cbBijrijderDo.IsChecked = false;
                cbBijrijderDo.IsEnabled = false;

                cbDefaulturenDo.IsChecked = false;
            }
            else
            {
                cbDefaulturenDo.IsChecked = true;

                cbChauffeurDo.IsEnabled = true;
                cbBijrijderDo.IsEnabled = true;
            }

            if (cbVrUur.SelectedIndex == 0 && cbVrMin.SelectedIndex == 0)
            {
                cbChauffeurVr.IsChecked = false;
                cbChauffeurVr.IsEnabled = false;
                cbBijrijderVr.IsChecked = false;
                cbBijrijderVr.IsEnabled = false;

                cbDefaulturenVr.IsChecked = false;
            }
            else
            {
                cbDefaulturenVr.IsChecked = true;

                cbChauffeurVr.IsEnabled = true;
                cbBijrijderVr.IsEnabled = true;
            }

            if (cbZaUur.SelectedIndex == 0 && cbZaMin.SelectedIndex == 0)
            {
                cbChauffeurZa.IsChecked = false;
                cbChauffeurZa.IsEnabled = false;
                cbBijrijderZa.IsChecked = false;
                cbBijrijderZa.IsEnabled = false;

                cbDefaulturenZa.IsChecked = false;
            }
            else
            {
                cbDefaulturenZa.IsChecked = true;

                cbChauffeurZa.IsEnabled = true;
                cbBijrijderZa.IsEnabled = true;
            }

            if (cbZoUur.SelectedIndex == 0 && cbZoMin.SelectedIndex == 0)
            {
                cbChauffeurZo.IsChecked = false;
                cbChauffeurZo.IsEnabled = false;
                cbBijrijderZo.IsChecked = false;
                cbBijrijderZo.IsEnabled = false;

                cbDefaulturenZo.IsChecked = false;
            }
            else
            {
                cbDefaulturenZo.IsChecked = true;

                cbChauffeurZo.IsEnabled = true;
                cbBijrijderZo.IsEnabled = true;
            }


        }

        private void txtBsn_KeyUp(object sender, KeyEventArgs e)
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
            }

            if (Tools.IsValidBsn(txtBsn.Text.Replace("_","0")))
            {
                lblBsnStatus.Content = "";

                PageContainer pc = (PageContainer)Tools.FindVisualParent<Window>(this);
                pc.SetVisibilityGereedButton(System.Windows.Visibility.Visible);

            }
            else
            {
                lblBsnStatus.Content = "Onjuist Bsn nummer";

                if (txtBsn.Text == "")
                {
                    PageContainer pc = (PageContainer)Tools.FindVisualParent<Window>(this);
                    pc.SetVisibilityGereedButton(System.Windows.Visibility.Visible);
                }
                else
                {
                    PageContainer pc = (PageContainer)Tools.FindVisualParent<Window>(this);
                    pc.SetVisibilityGereedButton(System.Windows.Visibility.Hidden);
                }
            }
        }

        private void cbWerkrelatie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (cbWerkrelatie.SelectedIndex == 0)
            {
                lbWerkrelatie.Content = "Werkgever:";
            }
            else
            {
                lbWerkrelatie.Content = "Bedrijfsnaam:";
            }

        }

        private void cbChauffeur_Checked(object sender, RoutedEventArgs e)
        {
            wpChauffeur.Visibility = System.Windows.Visibility.Visible;
        }

        private void cbChauffeur_Unchecked(object sender, RoutedEventArgs e)
        {
            wpChauffeur.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void dpGebdatum_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == dpGebdatum)
            {
                if (dpGebdatum.SelectedDate != null)
                {
                    txtDag.Text = ((DateTime)dpGebdatum.SelectedDate).ToString("dd");
                    txtMaand.Text = ((DateTime)dpGebdatum.SelectedDate).ToString("MM");
                    txtJaar.Text = ((DateTime)dpGebdatum.SelectedDate).ToString("yyyy");
                }
            }
        }

        private void dpGebdatum_CalendarOpened(object sender, RoutedEventArgs e)
        {
            int iDag = 1;
            int iMaand = 1;
            int iJaar = DateTime.Now.Year;


            if (!string.IsNullOrWhiteSpace(txtDag.Text))
            {
                try
                {
                    iDag = Convert.ToInt32(txtDag.Text);
                }
                catch { }
            }

            if (!string.IsNullOrWhiteSpace(txtMaand.Text))
            {
                try
                {
                    iMaand = Convert.ToInt32(txtMaand.Text);
                }
                catch { }
            }

            if (!string.IsNullOrWhiteSpace(txtJaar.Text))
            {
                try
                {
                    iJaar = Convert.ToInt32(txtJaar.Text);
                }
                catch { }
            }

            dpGebdatum.SelectedDate = new DateTime(iJaar, iMaand, iDag);

        }

        private void cbBijrijdersregistratie_Checked_1(object sender, RoutedEventArgs e)
        {
            wpBijrijder.Visibility = System.Windows.Visibility.Visible;
        }

        private void cbBijrijdersregistratie_Unchecked_1(object sender, RoutedEventArgs e)
        {
            wpBijrijder.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void cbDefaulturen_Checked_1(object sender, RoutedEventArgs e)
        {
            if (cbDefaulturenMa.IsChecked == true)
            {
                cbMaUur.IsEnabled = true;
                cbMaMin.IsEnabled = true;

                cbMaUur.SelectedValue = "08u";
            }
            else
            {
                cbMaUur.IsEnabled = false;
                cbMaMin.IsEnabled = false;

                cbMaUur.SelectedIndex = 0;
                cbMaMin.SelectedIndex = 0;
            }

            if (cbDefaulturenDi.IsChecked == true)
            {
                cbDiUur.IsEnabled = true;
                cbDiMin.IsEnabled = true;

                cbDiUur.SelectedValue = "08u";
            }
            else
            {
                cbDiUur.IsEnabled = false;
                cbDiMin.IsEnabled = false;

                cbDiUur.SelectedIndex = 0;
                cbDiMin.SelectedIndex = 0;
            }

            if (cbDefaulturenWo.IsChecked == true)
            {
                cbWoUur.IsEnabled = true;
                cbWoMin.IsEnabled = true;

                cbWoUur.SelectedValue = "08u";
            }
            else
            {
                cbWoUur.IsEnabled = false;
                cbWoMin.IsEnabled = false;

                cbWoUur.SelectedIndex = 0;
                cbWoMin.SelectedIndex = 0;
            }

            if (cbDefaulturenDo.IsChecked == true)
            {
                cbDoUur.IsEnabled = true;
                cbDoMin.IsEnabled = true;

                cbDoUur.SelectedValue = "08u";
            }
            else
            {
                cbDoUur.IsEnabled = false;
                cbDoMin.IsEnabled = false;

                cbDoUur.SelectedIndex = 0;
                cbDoMin.SelectedIndex = 0;
            }

            if (cbDefaulturenVr.IsChecked == true)
            {
                cbVrUur.IsEnabled = true;
                cbVrMin.IsEnabled = true;

                cbVrUur.SelectedValue = "08u";
            }
            else
            {
                cbVrUur.IsEnabled = false;
                cbVrMin.IsEnabled = false;

                cbVrUur.SelectedIndex = 0;
                cbVrMin.SelectedIndex = 0;
            }

            if (cbDefaulturenZa.IsChecked == true)
            {
                cbZaUur.IsEnabled = true;
                cbZaMin.IsEnabled = true;

                cbZaUur.SelectedValue = "08u";
            }
            else
            {
                cbZaUur.IsEnabled = false;
                cbZaMin.IsEnabled = false;

                cbZaUur.SelectedIndex = 0;
                cbZaMin.SelectedIndex = 0;
            }

            if (cbDefaulturenZo.IsChecked == true)
            {
                cbZoUur.IsEnabled = true;
                cbZoMin.IsEnabled = true;

                cbZoUur.SelectedValue = "08u";
            }
            else
            {
                cbZoUur.IsEnabled = false;
                cbZoMin.IsEnabled = false;

                cbZoUur.SelectedIndex = 0;
                cbZoMin.SelectedIndex = 0;
            }

            WerkweekUrenBerekenen(null, null);


        }

        private void cbChauffeurMa_Checked_1(object sender, RoutedEventArgs e)
        {
            if (cbChauffeurMa.IsChecked == true)
            {
                cbBijrijderMa.IsChecked = false;
            }

            if (cbChauffeurDi.IsChecked == true)
            {
                cbBijrijderDi.IsChecked = false;
            }

            if (cbChauffeurWo.IsChecked == true)
            {
                cbBijrijderWo.IsChecked = false;
            }

            if (cbChauffeurDo.IsChecked == true)
            {
                cbBijrijderDo.IsChecked = false;
            }

            if (cbChauffeurVr.IsChecked == true)
            {
                cbBijrijderVr.IsChecked = false;
            }

            if (cbChauffeurZa.IsChecked == true)
            {
                cbBijrijderZa.IsChecked = false;
            }

            if (cbChauffeurZo.IsChecked == true)
            {
                cbBijrijderZo.IsChecked = false;
            }

        }

        private void cbBijrijderMa_Checked_1(object sender, RoutedEventArgs e)
        {
            if (cbBijrijderMa.IsChecked == true)
            {
                cbChauffeurMa.IsChecked = false;
            }

            if (cbBijrijderDi.IsChecked == true)
            {
                cbChauffeurDi.IsChecked = false;
            }

            if (cbBijrijderWo.IsChecked == true)
            {
                cbChauffeurWo.IsChecked = false;
            }

            if (cbBijrijderDo.IsChecked == true)
            {
                cbChauffeurDo.IsChecked = false;
            }

            if (cbBijrijderVr.IsChecked == true)
            {
                cbChauffeurVr.IsChecked = false;
            }

            if (cbBijrijderZa.IsChecked == true)
            {
                cbChauffeurZa.IsChecked = false;
            }

            if (cbBijrijderZo.IsChecked == true)
            {
                cbChauffeurZo.IsChecked = false;
            }

        }



    }
}

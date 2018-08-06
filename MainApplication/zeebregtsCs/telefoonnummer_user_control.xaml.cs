using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Threading;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Web;
using System.Windows.Interop;
using System.ComponentModel;

namespace zeebregtsCs.usercontrols
{
    /// <summary>
    /// Interaction logic for telefoonnummer_user_control.xaml
    /// </summary>
    public partial class telefoonnummer_user_control : UserControl
    {
        private string _nummer_ingeladen;
        private string _nummer_working;
        private string _nummer_tonen;
        AdresDataSetTableAdapters.landcodesTableAdapter lta = new AdresDataSetTableAdapters.landcodesTableAdapter();
        AdresDataSet.landcodesDataTable lcDt = new AdresDataSet.landcodesDataTable();
        private Stack<string> _Net_Nummers;
        bool klaar = false;
        bool netnr_bekend = false;
        bool leeg = false;
        bool _wijzigstand = false;
        private bool vastgezet = false;
        private base_form parent;
        BackgroundWorker bw = new BackgroundWorker();
        public telefoonnummer_user_control()
        {
           lta.Connection.ConnectionString = Global.ConnectionString_fileserver;

            InitializeComponent();
            lta.Fill(lcDt);
            CBB_overige_landen.DataContext = lcDt;
            CBB_overige_landen.DisplayMemberPath = lcDt.landColumn.ToString();
            CBB_overige_landen.SelectedValuePath = lcDt.codeColumn.ToString();
            CBB_land.IsEnabled = false;
            //CBB_type.IsEnabled = false;
            TB_prefix.IsEnabled = false;
            TB_abbonee_nr.IsEnabled = false;
            Wijzigstand(false);
            this.Visibility = Visibility.Collapsed;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                String log_line = "Exception occurred [api] @ " + DateTime.Now.ToString() + " error: " + e.Result;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
            }
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Stack<string> areacodes = new Stack<string>();
                String auth_key = Global.ZES_PP_AUTH_KEY;
                String nl_sixpp = e.Argument.ToString();// _postcode;
                // Optional parameter
                String streetnumber = "";
                // Create the GET request
                string uri = "http://api.pro6pp.nl/v1/autocomplete";
                uri += String.Format("?auth_key={0}&nl_sixpp={1}&per_page=25&streetnumber={2}", auth_key, HttpUtility.UrlEncode(nl_sixpp), streetnumber);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                request.ContentType = "text/xml; encoding='utf-8'";

                // Get the response
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string result = new StreamReader(response.GetResponseStream()).ReadToEnd();

                // Parse the response
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                APIResponse r = serializer.Deserialize<APIResponse>(result);
                if (r.status == "ok")
                {

                    foreach (APIResult rs in r.results)
                    {

                        areacodes.Push(rs.areacode);
                    }
                    netnr_bekend = true;
                }
                _Net_Nummers = areacodes;
            }
            catch (Exception we)
            {
                e.Result = we;
            }
        }
        public int type
        {
            get
            {
                if (TB_prefix.Text == "06")
                {
                    return 1;
                }
                else if (CBB_land.SelectedIndex == 0 && CBB_type.SelectedIndex == 1)
                {
                    return 0;
                }
                else
                {
                    return CBB_type.SelectedIndex;
                }
            }
            set { CBB_type.SelectedIndex = value; }
        }
        public void set_AreaCodes(Stack<string> Netnrs)
        {
            _Net_Nummers = Netnrs;
            if (_Net_Nummers.Count > 0)
            {
                netnr_bekend = true;
            }
        }
        public void set_Nummer(string numr,base_form frmparent)
        {
            parent = frmparent;
            if (numr != null && numr != String.Empty && numr.Length > 4)
            {
                this.Visibility = Visibility.Visible;
                _nummer_ingeladen = numr;
                string[] breakdown = Regex.Split(numr, @"\({1}|\){1}|\s{1}");
                //MessageBox.Show(breakdown[0] + ": en :" + breakdown[1] + ": en :" + breakdown[2]);
                bool overig_land = true;
                switch (breakdown[0])
                {
                    case "+31":
                        CBB_land.SelectedIndex = 0;
                        overig_land = false;
                        break;
                    case "+32":
                        CBB_land.SelectedIndex = 1;
                        overig_land = false;
                        break;
                    case "+49":
                        CBB_land.SelectedIndex = 2;
                        overig_land = false;
                        break;

                }
                if (overig_land)
                {
                    CBB_land.SelectedIndex = 3;
                    if (breakdown[0].Length > 1)
                    {

                        CBB_overige_landen.SelectedValue = breakdown[0].Substring(1);
                    }
                }

                int netnr;
                int.TryParse(breakdown[2], out netnr);
                if (CBB_type.SelectedIndex == -1)
                {
                    switch (CBB_land.SelectedIndex)
                    {
                        case 0:
                            if (netnr == 6)
                            {
                                CBB_type.SelectedIndex = 1;
                            }
                            else if (netnr == 88)
                            {
                                CBB_type.SelectedIndex = 5;
                            }
                            else if (netnr == 85 || netnr == 91)
                            {
                                CBB_type.SelectedIndex = 3;
                            }
                            else
                            {
                                CBB_type.SelectedIndex = 0;
                            }
                            break;
                        case 1:
                            if ((netnr >= 4681 && netnr <= 4683) || (netnr >= 470 && netnr <= 479) || (netnr >= 483 && netnr <= 489) || (netnr >= 491 && netnr <= 499) || (netnr == 461))
                            {
                                CBB_type.SelectedIndex = 1;
                            }
                            else
                            {
                                CBB_type.SelectedIndex = 0;
                            }
                            break;
                        case 2:
                            if ((netnr >= 170 && netnr <= 179) || (netnr >= 160 && netnr <= 163) || (netnr == 1505) || (netnr == 151))
                            {
                                CBB_type.SelectedIndex = 1;
                            }
                            else
                            {
                                CBB_type.SelectedIndex = 0;
                            }
                            break;
                        case 3:
                            CBB_type.SelectedIndex = 0;
                            break;
                    }
                }
                if (breakdown[2] != "(0)" || breakdown[2] != "0")
                {
                    TB_prefix.Text = "0" + breakdown[2];
                }
                if (breakdown.Length > 2)
                {
                    TB_abbonee_nr.Text = breakdown[3];
                }
                TB_abbonee_nr.IsEnabled = true;
                TB_prefix.IsEnabled = true;
                bool niet_nl = true;
                string landprefix = String.Empty;
                switch (CBB_land.SelectedIndex)
                {
                    case 0://nl

                        niet_nl = false;
                        break;
                    case 1://be
                        landprefix = "+32";
                        break;
                    case 2://de
                        landprefix = "+49";
                        break;
                    case 3://overig
                        if (CBB_overige_landen.SelectedValue != null)
                        {
                            landprefix = "+" + CBB_overige_landen.SelectedValue.ToString();
                        }
                        break;
                }
                _nummer_working += "(0)" + TB_prefix.Text.Substring(1) + " " + TB_abbonee_nr.Text;
                if (niet_nl)
                {
                    _nummer_tonen = landprefix + " " + TB_prefix.Text.Substring(1) + " " + split_abbo_nr(TB_abbonee_nr.Text);
                    lbl_nr.Content = _nummer_tonen;
                }
                else
                {
                    _nummer_tonen = TB_prefix.Text + " " + split_abbo_nr(TB_abbonee_nr.Text);
                    lbl_nr.Content = _nummer_tonen;
                }

            }
            else
            {
               // this.Visibility = Visibility.Collapsed;
                TB_prefix.Text = String.Empty;
                TB_abbonee_nr.Text = String.Empty;
                _nummer_tonen = String.Empty;
                _nummer_working = String.Empty;
                _nummer_ingeladen = String.Empty;
                lbl_nr.Content = String.Empty;
                CBB_overige_landen.SelectedIndex = -1;
                CBB_land.SelectedIndex = -1;
                if (!vastgezet)
                {
                    CBB_type.SelectedIndex = -1;
                }
                leeg = true;

            }
        }
        private string split_abbo_nr(string abbonr)
        {
            string retval = String.Empty;
            if (abbonr.Length == 6)
            {
                retval = abbonr.Substring(0, 3) + " " + abbonr.Substring(3, 3);
            }
            else if (abbonr.Length == 7)
            {
                retval = abbonr.Substring(0, 3) + " " + abbonr.Substring(3, 2) + " " + abbonr.Substring(5, 2);
            }
            else if (abbonr.Length == 8)
            {
                retval = abbonr.Substring(0, 3) + " " + abbonr.Substring(3, 2) + " " + abbonr.Substring(5, 3);
            }
            else
            {
                retval = abbonr;
            }
            return retval;
        }
        public void set_naam(int tp, bool verplicht)
        {
            this.type = tp;
            vastgezet = verplicht;
            if (vastgezet)
            {
                CBB_type.IsEnabled = false;
            }
            else
            {
                CBB_type.IsEnabled = true;
            }
            naam_lbl.Content = "Telefoon " + CBB_type.Text;
        }
        public bool check_verplicht()
        {
            Is_klaar();
            if (klaar)
            {
                TB_abbonee_nr.Background = Brushes.White;
                TB_abbonee_nr.Foreground = Brushes.Black;
                TB_prefix.Background = Brushes.White;
                TB_prefix.Foreground = Brushes.Black;
            }
            else
            {
                TB_abbonee_nr.Background = Brushes.Crimson;
                TB_abbonee_nr.Foreground = Brushes.White;
                TB_prefix.Background = Brushes.Crimson;
                TB_prefix.Foreground = Brushes.White;
            }
            return klaar;
        }
        public KeyValuePair<string,bool> Get_Nummer()
        {
            KeyValuePair<string, bool> retval;
            
            Is_klaar();
            if (Regex.IsMatch(TB_abbonee_nr.Text, @"\D") || Regex.IsMatch(TB_prefix.Text, @"\D"))
            {
                TB_prefix.Background = Brushes.Crimson;
                TB_prefix.Foreground = Brushes.White;
                TB_abbonee_nr.Background = Brushes.Crimson;
                TB_abbonee_nr.Background = Brushes.White;
                retval = new KeyValuePair<string, bool>(_nummer_working, false);
                return retval;
            }
            else if (klaar)
            {
                TB_prefix.Background = Brushes.White;
                TB_prefix.Foreground = Brushes.Black;
                TB_abbonee_nr.Background = Brushes.White;
                TB_abbonee_nr.Foreground = Brushes.Black;
                retval = new KeyValuePair<string,bool>(_nummer_working,true);
                return retval;
            }
            else if (TB_prefix.Text.Length == 0 && TB_abbonee_nr.Text.Length == 0)
            {
                leeg = true;
                TB_prefix.Background = Brushes.White;
                TB_prefix.Foreground = Brushes.Black;
                TB_abbonee_nr.Background = Brushes.White;
                TB_abbonee_nr.Foreground = Brushes.Black;
                retval = new KeyValuePair<string, bool>(null, true);
                return retval;
            }
            else
            {
                TB_prefix.Background = Brushes.Crimson;
                TB_prefix.Foreground = Brushes.White;
                TB_abbonee_nr.Background = Brushes.Crimson;
                TB_abbonee_nr.Foreground = Brushes.White;
                retval = new KeyValuePair<string, bool>(_nummer_working, false);
                return retval;
            }
        }
        private void Is_klaar()
        {
            if (TB_prefix.Text.Length > 0)
            {
                TB_abbonee_nr.IsEnabled = true;
            }
            else
            {
                TB_abbonee_nr.IsEnabled = false;
            }
            int numcount = TB_prefix.Text.Length + TB_abbonee_nr.Text.Length;
            if (numcount == 0)
            {
                leeg = true;
                klaar = false;
            }
            else { leeg = false; }
            switch (CBB_land.SelectedIndex)
            {
                case 0://NL
                    if (numcount == 10)
                    {
                        klaar = true;
                    }
                    else
                    {
                        klaar = false;
                    }
                    break;
                case 1://BE
                   
                        if (numcount > 4 && numcount < 14)
                        {
                            klaar = true;
                        }
                        else
                        {
                            klaar = false;
                        }
                    break;
                case 2://DE
                    if (numcount > 4 && numcount < 14)
                    {
                        klaar = true;
                    }
                    else
                    {
                        klaar = false;
                    }
                    break;
                case 3:
                    if (TB_prefix.Text.Length > 1 && TB_abbonee_nr.Text.Length > 4)
                    {
                        klaar = true;
                    }
                    else
                    {
                        klaar = false;
                    }
                    break;
            }
            if (klaar)
            {

                bool niet_nl = true;
                string landprefix = String.Empty;
                switch (CBB_land.SelectedIndex)
                {
                    case 0://nl
                        _nummer_working = "+31";
                        niet_nl = false;
                        break;
                    case 1://be
                        _nummer_working = "+32";
                        landprefix = "+32";
                        break;
                    case 2://de
                        _nummer_working = "+49";
                        landprefix = "+49";
                        break;
                    case 3://overig
                        _nummer_working ="+"+ CBB_overige_landen.SelectedValue.ToString();
                        landprefix = "+"+ CBB_overige_landen.SelectedValue.ToString();
                        break;
                }
                string hlpr1 = String.Empty;
                if (TB_prefix.Text.Length > 1)
                {
                    hlpr1 = TB_prefix.Text.Substring(1);
                }
                else
                {
                    hlpr1 = TB_prefix.Text;
                }
                if (hlpr1.StartsWith("0"))
                {
                    hlpr1.Remove(0, 1);
                }
                _nummer_working += "(0)" + hlpr1 + " " + TB_abbonee_nr.Text;
                
                if (niet_nl)
                {
                    string hlpr = String.Empty;
                    if (TB_prefix.Text.Length > 1)
                    {
                        hlpr = TB_prefix.Text.Substring(1);
                    }
                    else
                    {
                        hlpr = TB_prefix.Text;
                    }
                    if (hlpr.StartsWith("0"))
                    {
                        hlpr.Remove(0, 1);
                    }
                    _nummer_tonen = landprefix + " " +hlpr  + " " + split_abbo_nr(TB_abbonee_nr.Text);
                    lbl_nr.Content = _nummer_tonen;
                }
                else
                {
                    _nummer_tonen = TB_prefix.Text + " " + split_abbo_nr(TB_abbonee_nr.Text);
                    lbl_nr.Content = _nummer_tonen;
                }
            }
        }
        public void Annuleer()
        {
            set_Nummer(_nummer_ingeladen,parent);
        }
        public void Wijzigstand(bool wijzig)
        {
            _wijzigstand = wijzig;
            if (wijzig)
            {
                this.Visibility = Visibility.Visible;
                lbl_nr.Visibility = Visibility.Collapsed;
                CBB_type.Visibility = Visibility.Visible;
                TB_prefix.Visibility = Visibility.Visible;
                TB_abbonee_nr.Visibility = Visibility.Visible;
                if (CBB_land.SelectedIndex == 3)
                {
                    CBB_overige_landen.Visibility = Visibility.Visible;
                }
                else
                {
                    CBB_land.Visibility = Visibility.Visible;
                }
                del.Visibility = Visibility.Visible;

               /* if (TB_abbonee_nr.Text.Length > 0 || TB_prefix.Text.Length > 0)
                {
                    //this.Visibility = Visibility.Visible;
                  if (parent is persoon_form)
                    {
                        (parent as persoon_form).toon_tel_nrs(this, true);
                    }
                    else if (parent is bedrijf_form)
                    {
                        (parent as bedrijf_form).toon_tel_nrs(this, true);
                    }
                }
                else if (TB_abbonee_nr.Text.Length < 1 && TB_prefix.Text.Length < 1)
                {
                    this.Visibility = Visibility.Collapsed;
                    if (parent is persoon_form)
                    {
                        (parent as persoon_form).toon_tel_nrs(this, false);
                    }
                    else if (parent is bedrijf_form)
                    {
                        (parent as bedrijf_form).toon_tel_nrs(this, false);
                    }
                }*/
            }
            else
            {
                
                lbl_nr.Visibility = Visibility.Visible;
                CBB_land.Visibility = Visibility.Collapsed;
                CBB_type.Visibility = Visibility.Collapsed;
                TB_prefix.Visibility = Visibility.Collapsed;
                TB_abbonee_nr.Visibility = Visibility.Collapsed;
                CBB_overige_landen.Visibility = Visibility.Collapsed;
                del.Visibility = Visibility.Collapsed;
                Is_klaar();
                if (klaar)
                {
                    lbl_nr.Content = _nummer_tonen;
                }
                else if(leeg)
                {
                    lbl_nr.Content = String.Empty;
                }
                else
                {
                      TB_prefix.Text = String.Empty;
                      TB_abbonee_nr.Text = String.Empty;
                    lbl_nr.Content = string.Empty;
                }
                if (TB_abbonee_nr.Text.Length == 0)
                {
                  //  this.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                }
            }
        }
        private void CBB_land_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TB_prefix.Text = String.Empty;
            TB_abbonee_nr.Text = String.Empty;
            switch (CBB_land.SelectedIndex)
            {
                case -1:
                    _nummer_working = String.Empty;
                    CBB_overige_landen.Visibility = Visibility.Collapsed;
                    CBB_land.Visibility = Visibility.Visible;
                    TB_prefix.IsEnabled = false;
                    break;
                case 0://NL
                    _nummer_working = "+31";
                    if (!vastgezet)
                    {
                        CBB_type.IsEnabled = true;
                    }
                    TB_prefix.IsEnabled = true;
                    TB_prefix.MaxLength = 4;
                    
                        
                    break;
                case 1://BE
                    _nummer_working = "+32";
                    if (!vastgezet)
                    {
                        CBB_type.IsEnabled = true;
                    }
                    TB_prefix.IsEnabled = true;
                    TB_prefix.MaxLength = 5;
                    TB_abbonee_nr.MaxLength = 9;
                    break;
                case 2://DE
                    _nummer_working = "+49";
                    if (!vastgezet)
                    {
                        CBB_type.IsEnabled = true;
                    }
                    TB_prefix.IsEnabled = true;
                     TB_prefix.MaxLength = 5;
                        TB_abbonee_nr.MaxLength = 10;
                    break;
                case 3://overig
                    CBB_land.Visibility = Visibility.Collapsed;
                    TB_prefix.MaxLength = 5;
                        TB_abbonee_nr.MaxLength = 10;
                    if (_wijzigstand)
                    {
                        CBB_overige_landen.Visibility = Visibility.Visible;
                    }
                    break;
            }
            if (CBB_type.SelectedIndex == 1)
            {
                switch (CBB_land.SelectedIndex)
                {
                    case 0://NL
                        _nummer_working += "(0)6 ";
                        TB_prefix.Text = "06";
                        TB_prefix.IsEnabled = false;
                        TB_abbonee_nr.IsEnabled = true;
                        TB_prefix.MaxLength = 2;
                        TB_abbonee_nr.MaxLength = 8;
                        break;
                    case 1://BE
                        _nummer_working += "(0)4 ";
                        TB_prefix.Text = "04";
                        //  TB_prefix.IsEnabled = false;
                        TB_abbonee_nr.IsEnabled = true;
                        TB_prefix.MaxLength = 5;
                        TB_abbonee_nr.MaxLength = 8;
                        break;
                    case 2://DE
                        _nummer_working += "(0)1 ";
                        TB_prefix.Text = "01";
                        //   TB_prefix.IsEnabled = false;
                        TB_abbonee_nr.IsEnabled = true;
                        TB_prefix.MaxLength = 5;
                        TB_abbonee_nr.MaxLength = 10;
                        break;
                    case 3:
                        TB_prefix.IsEnabled = true;
                        TB_abbonee_nr.IsEnabled = true;
                        TB_prefix.MaxLength = 5;
                        TB_abbonee_nr.MaxLength = 10;
                        break;
                }
            }
        }
        private void CBB_type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TB_prefix.Text = String.Empty;
            if (CBB_type.SelectedIndex != -1)
            {
                CBB_land.IsEnabled = true;
                CBB_land.SelectedIndex = -1;
                if (CBB_type.SelectedIndex != 1)
                {
                    TB_prefix.IsEnabled = true;
                }
            }
            
            
        }
        private void TB_prefix_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ( e.Key != Key.D0 && e.Key != Key.NumPad0 &&TB_prefix.Text.Length == 0 && !(e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right))
            {
                TB_prefix.Text = "0";
                TB_prefix.SelectionStart = 1;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Key.ToString(), "\\d+") && !(e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right))
            {
                e.Handled = true;
            }
            else if (!(e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right))
            {
                leeg = false;
            }
            if (TB_prefix.Text.Length > 4 && !(e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right))
            {
                e.Handled = true;
            }
            else if (!(e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right))
            {
                leeg = false;
            }
            Is_klaar();
        }
        private void TB_abbonee_nr_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Key.ToString(), "\\d+") && !(e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right))
            {
                e.Handled = true;
            }
            else if(!(e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right))
            {
                leeg = false;
            }
            if (CBB_land.SelectedIndex == 0)
            {
                if (((TB_abbonee_nr.Text.Length == 6 && TB_prefix.Text.Length == 4) || (TB_abbonee_nr.Text.Length == 7 && TB_prefix.Text.Length == 3))&& !(e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right))
                {
                    e.Handled = true;
                }
                else if (TB_abbonee_nr.Text.Length == 8 && !(e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right))
                {
                    e.Handled = true;
                }
            }
            Is_klaar();
        }
        private void CBB_overige_landen_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (CBB_overige_landen.Items.Count > 0)
            {
                bool hit = false;
                TextBox txt = CBB_overige_landen.Template.FindName("PART_EditableTextBox", CBB_overige_landen) as TextBox;
                string test;
                if (txt.SelectedText.Length > 0)
                {
                    int indx = txt.Text.Length - txt.SelectedText.Length;
                    test = txt.Text.Remove(indx) + e.Text.ToString();
                }
                else
                {
                        test = CBB_overige_landen.Text + e.Text.ToString();
                }
                foreach (AdresDataSet.landcodesRow LR in lcDt)
                {
                    
                    if (LR.land.StartsWith(test, true, System.Globalization.CultureInfo.CurrentCulture))//SI.straat.StartsWith(test, true, System.Globalization.CultureInfo.CurrentCulture))
                    {
                        hit = true;
                    }
                }
                if (!hit)
                {
                    e.Handled = true;

                }
            }
        }
        private void del_MouseDown(object sender, MouseButtonEventArgs e)
        {
            leeg = true;
            CBB_land.SelectedIndex = -1;
            CBB_overige_landen.SelectedIndex = -1;
            if (!vastgezet)
            {
                CBB_type.SelectedIndex = -1;
                CBB_land.IsEnabled = false;
            }
            TB_prefix.Text = String.Empty;
            TB_abbonee_nr.Text = String.Empty;
            CBB_overige_landen.Visibility = Visibility.Collapsed;
            CBB_land.Visibility = Visibility.Visible;
            
            //CBB_type.IsEnabled = false;
            TB_prefix.IsEnabled = false;
            TB_abbonee_nr.IsEnabled = false;
            _nummer_working = String.Empty;
            _nummer_tonen = String.Empty;
            if (parent is persoon_form)
            {
                (parent as persoon_form).Hide_tel_ctrl(this);
            }
            else if (parent is bedrijf_form)
            {
                (parent as bedrijf_form).Hide_tel_ctrl(this);
            }
            else if (parent is project_form)
            {
                (parent as project_form).Hide_tel_ctrl(this);
            }
        }
        bool selecta = false;
        private void TB_prefix_LostFocus(object sender, RoutedEventArgs e)
        {
            selecta = false;
            TextBox tb = sender as TextBox;
            switch (tb.Name)
            {
                case "TB_prefix":
                    if (CBB_land.SelectedIndex == 0)
                    {
                        if (tb.Text.Length == 3)
                        {
                            TB_abbonee_nr.MaxLength = 7;
                        }
                        else if (tb.Text.Length == 4)
                        {
                            TB_abbonee_nr.MaxLength = 6;
                        }
                        else if (tb.Text.Length == 2)
                        {
                            TB_abbonee_nr.MaxLength = 8;
                        }
                    }
                    break;
                case "TB_abbonee_nr":
                    if (CBB_land.SelectedIndex == 0)
                    {
                        if (tb.Text.Length == 6)
                        {
                            TB_prefix.MaxLength = 4;
                        }
                        else if (tb.Text.Length == 7)
                        {
                            TB_prefix.MaxLength = 3;
                        }
                        else if (tb.Text.Length == 8)
                        {
                            TB_prefix.MaxLength = 2;
                        }
                    }
                    break;
            }
        }
        private void TB_prefix_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!selecta)
            {
                (sender as TextBox).SelectAll();
                (sender as TextBox).ReleaseMouseCapture();
                selecta = true;
            }
            else
            {
                selecta = false;
            }
            if (netnr_bekend && TB_prefix.Text.Length < 1 && (CBB_type.SelectedIndex == 0 || CBB_type.SelectedIndex == 2))
            {
                TB_prefix.Text = _Net_Nummers.Peek();
                (sender as TextBox).SelectAll();
                TB_abbonee_nr.IsEnabled = true;
            }
        }
        private void TB_prefix_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (t.SelectedText.Length != t.Text.Length)
            {
                t.SelectAll();

                t.ReleaseMouseCapture();

                e.Handled = true;
            }
        }
        private void CBB_overige_landen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbb = sender as ComboBox;
            
            if (cbb.SelectedIndex != -1 && cbb.SelectedValue != null)
            {
                TB_prefix.IsEnabled = true;
                TB_prefix.MaxLength = 5; TB_abbonee_nr.MaxLength = 10;
                switch (cbb.SelectedValue.ToString())
                {
                    case "0"://terug
                        cbb.Visibility = Visibility.Collapsed;
                        CBB_land.Visibility = Visibility.Visible;
                        cbb.SelectedIndex = -1;
                        CBB_land.SelectedIndex = -1;
                        break;
                    case "31"://nl
                        TB_prefix.Text = String.Empty;
                        TB_abbonee_nr.Text = String.Empty;
                        cbb.Visibility = Visibility.Collapsed;
                        CBB_land.Visibility = Visibility.Visible;
                        cbb.SelectedIndex = -1;
                        CBB_land.SelectedIndex = 0;
                        TB_prefix.IsEnabled = true;
                        TB_abbonee_nr.IsEnabled = true;

                        break;
                    case "32"://be
                        TB_prefix.Text = String.Empty;
                        TB_abbonee_nr.Text = String.Empty;
                        cbb.Visibility = Visibility.Collapsed;
                        CBB_land.Visibility = Visibility.Visible;
                        cbb.SelectedIndex = -1;
                        CBB_land.SelectedIndex = 1;
                        TB_prefix.IsEnabled = true;
                        TB_abbonee_nr.IsEnabled = true;
                        break;
                    case "49"://de
                        TB_prefix.Text = String.Empty;
                        TB_abbonee_nr.Text = String.Empty;
                        cbb.Visibility = Visibility.Collapsed;
                        CBB_land.Visibility = Visibility.Visible;
                        cbb.SelectedIndex = -1;
                        CBB_land.SelectedIndex = 2;
                        TB_prefix.IsEnabled = true;
                        TB_abbonee_nr.IsEnabled = true;
                        break;
                }
            }
        }
        private void TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
           /* if (tb.Text.Length > 0)
            {
                this.Visibility = Visibility.Visible;
               if (parent is persoon_form)
                {
                    (parent as persoon_form).toon_tel_nrs(this, true);
                }
                else if (parent is bedrijf_form)
                {
                    (parent as bedrijf_form).toon_tel_nrs(this, true);
                }
            }
            else if(TB_abbonee_nr.Text.Length <1 && TB_prefix.Text.Length < 1)
            {
                this.Visibility = Visibility.Collapsed;
                if (parent is persoon_form)
                {
                    (parent as persoon_form).toon_tel_nrs(this, false);
                }
                else if (parent is bedrijf_form)
                {
                    (parent as bedrijf_form).toon_tel_nrs(this, false);
                }
            }*/
        }
        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            telefoonnummer_user_control tuc = sender as telefoonnummer_user_control;
            if (tuc.Visibility == Visibility.Collapsed)
            {
               /* _nummer_working = String.Empty;
                _nummer_tonen = String.Empty;
                CBB_land.SelectedIndex = -1;
                CBB_type.SelectedIndex = -1;
                TB_prefix.Text = String.Empty;
                TB_abbonee_nr.Text = String.Empty;
                CBB_overige_landen.SelectedIndex = -1;
                CBB_overige_landen.Visibility = Visibility.Collapsed;*/
            }
            else
            {
                this.Refresh();
            }
            
        }
        public void Api_get_areacode(string _postcode)
        {
            bw.RunWorkerAsync(_postcode);
        }
        public void Minder_opties()
        {
            CBB_type.Items.RemoveAt(2);
            CBB_type.Items.RemoveAt(2);
            CBB_type.Items.RemoveAt(2);
            CBB_type.Items.RemoveAt(2);
            del.IsEnabled = false;
        }
    }
}

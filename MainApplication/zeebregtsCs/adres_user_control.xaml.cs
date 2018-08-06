using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Linq;
using System.Windows.Interop;
namespace zeebregtsCs
{
    /// <summary>
    /// Interaction logic for adres_user_control.xaml
    /// </summary>
    public partial class adres_user_control : UserControl
    {

        ComboEngine _cbbeng = new ComboEngine();
        BackgroundWorker lastPlaatsWorker;
        BackgroundWorker lastStraatWorker;
        private bool _viaPostcode;
        private bool changedFromCode;
        private int adres_id;
        private bool ISproject = false;
        private bool starting_up = false;
        private bool _wijzigstand;
        private Stack<string> _areacode = new Stack<string>();
        private string[] blackList = { "--", ";--", ";", "/*", "*/", "@@", "@" };
        public delegate void standchangedeventhandler(bool wijzig, EventArgs e);
        public event standchangedeventhandler standchanged;
        public string _City_key;
        public string LoadedCityKey;
        public adres_user_control()
        {
            InitializeComponent();
            this.standchanged += new standchangedeventhandler(adres_standchanged);
            _cbbeng = (ComboEngine)this.DataContext;

        }
        private BackgroundWorker newRequestPlaats()
        {
            var requestPlaats = new BackgroundWorker();
            requestPlaats.DoWork += requestPlaats_DoWork;
            requestPlaats.RunWorkerCompleted += requestPlaats_RunWorkerCompleted;
            requestPlaats.WorkerSupportsCancellation = true;
            return requestPlaats;
        }
        private BackgroundWorker newRequestStraat()
        {
            var requestStraat = new BackgroundWorker();
            requestStraat.DoWork += requestStraat_DoWork;
            requestStraat.RunWorkerCompleted += requestStraat_RunWorkerCompleted;
            requestStraat.WorkerSupportsCancellation = true;
            return requestStraat;
        }
        void requestStraat_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TextBox txt = CBB_straat.Template.FindName("PART_EditableTextBox", CBB_straat) as TextBox;
            if (!e.Cancelled)
            {
                var results = (List<object>)e.Result;
                var resFound = (bool)results[0];
                var pStratenTmp = (ObservableCollection<StraatItem>)results[1];

                if (resFound)
                {
                    var texttmp = CBB_straat.Text;
                    changedFromCode = true;
                    _cbbeng.PStraten.Clear();
                    _cbbeng.PStraten = pStratenTmp;
                    CBB_straat.Text = texttmp;
                    changedFromCode = false;
                    CBB_straat.Refresh();

                    if (!txt.IsFocused)
                    {
                        if( CBB_straat.Items.Count == 1)
                        {
                        CBB_straat.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        CBB_straat.IsDropDownOpen = true;
                    }
                }
                else
                {
                    CBB_straat.IsDropDownOpen = false;
                }
            }
           // TextBox txt = CBB_straat.Template.FindName("PART_EditableTextBox", CBB_straat) as TextBox;
            if (txt != null)
            {
                txt.Select(txt.Text.Length, 0);
            }
        }

        void requestStraat_DoWork(object sender, DoWorkEventArgs e)
        {
            var requestStraat = (BackgroundWorker)sender;

            var argus = (List<object>)e.Argument;
            var _straat = (string)argus[0];
            var cbb_landIndx = (int)argus[1];
            var pStratenTmp = new ObservableCollection<StraatItem>();
            ServicePointManager.DefaultConnectionLimit = 15;
            if (requestStraat.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            bool resultsfound = false;
            try
            {
                if (Wijzigstand)
                {
                    if (cbb_landIndx == 0)
                    {
                        ///////////////////////////////////////////////////////////////
                        String auth_key = Global.ZES_PP_AUTH_KEY;
                        String street = _straat;
                        String city_key = _City_key;

                        // Create the GET request
                        string uri = "http://api.pro6pp.nl/v1/suggest";
                        uri += String.Format("?auth_key={0}&street={1}&city_key={2}&per_page=200", auth_key, HttpUtility.UrlEncode(street), city_key);
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                        request.Method = "GET";
                        request.ContentType = "text/xml; encoding='utf-8'";
                        request.Timeout = 5000;

                        if (requestStraat.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                        // Get the response
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (requestStraat.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }

                            string result = new StreamReader(response.GetResponseStream()).ReadToEnd();

                            // Parse the response
                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            APIResponse_street r = serializer.Deserialize<APIResponse_street>(result);

                            if (r.status == "ok")
                            {
                                resultsfound = true;
                                foreach (APIResult_street rs in r.results)
                                {
                                    StraatItem si = new StraatItem { straat = rs.street, postcode = rs.nl_sixpps };
                                    pStratenTmp.Add(si);
                                }
                                
                            }
                            else
                            {
                                
                                /*this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
                                {
                    
                                    //MessageBox.Show("Geen straten gevonden, fout in straatnaam of straat niet bekend bij deze plaats. ");
                                }));*/
                                resultsfound = false;
                            }
                            ///////////////////////////////////////////////////////////////
                        }
                    }
                }
                else
                {
                    resultsfound = true;
                }
            }
            catch (WebException we)
            {
                String log_line = "Exception occurred @ " + DateTime.Now.ToString() + " error: " + we;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
            }
            if (requestStraat.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            var retvals = new List<object>();
            retvals.Add(resultsfound);
            retvals.Add(pStratenTmp);
            e.Result = retvals;
        }

        void requestPlaats_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TextBox txt = CBB_plaats.Template.FindName("PART_EditableTextBox", CBB_plaats) as TextBox;
            if (!e.Cancelled)
            {
                var results = (List<object>)e.Result;
                var resFound = (bool)results[0];
                var pStedenTmp = (ObservableCollection<PlaatsItem>)results[1];
                if (resFound == true)
                {
                    var texttmp = CBB_plaats.Text;
                    changedFromCode = true;
                    _cbbeng.PSteden.Clear();
                    _cbbeng.PSteden = pStedenTmp;
                    CBB_plaats.Text = texttmp;
                    changedFromCode = false;
                    //CBB_plaats.SetBorder(true);
                    CBB_plaats.Refresh();
                    CBB_straat.IsEnabled = true;

                    if (!txt.IsFocused)
                    {
                        if (CBB_plaats.Items.Count == 1)
                        {
                            CBB_plaats.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        CBB_plaats.IsDropDownOpen = true;
                    }

                }
                else
                {
                    CBB_plaats.IsDropDownOpen = false;
                }
            }
         //   TextBox txt = CBB_plaats.Template.FindName("PART_EditableTextBox", CBB_plaats) as TextBox;
            if (txt != null)
            {
                txt.Select(txt.Text.Length, 0);
            }
        }
        void requestPlaats_DoWork(object sender, DoWorkEventArgs e)
        {
            var requestPlaats = (BackgroundWorker)sender;
            var argus = (List<object>)e.Argument;
            if (requestPlaats.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            var _plaats = (string)argus[0];
            var cbbland_selIndx = (int)argus[1];
            var pStedenTmp = new ObservableCollection<PlaatsItem>();
             ServicePointManager.DefaultConnectionLimit = 15;
            bool resultsfound = false;

            try
            {
                if (Wijzigstand)
                {
                    if (cbbland_selIndx == 0)
                    {   ///////////////////////////////////////////
                        String auth_key = Global.ZES_PP_AUTH_KEY;
                        String nl_city = Trim4Api(_plaats);

                        // Create the GET request
                        string uri = "http://api.pro6pp.nl/v1/suggest";
                        uri += String.Format("?auth_key={0}&nl_city={1}&per_page=40", auth_key, HttpUtility.UrlEncode(nl_city));
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                        request.Method = "GET";
                        request.ContentType = "text/xml; encoding='utf-8'";
                        request.Timeout = 5000;
                        // Get the response
                        if (requestPlaats.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            if (requestPlaats.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }
                            string result = new StreamReader(response.GetResponseStream()).ReadToEnd();

                            // Parse the response
                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                            APIResponse_suggest r = serializer.Deserialize<APIResponse_suggest>(result);

                            if (r.status == "ok")
                            {
                                pStedenTmp.Clear();
                                resultsfound = true;
                                if (r.results.Count > 0) { _City_key = r.results[0].city_key; }
                                foreach (APIResult_suggest rs in r.results)
                                {
                                    PlaatsItem pi = new PlaatsItem { city_key = rs.city_key, city = rs.city, official_city = rs.official_city, vierpostcode = rs.nl_fourpps };
                                    pStedenTmp.Add(pi);
                                    //_City_key = rs.city_key;
                                }

                               
                            }
                            else
                            {
                                //if (Wijzigstand )
                                //{
                                //    this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
                                //    {

                                //        MessageBox.Show("Geen plaatsen gevonden, fout in plaatsnaam.");
                                //    }));
                                //}
                                resultsfound = false;
                            }

                            ///////////////////////////////////////////

                            
                        }
                    }
                }
                else
                {
                    resultsfound = true;
                }
            }
            catch (WebException we)
            {
                String log_line = "Exception occurred @ " + DateTime.Now.ToString() + " error: " + we;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
            }
            if (requestPlaats.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            var results = new List<object>();
            results.Add(resultsfound);
            results.Add(pStedenTmp);
            e.Result = results;
        }
        bool newproj = false;
        public void initialiseer_newproj()
        {
            newproj = true;
            starting_up = true;
            CBB_land.SelectedIndex = 0;
            Wijzigstand = true;
            ViaPostcode = false;
            row_left_modus.Visibility = Visibility.Collapsed;
            row_left_land.Visibility = Visibility.Collapsed;
            row_left_straat.Visibility = Visibility.Collapsed;
            row_left_postcode.Visibility = Visibility.Collapsed;
            row_left_huisnummer.Visibility = Visibility.Collapsed;
            row_left_straat2.Visibility = Visibility.Collapsed;
            row_right_modus.Visibility = Visibility.Collapsed;
            row_right_land.Visibility = Visibility.Collapsed;
            row_right_straat.Visibility = Visibility.Collapsed;
            row_right_postcode.Visibility = Visibility.Collapsed;
            row_right_huisnummer.Visibility = Visibility.Collapsed;
            row_right_straat2.Visibility = Visibility.Collapsed;
            RB_handmatig.IsChecked = true;
            starting_up = false;
        }
        public void initialiseer(bool wijzig, bool proj)
        {
            starting_up = true;
            this.ISproject = proj;
            if (ISproject)
            {
                RB_handmatig.IsChecked = true;
                row_left_modus.Visibility = System.Windows.Visibility.Collapsed;
                row_right_modus.Visibility = System.Windows.Visibility.Collapsed;
                row_left_plaats.Visibility = Visibility.Visible;
                row_right_plaats.Visibility = Visibility.Visible;
                row_left_straat2.Visibility = Visibility.Collapsed;
                row_right_straat2.Visibility = Visibility.Collapsed;
                LBL_straat2.Visibility = Visibility.Visible;

            }
            else
            {
                RB_via_postcode.IsChecked = true;//hier
                row_left_straat2.Visibility = System.Windows.Visibility.Collapsed;
                row_right_straat2.Visibility = System.Windows.Visibility.Collapsed;
                row_left_postcode.Visibility = System.Windows.Visibility.Visible;
                row_right_postcode.Visibility = System.Windows.Visibility.Visible;
                //row_left_plaats.Visibility = Visibility.Collapsed;
               // row_right_plaats.Visibility = Visibility.Collapsed;
            }

            Wijzigstand = wijzig;
            starting_up = false;
        }

        
        
        
        
        public void load_data(string _plaats, string _straat, string _postcode)
        {
            TB_postcode_cijfers.Text = String.Empty;
            TB_postcode_letters.Text = String.Empty;
            TB_huisnummer.Text = String.Empty;
            TB_huisnummer_toevoeging.Text = String.Empty;
           // CBB_land.Text = String.Empty;
            CBB_land.SelectedIndex = 0;
            CBB_plaats.Text = String.Empty;
            CBB_straat.Text = String.Empty;
            CBB_straat2.Text = String.Empty;
            if (_postcode != null)
            {

                var regx = new Regex(@"(\d{4}\s*)(\D{2})");
                var match = regx.Match(_postcode);
                if (match.Success)
                {
                    RB_via_postcode.IsChecked = true;
                    Postcode_cijfers = int.Parse(match.Groups[1].ToString());
                    Postcode_letters = match.Groups[2].ToString();
                    

                }
                else
                {
                    RB_handmatig.IsChecked = true;
                    if (_plaats != null)
                    {

                        Plaats = _plaats;
                       
                    }
                    if (_straat != null)
                    {
                        var regx2 = new Regex(@"^(.+)\s(\d+(\s*[^\d\s]+)*)$");
                        var match2 = regx2.Match(_straat);
                        if (match2.Success)
                        {
                            Straat = match2.Groups[1].ToString();
                            
                        }
                        else
                        {
                            string[] tmp_strt = Regex.Split(_straat, @"\d");
                            Straat = tmp_strt[0];
                            
                        }
                    }
                }

            }
            else
            {
                RB_handmatig.IsChecked = true;
                if (_plaats != null)
                {

                    Plaats = _plaats;
                    
                }
                if (_straat != null)
                {
                    var regx2 = new Regex(@"^(.+)\s(\d+(\s*[^\d\s]+)*)$");
                    var match2 = regx2.Match(_straat);
                    if (match2.Success)
                    {
                        Straat = match2.Groups[1].ToString();
                        
                    }
                    else
                    {
                        string[] tmp_strt = Regex.Split(_straat, @"\d");
                        Straat = tmp_strt[0];
                        
                    }
                }
            }

            if (_straat != null)
            {
                var regx0 = new Regex(@"^(.+)\s(\d+(\s*[^\d\s]+)*)$");
                var match0 = regx0.Match(_straat);
                if (match0.Success)
                {
                    string[] tmp_hsnr = Regex.Split(match0.Groups[2].ToString(), @"[a-z]|[A-Z]");
                    Huisnummer = tmp_hsnr[0];
                    if (tmp_hsnr.Length > 1)
                    {
                        Huisnummer_toevoeging = tmp_hsnr[1];
                    }
                }
                else
                {
                    string[] tmp_strt = Regex.Split(_straat, @"\d");
                    if (tmp_strt.Length > 1)
                    {
                        string[] tmp_hsnr = Regex.Split(tmp_strt[1], @"[a-z]|[A-Z]");
                        Huisnummer = tmp_hsnr[0];
                        if (tmp_hsnr.Length > 1)
                        {
                            Huisnummer_toevoeging = tmp_hsnr[1];
                        }
                    }
                }
            }


        }
       /* public string[] save_data()
        {
            string[] returns = new string[4];
            if (Plaats != null && Plaats != string.Empty)
            {
                returns[0] = Plaats;
            }
            if (Straat != null && Straat != string.Empty)
            {
                returns[1] += Straat;
            }
            if (Huisnummer != null && Huisnummer != string.Empty)
            {
                returns[1] += " " + Huisnummer;
            }
            if (Huisnummer_toevoeging != null && Huisnummer_toevoeging != string.Empty)
            {
                returns[1] += Huisnummer_toevoeging;
            }
            if (Postcode_cijfers.ToString().Length > 3 && Postcode_letters != string.Empty && Postcode_letters != null && Postcode_letters.Length == 2)
            {
                returns[2] = Postcode_cijfers + Postcode_letters;
            }
            else if (Land != "Nederland" && Postcode_cijfers > 999)
            {
                returns[2] = Postcode_cijfers.ToString();
            }
            if (ISproject)
            {
                if (Straat2 != null && Straat2 != string.Empty)
                {
                    returns[3] = Straat2;
                }
            }
            return returns;
        }
        * */
        public bool invul_check()
        {
            bool retval = true;
            //CBB_plaats.SetBorder(true);
            CBB_plaats.Refresh();
            if (newproj) 
            {
                if (Plaats == String.Empty)
                {
                    //CBB_plaats.SetBorder(false);
                    CBB_plaats.Refresh();
                    retval = false;
                }
                else
                {
                    //CBB_plaats.SetBorder(true);
                    CBB_plaats.Refresh();
                }
            }
            else
            {
                CBB_land.Background = Brushes.White;
                CBB_land.Foreground = Brushes.Black;
                CBB_straat.Background = Brushes.White;
                CBB_straat.Foreground = Brushes.Black;
                TB_postcode_cijfers.Background = Brushes.White;
                TB_postcode_cijfers.Foreground = Brushes.Black;
                TB_postcode_letters.Background = Brushes.White;
                TB_postcode_letters.Foreground = Brushes.Black;
                TB_huisnummer.Background = Brushes.White;
                TB_huisnummer.Foreground = Brushes.Black;
                
                //CBB_plaats.SetBorder(true);
                if (Plaats == String.Empty)
                {
                    //CBB_plaats.SetBorder(false);
                    CBB_plaats.Refresh();
                    retval = false;
                }
                if (Land == String.Empty)
                {
                    //CBB_land.Background = Brushes.Crimson;
                    //CBB_land.Foreground = Brushes.White;
                    retval = false;
                }
                if (Straat == String.Empty)
                {
                    //CBB_straat.Background = Brushes.Crimson;
                    //CBB_straat.Foreground = Brushes.White;
                    retval = false;
                }
                if (Postcode_cijfers < 999)
                {
                    //TB_postcode_cijfers.Background = Brushes.Crimson;
                    //TB_postcode_cijfers.Foreground = Brushes.White;
                    retval = false;
                }
                if (Land == "Nederland" && Postcode_letters == String.Empty)
                {
                    //TB_postcode_letters.Background = Brushes.Crimson;
                    //TB_postcode_letters.Foreground = Brushes.White;
                    retval = false;
                }
                if (Huisnummer == String.Empty)
                {
                    //TB_huisnummer.Background = Brushes.Crimson;
                    //TB_huisnummer.Foreground = Brushes.White;
                    retval = false;
                }
            }
            return retval;
        }
        public string Huisnummer
        {
            get { return TB_huisnummer.Text; }
            set
            {
                TB_huisnummer.Text = value.ToString();
                LBL_huisnummer.Content = value.ToString() + TB_huisnummer_toevoeging.Text;
                text_veranderd(TB_huisnummer);
            }
        }
        public string Huisnummer_toevoeging
        {
            get { return TB_huisnummer_toevoeging.Text; }
            set
            {
                TB_huisnummer_toevoeging.Text = value;
                LBL_huisnummer.Content = TB_huisnummer.Text + value;
                text_veranderd(TB_huisnummer_toevoeging);
            }
        }
        public int Postcode_cijfers
        {
            get
            {
                if (TB_postcode_cijfers.Text != string.Empty)
                {
                    return int.Parse(TB_postcode_cijfers.Text);
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if ((int)value > 0)
                {
                    TB_postcode_cijfers.IsEnabled = true;
                    TB_postcode_cijfers.Text = value.ToString();
                    LBL_postcode.Content = value.ToString() + " " + TB_postcode_letters.Text;
                }
                else
                {
                    TB_postcode_cijfers.Text ="-";
                    LBL_postcode.Content = "-";
                }
                if (!Wijzigstand)
                {
                    if (value.ToString().Length > 3 && TB_postcode_letters.Text.Length > 1)
                    {
                        LBL_postcode.Content = value.ToString() + " " + TB_postcode_letters.Text + " " + LBL_plaats.Content;
                        postcode_lbl.Content = "Postcode";
                    }
                    else
                    {
                        LBL_postcode.Content = LBL_plaats.Content;
                        postcode_lbl.Content = "Plaats";
                    }
                    LBL_postcode.Visibility = Visibility.Visible;
                    LBL_postcode.Refresh();
                    
                }
                if (TB_postcode_cijfers.Text.Length > 0 && ISproject)
                {
                    row_left_postcode.Visibility = Visibility.Visible;
                    row_right_postcode.Visibility = Visibility.Visible;
                }
                
            }
        }
        public string Postcode_letters
        {
            get { return TB_postcode_letters.Text; }
            set
            {
                TB_postcode_letters.Text = value;
                LBL_postcode.Content = TB_postcode_cijfers.Text + " " + value;
                if (TB_postcode_letters.Text.Length > 0)
                {
                    TB_postcode_letters.IsEnabled = true;
                }
                if (!Wijzigstand)
                {
                    if (value.ToString().Length > 1 && TB_postcode_cijfers.Text.Length > 3)
                    {
                        LBL_postcode.Content = TB_postcode_cijfers.Text + " " + value.ToString() + " " + LBL_plaats.Content;
                        postcode_lbl.Content = "Postcode";
                    }
                    else
                    {
                        LBL_postcode.Content = LBL_plaats.Content;
                        postcode_lbl.Content = "Plaats";
                    }
                    LBL_postcode.Visibility = Visibility.Visible;
                    LBL_postcode.Refresh();
                    row_left_plaats.Visibility = Visibility.Collapsed;
                    row_right_plaats.Visibility = Visibility.Collapsed;
                    row_right_postcode.Visibility = Visibility.Visible;
                    row_left_postcode.Visibility = Visibility.Visible;

                }
                else
                {
                    row_left_plaats.Visibility = Visibility.Visible;
                    row_right_plaats.Visibility = Visibility.Visible;
                }
                
            }
        }
        public string Plaats
        {
            get { return CBB_plaats.Text; }
            set
            {
                CBB_plaats.Text = value;
                LBL_plaats.Content = value;
                if (value.Length > 2)
                {
                    ZoekNaarPlaats(value);
                    if (CBB_plaats.Items.Count> 0) 
                    {
                        if (LoadedCityKey != null)
                        {
                            CBB_plaats.SelectedIndex = CBB_plaats.Items.IndexOf(_cbbeng.PSteden.FirstOrDefault(x=>x.city_key == LoadedCityKey));
                        }
                        else
                        {
                            CBB_plaats.SelectedIndex = 0;
                        }
                    }
					//CBB_plaats.SelectedIndex = 0;
                    if (Wijzigstand)
                    {
                        row_left_plaats.Visibility = Visibility.Visible;
                        row_right_plaats.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    LBL_plaats.Content = "-";
                }
                
            }
        }
        public string Straat
        {
            get { return CBB_straat.Text; }
            set
            {
                CBB_straat.Text = value;
                LBL_straat.Content = value;
                if (CBB_straat.Text.Length > 0)
                {
                    CBB_straat.IsEnabled = true;
                }
                if (!Wijzigstand)
                {
                    LBL_straat.Content += " " + LBL_huisnummer.Content;
                }
                if (value.Length > 2)
                {
                    ZoekNaarStraat(value);
                    if (CBB_straat.Items.Count == 1)
                    {
                        CBB_straat.SelectedIndex = 0;
                        TB_postcode_cijfers.IsEnabled = true;
                        TB_postcode_letters.IsEnabled = true;
                    }
                }
                else
                {
                    LBL_straat.Content = "-";
                }
            }
        }
        public string Straat2
        {
            get { return CBB_straat2.Text; }
            set
            {
                CBB_straat2.Text = value;
                LBL_straat2.Content = value;
                if (CBB_straat2.Text.Length > 0 && ISproject)
                {
                    row_left_straat2.Visibility = Visibility.Visible;
                    row_right_straat2.Visibility = Visibility.Visible;
                }
            }
        }
        public string Land
        {
            get { return CBB_land.Text; }
            set
            {
                CBB_land.Text = value;
                LBL_land.Content = value;
                LBL_land.Refresh();
            }
        }
        public int Adres_ID
        {
            get { return adres_id; }
            set { adres_id = value; }
        }
        public bool Wijzigstand
        {
            get { return _wijzigstand; }
            set
            {
                _wijzigstand = value;
                if (this.standchanged != null)
                {
                    this.standchanged(value, new EventArgs());
                }
            }
        }
        public bool ViaPostcode
        {
            get { return _viaPostcode; }
            set
            {
                _viaPostcode = value;
                if (value)
                {
                    LBL_modus.Content = "via postcode";
                    if (!RB_via_postcode.IsChecked.Value)
                    {
                        RB_via_postcode.IsChecked = true;
                        RB_handmatig.IsChecked = false;
                    }
                }
                else
                {
                    if (RB_via_postcode.IsChecked.Value)
                    {
                        RB_via_postcode.IsChecked = false;
                        RB_handmatig.IsChecked = true;
                    }
                    LBL_modus.Content = "handmatig";
                }
            }
        }
       
        
        private void Change_mode()
        {
            if (Wijzigstand)
            {
                del_huisnr.Visibility = System.Windows.Visibility.Visible;
                del_postcode.Visibility = System.Windows.Visibility.Visible;
                
                RB_handmatig.Visibility = System.Windows.Visibility.Visible;
                RB_via_postcode.Visibility = System.Windows.Visibility.Visible;

                LBL_huisnummer.Visibility = System.Windows.Visibility.Collapsed;
                LBL_modus.Visibility = System.Windows.Visibility.Collapsed;
                LBL_postcode.Visibility = System.Windows.Visibility.Collapsed;
                LBL_straat.Visibility = System.Windows.Visibility.Collapsed;

                CBB_straat.Visibility = System.Windows.Visibility.Visible;
                

                TB_postcode_cijfers.Visibility = System.Windows.Visibility.Visible;
                TB_postcode_letters.Visibility = System.Windows.Visibility.Visible;
                TB_huisnummer.Visibility = System.Windows.Visibility.Visible;
                if (CBB_straat.Text == "postbus" || CBB_straat.Text == "Postbus")
                {
                    TB_huisnummer_toevoeging.Visibility = Visibility.Collapsed;
                    toevoeging_lbl.Visibility = Visibility.Collapsed;
                }
                else
                {
                    TB_huisnummer_toevoeging.Visibility = Visibility.Visible;
                    toevoeging_lbl.Visibility = Visibility.Visible;
                }
               if (_viaPostcode)
                {
                    LBL_land.Visibility = System.Windows.Visibility.Visible;
                    LBL_plaats.Visibility = System.Windows.Visibility.Visible;
                    
                    CBB_plaats.Visibility = System.Windows.Visibility.Collapsed;
                    CBB_land.Visibility = System.Windows.Visibility.Collapsed;
                    CBB_straat.Visibility = Visibility.Collapsed;
                    LBL_straat.Visibility = Visibility.Visible;

                    TB_postcode_cijfers.IsEnabled = true;
                    TB_postcode_letters.IsEnabled = true;
                    
                }
                else//handmatig
                {
                    LBL_land.Visibility = System.Windows.Visibility.Collapsed;
                    LBL_plaats.Visibility = System.Windows.Visibility.Collapsed;

                    CBB_plaats.Visibility = System.Windows.Visibility.Visible;
                    CBB_land.Visibility = System.Windows.Visibility.Visible;
                    CBB_straat.Visibility = Visibility.Visible;
                    LBL_straat.Visibility = Visibility.Collapsed;
                    if (CBB_plaats.Text != String.Empty)
                    { CBB_straat.IsEnabled = true; }
                    else
                    { CBB_straat.IsEnabled = false; }
                    if (CBB_straat.Text == String.Empty)
                    {
                        TB_postcode_cijfers.IsEnabled = false;
                        TB_postcode_letters.IsEnabled = false;
                    }
                    if (CBB_land.SelectedIndex > 0)
                    {
                        TB_postcode_letters.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        TB_postcode_letters.Visibility = Visibility.Visible;
                    }
                }
            }
        }
        void adres_standchanged(bool wijzig, EventArgs e)
        {
            var d = this.Dispatcher.DisableProcessing();
            if (wijzig)
            {
                postcode_lbl.Content = "Postcode";
                row_left_modus.Visibility = Visibility.Visible;
                row_right_modus.Visibility = Visibility.Visible;
                row_left_huisnummer.Visibility = Visibility.Visible;
                row_right_huisnummer.Visibility = Visibility.Visible;
                row_left_plaats.Visibility = Visibility.Visible;
                row_right_plaats.Visibility = Visibility.Visible;
                if (ISproject)
                {
                    //cropview
                    row_left_straat2.Visibility = Visibility.Visible;
                    row_right_straat2.Visibility = Visibility.Visible;
                    LBL_straat2.Visibility = Visibility.Collapsed;
                    CBB_straat2.Visibility = Visibility.Visible;
                    
                }
                row_left_postcode.Visibility = Visibility.Visible;
                row_right_postcode.Visibility = Visibility.Visible;
                Change_mode();
            }
            else
            {
                LBL_straat.Content =Straat+ " " + LBL_huisnummer.Content;
                
                if (ISproject)
                {
                    LBL_straat2.Visibility = Visibility.Visible;
                    CBB_straat2.Visibility = Visibility.Collapsed;
                    if (CBB_straat2.Text.Length < 1)
                    {
                        row_left_straat2.Visibility = Visibility.Collapsed;
                        row_right_straat2.Visibility = Visibility.Collapsed;
                    }
                }
                if (TB_postcode_cijfers.Text.Length < 1)
                {
                    row_left_postcode.Visibility = Visibility.Collapsed;
                    row_right_postcode.Visibility = Visibility.Collapsed;
                    row_left_plaats.Visibility = Visibility.Visible;
                    row_right_plaats.Visibility = Visibility.Visible;
                }
                else
                {
                    row_left_plaats.Visibility = Visibility.Collapsed;
                    row_right_plaats.Visibility = Visibility.Collapsed;
                    row_left_postcode.Visibility = Visibility.Visible;
                    row_right_postcode.Visibility = Visibility.Visible;
                }
                
                RB_via_postcode.Visibility = System.Windows.Visibility.Collapsed;
                RB_handmatig.Visibility = System.Windows.Visibility.Collapsed;
                CBB_plaats.Visibility = System.Windows.Visibility.Collapsed;
                CBB_land.Visibility = System.Windows.Visibility.Collapsed;
                CBB_straat.Visibility = System.Windows.Visibility.Collapsed;
                TB_postcode_cijfers.Visibility = System.Windows.Visibility.Collapsed;
                TB_postcode_letters.Visibility = System.Windows.Visibility.Collapsed;
                TB_huisnummer_toevoeging.Visibility = System.Windows.Visibility.Collapsed;
                TB_huisnummer.Visibility = System.Windows.Visibility.Collapsed;
                del_huisnr.Visibility = System.Windows.Visibility.Collapsed;
                del_postcode.Visibility = System.Windows.Visibility.Collapsed;
                toevoeging_lbl.Visibility = System.Windows.Visibility.Collapsed;
                //cropview
                row_left_modus.Visibility = Visibility.Collapsed;
                row_right_modus.Visibility = Visibility.Collapsed;
                row_left_huisnummer.Visibility = Visibility.Collapsed;
                row_right_huisnummer.Visibility = Visibility.Collapsed;
                

                LBL_huisnummer.Visibility = System.Windows.Visibility.Visible;
                LBL_land.Visibility = System.Windows.Visibility.Visible;
                LBL_modus.Visibility = System.Windows.Visibility.Visible;
                LBL_plaats.Visibility = System.Windows.Visibility.Visible;
                LBL_postcode.Visibility = System.Windows.Visibility.Visible;
                LBL_straat.Visibility = System.Windows.Visibility.Visible;
                LBL_postcode.Content = TB_postcode_cijfers.Text + TB_postcode_letters.Text;
                if (LBL_postcode.Content.ToString().Length > 2)
                {
                    LBL_postcode.Content += " " + LBL_plaats.Content;
                }
                else
                {
                    LBL_postcode.Content = LBL_plaats.Content;
                }
            }

            
            d.Dispose();
        }
        private string Injectioncheck(string inputSQL)
        {
            string InputSQL = inputSQL;
            if (InputSQL.Length > 0)
            {
                for (int i = 0; i < blackList.Length; i++)
                {
                    if ((InputSQL.IndexOf(blackList[i], StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        //
                        //Handle the discovery of suspicious Sql characters here
                        //
                        MessageBox.Show("Ongeweste tekst of tekens ingevoerd");
                        InputSQL = "";

                    }
                }
                return InputSQL.Replace("'", "''");
            }
            else
            {
                return InputSQL;
            }
        }

		private string Trim4Api(string nm)
		{
			var s = Regex.Split(nm,@"\W\s");
            var extraSplit = s[0].Split('(');

			return extraSplit[0];
		}
        private bool ZoekOpPostcode(string _postcode)
		{
			ServicePointManager.DefaultConnectionLimit = 15;
            bool resultfound = false;
            try
            {
                if (Wijzigstand)
                {
                    _cbbeng.PStraten.Clear();
                    _cbbeng.PSteden.Clear();

                    ////
                    String auth_key = Global.ZES_PP_AUTH_KEY;
                    String nl_sixpp = Trim4Api(_postcode);
                    // Optional parameter
                    String streetnumber = "";
                    // Create the GET request
                    string uri = "http://api.pro6pp.nl/v1/autocomplete";
                    uri += String.Format("?auth_key={0}&nl_sixpp={1}&per_page=25&streetnumber={2}", auth_key, HttpUtility.UrlEncode(nl_sixpp), streetnumber);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Method = "GET";
                    request.ContentType = "text/xml; encoding='utf-8'";
                    request.Timeout = 5000;
                    // Get the response
					using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
					{
						string result = new StreamReader(response.GetResponseStream()).ReadToEnd();

						// Parse the response
						JavaScriptSerializer serializer = new JavaScriptSerializer();
						APIResponse r = serializer.Deserialize<APIResponse>(result);
						if (r.status == "ok")
						{
							resultfound = true;
							Plaats = r.results[0].city;
							foreach (APIResult rs in r.results)
							{
								_cbbeng.PStraten.Add(new StraatItem
								{
									straat = rs.street,
									postcode = rs.nl_sixpp
								});
								_areacode.Push(rs.areacode);
							}
							//Console.WriteLine(String.Format("Success. Street: {0}, city: {1}", r.results[0].street, r.results[0].city));
						}
						else
						{
							resultfound = false;
							Console.WriteLine(String.Format("Error message: {0}", r.error.message));
							if (Wijzigstand)
							{
								this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
								 {

									 if (MessageBox.Show("Geen resultaat gevonden, onbekende postcode.\n Wilt u overschakelen naar handmatig? ", "Handmatig?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
									 {
										 RB_handmatig.IsChecked = true;
									 }
									 else
									 {
										 //
									 }
								 }));
							}
						}
					}
                }
                else
                {
                    resultfound = true;
                }
            }
            catch (WebException we)
            {
                String log_line = "Exception occurred @ " + DateTime.Now.ToString() + " error: " + we;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
            }
            return resultfound;
        }
        private bool ZoekNaarPlaats(string _plaats)
		{
			ServicePointManager.DefaultConnectionLimit = 15;
            bool resultsfound = false;
            try
            {
               if (Wijzigstand)
                {
					if (CBB_land.SelectedIndex == 0)
					{   ///////////////////////////////////////////
						String auth_key = Global.ZES_PP_AUTH_KEY;
						String nl_city =Trim4Api( _plaats);

						// Create the GET request
						string uri = "http://api.pro6pp.nl/v1/suggest";
						uri += String.Format("?auth_key={0}&nl_city={1}&per_page=40", auth_key, HttpUtility.UrlEncode(nl_city));
						HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
						request.Method = "GET";
						request.ContentType = "text/xml; encoding='utf-8'";
						request.Timeout = 5000;
						// Get the response

						using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
						{
							string result = new StreamReader(response.GetResponseStream()).ReadToEnd();

							// Parse the response
							JavaScriptSerializer serializer = new JavaScriptSerializer();
							APIResponse_suggest r = serializer.Deserialize<APIResponse_suggest>(result);

							if (r.status == "ok")
							{
								_cbbeng.PSteden.Clear();
								resultsfound = true;
								if (r.results.Count > 0) 
                                {
                                    if (LoadedCityKey != null)
                                    {
                                        var ckey = r.results.FirstOrDefault(x => x.city_key == LoadedCityKey);
                                        _City_key = ckey != null ? ckey.city_key : "";
                                    }
                                    else
                                    {
                                       _City_key = r.results[0].city_key;
                                    }
                                }
								foreach (APIResult_suggest rs in r.results)
								{
									PlaatsItem pi = new PlaatsItem { city_key = rs.city_key, city = rs.city, official_city = rs.official_city, vierpostcode = rs.nl_fourpps };
									_cbbeng.PSteden.Add(pi);
									//_City_key = rs.city_key;
								}
                                //CBB_plaats.SetBorder(true);
								CBB_plaats.Refresh();
							}
							else
							{
								if (Wijzigstand)
								{
									this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
									{

										MessageBox.Show("Geen plaatsen gevonden, fout in plaatsnaam.");
									}));
								}
								resultsfound = false;
							}

							///////////////////////////////////////////

							CBB_straat.IsEnabled = true;
						}
					}
                }
                else
                {
                    resultsfound = true;
                }
            }
            catch (WebException we)
            {
                String log_line = "Exception occurred @ " + DateTime.Now.ToString() + " error: " + we;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
            }
            return resultsfound;
        }
        private bool ZoekNaarStraat(string _straat)
        {
			ServicePointManager.DefaultConnectionLimit = 15;
            bool resultsfound = false;
            CBB_straat.Background = Brushes.White;
            CBB_straat.Foreground = Brushes.Black;
            try
            {
                if (Wijzigstand)
                {
					if (CBB_land.SelectedIndex == 0)
					{
						///////////////////////////////////////////////////////////////
						String auth_key = Global.ZES_PP_AUTH_KEY;
						String street = _straat;
						String city_key = _City_key;

						// Create the GET request
						string uri = "http://api.pro6pp.nl/v1/suggest";
						uri += String.Format("?auth_key={0}&street={1}&city_key={2}&per_page=200", auth_key, HttpUtility.UrlEncode(street), city_key);
						HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
						request.Method = "GET";
						request.ContentType = "text/xml; encoding='utf-8'";
						request.Timeout = 5000;
						// Get the response
						using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
						{
							string result = new StreamReader(response.GetResponseStream()).ReadToEnd();

							// Parse the response
							JavaScriptSerializer serializer = new JavaScriptSerializer();
							APIResponse_street r = serializer.Deserialize<APIResponse_street>(result);

							if (r.status == "ok")
							{
								_cbbeng.PStraten.Clear();
								resultsfound = true;
								foreach (APIResult_street rs in r.results)
								{
									StraatItem si = new StraatItem { straat = rs.street, postcode = rs.nl_sixpps };
									_cbbeng.PStraten.Add(si);

								}
								if (CBB_straat.IsEnabled)
								{
									CBB_straat.Background = Brushes.White;
									CBB_straat.Foreground = Brushes.Black;
								}
								else
								{
									CBB_straat.Background = Brushes.Gray;
									CBB_straat.Foreground = Brushes.Black;
								}
								CBB_straat.Refresh();
							}
							else
							{
								if (Wijzigstand)
								{
                                    CBB_straat.Background = Brushes.White;
                                    CBB_straat.Foreground = Brushes.Black;
									CBB_straat.Refresh();
								}
								/*this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
								{
                    
									//MessageBox.Show("Geen straten gevonden, fout in straatnaam of straat niet bekend bij deze plaats. ");
								}));*/
								resultsfound = false;
							}
							///////////////////////////////////////////////////////////////
						}
					}
                }
                else
                {
                    resultsfound = true;
                }
            }
            catch (WebException we)
            {
                String log_line = "Exception occurred @ " + DateTime.Now.ToString() + " error: " + we;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
            }
            return resultsfound;
        }


        private void text_veranderd(object sender)
        {
            TextBox tb = sender as TextBox;
            switch (tb.Name)
            {
                case "TB_huisnummer":
                    LBL_huisnummer.Content = Huisnummer + Huisnummer_toevoeging;
                    break;
                case "TB_huisnummer_toevoeging":
                    LBL_huisnummer.Content = Huisnummer + Huisnummer_toevoeging;
                    break;

            }
        }
        private void TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            text_veranderd(sender);
        }
        private void RB_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            switch (rb.Name)
            {
                case "RB_via_postcode":
                    if (!ViaPostcode)
                    {
                        ViaPostcode = true;
                    }
                    if (CBB_land.SelectedIndex != 0)
                    {
                        CBB_land.SelectedIndex = 0;
                        LBL_land.Content = (CBB_land.SelectedItem as ComboBoxItem).Content;
                    }
                    break;
                case "RB_handmatig":
                    if (ViaPostcode)
                    {
                        ViaPostcode = false;
                    }
                    CBB_land.SelectedIndex = 0;
                    if (CBB_straat.IsEditable == false)
                    {
                        CBB_straat.IsEditable = true;
                    }
                    //Land = string.Empty;
                    break;
            }
            if (!starting_up)
            {
                Straat = string.Empty;
                Plaats = string.Empty;
                TB_postcode_cijfers.Items.Clear();
                TB_postcode_letters.Items.Clear();
                TB_postcode_cijfers.Text = string.Empty;
                TB_postcode_letters.Text = string.Empty;

                _cbbeng.PSteden.Clear();
                _cbbeng.PStraten.Clear();
                TB_huisnummer.Clear();
                TB_huisnummer_toevoeging.Clear();
            }
            Change_mode();

        }
        private void TB_postcode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ComboBox tb = sender as ComboBox;


            switch (tb.Name)
            {
                case "TB_postcode_cijfers":
                    TextBox TxtBox = (TextBox)tb.Template.FindName("PART_EditableTextBox", tb);
                    if ((CBB_land.SelectedIndex == 0 || CBB_land.SelectedIndex > 2) && (tb.Text.Length > 3 && TxtBox.SelectedText.Length == 0) && !(e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right))//NL
                    {
                        e.Handled = true;
                        break;
                    }
                    else if (CBB_land.SelectedIndex == 1 && (tb.Text.Length > 3 && TxtBox.SelectedText.Length == 0) && !(e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right))//BE
                    {
                        e.Handled = true;
                        break;
                    }
                    else if (CBB_land.SelectedIndex == 2 && (tb.Text.Length > 4 && TxtBox.SelectedText.Length == 0) && !(e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right))//DE
                    {
                        e.Handled = true;
                        break;
                    }
                    else if ((CBB_land.SelectedIndex < 0 || CBB_land.SelectedIndex > 2) && (tb.Text.Length > 7 && TxtBox.SelectedText.Length == 0) && !(e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right))//other?
                    {
                        e.Handled = true;
                        break;
                    } 
                    if (!(System.Text.RegularExpressions.Regex.IsMatch(e.Key.ToString(), "\\d+")) && !(e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right))
                    {
                        e.Handled = true;
                    }

                    break;
                case "TB_postcode_letters":
                    TxtBox = (TextBox)tb.Template.FindName("PART_EditableTextBox", tb);
                    if ((TxtBox.SelectedText.Length == 0 && tb.Text.Length > 1) && !(e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right))
                    {
                        e.Handled = true;
                        break;
                    }
                    if (!(System.Text.RegularExpressions.Regex.IsMatch(e.Key.ToString(), @"^[a-zA-Z]+$")) && !(e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Left || e.Key == Key.Right))
                    {
                        e.Handled = true;
                    }


                    break;
            }
        }
       
        private void CBB_GotFocus(object sender, RoutedEventArgs e)
        {

            //ComboBox cbb = sender as ComboBox;
            //if (cbb.IsEditable)
            //{
            //    if (cbb.Items.Count == 1)
            //    {
            //        cbb.SelectedIndex = 0;
            //    }
            //    else if (cbb.Items.Count > 1)
            //    {
            //        if (!cbb.IsDropDownOpen)
            //        {
            //            cbb.IsDropDownOpen = true;
            //        }

            //    }
            //}


        }

        private void del_postcode_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TB_postcode_cijfers.Text = String.Empty;
            TB_postcode_letters.Text = String.Empty;
            TB_postcode_cijfers.Items.Clear();
            TB_postcode_letters.Items.Clear();
        }

        private void del_huisnr_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TB_huisnummer.Text = String.Empty;
            TB_huisnummer_toevoeging.Text = String.Empty;

        }

        private void CBB_land_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CBB_land.Text != string.Empty)
            {
                LBL_land.Content = CBB_land.Text;
            }
            
        }

        private void CBB_land_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Wijzigstand)
            {
                if (CBB_land.SelectedIndex != -1)
                {
                    Straat = string.Empty;
                    Plaats = string.Empty;
                    TB_postcode_cijfers.Items.Clear();
                    TB_postcode_letters.Items.Clear();
                    TB_postcode_cijfers.Text = string.Empty;
                    TB_postcode_letters.Text = string.Empty;
                    _cbbeng.PSteden.Clear();
                    _cbbeng.PStraten.Clear();
                }
                if (CBB_land.SelectedIndex > 0)
                {

                    RB_handmatig.IsChecked = true;
                    TB_postcode_letters.Visibility = Visibility.Collapsed;
                }
                else
                {
                    TB_postcode_letters.Visibility = Visibility.Visible;
                }
            }
           
         
        }

        private void CBB_plaats_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (changedFromCode)
            {
                changedFromCode = false;
                return;
            }
            if (CBB_plaats.Text != string.Empty)
            {
                LBL_plaats.Content = CBB_plaats.Text;
                if (CBB_straat.IsEditable == false)
                {
                    CBB_straat.IsEditable = true;
                }
            }
            else
            {
                _cbbeng.PSteden.Clear();
                CBB_plaats.IsDropDownOpen = false;
                CBB_plaats.Refresh();
            }
                if (Wijzigstand)
                {
                    if (!ViaPostcode)//handmatig
                    {
                        if (CBB_land.SelectedIndex == 0)
                        {
                            if (CBB_plaats.Text != string.Empty)
                            {
                                LBL_plaats.Content = CBB_plaats.Text;
                                CBB_straat.IsEnabled = true;
                                if (CBB_plaats.Text.Length > 2 /* && !CBB_plaats.IsDropDownOpen*/)
                                {
                                   
                                    var argus = new List<object>();
                                    TextBox txt = CBB_plaats.Template.FindName("PART_EditableTextBox", CBB_plaats) as TextBox;
                                    if (txt.SelectedText != null && txt.SelectedText.Length > 0 && txt.SelectedText.Length < txt.Text.Length)
                                    {
                                        CBB_plaats.Text = txt.Text.Remove(txt.SelectionStart);
                                        txt.Select(txt.Text.Length, 0);
                                    }
                                    if (txt.Text.Contains("("))
                                    {
                                        CBB_plaats.Text = txt.Text.Remove(txt.Text.IndexOf("("));
                                    }
                                    argus.Add(Injectioncheck(CBB_plaats.Text));
                                    argus.Add(CBB_land.SelectedIndex);
                                    //argus.Add(
                                    var requestPlaats = newRequestPlaats();
                                    //_cbbeng.PSteden.Clear();

                                    if (lastPlaatsWorker != null && lastPlaatsWorker.IsBusy)
                                    {
                                        lastPlaatsWorker.CancelAsync();
                                    }
                                    requestPlaats.RunWorkerAsync(argus);
                                    lastPlaatsWorker = requestPlaats;
                                }
                                else if(CBB_plaats.Text.Length < 4)
                                {
                                    //CBB_plaats.SetBorder(true);
                                    CBB_plaats.Refresh();
                                }

                            }
                            else
                            {
                                _cbbeng.PSteden.Clear();
                                _cbbeng.PStraten.Clear();
                                CBB_straat.IsEnabled = false;
                                CBB_plaats.IsDropDownOpen = false;
                                CBB_plaats.Refresh();
                                TB_postcode_cijfers.IsEnabled = false;
                                TB_postcode_letters.IsEnabled = false;
                                TB_postcode_letters.Items.Clear();
                                TB_postcode_cijfers.Items.Clear();
                                if (lastPlaatsWorker != null && lastPlaatsWorker.IsBusy)
                                {
                                    lastPlaatsWorker.CancelAsync();
                                }
                            }
                        }
                        else
                        {
                            CBB_straat.IsEnabled = true;
                        }
                    }
                }
                else//leesstand
                {

                }
            
        }

        private void CBB_plaats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            changedFromCode = true;
            if (ViaPostcode)
                {
                    if (CBB_plaats.SelectedIndex > -1)
                    {
                        CBB_straat.IsEnabled = true;
                        _City_key = (CBB_plaats.SelectedItem as PlaatsItem).city_key;
                        string[] postcodes = (CBB_plaats.SelectedItem as PlaatsItem).vierpostcode.Split(',');
                        foreach (string pc in postcodes)
                        {
                            TB_postcode_cijfers.Items.Add(pc);
                        }
                       // TB_postcode_cijfers.IsEnabled = false;
                        if (TB_postcode_cijfers.Items.Count == 1)
                        {
                            TB_postcode_cijfers.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                      //tbcijfrsmpty
                    }
                }
                else//handmatig
                {
                    _cbbeng.PStraten.Clear();
                    CBB_straat.Text = String.Empty;
                    if (CBB_plaats.SelectedIndex > -1)
                    {
                        if (CBB_straat.IsEnabled)
                        {
                            CBB_straat.Background = Brushes.White;
                            CBB_straat.Foreground = Brushes.Black;
                        }
                        else
                        {
                            CBB_straat.Background = Brushes.Gray;
                            CBB_straat.Foreground = Brushes.Black;
                        }
                        CBB_straat.Refresh();
                        _City_key = (CBB_plaats.SelectedItem as PlaatsItem).city_key;
                        LoadedCityKey = _City_key;
                        CBB_straat.IsEnabled = true;
                    }
                }

        }

        private void CBB_straat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (changedFromCode)
            {
                changedFromCode = false;
                return;
            }
            if (Wijzigstand)
                {
                    if (ViaPostcode)
                    {
                        if (CBB_straat.Text != string.Empty || CBB_straat.Text.Length < 1)
                        {
                            LBL_straat.Content = CBB_straat.Text;
                            if (CBB_straat.Text == "postbus" || CBB_straat.Text == "Postbus" || (string)huisnummer_postbus_lbl.Content == "postbus" || (string)huisnummer_postbus_lbl.Content == "Postbus")
                            {
                                huisnummer_postbus_lbl.Content = "Postbus nummer";

                                TB_huisnummer_toevoeging.Visibility = System.Windows.Visibility.Collapsed;
                                toevoeging_lbl.Visibility = System.Windows.Visibility.Collapsed;

                            }
                            else
                            {
                                huisnummer_postbus_lbl.Content = "Huisnummer";

                                if (Wijzigstand)
                                {
                                    TB_huisnummer_toevoeging.Visibility = System.Windows.Visibility.Visible;
                                    toevoeging_lbl.Visibility = System.Windows.Visibility.Visible;
                                }
                            }
                        }
                        else
                        {
                            _cbbeng.PStraten.Clear();
                            CBB_straat.IsDropDownOpen = false;
                            
                        }
                    }
                    else//handmatig
                    {
                        if (CBB_land.SelectedIndex == 0)
                        {
                            if (CBB_straat.Text != string.Empty)
                            {
                                TB_postcode_cijfers.IsEnabled = true;
                                TB_postcode_letters.IsEnabled = true;
                                LBL_straat.Content = CBB_straat.Text;
                                if (CBB_straat.Text == "postbus" || CBB_straat.Text == "Postbus" || (string)huisnummer_postbus_lbl.Content == "postbus" || (string)huisnummer_postbus_lbl.Content == "Postbus")
                                {
                                    huisnummer_postbus_lbl.Content = "Postbus nummer";

                                    TB_huisnummer_toevoeging.Visibility = System.Windows.Visibility.Collapsed;
                                    toevoeging_lbl.Visibility = System.Windows.Visibility.Collapsed;

                                }
                                else
                                {
                                    huisnummer_postbus_lbl.Content = "Huisnummer";

                                    if (Wijzigstand)
                                    {
                                        TB_huisnummer_toevoeging.Visibility = System.Windows.Visibility.Visible;
                                        toevoeging_lbl.Visibility = System.Windows.Visibility.Visible;
                                    }
                                }
                                if (CBB_straat.Text.Length > 2 /* && !CBB_straat.IsDropDownOpen*/)
                                {
                                    if (_City_key == String.Empty || _City_key == null)
                                    {
                                        ZoekNaarPlaats(Injectioncheck(CBB_plaats.Text));
                                    }

                                    var argus = new List<object>();
                                    TextBox txt = CBB_straat.Template.FindName("PART_EditableTextBox", CBB_straat) as TextBox;
                                    if (txt.SelectedText != null && txt.SelectedText.Length > 0 && txt.SelectedText.Length < txt.Text.Length)
                                    {
                                        CBB_straat.Text = txt.Text.Remove(txt.SelectionStart);
                                        txt.Select(txt.Text.Length, 0);
                                    }
                                    argus.Add(Injectioncheck(CBB_straat.Text));
                                    argus.Add(CBB_land.SelectedIndex);
                                    var requestStraat = newRequestStraat();

                                    if (lastStraatWorker != null && lastStraatWorker.IsBusy)
                                    {
                                        lastStraatWorker.CancelAsync();
                                    }
                                    requestStraat.RunWorkerAsync(argus);
                                    lastStraatWorker = requestStraat;
                                }
                                //else if(CBB_straat.Text.Length < 4)
                                //{
                                //    if (CBB_straat.IsEnabled)
                                //    {
                                //        CBB_straat.Background = Brushes.White;
                                //        CBB_straat.Foreground = Brushes.Black;
                                //    }
                                //    else
                                //    {
                                //        CBB_straat.Background = Brushes.Gray;
                                //        CBB_straat.Foreground = Brushes.Black;
                                //    }
                                //    CBB_straat.Refresh();
                                //}

                            }
                            else
                            {
                                _cbbeng.PStraten.Clear();
                                CBB_straat.IsDropDownOpen = false;
                                CBB_straat.Refresh();
                                TB_postcode_cijfers.IsEnabled = false;
                                
                                TB_postcode_cijfers.Items.Clear();
                                TB_postcode_cijfers.Text = String.Empty;
                                TB_postcode_letters.IsEnabled = false;
                                TB_postcode_letters.Items.Clear();
                                TB_postcode_letters.Text = String.Empty;
                                if (lastStraatWorker != null && lastStraatWorker.IsBusy)
                                {
                                    lastStraatWorker.CancelAsync();
                                }
                            }
                        }
                        else
                        {
                            TB_postcode_cijfers.IsEnabled = true;
                            TB_postcode_letters.IsEnabled = true;

                        }
                    }
                }
                else//leesstand
                {

                }
            
        }

        private void CBB_straat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            changedFromCode = true;
            if (CBB_straat.SelectedIndex > -1)
            {
                TB_postcode_letters.IsEnabled = true;
                if (!_viaPostcode)
                {
                    TB_postcode_letters.Text = String.Empty;
                    TB_postcode_cijfers.Text = String.Empty;
                    TB_postcode_cijfers.Items.Clear();
                    TB_postcode_letters.Items.Clear();
                    if (CBB_straat.SelectedIndex > -1 && CBB_straat.SelectedItem != null && CBB_straat.SelectedItem.GetType() == typeof(StraatItem) && (CBB_straat.SelectedItem as StraatItem).postcode != String.Empty && (CBB_straat.SelectedItem as StraatItem).postcode != null)
                    {

                        string[] postcodes_vol = (CBB_straat.SelectedItem as StraatItem).postcode.Split(',');


                        foreach (string pc_vol in postcodes_vol)
                        {
                            bool uniek = true;
                            if (pc_vol.Length > 3)
                            {
                                foreach (string cbi in TB_postcode_cijfers.Items)
                                {
                                    if (cbi == pc_vol.Substring(0, 4))
                                    {
                                        uniek = false;
                                    }
                                }
                                if (uniek)
                                {
                                    TB_postcode_cijfers.Items.Add(pc_vol.Substring(0, 4));
                                }
                                TB_postcode_letters.Items.Add(pc_vol.Substring(4, 2));
                            }
                        }
                    }
                }
                if (TB_postcode_cijfers.Items.Count == 1)
                {
                    TB_postcode_cijfers.SelectedIndex = 0;
                }
                if (TB_postcode_letters.Items.Count == 1)
                {
                    TB_postcode_letters.SelectedIndex = 0;
                }
                if (CBB_straat.Text == "postbus" || CBB_straat.Text == "Postbus" || (string)huisnummer_postbus_lbl.Content == "postbus" || (string)huisnummer_postbus_lbl.Content == "Postbus")
                {
                    huisnummer_postbus_lbl.Content = "Postbus nummer";

                    TB_huisnummer_toevoeging.Visibility = System.Windows.Visibility.Collapsed;
                    toevoeging_lbl.Visibility = System.Windows.Visibility.Collapsed;

                }
                else
                {
                    huisnummer_postbus_lbl.Content = "Huisnummer";

                    if (Wijzigstand)
                    {
                        TB_huisnummer_toevoeging.Visibility = System.Windows.Visibility.Visible;
                        toevoeging_lbl.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }
            else
            {
                if (!_viaPostcode)
                {
                    TB_postcode_letters.Text = String.Empty;
                    TB_postcode_cijfers.Text = String.Empty;
                    TB_postcode_cijfers.Items.Clear();
                    TB_postcode_letters.Items.Clear();
                }//TB_postcode_letters.IsReadOnly = false;
            }
               
               
           
        }

        private void TB_postcode_cijfers_TextChanged(object sender, TextChangedEventArgs e)
        {
           if (Wijzigstand)
                {
                    if (ViaPostcode)
                    {
                        if (TB_postcode_cijfers.Text != String.Empty || TB_postcode_cijfers.Text.Length > 0)
                        {
                            LBL_postcode.Content = Postcode_cijfers + Postcode_letters;
                            if (TB_postcode_cijfers.Text.Length == 4 && TB_postcode_letters.Text.Length == 2)
                            {
                                if (ZoekOpPostcode(Injectioncheck(TB_postcode_cijfers.Text + TB_postcode_letters.Text)))
                                {

                                    if (CBB_straat.Items.Count == 1)
                                    {
                                       // CBB_straat.DisplayMemberPath = "straat";
                                        CBB_straat.SelectedIndex = 0;
                                        CBB_straat.Visibility = System.Windows.Visibility.Collapsed;
                                        LBL_straat.Visibility = System.Windows.Visibility.Visible;
                                        LBL_straat.Content = (CBB_straat.SelectedItem as StraatItem).straat.ToString();
                                        CBB_straat.IsEditable = true;
                                    }
                                    else
                                    {
                                        if (Wijzigstand)
                                        {
                                            CBB_straat.IsEditable = false;
                                            CBB_straat.Visibility = System.Windows.Visibility.Visible;
                                            LBL_straat.Visibility = System.Windows.Visibility.Collapsed;
                                        }
                                    }
                                }
                                if (CBB_straat.Text == "postbus" || CBB_straat.Text == "Postbus" || (string)huisnummer_postbus_lbl.Content == "postbus" || (string)huisnummer_postbus_lbl.Content == "Postbus")
                                {
                                    huisnummer_postbus_lbl.Content = "Postbus nummer";
                                    TB_huisnummer_toevoeging.Visibility = System.Windows.Visibility.Collapsed;
                                    toevoeging_lbl.Visibility = System.Windows.Visibility.Collapsed;
                                }
                                else
                                {
                                    huisnummer_postbus_lbl.Content = "Huisnummer";
                                    if (Wijzigstand)
                                    {
                                        TB_huisnummer_toevoeging.Visibility = System.Windows.Visibility.Visible;
                                        toevoeging_lbl.Visibility = Visibility.Visible;
                                    }
                                }

                            }
                        }
                        else
                        {
                            _cbbeng.PStraten.Clear();
                            _cbbeng.PSteden.Clear();
                            TB_postcode_cijfers.Items.Clear();
                            TB_postcode_letters.Text = String.Empty;
                            TB_postcode_letters.Items.Clear();
                        }
                    }
                    else//handmatig
                    {
                        LBL_postcode.Content = Postcode_cijfers + Postcode_letters;
                        if (TB_postcode_cijfers.Text.Length > 0)
                        {
                            TB_postcode_letters.IsEnabled = true;
                        }
                        else
                        {

                        }
                    }
                }
                else//leesstand
                {

                }
            
        }

        private void TB_postcode_letters_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Wijzigstand)
            {
                if (ViaPostcode)
                {
                    if (!String.IsNullOrEmpty(TB_postcode_letters.Text))
                    {
                      //  TB_postcode_letters.Text = TB_postcode_letters.Text.ToUpper();
                        TextBox txt = TB_postcode_letters.Template.FindName("PART_EditableTextBox", TB_postcode_letters) as TextBox;
                        if (txt != null)
                        {
                            txt.Select(txt.Text.Length, 0);
                        }
                        LBL_postcode.Content = Postcode_cijfers + Postcode_letters;
                        if (TB_postcode_cijfers.Text.Length == 4 && TB_postcode_letters.Text.Length == 2)
                        {
                            if (ZoekOpPostcode(Injectioncheck(TB_postcode_cijfers.Text + TB_postcode_letters.Text)))
                            {
                                if (CBB_straat.Items.Count == 1)
                                {
                                    // CBB_straat.DisplayMemberPath = "straat";
                                    CBB_straat.SelectedIndex = 0;
                                    CBB_straat.Visibility = System.Windows.Visibility.Collapsed;
                                    LBL_straat.Visibility = System.Windows.Visibility.Visible;
                                    LBL_straat.Content = (CBB_straat.SelectedItem as StraatItem).straat.ToString();
                                    CBB_straat.IsEditable = true;
                                }
                                else if (CBB_straat.Items.Count > 1)
                                {
                                    if (Wijzigstand)
                                    {
                                        CBB_straat.IsEditable = false;
                                        CBB_straat.Visibility = System.Windows.Visibility.Visible;
                                        LBL_straat.Visibility = System.Windows.Visibility.Collapsed;
                                    }
                                }
                            }
                            if (CBB_straat.Text == "postbus" || CBB_straat.Text == "Postbus" || (string)huisnummer_postbus_lbl.Content == "postbus" || (string)huisnummer_postbus_lbl.Content == "Postbus")
                            {
                                huisnummer_postbus_lbl.Content = "Postbus nummer";
                                TB_huisnummer_toevoeging.Visibility = System.Windows.Visibility.Collapsed;
                                toevoeging_lbl.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                huisnummer_postbus_lbl.Content = "Huisnummer";
                                if (Wijzigstand)
                                {
                                    TB_huisnummer_toevoeging.Visibility = System.Windows.Visibility.Visible;
                                    toevoeging_lbl.Visibility = Visibility.Visible;
                                }
                            }

                        }
                    }
                    else
                    {
                        _cbbeng.PStraten.Clear();
                        _cbbeng.PSteden.Clear();
                        TB_postcode_letters.Items.Clear();
                    }
                }
                else//handmatig
                {
                    TB_postcode_letters.Text = TB_postcode_letters.Text.ToUpper();
                    TextBox txt = TB_postcode_letters.Template.FindName("PART_EditableTextBox", TB_postcode_letters) as TextBox;
                    if (txt != null)
                    {
                        txt.Select(txt.Text.Length, 0);
                    }
                    LBL_postcode.Content = Postcode_cijfers + Postcode_letters;
                    if (TB_postcode_letters.Text.Length > 0)
                    {

                    }
                    else
                    {

                    }
                }
            }
            else//leesstand
            {

            }

        }

        private void CBB_straat2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (CBB_straat2.Text != String.Empty)
            {
                LBL_straat2.Content = CBB_straat2.Text;
            }
        }

        private void TB_huisnummer_toevoeging_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(System.Text.RegularExpressions.Regex.IsMatch(e.Key.ToString(), @"\d") && TB_huisnummer_toevoeging.Text.Length == 0)
            {
                TB_huisnummer_toevoeging.Text = "-";
                TB_huisnummer_toevoeging.SelectionStart = TB_huisnummer_toevoeging.Text.Length;
            }
        }
        bool selecta = false;
        private void TB_huisnummer_GotFocus(object sender, RoutedEventArgs e)
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
        }

        private void TB_huisnummer_toevoeging_LostFocus(object sender, RoutedEventArgs e)
        {
            selecta = false;
        }

        private void TB_huisnummer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (t.SelectedText.Length != t.Text.Length)
            {
                t.SelectAll();

                t.ReleaseMouseCapture();

                e.Handled = true;
            }
        }

        private void CBB_plaats_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (CBB_plaats.Items.Count > 0)
            {
                TextBox txt = CBB_plaats.Template.FindName("PART_EditableTextBox", CBB_plaats) as TextBox;
                if (txt.SelectedText.Length > 0)
                {
                    int indx = txt.Text.Length - txt.SelectedText.Length;
                    txt.Text =  txt.Text.Remove(indx);
                    txt.SelectionStart = txt.Text.Length;
                }
                
            }
        }

        private void CBB_straat_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (CBB_straat.Items.Count > 0)
            {
                TextBox txt = CBB_straat.Template.FindName("PART_EditableTextBox", CBB_straat) as TextBox;
                if (txt.SelectedText.Length > 0)
                {
                    int indx = txt.Text.Length - txt.SelectedText.Length;
                    txt.Text = txt.Text.Remove(indx);
                    txt.SelectionStart = txt.Text.Length;
                }
            }
        }

        private void CBB_LostFocus(object sender, RoutedEventArgs e)
        {
            ComboBox cbb = sender as ComboBox;
            bool resolved = false;
            if (cbb.Name == "CBB_plaats")
            {
                if (cbb.Items.Count == 1)
                {
                    var item = cbb.Items[0] as PlaatsItem;
                    if (item.city.ToLower().StartsWith(cbb.Text.ToLower()))
                    {
                        cbb.SelectedIndex = 0;
                        resolved = true;
                    }
                }

                if (! resolved && !String.IsNullOrEmpty(cbb.Text) && cbb.SelectedIndex == -1)
                {
                    MessageBox.Show("Onbekende plaatsnaam");
                }
            }
            else if (cbb.Name == "CBB_straat")
            {
                if (cbb.Items.Count == 1)
                {
                    var item = cbb.Items[0] as StraatItem;
                    if (item.straat.ToLower().StartsWith(cbb.Text.ToLower()))
                    {
                        cbb.SelectedIndex = 0;
                        resolved = true;
                    }
                }

                if (!resolved && !String.IsNullOrEmpty(cbb.Text) && cbb.SelectedIndex == -1)
                {
                    MessageBox.Show("Onbekende straatnaan");
                }
            }
        }

        private void CBB_straat_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (CBB_straat.IsEnabled)
            {
                CBB_straat.Background = Brushes.White;
                CBB_straat.Foreground = Brushes.Black;
            }
            else
            {
                CBB_straat.Background = Brushes.LightGray;
                CBB_straat.Foreground = Brushes.Black;
            }
        }

        private void TB_postcode_letters_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox txt = TB_postcode_letters.Template.FindName("PART_EditableTextBox", TB_postcode_letters) as TextBox;
            if (txt != null)
            {
                txt.CharacterCasing = CharacterCasing.Upper;
            }
        }

        private void adres_box_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Refresh();
            //MessageBox.Show("first");
        }

        private void CBB_plaats_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                CBB_plaats.Focus();
            }
        }

       



    }
    public class ComboEngine:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<PlaatsItem> Steden = new ObservableCollection<PlaatsItem>();
        public ObservableCollection<PlaatsItem> PSteden
        {
            get { return Steden; }
            set { Steden = value; NotifyPropertyChanged("PSteden"); }
        }
        private ObservableCollection<StraatItem> Straten = new ObservableCollection<StraatItem>();
        public ObservableCollection<StraatItem> PStraten
        {
            get { return Straten; }
            set { Straten = value; NotifyPropertyChanged("PStraten"); }
        }
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
    public class PlaatsItem
    {
        public string city { get; set; }
        public string city_key { get; set; }
        public string official_city { get; set; }
        public string vierpostcode { get; set; }
    }
    public class StraatItem
    {
        public string straat { get; set; }
        public string postcode { get; set; }
    }

    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate() { };

        public static void Refresh(this UIElement uiElement)
        {
            try
            {
                uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
            }
            catch (InvalidOperationException e)
            {
                String log_line = "Exception occurred @ " + DateTime.Now.ToString() + " error: " + e;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
            }
        }
    }
}

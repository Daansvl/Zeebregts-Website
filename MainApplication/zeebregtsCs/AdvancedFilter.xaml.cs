using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Windows.Automation.Peers;
using zeebregtsCs.usercontrols;


namespace zeebregtsCs
{
    /// <summary>
    /// Interaction logic for AdvancedFilter.xaml
    /// </summary>
    public partial class AdvancedFilter : UserControl
    {
        MenuEngine _menuEngine = new MenuEngine();
        bool[] Advanched = {false,false, false, false};
        bool logged = false;
        public base_form start_parent;
        public base_form close_parent;
        BackgroundWorker tmpbw;
        overview1 listv;
        DataTable _dt;
        System.Windows.Forms.DataGridView dgv1;
        AdvFilterDataset.bedrijvenlijstDataTable bdrlijsttabel;
        AdvFilterDataset.personenlijstDataTable perslijsttabel;
        int[] speciaalfilter = {-1,-1,-1,-1};
        int[] functie_regel_nrs = { -1, -1, -1, -1 };
        bool[] regels = { false, false, false, false };
        bool[] isbedrijf = { false, false, false, false };
		bool[] IsGelijkFilter = { false, false, false, false };
        KeyValuePair<int, bool>[] afgl_functies = new KeyValuePair<int, bool>[4];
        string[] kolnamen = new string[4];
        string[] zoektermen = new string[4];
        int type = 0;
        public AdvancedFilter()
        {
            InitializeComponent();
            AdvBtn2.Visibility = System.Windows.Visibility.Hidden;
            AdvBtn3.Visibility = System.Windows.Visibility.Hidden;
            AdvBtn4.Visibility = System.Windows.Visibility.Hidden;
            _menuEngine = (MenuEngine)this.DataContext;
            logged = false;
        }
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            if ((worker.CancellationPending == true))
            {
                e.Cancel = true;
                
            }
            else
            {
               AdvFilterQRYbuilder AdvFiltQRYBuildr = new AdvFilterQRYbuilder();
                DataTable dt = AdvFiltQRYBuildr.start_building(type, regels, zoektermen, kolnamen, Advanched, speciaalfilter, isbedrijf, functie_regel_nrs,afgl_functies, IsGelijkFilter);
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                }
                else
                {
                    _dt = dt;
                }
            }
        }
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            bw.Dispose();
            zoekklaar();
        }
        private void zoekklaar()
        {
            if (!lijst_is_leeg)
            {
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        listv.ChangeDatasource(_dt);
                    }
                    else
                    {
                        listv.ChangeDatasource(null);
                    }
                }
                else
                {
                    listv.ChangeDatasource(null);
                }
            }
            
            
        }
        public void linkbtnclicked(multibox mb, int tp)
        {
            string zknm = String.Empty;
            switch(mb.Name)
            {
                case "fase3mcbb1":
                    zknm = fase2cbb1.Text;
                    break;
                case "fase3mcbb2":
                    zknm = fase2cbb2.Text;
                    break;
                case "fase3mcbb3":
                    zknm = fase2cbb3.Text;
                    break;
                case "fase3mcbb4":
                    zknm = fase2cbb4.Text;
                    break;
            }
			
            overview1 ov = new overview1(mb, tp, start_parent, close_parent,zknm);
			ov.StartPosition = System.Windows.Forms.FormStartPosition.Manual; 
			ov.WindowState = start_parent.WindowState;
			if (start_parent.WindowState != System.Windows.Forms.FormWindowState.Maximized)
			{
				ov.Size = start_parent.Size;
				ov.Location = start_parent.Location;
			}
            ov.Show();
            start_parent.Hide();
            

        }
        public void set_startNcloseParent(base_form startparent, base_form closeparent)
        {
            start_parent = startparent;
            close_parent = closeparent;
        }
        public void back2default()
        {
            quickcheck1.IsChecked = false;
            quickcheck2.IsChecked = false;
            quickcheck3.IsChecked = false;
            quickcheck4.IsChecked = false;
            if (Quicktb4.Visibility == System.Windows.Visibility.Visible)
            {
                AdvBtn4.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            if (Quicktb3.Visibility == System.Windows.Visibility.Visible)
            {
                AdvBtn3.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            if (Quicktb2.Visibility == System.Windows.Visibility.Visible)
            {
                AdvBtn2.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
            Quicktb1.Text = string.Empty;
        }
        public AdvFilterDataset.bedrijvenlijstDataTable kies_bedrijvenlijst()
        {
           AdvFilterDatasetTableAdapters.bedrijvenlijstTableAdapter bdrlijstadapt = new AdvFilterDatasetTableAdapters.bedrijvenlijstTableAdapter();
           bdrlijstadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            AdvFilterDataset.bedrijvenlijstDataTable bdrlijsttabel = new AdvFilterDataset.bedrijvenlijstDataTable();
            
            switch (type)
            {
                case 1:
                    bdrlijsttabel = bdrlijstadapt.GetData();
                    break;
                case 3:
                    bdrlijsttabel = bdrlijstadapt.GetDatavoorpers();
                    break;

            }
            
            return bdrlijsttabel;
        }
        public AdvFilterDataset.personenlijstDataTable kies_personenlijst()
        {
            AdvFilterDatasetTableAdapters.personenlijstTableAdapter perslijstadapt = new AdvFilterDatasetTableAdapters.personenlijstTableAdapter();
            perslijstadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            AdvFilterDataset.personenlijstDataTable perslijsttabel = new AdvFilterDataset.personenlijstDataTable();
             return perslijsttabel = perslijstadapt.GetData();
        
        }
        private DataTable Get_afgl_tbl()
        {
            
            if (type == 2)
            {
                AdvFilterDatasetTableAdapters.bedrijvenlijstTableAdapter bdrlijstadapt = new AdvFilterDatasetTableAdapters.bedrijvenlijstTableAdapter();
                bdrlijstadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
                AdvFilterDataset.bedrijvenlijstDataTable bdrlijsttabel = new AdvFilterDataset.bedrijvenlijstDataTable();
                bdrlijstadapt.Fill_afgl(bdrlijsttabel);
                return bdrlijsttabel;
            }
            else if (type == 3)
            {
                AdvFilterDatasetTableAdapters.personenlijstTableAdapter perslijstadapt = new AdvFilterDatasetTableAdapters.personenlijstTableAdapter();
                perslijstadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
                AdvFilterDataset.personenlijstDataTable perslijsttabel = new AdvFilterDataset.personenlijstDataTable();
                perslijstadapt.Fill_afgl(perslijsttabel);
                return perslijsttabel;
            }
            else
            {
                return null;
            }
            
        }
        private void Fase1Select(object sender, RoutedEventArgs e)
        {
            
            FrameworkElement feSource = e.Source as FrameworkElement;
            switch (feSource.Name)
            {
                case "fase1cbb1":
                    fase2cbb1.Visibility = System.Windows.Visibility.Hidden;
                    fase2cbb1.SelectedIndex = -1;
                    fase3cbb1.Visibility = System.Windows.Visibility.Hidden;
                    fase3cbb1.SelectedIndex = -1;
                    fase3mcbb1.Visibility = System.Windows.Visibility.Hidden;
                    fase3tbb1.Text = "";
                    fase3tbb1.Visibility = System.Windows.Visibility.Hidden;
                    fase3ckb1J.Visibility = System.Windows.Visibility.Hidden;
                    fase3ckb1N.Visibility = System.Windows.Visibility.Hidden;
                    regels[0] = false;
                    break;
                case "fase1cbb2":
                    fase2cbb2.Visibility = System.Windows.Visibility.Hidden;
                    fase2cbb2.SelectedIndex = -1;
                    fase3cbb2.Visibility = System.Windows.Visibility.Hidden;
                    fase3cbb2.SelectedIndex = -1;
                    fase3mcbb2.Visibility = System.Windows.Visibility.Hidden;
                    fase3tbb2.Text = "";
                    fase3tbb2.Visibility = System.Windows.Visibility.Hidden;
                    fase3ckb2J.Visibility = System.Windows.Visibility.Hidden;
                    fase3ckb2N.Visibility = System.Windows.Visibility.Hidden;
                    regels[1] = false;
                    break;
                case "fase1cbb3":
                    fase2cbb3.Visibility = System.Windows.Visibility.Hidden;
                    fase2cbb3.SelectedIndex = -1;
                    fase3cbb3.Visibility = System.Windows.Visibility.Hidden;
                    fase3cbb3.SelectedIndex = -1;
                    fase3mcbb3.Visibility = System.Windows.Visibility.Hidden;
                    fase3tbb3.Text = "";
                    fase3tbb3.Visibility = System.Windows.Visibility.Hidden;
                    fase3ckb3J.Visibility = System.Windows.Visibility.Hidden;
                    fase3ckb3N.Visibility = System.Windows.Visibility.Hidden;
                    regels[2] = false;
                    break;
                case "fase1cbb4":
                    fase2cbb4.Visibility = System.Windows.Visibility.Hidden;
                    fase2cbb4.SelectedIndex = -1;
                    fase3cbb4.Visibility = System.Windows.Visibility.Hidden;
                    fase3cbb4.SelectedIndex = -1;
                    fase3mcbb4.Visibility = System.Windows.Visibility.Hidden;
                    fase3tbb4.Text = "";
                    fase3tbb4.Visibility = System.Windows.Visibility.Hidden;
                    fase3ckb4J.Visibility = System.Windows.Visibility.Hidden;
                    fase3ckb4N.Visibility = System.Windows.Visibility.Hidden;
                    regels[3] = false;
                    break;
            }
            if (sender is ComboBox)
            {
                if ((sender as ComboBox).SelectedIndex > -1)
                {
                    ComboBox tmpcbb = (ComboBox)sender;
                    int tmpwaarde = (int)tmpcbb.SelectedValue;
                    ObservableCollection<MenuItem> target = new ObservableCollection<MenuItem>();
                    switch (tmpwaarde)
                    {
                        case 1:
                            //
                            target = _menuEngine.Fase2_1;
                            switch (feSource.Name)
                            {
                                case "fase1cbb1":
                                    //
                                    fase3cbb1.Visibility = System.Windows.Visibility.Hidden;
                                    fase3tbb1.Visibility = System.Windows.Visibility.Hidden;
                                    fase3mcbb1.reset();
                                    fase2cbb1.Visibility = System.Windows.Visibility.Visible;
                                    fase2cbb1.ItemsSource = target;
                                    fase2cbb1.DisplayMemberPath = "Text";
                                    fase2cbb1.SelectedValuePath = "Waarde";
                                    //fase2cbb1.IsDropDownOpen = true;
                                    break;
                                case "fase1cbb2":
                                    //
                                    fase3cbb2.Visibility = System.Windows.Visibility.Hidden;
                                    fase3tbb2.Visibility = System.Windows.Visibility.Hidden;
                                    fase3mcbb2.reset();
                                    fase2cbb2.Visibility = System.Windows.Visibility.Visible;
                                    fase2cbb2.ItemsSource = target;
                                    fase2cbb2.DisplayMemberPath = "Text";
                                    fase2cbb2.SelectedValuePath = "Waarde";
                                  //  fase2cbb2.IsDropDownOpen = true;
                                    break;
                                case "fase1cbb3":
                                    //
                                    fase3cbb3.Visibility = System.Windows.Visibility.Hidden;
                                    fase3tbb3.Visibility = System.Windows.Visibility.Hidden;
                                    fase3mcbb3.reset();
                                    fase2cbb3.Visibility = System.Windows.Visibility.Visible;
                                    fase2cbb3.ItemsSource = target;
                                    fase2cbb3.DisplayMemberPath = "Text";
                                    fase2cbb3.SelectedValuePath = "Waarde";
                                 //   fase2cbb3.IsDropDownOpen = true;
                                    break;
                                case "fase1cbb4":
                                    //
                                    fase3cbb4.Visibility = System.Windows.Visibility.Hidden;
                                    fase3tbb4.Visibility = System.Windows.Visibility.Hidden;
                                    fase3mcbb4.reset();
                                    fase2cbb4.Visibility = System.Windows.Visibility.Visible;
                                    fase2cbb4.ItemsSource = target;
                                    fase2cbb4.DisplayMemberPath = "Text";
                                    fase2cbb4.SelectedValuePath = "Waarde";
                             //       fase2cbb4.IsDropDownOpen = true;
                                    break;
                            }
                            break;
                        case 2:
                            //
                            AdvFilterDatasetTableAdapters.statusTableAdapter statTableAdapter = new AdvFilterDatasetTableAdapters.statusTableAdapter();
                            statTableAdapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
                            AdvFilterDataset.statusDataTable statDataTable = new AdvFilterDataset.statusDataTable();
                            statTableAdapter.Fill(statDataTable);
                            switch (feSource.Name)
                            {
                                case "fase1cbb1":
                                    fase2cbb1.Visibility = System.Windows.Visibility.Hidden;
                                    fase2cbb1.SelectedIndex = -1;
                                    fase2cbb1.Text = "";
                                    fase3cbb1.Visibility = System.Windows.Visibility.Visible;
                                    fase3cbb1.ItemsSource = statDataTable;
                                    fase3cbb1.DisplayMemberPath = "omschrijving";
                                    fase3cbb1.SelectedValuePath = "omschrijving_nr";
                              //      fase3cbb1.IsDropDownOpen = true;

                                    break;
                                case "fase1cbb2":
                                    fase2cbb2.Visibility = System.Windows.Visibility.Hidden;
                                    fase2cbb2.SelectedIndex = -1;
                                    fase2cbb2.Text = "";
                                    fase3cbb2.Visibility = System.Windows.Visibility.Visible;
                                    fase3cbb2.ItemsSource = statDataTable;
                                    fase3cbb2.DisplayMemberPath = "omschrijving";
                                    fase3cbb2.SelectedValuePath = "omschrijving_nr";
                             //       fase3cbb2.IsDropDownOpen = true;
                                    break;
                                case "fase1cbb3":
                                    fase2cbb3.Visibility = System.Windows.Visibility.Hidden;
                                    fase2cbb3.SelectedIndex = -1;
                                    fase2cbb3.Text = "";
                                    fase3cbb3.Visibility = System.Windows.Visibility.Visible;
                                    fase3cbb3.ItemsSource = statDataTable;
                                    fase3cbb3.DisplayMemberPath = "omschrijving";
                                    fase3cbb3.SelectedValuePath = "omschrijving_nr";
                            //        fase3cbb3.IsDropDownOpen = true;
                                    break;
                                case "fase1cbb4":
                                    fase2cbb4.Visibility = System.Windows.Visibility.Hidden;
                                    fase2cbb4.SelectedIndex = -1;
                                    fase2cbb4.Text = "";
                                    fase3cbb4.Visibility = System.Windows.Visibility.Visible;
                                    fase3cbb4.ItemsSource = statDataTable;
                                    fase3cbb4.DisplayMemberPath = "omschrijving";
                                    fase3cbb4.SelectedValuePath = "omschrijving_nr";
                            //        fase3cbb4.IsDropDownOpen = true;
                                    break;
                            }
                            break;
                        case 3:
                            //
                             perslijsttabel = kies_personenlijst();

                            switch (feSource.Name)
                            {
                                case "fase1cbb1":
                                    fase2cbb1.Visibility = System.Windows.Visibility.Visible;
                                    fase2cbb1.SelectedIndex = -1;
                                    fase2cbb1.Text = "";
                                    fase2cbb1.ItemsSource = perslijsttabel;
                                    fase2cbb1.DisplayMemberPath = "weergavenaam";
                                    fase2cbb1.SelectedValuePath = "tmpnr";
                                    break;
                                case "fase1cbb2":
                                    fase2cbb2.Visibility = System.Windows.Visibility.Visible;
                                    fase2cbb2.SelectedIndex = -1;
                                    fase2cbb2.Text = "";
                                    fase2cbb2.ItemsSource = perslijsttabel;
                                    fase2cbb2.DisplayMemberPath = "weergavenaam";
                                    fase2cbb2.SelectedValuePath = "tmpnr";
                             //       fase2cbb2.IsDropDownOpen = true;
                                    break;
                                case "fase1cbb3":
                                    fase2cbb3.Visibility = System.Windows.Visibility.Visible;
                                    fase2cbb3.SelectedIndex = -1;
                                    fase2cbb3.Text = "";
                                    fase2cbb3.ItemsSource = perslijsttabel;
                                    fase2cbb3.DisplayMemberPath = "weergavenaam";
                                    fase2cbb3.SelectedValuePath = "tmpnr";
                             //       fase2cbb3.IsDropDownOpen = true;
                                    break;
                                case "fase1cbb4":
                                    fase2cbb4.Visibility = System.Windows.Visibility.Visible;
                                    fase2cbb4.SelectedIndex = -1;
                                    fase2cbb4.Text = "";
                                    fase2cbb4.ItemsSource = perslijsttabel;
                                    fase2cbb4.DisplayMemberPath = "weergavenaam";
                                    fase2cbb4.SelectedValuePath = "tmpnr";
                               //     fase2cbb4.IsDropDownOpen = true;
                                    break;
                            }
                            break;
                        case 4:
                            //
                            bdrlijsttabel = kies_bedrijvenlijst();
                            switch (feSource.Name)
                            {
                                case "fase1cbb1":
                                    fase2cbb1.Visibility = System.Windows.Visibility.Visible;
                                    fase2cbb1.SelectedIndex = -1;
                                    fase2cbb1.Text = "";
                                    fase2cbb1.ItemsSource = bdrlijsttabel;
                                    fase2cbb1.DisplayMemberPath = "weergavenaam";
                                    fase2cbb1.SelectedValuePath = "tmpnr";
                             //       fase2cbb1.IsDropDownOpen = true;
                                    break;
                                case "fase1cbb2":
                                    fase2cbb2.Visibility = System.Windows.Visibility.Visible;
                                    fase2cbb2.SelectedIndex = -1;
                                    fase2cbb2.Text = "";
                                    fase2cbb2.ItemsSource = bdrlijsttabel;
                                    fase2cbb2.DisplayMemberPath = "weergavenaam";
                                    fase2cbb2.SelectedValuePath = "tmpnr";
                            //        fase2cbb2.IsDropDownOpen = true;
                                    break;
                                case "fase1cbb3":
                                    fase2cbb3.Visibility = System.Windows.Visibility.Visible;
                                    fase2cbb3.SelectedIndex = -1;
                                    fase2cbb3.Text = "";
                                    fase2cbb3.ItemsSource = bdrlijsttabel;
                                    fase2cbb3.DisplayMemberPath = "weergavenaam";
                                    fase2cbb3.SelectedValuePath = "tmpnr";
                             //       fase2cbb3.IsDropDownOpen = true;
                                    break;
                                case "fase1cbb4":
                                    fase2cbb4.Visibility = System.Windows.Visibility.Visible;
                                    fase2cbb4.SelectedIndex = -1;
                                    fase2cbb4.Text = "";
                                    fase2cbb4.ItemsSource = bdrlijsttabel;
                                    fase2cbb4.DisplayMemberPath = "weergavenaam";
                                    fase2cbb4.SelectedValuePath = "tmpnr";
                                 //   fase2cbb4.IsDropDownOpen = true;
                                    break;
                            }
                            break;
                        case 5:
                            switch (feSource.Name)
                            {
                                case "fase1cbb1":
                                    fase2cbb1.Visibility = System.Windows.Visibility.Hidden;
                                    fase2cbb1.SelectedIndex = -1;
                                    fase2cbb1.Text = "";
                                    fase3cbb1.Visibility = System.Windows.Visibility.Visible;
                                    fase3cbb1.ItemsSource = Get_afgl_tbl().AsEnumerable();
                                    fase3cbb1.DisplayMemberPath = "weergavenaam";
                                    fase3cbb1.SelectedValuePath = "tmpnr";
                                    fase3cbb1.Focus();
                                    break;
                                case "fase1cbb2":
                                    fase2cbb2.Visibility = System.Windows.Visibility.Hidden;
                                    fase2cbb2.SelectedIndex = -1;
                                    fase2cbb2.Text = "";
                                    fase3cbb2.Visibility = System.Windows.Visibility.Visible;
                                    fase3cbb2.ItemsSource = Get_afgl_tbl().AsEnumerable();
                                    fase3cbb2.DisplayMemberPath = "weergavenaam";
                                    fase3cbb2.SelectedValuePath = "tmpnr";
                                    fase3cbb2.Focus();
                                    break;
                                case "fase1cbb3":
                                    fase2cbb3.Visibility = System.Windows.Visibility.Hidden;
                                    fase2cbb3.SelectedIndex = -1;
                                    fase2cbb3.Text = "";
                                    fase3cbb3.Visibility = System.Windows.Visibility.Visible;
                                    fase3cbb3.ItemsSource = Get_afgl_tbl().AsEnumerable();
                                    fase3cbb3.DisplayMemberPath = "weergavenaam";
                                    fase3cbb3.SelectedValuePath = "tmpnr";
                                    fase3cbb3.Focus();
                                    break;
                                case "fase1cbb4":
                                    fase2cbb4.Visibility = System.Windows.Visibility.Hidden;
                                    fase2cbb4.SelectedIndex = -1;
                                    fase2cbb4.Text = "";
                                    fase3cbb4.Visibility = System.Windows.Visibility.Visible;
                                    fase3cbb4.ItemsSource = Get_afgl_tbl().AsEnumerable();
                                    fase3cbb4.DisplayMemberPath = "weergavenaam";
                                    fase3cbb4.SelectedValuePath = "tmpnr";
                                    fase3cbb4.Focus();
                                    break;
                            }
                            break;
						case 6:
							switch (feSource.Name)
							{
								case "fase1cbb1":
									fase2cbb1.Visibility = System.Windows.Visibility.Hidden;
									fase2cbb1.SelectedIndex = -1;
									fase2cbb1.Text = "";
									fase3tbb1.Visibility = System.Windows.Visibility.Visible;
									break;
								case "fase1cbb2":
									fase2cbb2.Visibility = System.Windows.Visibility.Hidden;
									fase2cbb2.SelectedIndex = -1;
									fase2cbb2.Text = "";
									fase3tbb2.Visibility = System.Windows.Visibility.Visible;
									break;
								case "fase1cbb3":
									fase2cbb3.Visibility = System.Windows.Visibility.Hidden;
									fase2cbb3.SelectedIndex = -1;
									fase2cbb3.Text = "";
									fase3tbb3.Visibility = System.Windows.Visibility.Visible;
									break;
								case "fase1cbb4":
									fase2cbb4.Visibility = System.Windows.Visibility.Hidden;
									fase2cbb4.SelectedIndex = -1;
									fase2cbb4.Text = "";
									fase3tbb4.Visibility = System.Windows.Visibility.Visible;
									break;
							}
							break;
                    }  
                }
            }
            zoeken();
            e.Handled = true;
        }
        private void Fase2Select(object sender, RoutedEventArgs e)
        {
            
            FrameworkElement feSource = e.Source as FrameworkElement;
            switch (feSource.Name)
            {
                case "fase2cbb1":
                    fase3cbb1.Visibility = System.Windows.Visibility.Hidden;
                    fase3cbb1.SelectedIndex = -1;
                    fase3mcbb1.Visibility = System.Windows.Visibility.Hidden;
                    fase3tbb1.Text = "";
                    fase3tbb1.Visibility = System.Windows.Visibility.Hidden;
                    fase3ckb1J.Visibility = System.Windows.Visibility.Hidden;
                    fase3ckb1N.Visibility = System.Windows.Visibility.Hidden;
                    regels[0] = false;
                    break;
                case "fase2cbb2":
                    fase3cbb2.Visibility = System.Windows.Visibility.Hidden;
                    fase3cbb2.SelectedIndex = -1;
                    fase3mcbb2.Visibility = System.Windows.Visibility.Hidden;
                    fase3tbb2.Text = "";
                    fase3tbb2.Visibility = System.Windows.Visibility.Hidden;
                    fase3ckb2J.Visibility = System.Windows.Visibility.Hidden;
                    fase3ckb2N.Visibility = System.Windows.Visibility.Hidden;
                    regels[1] = false;
                    break;
                case "fase2cbb3":
                    fase3cbb3.Visibility = System.Windows.Visibility.Hidden;
                    fase3cbb3.SelectedIndex = -1;
                    fase3mcbb3.Visibility = System.Windows.Visibility.Hidden;
                    fase3tbb3.Text = "";
                    fase3tbb3.Visibility = System.Windows.Visibility.Hidden;
                    fase3ckb3J.Visibility = System.Windows.Visibility.Hidden;
                    fase3ckb3N.Visibility = System.Windows.Visibility.Hidden;
                    regels[2] = false;
                    break;
                case "fase2cbb4":
                    fase3cbb4.Visibility = System.Windows.Visibility.Hidden;
                    fase3cbb4.SelectedIndex = -1;
                    fase3mcbb4.Visibility = System.Windows.Visibility.Hidden;
                    fase3tbb4.Text = "";
                    fase3tbb4.Visibility = System.Windows.Visibility.Hidden;
                    fase3ckb4J.Visibility = System.Windows.Visibility.Hidden;
                    fase3ckb4N.Visibility = System.Windows.Visibility.Hidden;
                    regels[3] = false;
                    break;
            }
            if (sender is ComboBox)
            {
                if ((sender as ComboBox).SelectedIndex > -1)
                {
                    
                    switch (feSource.Name)
                    {
                        case "fase2cbb1":
                            //
                                fase3cbb1.Visibility = System.Windows.Visibility.Hidden;
                                fase3tbb1.Visibility = System.Windows.Visibility.Hidden;
                                fase3mcbb1.reset();
                                int funct = fase2cbb1.SelectedIndex;
                            if ((int)fase1cbb1.SelectedValue == 3)
                            {
                               
                                fase3mcbb1.setTypeNparent(3, this,funct);
                                fase3mcbb1.toon();
                            }
                            else if ((int)fase1cbb1.SelectedValue == 4)
                            {
                                fase3mcbb1.setTypeNparent(2,this,funct);
                                fase3mcbb1.toon();
                            }
							else if ((fase2cbb1.SelectedItem as MenuItem).Text.Contains("actief"))
							{
								fase3ckb1J.Visibility = System.Windows.Visibility.Visible;
								fase3ckb1N.Visibility = System.Windows.Visibility.Visible;
							}
							else
							{
								fase3ckb1J.Visibility = System.Windows.Visibility.Hidden;
								fase3ckb1N.Visibility = System.Windows.Visibility.Hidden;
								fase3tbb1.Visibility = System.Windows.Visibility.Visible;
							}
                            break;
                        case "fase2cbb2":
                            //
                                fase3cbb2.Visibility = System.Windows.Visibility.Hidden;
                                fase3tbb2.Visibility = System.Windows.Visibility.Hidden;
                                fase3mcbb2.reset();
                            int funct_id = fase2cbb2.SelectedIndex;
                            if ((int)fase1cbb2.SelectedValue == 3)
                            {
                                fase3mcbb2.setTypeNparent(3,this,funct_id);
                                fase3mcbb2.toon();
                            }
                            else if ((int)fase1cbb2.SelectedValue == 4)
                            {
                                fase3mcbb2.setTypeNparent(2,this,funct_id);
                                fase3mcbb2.toon();
                            }
                            else if ((fase2cbb2.SelectedItem as MenuItem).Text.Contains("actief"))
                            {
                                fase3ckb2J.Visibility = System.Windows.Visibility.Visible;
                                fase3ckb2N.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                fase3ckb2J.Visibility = System.Windows.Visibility.Hidden;
                                fase3ckb2N.Visibility = System.Windows.Visibility.Hidden;
                                fase3tbb2.Visibility = System.Windows.Visibility.Visible;
                            }
                            break;
                        case "fase2cbb3":
                            //
                                fase3cbb3.Visibility = System.Windows.Visibility.Hidden;
                                fase3tbb3.Visibility = System.Windows.Visibility.Hidden;
                                fase3mcbb3.reset();
                                int funct_indx = fase2cbb3.SelectedIndex;
                            if ((int)fase1cbb3.SelectedValue == 3)
                            {
                                fase3mcbb3.setTypeNparent(3,this,funct_indx);
                                fase3mcbb3.toon();
                            }
                            else if ((int)fase1cbb3.SelectedValue == 4)
                            {
                                fase3mcbb3.setTypeNparent(2,this,funct_indx);
                                fase3mcbb3.toon();
                            }
                            else if ((fase2cbb3.SelectedItem as MenuItem).Text.Contains("actief"))
                            {
                                fase3ckb3J.Visibility = System.Windows.Visibility.Visible;
                                fase3ckb3N.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                fase3ckb3J.Visibility = System.Windows.Visibility.Hidden;
                                fase3ckb3N.Visibility = System.Windows.Visibility.Hidden;
                                fase3tbb3.Visibility = System.Windows.Visibility.Visible;
                            }
                            break;
                        case "fase2cbb4":
                            //
                                fase3cbb4.Visibility = System.Windows.Visibility.Hidden;
                                fase3tbb4.Visibility = System.Windows.Visibility.Hidden;
                                fase3mcbb4.reset();
                                int funct_index = fase2cbb4.SelectedIndex;
                            if ((int)fase1cbb4.SelectedValue == 3)
                            {
                                fase3mcbb4.setTypeNparent(3,this,funct_index);
                                fase3mcbb4.toon();
                            }
                            else if ((int)fase1cbb4.SelectedValue == 4)
                            {
                                fase3mcbb4.setTypeNparent(2,this,funct_index);
                                fase3mcbb4.toon();
                            }
                            else if ((fase2cbb4.SelectedItem as MenuItem).Text.Contains("actief"))
                            {
                                fase3ckb4J.Visibility = System.Windows.Visibility.Visible;
                                fase3ckb4N.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                fase3ckb4J.Visibility = System.Windows.Visibility.Hidden;
                                fase3ckb4N.Visibility = System.Windows.Visibility.Hidden;
                                fase3tbb4.Visibility = System.Windows.Visibility.Visible;
                            }
                            break;
                    }
                }
            }
            zoeken();
            e.Handled = true;
        }
        private void AdvBtn1_Click(object sender, RoutedEventArgs e)
        {
            
            if (fase1cbb1.IsVisible || Quicktb1.IsVisible)//aan naar uit
            {
                regels[0] = false;
                fase0lbl1.Visibility = System.Windows.Visibility.Hidden;
                row1image.Source = (ImageSource)FindResource("PijlDown");
                AdvBtn2.Visibility = System.Windows.Visibility.Hidden;
                quickcheck1.Visibility = System.Windows.Visibility.Hidden;
                Quicktb1.Visibility = System.Windows.Visibility.Hidden;
                Quicktb1.Text = string.Empty;
                fase1cbb1.Visibility = System.Windows.Visibility.Hidden;
                fase1cbb1.SelectedIndex = -1;
                fase1cbb1.Text = "";
                fase2cbb1.Visibility = System.Windows.Visibility.Hidden;
                fase2cbb1.SelectedIndex = -1;
                fase2cbb1.Text = "";
                fase3tbb1.Visibility = System.Windows.Visibility.Hidden;
                fase3tbb1.Clear();
                fase3cbb1.SelectedIndex = -1;
                fase3cbb1.Text = "";
                fase3mcbb1.reset();
                fase3ckb1J.Visibility = System.Windows.Visibility.Hidden;
                fase3ckb1N.Visibility = System.Windows.Visibility.Hidden;
              
            }
            else//uit naar aan
            {
                fase0lbl1.Visibility = System.Windows.Visibility.Visible;
                row1image.Source = (ImageSource)FindResource("PijlUp");
                AdvBtn2.Visibility = System.Windows.Visibility.Visible;
                quickcheck1.IsChecked = false;
                quickcheck1.Visibility = System.Windows.Visibility.Visible;
                if (!Advanched[0])
                {
                    Quicktb1.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    fase1cbb1.Visibility = System.Windows.Visibility.Visible;
                    
                }
                handelingen_logger.log_handeling(0, type, 14);
            }
            zoeken();
        }
        private void AdvBtn2_Click(object sender, RoutedEventArgs e)
        {
            (start_parent as overview1).zoekbox_MouseClick();
            if (fase1cbb2.IsVisible || Quicktb2.IsVisible)//aan naar uit
            {
                regels[1] = false;
                fase0lbl2.Visibility = System.Windows.Visibility.Hidden;
                row2image.Source = (ImageSource)FindResource("PijlDown");
                AdvBtn3.Visibility = System.Windows.Visibility.Hidden;
                AdvBtn1.Visibility = System.Windows.Visibility.Visible;
                quickcheck2.Visibility = System.Windows.Visibility.Hidden;
                Quicktb2.Visibility = System.Windows.Visibility.Hidden;
                Quicktb2.Text = string.Empty;
                fase1cbb2.Visibility = System.Windows.Visibility.Hidden;
                fase1cbb2.SelectedIndex = -1;
                fase1cbb2.Text = "";
                fase2cbb2.Visibility = System.Windows.Visibility.Hidden;
                fase2cbb2.SelectedIndex = -1;
                fase2cbb2.Text = "";
                fase3tbb2.Visibility = System.Windows.Visibility.Hidden;
                fase3tbb2.Clear();
                fase3cbb2.SelectedIndex = -1;
                fase3cbb2.Text = "";
                fase3mcbb2.reset();
                fase3ckb2J.Visibility = System.Windows.Visibility.Hidden;
                fase3ckb2N.Visibility = System.Windows.Visibility.Hidden;
            }
            else//uit naar aan
            {
                fase0lbl2.Visibility = System.Windows.Visibility.Visible;
                row2image.Source = (ImageSource)FindResource("PijlUp");
                AdvBtn3.Visibility = System.Windows.Visibility.Visible;
                AdvBtn1.Visibility = System.Windows.Visibility.Hidden;
                quickcheck2.IsChecked = false;
                quickcheck2.Visibility = System.Windows.Visibility.Visible;
                if (!Advanched[1])
                {
                    Quicktb2.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    fase1cbb2.Visibility = System.Windows.Visibility.Visible;

                }
                handelingen_logger.log_handeling(0, type, 15);
            }
            zoeken();
        }
        private void AdvBtn3_Click(object sender, RoutedEventArgs e)
        {
            
            if (fase1cbb3.IsVisible || Quicktb3.IsVisible)//aan naar uit
            {
                regels[2] = false;
                fase0lbl3.Visibility = System.Windows.Visibility.Hidden;
                row3image.Source = (ImageSource)FindResource("PijlDown");
                AdvBtn4.Visibility = System.Windows.Visibility.Hidden;
                AdvBtn2.Visibility = System.Windows.Visibility.Visible;
                quickcheck3.Visibility = System.Windows.Visibility.Hidden;
                Quicktb3.Visibility = System.Windows.Visibility.Hidden;
                Quicktb3.Text = string.Empty;
                fase1cbb3.Visibility = System.Windows.Visibility.Hidden;
                fase1cbb3.SelectedIndex = -1;
                fase1cbb3.Text = "";
                fase2cbb3.Visibility = System.Windows.Visibility.Hidden;
                fase2cbb3.SelectedIndex = -1;
                fase2cbb3.Text = "";
                fase3tbb3.Visibility = System.Windows.Visibility.Hidden;
                fase3tbb3.Clear();
                fase3cbb3.SelectedIndex = -1;
                fase3cbb3.Text = "";
                fase3mcbb3.reset();
                fase3ckb3J.Visibility = System.Windows.Visibility.Hidden;
                fase3ckb3N.Visibility = System.Windows.Visibility.Hidden;
                
            }
            else//uit naar aan
            {
                fase0lbl3.Visibility = System.Windows.Visibility.Visible;
                row3image.Source = (ImageSource)FindResource("PijlUp");
                AdvBtn4.Visibility = System.Windows.Visibility.Visible;
                AdvBtn2.Visibility = System.Windows.Visibility.Hidden;
                quickcheck3.IsChecked = false;
                quickcheck3.Visibility = System.Windows.Visibility.Visible;
                if (!Advanched[2])
                {
                    Quicktb3.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    fase1cbb3.Visibility = System.Windows.Visibility.Visible;

                }
                handelingen_logger.log_handeling(0, type, 16);
            }
            zoeken();
        }
        private void AdvBtn4_Click(object sender, RoutedEventArgs e)
        {
            
            if (fase1cbb4.IsVisible || Quicktb4.IsVisible)//aan naar uit
            {
                regels[3] = false;
                fase0lbl4.Visibility = System.Windows.Visibility.Hidden;
                row4image.Source = (ImageSource)FindResource("PijlDown");
                AdvBtn3.Visibility = System.Windows.Visibility.Visible;
                quickcheck4.Visibility = System.Windows.Visibility.Hidden;
                Quicktb4.Visibility = System.Windows.Visibility.Hidden;
                Quicktb4.Text = string.Empty;
                fase1cbb4.Visibility = System.Windows.Visibility.Hidden;
                fase1cbb4.SelectedIndex = -1;
                fase1cbb4.Text = "";
                fase2cbb4.Visibility = System.Windows.Visibility.Hidden;
                fase2cbb4.SelectedIndex = -1;
                fase2cbb4.Text = "";
                fase3tbb4.Visibility = System.Windows.Visibility.Hidden;
                fase3tbb4.Clear();
                fase3cbb4.SelectedIndex = -1;
                fase3cbb4.Text = "";
                fase3mcbb4.reset();
                fase3ckb4J.Visibility = System.Windows.Visibility.Hidden;
                fase3ckb4N.Visibility = System.Windows.Visibility.Hidden;
            }
            else//uit naar aan
            {
                fase0lbl4.Visibility = System.Windows.Visibility.Visible;
                row4image.Source = (ImageSource)FindResource("PijlUp");
                AdvBtn3.Visibility = System.Windows.Visibility.Hidden;
                quickcheck4.Visibility = System.Windows.Visibility.Visible;
                quickcheck4.IsChecked = false;
                if (!Advanched[3])
                {
                    Quicktb4.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    fase1cbb4.Visibility = System.Windows.Visibility.Visible;

                }
                handelingen_logger.log_handeling(0, type, 17);
            }
            zoeken();
            
        }
        private void cbb_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ComboBox tmpbox = (ComboBox)sender;
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (tmpbox.IsDropDownOpen)
                {

                }
                else
                {
                    tmpbox.IsDropDownOpen = true;
                }
            }
            
        }
        private void quickcheck_Checked(object sender, RoutedEventArgs e)
        {
            (start_parent as overview1).zoekbox_MouseClick();
            FrameworkElement feSource = e.Source as FrameworkElement;
            switch (feSource.Name)
            {
                case "quickcheck1":
                    Advanched[0] = true;
                    Quicktb1.Visibility = System.Windows.Visibility.Hidden;
                    Quicktb1.Clear();
                    fase1cbb1.Visibility = System.Windows.Visibility.Visible;
                    fase1cbb1.Focus();
                    fase1cbb1.IsDropDownOpen = true;
                    break;
                case "quickcheck2":
                    Advanched[1] = true;
                    Quicktb2.Visibility = System.Windows.Visibility.Hidden;
                    Quicktb2.Clear();
                    fase1cbb2.Visibility = System.Windows.Visibility.Visible;
                    fase1cbb2.Focus();
                    fase1cbb2.IsDropDownOpen = true;
                    break;
                case "quickcheck3":
                    Advanched[2] = true;
                    Quicktb3.Visibility = System.Windows.Visibility.Hidden;
                    Quicktb3.Clear();
                    fase1cbb3.Visibility = System.Windows.Visibility.Visible;
                    fase1cbb3.Focus();
                    fase1cbb3.IsDropDownOpen = true;
                    break;
                case "quickcheck4":
                    Advanched[3] = true;
                    Quicktb4.Visibility = System.Windows.Visibility.Hidden;
                    Quicktb4.Clear();
                    fase1cbb4.Visibility = System.Windows.Visibility.Visible;
                    fase1cbb4.Focus();
                    fase1cbb4.IsDropDownOpen = true;
                    break;

            }
            zoeken();
            if (!logged)
            {
                handelingen_logger.log_handeling(0, type, 13);
                logged = true;
            }
        }
        private void quickcheck_Unchecked(object sender, RoutedEventArgs e)
        {
            
            FrameworkElement feSource = e.Source as FrameworkElement;
            switch (feSource.Name)
            {
                case "quickcheck1":
                    Advanched[0] = false;
                    Quicktb1.Visibility = System.Windows.Visibility.Visible;
                    fase1cbb1.Visibility = System.Windows.Visibility.Hidden;
                    fase1cbb1.SelectedIndex = -1;
                    fase1cbb1.Text = "";
                    fase2cbb1.Visibility = System.Windows.Visibility.Hidden;
                    fase2cbb1.SelectedIndex = -1;
                    fase2cbb1.Text = "";
                    fase3tbb1.Visibility = System.Windows.Visibility.Hidden;
                    fase3tbb1.Clear();
                    fase3mcbb1.reset();
                    break;
                case "quickcheck2":
                    Advanched[1] = false;
                    Quicktb2.Visibility = System.Windows.Visibility.Visible;
                    fase1cbb2.Visibility = System.Windows.Visibility.Hidden;
                    fase1cbb2.SelectedIndex = -1;
                    fase1cbb2.Text = "";
                    fase2cbb2.Visibility = System.Windows.Visibility.Hidden;
                    fase2cbb2.SelectedIndex = -1;
                    fase2cbb2.Text = "";
                    fase3tbb2.Visibility = System.Windows.Visibility.Hidden;
                    fase3tbb2.Clear();
                    fase3mcbb2.reset();
                    break;
                case "quickcheck3":
                    Advanched[2] = false;
                    Quicktb3.Visibility = System.Windows.Visibility.Visible;
                    fase1cbb3.Visibility = System.Windows.Visibility.Hidden;
                    fase1cbb3.SelectedIndex = -1;
                    fase1cbb3.Text = "";
                    fase2cbb3.Visibility = System.Windows.Visibility.Hidden;
                    fase2cbb3.SelectedIndex = -1;
                    fase2cbb3.Text = "";
                    fase3tbb3.Visibility = System.Windows.Visibility.Hidden;
                    fase3tbb3.Clear();
                    fase3mcbb3.reset();
                    break;
                case "quickcheck4":
                    Advanched[3] = false;
                    Quicktb4.Visibility = System.Windows.Visibility.Visible;
                    fase1cbb4.Visibility = System.Windows.Visibility.Hidden;
                    fase1cbb4.SelectedIndex = -1;
                    fase1cbb4.Text = "";
                    fase2cbb4.Visibility = System.Windows.Visibility.Hidden;
                    fase2cbb4.SelectedIndex = -1;
                    fase2cbb4.Text = "";
                    fase3tbb4.Visibility = System.Windows.Visibility.Hidden;
                    fase3tbb4.Clear();
                    fase3mcbb4.reset();
                    break;

            }
            zoeken();
        }
        private void fase3tbb_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            FrameworkElement feSource = e.Source as FrameworkElement;
            if ((sender as TextBox).Text.Length > 0)
            {
                switch (feSource.Name)
                {
                    case "fase3tbb1":
						regels[0] = true;
						zoektermen[0] = fase3tbb1.Text;
						if ((fase1cbb1.SelectedItem as MenuItem).Waarde == 6)
						{
							IsGelijkFilter[0] = true;
						}
						else
						{
							kolnamen[0] = (fase2cbb1.SelectedItem as MenuItem).veldnaam;
							
						}
                       
                        break;
                    case "fase3tbb2":
						zoektermen[1] = fase3tbb2.Text;
						regels[1] = true;
						if ((fase1cbb2.SelectedItem as MenuItem).Waarde == 6)
						{
							IsGelijkFilter[1] = true;
						}
						else
						{
							
							kolnamen[1] = (fase2cbb2.SelectedItem as MenuItem).veldnaam;
						}
                        
                        break;
                    case "fase3tbb3":
						zoektermen[2] = fase3tbb3.Text;
						regels[2] = true;
						if ((fase1cbb3.SelectedItem as MenuItem).Waarde == 6)
						{
							IsGelijkFilter[2] = true;
						}
						else
						{
							kolnamen[2] = (fase2cbb3.SelectedItem as MenuItem).veldnaam;
						}
                       
                        break;
                    case "fase3tbb4":
						zoektermen[3] = fase3tbb4.Text;
						regels[3] = true;
						if ((fase1cbb4.SelectedItem as MenuItem).Waarde == 6)
						{
							IsGelijkFilter[3] = true;
						}
						else
						{
							kolnamen[3] = (fase2cbb4.SelectedItem as MenuItem).veldnaam;
							
						}
                        break;
                }
                
            }
            else
            {
                switch (feSource.Name)
                {
                    case "fase3tbb1":
                        regels[0] = false;
						IsGelijkFilter[0] = false;
                        break;
                    case "fase3tbb2":
                        regels[1] = false;
						IsGelijkFilter[2] = false;
                        break;
                    case "fase3tbb3":
                        regels[2] = false;
						IsGelijkFilter[2] = false;
                        break;
                    case "fase3tbb4":
                        regels[3] = false;
						IsGelijkFilter[3] = false;
                        break;
                }
            } zoeken();
        }
        public void link_grid(overview1 lv, System.Windows.Forms.DataGridView dgv, int tp)
        {
            listv = lv;
            dgv1 = dgv;
            type = tp;
            _menuEngine.LoadLists(type);
            AdvBtn1.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        private void Quicktb_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            FrameworkElement feSource = e.Source as FrameworkElement;
            if ((sender as TextBox).Text.Length > 0)
            {
                switch (feSource.Name)
                {
                    case "Quicktb1":
                        zoektermen[0] = Quicktb1.Text;
                        regels[0] = true;
                        break;
                    case "Quicktb2":
                        zoektermen[1] = Quicktb2.Text;
                        regels[1] = true;
                        break;
                    case "Quicktb3":
                        zoektermen[2] = Quicktb3.Text;
                        regels[2] = true;
                        break;
                    case "Quicktb4":
                        zoektermen[3] = Quicktb4.Text;
                        regels[3] = true;
                        break;
                }
                
            }
            else
            {
                switch (feSource.Name)
                {
                    case "Quicktb1":
                        regels[0] = false;
                        break;
                    case "Quicktb2":
                        regels[1] = false;
                        break;
                    case "Quicktb3":
                        regels[2] = false;
                        break;
                    case "Quicktb4":
                        regels[3] = false;
                        break;
                }
            }
            (start_parent as overview1).textBox_TextChanged();
            zoeken();
        }
        bool lijst_is_leeg = false;
        private void leeglijst()
        {
            lijst_is_leeg = true;   
            if (tmpbw != null)
                {
                    tmpbw.CancelAsync();
                }
                
             System.Data.SqlClient.SqlConnection  con = new System.Data.SqlClient.SqlConnection();
             con.ConnectionString = Global.ConnectionString_fileserver;
             System.Data.SqlClient.SqlCommand command1 = new SqlCommand();
             con.Open();
            if (type == 1)
            {
                 command1 = new SqlCommand("project overview", con);
                 
            }
            else if (type == 2)
            {
                 command1 = new SqlCommand("bedrijf_overview", con);
                 
            }
            else if(type == 3)
            {
                 command1 = new SqlCommand("persoon_overview", con);
                 
            }
            command1.CommandType = CommandType.StoredProcedure;
           
            SqlDataAdapter adapt = new SqlDataAdapter();
            DataTable dt = new DataTable();
            adapt.SelectCommand = command1;
            adapt.Fill(dt);
            con.Close();
            listv.ChangeDatasource(dt);
        }
        public void zoeken()
        {
            if (tmpbw != null)
            {
                tmpbw.CancelAsync();
            }

            if (regels[0] || regels[1] || regels[2] || regels[3])
            {
                lijst_is_leeg = false;
                BackgroundWorker BW = new BackgroundWorker();
                tmpbw = BW;
                BW.WorkerReportsProgress = true;
                BW.WorkerSupportsCancellation = true;
                BW.DoWork += new DoWorkEventHandler(bw_DoWork);
                BW.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
                BW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                BW.RunWorkerAsync();
                //DataTable dt = AdvFiltQRYBuildr.start_building(type, regels, zoektermen, kolnamen, Advanched, speciaalfilter, isbedrijf, functie_regel_nrs);
            }
            else
            {
                leeglijst();
            }
            
        }
        public void minder_regels(multibox mb)
        {
            switch (mb.Name)
            {
                case "fase3mcbb1":
                    regels[0] = false;
                    speciaalfilter[0] = -1;
                    break;
                case "fase3mcbb2":
                    regels[1] = false;
                    speciaalfilter[1] = -1;
                    break;
                case "fase3mcbb3":
                    regels[2] = false;
                    speciaalfilter[2] = -1;
                    break;
                case "fase3mcbb4":
                    regels[3] = false;
                    speciaalfilter[3] = -1;
                    break;
            }
        }
        public void fase3mcbb_selectionChanged(int id, multibox mtb, int tp)
        {
            
            AdvFilterDatasetTableAdapters.bedrijf_nr_locatiesTableAdapter bdrnrlocadapt = new AdvFilterDatasetTableAdapters.bedrijf_nr_locatiesTableAdapter();
            AdvFilterDatasetTableAdapters.persoon_nr_locatiesTableAdapter persnrlocadapt = new AdvFilterDatasetTableAdapters.persoon_nr_locatiesTableAdapter();
            AdvFilterDatasetTableAdapters.functiebedrijvenTableAdapter functbdradapt = new AdvFilterDatasetTableAdapters.functiebedrijvenTableAdapter();
            AdvFilterDatasetTableAdapters.functiepersoonTableAdapter functpersadapt = new AdvFilterDatasetTableAdapters.functiepersoonTableAdapter();
            bdrnrlocadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            persnrlocadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            functbdradapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            functpersadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            AdvFilterDataset.bedrijf_nr_locatiesDataTable bdrnrloctable = new AdvFilterDataset.bedrijf_nr_locatiesDataTable();
            AdvFilterDataset.persoon_nr_locatiesDataTable persnrloctable = new AdvFilterDataset.persoon_nr_locatiesDataTable();
            AdvFilterDataset.functiebedrijvenDataTable functbdrtable = new AdvFilterDataset.functiebedrijvenDataTable();
            AdvFilterDataset.functiepersoonDataTable functperstable = new AdvFilterDataset.functiepersoonDataTable();
            AdvFilterDataset.bedrijf_nr_locatiesRow bdrlocrow;
            AdvFilterDataset.persoon_nr_locatiesRow perslocrow;
            bdrnrloctable = bdrnrlocadapt.GetData();
            persnrloctable = persnrlocadapt.GetData();
            functbdrtable = functbdradapt.GetData();
            functperstable = functpersadapt.GetData();
            string knm = "";
            if(type == 3)
            {
                switch (mtb.Name)
                {
                    case "fase3mcbb1":
                        regels[0] = true;
                        zoektermen[0] = id.ToString();
                        kolnamen[0] = "bedrijf_nr";
                        break;
                    case "fase3mcbb2":
                        regels[1] = true;
                        zoektermen[1] = id.ToString();
                        kolnamen[1] = "bedrijf_nr";
                        break;
                    case "fase3mcbb3":
                        regels[2] = true;
                        zoektermen[2] = id.ToString();
                        kolnamen[2] = "bedrijf_nr";
                        break;
                    case "fase3mcbb4":
                        regels[3] = true;
                        zoektermen[3] = id.ToString();
                        kolnamen[3] = "bedrijf_nr";
                        break;

                }
            }
            else
            {
                switch (mtb.Name)
                {
                    case "fase3mcbb1":
                        regels[0] = true;
                        zoektermen[0] = id.ToString();
                        if(tp == 3)
                        {
                            isbedrijf[0] = false;
                            foreach(AdvFilterDataset.personenlijstRow pRow in perslijsttabel)
                            {
                                if((int)fase2cbb1.SelectedValue == pRow.tmpnr && fase2cbb1.Text == pRow.weergavenaam)
                                {
                                    speciaalfilter[0] = pRow.var;
                                    zoektermen[0] = id.ToString();
                                    functie_regel_nrs[0] = pRow.tmpnr;
                                    if (pRow.var == 1)
                                    {
                                        perslocrow = persnrloctable.FindByregel_nr(pRow.tmpnr);
                                       knm = perslocrow.column;
                                    }
                                }
                            }
                        }
                        else if(tp ==2)
                        {
                            isbedrijf[0] = true;
                            foreach(AdvFilterDataset.bedrijvenlijstRow bRow in bdrlijsttabel)
                            {
                                if((int)fase2cbb1.SelectedValue == bRow.tmpnr && fase2cbb1.Text == bRow.weergavenaam)
                                {
                                    speciaalfilter[0] = bRow.var;
                                    zoektermen[0] = id.ToString();
                                    functie_regel_nrs[0] = bRow.tmpnr;
                                    if (bRow.var == 1)
                                    {
                                        bdrlocrow = bdrnrloctable.FindByregel_nr(bRow.tmpnr);
                                        knm = bdrlocrow.column;
                                    }
                                }
                            }
                        }
                        kolnamen[0] = knm;
                        break;
                    case "fase3mcbb2":
                        regels[1] = true;
                        zoektermen[1] = id.ToString();
                        if(tp == 3)
                        {
                            isbedrijf[1] = false;
                            foreach(AdvFilterDataset.personenlijstRow pRow in perslijsttabel)
                            {
                                if((int)fase2cbb2.SelectedValue == pRow.tmpnr && fase2cbb2.Text == pRow.weergavenaam)
                                {
                                    speciaalfilter[1] = pRow.var;
                                    zoektermen[1] = id.ToString();
                                    functie_regel_nrs[1] = pRow.tmpnr;
                                    if (pRow.var == 1)
                                    {
                                        perslocrow = persnrloctable.FindByregel_nr(pRow.tmpnr);
                                        knm = perslocrow.column;
                                    }
                                }
                            }
                        }
                        else if(tp ==2)
                        {
                            isbedrijf[1] = true;
                            foreach(AdvFilterDataset.bedrijvenlijstRow bRow in bdrlijsttabel)
                            {
                                if ((int)fase2cbb2.SelectedValue == bRow.tmpnr && fase2cbb2.Text == bRow.weergavenaam)
                                {
                                    speciaalfilter[1] = bRow.var;
                                    zoektermen[1] = id.ToString();
                                    functie_regel_nrs[1] = bRow.tmpnr;
                                    if (bRow.var == 1)
                                    {
                                        bdrlocrow = bdrnrloctable.FindByregel_nr(bRow.tmpnr);
                                        knm = bdrlocrow.column;
                                    }
                                
                                }
                            }
                        }
                        kolnamen[1] = knm;
                        break;
                    case "fase3mcbb3":
                        regels[2] = true;
                        zoektermen[2] = id.ToString();
                        if(tp == 3)
                        {
                            isbedrijf[2] = false;
                            foreach(AdvFilterDataset.personenlijstRow pRow in perslijsttabel)
                            {
                                if ((int)fase2cbb3.SelectedValue == pRow.tmpnr && fase2cbb3.Text == pRow.weergavenaam)
                                {
                                    speciaalfilter[2] = pRow.var;
                                    zoektermen[2] = id.ToString();
                                    functie_regel_nrs[2] = pRow.tmpnr;
                                    if (pRow.var == 1)
                                    {
                                        perslocrow = persnrloctable.FindByregel_nr(pRow.tmpnr);
                                        knm = perslocrow.column;
                                    }
                                }
                            }
                        }
                        else if(tp ==2)
                        {
                            isbedrijf[2] = true;
                            foreach(AdvFilterDataset.bedrijvenlijstRow bRow in bdrlijsttabel)
                            {
                                if ((int)fase2cbb3.SelectedValue == bRow.tmpnr && fase2cbb3.Text == bRow.weergavenaam)
                                {
                                    speciaalfilter[2] = bRow.var;
                                    zoektermen[2] = id.ToString();
                                    functie_regel_nrs[2] = bRow.tmpnr;
                                    if (bRow.var == 1)
                                    {
                                        bdrlocrow = bdrnrloctable.FindByregel_nr(bRow.tmpnr);
                                        knm = bdrlocrow.column;
                                    }
                                }
                            }
                        }
                        kolnamen[2] = knm;
                        break;
                    case "fase3mcbb4":
                        regels[3] = true;
                        zoektermen[3] = id.ToString();
                        if(tp == 3)
                        {
                            isbedrijf[3] = false;
                            foreach(AdvFilterDataset.personenlijstRow pRow in perslijsttabel)
                            {
                                if ((int)fase2cbb4.SelectedValue == pRow.tmpnr && fase2cbb4.Text == pRow.weergavenaam)
                                {
                                    speciaalfilter[3] = pRow.var;
                                    zoektermen[3] = id.ToString();
                                    functie_regel_nrs[3] = pRow.tmpnr;
                                    if (pRow.var == 1)
                                    {
                                        perslocrow = persnrloctable.FindByregel_nr(pRow.tmpnr);
                                        knm = perslocrow.column;
                                    }
                                }
                            }
                        }
                        else if(tp ==2)
                        {
                            isbedrijf[3] = true;
                            foreach(AdvFilterDataset.bedrijvenlijstRow bRow in bdrlijsttabel)
                            {
                                if ((int)fase2cbb4.SelectedValue == bRow.tmpnr && fase2cbb4.Text == bRow.weergavenaam)
                                {
                                    speciaalfilter[3] = bRow.var;
                                    zoektermen[3] = id.ToString();
                                    functie_regel_nrs[3] = bRow.tmpnr;
                                    if (bRow.var == 1)
                                    {
                                        bdrlocrow = bdrnrloctable.FindByregel_nr(bRow.tmpnr);
                                        knm = bdrlocrow.column;
                                    }
                                }
                            }
                        }
                        kolnamen[3] = knm;
                        break;
                }
            }
            zoeken();
        }
        private void fase3cbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            FrameworkElement feSource = e.Source as FrameworkElement;
            if ((sender as ComboBox).SelectedIndex > -1)
            {
                
                switch (feSource.Name)
                {
                    case "fase3cbb1":
                        if (fase1cbb1.Text == "status")//status
                        {
                            zoektermen[0] = fase3cbb1.SelectedValue.ToString();
                            kolnamen[0] = "status";
                            regels[0] = true;
                        }
                        else if ((int)fase1cbb1.SelectedValue == 5)//afgeleide functies
                        {
                            int key = int.Parse( Get_afgl_tbl().Rows[fase3cbb1.SelectedIndex].ItemArray[1].ToString() );
                            bool value;
                            if (int.Parse(Get_afgl_tbl().Rows[fase3cbb1.SelectedIndex].ItemArray[2].ToString()) == 0)
                            {
                                value = false;
                            }
                            else                            
                            {
                                value = true;
                            }
                            afgl_functies[0] = new KeyValuePair<int, bool>(key, value);
                            regels[0] = true;
                        }
                        break;
                    case "fase3cbb2":
                        if (fase1cbb2.Text == "status")//status
                        {
                            zoektermen[1] = fase3cbb2.SelectedValue.ToString();
                            kolnamen[1] = "status";
                            regels[1] = true;
                        }
                        else if ((int)fase1cbb2.SelectedValue == 5)//afgeleide functies
                        {
                            int key = int.Parse(Get_afgl_tbl().Rows[fase3cbb2.SelectedIndex].ItemArray[1].ToString());
                            bool value;
                            if (int.Parse(Get_afgl_tbl().Rows[fase3cbb2.SelectedIndex].ItemArray[2].ToString()) == 0)
                            {
                                value = false;
                            }
                            else
                            {
                                value = true;
                            }
                            afgl_functies[1] = new KeyValuePair<int, bool>(key, value);
                            regels[1] = true;
                        }
                        break;
                    case "fase3cbb3":
                        if (fase1cbb3.Text == "status")//status
                        {
                            zoektermen[2] = fase3cbb3.SelectedValue.ToString();
                            kolnamen[2] = "status";
                            regels[2] = true;
                        }
                        else if ((int)fase1cbb3.SelectedValue == 5)//afgeleide functies
                        {
                            int key = int.Parse(Get_afgl_tbl().Rows[fase3cbb3.SelectedIndex].ItemArray[1].ToString());
                            bool value;
                            if (int.Parse(Get_afgl_tbl().Rows[fase3cbb3.SelectedIndex].ItemArray[2].ToString()) == 0)
                            {
                                value = false;
                            }
                            else
                            {
                                value = true;
                            }
                            afgl_functies[2] = new KeyValuePair<int, bool>(key, value);
                            regels[2] = true;
                        }
                        break;
                    case "fase3cbb4":
                        if (fase1cbb4.Text == "status")//status
                        {
                            zoektermen[3] = fase3cbb4.SelectedValue.ToString();
                            kolnamen[3] = "status";
                            regels[3] = true;
                        }
                        else if ((int)fase1cbb4.SelectedValue == 5)//afgeleide functies
                        {
                            int key = int.Parse(Get_afgl_tbl().Rows[fase3cbb4.SelectedIndex].ItemArray[1].ToString());
                            bool value;
                            if (int.Parse(Get_afgl_tbl().Rows[fase3cbb4.SelectedIndex].ItemArray[2].ToString()) == 0)
                            {
                                value = false;
                            }
                            else
                            {
                                value = true;
                            }
                            afgl_functies[3] = new KeyValuePair<int, bool>(key, value);
                            regels[3] = true;
                        }
                        break;
                }
                
            }
            else
            {
                KeyValuePair<int, bool> kv = new KeyValuePair<int, bool>(0, false);
                switch (feSource.Name)
                {
                    case "fase3cbb1":
                        afgl_functies.SetValue(kv, 0);
                        regels[0] = false;
                        break;
                    case "fase3cbb2":
                        afgl_functies.SetValue(kv, 1);
                        regels[1] = false;
                        break;
                    case "fase3cbb3":
                        afgl_functies.SetValue(kv, 2);
                        regels[2] = false;
                        break;
                    case "fase3cbb4":
                        afgl_functies.SetValue(kv, 4);
                        regels[3] = false;
                        break;
                }
            } zoeken();
        }
        private void fase3ckb_Checked(object sender, RoutedEventArgs e)
        {
            
            FrameworkElement feSource = e.Source as FrameworkElement;
            switch (feSource.Name)
            {

                case "fase3ckb1J":
                    kolnamen[0] =(fase2cbb1.SelectedItem as MenuItem).veldnaam;
                    zoektermen[0] = "1";
                    break;
                case "fase3ckb1N":
                    kolnamen[0] =(fase2cbb1.SelectedItem as MenuItem).veldnaam;
                    zoektermen[0] = "0";
                    break;
                case "fase3ckb2J":
                    kolnamen[1] =(fase2cbb2.SelectedItem as MenuItem).veldnaam;
                    zoektermen[1] = "1";
                    break;
                case "fase3ckb2N":
                    kolnamen[1] =(fase2cbb1.SelectedItem as MenuItem).veldnaam;
                    zoektermen[1] = "0";
                    break;
                case "fase3ckb3J":
                    kolnamen[2] =(fase2cbb3.SelectedItem as MenuItem).veldnaam;
                    zoektermen[2] = "1";
                    break;
                case "fase3ckb3N":
                    kolnamen[2] =(fase2cbb3.SelectedItem as MenuItem).veldnaam;
                    zoektermen[2] = "0";
                    break;
                case "fase3ckb4J":
                    kolnamen[3] =(fase2cbb4.SelectedItem as MenuItem).veldnaam;
                    zoektermen[3] = "1";
                    break;
                case "fase3ckb4N":
                    kolnamen[3] =(fase2cbb4.SelectedItem as MenuItem).veldnaam;
                    zoektermen[3] = "0";
                    break;
            }
            zoeken();
        }

        private void overig_cbb_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBox myComboBox = sender as ComboBox;
            // Get the textbox part of the combobox
            TextBox textBox = myComboBox.Template.FindName("PART_EditableTextBox", myComboBox) as TextBox;

            // holds the list of combobox items as strings
            List<System.String> items = new List<System.String>();

            // indicates whether the new character added should be removed
            bool shouldRemove = true;
            switch (myComboBox.Name)
            {
                case "fase1cbb1":
                    for (int i = 0; i < myComboBox.Items.Count; i++)
                    {
                        items.Add(((MenuItem)myComboBox.Items.GetItemAt(i)).Text.ToString());
                    }
                    break;
                case "fase1cbb2":
                    for (int i = 0; i < myComboBox.Items.Count; i++)
                    {
                        items.Add(((MenuItem)myComboBox.Items.GetItemAt(i)).Text.ToString());
                    }
                    break;
                case "fase1cbb3":
                    for (int i = 0; i < myComboBox.Items.Count; i++)
                    {
                        items.Add(((MenuItem)myComboBox.Items.GetItemAt(i)).Text.ToString());
                    }
                    break;
                case "fase1cbb4":
                    for (int i = 0; i < myComboBox.Items.Count; i++)
                    {
                        items.Add(((MenuItem)myComboBox.Items.GetItemAt(i)).Text.ToString());
                    }
                    break;
                case "fase2cbb1":
                    if ((fase1cbb1.SelectedItem as MenuItem).Waarde == 1)
                    {
                        for (int i = 0; i < myComboBox.Items.Count; i++)
                        {
                            items.Add(((MenuItem)myComboBox.Items.GetItemAt(i)).Text.ToString());
                        }
                    }
                    else
                    {
                        for (int i = 0; i < myComboBox.Items.Count; i++)
                        {
                            items.Add(((DataRowView)myComboBox.Items.GetItemAt(i))[0].ToString());
                        }
                    }
                    break;
                case "fase2cbb2":
                    if ((fase1cbb2.SelectedItem as MenuItem).Waarde == 1)
                    {
                        for (int i = 0; i < myComboBox.Items.Count; i++)
                        {
                            items.Add(((MenuItem)myComboBox.Items.GetItemAt(i)).Text.ToString());
                        }
                    }
                    else
                    {
                        for (int i = 0; i < myComboBox.Items.Count; i++)
                        {
                            items.Add(((DataRowView)myComboBox.Items.GetItemAt(i))[0].ToString());
                        }
                    }
                    break;
                case "fase2cbb3":
                    if ((fase1cbb3.SelectedItem as MenuItem).Waarde == 1)
                    {
                        for (int i = 0; i < myComboBox.Items.Count; i++)
                        {
                            items.Add(((MenuItem)myComboBox.Items.GetItemAt(i)).Text.ToString());
                        }
                    }
                    else
                    {
                        for (int i = 0; i < myComboBox.Items.Count; i++)
                        {
                            items.Add(((DataRowView)myComboBox.Items.GetItemAt(i))[0].ToString());
                        }
                    }
                    break;
                case "fase2cbb4":
                    if ((fase1cbb4.SelectedItem as MenuItem).Waarde == 1)
                    {
                        for (int i = 0; i < myComboBox.Items.Count; i++)
                        {
                            items.Add(((MenuItem)myComboBox.Items.GetItemAt(i)).Text.ToString());
                        }
                    }
                    else
                    {
                        for (int i = 0; i < myComboBox.Items.Count; i++)
                        {
                            items.Add(((DataRowView)myComboBox.Items.GetItemAt(i))[0].ToString());
                        }
                    }
                    break;
                case "fase3cbb1":
                    for (int i = 0; i < myComboBox.Items.Count; i++)
                    {
                        try { items.Add(((DataRowView)myComboBox.Items.GetItemAt(i))[2].ToString()); }
                        catch (Exception exp1)
                        {
                            listv.log_exception(exp1);
                            try
                            {
                                if (type == 3) { items.Add(((AdvFilterDataset.personenlijstRow)myComboBox.Items.GetItemAt(i))[0].ToString()); }
                                if (type == 2) { items.Add(((AdvFilterDataset.bedrijvenlijstRow)myComboBox.Items.GetItemAt(i))[0].ToString()); }
                            }
                            catch (Exception exp2)
                            { listv.log_exception(exp2); }
                        }
                    }
                    break;
                case "fase3cbb2":
                    for (int i = 0; i < myComboBox.Items.Count; i++)
                    {
                        try { items.Add(((DataRowView)myComboBox.Items.GetItemAt(i))[2].ToString()); }
                        catch (Exception exp1)
                        {
                            listv.log_exception(exp1);
                            try
                            {
                                if (type == 3) { items.Add(((AdvFilterDataset.personenlijstRow)myComboBox.Items.GetItemAt(i))[0].ToString()); }
                                if (type == 2) { items.Add(((AdvFilterDataset.bedrijvenlijstRow)myComboBox.Items.GetItemAt(i))[0].ToString()); }
                            }
                            catch (Exception exp2)
                            { listv.log_exception(exp2); }
                        }
                    }
                    break;
                case "fase3cbb3":
                    for (int i = 0; i < myComboBox.Items.Count; i++)
                    {
                        try { items.Add(((DataRowView)myComboBox.Items.GetItemAt(i))[2].ToString()); }
                        catch (Exception exp1)
                        {
                            listv.log_exception(exp1);
                            try
                            {
                                if (type == 3) { items.Add(((AdvFilterDataset.personenlijstRow)myComboBox.Items.GetItemAt(i))[0].ToString()); }
                                if (type == 2) { items.Add(((AdvFilterDataset.bedrijvenlijstRow)myComboBox.Items.GetItemAt(i))[0].ToString()); }
                            }
                            catch (Exception exp2)
                            { listv.log_exception(exp2); }
                        }
                    }
                    break;
                case "fase3cbb4":
                    for (int i = 0; i < myComboBox.Items.Count; i++)
                    {
                        try { items.Add(((DataRowView)myComboBox.Items.GetItemAt(i))[2].ToString()); }
                        catch (Exception exp1)
                        {
                            listv.log_exception(exp1);
                            try
                            {
                                if (type == 3) { items.Add(((AdvFilterDataset.personenlijstRow)myComboBox.Items.GetItemAt(i))[0].ToString()); }
                                if (type == 2) { items.Add(((AdvFilterDataset.bedrijvenlijstRow)myComboBox.Items.GetItemAt(i))[0].ToString()); }
                            }
                            catch (Exception exp2)
                            { listv.log_exception(exp2); }
                        }
                    }
                    break;
            }
           // for (int i = 0; i < myComboBox.Items.Count; i++)
           // {
           //     items.Add(((MenuItem)myComboBox.Items.GetItemAt(i)).Text.ToString());
           // }

            for (int i = 0; i < items.Count; i++)
            {
                // legal character input
                if (textBox.Text != "" && items[i].StartsWith(textBox.Text))
                {
                    shouldRemove = false;
                    break;
                }
            }

            // illegal character input
            if (textBox.Text != "" && shouldRemove)
            {
                textBox.Text = textBox.Text.Remove(textBox.Text.Length - 1);
                textBox.CaretIndex = textBox.Text.Length;
            }
        
        }

        private void Quicktb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Down)
            {
                dgv1.Focus();
            }
        }
        bool selecta = false;
        private void Quicktb_LostFocus(object sender, RoutedEventArgs e)
        {
            selecta = false;
        }
        private void Quicktb_GotFocus(object sender, RoutedEventArgs e)
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
            (start_parent as overview1).zoekbox_MouseClick();
        }
        private void Quicktb_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (t.SelectedText.Length != t.Text.Length)
            {
                t.SelectAll();

                t.ReleaseMouseCapture();

                e.Handled = true;
            }
            
        }
        
        private void cbb_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cbb = sender as ComboBox;
            if (cbb.SelectedIndex == -1)
            {
                
            }
            else
            {
                switch (cbb.Name)
                {
                    case "fase1cbb1":
                        if (fase1cbb1.SelectedIndex > -1)
                        {
                            if ((int)cbb.SelectedValue == 2)
                            {
                                fase3cbb1.Focus();
                                //fase3cbb1.IsDropDownOpen = true;
                            }
                            else
                            {
                                fase2cbb1.Focus();
                                //fase2cbb1.IsDropDownOpen = true;
                            }
                        }
                        break;
                    case "fase1cbb2":
                        if (fase1cbb2.SelectedIndex > -1)
                        {
                            if ((int)cbb.SelectedValue == 2)
                            {
                                fase3cbb2.Focus();
                                //fase3cbb2.IsDropDownOpen = true;
                            }
                            else
                            {
                                fase2cbb2.Focus();
                                //fase2cbb2.IsDropDownOpen = true;
                            }
                        }
                        break;
                    case "fase1cbb3":
                        if (fase1cbb3.SelectedIndex > -1)
                        {
                            if ((int)cbb.SelectedValue == 2)
                            {
                                fase3cbb3.Focus();
                                //fase3cbb3.IsDropDownOpen = true;
                            }
                            else
                            {
                                fase2cbb3.Focus();
                                //fase2cbb3.IsDropDownOpen = true;
                            }
                        }
                        break;
                    case "fase1cbb4":
                        if (fase1cbb4.SelectedIndex > -1)
                        {
                            if ((int)cbb.SelectedValue == 2)
                            {
                                fase3cbb4.Focus();
                                //fase3cbb4.IsDropDownOpen = true;
                            }
                            else
                            {
                                fase2cbb4.Focus();
                                //fase2cbb4.IsDropDownOpen = true;
                            }
                        }
                        break;
                    case "fase2cbb1":
                        if (fase1cbb1.SelectedIndex > -1)
                        {
                            if ((int)fase1cbb1.SelectedValue == 3 || (int)fase1cbb1.SelectedValue == 4)
                            {
                                fase3mcbb1.dropthelist();
                            }
                            else
                            {
                                fase3tbb1.Focus();
                            }
                        }
                        break;
                    case "fase2cbb2":
                        if (fase1cbb2.SelectedIndex > -1)
                        {
                            if ((int)fase1cbb2.SelectedValue == 3 || (int)fase1cbb2.SelectedValue == 4)
                            {
                                fase3mcbb2.dropthelist();
                            }
                            else
                            {
                                fase3tbb2.Focus();
                            }
                        }
                        break;
                    case "fase2cbb3":
                        if (fase1cbb3.SelectedIndex > -1)
                        {
                            if ((int)fase1cbb3.SelectedValue == 3 || (int)fase1cbb3.SelectedValue == 4)
                            {
                                fase3mcbb3.dropthelist();
                            }
                            else
                            {
                                fase3tbb3.Focus();
                            }
                        }
                        break;
                    case "fase2cbb4":
                        if (fase1cbb4.SelectedIndex > -1)
                        {
                            if ((int)fase1cbb4.SelectedValue == 3 || (int)fase1cbb4.SelectedValue == 4)
                            {
                                fase3mcbb4.dropthelist();
                            }
                            else
                            {
                                fase3tbb4.Focus();
                            }
                        }
                        break;
                }
            }
        }
        private void cbb_LostFocus(object sender, RoutedEventArgs e)
        {
            ComboBox cbb = sender as ComboBox;
            if (cbb.IsDropDownOpen)
            {
                cbb.IsDropDownOpen = false;
            }
            int tmpindx = cbb.SelectedIndex;
            cbb.SelectedIndex = -1;
            cbb.SelectedIndex = tmpindx;
            
            
        }

        private void cbb_GotFocus(object sender, RoutedEventArgs e)
        {
            ComboBox cbb = sender as ComboBox;
            cbb.IsDropDownOpen = true;
        }

        private void cbb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ComboBox cbb = sender as ComboBox;
            if (cbb.Items.Count > 0)
            {
                bool hit = false;
                TextBox txt = cbb.Template.FindName("PART_EditableTextBox", cbb) as TextBox;
                if (txt.SelectedText.Length > 0)
                {
                    int indx = txt.Text.Length - txt.SelectedText.Length;
                    txt.Text = txt.Text.Remove(indx);
                    txt.SelectionStart = txt.Text.Length;
                }
                string test = cbb.Text + e.Text.ToString();
                foreach (var cb_it in cbb.Items)
                {
                    if (cb_it.GetType() == typeof(MenuItem))
                    {
                        if ((cb_it as MenuItem).Text.StartsWith(test, true, System.Globalization.CultureInfo.CurrentCulture))
                        {
                            hit = true;
                        }
                    }
                    else if(cb_it.GetType() == typeof(DataRowView))
                    {
                        if((cb_it as DataRowView).Row[0].ToString().StartsWith(test,true,System.Globalization.CultureInfo.CurrentCulture))
                        {
                            hit = true;
                        }
                        else if ((cb_it as DataRowView).Row[2].ToString().StartsWith(test, true, System.Globalization.CultureInfo.CurrentCulture))
                        {
                            hit = true;
                        }
                    }
                    else if (cb_it.GetType() == typeof(AdvFilterDataset.personenlijstRow))
                    {
                        if ((cb_it as AdvFilterDataset.personenlijstRow)[0].ToString().StartsWith(test, true, System.Globalization.CultureInfo.CurrentCulture))
                        {
                            hit = true;
                        }
                    }
                    else if (cb_it.GetType() == typeof(AdvFilterDataset.bedrijvenlijstRow))
                    {
                        if ((cb_it as AdvFilterDataset.bedrijvenlijstRow)[0].ToString().StartsWith(test, true, System.Globalization.CultureInfo.CurrentCulture))
                        {
                            hit = true;
                        }
                    }

                    
                }
                if (!hit)
                {
                    e.Handled = true;
                }
                else
                {
                   //
                }
            }
        }

        private void q_lbl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement feSource = e.Source as FrameworkElement;
            switch (feSource.Name)
            {
                case "fase0lbl1":
                    if (quickcheck1.IsChecked == true)
                    {
                        quickcheck1.IsChecked = false;
                    }
                    else
                    {
                        quickcheck1.IsChecked = true;
                    }
                    break;
                case "fase0lbl2":
                    if (quickcheck2.IsChecked == true)
                    {
                        quickcheck2.IsChecked = false;
                    }
                    else
                    {
                        quickcheck2.IsChecked = true;
                    }
                    break;
                case "fase0lbl3":
                    if (quickcheck3.IsChecked == true)
                    {
                        quickcheck3.IsChecked = false;
                    }
                    else
                    {
                        quickcheck3.IsChecked = true;
                    }
                    break;
                case "fase0lbl4":
                    if (quickcheck4.IsChecked == true)
                    {
                        quickcheck4.IsChecked = false;
                    }
                    else
                    {
                        quickcheck4.IsChecked = true;
                    }
                    break;

            }
        }

        
           

        
        
    }
    public class MenuEngine
    {
        private ObservableCollection<MenuItem> _Fase1   = new ObservableCollection<MenuItem>();
        private ObservableCollection<MenuItem> _Fase2_1 = new ObservableCollection<MenuItem>();
        private ObservableCollection<MenuItem> _Fase2_2 = new ObservableCollection<MenuItem>();
        private ObservableCollection<MenuItem> _Fase2_3 = new ObservableCollection<MenuItem>();
        private ObservableCollection<MenuItem> _Fase2_4 = new ObservableCollection<MenuItem>();

        public ObservableCollection<MenuItem> Fase1
        {
            get{return _Fase1;}
        }
        public ObservableCollection<MenuItem> Fase2_1
        {
            get {return _Fase2_1;}
        }
        public ObservableCollection<MenuItem> Fase2_2
        {
            get { return _Fase2_2; }
        }
        public ObservableCollection<MenuItem> Fase2_3
        {
            get { return _Fase2_3; }
        }
        public ObservableCollection<MenuItem> Fase2_4
        {
            get { return _Fase2_4; }
        }
       public void LoadLists(int type)
       {
           AdvFilterDatasetTableAdapters.AdvFilterHelperTabelTableAdapter AFTableAdapter = new AdvFilterDatasetTableAdapters.AdvFilterHelperTabelTableAdapter();
           AFTableAdapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
           AdvFilterDataset.AdvFilterHelperTabelDataTable AFDataTable = new AdvFilterDataset.AdvFilterHelperTabelDataTable();
           AFTableAdapter.Fill(AFDataTable);

           foreach (AdvFilterDataset.AdvFilterHelperTabelRow AFRow in AFDataTable)
           {
               if (AFRow.Fase == 1)
               {
                   if (type == 2)
                   {
                       if (AFRow.ID == 1 || AFRow.ID == 44)
                       {
                           _Fase1.Add(new MenuItem
                           {
                               ID = AFRow.ID,
                               Type = AFRow.type,
                               Fase = AFRow.Fase,
                               Text = AFRow.Tekst,
                               Waarde = AFRow.waarde,
                               veldnaam = AFRow.column_exact
                           });
                       }
                   }
                   else if (type == 3)
                   {
                       if (AFRow.ID != 2 && AFRow.ID != 3 && AFRow.ID !=46)
                       {
                           _Fase1.Add(new MenuItem
                           {
                               ID = AFRow.ID,
                               Type = AFRow.type,
                               Fase = AFRow.Fase,
                               Text = AFRow.Tekst,
                               Waarde = AFRow.waarde,
                               veldnaam = AFRow.column_exact
                           });
                       }
                   }
                   else
                   {
                       if (AFRow.ID != 44)
                       {
                           _Fase1.Add(new MenuItem
                           {
                               ID = AFRow.ID,
                               Type = AFRow.type,
                               Fase = AFRow.Fase,
                               Text = AFRow.Tekst,
                               Waarde = AFRow.waarde,
                               veldnaam = AFRow.column_exact
                           });
                       }
                   }


               }
               else if (AFRow.Fase == 2)
               {
                   switch (AFRow.waarde)
                   {
                       case 1:
                           if (type == 1)
                           {
                               if (AFRow.type == 1 || AFRow.type == 0)
                               {
                                   _Fase2_1.Add(new MenuItem
                                   {
                                       ID = AFRow.ID,
                                       Type = AFRow.type,
                                       Fase = AFRow.Fase,
                                       Text = AFRow.Tekst,
                                       Waarde = AFRow.waarde,
                                       veldnaam = AFRow.column_exact
                                   });
                               }
                           }
                           else if (type == 2)
                           {
                               if (AFRow.type == 2 || AFRow.type == 0)
                               {
                                   _Fase2_1.Add(new MenuItem
                                   {
                                       ID = AFRow.ID,
                                       Type = AFRow.type,
                                       Fase = AFRow.Fase,
                                       Text = AFRow.Tekst,
                                       Waarde = AFRow.waarde,
                                       veldnaam = AFRow.column_exact
                                   });
                               }
                           }
                           else if (type == 3)
                           {
                               if (AFRow.type == 3 || AFRow.type == 0)
                               {
                                   _Fase2_1.Add(new MenuItem
                                   {
                                       ID = AFRow.ID,
                                       Type = AFRow.type,
                                       Fase = AFRow.Fase,
                                       Text = AFRow.Tekst,
                                       Waarde = AFRow.waarde,
                                       veldnaam = AFRow.column_exact
                                   });
                               }
                           }
                          
                           break;
                       case 2:
                           
                           break;
                       case 3:
                           
                           break;
                       case 4:
                          
                           break;
                   }
                }
           }
       }
    }
    public class MenuItem
    {
        public string Text     {get;set;}
        public int    Fase     {get;set;}
        public int    Type     {get;set;}
        public int    Waarde   {get;set;}
        public int    ID       {get;set;}
        public string veldnaam { get; set; }
    }
    public class AdvFilterQRYbuilder
    {
        
        SqlCommand cmd;
        private  bool[] aantal_regels ={false,false,false,false};
        private  string[] zoekterm = new string[4];
        private int[] speciaalfltr = new int[4];
        private List<DataTable> results = new List<DataTable>();
        private  int type = 0;
        private  string[] kolom_naam = new string[4];
        private  bool[] advanced = new bool[4];
        private bool[] isbedrijf = new bool[4];
        private int[] regels_functie_nrs = new int[4];
        private KeyValuePair<int, bool>[] afgeleide_functies = new KeyValuePair<int, bool>[4];
        private  string SQL = "";
        private int cntr = 0;
        private  string[] blackList = {"--",";--",";","/*","*/","@@","@"};/*,
                                           "char","nchar","varchar","nvarchar",
                                           "alter","begin","cast","create","cursor","declare","delete","drop","end","exec","execute",
                                           "fetch","insert","kill","open",
                                           "select", "sys","sysobjects","syscolumns",
                                           "table","update"};*/
        public AdvFilterQRYbuilder()
        {
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
        public DataTable start_building(int tp, bool[] rgls, string[] zktrms, string[] koL_nm, bool[] q, int[] specs, bool[] isbdrf, int[] functrenrs,KeyValuePair<int,bool>[] afgl_funct, bool[] zoekExact)
        {
            cntr = 0;
            type = tp;
            results.Clear();
            for (int i = 0; i < 4; i++)
            {
                if (rgls[i])
                {
                    cntr++;
                }
                aantal_regels[i] = rgls[i];
                speciaalfltr[i] = specs[i];
                isbedrijf[i] = isbdrf[i];
                advanced[i] = q[i];
                regels_functie_nrs[i] = functrenrs[i];
                afgeleide_functies[i] = afgl_funct[i];
                if (afgeleide_functies[i].Key > 0)
                {
                }
				else if (zoekExact[i])
				{
					zoekterm[i] = Injectioncheck(zktrms[i]);
				}
				else if (aantal_regels[i] && advanced[i] || aantal_regels[i] && speciaalfltr[i] > 0)
				{
					zoekterm[i] = Injectioncheck(zktrms[i]);
					kolom_naam[i] = Injectioncheck(koL_nm[i]);

				}
				else if (aantal_regels[i] && !q[i] || aantal_regels[i] && speciaalfltr[i] == 0)
				{
					zoekterm[i] = Injectioncheck(zktrms[i]);
					advanced[i] = q[i];
				}
			
            }
            for (int i = 0; i < 4; i++)
            {
                if(afgeleide_functies[i].Key > 0)
                {
                    cntr--;
                    results.Add(genereerQRY_afgeleide_functies(afgeleide_functies[i].Key, afgeleide_functies[i].Value));
                }
                else if (!advanced[i] && aantal_regels[i] && speciaalfltr[i] == -1)
                {
                    cntr--;
                    results.Add(uitvoerQuickF(zoekterm[i]));
                }
                else if (aantal_regels[i] && speciaalfltr[i] == 1)//combo qry
                {
                    cntr--;
                    results.Add(genereerQRY_projSpeciaal_combo(isbedrijf[i],regels_functie_nrs[i],int.Parse(zoekterm[i]), kolom_naam[i]));
                }
                else if (aantal_regels[i] && speciaalfltr[i] == 0)//varset qry
                {
                    cntr--;
                    results.Add(genereerQRY_projSpeciaal_varset(isbedrijf[i], regels_functie_nrs[i], int.Parse(zoekterm[i])));
                }
				else if (aantal_regels[i] && zoekExact[i])
				{
					cntr--;
					results.Add(genereerQRY_ZoekExact(int.Parse(zoekterm[i])));
				}


            }
			if (cntr > 0)
            {
                switch (type)
                {
                    case 1:
                        results.Add(genereerQRY_projecten());
                        break;
                    case 2:
                        results.Add(genereerQRY_bedrijven());
                        break;
                    case 3:
                        results.Add(genereerQRY_personen());
                        break;
                }
            }
           return  combineer_data();
        }
        private DataTable getDubl(DataTable ds1, DataTable ds2)
        {   DataTable tmptbl = new DataTable();
        
            if (ds1 == null || ds2 == null)
            {
                tmptbl = null;
            }
            else if(ds1== null)
            {
                tmptbl = ds2;
            }
            else if (ds2 == null)
            {
                tmptbl = ds1;
            }
            else if (ds2.Rows.Count < 1 || ds1.Rows.Count < 1)
            {
                tmptbl = null;
            }
            else if (ds1.Rows.Count <= ds2.Rows.Count)
            {
                tmptbl = ds1.Clone();
                foreach (DataRow dr in ds1.Rows)
                {
                    foreach (DataRow drx in ds2.Rows)
                    {
                        if (dr[0].ToString() == drx[0].ToString())
                        {
                            tmptbl.ImportRow(dr);

                        }
                    }
                }
            }
            else
            {
                tmptbl = ds2.Clone();
                foreach (DataRow dr in ds2.Rows)
                {
                    foreach (DataRow drx in ds1.Rows)
                    {
                        if (dr[0].ToString() == drx[0].ToString())
                        {
                            tmptbl.ImportRow(dr);
                        }
                    }
                }
            }
            return tmptbl;
        }
        private DataTable genereerQRY_projecten()
        {
            SQL = "";
            bool first = true;
            bool doeniet = true;
            SQL += "SELECT project.project_NR, project.naam_project, project.plaats, status.omschrijving, bedrijf.naam AS opdrachtgever ";
            SQL += "FROM status INNER JOIN (project LEFT JOIN bedrijf ON project.opdrachtgeverZEEBREGTS_nr=bedrijf.bedrijf_nr) ON status.omschrijving_nr=project.status ";
            SQL += "WHERE ( ";

            for (int i = 0; i < 4;i++ )
            {
                if (first)
                {
                    if (aantal_regels[i] && advanced[i] && speciaalfltr[i] == -1)
                    {
                        first = false;
                        doeniet = false;
                        SQL += "project.[" + kolom_naam[i] + "] ";
                        if (kolom_naam[i] == "status")
                        {
                            SQL += "= '" + zoekterm[i] + "' ";
                        }
                        else
                        {
                            
                            SQL += "LIKE '%" + zoekterm[i] + "%' ";
                        }
                    }
                }
                else if (aantal_regels[i] && advanced[i] && speciaalfltr[i] == -1)
                {
                    doeniet = false;
                    SQL += "AND " + "project.[" + kolom_naam[i] + "] ";
                    if (kolom_naam[i] == "status")
                    {
                        SQL += "= '" + zoekterm[i] + "' ";
                    }
                    else
                    {
                        
                        SQL += "LIKE '%" + zoekterm[i] + "%' ";
                    }
                    
                }
               
            }

            if (!doeniet)
            {
                SQL += ") ORDER BY status.planning DESC, status.volgorde ASC, opdrachtgever ASC, project.plaats ASC";
                return uitvoerSQL(SQL);
            }
            else
            {
                return null;
            }

            
        }
        private DataTable genereerQRY_bedrijven()
        {
            SQL = "";
            bool first = true;
            bool doeniet = true;
            SQL += "SELECT bedrijf.bedrijf_nr, bedrijf.zoeknaam, bedrijf.plaats ";
            SQL += "FROM bedrijf ";
            SQL += "WHERE ( ";
            for (int i = 0; i < 4; i++)
            {
                if (first)
                {
                    if (aantal_regels[i] && advanced[i] && speciaalfltr[i] < 0 && afgeleide_functies[i].Key <= 0)
                    {
                        doeniet = false;
                        first = false;
                        SQL += "bedrijf." + kolom_naam[i] + " ";
                        SQL += "LIKE '%" + zoekterm[i] + "%' ";
                    }
                }
                else if (aantal_regels[i] && advanced[i] && speciaalfltr[i] < 0 && afgeleide_functies[i].Key <= 0)
                {
                    doeniet = false;
                    SQL += "AND " + "bedrijf." + kolom_naam[i] + " ";
                    SQL += "LIKE '%" + zoekterm[i] + "%' ";
                }
            }
            SQL += ") ORDER BY bedrijf.zoeknaam, bedrijf.plaats";
            if (!doeniet)
            {
                return uitvoerSQL(SQL);
            }
            else { return null; }
        }
        private DataTable genereerQRY_personen()
        {
            SQL = "";
            bool first = true;
            int outp;
            bool doeniet = true;
            //////////
            SQL += @" SELECT P.persoon_nr, 
	                    (SELECT CAST(CASE WHEN P.man = 0 THEN 'Dhr.' ELSE 'Mvr.' END AS nvarchar))+' '+LTRIM(ISNULL(P.voorletters,'')+ ' '+ISNULL(P.voornaam,'')+' '+ISNULL(P.tussenvoegsel,'') +' '+ ISNULL(P.achternaam,'')) AS naam,
	                    B.zoeknaam,coalesce(pnrloc.weergavenaam, FuP.omschrijving) AS functie
	                    FROM persoon AS P
	                    JOIN bedrijf AS B ON P.bedrijf_nr = B.bedrijf_nr
	                    LEFT OUTER JOIN afgeleide_functies AS af ON P.persoon_nr = af.record_id AND af.type = 3
	                    LEFT OUTER JOIN persoon_nr_locaties  AS pnrloc ON af.find_key = pnrloc.regel_nr AND af.uit_var = 0
	                    LEFT OUTER JOIN functiepersoon  AS FuP ON af.find_key = FuP.functie_ID AND af.uit_var = 1";
            /////////
           // SQL += "SELECT persoon.persoon_nr, (SELECT CAST(CASE WHEN persoon.man = 0 THEN 'Dhr.' ELSE 'Mvr.' END AS nvarchar))+' '+LTRIM(ISNULL(persoon.voorletters,'')+ ' '+ISNULL(persoon.voornaam,'')+' '+ISNULL(persoon.tussenvoegsel,'') +' '+ ISNULL(persoon.achternaam,'')) AS naam, bedrijf.zoeknaam, (SELECT weergavenaam FROM persoon_nr_locaties WHERE regel_nr = (SELECT find_key FROM afgeleide_functies WHERE record_id = persoon.persoon_nr AND type = 3)) AS functie ";
            //SQL += "FROM persoon JOIN bedrijf ON (persoon.bedrijf_nr = bedrijf.bedrijf_nr) ";
            SQL += "WHERE ( ";
            for (int i = 0; i < 4; i++)
            {
                if (first)
                {
                    if (aantal_regels[i] && advanced[i] && speciaalfltr[i] < 0 && afgeleide_functies[i].Key <= 0)
                    {
                        doeniet = false;
                        first = false;
                        SQL += "P." + kolom_naam[i] + " ";
                        if(int.TryParse(zoekterm[i], out outp))
                        {
                            SQL += "= '" + zoekterm[i] + "' ";
                        }
                        else
                        {
                            SQL += "LIKE '%" + zoekterm[i] + "%' ";
                        }
                    }
                }
                else if (aantal_regels[i] && advanced[i] && speciaalfltr[i] < 0 && afgeleide_functies[i].Key <= 0)
                {
                    doeniet = false;
                    SQL += "AND " + "P." + kolom_naam[i] + " ";
                    SQL += "LIKE '%" + zoekterm[i] + "%' ";
                }
            }
            SQL += ") ORDER BY achternaam, zoeknaam, voornaam";
            if (!doeniet)
            {
                return uitvoerSQL(SQL);
            }
            else
            {
                return null;
            }
        }
        private DataTable genereerQRY_projSpeciaal_varset(bool bedrijf, int f_id, int r_id)
        {
            SQL = "";
            SQL += "SELECT zeebregtsdb.dbo.project.project_NR, zeebregtsdb.dbo.project.naam_project, zeebregtsdb.dbo.project.plaats, zeebregtsdb.dbo.status.omschrijving, zeebregtsdb.dbo.bedrijf.naam AS opdrachtgever ";
            SQL += "FROM zeebregtsdb.dbo.status INNER JOIN (zeebregtsdb.dbo.project LEFT JOIN zeebregtsdb.dbo.bedrijf ON zeebregtsdb.dbo.project.opdrachtgeverZEEBREGTS_nr=zeebregtsdb.dbo.bedrijf.bedrijf_nr) ON zeebregtsdb.dbo.status.omschrijving_nr=zeebregtsdb.dbo.project.status ";
            if (bedrijf)
            {
                SQL += "WHERE ( zeebregtsdb.dbo.project.project_NR IN (SELECT zeebregtsdb.dbo.bedrijvensetvariabel.project_nr FROM zeebregtsdb.dbo.bedrijvensetvariabel WHERE zeebregtsdb.dbo.bedrijvensetvariabel.functie_nr = " + f_id + " AND zeebregtsdb.dbo.bedrijvensetvariabel.bedrijf_nr = " + r_id + " ) )";
            }
            else
            {
                SQL += "WHERE ( zeebregtsdb.dbo.project.project_NR IN (SELECT zeebregtsdb.dbo.personensetvariabel.project_nr FROM zeebregtsdb.dbo.personensetvariabel WHERE zeebregtsdb.dbo.personensetvariabel.functie_nr = " + f_id + " AND zeebregtsdb.dbo.personensetvariabel.persoon_nr = " + r_id + ") )";
            }

            SQL += " ORDER BY zeebregtsdb.dbo.status.planning DESC, zeebregtsdb.dbo.status.volgorde ASC, opdrachtgever ASC, zeebregtsdb.dbo.project.plaats ASC";
            return uitvoerSQL(SQL);

        }
        private DataTable genereerQRY_projSpeciaal_combo(bool bedrijf, int regel_nr, int r_id, string column)
        {
            SQL = "";
            SQL += "SELECT zeebregtsdb.dbo.project.project_NR, zeebregtsdb.dbo.project.naam_project, zeebregtsdb.dbo.project.plaats, zeebregtsdb.dbo.status.omschrijving, zeebregtsdb.dbo.bedrijf.naam AS opdrachtgever ";
            SQL += "FROM zeebregtsdb.dbo.status INNER JOIN (zeebregtsdb.dbo.project LEFT JOIN zeebregtsdb.dbo.bedrijf ON zeebregtsdb.dbo.project.opdrachtgeverZEEBREGTS_nr=zeebregtsdb.dbo.bedrijf.bedrijf_nr) ON zeebregtsdb.dbo.status.omschrijving_nr=zeebregtsdb.dbo.project.status ";
            SQL += "WHERE project_NR IN ";
            if (regel_nr == 1)
            {
                if (bedrijf)
                {
                    SQL += "(SELECT zeebregtsdb.dbo.project.project_NR FROM zeebregtsdb.dbo.project WHERE ((opdrachtgeverZEEBREGTS_nr = " + r_id + ") OR (bouwbedrijf_nr = " + r_id + ") OR (kopersbegeleidingbedrijf_nr = " + r_id + ") OR (facturatieMEERWERKbedrijf_nr = " + r_id + ") OR (betalingbedrijf = " + r_id + ") OR ( tegelshowroom_nr = " + r_id + ") OR ( sanitairshowroom_nr = " + r_id + ") OR (keukenshowroom_nr = " + r_id + ") OR (projectontwikkelaar_nr = " + r_id + "))";
                    SQL += "OR zeebregtsdb.dbo.project.project_NR IN (SELECT zeebregtsdb.dbo.bedrijvensetvariabel.project_nr FROM zeebregtsdb.dbo.bedrijvensetvariabel WHERE bedrijf_nr = " + r_id + ")";
                }
                else
                {
                    SQL += "(SELECT zeebregtsdb.dbo.project.project_NR FROM zeebregtsdb.dbo.project WHERE ((inkoper_nr = " + r_id + ") OR (werkvoorbereider_nr = " + r_id + ") OR (projectleider_nr = " + r_id + ") OR (uitvoerder_nr = " + r_id + ") OR (kopersbegeleider_nr = " + r_id + ") OR ( contactpersoonTegelshowroom_nr = " + r_id + ") OR ( contactpersoonSANITAIRshowroom_nr = " + r_id + ") OR (contactpersoonKEUKENshowroom_nr = " + r_id + ") OR (offerte_persoon_nr = " + r_id + ")OR ([0offerte_persoon_nr] = " + r_id + ")OR (koperofferte_persoon_nr = " + r_id + ")OR (uitvoerderAfbouw_persoon_nr = " + r_id + ")OR (uitvoerderZeebregts_persoon_nr = " + r_id + ")OR (tegelzetter_persoon_nr = " + r_id + ")OR ([opzichter_persoon-nr] = " + r_id + "))";
                    SQL += "OR zeebregtsdb.dbo.project.project_NR IN (SELECT zeebregtsdb.dbo.personensetvariabel.project_nr FROM zeebregtsdb.dbo.personensetvariabel WHERE persoon_nr = " + r_id + ")";
                
                }
            }
            else
            {
                if (bedrijf)
                {
                    //variabel deel
                    SQL += "(SELECT zeebregtsdb.dbo.bedrijvensetvariabel.project_nr FROM zeebregtsdb.dbo.bedrijvensetvariabel WHERE bedrijf_nr = " + r_id + " AND functie_nr = (SELECT functie_ID FROM zeebregtsdb.dbo.functiebedrijven WHERE ADV_koppel_nr =" + regel_nr + "))";
                    //vast deel
                    SQL += "OR zeebregtsdb.dbo.project.project_NR IN ( SELECT zeebregtsdb.dbo.project.project_NR FROM zeebregtsdb.dbo.project WHERE [" + column + "] = " + r_id + " ";
                }
                else
                {
                    //variabel deel
                    SQL += "(SELECT zeebregtsdb.dbo.personensetvariabel.project_nr FROM zeebregtsdb.dbo.personensetvariabel WHERE persoon_nr = " + r_id + " AND functie_nr = (SELECT functie_ID FROM zeebregtsdb.dbo.functiepersoon WHERE ADV_koppel_nr =" + regel_nr + "))";
                    //vast deel
                    SQL += "OR zeebregtsdb.dbo.project.project_NR IN ( SELECT project_NR FROM zeebregtsdb.dbo.project WHERE [" + column + "] = " + r_id + " ";
                }
            }
            SQL += ") ORDER BY zeebregtsdb.dbo.status.planning DESC, zeebregtsdb.dbo.status.volgorde ASC, opdrachtgever ASC, zeebregtsdb.dbo.project.plaats ASC";
            return uitvoerSQL(SQL);
        }
        private DataTable genereerQRY_afgeleide_functies(int find_key, bool var)
        {
            DataTable dt = new DataTable();
            string sql = String.Empty;
            int uit_var;
            if (!var)
            {
                uit_var = 1;
            }
            else
            {
                uit_var = 0;
            }
            /////////////////
            switch (type)
            {
                case 2:
                    sql = @"SELECT bedrijf.bedrijf_nr, bedrijf.zoeknaam, bedrijf.plaats
                            FROM bedrijf
                            INNER JOIN afgeleide_functies
                            ON bedrijf.bedrijf_nr = afgeleide_functies.record_id
                            WHERE afgeleide_functies.type = 2 AND afgeleide_functies.find_key = "+find_key+" AND afgeleide_functies.uit_var = "+uit_var +" ORDER BY bedrijf.zoeknaam, bedrijf.plaats";
                    break;
                case 3:
                    sql = @"SELECT P.persoon_nr, 
	                        (SELECT CAST(CASE WHEN P.man = 0 THEN 'Dhr.' ELSE 'Mvr.' END AS nvarchar))+' '+LTRIM(ISNULL(P.voorletters,'')+ ' '+ISNULL(P.voornaam,'')+' '+ISNULL(P.tussenvoegsel,'') +' '+ ISNULL(P.achternaam,'')) AS naam,
	                        B.zoeknaam,coalesce(pnrloc.weergavenaam, FuP.omschrijving) AS functie
	                        FROM persoon AS P
	                        JOIN bedrijf AS B ON P.bedrijf_nr = B.bedrijf_nr
	                        LEFT OUTER JOIN afgeleide_functies AS af ON P.persoon_nr = af.record_id AND af.type = 3
	                        LEFT OUTER JOIN persoon_nr_locaties  AS pnrloc ON af.find_key = pnrloc.regel_nr AND af.uit_var = 0
	                        LEFT OUTER JOIN functiepersoon  AS FuP ON af.find_key = FuP.functie_ID AND af.uit_var = 1
                            WHERE af.type = 3 AND af.find_key = " + find_key + " AND af.uit_var = " + uit_var + " ORDER BY achternaam, zoeknaam, voornaam";
                                                                 
                    break;
            }
            /////////////////
            return uitvoerSQL(sql);
        }
		private DataTable genereerQRY_ZoekExact(int zoeknr)
		{
			string SQL= String.Empty;
			SQL += "SELECT project.project_NR, project.naam_project, project.plaats, status.omschrijving, bedrijf.naam AS opdrachtgever ";
			SQL += "FROM status INNER JOIN (project LEFT JOIN bedrijf ON project.opdrachtgeverZEEBREGTS_nr=bedrijf.bedrijf_nr) ON status.omschrijving_nr=project.status ";
			SQL += "WHERE ( project_NR = "+zoeknr ;
			SQL += ") ORDER BY status.planning DESC, status.volgorde ASC, opdrachtgever ASC, project.plaats ASC";
                
			return uitvoerSQL(SQL);
		}
		private DataTable uitvoerQuickF(string term)
        {
            try
            {
                System.Data.SqlClient.SqlConnection con = new SqlConnection();
                con.ConnectionString = Global.ConnectionString_fileserver;
                con.Open();
                if (type == 1)
                {
                    cmd = new SqlCommand("quick_project", con);
                }
                else if (type == 2)
                {
                    cmd = new SqlCommand("quick_bedrijf", con);
                }
                else if (type == 3)
                {
                    cmd = new SqlCommand("quick_persoon", con);
                }
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter zoekterm = cmd.Parameters.Add("@zoekterm", SqlDbType.VarChar, 100);
                zoekterm.Direction = ParameterDirection.Input;
                zoekterm.Value = "%" + term + "%";
                SqlDataAdapter adapt = new SqlDataAdapter();
                DataTable dt = new DataTable();
                adapt.SelectCommand = cmd;
                adapt.Fill(dt);
                con.Close();
                con.Dispose();
                return dt;
            }
            catch (Exception e)
            {
                String log_line = "crash sql @ " + DateTime.Now.ToString() + "error: " + e;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
                return null;
            }
            
        }
        private DataTable uitvoerSQL(string sql)
        {
            try
            {
                System.Data.SqlClient.SqlConnection con = new SqlConnection();
                con.ConnectionString = Global.ConnectionString_fileserver;
                con.Open();
                cmd = new SqlCommand(sql, con);
                SqlDataAdapter adapt = new SqlDataAdapter();
                DataTable dt = new DataTable();
                adapt.SelectCommand = cmd;
                adapt.Fill(dt);
                con.Close();
                con.Dispose();
                return dt;
            }
            catch(Exception e)
            {
                String log_line = "crash sql @ " + DateTime.Now.ToString() + "error: " + e;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
                return null;
            }
            
        }
        private DataTable combineer_data()
        {
            DataTable result = new DataTable();
            if (results.Count > 0)
            {
                result = results[0];
                //results.RemoveAt(0);
                foreach (DataTable dt in results)
                {
                    result = getDubl(result, dt);
                }
            }
            return result;
        }
    }
}

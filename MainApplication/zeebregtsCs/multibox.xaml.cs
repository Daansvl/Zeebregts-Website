using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Collections.Generic;
using zeebregtsCs.usercontrols;
using System.Data;
namespace zeebregtsCs
{
    /// <summary>
    /// Interaction logic for multibox.xaml
    /// </summary>
    public partial class multibox: UserControl
    {
        ObservableCollection<multiItem> items = new ObservableCollection<multiItem>();
        filterd_boxTableAdapters.bedrijf_nr_locatiesTableAdapter bdr_nr_loc_adapt = new filterd_boxTableAdapters.bedrijf_nr_locatiesTableAdapter();
        filterd_boxTableAdapters.persoon_nr_locatiesTableAdapter pers_nr_loc_adapt = new filterd_boxTableAdapters.persoon_nr_locatiesTableAdapter();
        filterd_box.bedrijf_nr_locatiesDataTable bdr_nr_loc_dt = new filterd_box.bedrijf_nr_locatiesDataTable();
        filterd_box.persoon_nr_locatiesDataTable pers_nr_loc_dt = new filterd_box.persoon_nr_locatiesDataTable();
        filterd_boxTableAdapters.Filter_adv_listTableAdapter FTA = new filterd_boxTableAdapters.Filter_adv_listTableAdapter();
        public List<int> valid_numbers = new List<int>();
        bool include_all = false;
        public int type;
        public AdvancedFilter parent;
        private int ID;
        public multibox()
        {
            InitializeComponent();
            bdr_nr_loc_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            pers_nr_loc_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            FTA.Connection.ConnectionString = Global.ConnectionString_fileserver;
            
        }
        private bool Is_in_list(filterd_box.Filter_adv_listDataTable record_nrs, int nrs)
        {
            valid_numbers.Clear();
            
            bool hit = false;
            if (include_all)
            {
                hit = true;
            }
            else
            {
                foreach (filterd_box.Filter_adv_listRow FLR in record_nrs)
                {
                    int testnr;
                    int.TryParse(FLR.nummers, out testnr);
                    if (!valid_numbers.Contains(testnr) && testnr != 0)
                    {
                        valid_numbers.Add(testnr);
                    }
                    if (testnr == nrs)
                    {
                        hit = true;
                    }
                }
            }
            return hit;
        }
        public void setTypeNparent(int tp, AdvancedFilter pa, int func_ind)
        {
            parent = pa;
            type = tp;
            if (func_ind == 0)
            {
                include_all = true;
            }
            else
            {
                include_all = false;
            }
            switch (type)
            {
                case 1://project
                    load_als_project();
                    break;
                case 2://bedrijf
                    bdr_nr_loc_adapt.Fill(bdr_nr_loc_dt);
                    AdvFilterDataset.bedrijvenlijstDataTable bldt = parent.kies_bedrijvenlijst();
                    AdvFilterDataset.bedrijvenlijstRow blr = (AdvFilterDataset.bedrijvenlijstRow)bldt.Rows[func_ind];
                    load_als_bedrijf(blr);
                    break;
                case 3://contact
                    pers_nr_loc_adapt.Fill(pers_nr_loc_dt);
                    AdvFilterDataset.personenlijstDataTable pldt = parent.kies_personenlijst();
                    AdvFilterDataset.personenlijstRow plr = (AdvFilterDataset.personenlijstRow)pldt.Rows[func_ind];
                    load_als_contact(plr);
                    break;
            }
            init_all();
        }
        private void init_all()
        {
            CollectionView collectionView = (CollectionView)CollectionViewSource.GetDefaultView(items);
             this.multibox1.ItemsSource = collectionView;
             multibox1.SelectedValuePath = "nummer";
             TextSearch.SetTextPath(this.multibox1, "naam");
        }
        public void reset()
        {
            
            multibox1.SelectedIndex = -1;
            multibox1.Text = "";
            multibox1.Visibility = System.Windows.Visibility.Hidden;
            items.Clear();
            linkbtn1.Visibility = System.Windows.Visibility.Hidden;
        }
        public void toon()
        {
            this.Visibility = System.Windows.Visibility.Visible;
            multibox1.Visibility = System.Windows.Visibility.Visible;
            linkbtn1.Visibility = System.Windows.Visibility.Visible;
            
        }
        private void load_als_project()
        {
           
            
        }
        private void load_als_bedrijf(AdvFilterDataset.bedrijvenlijstRow blr)
        {
            items.Clear();
            AdvFilterDatasetTableAdapters.bedrijfTableAdapter bdradapt = new AdvFilterDatasetTableAdapters.bedrijfTableAdapter();
            bdradapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            AdvFilterDataset.bedrijfDataTable bdrtabel = new AdvFilterDataset.bedrijfDataTable();
            bdradapt.Fill(bdrtabel);
            filterd_box.Filter_adv_listDataTable bedrijf_nrs = new filterd_box.Filter_adv_listDataTable();
            filterd_boxTableAdapters.functiebedrijvenTableAdapter fbta = new filterd_boxTableAdapters.functiebedrijvenTableAdapter();
            fbta.Connection.ConnectionString = Global.ConnectionString_fileserver;
            filterd_box.functiebedrijvenDataTable fbdt = fbta.GetData();
            if (!include_all)
            {
                
                if (blr.var == 1)
                {
                    var colnaam_houder = from i in bdr_nr_loc_dt.AsEnumerable() where i.regel_nr == blr.tmpnr select i.column;
                    string colnaam = string.Empty;
                    foreach (var x in colnaam_houder)
                    {
                        colnaam = x.ToString();
                    }
                    bedrijf_nrs = FTA.GetData(colnaam);


                    var koppl = from k in fbdt.AsEnumerable() where k.IsADV_koppel_nrNull() == false && k.ADV_koppel_nr == blr.tmpnr select k.functie_ID;
                    int functie_ID = 0;
                    foreach (var x in koppl)
                    {
                        if (int.TryParse(x.ToString(), out functie_ID))
                        {
                            bedrijf_nrs.Merge(FTA.GetDataBybedrijfvar(functie_ID));
                        }
                    }
                }
                else
                {
                    bedrijf_nrs = FTA.GetDataBybedrijfvar(blr.tmpnr);
                }
            }
            
            //////////////////
            foreach(AdvFilterDataset.bedrijfRow BdRow in bdrtabel)
            {
                
                if (Is_in_list(bedrijf_nrs,BdRow.bedrijf_nr))
                {
                    items.Add(new multiItem
                    {
                        naam = BdRow.zoeknaam,
                        informatie = BdRow.plaats,
                        nummer = BdRow.bedrijf_nr
                    });
                }
            }
        }
        private void load_als_contact(AdvFilterDataset.personenlijstRow plr)
        {
            AdvFilterDatasetTableAdapters.persoonTableAdapter persadapt = new AdvFilterDatasetTableAdapters.persoonTableAdapter();
            persadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            AdvFilterDataset.persoonDataTable perstable = new AdvFilterDataset.persoonDataTable();
            filterd_box.Filter_adv_listDataTable persoon_nrs = new filterd_box.Filter_adv_listDataTable();
            filterd_boxTableAdapters.functiepersoonTableAdapter fpta = new filterd_boxTableAdapters.functiepersoonTableAdapter();
            fpta.Connection.ConnectionString = Global.ConnectionString_fileserver;
            items.Clear();
            persadapt.Fill(perstable);
            filterd_box.functiepersoonDataTable fpdt = fpta.GetData();
            ////////////
            if (!include_all)
            {
                if (plr.var == 1)
                {
                    var colnaam_houder = from i in pers_nr_loc_dt.AsEnumerable() where i.regel_nr != 1 select i.column;
                    foreach (var x in colnaam_houder)
                    {
                       persoon_nrs.Merge(FTA.GetData(x.ToString()));
                    }

                    //var colnaam_houder = from i in pers_nr_loc_dt.AsEnumerable() where i.regel_nr == plr.tmpnr select i.column;
                    //string colnaam = string.Empty;
                    //foreach (var x in colnaam_houder)
                    //{
                    //    colnaam = x.ToString();
                    //}
                    //persoon_nrs = FTA.GetData(colnaam);

                    var koppl = from k in fpdt.AsEnumerable() where k.IsADV_koppel_nrNull() == false && k.ADV_koppel_nr == plr.tmpnr select k.functie_ID;
                    int functie_ID = 0;
                    foreach (var x in koppl)
                    {
                        if (int.TryParse(x.ToString(), out functie_ID))
                        {
                            persoon_nrs.Merge(FTA.GetDataBypersoonvar(functie_ID));
                        }
                    }
                }
                else
                {
                    persoon_nrs = FTA.GetDataBypersoonvar(plr.tmpnr);
                }
            }

            ///////////////
            foreach (AdvFilterDataset.persoonRow PRow in perstable)
            {
                if (Is_in_list(persoon_nrs, PRow.persoon_nr))
                {
                    string volnaam = "";
                    if (!PRow.IsachternaamNull())
                    {
                        volnaam += PRow.achternaam + ", ";
                    }
                    if (!PRow.IsvoorlettersNull())
                    {
                        volnaam += "(" + PRow.voorletters + ") ";
                    }
                    if (!PRow.IsvoornaamNull())
                    {
                        volnaam += PRow.voornaam + " ";
                    }
                    if (!PRow.IstussenvoegselNull())
                    {
                        volnaam += PRow.tussenvoegsel;
                    }

                    items.Add(new multiItem
                    {
                        naam = volnaam,
                        informatie = PRow.bedrijfnaam,
                        nummer = PRow.persoon_nr
                    });
                }
            }
        }
        public void dropthelist()
        {
            multibox1.Focus();
            multibox1.IsDropDownOpen = true;
        }
        private void multibox1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (multibox1.IsDropDownOpen)
                {
                  
                }
                else
                {
                    multibox1.IsDropDownOpen = true;
                }
            }
           
        }
        public void fill_id(int id)
        {
            multibox1.SelectedValue = id;
        }
        private void linkbtn1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
            parent.linkbtnclicked(this, type);
        }
        private void multibox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (multibox1.SelectedIndex > -1)
            {
                ID = (multibox1.SelectedItem as multiItem).nummer;
                parent.fase3mcbb_selectionChanged(ID, this, type);

            }
            else
            {
                parent.minder_regels(this);
                parent.zoeken();
            }
        }
        private void multibox1_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ComboBox myComboBox = sender as ComboBox;
            // Get the textbox part of the combobox
            TextBox textBox = myComboBox.Template.FindName("PART_EditableTextBox", myComboBox) as TextBox;

            // holds the list of combobox items as strings
            List<System.String> items = new List<System.String>();

            // indicates whether the new character added should be removed
            bool shouldRemove = true;

            for (int i = 0; i < myComboBox.Items.Count; i++)
            {
                items.Add(((multiItem)myComboBox.Items.GetItemAt(i)).naam.ToString());
            }
            
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

        private void multibox1_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            ComboBox cbb = sender as ComboBox;
            cbb.IsDropDownOpen = false;
            int tmpindx = cbb.SelectedIndex;
            cbb.SelectedIndex = -1;
            cbb.SelectedIndex = tmpindx;
        }

       

        

    }
    public class multiItem
    {
        public multiItem()
        {
        }

        public string naam { get; set;}
        public string informatie {get;set;}
        public int nummer { get; set; }
    }
    
}

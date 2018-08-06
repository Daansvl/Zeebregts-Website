using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Data.SqlClient;

namespace zeebregtsCs
{
    public partial class Adminscreen : base_form
    {
        base_form mParent;
        zeebregtsdbDataSet1.functiebedrijvenDataTable bdr_var_funct_table;
        zeebregtsdbDataSet1.functiepersoonDataTable pers_var_funct_table;
        zeebregtsdbDataSet1TableAdapters.bedrijvensetvariabelTableAdapter bsetvar_adapt = new zeebregtsdbDataSet1TableAdapters.bedrijvensetvariabelTableAdapter();
        zeebregtsdbDataSet1TableAdapters.personensetvariabelTableAdapter psetvar_adapt = new zeebregtsdbDataSet1TableAdapters.personensetvariabelTableAdapter();
        zeebregtsdbDataSet1TableAdapters.functiebedrijvenTableAdapter bdr_var_funct_adapt = new zeebregtsdbDataSet1TableAdapters.functiebedrijvenTableAdapter();
        zeebregtsdbDataSet1TableAdapters.functiepersoonTableAdapter pers_var_funct_adapt = new zeebregtsdbDataSet1TableAdapters.functiepersoonTableAdapter();
        zeebregtsdbDataSet1TableAdapters.bedrijf_nr_locatiesTableAdapter bdr_nr_loc_adapt = new zeebregtsdbDataSet1TableAdapters.bedrijf_nr_locatiesTableAdapter();
        zeebregtsdbDataSet1TableAdapters.persoon_nr_locatiesTableAdapter pers_nr_loc_adapt = new zeebregtsdbDataSet1TableAdapters.persoon_nr_locatiesTableAdapter();
        zeebregtsdbDataSet1.bedrijf_nr_locatiesDataTable bdr_nr_loc_table = new zeebregtsdbDataSet1.bedrijf_nr_locatiesDataTable();
        zeebregtsdbDataSet1.persoon_nr_locatiesDataTable pers_nr_loc_table = new zeebregtsdbDataSet1.persoon_nr_locatiesDataTable();
        int max_c_id;
        int max_b_id;
        int cur_b_id;
        int cur_c_id;
        int sel_c_id= -1;
        int sel_b_id = -1;
       
        public Adminscreen(base_form parent)
        {
            bsetvar_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            psetvar_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            bdr_var_funct_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            pers_var_funct_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            bdr_nr_loc_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            pers_nr_loc_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;

            mParent = parent;
            InitializeComponent();
            initialiseer();
            button5.Enabled = false;
            button6.Enabled = false;
            
        }
        
        private void initialiseer()
        {   //bedrijf list
            try
            {
                laad_invoergrid();
                laad_handelinggrid();
            }
            catch (Exception e)
            {
                log_exception(e);
            }
            try
            {
                this.Text = Global.WTitle;
                listView2.BindingContext = new BindingContext();
                bdr_var_funct_table = bdr_var_funct_adapt.GetData();
                listView2.Columns.Add("ID", 0, HorizontalAlignment.Left);
                listView2.Columns.Add("naam", 125, HorizontalAlignment.Left);
                listView2.Columns.Add("omschrijving", 200, HorizontalAlignment.Left);
                listView2.Columns.Add("volgorde", 0, HorizontalAlignment.Left);
                listView2.Columns.Add("zoek categorie",100, HorizontalAlignment.Left);
                listView2.Columns.Add("zoek cat nr", 0, HorizontalAlignment.Left);
                listView2.Columns.Add("x variabel", 65, HorizontalAlignment.Left);
                listView2.Columns.Add("x vast", 65, HorizontalAlignment.Left);
                bdr_var_funct_table = bdr_var_funct_adapt.GetData();
                listView2.Items.Clear();
                for (int i = 0; i < bdr_var_funct_table.Rows.Count; i++)
                {
                    DataRow drow = bdr_var_funct_table.Rows[i];
                    if (drow.RowState != DataRowState.Deleted)
                    {
                        ListViewItem lvi = new ListViewItem(drow["functie_ID"].ToString());
                        lvi.SubItems.Add(drow["naam"].ToString());
                        lvi.SubItems.Add(drow["omschrijving"].ToString());
                        lvi.SubItems.Add(drow["volgorde"].ToString());
                        lvi.SubItems.Add(drow["weergavenaam"].ToString());
                        lvi.SubItems.Add(drow["ADV_koppel_nr"].ToString());
                        lvi.SubItems.Add(drow["aantal_keer"].ToString());
                        int adv_nr;
                        if (int.TryParse(drow["ADV_koppel_nr"].ToString(), out adv_nr))
                        {
                            if (adv_nr > 0)
                            {
                                lvi.SubItems.Add(bdr_var_funct_adapt.count_funct_col_bdr(adv_nr).ToString());
                            }
                        }
                        listView2.Items.Add(lvi);
                    }
                }
                max_b_id = 0;
                for (int i = 0; i < listView2.Items.Count; i++)
                {
                    int tmp = int.Parse(listView2.Items[i].SubItems[0].Text);
                    if (tmp > max_b_id)
                    {
                        max_b_id = tmp;
                    }
                }
            }
            catch (Exception e)
            {
                log_exception(e);
            }
       
            //contact list
            try
            {
                listView1.BindingContext = new BindingContext();
                listView1.Columns.Add("ID", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("naam", 125, HorizontalAlignment.Left);
                listView1.Columns.Add("omschrijving", 200, HorizontalAlignment.Left);
                listView1.Columns.Add("volgorde", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("zoek categorie", 100, HorizontalAlignment.Left);
                listView1.Columns.Add("zoek cat nr", 0, HorizontalAlignment.Left);
                listView1.Columns.Add("x variabel", 65, HorizontalAlignment.Left);
                listView1.Columns.Add("x vast", 65, HorizontalAlignment.Left);
                pers_var_funct_table = pers_var_funct_adapt.GetData();
                listView1.Items.Clear();
                for (int i = 0; i < pers_var_funct_table.Rows.Count; i++)
                {
                    DataRow drow = pers_var_funct_table.Rows[i];
                    if (drow.RowState != DataRowState.Deleted)
                    {
                        ListViewItem lvi = new ListViewItem(drow["functie_ID"].ToString());
                        lvi.SubItems.Add(drow["naam"].ToString());
                        lvi.SubItems.Add(drow["omschrijving"].ToString());
                        lvi.SubItems.Add(drow["volgorde"].ToString());
                        lvi.SubItems.Add(drow["weergavenaam"].ToString());
                        lvi.SubItems.Add(drow["ADV_koppel_nr"].ToString());
                        lvi.SubItems.Add(drow["keer_gebruikt"].ToString());
                        int adv_k_n;
                        if (int.TryParse(drow["ADV_koppel_nr"].ToString(), out adv_k_n) && adv_k_n > 0)
                        {
                            if (adv_k_n > 0)
                            {
                                lvi.SubItems.Add(pers_var_funct_adapt.count_funct_col(adv_k_n).ToString());
                            }
                        }
                        listView1.Items.Add(lvi);
                    }
                }
                max_c_id = 0;
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    int tmp = int.Parse(listView1.Items[i].SubItems[0].Text);
                    if (tmp > max_c_id)
                    {
                        max_c_id = tmp;
                    }
                }

                if (sel_c_id > -1 && sel_c_id <= max_c_id)
                {
                    listView1.Focus();
                    int tmpid = sel_c_id;
                    listView1.FocusedItem = listView1.Items[tmpid];
                    listView1.EnsureVisible(tmpid);
                    listView1.Items[tmpid].Selected = true;
                }
                if (sel_b_id > -1 && sel_b_id <= max_b_id)
                {
                    listView2.Focus();
                    int tmpid = sel_b_id;
                    listView2.FocusedItem = listView2.Items[tmpid];
                    listView2.EnsureVisible(tmpid);
                    listView2.Items[tmpid].Selected = true;
                }
            }
            catch (Exception e)
            {
                log_exception(e);
            }
            pers_nr_loc_table = pers_nr_loc_adapt.GetData();
            comboBox1.BindingContext = new BindingContext();
            comboBox1.DataSource = pers_nr_loc_table ;//
            comboBox1.DisplayMember = "weergavenaam";
            comboBox1.ValueMember = "regel_nr";
            comboBox1.SelectedIndex = -1;
            bdr_nr_loc_table = bdr_nr_loc_adapt.GetData();
            comboBox2.BindingContext = new BindingContext();
            comboBox2.DataSource = bdr_nr_loc_table ;//
            comboBox2.DisplayMember = "weergavenaam";
            comboBox2.ValueMember = "regel_nr";
            comboBox2.SelectedIndex = -1;
            
        }
        private void btnterug_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Global.size = this.Size;
                Global.position = this.Location;
            }
            Global.windowstate = this.WindowState;
            this.Close();
            mParent.herlaad();
        }

        private void Adminscreen_Load(object sender, EventArgs e)
        {
            this.MinimumSize = new Size(450, 310);
     //       this.Size = Global.size;
      //      this.Location = Global.position;
     //       this.WindowState = Global.windowstate;
            Point pt = new Point(panel1.Width / 2 - lbl_mid.Width / 2, 28);
            lbl_mid.Location = pt;
            btn_dwn_lv1.Enabled = false;
            btn_up_lv1.Enabled = false;
            btn_dwn_lv2.Enabled = false;
            btn_up_lv2.Enabled = false;
           
        }

        private void Adminscreen_Resize(object sender, EventArgs e)
        {
            Point pt = new Point(panel1.Width / 2 - lbl_mid.Width / 2,28);
            lbl_mid.Location = pt;
        }
        private void reload(int i)
        {
            try
            {
                if (i == 1 && listView1.SelectedIndices.Count > 0 && sel_c_id > -1)
                {
                    sel_c_id = listView1.SelectedIndices[0];
                }
                else if (i == 2 && listView2.SelectedIndices.Count > 0 && sel_b_id > -1)
                {
                    sel_b_id = listView2.SelectedIndices[0];
                }


                listView1.Clear();
                listView2.Clear();
            }
            catch (Exception e)
            {
                log_exception(e);
            }
            initialiseer();
        }

        private void laad_invoergrid()
        {
            zeebregtsdbDataSet1TableAdapters.new_del_record_logTableAdapter NdrL_adapt = new zeebregtsdbDataSet1TableAdapters.new_del_record_logTableAdapter();
            NdrL_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            zeebregtsdbDataSet1.new_del_record_logDataTable NdrL_dt = NdrL_adapt.GetData();
            invoer_grid.DataSource = NdrL_dt;
        }
        private void laad_handelinggrid()
        {
            handelingen_log_datasetTableAdapters.adminscreen_handelingenTableAdapter ah_adapt = new handelingen_log_datasetTableAdapters.adminscreen_handelingenTableAdapter();
            ah_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            handelingen_log_dataset.adminscreen_handelingenDataTable ah_dt = ah_adapt.GetData();
            handeling_grid.DataSource = ah_dt;
        }
        private void button1_Click(object sender, EventArgs e)//voegtoe contact
        {
            try
            {
                //listBox1.Items.Add(textBox1.Text);
                if (contact_naam_tb.TextLength > 0 && contact_omschrijving_tb.TextLength > 0)
                {
                    int max_volg = int.Parse(listView1.Items[listView1.Items.Count - 1].SubItems[3].Text);
                    int koppel_nr;
                    if (comboBox1.SelectedIndex > -1)
                    {
                        int.TryParse(comboBox1.SelectedValue.ToString(), out koppel_nr);
                    
                        pers_var_funct_adapt.new_pers_var_funct(max_c_id + 1, contact_naam_tb.Text, contact_omschrijving_tb.Text, max_volg + 1, koppel_nr);
                    }
                    else
                    {
                        pers_var_funct_adapt.new_pers_var_funct(max_c_id + 1, contact_naam_tb.Text, contact_omschrijving_tb.Text, max_volg + 1, null);
                    }
                    reload(1);
                }
            }
            catch (Exception ex)
            {
                log_exception(ex);
            }
        }

        private void button2_Click(object sender, EventArgs e)//voegtoe bedrijf
        {
            try
            {
                //listBox2.Items.Add(textBox2.Text);
                if (bedrijf_naam_tb.TextLength > 0 && bedrijf_omschrijving_tb.TextLength > 0)
                {
                    int max_volg = int.Parse(listView2.Items[listView2.Items.Count - 1].SubItems[3].Text);
                    int koppel_nr;
                    if (comboBox2.SelectedIndex > -1)
                    {
                        int.TryParse(comboBox2.SelectedValue.ToString(), out koppel_nr);
                        bdr_var_funct_adapt.new_bdr_var_funct(max_b_id + 1, bedrijf_naam_tb.Text, bedrijf_omschrijving_tb.Text, max_volg + 1, koppel_nr);
                    }
                    else
                    {
                        bdr_var_funct_adapt.new_bdr_var_funct(max_b_id + 1, bedrijf_naam_tb.Text, bedrijf_omschrijving_tb.Text, max_volg + 1, null);
                    }
                    reload(2);
                }
            }
            catch (Exception ex)
            {
                log_exception(ex);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = -1;
            try
            {
                if (listView1.SelectedIndices.Count != 0)
                {

                    btn_dwn_lv1.Enabled = true;
                    btn_up_lv1.Enabled = true;
                    cur_c_id = int.Parse(listView1.SelectedItems[0].SubItems[0].Text);
                    sel_c_id = cur_c_id;
                    if (cur_c_id == int.Parse(listView1.Items[0].SubItems[0].Text))
                    {
                        btn_up_lv1.Enabled = false;
                    }
                    else if (cur_c_id == int.Parse(listView1.Items[listView1.Items.Count - 1].SubItems[0].Text))
                    {
                        btn_dwn_lv1.Enabled = false;
                    }
                    contact_naam_tb.Text = listView1.SelectedItems[0].SubItems[1].Text;
                    contact_omschrijving_tb.Text = listView1.SelectedItems[0].SubItems[2].Text;
                    if (listView1.SelectedItems[0].SubItems[5].Text != "")
                    { comboBox1.SelectedValue = listView1.SelectedItems[0].SubItems[5].Text; }
                    int result = int.Parse(psetvar_adapt.pers_functie_ingebruik(cur_c_id).ToString());
                    if (result > 0)
                    {
                        button5.Enabled = false;
                    }
                    else
                    {
                        button5.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                log_exception(ex);
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = -1;
            try
            {
                if (listView2.SelectedIndices.Count != 0)
                {
                    btn_dwn_lv2.Enabled = true;
                    btn_up_lv2.Enabled = true;
                    cur_b_id = int.Parse(listView2.SelectedItems[0].SubItems[0].Text);
                    sel_b_id = cur_b_id;
                    if (cur_b_id == int.Parse(listView2.Items[0].SubItems[0].Text))
                    {
                        btn_up_lv2.Enabled = false;
                    }
                    else if (cur_b_id == int.Parse(listView2.Items[listView2.Items.Count - 1].SubItems[0].Text))
                    {
                        btn_dwn_lv2.Enabled = false;
                    }
                    bedrijf_naam_tb.Text = listView2.SelectedItems[0].SubItems[1].Text;
                    bedrijf_omschrijving_tb.Text = listView2.SelectedItems[0].SubItems[2].Text;
                    if (listView2.SelectedItems[0].SubItems[5].Text != "")
                    {comboBox2.SelectedValue = listView2.SelectedItems[0].SubItems[5].Text;}
                    int result = int.Parse(bsetvar_adapt.bdr_functie_ingebruik(cur_b_id).ToString());
                    if (result > 0)
                    {
                        button6.Enabled = false;
                    }
                    else
                    {
                        button6.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                log_exception(ex);
            }
        }

        private void button3_Click(object sender, EventArgs e)//aanpassen contact
        {
            try
            {//cont aanp
                if (contact_naam_tb.TextLength > 0 && contact_omschrijving_tb.TextLength > 0)
                {
                    int kopplnr;
                    if(comboBox1.SelectedIndex > -1)
                    {
                        kopplnr = int.Parse(comboBox1.SelectedValue.ToString());
                    }
                    else
                    {
                        kopplnr = 0;
                    }
                    pers_var_funct_adapt.update_pers_var_funct(contact_naam_tb.Text, contact_omschrijving_tb.Text, cur_c_id, kopplnr );
                    reload(1);
                }
            }
            catch (Exception ex)
            {
                log_exception(ex);
            }
        }

        private void button4_Click(object sender, EventArgs e)//aanpassen bedrijf
        {

            
            //bdr aanp
            try
            {
                if (bedrijf_naam_tb.TextLength > 0 && bedrijf_omschrijving_tb.TextLength > 0)
                {
                    int kopplnr;
                    if (comboBox2.SelectedIndex > -1)
                    {
                        kopplnr = int.Parse(comboBox2.SelectedValue.ToString());
                    }
                    else
                    {
                        kopplnr = 0;
                    }
                    bdr_var_funct_adapt.update_bdr_var_funct(bedrijf_naam_tb.Text, bedrijf_omschrijving_tb.Text, cur_b_id, kopplnr);
                    reload(2);
                }
            }
            catch (Exception ex)
            {
                log_exception(ex);
            }
        }

        
        private void contact_naam_tb_Enter(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.BeginInvoke(new MethodInvoker(tb.SelectAll));        
        }

        private void contact_naam_tb_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (37 == e.KeyValue)
            {
                if (tb.TextLength == tb.SelectionLength)
                {
                    tb.SelectionStart = 0;
                    tb.SelectionLength = 0;
                }
            }
        }

        private void btn_up_lv1_Click(object sender, EventArgs e)
        {
            try
            {
                int start_id = cur_c_id;
                int start_volgorde = int.Parse(listView1.SelectedItems[0].SubItems[3].Text.ToString());
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    int comp_volgorde = int.Parse(listView1.Items[i].SubItems[3].Text.ToString());
                    if (comp_volgorde == start_volgorde - 1)
                    {
                        pers_var_funct_adapt.pers_funct_var_up(int.Parse(listView1.Items[i].SubItems[0].Text.ToString()), comp_volgorde + 1);

                    }


                }
                pers_var_funct_adapt.pers_funct_var_up(cur_c_id, start_volgorde - 1);
                int old = listView1.SelectedIndices[0];
                if (old - 1 >= 0)
                {
                    listView1.Items[old - 1].Selected = true;
                }

                reload(1);
            }
            catch (Exception ex)
            {
                log_exception(ex);
            }
        }

        private void btn_dwn_lv1_Click(object sender, EventArgs e)
        {
            try
            {
                int start_id = cur_c_id;
                int start_volgorde = int.Parse(listView1.SelectedItems[0].SubItems[3].Text.ToString());
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    int comp_volgorde = int.Parse(listView1.Items[i].SubItems[3].Text.ToString());
                    if (comp_volgorde == start_volgorde + 1)
                    {
                        pers_var_funct_adapt.pers_funct_var_up(int.Parse(listView1.Items[i].SubItems[0].Text.ToString()), comp_volgorde - 1);

                    }


                }
                pers_var_funct_adapt.pers_funct_var_up(cur_c_id, start_volgorde + 1);
                int old = listView1.SelectedIndices[0];
                if (old + 1 < listView1.Items.Count)
                {
                    listView1.Items[old + 1].Selected = true;
                }
                reload(1);
            }
            catch (Exception ex)
            {
                log_exception(ex);
            }
        }

        private void btn_up_lv2_Click(object sender, EventArgs e)
        {
            try
            {
                int start_id = cur_b_id;
                int start_volgorde = int.Parse(listView2.SelectedItems[0].SubItems[3].Text.ToString());
                for (int i = 0; i < listView2.Items.Count; i++)
                {
                    int comp_volgorde = int.Parse(listView2.Items[i].SubItems[3].Text.ToString());
                    if (comp_volgorde == start_volgorde - 1)
                    {
                        bdr_var_funct_adapt.bdr_var_funct_up(int.Parse(listView2.Items[i].SubItems[0].Text.ToString()), comp_volgorde + 1);

                    }


                }
                bdr_var_funct_adapt.bdr_var_funct_up(cur_b_id, start_volgorde - 1);
                int old = listView2.SelectedIndices[0];
                if (old - 1 >= 0)
                {
                    listView2.Items[old - 1].Selected = true;
                }
                reload(2);
            }
            catch (Exception ex)
            {
                log_exception(ex);
            }

        }

        private void btn_dwn_lv2_Click(object sender, EventArgs e)
        {
            try
            {
                int start_id = cur_b_id;
                int start_volgorde = int.Parse(listView2.SelectedItems[0].SubItems[3].Text.ToString());
                for (int i = 0; i < listView2.Items.Count; i++)
                {
                    int comp_volgorde = int.Parse(listView2.Items[i].SubItems[3].Text.ToString());
                    if (comp_volgorde == start_volgorde + 1)
                    {
                        bdr_var_funct_adapt.bdr_var_funct_up(int.Parse(listView2.Items[i].SubItems[0].Text.ToString()), comp_volgorde - 1);

                    }


                }
                bdr_var_funct_adapt.bdr_var_funct_up(cur_b_id, start_volgorde + 1);
                int old = listView2.SelectedIndices[0];
                if (old + 1 < listView2.Items.Count)
                {
                    listView2.Items[old + 1].Selected = true;
                }
                reload(2);
            }
            catch (Exception ex)
            {
                log_exception(ex);
            }
        }

        private void button5_Click(object sender, EventArgs e)//verw cont  
        {
            try
            {
                pers_var_funct_adapt.Del(cur_c_id);
                sel_c_id = -1;
                reload(1);
            }
            catch (Exception ex)
            {
                log_exception(ex);
            }
        }

        private void button6_Click(object sender, EventArgs e)//verw bdr
        {
            try
            {
                bdr_var_funct_adapt.Del(cur_b_id);
                sel_b_id = -1;
                reload(2);
            }
            catch (Exception ex)
            {
                log_exception(ex);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)//bedrijven
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)//personen
        {

        }

        private void ckb_UL_2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ckb_pressed = sender as CheckBox;
            if (ckb_pressed == ckb_UL_2)
            {
                if (ckb_pressed.Checked == true)
                {
                    ckb_UL_3.Checked = false;
                    Global.UserLevel = 2;
                }
                else
                {
                    ckb_UL_3.Checked = true;
                    Global.UserLevel = 3;
                }

            }
            else if (ckb_pressed == ckb_UL_3)
            {
                if (ckb_pressed.Checked == true)
                {
                    ckb_UL_2.Checked = false;
                    Global.UserLevel = 3;
                }
                else
                {
                    ckb_UL_2.Checked = true;
                    Global.UserLevel = 2;
                }
            }
           
        }

        private void invoer_grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                e.PaintBackground(e.ClipBounds, true);
                Rectangle rect = this.invoer_grid.GetColumnDisplayRectangle(e.ColumnIndex, true);
                Size titleSize = TextRenderer.MeasureText(e.Value.ToString(), e.CellStyle.Font);
                if (this.invoer_grid.ColumnHeadersHeight < titleSize.Width)
                    this.invoer_grid.ColumnHeadersHeight = titleSize.Width;
                e.Graphics.TranslateTransform(0, titleSize.Width);
                e.Graphics.RotateTransform(-90.0F);
                e.Graphics.DrawString(e.Value.ToString(), this.Font, Brushes.Black, new PointF(rect.Y, rect.X));
                e.Graphics.RotateTransform(90.0F);
                e.Graphics.TranslateTransform(0, -titleSize.Width);
                e.Handled = true;
            } 
        }

        private void handeling_grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                e.PaintBackground(e.ClipBounds, true);
                Rectangle rect = this.handeling_grid.GetColumnDisplayRectangle(e.ColumnIndex, true);
                Size titleSize = TextRenderer.MeasureText(e.Value.ToString(), e.CellStyle.Font);
                if (this.handeling_grid.ColumnHeadersHeight < titleSize.Width)
                    this.handeling_grid.ColumnHeadersHeight = titleSize.Width;
                e.Graphics.TranslateTransform(0, titleSize.Width);
                e.Graphics.RotateTransform(-90.0F);
                e.Graphics.DrawString(e.Value.ToString(), this.Font, Brushes.Black, new PointF(rect.Y, rect.X));
                e.Graphics.RotateTransform(90.0F);
                e.Graphics.TranslateTransform(0, -titleSize.Width);
                e.Handled = true;
            } 
        }

        private void button7_Click(object sender, EventArgs e)
        {
            bool useDebugDb = false;
            if (button7.Text == "Switch to Debug")
            {
                useDebugDb = true;
                button7.Text = "Switch to Live";
            }
            else
            {
                useDebugDb = false;
                button7.Text = "Switch to Debug";
            }

            Global.isRdp = System.Windows.Forms.SystemInformation.TerminalServerSession;//System.Environment.GetEnvironmentVariable("SESSIONNAME").StartsWith("RDP-");
            if (Global.isRdp)
            {
                //connection string is windows authentication is default
                if (useDebugDb)
                {
                    Global.ConnectionString_fileserver = Properties.Settings.Default.ZeebregtsDbLocalBeta;
                }
                else
                {
                    Global.ConnectionString_Mdr = "Data Source=SQL-SERVER; Initial Catalog=MandagenRegistratie; MultipleActiveResultSets=True;User ID=sa;Password=Zeebregts2013##;";
                }

            }
            else
            {
                if (useDebugDb)
                {
                    Global.ConnectionString_fileserver = Properties.Settings.Default.ZeebregtsDbRemoteBeta;
                    Global.ConnectionString_Mdr = "Data Source=SQL-SERVER; Initial Catalog=MandagenRegistratieBeta; MultipleActiveResultSets=True;User ID=sa;Password=Zeebregts2013##;";
                }
                else
                {
                    Global.ConnectionString_fileserver = Properties.Settings.Default.zeebregtsdbConnectionStringRemote;
                    Global.ConnectionString_Mdr = "Data Source=SQL-SERVER; Initial Catalog=MandagenRegistratie; MultipleActiveResultSets=True;User ID=sa;Password=Zeebregts2013##;";
                }
                //tryget usernm+pswd for sql server
                if (Properties.Settings.Default.UserName.Length < 2)
                {
                    //prompt user
                }
                else if (Properties.Settings.Default.PassWord.Length < 4)
                {
                    //promt pswd
                    // string pswd = "Blaat";
                    //var sha2 = SHA256Managed.Create();
                    //sha2.h
                }
                else
                {
                    //set connstring met ingevoerde usrname +pswd
                }
            }
            ChangeConnectionString(Global.ConnectionString_fileserver);
        }

        public static void ChangeConnectionString(string connstr)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
            connectionStringsSection.ConnectionStrings["UserConnectionString"].ConnectionString = connstr;
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");

        }

        private void ExportTelBtn_Click(object sender, EventArgs e)
        {
            var fdres = new SaveFileDialog();
            fdres.Filter = "Excel File|*.xls";
            fdres.Title = "Save an Excel File";
            fdres.ShowDialog();
            if(fdres.FileName != null)
            {
                ExportTelNrs(fdres.FileName);
            }

        }
        private string formatTelNR(string telnrIN)
        {
            var telnrOUT = "";
            telnrOUT = telnrIN.Replace("+", "00");
            telnrOUT = telnrOUT.Replace("(0)", "");
            telnrOUT = telnrOUT.Replace(" ", "");
            return telnrOUT;
        }
        private string convertTelType(string inputtype)
        {
            var result = "";
            if (inputtype.StartsWith("-") == false)
            {
                var parsed = int.Parse(inputtype);
                switch (parsed)
                {
                    case 0:
                        result = "vast";
                        break;
                    case 1:
                        result = "mobiel";
                        break;
                    case 2:
                        result = "fax";
                        break;
                    case 3:
                        result = "v.o.i.p";
                        break;
                    case 4:
                        result = "Skype";
                        break;
                    case 5:
                        result = "bedrijfsnummer";
                        break;
                    case 6:
                        result = "alternatief 1";
                        break;
                    case 7:
                        result = "alternatief 2";
                        break;
                }
            }
            return result;
        }

        private void ExportTelNrs(string path)
        {
            try
            {
                var ds = new DataSet("ContactsDataSet");
                var dt = new DataTable("ContactsDataTable");
                ds.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;
                dt.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;


                var con = new SqlConnection("Data Source=192.160.0.120;Initial Catalog=zeebregtsdb;User ID=daan;Password=Bl22sk22k!");
                con.Open();
                var command = @"SELECT persoon.voornaam,persoon.tussenvoegsel,persoon.achternaam,
                                       persoon.zaemail,persoon.telefoon_nr_1,persoon.telefoon_nr_2,
                                       persoon.telefoon_nr_3, persoon.telefoon_nr_settings, bedrijf.naam
                                       FROM persoon  INNER JOIN
                                       bedrijf ON persoon.bedrijf_nr = bedrijf.bedrijf_nr
                                       WHERE persoon.NIETactief = 'FALSE' ";
                var adptr = new SqlDataAdapter(command, con);
                adptr.Fill(dt);
                con.Close();
                ds.Tables.Add(dt);
                var customDs = new DataSet();
                var customTable = new DataTable();
                customTable.Columns.Add("Naam");
                customTable.Columns.Add("Bedrijf");
                customTable.Columns.Add("Nummer");

                ///////
                //add algemeen bedrijf nrs
                var ds_bdr = new DataSet("bedrijfDataSet");
                var dt_bdr = new DataTable("bedrijfDataTable");
                ds_bdr.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;
                dt_bdr.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;


                con.Open();
                var command_bdr = @"SELECT naam,tel
                                       FROM bedrijf
                                       WHERE NIETactief = 'FALSE' ";
                var adptr_bdr = new SqlDataAdapter(command_bdr, con);
                adptr_bdr.Fill(dt_bdr);
                con.Close();
                ds_bdr.Tables.Add(dt_bdr);
                var checkdubNrList = new List<string>();
                foreach (DataRow row in dt_bdr.Rows)
                {
                    if (row[0] != null && !String.IsNullOrEmpty(row[0].ToString()) && row[1] != null && !String.IsNullOrEmpty(row[1].ToString()))
                    {
                        customTable.Rows.Add(new Object[] { row[0].ToString() + " (Algemeen)", row[0].ToString(), formatTelNR(row[1].ToString()) });
                        checkdubNrList.Add(formatTelNR(row[1].ToString()));
                    }
                }
                ////
                ///////


               
                foreach (DataRow row in dt.Rows)
                {

                    var name = "";
                    name += row[0] != null ? row[0].ToString() : "";
                    name = name.Trim();
                    name += row[1] != null ? " " + row[1].ToString() : "";
                    name = name.Trim();
                    name += row[2] != null ? " " + row[2].ToString() : "";
                    name = name.Trim();

                    Console.WriteLine(name);
                    var bedrijfnaam = row[8] != null ? row[8].ToString() : "";
                    if (row[7] != null)
                    {
                        var telnrSettings = row[7].ToString().Split(',');
                        if (telnrSettings != null && telnrSettings.Count() == 3)
                        {
                            if (telnrSettings[1] == telnrSettings[0])
                            {
                                telnrSettings[1] = "6";
                            }
                            if (telnrSettings[2] == telnrSettings[1] || telnrSettings[2] == telnrSettings[0])
                            {
                                telnrSettings[2] = "7";
                            }
                            if (row[4] != null && String.IsNullOrEmpty(row[4].ToString()) == false && convertTelType(telnrSettings[0]) != "fax" && !checkdubNrList.Contains(formatTelNR(row[4].ToString())))
                            {
                                var fullname1 = name + " (" + convertTelType(telnrSettings[0]) + ")";
                                customTable.Rows.Add(new Object[] { fullname1, bedrijfnaam, formatTelNR(row[4].ToString()) });
                            }
                            if (row[5] != null && String.IsNullOrEmpty(row[5].ToString()) == false && convertTelType(telnrSettings[1]) != "fax" && !checkdubNrList.Contains(formatTelNR(row[5].ToString())))
                            {
                                var fullname2 = name + " (" + convertTelType(telnrSettings[1]) + ")";
                                customTable.Rows.Add(new Object[] { fullname2, bedrijfnaam, formatTelNR(row[5].ToString()) });
                            }
                            if (row[6] != null && String.IsNullOrEmpty(row[6].ToString()) == false && convertTelType(telnrSettings[2]) != "fax" && !checkdubNrList.Contains(formatTelNR(row[5].ToString())))
                            {
                                var fullname3 = name + " (" + convertTelType(telnrSettings[2]) + ")";
                                customTable.Rows.Add(new Object[] { fullname3, bedrijfnaam, formatTelNR(row[6].ToString()) });
                            }
                        }
                    }
                }

              
                customDs.Tables.Add(customTable);

                ExcelLibrary.DataSetHelper.CreateWorkbook(path, customDs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ExchangeContactsBtn_Click(object sender, EventArgs e)
        {
            var con = new SqlConnection("Data Source=192.160.0.120;Initial Catalog=zeebregtsdb;User ID=daan;Password=Bl22sk22k!");
            con.Open();
            SqlCommand command = new SqlCommand(@"SELECT persoon.voornaam,persoon.tussenvoegsel,persoon.achternaam,
                                                persoon.zaemail,persoon.telefoon_nr_1,persoon.telefoon_nr_2,
                                                persoon.telefoon_nr_3, persoon.telefoon_nr_settings, bedrijf.naam
                                                FROM persoon  INNER JOIN
                                                bedrijf ON persoon.bedrijf_nr = bedrijf.bedrijf_nr
                                                WHERE persoon.NIETactief = 'FALSE' ", con);
            var reader = command.ExecuteReader();

            var service = ExchangeComs.EWSFunctions.GetNewServiceHook();



            while (reader.Read())
            {
                try
                {
                    var cont = new ExchangeComs.ExchangeContactItem();

                    cont.Voornaam = reader[0] != null ? String.IsNullOrEmpty(reader[0].ToString()) == false ? reader[0].ToString() : "" : "";
                    cont.Tussenvoegsel = reader[1] != null ? String.IsNullOrEmpty(reader[1].ToString()) == false ? reader[1].ToString() : "" : "";
                    cont.Achternaam = reader[2] != null ? String.IsNullOrEmpty(reader[2].ToString()) == false ? reader[2].ToString() : "" : "";
                    cont.Email1 = reader[3] != null ? String.IsNullOrEmpty(reader[3].ToString()) == false ? reader[3].ToString() : "" : "";// reader[12].ToString();
                    cont.TelNr1 = reader[4] != null ? String.IsNullOrEmpty(reader[4].ToString()) == false ? reader[4].ToString() : "" : "";//reader[20].ToString();
                    cont.TelNr2 = reader[5] != null ? String.IsNullOrEmpty(reader[5].ToString()) == false ? reader[5].ToString() : "" : "";
                    cont.TelNr3 = reader[6] != null ? String.IsNullOrEmpty(reader[6].ToString()) == false ? reader[6].ToString() : "" : "";
                    cont.BedrijfNaam = reader[8] != null ? String.IsNullOrEmpty(reader[8].ToString()) == false ? reader[8].ToString() : "" : "";
                    //var contactId = ExchangeComs.EWSFunctions.FindContact(service, achternaam + "," + voornaam);
                    //if (contactId.ToString() != "0")
                    //{
                    //    ExchangeComs.EWSFunctions.DeleteContact(service, contactId);
                    //}
                    ExchangeComs.EWSFunctions.MakeNewContact(service, cont);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }


        }
       

   }
}

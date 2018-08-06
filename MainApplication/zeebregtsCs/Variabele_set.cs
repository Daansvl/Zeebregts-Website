using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MultiColumnComboBoxDemo;

namespace zeebregtsCs
{
    public partial class Variabele_set : UserControl
    {
        private bool wijzigstand = false;
        int num_rows = 0;
        int project_nr;
        int type = 0;
        int stand = 0;
        int prev_rows = 0;
        bool init_lbl = false;
        bool[] to_del = new bool[15];
        int verwijderd = 0;
        Label lbl;
        Label llbl;
        ComboBox cbb;
        MultiColumnComboBox mcbb;
        SplitContainer[] SC_ar = new SplitContainer[16];
        zeebregtsdbDataSet1.bedrijvensetvariabelRow bdr_var_row;
        zeebregtsdbDataSet1.personensetvariabelRow pers_var_row;
        zeebregtsdbDataSet1.bedrijvensetvariabelDataTable bdr_var_table;
        zeebregtsdbDataSet1.personensetvariabelDataTable pers_var_table;
        zeebregtsdbDataSet1TableAdapters.bedrijvensetvariabelTableAdapter bdr_var_adapt =  new zeebregtsdbDataSet1TableAdapters.bedrijvensetvariabelTableAdapter();
        zeebregtsdbDataSet1TableAdapters.personensetvariabelTableAdapter pers_var_adapt = new zeebregtsdbDataSet1TableAdapters.personensetvariabelTableAdapter();
        testsetTableAdapters.bedrijfTableAdapter bdr_tbl_adapt = new testsetTableAdapters.bedrijfTableAdapter();
        testsetTableAdapters.persoonTableAdapter pers_tbl_adapt =  new testsetTableAdapters.persoonTableAdapter();
        zeebregtsdbDataSet1.functiebedrijvenDataTable  bdr_var_funct_table;
        zeebregtsdbDataSet1.functiepersoonDataTable pers_var_funct_table;
        zeebregtsdbDataSet1TableAdapters.functiebedrijvenTableAdapter bdr_var_funct_adapt = new zeebregtsdbDataSet1TableAdapters.functiebedrijvenTableAdapter();
        zeebregtsdbDataSet1TableAdapters.functiepersoonTableAdapter pers_var_funct_adapt = new zeebregtsdbDataSet1TableAdapters.functiepersoonTableAdapter();
        testset.bedrijfDataTable bdr_dt_table;
        testset.persoonDataTable pers_dt_table;
        Form_helper PFSH = new Form_helper();
        bool deleting = false;
        String projnaam = "";
        project_form parent;
        Stack<int> delnrs = new Stack<int>();
        public Variabele_set()
        {
            InitializeComponent();
            bdr_var_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            pers_var_adapt.Connection.ConnectionString =Global.ConnectionString_fileserver;
            bdr_tbl_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            pers_tbl_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            bdr_var_funct_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            pers_var_funct_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;

            stand = 1;
            btn_del.Hide();
            btn_add.Hide();
        }
        public void set_type_n_projnum(int t, int pnr, project_form prnt)
        {
            type = t;
            project_nr = pnr;
            parent = prnt;
            if (type == 2)
            {
                label1.Text = "Variabele bedrijven";
            }
            else if (type == 3)
            {
                label1.Text = "Variabele contacten";
            }
            load();
        }
        public void set_projnm(String prnm)
        {
            projnaam = prnm;
        }
        private void btn_add_Click(object sender, EventArgs e)
        {
            if (deleting)
            {
                btn_del_Click(this, e);
            }
           add_line(-1,-1);
           (parent as project_form).scrolldown(this);
           //handelingen_logger.log_handeling(project_nr, 1, 3);
        }
        private void add_line(int cbbint, int mcbbint)
        {
            if (num_rows < 15)
            {
                SC_ar[num_rows] = new SplitContainer();
                SC_ar[num_rows].TabStop = false;
                
                
                if (num_rows == 0)
                {
                    SC_ar[num_rows].Location = new System.Drawing.Point(10, 0);
                     
                }
                else
                {
                    SC_ar[num_rows].Location = new System.Drawing.Point(10, SC_ar[0].Location.Y + (num_rows * 25)-(verwijderd * 25));
                    
                }
                SC_ar[num_rows].Panel1.BackColor = System.Drawing.SystemColors.Control;// System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(122)))), ((int)(((byte)(122)))));
                SC_ar[num_rows].Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
                SC_ar[num_rows].Size = new System.Drawing.Size(410, 25);
                SC_ar[num_rows].SplitterDistance = 136;
                panel1.Controls.Add(SC_ar[num_rows]);
                add_controls(SC_ar[num_rows],cbbint, mcbbint);
                num_rows += 1;
                kies_stand(stand);
                btn_add.Location = new Point(btn_add.Location.X, (SC_ar[0].Location.Y + (num_rows * 25) - (verwijderd * 25)));
               // btn_del.Location = new Point(btn_add.Location.X, btn_del.Location.Y);
            }
            else
            {
                MessageBox.Show("maximum bereikt");
            }
            
        }
        
        public void load()
        { //test if set exist!!!
            switch (type)
            {
                case 2://bedrijf
                    bdr_dt_table = bdr_tbl_adapt.GetData();
                    bdr_var_funct_table = bdr_var_funct_adapt.GetData();
                   int x = (int)bdr_var_adapt.KomtVoor(project_nr);
                   prev_rows = x;
                    if(x>0)
                    {
                       bdr_var_table = bdr_var_adapt.GetDataBy11(project_nr);
                       for (int i = 0; i < x; i++)
                       {
                           add_line(int.Parse(bdr_var_table.Rows[i].ItemArray[2].ToString()),int.Parse(bdr_var_table.Rows[i].ItemArray[3].ToString()));
                       }
                   }
                   else 
                   {
                        btn_del.Hide();
                   }
                   break;
                case 3://contact
                        pers_dt_table = pers_tbl_adapt.GetData();
                        pers_var_funct_table = pers_var_funct_adapt.GetData();
                    int y = (int)pers_var_adapt.KomtVoor(project_nr);
                    prev_rows = y;
                    if (y > 0)
                    {
                        pers_var_table = pers_var_adapt.GetDataBy1(project_nr);
                        for (int i = 0; i < y; i++)
                        {
                            add_line(int.Parse(pers_var_table.Rows[i].ItemArray[2].ToString()), int.Parse(pers_var_table.Rows[i].ItemArray[3].ToString()));
                        }
                    }
                    else { btn_del.Hide(); }
                   break;
            }
        }
        private void refresh()
        {
            for (int i = 0; i < num_rows; i++)
            {
                SC_ar[i].Dispose();
                
            }
            Array.Clear(SC_ar, 0, 16);
            Array.Clear(to_del, 0, 15);
            num_rows = 0;
            verwijderd = 0;
            btn_add.Location = new System.Drawing.Point(btn_add.Location.X, 0);
            
            load();
        }
        private void get_controls(SplitContainer SC)
        {
            foreach (Control c in SC.Panel1.Controls)
            {
                if (c is ComboBox)
                {
                    cbb = (c as ComboBox);
                }
                if (c is Label)
                {
                    lbl = (c as Label);
                }
            }
            foreach (Control c in SC.Panel2.Controls)
            {
                if (c is MultiColumnComboBox)
                {
                    mcbb = (c as MultiColumnComboBox);
                }
                if (c is Label && c.Text !="X")
                {
                    llbl = (c as Label);
                }
            }
        }
        public bool VerplichtCheck()
        {
            bool retval = true;
            if (verwijderd == num_rows) { retval = true; }
            else
            {
                for (int i = 0; i < num_rows; i++)
                {
                    get_controls(SC_ar[i]);
                    if (cbb.SelectedIndex < 0 && mcbb.SelectedIndex >= 0)
                    {
                        cbb.Focus();
                        retval = false;
                    }
                    else if (cbb.SelectedIndex >= 0 && mcbb.SelectedIndex < 0)
                    {
                        mcbb.Focus();
                        retval = false;
                    }
                    else
                    {
                        retval = true; ;
                    }
                }
            }
            return retval;
        }
        public void recalc_bij_del()
        {
            taak_bepaler tb = new taak_bepaler();
            ///////////////////////////////
            if (prev_rows > 0)
            {
                for (int i = 0; i < prev_rows; i++)
                {
                    get_controls(SC_ar[i]);

                    
                    tb.Recalc_function(int.Parse(mcbb.SelectedValue.ToString()), type);

                }
            }

            //////////////////////////////////////////
            for (int i = prev_rows; i < num_rows; i++)
            {
                if (!to_del[i])
                {
                    get_controls(SC_ar[i]);
                    if (cbb.SelectedIndex >= 0 || mcbb.SelectedIndex >= 0)
                    {

                        tb.Recalc_function(int.Parse(mcbb.SelectedValue.ToString()), type);
                    }
                }

            }
            //////////////////////////////////////////
            for (int j = 0; j < prev_rows; j++)
            {
                if (to_del[j])
                {
                   
                    tb.Recalc_function(int.Parse(mcbb.SelectedValue.ToString()), type);
                }
            }
            /////////////////////////////////////////

           
         
        }
        public void save()
        {
            taak_bepaler tb = new taak_bepaler();
            ///////////////////////////////
            if (prev_rows > 0)
            {
                for (int i = 0; i < prev_rows; i++)
                {
                    get_controls(SC_ar[i]);
                   
                        switch (type)
                        {
                            case 1://proj
                                break;
                            case 2://bdr

                                bdr_var_row = (zeebregtsdbDataSet1.bedrijvensetvariabelRow)bdr_var_table.Rows[i];
                                
                                    if(int.Parse(mcbb.SelectedIndex.ToString()) > -1)
                                    {
                                        bdr_var_row.bedrijf_nr = int.Parse(mcbb.SelectedValue.ToString());
                                        bdr_var_row.functie_nr = int.Parse(cbb.SelectedValue.ToString());
                                        bdr_var_adapt.Update(bdr_var_row);
                                        
                                    }
                                
                                break;
                            case 3:
                                pers_var_row = (zeebregtsdbDataSet1.personensetvariabelRow)pers_var_table.Rows[i];
                                if (int.Parse(mcbb.SelectedIndex.ToString()) > -1)
                                {
                                    pers_var_row.persoon_nr = int.Parse(mcbb.SelectedValue.ToString());
                                    pers_var_row.functie_nr = int.Parse(cbb.SelectedValue.ToString());
                                    pers_var_adapt.Update(pers_var_row);
                                }
                                break;
                        }
                        tb.Recalc_function(int.Parse(mcbb.SelectedValue.ToString()), type);
                   
                }
            }
            
            //////////////////////////////////////////
            for (int i = prev_rows; i < num_rows; i++)
            {
                if (!to_del[i])
                {
                    get_controls(SC_ar[i]);
                    if (cbb.SelectedIndex >= 0 || mcbb.SelectedIndex >= 0)
                    {
                        
                        switch (type)
                        {
                            case 1://proj
                                break;
                            case 2://bdr
                                bdr_var_adapt.new_var_bdr(project_nr, int.Parse(cbb.SelectedValue.ToString()), int.Parse(mcbb.SelectedValue.ToString()));
                                break;
                            case 3://cont
                                pers_var_adapt.new_var_pers(project_nr, int.Parse(cbb.SelectedValue.ToString()), int.Parse(mcbb.SelectedValue.ToString())); ;
                                break;
                        }
                        tb.Recalc_function(int.Parse(mcbb.SelectedValue.ToString()), type);
                    }
                }

            }
            //////////////////////////////////////////
            for (int j = 0; j < prev_rows; j++)
            {
                if (to_del[j])
                {
                    switch (type)
                    {
                        case 1:
                            break;
                        case 2:
                            bdr_var_row = (zeebregtsdbDataSet1.bedrijvensetvariabelRow)bdr_var_table.Rows[j];
                            bdr_var_adapt.Delete_bdr_var_entry(bdr_var_row.bedrijvenset_ID);
                            break;
                        case 3:
                            pers_var_row = (zeebregtsdbDataSet1.personensetvariabelRow)pers_var_table.Rows[j];
                            pers_var_adapt.Delete_pers_var_entry(pers_var_row.personenSet_ID);
                            break;
                    }
                    tb.Recalc_function(int.Parse(mcbb.SelectedValue.ToString()), type);
                }
            }
            /////////////////////////////////////////
            
                refresh();
            
        }
        private void btn_del_Click(object sender, EventArgs e)
        {
            
            if (deleting)
            {
                (parent as project_form).resume_autosize(this);
                deleting = false;
                btn_add.Show();
                btn_del.Text = "Verwijderen";
                int cnt = delnrs.Count;
                for (int i = 0; i < cnt; i++)
                {
                    collaps(delnrs.Pop());
                }
                foreach (Control c in panel1.Controls)
                {
                    if (c is SplitContainer)
                    {
                        (c as SplitContainer).Panel1.Enabled = true;
                        foreach (Control cc in (c as SplitContainer).Panel2.Controls)
                        {
                            if (cc is Label)
                            {
                            }
                            else
                            {
                                cc.Enabled = true;
                            }
                        }
                    }
                }
                for (int i = 0; i < num_rows; i++)
                {
                    SplitContainer SC = SC_ar[i];
                    foreach (Control c in SC.Panel2.Controls)
                    {
                        if (c is Button)
                        {
                            c.Show();
                        }
                        else if (c is Label && c.Text == "X")
                        {
                            c.Hide();
                        }
                    }
                }
            }
            else
            {
                
                btn_add.Hide();
                btn_del.Text = "Gereed";
                foreach (Control c in panel1.Controls)
                {
                    if (c is SplitContainer)
                    {
                        (c as SplitContainer).Panel1.Enabled = false;
                        foreach (Control cc in (c as SplitContainer).Panel2.Controls)
                        {
                            if (cc is Label)
                            {
                            }
                            else
                            {
                                cc.Enabled = false;
                            }
                        }
                    }
                }
                deleting = true;
                for (int i = 0; i < num_rows;i++)
                {
                    SplitContainer SC = SC_ar[i];
                    foreach (Control c in SC.Panel2.Controls)
                    {
                        if (c is Button)
                        {
                            c.Hide();
                        }
                        else if (c is Label && c.Text == "X")
                        {
                            c.Show();
                            c.Enabled = true;
                        }
                    }
                }
            }
        }
        public void UpdateCBBs()
        {
            mcbdict.Clear();
            Save_cbb(this);
            bdr_dt_table = bdr_tbl_adapt.GetData();
            pers_dt_table = pers_tbl_adapt.GetData();
            for (int i = 0; i < num_rows; i++)
            {
                SplitContainer SC = SC_ar[i];
                foreach (Control c in SC.Panel2.Controls)
                {
                    if (c is MultiColumnComboBox)
                    {
                        switch (type)
                        {
                            case 1:
                                break;
                            case 2:
                                (c as MultiColumnComboBox).DataSource = bdr_dt_table;
                                (c as MultiColumnComboBox).DisplayMember = "zoeknaam";
                                (c as MultiColumnComboBox).ValueMember = "bedrijf_nr";
                                break;
                            case 3:
                                (c as MultiColumnComboBox).DataSource = pers_dt_table;
                                (c as MultiColumnComboBox).DisplayMember = "SearchName";
                                (c as MultiColumnComboBox).ValueMember = "persoon_nr";
                                break;
                        }
                        c.Invalidate();

                    }
                }
            }
            Load_cbb();
        }
        private void add_controls(SplitContainer SC, int cbbint, int mcbbint)
        {
            ComboBox cb1 = new ComboBox();
            
            cb1.Location = new Point(6,2);
            MultiColumnComboBox mcb2 = new MultiColumnComboBox();
            mcb2.Location = new Point(6, 2);
            mcb2.Size = new Size(235, 21);
            mcb2.AutoComplete = true;
            mcb2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            mcb2.AutoDropdown = true;
            mcb2.DropDownHeight = 106;
            mcb2.DropDownWidth = 235;
            
            if (type == 2)//bedrijf
            {
                mcb2.ColumnWidths = "0;200;0;100;0";
                mcb2.BindingContext = new BindingContext();
                mcb2.DataSource = bdr_dt_table;
                mcb2.DisplayMember = "zoeknaam";
                mcb2.ValueMember = "bedrijf_nr";
                mcb2.SelectedIndex = -1;
                cb1.BindingContext = new BindingContext();
                cb1.DataSource = bdr_var_funct_table;
                cb1.DisplayMember = "naam";
                cb1.ValueMember = "functie_ID";
                cb1.SelectedIndex = -1;
                
            }
            else if (type == 3)//contact
            {
                mcb2.ColumnWidths = "0;200;0;200";
                mcb2.BindingContext = new BindingContext();
                mcb2.DataSource = pers_dt_table;
                mcb2.DisplayMember = "SearchName";
                mcb2.ValueMember = "persoon_nr";
                mcb2.SelectedIndex = -1;
                cb1.BindingContext = new BindingContext();
                cb1.DataSource = pers_var_funct_table;
                cb1.DisplayMember = "naam";
                cb1.ValueMember = "functie_ID";
                cb1.SelectedIndex = -1;
            }
           
            Label lbl1 = new Label();
            
            lbl1.Location = new Point(6, 6);
            lbl1.AutoSize = true;
            lbl1.AutoEllipsis = true;
            Label lbl2 = new Label();
            lbl2.AutoSize = true;
            lbl2.Location = new Point(6, 6);
            
                if (cbbint < 0 && mcbbint < 0)
                {
                    init_lbl = true;
                    lbl2.Text = "nog niet toegekent";
                    lbl1.Text = "nog niet toegekent";
                }
                else
                {
                    init_lbl = false;

                    foreach (DataRow dr in ((DataTable)mcb2.DataSource).Rows)
                    {
                        if (type == 2)
                        {
                            if ((int)dr["bedrijf_nr"] == mcbbint)
                            {
                                mcb2.SelectedValue = mcbbint;
                                DataRowView drv = (DataRowView)mcb2.SelectedItem;
                                lbl2.Text = drv.Row.ItemArray[4].ToString();
                            }
                        }
                        else if (type == 3)
                        {
                            if ((int)dr["persoon_nr"] == mcbbint)
                            {
                                mcb2.SelectedValue = mcbbint;
                                DataRowView drv = (DataRowView)mcb2.SelectedItem;
                                lbl2.Text = drv.Row.ItemArray[2].ToString();
                            }
                        
                        }
                    }
                   
                    foreach (DataRow dr2 in ((DataTable)cb1.DataSource).Rows)
                    {
                        if ((int)dr2["functie_ID"] == cbbint)
                        {
                            cb1.SelectedValue = cbbint;
                            lbl1.Text = cb1.Text;
                        }
                    }
                }
            
            Button btn = new Button();
            btn.TabStop = false;
            btn.Image = Properties.Resources.new_pijl;
            btn.Location = new Point(246, 1);
            btn.Text = "";
            btn.Size = new Size(21, 21);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += new EventHandler(btn_Click);
            btn.MouseEnter += new EventHandler(btn_MouseEnter);
            btn.MouseLeave += new EventHandler(btn_MouseLeave);
            Label del_lbl = new Label();
            del_lbl.Click += new System.EventHandler(this.del_Click);
            del_lbl.Location = new Point(246, 3);
            del_lbl.Text = "X";
            del_lbl.ForeColor = Color.DarkGray;
            del_lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12,FontStyle.Bold);
            SC.Panel1.Controls.Add(cb1);
            SC.Panel2.Controls.Add(mcb2);
            SC.Panel1.Controls.Add(lbl1);
            SC.Panel2.Controls.Add(lbl2);
            SC.Panel2.Controls.Add(btn);
            SC.Panel2.Controls.Add(del_lbl);
           
        }

        void btn_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.FlatStyle = FlatStyle.Flat;
        }

        void btn_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.FlatStyle = FlatStyle.Standard;
        }

        
        
        Dictionary<MultiColumnComboBox,int> mcbdict = new Dictionary<MultiColumnComboBox,int>();
        private void Save_cbb(Control parent)
        {
            
            foreach (Control c in parent.Controls)
            {
                if (c is MultiColumnComboBoxDemo.MultiColumnComboBox)
                {
                    if ((c as MultiColumnComboBoxDemo.MultiColumnComboBox).SelectedIndex < 0)
                    { mcbdict.Add((c as MultiColumnComboBoxDemo.MultiColumnComboBox), -1); }
                    else
                    {
                        int a;
                        int.TryParse((c as MultiColumnComboBoxDemo.MultiColumnComboBox).SelectedValue.ToString(), out a);
                        mcbdict.Add((c as MultiColumnComboBoxDemo.MultiColumnComboBox), a);
                    }
                }
                if (c.Controls.Count > 0)
                {
                    Save_cbb(c);
                }
            }
        }
        private void Load_cbb()
        {
            foreach (MultiColumnComboBoxDemo.MultiColumnComboBox mcb in mcbdict.Keys)
            {
                int b;
                mcbdict.TryGetValue(mcb, out b);
                if (b == -1)
                {
                    mcb.SelectedIndex = -1;
                }
                else
                {
                    mcb.SelectedValue = b;
                }
            }
        }
        void lbl2_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Cursor.Current = Cursors.WaitCursor;
            if (parent.WindowState == FormWindowState.Normal)
            {
                Global.size = parent.Size;
                Global.position = parent.Location;
            }
            Global.windowstate = parent.WindowState;
            LinkLabel tmplbl = (LinkLabel)sender;
            SplitContainer SC = (SplitContainer)tmplbl.Parent.Parent;
            string vnm= "";
            for (int i = 0; i < num_rows; i++)
            {
                if (SC == SC_ar[i])
                {
                    get_controls(SC_ar[i]);
                    string temp = mcbb.SelectedValue.ToString();
                    switch (type)
                    {
                        case 1:
                            break;
                        case 2:
                             bdr_var_funct_table = bdr_var_funct_adapt.GetData();
                             vnm = bdr_var_funct_table.Rows[int.Parse(cbb.SelectedIndex.ToString())].ItemArray[1].ToString()+ " ";
                            break;
                        case 3:
                             pers_var_funct_table = pers_var_funct_adapt.GetData();
                             vnm = pers_var_funct_table.Rows[int.Parse(cbb.SelectedIndex.ToString())].ItemArray[1].ToString() + " ";
                            break;
                    }
                   
                    Global.edit_form_pers = false;
                    Global.overzicht_select = temp;
                    vnm = vnm + " ";
                    PFSH.Start_route(type, 2,parent, parent, project_nr, vnm, projnaam);
                    Cursor.Current = Cursors.Default;
                    parent.Hide();
                }
            }

        }

        void btn_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Button tmpbtn = (Button)sender;
            if (wijzigstand)
            {
                //throw new NotImplementedException();
                mcbdict.Clear();
                Save_cbb(this);
                (parent as project_form).SaveCBB_extern();
                if (parent.WindowState == FormWindowState.Normal)
                {
                    Global.size = parent.Size;
                    Global.position = parent.Location;
                }
                Global.windowstate = parent.WindowState;
               
                Global.give_return = true;
                SplitContainer SC = (SplitContainer)tmpbtn.Parent.Parent;
                string vnm = "";

                for (int i = 0; i < num_rows; i++)
                {
                    if (SC == SC_ar[i])
                    {
                        get_controls(SC_ar[i]);
                        vul_hier = mcbb;

                        switch (type)
                        {
                            case 1:
                                break;
                            case 2:
                                (parent as project_form).combox = -2;
                                if (cbb.SelectedIndex >= 0)
                                {
                                    bdr_var_funct_table = bdr_var_funct_adapt.GetData();
                                    vnm = bdr_var_funct_table.Rows[int.Parse(cbb.SelectedIndex.ToString())].ItemArray[1].ToString() + " ";
                                }
                                else
                                {
                                    vnm = "een nog nader te bepalen bedrijfs-functie ";
                                }
                                break;
                            case 3:
                                (parent as project_form).combox = -3;
                                if (cbb.SelectedIndex >= 0)
                                {
                                    pers_var_funct_table = pers_var_funct_adapt.GetData();
                                    vnm = pers_var_funct_table.Rows[int.Parse(cbb.SelectedIndex.ToString())].ItemArray[1].ToString() + " ";
                                }
                                else
                                {
                                    vnm = "een nog nader te bepalen contact-functie ";
                                }
                                break;
                        }
                        PFSH.Start_route(type, 3, parent, parent, project_nr, vnm, projnaam);
                        parent.Hide();
                        Global.give_return = false;
                    }
                }
            }
            else
            {
               SplitContainer SC = (SplitContainer)tmpbtn.Parent.Parent;
                string vnm = "";
                for (int i = 0; i < num_rows; i++)
                {
                    if (SC == SC_ar[i])
                    {
                        get_controls(SC_ar[i]);
                        vul_hier = mcbb;
                        string temp = mcbb.SelectedValue.ToString();
                        switch (type)
                        {
                            case 1:
                                break;
                            case 2:
                                (parent as project_form).combox = -2;
                                bdr_var_funct_table = bdr_var_funct_adapt.GetData();
                                vnm = bdr_var_funct_table.Rows[int.Parse(cbb.SelectedIndex.ToString())].ItemArray[1].ToString() + " ";
                                break;
                            case 3:
                                (parent as project_form).combox = -3;
                                pers_var_funct_table = pers_var_funct_adapt.GetData();
                                vnm = pers_var_funct_table.Rows[int.Parse(cbb.SelectedIndex.ToString())].ItemArray[1].ToString() + " ";
                                break;
                        }

                        Global.edit_form_pers = false;
                        Global.overzicht_select = temp;
                        vnm = vnm + " ";
                        PFSH.Start_route(type, 2, parent, parent, project_nr, vnm, projnaam);
                        
                        parent.Hide();
                    }
                }

            }
            Cursor.Current = Cursors.Default;
        }
        private MultiColumnComboBox vul_hier;
        public void fill_id(int retval)
        {
            bdr_dt_table = bdr_tbl_adapt.GetData();
            pers_dt_table = pers_tbl_adapt.GetData();
            for (int i = 0; i < num_rows; i++)
            {
                SplitContainer SC = SC_ar[i];
                foreach (Control c in SC.Panel2.Controls)
                {
                    if (c is MultiColumnComboBox)
                    {
                        switch(type)
                        {
                            case 1:
                                break;
                            case 2:
                                (c as MultiColumnComboBox).DataSource = bdr_dt_table ;
                                (c as MultiColumnComboBox).DisplayMember = "zoeknaam";
                                (c as MultiColumnComboBox).ValueMember = "bedrijf_nr";
                                break;
                            case 3:
                                (c as MultiColumnComboBox).DataSource = pers_dt_table;
                                (c as MultiColumnComboBox).DisplayMember = "SearchName";
                                (c as MultiColumnComboBox).ValueMember = "persoon_nr";
                                break;
                        }
                        c.Invalidate();
                        
                    }
                }
            }
            Load_cbb();
            vul_hier.SelectedValue = retval;
        }
        
        private void del_Click(System.Object sender, EventArgs e)
        {
            (parent as project_form).hold_autosize(this);
            Label tmplbl = (Label)sender;
            SplitContainer SC = (SplitContainer)tmplbl.Parent.Parent;
            verwijderd += 1;
            for (int i = 0; i < num_rows; i++)
            {
                if (SC == SC_ar[i])
                {
                    delnrs.Push(i);
                    SC.Hide();
                    to_del[i] = true;
                    //collaps(i);
                    //btn_add.Location = new System.Drawing.Point(btn_add.Location.X, btn_add.Location.Y-33);   
                }
            }
            
        }
        private void collaps(int SC_gat)
        {
            SC_ar[SC_gat].Hide();
            
            for (int i = SC_gat + 1; i < num_rows; i++)
            {
                SC_ar[i].Location = new System.Drawing.Point(10, SC_ar[i].Location.Y - 25);
                btn_add.Location = new System.Drawing.Point(btn_add.Location.X, SC_ar[i].Location.Y + 25);   
            }
            if (SC_gat + 1 == num_rows)
            {
                btn_add.Location = new Point(btn_add.Location.X, btn_add.Location.Y - 25);
            }
            
        }
        public void kies_stand(int enr)
        {
            
            if (enr == 1)
            {
                wijzigstand = false;
                lees_stnd();
            }
            else if (enr == 2)
            {
                wijzigstand = true;
                wijzig_stnd();
            }
        }
        private void wijzig_stnd()
        {
            stand = 2;

            if (num_rows > 0)
            { btn_del.Show(); }
            else
            {
                btn_del.Hide();
            }
            btn_add.Show();

            for (int i = 0; i < num_rows; i++)
            {
                SplitContainer SC = SC_ar[i];
                //   
                foreach (Control c in SC.Panel2.Controls)
                {
                    if (c is MultiColumnComboBox)
                    {
                        c.Show();
                    }
                    else if (c is Label)
                    {
                        c.Hide();
                    }

                }
                foreach (Control c in SC.Panel1.Controls)
                {
                    if (c is ComboBox)
                    {
                        c.Show();
                    }
                    else if (c is Label)
                    {
                        c.Hide();
                    }

                }
            }
            
        }
        private void lees_stnd()
        {
            stand = 1;
            btn_del.Hide();
            btn_add.Hide();

            if (deleting)
            {
                EventArgs e = new EventArgs();
                btn_del_Click(this, e);
            }
            for (int i = 0; i < num_rows; i++)
            {
                SplitContainer SC = SC_ar[i];
                //  SC.Location = new Point(SC.Location.X, SC.Location.Y - 33);
                foreach (Control c in SC.Panel2.Controls)
                {
                    if (c is MultiColumnComboBox)
                    {
                        mcbb = (c as MultiColumnComboBox);
                        c.Hide();
                    }
                    else if (c is Label && c.Text != "X")
                    {
                        llbl = (c as Label);
                        c.Show();
                    }
                    else if (c is Label)
                    {
                        c.Hide();
                    }
                }
                foreach (Control c in SC.Panel1.Controls)
                {
                    if (c is ComboBox)
                    {
                        cbb = (c as ComboBox);
                        c.Hide();
                    }
                    else if (c is Label)
                    {
                        lbl = (c as Label);
                        c.Show();
                    }
                }
                if (!init_lbl)
                {
                    if (mcbb.SelectedIndex > 0)
                    {
                        DataRowView drv = (DataRowView)mcbb.SelectedItem;
                        if (type == 2)
                        { llbl.Text = drv.Row.ItemArray[4].ToString(); }
                        else if (type == 3)
                        { llbl.Text = drv.Row.ItemArray[2].ToString(); }
                    }
                    lbl.Text = cbb.Text;
                }
            }
        }
        public void annuleer()
        {
            stand = 1;
            kies_stand(1);
            refresh();
        }

        private void bnt_annuleer_Click(object sender, EventArgs e)
        {
            if (deleting)
            {
                btn_del_Click(this, e);
            }
            Array.Clear(to_del, 0, 15);
            verwijderd = 0;
            if (num_rows > 0)
            {
                SC_ar[0].Location = new System.Drawing.Point(6, 8);
                SC_ar[0].Show();
                for (int i = 1; i < num_rows; i++)
                {
                    SC_ar[i].Show();
                    SC_ar[i].Location = new System.Drawing.Point(6, SC_ar[0].Location.Y +(i * 33));
                    btn_add.Location = new System.Drawing.Point(btn_add.Location.X, SC_ar[i].Location.Y + 33);


                }
            }
        }

        private void Variabele_set_Leave(object sender, EventArgs e)
        {
            if (deleting)
            {
                btn_del_Click(sender, e);
            }
        }

        
        
       
    }
}

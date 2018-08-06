using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace zeebregtsCs
{
    public partial class instellingen_zb : Form
    {
        System.Data.SqlClient.SqlConnection con;
        private String[] connection_strings = new String[2]{"Data Source=SQL-SERVER;Initial Catalog=zeebregtsdb;User ID=daan;Password=Bl22sk22k!","Data Source=SQL-SERVER;Initial Catalog=zeebregtsdb;MultipleActiveResultSets=True;Integrated Security=SSPI"} ;
        
        base_form mParent;
        public instellingen_zb(base_form parent)
        {
            mParent = parent;
            InitializeComponent();
        }
        private void instellingen_zb_Load(object sender, EventArgs e)
        {

            con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = Global.ConnectionString_fileserver;
            huidig_log();
            
            
        }
        private void btn_terug_Click(object sender, EventArgs e)
        {
            
            this.Close();
        }

        private void btn_import_Click(object sender, EventArgs e)
        {
            con.Close();
            con.Open();
            string stopro = "";



                stopro = "import_test";
                SqlCommand command = new SqlCommand(stopro, con);
                command.CommandType = CommandType.StoredProcedure;

                int numrow = command.ExecuteNonQuery();
                if (numrow > 0)
                {
                    MessageBox.Show(numrow.ToString() + " rows affected");
                }
           
            con.Close();
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            con.Close();
            con.Open();
            string stopro = "";



            stopro = "update_test";
            SqlCommand command = new SqlCommand(stopro, con);
            command.CommandType = CommandType.StoredProcedure;

            int numrow = command.ExecuteNonQuery();
            if (numrow > 0)
            {
                MessageBox.Show(numrow.ToString() + " rows affected");
            }
            

            con.Close();
        }

       

        private void instellingen_zb_FormClosing(object sender, FormClosingEventArgs e)
        {

            FormManager.GetMenu().herlaad();
           
        }

        private void btn_vergelijk_Click(object sender, EventArgs e)
        {
            con.Close();
            con.Open();
            string stopro = "";



            stopro = "vergelijk_test";
            SqlCommand command = new SqlCommand(stopro, con);
            command.CommandType = CommandType.StoredProcedure;

            int numrow = command.ExecuteNonQuery();
            if (numrow > 0)
            {
                MessageBox.Show(numrow.ToString() + " rows affected");
            }

            con.Close();
        }

        private void connectionstringchange_btn_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == 1)
            {
                Global.ConnectionString_fileserver = connection_strings[1];
            }
            else
            {
                Global.ConnectionString_fileserver = connection_strings[0];
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Global.ConnectionString_fileserver);
        }

        private void button2_Click(object sender, EventArgs e)//kies logfile
        {
            OpenFileDialog ofd = new OpenFileDialog();
            string selected_file = "";
            ofd.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            ofd.FileName = "";
            ofd.ShowDialog();
            selected_file = ofd.FileName;
            textBox1.Text = selected_file;
            huidig_log();
           
        }
        private void huidig_log()
        {
            label3.Text = Global.log_file_path;
        }
        private void button3_Click(object sender, EventArgs e)//change
        {
            Global.log_file_path = textBox1.Text;
            huidig_log();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Global.NoConnection = true;
            }
            else
            {
                Global.NoConnection = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //overview1 tmp = new overview1((Global.FW_menu() as Menu));
           // project_form test_tmp = new project_form(tmp);
          //  test_tmp.Show();
           // this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
           // overview1 tmp = new overview1((Global.FW_menu() as Menu));
            //bedrijf_form test_tmp = new bedrijf_form(tmp);
          //  test_tmp.Show();
            //this.Close();

        }

        private void button6_Click(object sender, EventArgs e)
        {
          //  overview1 tmp = new overview1((Global.FW_menu() as Menu));
           // persoon_form test_tmp = new persoon_form(tmp);
          //  test_tmp.Show();
           // this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Adminscreen adminscrn = new Adminscreen(mParent);
            adminscrn.Show();
            this.Hide();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (listBox2.SelectedIndex)
            {
                case 0:
                    Global.overzicht_type = 1;
                    break;
                case 1:
                    Global.overzicht_type = 2;
                    break;
                case 2:
                    Global.overzicht_type = 3;
                    break;

            }
        }

		private void button8_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("ZEKER WETEN>>>!!!!!!!", "MegaTabelMaak", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
			{
				var conn = new System.Data.SqlClient.SqlConnection();
				conn.ConnectionString = "Data Source=SQL-SERVER;Initial Catalog=zeebregtsdb;User ID=daan;Password=Bl22sk22k!";
				conn.Open();
				for (int i = 0; i < 100000; i++)
				{
					var r = new Random(DateTime.Now.Second);
					string sql = "INSERT INTO StresTestTable ([Spoef]) VALUES (" + r.GetHashCode().ToString() + ")";
					SqlCommand command = conn.CreateCommand();
					command.CommandText = sql;
					command.ExecuteNonQuery();
				}
				conn.Close();
			}
		}

        private void ExportContactsBtn_Click(object sender, EventArgs e)
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
                    cont.BedrijfNaam = reader[8] != null ? String.IsNullOrEmpty(reader[8].ToString()) == false ? reader[8].ToString() : "" :"";
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

        
        private void button9_Click(object sender, EventArgs e)
        {
            var fdres = new SaveFileDialog();
            fdres.Filter = "Excel File|*.xls";
            fdres.Title = "Save an Excel File";
            fdres.ShowDialog();
            if (fdres.FileName != null)
            {
                ExportTelNrs(fdres.FileName);
            }
//            try
//            {
//                var ds = new DataSet("ContactsDataSet");
//                var dt = new DataTable("ContactsDataTable");
//                ds.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;
//                dt.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;


//                var con = new SqlConnection("Data Source=192.160.0.120;Initial Catalog=zeebregtsdb;User ID=daan;Password=Bl22sk22k!");
//                con.Open();
//                var command = @"SELECT persoon.voornaam,persoon.tussenvoegsel,persoon.achternaam,
//                                       persoon.zaemail,persoon.telefoon_nr_1,persoon.telefoon_nr_2,
//                                       persoon.telefoon_nr_3, persoon.telefoon_nr_settings, bedrijf.naam
//                                       FROM persoon  INNER JOIN
//                                       bedrijf ON persoon.bedrijf_nr = bedrijf.bedrijf_nr
//                                       WHERE persoon.NIETactief = 'FALSE' ";
//                var adptr = new SqlDataAdapter(command, con);
//                adptr.Fill(dt);
//                con.Close();
//                ds.Tables.Add(dt);

//                var customDs = new DataSet();
//                var customTable = new DataTable();
//                customTable.Columns.Add("Naam");
//                customTable.Columns.Add("Bedrijf");
//                customTable.Columns.Add("Nummer");
//                foreach (DataRow row in dt.Rows)
//                {

//                    var name = "";
//                    name += row[0] != null ? row[0].ToString() : "";
//                    name = name.Trim();
//                    name += row[1] != null ? " " + row[1].ToString() : "";
//                    name = name.Trim();
//                    name += row[2] != null ? " " + row[2].ToString() : "";
//                    name = name.Trim();

//                    Console.WriteLine(name);
//                    var bedrijfnaam = row[8] != null ? row[8].ToString() : "";
//                    if(row[7] != null)
//                    {
//                        var telnrSettings = row[7].ToString().Split(',');
//                        if (telnrSettings != null && telnrSettings.Count() == 3)
//                        {
//                            if (telnrSettings[1] == telnrSettings[0])
//                            {
//                                telnrSettings[1] = "6";
//                            }
//                            if (telnrSettings[2] == telnrSettings[1] || telnrSettings[2] == telnrSettings[0])
//                            {
//                                telnrSettings[2] = "7";
//                            }
//                            if (row[4] != null &&String.IsNullOrEmpty(row[4].ToString()) == false && convertTelType(telnrSettings[0]) != "fax")
//                            {
//                               var fullname1 = name + " (" + convertTelType(telnrSettings[0]) + ")";
//                               customTable.Rows.Add(new Object[] { fullname1, bedrijfnaam, formatTelNR(row[4].ToString()) });
//                            }
//                            if (row[5] != null && String.IsNullOrEmpty(row[5].ToString()) == false && convertTelType(telnrSettings[1]) != "fax")
//                            {
//                                var fullname2 = name + " (" + convertTelType(telnrSettings[1]) + ")";
//                                customTable.Rows.Add(new Object[] { fullname2, bedrijfnaam, formatTelNR(row[5].ToString()) });
//                            }
//                            if (row[6] != null && String.IsNullOrEmpty(row[6].ToString()) == false && convertTelType(telnrSettings[2]) != "fax")
//                            {
//                                var fullname3 = name + " (" + convertTelType(telnrSettings[2]) + ")";
//                                customTable.Rows.Add(new Object[] { fullname3, bedrijfnaam, formatTelNR(row[6].ToString()) });
//                            }
//                        }
//                    }
//                }

//                //add algemeen bedrijf nrs
//                var ds_bdr = new DataSet("bedrijfDataSet");
//                var dt_bdr = new DataTable("bedrijfDataTable");
//                ds_bdr.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;
//                dt_bdr.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;


//                con.Open();
//                var command_bdr = @"SELECT naam,tel
//                                       FROM bedrijf
//                                       WHERE NIETactief = 'FALSE' ";
//                var adptr_bdr = new SqlDataAdapter(command_bdr, con);
//                adptr_bdr.Fill(dt_bdr);
//                con.Close();
//                ds_bdr.Tables.Add(dt_bdr);
//                foreach (DataRow row in dt_bdr.Rows)
//                {
//                    if(row[0] != null && !String.IsNullOrEmpty(row[0].ToString()) && row[1] != null && !String.IsNullOrEmpty(row[1].ToString()))
//                    {
//                        customTable.Rows.Add(new Object[] { row[0].ToString() + " (Algemeen)", row[0].ToString(), formatTelNR(row[1].ToString()) });
//                    }
//                }
//                ////
//                customDs.Tables.Add(customTable);

//                ExcelLibrary.DataSetHelper.CreateWorkbook("ContactsExport.xls", customDs);
//            }
//            catch(Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }

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
                            if (row[4] != null && String.IsNullOrEmpty(row[4].ToString()) == false && convertTelType(telnrSettings[0]) != "fax")
                            {
                                var fullname1 = name + " (" + convertTelType(telnrSettings[0]) + ")";
                                customTable.Rows.Add(new Object[] { fullname1, bedrijfnaam, formatTelNR(row[4].ToString()) });
                            }
                            if (row[5] != null && String.IsNullOrEmpty(row[5].ToString()) == false && convertTelType(telnrSettings[1]) != "fax")
                            {
                                var fullname2 = name + " (" + convertTelType(telnrSettings[1]) + ")";
                                customTable.Rows.Add(new Object[] { fullname2, bedrijfnaam, formatTelNR(row[5].ToString()) });
                            }
                            if (row[6] != null && String.IsNullOrEmpty(row[6].ToString()) == false && convertTelType(telnrSettings[2]) != "fax")
                            {
                                var fullname3 = name + " (" + convertTelType(telnrSettings[2]) + ")";
                                customTable.Rows.Add(new Object[] { fullname3, bedrijfnaam, formatTelNR(row[6].ToString()) });
                            }
                        }
                    }
                }

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
                foreach (DataRow row in dt_bdr.Rows)
                {
                    if (row[0] != null && !String.IsNullOrEmpty(row[0].ToString()) && row[1] != null && !String.IsNullOrEmpty(row[1].ToString()))
                    {
                        customTable.Rows.Add(new Object[] { row[0].ToString() + " (Algemeen)", row[0].ToString(), formatTelNR(row[1].ToString()) });
                    }
                }
                ////
                customDs.Tables.Add(customTable);

                ExcelLibrary.DataSetHelper.CreateWorkbook(path, customDs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
       
      
        

       
    }
}

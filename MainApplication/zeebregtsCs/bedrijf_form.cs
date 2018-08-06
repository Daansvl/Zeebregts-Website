using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace zeebregtsCs
{
    public partial class bedrijf_form : Form
    {
        //initializer
        DataSet ds1;
        System.Data.SqlClient.SqlConnection con;
        System.Data.SqlClient.SqlDataAdapter da;
        
        DataRow dRow;
        
        int MaxRows = 0;
        int inc = 0;


        public bedrijf_form()
        {
            InitializeComponent();
        }

        private void bedrijf_form_Load(object sender, EventArgs e)
        {
            //make and open db connection
            
            con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=|DataDirectory|\\zeebregtsDB.mdf;Integrated Security=True;User Instance=True";
            con.Open();

            //make database communication objects
            
            ds1 = new DataSet();
            
            string sql = "SELECT * From bedrijf";
            da = new System.Data.SqlClient.SqlDataAdapter(sql, con);
            da.Fill(ds1, "bedrijf");
            NavigateRecord();
            MaxRows = ds1.Tables["bedrijf"].Rows.Count;


            con.Close();
           // con.Dispose();
        }

        private void NavigateRecord()
        {
            dRow = ds1.Tables["bedrijf"].Rows[inc];
            textBox1.Text = dRow.ItemArray.GetValue(1).ToString();
            textBox2.Text = dRow.ItemArray.GetValue(2).ToString();
            textBox3.Text = dRow.ItemArray.GetValue(3).ToString();
            textBox4.Text = dRow.ItemArray.GetValue(4).ToString();
            textBox5.Text = dRow.ItemArray.GetValue(5).ToString();
            textBox6.Text = dRow.ItemArray.GetValue(6).ToString();
            textBox7.Text = dRow.ItemArray.GetValue(7).ToString();
            textBox8.Text = dRow.ItemArray.GetValue(8).ToString();
            textBox9.Text = dRow.ItemArray.GetValue(9).ToString();
            textBox10.Text = dRow.ItemArray.GetValue(10).ToString();
            textBox11.Text = dRow.ItemArray.GetValue(11).ToString();
            textBox12.Text = dRow.ItemArray.GetValue(12).ToString();
            textBox13.Text = dRow.ItemArray.GetValue(13).ToString();
            textBox14.Text = dRow.ItemArray.GetValue(14).ToString();
            textBox15.Text = dRow.ItemArray.GetValue(15).ToString();
            textBox16.Text = dRow.ItemArray.GetValue(16).ToString();
            textBox17.Text = dRow.ItemArray.GetValue(17).ToString();
            textBox18.Text = dRow.ItemArray.GetValue(18).ToString();
            textBox19.Text = dRow.ItemArray.GetValue(19).ToString();
            textBox20.Text = dRow.ItemArray.GetValue(20).ToString();
            textBox21.Text = dRow.ItemArray.GetValue(21).ToString();
            
        }

        private void button1_Click(object sender, EventArgs e) //back
        {
            if (inc > 0)
            {
                inc--;
                NavigateRecord();
            }
            else
            {
                MessageBox.Show("eerste record");
            }
        }

        private void button2_Click(object sender, EventArgs e) //next
        {
            if (inc != MaxRows - 1)
            {
                inc++;
                NavigateRecord();
            }
            else
            {
                MessageBox.Show("laatste record");
            }

        }

        private void button3_Click(object sender, EventArgs e) //submit
        {
            System.Data.SqlClient.SqlCommandBuilder cb;
            DataRow dRow = ds1.Tables["bedrijf"].NewRow();
            cb = new System.Data.SqlClient.SqlCommandBuilder(da);
            dRow[1] = textBox1.Text;//naam
            dRow[2] = textBox2.Text;//voorvoegsel
            dRow[3] = textBox3.Text;//zoeknaam
            dRow[4] = textBox4.Text;//straat
            dRow[5] = textBox5.Text;
            dRow[6] = textBox6.Text;
            dRow[7] = textBox7.Text;
            dRow[8] = textBox8.Text;
            dRow[9] = textBox9.Text;
            dRow[10] = textBox10.Text;
            dRow[11] = textBox11.Text;
            dRow[12] = textBox12.Text;
            dRow[13] = textBox13.Text;
            dRow[14] = textBox14.Text;
            dRow[15] = textBox15.Text;
            dRow[16] = textBox16.Text;
            dRow[17] = textBox17.Text;
            dRow[18] = textBox18.Text;
            dRow[19] = textBox19.Text;
            dRow[20] = textBox20.Text;
            dRow[21] = textBox21.Text;
           


            ds1.Tables["bedrijf"].Rows.Add(dRow);
            MaxRows = MaxRows + 1;
            inc = MaxRows - 1;

            da.Update(ds1, "bedrijf");
            

            MessageBox.Show("record toegevoegd");
        }

        private void button4_Click(object sender, EventArgs e) // clear
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox9.Clear();
            textBox10.Clear();
            textBox11.Clear();
            textBox12.Clear();
            textBox13.Clear();
            textBox14.Clear();
            textBox15.Clear();
            textBox16.Clear();
            textBox17.Clear();
            textBox18.Clear();
            textBox19.Clear();
            textBox20.Clear();
            textBox21.Clear();
            
        }

        private void button5_Click(object sender, EventArgs e)//update
        {
            System.Data.SqlClient.SqlCommandBuilder cb;
            cb = new System.Data.SqlClient.SqlCommandBuilder(da);

            System.Data.DataRow dRow2 = ds1.Tables["bedrijf"].Rows[inc];

            dRow2[1] = textBox1.Text;//naam
            dRow2[2] = textBox2.Text;//voorvoegsel
            dRow2[3] = textBox3.Text;//zoeknaam
            dRow2[4] = textBox4.Text;//straat
            dRow2[5] = textBox5.Text;
            dRow2[6] = textBox6.Text;
            dRow2[7] = textBox7.Text;
            dRow2[8] = textBox8.Text;
            dRow2[9] = textBox9.Text;
            dRow2[10] = textBox10.Text;
            dRow2[11] = textBox11.Text;
            dRow2[12] = textBox12.Text;
            dRow2[13] = textBox13.Text;
            dRow2[14] = textBox14.Text;
            dRow2[15] = textBox15.Text;
            dRow2[16] = textBox16.Text;
            dRow2[17] = textBox17.Text;
            dRow2[18] = textBox18.Text;
            dRow2[19] = textBox19.Text;
            dRow2[20] = textBox20.Text;
            dRow2[21] = textBox21.Text;

            da.Update(ds1, "bedrijf");

            MessageBox.Show("record bijgewerkt");


        }

        private void button6_Click(object sender, EventArgs e)//delete
        {
            System.Data.SqlClient.SqlCommandBuilder cb;
            cb = new System.Data.SqlClient.SqlCommandBuilder(da);

            ds1.Tables["bedrijf"].Rows[inc].Delete();
            MaxRows--;
            inc = 0;
            NavigateRecord();

            da.Update(ds1, "bedrijf");

            MessageBox.Show("record verwijderd");
        }

        private void button7_Click(object sender, EventArgs e)//zoek
        {
            string searchFor = this.zoekveld.Text;
            int results = 0;
            DataRow[] returnedRows;

            returnedRows = ds1.Tables["bedrijf"].Select("naam='" + searchFor + "'");

            results = returnedRows.Length;

            if (results > 0)
            {
                DataRow dr1;
                dr1 = returnedRows[0];

                string a = dr1[0].ToString();
                inc = int.Parse(a);
                textBox1.Text = dr1.ItemArray.GetValue(1).ToString();
                textBox2.Text = dr1.ItemArray.GetValue(2).ToString();
                textBox3.Text = dr1.ItemArray.GetValue(3).ToString();
                textBox4.Text = dr1.ItemArray.GetValue(4).ToString();
                textBox5.Text = dr1.ItemArray.GetValue(5).ToString();
                textBox6.Text = dr1.ItemArray.GetValue(6).ToString();
                textBox7.Text = dr1.ItemArray.GetValue(7).ToString();
                textBox8.Text = dr1.ItemArray.GetValue(8).ToString();
                textBox9.Text = dr1.ItemArray.GetValue(9).ToString();
                textBox10.Text = dr1.ItemArray.GetValue(10).ToString();
                textBox11.Text = dr1.ItemArray.GetValue(11).ToString();
                textBox12.Text = dr1.ItemArray.GetValue(12).ToString();
                textBox13.Text = dr1.ItemArray.GetValue(13).ToString();
                textBox14.Text = dr1.ItemArray.GetValue(14).ToString();
                textBox15.Text = dr1.ItemArray.GetValue(15).ToString();
                textBox16.Text = dr1.ItemArray.GetValue(16).ToString();
                textBox17.Text = dr1.ItemArray.GetValue(17).ToString();
                textBox18.Text = dr1.ItemArray.GetValue(18).ToString();
                textBox19.Text = dr1.ItemArray.GetValue(19).ToString();
                textBox20.Text = dr1.ItemArray.GetValue(20).ToString();
                textBox21.Text = dr1.ItemArray.GetValue(21).ToString();
                


            }
            else
            {
                MessageBox.Show("niet gevonden");
            }
        }

       

        

        

        
    }
}

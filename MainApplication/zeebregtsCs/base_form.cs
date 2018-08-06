using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Principal;

namespace zeebregtsCs
{
    public class base_form: Form
    {
        protected bool wijzigstand = false;
        System.Data.SqlClient.SqlConnection con;
        public bool program_closes = false;
        protected  bool skip_close;
       protected String veldnaam = "";
       protected String ownernaam = "";
       
       protected int Parent_ID;
       protected base_form start_parent;
       protected base_form close_parent;
       public base_form()
       {
           this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
           this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
       }
        public bool kan_sluiten()
        {
            if (wijzigstand)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void sluit()
        {
            skip_close = true;
        }
        public virtual void save()
        {
            //
        }
        public void reclose()
        {
            this.save();
            this.Close();
        }
        public virtual void herlaad()
        {

            this.Show();
            this.WindowState = Global.windowstate;
            if (Global.windowstate != FormWindowState.Maximized)
            {
                this.Size = Global.size;
                this.Location = Global.position;
            }
          
            
        }
        public void log_debug(String s)
        {
            String log_line = "DEBUG!! @ " + DateTime.Now.ToString() + "MSG: " + s;
            System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
            file.WriteLine(log_line);
            file.Close();
        }
        public void log_exception(Exception exep)
        {
            String log_line = "Exception occurred [b] @ " + DateTime.Now.ToString() + " error: " + exep;
            System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
            file.WriteLine(log_line);
            file.Close();
        }
        public void Change_parents(base_form s_parent, base_form c_parent)
        {
            start_parent = s_parent;
            close_parent = c_parent;
        }
        public base_form c_parent()
        {
            return close_parent;
        }
        public base_form s_parent()
        {
            return start_parent;
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
           
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            //////
            if (this.WindowState == FormWindowState.Normal)
            {
                Global.size = this.Size;
                Global.position = this.Location;
            }
            Global.windowstate = this.WindowState;
           // String log_line = "sluit @ " + DateTime.Now.ToString() + ": huidig: " + this.Name.ToString() + ": close_naar: " + close_parent.Name.ToString();
         //   System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
         //   file.WriteLine(log_line);
         //   file.Close();
            base.OnFormClosing(e);
        }
        
        public void disable_splitter(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is SplitContainer)
                {
                    (c as SplitContainer).IsSplitterFixed = true;
                   
                }
                if (c.Controls.Count > 0) disable_splitter(c);
            }
        }
        public string Is_ingebruik(int id, int type)
        {
            String rtrn = "false";
            con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = Global.ConnectionString_fileserver;
            string stopro = "Is_ingebruik";
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = stopro;
            command.Connection = con;
            SqlParameter record_id = command.Parameters.Add("@record_id", SqlDbType.Int);
            SqlParameter record_type = command.Parameters.Add("@record_type", SqlDbType.Int);
            SqlParameter resultP = command.Parameters.Add("@result", SqlDbType.NVarChar, 50);
            record_id.Value = id;
            record_id.Direction = ParameterDirection.Input;
            record_type.Value = type;
            record_type.Direction = ParameterDirection.Input;
            resultP.Direction = ParameterDirection.Output;
            con.Open();
            command.ExecuteNonQuery();
            con.Close();
            rtrn = resultP.Value.ToString();
            return rtrn;

        }
        public bool Neem_ingebruik(int id, int type, string naam)
        {
            int ack = 0;
            bool rtrn = false;
            con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = Global.ConnectionString_fileserver;
            string stopro = "neem_ingebruik";
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = stopro;
            command.Connection = con;
            SqlParameter record_id = command.Parameters.Add("@record_id", SqlDbType.Int);
            SqlParameter record_type = command.Parameters.Add("@record_type", SqlDbType.Int);
            SqlParameter naam_gebruiker = command.Parameters.Add("@naam_gebruiker", SqlDbType.NVarChar, 50);
            record_id.Value = id;
            record_id.Direction = ParameterDirection.Input;
            record_type.Value = type;
            record_type.Direction = ParameterDirection.Input;
            naam_gebruiker.Value = naam;
            naam_gebruiker.Direction = ParameterDirection.Input;
            con.Open();
            ack = command.ExecuteNonQuery();
            con.Close();
            if (ack > 0)
            {
                rtrn = true;
            }
            else
            {
                rtrn = false;
            }
            return rtrn;
        }
        public static bool Verwijder_ingebruik(int id, int type)
        {
            int ack = 0;
            bool rtrn = false;   
            var con = new System.Data.SqlClient.SqlConnection();
            con.ConnectionString = Global.ConnectionString_fileserver;
            string stopro = "verwijder_ingebruik";
            SqlCommand command = new SqlCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = stopro;
            command.Connection = con;
            SqlParameter record_id = command.Parameters.Add("@record_id", SqlDbType.Int);
            SqlParameter record_type = command.Parameters.Add("@record_type", SqlDbType.Int);
            record_id.Value = id;
            record_id.Direction = ParameterDirection.Input;
            record_type.Value = type;
            record_type.Direction = ParameterDirection.Input;
            con.Open();
            ack = command.ExecuteNonQuery();
            con.Close(); 
            if (ack > 0)
            {
                rtrn = true;
            }
            else
            {
                rtrn = false;
            }
            return rtrn;
        }

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(base_form));
			this.SuspendLayout();
			// 
			// base_form
			// 
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "base_form";
			this.ResumeLayout(false);

		}

	
    }
}

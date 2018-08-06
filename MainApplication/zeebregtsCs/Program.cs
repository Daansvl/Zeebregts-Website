using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Security.Cryptography;
using System.Configuration;

namespace zeebregtsCs
{
    
     public class Program
    {
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
         [STAThread]
         public static void Main(string[] args)
         {
             // Created?
             bool created = false;
             // Create the mutex.
             using (Mutex mutex = new Mutex(true, "MyMutex", out created))
             {
                 // If this is not created, get out.
                 if (!created)
                 {
                     MessageBox.Show("Het programma is reeds geopend");
                     return;
                 }

                 // Run your app code here.
                 Application.EnableVisualStyles();
                 Application.SetCompatibleTextRenderingDefault(false);
                 bool useDebugDb = false;
                 if (args != null && args.Length > 0 &&!String.IsNullOrEmpty(args[0]))
                 {
                     if (args[0] == "debug")
                     {
                         useDebugDb = true;
                     }
                   //  MessageBox.Show("useDebugDb status is " + (useDebugDb == true ? "true" : "false"));
                 }
                 if (!useDebugDb)
                 {
                     if (System.Diagnostics.Debugger.IsAttached)
                     {
                         useDebugDb = true;
                     }
                 }
				 /////
                 Global.isRdp = System.Windows.Forms.SystemInformation.TerminalServerSession;//System.Environment.GetEnvironmentVariable("SESSIONNAME").StartsWith("RDP-");
                 if (Global.isRdp)
				 {
					 //connection string is windows authentication is default
                     if(useDebugDb)
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
                         Global.ConnectionString_Mdr =  "Data Source=SQL-SERVER; Initial Catalog=MandagenRegistratie; MultipleActiveResultSets=True;User ID=sa;Password=Zeebregts2013##;";
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
				 /////
           
                 try
                 {
                     Menu m = new Menu();
                    
                    
                    
                     if (m != null && !m.IsDisposed)
                     {
                         m.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                         m.WindowState = Properties.Settings.Default.MyState;
                         if (Properties.Settings.Default.MyState != FormWindowState.Maximized)
                         {
                             m.Location = Properties.Settings.Default.MyLoc;
                             m.Size = Properties.Settings.Default.MySize;
                         }
                         Application.Run(m);
                     }
                 }
                 catch (Exception e)
                 {
                     String log_line = "crash program @ " + DateTime.Now.ToString() + "error: " + e;
                     System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                     file.WriteLine(log_line);
                     file.Close();
                   
                 }
                 finally
                 {
                     String log_line = "close program and clear @ " + DateTime.Now.ToString();
                     System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                     file.WriteLine(log_line);
                    
                     file.Close();
                 }

             }
         }
         public static void ChangeConnectionString(string connstr)
         {
             var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
             var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
             connectionStringsSection.ConnectionStrings["UserConnectionString"].ConnectionString = connstr;
             config.Save();
             ConfigurationManager.RefreshSection("connectionStrings");

         }
    }
}
 
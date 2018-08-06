using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeebregtsCs
{
    public static class handelingen_logger
    {
        public static void log_handeling(int record_id, int record_type, int handeling_nr)
        {
            try
            {
                handelingen_log_datasetTableAdapters.log_handeling_gebruikerTableAdapter lhg_adapt = new handelingen_log_datasetTableAdapters.log_handeling_gebruikerTableAdapter();
                lhg_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
                lhg_adapt.new_handeling_log(Global.UserLevel, Global.username, record_id, record_type, handeling_nr, DateTime.Now);
            }
            catch (Exception e)
            {
                String log_line = "Exception occurred @ " + DateTime.Now.ToString() + " error: " + e;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
            }
        }
    }
}

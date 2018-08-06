using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using System.Configuration;

namespace ZeebregtsLogic
{

    public static class Global
    {
        private static string m_6PPAUTH_KEY = "zYQZgaosidH8UtcE";
        public static string ZES_PP_AUTH_KEY
        {
            get { return m_6PPAUTH_KEY; }
        }

        public enum selectionMode
        {
            Deselecting = -1,
            Selecting = 1,
            Unknown = 0
        }

        public enum selectionDirection
        {
            Left = -1,
            Right = 1,
            Unknown = 0
        }

        public static bool useChatFunction
        {
            get { return ConfigurationManager.AppSettings["useChatFunction"].ToLower() == "true"; }
        }

        public static string ChannelServiceIpAddress
        {
            get { return ConfigurationManager.AppSettings["ChannelServiceIpAddress"].ToString(); }
        }

        public static int ChannelServicePort
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["ChannelServicePort"].ToString()); }
        }

        public static bool useWeekviewLeesstand
        {
            get { return ApplicationState.GetValue<bool>("WeekviewLeesstand"); }
            set { ApplicationState.SetValue("WeekviewLeesstand", value); }
        }

    }
}

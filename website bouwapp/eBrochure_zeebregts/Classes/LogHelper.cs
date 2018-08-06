using System;
using eBrochure_zeebregts.Web;
using eBrochure_zeebregts.Web.Services;

namespace eBrochure_zeebregts.Classes
{
	public enum LogType {status,error,activity}

	public static class LogHelper
	{
		public static void SendLog(string msg, LogType lt)
		{
			var ctx = new eBrochureDomainContext();
			ServerLog sl = new ServerLog();
			
			var MSG = "";
			switch (lt)
			{
				case LogType.status:
					MSG += @"[STATUS]-";
					break;
				case LogType.activity:
					MSG += @"[ACTIVITY]-";
					break;
				case LogType.error:
					MSG += @"[ERROR]-";
					break;
			}
			MSG += @"-[MSG]- " + msg + " -[END]";
			
			sl.logmsg = MSG;
			sl.TimeStamp = DateTime.Now;
            ctx.GetServerTime(operation => { sl.TimeStamp = operation.Value; CompleteSendLog(sl); }, null);
			//ctx.ServerLogs.Add(sl);
		//	ctx.SubmitChanges();
		
		}
        private static void CompleteSendLog(ServerLog sl)
        {
            var ctx = new eBrochureDomainContext();
            ctx.ServerLogs.Add(sl);
            ctx.SubmitChanges();
        }
	}
	
}

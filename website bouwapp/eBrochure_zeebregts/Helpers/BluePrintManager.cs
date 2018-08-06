using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using eBrochure_zeebregts.Classes;
namespace eBrochure_zeebregts.Helpers
{
    public class BluePrintManager
    {
        private ObservableCollection<BluePrintHolder> _bluePrints = new ObservableCollection<BluePrintHolder>();
		public ObservableCollection<BluePrintHolder> bluePrints { get { return _bluePrints; } set { _bluePrints = value; } }
		private Dictionary<string, string> _huidigeruimtesetnrs = new Dictionary<string, string>();
		public Dictionary<string, string> HuidigeRuimteSetNrs {
			get { return _huidigeruimtesetnrs; }
			set { _huidigeruimtesetnrs = value; }
		}
        private bool lockacumu = false;

		public BluePrintHolder RetryDownload(string ruimteID) {
		if(HuidigeRuimteSetNrs.ContainsKey(ruimteID))
		{
			var holder = (from h in bluePrints
						  where h.ruimteId == ruimteID && h.ruimteSetNr == HuidigeRuimteSetNrs[ruimteID]
						  select h).FirstOrDefault();
            if (holder == null)
            {
                //holder = (from h in bluePrints
                //          where h.ruimteId == ruimteID
                //          select h).FirstOrDefault();
            }
                holder.statusInfo = BluePrintStatus.Retrying;
                string apath = "http://mybouwapp.nl/Images/Blueprints/" + holder.filepath;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apath);
                request.UseDefaultCredentials = true;
                //Acumulator.Instance().updateDownActive(ruimteid, true);
                request.BeginGetResponse(RequestReady, request);
                return holder;
            
            
		}
		else
			return null;
		}
		public void ClearAll() {
			bluePrints.Clear();
			HuidigeRuimteSetNrs.Clear();
		}
		public void SetHuidigRuimteSetNr(string ruimteID, string setNr) {
			if (HuidigeRuimteSetNrs.ContainsKey(ruimteID))
				HuidigeRuimteSetNrs[ruimteID] = setNr;
			else
				HuidigeRuimteSetNrs.Add(ruimteID, setNr);
		}
		public byte[] getBlueprintData(string ruimteID, bool basis)
		{
            if (HuidigeRuimteSetNrs.ContainsKey(ruimteID))
            {
                if (basis)
                {
                    try
                    {
                        var data = (from h in bluePrints
                                    where h.ruimteId == ruimteID && h.ruimteSetNr == "Basis"
                                    select h.pixelData).First();
                        return data;
                    }
                    catch (Exception e)
                    {
                        LogHelper.SendLog("No image available error, "+e.Message, LogType.error);
                    }
                }
                try
                {
                    var data = (from h in bluePrints
                                where h.ruimteId == ruimteID && h.ruimteSetNr == HuidigeRuimteSetNrs[ruimteID]
                                select h.pixelData).First();
                    return data;
                }
                catch (Exception e)
                {
                    LogHelper.SendLog("No image available error", LogType.error);
                }
            }
            return null;
		}
        public BluePrintStatus GetBlueprintStatus(string ruimteid)
        {
            var ctx = Acumulator.Instance().ctx;
            if (HuidigeRuimteSetNrs.ContainsKey(ruimteid))
            {
                var results = (from h in bluePrints
                                where h != null && h.ruimteId == ruimteid && h.ruimteSetNr == HuidigeRuimteSetNrs[ruimteid]
                                select h.statusInfo);
                if (results.Count() > 0)
                {
                    return results.First();
                }
                else
                {

                    var hasImage = (from i in ctx.RuimteS
                                    where i.RS_ID == HuidigeRuimteSetNrs[ruimteid]
                                    select i.BlueprintPath).FirstOrDefault();
                    if (hasImage != null && hasImage.Length > 0)
                    {
                        return BluePrintStatus.Idle;
                    }
                    else
                    {
                        return BluePrintStatus.Unavailable;
                    }
                }
            }
            else
            {
                return BluePrintStatus.Unavailable;
            }
		}
		public Dictionary<string, bool> GetAllBlueprintStatus() 
        {
			var result = new Dictionary<string, bool>();
			foreach (Ruimte r in Acumulator.Instance().OTracker.offerteRuimte_.Children) {
				var bpstatus = (from h in bluePrints
							   where h.ruimteId == r.RuimteID && h.ruimteSetNr == HuidigeRuimteSetNrs[h.ruimteId]
							   select h.statusInfo).FirstOrDefault();

				result.Add(r.RuimteID, bpstatus == BluePrintStatus.Complete);
			}
			return result;
		}
		public bool GetAllDone()
        {
			bool result = true;
            foreach (Ruimte r in Acumulator.Instance().OTracker.offerteRuimte_.Children)
            {
                var bpstatus = (from h in bluePrints
                                where h.ruimteId == r.RuimteID && h.ruimteSetNr == HuidigeRuimteSetNrs[h.ruimteId]
                                select h.statusInfo);
                foreach (var bstat in bpstatus)
                {
                    if (bstat != BluePrintStatus.Complete)
                    {
                        result = false;
                    }
                }
                
            }
			return result;
		}

        public void GetAllBlueprints()
        {
            //Acumulator.Instance().Blueprints.Clear();
			bluePrints.Clear();
            foreach (Ruimte r in Acumulator.Instance().OTracker.offerteRuimte_.Children)
            {
                string path = Acumulator.Instance().GetPathGespiegeld(r.RuimteID);
				BluePrintHolder holder = new BluePrintHolder() {
					filepath = path,
					ruimteId = r.RuimteID,
					ruimteSetNr = "Basis",
					statusInfo = BluePrintStatus.Downloading
				};
				if (HuidigeRuimteSetNrs.ContainsKey(r.RuimteID))
					HuidigeRuimteSetNrs[r.RuimteID] = "Basis";
				else
					HuidigeRuimteSetNrs.Add(r.RuimteID, "Basis");
				bluePrints.Add(holder);
                if (path != null && path != "")
                {
                    string apath = "http://mybouwapp.nl/Images/Blueprints/" + path;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apath);

                    request.UseDefaultCredentials = true;
                    Acumulator.Instance().updateDownActive(r.RuimteID, true);
                    request.BeginGetResponse(RequestReady, request);
                }
            }
        }
		public void DownloadBlueprint(string path, string ruimteid, string setnr) {
			var holder = (from h in bluePrints
						  where h.filepath == path && h.ruimteId == ruimteid && h.ruimteSetNr == setnr
						  select h).FirstOrDefault();
			if (holder == null) {
				holder = new BluePrintHolder() {
					filepath = path,
					ruimteId = ruimteid,
					ruimteSetNr = setnr != null ? setnr : "Basis",
					statusInfo = BluePrintStatus.Downloading
				};
				bluePrints.Add(holder);
			}
			if (HuidigeRuimteSetNrs.ContainsKey(ruimteid))
				HuidigeRuimteSetNrs[ruimteid] = setnr != null ? setnr : "Basis";
			else
				HuidigeRuimteSetNrs.Add(ruimteid, setnr != null ? setnr : "Basis");
			string apath = "http://mybouwapp.nl/Images/Blueprints/" + path;
			
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apath);
			request.UseDefaultCredentials = true;
			Acumulator.Instance().updateDownActive(ruimteid, true);
			request.BeginGetResponse(RequestReady, request);
		}
        private void RequestReady(IAsyncResult asyncResult)
        {
            var req = asyncResult.AsyncState as HttpWebRequest;
			BluePrintHolder holder = null;
            try
            {
                using (WebResponse wrs = req.EndGetResponse(asyncResult))
                {
					
                    var path = wrs.ResponseUri.ToString().Replace("http://mybouwapp.nl/Images/Blueprints/", "");
                    var foo = wrs.GetResponseStream();

                    var bar = foo.CloneToMemoryStream();
                    var foobar = bar.ToArray();
					holder = (from h in bluePrints
								  where (h.statusInfo == BluePrintStatus.Downloading || h.statusInfo == BluePrintStatus.Retrying) && h.filepath == path
								  select h).FirstOrDefault();
					holder.pixelData = foobar;
					holder.statusInfo = BluePrintStatus.Complete;
								 
					while (lockacumu)
                    { }
                    lockacumu = true;

				  //  Acumulator.Instance().BlueprintRequestReady(path, foobar, null);
                    lockacumu = false;
					UIThread.Invoke(() => Acumulator.Instance().BB.ReloadImage());
                }
            }
            catch (Exception e)
            {
				var path = req.RequestUri.ToString().Replace("http://mybouwapp.nl/Images/Blueprints/", "");
				holder = (from h in bluePrints
						  where (h.statusInfo == BluePrintStatus.Downloading || h.statusInfo == BluePrintStatus.Retrying) && h.filepath == path
						  select h).FirstOrDefault();
				if (holder != null) {
					holder.statusInfo = BluePrintStatus.Retrying;
					if (holder.numRetry < 3) {
						holder.numRetry++;
						DownloadBlueprint(holder.filepath, holder.ruimteId, holder.ruimteSetNr);
					}
					else {
						holder.statusInfo = BluePrintStatus.Failed;
						UIThread.Invoke(() => LogHelper.SendLog("download failed after retry: "+ e.Message, LogType.error));
					}
				}
            }
        }
    }
    public enum BluePrintStatus:int
    {
        Idle,
        Downloading,
        Complete,
        Failed,
        Retrying,
        Unavailable
	}
    public class BluePrintHolder
    {
        private BluePrintStatus _statusInfo;
        public BluePrintStatus statusInfo
        { get { return _statusInfo; } set { _statusInfo = value; } }
        private byte[] _pixelData;
        public byte[] pixelData
        { get { return _pixelData; } set { _pixelData = value; } }
        private string _ruimteSetNr;
        public string ruimteSetNr
        { get { return _ruimteSetNr; } set { _ruimteSetNr = value; } }
        private string _ruimteId;
        public string ruimteId
        { get { return _ruimteId; } set { _ruimteId = value; } }
		private string _path;
		public string filepath { get { return _path; } set { _path = value; } }
		private int _numretry;
		public int numRetry { get { return _numretry; } set { _numretry = value; } }
    }
}

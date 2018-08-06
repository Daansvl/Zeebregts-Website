using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web;
using System.Windows;
using System.Windows.Media;
using System.Globalization;

namespace ZeebregtsLogic
{
    public class Tools
    {



        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the queried item.</param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, a null reference is being returned.</returns>
        public static T FindVisualParent<T>(DependencyObject child)
          where T : DependencyObject
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }



        public static DateTime CalculateWeekstart(DateTime datetime)
        {
            // 0:00 uur de dag beginnen
            datetime = new DateTime(datetime.Year, datetime.Month, datetime.Day);

            if (datetime.DayOfWeek != DayOfWeek.Monday)
            {
                for (int i = 6; i > 0; i--)
                {
                    if (datetime.AddDays(-i).DayOfWeek == DayOfWeek.Monday)
                    {
                        datetime = datetime.AddDays(-i);
                        break;
                    }
                }
            }

            return datetime;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentVakmanId"></param>
        /// <param name="currentProjectId"></param>
        /// <param name="currentStartdate"></param>
        /// <param name="currentEnddate"></param>
        /// <param name="refreshParameters"></param>
        /// <returns></returns>
        public static bool RefreshView(List<int> currentVakmanIds, DateTime currentStartdate, DateTime currentEnddate, string refreshParameters)
        {

            string[] refreshParamArray = refreshParameters.Split('|');

            string alteredVakmanIds = refreshParamArray[0];
            DateTime alteredStartdate = Convert.ToDateTime(refreshParamArray[1]);
            DateTime alteredEnddate = Convert.ToDateTime(refreshParamArray[2]);

            string[] alteredVakmanIdsArray = alteredVakmanIds.Split(',');

            bool isSameVakmanId = false;
            foreach (string vm in alteredVakmanIdsArray)
            {
                if (currentVakmanIds.Contains(Convert.ToInt32(vm)))
                {
                    isSameVakmanId = true;
                    break;
                }
            }

            // TODO: checken of het nu klopt: >= en <= was het eigenlijk
            bool isSameDate = ((alteredEnddate > currentStartdate) && (alteredStartdate < currentEnddate));

            if (isSameVakmanId && isSameDate)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alteredVakmanId"></param>
        /// <param name="alteredProjectId"></param>
        /// <param name="currentStartdate"></param>
        /// <param name="currentEnddate"></param>
        /// <returns></returns>
        public static string CreateChannelMessage(List<int> listAlteredVakmanIds, DateTime currentStartdate, DateTime currentEnddate)
        {
            string alteredVakmanIds = string.Empty;

            foreach (int vi in listAlteredVakmanIds)
            {
                alteredVakmanIds += vi.ToString() + ",";
            }

            alteredVakmanIds = alteredVakmanIds.TrimEnd(',');

            return alteredVakmanIds + "|" + currentStartdate.ToString() + "|" + currentEnddate.ToString();
        }


        public static bool IsValidBsn(string bsn)
        {
            try
            {
                bool isValidBsn = true;

                if (bsn.Length == 8)
                {
                    bsn = "0" + bsn;

                }

                if (bsn.Length == 9)
                {
                    int checksum = 0;
                    string[] bsnarray = bsn.Select(c => c.ToString()).ToArray();

                    checksum += 9 * Convert.ToInt32(bsnarray[0]);
                    checksum += 8 * Convert.ToInt32(bsnarray[1]);
                    checksum += 7 * Convert.ToInt32(bsnarray[2]);
                    checksum += 6 * Convert.ToInt32(bsnarray[3]);
                    checksum += 5 * Convert.ToInt32(bsnarray[4]);
                    checksum += 4 * Convert.ToInt32(bsnarray[5]);
                    checksum += 3 * Convert.ToInt32(bsnarray[6]);
                    checksum += 2 * Convert.ToInt32(bsnarray[7]);
                    checksum += -1 * Convert.ToInt32(bsnarray[8]);

                    if ((checksum % 11) != 0 || checksum == 0)
                    {
                        isValidBsn = false;
                    }
                }
                else
                {
                    isValidBsn = false;
                }

                return isValidBsn;
            }
            catch
            {
                return false;
            }
        }

        public static int GetWeekNumber(DateTime dtPassed)
        {
            //CultureInfo ciCurr = CultureInfo.CurrentCulture;
            CultureInfo ciCurr = new CultureInfo("en-US");
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            if (weekNum == 53)
            {
                // als het na 3 dagen al nieuwjaar is, dan vallen er 4 of meer dagen van deze week in het nieuwe jaar, hence, week 1
                if (dtPassed.Year < dtPassed.AddDays(3).Year)
                {
                    weekNum = 1;
                }
            }

            return weekNum;
        }


        private string Trim4Api(string nm)
        {
            var s = Regex.Split(nm, @"\W\s");
            return s[0];
        }


        public string ZoekOpPostcode(string _postcode)
        {
            ServicePointManager.DefaultConnectionLimit = 15;
            string returnValue = "";
            try
            {

                ////
                String auth_key = ZeebregtsLogic.Global.ZES_PP_AUTH_KEY;
                String nl_sixpp = Trim4Api(_postcode);
                // Optional parameter
                String streetnumber = "";
                // Create the GET request
                string uri = "http://api.pro6pp.nl/v1/autocomplete";
                uri += String.Format("?auth_key={0}&nl_sixpp={1}&per_page=25&streetnumber={2}", auth_key, HttpUtility.UrlEncode(nl_sixpp), streetnumber);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                request.ContentType = "text/xml; encoding='utf-8'";
                request.Timeout = 5000;
                // Get the response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    string result = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    // Parse the response
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    APIResponse r = serializer.Deserialize<APIResponse>(result);
                    if (r.status == "ok")
                    {
                        returnValue = r.results[0].street;
                    }
                }
            }
            catch (WebException we)
            {
            }
            return returnValue;
        }

        public APIResponse ZoekOpPostcodeComplete(string _postcode)
        {
            ServicePointManager.DefaultConnectionLimit = 15;
            APIResponse returnValue = null;
            try
            {

                ////
                String auth_key = ZeebregtsLogic.Global.ZES_PP_AUTH_KEY;
                String nl_sixpp = Trim4Api(_postcode);
                // Optional parameter
                String streetnumber = "";
                // Create the GET request
                string uri = "http://api.pro6pp.nl/v1/autocomplete";
                uri += String.Format("?auth_key={0}&nl_sixpp={1}&per_page=25&streetnumber={2}", auth_key, HttpUtility.UrlEncode(nl_sixpp), streetnumber);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                request.ContentType = "text/xml; encoding='utf-8'";
                request.Timeout = 5000;
                // Get the response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    string result = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    // Parse the response
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    APIResponse r = serializer.Deserialize<APIResponse>(result);
                    if (r.status == "ok")
                    {
                        returnValue = r;
                    }
                }
            }
            catch (WebException we)
            {
            }
            return returnValue;
        }


    }
}

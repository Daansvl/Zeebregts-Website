using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Data.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ZeebregtsLogic
{
    public static class ZBExtensions
    {

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            if (source != null)
            {
                return source.IndexOf(toCheck, comp) >= 0;
            }
            else
            {
                return false;
            }
        }

        public static int GetWeekOfYear(this DateTime date)
        {
            System.Globalization.Calendar cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            return cal.GetWeekOfYear(date, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
        }

        public static double ToDouble(this string source)
        {

            if (string.IsNullOrEmpty(source))
            {
                source = "0";
            }
            else
            {
                if (source.Length > 1)
                {
                    source = source.Replace(',', '.');
                }

                if (source.Length == 2)
                {
                    source = source.Substring(0, 1);
                }
            }

            return Convert.ToDouble(source);
        }

        public static string ToTime(this string source)
        {
            string result = string.Empty;
            int roundedHour = Convert.ToInt32(Math.Floor(Convert.ToDouble(source)));
            int roundedMinute = Convert.ToInt32((Convert.ToDouble(source) - Math.Floor(Convert.ToDouble(source))) * 60);

            if (roundedHour <= 9)
            {
                result = "0" + roundedHour.ToString();
            }
            else
            {
                result = roundedHour.ToString();
            }

            if (roundedMinute <= 9)
            {
                result += ":0" + roundedMinute.ToString();
            }
            else
            {
                result += ":" + roundedMinute.ToString();
            }

            return result;
        }

        public static string ToHour(this string source)
        {
            string result = string.Empty;
            int roundedHour = Convert.ToInt32(Math.Floor(Convert.ToDouble(source)));

            if(roundedHour <= 9)
            {
                return "0" + roundedHour.ToString() + "u";
            }
            else
            {
                return roundedHour.ToString() + "u";
            }
        }

        public static string ToMinute(this string source)
        {
            string result = string.Empty;
            int roundedMinute = Convert.ToInt32((Convert.ToDouble(source)-Math.Floor(Convert.ToDouble(source))) * 60);

            if (roundedMinute <= 9)
            {
                return "0" + roundedMinute.ToString() + "m";
            }
            else
            {
                return roundedMinute.ToString() + "m";
            }
        }


        public static double ToTimeDouble(this string source)
        {
            double result = 0.0;

            string[] timearray = source.Split(':');

            if (timearray.Length == 2)
            {
                if (timearray[1].ToString() == "15")
                {
                    result = Convert.ToDouble(timearray[0]) + 0.25;
                }
                else if (timearray[1].ToString() == "30")
                {
                    result = Convert.ToDouble(timearray[0]) + 0.50;
                }
                else if (timearray[1].ToString() == "45")
                {
                    result = Convert.ToDouble(timearray[0]) + 0.75;
                }
                else
                {
                    result = Convert.ToDouble(timearray[0]);
                }
            }

            return result;
        }

        public static double HourToTimeDouble(this string source)
        {
            double result = 0.0;

            result = Convert.ToDouble(source.Split('u')[0]);

            return result;
        }

        public static double MinuteToTimeDouble(this string source)
        {
            double result = 0.0;

            if (source == "15m")
            {
                result = 0.25;
            }
            else if (source == "30m")
            {
                result = 0.50;
            }
            else if (source == "45m")
            {
                result = 0.75;
            }

            return result;
        }

        public static int ToInt(this string source)
        {

            if (string.IsNullOrEmpty(source))
            {
                source = "0";
            }
            else
            {
                if (source.Length > 1)
                {
                    if (source.StartsWith("0"))
                    {
                        source = source.Substring(1, source.Length - 1);
                    }
                }
            }

            return Convert.ToInt32(source);
        }

        public static void AddText(this StackPanel source, string text)
        {
            source.AddText(text, true);
        }

        //public static void AddText(this StackPanel source, StackPanel text)
        //{
        //    source.Children.Add(text);
        //}


        public static void AddText(this StackPanel source, string text, bool isBold)
        {
            TextBlock tb = new TextBlock();
            tb.Text = text;
            tb.Foreground = (isBold ? Brushes.Black : Brushes.Gray);
            
            // tijdelijk even altijd zwart
            tb.Foreground = Brushes.Black;

            source.Children.Add(tb);
        }

        public static void InsertText(this StackPanel source, string text, bool isBold)
        {
            TextBlock tb = new TextBlock();
            tb.Text = text;
            tb.Foreground = (isBold ? Brushes.Black : Brushes.Gray);

            // tijdelijk even altijd zwart
            tb.Foreground = Brushes.Black;
            
            source.Children.Insert(0, tb);
        }

        public static void InsertText(this StackPanel source, string text, bool isBold, int index)
        {
            TextBlock tb = new TextBlock();
            tb.Text = text;
            tb.Foreground = (isBold ? Brushes.Black : Brushes.Gray);

            // tijdelijk even altijd zwart
            tb.Foreground = Brushes.Black;

            source.Children.Insert(index, tb);
        }

        public static string FromListToString(this List<int> source)
        {
            string result = string.Empty;

            if (source != null)
            {
                foreach (int i in source)
                {
                    result += i.ToString() + "|";
                }

                result = result.Trim('|');
            }

            return result;
        }

        public static List<int> FromStringToList(this string source)
        {
            List<int> result = new List<int>();

            if (!string.IsNullOrWhiteSpace(source))
            {
                foreach (string s in source.Split('|'))
                {
                    result.Add(Convert.ToInt32(s));
                }
            }
            return result;
        }



        public static int WordCount(this String str)
        {
            return str.Split(new char[] { ' ', '.', '?' },
                             StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static string ToStringTrimmed(this string source)
        {
            return Regex.Replace(source, @"\s+", " ");
        }

        public static string ToString(this string source, double maxWidth)
        {
            int maxCharacters = 0;

            maxCharacters = Convert.ToInt32(maxWidth / 10);

            if (source.Length > maxCharacters)
            {
                return source.Substring(0, maxCharacters) + "...";
            }
            else
            {
                return source;
            }

        }

        public static string ToString(this int? source, int fixedWidth)
        {
            if (source != null)
            {
                int intAddWidth = fixedWidth - source.ToString().Length;

                string strResult = source.ToString();


                if (intAddWidth > 0)
                {
                    for (int i = 0; i < intAddWidth; i++)
                    {

                        strResult += " ";

                    }
                }

                return strResult;
            }
            else
            {
                return "";
            }
        }


        //public static List<T> LinqCache<T>(this System.Data.Linq.Table<T> query) where T : class
        //{
        //    string tableName = query.Context.Mapping.GetTable(typeof(T)).TableName;
        //    //List<T> result = HttpContext.Current.Cache[tableName] as List<T>;
        //    List<T> result = ApplicationState.GetValue<List<T>>(tableName) as List<T>;

        //    if (result == null)
        //    {
        //        using (SqlConnection cn = new SqlConnection(query.Context.Connection.ConnectionString))
        //        {
        //            cn.Open();
        //            SqlCommand cmd = new SqlCommand(query.Context.GetCommand(query).CommandText, cn);
        //            cmd.Notification = null;
        //            cmd.NotificationAutoEnlist = true;
        //            SqlCacheDependencyAdmin.EnableNotifications(query.Context.Connection.ConnectionString);
        //            if (!SqlCacheDependencyAdmin.GetTablesEnabledForNotifications(query.Context.Connection.ConnectionString).Contains(tableName))
        //            {
        //                SqlCacheDependencyAdmin.EnableTableForNotifications(query.Context.Connection.ConnectionString, tableName);
        //            }

        //            SqlCacheDependency dependency = new SqlCacheDependency(cmd);
        //            //cmd.ExecuteNonQuery();

        //            //result = query.ToList();
        //            result = query.Context.Translate<T>(cmd.ExecuteReader()).ToList();

        //            //HttpContext.Current.Cache.Insert(tableName, result, dependency);
        //            ApplicationState.SetValue(tableName, result);
        //        }
        //    }
        //    return result;
        //}





        public static List<T> LinqCache<T>(this System.Data.Linq.Table<T> query) where T : class
        {
            try
            {
                string tableName = query.Context.Mapping.GetTable(typeof(T)).TableName;

                DateTime lastUsedCache = ApplicationState.GetValue<DateTime>(tableName + "CacheDate");

                List<T> result = null;

                if (lastUsedCache != null && lastUsedCache.AddSeconds(8) > DateTime.Now)
                {
                    result = ApplicationState.GetValue<List<T>>(tableName + "Cache") as List<T>;
                }


                if (result == null)
                {
                    using (SqlConnection cn = new SqlConnection(query.Context.Connection.ConnectionString))
                    {
                        cn.Open();
                        SqlCommand cmd = new SqlCommand(query.Context.GetCommand(query).CommandText, cn);
                        cmd.ExecuteNonQuery();

                        result = query.ToList();

                        ApplicationState.SetValue(tableName + "Cache", result);

                        // make sure we DO use the cache the next time
                        ApplicationState.SetValue(tableName + "CacheDate", DateTime.Now);

                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in caching systeem", ex);
            }
        }


    }
}

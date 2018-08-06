using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MandagenRegistratieDomain;

namespace MandagenRegistratie.tools
{
    public static class Functies
    {

        //public static int CalculateUren(Mandagen mandag)
        //{

        //    if (mandag.Status)
        //    {
        //        long lnMandagTicks = (mandag.Eindtijd.Ticks - mandag.Begintijd.Ticks);


        //        TimeSpan tsMandag = TimeSpan.FromTicks(lnMandagTicks);

        //        return ((int)Math.Ceiling(tsMandag.TotalHours));
        //    }
        //    else
        //    {
        //        return 0;
        //    }

        //}

        //public static int CalculateUren(List<Mandagen> mandagen)
        //{
        //    long lnMandagTicks = 0;

        //    foreach (Mandagen mandag in mandagen)
        //    {
        //        if (mandag.Status)
        //        {
        //            lnMandagTicks += (mandag.Eindtijd.Ticks - mandag.Begintijd.Ticks);
        //        }
        //    }

        //    TimeSpan tsMandag = TimeSpan.FromTicks(lnMandagTicks);

        //    return ((int)Math.Ceiling(tsMandag.TotalHours));

        //}

        //public static string CalculateUrenExact(List<Mandagen> mandagen)
        //{
        //    long lnMandagTicks = 0;

        //    foreach (Mandagen mandag in mandagen)
        //    {
        //        if (mandag.Status)
        //        {
        //            lnMandagTicks += (mandag.Eindtijd.Ticks - mandag.Begintijd.Ticks);
        //        }
        //    }

        //    TimeSpan tsMandag = TimeSpan.FromTicks(lnMandagTicks);

        //    if (tsMandag.TotalHours == 0 && tsMandag.Minutes == 0)
        //    {
        //        return string.Empty;
        //    }
        //    else
        //    {
        //        return ((int)(Math.Floor(tsMandag.TotalHours))).ToString() + ":" + (tsMandag.Minutes < 10 ? "0" + tsMandag.Minutes.ToString() : tsMandag.Minutes.ToString());
        //    }

        //}

        public static int CalculateUren(Mandagen mandag)
        {
            if (mandag.Status)
            {
                if (mandag.Minuten == 0)
                {
                    return mandag.Uren;
                }
                else
                {
                    return mandag.Uren + 1;
                }
            }
            else
            {
                return 0;
            }
        }

        public static int CalculateUren(List<Mandagen> mandagen)
        {
            int uren = mandagen.Sum(m => m.Uren);
            int minuten = mandagen.Sum(m => m.Minuten);
            int minutenRemainder = minuten % 60;

            return uren + (minuten - minutenRemainder) / 60 + (minutenRemainder == 0 ? 0 : 1);
        }

        public static string CalculateUrenExact(List<Mandagen> mandagen)
        {
            int uren = mandagen.Sum(m => m.Uren);
            int minuten = mandagen.Sum(m => m.Minuten);
            int minutenRemainder = minuten % 60;
            if (uren == 0 && minuten == 0)
            {
                return "";
            }
            else
            {
                return (uren + ((minuten - minutenRemainder) / 60)).ToString() + ":" + (minutenRemainder < 10 ? "0" : "") + minutenRemainder.ToString();
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


        public static string GetDayOfWeek(int intDay)
        {
            switch (intDay)
            {
                case 0:
                    return "Maandag";
                case 1:
                    return "Dinsdag";
                case 2:
                    return "Woensdag";
                case 3:
                    return "Donderdag";
                case 4:
                    return "Vrijdag";
                case 5:
                    return "Zaterdag";
                case 6:
                    return "Zondag";
                default:
                    return "";
            }
        }


    }
}

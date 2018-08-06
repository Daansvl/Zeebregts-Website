using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Admin
{
    /// <summary>
    /// Interaction logic for Comparison.xaml
    /// </summary>
    public partial class Comparison : MenuControl
    {
        public Comparison()
        {
            InitializeComponent();
        }

        public void VulWeeknummers()
        {
            List<int> listWeeknummers = new List<int>();

            for (int i = 1; i <= 53; i++)
            {
                listWeeknummers.Add(i);
            }

            cbWeeknummers.ItemsSource = listWeeknummers;

            cbWeeknummers.SelectedValue = DateTime.Now.GetWeekOfYear();


            DateTime dtMaandag = Tools.CalculateWeekstart(DateTime.Now);

            calCalendar.SelectedDate = dtMaandag;
            calCalendar2.SelectedDate = dtMaandag.AddDays(6);


        }


        public void Load()
        {
            try
            {

                VulWeeknummers();

                dbRepository dbrep = new dbRepository();

                List<vwMandagen> listMandagen = new List<vwMandagen>();
                List<vwMandagenBeta> listMandagenBeta = new List<vwMandagenBeta>();

                List<vwMandagen> listResultMandagen = new List<vwMandagen>();
                List<vwMandagenBeta> listResultMandagenBeta = new List<vwMandagenBeta>();

                List<Resultmandag> listResults = new List<Resultmandag>();
                List<Resultmandag> listResultsBeta = new List<Resultmandag>();

                listMandagen = dbrep.datacontext.vwMandagens.ToList();
                listMandagenBeta = dbrep.datacontext.vwMandagenBetas.ToList();

                listResultMandagen = listMandagen.Where(m => m.Status && (m.Uren > 0 || m.Minuten > 0) && !listMandagenBeta.Any(mb => mb.ContactIdOrigineel == m.ContactIdOrigineel && mb.project_NR == m.project_NR && mb.Status == m.Status && mb.Begintijd == m.Begintijd && mb.Eindtijd == m.Eindtijd)).ToList();
                listResultMandagenBeta = listMandagenBeta.Where(m => m.Status && (m.Uren > 0 || m.Minuten > 0) && !listMandagen.Any(mb => mb.ContactIdOrigineel == m.ContactIdOrigineel && mb.project_NR == m.project_NR && mb.Status == m.Status && mb.Begintijd == m.Begintijd && mb.Eindtijd == m.Eindtijd)).ToList();


                foreach (vwMandagen vmd in listResultMandagen)
                {
                    Resultmandag result = new Resultmandag();
                    result.Begintijd = vmd.Begintijd;
                    result.Bevestigd = vmd.Bevestigd;
                    result.Eindtijd = vmd.Eindtijd;
                    result.Geannulleerd = vmd.Geannulleerd;
                    result.Minuten = vmd.Minuten;
                    result.projectNr = (int)vmd.project_NR;
                    result.projectNaam = vmd.naam_project;
                    result.Uren = vmd.Uren;
                    result.naam = (vmd.voornaam + vmd.tussenvoegsel + vmd.achternaam).ToStringTrimmed();
                    result.Live = true;
                    result.projectLeiderNaam = vmd.Gebruikersnaam;
                    result.weeknr = vmd.Begintijd.GetWeekOfYear();
                    result.dag = vmd.Begintijd.DayOfWeek.ToString();

                    listResults.Add(result);
                }

                foreach (vwMandagenBeta vmd in listResultMandagenBeta)
                {
                    Resultmandag result = new Resultmandag();
                    result.Begintijd = vmd.Begintijd;
                    result.Bevestigd = vmd.Bevestigd;
                    result.Eindtijd = vmd.Eindtijd;
                    result.Geannulleerd = vmd.Geannulleerd;
                    result.Minuten = vmd.Minuten;
                    result.projectNr = (int)vmd.project_NR;
                    result.projectNaam = vmd.naam_project;
                    result.Uren = vmd.Uren;
                    result.naam = (vmd.voornaam + vmd.tussenvoegsel + vmd.achternaam).ToStringTrimmed();
                    result.Beta = true;
                    result.projectLeiderNaam = vmd.Gebruikersnaam;
                    result.weeknr = vmd.Begintijd.GetWeekOfYear();
                    result.dag = vmd.Begintijd.DayOfWeek.ToString();

                    listResults.Add(result);
                }

                dgmandagen.ItemsSource = listResults.OrderBy(r => r.projectLeiderNaam).ThenBy(q => q.projectNr).ThenBy(s => s.Begintijd.GetWeekOfYear()).ThenBy(t => t.Begintijd.DayOfWeek).ThenBy(u => u.naam).ThenByDescending(v => v.Live).ToList();

                // projectleider, weeknummer, dag

                // sorteren op: Projectleider, project, week, dag, vakman, database
                lblLiveCount.Content = "Live: " + listResultMandagen.Count.ToString() + " records, Beta: " + listResultMandagenBeta.Count.ToString() + " records";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        public void Load(DateTime startDatum, DateTime eindDatum)
        {

            try
            {



                dbRepository dbrep = new dbRepository();

                List<vwMandagen> listMandagen = new List<vwMandagen>();
                List<vwMandagenBeta> listMandagenBeta = new List<vwMandagenBeta>();

                List<vwMandagen> listResultMandagen = new List<vwMandagen>();
                List<vwMandagenBeta> listResultMandagenBeta = new List<vwMandagenBeta>();

                List<Resultmandag> listResults = new List<Resultmandag>();
                List<Resultmandag> listResultsBeta = new List<Resultmandag>();

                listMandagen = dbrep.datacontext.vwMandagens.ToList();
                listMandagenBeta = dbrep.datacontext.vwMandagenBetas.ToList();

                listResultMandagen = listMandagen.Where(m => m.Begintijd >= startDatum && m.Begintijd < eindDatum && m.Status && (m.Uren > 0 || m.Minuten > 0) && !listMandagenBeta.Any(mb => mb.ContactIdOrigineel == m.ContactIdOrigineel && mb.project_NR == m.project_NR && mb.Status == m.Status && mb.Begintijd == m.Begintijd && mb.Eindtijd == m.Eindtijd)).ToList();
                listResultMandagenBeta = listMandagenBeta.Where(m => m.Begintijd >= startDatum && m.Begintijd < eindDatum && m.Status && (m.Uren > 0 || m.Minuten > 0) && !listMandagen.Any(mb => mb.ContactIdOrigineel == m.ContactIdOrigineel && mb.project_NR == m.project_NR && mb.Status == m.Status && mb.Begintijd == m.Begintijd && mb.Eindtijd == m.Eindtijd)).ToList();


                foreach (vwMandagen vmd in listResultMandagen)
                {
                    Resultmandag result = new Resultmandag();
                    result.Begintijd = vmd.Begintijd;
                    result.Bevestigd = vmd.Bevestigd;
                    result.Eindtijd = vmd.Eindtijd;
                    result.Geannulleerd = vmd.Geannulleerd;
                    result.Minuten = vmd.Minuten;
                    result.projectNr = (int)vmd.project_NR;
                    result.projectNaam = vmd.naam_project;
                    result.Uren = vmd.Uren;
                    result.naam = (vmd.voornaam + vmd.tussenvoegsel + vmd.achternaam).ToStringTrimmed();
                    result.Live = true;
                    result.projectLeiderNaam = vmd.Gebruikersnaam;
                    result.weeknr = vmd.Begintijd.GetWeekOfYear();
                    result.dag = vmd.Begintijd.DayOfWeek.ToString();
           
                    listResults.Add(result);
                }

                foreach (vwMandagenBeta vmd in listResultMandagenBeta)
                {
                    Resultmandag result = new Resultmandag();
                    result.Begintijd = vmd.Begintijd;
                    result.Bevestigd = vmd.Bevestigd;
                    result.Eindtijd = vmd.Eindtijd;
                    result.Geannulleerd = vmd.Geannulleerd;
                    result.Minuten = vmd.Minuten;
                    result.projectNr = (int)vmd.project_NR;
                    result.projectNaam = vmd.naam_project;
                    result.Uren = vmd.Uren;
                    result.naam = (vmd.voornaam + vmd.tussenvoegsel + vmd.achternaam).ToStringTrimmed();
                    result.Beta = true;
                    result.projectLeiderNaam = vmd.Gebruikersnaam;
                    result.weeknr = vmd.Begintijd.GetWeekOfYear();
                    result.dag = vmd.Begintijd.DayOfWeek.ToString();

                    listResults.Add(result);
                }

                dgmandagen.ItemsSource = listResults.OrderBy(r => r.projectLeiderNaam).ThenBy(q => q.projectNr).ThenBy(s => s.Begintijd.GetWeekOfYear()).ThenBy(t => t.Begintijd.DayOfWeek).ThenBy(u => u.naam).ThenByDescending(v => v.Live).ToList();

                // projectleider, weeknummer, dag

                // sorteren op: Projectleider, project, week, dag, vakman, database
                lblLiveCount.Content = "Live: " + listResultMandagen.Count.ToString() + " records, Beta: " + listResultMandagenBeta.Count.ToString() + " records";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }

        }

        public class Resultmandag
        {
            public string projectLeiderNaam { get; set; }
            public int projectNr { get; set; }
            public string projectNaam { get; set; }
            public int weeknr { get; set; }
            public string dag { get; set; }
            public string naam { get; set; }
            public bool Live { get; set; }
            public bool Beta { get; set; }
            public DateTime Begintijd { get; set; }
            public DateTime Eindtijd { get; set; }
            public int Uren { get; set; }
            public int Minuten { get; set; }
            public bool Bevestigd { get; set; }
            public bool Geannulleerd { get; set; }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                //int count = calCalendar.SelectedDates.Count;
                DateTime startdatum = calCalendar.SelectedDate.Value;
                DateTime einddatum = calCalendar2.SelectedDate.Value;

                Load(startdatum, einddatum);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cbWeeknummers_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

            int weeknummer = Convert.ToInt32(cbWeeknummers.SelectedValue);

            System.Globalization.Calendar cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            
            DateTime lastdate = DateTime.Now.AddMonths(1).AddYears(-1);

            for (int i = 0; i < 54; i++)
            {
                if (lastdate.GetWeekOfYear() == weeknummer)
                {
                    break;
                }
                else
                {
                    lastdate = lastdate.AddDays(7);
                }
            }

            // lastdate is nu een dag ui de gekozen week
            // de maandag erbij zoeken

            DateTime dtMaandag = Tools.CalculateWeekstart(lastdate);

            calCalendar.SelectedDate = dtMaandag;
            calCalendar2.SelectedDate = dtMaandag.AddDays(6);
        }

    }
}

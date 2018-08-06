using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Globalization;
using ZeebregtsLogic;
using MandagenRegistratie.controls.Vakmannen.Dagview;
using MandagenRegistratie.controls;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls
{
    /// <summary>
    /// Interaction logic for VakmanDagenView.xaml
    /// </summary>
    public partial class VakmanDagenView : MenuControl
    {
        public DateTime Weekstart = DateTime.Now;
        public Projectleider objProjectleider;
        public Vakman objVakman;
        public List<Project> listProjecten;
        public List<Project> listProjectenAll;
        public int intProjectleider;
        public int intVakmanId;

        public VakmanDagenView()
        {
            InitializeComponent();

            this.OkClick += new FunctieOK(LoadVakmandagenView);
            // this.Gereed += LoadVakmandagenView; // zo kan het ook

        }

        #region Bubbling Event - GoTo
        public void GoTo(Control control)
        {
            RoutedEventArgs args = new RoutedEventArgs(GoToEvent, control);
            args.RoutedEvent = GoToEvent;
            RaiseEvent(args);

        }

        public static readonly RoutedEvent GoToEvent = EventManager.RegisterRoutedEvent("OnGoTo", RoutingStrategy.Bubble,
typeof(RoutedEventHandler), typeof(VakmanDagenView));

        public event RoutedEventHandler OnGoTo
        {
            add { AddHandler(GoToEvent, value); }
            remove { RemoveHandler(GoToEvent, value); }
        }
        #endregion

        public DateTime CalculateWeekstart(DateTime datetime)
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



        public void LoadVakmandagenView()
        {
            spVakmanDagen.Children.Clear();

            dbRepository dbrep = new dbRepository();
            bool blnThisWeek = false;


            // 0:00 uur de dag beginnen
            Weekstart = CalculateWeekstart(Weekstart);

            // is this week?
            if (Weekstart < DateTime.Now && DateTime.Now < Weekstart.AddDays(7))
            {
                blnThisWeek = true;
            }

           
            // automatically confirm my previous changes
            //dbrep.AutoConfirmMandagenForWeekView(intVakmanId, intProjectleider);
            intVakmanId = ApplicationState.GetValue<int>("intVakmanId");
            intProjectleider = ApplicationState.GetValue<int>("intProjectleider");

            objVakman = dbrep.GetVakman(intVakmanId);
            objProjectleider = dbrep.GetProjectleider(intProjectleider);
            listProjecten = dbrep.GetProjectsByVakmanId(intVakmanId, Weekstart);

            lblVakmanNaam.Content = objVakman.Bsn + " : " + objVakman.VakmanId.ToString();



            // overzichtskolom toevoegen
            VakmanDagOverzicht vakmandagoverzicht = new VakmanDagOverzicht();
            vakmandagoverzicht.listProjecten = listProjecten;
            vakmandagoverzicht.objProjectleider = objProjectleider;
            vakmandagoverzicht.LoadVakmanDagOverzicht();
            spVakmanDagen.Children.Add(vakmandagoverzicht);


            listProjectenAll = dbrep.GetProjectsToAddByVakmanId(intVakmanId, Weekstart);

            ddlProjecten.ItemsSource = listProjectenAll;
            ddlProjecten.DisplayMemberPath = "Naam";
            ddlProjecten.SelectedValuePath = "ProjectId";



            // 7 dagen van de week toevoegen
            for (int i = 0; i < 7; i++)
            {
                VakmanDag vakmandag = new VakmanDag();

                vakmandag.OnVakmanDagUpdate += vakmandag_OnVakmanDagUpdate;
                vakmandag.OnVakmanDagSelect += vakmandag_OnVakmanDagSelect;
                vakmandag.objProjectleider = objProjectleider;
                vakmandag.listProjecten = listProjecten;
                vakmandag.dtBegintijd = Weekstart.AddDays(i);
                vakmandag.Vakman = objVakman;

                if (blnThisWeek && Weekstart.AddDays(i).DayOfWeek == DateTime.Now.DayOfWeek)
                {
                    vakmandag.IsSelected = true; //select this one
                }
                else if (!blnThisWeek)
                {
                    vakmandag.IsSelected = (i == 0); //select first one
                }

                vakmandag.LoadVakmanDag();

                // CultureInfo ciCurr = CultureInfo.CurrentCulture;
                vakmandag.lblDag.Content = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)Weekstart.AddDays(i).DayOfWeek].ToString();
                spVakmanDagen.Children.Add(vakmandag);
            }

            LoadWeekInfo();
        }

        public void LoadWeekInfo()
        {
            lblWeekInfo.Content = "Week " + GetWeekNumber(Weekstart).ToString() + ", " + Weekstart.ToString("dddd dd MMM yyyy");

            lblVakmanBsn.Content = objVakman.Bsn;
            lblVakmanNaam.Content = objVakman.Adres;
            lblVakmanWerkweek.Content = objVakman.Werkweek.ToString();
            //lblVakmanMa.Content = objVakman.Ma;
            //lblVakmanDi.Content = objVakman.Di;
            //lblVakmanWo.Content = objVakman.Wo;
            //lblVakmanDo.Content = objVakman.Do;
            //lblVakmanVr.Content = objVakman.Vr;
            //lblVakmanZa.Content = objVakman.Za;
            //lblVakmanZo.Content = objVakman.Zo;



        }

        public static readonly RoutedEvent VakmanDagenViewEvent = EventManager.RegisterRoutedEvent("OnVakmanDagenViewUpdate", RoutingStrategy.Bubble,
        typeof(RoutedEventHandler), typeof(VakmanDagenView));

        public event RoutedEventHandler OnVakmanDagenViewUpdate
        {
            add { AddHandler(VakmanDagenViewEvent, value); }
            remove { RemoveHandler(VakmanDagenViewEvent, value); }
        }

        
        void vakmandag_OnVakmanDagUpdate(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(VakmanDagenViewEvent, sender);
            args.RoutedEvent = VakmanDagenViewEvent;
            RaiseEvent(args);

            foreach (Control control in spVakmanDagen.Children)
            {
                if (control.GetType() == typeof(VakmanDag))
                {
                    VakmanDag vakmandag = (VakmanDag)control;
                    vakmandag.IsGewijzigd = true;
                    vakmandag.IsSelected = false;
                    vakmandag.LoadVakmanDag();
                }
            }
        }

        void vakmandag_OnVakmanDagSelect(object sender, RoutedEventArgs e)
        {

            foreach (Control control in spVakmanDagen.Children)
            {
                if (control.GetType() == typeof(VakmanDag))
                {
                    VakmanDag vakmandag = (VakmanDag)control;
                    vakmandag.IsSelected = true;
                    vakmandag.IsSelected = false;
                    vakmandag.IsGewijzigd = true;
                    //vakmandag.LoadVakmanDag();
                }
            }

            ((VakmanDag)sender).IsSelected = true;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            btnVorige.IsEnabled = false;

            Weekstart = Weekstart.AddDays(-7);
            LoadVakmandagenView();
            LoadWeekInfo();

            btnVorige.IsEnabled = true;

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {

            btnVolgende.IsEnabled = false;

            Weekstart = Weekstart.AddDays(7);
            LoadVakmandagenView();
            LoadWeekInfo();

            btnVolgende.IsEnabled = true;

        }


        public static int GetWeekNumber(DateTime dtPassed)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }

        private void btnAddProject_Click(object sender, RoutedEventArgs e)
        {
            dbRepository dbrep = new dbRepository();

            Project objNewProject = dbrep.GetProject(Convert.ToInt32(ddlProjecten.SelectedValue));
            
            Mandagen mandag = new Mandagen();
            mandag.VakmanId = intVakmanId;
            mandag.ProjectId = objNewProject.ProjectId;
            mandag.Begintijd = Weekstart;
            mandag.Eindtijd = Weekstart;
            mandag.Uren = 0;
            mandag.UrenGewijzigd = 0;
            mandag.Minuten = 0;
            mandag.MinutenGewijzigd = 0;
            mandag.Mutatiedatum = DateTime.Now;
            mandag.MutatieDoorProjectleiderId = intProjectleider;
            mandag.ProjectleiderId = objNewProject.ProjectleiderId;
            mandag.VakmansoortId = 1;
            mandag.VakmanstatusId = 1;
            mandag.Gewijzigd = false;
            mandag.IsChauffeurHeen = false;
            mandag.IsChauffeurTerug = false;
            mandag.KentekenHeen = "";
            mandag.KentekenTerug = "";
            mandag.Geannulleerd = false;
            mandag.Definitief = false;
            mandag.Bevestigd = true;
            mandag.Bevestigingsdatum = DateTime.Now;

            mandag.Status = true;

            dbrep.InsertMandag(mandag);

            LoadVakmandagenView();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void clCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (sender != null)
            //{
            //    Weekstart = CalculateWeekstart((DateTime)e.AddedItems[0]);

            //    clCalendar.SelectedDates.Clear();
            //    clCalendar.SelectedDates.Add(Weekstart);
            //    clCalendar.SelectedDates.Add(Weekstart.AddDays(1));
            //    clCalendar.SelectedDates.Add(Weekstart.AddDays(2));
            //    clCalendar.SelectedDates.Add(Weekstart.AddDays(3));
            //    clCalendar.SelectedDates.Add(Weekstart.AddDays(4));
            //    clCalendar.SelectedDates.Add(Weekstart.AddDays(5));
            //    clCalendar.SelectedDates.Add(Weekstart.AddDays(6));

            //    LoadVakmandagenView();

            //}


        }


    }
}

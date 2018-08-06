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

namespace MandagenRegistratie.controls.Projecten.Weekview
{
    /// <summary>
    /// Interaction logic for ProjectDagenView.xaml
    /// </summary>
    public partial class ProjectDagenView : UserControl
    {
        public DateTime Weekstart = DateTime.Now;
        public Gebruiker objProjectleider;
        public Project objProject = new Project();
        public List<vwVakman> listVakmannen;

        public List<Vakman> listVakmannenAll = new List<Vakman>();

        public int intProjectleider;
        public int intProjectId;

        public ProjectDagenView()
        {
            InitializeComponent();
        }

        public string ToonNaam(persoon objPersoon)
        {
            return objPersoon.voornaam + " " + (string.IsNullOrWhiteSpace(objPersoon.tussenvoegsel) ? "" : objPersoon.tussenvoegsel + " ") + objPersoon.achternaam;
        }

        public void LoadProjectDagenView()
        {

            intProjectleider = ApplicationState.GetValue<int>("intProjectleider");
            intProjectId = ApplicationState.GetValue<int>("intProjectId");

            spProjecten.Children.Clear();

            dbRepository dbrep = new dbRepository();

            listVakmannenAll = dbrep.GetVakmannenToAddByProjectId(intProjectId);
            ddlVakmannen.ItemsSource = listVakmannenAll;
            ddlVakmannen.DisplayMemberPath = "Bsn";
            ddlVakmannen.SelectedValuePath = "VakmanId";

            objProject = dbrep.GetProject(intProjectId);
            //intProjectleider = objProject.ProjectleiderId;

            objProjectleider = dbrep.GetProjectleider(intProjectleider);
            listVakmannen = dbrep.GetVakmannenByProjectId(intProjectId, Weekstart);

            // set projectinfo
            lblProjectnaam.Content = objProject.Naam + " : " + objProject.ProjectId.ToString();
            lblProjectId.Content = objProject.ProjectId;
            lblProjectleider.Content = objProject.ProjectleiderId;

            if (Weekstart.DayOfWeek != DayOfWeek.Monday)
            {
                for (int i = 6; i > 0; i--)
                {
                    if (Weekstart.AddDays(-i).DayOfWeek == DayOfWeek.Monday)
                    {
                        Weekstart = Weekstart.AddDays(-i);
                        break;
                    }
                }

            }



            //// overzichtskolom toevoegen
            ProjectDagOverzicht vakmandagoverzicht = new ProjectDagOverzicht();
            vakmandagoverzicht.listVakmannen = listVakmannen;
            vakmandagoverzicht.objProjectleider = objProjectleider;
            vakmandagoverzicht.objProject = objProject;
            vakmandagoverzicht.LoadProjectDagOverzicht();
            spProjecten.Children.Add(vakmandagoverzicht);

            // 7 dagen van de week toevoegen
            for (int i = 0; i < 7; i++)
            {
                try
                {

                    ProjectDag projectdag = new ProjectDag();
                    projectdag.OnProjectDagUpdate += projectdag_OnProjectDagUpdate;
                    projectdag.objProjectleider = objProjectleider;
                    projectdag.listVakmannen = listVakmannen;
                    projectdag.objProject = objProject;
                    projectdag.dtBegintijd = Weekstart.AddDays(i);
                    projectdag.LoadProjectDag();

                    projectdag.lblDag.Content = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)Weekstart.AddDays(i).DayOfWeek].ToString();
                    spProjecten.Children.Add(projectdag);
                }
                catch (Exception ex)
                {

                }
            }

            LoadWeekInfo();
        }


        public static readonly RoutedEvent ProjectDagenViewEvent = EventManager.RegisterRoutedEvent("OnProjectDagenViewUpdate", RoutingStrategy.Bubble,
        typeof(RoutedEventHandler), typeof(VakmanDagenView));

        //public event RoutedEventHandler OnProjectDagenViewUpdate
        //{
        //    add { AddHandler(ProjectDagenViewEvent, value); }
        //    remove { RemoveHandler(ProjectDagenViewEvent, value); }
        //}

        public static readonly RoutedEvent RefreshEvent = EventManager.RegisterRoutedEvent("OnRefresh", RoutingStrategy.Bubble,
typeof(RoutedEventHandler), typeof(VakmanDagenView));

        public event RoutedEventHandler OnRefreshEvent
        {
            add { AddHandler(RefreshEvent, value); }
            remove { RemoveHandler(RefreshEvent, value); }
        }

        public void Refresh()
        {
            RoutedEventArgs args = new RoutedEventArgs(RefreshEvent, this);
            args.RoutedEvent = RefreshEvent;
            RaiseEvent(args);
        }


        void projectdag_OnProjectDagUpdate(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(ProjectDagenViewEvent, sender);
            args.RoutedEvent = ProjectDagenViewEvent;
            RaiseEvent(args);

            foreach (Control control in spProjecten.Children)
            {
                if (control.GetType() == typeof(ProjectDag))
                {
                    ProjectDag vakmandag = (ProjectDag)control;
                    vakmandag.IsGewijzigd = false;
                    vakmandag.LoadProjectDag();
                }
            }
        }


        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Weekstart = Weekstart.AddDays(-7);
            LoadProjectDagenView();
            LoadWeekInfo();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            Weekstart = Weekstart.AddDays(7);
            LoadProjectDagenView();
            LoadWeekInfo();
        }


        public void LoadWeekInfo()
        {
            lblWeekInfo.Content = "Week " + GetWeekNumber(Weekstart).ToString() + ", " + Weekstart.ToString("dddd dd MMM yyyy");
        }

        public static int GetWeekNumber(DateTime dtPassed)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }

        private void btnAddVakman_Click(object sender, RoutedEventArgs e)
        {
            dbRepository dbrep = new dbRepository();

            Mandagen mandag = new Mandagen();
            mandag.VakmanId = Convert.ToInt32(ddlVakmannen.SelectedValue);
            mandag.ProjectId = objProject.ProjectId;
            mandag.Begintijd = Weekstart;
            mandag.Eindtijd = Weekstart;
            mandag.Uren = 0;
            mandag.UrenGewijzigd = 0;
            mandag.Minuten = 0;
            mandag.MinutenGewijzigd = 0;
            mandag.Mutatiedatum = DateTime.Now;
            mandag.MutatieDoorProjectleiderId = objProject.ProjectleiderId;
            mandag.ProjectleiderId = objProject.ProjectleiderId;
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

            dbrep.InsertMandag(mandag);

            LoadProjectDagenView();
        }

    }
}

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
using MandagenRegistratieLogic;
using System.Globalization;
using System.ComponentModel;

namespace MandagenRegistratie.controls
{
    /// <summary>
    /// Interaction logic for VakmanDagView.xaml
    /// </summary>
    public partial class VakmanDagView : UserControl
    {
        #region "INotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));


            switch (propertyName)
            {
                case "IsGewijzigd":
                    //ToggleButtons();
                    break;
                case "IsSelected":
                    //ToggleButtons();
                    break;
                default:
                    break;
            }
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public DateTime Weekstart = DateTime.Now;
        //public Projectleider objProjectleider;
        //public List<Project> listProjecten;
        public List<Project> listProjectenAll;
        public int intProjectleider;
        public int intVakmanId;

        public int intLastleft;

        private bool isGewijzigd;
        public bool IsGewijzigd
        {
            get { return isGewijzigd; }
            set { SetField(ref isGewijzigd, value, "IsGewijzigd"); }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { SetField(ref isSelected, value, "IsSelected"); }
        }

        private Vakman objVakman;
        public Vakman Vakman
        {
            get { return objVakman; }
            set { SetField(ref objVakman, value, "Vakman"); }
        }

        #endregion

        public Projectleider objProjectleider;

        public List<Project> listProjecten;
        //public Project objProjectLeidend;

        //public Project objProject;

        public int ProjectId { get; set; }

        // extra
        private List<Mandagen> listMandagen;
        public DateTime dtBegintijd;


        public VakmanDagView()
        {
            InitializeComponent();
        }


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

        public void LoadVakmanDagView()
        {
            dtBegintijd = ApplicationState.GetValue<DateTime>("dtSelectedDay");


            //lblDagInfo.Content = 

            // intialize
            spVakmanDag.Children.Clear();
            dbRepository dbrep = new dbRepository();
            intVakmanId = ApplicationState.GetValue<int>("intVakmanId");
            intProjectleider = ApplicationState.GetValue<int>("intProjectleider");

            objVakman = dbrep.GetVakman(intVakmanId);
            objProjectleider = dbrep.GetProjectleider(intProjectleider);
            listProjecten = dbrep.GetProjectsByVakmanId(intVakmanId, Weekstart);

            listMandagen = dbrep.GetMandagen(objVakman.VakmanId);
            bool blnIsOpVreemdProjectIngepland = false;
            bool blnIsOpVreemdProjectAangevraagd = false;
            bool blnIsOpProjectAangevraagd = false;
            bool blnIsOpProjectIngepland;
            bool blnIsNotOwner = false;


            // load overzicht
            spOverzicht.Children.Clear();

            foreach (Project project in listProjecten)
            {
                VakmanDagProjectHeader vakmandagprojectheader = new VakmanDagProjectHeader();
                if (project.ProjectId == ApplicationState.GetValue<int>("intProjectId"))
                {
                    vakmandagprojectheader.btnGoToProject.Content = "<";
                }

                vakmandagprojectheader.Project = project;
                spOverzicht.Children.Add(vakmandagprojectheader);
            }







            bool blnIsOpMijnProjectIngepland;

            DateTime startDatum = new DateTime(dtBegintijd.Year, dtBegintijd.Month, dtBegintijd.Day);
            DateTime eindDatum = startDatum.AddDays(1);

            listMandagen = listMandagen.Where(p => p.Begintijd >= startDatum && p.Begintijd < eindDatum).ToList();

            blnIsOpVreemdProjectIngepland = listMandagen.Any(m => m.Uren > 0 && m.Project.ProjectleiderId != objProjectleider.ProjectleiderId && (m.VakmanstatusId == 1 || m.VakmanstatusId == 2 || m.VakmanstatusId == 3));
            blnIsOpProjectAangevraagd = listMandagen.Any(m => m.Gewijzigd && m.UrenGewijzigd > 0 && (m.VakmanstatusId == 1 || m.VakmanstatusId == 2 || m.VakmanstatusId == 3));
            blnIsOpVreemdProjectAangevraagd = listMandagen.Any(m => m.Gewijzigd && m.UrenGewijzigd > 0 && m.MutatieDoorProjectleiderId != objProjectleider.ProjectleiderId && (m.VakmanstatusId == 1 || m.VakmanstatusId == 2 || m.VakmanstatusId == 3));
            blnIsOpProjectIngepland = listMandagen.Any(m => m.Bevestigd && m.Uren > 0 && (m.VakmanstatusId == 1 || m.VakmanstatusId == 2 || m.VakmanstatusId == 3));

            // volgens mij klopt deze query alleen toevallig omdat je in de weekview alle mandagen van 1 projectleider zijn
            blnIsNotOwner = !listMandagen.Any(m => m.Bevestigd && m.Uren > 0 && (m.VakmanstatusId == 1 || m.VakmanstatusId == 2 || m.VakmanstatusId == 3));

            blnIsOpMijnProjectIngepland = listMandagen.Any(m => m.Uren > 0 && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId && (m.VakmanstatusId == 1 || m.VakmanstatusId == 2 || m.VakmanstatusId == 3));

            bool blnIsAllowedToCancel = false;
            bool blnIsAllowedToConfirm = false;
            bool blnIsAllowedToAcceptAnnulleringen = false;

            bool blnIsAllowedToSetZiek = true;
            bool blnIsAllowedToSetVakantie = true;


            // voor alle projecten van deze vakman
            foreach (Project project in listProjecten)
            {

                bool blnIsOwner = false;
                bool blnIsEditor = false;

                // als ik (degene die dit scherm bekijkt) de projectleider ben
                blnIsOwner = project.ProjectleiderId == objProjectleider.ProjectleiderId;

                //VakmanDagViewProjectUren vakmandagproject = new VakmanDagViewProjectUren();
                //vakmandagproject.ProjectId = project.ProjectId;
                ////vakmandagproject.OnVakmanDagProjectUpdate += new RoutedEventHandler(recDag_MouseDown);
                ////vakmandagproject.OnVakmanDagProjectSelect += new RoutedEventHandler(spVakmandagProject_OnVakmanDagSelectEvent);

                //// als ik (degene die dit scherm bekijkt) de projectleider ben
                //vakmandagproject.IsGroen = blnIsOwner;

                //// als het niet mijn project is EN er ergens uren staan ingevuld op een (ANDER) project
                //vakmandagproject.ReadOnly = (!blnIsOwner && blnIsOpVreemdProjectIngepland) || (blnIsOpProjectAangevraagd);

                // kijken of hij ingepland is voor dit project
                Mandagen mandag = listMandagen.Where(m => m.ProjectId == project.ProjectId && m.VakmanstatusId == 1).FirstOrDefault();

                foreach (VakmanDagViewProjectUur vpu in GetProjectUren(listMandagen.Where(m => m.ProjectId == project.ProjectId && m.VakmanstatusId == 1).ToList(), project, dtBegintijd))
                {
                    vpu.project = project;
                    vpu.vakman = objVakman;

                    // oneven uren 'arceren'
                    if (vpu.Uur % 2 == 1)
                    {
                        vpu.spUur.Background = new SolidColorBrush(Colors.Beige);
                    }



                    // als hij ingepland is
                    if (mandag != null)
                    {

                        blnIsEditor = mandag.MutatieDoorProjectleiderId == objProjectleider.ProjectleiderId;

                        // iemand geeft een vakman aan mij
                        // stippelijn door iemand anders bij mij geplaatst
                        // niet bevestigd wel owner en gewijzigd door iemand anders
                        // als het project gewijzigd is, door iemand anders, en de wijziging mijn uren betreft
                        if (!mandag.Bevestigd && blnIsOwner && mandag.Gewijzigd && !blnIsEditor && mandag.UrenGewijzigd > 0)
                        {
                            blnIsAllowedToConfirm = true;
                            blnIsAllowedToCancel = true;
                        }

                        // iemand pakt een vakman van mij
                        // solid lijn
                        // wel bevestigd, maar gemuteerd door iemand anders en ik ben owner
                        if (mandag.Bevestigd && blnIsOwner && mandag.Gewijzigd && !blnIsEditor && mandag.Uren > 0)
                        {
                            blnIsAllowedToConfirm = true;
                            blnIsAllowedToCancel = true;
                        }

                        if (mandag.Geannulleerd && !blnIsEditor)
                        {

                            //vakmandagproject.IsOranje = true;
                            //vakmandagproject.Duration = mandag.UrenGewijzigd;
                            //vakmandagproject.IsSolid = false;
                            blnIsAllowedToAcceptAnnulleringen = blnIsOwner;
                            // flag op true zetten, zorgt er automatisch voor dat cancel en confirm buttons weergegeven kunnen worden
                            IsGewijzigd = true;

                        }
                        // als de mandag bevestigd is, niks aan de hand, gewoon de normale uren weergeven
                        else if (mandag.Bevestigd)
                        {
                            //vakmandagproject.Duration = mandag.Uren;
                            //vakmandagproject.IsSolid = true;

                            // if (!blnIsOwner && vakmandagproject.Duration > 0)
                            if (blnIsOpVreemdProjectIngepland && !blnIsOwner)
                            {
                                //vakmandagproject.ReadOnly = true;
                                blnIsAllowedToSetZiek = false;
                                blnIsAllowedToSetVakantie = false;
                            }
                            //else if (blnIsOwner && vakmandagproject.Duration > 0)
                            //{
                            //    // TODO: weeer aanzetten, nu even uitzetten
                            //    // vakmandagproject.spVakmanDagUren.Visibility = System.Windows.Visibility.Visible;
                            //    //vakmandagproject.lblVakmandagUren.Content = mandag.Uren;
                            //    //vakmandagproject.lblVakmandagMinuten.Content = mandag.Minuten;
                            //}
                        }
                        else // als niet bevestigd, dan is er iets gewijzigd, hetzij een wijziging, hetzij een nieuwe invoer
                        {
                            //vakmandagproject.Duration = mandag.UrenGewijzigd;
                            //vakmandagproject.IsSolid = false;

                            if (blnIsOwner || blnIsEditor)
                            {
                                blnIsAllowedToCancel = true;
                            }

                            // flag op true zetten, zorgt er automatisch voor dat cancel en confirm buttons weergegeven kunnen worden
                            IsGewijzigd = true;

                            // bij gewijzigde items de gewijzigde uren weergeven
                        }
                    }
                    else // niet ingepland, dus niks veranderen aan default situatie, duration = 0
                    {
                        //vakmandagproject.Duration = 0;
                    }

                    vpu.blnIsEnabled = blnIsOwner || blnIsOpMijnProjectIngepland;

                    // uiteindelijk toevoegen
                    // altijd 24x, voor elk uur 1
                    spVakmanDag.Children.Add(vpu);
                }


                // klaar met instellingen, vakmandagproject toevoegen
                // spVakmanDag.Children.Add(GetProjectUren();
            }




            // Niet ingevuld toevoegen
            VakmanDagProject vakmandagprojectNietIngevuld = new VakmanDagProject();
            vakmandagprojectNietIngevuld.ProjectId = 0;
            //vakmandagprojectNietIngevuld.OnVakmanDagProjectUpdate += new RoutedEventHandler(recDag_MouseDown);
            vakmandagprojectNietIngevuld.IsNietIngevuld = true;

            // als ik (degene die dit scherm bekijkt) de projectleider ben
            vakmandagprojectNietIngevuld.IsGroen = false;
            vakmandagprojectNietIngevuld.ReadOnly = blnIsAllowedToCancel || blnIsAllowedToAcceptAnnulleringen || ((blnIsOpVreemdProjectIngepland || blnIsNotOwner));
            //vakmandagprojectNietIngevuld.ReadOnly = blnIsAllowedToCancel || blnIsAllowedToAcceptAnnulleringen || ((blnIsOpVreemdProjectIngepland || blnIsNotOwner) && !blnIsZiek && !blnIsVakantie);


            vakmandagprojectNietIngevuld.IsSolid = true;
            vakmandagprojectNietIngevuld.Duration = blnIsOpProjectIngepland ? 0 : 8; ;

            // klaar met instellingen, vakmandagproject toevoegen
            //spVakmanDag.Children.Add(vakmandagprojectNietIngevuld);


            svScrollviewer.ScrollToHorizontalOffset(560);


            //// visibility van knoppen instellen
            //if (blnIsAllowedToConfirm)
            //{
            //    recOK.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    recOK.Visibility = System.Windows.Visibility.Hidden;
            //}

            //// visibility van knoppen instellen
            //if (blnIsAllowedToCancel)
            //{
            //    recCancel.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    recCancel.Visibility = System.Windows.Visibility.Hidden;
            //}

            //if (blnIsAllowedToAcceptAnnulleringen)
            //{
            //    recReadGeannulleerd.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    recReadGeannulleerd.Visibility = System.Windows.Visibility.Hidden;
            //}

        }

        public List<VakmanDagViewProjectUur> GetProjectUren(List<Mandagen> mandagen, Project project, DateTime dtDag)
        {
            List<VakmanDagViewProjectUur> resultSet = new List<VakmanDagViewProjectUur>();

            for (int i = 0; i < 24; i++)
            {
                VakmanDagViewProjectUur vdvpu = new VakmanDagViewProjectUur();
                vdvpu.lblUur.Content = i.ToString() + "u";

                bool blnQ1 = false;
                bool blnQ2 = false;
                bool blnQ3 = false;
                bool blnQ4 = false;

                DateTime dtQ1 = new DateTime(dtDag.Year, dtDag.Month, dtDag.Day, i, 0, 0);
                DateTime dtQ2 = new DateTime(dtDag.Year, dtDag.Month, dtDag.Day, i, 15, 0);
                DateTime dtQ3 = new DateTime(dtDag.Year, dtDag.Month, dtDag.Day, i, 30, 0);
                DateTime dtQ4 = new DateTime(dtDag.Year, dtDag.Month, dtDag.Day, i, 45, 0);

                blnQ1 = mandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1);
                blnQ2 = mandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2);
                blnQ3 = mandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3);
                blnQ4 = mandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4);

                // set colors
                vdvpu.btn15.Background = blnQ1 ? new SolidColorBrush(Colors.OrangeRed) : new SolidColorBrush(Colors.White);
                vdvpu.btn30.Background = blnQ2 ? new SolidColorBrush(Colors.OrangeRed) : new SolidColorBrush(Colors.White);
                vdvpu.btn45.Background = blnQ3 ? new SolidColorBrush(Colors.OrangeRed) : new SolidColorBrush(Colors.White);
                vdvpu.btn60.Background = blnQ4 ? new SolidColorBrush(Colors.OrangeRed) : new SolidColorBrush(Colors.White);

                vdvpu.Q1Selected = blnQ1;
                vdvpu.Q2Selected = blnQ2;
                vdvpu.Q3Selected = blnQ3;
                vdvpu.Q4Selected = blnQ4;
                
                vdvpu.Uur = i;

                resultSet.Add(vdvpu);
            }

            return resultSet;
        }


        public void LoadWeekInfo()
        {
            lblDagInfo.Content = "Week " + GetWeekNumber(dtBegintijd).ToString() + ", " + dtBegintijd.ToString("dddd dd MMM yyyy");

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

        public static readonly RoutedEvent VakmanDagViewEvent = EventManager.RegisterRoutedEvent("OnVakmanDagViewUpdate", RoutingStrategy.Bubble,
        typeof(RoutedEventHandler), typeof(VakmanDagView));

        public event RoutedEventHandler OnVakmanDagViewUpdate
        {
            add { AddHandler(VakmanDagViewEvent, value); }
            remove { RemoveHandler(VakmanDagViewEvent, value); }
        }


        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            ApplicationState.SetValue("dtSelectedDay",dtBegintijd.AddDays(-1));
            LoadVakmanDagView();
            LoadWeekInfo();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            ApplicationState.SetValue("dtSelectedDay", dtBegintijd.AddDays(1));
            LoadVakmanDagView();
            LoadWeekInfo();
        }


        public static int GetWeekNumber(DateTime dtPassed)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
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

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            int intCountMandagen = 0;
            int i = -1;
            int intCountQuarters = 0;

            bool blnIsMandagFound = false;

            List<Mandagen> listMandagen = new List<Mandagen>();

            foreach (VakmanDagViewProjectUur vpu in spVakmanDag.Children)
            {


                // 
                if (vpu.Q1Selected && !blnIsMandagFound)
                {
                    Mandagen mandag = new Mandagen();
                    listMandagen.Add(mandag);
                    intCountMandagen++;
                    i++;
                    intCountQuarters++;

                    blnIsMandagFound = true;

                    // info die je bij adden al weet:
                    mandag.VakmanId = vpu.vakman.VakmanId;
                    mandag.ProjectId = vpu.project.ProjectId;
                    mandag.Mutatiedatum = DateTime.Now;
                    mandag.MutatieDoorProjectleiderId = objProjectleider.ProjectleiderId;
                    mandag.ProjectleiderId = vpu.project.ProjectleiderId;
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

                    // velden te berekenen
                    mandag.Begintijd = new DateTime(dtBegintijd.Year,dtBegintijd.Month,dtBegintijd.Day,vpu.Uur,0,0);


                }
                else if(vpu.Q1Selected)
                {
                    intCountQuarters++;
                }
                else if (!vpu.Q1Selected)
                {
                    if (i >= 0 && blnIsMandagFound)
                    {
                        // velden te berekenen
                        listMandagen[i].Eindtijd = listMandagen[i].Begintijd.AddMinutes(intCountQuarters * 15);
                        listMandagen[i].Uren = Convert.ToInt32(Math.Floor(Convert.ToDouble(intCountQuarters / 4)));
                        listMandagen[i].UrenGewijzigd = listMandagen[i].Uren;
                        listMandagen[i].Minuten = (intCountQuarters * 15) - (listMandagen[i].Uren * 60);
                        listMandagen[i].MinutenGewijzigd = listMandagen[i].Minuten;

                        intCountQuarters = 0;
                    }

                    blnIsMandagFound = false;

                }

                if (vpu.Q2Selected && !blnIsMandagFound)
                {
                    Mandagen mandag = new Mandagen();
                    listMandagen.Add(mandag);
                    intCountMandagen++;
                    i++;
                    intCountQuarters++;

                    blnIsMandagFound = true;

                    // info die je bij adden al weet:
                    mandag.VakmanId = vpu.vakman.VakmanId;
                    mandag.ProjectId = vpu.project.ProjectId;
                    mandag.Mutatiedatum = DateTime.Now;
                    mandag.MutatieDoorProjectleiderId = objProjectleider.ProjectleiderId;
                    mandag.ProjectleiderId = vpu.project.ProjectleiderId;
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

                    // velden te berekenen
                    mandag.Begintijd = new DateTime(dtBegintijd.Year, dtBegintijd.Month, dtBegintijd.Day, vpu.Uur, 15, 0);

                }
                else if(vpu.Q2Selected)
                {
                    intCountQuarters++;
                }
                else if (!vpu.Q2Selected)
                {
                    if (i >= 0 && blnIsMandagFound)
                    {
                        listMandagen[i].Eindtijd = listMandagen[i].Begintijd.AddMinutes(intCountQuarters * 15);
                        listMandagen[i].Uren = Convert.ToInt32(Math.Floor(Convert.ToDouble(intCountQuarters / 4)));
                        listMandagen[i].UrenGewijzigd = listMandagen[i].Uren;
                        listMandagen[i].Minuten = (intCountQuarters * 15) - (listMandagen[i].Uren * 60);
                        listMandagen[i].MinutenGewijzigd = listMandagen[i].Minuten;

                        intCountQuarters = 0;
                    }

                    blnIsMandagFound = false;
                }

                if (vpu.Q3Selected && !blnIsMandagFound)
                {
                    Mandagen mandag = new Mandagen();
                    listMandagen.Add(mandag);
                    intCountMandagen++;
                    i++;
                    intCountQuarters++;

                    blnIsMandagFound = true;

                    // info die je bij adden al weet:
                    mandag.VakmanId = vpu.vakman.VakmanId;
                    mandag.ProjectId = vpu.project.ProjectId;
                    mandag.Mutatiedatum = DateTime.Now;
                    mandag.MutatieDoorProjectleiderId = objProjectleider.ProjectleiderId;
                    mandag.ProjectleiderId = vpu.project.ProjectleiderId;
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

                    // velden te berekenen
                    mandag.Begintijd = new DateTime(dtBegintijd.Year, dtBegintijd.Month, dtBegintijd.Day, vpu.Uur, 30, 0);

                }
                else if(vpu.Q3Selected)
                {
                    intCountQuarters++;
                }
                else if (!vpu.Q3Selected)
                {
                    if (i >= 0 && blnIsMandagFound)
                    {
                        listMandagen[i].Eindtijd = listMandagen[i].Begintijd.AddMinutes(intCountQuarters * 15);
                        listMandagen[i].Uren = Convert.ToInt32(Math.Floor(Convert.ToDouble(intCountQuarters / 4)));
                        listMandagen[i].UrenGewijzigd = listMandagen[i].Uren;
                        listMandagen[i].Minuten = (intCountQuarters * 15) - (listMandagen[i].Uren * 60);
                        listMandagen[i].MinutenGewijzigd = listMandagen[i].Minuten;

                        intCountQuarters = 0;
                    }

                    blnIsMandagFound = false;
                }

                if (vpu.Q4Selected && !blnIsMandagFound)
                {
                    Mandagen mandag = new Mandagen();
                    listMandagen.Add(mandag);
                    intCountMandagen++;
                    i++;
                    intCountQuarters++;

                    blnIsMandagFound = true;

                    // info die je bij adden al weet:
                    mandag.VakmanId = vpu.vakman.VakmanId;
                    mandag.ProjectId = vpu.project.ProjectId;
                    mandag.Mutatiedatum = DateTime.Now;
                    mandag.MutatieDoorProjectleiderId = objProjectleider.ProjectleiderId;
                    mandag.ProjectleiderId = vpu.project.ProjectleiderId;
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

                    // velden te berekenen
                    mandag.Begintijd = new DateTime(dtBegintijd.Year, dtBegintijd.Month, dtBegintijd.Day, vpu.Uur, 45, 0);

                }
                else if(vpu.Q4Selected)
                {
                    intCountQuarters++;
                }
                else if (!vpu.Q4Selected)
                {
                    if (i >= 0 && blnIsMandagFound)
                    {
                        listMandagen[i].Eindtijd = listMandagen[i].Begintijd.AddMinutes(intCountQuarters * 15);
                        listMandagen[i].Uren = Convert.ToInt32(Math.Floor(Convert.ToDouble(intCountQuarters / 4)));
                        listMandagen[i].UrenGewijzigd = listMandagen[i].Uren;
                        listMandagen[i].Minuten = (intCountQuarters * 15) - (listMandagen[i].Uren * 60);
                        listMandagen[i].MinutenGewijzigd = listMandagen[i].Minuten;

                        intCountQuarters = 0;
                    }

                    blnIsMandagFound = false;
                }

            }

            lblVakmanBsn.Content = intCountMandagen.ToString();



            dbRepository dbrep = new dbRepository();
            dbrep.ResetMandagenNietIngevuldVoorVakmanId(objVakman.VakmanId, dtBegintijd, objProjectleider.ProjectleiderId);

            // allemaal toevoegen
            foreach (Mandagen mandag in listMandagen)
            {
                dbrep.InsertMandag(mandag);
            }


        }
    }

}

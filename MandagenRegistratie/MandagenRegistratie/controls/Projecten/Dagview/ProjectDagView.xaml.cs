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
using ZeebregtsLogic;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Forms.VisualStyles;
using System.Threading;
using MandagenRegistratie.controls.Vakmannen.Detail;
using MandagenRegistratie.controls.Projecten.Weekview;

namespace MandagenRegistratie.controls.Projecten.Dagview
{
    /// <summary>
    /// Interaction logic for VakmanDagView.xaml
    /// </summary>
    public partial class ProjectDagView : UserControl
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

        public DateTime dtDatumNu = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
        public DateTime dtMaandag;

        //public Projectleider objProjectleider;
        //public List<Project> listProjecten;
        public List<Project> listProjectenAll;
        public int intProjectleider;
        public int intProjectId;

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


        public Gebruiker objProjectleider;

        public List<vwVakman> listVakmannen;
        public List<int> listViewVakmannen = new List<int>();
        //public Project objProjectLeidend;

        //public Project objProject;

        public int ProjectId { get; set; }

        // extra
        private List<Mandagen> listMandagen;
        public DateTime dtBegintijd;

        public delegate void backgroundWorkerDelegate();
        bool canExecute = true;
        private BackgroundWorker worker = new BackgroundWorker();
        private BackgroundWorker workerTerug = new BackgroundWorker();

        public Project objProject;

        public ProjectDagView()
        {
            InitializeComponent();

            // default op adden staan
            ApplicationState.SetValue(ApplicationVariables.intDefaultMode, 1);

        }

                /// <summary>
        /// 
        /// </summary>
        public void LoadVakmanDagView(bool blnDefaultScroll, int projectId, DateTime selectedDay, bool isReload = false)
        {

            ApplicationState.SetValue(ApplicationVariables.intProjectId, projectId);
            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, selectedDay);

            ApplicationState.SetValue("blnIsReload", isReload);

            dtBegintijd = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);

            if (datePicker1.SelectedDate == dtBegintijd)
            {
                LoadVakmanDagView(blnDefaultScroll, -1, isReload);
            }
            else
            { 
                // will automatically trigger LoadVakmanDagView()
                datePicker1.SelectedDate = dtBegintijd;
            }

            //LoadVakmanDagView(blnDefaultScroll);
        }


        public void ResetDagButtons()
        {
            btnMa.FontSize = 11;
            btnMa.FontWeight = FontWeights.Normal;
            btnDi.FontSize = 11;
            btnDi.FontWeight = FontWeights.Normal;
            btnWo.FontSize = 11;
            btnWo.FontWeight = FontWeights.Normal;
            btnDo.FontSize = 11;
            btnDo.FontWeight = FontWeights.Normal;
            btnVr.FontSize = 11;
            btnVr.FontWeight = FontWeights.Normal;
            btnZa.FontSize = 11;
            btnZa.FontWeight = FontWeights.Normal;
            btnZo.FontSize = 11;
            btnZo.FontWeight = FontWeights.Normal;

            txtMa.TextDecorations = null;
            txtDi.TextDecorations = null;
            txtWo.TextDecorations = null;
            txtDo.TextDecorations = null;
            txtVr.TextDecorations = null;
            txtZa.TextDecorations = null;
            txtZo.TextDecorations = null;


            btnMa2.FontSize = 11;
            btnMa2.FontWeight = FontWeights.Normal;
            btnDi2.FontSize = 11;
            btnDi2.FontWeight = FontWeights.Normal;
            btnWo2.FontSize = 11;
            btnWo2.FontWeight = FontWeights.Normal;
            btnDo2.FontSize = 11;
            btnDo2.FontWeight = FontWeights.Normal;
            btnVr2.FontSize = 11;
            btnVr2.FontWeight = FontWeights.Normal;
            btnZa2.FontSize = 11;
            btnZa2.FontWeight = FontWeights.Normal;
            btnZo2.FontSize = 11;
            btnZo2.FontWeight = FontWeights.Normal;

            txtMa2.TextDecorations = null;
            txtDi2.TextDecorations = null;
            txtWo2.TextDecorations = null;
            txtDo2.TextDecorations = null;
            txtVr2.TextDecorations = null;
            txtZa2.TextDecorations = null;
            txtZo2.TextDecorations = null;

        }

        /// <summary>
        /// mag niet enumeraten in wrappanel, dus deze functie blijft ongebruikt
        /// </summary>
        /// <param name="selectedDay"></param>
        public void SetColors(DateTime selectedDay)
        {
            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, selectedDay);

            dtBegintijd = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);

            SetDagButtons(selectedDay);
            int count = 0;

            //foreach (Control control in spVakmanDagLabels)
            //{
            //    if (control.GetType() == typeof(ProjectWeekViewProjectDagLabel))
            //    {
            //        ProjectWeekViewProjectDagLabel vdl = ((ProjectWeekViewProjectDagLabel)control);

            //        vdl.Dag = tools.Functies.GetDayOfWeek(count);
            //        vdl.IsSelected = dtMaandag.AddDays(count).DayOfWeek == selectedDay.DayOfWeek;
            //        vdl.Load();
            //    }
            //    count++;
            
            //}

        }


        public void SetDagButtons(DateTime dtDatum)
        {
            ResetDagButtons();

            switch (dtDatum.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    btnMa.FontSize = 12;
                    btnMa.FontWeight = FontWeights.ExtraBold;
                    txtMa.TextDecorations = TextDecorations.Underline;
                    btnMa2.FontSize = 12;
                    btnMa2.FontWeight = FontWeights.ExtraBold;
                    txtMa2.TextDecorations = TextDecorations.Underline;
                    break;
                case DayOfWeek.Tuesday:
                    btnDi.FontSize = 12;
                    btnDi.FontWeight = FontWeights.ExtraBold;
                    txtDi.TextDecorations = TextDecorations.Underline;
                    btnDi2.FontSize = 12;
                    btnDi2.FontWeight = FontWeights.ExtraBold;
                    txtDi2.TextDecorations = TextDecorations.Underline;
                    break;
                case DayOfWeek.Wednesday:
                    btnWo.FontSize = 12;
                    btnWo.FontWeight = FontWeights.ExtraBold;
                    txtWo.TextDecorations = TextDecorations.Underline;
                    btnWo2.FontSize = 12;
                    btnWo2.FontWeight = FontWeights.ExtraBold;
                    txtWo2.TextDecorations = TextDecorations.Underline;
                    break;
                case DayOfWeek.Thursday:
                    btnDo.FontSize = 12;
                    btnDo.FontWeight = FontWeights.ExtraBold;
                    txtDo.TextDecorations = TextDecorations.Underline;
                    btnDo2.FontSize = 12;
                    btnDo2.FontWeight = FontWeights.ExtraBold;
                    txtDo2.TextDecorations = TextDecorations.Underline;
                    break;
                case DayOfWeek.Friday:
                    btnVr.FontSize = 12;
                    btnVr.FontWeight = FontWeights.ExtraBold;
                    txtVr.TextDecorations = TextDecorations.Underline;
                    btnVr2.FontSize = 12;
                    btnVr2.FontWeight = FontWeights.ExtraBold;
                    txtVr2.TextDecorations = TextDecorations.Underline;
                    break;
                case DayOfWeek.Saturday:
                    btnZa.FontSize = 12;
                    btnZa.FontWeight = FontWeights.ExtraBold;
                    txtZa.TextDecorations = TextDecorations.Underline;
                    btnZa2.FontSize = 12;
                    btnZa2.FontWeight = FontWeights.ExtraBold;
                    txtZa2.TextDecorations = TextDecorations.Underline;
                    break;
                case DayOfWeek.Sunday:
                    btnZo.FontSize = 12;
                    btnZo.FontWeight = FontWeights.ExtraBold;
                    txtZo.TextDecorations = TextDecorations.Underline;
                    btnZo2.FontSize = 12;
                    btnZo2.FontWeight = FontWeights.ExtraBold;
                    txtZo2.TextDecorations = TextDecorations.Underline;
                    break;
                default:
                    break;
            }

        }


        protected void SetModeAdding(object sender, EventArgs e)
        {
            foreach (UIElement control in spOverzicht.Children)
            {
                if (control.GetType() == typeof(WrapPanel))
                {
                    foreach (UIElement innerControl2 in ((WrapPanel)control).Children)
                    {
                        if (innerControl2.GetType() == typeof(WrapPanel))
                        {
                            foreach (UIElement innerControl in ((WrapPanel)innerControl2).Children)
                            {
                                if (innerControl.GetType() == typeof(Button))
                                {
                                    Button btn = (Button)innerControl;
                                    if ((btn).Name == "btn2")
                                    {
                                        // default invullen knop zichtbaar maken
                                        btn.Visibility = System.Windows.Visibility.Visible;
                                    }

                                    if ((btn).Name == "btn3")
                                    {
                                        // default verwijderen knop onzichtbaar maken
                                        btn.Visibility = System.Windows.Visibility.Collapsed;
                                    }
                                }
                            }
                        }
                    }

                }
            }


            ApplicationState.SetValue(ApplicationVariables.intDefaultMode, 1);

            foreach (UIElement control in spOverzicht.Children)
            {
                if (control.GetType() == typeof(ProjectDagProjectHeader))
                {
                    ProjectDagProjectHeader projectDagProjectHeader = (ProjectDagProjectHeader)control;
                    
                    //projectDagProjectHeader.borderDeleting.Visibility = System.Windows.Visibility.Hidden;
                    
                    if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsAdding) == null)
                    {
                        ApplicationState.SetValue(ApplicationVariables.listSelectedVakmanIdsAdding, new List<int>());
                    }
                    
                    if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsAdding).Contains(projectDagProjectHeader.Vakman.VakmanId))
                    {
                        projectDagProjectHeader.cbVakmanSelected.IsChecked = true;
                    }
                    else
                    {
                        projectDagProjectHeader.cbVakmanSelected.IsChecked = false;
                    }

                    projectDagProjectHeader.SetBullit();

                }
            }

        }

        protected void SetModeDelete(object sender, EventArgs e)
        {
            foreach (UIElement control in spOverzicht.Children)
            {
                if (control.GetType() == typeof(WrapPanel))
                {
                    foreach (UIElement innerControl2 in ((WrapPanel)control).Children)
                    {
                        if (innerControl2.GetType() == typeof(WrapPanel))
                        {
                            foreach (UIElement innerControl in ((WrapPanel)innerControl2).Children)
                            {
                                if (innerControl.GetType() == typeof(Button))
                                {
                                    Button btn = (Button)innerControl;
                                    if ((btn).Name == "btn3")
                                    {
                                        // default verwijderen knop zichtbaar maken
                                        btn.Visibility = System.Windows.Visibility.Visible;
                                    }

                                    if ((btn).Name == "btn2")
                                    {
                                        // default invullen knop onzichtbaar maken
                                        btn.Visibility = System.Windows.Visibility.Collapsed;
                                    }
                                }
                            }
                        }
                    }

                }
            }

            ApplicationState.SetValue(ApplicationVariables.intDefaultMode, 0);

            foreach (UIElement control in spOverzicht.Children)
            {
                if (control.GetType() == typeof(ProjectDagProjectHeader))
                {
                    ProjectDagProjectHeader projectDagProjectHeader = (ProjectDagProjectHeader)control;

                    //projectDagProjectHeader.borderAdding.Visibility = System.Windows.Visibility.Hidden;

                    if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsDeleting) == null)
                    {
                        ApplicationState.SetValue(ApplicationVariables.listSelectedVakmanIdsDeleting, new List<int>());
                    }
                    
                    if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsDeleting).Contains(projectDagProjectHeader.Vakman.VakmanId))
                    {
                        projectDagProjectHeader.cbVakmanSelected.IsChecked = true;
                    }
                    else
                    {
                        projectDagProjectHeader.cbVakmanSelected.IsChecked = false;
                    }

                    projectDagProjectHeader.SetBullit();

                }
            }

            

        }


        /// <summary>
        /// 
        /// </summary>
        public void LoadVakmanDagView(bool blnDefaultScroll, int reloadVakmanId = -1, bool isReload = false)
        {
            try
            {
                Logging log = new Logging();

                Logfile.ResetTimer();

                Logfile.Log("Start LoadVakmanDagView()");

                if (ApplicationState.GetValue<int>(ApplicationVariables.intVakmanViewModus) == 0)
                {

                    Mouse.OverrideCursor = Cursors.Wait;

                    spContainer.Width = 1920;
                    spVakmanDag.Width = 1920;
                    spVakmanDagLabels.Width = 1920;
                    spVakmanDagLabelsBottom.Width = 1920;

                    //dtMaandag = CalculateWeekstart(dtBegintijd);
                    SetDagButtons(dtBegintijd);

                    // set de laatste ververstijd van het scherm
                    ApplicationState.SetValue(ApplicationVariables.dtLastRefreshDagView, DateTime.Now);

                    DateTime starttijd = DateTime.Now;

                    //lblDagInfo.Content = 

                    // alleen verwijderen en opnieuw toevoegen bij laden pagina
                    if (reloadVakmanId < 0 && !isReload)
                    {
                        spVakmanDag.Children.Clear();
                    }

                    dbRepository dbrep = new dbRepository();
                    dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
                    intProjectId = ApplicationState.GetValue<int>(ApplicationVariables.intProjectId);
                    intProjectleider = ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider);
                    objProject = dbrep.GetProject(intProjectId);
                    MDRproject objProjectOriginal = dbrepOriginal.GetProject((int)objProject.ProjectNr);


                    //objVakman = dbrep.GetVakman(intVakmanId);
                    objProjectleider = dbrep.GetProjectleider(intProjectleider);
                    MDRpersoon objPersoon = dbrepOriginal.GetContact(dbrep.GetProjectleider(objProject.ProjectleiderId, intProjectId, dtBegintijd).ContactIdOrigineel);

                    lblProject.Content = "Project " + objProjectOriginal.naam_project;
                    lblProject.ToolTip = lblProject.Content.ToString();

                    if (objPersoon != null)
                    {
                        lblProjectleider.Content = ("" + objPersoon.voornaam + " " + objPersoon.tussenvoegsel + " " + objPersoon.achternaam).ToStringTrimmed();
                    }
                    else
                    {
                        lblProjectleider.Content = "..";
                    }


                    //listMandagenByProject = dbrep.GetMandagenByProject(intProjectId, dtBegintijd);
                    listMandagen = dbrep.GetMandagen(dtBegintijd);

                    DateTime dtEind = dtBegintijd.AddHours(24);
                    //bool blnIngepland = listMandagen.Any(m => m.Begintijd >= dtBegintijd && m.Begintijd < dtEind && m.Eindtijd != m.Begintijd && m.Status);
                    //bool blnIngeplandDoorMij = listMandagen.Any(m => m.Begintijd >= dtBegintijd && m.Begintijd < dtEind && m.ProjectleiderId == objProjectleider.ProjectleiderId && m.Eindtijd != m.Begintijd && m.Status);
                    
                    // als ik (degene die dit scherm bekijkt) de projectleider ben
                    bool blnIsProjectOwner = objProject.ProjectleiderId == objProjectleider.ProjectleiderId;

                    //bool blnIsOwner = (blnIsProjectOwner && !blnIngepland) || blnIngeplandDoorMij;
                    bool blnDagEigenaar = dbrep.GetProjectleider(objProject.ProjectleiderId, objProject.ProjectId, dtBegintijd).ProjectleiderId == objProjectleider.ProjectleiderId;


                    if (blnIsProjectOwner || blnDagEigenaar)
                    {
                        listVakmannen = dbrep.GetVakmannenByProjectId(intProjectId, Tools.CalculateWeekstart(dtBegintijd)).OrderBy(v => v.voornaam).ToList();
                    }
                    else
                    {
                        listVakmannen = dbrep.GetVakmannenIngeplandByProjectId(intProjectId, dtBegintijd, 1).OrderBy(v => v.voornaam).ToList();
                    }

                    Logfile.Log("Done getting lists");
                    Logfile.ResetTimer();
                    Logfile.Log("Start region #1");


                    bool blnIsOpVreemdProjectIngepland = false;
                    bool blnIsOpVreemdProjectAangevraagd = false;
                    bool blnIsOpProjectAangevraagd = false;
                    bool blnIsOpProjectIngepland;
                    bool blnIsNotOwner = false;

                    //cbProjecten.ItemsSource = dbrep.GetProjects();
                    //cbProjecten.DisplayMemberPath = "Naam";
                    //cbProjecten.SelectedValuePath = "ProjectId";

                    // load overzicht
                                        // alleen verwijderen en opnieuw toevoegen bij laden pagina
                    if (reloadVakmanId < 0)
                    {


                        spVakmanDagLabels.Children.Clear();
                        spVakmanDagLabelsBottom.Children.Clear();
                    }

                    // canvasWrapper.Children.Clear()
                    // workaround omdat ik anders het WrapPanel ook clear()
                    List<UIElement> controlsToRemove = new List<UIElement>();

                    
                                        // alleen verwijderen en opnieuw toevoegen bij laden pagina
                    if (reloadVakmanId < 0)
                    {
                        foreach (UIElement control in canvasWrapper.Children)
                        {
                            if (control.GetType() != typeof(WrapPanel))
                            {
                                controlsToRemove.Add(control);
                            }
                        }

                        foreach (UIElement control in controlsToRemove)
                        {
                            canvasWrapper.Children.Remove(control);
                        }



                        // loop door de 24 uren
                        foreach (ProjectDagViewProjectUurLabel vpu in GetProjectUrenLabels())
                        {

                            // oneven uren 'arceren'
                            if (vpu.Uur % 2 == 1)
                            {
                                vpu.lblUur.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5D9F1")); // new SolidColorBrush(Colors.LightGray); // LightSteelBlue
                            }

                            // uiteindelijk toevoegen
                            // altijd 24x, voor elk uur 1
                            spVakmanDagLabels.Children.Insert(0, vpu);
                        }

                        for (int i = 0; i < 24; i++)
                        {
                            ProjectDagViewProjectUurLabel vpuEmpty = new ProjectDagViewProjectUurLabel();
                            vpuEmpty.Height = 10;

                            if (i % 2 == 1)
                            {
                                vpuEmpty.lblUur.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5D9F1")); // new SolidColorBrush(Colors.LightGray); // LightSteelBlue
                            }
                            spVakmanDagLabels.Children.Add(vpuEmpty); // TODO: check

                        }


                        // loop door de 24 uren
                        foreach (ProjectDagViewProjectUurLabel vpu in GetProjectUrenLabels())
                        {

                            // oneven uren 'arceren'
                            if (vpu.Uur % 2 == 1)
                            {
                                vpu.lblUur.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5D9F1")); // new SolidColorBrush(Colors.LightGray); // LightSteelBlue
                            }

                            if (listVakmannen.Count <= 5)
                            {
                                vpu.lblUur.Content = " ";
                            }

                            // uiteindelijk toevoegen
                            // altijd 24x, voor elk uur 1
                            spVakmanDagLabelsBottom.Children.Insert(0, vpu);
                        }


                        Logfile.Log("Einde region #1");
                        Logfile.ResetTimer();
                        Logfile.Log("Start region #2");

                    }

                    // alleen verwijderen en opnieuw toevoegen bij laden pagina
                    if (reloadVakmanId < 0)
                    {

                        spOverzicht.Children.Clear();

                        ProjectDagViewProjectUurLabel vpu2 = new ProjectDagViewProjectUurLabel();

                        WrapPanel wpSelectPanel = new WrapPanel();
                        wpSelectPanel.Orientation = Orientation.Horizontal;

                        ProjectDagViewProjectUurLabel vpuEmpty2 = new ProjectDagViewProjectUurLabel();
                        vpuEmpty2.Height = 10;

                        // alleen toevoegen als de gebruiker de owner van het project is
                        if (blnDagEigenaar && listVakmannen.Count > 0)
                        {
                            RadioButton rbDelete = new RadioButton();
                            rbDelete.Margin = new Thickness(5, 7, 5, 0);
                            rbDelete.Checked += new RoutedEventHandler(SetModeDelete);

                            RadioButton rbAdd = new RadioButton();
                            rbAdd.Margin = new Thickness(5, 7, 5, 0);
                            rbAdd.Checked += new RoutedEventHandler(SetModeAdding);


                            if (ApplicationState.GetValue<int>(ApplicationVariables.intDefaultMode) == 1)
                            {
                                rbAdd.IsChecked = true;
                            }
                            else
                            {
                                rbDelete.IsChecked = true;
                            }

                            wpSelectPanel.Children.Add(rbDelete);
                            wpSelectPanel.Children.Add(rbAdd);
                        }
                        else
                        {
                            vpu2.Margin = new Thickness(50, 0, 0, 0);
                        }

                        // 
                        vpuEmpty2.Width = 200;
                        //wpSelectPanel.Children.Add(vpuEmpty2);



                        vpu2.lblUur.Content = "Vakmannen";
                        vpu2.lblUur.FontSize = 14;
                        //vpu2.lblUur.FontWeight = FontWeights.Bold;
                        vpu2.lblUur.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                        vpu2.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                        vpu2.Width = 140;

                        wpSelectPanel.Children.Add(vpu2); // TODO: check
                        spOverzicht.Children.Add(wpSelectPanel); // TODO: check
                        spOverzicht.Children.Add(vpuEmpty2);



                        Logfile.Log("Einde Region #2");

                        Logfile.ResetTimer();
                        Logfile.Log("Start looping listVakmannen");

                        //dbOriginalRepository dbrepOr = new dbOriginalRepository();


                        //List<persoon> listpersoonOrigineel = dbrepOr.GetContactenIn(listVakmannen.Select(vm => Convert.ToInt32(vm.persoon_ID)).ToList());
                        //List<project> listprojectsOrigineel = dbrepOr.GetProjectsIn(listMandagen.Select(md => Convert.ToInt32(md.Project.ProjectIdOrigineel)).ToList());

                    }
                    // einde if(reloadVakmanId < 0)

                        int count = 0;

                    // voor alle vakmannen van dit project
                        foreach (vwVakman vakman in listVakmannen)
                        {
                            count++;

                            Logfile.Log("start new vakman");
                            // HEADERS TOEVOEGEN
                            ProjectDagProjectHeader projectDagProjectHeader = new ProjectDagProjectHeader();
                            if (vakman.VakmanId == ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId))
                            {
                                projectDagProjectHeader.btnHeader.FontWeight = FontWeights.ExtraBold;
                                projectDagProjectHeader.btnHeader.FontSize = 12;

                                //projectDagProjectHeader.btnGoToProject.Content = "<";
                            }


                            bool isEnabled = true;
                            bool isEnabledForDeleting = true;
                            bool hasMandagen = false;

                            //bool isChecked = true;

                            projectDagProjectHeader.Vakman = vakman;

                            //string tooltip = string.Empty;

                            ////persoon p = dbRepOriginal.GetContact(vakman.ContactIdOrigineel);
                            //persoon p = listpersoonOrigineel.FirstOrDefault(v => v.persoon_ID == vakman.ContactIdOrigineel);

                            //if(p != null)
                            //{
                            //    projectDagProjectHeader.btnHeader.Content = (p.voornaam + " " + (p.tussenvoegsel == null ? "" : p.tussenvoegsel) + " " + p.achternaam).ToStringTrimmed();
                            //}


                            //foreach (Mandagen mandag in listMandagen.Where(m => m.Begintijd != m.Eindtijd && m.VakmanId == vakman.VakmanId))
                            //{
                            //    projectDagProjectHeader.btnHeader.ToolTip += "(" + tools.Functies.CalculateUren(mandag).ToString() + "u)";
                            //    projectDagProjectHeader.btnHeader.ToolTip += " " + listprojectsOrigineel.FirstOrDefault(pop => pop.project_ID == mandag.Project.ProjectIdOrigineel).naam_project;
                            //    projectDagProjectHeader.btnHeader.ToolTip += Environment.NewLine;
                            //}


                            // 24 UREN TOEVOEGEN


                            bool blnCanCancel = false;


                            int heightOffset = (25 * count) - 9;
                            int widthOffset = Convert.ToInt32(Math.Round((CalculateDefaultUren(vakman, dtBegintijd) * 80) - 4));


                            if (widthOffset < 0)
                            {
                                widthOffset = -2;
                            }

                            // TEST with vierkantje
                            Border rect = new Border();
                            rect.Width = 8;
                            rect.Height = 8;
                            rect.Background = new SolidColorBrush(Colors.Black);
                            rect.BorderBrush = new SolidColorBrush(Colors.White);
                            rect.BorderThickness = new Thickness(1);
                            rect.Margin = new Thickness(widthOffset, heightOffset, 0, 0);
                            rect.Cursor = Cursors.Cross;


                            Logfile.Log("breakpoint #1 new vakman");

                            // loop door de 24 uren

                            List<ProjectDagViewProjectUur> listProjectDagViewProjectUur = new List<ProjectDagViewProjectUur>();
                            List<ProjectDagViewProjectUur> listProjectDagViewProjectUur2 = new List<ProjectDagViewProjectUur>();

                            if (reloadVakmanId == vakman.VakmanId || isReload)
                            {
                                int teller = 0;
                                foreach (UIElement uie in spVakmanDag.Children)
                                {


                                    if (uie.GetType() == typeof(ProjectDagViewProjectUur))
                                    {
                                        if (teller < (count * 24) && teller >= (count - 1) * 24)
                                        // alleen 24x ophalen
                                        {

                                            listProjectDagViewProjectUur2.Add((ProjectDagViewProjectUur)uie);
                                        }
                                        teller++;
                                    }
                                }

                                listProjectDagViewProjectUur = GetProjectUren(listMandagen, vakman, dtBegintijd, blnDagEigenaar, listProjectDagViewProjectUur2);

                            }
                            else if (reloadVakmanId < 0)
                            {
                                listProjectDagViewProjectUur = GetProjectUren(listMandagen, vakman, dtBegintijd, blnDagEigenaar);
                            }

                            // loop komt hier vanzelf maar 1x langs, omdat alle andere keren de listProjectDagViewProjectUur niet gevuld wordt zie hierboven
                            foreach (ProjectDagViewProjectUur vpu in listProjectDagViewProjectUur)
                            {

                                vpu.project = objProject;
                                vpu.vakman = vakman;

                                // dit alvast doen voordat de setcolors wordt aangeroepen
                                vpu.txt15.Visibility = System.Windows.Visibility.Collapsed;
                                vpu.txt30.Visibility = System.Windows.Visibility.Collapsed;
                                vpu.txt45.Visibility = System.Windows.Visibility.Collapsed;
                                vpu.txt60.Visibility = System.Windows.Visibility.Collapsed;

                                // oneven uren 'arceren'
                                if (vpu.Uur % 2 == 1)
                                {
                                    vpu.spUur.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5D9F1")); // new SolidColorBrush(Colors.LightGray); // LightSteelBlue
                                }

                                // bereken de kleuren
                                vpu.SetColors();

                                // TODO: tijdelijk uitgezet, weer aanzetten
                                //vpu.SetTooltips();

                                // als ergens aangevraagd niet enablen voor default invullen
                                if (vpu.IsErgensDoorMijAangevraagdQ1 || vpu.IsErgensDoorMijAangevraagdQ1AndCancelled || !vpu.IsEnabledQ1)
                                {
                                    isEnabled = false;
                                }
                                else if (vpu.IsErgensDoorMijAangevraagdQ2 || vpu.IsErgensDoorMijAangevraagdQ2AndCancelled || !vpu.IsEnabledQ2)
                                {
                                    isEnabled = false;
                                }
                                else if (vpu.IsErgensDoorMijAangevraagdQ3 || vpu.IsErgensDoorMijAangevraagdQ3AndCancelled || !vpu.IsEnabledQ3)
                                {
                                    isEnabled = false;
                                }
                                else if (vpu.IsErgensDoorMijAangevraagdQ4 || vpu.IsErgensDoorMijAangevraagdQ4AndCancelled || !vpu.IsEnabledQ4)
                                {
                                    isEnabled = false;
                                }


                                // zelfde ook checken
                                if (!vpu.IsEnabledQ1)
                                {
                                    isEnabledForDeleting = false;
                                }
                                else if (!vpu.IsEnabledQ2)
                                {
                                    isEnabledForDeleting = false;
                                }
                                else if (!vpu.IsEnabledQ3)
                                {
                                    isEnabledForDeleting = false;
                                }
                                else if (!vpu.IsEnabledQ4)
                                {
                                    isEnabledForDeleting = false;
                                }

                                // als default is ingevuld, niet enablen
                                if (vpu.mandag != null)
                                {
                                    if (vpu.mandag.Begintijd.Hour == 0 && (vpu.mandag.Eindtijd.Hour + (Convert.ToDouble(vpu.mandag.Eindtijd.Minute) / 60)) == CalculateDefaultUren(vakman, dtBegintijd))
                                    {
                                        isEnabled = false;
                                        // default ingevuld, isEnabledForDeleting blijft true
                                    }
                                }

                                if (vpu.mandag != null)
                                {
                                    hasMandagen = true;
                                }

                                //if (vpu.IsAangevraagdQ1 || vpu.IsAangevraagdQ2 || vpu.IsAangevraagdQ3 || vpu.IsAangevraagdQ4
                                //    || vpu.IsIngeplandQ1 || vpu.IsIngeplandQ2 || vpu.IsIngeplandQ3 || vpu.IsIngeplandQ4
                                //    || !vpu.IsEnabledQ1 || !vpu.IsEnabledQ2 || !vpu.IsEnabledQ3 || !vpu.IsEnabledQ4)
                                //{
                                //    isChecked = false;
                                //}


                                // uiteindelijk toevoegen
                                // altijd 24x, voor elk uur 1

                                if (reloadVakmanId < 0 && !isReload)
                                {
                                    spVakmanDag.Children.Add(vpu);
                                }

                            }
                            // eind loop door 24 uren

                            if (reloadVakmanId < 0 || reloadVakmanId == vakman.VakmanId)
                            {

                                Logfile.Log("breakpoint #2 new vakman");


                                //MessageBox.Show("after#GetProjectUren");

                                // hierna toevoegen aan overzichtskolom
                                projectDagProjectHeader.cbVakmanSelected.IsEnabled = true; // isEnabled;

                                bool isChecked = false;

                                if (ApplicationState.GetValue<int>(ApplicationVariables.intDefaultMode) == 1)
                                {
                                    if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsAdding) != null && ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsAdding).Contains(projectDagProjectHeader.Vakman.VakmanId))
                                    {
                                        isChecked = true;
                                    }
                                }
                                else
                                {
                                    if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsDeleting) != null && ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsDeleting).Contains(projectDagProjectHeader.Vakman.VakmanId))
                                    {
                                        isChecked = true;
                                    }
                                }

                                // TEST
                                // vierkantje alleen tonen als is eigenaar en aangevinkt
                                rect.Visibility = System.Windows.Visibility.Hidden;

                                if (blnDagEigenaar && isChecked)
                                {
                                    rect.Visibility = System.Windows.Visibility.Visible;
                                }

                                rect.Tag = projectDagProjectHeader;

                                canvasWrapper.Children.Add(rect);

                                projectDagProjectHeader.cbVakmanSelected.IsChecked = isChecked;

                                projectDagProjectHeader.IsOwner = blnDagEigenaar;
                                projectDagProjectHeader.IsEnabled = isEnabled && (CalculateDefaultUren(vakman, dtBegintijd) > 0 || CalculateDefaultUren(vakman, dtBegintijd) == 0 && hasMandagen);
                                projectDagProjectHeader.IsEnabledForDeleting = isEnabledForDeleting && hasMandagen;

                                projectDagProjectHeader.SetBullit();

                                projectDagProjectHeader.cbVakmanSelected.Visibility = System.Windows.Visibility.Hidden;

                                // zichtbaar als eigenaar
                                if (blnDagEigenaar)
                                {
                                    projectDagProjectHeader.cbVakmanSelected.Visibility = System.Windows.Visibility.Visible;
                                }

                                if (reloadVakmanId < 0 || reloadVakmanId == vakman.VakmanId)
                                {
                                    int insertPosition = spOverzicht.Children.Count;
                                    if (reloadVakmanId == vakman.VakmanId)
                                    {
                                        insertPosition = count + 1;
                                        spOverzicht.Children.RemoveAt(insertPosition);
                                    }
                                    spOverzicht.Children.Insert(insertPosition, projectDagProjectHeader);
                                }
                                Logfile.Log("breakpoint #3 new vakman");

                                // klaar met instellingen, vakmandagproject toevoegen
                                // spVakmanDag.Children.Add(GetProjectUren();

                            }

                        }

                    Logfile.Log("Done looping listVakmannen");

                    //MessageBox.Show("after#listVakmannen");

                    if (blnDefaultScroll)
                    {
                        //svScrollviewer.ScrollToHorizontalOffset(480);
                    }

                    if (reloadVakmanId < 0)
                    {
                        WrapPanel sp = new WrapPanel();
                        sp.Width = 250;
                        sp.Orientation = Orientation.Horizontal;

                        WrapPanel spInner = new WrapPanel();
                        spInner.Width = 55;
                        spInner.Orientation = Orientation.Horizontal;


                        Button btn2 = new Button();
                        // <Button Content="Vakman toevoegen" Height="23" Name="btnNaarProjecten" Width="116" Click="btnNaarVakmannen_Click" />
                        btn2.Content = " ";
                        btn2.Name = "btn2";

                        // btn2.Visibility = System.Windows.Visibility.Visible;
                        btn2.Height = 23;
                        btn2.Width = 37;
                        btn2.Click += btnDefaultInvullen_Click;
                        btn2.Margin = new Thickness(6, 0, 0, 0);

                        btn2.Visibility = System.Windows.Visibility.Collapsed;

                        //if (blnIsOwner)
                        //{
                        //    btn2.Visibility = System.Windows.Visibility.Visible;
                        //}

                        spInner.Children.Add(btn2);

                        Button btn3 = new Button();
                        btn3.Content = " ";
                        btn3.Name = "btn3";
                        btn3.Height = 23;
                        btn3.Width = 38;
                        btn3.Click += btnUrenVerwijderen_Click;
                        btn3.Margin = new Thickness(6, 0, 0, 0);
                        //sp.Children.Add(btn3);

                        btn3.Visibility = System.Windows.Visibility.Collapsed;

                        if (blnDagEigenaar)
                        {
                            if (ApplicationState.GetValue<int>(ApplicationVariables.intDefaultMode) == 1)
                            {
                                btn2.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                btn3.Visibility = System.Windows.Visibility.Visible;
                            }
                        }


                        spInner.Children.Add(btn3);

                        // vakman toevoegen button
                        Button btn = new Button();
                        //<Button Content="Vakman toevoegen" Height="23" Name="btnNaarProjecten" Width="116" Click="btnNaarVakmannen_Click" />
                        btn.Content = "+";
                        btn.Height = 23;
                        btn.Width = 38;
                        btn.FontSize = 14;
                        btn.Click += btnNaarVakmannen_Click;
                        btn.Margin = new Thickness(0, 0, 0, 0);

                        btn.Visibility = System.Windows.Visibility.Hidden;


                        if (blnDagEigenaar && Rechten.IsProjectleider)
                        {
                            btn.Visibility = System.Windows.Visibility.Visible;
                        }

                        // onzichtbaar maken als er geen vakmannen zijn
                        if (listVakmannen.Count == 0)
                        {
                            spInner.Visibility = System.Windows.Visibility.Hidden;
                        }

                        sp.Children.Add(spInner);



                        sp.Children.Add(btn);


                        spOverzicht.Children.Add(sp);

                        if (count > 0)
                        {
                            canvasWrapper.Height = (count * 25) - 1;
                        }
                        else
                        {
                            canvasWrapper.Height = 0;
                        }

                    }

                    TimeSpan ts = ((TimeSpan)(DateTime.Now - starttijd));
                    //lblVakmanBsn.Content = ts.Seconds.ToString() + "." + ts.Milliseconds.ToString();

                    LoadWeekInfo();

                    Logfile.Log("Done");


                }
                else if (ApplicationState.GetValue<int>(ApplicationVariables.intVakmanViewModus) == 1)
                {
                    LoadProjectWeekView(blnDefaultScroll, isReload);
                }

                Logfile.Log("Eind loadvakmandagview()");

                Logfile.Log("Done 2");

                Logfile.Save();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Mouse.OverrideCursor = null;


        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadProjectWeekView(bool blnDefaultScroll, bool IsReload = false)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            Logging log = new Logging();

            SetDagButtons(dtBegintijd);

            dtMaandag = tools.Functies.CalculateWeekstart(dtBegintijd);

            //log.Log("Start LoadProjectWeekView()");

            // set de laatste ververstijd van het scherm
            ApplicationState.SetValue(ApplicationVariables.dtLastRefreshDagView, DateTime.Now);


            DateTime starttijd = DateTime.Now;

            // intialize
            if (!IsReload)
            {
                spVakmanDag.Children.Clear();
            }

            dbRepository dbrep = new dbRepository();
            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
            intProjectId = ApplicationState.GetValue<int>(ApplicationVariables.intProjectId);
            intProjectleider = ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider);
            objProject = dbrep.GetProject(intProjectId);
            MDRproject objProjectOriginal = dbrepOriginal.GetProject((int)objProject.ProjectNr);

            //objVakman = dbrep.GetVakman(intVakmanId);
            objProjectleider = dbrep.GetProjectleider(intProjectleider);
            MDRpersoon objPersoon = dbrepOriginal.GetContact(dbrep.GetProjectleider(objProject.ProjectleiderId, intProjectId, dtBegintijd).ContactIdOrigineel);

            lblProject.Content = "Project " + objProjectOriginal.naam_project;
            lblProject.ToolTip = lblProject.Content.ToString();

            if (objPersoon != null)
            {
                lblProjectleider.Content = ("" + objPersoon.voornaam + " " + objPersoon.tussenvoegsel + " " + objPersoon.achternaam).ToStringTrimmed();
            }
            else
            {
                lblProjectleider.Content = "..";
            }

            listMandagen = dbrep.GetMandagen(dtBegintijd);

            DateTime dtEind = dtBegintijd.AddHours(24);
            bool blnIngepland = listMandagen.Any(m => m.Begintijd >= dtBegintijd && m.Begintijd < dtEind && m.Eindtijd != m.Begintijd && m.Status);
            bool blnIngeplandDoorMij = listMandagen.Any(m => m.Begintijd >= dtBegintijd && m.Begintijd < dtEind && m.ProjectleiderId == objProjectleider.ProjectleiderId && m.Eindtijd != m.Begintijd && m.Status);

            // als ik (degene die dit scherm bekijkt) de projectleider ben
            bool blnIsOwner = objProject.ProjectleiderId == objProjectleider.ProjectleiderId;

            blnIsOwner = (blnIsOwner && !blnIngepland) || blnIngeplandDoorMij;

            if (Global.useWeekviewLeesstand)
            {
                listVakmannen = dbrep.GetVakmannenIngeplandByProjectId(intProjectId, Tools.CalculateWeekstart(dtBegintijd), 7).OrderBy(v => v.voornaam).ToList();
            }
            else
            {
                if (blnIsOwner)
                {
                    listVakmannen = dbrep.GetVakmannenByProjectId(intProjectId, Tools.CalculateWeekstart(dtBegintijd)).OrderBy(v => v.voornaam).ToList();
                }
                else
                {
                    listVakmannen = dbrep.GetVakmannenIngeplandByProjectId(intProjectId, Tools.CalculateWeekstart(dtBegintijd), 7).OrderBy(v => v.voornaam).ToList();
                }
            }

            //listVakmannen = dbrep.GetVakmannenIngeplandByProjectId(intProjectId, dtMaandag, 7).OrderBy(v => v.voornaam).ToList();

            //listMandagen = dbrep.GetMandagenByProject(intProjectId, dtBegintijd);
            //listMandagen = dbrep.GetMandagenByProjectWeekview(intProjectId, dtMaandag);
            listMandagen = dbrep.GetMandagenByProjectWeekviewAll(dtMaandag);


            //log.Log("Done getting all lists");


            bool blnIsOpVreemdProjectIngepland = false;
            bool blnIsOpVreemdProjectAangevraagd = false;
            bool blnIsOpProjectAangevraagd = false;
            bool blnIsOpProjectIngepland;
            bool blnIsNotOwner = false;

            // load overzicht
            spOverzicht.Children.Clear();

            spVakmanDagLabels.Children.Clear();
            spVakmanDagLabelsBottom.Children.Clear();


            // canvasWrapper.Children.Clear()
            // workaround omdat ik anders het WrapPanel ook clear()
            List<UIElement> controlsToRemove = new List<UIElement>();

            foreach (UIElement control in canvasWrapper.Children)
            {
                if (control.GetType() != typeof(WrapPanel))
                {
                    controlsToRemove.Add(control);
                }
            }

            foreach (UIElement control in controlsToRemove)
            {
                canvasWrapper.Children.Remove(control);
            }


            // loop door de 7 dagen
            for (int i = 0; i < 7; i++)
            {
                ProjectWeekViewProjectDagLabel vdl = new ProjectWeekViewProjectDagLabel();
                vdl.Dag = tools.Functies.GetDayOfWeek(i);
                vdl.IsSelected = (((int)dtBegintijd.DayOfWeek == (i + 1)) || (i == 6 && dtBegintijd.DayOfWeek == DayOfWeek.Sunday));
                vdl.Load();

                // uiteindelijk toevoegen
                // altijd 24x, voor elk uur 1
                spVakmanDagLabels.Children.Add(vdl);
            }

            //log.Log("Done getting all labels");


            ProjectWeekViewProjectDagLabel vpu2 = new ProjectWeekViewProjectDagLabel();
            vpu2.lblDag.Content = "Vakmannen";
            vpu2.lblDag.FontSize = 14;
            //vpu2.lblUur.FontWeight = FontWeights.Bold;
            vpu2.lblDag.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            vpu2.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            
            vpu2.Height = 30;

            spOverzicht.Children.Add(vpu2); // TODO: check


            int count = 0;

            //log.Log("Start looping listVakmannen");

            //dbOriginalRepository dbrepOr = new dbOriginalRepository();

            //List<persoon> listpersoonOrigineel = dbrepOr.GetContactenIn(listVakmannen.Select(vm => Convert.ToInt32(vm.persoon_ID)).ToList());
            //List<project> listprojectsOrigineel = dbrepOr.GetProjectsIn(listMandagen.Select(md => Convert.ToInt32(md.Project.ProjectIdOrigineel)).ToList());
            bool hasVakmannen = false;

            // voor alle vakmannen van dit project
            foreach (vwVakman vakman in listVakmannen)
            {
                hasVakmannen = true;
                count++;

                // HEADERS TOEVOEGEN
                ProjectDagProjectHeader projectDagProjectHeader = new ProjectDagProjectHeader();
                if (vakman.VakmanId == ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId))
                {
                    projectDagProjectHeader.btnHeader.FontWeight = FontWeights.ExtraBold;
                    projectDagProjectHeader.btnHeader.FontSize = 12;

                    //projectDagProjectHeader.btnGoToProject.Content = "<";
                }


                // als ik (degene die dit scherm bekijkt) de projectleider ben
                bool isChecked = false;
                bool isEnabled = false;
                bool isEnabledForDeleting = false;
                bool hasMandagen = false;

                //projectDagProjectHeader.listMandagenTotal = listMandagen;
                projectDagProjectHeader.Vakman = vakman;

                //string tooltip = string.Empty;

                ////persoon p = dbRepOriginal.GetContact(vakman.ContactIdOrigineel);
                //persoon p = listpersoonOrigineel.FirstOrDefault(v => v.persoon_ID == vakman.ContactIdOrigineel);

                //if (p != null)
                //{
                //    projectDagProjectHeader.btnHeader.Content = (p.voornaam + " " + (p.tussenvoegsel == null ? "" : p.tussenvoegsel) + " " + p.achternaam).ToStringTrimmed();
                //}


                //foreach (Mandagen mandag in listMandagen.Where(m => m.Begintijd != m.Eindtijd && m.VakmanId == vakman.VakmanId))
                //{
                //    projectDagProjectHeader.btnHeader.ToolTip += "(" + tools.Functies.CalculateUren(mandag).ToString() + "u)";
                //    projectDagProjectHeader.btnHeader.ToolTip += " " + listprojectsOrigineel.FirstOrDefault(pop => pop.project_ID == mandag.Project.ProjectIdOrigineel).naam_project;
                //    projectDagProjectHeader.btnHeader.ToolTip += Environment.NewLine;
                //}




                projectDagProjectHeader.cbVakmanSelected.IsChecked = isChecked;

                projectDagProjectHeader.IsOwner = blnIsOwner;
                projectDagProjectHeader.IsEnabled = isEnabled;
                projectDagProjectHeader.IsEnabledForDeleting = isEnabledForDeleting && hasMandagen;

                projectDagProjectHeader.spControlWrapper.Visibility = System.Windows.Visibility.Collapsed;
                
                projectDagProjectHeader.spWrapper.Height = 30;

                projectDagProjectHeader.SetBullit();

                projectDagProjectHeader.cbVakmanSelected.Visibility = System.Windows.Visibility.Hidden;

                // zichtbaar als eigenaar
                if (blnIsOwner)
                {
                    projectDagProjectHeader.cbVakmanSelected.Visibility = System.Windows.Visibility.Visible;
                }

                spOverzicht.Children.Add(projectDagProjectHeader);

                // 24 UREN TOEVOEGEN


                bool blnCanCancel = false;

                //int heightOffset = (45 * count) - 9;
                //int widthOffset = Convert.ToInt32(Math.Round((CalculateDefaultUren(objVakman, dtBegintijd) * 80) - 4));

                //if (widthOffset < 0)
                //{
                //    widthOffset = 0;
                //}

                List<ProjectWeekViewProjectDag> listProjectWeekViewProjectDag = new List<ProjectWeekViewProjectDag>();

                if (IsReload)
                {
                    foreach (Control control in spVakmanDag.Children)
                    {
                        if (control.GetType() == typeof(ProjectWeekViewProjectDag))
                        {
                            ProjectWeekViewProjectDag uc = (ProjectWeekViewProjectDag)control;
                            listProjectWeekViewProjectDag.Add(uc);
                        }
                    }

                }
                else
                {
                    for (int i = 0; i < 7; i++)
                    {
                        listProjectWeekViewProjectDag.Add(new ProjectWeekViewProjectDag());
                    }

                }

                // loop door de 7 dagen
                for (int i = 0; i < 7; i++)
                {
                    List<Mandagen> listMandagenTotal = listMandagen.Where(m => m.Begintijd >= dtMaandag.AddDays(i) && m.Eindtijd <= dtMaandag.AddDays(i + 1)).ToList();

                    ProjectWeekViewProjectDag vwd = listProjectWeekViewProjectDag[i];
                    vwd.project = objProject;

                    vwd.vakman = dbrep.GetVakman(vakman.VakmanId);
                    List<Mandagen> listMV = listMandagenTotal.Where(m => m.ProjectId == objProject.ProjectId && m.Begintijd != m.Eindtijd && m.VakmanId == vakman.VakmanId).ToList();
                    vwd.listMandagen = listMV;

                    // vandaag, ander project, {geen projectleider} verwijderd, niet bevestigd (dus aanvraag) 
                    List<Mandagen> listAlleAanvragen = listMandagenTotal.Where(m => m.VakmanId == vakman.VakmanId && !m.Status && m.Begintijd != m.Eindtijd).ToList();
                    List<Mandagen> listAanvragen = listMandagenTotal.Where(m => m.VakmanId == vakman.VakmanId && m.ProjectId != objProject.ProjectId && !m.Status && m.Begintijd != m.Eindtijd).ToList();

                    vwd.IsSelected = (((int)dtBegintijd.DayOfWeek == (i + 1)) || (i == 6 && dtBegintijd.DayOfWeek == DayOfWeek.Sunday));
                    vwd.IsOwner = blnIsOwner;
                    // als in de uren van deze vakman een aanvraag of annulering zit
                    // of als er een aanvraag bestaat die invloed kan hebben op bestaande uren
                    // OF een annulering waarvan ik de eigenaar ben (en dus nog moet accepten)
                    vwd.IsDotted = listMV.Any(m => !m.Status || m.Geannulleerd) || (listAanvragen.Any(a=> !a.Geannulleerd) && listMV.Any(m => m.Status));

                    vwd.datum = dtMaandag.AddDays(i);

                    vwd.Load();

                    // uiteindelijk toevoegen
                    // altijd 24x, voor elk uur 1
                    if (!IsReload)
                    {
                        spVakmanDag.Children.Add(vwd);
                    }
                }

                // klaar met instellingen, vakmandagproject toevoegen
                // spVakmanDag.Children.Add(GetProjectUren();
            }

            //log.Log("Done looping listVakmannen");


            Button btn = new Button();
            //         <Button Content="Project toevoegen" Height="23" Name="btnNaarProjecten" Width="116" Click="btnNaarProjecten_Click" />
            btn.Content = "+";
            btn.FontSize = 14;
            btn.Height = 23;
            btn.Width = 40;
            btn.Margin = new Thickness(6, 0, 0, 0);

            btn.Click += btnNaarVakmannen_Click;

            StackPanel sdummy = new StackPanel();
            sdummy.Width = 250;

            // alleen toevoegen als de juiste rechten aanwezig zijn
            btn.Visibility = System.Windows.Visibility.Hidden;


            if (blnIsOwner && Rechten.IsProjectleider && !Global.useWeekviewLeesstand)
            {
                btn.Visibility = System.Windows.Visibility.Visible;
            }

            if (Rechten.IsProjectleider)
            {
                sdummy.Children.Add(btn);
            }

            sdummy.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            sdummy.Orientation = Orientation.Horizontal;
            //sdummy.Background = new SolidColorBrush(Colors.Blue);

            Label ldummy = new Label();
            ldummy.Width = 210;
            sdummy.Children.Add(ldummy);

            spOverzicht.Children.Add(sdummy);

            if (blnDefaultScroll)
            {
                //svScrollviewer.ScrollToHorizontalOffset(480);
            }

            if (count > 0)
            {
                canvasWrapper.Height = (count * 45) - 1;
            }
            else
            {
                canvasWrapper.Height = 0;
            }


            CalculateWeekviewWidths();

            //log.Log("Start LoadWeekInfo");
            LoadWeekInfo();

            //log.Log("Done LoadWeekInfo");

            Mouse.OverrideCursor = null;

        }


        public double CalculateDefaultUren(vwVakman vakman, DateTime tijd)
        {
            switch (tijd.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return (double)vakman.Ma;
                case DayOfWeek.Tuesday:
                    return (double)vakman.Di;
                case DayOfWeek.Wednesday:
                    return (double)vakman.Wo;
                case DayOfWeek.Thursday:
                    return (double)vakman.Do;
                case DayOfWeek.Friday:
                    return (double)vakman.Vr;
                case DayOfWeek.Saturday:
                    return (double)vakman.Za;
                case DayOfWeek.Sunday:
                    return (double)vakman.Zo;
                default:
                    return 0;
            }
        }

        public double CalculateDefaultUren(Vakman vakman, DateTime tijd)
        {
            switch (tijd.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return (double)vakman.Ma;
                case DayOfWeek.Tuesday:
                    return (double)vakman.Di;
                case DayOfWeek.Wednesday:
                    return (double)vakman.Wo;
                case DayOfWeek.Thursday:
                    return (double)vakman.Do;
                case DayOfWeek.Friday:
                    return (double)vakman.Vr;
                case DayOfWeek.Saturday:
                    return (double)vakman.Za;
                case DayOfWeek.Sunday:
                    return (double)vakman.Zo;
                default:
                    return 0;
            }
        }

        public void CalculateWeekviewWidths()
        {
            double doubleAvailableWidth = (svScrollviewer.Width - 20) / 7;
            double doubleAvailableMaxWidth = (svScrollviewer.Width - 4);

            spContainer.Width = doubleAvailableMaxWidth;
            spVakmanDag.Width = doubleAvailableMaxWidth;
            spVakmanDagLabels.Width = doubleAvailableMaxWidth;
            spVakmanDagLabelsBottom.Width = doubleAvailableMaxWidth;

            foreach (Control control in spVakmanDag.Children)
            {
                if (control.GetType() == typeof(ProjectWeekViewProjectDag))
                {
                    ProjectWeekViewProjectDag uc = (ProjectWeekViewProjectDag)control;
                    uc.bUren.Width = doubleAvailableWidth;
                }
            }

            foreach (Control control in spVakmanDagLabels.Children)
            {
                if (control.GetType() == typeof(ProjectWeekViewProjectDagLabel))
                {
                    ProjectWeekViewProjectDagLabel uc = (ProjectWeekViewProjectDagLabel)control;
                    uc.lblDag.Width = doubleAvailableWidth;
                }
            }

        }


        public Mandagen GetMandagOngeveer(List<Mandagen> mandagen, int vakmanId, DateTime datum, int vakmanStatus, bool status)
        {
            return mandagen.Where(m => m.ProjectId == intProjectId && m.VakmanId == vakmanId && m.Begintijd <= datum && m.Eindtijd >= datum && m.Status == status).OrderByDescending(m => m.Begintijd).FirstOrDefault();
        }


        public List<ProjectDagViewProjectUur> GetProjectUren(List<Mandagen> mandagen, vwVakman vakman, DateTime dtDag, bool blnIsOwner)
        {

            Logfile.ResetTimer();
            Logfile.Log("START getprojecturen");

            List<ProjectDagViewProjectUur> resultSet = new List<ProjectDagViewProjectUur>();

            // alle mandagen van deze vakman op deze dag
            List<Mandagen> alleMandagen = mandagen.Where(m => m.ProjectId > 0 && m.VakmanId == vakman.VakmanId && !m.Geannulleerd).ToList();
            List<Mandagen> mandagenCancelled = mandagen.Where(m => m.VakmanId == vakman.VakmanId && m.Geannulleerd).ToList();
            List<Mandagen> alleMandagenInclCancelled = mandagen.Where(m => m.VakmanId == vakman.VakmanId).ToList();

            Logfile.Log("alleMandagen.Count = " + alleMandagen.Count.ToString());
            Logfile.Log("mandagenCancelled.Count = " + mandagenCancelled.Count.ToString());
            Logfile.Log("alleMandagenInclCancelled.Count = " + alleMandagenInclCancelled.Count.ToString());


            // alle mandagen van deze vakman, voor dit project
            mandagen = mandagen.Where(m => m.VakmanId == vakman.VakmanId && !m.Geannulleerd && m.ProjectId == intProjectId).ToList();

            // bepaal of de vakman vandaag (door iemand anders) op dit project is ingevuld
            DateTime dtVandaagBegin = new DateTime(dtDag.Year, dtDag.Month, dtDag.Day, 0, 0, 0);
            DateTime dtVandaagEind = dtVandaagBegin.AddHours(24);

            bool blnIngepland = mandagen.Any(m => m.Begintijd >= dtVandaagBegin && m.Begintijd < dtVandaagEind && m.Eindtijd != m.Begintijd && m.Status);
            bool blnIngeplandDoorMij = mandagen.Any(m => m.Begintijd >= dtVandaagBegin && m.Begintijd < dtVandaagEind && m.ProjectleiderId == objProjectleider.ProjectleiderId && m.Eindtijd != m.Begintijd && m.Status);

            //ProjectDagViewProjectUur vdvpuTemplate = new ProjectDagViewProjectUur();

            for (int i = 0; i < 24; i++)
            {

                ProjectDagViewProjectUur vdvpu = new ProjectDagViewProjectUur();
                //vdvpu.lblUur.Content = i.ToString() + "u";

                // set de vakman in kwestie
                vdvpu.vakman = vakman;
                //vdvpu.project

                // set ownership!
                //vdvpu.IsOwner = objProject.ProjectleiderId == objProjectleider.ProjectleiderId;
                //vdvpu.IsOwner = blnIsOwner;
                vdvpu.IsOwner = (blnIsOwner && !blnIngepland) || blnIngeplandDoorMij;

                bool blnQ1Ingepland = false;
                bool blnQ2Ingepland = false;
                bool blnQ3Ingepland = false;
                bool blnQ4Ingepland = false;

                bool blnQ1ErgensIngepland = false;
                bool blnQ2ErgensIngepland = false;
                bool blnQ3ErgensIngepland = false;
                bool blnQ4ErgensIngepland = false;

                bool blnQ1Aangevraagd = false;
                bool blnQ2Aangevraagd = false;
                bool blnQ3Aangevraagd = false;
                bool blnQ4Aangevraagd = false;

                bool blnQ1DoorMijAangevraagd = false;
                bool blnQ2DoorMijAangevraagd = false;
                bool blnQ3DoorMijAangevraagd = false;
                bool blnQ4DoorMijAangevraagd = false;

                bool blnQ1AangevraagdAndCancelled = false;
                bool blnQ2AangevraagdAndCancelled = false;
                bool blnQ3AangevraagdAndCancelled = false;
                bool blnQ4AangevraagdAndCancelled = false;

                bool blnQ1ErgensAangevraagd = false;
                bool blnQ2ErgensAangevraagd = false;
                bool blnQ3ErgensAangevraagd = false;
                bool blnQ4ErgensAangevraagd = false;

                bool blnQ1ErgensDoormijIngepland = false;
                bool blnQ2ErgensDoormijIngepland = false;
                bool blnQ3ErgensDoormijIngepland = false;
                bool blnQ4ErgensDoormijIngepland = false;

                bool blnQ1ErgensDoormijAangevraagdEnGecancelled = false;
                bool blnQ2ErgensDoormijAangevraagdEnGecancelled = false;
                bool blnQ3ErgensDoormijAangevraagdEnGecancelled = false;
                bool blnQ4ErgensDoormijAangevraagdEnGecancelled = false;

                bool blnQ1ErgensDoormijAangevraagd = false;
                bool blnQ2ErgensDoormijAangevraagd = false;
                bool blnQ3ErgensDoormijAangevraagd = false;
                bool blnQ4ErgensDoormijAangevraagd = false;

                DateTime dtQ1 = new DateTime(dtDag.Year, dtDag.Month, dtDag.Day, i, 0, 0);
                DateTime dtQ2 = new DateTime(dtDag.Year, dtDag.Month, dtDag.Day, i, 15, 0);
                DateTime dtQ3 = new DateTime(dtDag.Year, dtDag.Month, dtDag.Day, i, 30, 0);
                DateTime dtQ4 = new DateTime(dtDag.Year, dtDag.Month, dtDag.Day, i, 45, 0);

                blnQ1Ingepland = mandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && m.Status);
                blnQ2Ingepland = mandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && m.Status);
                blnQ3Ingepland = mandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && m.Status);
                blnQ4Ingepland = mandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && m.Status);

                blnQ1ErgensIngepland = alleMandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && m.Status);
                blnQ2ErgensIngepland = alleMandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && m.Status);
                blnQ3ErgensIngepland = alleMandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && m.Status);
                blnQ4ErgensIngepland = alleMandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && m.Status);

                //Logfile.Log("breakpoint #2." + i.ToString());

                blnQ1Aangevraagd = mandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status);
                blnQ2Aangevraagd = mandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status);
                blnQ3Aangevraagd = mandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status);
                blnQ4Aangevraagd = mandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status);

                blnQ1DoorMijAangevraagd = mandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status && m.ProjectleiderId == objProjectleider.ProjectleiderId);
                blnQ2DoorMijAangevraagd = mandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status && m.ProjectleiderId == objProjectleider.ProjectleiderId);
                blnQ3DoorMijAangevraagd = mandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status && m.ProjectleiderId == objProjectleider.ProjectleiderId);
                blnQ4DoorMijAangevraagd = mandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status && m.ProjectleiderId == objProjectleider.ProjectleiderId);

                blnQ1AangevraagdAndCancelled = mandagenCancelled.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status && m.ProjectId == objProject.ProjectId);
                blnQ2AangevraagdAndCancelled = mandagenCancelled.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status && m.ProjectId == objProject.ProjectId);
                blnQ3AangevraagdAndCancelled = mandagenCancelled.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status && m.ProjectId == objProject.ProjectId);
                blnQ4AangevraagdAndCancelled = mandagenCancelled.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status && m.ProjectId == objProject.ProjectId);

                blnQ1ErgensAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status);
                blnQ2ErgensAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status);
                blnQ3ErgensAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status);
                blnQ4ErgensAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status);

                //Logfile.Log("breakpoint #3." + i.ToString());

                blnQ1ErgensDoormijIngepland = alleMandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId);
                blnQ2ErgensDoormijIngepland = alleMandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId);
                blnQ3ErgensDoormijIngepland = alleMandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId);
                blnQ4ErgensDoormijIngepland = alleMandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId);

                blnQ1ErgensDoormijAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId);
                blnQ2ErgensDoormijAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId);
                blnQ3ErgensDoormijAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId);
                blnQ4ErgensDoormijAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId);

                blnQ1ErgensDoormijAangevraagdEnGecancelled = alleMandagenInclCancelled.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId && m.Geannulleerd);
                blnQ2ErgensDoormijAangevraagdEnGecancelled = alleMandagenInclCancelled.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId && m.Geannulleerd);
                blnQ3ErgensDoormijAangevraagdEnGecancelled = alleMandagenInclCancelled.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId && m.Geannulleerd);
                blnQ4ErgensDoormijAangevraagdEnGecancelled = alleMandagenInclCancelled.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId && m.Geannulleerd);

                //Logfile.Log("breakpoint #4." + i.ToString());

                vdvpu.IsIngeplandQ1 = blnQ1Ingepland;
                vdvpu.IsIngeplandQ2 = blnQ2Ingepland;
                vdvpu.IsIngeplandQ3 = blnQ3Ingepland;
                vdvpu.IsIngeplandQ4 = blnQ4Ingepland;

                vdvpu.IsErgensDoorMijIngeplandQ1 = blnQ1ErgensDoormijIngepland;
                vdvpu.IsErgensDoorMijIngeplandQ2 = blnQ2ErgensDoormijIngepland;
                vdvpu.IsErgensDoorMijIngeplandQ3 = blnQ3ErgensDoormijIngepland;
                vdvpu.IsErgensDoorMijIngeplandQ4 = blnQ4ErgensDoormijIngepland;

                vdvpu.IsErgensDoorMijAangevraagdQ1 = blnQ1ErgensDoormijAangevraagd;
                vdvpu.IsErgensDoorMijAangevraagdQ2 = blnQ2ErgensDoormijAangevraagd;
                vdvpu.IsErgensDoorMijAangevraagdQ3 = blnQ3ErgensDoormijAangevraagd;
                vdvpu.IsErgensDoorMijAangevraagdQ4 = blnQ4ErgensDoormijAangevraagd;

                vdvpu.IsErgensDoorMijAangevraagdQ1AndCancelled = blnQ1ErgensDoormijAangevraagdEnGecancelled;
                vdvpu.IsErgensDoorMijAangevraagdQ2AndCancelled = blnQ2ErgensDoormijAangevraagdEnGecancelled;
                vdvpu.IsErgensDoorMijAangevraagdQ3AndCancelled = blnQ3ErgensDoormijAangevraagdEnGecancelled;
                vdvpu.IsErgensDoorMijAangevraagdQ4AndCancelled = blnQ4ErgensDoormijAangevraagdEnGecancelled;

                vdvpu.IsSelectedQ1 = blnQ1Ingepland;
                vdvpu.IsSelectedQ2 = blnQ2Ingepland;
                vdvpu.IsSelectedQ3 = blnQ3Ingepland;
                vdvpu.IsSelectedQ4 = blnQ4Ingepland;



                vdvpu.IsAangevraagdQ1 = blnQ1Aangevraagd;
                vdvpu.IsAangevraagdQ2 = blnQ2Aangevraagd;
                vdvpu.IsAangevraagdQ3 = blnQ3Aangevraagd;
                vdvpu.IsAangevraagdQ4 = blnQ4Aangevraagd;

                vdvpu.IsAangevraagdQ1AndCancelled = blnQ1AangevraagdAndCancelled;
                vdvpu.IsAangevraagdQ2AndCancelled = blnQ2AangevraagdAndCancelled;
                vdvpu.IsAangevraagdQ3AndCancelled = blnQ3AangevraagdAndCancelled;
                vdvpu.IsAangevraagdQ4AndCancelled = blnQ4AangevraagdAndCancelled;

                vdvpu.IsErgensAangevraagdQ1 = blnQ1ErgensAangevraagd;
                vdvpu.IsErgensAangevraagdQ2 = blnQ2ErgensAangevraagd;
                vdvpu.IsErgensAangevraagdQ3 = blnQ3ErgensAangevraagd;
                vdvpu.IsErgensAangevraagdQ4 = blnQ4ErgensAangevraagd;

                // hier draaide dit allemaal om....
                vdvpu.IsEnabledQ1 = vdvpu.IsOwner && (!blnQ1ErgensIngepland || blnQ1Ingepland) && !blnQ1ErgensAangevraagd;
                vdvpu.IsEnabledQ2 = vdvpu.IsOwner && (!blnQ2ErgensIngepland || blnQ2Ingepland) && !blnQ2ErgensAangevraagd;
                vdvpu.IsEnabledQ3 = vdvpu.IsOwner && (!blnQ3ErgensIngepland || blnQ3Ingepland) && !blnQ3ErgensAangevraagd;
                vdvpu.IsEnabledQ4 = vdvpu.IsOwner && (!blnQ4ErgensIngepland || blnQ4Ingepland) && !blnQ4ErgensAangevraagd;

                //Logfile.Log("breakpoint #5." + i.ToString());

                vdvpu.CanApprove =
                    (
                        (blnQ1Aangevraagd && blnQ1ErgensDoormijIngepland)
                        || (blnQ2Aangevraagd && blnQ2ErgensDoormijIngepland)
                        || (blnQ3Aangevraagd && blnQ3ErgensDoormijIngepland)
                        || (blnQ4Aangevraagd && blnQ4ErgensDoormijIngepland)
                    );

                //vdvpu.CanCancel = vdvpu.IsOwner &&
                //    (
                //        (blnQ1Aangevraagd)
                //        || (blnQ2Aangevraagd)
                //        || (blnQ3Aangevraagd)
                //        || (blnQ4Aangevraagd)
                //    );

                vdvpu.CanCancel =
                    (
                        (blnQ1DoorMijAangevraagd)
                        || (blnQ2DoorMijAangevraagd)
                        || (blnQ3DoorMijAangevraagd)
                        || (blnQ4DoorMijAangevraagd)
                    );

                vdvpu.CanConfirmHasSeen = vdvpu.IsOwner &&
                    (
                        (blnQ1AangevraagdAndCancelled)
                        || (blnQ2AangevraagdAndCancelled)
                        || (blnQ3AangevraagdAndCancelled)
                        || (blnQ4AangevraagdAndCancelled)
                    );

                //Logfile.Log("breakpoint uur");

                // check voor welke mandag dit 'uur' als eerst van toepassing is.
                if (blnQ1Ingepland)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagen, vakman.VakmanId, dtQ1, 1, true);
                }
                else if (blnQ2Ingepland)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagen, vakman.VakmanId, dtQ2, 1, true);
                }
                else if (blnQ3Ingepland)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagen, vakman.VakmanId, dtQ3, 1, true);
                }
                else if (blnQ4Ingepland)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagen, vakman.VakmanId, dtQ4, 1, true);
                }
                else if (blnQ1Aangevraagd)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagen, vakman.VakmanId, dtQ1, 1, false);
                }
                else if (blnQ2Aangevraagd)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagen, vakman.VakmanId, dtQ2, 1, false);
                }
                else if (blnQ3Aangevraagd)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagen, vakman.VakmanId, dtQ3, 1, false);
                }
                else if (blnQ4Aangevraagd)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagen, vakman.VakmanId, dtQ4, 1, false);
                }
                else if (blnQ1AangevraagdAndCancelled)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagenCancelled, vakman.VakmanId, dtQ1, 1, false);
                }
                else if (blnQ2AangevraagdAndCancelled)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagenCancelled, vakman.VakmanId, dtQ2, 1, false);
                }
                else if (blnQ3AangevraagdAndCancelled)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagenCancelled, vakman.VakmanId, dtQ3, 1, false);
                }
                else if (blnQ4AangevraagdAndCancelled)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagenCancelled, vakman.VakmanId, dtQ4, 1, false);
                }

                // als is aangevraagd && projectleider
                if (vdvpu.mandag != null)
                {

                    if (vdvpu.mandag.Begintijd < dtQ1)
                    {
                        vdvpu.CanApprove = false;
                        vdvpu.CanCancel = false;
                        vdvpu.CanConfirmHasSeen = false;
                    }

                }

                vdvpu.dtUur = dtDag.AddHours(i);

                vdvpu.Uur = i;

                //// hook event on local function (local:)
                vdvpu.OnVakmanDagViewUpdate += new RoutedEventHandler(vakmandagview_OnVakmanDagViewUpdate);
                vdvpu.OnVakmanDagViewRefreshUpdate += new RoutedEventHandler(vdvpu_OnVakmanDagViewRefreshUpdate);
                vdvpu.OnVakmanDagViewHighlight += new RoutedEventHandler(vdvpu_OnVakmanDagViewHighlight);
                vdvpu.OnVakmanDagViewHighlightOnLeave += new RoutedEventHandler(vdvpu_OnVakmanDagViewHighlightOnLeave);

                resultSet.Add(vdvpu);

            }

            Logfile.Log("EINDE getprojecturen");

            return resultSet;
        }

        public List<ProjectDagViewProjectUur> GetProjectUren(List<Mandagen> mandagen, vwVakman vakman, DateTime dtDag, bool blnIsOwner, List<ProjectDagViewProjectUur> listProjectDagViewProjectUur)
        {

            Logfile.ResetTimer();
            Logfile.Log("START getprojecturen");

            List<ProjectDagViewProjectUur> resultSet = new List<ProjectDagViewProjectUur>();

            // alle mandagen van deze vakman op deze dag
            List<Mandagen> alleMandagen = mandagen.Where(m => m.ProjectId > 0 && m.VakmanId == vakman.VakmanId && !m.Geannulleerd).ToList();
            List<Mandagen> mandagenCancelled = mandagen.Where(m => m.VakmanId == vakman.VakmanId && m.Geannulleerd).ToList();
            List<Mandagen> alleMandagenInclCancelled = mandagen.Where(m => m.VakmanId == vakman.VakmanId).ToList();

            Logfile.Log("alleMandagen.Count = " + alleMandagen.Count.ToString());
            Logfile.Log("mandagenCancelled.Count = " + mandagenCancelled.Count.ToString());
            Logfile.Log("alleMandagenInclCancelled.Count = " + alleMandagenInclCancelled.Count.ToString());


            // alle mandagen van deze vakman, voor dit project
            mandagen = mandagen.Where(m => m.VakmanId == vakman.VakmanId && !m.Geannulleerd && m.ProjectId == intProjectId).ToList();

            // bepaal of de vakman vandaag (door iemand anders) op dit project is ingevuld
            DateTime dtVandaagBegin = new DateTime(dtDag.Year, dtDag.Month, dtDag.Day, 0, 0, 0);
            DateTime dtVandaagEind = dtVandaagBegin.AddHours(24);

            bool blnIngepland = mandagen.Any(m => m.Begintijd >= dtVandaagBegin && m.Begintijd < dtVandaagEind && m.Eindtijd != m.Begintijd && m.Status);
            bool blnIngeplandDoorMij = mandagen.Any(m => m.Begintijd >= dtVandaagBegin && m.Begintijd < dtVandaagEind && m.ProjectleiderId == objProjectleider.ProjectleiderId && m.Eindtijd != m.Begintijd && m.Status);




            if (listProjectDagViewProjectUur.Count >= 24)
            {

                for (int i = 0; i < 24; i++)
                {

                    ProjectDagViewProjectUur vdvpu = listProjectDagViewProjectUur[i];
                    //vdvpu.lblUur.Content = i.ToString() + "u";

                    // set de vakman in kwestie
                    vdvpu.vakman = vakman;
                    //vdvpu.project

                    // set ownership!
                    //vdvpu.IsOwner = objProject.ProjectleiderId == objProjectleider.ProjectleiderId;
                    //vdvpu.IsOwner = blnIsOwner;
                    vdvpu.IsOwner = (blnIsOwner && !blnIngepland) || blnIngeplandDoorMij;

                    bool blnQ1Ingepland = false;
                    bool blnQ2Ingepland = false;
                    bool blnQ3Ingepland = false;
                    bool blnQ4Ingepland = false;

                    bool blnQ1ErgensIngepland = false;
                    bool blnQ2ErgensIngepland = false;
                    bool blnQ3ErgensIngepland = false;
                    bool blnQ4ErgensIngepland = false;

                    bool blnQ1Aangevraagd = false;
                    bool blnQ2Aangevraagd = false;
                    bool blnQ3Aangevraagd = false;
                    bool blnQ4Aangevraagd = false;

                    bool blnQ1DoorMijAangevraagd = false;
                    bool blnQ2DoorMijAangevraagd = false;
                    bool blnQ3DoorMijAangevraagd = false;
                    bool blnQ4DoorMijAangevraagd = false;

                    bool blnQ1AangevraagdAndCancelled = false;
                    bool blnQ2AangevraagdAndCancelled = false;
                    bool blnQ3AangevraagdAndCancelled = false;
                    bool blnQ4AangevraagdAndCancelled = false;

                    bool blnQ1ErgensAangevraagd = false;
                    bool blnQ2ErgensAangevraagd = false;
                    bool blnQ3ErgensAangevraagd = false;
                    bool blnQ4ErgensAangevraagd = false;

                    bool blnQ1ErgensDoormijIngepland = false;
                    bool blnQ2ErgensDoormijIngepland = false;
                    bool blnQ3ErgensDoormijIngepland = false;
                    bool blnQ4ErgensDoormijIngepland = false;

                    bool blnQ1ErgensDoormijAangevraagdEnGecancelled = false;
                    bool blnQ2ErgensDoormijAangevraagdEnGecancelled = false;
                    bool blnQ3ErgensDoormijAangevraagdEnGecancelled = false;
                    bool blnQ4ErgensDoormijAangevraagdEnGecancelled = false;

                    bool blnQ1ErgensDoormijAangevraagd = false;
                    bool blnQ2ErgensDoormijAangevraagd = false;
                    bool blnQ3ErgensDoormijAangevraagd = false;
                    bool blnQ4ErgensDoormijAangevraagd = false;

                    DateTime dtQ1 = new DateTime(dtDag.Year, dtDag.Month, dtDag.Day, i, 0, 0);
                    DateTime dtQ2 = new DateTime(dtDag.Year, dtDag.Month, dtDag.Day, i, 15, 0);
                    DateTime dtQ3 = new DateTime(dtDag.Year, dtDag.Month, dtDag.Day, i, 30, 0);
                    DateTime dtQ4 = new DateTime(dtDag.Year, dtDag.Month, dtDag.Day, i, 45, 0);

                    blnQ1Ingepland = mandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && m.Status);
                    blnQ2Ingepland = mandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && m.Status);
                    blnQ3Ingepland = mandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && m.Status);
                    blnQ4Ingepland = mandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && m.Status);

                    blnQ1ErgensIngepland = alleMandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && m.Status);
                    blnQ2ErgensIngepland = alleMandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && m.Status);
                    blnQ3ErgensIngepland = alleMandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && m.Status);
                    blnQ4ErgensIngepland = alleMandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && m.Status);

                    //Logfile.Log("breakpoint #2." + i.ToString());

                    blnQ1Aangevraagd = mandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status);
                    blnQ2Aangevraagd = mandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status);
                    blnQ3Aangevraagd = mandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status);
                    blnQ4Aangevraagd = mandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status);

                    blnQ1DoorMijAangevraagd = mandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status && m.ProjectleiderId == objProjectleider.ProjectleiderId);
                    blnQ2DoorMijAangevraagd = mandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status && m.ProjectleiderId == objProjectleider.ProjectleiderId);
                    blnQ3DoorMijAangevraagd = mandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status && m.ProjectleiderId == objProjectleider.ProjectleiderId);
                    blnQ4DoorMijAangevraagd = mandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status && m.ProjectleiderId == objProjectleider.ProjectleiderId);

                    blnQ1AangevraagdAndCancelled = mandagenCancelled.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status && m.ProjectId == objProject.ProjectId);
                    blnQ2AangevraagdAndCancelled = mandagenCancelled.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status && m.ProjectId == objProject.ProjectId);
                    blnQ3AangevraagdAndCancelled = mandagenCancelled.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status && m.ProjectId == objProject.ProjectId);
                    blnQ4AangevraagdAndCancelled = mandagenCancelled.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status && m.ProjectId == objProject.ProjectId);

                    blnQ1ErgensAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status);
                    blnQ2ErgensAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status);
                    blnQ3ErgensAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status);
                    blnQ4ErgensAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status);

                    //Logfile.Log("breakpoint #3." + i.ToString());

                    blnQ1ErgensDoormijIngepland = alleMandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId);
                    blnQ2ErgensDoormijIngepland = alleMandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId);
                    blnQ3ErgensDoormijIngepland = alleMandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId);
                    blnQ4ErgensDoormijIngepland = alleMandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId);

                    blnQ1ErgensDoormijAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId);
                    blnQ2ErgensDoormijAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId);
                    blnQ3ErgensDoormijAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId);
                    blnQ4ErgensDoormijAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId);

                    blnQ1ErgensDoormijAangevraagdEnGecancelled = alleMandagenInclCancelled.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId && m.Geannulleerd);
                    blnQ2ErgensDoormijAangevraagdEnGecancelled = alleMandagenInclCancelled.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId && m.Geannulleerd);
                    blnQ3ErgensDoormijAangevraagdEnGecancelled = alleMandagenInclCancelled.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId && m.Geannulleerd);
                    blnQ4ErgensDoormijAangevraagdEnGecancelled = alleMandagenInclCancelled.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status && m.Project.ProjectleiderId == objProjectleider.ProjectleiderId && m.Geannulleerd);

                    //Logfile.Log("breakpoint #4." + i.ToString());

                    vdvpu.IsIngeplandQ1 = blnQ1Ingepland;
                    vdvpu.IsIngeplandQ2 = blnQ2Ingepland;
                    vdvpu.IsIngeplandQ3 = blnQ3Ingepland;
                    vdvpu.IsIngeplandQ4 = blnQ4Ingepland;

                    vdvpu.IsErgensDoorMijIngeplandQ1 = blnQ1ErgensDoormijIngepland;
                    vdvpu.IsErgensDoorMijIngeplandQ2 = blnQ2ErgensDoormijIngepland;
                    vdvpu.IsErgensDoorMijIngeplandQ3 = blnQ3ErgensDoormijIngepland;
                    vdvpu.IsErgensDoorMijIngeplandQ4 = blnQ4ErgensDoormijIngepland;

                    vdvpu.IsErgensDoorMijAangevraagdQ1 = blnQ1ErgensDoormijAangevraagd;
                    vdvpu.IsErgensDoorMijAangevraagdQ2 = blnQ2ErgensDoormijAangevraagd;
                    vdvpu.IsErgensDoorMijAangevraagdQ3 = blnQ3ErgensDoormijAangevraagd;
                    vdvpu.IsErgensDoorMijAangevraagdQ4 = blnQ4ErgensDoormijAangevraagd;

                    vdvpu.IsErgensDoorMijAangevraagdQ1AndCancelled = blnQ1ErgensDoormijAangevraagdEnGecancelled;
                    vdvpu.IsErgensDoorMijAangevraagdQ2AndCancelled = blnQ2ErgensDoormijAangevraagdEnGecancelled;
                    vdvpu.IsErgensDoorMijAangevraagdQ3AndCancelled = blnQ3ErgensDoormijAangevraagdEnGecancelled;
                    vdvpu.IsErgensDoorMijAangevraagdQ4AndCancelled = blnQ4ErgensDoormijAangevraagdEnGecancelled;

                    vdvpu.IsSelectedQ1 = blnQ1Ingepland;
                    vdvpu.IsSelectedQ2 = blnQ2Ingepland;
                    vdvpu.IsSelectedQ3 = blnQ3Ingepland;
                    vdvpu.IsSelectedQ4 = blnQ4Ingepland;

                    // JURACI TEST
                    // dit altijd resetten zodra we hierlangs komen
                    vdvpu.IsDeSelectedQ1 = false;
                    vdvpu.IsDeSelectedQ2 = false;
                    vdvpu.IsDeSelectedQ3 = false;
                    vdvpu.IsDeSelectedQ4 = false;


                    vdvpu.IsAangevraagdQ1 = blnQ1Aangevraagd;
                    vdvpu.IsAangevraagdQ2 = blnQ2Aangevraagd;
                    vdvpu.IsAangevraagdQ3 = blnQ3Aangevraagd;
                    vdvpu.IsAangevraagdQ4 = blnQ4Aangevraagd;

                    vdvpu.IsAangevraagdQ1AndCancelled = blnQ1AangevraagdAndCancelled;
                    vdvpu.IsAangevraagdQ2AndCancelled = blnQ2AangevraagdAndCancelled;
                    vdvpu.IsAangevraagdQ3AndCancelled = blnQ3AangevraagdAndCancelled;
                    vdvpu.IsAangevraagdQ4AndCancelled = blnQ4AangevraagdAndCancelled;

                    vdvpu.IsErgensAangevraagdQ1 = blnQ1ErgensAangevraagd;
                    vdvpu.IsErgensAangevraagdQ2 = blnQ2ErgensAangevraagd;
                    vdvpu.IsErgensAangevraagdQ3 = blnQ3ErgensAangevraagd;
                    vdvpu.IsErgensAangevraagdQ4 = blnQ4ErgensAangevraagd;

                    // hier draaide dit allemaal om....
                    vdvpu.IsEnabledQ1 = vdvpu.IsOwner && (!blnQ1ErgensIngepland || blnQ1Ingepland) && !blnQ1ErgensAangevraagd;
                    vdvpu.IsEnabledQ2 = vdvpu.IsOwner && (!blnQ2ErgensIngepland || blnQ2Ingepland) && !blnQ2ErgensAangevraagd;
                    vdvpu.IsEnabledQ3 = vdvpu.IsOwner && (!blnQ3ErgensIngepland || blnQ3Ingepland) && !blnQ3ErgensAangevraagd;
                    vdvpu.IsEnabledQ4 = vdvpu.IsOwner && (!blnQ4ErgensIngepland || blnQ4Ingepland) && !blnQ4ErgensAangevraagd;

                    //Logfile.Log("breakpoint #5." + i.ToString());

                    vdvpu.CanApprove =
                        (
                            (blnQ1Aangevraagd && blnQ1ErgensDoormijIngepland)
                            || (blnQ2Aangevraagd && blnQ2ErgensDoormijIngepland)
                            || (blnQ3Aangevraagd && blnQ3ErgensDoormijIngepland)
                            || (blnQ4Aangevraagd && blnQ4ErgensDoormijIngepland)
                        );

                    //vdvpu.CanCancel = vdvpu.IsOwner &&
                    //    (
                    //        (blnQ1Aangevraagd)
                    //        || (blnQ2Aangevraagd)
                    //        || (blnQ3Aangevraagd)
                    //        || (blnQ4Aangevraagd)
                    //    );

                    vdvpu.CanCancel =
                        (
                            (blnQ1DoorMijAangevraagd)
                            || (blnQ2DoorMijAangevraagd)
                            || (blnQ3DoorMijAangevraagd)
                            || (blnQ4DoorMijAangevraagd)
                        );

                    vdvpu.CanConfirmHasSeen = vdvpu.IsOwner &&
                        (
                            (blnQ1AangevraagdAndCancelled)
                            || (blnQ2AangevraagdAndCancelled)
                            || (blnQ3AangevraagdAndCancelled)
                            || (blnQ4AangevraagdAndCancelled)
                        );

                    //Logfile.Log("breakpoint uur");


                    // JURACI TEST, eerst de mandag resetten aangezien we met een 'gebruikte' ProjectDagViewProjectUur object werken
                    vdvpu.mandag = null;



                    // check voor welke mandag dit 'uur' als eerst van toepassing is.
                    if (blnQ1Ingepland)
                    {
                        vdvpu.mandag = GetMandagOngeveer(mandagen, vakman.VakmanId, dtQ1, 1, true);
                    }
                    else if (blnQ2Ingepland)
                    {
                        vdvpu.mandag = GetMandagOngeveer(mandagen, vakman.VakmanId, dtQ2, 1, true);
                    }
                    else if (blnQ3Ingepland)
                    {
                        vdvpu.mandag = GetMandagOngeveer(mandagen, vakman.VakmanId, dtQ3, 1, true);
                    }
                    else if (blnQ4Ingepland)
                    {
                        vdvpu.mandag = GetMandagOngeveer(mandagen, vakman.VakmanId, dtQ4, 1, true);
                    }
                    else if (blnQ1Aangevraagd)
                    {
                        vdvpu.mandag = GetMandagOngeveer(mandagen, vakman.VakmanId, dtQ1, 1, false);
                    }
                    else if (blnQ2Aangevraagd)
                    {
                        vdvpu.mandag = GetMandagOngeveer(mandagen, vakman.VakmanId, dtQ2, 1, false);
                    }
                    else if (blnQ3Aangevraagd)
                    {
                        vdvpu.mandag = GetMandagOngeveer(mandagen, vakman.VakmanId, dtQ3, 1, false);
                    }
                    else if (blnQ4Aangevraagd)
                    {
                        vdvpu.mandag = GetMandagOngeveer(mandagen, vakman.VakmanId, dtQ4, 1, false);
                    }
                    else if (blnQ1AangevraagdAndCancelled)
                    {
                        vdvpu.mandag = GetMandagOngeveer(mandagenCancelled, vakman.VakmanId, dtQ1, 1, false);
                    }
                    else if (blnQ2AangevraagdAndCancelled)
                    {
                        vdvpu.mandag = GetMandagOngeveer(mandagenCancelled, vakman.VakmanId, dtQ2, 1, false);
                    }
                    else if (blnQ3AangevraagdAndCancelled)
                    {
                        vdvpu.mandag = GetMandagOngeveer(mandagenCancelled, vakman.VakmanId, dtQ3, 1, false);
                    }
                    else if (blnQ4AangevraagdAndCancelled)
                    {
                        vdvpu.mandag = GetMandagOngeveer(mandagenCancelled, vakman.VakmanId, dtQ4, 1, false);
                    }

                    // als is aangevraagd && projectleider
                    if (vdvpu.mandag != null)
                    {

                        if (vdvpu.mandag.Begintijd < dtQ1)
                        {
                            vdvpu.CanApprove = false;
                            vdvpu.CanCancel = false;
                            vdvpu.CanConfirmHasSeen = false;
                        }

                    }

                    vdvpu.dtUur = dtDag.AddHours(i);

                    vdvpu.Uur = i;

                    ////// hook event on local function (local:)
                    //vdvpu.OnVakmanDagViewUpdate += new RoutedEventHandler(vakmandagview_OnVakmanDagViewUpdate);
                    //vdvpu.OnVakmanDagViewRefreshUpdate += new RoutedEventHandler(vdvpu_OnVakmanDagViewRefreshUpdate);
                    //vdvpu.OnVakmanDagViewHighlight += new RoutedEventHandler(vdvpu_OnVakmanDagViewHighlight);
                    //vdvpu.OnVakmanDagViewHighlightOnLeave += new RoutedEventHandler(vdvpu_OnVakmanDagViewHighlightOnLeave);

                    resultSet.Add(vdvpu);

                }
            }

            Logfile.Log("EINDE getprojecturen");

            return resultSet;
        }

        public void vdvpu_OnVakmanDagViewRefreshUpdate(object sender, RoutedEventArgs e)
        {
            //LoadVakmanDagView(false);
        }

        public void vdvpu_OnVakmanDagViewHighlight(object sender, RoutedEventArgs e)
        {
            foreach (ProjectDagViewProjectUur vpu in spVakmanDag.Children)
            {
                if (ApplicationState.GetValue<Mandagen>("highlightMandag") == vpu.mandag)
                {
                    if (vpu.IsAangevraagdQ1)
                    {
                        vpu.txt15.Foreground = new SolidColorBrush(Colors.White);
                    }
                    if (vpu.IsAangevraagdQ2)
                    {
                        vpu.txt30.Foreground = new SolidColorBrush(Colors.White);
                    }
                    if (vpu.IsAangevraagdQ3)
                    {
                        vpu.txt45.Foreground = new SolidColorBrush(Colors.White);
                    }
                    if (vpu.IsAangevraagdQ4)
                    {
                        vpu.txt60.Foreground = new SolidColorBrush(Colors.White);
                    }
                }
            }
        }

        public void vdvpu_OnVakmanDagViewHighlightOnLeave(object sender, RoutedEventArgs e)
        {
            foreach (ProjectDagViewProjectUur vpu in spVakmanDag.Children)
            {
                if (ApplicationState.GetValue<Mandagen>("highlightMandag") == vpu.mandag)
                {
                    if (vpu.IsAangevraagdQ1)
                    {
                        vpu.txt15.Foreground = new SolidColorBrush(Colors.Black);
                    }
                    if (vpu.IsAangevraagdQ2)
                    {
                        vpu.txt30.Foreground = new SolidColorBrush(Colors.Black);
                    }
                    if (vpu.IsAangevraagdQ3)
                    {
                        vpu.txt45.Foreground = new SolidColorBrush(Colors.Black);
                    }
                    if (vpu.IsAangevraagdQ4)
                    {
                        vpu.txt60.Foreground = new SolidColorBrush(Colors.Black);
                    }
                }
            }
        }


        public List<ProjectDagViewProjectUurLabel> GetProjectUrenLabels()
        {
            List<ProjectDagViewProjectUurLabel> resultSet = new List<ProjectDagViewProjectUurLabel>();

            for (int i = 23; i >= 0; i--)
            {
                ProjectDagViewProjectUurLabel vdvpu = new ProjectDagViewProjectUurLabel();
                vdvpu.lblUur.Content = "u" + (i + 1).ToString();
                vdvpu.Uur = i;

                resultSet.Add(vdvpu);
            }

            return resultSet;
        }

        public void LoadWeekInfo()
        {
            lblWeekInfo.Content = "Week " + Tools.GetWeekNumber(dtBegintijd).ToString();
            lblWeekInfo2.Content = "Week " + Tools.GetWeekNumber(dtBegintijd).ToString();

            btnWeekInfo.Content = lblWeekInfo.Content;
            lblDatumInfo.Content = dtBegintijd.ToString("dd MMM yyyy");
            string dagvandeweek = dtBegintijd.ToString("dddd");
            lblDagInfo.Content = dagvandeweek.Substring(0, 1).ToUpper() + dagvandeweek.Substring(1);
            btnDagInfo.Content = lblDagInfo.Content;

            lblDagInfo2.Content = lblDagInfo.Content;
            btnDagInfo2.Content = lblDagInfo.Content;

            if (ApplicationState.GetValue<int>(ApplicationVariables.intVakmanViewModus) == 0)
            {
                bDatepicker.Visibility = Visibility.Visible;
                bDatepicker2.Visibility = Visibility.Collapsed;

                btnDagInfo.Visibility = System.Windows.Visibility.Collapsed;
                lblDagInfo.Visibility = System.Windows.Visibility.Visible;

                btnWeekInfo.Visibility = System.Windows.Visibility.Visible;
                lblWeekInfo.Visibility = System.Windows.Visibility.Collapsed;

            }
            else if (ApplicationState.GetValue<int>(ApplicationVariables.intVakmanViewModus) == 1)
            {
                bDatepicker.Visibility = Visibility.Collapsed;
                bDatepicker2.Visibility = Visibility.Visible;


                //btnWeekInfo.Visibility = System.Windows.Visibility.Collapsed;
                //lblWeekInfo.Visibility = System.Windows.Visibility.Visible;

                //btnDagInfo.Visibility = System.Windows.Visibility.Visible;
                //lblDagInfo.Visibility = System.Windows.Visibility.Collapsed;

                btnWeekInfo2.Visibility = System.Windows.Visibility.Collapsed;
                lblWeekInfo2.Visibility = System.Windows.Visibility.Visible;

                btnDagInfo2.Visibility = System.Windows.Visibility.Visible;
                lblDagInfo2.Visibility = System.Windows.Visibility.Collapsed;



            }

        }

        private void btnVorige_Click(object sender, RoutedEventArgs e)
        {
            GoTerug(1);
        }

        private void btnVolgende_Click(object sender, RoutedEventArgs e)
        {
            GoVerder(1);
        }


        public void GoVerder(int days)
        {
            if (days >= 7)
            {
                ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, Tools.CalculateWeekstart(dtBegintijd.AddDays(days)));
            }
            else
            {
                ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, dtBegintijd.AddDays(days));
            }

            LoadVakmanDagView(false, intProjectId, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay));
            LoadWeekInfo();

        }


        public void GoTerug(int days)
        {
            if (days >= 7)
            {
                ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, Tools.CalculateWeekstart(dtBegintijd.AddDays(-1 * days)));
            }
            else
            {
                ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, dtBegintijd.AddDays(-1 * days));
            }


            LoadVakmanDagView(false, intProjectId, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay));
            LoadWeekInfo();

        }



        int counter = 0;

        private void clCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            counter++;

            // 2e keer niks doen
            if (counter != 2)
            {
                ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, datePicker1.SelectedDate);
                dtBegintijd = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);

                // how long ago?
                TimeSpan interval = DateTime.Now - ApplicationState.GetValue<DateTime>(ApplicationVariables.dtLastRefreshDagView);
                //if (interval.TotalMilliseconds > 500)
                //{

                bool isReload = ApplicationState.GetValue<bool>("blnIsReload");

                LoadVakmanDagView(true, -1, isReload);
                LoadWeekInfo();

                // reset reload
                ApplicationState.SetValue("blnIsReload", false);
                //}

            }

        }

        bool blnCanChange = true;

        private Mandagen CreateMandag(ProjectDagViewProjectUur vpu, DateTime begintijd)
        {

            Mandagen mandag = new Mandagen();
            // info die je bij adden al weet:

            // unieke key
            mandag.VakmanId = vpu.vakman.VakmanId;
            mandag.ProjectId = vpu.project.ProjectId;
            // begintijd
            mandag.Begintijd = begintijd;
            // dit is een wijziging, status true is alleen voor solid kleur
            mandag.Status = false;
            // gedaan door
            mandag.MutatieDoorProjectleiderId = objProjectleider.ProjectleiderId;
            // op
            mandag.Mutatiedatum = DateTime.Now;
            // einde key

            // niet geannulleerd
            mandag.Geannulleerd = false;

            // niet definitief
            mandag.Definitief = false;

            // niet bevestigd
            mandag.Bevestigd = false;


            mandag.IsChauffeurHeen = false;
            mandag.IsChauffeurTerug = false;
            mandag.KentekenHeen = "";
            mandag.KentekenTerug = "";
            mandag.Bevestigingsdatum = DateTime.Now;


            // projectleider op dit moment, voor later in t archief na een eventuele projectleider mutatie
            mandag.ProjectleiderId = objProjectleider.ProjectleiderId; // vpu.project.ProjectleiderId;


            // obsolete, vervangen door status (composite key)
            mandag.Gewijzigd = true;

            // obsolete
            mandag.Uren = 0;
            mandag.Minuten = 0;

            return mandag;

        }

        private Mandagen CreateMandag(Mandagen m, DateTime begintijd, DateTime eindtijd, bool status)
        {

            Mandagen mandag = new Mandagen();
            // info die je bij adden al weet:

            // unieke key
            mandag.VakmanId = m.VakmanId;
            mandag.ProjectId = m.ProjectId;
            // begintijd
            mandag.Begintijd = begintijd;
            mandag.Eindtijd = eindtijd;

            mandag.Status = status;


            // gedaan door
            mandag.MutatieDoorProjectleiderId = m.MutatieDoorProjectleiderId;
            // op
            mandag.Mutatiedatum = m.Mutatiedatum;
            // einde key

            // niet geannulleerd
            mandag.Geannulleerd = m.Geannulleerd;

            // niet definitief
            mandag.Definitief = m.Definitief;

            // niet bevestigd
            mandag.Bevestigd = m.Bevestigd;


            mandag.IsChauffeurHeen = m.IsChauffeurHeen;
            mandag.IsChauffeurTerug = m.IsChauffeurTerug;
            mandag.KentekenHeen = m.KentekenHeen;
            mandag.KentekenTerug = m.KentekenTerug;
            mandag.Bevestigingsdatum = m.Bevestigingsdatum;


            // projectleider op dit moment, voor later in t archief na een eventuele projectleider mutatie
            mandag.ProjectleiderId = m.ProjectleiderId;


            // obsolete, vervangen door status (composite key)
            mandag.Gewijzigd = m.Gewijzigd;

            // obsolete
            mandag.Uren = 0;
            mandag.Minuten = 0;

            return mandag;

        }

        private DateTime CalculateEindtijd(DateTime begintijd, int quarters)
        {
            return begintijd.AddMinutes(quarters * 15);
        }

        private int CalculateQuartersToUren(int quarters)
        {
            return Convert.ToInt32(Math.Floor(Convert.ToDouble(quarters / 4)));
        }

        private int CalculateMinuten(int quarters, int uren)
        {
            return (quarters * 15) - (uren * 60);
        }



        private void vakmandagview_OnVakmanDagViewUpdate(object sender, RoutedEventArgs e)
        {

            //btnConfirm_Click(sender, e);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ConfirmUpdate(int vakmanId)
        {
            if (Rechten.IsProjectleider)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                DateTime starttijd = DateTime.Now;

                int intCountMandagen = 0;
                int i = -1;
                int intCountQuarters = 0;

                //ProjectDagViewProjectUur ppu = ((ProjectDagViewProjectUur)sender);
                //if (ppu.IsEnabledQ1 || ppu.IsEnabledQ2 || ppu.IsEnabledQ3 || ppu.IsEnabledQ4)
                //{
                // set vakmanId to clicked one
                int intVakmanId = vakmanId; //ppu.vakman.VakmanId;

                bool blnIsMandagFound = false;

                List<Mandagen> listMandagen = new List<Mandagen>();

                foreach (ProjectDagViewProjectUur vpu in spVakmanDag.Children)
                {
                    //// alleen als je de owner bent hier iets mee doen
                    if (vpu.IsOwner)
                    {

                        // stabielere manier om te checken of iets 'selected' moet zijn
                        // (we willen af van de 'IsDeSelected')
                        bool isCheckedQ1 = (vpu.IsSelectedQ1 || vpu.IsIngeplandQ1 || vpu.IsAangevraagdQ1) && !vpu.IsDeSelectedQ1;
                        bool isCheckedQ2 = (vpu.IsSelectedQ2 || vpu.IsIngeplandQ2 || vpu.IsAangevraagdQ2) && !vpu.IsDeSelectedQ2;
                        bool isCheckedQ3 = (vpu.IsSelectedQ3 || vpu.IsIngeplandQ3 || vpu.IsAangevraagdQ3) && !vpu.IsDeSelectedQ3;
                        bool isCheckedQ4 = (vpu.IsSelectedQ4 || vpu.IsIngeplandQ4 || vpu.IsAangevraagdQ4) && !vpu.IsDeSelectedQ4;

                        // nieuwe
                        if (isCheckedQ1 && !blnIsMandagFound)
                        {
                            DateTime begintijd = new DateTime(dtBegintijd.Year, dtBegintijd.Month, dtBegintijd.Day, vpu.Uur, 0, 0);
                            Mandagen mandag = CreateMandag(vpu, begintijd);

                            // hebben we nodig om te kijken welke we gaan toevoegen
                            if (vpu.IsOwner)
                            {
                                mandag.MutatieDoorProjectleiderId = objProjectleider.ProjectleiderId;
                            }

                            intCountMandagen++;
                            i++;
                            //intCountQuarters++;

                            blnIsMandagFound = true;

                            listMandagen.Add(mandag);

                        } // bestaande
                        // else if // dit was het
                        if (isCheckedQ1)
                        {
                            intCountQuarters++;

                            if (i >= 0 && blnIsMandagFound)
                            {
                                // velden te berekenen
                                listMandagen[i].Eindtijd = CalculateEindtijd(listMandagen[i].Begintijd, intCountQuarters);
                            }

                        }
                        else if (!isCheckedQ1)
                        {
                            if (i >= 0 && blnIsMandagFound)
                            {
                                // velden te berekenen
                                //listMandagen[i].Eindtijd = CalculateEindtijd(listMandagen[i].Begintijd, intCountQuarters);

                                intCountQuarters = 0;

                            }

                            blnIsMandagFound = false;

                        }

                        if (isCheckedQ2 && !blnIsMandagFound)
                        {
                            DateTime begintijd = new DateTime(dtBegintijd.Year, dtBegintijd.Month, dtBegintijd.Day, vpu.Uur, 15, 0);
                            Mandagen mandag = CreateMandag(vpu, begintijd);

                            // hebben we nodig om te kijken welke we gaan toevoegen
                            if (vpu.IsOwner)
                            {
                                mandag.MutatieDoorProjectleiderId = objProjectleider.ProjectleiderId;
                            }

                            listMandagen.Add(mandag);
                            intCountMandagen++;
                            i++;
                            //intCountQuarters++;

                            blnIsMandagFound = true;

                        }
                        // else if // dit was het
                        if (isCheckedQ2)
                        {
                            intCountQuarters++;

                            if (i >= 0 && blnIsMandagFound)
                            {
                                // velden te berekenen
                                listMandagen[i].Eindtijd = CalculateEindtijd(listMandagen[i].Begintijd, intCountQuarters);
                            }

                        }
                        else if (!isCheckedQ2)
                        {
                            if (i >= 0 && blnIsMandagFound)
                            {
                                // velden te berekenen
                                //listMandagen[i].Eindtijd = CalculateEindtijd(listMandagen[i].Begintijd, intCountQuarters);

                                intCountQuarters = 0;

                            }

                            blnIsMandagFound = false;
                        }

                        if (isCheckedQ3 && !blnIsMandagFound)
                        {
                            DateTime begintijd = new DateTime(dtBegintijd.Year, dtBegintijd.Month, dtBegintijd.Day, vpu.Uur, 30, 0);
                            Mandagen mandag = CreateMandag(vpu, begintijd);

                            // hebben we nodig om te kijken welke we gaan toevoegen
                            if (vpu.IsOwner)
                            {
                                mandag.MutatieDoorProjectleiderId = objProjectleider.ProjectleiderId;
                            }

                            listMandagen.Add(mandag);
                            intCountMandagen++;
                            i++;
                            //intCountQuarters++;

                            blnIsMandagFound = true;

                        }
                        // else if // dit was het
                        if (isCheckedQ3)
                        {
                            intCountQuarters++;

                            if (i >= 0 && blnIsMandagFound)
                            {
                                // velden te berekenen
                                listMandagen[i].Eindtijd = CalculateEindtijd(listMandagen[i].Begintijd, intCountQuarters);
                            }

                        }
                        else if (!isCheckedQ3)
                        {
                            if (i >= 0 && blnIsMandagFound)
                            {
                                // velden te berekenen
                                //listMandagen[i].Eindtijd = CalculateEindtijd(listMandagen[i].Begintijd, intCountQuarters);

                                intCountQuarters = 0;

                            }

                            blnIsMandagFound = false;
                        }

                        if (isCheckedQ4 && !blnIsMandagFound)
                        {
                            DateTime begintijd = new DateTime(dtBegintijd.Year, dtBegintijd.Month, dtBegintijd.Day, vpu.Uur, 45, 0);
                            Mandagen mandag = CreateMandag(vpu, begintijd);

                            // hebben we nodig om te kijken welke we gaan toevoegen
                            if (vpu.IsOwner)
                            {
                                mandag.MutatieDoorProjectleiderId = objProjectleider.ProjectleiderId;
                            }

                            listMandagen.Add(mandag);
                            intCountMandagen++;
                            i++;
                            //intCountQuarters++;

                            blnIsMandagFound = true;

                        }
                        // else if // dit was het
                        if (isCheckedQ4)
                        {
                            intCountQuarters++;

                            if (i >= 0 && blnIsMandagFound)
                            {
                                // velden te berekenen
                                listMandagen[i].Eindtijd = CalculateEindtijd(listMandagen[i].Begintijd, intCountQuarters);
                            }

                        }
                        else if (!isCheckedQ4)
                        {
                            if (i >= 0 && blnIsMandagFound)
                            {
                                // velden te berekenen
                                //listMandagen[i].Eindtijd = CalculateEindtijd(listMandagen[i].Begintijd, intCountQuarters);

                                intCountQuarters = 0;

                            }

                            blnIsMandagFound = false;
                        }
                    }
                }

                //lblVakmanBsn.Content = intCountMandagen.ToString();


                dbRepository dbrep = new dbRepository();

                // get aanvullende lijst met mandagen om de totale mandagenlijst te construeren
                List<Mandagen> listMandagenNoOwner = dbrep.GetMandagenVoorVakmanIdNoOwner(intVakmanId, dtBegintijd, objProjectleider.ProjectleiderId);


                // TODO: CHECK DIT
                // alleen in projectview, splitten hoeft alleen bij mandagen van de aangeklikte vakman
                listMandagen = listMandagen.Where(m => m.VakmanId == intVakmanId).ToList();


                List<Mandagen> listMandagenTotal = new List<Mandagen>();
                listMandagenTotal.AddRange(listMandagen);
                listMandagenTotal.AddRange(listMandagenNoOwner);

                // lijst met alle datums om te splitten
                List<DateTime> datums = CalculateDatums(listMandagenTotal);

                // split de mandagen op basis van de lijst van datums
                listMandagen = SplitMandagen(listMandagen, datums);

                // Delete alle mandagen waarvan ik de owner ben van dit project
                int changeStatus = dbrep.DeleteMandagenVoorVakmanId(intVakmanId, intProjectId, dtBegintijd, objProjectleider.ProjectleiderId, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtLastRefreshDagView));

                if (changeStatus == 1)
                {

                    // TODO: CHECKEN, hoeft volgens mij niet, want anders kon je deze niet eens aanklikken in de projectview
                    // Delete alle (andere) mandagen die ik heb aangevraagd (per definitie dus ook owner)
                    //dbrep.ResetMandagenVoorVakmanId(intVakmanId, dtBegintijd, objProjectleider.ProjectleiderId);

                    // nu alle (nieuwe) aanvragingen toevoegen voor de aangeklikte vakman alleen!
                    foreach (Mandagen mandag in listMandagen.Where(m => m.VakmanId == intVakmanId))
                    {
                        bool blnExists = false;
                        bool blnCausesChanges = false;

                        TimeSpan tsTijdsduur = TimeSpan.FromTicks(mandag.Eindtijd.Ticks - mandag.Begintijd.Ticks);
                        mandag.Uren = tsTijdsduur.Hours;
                        mandag.Minuten = tsTijdsduur.Minutes;


                        // mandag toevoegen
                        dbrep.InsertMandag(mandag);

                        // TODO: DIT TESTEN: heb ik invloed op een bestaande mandag van iemand anders: flag setten
                        //if (dbrep.GetMandagen(intVakmanId, dtBegintijd).Any(m => mandag.Eindtijd > m.Begintijd && mandag.Begintijd < m.Eindtijd && m.ProjectleiderId != objProjectleider.ProjectleiderId && m.Status) || dbrep.GetProject(mandag.ProjectId).ProjectleiderId != objProjectleider.ProjectleiderId)
                        if (dbrep.GetMandagen(intVakmanId, dtBegintijd).Any(m => mandag.Eindtijd > m.Begintijd && mandag.Begintijd < m.Eindtijd && m.ProjectleiderId != objProjectleider.ProjectleiderId && m.Status))
                        {
                            blnCanChange = false;
                        }
                        else
                        {
                            // geen invloed, confirm meteen

                            dbrep.ConfirmMandag(mandag, false);
                        }

                    }

                }
                else if (changeStatus == 0)
                {
                    MessageBox.Show("Wijzigingen niet doorgevoerd, uw vorige wijzigingen zijn nu pas bevestigd.");
                }
                else
                {
                    MessageBox.Show("Wijzigingen niet doorgevoerd, het scherm is in de tussentijd door iemand anders gewijzigd.");
                    Logging log = new Logging();
                    log.Log("Wijzigingen niet doorgevoerd, het scherm is in de tussentijd door iemand anders gewijzigd.");

                }
                //} // end if

                //// reload page:
                //LoadVakmanDagView(false);
                //LoadVakmanDagView(false, intProjectId, dtBegintijd);



                TimeSpan ts = ((TimeSpan)(DateTime.Now - starttijd));
                //lblVakmanBsn.Content = ts.Seconds.ToString() + "." + ts.Milliseconds.ToString();


                //// refresh even alle andere programma's

                //MenuControl owner = Tools.FindVisualParent<MenuControl>(this);

                ////#if DEBUG
                ////#else

                //DateTime dt1 = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
                //DateTime dt2 = dt1.AddDays(1);
                //List<int> vakmanIds = new List<int>();

                //foreach (vwVakman vm in listVakmannen)
                //{
                //    vakmanIds.Add(vm.VakmanId);
                //    listViewVakmannen.Add(vm.VakmanId);
                //}

                if (Global.useChatFunction)
                {
                    SendChannelMessage();
                    //owner.PageChannelMessage("projectdagview", Tools.CreateChannelMessage(vakmanIds, dt1, dt2));
                }

                // JURACI: dit even uitzetten, kijken of de behaviour van de pagina gelijk blijft
                // JURACI TEST, reload = true aangezet hier
                LoadVakmanDagView(false, vakmanId, true);


                //#endif


                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// function can get called by any of the other app instances
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        public void ReceiveChannelMessage(string sender, string message)
        {
            DateTime dt1 = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
            DateTime dt2 = dt1.AddDays(1);

            // indien weekview, dan vergelijken met maandag + 7
            if (ApplicationState.GetValue<int>(ApplicationVariables.intVakmanViewModus) == 1)
            {
                dt1 = dtMaandag;
                dt2 = dt1.AddDays(7);
            }

            List<int> vakmanIds = new List<int>();

            foreach(vwVakman vm in listVakmannen)
            {
                vakmanIds.Add(vm.VakmanId);
            }

            //MessageBox.Show("ReceiveChannelMessage(" + sender + ", " + message + ")");


            if (Tools.RefreshView(vakmanIds, dt1, dt2, message))
            {
                //MessageBox.Show("RefreshPage");
                LoadVakmanDagView(true);
            }
        }

        public List<DateTime> CalculateDatums(List<Mandagen> mandagen)
        {
            List<DateTime> datums = new List<DateTime>();

            foreach (Mandagen mandag in mandagen)
            {
                if (!datums.Contains(mandag.Begintijd))
                {
                    datums.Add(mandag.Begintijd);
                }
                if (!datums.Contains(mandag.Eindtijd))
                {
                    datums.Add(mandag.Eindtijd);
                }
            }

            return datums;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origineleMandagen"></param>
        /// <param name="datums"></param>
        /// <returns></returns>
        public List<Mandagen> SplitMandagen(List<Mandagen> origineleMandagen, List<DateTime> datums)
        {

            List<Mandagen> adjustedMandagen = new List<Mandagen>();

            foreach (Mandagen mandag in origineleMandagen)
            {
                List<Mandagen> mandagResult = new List<Mandagen>();
                mandagResult.Add(mandag);

                // datums op volgorde van datum
                foreach (DateTime datum in datums)
                {
                    Mandagen m = mandagResult.LastOrDefault();
                    if (m.Begintijd < datum && m.Eindtijd > datum)
                    {
                        // moet worden gesplit door deze datum
                        Mandagen nieuweMandag = CreateMandag(m, datum, m.Eindtijd, false);

                        // huidige krijgt andere einddaatum
                        m.Eindtijd = datum;

                        mandagResult.Add(nieuweMandag);
                    }
                }

                adjustedMandagen.AddRange(mandagResult);
            }
            return adjustedMandagen;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="origineleMandagen"></param>
        /// <param name="datums"></param>
        /// <returns></returns>
        public List<Mandagen> SplitMandagenOnSave(List<Mandagen> origineleMandagen, List<DateTime> datums)
        {
            
            List<Mandagen> adjustedMandagen = new List<Mandagen>();

            foreach (Mandagen mandag in origineleMandagen)
            {
                List<Mandagen> mandagResult = new List<Mandagen>();
                mandagResult.Add(mandag);

                // alleen splitten als het een aanvraag is en ik ben niet de eigenaar
                if (mandag.Status == false && mandag.ProjectleiderId != objProjectleider.ProjectleiderId)
                {
                    // datums op volgorde van datum
                    foreach (DateTime datum in datums)
                    {
                        Mandagen m = mandagResult.LastOrDefault();
                        if (m.Begintijd < datum && m.Eindtijd > datum)
                        {
                            // moet worden gesplit door deze datum
                            Mandagen nieuweMandag = CreateMandag(m, datum,m.Eindtijd,false);

                            // huidige krijgt andere einddaatum
                            m.Eindtijd = datum;

                            mandagResult.Add(nieuweMandag);
                        }
                    }
                }
                adjustedMandagen.AddRange(mandagResult);
            }
            return adjustedMandagen;
        }


        public void ProjectToevoegen(Project objNewProject, int vakmanId, int projectleiderId)
        {
            dbRepository dbrep = new dbRepository();

            dtBegintijd = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
            DateTime Weekstart = new DateTime();
            Weekstart = Tools.CalculateWeekstart(dtBegintijd);

            Mandagen mandag = new Mandagen();
            mandag.VakmanId = vakmanId;
            mandag.ProjectId = objNewProject.ProjectId;
            mandag.Begintijd = Weekstart;
            mandag.Eindtijd = Weekstart;
            mandag.Uren = 0;
            mandag.Minuten = 0;
            mandag.Mutatiedatum = DateTime.Now;
            mandag.MutatieDoorProjectleiderId = projectleiderId;
            mandag.ProjectleiderId = objNewProject.ProjectleiderId;
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


        }

        public void VakmanDefaultInvullen(int projectId, int vakmanId, int projectleiderId, DateTime begintijd, DateTime eindtijd)
        {

            dbRepository dbrep = new dbRepository();
            dbrep.DeleteMandagenVoorVakmanId(vakmanId, projectId, begintijd, projectleiderId, DateTime.Now);


            if (eindtijd > begintijd)
            {

                Mandagen mandag = new Mandagen();
                mandag.VakmanId = vakmanId;
                mandag.ProjectId = projectId;
                mandag.Begintijd = begintijd;
                mandag.Eindtijd = eindtijd;

                TimeSpan tsTijdsduur = TimeSpan.FromTicks(mandag.Eindtijd.Ticks - mandag.Begintijd.Ticks);
                mandag.Uren = tsTijdsduur.Hours;
                mandag.Minuten = tsTijdsduur.Minutes;

                //mandag.Uren = 0;
                //mandag.Minuten = 0;
                mandag.Mutatiedatum = DateTime.Now;
                mandag.MutatieDoorProjectleiderId = projectleiderId;
                mandag.ProjectleiderId = projectleiderId; // dummy
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
            }

            //// refresh even alle andere programma's

            //MenuControl owner = Tools.FindVisualParent<MenuControl>(this);

            ////#if DEBUG
            ////#else

            //DateTime dt1 = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
            //DateTime dt2 = dt1.AddDays(1);
            //List<int> vakmanIds = new List<int>();

            //foreach (vwVakman vm in listVakmannen)
            //{
            //    vakmanIds.Add(vm.VakmanId);
            //    listViewVakmannen.Add(vm.VakmanId);
            //}



        }


        public void SendChannelMessage()
        {
            // refresh even alle andere programma's

            MenuControl owner = Tools.FindVisualParent<MenuControl>(this);

            if (Global.useChatFunction)
            {
                DateTime dt1 = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
                DateTime dt2 = dt1.AddDays(1);

                List<int> query = (from m in listVakmannen
                         select m.VakmanId).ToList<int>();

                owner.PageChannelMessage("projectdagview", Tools.CreateChannelMessage(query, dt1, dt2));
            }

        }


        public void VakmanUrenVerwijderen(int projectId, int vakmanId, int projectleiderId, DateTime begintijd)
        {
            //dbRepository dbrep = new dbRepository();
            //foreach (Mandagen mandag in dbrep.GetMandagenByProject(projectId))
            //{
            //    if (mandag.VakmanId == vakmanId && mandag.ProjectleiderId == projectleiderId && mandag.Eindtijd > mandag.Begintijd)
            //    {
            //        dbrep.Deletemandag(mandag);
            //    }
            //}

            dbRepository dbrep = new dbRepository();
            dbrep.DeleteMandagenVoorVakmanId(vakmanId, projectId, begintijd, projectleiderId, DateTime.Now);

            //// refresh even alle andere programma's

            //MenuControl owner = Tools.FindVisualParent<MenuControl>(this);

            ////#if DEBUG
            ////#else

            //DateTime dt1 = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
            //DateTime dt2 = dt1.AddDays(1);
            //List<int> vakmanIds = new List<int>();

            //foreach (vwVakman vm in listVakmannen)
            //{
            //    vakmanIds.Add(vm.VakmanId);
            //    listViewVakmannen.Add(vm.VakmanId);
            //}



        }

        private void btnProjectToevoegen_Click(object sender, RoutedEventArgs e)
        {
            //Project objNewProject = (Project)cbProjecten.SelectedItem;
            //ProjectToevoegen(objNewProject, intVakmanId, intProjectleider);


            //LoadVakmanDagView(true);
            //LoadWeekInfo();

        }

        private void btnNaarVakmannen_Click(object sender, RoutedEventArgs e)
        {


            // create the page and load all values
            Vakmannen.Lijst.VakmannenOverview vo = new Vakmannen.Lijst.VakmannenOverview(true);
            // TODO: {VIEW} 22 april, CHECKEN gaat dit nog goed?

            foreach (vwVakman vm in listVakmannen)
            {
                listViewVakmannen.Add(vm.VakmanId);
            }

            vo.listVakmannen = listViewVakmannen;

            vo.blnRememberSelectedDay = true;
            vo.LoadView();

            // load the page into the contentcontrol
            MenuControl owner = Tools.FindVisualParent<MenuControl>(this);

            if (Rechten.IsProjectleider)
            {
                vo.PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                vo.PageGereedButtonVisibility = System.Windows.Visibility.Hidden;
            }



            vo.PageOKButtonText = "Nieuwe Vakman";

            vo.PageTitle = "Vakman Kiezen";
            vo.PageSubtitle = "Voor " + objProject.Naam;

            vo.OkClick -= vo.Okay;
            vo.OkClick += vo.Okay;

            vo.CustomActieClick -= vo.SelectVakman;
            vo.CustomActieClick += vo.ToevoegenAanProject;

            // kan op 2 manieren, kan ook door in doubleclick functie de customactie aan te roepen
            vo.dgVakmannen.MouseDoubleClick -= vo.dgVakmannen_MouseDoubleClick;
            vo.dgVakmannen.MouseDoubleClick += vo.dgVakmannen_MouseDoubleClickForProject;

            owner.PageGoToPage(vo);

        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (Window.GetWindow(this) != null)
            {
                svScrollviewer.Width = Window.GetWindow(this).ActualWidth - 284;
                svScrollviewerWrapper.Height = Window.GetWindow(this).ActualHeight - 260;
                svScrollviewer.MinHeight = svScrollviewerWrapper.Height;

                //spProjectInfo2.Width = spProjectInfo.ActualWidth;
                spProjectInfo.Width = ((Window.GetWindow(this).ActualWidth - 244) / 2);
                spProjectInfo2.Width = ((Window.GetWindow(this).ActualWidth - 244) / 2);
                lblDatumInfo.Width = spProjectInfo2.Width - 40;
                //spDatepicker.SetValue(Canvas.LeftProperty, ((Window.GetWindow(this).ActualWidth - 540) / 2) - 7);
                lblProject.Content = lblProject.ToolTip.ToString().ToString(spProjectInfo.Width);

                //datePicker1.Margin = new Thickness(((int)Window.GetWindow(this).ActualWidth / 2) - 100, 5,0,0);

                if (ApplicationState.GetValue<int>(ApplicationVariables.intVakmanViewModus) == 0)
                {
                    spContainer.Width = 1920;
                    canvasWrapper.Width = 1920;
                    spVakmanDag.Width = 1920;
                    spVakmanDagLabels.Width = 1920;
                    spVakmanDagLabelsBottom.Width = 1920;

                }
                else if (ApplicationState.GetValue<int>(ApplicationVariables.intVakmanViewModus) == 1)
                {
                    CalculateWeekviewWidths();
                }


            }

            //MessageBox.Show(spProjectInfo.ActualWidth.ToString() + ";" + spDatepicker.ActualWidth.ToString());

        }

        private void btnDefaultInvullen_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            foreach(UIElement control in spOverzicht.Children)
            {
                if (control.GetType() == typeof(ProjectDagProjectHeader))
                {
                    ProjectDagProjectHeader projectDagProjectHeader = (ProjectDagProjectHeader)control;
                    if (projectDagProjectHeader.cbVakmanSelected.IsChecked == true && projectDagProjectHeader.IsEnabled)
                    {
                        vwVakman vakman = projectDagProjectHeader.Vakman;
                        DateTime dtStarttijd = dtBegintijd.AddHours((int)vakman.DefaultBeginuur).AddMinutes((int)vakman.DefaultBeginminuut);


                        switch (dtBegintijd.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                VakmanDefaultInvullen(intProjectId, vakman.VakmanId, intProjectleider, dtStarttijd, dtStarttijd.AddHours((double)vakman.Ma));
                                break;
                            case DayOfWeek.Tuesday:
                                VakmanDefaultInvullen(intProjectId, vakman.VakmanId, intProjectleider, dtStarttijd, dtStarttijd.AddHours((double)vakman.Di));
                                break;
                            case DayOfWeek.Wednesday:
                                VakmanDefaultInvullen(intProjectId, vakman.VakmanId, intProjectleider, dtStarttijd, dtStarttijd.AddHours((double)vakman.Wo));
                                break;
                            case DayOfWeek.Thursday:
                                VakmanDefaultInvullen(intProjectId, vakman.VakmanId, intProjectleider, dtStarttijd, dtStarttijd.AddHours((double)vakman.Do));
                                break;
                            case DayOfWeek.Friday:
                                VakmanDefaultInvullen(intProjectId, vakman.VakmanId, intProjectleider, dtStarttijd, dtStarttijd.AddHours((double)vakman.Vr));
                                break;
                            case DayOfWeek.Saturday:
                                VakmanDefaultInvullen(intProjectId, vakman.VakmanId, intProjectleider, dtStarttijd, dtStarttijd.AddHours((double)vakman.Za));
                                break;
                            case DayOfWeek.Sunday:
                                VakmanDefaultInvullen(intProjectId, vakman.VakmanId, intProjectleider, dtStarttijd, dtStarttijd.AddHours((double)vakman.Zo));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            if (Global.useChatFunction)
            {
                SendChannelMessage();
            }

            LoadVakmanDagView(true, -1 ,true);

            //LoadVakmanDagView(false, intProjectId, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay));

            Mouse.OverrideCursor = null;

        }

        private void btnUrenVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            foreach (UIElement control in spOverzicht.Children)
            {
                if (control.GetType() == typeof(ProjectDagProjectHeader))
                {
                    ProjectDagProjectHeader projectDagProjectHeader = (ProjectDagProjectHeader)control;
                    if (projectDagProjectHeader.cbVakmanSelected.IsChecked == true && projectDagProjectHeader.IsEnabledForDeleting)
                    {
                        vwVakman vakman = projectDagProjectHeader.Vakman;
                        VakmanUrenVerwijderen(intProjectId, vakman.VakmanId, intProjectleider, dtBegintijd);
                    }
                }
            }

            if (Global.useChatFunction)
            {
                SendChannelMessage();
                //owner.PageChannelMessage("projectdagview", Tools.CreateChannelMessage(vakmanIds, dt1, dt2));
            }

            LoadVakmanDagView(true, -1, true);

            //LoadVakmanDagView(false, intProjectId, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay));

            Mouse.OverrideCursor = null;

        }

        private void btnMa_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = Tools.CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intProjectId, dtMaandag, true);
        }

        private void btnDi_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = Tools.CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intProjectId, dtMaandag.AddDays(1), true);
        }

        private void btnWo_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = Tools.CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intProjectId, dtMaandag.AddDays(2), true);
        }

        private void btnDo_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = Tools.CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intProjectId, dtMaandag.AddDays(3), true);
        }

        private void btnVr_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = Tools.CalculateWeekstart(dtBegintijd);
            //SetColors(dtMaandag.AddDays(4));
            LoadVakmanDagView(false, intProjectId, dtMaandag.AddDays(4), true);
        }

        private void btnZa_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = Tools.CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intProjectId, dtMaandag.AddDays(5), true);
        }

        private void btnZo_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = Tools.CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intProjectId, dtMaandag.AddDays(6), true);
        }

        private void btnVorigeWeek_Click(object sender, RoutedEventArgs e)
        {
            GoTerug(7);
        }

        private void btnVolgendeWeek_Click(object sender, RoutedEventArgs e)
        {
            GoVerder(7);
        }

        private void btnResetData_Click(object sender, RoutedEventArgs e)
        {
            dbRepository dbrep = new dbRepository();
            List<Mandagen> mandagen = new List<Mandagen>();

            foreach (Mandagen mandag in dbrep.datacontext.Mandagens.ToList())
            {
                if (mandag.Eindtijd > mandag.Begintijd)
                {
                    Mandagen mandagNew = new Mandagen();
                    mandagNew.Begintijd = mandag.Begintijd;
                    mandagNew.Bevestigd = mandag.Bevestigd;
                    mandagNew.Bevestigingsdatum = mandag.Bevestigingsdatum;
                    mandagNew.Definitief = mandag.Definitief;
                    mandagNew.Eindtijd = mandag.Eindtijd;
                    mandagNew.Geannulleerd = mandag.Geannulleerd;
                    mandagNew.Gewijzigd = mandag.Gewijzigd;
                    mandagNew.IsChauffeurHeen = mandag.IsChauffeurHeen;
                    mandagNew.IsChauffeurTerug = mandag.IsChauffeurTerug;
                    mandagNew.KentekenHeen = mandag.KentekenHeen;
                    mandagNew.KentekenTerug = mandag.KentekenTerug;
                    mandagNew.Minuten = mandag.Minuten;
                    mandagNew.Mutatiedatum = mandag.Mutatiedatum;
                    mandagNew.MutatieDoorProjectleiderId = mandag.MutatieDoorProjectleiderId;
                    mandagNew.ProjectId = mandag.ProjectId;
                    mandagNew.ProjectleiderId = mandag.ProjectleiderId;
                    mandagNew.Status = mandag.Status;
                    mandagNew.Uren = mandag.Uren;
                    mandagNew.VakmanId = mandag.VakmanId;


                    TimeSpan ts = new TimeSpan(mandag.Begintijd.Hour, mandag.Begintijd.Minute, mandag.Begintijd.Second);
                    mandagNew.Begintijd = mandag.Begintijd.AddTicks(-ts.Ticks);
                    mandagNew.Eindtijd = mandag.Eindtijd.AddTicks(-ts.Ticks);

                    mandagen.Add(mandagNew);
                    dbrep.datacontext.Mandagens.DeleteOnSubmit(mandag);

                    //dbrep.datacontext.Mandagens.InsertOnSubmit(mandagNew);
                }
            }
            dbrep.datacontext.SubmitChanges();


            foreach (Mandagen mandag in mandagen)
            {
                try
                {
                    dbrep.datacontext.Mandagens.InsertOnSubmit(mandag);
                    dbrep.datacontext.SubmitChanges();
                }
                catch(Exception ex)
                {
                    string test = ex.Message;
                }
            }


        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ConfirmStopSlepen();
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            ConfirmStopSlepen();
        }

        private void ConfirmStopSlepen()
        {
            if ((Global.selectionMode)ApplicationState.GetValue<int>("selectionMode") != Global.selectionMode.Unknown)
            {
                ApplicationState.SetValue("selectionDirection", (int)Global.selectionDirection.Unknown);
                ApplicationState.SetValue("selectionMode", (int)Global.selectionMode.Unknown);
                ApplicationState.SetValue("LastMouseLeavePosition", 0);
                ApplicationState.SetValue("MouseFirstEnterPosition", 0);
                ApplicationState.SetValue("MouseFirstEnterPositionHold", 0);

                ConfirmUpdate(ApplicationState.GetValue<int>(ApplicationVariables.intVakmanIdSelecting));

                // voor alle child controls mousebuttonPressed uitzetten
                //blnIsMouseButtonPressed = false;
            }

        }

        private void btnDagInfo_Click(object sender, RoutedEventArgs e)
        {

            ApplicationState.SetValue(ApplicationVariables.intVakmanViewModus, 0);

            btnDagInfo.Visibility = System.Windows.Visibility.Collapsed;
            lblDagInfo.Visibility = System.Windows.Visibility.Visible;

            btnWeekInfo.Visibility = System.Windows.Visibility.Visible;
            lblWeekInfo.Visibility = System.Windows.Visibility.Collapsed;

            LoadVakmanDagView(false);

        }

        private void btnWeekInfo_Click_1(object sender, RoutedEventArgs e)
        {

            ApplicationState.SetValue(ApplicationVariables.intVakmanViewModus, 1);

            btnWeekInfo.Visibility = System.Windows.Visibility.Collapsed;
            lblWeekInfo.Visibility = System.Windows.Visibility.Visible;

            btnDagInfo.Visibility = System.Windows.Visibility.Visible;
            lblDagInfo.Visibility = System.Windows.Visibility.Collapsed;

            LoadProjectWeekView(false);

        }

        private void svScrollviewer_PreviewMouseWheel_1(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer && !e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }

    }

}

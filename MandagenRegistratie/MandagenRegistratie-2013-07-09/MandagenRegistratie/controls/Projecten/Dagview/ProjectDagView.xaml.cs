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


        public Projectleider objProjectleider;

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
        }


                /// <summary>
        /// 
        /// </summary>
        public void LoadVakmanDagView(bool blnDefaultScroll, int projectId, DateTime selectedDay)
        {

            ApplicationState.SetValue(ApplicationVariables.intProjectId, projectId);
            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, selectedDay);

            dtBegintijd = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);

            if (datePicker1.SelectedDate == dtBegintijd)
            {
                LoadVakmanDagView(blnDefaultScroll);
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
                    break;
                case DayOfWeek.Tuesday:
                    btnDi.FontSize = 12;
                    btnDi.FontWeight = FontWeights.ExtraBold;
                    txtDi.TextDecorations = TextDecorations.Underline;
                    break;
                case DayOfWeek.Wednesday:
                    btnWo.FontSize = 12;
                    btnWo.FontWeight = FontWeights.ExtraBold;
                    txtWo.TextDecorations = TextDecorations.Underline;
                    break;
                case DayOfWeek.Thursday:
                    btnDo.FontSize = 12;
                    btnDo.FontWeight = FontWeights.ExtraBold;
                    txtDo.TextDecorations = TextDecorations.Underline;
                    break;
                case DayOfWeek.Friday:
                    btnVr.FontSize = 12;
                    btnVr.FontWeight = FontWeights.ExtraBold;
                    txtVr.TextDecorations = TextDecorations.Underline;
                    break;
                case DayOfWeek.Saturday:
                    btnZa.FontSize = 12;
                    btnZa.FontWeight = FontWeights.ExtraBold;
                    txtZa.TextDecorations = TextDecorations.Underline;
                    break;
                case DayOfWeek.Sunday:
                    btnZo.FontSize = 12;
                    btnZo.FontWeight = FontWeights.ExtraBold;
                    txtZo.TextDecorations = TextDecorations.Underline;
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
                    foreach (UIElement innerControl in ((WrapPanel)control).Children)
                    {
                        if (innerControl.GetType() == typeof(Button))
                        {
                            Button btn = (Button)innerControl;
                            if ((btn).Name == "btn2")
                            {
                                // default invullen knop zichtbaar maken
                                btn.Visibility = System.Windows.Visibility.Visible;
                            }
                            else if ((btn).Name == "btn3")
                            {
                                // default verwijderen knop onzichtbaar maken
                                btn.Visibility = System.Windows.Visibility.Collapsed;
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
                    
                    if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIds) == null)
                    {
                        ApplicationState.SetValue(ApplicationVariables.listSelectedVakmanIds, new List<int>());
                    }

                    if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIds).Contains(projectDagProjectHeader.Vakman.VakmanId))
                    {
                        projectDagProjectHeader.cbVakmanSelected.IsChecked = true;
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
                    foreach (UIElement innerControl in ((WrapPanel)control).Children)
                    {
                        if (innerControl.GetType() == typeof(Button))
                        {
                            Button btn = (Button)innerControl;
                            if ((btn).Name == "btn3")
                            {
                                // default verwijderen knop zichtbaar maken
                                btn.Visibility = System.Windows.Visibility.Visible;
                            }
                            else if ((btn).Name == "btn2")
                            {
                                // default invullen knop onzichtbaar maken
                                btn.Visibility = System.Windows.Visibility.Collapsed;
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

                    if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIds) == null)
                    {
                        ApplicationState.SetValue(ApplicationVariables.listSelectedVakmanIds, new List<int>());
                    }
                    else if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIds).Contains(projectDagProjectHeader.Vakman.VakmanId))
                    {
                        projectDagProjectHeader.cbVakmanSelected.IsChecked = true;
                    }

                    projectDagProjectHeader.SetBullit();

                }
            }

            

        }


        /// <summary>
        /// 
        /// </summary>
        public void LoadVakmanDagView(bool blnDefaultScroll)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            //dtMaandag = CalculateWeekstart(dtBegintijd);
            SetDagButtons(dtBegintijd);

            // set de laatste ververstijd van het scherm
            ApplicationState.SetValue(ApplicationVariables.dtLastRefreshDagView, DateTime.Now);

            DateTime starttijd = DateTime.Now;

            //lblDagInfo.Content = 

            // intialize
            spVakmanDag.Children.Clear();
            dbRepository dbrep = new dbRepository();
            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
            intProjectId = ApplicationState.GetValue<int>(ApplicationVariables.intProjectId);
            intProjectleider = ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider);
            objProject = dbrep.GetProject(intProjectId);
            project objProjectOriginal = dbrepOriginal.GetProject(objProject.ProjectIdOrigineel);

            //objVakman = dbrep.GetVakman(intVakmanId);
            objProjectleider = dbrep.GetProjectleider(intProjectleider);
            persoon objPersoon = dbrepOriginal.GetContact(dbrep.GetProjectleider(objProject.ProjectleiderId).ContactIdOrigineel);

            lblProject.Content = "Project " + objProjectOriginal.naam_project;
            if (lblProject.Content.ToString().Length > 50)
            {
                lblProject.ToolTip = lblProject.Content.ToString();
                lblProject.Content = lblProject.Content.ToString().Substring(0, 48) + "...";
            }

            lblProjectleider.Content = "Projectleider " + objPersoon.voornaam + " " + objPersoon.tussenvoegsel + " " + objPersoon.achternaam;

            listVakmannen = dbrep.GetVakmannenByProjectId(intProjectId, Tools.CalculateWeekstart(dtBegintijd)).OrderBy(v => v.voornaam).ToList();
            
            //listMandagen = dbrep.GetMandagenByProject(intProjectId, dtBegintijd);
            listMandagen = dbrep.GetMandagen(dtBegintijd);

            bool blnIsOpVreemdProjectIngepland = false;
            bool blnIsOpVreemdProjectAangevraagd = false;
            bool blnIsOpProjectAangevraagd = false;
            bool blnIsOpProjectIngepland;
            bool blnIsNotOwner = false;

            //cbProjecten.ItemsSource = dbrep.GetProjects();
            //cbProjecten.DisplayMemberPath = "Naam";
            //cbProjecten.SelectedValuePath = "ProjectId";

            // load overzicht
            spOverzicht.Children.Clear();

            spVakmanDagLabels.Children.Clear();
            spVakmanDagLabelsBottom.Children.Clear();


            //// canvasWrapper.Children.Clear()
            //// workaround omdat ik anders het WrapPanel ook clear()
            //List<UIElement> controlsToRemove = new List<UIElement>();

            //foreach (UIElement control in canvasWrapper.Children)
            //{
            //    if (control.GetType() != typeof(WrapPanel))
            //    {
            //        controlsToRemove.Add(control);
            //    }
            //}

            //foreach (UIElement control in controlsToRemove)
            //{
            //    canvasWrapper.Children.Remove(control);
            //}

            // loop door de 24 uren
            foreach (ProjectDagViewProjectUurLabel vpu in GetProjectUrenLabels())
            {

                // oneven uren 'arceren'
                if (vpu.Uur % 2 == 1)
                {
                    vpu.lblUur.Background = new SolidColorBrush(Colors.Chocolate); // LightSteelBlue
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
                    vpuEmpty.lblUur.Background = new SolidColorBrush(Colors.Chocolate); // LightSteelBlue
                }
                spVakmanDagLabels.Children.Add(vpuEmpty); // TODO: check

            }

                // loop door de 24 uren
            foreach (ProjectDagViewProjectUurLabel vpu in GetProjectUrenLabels())
            {

                // oneven uren 'arceren'
                if (vpu.Uur % 2 == 1)
                {
                    vpu.lblUur.Background = new SolidColorBrush(Colors.LightSteelBlue);
                }

                if (listVakmannen.Count <= 5)
                {
                    vpu.lblUur.Content = " ";
                }

                // uiteindelijk toevoegen
                // altijd 24x, voor elk uur 1
                spVakmanDagLabelsBottom.Children.Insert(0, vpu);
            }


            ProjectDagViewProjectUurLabel vpu2 = new ProjectDagViewProjectUurLabel();

            WrapPanel wpSelectPanel = new WrapPanel();
            wpSelectPanel.Orientation = Orientation.Horizontal;

            ProjectDagViewProjectUurLabel vpuEmpty2 = new ProjectDagViewProjectUurLabel();
            vpuEmpty2.Height = 10;
            wpSelectPanel.Children.Add(vpuEmpty2);

            RadioButton rbDelete = new RadioButton();
            rbDelete.Margin = new Thickness(5,0,5,0);
            rbDelete.Checked += new RoutedEventHandler(SetModeDelete);

            RadioButton rbAdd = new RadioButton();
            rbAdd.Margin = new Thickness(5,0,5,0);
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

            vpuEmpty2.Width = 200;


            vpu2.lblUur.Content = "Vakmannen";
            vpu2.lblUur.FontSize = 14;
            //vpu2.lblUur.FontWeight = FontWeights.Bold;
            vpu2.lblUur.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            vpu2.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;

            spOverzicht.Children.Add(vpu2); // TODO: check
            spOverzicht.Children.Add(wpSelectPanel); // TODO: check

            int count = 0;

            // als ik (degene die dit scherm bekijkt) de projectleider ben
            bool blnIsOwner = objProject.ProjectleiderId == objProjectleider.ProjectleiderId;

            // voor alle vakmannen van dit project
            foreach (vwVakman vakman in listVakmannen)
            {
                count++;

                // HEADERS TOEVOEGEN
                ProjectDagProjectHeader projectDagProjectHeader = new ProjectDagProjectHeader();
                if (vakman.VakmanId == ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId))
                {
                    projectDagProjectHeader.btnHeader.FontWeight = FontWeights.ExtraBold;
                    projectDagProjectHeader.btnHeader.FontSize = 12;

                    //projectDagProjectHeader.btnGoToProject.Content = "<";
                }

                

                bool isEnabled = true;
                //bool isChecked = true;

                projectDagProjectHeader.Vakman = vakman;

                
                // 24 UREN TOEVOEGEN


                bool blnCanCancel = false;


                int heightOffset = (25 * count) - 9;
                int widthOffset = (CalculateDefaultUren(vakman, dtBegintijd) * 80) -4;

                // TEST with vierkantje
                Border rect = new Border();
                rect.Width = 8;
                rect.Height = 8;
                rect.Background = new SolidColorBrush(Colors.Black);
                rect.BorderBrush = new SolidColorBrush(Colors.White);
                rect.BorderThickness = new Thickness(1);
                rect.Margin = new Thickness(widthOffset, heightOffset, 0, 0);
                rect.Cursor = Cursors.Cross;


                // loop door de 24 uren
                foreach (ProjectDagViewProjectUur vpu in GetProjectUren(listMandagen, vakman, dtBegintijd, blnIsOwner))
                {

                    vpu.project = objProject;
                    vpu.vakman = vakman;

                    // oneven uren 'arceren'
                    if (vpu.Uur % 2 == 1)
                    {
                        vpu.spUur.Background = new SolidColorBrush(Colors.LightSteelBlue);
                    }

                    // bereken de kleuren
                    vpu.SetColors();

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

                    //if (vpu.IsAangevraagdQ1 || vpu.IsAangevraagdQ2 || vpu.IsAangevraagdQ3 || vpu.IsAangevraagdQ4
                    //    || vpu.IsIngeplandQ1 || vpu.IsIngeplandQ2 || vpu.IsIngeplandQ3 || vpu.IsIngeplandQ4
                    //    || !vpu.IsEnabledQ1 || !vpu.IsEnabledQ2 || !vpu.IsEnabledQ3 || !vpu.IsEnabledQ4)
                    //{
                    //    isChecked = false;
                    //}


                    // uiteindelijk toevoegen
                    // altijd 24x, voor elk uur 1
                    spVakmanDag.Children.Add(vpu);
                }

                // TEST
                //canvasWrapper.Children.Add(rect);


                // hierna toevoegen aan overzichtskolom
                projectDagProjectHeader.cbVakmanSelected.IsEnabled = isEnabled; // isEnabled;

                bool isChecked = false;

                if(ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIds) != null && ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIds).Contains(projectDagProjectHeader.Vakman.VakmanId))
                {
                    isChecked = true;
                }

                projectDagProjectHeader.cbVakmanSelected.IsChecked = isChecked;

                projectDagProjectHeader.IsOwner = blnIsOwner;
                projectDagProjectHeader.IsEnabled = isEnabled;

                projectDagProjectHeader.SetBullit();


                // zichtbaar als eigenaar
                if(objProject.ProjectleiderId == ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider))
                {
                    projectDagProjectHeader.cbVakmanSelected.Visibility = System.Windows.Visibility.Visible;
                }

                spOverzicht.Children.Add(projectDagProjectHeader);

                // klaar met instellingen, vakmandagproject toevoegen
                // spVakmanDag.Children.Add(GetProjectUren();
            }

            if (blnDefaultScroll)
            {
                //svScrollviewer.ScrollToHorizontalOffset(480);
            }

            WrapPanel sp = new WrapPanel();
            sp.Width = 250;

            sp.Orientation = Orientation.Horizontal;

            Button btn = new Button();
                    //<Button Content="Vakman toevoegen" Height="23" Name="btnNaarProjecten" Width="116" Click="btnNaarVakmannen_Click" />
            btn.Content = "+";
            btn.Height = 23;
            btn.Width = 40;
            btn.FontSize = 14;
            btn.Click += btnNaarVakmannen_Click;
            btn.Margin = new Thickness(0, 0, 40, 0);
            
            btn.Visibility = System.Windows.Visibility.Hidden;

            if (blnIsOwner)
            {
                btn.Visibility = System.Windows.Visibility.Visible;
            }

            sp.Children.Add(btn);

            Button btn2 = new Button();
            // <Button Content="Vakman toevoegen" Height="23" Name="btnNaarProjecten" Width="116" Click="btnNaarVakmannen_Click" />
            btn2.Content = "-->";
            btn2.Name = "btn2";

            // btn2.Visibility = System.Windows.Visibility.Visible;
            btn2.Height = 23;
            btn2.Width = 40;
            btn2.Click += btnDefaultInvullen_Click;
            btn2.Margin = new Thickness(118, 0, 0, 0);

            btn2.Visibility = System.Windows.Visibility.Collapsed;

            if (blnIsOwner)
            {
                btn2.Visibility = System.Windows.Visibility.Visible;
            }

            sp.Children.Add(btn2);

            Button btn3 = new Button();
            btn3.Content = "<--";
            btn3.Name = "btn3";
            btn3.Height = 23;
            btn3.Width = 40;
            btn3.Click += btnUrenVerwijderen_Click;
            btn3.Margin = new Thickness(118, 0, 0, 0);
            //sp.Children.Add(btn3);

            btn3.Visibility = System.Windows.Visibility.Collapsed;

            if (blnIsOwner)
            {
                btn3.Visibility = System.Windows.Visibility.Visible;
            }

            sp.Children.Add(btn3);


            spOverzicht.Children.Add(sp);


            TimeSpan ts = ((TimeSpan)(DateTime.Now - starttijd));
            //lblVakmanBsn.Content = ts.Seconds.ToString() + "." + ts.Milliseconds.ToString();

            Mouse.OverrideCursor = null;


        }

        public int CalculateDefaultUren(vwVakman vakman, DateTime tijd)
        {
            switch (tijd.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return (int)vakman.Ma;
                    break;
                case DayOfWeek.Tuesday:
                    return (int)vakman.Di;
                    break;
                case DayOfWeek.Wednesday:
                    return (int)vakman.Wo;
                    break;
                case DayOfWeek.Thursday:
                    return (int)vakman.Do;
                    break;
                case DayOfWeek.Friday:
                    return (int)vakman.Vr;
                    break;
                case DayOfWeek.Saturday:
                    return (int)vakman.Za;
                    break;
                case DayOfWeek.Sunday:
                    return (int)vakman.Zo;
                    break;
                default:
                    return 0;
                    break;
            }
        }


        public Mandagen GetMandagOngeveer(List<Mandagen> mandagen, int vakmanId, DateTime datum, int vakmanStatus, bool status)
        {
            return mandagen.Where(m => m.ProjectId == intProjectId && m.VakmanId == vakmanId && m.Begintijd <= datum && m.Eindtijd >= datum && m.VakmanstatusId == vakmanStatus && m.Status == status).OrderByDescending(m => m.Begintijd).FirstOrDefault();
        }


        public List<ProjectDagViewProjectUur> GetProjectUren(List<Mandagen> mandagen, vwVakman vakman, DateTime dtDag, bool blnIsOwner)
        {
            List<ProjectDagViewProjectUur> resultSet = new List<ProjectDagViewProjectUur>();

            // alle mandagen van deze vakman op deze dag
            List<Mandagen> alleMandagen = mandagen.Where(m => m.ProjectId > 0 && m.VakmanId == vakman.VakmanId && m.VakmanstatusId == 1 && !m.Geannulleerd).ToList();
            List<Mandagen> mandagenCancelled = mandagen.Where(m => m.VakmanId == vakman.VakmanId && m.VakmanstatusId == 1 && m.Geannulleerd).ToList();
            List<Mandagen> alleMandagenInclCancelled = mandagen.Where(m => m.VakmanId == vakman.VakmanId && m.VakmanstatusId == 1).ToList();

            // alle mandagen van deze vakman, voor dit project
            mandagen = mandagen.Where(m => m.VakmanId == vakman.VakmanId && m.VakmanstatusId == 1 && !m.Geannulleerd && m.ProjectId == intProjectId).ToList();

            for (int i = 0; i < 24; i++)
            {
                ProjectDagViewProjectUur vdvpu = new ProjectDagViewProjectUur();
                //vdvpu.lblUur.Content = i.ToString() + "u";

                // set de vakman in kwestie
                vdvpu.vakman = vakman;
                //vdvpu.project

                // set ownership!
                //vdvpu.IsOwner = objProject.ProjectleiderId == objProjectleider.ProjectleiderId;
                vdvpu.IsOwner = blnIsOwner;

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

                blnQ1Aangevraagd = mandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status);
                blnQ2Aangevraagd = mandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status);
                blnQ3Aangevraagd = mandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status);
                blnQ4Aangevraagd = mandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status);

                blnQ1AangevraagdAndCancelled = mandagenCancelled.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status && m.ProjectId == objProject.ProjectId);
                blnQ2AangevraagdAndCancelled = mandagenCancelled.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status && m.ProjectId == objProject.ProjectId);
                blnQ3AangevraagdAndCancelled = mandagenCancelled.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status && m.ProjectId == objProject.ProjectId);
                blnQ4AangevraagdAndCancelled = mandagenCancelled.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status && m.ProjectId == objProject.ProjectId);

                blnQ1ErgensAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status);
                blnQ2ErgensAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status);
                blnQ3ErgensAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status);
                blnQ4ErgensAangevraagd = alleMandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status);

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
                vdvpu.IsEnabledQ1 = vdvpu.IsOwner && (!blnQ1ErgensIngepland || blnQ1Ingepland ) && !blnQ1ErgensAangevraagd;
                vdvpu.IsEnabledQ2 = vdvpu.IsOwner && (!blnQ2ErgensIngepland || blnQ2Ingepland) && !blnQ2ErgensAangevraagd;
                vdvpu.IsEnabledQ3 = vdvpu.IsOwner && (!blnQ3ErgensIngepland || blnQ3Ingepland) && !blnQ3ErgensAangevraagd;
                vdvpu.IsEnabledQ4 = vdvpu.IsOwner && (!blnQ4ErgensIngepland || blnQ4Ingepland) && !blnQ4ErgensAangevraagd;


                vdvpu.CanApprove =
                    (
                        (blnQ1Aangevraagd && blnQ1ErgensDoormijIngepland)
                        || (blnQ2Aangevraagd && blnQ2ErgensDoormijIngepland)
                        || (blnQ3Aangevraagd && blnQ3ErgensDoormijIngepland)
                        || (blnQ4Aangevraagd && blnQ4ErgensDoormijIngepland)
                    );

                vdvpu.CanCancel = vdvpu.IsOwner &&
                    (
                        (blnQ1Aangevraagd)
                        || (blnQ2Aangevraagd)
                        || (blnQ3Aangevraagd)
                        || (blnQ4Aangevraagd)
                    );

                vdvpu.CanConfirmHasSeen = vdvpu.IsOwner &&
                    (
                        (blnQ1AangevraagdAndCancelled)
                        || (blnQ2AangevraagdAndCancelled)
                        || (blnQ3AangevraagdAndCancelled)
                        || (blnQ4AangevraagdAndCancelled)
                    );

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

                // hook event on local function (local:)
                vdvpu.OnVakmanDagViewUpdate += new RoutedEventHandler(vakmandagview_OnVakmanDagViewUpdate);
                vdvpu.OnVakmanDagViewRefreshUpdate += new RoutedEventHandler(vdvpu_OnVakmanDagViewRefreshUpdate);
                vdvpu.OnVakmanDagViewHighlight += new RoutedEventHandler(vdvpu_OnVakmanDagViewHighlight);
                vdvpu.OnVakmanDagViewHighlightOnLeave += new RoutedEventHandler(vdvpu_OnVakmanDagViewHighlightOnLeave);

                resultSet.Add(vdvpu);
            }

            return resultSet;
        }

        public void vdvpu_OnVakmanDagViewRefreshUpdate(object sender, RoutedEventArgs e)
        {
            LoadVakmanDagView(false);
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
            lblWeekInfo.Content = "Week " + GetWeekNumber(dtBegintijd).ToString() + ", " + dtBegintijd.ToString("dd MMM yyyy");
            string dagvandeweek = dtBegintijd.ToString("dddd");
            lblDagInfo.Content = dagvandeweek.Substring(0, 1).ToUpper() + dagvandeweek.Substring(1);
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

            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, dtBegintijd.AddDays(days));
            LoadVakmanDagView(false, intProjectId, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay));
            LoadWeekInfo();

        }


        public void GoTerug(int days)
        {
            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, dtBegintijd.AddDays(-1 * days));

            LoadVakmanDagView(false, intProjectId, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay));
        }


        public static int GetWeekNumber(DateTime dtPassed)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }

        private void clCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {

            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, datePicker1.SelectedDate);
            dtBegintijd = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);

            // how long ago?
            TimeSpan interval = DateTime.Now - ApplicationState.GetValue<DateTime>(ApplicationVariables.dtLastRefreshDagView);
            //if (interval.TotalMilliseconds > 500)
            //{
                LoadVakmanDagView(true);
                LoadWeekInfo();
            //}

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


            mandag.VakmansoortId = 1;
            mandag.VakmanstatusId = 1;
            mandag.IsChauffeurHeen = false;
            mandag.IsChauffeurTerug = false;
            mandag.KentekenHeen = "";
            mandag.KentekenTerug = "";
            mandag.Bevestigingsdatum = DateTime.Now;


            // projectleider op dit moment, voor later in t archief na een eventuele projectleider mutatie
            mandag.ProjectleiderId = vpu.project.ProjectleiderId;


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


            mandag.VakmansoortId = m.VakmansoortId;
            mandag.VakmanstatusId = m.VakmanstatusId;
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

            btnConfirm_Click(sender, e);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            DateTime starttijd = DateTime.Now;

            int intCountMandagen = 0;
            int i = -1;
            int intCountQuarters = 0;

            ProjectDagViewProjectUur ppu = ((ProjectDagViewProjectUur)sender);
            if (ppu.IsEnabledQ1 || ppu.IsEnabledQ2 || ppu.IsEnabledQ3 || ppu.IsEnabledQ4)
            {
                // set vakmanId to clicked one
                int intVakmanId = ppu.vakman.VakmanId;

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
                            intCountQuarters++;

                            blnIsMandagFound = true;

                            listMandagen.Add(mandag);

                        } // bestaande
                        else if (isCheckedQ1)
                        {
                            intCountQuarters++;
                        }
                        else if (!isCheckedQ1)
                        {
                            if (i >= 0 && blnIsMandagFound)
                            {
                                // velden te berekenen
                                listMandagen[i].Eindtijd = CalculateEindtijd(listMandagen[i].Begintijd, intCountQuarters);
                                //listMandagen[i].UrenGewijzigd = CalculateQuartersToUren(intCountQuarters);
                                //listMandagen[i].MinutenGewijzigd = CalculateMinuten(intCountQuarters, listMandagen[i].UrenGewijzigd);

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
                            intCountQuarters++;

                            blnIsMandagFound = true;

                        }
                        else if (isCheckedQ2)
                        {
                            intCountQuarters++;
                        }
                        else if (!isCheckedQ2)
                        {
                            if (i >= 0 && blnIsMandagFound)
                            {
                                // velden te berekenen
                                listMandagen[i].Eindtijd = CalculateEindtijd(listMandagen[i].Begintijd, intCountQuarters);

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
                            intCountQuarters++;

                            blnIsMandagFound = true;

                        }
                        else if (isCheckedQ3)
                        {
                            intCountQuarters++;
                        }
                        else if (!isCheckedQ3)
                        {
                            if (i >= 0 && blnIsMandagFound)
                            {
                                // velden te berekenen
                                listMandagen[i].Eindtijd = CalculateEindtijd(listMandagen[i].Begintijd, intCountQuarters);

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
                            intCountQuarters++;

                            blnIsMandagFound = true;

                        }
                        else if (isCheckedQ4)
                        {
                            intCountQuarters++;
                        }
                        else if (!isCheckedQ4)
                        {
                            if (i >= 0 && blnIsMandagFound)
                            {
                                // velden te berekenen
                                listMandagen[i].Eindtijd = CalculateEindtijd(listMandagen[i].Begintijd, intCountQuarters);

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

                        // mandag toevoegen
                        dbrep.InsertMandag(mandag);

                        // TODO: DIT TESTEN: heb ik invloed op een bestaande mandag van iemand anders: flag setten
                        if (dbrep.GetMandagen(intVakmanId, dtBegintijd).Any(m => mandag.Eindtijd > m.Begintijd && mandag.Begintijd < m.Eindtijd && m.Project.ProjectleiderId != objProjectleider.ProjectleiderId && m.Status) || dbrep.GetProject(mandag.ProjectId).ProjectleiderId != objProjectleider.ProjectleiderId)
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
                }
            } // end if

            //// reload page:
            LoadVakmanDagView(false);
            //LoadVakmanDagView(false, intProjectId, dtBegintijd);



            TimeSpan ts = ((TimeSpan)(DateTime.Now - starttijd));
            //lblVakmanBsn.Content = ts.Seconds.ToString() + "." + ts.Milliseconds.ToString();


            // refresh even alle andere programma's

            MenuControl owner = Tools.FindVisualParent<MenuControl>(this);

//#if DEBUG
//#else

            DateTime dt1 = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
            DateTime dt2 = dt1.AddDays(1);
            List<int> vakmanIds = new List<int>();

            foreach (vwVakman vm in listVakmannen)
            {
                vakmanIds.Add(vm.VakmanId);
                listViewVakmannen.Add(vm.VakmanId);
            }

            owner.PageChannelMessage("projectdagview", Tools.CreateChannelMessage(vakmanIds, dt1, dt2));

//#endif
            Mouse.OverrideCursor = null;

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
            List<int> vakmanIds = new List<int>();

            foreach(vwVakman vm in listVakmannen)
            {
                vakmanIds.Add(vm.VakmanId);
            }

            if (Tools.RefreshView(vakmanIds, dt1, dt2, message))
            {
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
            mandag.UrenGewijzigd = 0;
            mandag.Minuten = 0;
            mandag.MinutenGewijzigd = 0;
            mandag.Mutatiedatum = DateTime.Now;
            mandag.MutatieDoorProjectleiderId = projectleiderId;
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


        }

        public void VakmanDefaultInvullen(int projectId, int vakmanId, int projectleiderId, DateTime begintijd, DateTime eindtijd)
        {

            dbRepository dbrep = new dbRepository();
            dbrep.DeleteMandagenVoorVakmanId(vakmanId, projectId, begintijd, projectleiderId, DateTime.MinValue);


            if (eindtijd > begintijd)
            {

                Mandagen mandag = new Mandagen();
                mandag.VakmanId = vakmanId;
                mandag.ProjectId = projectId;
                mandag.Begintijd = begintijd;
                mandag.Eindtijd = eindtijd;
                mandag.Uren = 0;
                mandag.UrenGewijzigd = 0;
                mandag.Minuten = 0;
                mandag.MinutenGewijzigd = 0;
                mandag.Mutatiedatum = DateTime.Now;
                mandag.MutatieDoorProjectleiderId = projectleiderId;
                mandag.ProjectleiderId = projectleiderId; // dummy
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
            dbrep.DeleteMandagenVoorVakmanId(vakmanId, projectId, begintijd, projectleiderId, DateTime.MinValue);

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
            Vakmannen.Overzicht.VakmannenOverview vo = new Vakmannen.Overzicht.VakmannenOverview();
            // TODO: {VIEW} 22 april, CHECKEN gaat dit nog goed?

            foreach (vwVakman vm in listVakmannen)
            {
                listViewVakmannen.Add(vm.VakmanId);
            }

            vo.listVakmannen = listViewVakmannen;

            vo.LoadView();

            // load the page into the contentcontrol
            MenuControl owner = Tools.FindVisualParent<MenuControl>(this);
            vo.PageGereedButtonVisibility = System.Windows.Visibility.Visible;
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
                //lblVakmanBsn.Content = Window.GetWindow(this).ActualHeight - 280; //svScrollviewer.Width.ToString();

                svScrollviewer.Width = Window.GetWindow(this).ActualWidth - 284;
                svScrollviewerWrapper.Height = Window.GetWindow(this).ActualHeight - 260;
                svScrollviewer.MinHeight = svScrollviewerWrapper.Height;

                spProjectInfo2.Width = spProjectInfo.ActualWidth;
            }
        }

        private void btnDefaultInvullen_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            foreach(UIElement control in spOverzicht.Children)
            {
                if (control.GetType() == typeof(ProjectDagProjectHeader))
                {
                    ProjectDagProjectHeader projectDagProjectHeader = (ProjectDagProjectHeader)control;
                    if (projectDagProjectHeader.cbVakmanSelected.IsChecked == true)
                    {
                        vwVakman vakman = projectDagProjectHeader.Vakman;
                        DateTime dtStarttijd = dtBegintijd.AddHours((int)vakman.DefaultBeginuur).AddMinutes((int)vakman.DefaultBeginminuut);


                        switch (dtBegintijd.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                VakmanDefaultInvullen(intProjectId, vakman.VakmanId, intProjectleider, dtStarttijd, dtStarttijd.AddHours((int)vakman.Ma));
                                break;
                            case DayOfWeek.Tuesday:
                                VakmanDefaultInvullen(intProjectId, vakman.VakmanId, intProjectleider, dtStarttijd, dtStarttijd.AddHours((int)vakman.Di));
                                break;
                            case DayOfWeek.Wednesday:
                                VakmanDefaultInvullen(intProjectId, vakman.VakmanId, intProjectleider, dtStarttijd, dtStarttijd.AddHours((int)vakman.Wo));
                                break;
                            case DayOfWeek.Thursday:
                                VakmanDefaultInvullen(intProjectId, vakman.VakmanId, intProjectleider, dtStarttijd, dtStarttijd.AddHours((int)vakman.Do));
                                break;
                            case DayOfWeek.Friday:
                                VakmanDefaultInvullen(intProjectId, vakman.VakmanId, intProjectleider, dtStarttijd, dtStarttijd.AddHours((int)vakman.Vr));
                                break;
                            case DayOfWeek.Saturday:
                                VakmanDefaultInvullen(intProjectId, vakman.VakmanId, intProjectleider, dtStarttijd, dtStarttijd.AddHours((int)vakman.Za));
                                break;
                            case DayOfWeek.Sunday:
                                VakmanDefaultInvullen(intProjectId, vakman.VakmanId, intProjectleider, dtStarttijd, dtStarttijd.AddHours((int)vakman.Zo));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            LoadVakmanDagView(true);
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
                    if (projectDagProjectHeader.cbVakmanSelected.IsChecked == true)
                    {
                        vwVakman vakman = projectDagProjectHeader.Vakman;
                        VakmanUrenVerwijderen(intProjectId, vakman.VakmanId, intProjectleider, dtBegintijd);
                    }
                }
            }

            LoadVakmanDagView(true);
            //LoadVakmanDagView(false, intProjectId, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay));

            Mouse.OverrideCursor = null;

        }

        private void btnMa_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = Tools.CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intProjectId, dtMaandag);
        }

        private void btnDi_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = Tools.CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intProjectId, dtMaandag.AddDays(1));
        }

        private void btnWo_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = Tools.CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intProjectId, dtMaandag.AddDays(2));
        }

        private void btnDo_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = Tools.CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intProjectId, dtMaandag.AddDays(3));
        }

        private void btnVr_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = Tools.CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intProjectId, dtMaandag.AddDays(4));
        }

        private void btnZa_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = Tools.CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intProjectId, dtMaandag.AddDays(5));
        }

        private void btnZo_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = Tools.CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intProjectId, dtMaandag.AddDays(6));
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
                    mandagNew.MinutenGewijzigd = mandag.MinutenGewijzigd;
                    mandagNew.Mutatiedatum = mandag.Mutatiedatum;
                    mandagNew.MutatieDoorProjectleiderId = mandag.MutatieDoorProjectleiderId;
                    mandagNew.ProjectId = mandag.ProjectId;
                    mandagNew.ProjectleiderId = mandag.ProjectleiderId;
                    mandagNew.Status = mandag.Status;
                    mandagNew.Uren = mandag.Uren;
                    mandagNew.UrenGewijzigd = mandag.UrenGewijzigd;
                    mandagNew.VakmanId = mandag.VakmanId;
                    mandagNew.VakmansoortId = mandag.VakmansoortId;
                    mandagNew.VakmanstatusId = mandag.VakmanstatusId;


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
    }

}

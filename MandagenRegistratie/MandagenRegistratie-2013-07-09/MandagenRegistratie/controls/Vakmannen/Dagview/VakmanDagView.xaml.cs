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

namespace MandagenRegistratie.controls.Vakmannen.Dagview
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

        public DateTime Weekstart = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,0,0,0);
        public DateTime dtMaandag;

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


        dbOriginalRepository dbrepOriginal = new dbOriginalRepository();


        public Projectleider objProjectleider;

        public List<Project> listProjecten;

        public int ProjectId { get; set; }

        // extra
        private List<Mandagen> listMandagen;
        public DateTime dtBegintijd;

        public delegate void backgroundWorkerDelegate();
        bool canExecute = true;
        private BackgroundWorker worker = new BackgroundWorker();
        private BackgroundWorker workerTerug = new BackgroundWorker();


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

        public void LoadVakmanDagView(bool blnDefaultScroll, int vakmanId, DateTime selectedDay)
        {
            ApplicationState.SetValue(ApplicationVariables.intVakmanId, vakmanId);
            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, selectedDay);


            dtBegintijd = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
            //Weekstart = CalculateWeekstart(dtBegintijd);

            if (datePicker1.SelectedDate == dtBegintijd)
            {
                LoadVakmanDagView(blnDefaultScroll);
            }
            else
            { // will automatically trigger LoadVakmanDagView()
                datePicker1.SelectedDate = dtBegintijd;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void LoadVakmanDagView(bool blnDefaultScroll)
        {

            SetDagButtons(dtBegintijd);

            // set de laatste ververstijd van het scherm
            ApplicationState.SetValue(ApplicationVariables.dtLastRefreshDagView, DateTime.Now);


            // intialize
            spVakmanDag.Children.Clear();
            dbRepository dbrep = new dbRepository();

            intVakmanId = ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId);
            intProjectleider = ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider);

            objVakman = dbrep.GetVakman(intVakmanId);

            persoon objPersoon = dbrepOriginal.GetContact(objVakman.ContactIdOrigineel);
            lblVakman.Content = "Vakman " + objPersoon.voornaam + " " + (string.IsNullOrEmpty(objPersoon.tussenvoegsel) ? "" : objPersoon.tussenvoegsel + " ") + objPersoon.achternaam;

            objProjectleider = dbrep.GetProjectleider(intProjectleider);
            listProjecten = dbrep.GetProjectsByVakmanId(intVakmanId, CalculateWeekstart(dtBegintijd));

            listMandagen = dbrep.GetMandagen(objVakman.VakmanId, dtBegintijd);

            bool blnIsOpVreemdProjectIngepland = false;
            bool blnIsOpVreemdProjectAangevraagd = false;
            bool blnIsOpProjectAangevraagd = false;
            bool blnIsOpProjectIngepland;
            bool blnIsNotOwner = false;

            // load overzicht
            spOverzicht.Children.Clear();

            spVakmanDagLabels.Children.Clear();
            spVakmanDagLabelsBottom.Children.Clear();

            // loop door de 24 uren
            foreach (VakmanDagViewProjectUurLabel vpu in GetProjectUrenLabels())
            {

                // oneven uren 'arceren'
                if (vpu.Uur % 2 == 1)
                {
                    vpu.lblUur.Background = new SolidColorBrush(Colors.LightSteelBlue);
                }

                // uiteindelijk toevoegen
                // altijd 24x, voor elk uur 1
                spVakmanDagLabels.Children.Insert(0, vpu);
            }

            // loop door de 24 uren
            foreach (VakmanDagViewProjectUurLabel vpu in GetProjectUrenLabels())
            {

                // oneven uren 'arceren'
                if (vpu.Uur % 2 == 1)
                {
                    vpu.lblUur.Background = new SolidColorBrush(Colors.LightSteelBlue);
                }

                if (listProjecten.Count <= 5)
                {
                    vpu.lblUur.Content = " ";
                }

                // uiteindelijk toevoegen
                // altijd 24x, voor elk uur 1
                spVakmanDagLabelsBottom.Children.Insert(0, vpu);
            }


            VakmanDagViewProjectUurLabel vpu2 = new VakmanDagViewProjectUurLabel();
            vpu2.lblUur.Content = "Projecten";
            vpu2.lblUur.FontSize = 14;
            //vpu2.lblUur.FontWeight = FontWeights.Bold;
            vpu2.lblUur.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            vpu2.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;

            spOverzicht.Children.Add(vpu2); // TODO: check

            // voor alle projecten van deze vakman
            foreach (Project project in listProjecten)
            {
                // HEADERS TOEVOEGEN
                VakmanDagProjectHeader vakmandagprojectheader = new VakmanDagProjectHeader();
                if (project.ProjectId == ApplicationState.GetValue<int>(ApplicationVariables.intProjectId))
                {
                    vakmandagprojectheader.btnHeader.FontWeight = FontWeights.ExtraBold;
                    vakmandagprojectheader.btnHeader.FontSize = 12;
                    //vakmandagprojectheader.btnGoToProject.Content = "<";
                }


                vakmandagprojectheader.Project = project;
                spOverzicht.Children.Add(vakmandagprojectheader);

                // 24 UREN TOEVOEGEN

                // als ik (degene die dit scherm bekijkt) de projectleider ben
                bool blnIsOwner = project.ProjectleiderId == objProjectleider.ProjectleiderId;

                bool blnCanCancel = false;


                // loop door de 24 uren
                foreach (VakmanDagViewProjectUur vpu in GetProjectUren(listMandagen, project, dtBegintijd))
                {
                    vpu.project = project;
                    vpu.vakman = objVakman;

                    // oneven uren 'arceren'
                    if (vpu.Uur % 2 == 1)
                    {
                        vpu.spUur.Background = new SolidColorBrush(Colors.LightSteelBlue);
                    }

                    vpu.IsOwner = blnIsOwner;

                    // bereken de kleuren
                    vpu.SetColors();


                    // uiteindelijk toevoegen
                    // altijd 24x, voor elk uur 1
                    spVakmanDag.Children.Add(vpu);
                }


                // klaar met instellingen, vakmandagproject toevoegen
                // spVakmanDag.Children.Add(GetProjectUren();
            }

            Button btn = new Button();
            //         <Button Content="Project toevoegen" Height="23" Name="btnNaarProjecten" Width="116" Click="btnNaarProjecten_Click" />
            btn.Content = "+";
            btn.FontSize = 14;
            btn.Height = 23;
            btn.Width = 40;
            btn.Click += btnNaarProjecten_Click;
            
            StackPanel sdummy = new StackPanel();
            sdummy.Width = 250;
            sdummy.Children.Add(btn);
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
        }


        public Mandagen GetMandagOngeveer(List<Mandagen> mandagen, int vakmanId, int projectId, DateTime datum, int vakmanStatus, bool status)
        {
            return mandagen.Where(m => m.ProjectId == projectId && m.VakmanId == vakmanId && m.Begintijd <= datum && m.Eindtijd >= datum && m.VakmanstatusId == vakmanStatus && m.Status == status).OrderByDescending(m => m.Begintijd).FirstOrDefault();
        }


        public List<VakmanDagViewProjectUur> GetProjectUren(List<Mandagen> mandagen, Project project, DateTime dtDag)
        {
            List<VakmanDagViewProjectUur> resultSet = new List<VakmanDagViewProjectUur>();

            List<Mandagen> alleMandagen = mandagen.Where(m => m.ProjectId > 0 && m.VakmanstatusId == 1 && !m.Geannulleerd).ToList();
            List<Mandagen> mandagenCancelled = mandagen.Where(m => m.ProjectId == project.ProjectId && m.VakmanstatusId == 1 && m.Geannulleerd).ToList();
            List<Mandagen> alleMandagenInclCancelled = mandagen.Where(m => m.VakmanstatusId == 1).ToList();

            mandagen = mandagen.Where(m => m.ProjectId == project.ProjectId && m.VakmanstatusId == 1 && !m.Geannulleerd).ToList();

            for (int i = 0; i < 24; i++)
            {
                VakmanDagViewProjectUur vdvpu = new VakmanDagViewProjectUur();
                //vdvpu.lblUur.Content = i.ToString() + "u";

                // set ownership!
                vdvpu.IsOwner = project.ProjectleiderId == objProjectleider.ProjectleiderId;

                bool blnQ1Ingepland = false;
                bool blnQ2Ingepland = false;
                bool blnQ3Ingepland = false;
                bool blnQ4Ingepland = false;

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

                bool blnQ1ErgensDoormijAangevraagdEnGecancelled = false;
                bool blnQ2ErgensDoormijAangevraagdEnGecancelled = false;
                bool blnQ3ErgensDoormijAangevraagdEnGecancelled = false;
                bool blnQ4ErgensDoormijAangevraagdEnGecancelled = false;

                bool blnQ1ErgensDoormijIngepland = false;
                bool blnQ2ErgensDoormijIngepland = false;
                bool blnQ3ErgensDoormijIngepland = false;
                bool blnQ4ErgensDoormijIngepland = false;

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

                blnQ1Aangevraagd = mandagen.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status);
                blnQ2Aangevraagd = mandagen.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status);
                blnQ3Aangevraagd = mandagen.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status);
                blnQ4Aangevraagd = mandagen.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status);
                
                blnQ1AangevraagdAndCancelled = mandagenCancelled.Any(m => m.Begintijd <= dtQ1 && m.Eindtijd > dtQ1 && !m.Status);
                blnQ2AangevraagdAndCancelled = mandagenCancelled.Any(m => m.Begintijd <= dtQ2 && m.Eindtijd > dtQ2 && !m.Status);
                blnQ3AangevraagdAndCancelled = mandagenCancelled.Any(m => m.Begintijd <= dtQ3 && m.Eindtijd > dtQ3 && !m.Status);
                blnQ4AangevraagdAndCancelled = mandagenCancelled.Any(m => m.Begintijd <= dtQ4 && m.Eindtijd > dtQ4 && !m.Status);

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

                vdvpu.IsSelectedQ1 = blnQ1Ingepland;
                vdvpu.IsSelectedQ2 = blnQ2Ingepland;
                vdvpu.IsSelectedQ3 = blnQ3Ingepland;
                vdvpu.IsSelectedQ4 = blnQ4Ingepland;

                vdvpu.IsErgensDoorMijAangevraagdQ1AndCancelled = blnQ1ErgensDoormijAangevraagdEnGecancelled;
                vdvpu.IsErgensDoorMijAangevraagdQ2AndCancelled = blnQ2ErgensDoormijAangevraagdEnGecancelled;
                vdvpu.IsErgensDoorMijAangevraagdQ3AndCancelled = blnQ3ErgensDoormijAangevraagdEnGecancelled;
                vdvpu.IsErgensDoorMijAangevraagdQ4AndCancelled = blnQ4ErgensDoormijAangevraagdEnGecancelled;

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

                vdvpu.IsEnabledQ1 = vdvpu.IsOwner && !(blnQ1ErgensAangevraagd && blnQ1ErgensDoormijIngepland) && !blnQ1ErgensDoormijAangevraagd && !blnQ1ErgensDoormijAangevraagdEnGecancelled;
                vdvpu.IsEnabledQ2 = vdvpu.IsOwner && !(blnQ2ErgensAangevraagd && blnQ2ErgensDoormijIngepland) && !blnQ2ErgensDoormijAangevraagd && !blnQ2ErgensDoormijAangevraagdEnGecancelled;
                vdvpu.IsEnabledQ3 = vdvpu.IsOwner && !(blnQ3ErgensAangevraagd && blnQ3ErgensDoormijIngepland) && !blnQ3ErgensDoormijAangevraagd && !blnQ3ErgensDoormijAangevraagdEnGecancelled;
                vdvpu.IsEnabledQ4 = vdvpu.IsOwner && !(blnQ4ErgensAangevraagd && blnQ4ErgensDoormijIngepland) && !blnQ4ErgensDoormijAangevraagd && !blnQ4ErgensDoormijAangevraagdEnGecancelled;

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
                if (blnQ1AangevraagdAndCancelled)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagenCancelled, objVakman.VakmanId, project.ProjectId, dtQ1, 1, false);
                }
                else if (blnQ2AangevraagdAndCancelled)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagenCancelled, objVakman.VakmanId, project.ProjectId, dtQ2, 1, false);
                }
                else if (blnQ3AangevraagdAndCancelled)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagenCancelled, objVakman.VakmanId, project.ProjectId, dtQ3, 1, false);
                }
                else if (blnQ4AangevraagdAndCancelled)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagenCancelled, objVakman.VakmanId, project.ProjectId, dtQ4, 1, false);
                }
                else if (blnQ1Aangevraagd)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagen, objVakman.VakmanId, project.ProjectId, dtQ1, 1, false);
                }
                else if (blnQ2Aangevraagd)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagen, objVakman.VakmanId, project.ProjectId, dtQ2, 1, false);
                }
                else if (blnQ3Aangevraagd)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagen, objVakman.VakmanId, project.ProjectId, dtQ3, 1, false);
                }
                else if (blnQ4Aangevraagd)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagen, objVakman.VakmanId, project.ProjectId, dtQ4, 1, false);
                }
                else if (blnQ1Ingepland)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagen, objVakman.VakmanId, project.ProjectId, dtQ1, 1, true);
                }
                else if (blnQ2Ingepland)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagen, objVakman.VakmanId, project.ProjectId, dtQ2, 1, true);
                }
                else if (blnQ3Ingepland)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagen, objVakman.VakmanId, project.ProjectId, dtQ3, 1, true);
                }
                else if (blnQ4Ingepland)
                {
                    vdvpu.mandag = GetMandagOngeveer(mandagen, objVakman.VakmanId, project.ProjectId, dtQ4, 1, true);
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
            foreach (VakmanDagViewProjectUur vpu in spVakmanDag.Children)
            {
                Mandagen mandag = ApplicationState.GetValue<Mandagen>("highlightMandag");

                DateTime vpuBegintijd = new DateTime(mandag.Begintijd.Year, mandag.Begintijd.Month, mandag.Begintijd.Day, vpu.Uur, 0, 0);
                DateTime vpuEindtijd = vpuBegintijd.AddHours(1);

                DateTime dtQ1Eindtijd = vpuBegintijd.AddMinutes(15);
                DateTime dtQ2Eindtijd = vpuBegintijd.AddMinutes(30);
                DateTime dtQ3Eindtijd = vpuBegintijd.AddMinutes(45);


                // TODO: fixen!
                if (vpu.mandag != null && mandag != null && ((mandag == vpu.mandag) || ((mandag.Eindtijd >= vpuBegintijd) && (mandag.Begintijd < vpuEindtijd))))
                //if (vpu.mandag != null && ((VakmanDagViewProjectUur)sender).Uur == vpu.Uur)
                {
                    if (((vpu.IsAangevraagdQ1 && mandag == vpu.mandag) || (vpu.IsAangevraagdQ1 && vpu.IsOwner) || (vpu.IsAangevraagdQ1AndCancelled && vpu.IsOwner) || vpu.IsIngeplandQ1) && ((mandag.Eindtijd > vpuBegintijd) && (mandag.Begintijd < dtQ1Eindtijd)))
                    {
                        vpu.txt15.Foreground = new SolidColorBrush(Colors.White);
                        if (vpu.IsAangevraagdQ1AndCancelled)
                        {
                            //vpu.txt15.TextDecorations = TextDecorations.Strikethrough;
                        }
                    }
                    if (((vpu.IsAangevraagdQ2 && mandag == vpu.mandag) || (vpu.IsAangevraagdQ2 && vpu.IsOwner) || (vpu.IsAangevraagdQ2AndCancelled && vpu.IsOwner) || vpu.IsIngeplandQ2) && ((mandag.Eindtijd > dtQ1Eindtijd) && (mandag.Begintijd < dtQ2Eindtijd)))
                    {
                        vpu.txt30.Foreground = new SolidColorBrush(Colors.White);
                        if (vpu.IsAangevraagdQ2AndCancelled)
                        {
                            //vpu.txt30.TextDecorations = TextDecorations.Strikethrough;
                        }
                    }
                    if (((vpu.IsAangevraagdQ3 && mandag == vpu.mandag) || (vpu.IsAangevraagdQ3 && vpu.IsOwner) || (vpu.IsAangevraagdQ3AndCancelled && vpu.IsOwner) || vpu.IsIngeplandQ3) && ((mandag.Eindtijd > dtQ2Eindtijd) && (mandag.Begintijd < dtQ3Eindtijd)))
                    {
                        vpu.txt45.Foreground = new SolidColorBrush(Colors.White);
                        if (vpu.IsAangevraagdQ3AndCancelled)
                        {
                            //vpu.txt45.TextDecorations = TextDecorations.Strikethrough;
                        }
                    }
                    if (((vpu.IsAangevraagdQ4 && mandag == vpu.mandag) || (vpu.IsAangevraagdQ4 && vpu.IsOwner) || (vpu.IsAangevraagdQ4AndCancelled && vpu.IsOwner) || vpu.IsIngeplandQ4) && ((mandag.Eindtijd > dtQ3Eindtijd) && (mandag.Begintijd < vpuEindtijd)))
                    {
                        vpu.txt60.Foreground = new SolidColorBrush(Colors.White);
                        if (vpu.IsAangevraagdQ4AndCancelled)
                        {
                            //vpu.txt60.TextDecorations = TextDecorations.Strikethrough;
                        }
                    }
                }
            }
        }

        public void vdvpu_OnVakmanDagViewHighlightOnLeave(object sender, RoutedEventArgs e)
        {
            foreach (VakmanDagViewProjectUur vpu in spVakmanDag.Children)
            {
                Mandagen mandag = ApplicationState.GetValue<Mandagen>("highlightMandag");

                DateTime vpuBegintijd = new DateTime(mandag.Begintijd.Year, mandag.Begintijd.Month, mandag.Begintijd.Day, vpu.Uur, 0, 0);
                DateTime vpuEindtijd = vpuBegintijd.AddHours(1);

                DateTime dtQ1Eindtijd = vpuBegintijd.AddMinutes(15);
                DateTime dtQ2Eindtijd = vpuBegintijd.AddMinutes(30);
                DateTime dtQ3Eindtijd = vpuBegintijd.AddMinutes(45);

                if (vpu.mandag != null && mandag != null && ((mandag == vpu.mandag) || ((mandag.Eindtijd >= vpuBegintijd) && (mandag.Begintijd < vpuEindtijd))))
                //if (vpu.mandag != null && ((VakmanDagViewProjectUur)sender).Uur == vpu.Uur)
                {
                    if (((vpu.IsAangevraagdQ1 && mandag == vpu.mandag) || (vpu.IsAangevraagdQ1 && vpu.IsOwner) || (vpu.IsAangevraagdQ1AndCancelled && vpu.IsOwner) || vpu.IsIngeplandQ1) && ((mandag.Eindtijd > vpuBegintijd) && (mandag.Begintijd < dtQ1Eindtijd)))
                    {
                        vpu.txt15.Foreground = new SolidColorBrush(Colors.Black);
                    }
                    if (((vpu.IsAangevraagdQ2 && mandag == vpu.mandag) || (vpu.IsAangevraagdQ2 && vpu.IsOwner) || (vpu.IsAangevraagdQ2AndCancelled && vpu.IsOwner) || vpu.IsIngeplandQ2) && ((mandag.Eindtijd > dtQ1Eindtijd) && (mandag.Begintijd < dtQ2Eindtijd)))
                    {
                        vpu.txt30.Foreground = new SolidColorBrush(Colors.Black);
                    }
                    if (((vpu.IsAangevraagdQ3 && mandag == vpu.mandag) || (vpu.IsAangevraagdQ3 && vpu.IsOwner) || (vpu.IsAangevraagdQ3AndCancelled && vpu.IsOwner) || vpu.IsIngeplandQ3) && ((mandag.Eindtijd > dtQ2Eindtijd) && (mandag.Begintijd < dtQ3Eindtijd)))
                    {
                        vpu.txt45.Foreground = new SolidColorBrush(Colors.Black);
                    }
                    if (((vpu.IsAangevraagdQ4 && mandag == vpu.mandag) || (vpu.IsAangevraagdQ4 && vpu.IsOwner) || (vpu.IsAangevraagdQ4AndCancelled && vpu.IsOwner) || vpu.IsIngeplandQ4) && ((mandag.Eindtijd > dtQ3Eindtijd) && (mandag.Begintijd < vpuEindtijd)))
                    {
                        vpu.txt60.Foreground = new SolidColorBrush(Colors.Black);
                    }
                }
            }
        }


        public List<VakmanDagViewProjectUurLabel> GetProjectUrenLabels()
        {
            List<VakmanDagViewProjectUurLabel> resultSet = new List<VakmanDagViewProjectUurLabel>();

            for (int i = 23; i >= 0; i--)
            {
                VakmanDagViewProjectUurLabel vdvpu = new VakmanDagViewProjectUurLabel();
                vdvpu.lblUur.Content = "u" + (i + 1).ToString();
                vdvpu.Uur = i;

                resultSet.Add(vdvpu);
            }

            return resultSet;
        }

        public void LoadWeekInfo()
        {
            dbRepository dbrep = new dbRepository();
            int vakmanid = ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId);
            Vakman vm = dbrep.GetVakman(vakmanid);

            persoon persoon = dbrepOriginal.GetContact(vm.ContactIdOrigineel);


            lblWeekInfo.Content = "Week " + GetWeekNumber(dtBegintijd).ToString() + ", "+ dtBegintijd.ToString("dd MMM yyyy");

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
            //Thread.Sleep(TimeSpan.FromSeconds(5));

            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, dtBegintijd.AddDays(days));

            LoadVakmanDagView(false, intVakmanId, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay));
            LoadWeekInfo();

        }

        public void GoTerug(int days)
        {
            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, dtBegintijd.AddDays(-1 * days));

            LoadVakmanDagView(false, intVakmanId, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay));
            LoadWeekInfo();
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

        private Mandagen CreateMandag(VakmanDagViewProjectUur vpu, DateTime begintijd)
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

            int intCountMandagen = 0;
            int i = -1;
            int intCountQuarters = 0;

            bool blnIsMandagFound = false;

            List<Mandagen> listMandagen = new List<Mandagen>();

            foreach (VakmanDagViewProjectUur vpu in spVakmanDag.Children)
            {
                //// alleen als je de owner bent hier iets mee doen
                if (vpu.IsOwner)
                {

                    // stabielere manier op te checken of iets 'selected' moet zijn
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
                        if(vpu.IsOwner)
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

                            intCountQuarters = 0;

                        }

                        blnIsMandagFound = false;

                    }

                    if (isCheckedQ2 && !blnIsMandagFound)
                    {
                        DateTime begintijd = new DateTime(dtBegintijd.Year, dtBegintijd.Month, dtBegintijd.Day, vpu.Uur, 15, 0);
                        Mandagen mandag = CreateMandag(vpu, begintijd);

                        // hebben we nodig om te kijken welke we gaan toevoegen
                        if(vpu.IsOwner)
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
                        if(vpu.IsOwner)
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
                        if(vpu.IsOwner)
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

           dbRepository dbrep = new dbRepository();

            // get aanvullende lijst met mandagen om
            List<Mandagen> listMandagenNoOwner = dbrep.GetMandagenVoorVakmanIdNoOwner(intVakmanId, dtBegintijd, objProjectleider.ProjectleiderId);

            List<Mandagen> listMandagenTotal = new List<Mandagen>();
            listMandagenTotal.AddRange(listMandagen);
            listMandagenTotal.AddRange(listMandagenNoOwner);

            // lijst met alle datums om te splitten
            List<DateTime> datums = CalculateDatums(listMandagenTotal);

            // split de mandagen op basis van de lijst van datums
            listMandagen = SplitMandagen(listMandagen, datums);

            // Delete alle mandagen waarvan ik de owner ben
            int changeStatus = dbrep.DeleteMandagenVoorVakmanId(intVakmanId, dtBegintijd, objProjectleider.ProjectleiderId, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtLastRefreshDagView));

            if (changeStatus == 1)
            {
                // Delete alle (andere) mandagen die ik heb aangevraagd (per definitie dus ook owner)
                dbrep.ResetMandagenVoorVakmanId(intVakmanId, dtBegintijd, objProjectleider.ProjectleiderId);

                // nu alle (nieuwe) aanvragingen toevoegen
                foreach (Mandagen mandag in listMandagen)
                {

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

            // reload page:
            LoadVakmanDagView(false);

            // refresh alle andere programma's
            MenuControl owner = Tools.FindVisualParent<MenuControl>(this);
//#if DEBUG
//#else
            DateTime dt1 = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
            DateTime dt2 = dt1.AddDays(1);
            List<int> vakmanIds = new List<int>();
            vakmanIds.Add(objVakman.VakmanId);

            owner.PageChannelMessage("vakmandagview", Tools.CreateChannelMessage(vakmanIds, dt1, dt2));
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
            vakmanIds.Add(objVakman.VakmanId);

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

            dtBegintijd = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
            Weekstart = CalculateWeekstart(dtBegintijd);


            dbRepository dbrep = new dbRepository();

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

        private void btnNaarProjecten_Click(object sender, RoutedEventArgs e)
        {


            // create the page and load all values
            Projecten.Overzicht.Projecten po = new Projecten.Overzicht.Projecten();
            po.listProjecten = listProjecten;
            po.filterProjectleiderId = true;
            po.LoadView();

            // load the page into the contentcontrol
            MenuControl owner = Tools.FindVisualParent<MenuControl>(this);
            po.PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            po.PageOKButtonText = "Nieuw project";
            po.PageTitle = "Project Kiezen";

            dbOriginalRepository dbOriginalrep = new dbOriginalRepository();
            persoon p = dbOriginalrep.GetContact(objVakman.ContactIdOrigineel);

            po.PageSubtitle = "Voor " + p.voornaam + " " + p.achternaam;
            po.OkClick -= po.VakmannenOverview_Gereed;
            po.OkClick += po.VakmannenOverview_Gereed;

            po.CustomActieClick -= po.Selectproject;
            po.CustomActieClick += po.ToevoegenAanVakman;

            owner.PageGoToPage(po);

        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Window.GetWindow(this) != null)
            {
                svScrollviewer.Width = Window.GetWindow(this).ActualWidth - 284;
                svScrollviewerWrapper.Height = Window.GetWindow(this).ActualHeight - 260;
                svScrollviewer.MinHeight = svScrollviewerWrapper.Height;

                spProjectInfo2.Width = spProjectInfo.ActualWidth;
            }

        }

        #region dagen van de week

        private void btnMa_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intVakmanId, dtMaandag);
        }

        private void btnDi_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intVakmanId, dtMaandag.AddDays(1));
        }

        private void btnWo_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intVakmanId, dtMaandag.AddDays(2));
        }

        private void btnDo_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intVakmanId, dtMaandag.AddDays(3));
        }

        private void btnVr_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intVakmanId, dtMaandag.AddDays(4));
        }

        private void btnZa_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intVakmanId, dtMaandag.AddDays(5));
        }

        private void btnZo_Click(object sender, RoutedEventArgs e)
        {
            dtMaandag = CalculateWeekstart(dtBegintijd);
            LoadVakmanDagView(false, intVakmanId, dtMaandag.AddDays(6));
        }

        #endregion


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

        private void btnVorigeWeek_Click(object sender, RoutedEventArgs e)
        {
            GoTerug(7);
        }

        private void btnVolgendeWeek_Click(object sender, RoutedEventArgs e)
        {
            GoVerder(7);
        }




    }

}

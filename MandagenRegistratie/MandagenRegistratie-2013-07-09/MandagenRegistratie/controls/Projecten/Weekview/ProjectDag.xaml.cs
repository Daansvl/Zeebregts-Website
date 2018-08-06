using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using MandagenRegistratie.classes;
using MandagenRegistratieDomain;

namespace MandagenRegistratie.controls.Projecten.Weekview
{
    /// <summary>
    /// Interaction logic for VakmanDag.xaml
    /// </summary>
    public partial class ProjectDag : UserControl, INotifyPropertyChanged
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

            private bool isGewijzigd;
            public bool IsGewijzigd
            {
                get { return isGewijzigd; }
                set { SetField(ref isGewijzigd, value, "IsGewijzigd"); }
            }

        #endregion

        public Projectleider objProjectleider;
        public Vakman objVakman;
        public List<vwVakman> listVakmannen;
        //public Project objProjectLeidend;

        public Project objProject;

        public int ProjectId { get; set; }

        // extra
        private List<Mandagen> listMandagen;
        public DateTime dtBegintijd;


        #region "RoutedEvents"

        public static readonly RoutedEvent ProjectDagEvent = EventManager.RegisterRoutedEvent("OnProjectDagUpdate", RoutingStrategy.Bubble,
        typeof(RoutedEventHandler), typeof(ProjectDag));

        public event RoutedEventHandler OnProjectDagUpdate
        {
            add { AddHandler(ProjectDagEvent, value); }
            remove { RemoveHandler(ProjectDagEvent, value); }
        }


        #endregion


        public ProjectDag()
        {
            InitializeComponent();
        }

        public void LoadProjectDag()
        {

            // intialize
            spProjectDag.Children.Clear();
            dbRepository dbrep = new dbRepository();
            listMandagen = dbrep.GetMandagenByProject(objProject.ProjectId);

            DateTime startDatum = new DateTime(dtBegintijd.Year, dtBegintijd.Month, dtBegintijd.Day);
            DateTime eindDatum = startDatum.AddDays(1);

            listMandagen = listMandagen.Where(p => p.Begintijd >= startDatum && p.Begintijd < eindDatum && p.ProjectId == objProject.ProjectId).ToList();

            bool blnIsOwner = false;

            // als ik (degene die dit scherm bekijkt) de projectleider ben
            blnIsOwner = objProject.ProjectleiderId == objProjectleider.ProjectleiderId;


            // voor alle vakammen van dit project
            foreach (vwVakman vakman in listVakmannen)
            {
                // ziek en vakantie toevoegen
                VakmanDagProject vakmandagproject = new VakmanDagProject();

                // als ik (degene die dit scherm bekijkt) de projectleider ben
                vakmandagproject.IsGroen = blnIsOwner;
                vakmandagproject.ReadOnly = true;
                //vakmandagproject.IsSolid = true;

                // kijken of hij ingepland is voor dit project
                Mandagen mandag = listMandagen.Where(m => m.VakmanId == vakman.VakmanId && m.VakmanstatusId == 1).FirstOrDefault();

                // als hij ingepland is
                if (mandag != null)
                {
                    vakmandagproject.IsSolid = !mandag.Gewijzigd;

                    vakmandagproject.Duration = mandag.Gewijzigd ? mandag.UrenGewijzigd : mandag.Uren;
                }

                bool blnIsEditor;

                // als hij ingepland is
                if (mandag != null)
                {
                    blnIsEditor = mandag.MutatieDoorProjectleiderId == objProjectleider.ProjectleiderId;



                    if (mandag.Geannulleerd && !blnIsEditor)
                    {

                        vakmandagproject.IsOranje = true;
                        vakmandagproject.Duration = mandag.UrenGewijzigd;
                        vakmandagproject.IsSolid = false;

                        // flag op true zetten, zorgt er automatisch voor dat cancel en confirm buttons weergegeven kunnen worden
                        IsGewijzigd = true;

                    }
                    // als de mandag bevestigd is, niks aan de hand, gewoon de normale uren weergeven
                    else if (mandag.Bevestigd)
                    {
                        vakmandagproject.Duration = mandag.Uren;
                        vakmandagproject.IsSolid = true;
                    }
                    else // als niet bevestigd, dan is er iets gewijzigd, hetzij een wijziging, hetzij een nieuwe invoer
                    {
                        vakmandagproject.Duration = mandag.UrenGewijzigd;
                        vakmandagproject.IsSolid = false;

                        // flag op true zetten, zorgt er automatisch voor dat cancel en confirm buttons weergegeven kunnen worden
                        IsGewijzigd = true;

                        // bij gewijzigde items de gewijzigde uren weergeven
                    }
                }
                else
                {
                    vakmandagproject.Duration = 0;
                }

                // klaar met instellingen, vakmandagproject toevoegen
                spProjectDag.Children.Add(vakmandagproject);
            }


        }
    }
}

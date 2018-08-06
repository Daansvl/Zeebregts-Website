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
using System.ComponentModel;
using MandagenRegistratieDomain;

namespace MandagenRegistratie.controls
{
    /// <summary>
    /// Interaction logic for VakmanDagViewProjectUren.xaml
    /// </summary>
    public partial class VakmanDagViewProjectUren : UserControl
    {
        public VakmanDagViewProjectUren()
        {
            InitializeComponent();

            




        }



                public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "IsGroen":
                    break;
                case "IsOranje":
                    break;
                case "IsSolid":
                    //DrawWidth();
                    break;
                case "Duration":
                    break;
                //case "DurationPrevious":
                //    DrawWidthPrevious();
                //    break;
                case "ProjectId":
                    //lblProjectId.Content = ProjectId.ToString();
                    dbRepository dbrep = new dbRepository();
                    Project = dbrep.GetProject(projectId);
                    break;
                case "ProjectStatus":
                    //lblProjectId.Content = ProjectId.ToString();
                    break;
                case "ReadOnly":
                    //btnCreateMandag.Visibility = readOnly ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            //if (EqualityComparer<T>.Default.Equals(field, value) && typeof(T) != typeof(bool)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public int duration;
        public int Duration
        {
            get { return duration; }
            set { SetField(ref duration, value, "Duration"); }
        }




        public void DrawWidthPrevious()
        {
            //// TODO: eventueel nog berekenen, als de dag later begint
            //recDagStartPrevious.Width = 10;

            //if (durationPrevious > 0)
            //{
            //    spVakmanDagProjectPrevious.Visibility = System.Windows.Visibility.Visible;
            //    spVakmanDagProject.Visibility = System.Windows.Visibility.Collapsed;
            //}
            //else
            //{
            //    spVakmanDagProjectPrevious.Visibility = System.Windows.Visibility.Collapsed;
            //    spVakmanDagProject.Visibility = System.Windows.Visibility.Visible;
            //}

            //// lengte van de dag zelf instellen
            //recDagPrevious.Width = durationPrevious * 10;

            //// einde van de dag goed instellen
            //recDagEndPrevious.Width = 100 - recDag.Width - recDagStart.Width;
        }

        //private int durationPrevious;
        //public int DurationPrevious
        //{
        //    get { return durationPrevious; }
        //    set { SetField(ref durationPrevious, value, "DurationPrevious"); }
        //}

        private bool readOnly;
        public bool ReadOnly
        {
            get { return readOnly; }
            set { SetField(ref readOnly, value, "ReadOnly"); }
        }

        private int projectId;
        public int ProjectId
        {
            get { return projectId; }
            set { SetField(ref projectId, value, "ProjectId"); }
        }

        private Project project;
        public Project Project
        {
            get { return project; }
            set { SetField(ref project, value, "Project"); }
        }

        private bool isZiek;
        public bool IsZiek
        {
            get { return isZiek; }
            set { SetField(ref isZiek, value, "IsZiek"); }
        }

        private bool isVakantie;
        public bool IsVakantie
        {
            get { return isVakantie; }
            set { SetField(ref isVakantie, value, "IsVakantie"); }
        }

        private bool isNietIngevuld;
        public bool IsNietIngevuld
        {
            get { return isNietIngevuld; }
            set { SetField(ref isNietIngevuld, value, "IsNietIngevuld"); }
        }

        private bool isGroen;
        public bool IsGroen
        {
            get { return isGroen; }
            set { SetField(ref isGroen, value, "IsGroen"); }
        }

        private bool isOranje;
        public bool IsOranje
        {
            get { return isOranje; }
            set { SetField(ref isOranje, value, "IsOranje"); }
        }

        private bool isSolid;
        public bool IsSolid
        {
            get { return isSolid; }
            set { SetField(ref isSolid, value, "IsSolid"); }
        }

        private enum ProjectStatusDefinities
        {
            Groen = 0,
            GroenArceer = 1,
            Rood = 2,
            RoodArceer = 3
        }

        private void recDag_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void btnCreateMandag_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

    }
}

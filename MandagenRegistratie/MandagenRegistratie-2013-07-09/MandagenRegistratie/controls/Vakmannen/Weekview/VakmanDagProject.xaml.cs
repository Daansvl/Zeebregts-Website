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
using MandagenRegistratieDomain;

namespace MandagenRegistratie.controls
{
    /// <summary>
    /// Interaction logic for VakmanDagProject.xaml
    /// </summary>
    public partial class VakmanDagProject : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "IsGroen":
                    DrawColor();
                    break;
                case "IsOranje":
                    DrawColor();
                    break;
                case "IsSolid":
                    DrawColor();
                    //DrawWidth();
                    break;
                case "Duration":
                    DrawWidth();
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
                    btnCreateMandag.Visibility = readOnly ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
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

        #region "RoutedEvents"
        public static readonly RoutedEvent VakmanDagProjectEvent = EventManager.RegisterRoutedEvent("OnVakmanDagProjectUpdate", RoutingStrategy.Bubble,
        typeof(RoutedEventHandler), typeof(VakmanDagProject));

        public event RoutedEventHandler OnVakmanDagProjectUpdate
        {
            add { AddHandler(VakmanDagProjectEvent, value); }
            remove { RemoveHandler(VakmanDagProjectEvent, value); }
        }

        public static readonly RoutedEvent VakmanDagProjectSelectEvent = EventManager.RegisterRoutedEvent("OnVakmanDagProjectSelect", RoutingStrategy.Bubble,
typeof(RoutedEventHandler), typeof(VakmanDagProject));

        public event RoutedEventHandler OnVakmanDagProjectSelect
        {
            add { AddHandler(VakmanDagProjectSelectEvent, value); }
            remove { RemoveHandler(VakmanDagProjectSelectEvent, value); }
        }

        #endregion

        public void DrawColor()
        {
            ImageBrush imagebrush = new ImageBrush();

            if (IsOranje && !IsSolid)
            {
                imagebrush.ImageSource = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "../../../images/oranje-stippel.png"));
                recDag.Fill = imagebrush;
                btnCreateMandag.BorderBrush = new SolidColorBrush(Colors.LawnGreen);
            }
            else if (IsGroen && IsSolid)
            {
                recDag.Fill = new SolidColorBrush(Colors.LawnGreen);
                btnCreateMandag.BorderBrush = new SolidColorBrush(Colors.LawnGreen);
            }
            else if (IsGroen && !IsSolid)
            {
                imagebrush.ImageSource = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "../../../images/groen-stippel.png"));
                recDag.Fill = imagebrush;
                btnCreateMandag.BorderBrush = new SolidColorBrush(Colors.LawnGreen);
            }
            else if (!IsGroen && IsSolid)
            {
                recDag.Fill = new SolidColorBrush(Colors.OrangeRed);
                btnCreateMandag.BorderBrush = new SolidColorBrush(Colors.OrangeRed);
            }
            else if (!IsGroen && !IsSolid)
            {
                imagebrush.ImageSource = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "../../../images/rood-stippel.png"));
                recDag.Fill = imagebrush;
                btnCreateMandag.BorderBrush = new SolidColorBrush(Colors.OrangeRed);
            }
        }

        public void DrawWidth()
        {
            // TODO: eventueel nog berekenen, als de dag later begint
            //recDagStart.Width = 10;
            
            // lengte van de dag zelf instellen
            //recDag.Width = duration * 10;

            if (IsOranje)
            {
                //recDag.Width = 80;
            }

            if (IsGroen && duration > 0)
            {
                btnCreateMandag.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (duration == 0)
            {
                spVakmanDagProject.Visibility = System.Windows.Visibility.Collapsed;

            }
            else
            {
                spVakmanDagProject.Visibility = System.Windows.Visibility.Visible;
            }
            // einde van de dag goed instellen
            //recDagEnd.Width = 100 - recDag.Width - recDagStart.Width;
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

        //public ProjectStatusDefinities projectStatus;

        //public ProjectStatusDefinities ProjectStatus
        //{
        //    get { return projectStatus; }
        //    set { SetField(ref projectStatus, value, "ProjectStatus"); }
        //}

        public VakmanDagProject()
        {
            InitializeComponent();

            //slider.LowerSlider.SelectionStart = 0;
            //slider.LowerSlider.SelectionEnd = 100;

        }

        private void recDag_MouseDown(object sender, MouseButtonEventArgs e)
        {

            //ToggleColor();
        }

        private void btnCreateMandag_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnCreateMandag_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(VakmanDagProjectEvent, sender);
            args.RoutedEvent = VakmanDagProjectEvent;

            //Reset();

            RaiseEvent(args);

            //IsDragging = false;

            //ProjectStatus = ProjectStatusDefinities.Groen;


            //recDag.Width = 80;
            //recDag.Fill = new SolidColorBrush(Colors.LawnGreen);

        }

        private void spWrapper_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(VakmanDagProjectSelectEvent, sender);
            args.RoutedEvent = VakmanDagProjectSelectEvent;
            RaiseEvent(args);

        }

        //private void btnUrenDecrease_Click(object sender, RoutedEventArgs e)
        //{

        //    int intUren = Convert.ToInt32(lblVakmandagUren.Content);
        //    int intMinuten = Convert.ToInt32(lblVakmandagMinuten.Content);

        //    lblVakmandagUren.Content = intUren - 1;

        //    RoutedEventArgs args = new RoutedEventArgs(VakmanDagProjectEvent, sender);
        //    args.RoutedEvent = VakmanDagProjectEvent;
        //    RaiseEvent(args);

        //}

        //private void btnUrenIncrease_Click(object sender, RoutedEventArgs e)
        //{
        //    int intUren = Convert.ToInt32(lblVakmandagUren.Content);
        //    int intMinuten = Convert.ToInt32(lblVakmandagMinuten.Content);

        //    lblVakmandagUren.Content = intUren + 1;

        //    RoutedEventArgs args = new RoutedEventArgs(VakmanDagProjectEvent, sender);
        //    args.RoutedEvent = VakmanDagProjectEvent;
        //    RaiseEvent(args);

        //}

        //private void btnMinutenDecrease_Click(object sender, RoutedEventArgs e)
        //{
        //    int intUren = Convert.ToInt32(lblVakmandagUren.Content);
        //    int intMinuten = Convert.ToInt32(lblVakmandagMinuten.Content);

        //    switch (intMinuten)
        //    {
        //        case 0:
        //            intUren = intUren - 1;
        //            intMinuten = 45;
        //            break;
        //        case 15:
        //            intUren = intUren;
        //            intMinuten = 0;
        //            break;
        //        case 30:
        //            intUren = intUren;
        //            intMinuten = 15;
        //            break;
        //        case 45:
        //            intUren = intUren;
        //            intMinuten = 30;
        //            break;

        //    }

        //    lblVakmandagUren.Content = intUren;
        //    lblVakmandagMinuten.Content = intMinuten;

        //    RoutedEventArgs args = new RoutedEventArgs(VakmanDagProjectEvent, sender);
        //    args.RoutedEvent = VakmanDagProjectEvent;
        //    RaiseEvent(args);

        //}

        //private void btnMinutenIncrease_Click(object sender, RoutedEventArgs e)
        //{
        //    int intUren = Convert.ToInt32(lblVakmandagUren.Content);
        //    int intMinuten = Convert.ToInt32(lblVakmandagMinuten.Content);

        //    switch (intMinuten)
        //    {
        //        case 0:
        //            intUren = intUren;
        //            intMinuten = 15;
        //            break;
        //        case 15:
        //            intUren = intUren;
        //            intMinuten = 30;
        //            break;
        //        case 30:
        //            intUren = intUren;
        //            intMinuten = 45;
        //            break;
        //        case 45:
        //            intUren = intUren + 1;
        //            intMinuten = 0;
        //            break;

        //    }

        //    lblVakmandagUren.Content = intUren;
        //    lblVakmandagMinuten.Content = intMinuten;

        //    RoutedEventArgs args = new RoutedEventArgs(VakmanDagProjectEvent, sender);
        //    args.RoutedEvent = VakmanDagProjectEvent;
        //    RaiseEvent(args);

        //}
    }
}

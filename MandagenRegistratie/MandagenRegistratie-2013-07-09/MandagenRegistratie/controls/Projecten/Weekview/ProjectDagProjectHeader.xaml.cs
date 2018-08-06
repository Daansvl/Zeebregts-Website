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
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Projecten.Weekview
{
    /// <summary>
    /// Interaction logic for ProjectDagProjectHeader.xaml
    /// </summary>
    public partial class ProjectDagProjectHeader : UserControl, INotifyPropertyChanged
    {
        #region "RoutedEvents"
        public static readonly RoutedEvent ProjectDagProjectHeaderEvent = EventManager.RegisterRoutedEvent("OnProjectDagProjectHeaderUpdate", RoutingStrategy.Bubble,
        typeof(RoutedEventHandler), typeof(ProjectDagProjectHeader));

        public event RoutedEventHandler OnProjectDagProjectHeaderUpdate
        {
            add { AddHandler(ProjectDagProjectHeaderEvent, value); }
            remove { RemoveHandler(ProjectDagProjectHeaderEvent, value); }
        }
        #endregion



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "Vakman":
                    lblHeader.Content = vakman.Adres;
                    if (ApplicationState.GetValue<int>("intVakmanId") == vakman.VakmanId)
                    {
                        // make bold
                        lblHeader.FontWeight = FontWeights.ExtraBold;
                    }
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

        private vwVakman vakman;
        public vwVakman Vakman
        {
            get { return vakman; }
            set { SetField(ref vakman, value, "Vakman"); }
        }


        public ProjectDagProjectHeader()
        {
            InitializeComponent();

            //slider.LowerSlider.SelectionStart = 0;
            //slider.LowerSlider.SelectionEnd = 100;

        }

        public void btnGoToVakman_Click(object sender, RoutedEventArgs e)
        {
            ApplicationState.SetValue("intVakmanId", vakman.VakmanId);

            RoutedEventArgs args = new RoutedEventArgs(ProjectDagProjectHeaderEvent, sender);
            args.RoutedEvent = ProjectDagProjectHeaderEvent;

            RaiseEvent(args);

        }

    }
}

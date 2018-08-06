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

namespace MandagenRegistratie.controls.Vakmannen.Dagview
{
    /// <summary>
    /// Interaction logic for VakmanDagProjectHeader.xaml
    /// </summary>
    public partial class VakmanDagProjectHeader : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "Project":
                    dbRepository dbrep = new dbRepository();
                    dbOriginalRepository dbreporiginal = new dbOriginalRepository();
                    persoon pp = dbreporiginal.GetContact(dbrep.GetProjectleider(project.ProjectleiderId).ContactIdOrigineel);
                    lblHeader.Content = pp.voornaam + " " + pp.tussenvoegsel + " " + pp.achternaam;
                    lblHeader.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    //lblHeader.BorderThickness = new Thickness(1);
                    //lblHeader.BorderBrush = new SolidColorBrush(Colors.Black);
                    
                    
                    btnHeader.Style = this.FindResource("LinkButton") as Style;
                    btnHeader.Content = project.Naam;

                    btnHeader.Margin = new Thickness(0, 0, 0, -8);

                    if (ApplicationState.GetValue<int>("intProjectId") == project.ProjectId)
                    {
                        // make bold
                        //btnHeader.FontWeight = FontWeights.ExtraBold;
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

        #region "RoutedEvents"
        public static readonly RoutedEvent VakmanDagProjectHeaderEvent = EventManager.RegisterRoutedEvent("OnVakmanDagProjectHeaderUpdate", RoutingStrategy.Bubble,
        typeof(RoutedEventHandler), typeof(VakmanDagProjectHeader));

        public event RoutedEventHandler OnVakmanDagProjectHeaderUpdate
        {
            add { AddHandler(VakmanDagProjectHeaderEvent, value); }
            remove { RemoveHandler(VakmanDagProjectHeaderEvent, value); }
        }
        #endregion


        private Project project;
        public Project Project
        {
            get { return project; }
            set { SetField(ref project, value, "Project"); }
        }


        public VakmanDagProjectHeader()
        {
            InitializeComponent();

            //slider.LowerSlider.SelectionStart = 0;
            //slider.LowerSlider.SelectionEnd = 100;

        }

        private void btnGoToProject_Click(object sender, RoutedEventArgs e)
        {
            dbRepository dbrep = new dbRepository();
            dbOriginalRepository dbOriginalRep = new dbOriginalRepository();

            ApplicationState.SetValue(ApplicationVariables.intProjectId, project.ProjectId);
            //ProjectView pv = new ProjectView();
            //pv.Show();
            persoon projectleider = dbOriginalRep.GetContact(dbrep.GetProjectleider(project.ProjectleiderId).ContactIdOrigineel);

            // create the page and load all values
            Projecten.Overzicht.Project po = new Projecten.Overzicht.Project(project.Naam + " - ID " + project.ProjectIdOrigineel, projectleider.voornaam + " " + projectleider.tussenvoegsel + " " + projectleider.achternaam);
            //new Projecten.Overzicht.Project(project.ProjectId + ": " + project.Naam, projectleider.voornaam + " " + projectleider.tussenvoegsel + " " + projectleider.achternaam);

            // load the page into the contentcontrol
            MenuControl owner = Tools.FindVisualParent<MenuControl>(this);
            //po.PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            //po.PageOKButtonText = "PROJECT TOEVOEGEN";

            //po.OkClick += po.ToevoegenAanVakman;
            //po.dgProjecten.MouseDoubleClick -= po.dgProjecten_MouseDoubleClick;
            //po.dgProjecten.MouseDoubleClick += po.dgProjecten_MouseDoubleClickForVakman;
            //po.Load();
            //po.tabControl.SelectedIndex = 1;

            owner.PageGoToPage(po);


        }


    }
}

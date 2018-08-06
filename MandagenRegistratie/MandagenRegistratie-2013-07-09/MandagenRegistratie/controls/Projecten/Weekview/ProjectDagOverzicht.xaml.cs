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
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Projecten.Weekview
{
    /// <summary>
    /// Interaction logic for ProjectDagOverzicht.xaml
    /// </summary>
    public partial class ProjectDagOverzicht : UserControl, INotifyPropertyChanged
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
        public List<vwVakman> listVakmannen;
        public Project objProject;


        //#region "RoutedEvents"

        //public static readonly RoutedEvent VakmanDagEvent = EventManager.RegisterRoutedEvent("OnVakmanDagUpdate", RoutingStrategy.Bubble,
        //typeof(RoutedEventHandler), typeof(VakmanDag));

        //public event RoutedEventHandler OnVakmanDagUpdate
        //{
        //    add { AddHandler(VakmanDagEvent, value); }
        //    remove { RemoveHandler(VakmanDagEvent, value); }
        //}


        //#endregion



        //public void Randomize()
        //{

        //    int aantalProjecten = 4;
        //    bool temp = true;

        //    for (int i = 0; i < aantalProjecten; i++)
        //    {
        //        Random rand = new Random();
        //        //aantalProjecten = aantalProjecten + rand.Next(3);

        //        VakmanDagProject project = new VakmanDagProject();

        //        project.ProjectId = 1270;
        //        if (i >= 1)
        //        {
        //            project.ProjectId = 1300;
        //        }

        //        project.Duration = 0;
                
        //        if (rand.NextDouble() > 0.5 && temp)
        //        {
        //            project.Duration = 8;
        //            temp = false;
        //        }
        //        project.ReadOnly = false;
        //        project.IsProjectleider = ProjectId == project.ProjectId;
        //        //ApplicationState.SetValue(Constants.ProjectleiderId, 1);
        //        //ApplicationState.GetValue<int>(Constants.ProjectleiderId);

        //        vakmanDagProjectInitial.Add(project);
        //    }

        //    LoadVakmanDag();
        //}

        public ProjectDagOverzicht()
        {
            InitializeComponent();
        }

        public void LoadProjectDagOverzicht()
        {

            // intialize
            spProjectDagOverzicht.Children.Clear();

            foreach (vwVakman vakman in listVakmannen)
            {
                ProjectDagProjectHeader projectdagprojectheader = new ProjectDagProjectHeader();
                if (vakman.VakmanId == ApplicationState.GetValue<int>("intVakmanId"))
                {
                    projectdagprojectheader.btnGoToVakman.Content = "<";
                }

                projectdagprojectheader.Vakman = vakman;
                spProjectDagOverzicht.Children.Add(projectdagprojectheader);
            }

        }

    }
}

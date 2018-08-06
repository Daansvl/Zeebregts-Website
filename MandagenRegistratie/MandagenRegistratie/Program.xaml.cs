using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MandagenRegistratie.classes;
using MandagenRegistratie.controls;
using MandagenRegistratieDomain;
using MandagenRegistratieLogic;
using MandagenRegistratie.controls.Projecten.Weekview;
using MandagenRegistratie.controls.Vakmannen.Dagview;

namespace MandagenRegistratie
{
    /// <summary>
    /// Interaction logic for VakmanView.xaml
    /// </summary>
    public partial class Program : Window
    {
        VakmanDagenView vakmandagenview = new VakmanDagenView();


        public Program()
        {
            InitializeComponent();

            ApplicationState.SetValue("intProjectleider", 1);
            ApplicationState.SetValue("intVakmanId", 1);

            // starten met deze view
            vakmandagenview.LoadVakmandagenView();
            ccPageContainer.Content = vakmandagenview;
        }


        public void vakmandag_OnVakmanDagenViewUpdate(object sender, RoutedEventArgs e)
        {

            Stopwatch s1 = Stopwatch.StartNew();
            s1.Start();

            foreach (Control control in vakmandagenview.spVakmanDagen.Children)
            {
                if (control.GetType() == typeof(VakmanDag))
                {
                    VakmanDag vakmandag = (VakmanDag)control;
                    vakmandag.IsGewijzigd = false;
                    vakmandag.LoadVakmanDag();
                }
            }
            s1.Stop();

        }

        public void vakmandag_OnVakmanDagProjectHeaderUpdate(object sender, RoutedEventArgs e)
        {
            ProjectDagenView projectDagenView = new ProjectDagenView();
            projectDagenView.LoadProjectDagenView();
            ccPageContainer.Content = projectDagenView;
        }

        public void vakmandag_OnProjectDagProjectHeaderUpdate(object sender, RoutedEventArgs e)
        {
            VakmanDagenView vakmanDagenView = new VakmanDagenView();
            vakmanDagenView.LoadVakmandagenView();
            ccPageContainer.Content = vakmanDagenView;
        }


        private bool IsDragging = false;

        private void GridSplitter_MouseMove_1(object sender, MouseEventArgs e)
        {

            if (IsDragging)
            {
            }


        }

        private void GridSplitter_DragStarted_1(object sender, DragStartedEventArgs e)
        {
            IsDragging = true;
            //IsKnownTargetWidth = false;
            //intCount = 0;
        }

        private List<Periode> Periodes = new List<Periode>();

        //public void CalculateSplitter()
        //{
        //    foreach (VakmanDag vakmandag in controlVakmanDagen.spVakmanDagen.Children)
        //    {
        //        Periode periode = new Periode();
        //        // als dit project ook in de vorige zat, vorige verlengen..
        //        if (vakmandag.VakmanDagProjectCurrent[0].Duration == 8)
        //        {
        //            periode.IsLeadingPeriod = true;
        //        }
        //        else
        //        {
        //            periode.IsLeadingPeriod = false;
        //        }

        //        periode.Duration = 1;
        //        periode.IsProjectleider = true;

        //        periode.ReadOnly = false; // vakmandag.IsReadOnly(); // TODO: moet false geven nu
        //        Periodes.Add(periode);
        //    }

        //    controlMultislider.Draw(Periodes);

        //}

        //public void GridSplitter_DragCompleted_1(object sender, RoutedEventArgs e)
        //{

        //    List<bool> listProjectleiders = new List<bool>();
        //    int countColumn = 0;

        //    lblGridsplitterStatus.Content = "projectId: " + controlMultislider.ProjectId.ToString();

        //    foreach (ColumnDefinition cd in controlMultislider.gridMultislider.ColumnDefinitions)
        //    {
        //        if (cd.Width.Value.ToString() != "1")
        //        {
        //            int aantalDagen = Convert.ToInt32(Math.Round(cd.Width.Value / 100, 0));


        //            for (int i = 0; i < aantalDagen; i++)
        //            {
        //                bool isProjectleider = false;
        //                if (((Label)controlMultislider.gridMultislider.Children[countColumn]).Background.ToString() == new SolidColorBrush(Colors.LawnGreen).ToString())
        //                {
        //                    isProjectleider = true;
        //                }
        //                listProjectleiders.Add(isProjectleider);
        //            }
        //        }
        //        countColumn++;

        //    }

        //    // checken of we evenveel dagen hebben ontdekt:
        //    if (controlVakmanDagen.spVakmanDagen.Children.Count == listProjectleiders.Count)
        //    {
        //        int countProjectleiders = 0;
        //        foreach (VakmanDag vakmandag in controlVakmanDagen.spVakmanDagen.Children)
        //        {
        //            bool isFirst = true;
        //            foreach (MandagenRegistratie.controls.VakmanDagProject project in vakmandag.spVakmanDag.Children)
        //            {
        //                if (isFirst)
        //                {
        //                    project.IsGroen = listProjectleiders[countProjectleiders];
        //                    isFirst = false;
        //                }
        //            }
        //            countProjectleiders++;
        //        }

        //    }



        //    IsDragging = false;
        //    //lblGridsplitterStatus.Content = "dragged";
        //}

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {






            DateTime nu = DateTime.Now;
            Stopwatch s1 = Stopwatch.StartNew();
            s1.Start();

            ApplicationState.SetValue("intProjectleider", 1);

            if (ccPageContainer.Content.GetType() == typeof(ProjectDagenView))
            {
                ProjectDagenView projectDagenView = new ProjectDagenView();
                projectDagenView.LoadProjectDagenView();
                ccPageContainer.Content = projectDagenView;
            }
            else if (ccPageContainer.Content.GetType() == typeof(VakmanDagView))
            {
                VakmanDagView vakmanDagView = new VakmanDagView();
                vakmanDagView.LoadVakmanDagView(false);
                ccPageContainer.Content = vakmanDagView;
            }
            else
            {
                vakmandagenview.LoadVakmandagenView();
                ccPageContainer.Content = vakmandagenview;
            }


            TimeSpan duration = DateTime.Now.Subtract(nu);

            s1.Stop();
            //lblGridsplitterStatus2.Content = duration.TotalMilliseconds.ToString();
            //lblGridsplitterStatus3.Content = s1.ElapsedMilliseconds.ToString();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ApplicationState.SetValue("intProjectleider", 2);
            if (ccPageContainer.Content.GetType() == typeof(ProjectDagenView))
            {
                ProjectDagenView projectDagenView = new ProjectDagenView();
                projectDagenView.LoadProjectDagenView();
                ccPageContainer.Content = projectDagenView;
            }
            else if (ccPageContainer.Content.GetType() == typeof(VakmanDagView))
            {
                VakmanDagView vakmanDagView = new VakmanDagView();
                vakmanDagView.LoadVakmanDagView(false);
                ccPageContainer.Content = vakmanDagView;
            }
            else
            {

                vakmandagenview.LoadVakmandagenView();
                ccPageContainer.Content = vakmandagenview;

            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ApplicationState.SetValue("intProjectleider", 3);

            if (ccPageContainer.Content.GetType() == typeof(ProjectDagenView))
            {
                ProjectDagenView projectDagenView = new ProjectDagenView();
                projectDagenView.LoadProjectDagenView();
                ccPageContainer.Content = projectDagenView;
            }
            else if (ccPageContainer.Content.GetType() == typeof(VakmanDagView))
            {
                VakmanDagView vakmanDagView = new VakmanDagView();
                vakmanDagView.LoadVakmanDagView(false);
                ccPageContainer.Content = vakmanDagView;
            }
            else
            {
                vakmandagenview.LoadVakmandagenView();
                ccPageContainer.Content = vakmandagenview;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ccPageContainer.Content.GetType() == typeof(ProjectDagenView))
            {
                ProjectDagenView projectDagenView = new ProjectDagenView();
                projectDagenView.LoadProjectDagenView();
                ccPageContainer.Content = projectDagenView;
            }
            else if (ccPageContainer.Content.GetType() == typeof(VakmanDagView))
            {
                VakmanDagView vakmanDagView = new VakmanDagView();
                vakmanDagView.LoadVakmanDagView(false);
                ccPageContainer.Content = vakmanDagView;
            }
            else
            {
                vakmandagenview.LoadVakmandagenView();
                ccPageContainer.Content = vakmandagenview;
            }

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            VakmanDagView vakmanDagView = new VakmanDagView();
            vakmanDagView.dtBegintijd = ApplicationState.GetValue<DateTime>("dtSelectedDay");
            vakmanDagView.LoadVakmanDagView(true);
            vakmanDagView.LoadWeekInfo();

            //vakmanDagView.Vakman;
                //vakmanDagViewProjectUren.Load()
                ccPageContainer.Content = vakmanDagView;
        }

    }
}

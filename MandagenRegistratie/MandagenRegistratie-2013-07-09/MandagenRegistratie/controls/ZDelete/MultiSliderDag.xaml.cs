using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MandagenRegistratie.controls
{
    /// <summary>
    /// Interaction logic for MultiSliderDag.xaml
    /// </summary>
    public partial class MultiSliderDag : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        private string name;
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value, "Name"); }
        }





        public MultiSliderDag()
        {
            InitializeComponent();

            listPeriodes.Add(new Periode(false, 1, false));

            LoadVakmanView();

        }

        public static readonly RoutedEvent MultiSliderDagEvent = EventManager.RegisterRoutedEvent("OnMultiSliderDagUpdate", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(MultiSlider));
        
        public event RoutedEventHandler OnMultiSliderDagUpdate
        {
            add { AddHandler(MultiSliderDagEvent, value); }
            remove { RemoveHandler(MultiSliderDagEvent, value); }
        }

        protected void OnUpdate()
        {
            //DemoEventArgs args = new DemoEventArgs(MultiSliderDagEvent,;
            //args.RoutedEvent = MultiSliderDagEvent;
            //RaiseEvent(args);
        }

        public List<Periode> listPeriodes = new List<Periode>();
        public Grid grid = new Grid();

        public class Periode
        {
            public bool IsProjectleider { get; set; }
            public int Duration { get; set; }
            public bool ReadOnly { get; set; }

            public Periode(bool isProjectleider, int duration, bool readOnly)
            {
                IsProjectleider = isProjectleider;
                Duration = duration;
                ReadOnly = readOnly;
            }
        }

        public void LoadVakmanView()
        {
            int intCount = 0;

            grid.Name = "myTest2";

            Name = "test";

            ColumnDefinition cdLast = new ColumnDefinition();
            GridSplitter gsLast = new GridSplitter();

            foreach (Periode periode in listPeriodes)
            {
                ColumnDefinition cdc = new ColumnDefinition();
                cdc.Name = "cdc" + intCount.ToString();
                cdc.Width = new GridLength(periode.Duration, GridUnitType.Star);

                ColumnDefinition cds = new ColumnDefinition();
                cds.Name = "cds" + intCount.ToString();
                cds.Width = new GridLength(periode.Duration * 100, GridUnitType.Auto);

                grid.ColumnDefinitions.Add(cdc);
                grid.ColumnDefinitions.Add(cds);

                cdLast = cds;

                Label label = new Label();
                label.Content = intCount.ToString();
                label.Background = new SolidColorBrush(periode.IsProjectleider ? Colors.LawnGreen : Colors.OrangeRed);

                grid.Children.Add(label);
                Grid.SetColumn(label, intCount * 2);

                GridSplitter gs = new GridSplitter();
                gs.ResizeBehavior = GridResizeBehavior.PreviousAndNext;
                gs.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                gs.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                gs.Width= 5;
                gs.Background = new SolidColorBrush(Colors.Black);
                //gs.DragStarted += new System.Windows.Controls.Primitives.DragStartedEventHandler(GridSplitter_DragStarted_1);
                //gs.DragCompleted += new System.Windows.Controls.Primitives.DragCompletedEventHandler(GridSplitter_DragCompleted_1);
                //gs.DragDelta += new System.Windows.Controls.Primitives.DragDeltaEventHandler(GridSplitter_DragDelta_1);

               // DragCompleted="GridSplitter_DragCompleted_1" MouseMove="GridSplitter_MouseMove_1"


                grid.Children.Add(gs);
                gsLast = gs;

                Grid.SetColumn(gs, (intCount * 2) + 1);

                intCount++;
            }


            grid.ColumnDefinitions.Remove(cdLast);
            grid.Children.Remove(gsLast);

            grid.Height = 30;

            spStackPanel.Children.Add(grid);
        }


        private bool IsDragging = false;


        private void GridSplitter_MouseMove_1(object sender, MouseEventArgs e)
        {

            if (IsDragging)
            {

            }


        }

        public void Reset()
        {
            foreach (ColumnDefinition cd in grid.ColumnDefinitions)
            {
                if (cd.Width.Value.ToString() != "1")
                {
                    cd.Width = new GridLength(Math.Round(cd.Width.Value/100,0)*100, GridUnitType.Star);
                }
            }

        }


        private void GridSplitter_DragStarted_1(object sender, DragStartedEventArgs e)
        {
            //foreach (ColumnDefinition cd in myTest.ColumnDefinitions)
            //{
            //    if (cd.Width.Value.ToString() != "1")
            //    {
            //        cd.Width = new GridLength(cd.Width.Value, GridUnitType.Star);
            //    }

            //}

            IsDragging = true;
            dblTotalChange = 0;
            //IsKnownTargetWidth = false;
            //intCount = 0;
        }

        private double dblTotalChange = 0;

        private void GridSplitter_DragDelta_1(object sender, DragDeltaEventArgs e)
        {
            //foreach (ColumnDefinition cd in myTest.ColumnDefinitions)
            //{
            //    if (cd.Width.Value.ToString() != "1")
            //    {
            //        cd.Width = new GridLength(cd.Width.Value, GridUnitType.Star);
            //    }

            //}
            dblTotalChange += e.HorizontalChange;


            IsDragging = true;
            //IsKnownTargetWidth = false;
            //intCount = 0;
        }

        private void GridSplitter_DragCompleted_1(object sender, DragCompletedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(MultiSliderDagEvent, sender);
            args.RoutedEvent = MultiSliderDagEvent;

            Reset();

            RaiseEvent(args);

            IsDragging = false;
            //OnUpdate();
        }

    }
}

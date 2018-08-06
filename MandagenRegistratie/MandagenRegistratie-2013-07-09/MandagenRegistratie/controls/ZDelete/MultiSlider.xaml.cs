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
using MandagenRegistratie.classes;

namespace MandagenRegistratie.controls
{
    /// <summary>
    /// Interaction logic for MultiSlider.xaml
    /// </summary>
    public partial class MultiSlider : UserControl, INotifyPropertyChanged
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

        //private string name;
        //public string Name
        //{
        //    get { return name; }
        //    set { SetField(ref name, value, "Name"); }
        //}
       
        public int ProjectId { get; set; }

        public MultiSlider()
        {
            InitializeComponent();
        }

        public void Draw(List<Periode> listBasicPeriodes)
        {
            List<Periode> listPeriodes = new List<Periode>();

            for(int i = 0; i < listBasicPeriodes.Count; i++)
            {
                if (i > 0 && listBasicPeriodes[i].IsLeadingPeriod != listBasicPeriodes[i - 1].IsLeadingPeriod)
                {
                    listPeriodes.Add(listBasicPeriodes[i]);
                }
                else if (i > 0)
                {
                    listPeriodes[listPeriodes.Count - 1].Duration += 1;
                }
                else
                {
                    listPeriodes.Add(listBasicPeriodes[i]);
                }

            }


                        // first reset grid
            //Grid gridMultislider = new Grid();
            gridMultislider.Children.Clear();

            //lblGridsplitterStatus.Content = ProjectId.ToString();

            int intCount = 0;

            ColumnDefinition cdLast = new ColumnDefinition();
            GridSplitter gsLast = new GridSplitter();

            foreach (Periode periode in listPeriodes)
            {
                    ColumnDefinition cdc = new ColumnDefinition();
                    cdc.Name = "cdc" + intCount.ToString();
                    cdc.Width = new GridLength(periode.Duration * 100, GridUnitType.Star);

                    ColumnDefinition cds = new ColumnDefinition();
                    cds.Name = "cds" + intCount.ToString();
                    cds.Width = new GridLength(1, GridUnitType.Auto);

                    gridMultislider.ColumnDefinitions.Add(cdc);
                        gridMultislider.ColumnDefinitions.Add(cds);
                    cdLast = cds;

                    Label label = new Label();
                    label.Content = intCount.ToString();
                    label.Background = new SolidColorBrush(periode.IsProjectleider ? Colors.LawnGreen : Colors.OrangeRed);
                    if (periode.ReadOnly)
                    {
                        label.Background = new SolidColorBrush(Colors.White);
                       //gridMultislider.Children.Remove(gsLast);
                       gsLast.Width = 0;
                    }

                    gridMultislider.Children.Add(label);
                    Grid.SetColumn(label, intCount * 2);

                    GridSplitter gs = new GridSplitter();
                    gs.SnapsToDevicePixels = true;
                    gs.DragIncrement = 100;

                    gs.ResizeBehavior = GridResizeBehavior.PreviousAndNext;

                    gs.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    gs.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    gs.Width = 5;
                    gs.Background = new SolidColorBrush(Colors.Black);
                    gs.DragStarted += new System.Windows.Controls.Primitives.DragStartedEventHandler(GridSplitter_DragStarted_1);
                    gs.DragCompleted += new System.Windows.Controls.Primitives.DragCompletedEventHandler(GridSplitter_DragCompleted_1);
                    gs.DragDelta += new System.Windows.Controls.Primitives.DragDeltaEventHandler(GridSplitter_DragDelta_1);

                    // DragCompleted="GridSplitter_DragCompleted_1" MouseMove="GridSplitter_MouseMove_1"

                    if (periode.ReadOnly)
                    {
                        gs.Width = 0;
                    }

                        gridMultislider.Children.Add(gs);
                        gsLast = gs;

                    Grid.SetColumn(gs, (intCount * 2) + 1);
                
                intCount++;
            }

            // laatste gridsplitter onzichtbaar maken
            gsLast.Width = 0;
            //gridMultislider.ColumnDefinitions.Remove(cdLast);
            //gridMultislider.Children.Remove(gsLast);

            gridMultislider.Height = 30;

            //spStackPanel.Children.Add(gridMultislider);

        }



        public static readonly RoutedEvent MultiSliderEvent = EventManager.RegisterRoutedEvent("OnMultiSliderUpdate", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(MultiSlider));
        
        public event RoutedEventHandler OnMultiSliderUpdate
        {
            add { AddHandler(MultiSliderEvent, value); }
            remove { RemoveHandler(MultiSliderEvent, value); }
        }

        protected void OnUpdate()
        {
            //DemoEventArgs args = new DemoEventArgs(MultiSliderEvent,;
            //args.RoutedEvent = MultiSliderEvent;
            //RaiseEvent(args);
        }


        public void LoadVakmanView()
        {
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
            foreach (ColumnDefinition cd in gridMultislider.ColumnDefinitions)
            {
                if (cd.Width.Value.ToString() != "1")
                {
                    cd.Width = new GridLength(Math.Round(cd.Width.Value/100,0)*100, GridUnitType.Star);
                }

            }

        }


        private void GridSplitter_DragStarted_1(object sender, DragStartedEventArgs e)
        {
            IsDragging = true;
            dblTotalChange = 0;
        }

        private double dblTotalChange = 0;

        private void GridSplitter_DragDelta_1(object sender, DragDeltaEventArgs e)
        {
            dblTotalChange += e.HorizontalChange;

            //lblGridsplitterStatus.Content = dblTotalChange.ToString();

            IsDragging = true;
            //IsKnownTargetWidth = false;
            //intCount = 0;
        }

        private void GridSplitter_DragCompleted_1(object sender, DragCompletedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(MultiSliderEvent, sender);
            args.RoutedEvent = MultiSliderEvent;

            Reset();

            RaiseEvent(args);

            IsDragging = false;
            //OnUpdate();
        }

    }
}

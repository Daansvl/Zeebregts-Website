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

namespace MandagenRegistratie.controls.Projecten.Dagview
{
    /// <summary>
    /// Interaction logic for ProjectDagProjectHeader.xaml
    /// </summary>
    public partial class ProjectDagProjectHeader : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "Vakman":
                    btnHeader.Style = this.FindResource("LinkButton") as Style;
                    //btnHeader.Style = Tools.FindVisualParent<PageContainer>(this).FindResource("LinkButton") as Style;
                    dbOriginalRepository dbRepOriginal = new dbOriginalRepository();

                    persoon p = dbRepOriginal.GetContact(vakman.ContactIdOrigineel);
                    btnHeader.Content = p.voornaam + " " + (string.IsNullOrEmpty(p.tussenvoegsel) ? "" : p.tussenvoegsel + " ") + p.achternaam;

                    if (ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId) == vakman.VakmanId)
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
        public static readonly RoutedEvent ProjectDagProjectHeaderEvent = EventManager.RegisterRoutedEvent("OnProjectDagProjectHeaderUpdate", RoutingStrategy.Bubble,
        typeof(RoutedEventHandler), typeof(ProjectDagProjectHeader));

        public event RoutedEventHandler OnProjectDagProjectHeaderUpdate
        {
            add { AddHandler(ProjectDagProjectHeaderEvent, value); }
            remove { RemoveHandler(ProjectDagProjectHeaderEvent, value); }
        }
        #endregion


        private vwVakman vakman;
        public vwVakman Vakman
        {
            get { return vakman; }
            set { SetField(ref vakman, value, "Vakman"); }
        }

        public bool IsOwner { get; set; }
        public bool IsEnabled { get; set; }

        public ProjectDagProjectHeader()
        {
            InitializeComponent();

            //slider.LowerSlider.SelectionStart = 0;
            //slider.LowerSlider.SelectionEnd = 100;

        }

        public void SetBullit()
        {
            if (ApplicationState.GetValue<int>(ApplicationVariables.intDefaultMode) == 1)
            {
                // deleting onzichtbaar
                borderDeleting.Visibility = System.Windows.Visibility.Hidden;

                if (IsOwner && IsEnabled && (bool)cbVakmanSelected.IsChecked)
                {
                    // groen
                    borderAdding.Visibility = System.Windows.Visibility.Visible;
                    borderAdding.Fill = new SolidColorBrush(Colors.Green);
                }
                else if (IsOwner && IsEnabled)
                {
                    // wit
                    borderAdding.Visibility = System.Windows.Visibility.Visible;
                    borderAdding.Fill = new SolidColorBrush(Colors.White);
                }
                else
                {
                    // onzichtbaar
                    borderAdding.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            else // deleting
            {
                // adding onzichtbaar
                borderAdding.Visibility = System.Windows.Visibility.Hidden;

                if (IsOwner && IsEnabled && (bool)cbVakmanSelected.IsChecked)
                {
                    // rood
                    borderDeleting.Visibility = System.Windows.Visibility.Visible;
                    borderDeleting.Fill = new SolidColorBrush(Colors.Red);
                }
                else if (IsOwner && IsEnabled)
                {
                    // wit
                    borderDeleting.Visibility = System.Windows.Visibility.Visible;
                    borderDeleting.Fill = new SolidColorBrush(Colors.White);
                }
                else
                {
                    // onzichtbaar
                    borderDeleting.Visibility = System.Windows.Visibility.Hidden;
                }

            }

        }

        private void btnGoToProject_Click(object sender, RoutedEventArgs e)
        {
            ApplicationState.SetValue(ApplicationVariables.intVakmanId, vakman.VakmanId);
            //ApplicationState.SetValue(ApplicationVariables.objVakman, vakman);

            //ProjectView pv = new ProjectView();
            //pv.Show();

            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
            persoon persoon = dbrepOriginal.GetContact(vakman.ContactIdOrigineel);

            // create the page and load all values
            Vakmannen.Detail.VakmanDetailView vdv = new Vakmannen.Detail.VakmanDetailView(persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam);

            // load the page into the contentcontrol
            MenuControl owner = Tools.FindVisualParent<MenuControl>(this);
            //po.PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            //po.PageOKButtonText = "PROJECT TOEVOEGEN";
            //vdv.Load();

            //vdv.tabControl.SelectedIndex = 1;

            //po.OkClick += po.ToevoegenAanVakman;
            //po.dgProjecten.MouseDoubleClick -= po.dgProjecten_MouseDoubleClick;
            //po.dgProjecten.MouseDoubleClick += po.dgProjecten_MouseDoubleClickForVakman;

            owner.PageGoToPage(vdv);


        }

        private void cbVakmanSelected_Checked(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIds) == null)
                {
                    ApplicationState.SetValue(ApplicationVariables.listSelectedVakmanIds, new List<int>());
                }
                
                ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIds).Add(this.Vakman.VakmanId);


            }
            else
            {

                if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIds) == null)
                {
                    ApplicationState.SetValue(ApplicationVariables.listSelectedVakmanIds, new List<int>());
                }
                else
                {
                    ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIds).Remove(this.Vakman.VakmanId);
                }
            }

            SetBullit();
        }


    }
}

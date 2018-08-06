using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace WPFcontrolLib
{
    /// <summary>
    /// Interaction logic for ProjectFolderExplorer.xaml
    /// </summary>
    public partial class ProjectFolderExplorer : Window
    {
        public ObservableCollection<ProjectFolderItem> InputList;
        public ProjectFolderExplorer(ObservableCollection<ProjectFolderItem> input)
        {
            InitializeComponent();
            InputList = input;
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            projectBox.ItemsSource = InputList;
        }

        private void openfolderButton_Click(object sender, RoutedEventArgs e)
        {
            var curItem = ((ListBoxItem)projectBox.ContainerFromElement((Button)sender)).Content as ProjectFolderItem;
            if (curItem != null)
            {
                GoToFolder(curItem);
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            searchbox.Clear();
        }
        private void GoToFolder(ProjectFolderItem curItem)
        {
           
            //MessageBox.Show("ProjNum is " + curItem.ProjNum );


            string myDocspath = @"\\OFFICE-SERVER\Bestanden\Bestanden\ProjectFolders (Module 1 Test)\" + curItem.ProjNum;
            if (Directory.Exists(myDocspath))
            {
                string windir = Environment.GetEnvironmentVariable("WINDIR");
                System.Diagnostics.Process prc = new System.Diagnostics.Process();
                prc.StartInfo.FileName = windir + @"\explorer.exe";
                prc.StartInfo.Arguments = myDocspath;
                prc.Start();
            }
            else
            {
                var result = MessageBox.Show("Directory bestaat nog niet. Wilt u deze nu aanmaken?", "Map Bestaat Niet", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    Directory.CreateDirectory(myDocspath);
                    string windir = Environment.GetEnvironmentVariable("WINDIR");
                    System.Diagnostics.Process prc = new System.Diagnostics.Process();
                    prc.StartInfo.FileName = windir + @"\explorer.exe";
                    prc.StartInfo.Arguments = myDocspath;
                    prc.Start();
                }

            }
        }
        private void FilterList()
        {
            var colViewSrc = CollectionViewSource.GetDefaultView(InputList);
            var srchTrm = searchbox.Text.ToLower();
            colViewSrc.Filter = pfi => ((ProjectFolderItem)pfi).ProjNum.ToString().ToLower().Contains(srchTrm) ||
                                       ((ProjectFolderItem)pfi).ProjName.ToLower().Contains(srchTrm) ||
                                       ((ProjectFolderItem)pfi).ProjPlaats.ToLower().Contains(srchTrm);
            projectBox.ItemsSource = colViewSrc;
        }

        
        private void searchbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterList();
        }

        private void projectBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            { 
                var curItem =   projectBox.SelectedItem as ProjectFolderItem;

                if (curItem != null)
                {
                    GoToFolder(curItem);
                }
            }
        }

        private void searchbox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                var curItem = projectBox.SelectedItem as ProjectFolderItem;
                if (curItem != null)
                {
                    GoToFolder(curItem);
                }            
            }
            else if(e.Key == Key.Down || e.Key == Key.Up)
            {
                //ugly ugly hax
                var txt = searchbox.Text;
                searchbox.Text = "0";
                searchbox.Text = txt;
                //
                projectBox.Focus();
            }
        }

        
    }
    public class ProjectFolderItem : INotifyPropertyChanged
    {
        private int projnum;
        public int ProjNum
        {
            get { return projnum; }
            set { projnum = value; OnPropertyChanged("ProjNum"); }
        }

        private string projname;
        public string ProjName
        {
            get { return projname; }
            set { projname = value; OnPropertyChanged("ProjName"); }
        }

        private string projplaats;
        public string ProjPlaats
        {
            get { return projplaats; }
            set { projplaats = value; OnPropertyChanged("ProjPlaats"); }
        }

        private string projomsch;
        public string ProjOmsch
        {
            get { return projomsch; }
            set { projomsch = value; OnPropertyChanged("ProjOmsch"); }
        }

        private string projopdr;
        public string ProjOpdr
        {
            get {return projopdr;}
            set {projopdr =value; OnPropertyChanged("ProjOpdr");}
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
    public class FilterListVisConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value == true ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

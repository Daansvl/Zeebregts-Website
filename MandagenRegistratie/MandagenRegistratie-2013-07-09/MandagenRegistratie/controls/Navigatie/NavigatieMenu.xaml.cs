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
using MandagenRegistratieLogic;
using MandagenRegistratie.controls.Vakmannen.Dagview;

namespace MandagenRegistratie.controls.Navigatie
{
    /// <summary>
    /// Interaction logic for NavigatieMenu.xaml
    /// </summary>
    public partial class NavigatieMenu : UserControl
    {
        public NavigatieMenu()
        {
            InitializeComponent();
        }

        private void btnTerug_Click(object sender, RoutedEventArgs e)
        {
            //List<Control> listNavigationHistory = ApplicationState.GetValue<List<Control>>(ApplicationVariables.listNavigationHistory);

            //if (ApplicationState.GetValue<List<Control>>(ApplicationVariables.listNavigationHistory).Count > 1)
            //{
            //    // load the previous page into the contentcontrol
            //    Control control = ApplicationState.GetValue<List<Control>>(ApplicationVariables.listNavigationHistory)[1];

            //    ((PageContainer)((StackPanel)((ContentControl)Parent).Parent).Parent).LoadControl(control);

            //    // verwijder het huidige item uit de navigatie
            //    ApplicationState.GetValue<List<Control>>(ApplicationVariables.listNavigationHistory).RemoveAt(0);

            //    // reload de navigatie
            //    ((PageContainer)((StackPanel)((ContentControl)Parent).Parent).Parent).LoadNavigatie();


            //}
        }

        private void btnDagview_Click(object sender, RoutedEventArgs e)
        {
            //VakmanDagView vakmanDagView = new VakmanDagView();
            //vakmanDagView.dtBegintijd = ApplicationState.GetValue<DateTime>("dtSelectedDay");
            //vakmanDagView.LoadVakmanDagView(true);
            //vakmanDagView.LoadWeekInfo();

            ////vakmanDagView.Vakman;
            ////vakmanDagViewProjectUren.Load()
            ////ccPageContainer.Content = vakmanDagView;

        }
    }
}

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
using System.Windows.Shapes;
//using MandagenRegistratie.controls.Interfaces;
using ZeebregtsLogic;
using MandagenRegistratie.controls.Navigatie;


namespace MandagenRegistratie.controls
{
    /// <summary>
    /// Interaction logic for PageContainer.xaml
    /// </summary>
    public partial class PageContainer : Window
    {
        public PageContainer()
        {
            InitializeComponent();
        }


        /// <summary>
        /// default load main menu
        /// </summary>
        public void LoadControl()
        {
            // default projectleiderId = 1
            ApplicationState.SetValue(ApplicationVariables.intProjectleider, 1);

            // default vakmanId = 1
            ApplicationState.SetValue(ApplicationVariables.intVakmanId, 1);

            // initialize the navigation history
            ApplicationState.SetValue(ApplicationVariables.listNavigationHistory, new List<object>());

            // default mainmenu laden
            MainMenu mainmenu = new MainMenu();
            ccPageContainer.Content = mainmenu;
            wfhTest.Child = null;

            // add the item to the navigation history
            ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Insert(0, mainmenu);

        }

        /// <summary>
        /// load the specified control
        /// </summary>
        /// <param name="control"></param>
        public void LoadControl(Control control)
        {

            // add the item to the navigation history
            ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Insert(0, control);
            
            // load the control
            ccPageContainer.Content = control;
            wfhTest.Child = null;

            // bind de functie aan het event om te kunnen bubbelen
            ((MenuControl)control).ReloadMenu += new RoutedEventHandler(ReloadMenuHandler);



            // maak menu zichtbaar
            gridMenu.Visibility = System.Windows.Visibility.Visible;

            // laad het menu
            LoadMenu(((MenuControl)control));

        }

        /// <summary>
        /// load the specified control
        /// </summary>
        /// <param name="control"></param>
        public void LoadControl(System.Windows.Forms.Control control)
        {

            // add the item to the navigation history
            ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Insert(0, control);

            // load the control
            wfhTest.Child = control;
            ccPageContainer.Content = null;

            // HACK: geef de WindowsFormsHost control mee aan het control
            ((WFControl)control).HHH = wfhTest;

            // bind de functie aan het event om te kunnen bubbelen
            //((WFControl)control).ReloadMenu += new RoutedEventHandler(ReloadMenuHandler);
            // ERROR: bubblen niet mogelijk in Windows Forms


            // maak menu zichtbaar
            gridMenu.Visibility = System.Windows.Visibility.Visible;

            // laad het menu
            LoadMenu(((WFControl)control));

        }

        /// <summary>
        /// deze functie laadt het menu en set de pagina-specifieke waardes
        /// </summary>
        /// <param name="menucontrol"></param>
        public void LoadMenu(MenuControl menucontrol)
        {

            lblPageTitle.Content = menucontrol.PageTitle;
            btnGereed.Content = menucontrol.GereedButtonText;
            btnTerug.Content = menucontrol.TerugButtonText;

        }

        /// <summary>
        /// deze functie laadt het menu en set de pagina-specifieke waardes
        /// </summary>
        /// <param name="menucontrol"></param>
        public void LoadMenu(WFControl menucontrol)
        {

            lblPageTitle.Content = menucontrol.PageTitle;
            btnGereed.Content = menucontrol.GereedButtonText;
            btnTerug.Content = menucontrol.TerugButtonText;

        }


        /// <summary>
        /// load the specified control
        /// </summary>
        /// <param name="control"></param>
        public void LoadControlGoingback(Control control)
        {

            // verwijder het huidige item uit de navigatie
            ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).RemoveAt(0);
           
            // load the control
            ccPageContainer.Content = control;
            wfhTest.Child = null;

            // laad het menu
            if (control.GetType().BaseType == typeof(MenuControl))
            {
                LoadMenu(((MenuControl)control));
            }
        }

        /// <summary>
        /// load the specified control
        /// </summary>
        /// <param name="control"></param>
        public void LoadControlGoingback(System.Windows.Forms.Control control)
        {

            // verwijder het huidige item uit de navigatie
            ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).RemoveAt(0);

            // load the control
            wfhTest.Child = control;
            ccPageContainer.Content = null;

            // laad het menu
            if (control.GetType().BaseType == typeof(WFControl))
            {
                LoadMenu(((WFControl)control));
            }
        }

        private void ReloadMenuHandler(object sender, RoutedEventArgs e)
        {
            btnTerug.Content = "TERUG NAAR " + ((MenuControl)sender).TerugButtonText;
            btnGereed.Content = ((MenuControl)sender).GereedButtonText;
        }

        private void btnTerug_Click(object sender, RoutedEventArgs e)
        {
            List<object> listNavigationHistory = ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory);

            if (ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Count > 1)
            {
                // load the previous page into the contentcontrol
                object control = ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory)[1];

                if (control.GetType().BaseType == typeof(WFControl))
                {
                    LoadControlGoingback((WFControl)control);
                }
                else if (control.GetType().BaseType == typeof(MenuControl))
                {
                    LoadControlGoingback((MenuControl)control);
                }
                else
                {
                    LoadControlGoingback((Control)control);
                }
            }

        }

        private void btnGereed_Click(object sender, RoutedEventArgs e)
        {
            if (ccPageContainer.Content != null)
            {
                ((MenuControl)ccPageContainer.Content).OK();
            }
            else if(wfhTest.Child != null)
            {
                ((WFControl)wfhTest.Child).OK();
            }
        }

    }
}

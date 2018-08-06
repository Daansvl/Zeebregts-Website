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
using MandagenRegistratieLogic;
//using ZeebregtsSysteem.controls.Navigatie;


namespace ZeebregtsSysteem.controls
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
            ApplicationState.SetValue(ApplicationVariables.listNavigationHistory, new List<Control>());

            // default mainmenu laden
            //MainMenu mainmenu = new MainMenu();
            //ccPageContainer.Content = mainmenu;

            // add the item to the navigation history
            //ApplicationState.GetValue<List<Control>>(ApplicationVariables.listNavigationHistory).Insert(0, mainmenu);

        }

        /// <summary>
        /// load the specified control
        /// </summary>
        /// <param name="control"></param>
        public void LoadControl(Control control)
        {

            // add the item to the navigation history
            ApplicationState.GetValue<List<Control>>(ApplicationVariables.listNavigationHistory).Insert(0, control);
            
            // load the control
            ccPageContainer.Content = control;

        }

        /// <summary>
        /// load the specified control
        /// </summary>
        /// <param name="control"></param>
        public void LoadControlGoingback(Control control)
        {

            // verwijder het huidige item uit de navigatie
            ApplicationState.GetValue<List<Control>>(ApplicationVariables.listNavigationHistory).RemoveAt(0);

           
            // load the control
            ccPageContainer.Content = control;

        }





        ///// <summary>
        ///// load the specified control
        ///// </summary>
        ///// <param name="control"></param>
        //public void LoadNavigatie()
        //{
        //    if (ApplicationState.GetValue<List<Control>>(ApplicationVariables.listNavigationHistory).Count > 1)
        //    {
        //        NavigatieMenu navmenu = new NavigatieMenu();
        //        ccNavigatieMenuContainer.Content = navmenu;
        //    }
        //    else
        //    {
        //        ccNavigatieMenuContainer.Content = null;
        //    }
        //}

    }
}

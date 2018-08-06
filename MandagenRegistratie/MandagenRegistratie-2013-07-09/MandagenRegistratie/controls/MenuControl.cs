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
using System.ComponentModel;

namespace MandagenRegistratie.controls
{

    public delegate void FunctieGereed();

    public class MenuControl : UserControl
    {
        public event FunctieGereed Gereed; // de gereed functie

        // This defines the custom event
        public static readonly RoutedEvent ReloadMenuEvent = EventManager.RegisterRoutedEvent(
            "ReloadMenu", // Event name
            RoutingStrategy.Bubble, // Bubble means the event will bubble up through the tree
            typeof(RoutedEventHandler), // The event type
            typeof(MenuControl)); // Belongs to ChildControlBase

        // Allows add and remove of event handlers to handle the custom event
        public event RoutedEventHandler ReloadMenu
        {
            add { AddHandler(ReloadMenuEvent, value); }
            remove { RemoveHandler(ReloadMenuEvent, value); }
        }

        // DEFAULT VALUES
        public Visibility GereedButtonVisibility = Visibility.Visible;
        public string PageTitle = "Bel de programmeur om ook deze pagina van een leuke titel te voorzien :-) 0646408407";
        public string GereedButtonText = "OK";
        public string TerugButtonText = "TERUG";

        public MenuControl()
        {

        }

        /// <summary>
        /// Kan aangeropen worden om de OK-functie van de control aan te roepen
        /// triggert het 
        /// </summary>
        public void OK()
        {
           Gereed();
        }


        /// <summary>
        /// Bubbles up the tree, and fires the reload menu event on the window
        /// </summary>
        public void ReloadMenuFunction()
        {
            // hier uitvoeren, maar kan ook op het control zelf
            // This actually raises the custom event
            var newEventArgs = new RoutedEventArgs(ReloadMenuEvent);
            RaiseEvent(newEventArgs);


        }

        /// <summary>
        /// Ga naar deze pagina
        /// </summary>
        /// <param name="control"></param>
        public void GoToPage(Control control)
        {
            // load the page into the contentcontrol
            ((PageContainer)((StackPanel)((ContentControl)Parent).Parent).Parent).LoadControl(control);
        }

        /// <summary>
        /// Ga naar deze pagina
        /// </summary>
        /// <param name="control"></param>
        public void GoToPage(System.Windows.Forms.Control control)
        {
            // load the page into the contentcontrol
            ((PageContainer)((StackPanel)((ContentControl)Parent).Parent).Parent).LoadControl(control);
        }


    }
}

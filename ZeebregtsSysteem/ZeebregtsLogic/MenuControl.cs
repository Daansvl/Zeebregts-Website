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

namespace ZeebregtsLogic
{


    public class MenuControl : UserControl
    {
        public delegate void FunctieOK();
        public event FunctieOK OkClick; // de gereed functie

        public delegate void FunctieCustomActie();
        public event FunctieCustomActie CustomActieClick; // de gereed functie

        public delegate void FunctieLoad();
        public event FunctieLoad Reloaded;

        public delegate void FunctieCommunicationChannel(string sender, string message);
        public event FunctieCommunicationChannel ChannelMessage; 

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
        public Visibility PageGereedButtonVisibility = Visibility.Visible;
        public string PageTitle = "";
        public string PageSubtitle = "";
        public string PageOKButtonText = "OK";
        public string PageBackButtonText = "TERUG";

        public MenuControl()
        {
        }

        /// <summary>
        /// Kan aangeroepen worden om de OK-functie van de control aan te roepen
        /// triggert het 
        /// </summary>
        public void PageOK()
        {
           OkClick();
        }

        /// <summary>
        /// Kan aangeroepen worden om de OK-functie van de control aan te roepen
        /// triggert het 
        /// </summary>
        public void PageCustomActie()
        {
            CustomActieClick();
        }

        /// <summary>
        /// Kan aangeroepen worden om de Channel message-functie van de control aan te roepen
        /// </summary>
        public void PageChannelMessage(string sender, string message)
        {

            //MessageBox.Show("PageChannelMessage(" + sender + ", " + message + ")");

            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).SendChannelMessage(sender, message);
            Tools.FindVisualParent<PageContainer>(this).SendChannelMessage(sender, message);

            // ChannelMessage(sender, message);
        }

        /// <summary>
        /// Kan aangeropen worden om de Load-functie van de control aan te roepen
        /// triggert het 
        /// </summary>
        public void PageReloaded()
        {
            Reloaded();
        }

        public void PageGoBack()
        {
            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).GoBack();
            Tools.FindVisualParent<PageContainer>(this).GoBack();

        }

        public void PageGoBack(MenuControl otherPage)
        {
            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).GoBack();
            //Tools.FindVisualParent<PageContainer>(this).GoBack();

            Tools.FindVisualParent<PageContainer>(this).LoadControl(otherPage, true);

        }


        /// <summary>
        /// Bubbles up the tree, and fires the reload menu event on the window
        /// </summary>
        public void PageReloadMenuFunction()
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
        public void PageGoToPage(Control control)
        {
            // load the page into the contentcontrol
            Tools.FindVisualParent<PageContainer>(this).LoadControl(control, false);
        }

        /// <summary>
        /// Ga naar deze pagina
        /// </summary>
        /// <param name="control"></param>
        public void PageGoToPage(System.Windows.Forms.Control control)
        {
            // load the page into the contentcontrol
            //((PageContainer)((DockPanel)((ContentControl)Parent).Parent).Parent).LoadControl(control);
            Tools.FindVisualParent<PageContainer>(this).LoadControl(control);
        }



        public void TriggerChannelMessage(string sender, string message)
        {
            ChannelMessage(sender, message);
        }
    }
}

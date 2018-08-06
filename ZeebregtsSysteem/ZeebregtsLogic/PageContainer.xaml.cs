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
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.ComponentModel;
using System.Threading;
// using WindowsFormsIntegration;
using System.Windows.Forms.Integration;

namespace ZeebregtsLogic
{
    /// <summary>
    /// Interaction logic for PageContainer.xaml
    /// </summary>
    public partial class PageContainer : Window
    {
        BackgroundWorker backgroundWorker1 = new BackgroundWorker();

        public void SetLabelSubtitleGegevens(bool blnGereedButton)
        {
            lblPageSubtitle.Content = lblPageSubtitle.Content.ToString().Replace("Uren", "Gegevens");

            if (blnGereedButton)
            {
                btnGereed.Visibility = Visibility.Visible;
            }
            else
            {
                btnGereed.Visibility = Visibility.Hidden;
            }
        }

        public void SetLabelSubtitlePlanning(bool blnGereedButton)
        {
            lblPageSubtitle.Content = lblPageSubtitle.Content.ToString().Replace("Gegevens", "Uren");

            if (blnGereedButton)
            {
                btnGereed.Visibility = Visibility.Hidden;
            }
            else
            {
                btnGereed.Visibility = Visibility.Hidden;
            }
        }

        public void SetPageTitle(string title)
        {
            lblPageTitle.Content = title;
        }

        public void SetPageSubTitle(string subtitle)
        {
            lblPageSubtitle.Content = subtitle;
        }

        public void SetPageGereedButtonText(string text)
        {
            btnGereed.Content = text;
        }

        public void SetPageTerugButtonText(string text)
        {
            btnTerug.Content = text;
        }

        public void SetVisibilityGereedButton(Visibility visibility)
        {
            btnGereed.Visibility = visibility;
        }

        protected void CallWithTimeout(Action action, int timeoutMilliseconds)
        {

            Thread threadToKill = null;
            Action wrappedAction = () =>
            {
                threadToKill = Thread.CurrentThread;
                action();
            };

            IAsyncResult result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
            {
                wrappedAction.EndInvoke(result);
            }
            else
            {
                threadToKill.Abort();
                throw new TimeoutException();
            }
        }

        public PageContainer(string endpoint)
        {
            InitializeComponent();
            //#if DEBUG
            //#else

            try
            {


                //if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
                //{
                //    System.Windows.MessageBox.Show("Het programma is al geopend. Dit proces wordt nu afgesloten.");
                //    System.Diagnostics.Process.GetCurrentProcess().Kill();
                //}

                backgroundWorker1.DoWork += backgroundWorker1_DoWork;
                backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
                backgroundWorker1.RunWorkerAsync(endpoint);

                //pcWindow.Title = "TESTING";

                //// find the currently logged in user
                //string strWindowsUser;
                //strWindowsUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                //string[] strWindowsUserArray = strWindowsUser.Split('\\');
                //string strCleanUsername = strWindowsUserArray[strWindowsUserArray.Length - 1];

                //pcWindow.Title = ConfigurationManager.AppSettings["WindowTitle"].ToString() + " || Welkom " + strCleanUsername; // +" Status ZeebregtsChannelService: Actief";

                //endpoint = strCleanUsername;

                //// save endpoint for later reference
                //ApplicationState.SetValue(ApplicationVariables.strEndpoint, endpoint);

                //string result = LoadCommunicationChannel(endpoint);

                //if (!string.IsNullOrEmpty(result))
                //{
                //    // foutje opgetreden, waarschijnlijk geen connectie met de server
                //    //System.Windows.Forms.MessageBox.Show(result,
                //    //"Belangrijke melding omtrent ZeebregtsChannelService",
                //    //MessageBoxButtons.OK,
                //    //MessageBoxIcon.Exclamation,
                //    //MessageBoxDefaultButton.Button1);

                //    ApplicationState.SetValue(ApplicationVariables.blnChatStatus, false);

                //    pcWindow.Title += " || Niet verbonden met ZeebregtsChannelService";

                //    //"Niet actief: kon niet verbinden met " + endpoint + ". Controleer poort :8089"
                //    //pcWindow.Title = ConfigurationManager.AppSettings["WindowTitle"].ToString() + " Status ZeebregtsChannelService: Niet actief";

                //}
                //else if (Global.useChatFunction)
                //{
                //    ApplicationState.SetValue(ApplicationVariables.blnChatStatus, true);
                //    pcWindow.Title += " || Verbonden met ZeebregtsChannelService";
                //}
                //else
                //{
                //    ApplicationState.SetValue(ApplicationVariables.blnChatStatus, false);
                //    pcWindow.Title += " || Niet verbonden met ZeebregtsChannelService";
                //}
                ////#endif

            }
            catch (Exception ex)
            {

                pcWindow.Title += " || Niet verbonden met ZeebregtsChannelService";

                System.Windows.Forms.MessageBox.Show(ex.Message,
        "Belangrijke melding omtrent ZeebregtsChannelService",
        MessageBoxButtons.OK,
        MessageBoxIcon.Exclamation,
        MessageBoxDefaultButton.Button1);
            }

        }

        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //pcWindow.Title = "COMPLETED";
        }

        void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                    string endpoint = e.Argument.ToString(); // e.Argument.ToString();

                    // find the currently logged in user
                    string strWindowsUser;

                    strWindowsUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                    string[] strWindowsUserArray = strWindowsUser.Split('\\');
                    string strCleanUsername = strWindowsUserArray[strWindowsUserArray.Length - 1];

                    string title = string.Empty;

                    title = ConfigurationManager.AppSettings["WindowTitle"].ToString() + " || Welkom " + strCleanUsername;

                    Dispatcher.Invoke((MethodInvoker)delegate
                    {
                        pcWindow.Title = title + " || Verbinden ..."; // +" Status ZeebregtsChannelService: Actief";
                    });



                    endpoint = strCleanUsername;

                    // save endpoint for later reference
                    ApplicationState.SetValue(ApplicationVariables.strEndpoint, endpoint);

                //string result = string.Empty;

                //CallWithTimeout(LoadCommunicationChannel, 5000);

                    Dispatcher.Invoke((MethodInvoker)delegate
         {

                  Logging log = new Logging();

            try
             {

                 bool result = LoadCommunicationChannel();


                 if (Global.useChatFunction)
                 {
                     if (result)
                     {
                         pcWindow.Title = title + " || Verbonden";

                         log.Log("Status: Verbonden");
                     }
                     else
                     {

                         pcWindow.Title = title + " || Niet verbonden";

                         log.Log("Status: Niet verbonden --> check firewall poort 8089 of meerdere geopende Remote Desktop Sessies");

                     }
                 }
                 else
                 {
                     pcWindow.Title = title; // + " || Niet verbonden";
                     log.Log("Status: Niet verbonden --> useChatFunction=false");
                 }
             }
             catch
             {
                 pcWindow.Title = title + " || Niet verbonden";
                 log.Log("Status: Niet verbonden --> error #1");
             }

         });
    //                if (!string.IsNullOrEmpty(result))
    //                {
    //                    // foutje opgetreden, waarschijnlijk geen connectie met de server
    //                    //System.Windows.Forms.MessageBox.Show(result,
    //                    //"Belangrijke melding omtrent ZeebregtsChannelService",
    //                    //MessageBoxButtons.OK,
    //                    //MessageBoxIcon.Exclamation,
    //                    //MessageBoxDefaultButton.Button1);

    //                    ApplicationState.SetValue(ApplicationVariables.blnChatStatus, false);

    //                    Dispatcher.Invoke((MethodInvoker)delegate
    //           {
    //               pcWindow.Title = title + " || Niet verbonden met ZeebregtsChannelService";
    //           });

    //                    //"Niet actief: kon niet verbinden met " + endpoint + ". Controleer poort :8089"
    //                    //pcWindow.Title = ConfigurationManager.AppSettings["WindowTitle"].ToString() + " Status ZeebregtsChannelService: Niet actief";

    //                }
    //                else if (Global.useChatFunction)
    //                {
    //                    ApplicationState.SetValue(ApplicationVariables.blnChatStatus, true);
    //                    Dispatcher.Invoke((MethodInvoker)delegate
    //         {
    //             pcWindow.Title = title + " || Verbonden met ZeebregtsChannelService";
    //         });
    //                }
    //                else
    //                {
    //                    ApplicationState.SetValue(ApplicationVariables.blnChatStatus, false);
    //                    Dispatcher.Invoke((MethodInvoker)delegate
    //{
    //    pcWindow.Title = title + " || Niet verbonden met ZeebregtsChannelService";
    //});
    //                }

                    //#endif
                
            }
            catch (Exception ex)
            {
                try
                {
                    Dispatcher.Invoke((MethodInvoker)delegate
{
    pcWindow.Title += " || Niet verbonden met ZeebregtsChannelService";
});
                }
                catch { }

                System.Windows.Forms.MessageBox.Show(ex.Message,
        "Belangrijke melding omtrent ZeebregtsChannelService",
        MessageBoxButtons.OK,
        MessageBoxIcon.Exclamation,
        MessageBoxDefaultButton.Button1);
            }

        }

        /// <summary>
        /// [Deprecated] default load main menu
        /// </summary>
        /// [Deprecated]
        //public void LoadControl()
        //{
        //    // default projectleiderId = 1
        //    ApplicationState.SetValue(ApplicationVariables.intProjectleider, 1);

        //    // default vakmanId = 1
        //    ApplicationState.SetValue(ApplicationVariables.intVakmanId, 1);

        //    // initialize the navigation history
        //    ApplicationState.SetValue(ApplicationVariables.listNavigationHistory, new List<object>());

        //    // default mainmenu laden
        //    MainMenu mainmenu = new MainMenu();
        //    ccPageContainer.Content = mainmenu;
        //    wfhTest.Child = null;

        //    // add the item to the navigation history
        //    ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Insert(0, mainmenu);

        //}

        /// <summary>
        /// load the specified control
        /// </summary>
        /// <param name="control"></param>
        public void LoadControlDefault(System.Windows.Controls.Control control, bool isDefault)
        {

            // default projectleiderId = 1
            //ApplicationState.SetValue(ApplicationVariables.intProjectleider, 1);

            // default vakmanId = 1
            //ApplicationState.SetValue(ApplicationVariables.intVakmanId, 1);

            // initialize the navigation history
            ApplicationState.SetValue(ApplicationVariables.listNavigationHistory, new List<object>());

            // add the item to the navigation history
            ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Add(control);

            // load the control
            ccPageContainer.Content = control;
            wfhTest.Child = null;

            // bind de functie aan het event om te kunnen bubbelen
            //((MenuControl)control).ReloadMenu += new RoutedEventHandler(ReloadMenuHandler);


            ccPageContainer.Visibility = System.Windows.Visibility.Visible;

            // maak menu onzichtbaar
            gridMenu.Visibility = System.Windows.Visibility.Collapsed;

            // laad het menu
            LoadMenu(control);

        }


        /// <summary>
        /// load the specified control
        /// </summary>
        /// <param name="control"></param>
        public void LoadControl(System.Windows.Controls.Control control, bool blnReplaceCurrentPage)
        {
            //
            control.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

            if (blnReplaceCurrentPage)
            {
                int count = ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Count;
                ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).RemoveAt(count-1);
            }

            // add the item to the navigation history
            ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Add(control);
            
            // load the control
            ccPageContainer.Content = control;
            wfhTest.Child = null;

            // bind de functie aan het event om te kunnen bubbelen
            ((MenuControl)control).ReloadMenu += new RoutedEventHandler(ReloadMenuHandler);



            // maak menu zichtbaar
            gridMenu.Visibility = System.Windows.Visibility.Visible;
            ccPageContainer.Visibility = System.Windows.Visibility.Visible;

            wfhTest.Visibility = System.Windows.Visibility.Collapsed;
            svTest.Visibility = System.Windows.Visibility.Collapsed;

            // laad het menu
            LoadMenu(((MenuControl)control));

        }

        public System.Windows.Controls.Control inloggenalscontrol; 

        /// <summary>
        /// load the specified control
        /// </summary>
        /// <param name="control"></param>
        public void LoadInloggenAls()
        {
            //
            inloggenalscontrol.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;


            // add the item to the navigation history
            ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Add(inloggenalscontrol);

            // load the control
            ccPageContainer.Content = inloggenalscontrol;
            wfhTest.Child = null;

            // bind de functie aan het event om te kunnen bubbelen
            ((MenuControl)inloggenalscontrol).ReloadMenu += new RoutedEventHandler(ReloadMenuHandler);



            // maak menu zichtbaar
            gridMenu.Visibility = System.Windows.Visibility.Visible;
            ccPageContainer.Visibility = System.Windows.Visibility.Visible;

            wfhTest.Visibility = System.Windows.Visibility.Collapsed;

            // laad het menu
            LoadMenu(((MenuControl)inloggenalscontrol));

        }



        /// <summary>
        /// load the specified control
        /// </summary>
        /// <param name="control"></param>
        public void LoadControl(System.Windows.Forms.Control control)
        {
            //
            //control.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

            // add the item to the navigation history
            ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Add(control);

            //wfhTest.Width = control.Width;
            //wfhTest.Height = control.Height;


            // load the control
            wfhTest.Child = control;
            
            ccPageContainer.Visibility = System.Windows.Visibility.Collapsed;
            
            ccPageContainer.Content = null;

            // HACK: geef de WindowsFormsHost control mee aan het control
            ((WFControl)control).HHH = wfhTest;

            //((WFControl)control).HHH.Background = new SolidColorBrush(Colors.Bisque);

            //((WFControl)control).HHH.Height = 791;
            //((WFControl)control).HHH.Width = 1397;


            ((WFControl)control).HHH.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            ((WFControl)control).HHH.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            

            //((WFControl)control).HHH.Height = control.Height;
            //((WFControl)control).HHH.Width = control.Width;

            ((WFControl)control).HHH.Visibility = System.Windows.Visibility.Visible;
            svTest.Visibility = System.Windows.Visibility.Visible;

            // bind de functie aan het event om te kunnen bubbelen
            //((WFControl)control).ReloadMenu += new RoutedEventHandler(ReloadMenuHandler);
            // ERROR: bubblen niet mogelijk in Windows Forms

            //((WFControl)control).HHH.SizeChanged += pcWindow_SizeChanged_1;

            // maak menu zichtbaar
            gridMenu.Visibility = System.Windows.Visibility.Visible;

            // laad het menu
            LoadMenu(((WFControl)control));

        }

        void PageContainer_SizeChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Default, deze functie laadt het menu en set de pagina-specifieke waardes
        /// </summary>
        /// <param name="menucontrol"></param>
        public void LoadMenu(System.Windows.Controls.Control control)
        {

            lblPageTitle.Content = "Zeebregts Systeem Hoofdmenu";
            lblPageSubtitle.Content = "Zeebregts Systeem Hoofdmenu";

            // maak menu onzichtbaar in het hoofdmenu
            gridMenu.Visibility = System.Windows.Visibility.Collapsed;
            
            // datum van vandaag tonen
            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, Tools.CalculateWeekstart(DateTime.Now));

            SetBreadcrumb();
        }


        private void pcWindow_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            //System.Windows.Forms.Integration.WindowsFormsHost hhh = (System.Windows.Forms.Integration.WindowsFormsHost)sender;
            CheckLayout();
        }

        public void CheckLayout()
        {

            //System.Windows.MessageBox.Show("Height: " + svTest.ActualHeight.ToString() + " Width: " + svTest.ActualWidth.ToString());

            //wfhTest.Height = pcWindow.ActualHeight - 132;
            int margin = 24;
            //wfhTest.MinHeight = svTest.ActualHeight < margin ? svTest.ActualHeight : svTest.ActualHeight - margin;
            //wfhTest.Width = svTest.ActualWidth < margin ? svTest.ActualWidth : svTest.ActualWidth - margin;

            //hhh.Width = pcWindow.ActualWidth;

            //if (Window.GetWindow(this) != null)
            //{
            //   System.Windows.MessageBox.Show(Window.GetWindow(this).ActualHeight.ToString());
            //    //((WFControl)control).HHH.Height = Window.GetWindow(this).ActualHeight - 172;
            //    //((WFControl)control).HHH.Width = Window.GetWindow(this).ActualWidth;
            //}
        }

        /// <summary>
        /// deze functie laadt het menu en set de pagina-specifieke waardes
        /// </summary>
        /// <param name="menucontrol"></param>
        public void LoadMenu(MenuControl menucontrol)
        {

            lblPageTitle.Content = menucontrol.PageTitle;
            lblPageSubtitle.Content = menucontrol.PageSubtitle.ToStringTrimmed();

            btnGereed.Content = menucontrol.PageOKButtonText;
            btnTerug.Content = menucontrol.PageBackButtonText;

            btnGereed.Visibility = menucontrol.PageGereedButtonVisibility;

            try
            {
                //menucontrol.PageReloaded(); LET OP: Juraci
            }
            catch {
                // testen hoevaak we hierin komen
                //System.Windows.MessageBox.Show("als dit vaker gebeurt, dan weghalen");
            }

            SetBreadcrumb();
        }

        /// <summary>
        /// deze functie laadt het menu en set de pagina-specifieke waardes
        /// </summary>
        /// <param name="menucontrol"></param>
        public void LoadMenu(WFControl menucontrol)
        {

            lblPageTitle.Content = menucontrol.PageTitle;
            lblPageSubtitle.Content = menucontrol.PageSubtitle.ToStringTrimmed();
            
            btnGereed.Content = menucontrol.PageGereedButtonText;
            btnTerug.Content = menucontrol.PageTerugButtonText;

            btnGereed.Visibility = menucontrol.PageGereedButtonVisibility;

            SetBreadcrumb();
        }




        public void GoBack2(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;

            int counter = ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Count;

            int aantalterug = counter - (Convert.ToInt32(button.Tag) + 1);

            for (int i = 0; i < aantalterug; i++)
            {

                // nog niet de laatste, dus gewoon verwijderen zonder tijd te verliezen
                if (i < aantalterug - 1)
                {
                    // verwijder het huidige item uit de navigatie
                    // eigenlijk GoBack() zonder het daadwerkelijk uit te voeren
                    int count = ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Count;
                    ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).RemoveAt(count-1);

                }
                else
                {
                    GoBack();
                }

            }
        }

        public int maxLengthCrumbs = 4;

        public void GoToInloggen(object sender, RoutedEventArgs e)
        {
            LoadInloggenAls();
        }


        /// <summary>
        /// Set the text in the breadcrumb
        /// </summary>
        public void SetBreadcrumb()
        {
            spBreadcrumb.Children.Clear();

            List<object> listCrumbs = ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory);

            int startPunt = listCrumbs.Count - maxLengthCrumbs;


            // experiment shortcut inloggen als..
            if (ApplicationState.GetValue<bool>(ApplicationVariables.blnIsAdmin))
            {

                System.Windows.Controls.Button inloggenals = new System.Windows.Controls.Button();
                inloggenals.Style = this.FindResource("LinkButton") as Style;
                inloggenals.Tag = (-1).ToString();
                inloggenals.Click += new RoutedEventHandler(GoToInloggen);
                inloggenals.Content = "Inloggen als...";
                inloggenals.ToolTip = "snel naar 'inloggen als...' scherm";
                inloggenals.Margin = new Thickness(0, 5, 0, 0);

                spBreadcrumb.Children.Add(inloggenals);

                System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                label.Content = " | ";
                spBreadcrumb.Children.Add(label);

            }











            System.Windows.Controls.Button homebutton = new System.Windows.Controls.Button();
            homebutton.Style = this.FindResource("LinkButton") as Style;
            homebutton.Tag = (0).ToString();
            homebutton.Click += new RoutedEventHandler(GoBack2);
            homebutton.Content = "Home";
            homebutton.ToolTip = "terug naar het hoofdmenu";
            homebutton.Margin = new Thickness(0, 5, 0, 0);

            spBreadcrumb.Children.Add(homebutton);

            bool blnDotsOnce = false;

            for (int i = 1; i < listCrumbs.Count; i++)
            {
                object pagina = listCrumbs[i];

                if (pagina.GetType().BaseType == typeof(WFControl))
                {

                    // voor de laatste een label gebruiken
                    if (i == (listCrumbs.Count - 1))
                    {
                        // lblBreadcrumb.Content += " > " + ((WFControl)pagina).PageTitle;
                        System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                        label.Content = " > ";
                        spBreadcrumb.Children.Add(label);

                        System.Windows.Controls.Label label2 = new System.Windows.Controls.Label();
                        label2.Content = ((WFControl)pagina).PageTitle;
                        label2.ToolTip = ((WFControl)pagina).PageSubtitle;

                        label2.Margin = new Thickness(-5, 0, 0, 0);

                        // netjes maken
                        label2.Content = label2.Content.ToString().ToStringTrimmed();

                        spBreadcrumb.Children.Add(label2);
                    }
                    else if (listCrumbs.Count > maxLengthCrumbs && i > 1 && i < startPunt)
                    {
                        if (!blnDotsOnce)
                        {
                            // lblBreadcrumb.Content += " > " + ((WFControl)pagina).PageTitle;
                            System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                            label.Content = " > ";
                            spBreadcrumb.Children.Add(label);

                            System.Windows.Controls.Button button = new System.Windows.Controls.Button();
                            button.Style = this.FindResource("LinkButton") as Style;
                            button.Tag = (startPunt - 2).ToString();
                            button.Click += new RoutedEventHandler(GoBack2);
                            button.Content = "...";
                            button.ToolTip = "...";
                            button.Margin = new Thickness(0, 5, 0, 0);
                            spBreadcrumb.Children.Add(button);
                            blnDotsOnce = true;
                        }
                    }

                    else // anders een link
                    {
                        // lblBreadcrumb.Content += " > " + ((WFControl)pagina).PageTitle;
                        System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                        label.Content = " > ";
                        spBreadcrumb.Children.Add(label);

                        System.Windows.Controls.Button button = new System.Windows.Controls.Button();
                        button.Style = this.FindResource("LinkButton") as Style;
                        button.Tag = (i).ToString();
                        button.Click += new RoutedEventHandler(GoBack2);
                        button.Content = ((WFControl)pagina).PageTitle;
                        button.ToolTip = ((WFControl)pagina).PageSubtitle;

                        button.Margin = new Thickness(0, 5, 0, 0);

                        // netjes maken
                        button.Content = button.Content.ToString().ToStringTrimmed();

                        spBreadcrumb.Children.Add(button);

                    }
                }
                else if (pagina.GetType().BaseType == typeof(MenuControl))
                {

                    // voor de laatste een label gebruiken
                    if (i == (listCrumbs.Count - 1))
                    {
                        //lblBreadcrumb.Content += " > " + ((MenuControl)pagina).PageTitle;
                        System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                        label.Content = " > ";
                        spBreadcrumb.Children.Add(label);

                        System.Windows.Controls.Label label2 = new System.Windows.Controls.Label();
                        label2.Content = ((MenuControl)pagina).PageTitle;
                        label2.ToolTip = ((MenuControl)pagina).PageSubtitle;

                        label2.Margin = new Thickness(-5, 0, 0, 0);

                        // netjes maken
                        label2.Content = label2.Content.ToString().ToStringTrimmed();

                        spBreadcrumb.Children.Add(label2);
                    }
                    else if (listCrumbs.Count > maxLengthCrumbs && i > 1 && i < startPunt)
                    {

                        if (!blnDotsOnce)
                        {
                            //lblBreadcrumb.Content += " > " + ((MenuControl)pagina).PageTitle;
                            System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                            label.Content = " > ";
                            spBreadcrumb.Children.Add(label);

                            System.Windows.Controls.Button button = new System.Windows.Controls.Button();
                            button.Style = this.FindResource("LinkButton") as Style;
                            button.Tag = (startPunt - 2).ToString();
                            button.Click += new RoutedEventHandler(GoBack2);
                            button.Content = "...";
                            button.ToolTip = "...";
                            button.Margin = new Thickness(0, 5, 0, 0);
                            spBreadcrumb.Children.Add(button);
                            blnDotsOnce = true;
                        }
                    }
                    else // anders een link
                    {

                        //lblBreadcrumb.Content += " > " + ((MenuControl)pagina).PageTitle;
                        System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                        label.Content = " > ";
                        spBreadcrumb.Children.Add(label);

                        System.Windows.Controls.Button button = new System.Windows.Controls.Button();
                        button.Style = this.FindResource("LinkButton") as Style;
                        button.Tag = (i).ToString();
                        button.Click += new RoutedEventHandler(GoBack2);
                        button.Content = ((MenuControl)pagina).PageTitle;
                        button.ToolTip = ((MenuControl)pagina).PageSubtitle;

                        button.Margin = new Thickness(0, 5, 0, 0);

                        // netjes maken
                        button.Content = button.Content.ToString().ToStringTrimmed();

                        spBreadcrumb.Children.Add(button);
                    }


                }
            }
        }

        /// <summary>
        /// load the specified control
        /// </summary>
        /// <param name="control"></param>
        public void LoadControlGoingback(System.Windows.Controls.Control control)
        {

            // verwijder het huidige item uit de navigatie
            int count = ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Count;
            ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).RemoveAt(count - 1);
           
            // load the control
            ccPageContainer.Content = control;
            wfhTest.Child = null;

            ccPageContainer.Visibility = System.Windows.Visibility.Visible;
            wfhTest.Visibility = System.Windows.Visibility.Collapsed;
            svTest.Visibility = System.Windows.Visibility.Collapsed;

            //wfhTest.Height = 0;
            //wfhTest.Visibility = System.Windows.Visibility.Collapsed;

            // laad het menu
            if (control.GetType().BaseType == typeof(MenuControl))
            {
                LoadMenu(((MenuControl)control));
                // TODO: TEST OF DIT WEG MAG ((MenuControl)control).PageReloaded();
                ((MenuControl)control).PageReloaded();
            }
            else if (control.GetType().BaseType == typeof(WFControl))
            {
                // kom je hier ooit in?
                // laadt menu voor WF Control
                LoadMenu(((MenuControl)control));
            }
                // als het de navigatie is, de buttons reloaden
            else if (control.GetType().BaseType == typeof(NavigationControl))
            {

                // maak menu onzichtbaar in het hoofdmenu
                gridMenu.Visibility = System.Windows.Visibility.Collapsed;

                ((NavigationControl)control).PageReloaded();
            }
            else
            {
                // kom je hier ooit in?
                // laadt menu voor normaal Control
                LoadMenu(control);
            }

        }

        /// <summary>
        /// load the specified control
        /// </summary>
        /// <param name="control"></param>
        public void LoadControlGoingback(System.Windows.Forms.Control control)
        {

            // verwijder het huidige item uit de navigatie
            int count = ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Count;
            ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).RemoveAt(count-1);

            // load the control
            wfhTest.Child = control;
            ccPageContainer.Content = null;

            ccPageContainer.Visibility = System.Windows.Visibility.Collapsed;

            // laad het menu
            if (control.GetType().BaseType == typeof(WFControl))
            {
                LoadMenu(((WFControl)control));
            }
        }

        /// <summary>
        /// go back -1 in history
        /// </summary>
        /// <param name="control"></param>
        public void GoBack()
        {
            List<object> listNavigationHistory = ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory);

            if (ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Count > 1)
            {
                int counter = ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Count;

                // load the previous page into the contentcontrol
                object control = ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory)[counter - 2];

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
                    LoadControlGoingback((System.Windows.Controls.Control)control);
                }
            }
        }

        public void GoBack(string pagesubtitle)
        {
            List<object> listNavigationHistory = ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory);

            if (ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Count > 1)
            {
                int counter = ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory).Count;

                // load the previous page into the contentcontrol
                object control = ApplicationState.GetValue<List<object>>(ApplicationVariables.listNavigationHistory)[counter - 2];

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
                    LoadControlGoingback((System.Windows.Controls.Control)control);
                }
            }

            SetPageSubTitle(pagesubtitle);

        }

        private void ReloadMenuHandler(object sender, RoutedEventArgs e)
        {
            btnTerug.Content = ((MenuControl)sender).PageBackButtonText;
            btnGereed.Content = ((MenuControl)sender).PageOKButtonText;
        }

        private void btnTerug_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private void btnGereed_Click(object sender, RoutedEventArgs e)
        {
            if (ccPageContainer.Content != null)
            {
                ((MenuControl)ccPageContainer.Content).PageOK();
            }
            else if(wfhTest.Child != null)
            {
                ((WFControl)wfhTest.Child).PageOK();
            }
        }


        #region Communication to other app instances

        PeerChat chat;
        void chat_OnChat(string sender, string message)
        {

            if (ccPageContainer.Content != null && ccPageContainer.Content.GetType().BaseType == typeof(MenuControl) && Global.useChatFunction)
            {
                Logging log = new Logging();
                log.Log(sender + "|" + message);

                if(ApplicationState.GetValue<string>(ApplicationVariables.strIpAddress) != sender)
                {
                    ((MenuControl)ccPageContainer.Content).TriggerChannelMessage(sender, message);
                }

            }

            //else if (wfhTest.Child != null)
            //{
            //    ((WFControl)wfhTest.Child).PageChannelMessage(sender, message);
            //}
        }

        void chat_OnLeave(string member)
        {
            //txtChat.Text += String.Format("[{0} left]" + Environment.NewLine, member);
        }

        void chat_OnJoin(string member)
        {
            //txtChat.Text += String.Format("[{0} joined]" + Environment.NewLine, member);
            //btnSend.IsEnabled = true;
        }

        void chat_OnChatOffline(object sender, EventArgs e)
        {
            //txtChat.Text += String.Format("**  Offline" + Environment.NewLine);
        }

        void chat_OnChatOnline(object sender, EventArgs e)
        {
            //txtChat.Text += String.Format("**  Online" + Environment.NewLine);
        }

        public void SendChannelMessage(string sender, string message)
        {

            if (chat != null && Global.useChatFunction)
            {
                if (!chat.SendMessage(message))
                {
//                    System.Windows.Forms.MessageBox.Show("Kan ZeebregtsChannelService niet bereiken." + Environment.NewLine + "Andere gebruikers zijn mogelijk niet op de hoogte van deze wijziging.",
//"Belangrijke melding omtrent ZeebregtsChannelService",
//MessageBoxButtons.OK,
//MessageBoxIcon.Exclamation,
//MessageBoxDefaultButton.Button1);

                    Logging log = new Logging();
                    log.Log("Andere gebruikers zijn mogelijk niet op de hoogte van deze wijziging.");

                }
            }

            //chat.Chat(sender, message);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (chat != null)
            {
                chat.End();
            }
        }

        //private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        chat.SendMessage(txtMessage.Text);
        //        txtMessage.Text = String.Empty;
        //    }
        //}

        public bool LoadCommunicationChannel()
        {


            if (chat == null && Global.useChatFunction)
            {
                //                IPHostEntry IPHost = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                string ipadresses = string.Empty;

                //var local = NetworkInterface.GetAllNetworkInterfaces().Where(i => i.Name == "Local Area Connection").FirstOrDefault();
                //var stringAddress = local.GetIPProperties().UnicastAddresses[0].Address.ToString();
                //var ipAddress = IPAddress.Parse(address);

                UdpClient u = new UdpClient("192.160.0.120", 8089);
                IPAddress localAddr = ((IPEndPoint)u.Client.LocalEndPoint).Address;

                //System.Windows.MessageBox.Show(localAddr.ToString());
                string endpoint;
                endpoint = localAddr.ToString();

                ApplicationState.SetValue(ApplicationVariables.strIpAddress, endpoint);

                //foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                //{
                //    try
                //    {
                //        foreach (UnicastIPAddressInformation ipadres in ni.GetIPProperties().UnicastAddresses)
                //        {
                //            if (ipadres.Address.ToString().Contains("192.160.0.") && Convert.ToInt32(ipadres.Address.ToString().Substring(10)) >= 70 && Convert.ToInt32(ipadres.Address.ToString().Substring(10)) <= 90)
                //            {
                //                ipadresses += Environment.NewLine + ipadres.Address.ToString() + ": " + ipadres.Address.AddressFamily.ToString();
                //            }
                //        }

                //    }
                //    catch
                //    {
                //        //System.Windows.MessageBox.Show("Error #401");
                //    }
                //}

                //IPAddress ip = System.Net.Dns.GetHostEntry(Environment.MachineName).AddressList.Where(i => i.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).FirstOrDefault();

                //System.Windows.MessageBox.Show("LoadCommunicationChannel(" + endpoint + ")" + ipadresses);
                //Random rnd = new Random();
                //string rand = rnd.Next(1000).ToString();

                //System.Windows.MessageBox.Show(endpoint + ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider).ToString() + rand);


                //chat = new PeerChat(endpoint + ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider).ToString() + rand);

                //chat = new PeerChat("MDR");
                chat = new PeerChat(endpoint);

                chat.OnChatOnline += new PeerChat.ChatOnline(chat_OnChatOnline);
                chat.OnChatOffline += new PeerChat.ChatOffline(chat_OnChatOffline);
                chat.OnJoin += new PeerChat.JoinChat(chat_OnJoin);
                chat.OnLeave += new PeerChat.LeaveChat(chat_OnLeave);
                chat.OnChat += new PeerChat.ChatSendReceive(chat_OnChat);
                return chat.Start(localAddr);
            }
            else
            {
                return false;
            }

            //return string.Empty;
        }

        #endregion


        //private void pcWindow_SizeChanged_1(object sender, SizeChangedEventArgs e)
        //{
        //    SetWidth();
        //}

        //private void SetWidth()
        //{
        //    if (wfhTest.Child != null)
        //    {
        //        if (wfhTest.Child.GetType().BaseType == typeof(WFControl))
        //        {
        //            //((WFControl)wfhTest.Child).HHH.Width = this.ActualWidth;
        //        }
        //    }

        //}




    }
}

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
using ZeebregtsLogic;
using System.ComponentModel;

namespace ZeebregtsSysteem.controls
{

    public delegate void FunctieGereed();

    public class MenuControl : UserControl, INotifyPropertyChanged
    {
        public static RoutedCommand btnTerugCmd = new RoutedCommand();
        public static RoutedCommand btnGereedCmd = new RoutedCommand();

        public event FunctieGereed Gereed;
        public event PropertyChangedEventHandler PropertyChanged;

        // statische variabelen, hier standaard waarden setten, op elk control dat inherit van MenuControl apart te wijzigen
        private string strTerugButtonText;

        public string TerugButtonText {
         get { return strTerugButtonText;}
            set{strTerugButtonText = value;
            OnPropertyChanged("TerugButtonText");
            }
        }


        public static Visibility GereedButtonVisibility = Visibility.Visible;
        public static string PageTitle = "Bel de programmeur om ook deze pagina van een leuke titel te voorzien :-) 0646408407";
        public static string GereedButtonText = "OK";

        public static Button btnTerug = new Button();

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));

            }

        }

        static MenuControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuControl), new FrameworkPropertyMetadata(typeof(MenuControl)));
        }

        public MenuControl()
        {
            this.CommandBindings.Add(new CommandBinding(btnTerugCmd, btnTerug_OnClick));
            this.CommandBindings.Add(new CommandBinding(btnGereedCmd, btnGereed_OnClick));

        }

        private void btnTerug_OnClick(object sender, ExecutedRoutedEventArgs e)
        {
            List<Control> listNavigationHistory = ApplicationState.GetValue<List<Control>>(ApplicationVariables.listNavigationHistory);

            if (ApplicationState.GetValue<List<Control>>(ApplicationVariables.listNavigationHistory).Count > 1)
            {
                // load the previous page into the contentcontrol
                Control control = ApplicationState.GetValue<List<Control>>(ApplicationVariables.listNavigationHistory)[1];

                ((PageContainer)((StackPanel)((ContentControl)Parent).Parent).Parent).LoadControlGoingback(control);

            }
        }


        private void btnGereed_OnClick(object sender, ExecutedRoutedEventArgs e)
        {
            Gereed();
        }

    }
}

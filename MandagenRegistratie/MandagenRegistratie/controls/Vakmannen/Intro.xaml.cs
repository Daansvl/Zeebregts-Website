using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MandagenRegistratie.controls.Vakmannen.Lijst;
using MandagenRegistratie.controls.Vakmannen.Overzicht;
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Vakmannen
{
    /// <summary>
    /// Interaction logic for Intro.xaml
    /// </summary>
    public partial class Intro : MenuControl
    {
        public Intro()
        {
            InitializeComponent();

            #region Pagina specifieke informatie
            PageTitle = "Urenoverzicht vakmannen"; // pageTitle;
            PageSubtitle = "Toon vakman uren op dag naar keuze";
            PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            PageOKButtonText = "VAKMANLIJST";
            PageBackButtonText = "TERUG";
            OkClick += LoadLijst;
            Reloaded += Load;
            ChannelMessage += Intro_ChannelMessage;
            #endregion

        }

        void Intro_ChannelMessage(string sender, string message)
        {
            //MessageBox.Show("message received from " + sender + ": " + message);


            List<string> arrayVakmannen = message.Split('|')[0].Split(',').ToList<string>();

            List<int> listVakmannen = arrayVakmannen.ConvertAll<int>(delegate(string i) { return i.ToInt(); });
            DateTime datum = Convert.ToDateTime(message.Split('|')[1]);

            Reload(listVakmannen, datum);

        }


        public void Reload(List<int> listVakmannen, DateTime datum)
        {

            Overzicht.Overzicht pp = (Overzicht.Overzicht)tiOverzicht.Content;

            pp.Reload(listVakmannen, datum);
        }

        public void Load()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            Overzicht.Overzicht pp = new Overzicht.Overzicht();
            //vov.LoadView();

            dbRepository dbrep = new dbRepository();
            List<Gebruiker> listProjectleiders = new List<Gebruiker>();
            listProjectleiders = dbrep.GetProjectleiders().Where(p => p.ProjectleiderId != ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider)).ToList();
            listProjectleiders.Insert(0, dbrep.GetProjectleider(ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider)));

            pp.listProjectleiders = listProjectleiders;

            pp.Load();

            //PageContainer pc = Tools.FindVisualParent<PageContainer>(this);

            //if (pc != null)
            //{
            //    #region Pagina specifieke informatie
            //    pc.SetPageTitle("Projecten"); // pageTitle;
            //    pc.SetPageSubTitle("Overzicht");
            //    pc.SetVisibilityGereedButton(System.Windows.Visibility.Hidden);
            //    pc.SetPageTerugButtonText("Menu");
            //    #endregion
            //}

            tcTabs.TabIndex = 0;

            tiOverzicht.Content = pp;

            //Reloaded -= Load;
            //Reloaded += Load;

            Mouse.OverrideCursor = null;

        }

        //public void NieuwProject()
        //{
        //    AlleProjecten ap = new AlleProjecten();
        //    Lijst.Projecten pp = (Lijst.Projecten)tiLijst.Content;
        //    ap.listVwProjecten = pp.listVwProjecten;

        //    OkClick -= NieuwProject;
        //    //CustomActieClick -= pp.Selectproject;

        //    //OkClick += NieuwProject;
        //    OkClick += ap.ToevoegenAanProjecten;
        //    //CustomActieClick += ap.ToevoegenAanProjecten;

        //    ap.ZoekResultatenOrigineel();



        //    tiLijst.Content = ap;

        //    PageContainer pc = Tools.FindVisualParent<PageContainer>(this);

        //    if (pc != null)
        //    {
        //        #region Pagina specifieke informatie
        //        pc.SetPageTitle("Projecten"); // pageTitle;
        //        pc.SetPageSubTitle("Overzicht");
        //        pc.SetVisibilityGereedButton(System.Windows.Visibility.Hidden);
        //        pc.SetPageTerugButtonText("Menu");
        //        pc.SetPageGereedButtonText("Nieuw project");
        //        #endregion
        //    }


        //}

        public void LoadLijst()
        {

            Mouse.OverrideCursor = Cursors.Wait;

            Lijst.VakmannenOverview pp = new Lijst.VakmannenOverview(false);

            pp.LoadView();

            ////////pp.Reloaded += pp.ZoekResultaten;

            ////////pp.OkClick += pp.nie;
            ////////pp.CustomActieClick += pp.Selectproject;

            //PageContainer pc = Tools.FindVisualParent<PageContainer>(this);

            //if (pc != null)
            //{
            //    #region Pagina specifieke informatie
            //    pc.SetPageTitle("Projecten"); // pageTitle;
            //    pc.SetPageSubTitle("Overzicht");


            //    if (Rechten.IsProjectleider)
            //    {
            //        pc.SetVisibilityGereedButton(System.Windows.Visibility.Visible);
            //    }
            //    else
            //    {
            //        pc.SetVisibilityGereedButton(System.Windows.Visibility.Hidden);
            //    }

            //    pc.SetPageTerugButtonText("Menu");
            //    pc.SetPageGereedButtonText("Nieuw project");
            //    #endregion
            //}



            PageGoToPage(pp);

            //tcTabs.TabIndex = 1;

            //tiLijst.Content = pp;

            Mouse.OverrideCursor = null;


        }

        private void tcTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (tiOverzicht.IsSelected)
                {
                    Load();
                }
                //else if (tiLijst.IsSelected)
                //{
                //    LoadLijst();
                //}
            }

        }

        private void MenuControl_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            CheckLayout();
        }

        public void CheckLayout()
        {
            if (Window.GetWindow(this) != null)
            {
                spWrapper.Height = Window.GetWindow(this).ActualHeight - 172;
                spWrapper.Width = Window.GetWindow(this).ActualWidth;
            }
        }
    }
}

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
using MandagenRegistratieDomain;
using MandagenRegistratie.controls.Projecten.Dagview;

namespace MandagenRegistratie.controls.Projecten.Overzicht
{
    /// <summary>
    /// Interaction logic for Project.xaml
    /// </summary>
    public partial class Project : MenuControl
    {
        public Project(string pageTitle, string pageSubtitle)
        {
            InitializeComponent();
            #region Pagina specifieke informatie
            PageTitle = "Projectview"; // pageTitle;
            PageSubtitle = "Planning voor " + pageTitle;
            PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            PageOKButtonText = "WIJZIG";
            #endregion

            // load info op pagina
            //this.Load();

            OkClick += Okay;
            Reloaded += Load;
//#if DEBUG
//#else
            ChannelMessage += ReceiveChannelMessage;

//#endif
            //// create the page and load all values
            //VakmanDagView vdv = new VakmanDagView();
            //vdv.LoadVakmanDagView(true);
            //vdv.LoadWeekInfo();

            //tabPlanning.Content = vdv;
        }

        int intProjectId = -1;
        DateTime dtSelectedDay = DateTime.MinValue;

        /// <summary>
        /// Voert de OKAY functie uit
        /// </summary>
        public void Okay()
        {

            ProjectEdit pe = new ProjectEdit(PageTitle);
            pe.Load();

            PageGoToPage(pe);

        }

        /// <summary>
        /// Voert de OKAY functie uit
        /// </summary>
        public void ReceiveChannelMessage(string sender, string message)
        {
            ((ProjectDagView)tabPlanning.Content).ReceiveChannelMessage(sender, message);
        }


        public void Load()
        {
            dbRepository dbrep = new dbRepository();

            if (intProjectId == -1)
            {
                intProjectId = ApplicationState.GetValue<int>(ApplicationVariables.intProjectId);
            }

            //if (dtSelectedDay == DateTime.MinValue)
            //{
                dtSelectedDay = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
            //}

            MandagenRegistratieDomain.Project project = dbrep.GetProject(intProjectId);

            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
            persoon persoon = dbrepOriginal.GetContact(dbrep.GetProjectleider(project.ProjectleiderId).ContactIdOrigineel);
            project dboproject = dbrepOriginal.GetProject(project.ProjectIdOrigineel);


            MandagenRegistratieDomain.adressen dboadres = null;

            if (dboproject.adres_id_bouw != null)
            {
                dboadres = dbrepOriginal.datacontext.adressens.Where(a => a.adres_id == dboproject.adres_id_bouw).FirstOrDefault();

            }
            else if (dboproject.adres_id_factuur != null)
            {
                dboadres = dbrepOriginal.datacontext.adressens.Where(a => a.adres_id == dboproject.adres_id_factuur).FirstOrDefault();
            }

            bedrijf bedrijfzdb = dbrepOriginal.datacontext.bedrijfs.Where(b => b.bedrijf_nr == dboproject.opdrachtgeverZEEBREGTS_nr).FirstOrDefault();

            if (bedrijfzdb != null)
            {
                lblOpdrachtgever.Content = bedrijfzdb.naam;
            }

            lblProjectNr.Content = dbrepOriginal.GetProject(project.ProjectIdOrigineel).project_NR.ToString();

            lblActief.Content = project.Actief == true ? "Ja" : "Nee";
            lblProjectnaam.Content = dboproject.naam_project;
            lblProjectleider.Content = persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam;
            if (dboadres != null)
            {
                lblPostcode.Content = dboadres.postcode_cijfers.HasValue ? dboadres.postcode_cijfers.Value.ToString() + dboadres.postcode_letters : "";
                lblHuisnummer.Content = dboadres.huis_postbus_nummer + dboadres.huisnummer_toevoeging;
                lblAdres.Content = dboadres.straat_1;
            }


            ProjectDagView pdv = new ProjectDagView();
            pdv.objProject = project;
            pdv.LoadVakmanDagView(true, intProjectId, dtSelectedDay);
            pdv.LoadWeekInfo();
            
            //pdv.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            tabPlanning.Content = pdv;

        }

        /// <summary>
        /// Voeg dit item toe aan het panel
        /// </summary>
        /// <param name="label"></param>
        /// <param name="waarde"></param>
        public void AddLabelToPanel(string label, string waarde)
        {

            Label lbl = new Label();
            lbl.Content = label;
            lbl.Width = 200;
            lbl.Background = Brushes.Bisque;

            Label lbl2 = new Label();
            lbl2.Content = waarde;
            lbl2.Width = 400;


            wpDetails.Children.Add(lbl);
            wpDetails.Children.Add(lbl2);

        }


        /// <summary>
        /// Voeg dit item toe aan het panel
        /// </summary>
        /// <param name="label"></param>
        /// <param name="waarde"></param>
        public void AddLabelToPanel(string label)
        {

            Label lbl = new Label();
            lbl.Content = label;
            lbl.Width = 600;
            lbl.FontWeight = FontWeights.Bold;

            wpDetails.Children.Add(lbl);

        }

        private void MenuControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Window.GetWindow(this) != null)
            {
                //tabControl.Height = Window.GetWindow(this).ActualHeight - 110;
                //tabControl.Height = Window.GetWindow(this).ActualHeight - 130;
                tabControl.Width = Window.GetWindow(this).ActualWidth;
            }

        }

        private void tabDetails_GotFocus(object sender, RoutedEventArgs e)
        {
            PageContainer pc = (PageContainer)Tools.FindVisualParent<Window>(this);
            pc.SetLabelSubtitleGegevens();
        }

        private void tabPlanning_GotFocus(object sender, RoutedEventArgs e)
        {
            PageContainer pc = (PageContainer)Tools.FindVisualParent<Window>(this);
            pc.SetLabelSubtitlePlanning();
        }


    }
}

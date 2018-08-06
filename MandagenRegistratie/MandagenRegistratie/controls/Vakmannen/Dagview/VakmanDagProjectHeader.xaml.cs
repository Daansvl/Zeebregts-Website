using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using MandagenRegistratie.tools;
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Vakmannen.Dagview
{
    /// <summary>
    /// Interaction logic for VakmanDagProjectHeader.xaml
    /// </summary>
    public partial class VakmanDagProjectHeader : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "Project":
                    dbRepository dbrep = new dbRepository();
                    DateTime dtSelectedDay = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
                    DbTools dbtools = new DbTools();

                    StackPanel strTooltip = new StackPanel();
                    StackPanel strTooltipAanvraag = new StackPanel();

                    dbOriginalRepository dbreporiginal = new dbOriginalRepository();
                    MDRpersoon pp = dbreporiginal.GetContact(dbrep.GetProjectleider(project.ProjectleiderId, project.ProjectId, dtSelectedDay).ContactIdOrigineel);
                    lblHeader.Content = ToonNaam(pp);
                    lblHeader.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    //lblHeader.BorderThickness = new Thickness(1);
                    //lblHeader.BorderBrush = new SolidColorBrush(Colors.Black);


                    btnHeader.Style = this.FindResource("LinkButton") as Style;

                    MDRproject pr = dbreporiginal.GetProject((int)project.ProjectNr);
                    double ww = 400;
                    if (btnHeader.Width > 0)
                    {
                        ww = btnHeader.Width;
                    }

                    if (btnHeader.FontSize > 11)
                    {
                        ww = ww * 0.8;
                    }

                    // TOOLTIP
                    // TOOLTIP
                    // TOOLTIP
                    DateTime dtdag = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
                    Vakman vakman = dbrep.GetVakman(ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId));
                    strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraagAll(vakman, project, dtdag, true, true));

                    if (pr != null)
                    {

                        btnHeader.Content = pr.project_NR.ToString(5) + " " + pr.naam_project.ToString(ww);
                        // de naam van het project ook tonen in de tooltip bovenaan
                        //strTooltip.AddText(pr.naam_project.ToString() + " (Project " + pr.project_NR + ")");
                    }
                    else
                    {
                        btnHeader.Content = "      " + project.Naam.ToString(ww);
                        // de naam van het project ook tonen in de tooltip bovenaan
                        //strTooltip.AddText(project.Naam.ToString() + " (Project --)");
                    }

                    //List<Mandagen> listMandagenTotal = dbrep.datacontext.Mandagens.Where(m => m.Begintijd != m.Eindtijd && m.Begintijd >= ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay) && m.Eindtijd <= ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay).AddDays(1)).ToList();

                    //List<Mandagen> listMandagen = listMandagenTotal.Where(m => m.ProjectId == project.ProjectId).ToList();
                    //// als in deze pool van mandagen een aanvraag zit:

                    //// vandaag, ander project, geen projectleider, niet bevestigd (dus aanvraag) 
                    //List<Mandagen> listAanvragen = listMandagenTotal.Where(m => m.ProjectleiderId != ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).ProjectleiderId && m.ProjectId != project.ProjectId && m.Begintijd >= ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay) && m.Eindtijd <= ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay).AddDays(1) && !m.Status).ToList();

                    //// als in deze pool van mandagen een aanvraag zit:
                    //// vandaag, ander project, geen projectleider, wel bevestigd (dus waar dit de aanvraag van is) 
                    //List<Mandagen> listMandagenWaarDitDeAanvraagVanIs = listMandagenTotal.Where(m => m.ProjectId != project.ProjectId && m.ProjectleiderId != ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).ProjectleiderId && m.ProjectId != project.ProjectId && m.Begintijd >= dtSelectedDay && m.Eindtijd <= dtSelectedDay.AddDays(1) && m.Status).ToList();

                    //int previousVakmanId = -1;

                    //foreach (Mandagen mandag in listMandagen)
                    //{


                    //    if (mandag.Status)
                    //    {
                    //        bool isBold = mandag.VakmanId == ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId);
                    //        //strTooltip.AddText("(" + ToonTijd(mandag) + ")" + " " + ToonNaam(dbreporiginal.GetContact(mandag.Vakman.ContactIdOrigineel, true)), isBold);

                    //        // OF we hebben te maken met externe aanvragen
                    //        Mandagen aanvraag = listAanvragen.FirstOrDefault(m => m.Begintijd < mandag.Eindtijd && m.Eindtijd > mandag.Begintijd && m.VakmanId == mandag.VakmanId);

                    //        if (aanvraag != null)
                    //        {
                    //            List<Mandagen> listIngeplandeMandagen = new List<Mandagen>();
                    //            listIngeplandeMandagen.Add(mandag);
                    //            if (previousVakmanId == mandag.VakmanId)
                    //            {
                    //                //strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraagMinOnly(listIngeplandeMandagen, aanvraag, false));
                    //                //strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraagAll(mandag.Vakman, mandag.Project, dbtools.GetHeleDag(mandag.Begintijd)));
                    //            }
                    //            else
                    //            {
                    //                //strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraag(listIngeplandeMandagen, aanvraag, false));
                    //                //strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraagAll(mandag.Vakman, mandag.Project, dbtools.GetHeleDag(mandag.Begintijd)));
                    //            }
                    //            previousVakmanId = mandag.VakmanId;

                    //        }

                    //    }
                    //    else
                    //    {

                    //        if (previousVakmanId == mandag.VakmanId)
                    //        {
                    //            //strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraagMinOnly(listMandagenWaarDitDeAanvraagVanIs, mandag, true));
                    //            //strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraagAll(mandag.Vakman, mandag.Project, dbtools.GetHeleDag(mandag.Begintijd), true, false));
                    //        }
                    //        else
                    //        {
                    //            // OF het is zelf de aanvraag
                    //            //strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraag(listMandagenWaarDitDeAanvraagVanIs, mandag, true));
                    //            //strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraagAll(mandag.Vakman, mandag.Project, dbtools.GetHeleDag(mandag.Begintijd), true, false));
                    //        }

                    //        previousVakmanId = mandag.VakmanId;
                    //    }
                    //}

                    //foreach (Mandagen mandag in listAanvragen)
                    //{
                    //    if (!mandag.Status)
                    //    {
                    //        // OF dit project heeft een andere aanvraag lopen die de vakman VAN het project af wil halen
                    //        listMandagenWaarDitDeAanvraagVanIs = listMandagenTotal.Where(m => m.VakmanId == mandag.VakmanId &&  m.ProjectId != mandag.ProjectId && m.ProjectleiderId != ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).ProjectleiderId && m.Begintijd >= dtSelectedDay && m.Eindtijd <= dtSelectedDay.AddDays(1) && m.Status).ToList();
                    //        strTooltipAanvraag += AddTooltipAanvraag(listMandagenWaarDitDeAanvraagVanIs, mandag, false);
                    //    }
                    //}

                    ToolTipService.SetShowDuration(btnHeader, 20000);
                    strTooltip.Children.Add(strTooltipAanvraag);
                    btnHeader.ToolTip = strTooltip;
                    btnHeader.Margin = new Thickness(0, 0, 0, -8);

                    if (ApplicationState.GetValue<int>("intProjectId") == project.ProjectId)
                    {
                        // make bold
                        //btnHeader.FontWeight = FontWeights.ExtraBold;
                    }

                    break;
                default:
                    break;
            }
        }


        //protected string AddTooltipAanvraag(List<Mandagen> listMandagenWaarDitDeAanvraagVanIs, Mandagen mandag, bool reverse)
        //{
        //    dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
        //    MDRpersoon objVakman = dbrepOriginal.GetContact(mandag.Vakman.ContactIdOrigineel);
        //    DbTools dbtools = new DbTools();

        //    string strTooltipAanvraag = string.Empty;

        //    if (listMandagenWaarDitDeAanvraagVanIs.Count > 0)
        //    {
        //        strTooltipAanvraag += Environment.NewLine;
        //        strTooltipAanvraag += "Aanvraag " + (objVakman.voornaam + " " + objVakman.tussenvoegsel + " " + objVakman.achternaam).ToStringTrimmed();

        //        if (mandag.Geannulleerd)
        //        {
        //            strTooltipAanvraag += " AFGEWEZEN";
        //        }

        //        strTooltipAanvraag += Environment.NewLine;
        //        Mandagen origineel = listMandagenWaarDitDeAanvraagVanIs.FirstOrDefault(m => m.Begintijd <= mandag.Eindtijd && m.Eindtijd >= mandag.Begintijd && m.VakmanId == mandag.VakmanId);
        //        if (origineel != null)
        //        {
        //            Project objProjectVan = origineel.Project;
        //            MDRproject objProjectNaar = dbrepOriginal.GetProject((int)mandag.Project.ProjectNr, true);

        //            if (reverse)
        //            {
        //                strTooltipAanvraag += dbtools.GetIngeplandeTijd(mandag.Project, mandag.Vakman, dbtools.GetHeleDag(mandag.Begintijd)) + " + (" + ToonTijd(mandag) + ") " + objProjectNaar.naam_project + " (Project " + objProjectNaar.project_NR.ToString() + ")";
        //                strTooltipAanvraag += Environment.NewLine;
        //                strTooltipAanvraag += dbtools.GetIngeplandeTijd(objProjectVan, mandag.Vakman, dbtools.GetHeleDag(mandag.Begintijd)) + " \u2212 (" + ToonTijd(mandag) + ") " + objProjectVan.Naam + " (Project " + objProjectVan.ProjectNr.ToString() + ")";
        //            }
        //            else
        //            {
        //                strTooltipAanvraag += dbtools.GetIngeplandeTijd(objProjectVan, mandag.Vakman, dbtools.GetHeleDag(mandag.Begintijd)) + " \u2212 (" + ToonTijd(mandag) + ") " + objProjectVan.Naam + " (Project " + objProjectVan.ProjectNr.ToString() + ")";
        //                strTooltipAanvraag += Environment.NewLine;
        //                strTooltipAanvraag += dbtools.GetIngeplandeTijd(mandag.Project, mandag.Vakman, dbtools.GetHeleDag(mandag.Begintijd)) + " + (" + ToonTijd(mandag) + ") " + objProjectNaar.naam_project + " (Project " + objProjectNaar.project_NR.ToString() + ")";
        //            }
        //        }
        //    }

        //    return strTooltipAanvraag;
        //}

        protected string ToonTijd(Mandagen mandag)
        {
            return mandag.Uren.ToString() + ":" + (mandag.Minuten < 10 ? "0" : "") + mandag.Minuten.ToString();
        }


        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #region "RoutedEvents"
        public static readonly RoutedEvent VakmanDagProjectHeaderEvent = EventManager.RegisterRoutedEvent("OnVakmanDagProjectHeaderUpdate", RoutingStrategy.Bubble,
        typeof(RoutedEventHandler), typeof(VakmanDagProjectHeader));

        public event RoutedEventHandler OnVakmanDagProjectHeaderUpdate
        {
            add { AddHandler(VakmanDagProjectHeaderEvent, value); }
            remove { RemoveHandler(VakmanDagProjectHeaderEvent, value); }
        }
        #endregion


        private Project project;
        public Project Project
        {
            get { return project; }
            set { SetField(ref project, value, "Project"); }
        }


        public VakmanDagProjectHeader()
        {
            InitializeComponent();

            //slider.LowerSlider.SelectionStart = 0;
            //slider.LowerSlider.SelectionEnd = 100;

        }

        private void btnGoToProject_Click(object sender, RoutedEventArgs e)
        {
            dbRepository dbrep = new dbRepository();
            dbOriginalRepository dbOriginalRep = new dbOriginalRepository();

            ApplicationState.SetValue(ApplicationVariables.intProjectId, project.ProjectId);
            //ProjectView pv = new ProjectView();
            //pv.Show();
            MDRpersoon projectleider = dbOriginalRep.GetContact(dbrep.GetProjectleider(project.ProjectleiderId).ContactIdOrigineel);

            MDRproject ppp = dbOriginalRep.GetProject((int)project.ProjectNr);

            // create the page and load all values
            Projecten.Lijst.Project po = new Projecten.Lijst.Project(ppp);
            //new Projecten.Overzicht.Project(project.ProjectId + ": " + project.Naam, projectleider.voornaam + " " + projectleider.tussenvoegsel + " " + projectleider.achternaam);

            // load the page into the contentcontrol
            MenuControl owner = Tools.FindVisualParent<MenuControl>(this);
            //po.PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            //po.PageOKButtonText = "PROJECT TOEVOEGEN";

            //po.OkClick += po.ToevoegenAanVakman;
            //po.dgProjecten.MouseDoubleClick -= po.dgProjecten_MouseDoubleClick;
            //po.dgProjecten.MouseDoubleClick += po.dgProjecten_MouseDoubleClickForVakman;
            //po.Load();
            //po.tabControl.SelectedIndex = 1;

            owner.PageGoToPage(po);


        }


        public static string ToonNaam(MDRpersoon objPersoon)
        {
            if (objPersoon != null)
            {
                return (objPersoon.voornaam + " " + objPersoon.tussenvoegsel + " " + objPersoon.achternaam).ToStringTrimmed();
            }
            else
            {
                return "";
            }
        }

    }
}

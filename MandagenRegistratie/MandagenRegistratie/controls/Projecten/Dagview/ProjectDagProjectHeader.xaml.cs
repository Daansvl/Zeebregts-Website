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
using MandagenRegistratie.controls.Vakmannen.Dagview;
using MandagenRegistratie.tools;
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Projecten.Dagview
{
    /// <summary>
    /// Interaction logic for ProjectDagProjectHeader.xaml
    /// </summary>
    public partial class ProjectDagProjectHeader : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "Vakman":

                    try
                    {
                        StackPanel strTooltip = new StackPanel();
                        StackPanel strTooltipAanvraag = new StackPanel();

                        btnHeader.Style = this.FindResource("LinkButton") as Style;
                        //btnHeader.Style = Tools.FindVisualParent<PageContainer>(this).FindResource("LinkButton") as Style;
                        dbOriginalRepository dbRepOriginal = new dbOriginalRepository();
                        dbRepository dbrep = new dbRepository();
                        DbTools dbtools = new DbTools();

                        MDRpersoon p = dbRepOriginal.GetContact(vakman.ContactIdOrigineel);


                        // TOOLTIP
                        // TOOLTIP
                        // TOOLTIP
                        DateTime dtdag = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
                        Project project = dbrep.GetProject(ApplicationState.GetValue<int>(ApplicationVariables.intProjectId));
                        Vakman objVakman = dbrep.GetVakman(vakman.VakmanId);
                        strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraagAll(objVakman, project, dtdag, false, false));




                        if (p != null)
                        {
                            btnHeader.Content = (p.voornaam + " " + (p.tussenvoegsel == null ? "" : p.tussenvoegsel) + " " + p.achternaam).ToStringTrimmed();
                            bool isBold = vakman.VakmanId == ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId);
                            //strTooltip.AddText((p.voornaam + " " + (p.tussenvoegsel == null ? "" : p.tussenvoegsel) + " " + p.achternaam).ToStringTrimmed(), isBold);
                        }

                        //List<Mandagen> listMandagenTotal = dbrep.GetMandagen(vakman.VakmanId, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay)).Where(m => m.Begintijd != m.Eindtijd).OrderBy(m => m.Status).ToList();


                        //int previousVakmanId = -1;
                        //foreach (Mandagen mandag in listMandagenTotal)
                        //{

                        //    // als in deze pool van mandagen een aanvraag zit:
                        //    // vandaag, ander project, geen projectleider, wel bevestigd (dus waar dit de aanvraag van is) 
                        //    List<Mandagen> listMandagenWaarDitDeAanvraagVanIs = listMandagenTotal.Where(m => m.ProjectId != mandag.ProjectId && m.ProjectleiderId != ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).ProjectleiderId && m.ProjectId != mandag.ProjectId && m.Begintijd >= ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay) && m.Eindtijd <= ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay).AddDays(1) && m.Status).ToList();

                        //    if (mandag.Status)
                        //    {
                        //        bool isBold = (ApplicationState.GetValue<int>(ApplicationVariables.intProjectId) == mandag.ProjectId);
                        //        strTooltip.AddText("(" + ToonTijd(mandag) + ")" + " " + dbRepOriginal.GetProject((int)mandag.Project.ProjectNr).naam_project, isBold);
                        //    }

                        //    if (!mandag.Status)
                        //    {


                        //        bool reverse = false;
                        //        if (ApplicationState.GetValue<int>(ApplicationVariables.intProjectId) == mandag.ProjectId)
                        //        {
                        //            reverse = true;
                        //        }
                        //        if (previousVakmanId == mandag.VakmanId)
                        //        {
                        //            //strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraagMinOnly(listMandagenWaarDitDeAanvraagVanIs, mandag, reverse));
                        //            //strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraagAll(mandag.Vakman, mandag.Project, dbtools.GetHeleDag(mandag.Begintijd)));
                        //        }
                        //        else
                        //        {
                        //            //strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraag(listMandagenWaarDitDeAanvraagVanIs, mandag, reverse));
                        //            //strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraagAll(mandag.Vakman, mandag.Project, dbtools.GetHeleDag(mandag.Begintijd)));
                        //        }
                        //    }

                        //    previousVakmanId = mandag.VakmanId;

                        //}

                        ToolTipService.SetShowDuration(btnHeader, 20000);
                        strTooltip.Children.Add(strTooltipAanvraag);
                        btnHeader.ToolTip = strTooltip;

                        if (ApplicationState.GetValue<int>(ApplicationVariables.intVakmanId) == vakman.VakmanId)
                        {
                            // make bold
                            //btnHeader.FontWeight = FontWeights.ExtraBold;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
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

        public static string ToonTijd(Mandagen mandag)
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
        public static readonly RoutedEvent ProjectDagProjectHeaderEvent = EventManager.RegisterRoutedEvent("OnProjectDagProjectHeaderUpdate", RoutingStrategy.Bubble,
        typeof(RoutedEventHandler), typeof(ProjectDagProjectHeader));

        public event RoutedEventHandler OnProjectDagProjectHeaderUpdate
        {
            add { AddHandler(ProjectDagProjectHeaderEvent, value); }
            remove { RemoveHandler(ProjectDagProjectHeaderEvent, value); }
        }
        #endregion


        private vwVakman vakman;
        public vwVakman Vakman
        {
            get { return vakman; }
            set { SetField(ref vakman, value, "Vakman"); }
        }

        public bool IsOwner { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsEnabledForDeleting { get; set; }

        public ProjectDagProjectHeader()
        {
            InitializeComponent();

            //slider.LowerSlider.SelectionStart = 0;
            //slider.LowerSlider.SelectionEnd = 100;

        }

        public void SetBullit()
        {
            if (ApplicationState.GetValue<int>(ApplicationVariables.intDefaultMode) == 1)
            {
                // deleting onzichtbaar
                borderDeleting.Visibility = System.Windows.Visibility.Hidden;

                if (IsOwner && IsEnabled)
                {
                    // groen
                    borderAdding.Fill = new SolidColorBrush(Colors.Green);
                    borderAdding.Stroke = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    // wit
                    borderAdding.Fill = new SolidColorBrush(Colors.White);
                    borderAdding.Stroke = new SolidColorBrush(Colors.Gray);
                }


                if (IsOwner && (bool)cbVakmanSelected.IsChecked)
                {
                    // zichtbaar
                    borderAdding.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    // onzichtbaar
                    borderAdding.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            else // deleting
            {
                // adding onzichtbaar
                borderAdding.Visibility = System.Windows.Visibility.Hidden;

                if (IsOwner && IsEnabledForDeleting)
                {
                    // groen
                    borderDeleting.Fill = new SolidColorBrush(Colors.Red);
                    borderDeleting.Stroke = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    // wit
                    borderDeleting.Fill = new SolidColorBrush(Colors.White);
                    borderDeleting.Stroke = new SolidColorBrush(Colors.Gray);
                }


                if (IsOwner && (bool)cbVakmanSelected.IsChecked)
                {
                    // zichtbaar
                    borderDeleting.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    // onzichtbaar
                    borderDeleting.Visibility = System.Windows.Visibility.Hidden;
                }

            }

        }

        private void btnGoToProject_Click(object sender, RoutedEventArgs e)
        {
            ApplicationState.SetValue(ApplicationVariables.intVakmanId, vakman.VakmanId);
            //ApplicationState.SetValue(ApplicationVariables.objVakman, vakman);

            //ProjectView pv = new ProjectView();
            //pv.Show();

            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
            MDRpersoon persoon = dbrepOriginal.GetContact(vakman.ContactIdOrigineel);

            // create the page and load all values
            Vakmannen.Detail.VakmanDetailView vdv = new Vakmannen.Detail.VakmanDetailView(persoon);

            // load the page into the contentcontrol
            MenuControl owner = Tools.FindVisualParent<MenuControl>(this);
            //po.PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            //po.PageOKButtonText = "PROJECT TOEVOEGEN";
            //vdv.Load();

            //vdv.tabControl.SelectedIndex = 1;

            //po.OkClick += po.ToevoegenAanVakman;
            //po.dgProjecten.MouseDoubleClick -= po.dgProjecten_MouseDoubleClick;
            //po.dgProjecten.MouseDoubleClick += po.dgProjecten_MouseDoubleClickForVakman;

            owner.PageGoToPage(vdv);


        }

        private void cbVakmanSelected_Checked(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                if (ApplicationState.GetValue<int>(ApplicationVariables.intDefaultMode) == 1) // adding
                {
                    if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsAdding) == null)
                    {
                        ApplicationState.SetValue(ApplicationVariables.listSelectedVakmanIdsAdding, new List<int>());
                    }

                    // voorkom duplicate entries
                    if (!ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsAdding).Contains(this.Vakman.VakmanId))
                    {
                        ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsAdding).Add(this.Vakman.VakmanId);
                    }
                }
                else // deleting
                {
                    if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsDeleting) == null)
                    {
                        ApplicationState.SetValue(ApplicationVariables.listSelectedVakmanIdsDeleting, new List<int>());
                    }

                    // voorkom duplicate entries
                    if (!ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsDeleting).Contains(this.Vakman.VakmanId))
                    {
                        ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsDeleting).Add(this.Vakman.VakmanId);
                    }
                }

            }
            else
            {

                if (ApplicationState.GetValue<int>(ApplicationVariables.intDefaultMode) == 1) // adding
                {
                    if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsAdding) == null)
                    {
                        ApplicationState.SetValue(ApplicationVariables.listSelectedVakmanIdsAdding, new List<int>());
                    }
                    else
                    {
                        ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsAdding).Remove(this.Vakman.VakmanId);
                    }
                }
                else // deleting
                {
                    if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsDeleting) == null)
                    {
                        ApplicationState.SetValue(ApplicationVariables.listSelectedVakmanIdsDeleting, new List<int>());
                    }
                    else
                    {
                        ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsDeleting).Remove(this.Vakman.VakmanId);
                    }
                }
            }

            ProjectDagView pdv = Tools.FindVisualParent<ProjectDagView>(this);

            if (pdv != null)
            {
                foreach (UIElement control in pdv.canvasWrapper.Children)
                {
                    if (control.GetType() == typeof(Border))
                    {
                        Border rectDefault = (Border)control;

                        // als het vierkantje bij deze checkbox hoort
                        if (rectDefault.Tag == this)
                        {
                            if (((CheckBox)sender).IsChecked == true)
                            {
                                rectDefault.Visibility = System.Windows.Visibility.Visible;
                            }
                            else
                            {
                                rectDefault.Visibility = System.Windows.Visibility.Hidden;
                            }
                        }

                    }
                }

                // als geen enkele checkbox geselecteerd is, dan knop weghalen, anders tonen
                if (ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsAdding).Count == 0)
                {
                    //pdv

                }
            }


            dbRepository dbrep = new dbRepository();
            Setting setting = dbrep.datacontext.Settings.Where(s => s.GebruikerId == ApplicationState.GetValue<int>(ApplicationVariables.intGebruikerId) && s.SettingsNaam == ApplicationVariables.strVakmanIdsAdding).FirstOrDefault();

            if (setting == null)
            {
                Setting addSetting = new Setting();
                addSetting.GebruikerId = ApplicationState.GetValue<int>(ApplicationVariables.intGebruikerId);
                addSetting.SettingsNaam = ApplicationVariables.strVakmanIdsAdding;
                addSetting.SettingsValue = ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsAdding).FromListToString();

                dbrep.datacontext.Settings.InsertOnSubmit(addSetting);
                dbrep.datacontext.SubmitChanges();
            }
            else
            {
                setting.SettingsValue = ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsAdding).FromListToString();
                dbrep.datacontext.SubmitChanges();
            }

            Setting setting2 = dbrep.datacontext.Settings.Where(s => s.GebruikerId == ApplicationState.GetValue<int>(ApplicationVariables.intGebruikerId) && s.SettingsNaam == ApplicationVariables.strVakmanIdsDeleting).FirstOrDefault();

            if (setting2 == null)
            {
                Setting addSetting2 = new Setting();
                addSetting2.GebruikerId = ApplicationState.GetValue<int>(ApplicationVariables.intGebruikerId);
                addSetting2.SettingsNaam = ApplicationVariables.strVakmanIdsDeleting;
                addSetting2.SettingsValue = ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsDeleting).FromListToString();

                dbrep.datacontext.Settings.InsertOnSubmit(addSetting2);
                dbrep.datacontext.SubmitChanges();
            }
            else
            {
                setting2.SettingsValue = ApplicationState.GetValue<List<int>>(ApplicationVariables.listSelectedVakmanIdsDeleting).FromListToString();
                dbrep.datacontext.SubmitChanges();
            }

            
            SetBullit();
        }

    }
}

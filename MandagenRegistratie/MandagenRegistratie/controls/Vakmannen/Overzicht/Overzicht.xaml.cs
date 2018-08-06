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
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Vakmannen.Overzicht
{
    /// <summary>
    /// Interaction logic for PerProjectleider.xaml
    /// </summary>
    public partial class Overzicht : MenuControl
    {
        public Overzicht()
        {
            InitializeComponent();

            //#if DEBUG
            //#else
            Reloaded += Load;

        }

        public List<Gebruiker> listProjectleiders { get; set; }
        public int Weeknummer { get; set; }


        public void Reload(List<int> listVakmannen, DateTime datum)
        {
            dbRepository dbrep = new dbRepository();
            // risky business, ik haal alleen de mandagen van 1 dag op, want meer hebben we
            // als het goed is niet nodig hier
            List<Mandagen> listMandagenTotal = dbrep.datacontext.Mandagens.Where(m => m.Begintijd >= datum && m.Eindtijd <= datum.AddDays(1) && m.Begintijd != m.Eindtijd).ToList();

            bool reloadAll = false;

            foreach (Control control in spHeader.Children)
            {

                if (control.GetType() == typeof(OverzichtHeader))
                {

                    OverzichtHeader pph = (OverzichtHeader)control;
                    foreach (int i in listVakmannen)
                    {
                        if (!pph.listVwVakmannen.Any(vv => vv.VakmanId == i))
                        {
                            reloadAll = true;

                        }
                    }

                    if (!reloadAll)
                    {
                        pph.Reload(listVakmannen, datum);
                    }
                }
            }




            if (!reloadAll)
            {

                foreach (Control control in spWeek1.Children)
                {

                    if (control.GetType() == typeof(OverzichtWeekSummary))
                    {

                        OverzichtWeekSummary ppws = (OverzichtWeekSummary)control;

                        if (datum >= ppws.dtDag && datum <= ppws.dtDag.AddDays(6))
                        {

                            ppws.ReLoad(listVakmannen, datum);
                        }


                    }
                }

                foreach (Control control in spWeek2.Children)
                {

                    if (control.GetType() == typeof(OverzichtWeekSummary))
                    {

                        OverzichtWeekSummary ppws = (OverzichtWeekSummary)control;

                        if (datum >= ppws.dtDag && datum <= ppws.dtDag.AddDays(6))
                        {

                            ppws.ReLoad(listVakmannen, datum);
                        }


                    }
                }
                foreach (Control control in spWeek3.Children)
                {

                    if (control.GetType() == typeof(OverzichtWeekSummary))
                    {

                        OverzichtWeekSummary ppws = (OverzichtWeekSummary)control;

                        if (datum >= ppws.dtDag && datum <= ppws.dtDag.AddDays(6))
                        {

                            ppws.ReLoad(listVakmannen, datum);
                        }


                    }
                }
                foreach (Control control in spWeek4.Children)
                {

                    if (control.GetType() == typeof(OverzichtWeekSummary))
                    {

                        OverzichtWeekSummary ppws = (OverzichtWeekSummary)control;

                        if (datum >= ppws.dtDag && datum <= ppws.dtDag.AddDays(6))
                        {

                            ppws.ReLoad(listVakmannen, datum);
                        }


                    }
                }
            }
            else
            {
                Load();
            }
        }


        public void Load()
        {

            try
            {
                DateTime starttijd = DateTime.Now;

                ClearSummary();

                DateTime startDag = Tools.CalculateWeekstart(ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay).AddDays(-14));
                dbRepository dbrep = new dbRepository();
                List<Mandagen> listMandagenTotal = dbrep.datacontext.Mandagens.Where(m => m.Begintijd >= startDag && m.Eindtijd <= startDag.AddDays(28)).ToList();


                int weeknumber1st = Tools.GetWeekNumber(startDag);
                int weeknumber2nd = Tools.GetWeekNumber(startDag.AddDays(7));
                int weeknumber3rd = Tools.GetWeekNumber(startDag.AddDays(14));
                int weeknumber4th = Tools.GetWeekNumber(startDag.AddDays(21));


                // voor 4 weken, week1, week2 header toevoegen aan stackpanel 1-4
                Label lbl1 = new Label();
                lbl1.Height = 21;
                lbl1.Content = "week " + weeknumber1st.ToString();
                lbl1.Width = 142;
                lbl1.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
                lbl1.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                lbl1.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                // ma di wo do vr za zo
                OverzichtWeekHeader pplwh1 = new OverzichtWeekHeader();
                pplwh1.Height = 26;
                pplwh1.dtDag = startDag;

                if (weeknumber1st % 2 == 0)
                {
                    spWeek1.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));
                    spWeek1Top.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));
                    pplwh1.IsEven = true;
                }
                else
                {
                    spWeek1.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
                    spWeek1Top.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
                    pplwh1.IsEven = false;
                }

                spWeek1Top.Children.Add(lbl1);

                pplwh1.Load();
                spWeek1Top.Children.Add(pplwh1);


                Label lbl2 = new Label();
                lbl2.Height = 21;
                lbl2.Content = "week " + (weeknumber2nd).ToString();
                lbl2.Width = 142;
                lbl2.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
                lbl2.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                lbl2.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                // ma di wo do vr za zo
                OverzichtWeekHeader pplwh2 = new OverzichtWeekHeader();
                pplwh2.Height = 26;
                pplwh2.dtDag = startDag.AddDays(7);

                if ((weeknumber2nd) % 2 == 0)
                {
                    spWeek2.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));
                    spWeek2Top.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));
                    pplwh2.IsEven = true;
                }
                else
                {
                    spWeek2.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
                    spWeek2Top.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
                    pplwh2.IsEven = false;
                }

                spWeek2Top.Children.Add(lbl2);

                pplwh2.Load();
                spWeek2Top.Children.Add(pplwh2);

                Label lbl3 = new Label();
                lbl3.Height = 21;
                lbl3.Content = "week " + (weeknumber3rd).ToString();
                lbl3.Width = 142;
                lbl3.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
                lbl3.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                lbl3.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                // ma di wo do vr za zo
                OverzichtWeekHeader pplwh3 = new OverzichtWeekHeader();
                pplwh3.Height = 26;
                pplwh3.dtDag = startDag.AddDays(14);

                if ((weeknumber3rd) % 2 == 0)
                {
                    spWeek3.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));
                    spWeek3Top.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));
                    pplwh3.IsEven = true;
                }
                else
                {
                    spWeek3.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
                    spWeek3Top.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
                    pplwh3.IsEven = false;
                }

                spWeek3Top.Children.Add(lbl3);

                pplwh3.Load();
                spWeek3Top.Children.Add(pplwh3);

                Label lbl4 = new Label();
                lbl4.Height = 21;
                lbl4.Content = "week " + (weeknumber4th).ToString();
                lbl4.Width = 142;
                lbl4.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
                lbl4.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                lbl4.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                // ma di wo do vr za zo
                OverzichtWeekHeader pplwh4 = new OverzichtWeekHeader();
                pplwh4.Height = 26;
                pplwh4.dtDag = startDag.AddDays(21);

                if ((weeknumber4th) % 2 == 0)
                {
                    spWeek4.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));
                    spWeek4Top.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));

                    pplwh4.IsEven = true;
                }
                else
                {
                    spWeek4.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
                    spWeek4Top.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
                    pplwh4.IsEven = false;
                }


                spWeek4Top.Children.Add(lbl4);

                pplwh4.Load();
                spWeek4Top.Children.Add(pplwh4);

                StackPanel spPrevious = new StackPanel();
                spPrevious.Height = 45;
                spPrevious.Width = 200;
                spPrevious.Orientation = Orientation.Horizontal;
                spPrevious.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                // lege space toevoegen aan eerste stackpanel
                Label lbl = new Label();
                lbl.Height = 45;
                lbl.Width = 110;
                lbl.Content = "Vakmannen";
                lbl.FontSize = 14;
                lbl.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                spPrevious.Children.Add(lbl);




                Button btnPreviousFast = new Button();
                btnPreviousFast.Content = " << ";
                btnPreviousFast.ToolTip = "week " + Tools.GetWeekNumber(startDag.AddDays(-28)).ToString();
                btnPreviousFast.Click += btnPreviousFast_Click;
                btnPreviousFast.Width = 20;
                btnPreviousFast.Margin = new Thickness(0, 0, 5, 0);
                btnPreviousFast.Height = 20;
                btnPreviousFast.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                spPrevious.Children.Add(btnPreviousFast);

                Button btnPrevious = new Button();
                btnPrevious.Content = " < ";
                btnPrevious.ToolTip = "week " + Tools.GetWeekNumber(startDag.AddDays(-7)).ToString();
                btnPrevious.Click += btnPrevious_Click;
                btnPrevious.Width = 20;
                btnPrevious.Margin = new Thickness(0, 0, 5, 0);
                btnPrevious.Height = 20;
                btnPrevious.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                spPrevious.Children.Add(btnPrevious);


                spHeaderTop.Children.Add(spPrevious);

                Border bbDivider = new Border();
                bbDivider.BorderThickness = new Thickness(1);
                bbDivider.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));

                spHeaderTop.Children.Add(bbDivider);


                StackPanel spTopNext = new StackPanel();
                spTopNext.Height = 45;
                spTopNext.Width = 50;
                spTopNext.Orientation = Orientation.Horizontal;
                spTopNext.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                spTopNext.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                Button btnNext = new Button();
                btnNext.Content = " > ";
                btnNext.ToolTip = "week " + (Tools.GetWeekNumber(startDag.AddDays(7).AddDays(21))).ToString();
                btnNext.Width = 20;
                btnNext.Margin = new Thickness(5, 0, 0, 0);
                btnNext.Height = 20;
                btnNext.Click += btnNext_Click;
                btnNext.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                spTopNext.Children.Add(btnNext);

                Button btnNextFast = new Button();
                btnNextFast.Content = " >> ";
                btnNextFast.ToolTip = "week " + (Tools.GetWeekNumber(startDag.AddDays(28).AddDays(21))).ToString();
                btnNextFast.Width = 20;
                btnNextFast.Margin = new Thickness(5, 0, 0, 0);
                btnNextFast.Height = 20;
                btnNextFast.Click += btnNextFast_Click;
                btnNextFast.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                spTopNext.Children.Add(btnNextFast);

                spNextTop.Children.Add(spTopNext);

                Border bbDivider2 = new Border();
                bbDivider2.BorderThickness = new Thickness(1);
                bbDivider2.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));
                bbDivider2.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

                spNextTop.Children.Add(bbDivider2);

                bool isFirstSummary = false;

                // voor elke projectleider in deze lijst, een header aanmaken met daarin alle projecten
                //foreach (Gebruiker objProjectleider in listProjectleiders)
                //{
                // lijst genereren met alle projecten die van toepassing zijn op deze projectleider
                OverzichtHeader pph = new OverzichtHeader();

                //pph.projectleider = objProjectleider;
                pph.startdatum = startDag.AddDays(-14);
                pph.einddatum = startDag.AddDays(28);
                dbOriginalRepository dboRep = new dbOriginalRepository();
                List<vwVakman> listVwVakmannen = dbrep.GetVakmannenAll(pph.startdatum, pph.einddatum).OrderBy(vm => vm.voornaam).ToList();

                List<MDRpersoon> listMDRPersoons = dboRep.Persoons(true).Where(m => listVwVakmannen.Select(a => a.ContactIdOrigineel).ToList<int>().Contains(m.persoon_ID)).ToList();
                List<Vakman> listVakmannen = dbrep.datacontext.Vakmans.Where(m => listVwVakmannen.Select(a => a.VakmanId).ToList<int>().Contains(m.VakmanId)).ToList();




                pph.listVwVakmannen = listVwVakmannen;
                pph.listMDRpersoons = listMDRPersoons;
                pph.listVakmannen = listVakmannen;


                pph.Load();


                // header toevoegen aan eerste stackpanel
                // naam van de projectleider

                // projecten uit de projectenlijst toevoegen aan header object


                // voor de stackpanels van de 4 weken 
                // summary control toevoegen
                OverzichtWeekSummary ppws1 = new OverzichtWeekSummary();
                ppws1.listVwVakmannen = listVwVakmannen;
                ppws1.listMDRpersoons = listMDRPersoons;
                ppws1.listVakmannen = listVakmannen;
                //ppws1.objProjectleider = objProjectleider;
                ppws1.dtDag = startDag;
                ppws1.weeknummer = weeknumber1st;
                ppws1.IsWeekVisible = isFirstSummary;
                ppws1.listMandagenTotal = listMandagenTotal;

                ppws1.Load();



                OverzichtWeekSummary ppws2 = new OverzichtWeekSummary();
                ppws2.listVwVakmannen = listVwVakmannen;
                ppws2.listMDRpersoons = listMDRPersoons;
                ppws2.listVakmannen = listVakmannen;
                //ppws2.objProjectleider = objProjectleider;
                ppws2.dtDag = startDag.AddDays(7);
                ppws2.weeknummer = weeknumber1st + 1;
                ppws2.IsWeekVisible = isFirstSummary;
                ppws2.listMandagenTotal = listMandagenTotal;
                ppws2.Load();


                OverzichtWeekSummary ppws3 = new OverzichtWeekSummary();
                ppws3.listVwVakmannen = listVwVakmannen;
                ppws3.listMDRpersoons = listMDRPersoons;
                ppws3.listVakmannen = listVakmannen;
                //ppws3.objProjectleider = objProjectleider;
                ppws3.dtDag = startDag.AddDays(14);
                ppws3.weeknummer = weeknumber1st + 2;
                ppws3.IsWeekVisible = isFirstSummary;
                ppws3.listMandagenTotal = listMandagenTotal;
                ppws3.Load();

                OverzichtWeekSummary ppws4 = new OverzichtWeekSummary();
                ppws4.listVwVakmannen = listVwVakmannen;
                ppws4.listMDRpersoons = listMDRPersoons;
                ppws4.listVakmannen = listVakmannen;
                //ppws4.objProjectleider = objProjectleider;
                ppws4.dtDag = startDag.AddDays(21);
                ppws4.weeknummer = weeknumber1st + 3;
                ppws4.IsWeekVisible = isFirstSummary;
                ppws4.listMandagenTotal = listMandagenTotal;
                ppws4.Load();

                int counti = 0;
                // alleen toevoegen als minimaal 1 overzicht uren bevat, anders niks toevoegen
                if (ppws1.HasHours || ppws2.HasHours || ppws3.HasHours || ppws4.HasHours)
                {
                    spHeader.Children.Add(pph);
                    spWeek1.Children.Add(ppws1);
                    spWeek2.Children.Add(ppws2);
                    spWeek3.Children.Add(ppws3);
                    spWeek4.Children.Add(ppws4);
                    counti++;
                }

                // for loop eindigen met false zetten van isFirstSummary
                isFirstSummary = false;



                CheckLayout();


                DateTime eindtijd = DateTime.Now;

                TimeSpan tijdsduur = TimeSpan.FromTicks(eindtijd.Ticks - starttijd.Ticks);

                //pcWindow.Title = tijdsduur.Seconds.ToString() + "." + tijdsduur.Milliseconds.ToString();

                // clear caches
                ApplicationState.SetValue("listProjects", null);
                ApplicationState.SetValue("listPersoons", null);

            }
            catch (Exception ex)
            {

                Logging log = new Logging();
                log.Log(ex.Message);

            }

        }

        void btnPrevious_Click(object sender, RoutedEventArgs e)
        {

            Mouse.OverrideCursor = Cursors.Wait;

            DateTime startdate = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);

            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, startdate.AddDays(-7));

            Load();
            Mouse.OverrideCursor = null;
        }

        void btnPreviousFast_Click(object sender, RoutedEventArgs e)
        {

            Mouse.OverrideCursor = Cursors.Wait;

            DateTime startdate = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);

            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, startdate.AddDays(-28));

            Load();
            Mouse.OverrideCursor = null;
        }

        private void ClearSummary()
        {
            spHeader.Children.Clear();
            spWeek1.Children.Clear();
            spWeek2.Children.Clear();
            spWeek3.Children.Clear();
            spWeek4.Children.Clear();
            //spNext.Children.Clear();
            spHeaderTop.Children.Clear();
            spWeek1Top.Children.Clear();
            spWeek2Top.Children.Clear();
            spWeek3Top.Children.Clear();
            spWeek4Top.Children.Clear();
            spNextTop.Children.Clear();

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            DateTime startdate = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);

            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, startdate.AddDays(7));

            Load();

            Mouse.OverrideCursor = null;

        }

        private void btnNextFast_Click(object sender, RoutedEventArgs e)
        {

            Mouse.OverrideCursor = Cursors.Wait;

            DateTime startdate = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);

            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, startdate.AddDays(28));

            Load();
            Mouse.OverrideCursor = null;
        }

        private void MenuControl_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            //if (Window.GetWindow(this) != null)
            //{
            //    svScrollviewerWrapper.Width = Window.GetWindow(this).ActualWidth - 17;
            //}

            //if (Window.GetWindow(this) != null)
            //{
                //svScrollviewerWrapper.MaxWidth = Window.GetWindow(this).ActualWidth - 17;
                //svScrollviewerWrapper.MaxWidth = Window.GetWindow(this).ActualWidth - 40;


            //}

            CheckLayout();

            //spWeek1.Width = (this.Width - 200) / 4;
            //spWeek2.Width = (this.Width - 200) / 4;
            //spWeek3.Width = (this.Width - 200) / 4;
            //spWeek4.Width = (this.Width - 200) / 4;

        }

        public void CheckLayout()
        {
            if (Window.GetWindow(this) != null)
            {

                //svScrollviewerWrapper.Height = Window.GetWindow(this).ActualHeight - 214;

                //double widthAvailable = Window.GetWindow(this).ActualWidth - 830;

                //if (widthAvailable > 320)
                //{
                //    spHeader.Width = widthAvailable / 2;
                //    spHeaderTop.Width = widthAvailable / 2;
                //    spNext.Width = (widthAvailable / 2);
                //    spNextTop.Width = widthAvailable / 2;
                //}
                //else
                //{
                //    spHeader.Width = 160;
                //    spHeaderTop.Width = 160;
                //    spNext.Width = widthAvailable - 160;
                //    spNextTop.Width = widthAvailable - 160;
                //}

                //foreach (UIElement sp in spHeaderTop.Children)
                //{
                //    if (sp.GetType() == typeof(StackPanel))
                //    {
                //        ((StackPanel)sp).Width = spHeaderTop.Width;

                //        foreach (UIElement ui in ((StackPanel)sp).Children)
                //        {
                //            if (ui.GetType() == typeof(Label))
                //            {
                //                ((Label)ui).Width = spHeaderTop.Width - 50;
                //            }
                //        }
                //    }
                //}

                //double totalcalc = spHeader.Width + spWeek1.Width + spWeek2.Width + spWeek3.Width + spWeek4.Width + spNext.Width;


                svScrollviewerWrapper.Height = Window.GetWindow(this).ActualHeight - 214;

                double widthAvailable = Window.GetWindow(this).ActualWidth - 830;

                // centreer
                if (widthAvailable > 600)
                {
                    spHeader.Width = widthAvailable / 2;
                    spHeaderTop.Width = widthAvailable / 2;
                    spNext.Width = (widthAvailable / 2);
                    spNextTop.Width = widthAvailable / 2;
                }
                else if (widthAvailable > 350)
                {
                    spHeader.Width = 350 - 50;
                    spHeaderTop.Width = 350 - 50;
                    spNext.Width = (widthAvailable - 350) + 50;
                    spNextTop.Width = (widthAvailable - 350) + 50;
                }
                else
                {
                    spHeader.Width = widthAvailable - 50;
                    spHeaderTop.Width = widthAvailable - 50;
                    spNext.Width = 50;
                    spNextTop.Width = 50;
                }


                //MessageBox.Show(widthAvailable.ToString());

                foreach (UIElement sp in spHeaderTop.Children)
                {
                    if (sp.GetType() == typeof(StackPanel))
                    {
                        ((StackPanel)sp).Width = spHeaderTop.Width;

                        foreach (UIElement ui in ((StackPanel)sp).Children)
                        {
                            if (ui.GetType() == typeof(Label))
                            {
                                ((Label)ui).Width = spHeaderTop.Width - 50;
                            }
                        }
                    }
                }

                double totalcalc = spHeader.Width + spWeek1.Width + spWeek2.Width + spWeek3.Width + spWeek4.Width + spNext.Width;

                RecalcHeaderTexts();

                //MessageBox.Show("Screen:" +Window.GetWindow(this).ActualWidth.ToString()+ ";Availablewidth:" + widthAvailable.ToString() + ";Left:" + spHeader.Width.ToString() + ";Right:" + spNext.Width.ToString() + "TOTALCALC:" +totalcalc.ToString());
            }

        }


        public void RecalcHeaderTexts()
        {
            foreach (UIElement ui in spHeader.Children)
            {
                if (ui.GetType() == typeof(OverzichtHeader))
                {
                    ((OverzichtHeader)ui).RecalcHeaderTexts(spHeader.Width);
                }
            }
        }
    }
}

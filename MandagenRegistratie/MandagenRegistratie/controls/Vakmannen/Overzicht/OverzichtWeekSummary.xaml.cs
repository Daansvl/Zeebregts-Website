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
using MandagenRegistratie.tools;
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Vakmannen.Overzicht
{
    /// <summary>
    /// Interaction logic for PerProjectleiderWeekSummary.xaml
    /// </summary>
    public partial class OverzichtWeekSummary : UserControl
    {

        public List<vwVakman> listVwVakmannen { get; set; }
        public List<Vakman> listVakmannen { get; set; }
        public List<MDRpersoon> listMDRpersoons { get; set; }
        public DateTime dtDag { get; set; }
        public Gebruiker objProjectleider { get; set; }
        public int weeknummer { get; set; }
        public bool IsWeekVisible { get; set; }

        public bool HasHours { get; set; }


        public OverzichtWeekSummary()
        {
            InitializeComponent();
        }

        public void ReLoad(List<int> listVakmannen2, DateTime datum )
        {

            dbRepository dbrep = new dbRepository();
            dbOriginalRepository dbo = new dbOriginalRepository();
            DbTools dbtools = new DbTools();

            // haal alle mandegen van deze week
            //Logfile.Log("Haal alle mandagen op (database)");

            // risky business, idem zie projectoverzicht
            List<Mandagen> listMandagenTotal = dbrep.datacontext.Mandagens.Where(m => m.Begintijd >= datum && m.Eindtijd <= datum.AddDays(1) && m.Begintijd != m.Eindtijd).ToList();
            
            foreach (Control control in spUren.Children)
            {

                if (control.GetType() == typeof(Button))
                {
                    // per dag extra tooltip informatie:
                    //string strTooltip = string.Empty;
                    StackPanel strTooltip = new StackPanel();
                    //Logfile.Log("volgende dag, dag " + i.ToString());

                    // filter alleen de mandagen voor vandaag,
                    // van de huidige vakman waar we naar kijken
                    //Logfile.Log("filter mandagen");
                    Button lblDag = (Button)control;
                    vwVakman objVakman = ((vwVakman)((List<object>)lblDag.Tag)[0]);
                    DateTime objDatum = ((DateTime)((List<object>)lblDag.Tag)[1]);

                    if (objDatum == datum && listVakmannen2.Contains(objVakman.VakmanId))
                    {
                        // EXPERIMENT uitzetten LET OP: Juraci
                        List<Mandagen> listMandagen = listMandagenTotal.Where(m => m.VakmanId == objVakman.VakmanId && m.Begintijd >= objDatum && m.Eindtijd <= objDatum.AddDays(1)).ToList();

                        // als in deze pool van mandagen een aanvraag zit:
                        // vandaag, ander project, geen projectleider, niet bevestigd (dus aanvraag) 
                        //Logfile.Log("filter aanvragen");
                        //List<Mandagen> listAanvragen = listMandagenTotal.Where(m => m.VakmanId == objVakman.VakmanId && m.Begintijd >= dtDag.AddDays(i) && m.Eindtijd <= dtDag.AddDays(i + 1) && !m.Status).ToList();


                        //long totalTicks = 0;
                        bool hasAanvraag = false;
                        int totalHours = 0;
                        int totalMinutes = 0;


                        // extra stukje als experiment LET OP: Juraci

                        //totalHours = listMandagenTotal.Where(m => m.Status && m.VakmanId == objVakman.VakmanId && m.Begintijd >= dtDag.AddDays(i) && m.Eindtijd <= dtDag.AddDays(i + 1)).Sum(m => m.Uren);
                        //totalMinutes = listMandagenTotal.Where(m => m.Status && m.VakmanId == objVakman.VakmanId && m.Begintijd >= dtDag.AddDays(i) && m.Eindtijd <= dtDag.AddDays(i + 1)).Sum(m => m.Minuten);

                        //hasAanvraag = listMandagenTotal.Any(m => !m.Status && m.VakmanId == objVakman.VakmanId && m.Begintijd >= dtDag.AddDays(i) && m.Eindtijd <= dtDag.AddDays(i + 1));

                        strTooltip = dbtools.AddTooltipAanvraagAll(dbtools.GetVakman(objVakman), new Project(), objDatum, false, true);


                        hasAanvraag = listMandagen.Any(a => !a.Status);
                        totalHours = listMandagen.Sum(a => a.Uren);
                        totalMinutes = listMandagen.Sum(a => a.Minuten);

                        //foreach (Mandagen mandag in listMandagen)
                        //{
                        //    //Logfile.Log("volgende mandag");
                        //    // bereken de som der uren 
                        //    //ALLEEN ALS HET EEN BEVESTIGD UUR IS
                        //    if (mandag.Status)
                        //    {
                        //        //long lnMandagTicks = (mandag.Eindtijd.Ticks - mandag.Begintijd.Ticks);
                        //        //totalTicks += lnMandagTicks;
                        //        //TimeSpan tsMandag = TimeSpan.FromTicks(lnMandagTicks);

                        //        //Logfile.Log("haal project titel bij daan (database)");
                        //        MDRproject objProject = dbo.GetProject((int)mandag.Project.ProjectNr, true);

                        //        //Logfile.Log("bereken uren");
                        //        totalHours += mandag.Uren;
                        //        totalMinutes += mandag.Minuten;

                        //        //Logfile.Log("set tooltip");
                        //        if (objProject != null)
                        //        {
                        //            strTooltip += Environment.NewLine;
                        //            //strTooltip += "(" + ((int)Math.Ceiling(tsMandag.TotalHours)).ToString() + "u) " + objProject.project_NR.ToString(5) + " " + objProject.naam_project;
                        //            strTooltip += "(" + mandag.Uren.ToString() + ":" + (mandag.Minuten < 10 ? "0" : "") + mandag.Minuten.ToString() + ") " + objProject.project_NR.ToString(5) + " " + objProject.naam_project;
                        //        }

                        //    }

                        //    // als de betreffende mandag een aanvraag is
                        //    // OF er een aanvraag is die deze overlapt qua tijden, voor een ander project is, maar voor dezelfde vakman, en nog niet geannulleerd dus lopende aanvraag
                        //    //if ((!mandag.Status && !mandag.Geannulleerd) || listAanvragen.Any(a => a.Eindtijd > mandag.Begintijd && a.Begintijd < mandag.Eindtijd && a.ProjectId != mandag.ProjectId && a.VakmanId == mandag.VakmanId && !a.Geannulleerd))
                        //    //if ((!mandag.Status && mandag.ProjectleiderId == ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).ProjectleiderId) || (!mandag.Status && !mandag.Geannulleerd && mandag.ProjectleiderId != ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).ProjectleiderId))
                        //    if (!mandag.Status)
                        //    {
                        //        hasAanvraag = true;
                        //    }
                        //}
                        //// CHECKECN


                        //Logfile.Log("klaar met loopje mandagen");

                        //TimeSpan totalTimespan = TimeSpan.FromTicks(totalTicks);
                        lblDag.Width = 24;
                        lblDag.Height = 17;
                        lblDag.Margin = new Thickness(2, 2, 2, 2);
                        lblDag.FontSize = 11;
                        lblDag.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;

                        lblDag.Padding = new Thickness(0, -1, 2, 0);

                        //Logfile.Log("haal contact bij daan (database)");

                        MDRpersoon objPersoon = dbo.GetContact(objVakman.ContactIdOrigineel, true);

                        //Logfile.Log("toon naam");
                        lblDag.ToolTip = ToonNaam(objPersoon) + strTooltip;

                        int totalHoursRounded = 0;
                        //totalHoursRounded = (int)Math.Ceiling(totalTimespan.TotalHours);
                        int minutesRemainder = totalMinutes % 60;

                        //Logfile.Log("bereken totaal uren om te tonen");
                        totalHoursRounded = totalHours + (int)((totalMinutes - minutesRemainder) / 60) + (minutesRemainder == 0 ? 0 : 1);

                        //Logfile.Log("bereken kleuren en andere toon opties");
                        lblDag.Content = totalHoursRounded == 0 ? "" : totalHoursRounded.ToString();
                        bool blnUseInvisibleColorsAKASwitchColors = false;

                        // 0 niet tonen
                        if (totalHoursRounded == 0 && !hasAanvraag)
                        {
                            blnUseInvisibleColorsAKASwitchColors = true;
                            //lblDag.Visibility = System.Windows.Visibility.Hidden;
                        }
                        else
                        {
                            HasHours = true;
                        }

                        List<object> tag = new List<object>();
                        tag.Add(objVakman);
                        tag.Add(objDatum);

                        lblDag.Tag = tag;




                        // END CHECKEN


                        // standaard kleuren van de border
                        if (weeknummer % 2 == 0)
                        {
                            if (blnUseInvisibleColorsAKASwitchColors)
                            {
                                // use this color to make it invisible (same color as background)
                                lblDag.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));
                            }
                            else
                            {
                                // original color
                                lblDag.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
                            }
                        }
                        else
                        {
                            if (blnUseInvisibleColorsAKASwitchColors)
                            {
                                // use this color to make it invisible (same color as background)
                                lblDag.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
                            }
                            else
                            {
                                lblDag.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));
                            }
                        }


                        // zwarte rand maken bij een aanvraag
                        if (hasAanvraag)
                        {
                            double[] dimensions = new double[] { 2, 2 };
                            Rectangle dottedRectangle = new Rectangle();
                            dottedRectangle.Stroke = new SolidColorBrush(Colors.Gray);
                            dottedRectangle.SnapsToDevicePixels = true;
                            dottedRectangle.Height = 2;
                            dottedRectangle.StrokeThickness = 2;
                            dottedRectangle.Height = 24;
                            dottedRectangle.Width = 45;
                            dottedRectangle.StrokeDashArray = new DoubleCollection(dimensions);

                            VisualBrush vb = new VisualBrush();
                            vb.Visual = dottedRectangle;

                            lblDag.BorderBrush = vb; // new SolidColorBrush(Colors.Black); // (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000"));
                        }



                        //Logfile.Log("voeg de dag toe");


                    }
                }
            }


            //Logfile.Log("klaar met vakmannen");
        }


        public void Load()
        {
            DbTools dbtools = new DbTools();

            // datums onder tooltip
            lblMa.ToolTip = GetDagTooltip(dtDag);
            lblDi.ToolTip = GetDagTooltip(dtDag.AddDays(1));
            lblWo.ToolTip = GetDagTooltip(dtDag.AddDays(2));
            lblDo.ToolTip = GetDagTooltip(dtDag.AddDays(3));
            lblVr.ToolTip = GetDagTooltip(dtDag.AddDays(4));
            lblZa.ToolTip = GetDagTooltip(dtDag.AddDays(5));
            lblZo.ToolTip = GetDagTooltip(dtDag.AddDays(6));

            if (!IsWeekVisible)
            {
                // datums onder tooltip
                lblMa.Visibility = System.Windows.Visibility.Collapsed;
                lblDi.Visibility = System.Windows.Visibility.Collapsed;
                lblWo.Visibility = System.Windows.Visibility.Collapsed;
                lblDo.Visibility = System.Windows.Visibility.Collapsed;
                lblVr.Visibility = System.Windows.Visibility.Collapsed;
                lblZa.Visibility = System.Windows.Visibility.Collapsed;
                lblZo.Visibility = System.Windows.Visibility.Collapsed;
                bbDivider.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (weeknummer % 2 == 0)
            {
                bbDivider.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
            }
            else
            {
                bbDivider.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));
            }


            dbRepository dbrep = new dbRepository();
            dbOriginalRepository dbo = new dbOriginalRepository();

            // haal alle mandegen van deze week
            //Logfile.Log("Haal alle mandagen op (database)");
            //listMandagenTotal = listMandagenTotal.Where(m => m.Begintijd >= dtDag && m.Eindtijd <= dtDag.AddDays(7) && m.Begintijd != m.Eindtijd).ToList();

            foreach (vwVakman objVwVakman in listVwVakmannen)
            {
                // 
                //Logfile.Log("volgende vakman");
                Vakman objVakman = listVakmannen.FirstOrDefault(a=>a.VakmanId == objVwVakman.VakmanId);
                MDRpersoon objPersoon = listMDRpersoons.FirstOrDefault(a => a.persoon_ID == objVwVakman.ContactIdOrigineel);

                for (int i = 0; i < 7; i++)
                {
                    // per dag extra tooltip informatie:
                    StackPanel strTooltip = new StackPanel();
                    //StackPanel strTooltipAanvraag = new StackPanel();

                    //Logfile.Log("volgende dag, dag " + i.ToString());

                    //strTooltip.AddText(ToonNaam(objPersoon)); 

                    // filter alleen de mandagen voor vandaag,
                    // van de huidige vakman waar we naar kijken
                    //Logfile.Log("filter mandagen");

                    // EXPERIMENT uitzetten LET OP: Juraci
                    List<Mandagen> listMandagen = listMandagenTotal.Where(m => m.VakmanId == objVwVakman.VakmanId && m.Begintijd >= dtDag.AddDays(i) && m.Eindtijd <= dtDag.AddDays(i + 1)).OrderBy(m => m.Begintijd).ToList();

                    // als in deze pool van mandagen een aanvraag zit:
                    // vandaag, ander project, geen projectleider, niet bevestigd (dus aanvraag) 
                    //Logfile.Log("filter aanvragen");
                    //List<Mandagen> listAanvragen = listMandagenTotal.Where(m => m.VakmanId == objVakman.VakmanId && m.Begintijd >= dtDag.AddDays(i) && m.Eindtijd <= dtDag.AddDays(i + 1) && !m.Status).ToList();
                    // als in deze pool van mandagen een aanvraag zit:
                    // vandaag, ander project, geen projectleider, wel bevestigd (dus waar dit de aanvraag van is) 
                    //List<Mandagen> listMandagenWaarDitDeAanvraagVanIs = listMandagenTotal.Where(m => m.ProjectleiderId != ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).ProjectleiderId && m.VakmanId == objVakman.VakmanId && m.Begintijd >= dtDag.AddDays(i) && m.Eindtijd <= dtDag.AddDays(i + 1) && m.Status).OrderBy(m => m.Begintijd).ToList();
          
                    
                    //long totalTicks = 0;
                    bool hasAanvraag = false;
                    int totalHours = 0;
                    int totalMinutes = 0;


                    // extra stukje als experiment LET OP: Juraci

                    //totalHours = listMandagenTotal.Where(m => m.Status && m.VakmanId == objVakman.VakmanId && m.Begintijd >= dtDag.AddDays(i) && m.Eindtijd <= dtDag.AddDays(i + 1)).Sum(m => m.Uren);
                    //totalMinutes = listMandagenTotal.Where(m => m.Status && m.VakmanId == objVakman.VakmanId && m.Begintijd >= dtDag.AddDays(i) && m.Eindtijd <= dtDag.AddDays(i + 1)).Sum(m => m.Minuten);

                    //hasAanvraag = listMandagenTotal.Any(m => !m.Status && m.VakmanId == objVakman.VakmanId && m.Begintijd >= dtDag.AddDays(i) && m.Eindtijd <= dtDag.AddDays(i + 1));

                    int vakmanId = -1;



                    hasAanvraag = listMandagen.Any(a => !a.Status);
                    totalHours = listMandagen.Where(a=> a.Status).Sum(a => a.Uren);
                    totalMinutes = listMandagen.Where(a => a.Status).Sum(a => a.Minuten);

                    bool hasUren = totalHours > 0 || totalMinutes > 0;

                    strTooltip = dbtools.AddTooltipAanvraagAll(objVakman, new Project(), dtDag.AddDays(i), false, true, listMandagenTotal.Where(m => m.Begintijd >= dtDag.AddDays(i) && m.Eindtijd <= dtDag.AddDays(i + 1)).ToList(), hasAanvraag, hasUren);


                    //TimeSpan totalTimespan = TimeSpan.FromTicks(totalTicks);
                    Button lblDag = new Button();
                    lblDag.Width = 24;
                    lblDag.Height = 17;
                    lblDag.Margin = new Thickness(2, 2, 2, 2);
                    lblDag.FontSize = 11;
                    lblDag.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;

                    lblDag.Padding = new Thickness(0, -1, 2, 0);

                    ToolTipService.SetShowDuration(lblDag, 20000);

                    lblDag.ToolTip = strTooltip;

                    int totalHoursRounded = 0;
                    //totalHoursRounded = (int)Math.Ceiling(totalTimespan.TotalHours);
                    int minutesRemainder = totalMinutes % 60;

                    //Logfile.Log("bereken totaal uren om te tonen");
                    totalHoursRounded = totalHours + (int)((totalMinutes - minutesRemainder) / 60) + (minutesRemainder == 0 ? 0 : 1);

                    //if (totalTimespan.Minutes > 0)
                    //{
                    //    totalHoursRounded ++;
                    //}

                    //Logfile.Log("bereken kleuren en andere toon opties");
                    lblDag.Content = totalHoursRounded == 0 ? "" : totalHoursRounded.ToString();
                    bool blnUseInvisibleColorsAKASwitchColors = false;

                    // 0 niet tonen
                    if (totalHoursRounded == 0 && !hasAanvraag)
                    {
                        blnUseInvisibleColorsAKASwitchColors = true;
                        //lblDag.Visibility = System.Windows.Visibility.Hidden;
                    }
                    else
                    {
                        HasHours = true;
                    }

                    List<object> tag = new List<object>();
                    tag.Add(objVwVakman);
                    tag.Add(dtDag.AddDays(i));

                    lblDag.Tag = tag;

                    // standaard kleuren van de border
                    if (weeknummer % 2 == 0)
                    {
                        if (blnUseInvisibleColorsAKASwitchColors)
                        {
                            // use this color to make it invisible (same color as background)
                            lblDag.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));
                        }
                        else
                        {
                            // original color
                            lblDag.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
                        }
                    }
                    else
                    {
                        if (blnUseInvisibleColorsAKASwitchColors)
                        {
                            // use this color to make it invisible (same color as background)
                            lblDag.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
                        }
                        else
                        {
                            lblDag.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));
                        }
                    }

                    // zwarte rand maken bij een aanvraag
                    if (hasAanvraag)
                    {
                        double[] dimensions = new double[] { 2, 2 };
                        Rectangle dottedRectangle = new Rectangle();
                        dottedRectangle.Stroke = new SolidColorBrush(Colors.Gray);
                        dottedRectangle.SnapsToDevicePixels = true;
                        dottedRectangle.Height = 2;
                        dottedRectangle.StrokeThickness = 2;
                        dottedRectangle.Height = 24;
                        dottedRectangle.Width = 45;
                        dottedRectangle.StrokeDashArray = new DoubleCollection(dimensions);

                        VisualBrush vb = new VisualBrush();
                        vb.Visual = dottedRectangle;

                        lblDag.BorderBrush = vb; // new SolidColorBrush(Colors.Black); // (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000"));
                    }

                    // standaard achtergrondkleuren van de dagen
                    if (weeknummer % 2 == 0)
                    {
                        lblDag.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));
                    }
                    else
                    {
                        lblDag.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
                    }

                    
                    lblDag.BorderThickness = new Thickness(2);

                    //lblDag.MouseLeftButtonDown += lblDag_Click;
                    lblDag.Click += lblDag_Click;
                    spUren.Children.Add(lblDag);

                    //Logfile.Log("voeg de dag toe");

                }
            }


            //Logfile.Log("klaar met vakmannen");
        }

        void lblDag_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            ApplicationState.SetValue(ApplicationVariables.intVakmanViewModus, 1);

            vwVakman cVakman = (vwVakman)((List<object>)((Button)sender).Tag)[0];
            DateTime selectedDay = (DateTime)((List<object>)((Button)sender).Tag)[1];

            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, selectedDay);
            ApplicationState.SetValue(ApplicationVariables.intVakmanId, cVakman.VakmanId);

            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();

            MDRpersoon objPersoon = dbrepOriginal.GetContact(cVakman.ContactIdOrigineel);

            // create the page and load all values
            MandagenRegistratie.controls.Vakmannen.Detail.VakmanDetailView vm = new MandagenRegistratie.controls.Vakmannen.Detail.VakmanDetailView(objPersoon);
            //vm.Load();

            
            // load the page into the contentcontrol
            MenuControl owner = Tools.FindVisualParent<MenuControl>(this);

            owner.PageGoToPage(vm);

            Mouse.OverrideCursor = null;

        }


        public string ToonNaam(MDRpersoon objPersoon)
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

        public string GetDagTooltip(DateTime dag)
        {
            return dag.ToString("dd-MM-yyyy");
        }



        public List<Mandagen> listMandagenTotal { get; set; }
    }
}

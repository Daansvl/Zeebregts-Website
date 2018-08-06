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

namespace MandagenRegistratie.controls.Projecten.Overzicht
{
    /// <summary>
    /// Interaction logic for PerProjectleiderWeekSummary.xaml
    /// </summary>
    public partial class PerProjectleiderWeekSummary : UserControl
    {

        public List<Mandagen> listMandagenTotal { get; set; }
        public List<Project> listProjecten { get; set; }
        public DateTime dtDag { get; set; }
        public Gebruiker objProjectleider { get; set; }
        public int weeknummer { get; set; }
        public bool IsWeekVisible { get; set; }
        public bool blnOwner;

        public bool HasHours { get; set; }


        public PerProjectleiderWeekSummary()
        {
            InitializeComponent();
        }

        public void Reload(List<int> listVakmannen, DateTime datum)
        {
            foreach (Control control in spUren.Children)
            {
                if (control.GetType() == typeof(Button))
                {
                    Button lbl = (Button)control;

                    DateTime d = (DateTime)((List<object>)lbl.Tag)[1];
                    if (datum == d)
                    {
                        Project project = (Project)((List<object>)lbl.Tag)[0];

                        if (project.Mandagens.Any(m => listVakmannen.Contains(m.VakmanId)))
                        {
                            calculateDag(lbl, project, datum);
                        }
                    }
                }
            }
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
                lblMa.Visibility = System.Windows.Visibility.Hidden;
                lblDi.Visibility = System.Windows.Visibility.Hidden;
                lblWo.Visibility = System.Windows.Visibility.Hidden;
                lblDo.Visibility = System.Windows.Visibility.Hidden;
                lblVr.Visibility = System.Windows.Visibility.Hidden;
                lblZa.Visibility = System.Windows.Visibility.Hidden;
                lblZo.Visibility = System.Windows.Visibility.Hidden;
                bbDivider.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (weeknummer % 2 == 0)
            {
                bbDivider.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
            }
            else
            {
                bbDivider.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5D9F1"));
            }

            int intProjectleider = ApplicationState.GetValue<int>(ApplicationVariables.intProjectleider);
            blnOwner = objProjectleider.ProjectleiderId == intProjectleider;

            dbRepository dbrep = new dbRepository();
            dbOriginalRepository dbo = new dbOriginalRepository();

            // haal alle mandegen van deze week
            //List<Mandagen> listMandagenTotal = dbrep.datacontext.Mandagens.Where(m => m.Begintijd >= dtDag && m.Eindtijd <= dtDag.AddDays(7) && m.Begintijd != m.Eindtijd).ToList();

            foreach (Project objProject in listProjecten)
            {
                // 

                for (int i = 0; i < 7; i++)
                {
                    // per dag extra tooltip informatie:
                    StackPanel strTooltip = new StackPanel();
                    StackPanel strTooltipAanvraag = new StackPanel();


                    // filter alleen de mandagen voor vandaag, waarvan de ingelogde kijker projectleider is,
                    // van het huidige project waar we naar kijken
                    List<Mandagen> listMandagen = listMandagenTotal.Where(m => m.ProjectleiderId == objProjectleider.ProjectleiderId && m.ProjectId == objProject.ProjectId && m.Begintijd >= dtDag.AddDays(i) && m.Eindtijd <= dtDag.AddDays(i + 1)).OrderBy( m => dbo.GetContact(m.Vakman.ContactIdOrigineel, true) == null ? "" : dbo.GetContact(m.Vakman.ContactIdOrigineel, true).voornaam).ThenByDescending(m => m.Status).ToList();

                    // als in deze pool van mandagen een aanvraag zit:
                    // vandaag, ander project, geen projectleider, niet bevestigd (dus aanvraag) 
                    List<Mandagen> listAanvragen = listMandagenTotal.Where(m => m.ProjectleiderId != objProjectleider.ProjectleiderId && m.ProjectId != objProject.ProjectId && m.Begintijd >= dtDag.AddDays(i) && m.Eindtijd <= dtDag.AddDays(i + 1) && !m.Status).ToList();

                    // als in deze pool van mandagen een aanvraag zit:
                    // vandaag, ander project, geen projectleider, wel bevestigd (dus waar dit de aanvraag van is) 
                    List<Mandagen> listMandagenWaarDitDeAanvraagVanIs = listMandagenTotal.Where(m => m.Begintijd != m.Eindtijd && m.ProjectleiderId != objProjectleider.ProjectleiderId && m.ProjectId != objProject.ProjectId && m.Begintijd >= dtDag.AddDays(i) && m.Eindtijd <= dtDag.AddDays(i + 1) && m.Status).ToList();

                    
                    //long totalTicks = 0;
                    bool hasAanvraag = false;
                    int totalHours = 0;
                    int totalMinutes = 0;

                    int previousVakmanId = -1;

                    //if ((listMandagen.Any(m=> !m.Status) || listAanvragen.Any(a => !a.Geannulleerd && a.Eindtijd > listMandagen.Min(m=>m.Begintijd) && a.Begintijd < listMandagen.Max(m=>m.Eindtijd && a.ProjectId != mandag.ProjectId && a.VakmanId == mandag.VakmanId))
                    //{
                    //    hasAanvraag = true;
                    //}


                    // TODO: WEER AANZETTEN, CACHE TEST
                    foreach (Mandagen mandag in listMandagen)
                    {

                        // bereken de som der uren 
                        //ALLEEN ALS HET EEN BEVESTIGD UUR IS
                        if (mandag.Status)
                        {
                            totalHours += mandag.Uren;
                            totalMinutes += mandag.Minuten;


                            ////long lnMandagTicks = (mandag.Eindtijd.Ticks - mandag.Begintijd.Ticks);
                            ////totalTicks += lnMandagTicks;
                            ////TimeSpan tsMandag = TimeSpan.FromTicks(lnMandagTicks);

                            //MDRpersoon persoon = dbo.GetContact(mandag.Vakman.ContactIdOrigineel, true);

                            //if (persoon != null)
                            //{
                            //    strTooltip += "(" + ((int)Math.Ceiling(tsMandag.TotalHours)).ToString() + "u) " + (persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam).ToStringTrimmed();
                            //    strTooltip.AddText("(" + mandag.Uren.ToString() + ":" + (mandag.Minuten < 10 ? "0" : "") + mandag.Minuten.ToString() + ") " + (persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam).ToStringTrimmed());
                            //}


                            //// 
                            //bool blnToonAanvraagtext = listAanvragen.Any(m => m.Begintijd < mandag.Eindtijd && m.Eindtijd > mandag.Begintijd && m.VakmanId == mandag.VakmanId && !m.Geannulleerd);

                            //if (blnToonAanvraagtext)
                            //{

                            //    strTooltipAanvraag.AddText("Aanvraag " + (persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam).ToStringTrimmed(), true);


                            //    // OF we hebben te maken met een externe aanvraag voor deze mandag
                            //    foreach (Mandagen aanvraag in listAanvragen.Where(m => m.Begintijd < mandag.Eindtijd && m.Eindtijd > mandag.Begintijd && m.VakmanId == mandag.VakmanId))
                            //    {

                            //        if (aanvraag != null && !aanvraag.Geannulleerd)
                            //        {
                            //            MDRpersoon persoonAanvraag = dbo.GetContact(aanvraag.Vakman.ContactIdOrigineel, true);

                            //            if (persoonAanvraag != null)
                            //            {
                            //                strTooltipAanvraag.AddText("(" + dbtools.GetIngeplandeTijd(objProject, aanvraag.Vakman, dbtools.GetHeleDag(aanvraag.Begintijd)) + ") \u2212 (" + aanvraag.Uren.ToString() + ":" + (aanvraag.Minuten < 10 ? "0" : "") + aanvraag.Minuten.ToString() + ") " + objProject.Naam + " (Project " + objProject.ProjectNr.ToString() + ")", true);
                            //                strTooltipAanvraag.AddText("(" + dbtools.GetIngeplandeTijd(aanvraag.Project, aanvraag.Vakman, dbtools.GetHeleDag(aanvraag.Begintijd)) + ") + (" + aanvraag.Uren.ToString() + ":" + (aanvraag.Minuten < 10 ? "0" : "") + aanvraag.Minuten.ToString() + ") " + aanvraag.Project.Naam + " (Project " + aanvraag.Project.ProjectNr.ToString() + ")", true);
                            //            }
                            //        }
                            //    }

                                
                            //}
                        }
                        //else
                        //{
                        //    // OF dit is zelf de aanvraag
                        //    MDRpersoon persoon = dbo.GetContact(mandag.Vakman.ContactIdOrigineel, true);


                        //    if (persoon != null && (listAanvragen.Any(m => m.VakmanId == mandag.VakmanId) || listMandagenWaarDitDeAanvraagVanIs.Any(m => m.VakmanId == mandag.VakmanId)))
                        //    {
                        //        // dit hoeft niet dubbel te worden weergegeven als de vakman al is ingepland, of als er meerdere aanvragen lopen
                        //        if (previousVakmanId != mandag.VakmanId)
                        //        {
                        //            // INDIEN ER EEN AANVRAAG IS, DEZE PERSOON OOK WEERGEVEN ALS ZOGENAAMD 0:00 INGEVULD OP HET PROJECT
                        //            // strTooltip += "(" + ((int)Math.Ceiling(tsMandag.TotalHours)).ToString() + "u) " + (persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam).ToStringTrimmed();
                        //            // strTooltip.AddText("(" + dbtools.GetIngeplandeTijd(mandag.Project, mandag.Vakman, dbtools.GetHeleDag(mandag.Begintijd)) + ")" + (persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam).ToStringTrimmed());
                        //        }

                        //        if (previousVakmanId == mandag.VakmanId)
                        //        {
                        //            // dezelfde vakman, alleen de minnen toevoegen
                        //            //strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraagMinOnly(listMandagenWaarDitDeAanvraagVanIs, mandag, true));
                        //            //strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraagAll(mandag.Vakman, mandag.Project, dbtools.GetHeleDag(mandag.Begintijd), true, true));
                        //        }
                        //        else
                        //        {
                        //            //strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraag(listMandagenWaarDitDeAanvraagVanIs, mandag, true));
                        //            //strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraagAll(mandag.Vakman, mandag.Project, dbtools.GetHeleDag(mandag.Begintijd)));
                        //        }
                        //        //strTooltipAanvraag += Environment.NewLine;
                        //        //strTooltipAanvraag += Environment.NewLine;
                        //        //strTooltipAanvraag += "Aanvraag test2" + (persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam).ToStringTrimmed();

                        //        //if (mandag.Geannulleerd)
                        //        //{
                        //        //    strTooltipAanvraag += " AFGEWEZEN";
                        //        //}

                        //        //// plustext is cumulatief
                        //        //strTooltipAanvraag += Environment.NewLine;
                        //        //strTooltipAanvraag += dbtools.GetIngeplandeTijd(mandag.Project, mandag.Vakman, dbtools.GetHeleDag(mandag.Begintijd)) + " + (" + dbtools.GetAangevraagdeTijd(mandag.Project, mandag.Vakman, dbtools.GetHeleDag(mandag.Begintijd)) + ") " + mandag.Project.Naam + " (Project " + mandag.Project.ProjectNr.ToString() + ")";

                        //        //// minus text is een opsomming van alle mandagen die beinvloed worden door de aanvraag
                        //        //foreach (Mandagen origineel in listMandagenWaarDitDeAanvraagVanIs.Where(m => m.Begintijd <= mandag.Eindtijd && m.Eindtijd >= mandag.Begintijd && m.VakmanId == mandag.VakmanId))
                        //        //{

                        //        //    Project objProject2 = origineel.Project;
                        //        //    // minustext 
                        //        //    strTooltipAanvraag += Environment.NewLine;
                        //        //    strTooltipAanvraag += dbtools.GetIngeplandeTijd(objProject2, mandag.Vakman, dbtools.GetHeleDag(mandag.Begintijd)) + " \u2212 (" + mandag.Uren.ToString() + ":" + (mandag.Minuten < 10 ? "0" : "") + mandag.Minuten.ToString() + ") " + objProject2.Naam + " (Project " + objProject2.ProjectNr.ToString() + ")";
                        //        //}
                        //    }

                      //    previousVakmanId = mandag.VakmanId;
                      //}

                        // zwart randje tonen als: een van de vakmannen een aanvraag is, 
                        // of er voor een van de vakmannen een aanvraag is gedaan die invloed heeft op dit project 
                        // en nog niet geannuleerd is als die door iemand anders is gedaan, of wel geannuleerd als die door mij is gedaan
                        // 
                        // DUS:
                        // als de betreffende mandag een aanvraag is
                        // OF er een aanvraag is die deze overlapt qua tijden, voor een ander project is, maar voor dezelfde vakman, en nog niet geannulleerd dus lopende aanvraag
                        //if ((!mandag.Status && !mandag.Geannulleerd) || listAanvragen.Any(a => a.Eindtijd > mandag.Begintijd && a.Begintijd < mandag.Eindtijd && a.ProjectId != mandag.ProjectId && a.VakmanId == mandag.VakmanId && !a.Geannulleerd))
                        if ((!mandag.Status) || listAanvragen.Any(a => !a.Geannulleerd && a.Eindtijd > mandag.Begintijd && a.Begintijd < mandag.Eindtijd && a.ProjectId != mandag.ProjectId && a.VakmanId == mandag.VakmanId))
                        {
                            hasAanvraag = true;
                        }

                    }
                    // END TODO: WEER AANZETTEN, CACHE TEST

                    //foreach (Mandagen mandag in listAanvragen)
                    //{

                    //    // OF dit is het originele project, waarop is aangevraagd

                    //    // bereken de som der uren 
                    //    //ALLEEN ALS HET EEN BEVESTIGD UUR IS
                    //    if (!mandag.Status)
                    //    {
                    //        MDRpersoon persoon = dbo.GetContact(mandag.Vakman.ContactIdOrigineel, true);

                    //        Mandagen temp = listMandagenWaarDitDeAanvraagVanIs.FirstOrDefault(m => m.Begintijd <= mandag.Eindtijd && m.Eindtijd >= mandag.Begintijd && m.VakmanId == mandag.VakmanId);

                    //        if (persoon != null && temp != null && temp.VakmanId == mandag.VakmanId && objProject.ProjectId == temp.ProjectId)
                    //        {
                    //            strTooltipAanvraag += Environment.NewLine;
                    //            strTooltipAanvraag += Environment.NewLine;
                    //            strTooltipAanvraag += "Aanvraag " + (persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam).ToStringTrimmed();
                    //            if (mandag.Geannulleerd)
                    //            {
                    //                strTooltipAanvraag += " AFGEWEZEN";
                    //            }
                    //            strTooltipAanvraag += Environment.NewLine;
                    //            strTooltipAanvraag += "\u2212 (" + mandag.Uren.ToString() + ":" + (mandag.Minuten < 10 ? "0" : "") + mandag.Minuten.ToString() + ") " + objProject.Naam + " (Project " + objProject.ProjectNr.ToString() + ")";
                    //            strTooltipAanvraag += Environment.NewLine;
                    //            strTooltipAanvraag += "+ (" + mandag.Uren.ToString() + ":" + (mandag.Minuten < 10 ? "0" : "") + mandag.Minuten.ToString() + ") " + mandag.Project.Naam + " (Project " + mandag.Project.ProjectNr.ToString() + ")";
                    //        }
                    //    }
                    //}

                    bool hasUren = totalHours > 0 || totalMinutes > 0;
                    //strTooltip.AddText(objProject.Naam + " (Project " + objProject.ProjectNr.ToString() + ")");
                    strTooltipAanvraag.Children.Add(dbtools.AddTooltipAanvraagAll(new Vakman(), objProject, dbtools.GetHeleDag(dtDag.AddDays(i)), true, true, listMandagenTotal.Where(m => m.Begintijd >= dtDag.AddDays(i) && m.Eindtijd <= dtDag.AddDays(i + 1)).ToList(),hasAanvraag, hasUren));



                    //TimeSpan totalTimespan = TimeSpan.FromTicks(totalTicks);
                    Button lblDag = new Button();
                    lblDag.Width = 24;
                    lblDag.Height = 17;
                    lblDag.Margin = new Thickness(2, 2, 2, 2);
                    lblDag.FontSize = 11;
                    lblDag.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;

                    lblDag.Padding = new Thickness(0, -1, 2, 0);

                    ToolTipService.SetShowDuration(lblDag, 20000);


                    strTooltip.Children.Add(strTooltipAanvraag);
                    lblDag.ToolTip = strTooltip;

                    // test
                    //lblDag.ToolTip = objProject.Naam + " (Project " + objProject.ProjectNr.ToString() + ")" + strTooltip + strTooltipAanvraag;


                    int totalHoursRounded = 0;
                    //totalHoursRounded = (int)Math.Ceiling(totalTimespan.TotalHours);
                    int minutesRemainder = totalMinutes % 60;

                    totalHoursRounded = totalHours + (int)((totalMinutes - minutesRemainder) / 60) + (minutesRemainder == 0 ? 0 : 1);

                    //if (totalTimespan.Minutes > 0)
                    //{
                    //    totalHoursRounded ++;
                    //}
                    
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
                    tag.Add(objProject);
                    tag.Add(dtDag.AddDays(i));

                    lblDag.Tag = tag;

                    // standaard kleuren van de border
                    if (weeknummer % 2 == 0)
                    {
                        if (blnUseInvisibleColorsAKASwitchColors)
                        {
                            // use this color to make it invisible (same color as background)
                            lblDag.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5D9F1"));
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
                            lblDag.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5D9F1"));
                        }
                    }

                    // zwarte rand maken bij een aanvraag
                    if (hasAanvraag)
                    {
                        double[] dimensions = new double[] { 2, 2 };
                        Rectangle dottedRectangle = new Rectangle();
                        dottedRectangle.Stroke = blnOwner ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);
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
                        lblDag.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5D9F1"));
                    }
                    else
                    {
                        lblDag.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
                    }

                    
                    lblDag.BorderThickness = new Thickness(2);

                    lblDag.Click += lblDag_Click;
                    spUren.Children.Add(lblDag);

                }
            }


        }


        public void calculateDag(Button lblDag, Project objProject, DateTime datum)
        {
            dbOriginalRepository dbo = new dbOriginalRepository();

            // per dag extra tooltip informatie:
            string strTooltip = string.Empty;


            // filter alleen de mandagen voor vandaag, waarvan de ingelogde kijker projectleider is,
            // van het huidige project waar we naar kijken
            List<Mandagen> listMandagen = listMandagenTotal.Where(m => m.ProjectleiderId == objProjectleider.ProjectleiderId && m.ProjectId == objProject.ProjectId && m.Begintijd >= datum && m.Eindtijd <= datum.AddDays(1)).ToList();

            // als in deze pool van mandagen een aanvraag zit:
            // vandaag, ander project, geen projectleider, niet bevestigd (dus aanvraag) 
            List<Mandagen> listAanvragen = listMandagenTotal.Where(m => m.ProjectleiderId != objProjectleider.ProjectleiderId && m.ProjectId != objProject.ProjectId && m.Begintijd >= datum && m.Eindtijd <= datum.AddDays(1) && !m.Status).ToList();


            //long totalTicks = 0;
            bool hasAanvraag = false;
            int totalHours = 0;
            int totalMinutes = 0;

            // TODO: WEER AANZETTEN, CACHE TEST
            foreach (Mandagen mandag in listMandagen)
            {
                // bereken de som der uren 
                //ALLEEN ALS HET EEN BEVESTIGD UUR IS
                if (mandag.Status)
                {
                    //long lnMandagTicks = (mandag.Eindtijd.Ticks - mandag.Begintijd.Ticks);
                    //totalTicks += lnMandagTicks;
                    //TimeSpan tsMandag = TimeSpan.FromTicks(lnMandagTicks);

                    MDRpersoon persoon = dbo.GetContact(mandag.Vakman.ContactIdOrigineel, true);

                    if (persoon != null)
                    {
                        strTooltip += Environment.NewLine;
                        //strTooltip += "(" + ((int)Math.Ceiling(tsMandag.TotalHours)).ToString() + "u) " + (persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam).ToStringTrimmed();
                        strTooltip += "(" + mandag.Uren.ToString() + ":" + (mandag.Minuten < 10 ? "0" : "") + mandag.Minuten.ToString() + ") " + (persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam).ToStringTrimmed();
                    }

                    totalHours += mandag.Uren;
                    totalMinutes += mandag.Minuten;

                }

                // zwart randje tonen als: een van de vakmannen een aanvraag is, 
                // of er voor een van de vakmannen een aanvraag is gedaan die invloed heeft op dit project 
                // en nog niet geannuleerd is als die door iemand anders is gedaan, of wel geannuleerd als die door mij is gedaan
                // 
                // DUS:
                // als de betreffende mandag een aanvraag is
                // OF er een aanvraag is die deze overlapt qua tijden, voor een ander project is, maar voor dezelfde vakman, en nog niet geannulleerd dus lopende aanvraag
                //if ((!mandag.Status && !mandag.Geannulleerd) || listAanvragen.Any(a => a.Eindtijd > mandag.Begintijd && a.Begintijd < mandag.Eindtijd && a.ProjectId != mandag.ProjectId && a.VakmanId == mandag.VakmanId && !a.Geannulleerd))
                if ((!mandag.Status) || listAanvragen.Any(a => !a.Geannulleerd && a.Eindtijd > mandag.Begintijd && a.Begintijd < mandag.Eindtijd && a.ProjectId != mandag.ProjectId && a.VakmanId == mandag.VakmanId))
                {
                    hasAanvraag = true;
                }
            }
            // END TODO: WEER AANZETTEN, CACHE TEST

            // CHECKEN
            //TimeSpan totalTimespan = TimeSpan.FromTicks(totalTicks);
            lblDag.Width = 24;
            lblDag.Height = 17;
            lblDag.Margin = new Thickness(2, 2, 2, 2);
            lblDag.FontSize = 11;
            lblDag.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;

            lblDag.Padding = new Thickness(0, -1, 2, 0);

            lblDag.ToolTip = objProject.Naam + strTooltip;

            int totalHoursRounded = 0;
            //totalHoursRounded = (int)Math.Ceiling(totalTimespan.TotalHours);
            int minutesRemainder = totalMinutes % 60;

            totalHoursRounded = totalHours + (int)((totalMinutes - minutesRemainder) / 60) + (minutesRemainder == 0 ? 0 : 1);

            //if (totalTimespan.Minutes > 0)
            //{
            //    totalHoursRounded ++;
            //}

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
            tag.Add(objProject);
            tag.Add(datum);

            lblDag.Tag = tag;

            // standaard kleuren van de border
            if (weeknummer % 2 == 0)
            {
                if (blnUseInvisibleColorsAKASwitchColors)
                {
                    // use this color to make it invisible (same color as background)
                    lblDag.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5D9F1"));
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
                    lblDag.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5D9F1"));
                }
            }

            // zwarte rand maken bij een aanvraag
            if (hasAanvraag)
            {
                double[] dimensions = new double[] { 2, 2 };
                Rectangle dottedRectangle = new Rectangle();
                dottedRectangle.Stroke = blnOwner ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);
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
                lblDag.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5D9F1"));
            }
            else
            {
                lblDag.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
            }


            lblDag.BorderThickness = new Thickness(2);

            // END CHECCKEN




            //lblDag.ToolTip = objProject.Naam + strTooltip;

            //int totalHoursRounded = 0;
            ////totalHoursRounded = (int)Math.Ceiling(totalTimespan.TotalHours);
            //int minutesRemainder = totalMinutes % 60;

            //totalHoursRounded = totalHours + (int)((totalMinutes - minutesRemainder) / 60) + (minutesRemainder == 0 ? 0 : 1);

            ////if (totalTimespan.Minutes > 0)
            ////{
            ////    totalHoursRounded ++;
            ////}

            //lblDag.Content = totalHoursRounded == 0 ? "" : totalHoursRounded.ToString();
            //bool blnUseInvisibleColorsAKASwitchColors = false;

            //// 0 niet tonen
            //if (totalHoursRounded == 0 && !hasAanvraag)
            //{
            //    blnUseInvisibleColorsAKASwitchColors = true;
            //    //lblDag.Visibility = System.Windows.Visibility.Hidden;
            //}
            //else
            //{
            //    HasHours = true;
            //}

            //List<object> tag = new List<object>();
            //tag.Add(objProject);
            //tag.Add(datum);

            //lblDag.Tag = tag;

            //// zwarte rand maken bij een aanvraag
            //if (hasAanvraag)
            //{
            //    double[] dimensions = new double[] { 2, 2 };
            //    Rectangle dottedRectangle = new Rectangle();
            //    dottedRectangle.Stroke = blnOwner ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);
            //    dottedRectangle.SnapsToDevicePixels = true;
            //    dottedRectangle.Height = 2;
            //    dottedRectangle.StrokeThickness = 2;
            //    dottedRectangle.Height = 24;
            //    dottedRectangle.Width = 45;
            //    dottedRectangle.StrokeDashArray = new DoubleCollection(dimensions);

            //    VisualBrush vb = new VisualBrush();
            //    vb.Visual = dottedRectangle;

            //    lblDag.BorderBrush = vb; // new SolidColorBrush(Colors.Black); // (SolidColorBrush)(new BrushConverter().ConvertFrom("#00000000"));
            //}

        }

        void lblDag_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            ApplicationState.SetValue(ApplicationVariables.intVakmanViewModus, 1);

            Project cProject = (Project)((List<object>)((Button)sender).Tag)[0];
            DateTime selectedDay = (DateTime)((List<object>)((Button)sender).Tag)[1];

            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, selectedDay);
            ApplicationState.SetValue(ApplicationVariables.intProjectId, cProject.ProjectId);

            dbOriginalRepository dbrepOriginal = new dbOriginalRepository();

            MDRproject projectorigineel = dbrepOriginal.GetProject((int)cProject.ProjectNr);
            MDRpersoon projectleider = dbrepOriginal.GetContact(objProjectleider.ContactIdOrigineel);

            // create the page and load all values
            MandagenRegistratie.controls.Projecten.Lijst.Project pv = new MandagenRegistratie.controls.Projecten.Lijst.Project(projectorigineel);
            //pv.Load();

            
            // load the page into the contentcontrol
            MenuControl owner = Tools.FindVisualParent<MenuControl>(this);

            owner.PageGoToPage(pv);

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


    }
}

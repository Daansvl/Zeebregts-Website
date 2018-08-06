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
using MandagenRegistratie.classes;
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie.controls
{
    /// <summary>
    /// Interaction logic for VakmanDag.xaml
    /// </summary>
    public partial class VakmanDag : UserControl, INotifyPropertyChanged
    {
        #region "INotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));


                switch (propertyName)
                {
                    case "IsGewijzigd":
                        ToggleButtons();
                        break;
                    case "IsSelected":
                        ToggleButtons();
                        break;
                    case "Vakman":
                        switch ((int)dtBegintijd.DayOfWeek)
                        {
                            case 1:
                                lblDagUren.Content = Vakman.Ma;
                                break;
                            case 2:
                                lblDagUren.Content = Vakman.Di;
                                break;
                            case 3:
                                lblDagUren.Content = Vakman.Wo;
                                break;
                            case 4:
                                lblDagUren.Content = Vakman.Do;
                                break;
                            case 5:
                                lblDagUren.Content = Vakman.Vr;
                                break;
                            case 6:
                                lblDagUren.Content = Vakman.Za;
                                break;
                            case 0:
                                lblDagUren.Content = Vakman.Zo;
                                break;
                            default:
                                lblDagUren.Content = "onbekend";
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }

        protected bool SetField<T>(ref T field, T value, string propertyName)
            {
                if (EqualityComparer<T>.Default.Equals(field, value)) return false;
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }

            private bool isGewijzigd;
            public bool IsGewijzigd
            {
                get { return isGewijzigd; }
                set { SetField(ref isGewijzigd, value, "IsGewijzigd"); }
            }

            private bool isSelected;
            public bool IsSelected
            {
                get { return isSelected; }
                set { SetField(ref isSelected, value, "IsSelected"); }
            }

        private Vakman objVakman;
            public Vakman Vakman
            {
                get { return objVakman; }
                set { SetField(ref objVakman, value, "Vakman"); }
            }

        #endregion

        public Projectleider objProjectleider;

        public List<Project> listProjecten;
        //public Project objProjectLeidend;

        //public Project objProject;

        public int ProjectId { get; set; }

        // extra
        private List<Mandagen> listMandagen;
        public DateTime dtBegintijd;


        #region "RoutedEvents"

        public static readonly RoutedEvent VakmanDagEvent = EventManager.RegisterRoutedEvent("OnVakmanDagUpdate", RoutingStrategy.Bubble,
        typeof(RoutedEventHandler), typeof(VakmanDag));

        public event RoutedEventHandler OnVakmanDagUpdate
        {
            add { AddHandler(VakmanDagEvent, value); }
            remove { RemoveHandler(VakmanDagEvent, value); }
        }

        public static readonly RoutedEvent VakmanDagSelectEvent = EventManager.RegisterRoutedEvent("OnVakmanDagSelect", RoutingStrategy.Bubble,
typeof(RoutedEventHandler), typeof(VakmanDag));

        public event RoutedEventHandler OnVakmanDagSelect
        {
            add { AddHandler(VakmanDagSelectEvent, value); }
            remove { RemoveHandler(VakmanDagSelectEvent, value); }
        }

        #endregion

        public VakmanDag()
        {
            InitializeComponent();
        }

        public void LoadVakmanDag()
        {

            // intialize
            spVakmanDag.Children.Clear();
            dbRepository dbrep = new dbRepository();
            listMandagen = dbrep.GetMandagen(objVakman.VakmanId, dtBegintijd);
            bool blnIsOpVreemdProjectIngepland = false;
            bool blnIsOpVreemdProjectAangevraagd = false;
            bool blnIsOpProjectAangevraagd = false;
            bool blnIsOpProjectIngepland;
            bool blnIsNotOwner = false;

            DateTime startDatum = new DateTime(dtBegintijd.Year, dtBegintijd.Month, dtBegintijd.Day);
            DateTime eindDatum = startDatum.AddDays(1);

            listMandagen = listMandagen.Where(p => p.Begintijd >= startDatum && p.Begintijd < eindDatum).ToList();

            blnIsOpVreemdProjectIngepland = listMandagen.Any(m => m.Uren > 0 && m.Project.ProjectleiderId != objProjectleider.ProjectleiderId && (m.VakmanstatusId == 1 || m.VakmanstatusId == 2 || m.VakmanstatusId == 3));
            blnIsOpProjectAangevraagd = listMandagen.Any(m => m.Gewijzigd && m.UrenGewijzigd > 0 && (m.VakmanstatusId == 1 || m.VakmanstatusId == 2 || m.VakmanstatusId == 3));
            blnIsOpVreemdProjectAangevraagd = listMandagen.Any(m => m.Gewijzigd && m.UrenGewijzigd > 0 && m.MutatieDoorProjectleiderId != objProjectleider.ProjectleiderId && (m.VakmanstatusId == 1 || m.VakmanstatusId == 2 || m.VakmanstatusId == 3));
            blnIsOpProjectIngepland = listMandagen.Any(m => m.Bevestigd && m.Uren > 0 && (m.VakmanstatusId == 1 || m.VakmanstatusId == 2 || m.VakmanstatusId == 3));
            blnIsNotOwner = !listMandagen.Any(m => m.Bevestigd && m.Uren > 0 && (m.VakmanstatusId == 1 || m.VakmanstatusId == 2 || m.VakmanstatusId == 3));


            bool blnIsAllowedToCancel = false;
            bool blnIsAllowedToConfirm = false;
            bool blnIsAllowedToAcceptAnnulleringen = false;

            bool blnIsAllowedToSetZiek = true;
            bool blnIsAllowedToSetVakantie = true;


            // voor alle projecten van deze vakman
            foreach (Project project in listProjecten)
            {

            bool blnIsOwner = false;
            bool blnIsEditor = false;

                // als ik (degene die dit scherm bekijkt) de projectleider ben
                blnIsOwner = project.ProjectleiderId == objProjectleider.ProjectleiderId;

                VakmanDagProject vakmandagproject = new VakmanDagProject();
                vakmandagproject.ProjectId = project.ProjectId;
                vakmandagproject.OnVakmanDagProjectUpdate += new RoutedEventHandler(recDag_MouseDown);
                vakmandagproject.OnVakmanDagProjectSelect += new RoutedEventHandler(spVakmandagProject_OnVakmanDagSelectEvent);

                // als ik (degene die dit scherm bekijkt) de projectleider ben
                vakmandagproject.IsGroen = blnIsOwner;

                // als het niet mijn project is EN er ergens uren staan ingevuld op een (ANDER) project
                vakmandagproject.ReadOnly = (!blnIsOwner && blnIsOpVreemdProjectIngepland) || (blnIsOpProjectAangevraagd);

                // kijken of hij ingepland is voor dit project
                Mandagen mandag = listMandagen.Where(m => m.ProjectId == project.ProjectId && m.VakmanstatusId == 1).FirstOrDefault();

                // als hij ingepland is
                if (mandag != null)
                {
                    blnIsEditor = mandag.MutatieDoorProjectleiderId == objProjectleider.ProjectleiderId;

                    // iemand geeft een vakman aan mij
                    // stippelijn door iemand anders bij mij geplaatst
                    // niet bevestigd wel owner en gewijzigd door iemand anders
                    // als het project gewijzigd is, door iemand anders, en de wijziging mijn uren betreft
                    if (!mandag.Bevestigd && blnIsOwner && mandag.Gewijzigd && !blnIsEditor && mandag.UrenGewijzigd > 0)
                    {
                        blnIsAllowedToConfirm = true;
                        blnIsAllowedToCancel = true;
                    }

                    // iemand pakt een vakman van mij
                    // solid lijn
                    // wel bevestigd, maar gemuteerd door iemand anders en ik ben owner
                    if (mandag.Bevestigd && blnIsOwner && mandag.Gewijzigd && !blnIsEditor && mandag.Uren > 0)
                    {
                        blnIsAllowedToConfirm = true;
                        blnIsAllowedToCancel = true;
                    }

                    if (mandag.Geannulleerd && !blnIsEditor)
                    {

                        vakmandagproject.IsOranje = true;
                        vakmandagproject.Duration = mandag.UrenGewijzigd;
                        vakmandagproject.IsSolid = false;
                        blnIsAllowedToAcceptAnnulleringen = blnIsOwner;
                        // flag op true zetten, zorgt er automatisch voor dat cancel en confirm buttons weergegeven kunnen worden
                        IsGewijzigd = true;

                    }
                    // als de mandag bevestigd is, niks aan de hand, gewoon de normale uren weergeven
                    else if (mandag.Bevestigd)
                    {
                        vakmandagproject.Duration = mandag.Uren;
                        vakmandagproject.IsSolid = true;

                        // if (!blnIsOwner && vakmandagproject.Duration > 0)
                        if (blnIsOpVreemdProjectIngepland && !blnIsOwner)
                        {
                            vakmandagproject.ReadOnly = true;
                            blnIsAllowedToSetZiek = false;
                            blnIsAllowedToSetVakantie = false;
                        }
                        else if (blnIsOwner && vakmandagproject.Duration > 0)
                        {
                            // TODO: weeer aanzetten, nu even uitzetten
                            // vakmandagproject.spVakmanDagUren.Visibility = System.Windows.Visibility.Visible;
                            //vakmandagproject.lblVakmandagUren.Content = mandag.Uren;
                            //vakmandagproject.lblVakmandagMinuten.Content = mandag.Minuten;
                        }
                    }
                    else // als niet bevestigd, dan is er iets gewijzigd, hetzij een wijziging, hetzij een nieuwe invoer
                    {
                        vakmandagproject.Duration = mandag.UrenGewijzigd;
                        vakmandagproject.IsSolid = false;

                        if (blnIsOwner || blnIsEditor)
                        {
                            blnIsAllowedToCancel = true;
                        }

                        // flag op true zetten, zorgt er automatisch voor dat cancel en confirm buttons weergegeven kunnen worden
                        IsGewijzigd = true;

                        // bij gewijzigde items de gewijzigde uren weergeven
                    }
                }
                else // niet ingepland, dus niks veranderen aan default situatie, duration = 0
                {
                    vakmandagproject.Duration = 0;
                }

                // klaar met instellingen, vakmandagproject toevoegen
                spVakmanDag.Children.Add(vakmandagproject);
            }



            // ziek en vakantie toevoegen
            VakmanDagProject vakmandagprojectZiek = new VakmanDagProject();
            vakmandagprojectZiek.ProjectId = 0;
            vakmandagprojectZiek.OnVakmanDagProjectUpdate += new RoutedEventHandler(recDag_MouseDown);
            vakmandagprojectZiek.IsZiek = true;
            bool blnIsZiek = false;

            // als ik (degene die dit scherm bekijkt) de projectleider ben
            vakmandagprojectZiek.IsGroen = false; // blnIsOwner;
            
            // TEST
            //vakmandagprojectZiek.ReadOnly = !blnIsAllowedToSetZiek;
            vakmandagprojectZiek.ReadOnly = !blnIsAllowedToSetZiek || blnIsAllowedToCancel || blnIsAllowedToConfirm || blnIsAllowedToAcceptAnnulleringen;
            //vakmandagprojectZiek.ReadOnly = (!blnIsOwner && blnIsOpVreemdProjectIngepland) || (blnIsOpVreemdProjectAangevraagd);


            // kijken of hij ingepland is voor dit project
            Mandagen mandagZiek = listMandagen.Where(m => m.ProjectId == 0 && m.VakmanstatusId == 2).FirstOrDefault();

            // als hij ingepland is
            if (mandagZiek != null)
            {
                bool blnIsOwner;
                bool blnIsEditor;

                blnIsOwner = mandagZiek.Project.ProjectleiderId == objProjectleider.ProjectleiderId;
                blnIsEditor = mandagZiek.MutatieDoorProjectleiderId == objProjectleider.ProjectleiderId;

                // iemand geeft een vakman aan mij
                // stippelijn door iemand anders bij mij geplaatst
                // niet bevestigd wel owner en gewijzigd door iemand anders
                // als het project gewijzigd is, door iemand anders, en de wijziging mijn uren betreft
                if (!mandagZiek.Bevestigd && blnIsOwner && mandagZiek.Gewijzigd && !blnIsEditor && mandagZiek.UrenGewijzigd > 0)
                {
                    blnIsAllowedToConfirm = true;
                    blnIsAllowedToCancel = true;
                }

                // iemand pakt een vakman van mij
                // solid lijn
                // wel bevestigd, maar gemuteerd door iemand anders en ik ben owner
                if (mandagZiek.Bevestigd && blnIsOwner && mandagZiek.Gewijzigd && !blnIsEditor && mandagZiek.Uren > 0)
                {
                    blnIsAllowedToConfirm = true;
                    blnIsAllowedToCancel = true;
                }


                if (mandagZiek.Geannulleerd && !blnIsEditor)
                {

                    vakmandagprojectZiek.Duration = mandagZiek.UrenGewijzigd;
                    vakmandagprojectZiek.IsOranje = true;
                    vakmandagprojectZiek.IsSolid = false;
                    // blnIsAllowedToAcceptAnnulleringen = true;
                    // flag op true zetten, zorgt er automatisch voor dat cancel en confirm buttons weergegeven kunnen worden
                    IsGewijzigd = true;

                }
                // als de mandag bevestigd is, niks aan de hand, gewoon de normale uren weergeven
                else if (mandagZiek.Bevestigd)
                {
                    blnIsZiek = mandagZiek.Uren > 0;

                    vakmandagprojectZiek.Duration = mandagZiek.Uren;
                    vakmandagprojectZiek.IsSolid = true;
                }
                else // als niet bevestigd, dan is er iets gewijzigd, hetzij een wijziging, hetzij een nieuwe invoer
                {
                    vakmandagprojectZiek.IsSolid = false;

                    if (blnIsOwner || blnIsEditor)
                    {
                        blnIsAllowedToCancel = true;
                    }

                    // flag op true zetten, zorgt er automatisch voor dat cancel en confirm buttons weergegeven kunnen worden
                    IsGewijzigd = true;

                    // bij gewijzigde items de gewijzigde uren weergeven
                    vakmandagprojectZiek.Duration = mandagZiek.UrenGewijzigd;
                }
            }
            else // niet ingepland, dus niks veranderen aan default situatie, duration = 0
            {
                vakmandagprojectZiek.Duration = 0;
            }

            // klaar met instellingen, vakmandagproject toevoegen
            spVakmanDag.Children.Add(vakmandagprojectZiek);



            // ziek en vakantie toevoegen
            // ziek en vakantie toevoegen
            VakmanDagProject vakmandagprojectVakantie = new VakmanDagProject();
            vakmandagprojectVakantie.ProjectId = 0;
            vakmandagprojectVakantie.OnVakmanDagProjectUpdate += new RoutedEventHandler(recDag_MouseDown);
            vakmandagprojectVakantie.IsVakantie = true;
            bool blnIsVakantie = false;

            // als ik (degene die dit scherm bekijkt) de projectleider ben
            vakmandagprojectVakantie.IsGroen = false; // blnIsOwner;

            // TEST
            vakmandagprojectVakantie.ReadOnly = !blnIsAllowedToSetVakantie || blnIsAllowedToCancel || blnIsAllowedToConfirm || blnIsAllowedToAcceptAnnulleringen;
            //vakmandagprojectVakantie.ReadOnly = (!blnIsOwner && blnIsOpVreemdProjectIngepland) || (blnIsOpVreemdProjectAangevraagd);

            // kijken of hij ingepland is voor dit project
            Mandagen mandagVakantie = listMandagen.Where(m => m.ProjectId == 0 && m.VakmanstatusId == 3).FirstOrDefault();

            // als hij ingepland is
            if (mandagVakantie != null)
            {
                bool blnIsOwner;
                bool blnIsEditor;

                blnIsEditor = mandagVakantie.MutatieDoorProjectleiderId == objProjectleider.ProjectleiderId;
                blnIsOwner = mandagVakantie.Project.ProjectleiderId == objProjectleider.ProjectleiderId;

                // iemand geeft een vakman aan mij
                // stippelijn door iemand anders bij mij geplaatst
                // niet bevestigd wel owner en gewijzigd door iemand anders
                // als het project gewijzigd is, door iemand anders, en de wijziging mijn uren betreft
                if (!mandagVakantie.Bevestigd && blnIsOwner && mandagVakantie.Gewijzigd && !blnIsEditor && mandagVakantie.UrenGewijzigd > 0)
                {
                    blnIsAllowedToConfirm = true;
                    blnIsAllowedToCancel = true;
                }

                // iemand pakt een vakman van mij
                // solid lijn
                // wel bevestigd, maar gemuteerd door iemand anders en ik ben owner
                if (mandagVakantie.Bevestigd && blnIsOwner && mandagVakantie.Gewijzigd && !blnIsEditor && mandagVakantie.Uren > 0)
                {
                    blnIsAllowedToConfirm = true;
                    blnIsAllowedToCancel = true;
                }

                if (mandagVakantie.Geannulleerd && !blnIsEditor)
                {

                    vakmandagprojectVakantie.Duration = mandagVakantie.UrenGewijzigd;
                    vakmandagprojectVakantie.IsOranje = true;
                    vakmandagprojectVakantie.IsSolid = false;
                    blnIsAllowedToAcceptAnnulleringen = true;
                    // flag op true zetten, zorgt er automatisch voor dat cancel en confirm buttons weergegeven kunnen worden
                    IsGewijzigd = true;

                }
                // als de mandag bevestigd is, niks aan de hand, gewoon de normale uren weergeven
                else if (mandagVakantie.Bevestigd)
                {
                    blnIsVakantie = mandagVakantie.Uren > 0;

                    vakmandagprojectVakantie.Duration = mandagVakantie.Uren;
                    vakmandagprojectVakantie.IsSolid = true;
                }
                else // als niet bevestigd, dan is er iets gewijzigd, hetzij een wijziging, hetzij een nieuwe invoer
                {
                    vakmandagprojectVakantie.IsSolid = false;

                    if (blnIsOwner || blnIsEditor)
                    {
                        blnIsAllowedToCancel = true;
                    }

                    // flag op true zetten, zorgt er automatisch voor dat cancel en confirm buttons weergegeven kunnen worden
                    IsGewijzigd = true;

                    // bij gewijzigde items de gewijzigde uren weergeven
                    vakmandagprojectVakantie.Duration = mandagVakantie.UrenGewijzigd;
                }
            }
            else // niet ingepland, dus niks veranderen aan default situatie, duration = 0
            {
                vakmandagprojectVakantie.Duration = 0;
            }

            // klaar met instellingen, vakmandagproject toevoegen
            spVakmanDag.Children.Add(vakmandagprojectVakantie);



            // Niet ingevuld toevoegen
            VakmanDagProject vakmandagprojectNietIngevuld = new VakmanDagProject();
            vakmandagprojectNietIngevuld.ProjectId = 0;
            vakmandagprojectNietIngevuld.OnVakmanDagProjectUpdate += new RoutedEventHandler(recDag_MouseDown);
            vakmandagprojectNietIngevuld.IsNietIngevuld = true;

            // als ik (degene die dit scherm bekijkt) de projectleider ben
            vakmandagprojectNietIngevuld.IsGroen = false;
            vakmandagprojectNietIngevuld.ReadOnly = blnIsAllowedToCancel || blnIsAllowedToAcceptAnnulleringen || ( (blnIsOpVreemdProjectIngepland || blnIsNotOwner) && !blnIsZiek && !blnIsVakantie);
            //vakmandagprojectNietIngevuld.ReadOnly = (blnIsNotOwner && blnIsOpVreemdProjectIngepland) || (blnIsOpVreemdProjectAangevraagd);
            
            vakmandagprojectNietIngevuld.IsSolid = true;
            vakmandagprojectNietIngevuld.Duration = blnIsOpProjectIngepland ? 0 : 8; ;

            // klaar met instellingen, vakmandagproject toevoegen
            spVakmanDag.Children.Add(vakmandagprojectNietIngevuld);





            // visibility van knoppen instellen
            if (blnIsAllowedToConfirm)
            {
                recOK.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                recOK.Visibility = System.Windows.Visibility.Hidden;
            }

            // visibility van knoppen instellen
            if (blnIsAllowedToCancel)
            {
                recCancel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                recCancel.Visibility = System.Windows.Visibility.Hidden;
            }

            if (blnIsAllowedToAcceptAnnulleringen)
            {
                recReadGeannulleerd.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                recReadGeannulleerd.Visibility = System.Windows.Visibility.Hidden;
            }

        }

        /// <summary>
        /// Instellen van de visibility van het stackpanel dat de knoppen OK en Cancel bevat
        /// </summary>
        private void ToggleButtons()
        {
            spIsGewijzigd.Visibility = IsGewijzigd ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            spVakmandag.Background = isSelected ? new SolidColorBrush(Colors.Gray) : new SolidColorBrush(Colors.White);
            lblDag.FontWeight = isSelected ? FontWeights.ExtraBold : FontWeights.Regular;
            ApplicationState.SetValue(ApplicationVariables.dtSelectedDay, this.dtBegintijd);

        }


                /// <summary>
        /// Toevoegen van een mandag aan de mandag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void spVakmandagProject_OnVakmanDagSelectEvent(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(VakmanDagSelectEvent, sender);
            args.RoutedEvent = VakmanDagSelectEvent;
            RaiseEvent(args);
        }


        /// <summary>
        /// Toevoegen van een mandag aan de mandag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void recDag_MouseDown(object sender, RoutedEventArgs e)
        {
            // set status gewijzigd is true
            IsGewijzigd = true;
            dbRepository dbrep = new dbRepository();

            if (((VakmanDagProject)sender).IsNietIngevuld)
            {
                dbrep.ResetMandagenNietIngevuldVoorVakmanId(objVakman.VakmanId, dtBegintijd, objProjectleider.ProjectleiderId);

            }
            else
            {

                bool canPerformAndConfirmChange = true; //  
                dbrep.ResetMandagenVoorVakmanId(objVakman.VakmanId, dtBegintijd, objProjectleider.ProjectleiderId);

                Mandagen mandag = new Mandagen();
                mandag.VakmanId = objVakman.VakmanId;
                mandag.ProjectId = ((VakmanDagProject)sender).ProjectId;
                mandag.Begintijd = new DateTime(dtBegintijd.Year, dtBegintijd.Month, dtBegintijd.Day, 8,0,0);
                mandag.Bevestigd = false;
                mandag.Gewijzigd = true;
                //mandag.Bevestigd = canPerformAndConfirmChange;
                //mandag.Gewijzigd = !canPerformAndConfirmChange;
                mandag.Bevestigingsdatum = DateTime.Now;
                mandag.Eindtijd = dtBegintijd.AddHours(8);

                mandag.IsChauffeurHeen = false;
                mandag.IsChauffeurTerug = false;
                mandag.KentekenHeen = "";
                mandag.KentekenTerug = "";

                //if (canPerformAndConfirmChange)
                //{
                //    mandag.Minuten = 0;
                //    mandag.Uren = 8;
                //    mandag.MinutenGewijzigd = 0;
                //    mandag.UrenGewijzigd = 0;
                //}
                //else
                //{


                //int intUren = Convert.ToInt32(((VakmanDagProject)sender).lblVakmandagUren.Content);
                //int intMinuten = Convert.ToInt32(((VakmanDagProject)sender).lblVakmandagMinuten.Content);

                mandag.Minuten = 0;
                mandag.Uren = 0;
                mandag.MinutenGewijzigd = 0;
                mandag.UrenGewijzigd = 8;

                //mandag.MinutenGewijzigd = intMinuten;
                //mandag.UrenGewijzigd = intUren;

                //}

                mandag.Mutatiedatum = DateTime.Now;
                mandag.Definitief = false;
                mandag.ProjectleiderId = objProjectleider.ProjectleiderId;
                mandag.MutatieDoorProjectleiderId = objProjectleider.ProjectleiderId;
                mandag.VakmansoortId = 1;
                mandag.VakmanstatusId = 1;
                if (((VakmanDagProject)sender).IsZiek)
                {
                    mandag.VakmanstatusId = 2;
                }
                else if (((VakmanDagProject)sender).IsVakantie)
                {
                    mandag.VakmanstatusId = 3;
                }

                dbrep.InsertMandag(mandag);
                // nieuwe mandag toevoegen

                // eigen project OF ziekte OF vakantie inplannen moet automatisch confirmed worden
                // if (canPerformAndConfirmChange && ((VakmanDagProject)sender).Project.ProjectleiderId == objProjectleider.ProjectleiderId || (((VakmanDagProject)sender).IsZiek || ((VakmanDagProject)sender).IsVakantie))

                if ((canPerformAndConfirmChange && ((VakmanDagProject)sender).ProjectId != 0 && ((VakmanDagProject)sender).Project.ProjectleiderId == objProjectleider.ProjectleiderId) || canPerformAndConfirmChange && ((VakmanDagProject)sender).ProjectId == 0)

                //if (canPerformAndConfirmChange && (((VakmanDagProject)sender).IsZiek || ((VakmanDagProject)sender).IsVakantie))
                {
                    //confirm myself
                    dbrep.ConfirmMandagenVoorVakmanId(objVakman.VakmanId, dtBegintijd, objProjectleider.ProjectleiderId);
                }

            }

            LoadVakmanDag();

            RoutedEventArgs args = new RoutedEventArgs(VakmanDagSelectEvent, sender);
            args.RoutedEvent = VakmanDagSelectEvent;
            RaiseEvent(args);

        }

        /// <summary>
        /// Wijzigingen goedkeuren voor de dag waarop geklikt is
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recOK_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dbRepository dbrep = new dbRepository();
            dbrep.ConfirmMandagenVoorVakmanId(objVakman.VakmanId, dtBegintijd, objProjectleider.ProjectleiderId);
            LoadVakmanDag();

            IsGewijzigd = false;
        }

        /// <summary>
        /// Annulleringen goedkeuren voor de dag waarop geklikt is
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recReadGeannulleerd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


            dbRepository dbrep = new dbRepository();
            dbrep.ResetMandagenGeannulleerdVoorVakmanId(objVakman.VakmanId, dtBegintijd, objProjectleider.ProjectleiderId);
            LoadVakmanDag();

            //IsGewijzigd = false;
        }


        
        /// <summary>
        /// Wijzigingen annuleren voor de dag waarop geklikt is
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recCancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dbRepository dbrep = new dbRepository();
            dbrep.CancelMandagenVoorVakmanId(objVakman.VakmanId, dtBegintijd, objProjectleider.ProjectleiderId);
            LoadVakmanDag();

            IsGewijzigd = false;
        }

        /// <summary>
        /// Wijzigingen goedkeuren voor alle dagen voor deze vakman
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReeksGoedkeuren_Click(object sender, RoutedEventArgs e)
        {
            dbRepository dbrep = new dbRepository();
            dbrep.ConfirmMandagenVoorVakmanId(objVakman.VakmanId, objProjectleider.ProjectleiderId);
            LoadVakmanDag();

            IsGewijzigd = false;

            RoutedEventArgs args = new RoutedEventArgs(VakmanDagEvent, sender);
            args.RoutedEvent = VakmanDagEvent;

            RaiseEvent(args);

        }

        /// <summary>
        /// Wijzigingen annuleren voor alle dagen voor deze vakman
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReeksAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            dbRepository dbrep = new dbRepository();
            dbrep.CancelMandagenVoorVakmanId(objVakman.VakmanId, objProjectleider.ProjectleiderId);
            LoadVakmanDag();

            IsGewijzigd = false;


            RoutedEventArgs args = new RoutedEventArgs(VakmanDagEvent, sender);
            args.RoutedEvent = VakmanDagEvent;

            RaiseEvent(args);

        }

        /// <summary>
        /// Wijzigingen annuleren voor de dag waarop geklikt is
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDagAnnuleren_Click_1(object sender, RoutedEventArgs e)
        {
            dbRepository dbrep = new dbRepository();
            dbrep.CancelMandagenVoorVakmanId(objVakman.VakmanId, dtBegintijd, objProjectleider.ProjectleiderId);
            LoadVakmanDag();

            IsGewijzigd = false;

        }

        /// <summary>
        /// Wijzigingen goedkeuren voor de dag waarop geklikt is
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDagGoedkeuren_Click_1(object sender, RoutedEventArgs e)
        {
            dbRepository dbrep = new dbRepository();
            dbrep.ConfirmMandagenVoorVakmanId(objVakman.VakmanId, dtBegintijd, objProjectleider.ProjectleiderId);
            LoadVakmanDag();

            IsGewijzigd = false;

        }

        /// <summary>
        /// Annuleren goedkeuren voor de dag waarop geklikt is
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDagReadGeannulleerd_Click_1(object sender, RoutedEventArgs e)
        {
            dbRepository dbrep = new dbRepository();
            dbrep.ResetMandagenGeannulleerdVoorVakmanId(objVakman.VakmanId, dtBegintijd, objProjectleider.ProjectleiderId);
            LoadVakmanDag();

            IsGewijzigd = false;

        }

        /// <summary>
        /// Annuleren goedkeuren voor de dag waarop geklikt is
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReeksReadGeannulleerd_Click_1(object sender, RoutedEventArgs e)
        {
            dbRepository dbrep = new dbRepository();
            dbrep.ResetMandagenGeannulleerdVoorVakmanId(objVakman.VakmanId, dtBegintijd, objProjectleider.ProjectleiderId);
            LoadVakmanDag();

            //IsGewijzigd = false;

        }

        private void spVakmandag_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            bool test = false;

            RoutedEventArgs args = new RoutedEventArgs(VakmanDagSelectEvent, sender);
            args.RoutedEvent = VakmanDagSelectEvent;
            RaiseEvent(args);


        }

        
    }
}

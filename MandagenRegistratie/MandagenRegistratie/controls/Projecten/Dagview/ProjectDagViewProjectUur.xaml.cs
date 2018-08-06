﻿using System;
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
using MandagenRegistratieDomain;
using System.Runtime.InteropServices;
using System.ComponentModel;
using ZeebregtsLogic;
using MandagenRegistratie.tools;

namespace MandagenRegistratie.controls.Projecten.Dagview
{
    /// <summary>
    /// Interaction logic for ProjectDagViewProjectUur.xaml
    /// </summary>
    public partial class ProjectDagViewProjectUur : UserControl
    {
        public ProjectDagViewProjectUur()
        {
            InitializeComponent();
        }


        public enum selectionMode
        {
            Deselecting = -1,
            Selecting = 1,
            Unknown = 0
        }

        public enum selectionDirection
        {
            Left = -1,
            Right = 1,
            Unknown = 0
        }

        public enum blockStatus
        {
            Current = 1,
            New = 0
        }

        public selectionMode enumSelectionMode;
        public selectionDirection enumSelectionDirection;



        #region "INotifyPropertyChanged"
        private bool isIngeplandQ1;
        public bool IsIngeplandQ1
        {
            get { return isIngeplandQ1; }
            set { isIngeplandQ1 = value; }
        }

        private bool isIngeplandQ2;
        public bool IsIngeplandQ2
        {
            get { return isIngeplandQ2; }
            set { isIngeplandQ2 = value; }
        }

        private bool isIngeplandQ3;
        public bool IsIngeplandQ3
        {
            get { return isIngeplandQ3; }
            set { isIngeplandQ3 = value; }
        }

        private bool isIngeplandQ4;
        public bool IsIngeplandQ4
        {
            get { return isIngeplandQ4; }
            set { isIngeplandQ4 = value; }
        }


        private bool isErgensDoorMijIngeplandQ1;
        public bool IsErgensDoorMijIngeplandQ1
        {
            get { return isErgensDoorMijIngeplandQ1; }
            set { isErgensDoorMijIngeplandQ1 = value; }
        }

        private bool isErgensDoorMijIngeplandQ2;
        public bool IsErgensDoorMijIngeplandQ2
        {
            get { return isErgensDoorMijIngeplandQ2; }
            set { isErgensDoorMijIngeplandQ2 = value; }
        }

        private bool isErgensDoorMijIngeplandQ3;
        public bool IsErgensDoorMijIngeplandQ3
        {
            get { return isErgensDoorMijIngeplandQ3; }
            set { isErgensDoorMijIngeplandQ3 = value; }
        }

        private bool isErgensDoorMijIngeplandQ4;
        public bool IsErgensDoorMijIngeplandQ4
        {
            get { return isErgensDoorMijIngeplandQ4; }
            set { isErgensDoorMijIngeplandQ4 = value; }
        }

        private bool isErgensDoorMijAangevraagdQ1AndCancelled;
        public bool IsErgensDoorMijAangevraagdQ1AndCancelled
        {
            get { return isErgensDoorMijAangevraagdQ1AndCancelled; }
            set { isErgensDoorMijAangevraagdQ1AndCancelled = value; }
        }

        private bool isErgensDoorMijAangevraagdQ2AndCancelled;
        public bool IsErgensDoorMijAangevraagdQ2AndCancelled
        {
            get { return isErgensDoorMijAangevraagdQ2AndCancelled; }
            set { isErgensDoorMijAangevraagdQ2AndCancelled = value; }
        }

        private bool isErgensDoorMijAangevraagdQ3AndCancelled;
        public bool IsErgensDoorMijAangevraagdQ3AndCancelled
        {
            get { return isErgensDoorMijAangevraagdQ3AndCancelled; }
            set { isErgensDoorMijAangevraagdQ3AndCancelled = value; }
        }

        private bool isErgensDoorMijAangevraagdQ4AndCancelled;
        public bool IsErgensDoorMijAangevraagdQ4AndCancelled
        {
            get { return isErgensDoorMijAangevraagdQ4AndCancelled; }
            set { isErgensDoorMijAangevraagdQ4AndCancelled = value; }
        }


        private bool isAangevraagdQ1;
        public bool IsAangevraagdQ1
        {
            get { return isAangevraagdQ1; }
            set { isAangevraagdQ1 = value; }
        }

        private bool isAangevraagdQ2;
        public bool IsAangevraagdQ2
        {
            get { return isAangevraagdQ2; }
            set { isAangevraagdQ2 = value; }
        }

        private bool isAangevraagdQ3;
        public bool IsAangevraagdQ3
        {
            get { return isAangevraagdQ3; }
            set { isAangevraagdQ3 = value; }
        }

        private bool isAangevraagdQ4;
        public bool IsAangevraagdQ4
        {
            get { return isAangevraagdQ4; }
            set { isAangevraagdQ4 = value; }
        }


        private bool isAangevraagdQ1AndCancelled;
        public bool IsAangevraagdQ1AndCancelled
        {
            get { return isAangevraagdQ1AndCancelled; }
            set { isAangevraagdQ1AndCancelled = value; }
        }

        private bool isAangevraagdQ2AndCancelled;
        public bool IsAangevraagdQ2AndCancelled
        {
            get { return isAangevraagdQ2AndCancelled; }
            set { isAangevraagdQ2AndCancelled = value; }
        }

        private bool isAangevraagdQ3AndCancelled;
        public bool IsAangevraagdQ3AndCancelled
        {
            get { return isAangevraagdQ3AndCancelled; }
            set { isAangevraagdQ3AndCancelled = value; }
        }

        private bool isAangevraagdQ4AndCancelled;
        public bool IsAangevraagdQ4AndCancelled
        {
            get { return isAangevraagdQ4AndCancelled; }
            set { isAangevraagdQ4AndCancelled = value; }
        }


        private bool isErgensAangevraagdQ1;
        public bool IsErgensAangevraagdQ1
        {
            get { return isErgensAangevraagdQ1; }
            set { isErgensAangevraagdQ1 = value; }
        }

        private bool isErgensAangevraagdQ2;
        public bool IsErgensAangevraagdQ2
        {
            get { return isErgensAangevraagdQ2; }
            set { isErgensAangevraagdQ2 = value; }
        }

        private bool isErgensAangevraagdQ3;
        public bool IsErgensAangevraagdQ3
        {
            get { return isErgensAangevraagdQ3; }
            set { isErgensAangevraagdQ3 = value; }
        }

        private bool isErgensAangevraagdQ4;
        public bool IsErgensAangevraagdQ4
        {
            get { return isErgensAangevraagdQ4; }
            set { isErgensAangevraagdQ4 = value; }
        }


        private bool isErgensDoorMijAangevraagdQ1;
        public bool IsErgensDoorMijAangevraagdQ1
        {
            get { return isErgensDoorMijAangevraagdQ1; }
            set { isErgensDoorMijAangevraagdQ1 = value; }
        }

        private bool isErgensDoorMijAangevraagdQ2;
        public bool IsErgensDoorMijAangevraagdQ2
        {
            get { return isErgensDoorMijAangevraagdQ2; }
            set { isErgensDoorMijAangevraagdQ2 = value; }
        }

        private bool isErgensDoorMijAangevraagdQ3;
        public bool IsErgensDoorMijAangevraagdQ3
        {
            get { return isErgensDoorMijAangevraagdQ3; }
            set { isErgensDoorMijAangevraagdQ3 = value; }
        }

        private bool isErgensDoorMijAangevraagdQ4;
        public bool IsErgensDoorMijAangevraagdQ4
        {
            get { return isErgensDoorMijAangevraagdQ4; }
            set { isErgensDoorMijAangevraagdQ4 = value; }
        }


        private bool isOwner;
        public bool IsOwner
        {
            get { return isOwner; }
            set { isOwner = value; }
        }

        private bool canApprove;
        public bool CanApprove
        {
            get { return canApprove; }
            set { canApprove = value; }
        }

        private bool canCancel;
        public bool CanCancel
        {
            get { return canCancel; }
            set { canCancel = value; }
        }

        private bool canConfirmHasSeen;
        public bool CanConfirmHasSeen
        {
            get { return canConfirmHasSeen; }
            set { canConfirmHasSeen = value; }
        }

        private bool isEnabledQ1;
        public bool IsEnabledQ1
        {
            get { return isEnabledQ1; }
            set { isEnabledQ1 = value; }
        }

        private bool isEnabledQ2;
        public bool IsEnabledQ2
        {
            get { return isEnabledQ2; }
            set { isEnabledQ2 = value; }
        }

        private bool isEnabledQ3;
        public bool IsEnabledQ3
        {
            get { return isEnabledQ3; }
            set { isEnabledQ3 = value; }
        }

        private bool isEnabledQ4;
        public bool IsEnabledQ4
        {
            get { return isEnabledQ4; }
            set { isEnabledQ4 = value; }
        }

        #endregion


        #region "RoutedEvents"

        public static readonly RoutedEvent VakmanDagViewEvent = EventManager.RegisterRoutedEvent("OnVakmanDagViewUpdate", RoutingStrategy.Bubble,
        typeof(RoutedEventHandler), typeof(ProjectDagViewProjectUur));

        public event RoutedEventHandler OnVakmanDagViewUpdate
        {
            add { AddHandler(VakmanDagViewEvent, value); }
            remove { RemoveHandler(VakmanDagViewEvent, value); }
        }

        public static readonly RoutedEvent VakmanDagViewRefreshEvent = EventManager.RegisterRoutedEvent("OnVakmanDagViewRefreshUpdate", RoutingStrategy.Bubble,
typeof(RoutedEventHandler), typeof(ProjectDagViewProjectUur));

        public event RoutedEventHandler OnVakmanDagViewRefreshUpdate
        {
            add { AddHandler(VakmanDagViewRefreshEvent, value); }
            remove { RemoveHandler(VakmanDagViewRefreshEvent, value); }
        }

        public static readonly RoutedEvent VakmanDagViewHighlightEvent = EventManager.RegisterRoutedEvent("OnVakmanDagViewHighlight", RoutingStrategy.Bubble,
typeof(RoutedEventHandler), typeof(ProjectDagViewProjectUur));

        public event RoutedEventHandler OnVakmanDagViewHighlight
        {
            add { AddHandler(VakmanDagViewHighlightEvent, value); }
            remove { RemoveHandler(VakmanDagViewHighlightEvent, value); }
        }


        public static readonly RoutedEvent VakmanDagViewHighlightOnLeaveEvent = EventManager.RegisterRoutedEvent("OnVakmanDagViewHighlightOnLeave", RoutingStrategy.Bubble,
typeof(RoutedEventHandler), typeof(ProjectDagViewProjectUur));

        public event RoutedEventHandler OnVakmanDagViewHighlightOnLeave
        {
            add { AddHandler(VakmanDagViewHighlightOnLeaveEvent, value); }
            remove { RemoveHandler(VakmanDagViewHighlightOnLeaveEvent, value); }
        }

        #endregion

        public bool IsSelectedQ1;
        public bool IsSelectedQ2;
        public bool IsSelectedQ3;
        public bool IsSelectedQ4;

        public bool IsDeSelectedQ1;
        public bool IsDeSelectedQ2;
        public bool IsDeSelectedQ3;
        public bool IsDeSelectedQ4;

        public Project project;
        public vwVakman vakman;
        public Mandagen mandag;

        public DateTime dtUur;

        public int Uur;

        public bool blnIsMouseButtonPressed = false;

        public int intPosY;

        public void SetColors()
        {
            // deze functie gebeurt bij laden pagina
            SetButtonColor(btn15, isIngeplandQ1, isAangevraagdQ1, IsSelectedQ1, isOwner, isEnabledQ1, isErgensAangevraagdQ1, IsDeSelectedQ1, isAangevraagdQ1AndCancelled, false, isErgensDoorMijIngeplandQ1, isErgensDoorMijAangevraagdQ1, isErgensDoorMijAangevraagdQ1AndCancelled);
            SetButtonColor(btn30, isIngeplandQ2, isAangevraagdQ2, IsSelectedQ2, isOwner, isEnabledQ2, isErgensAangevraagdQ2, IsDeSelectedQ2, isAangevraagdQ2AndCancelled, false, isErgensDoorMijIngeplandQ2, isErgensDoorMijAangevraagdQ2, isErgensDoorMijAangevraagdQ2AndCancelled);
            SetButtonColor(btn45, IsIngeplandQ3, isAangevraagdQ3, IsSelectedQ3, isOwner, isEnabledQ3, isErgensAangevraagdQ3, IsDeSelectedQ3, isAangevraagdQ3AndCancelled, false, isErgensDoorMijIngeplandQ3, isErgensDoorMijAangevraagdQ3, isErgensDoorMijAangevraagdQ3AndCancelled);
            SetButtonColor(btn60, IsIngeplandQ4, isAangevraagdQ4, IsSelectedQ4, isOwner, isEnabledQ4, isErgensAangevraagdQ4, IsDeSelectedQ4, isAangevraagdQ4AndCancelled, false, isErgensDoorMijIngeplandQ4, isErgensDoorMijAangevraagdQ4, isErgensDoorMijAangevraagdQ4AndCancelled);
        }


        public void SetColors(ProjectDagViewProjectUur vpu, int quarter)
        {
            switch (quarter)
            {
                case 1:
                    if (vpu.IsEnabledQ1)
                    {
                        SetButtonColor(vpu.btn15, vpu.IsIngeplandQ1, vpu.IsAangevraagdQ1, vpu.IsSelectedQ1, vpu.IsOwner, vpu.IsEnabledQ1, vpu.isErgensAangevraagdQ1, vpu.IsDeSelectedQ1, vpu.IsAangevraagdQ1AndCancelled, true, vpu.IsErgensDoorMijIngeplandQ1, vpu.IsErgensDoorMijAangevraagdQ1, vpu.IsErgensDoorMijAangevraagdQ1AndCancelled);
                    }
                    break;
                case 2:
                    if (vpu.IsEnabledQ2)
                    {
                        SetButtonColor(vpu.btn30, vpu.IsIngeplandQ2, vpu.IsAangevraagdQ2, vpu.IsSelectedQ2, vpu.IsOwner, vpu.IsEnabledQ2, vpu.isErgensAangevraagdQ2, vpu.IsDeSelectedQ2, vpu.IsAangevraagdQ2AndCancelled, true, vpu.IsErgensDoorMijIngeplandQ2, vpu.IsErgensDoorMijAangevraagdQ2, vpu.IsErgensDoorMijAangevraagdQ2AndCancelled);
                    }
                    break;
                case 3:
                    if (vpu.IsEnabledQ3)
                    {
                        SetButtonColor(vpu.btn45, vpu.IsIngeplandQ3, vpu.IsAangevraagdQ3, vpu.IsSelectedQ3, vpu.IsOwner, vpu.IsEnabledQ3, vpu.isErgensAangevraagdQ3, vpu.IsDeSelectedQ3, vpu.IsAangevraagdQ3AndCancelled, true, vpu.IsErgensDoorMijIngeplandQ3, vpu.IsErgensDoorMijAangevraagdQ3, vpu.IsErgensDoorMijAangevraagdQ3AndCancelled);
                    }
                    break;
                case 4:
                    if (vpu.IsEnabledQ4)
                    {
                        SetButtonColor(vpu.btn60, vpu.IsIngeplandQ4, vpu.IsAangevraagdQ4, vpu.IsSelectedQ4, vpu.IsOwner, vpu.IsEnabledQ4, vpu.isErgensAangevraagdQ4, vpu.IsDeSelectedQ4, vpu.IsAangevraagdQ4AndCancelled, true, vpu.IsErgensDoorMijIngeplandQ4, vpu.IsErgensDoorMijAangevraagdQ4, vpu.IsErgensDoorMijAangevraagdQ4AndCancelled);
                    }
                    break;
            }
        }

        private void SetButtonColor(Border button, bool isIngepland, bool isAangevraagd, bool isSelected, bool isowner, bool isEnabled, bool isErgensAangevraagd, bool isDeselected, bool isAangevraagdAndCancelled, bool IsMouseOver, bool isErgensDoorMijIngepland, bool isErgensDoorMijAangevraagd, bool isErgensDoorMijAangevraagdEnGecancelled)
        {


            // eigenaar van t project
            if (isowner)
            {
                // 
                if (((isIngepland && isErgensAangevraagd) || (isIngepland && !isSelected)) && !isDeselected)
                {
                    // oranje vlak
                    button.BorderThickness = new System.Windows.Thickness(1);
                    button.BorderBrush = new SolidColorBrush(Colors.Black); // Green weggehaald // Black weggehaald

                    if (isErgensAangevraagd)
                    {
                        button.Background = new SolidColorBrush(Colors.Yellow);
                    }
                    else
                    {
                        button.Background = new SolidColorBrush(Colors.Green);
                    }
                    //button.Background = new SolidColorBrush(Colors.Green);

                    ((TextBlock)button.Child).Visibility = System.Windows.Visibility.Visible;
                }
                else if ((isAangevraagd || isSelected) && !isDeselected)
                {
                    // groen vlak
                    if (IsMouseOver)
                    {
                        button.BorderBrush = new SolidColorBrush(Colors.Black);
                        button.Background = new SolidColorBrush(Colors.Gray);
                    }
                    else
                    {
                        if (isAangevraagd)
                        {
                            //button.BorderBrush = new SolidColorBrush(Colors.Orange);
                            button.BorderThickness = new System.Windows.Thickness(1);
                            button.BorderBrush = new SolidColorBrush(Colors.Black);

                            button.Background = new SolidColorBrush(Colors.Orange);
                        }
                        else
                        {
                            button.BorderThickness = new System.Windows.Thickness(1);
                            button.BorderBrush = new SolidColorBrush(Colors.Black);
                            button.Background = new SolidColorBrush(Colors.Green);
                        }
                    }

                    button.BorderThickness = new System.Windows.Thickness(1);
                    //button.BorderBrush = new SolidColorBrush(Colors.Gray);
                    ((TextBlock)button.Child).Visibility = System.Windows.Visibility.Visible;
                }
                else if (isAangevraagdAndCancelled)
                {
                    // blauw vlak
                    button.Background = new SolidColorBrush(Colors.Red);
                    button.BorderThickness = new System.Windows.Thickness(1);
                    button.BorderBrush = new SolidColorBrush(Colors.Black);
                    //button.BorderBrush = new SolidColorBrush(Colors.Gray);

                    // TODO: TEST THIS
                    ((TextBlock)button.Child).Visibility = System.Windows.Visibility.Visible;

                }
                else
                {
                    // wit
                    // oneven uren 'arceren'
                    if (Uur % 2 == 1)
                    {
                        button.BorderThickness = new System.Windows.Thickness(1);
                        button.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5D9F1")); // new SolidColorBrush(Colors.LightGray); // LightSteelBlue
                    }
                    else
                    {
                        button.BorderThickness = new System.Windows.Thickness(1);
                        button.Background = new SolidColorBrush(Colors.White);
                    }

                    // alleen randje eromheen als we hier iets kunnen wijzigen
                    if (isEnabled)
                    {
                        button.BorderBrush = new SolidColorBrush(Colors.Black);
                        //button.BorderBrush = new SolidColorBrush(Colors.Gray);
                    }
                    else if ((isErgensDoorMijIngepland && isErgensAangevraagd) || isErgensDoorMijAangevraagd || isErgensDoorMijAangevraagdEnGecancelled)
                    {
                        // transparant

                    }
                    // TODO: voor versie B, deze else gebruiken
                    else
                    //else if (!isErgensDoorMijIngepland && !isErgensAangevraagd) // TODO: versie A
                    {
                        button.BorderBrush = new SolidColorBrush(Colors.Gray);
                    }
                }
            }
            // geen eigenaar van t project
            else
            {
                // 
                if (((isIngepland && isErgensAangevraagd) || (isIngepland && !isSelected)) && !isDeselected)
                {
                    // oranje vlak
                    button.BorderThickness = new System.Windows.Thickness(1);
                    button.BorderBrush = new SolidColorBrush(Colors.Gray); // Gray
                    //button.BorderBrush = new SolidColorBrush(Colors.Gray);

                    if (isErgensAangevraagd)
                    {
                        button.Background = new SolidColorBrush(Colors.Yellow);
                    }
                    else
                    {
                        button.Background = new SolidColorBrush(Colors.Green);
                    }
                    //button.Background = new SolidColorBrush(Colors.Green);
                    ((TextBlock)button.Child).Visibility = System.Windows.Visibility.Visible;
                }
                else if ((isAangevraagd || isSelected) && !isDeselected)
                {
                    // rood vlak
                    button.BorderThickness = new System.Windows.Thickness(1);
                    button.BorderBrush = new SolidColorBrush(Colors.Gray);
                    //button.BorderBrush = new SolidColorBrush(Colors.Gray);

                    if (isAangevraagd)
                    {
                        //button.BorderBrush = new SolidColorBrush(Colors.Orange);
                        button.BorderThickness = new System.Windows.Thickness(1);
                        button.BorderBrush = new SolidColorBrush(Colors.Gray);

                        button.Background = new SolidColorBrush(Colors.Orange);
                    }
                    else
                    {
                        button.Background = new SolidColorBrush(Colors.Green);
                    }

                    ((TextBlock)button.Child).Visibility = System.Windows.Visibility.Visible;
                }
                // hoeft niks blauws te zien als die geen eigenaar is van t project
                else if (isAangevraagdAndCancelled)
                {
                    // rood vlak grijze rand 
                    button.Background = new SolidColorBrush(Colors.Red);
                    button.BorderThickness = new System.Windows.Thickness(1);
                    button.BorderBrush = new SolidColorBrush(Colors.Gray);

                    ((TextBlock)button.Child).Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    // wit
                    // oneven uren 'arceren'
                    if (Uur % 2 == 1)
                    {
                        button.BorderThickness = new System.Windows.Thickness(1);
                        button.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFC5D9F1")); // new SolidColorBrush(Colors.LightGray); // LightSteelBlue
                    }
                    else
                    {
                        button.BorderThickness = new System.Windows.Thickness(1);
                        button.Background = new SolidColorBrush(Colors.White);
                    }

                    // alleen randje eromheen als we hier iets kunnen wijzigen
                    //if (!isErgensAangevraagd)
                    //{
                    button.BorderBrush = new SolidColorBrush(Colors.Gray);
                    //}
                }
            }

        }

        private StackPanel GetTooltipText()
        {
            return GetTooltipText(false, false);
        }

        private StackPanel GetTooltipText(bool isAanvraag)
        {
            return GetTooltipText(isAanvraag, false);
        }

        private StackPanel GetTooltipText(bool isAanvraag, bool hasAanvraag)
        {
            StackPanel strTooltip = new StackPanel();

            if (mandag != null)
            {
                dbRepository dbrep = new dbRepository();
                dbOriginalRepository dbo = new dbOriginalRepository();
                MDRpersoon persoon = dbo.GetContact(vakman.ContactIdOrigineel, true);
                MDRproject objProjectNaar = dbo.GetProject((int)project.ProjectNr, true);
                DbTools dbtools = new DbTools();

                //if (!isAanvraag)
                //{
                //strTooltip.AddText(objProjectNaar.naam_project + " (Project " + objProjectNaar.project_NR.ToString() + ")");
                //strTooltip.AddText("(" + dbtools.GetIngeplandeTijd(mandag.Project, mandag.Vakman, dbtools.GetHeleDag(mandag.Begintijd)) + ") " + (persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam).ToStringTrimmed());
                //strTooltip.AddText((persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam).ToStringTrimmed(), true);
                //}

                if (isAanvraag)
                {
                    List<Mandagen> listMandagenTotal = dbrep.GetMandagen(vakman.VakmanId, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay)).Where(m => m.Begintijd != m.Eindtijd).ToList();

                    // als in deze pool van mandagen een aanvraag zit:
                    // vandaag, ander project, geen projectleider, wel bevestigd (dus waar dit de aanvraag van is) 
                    List<Mandagen> listMandagenWaarDitDeAanvraagVanIs = listMandagenTotal.Where(m => m.ProjectId != mandag.ProjectId && m.ProjectleiderId != ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).ProjectleiderId && m.ProjectId != mandag.ProjectId && m.Begintijd >= ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay) && m.Eindtijd <= ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay).AddDays(1) && m.Status).ToList();

                    //Mandagen origineel = listMandagenWaarDitDeAanvraagVanIs.FirstOrDefault(m => m.Begintijd <= mandag.Eindtijd && m.Eindtijd >= mandag.Begintijd && m.VakmanId == mandag.VakmanId);
                    //if (origineel != null)
                    //{
                    //    Project objProjectVan = origineel.Project;
                    //    MDRpersoon persoonVan = dbo.GetContact(origineel.Vakman.ContactIdOrigineel, true);
                    //    strTooltip += objProjectVan.Naam + " (Project " + objProjectVan.ProjectNr.ToString() + ")";

                    //    strTooltip += Environment.NewLine;
                    //    strTooltip += " (" + ProjectDagProjectHeader.ToonTijd(origineel) + ") " + (persoonVan.voornaam + " " + persoonVan.tussenvoegsel + " " + persoonVan.achternaam).ToStringTrimmed();
                    //}


                    tools.DbTools t = new tools.DbTools();
                    //strTooltip.Children.Add(t.AddTooltipAanvraag(listMandagenWaarDitDeAanvraagVanIs, mandag, true, true));
                    strTooltip.Children.Add(t.AddTooltipAanvraagAll(mandag.Vakman, mandag.Project, t.GetHeleDag(mandag.Begintijd), false, false, listMandagenTotal));
                }

                if (hasAanvraag)
                {

                    //strTooltip.AddText(Environment.NewLine);
                    List<Mandagen> listMandagenTotal = dbrep.GetMandagen(vakman.VakmanId, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay)).Where(m => m.Begintijd != m.Eindtijd).ToList();

                    // vandaag, ander project, geen projectleider, niet bevestigd (dus aanvraag) 
                    List<Mandagen> listAanvragen = listMandagenTotal.Where(m => m.ProjectleiderId != ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).ProjectleiderId && m.ProjectId != project.ProjectId && m.Begintijd >= ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay) && m.Eindtijd <= ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay).AddDays(1) && !m.Status).ToList();

                    // OF we hebben te maken met een externe aanvraag
                    Mandagen aanvraag = listAanvragen.FirstOrDefault(m => m.Begintijd < mandag.Eindtijd && m.Eindtijd > mandag.Begintijd && m.VakmanId == mandag.VakmanId);

                    if (aanvraag != null)
                    {
                        List<Mandagen> listIngeplandeMandagen = new List<Mandagen>();
                        listIngeplandeMandagen.Add(mandag);
                        tools.DbTools t = new tools.DbTools();
                        //strTooltip.Children.Add(t.AddTooltipAanvraag(listIngeplandeMandagen, aanvraag, false, true));
                        strTooltip.Children.Add(t.AddTooltipAanvraagAll(mandag.Vakman, mandag.Project, t.GetHeleDag(mandag.Begintijd), false, false, listMandagenTotal));

                    }

                }


                if (!isAanvraag && !hasAanvraag)
                {
                    List<Mandagen> listMandagenTotal = dbrep.GetMandagen(vakman.VakmanId, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay)).Where(m => m.Begintijd != m.Eindtijd).ToList();
                    tools.DbTools t = new tools.DbTools();
                    strTooltip.Children.Add(t.AddTooltipAanvraagAll(mandag.Vakman, mandag.Project, t.GetHeleDag(mandag.Begintijd), false, false, listMandagenTotal, false));
                }

            }

            return strTooltip;
        }

        public void SetTooltips()
        {
            StackPanel tooltip = new StackPanel();
            StackPanel tooltip2 = new StackPanel();
            StackPanel tooltip3 = new StackPanel();
            StackPanel tooltip4 = new StackPanel();

            if (btn15.Background.ToString() == (new SolidColorBrush(Colors.Green)).ToString())
            {
                tooltip = GetTooltipText();
                btn15.ToolTip = tooltip;
            }
            else if (btn15.Background.ToString() == (new SolidColorBrush(Colors.Orange)).ToString())
            {
                tooltip = GetTooltipText(true);
                btn15.ToolTip = tooltip;
            }
            else if (btn15.Background.ToString() == (new SolidColorBrush(Colors.Red)).ToString())
            {
                tooltip = GetTooltipText(true);
                btn15.ToolTip = tooltip;
            }
            else if (btn15.Background.ToString() == (new SolidColorBrush(Colors.Yellow)).ToString())
            {
                tooltip = GetTooltipText(false, true);
                btn15.ToolTip = tooltip;
            }

            if (btn30.Background.ToString() == (new SolidColorBrush(Colors.Green)).ToString())
            {
                tooltip2 = GetTooltipText();
                btn30.ToolTip = tooltip2;
            }
            else if (btn30.Background.ToString() == (new SolidColorBrush(Colors.Orange)).ToString())
            {
                tooltip2 = GetTooltipText(true);
                btn30.ToolTip = tooltip2;
            }
            else if (btn30.Background.ToString() == (new SolidColorBrush(Colors.Red)).ToString())
            {
                tooltip2 = GetTooltipText(true);
                btn30.ToolTip = tooltip2;
            }
            else if (btn30.Background.ToString() == (new SolidColorBrush(Colors.Yellow)).ToString())
            {
                tooltip2 = GetTooltipText(false, true);
                btn30.ToolTip = tooltip2;
            }

            if (btn45.Background.ToString() == (new SolidColorBrush(Colors.Green)).ToString())
            {
                tooltip3 = GetTooltipText();
                btn45.ToolTip = tooltip3;
            }
            else if (btn45.Background.ToString() == (new SolidColorBrush(Colors.Orange)).ToString())
            {
                tooltip3 = GetTooltipText(true);
                btn45.ToolTip = tooltip3;
            }
            else if (btn45.Background.ToString() == (new SolidColorBrush(Colors.Red)).ToString())
            {
                tooltip3 = GetTooltipText(true);
                btn45.ToolTip = tooltip3;
            }
            else if (btn45.Background.ToString() == (new SolidColorBrush(Colors.Yellow)).ToString())
            {
                tooltip3 = GetTooltipText(false, true);
                btn45.ToolTip = tooltip3;
            }

            if (btn60.Background.ToString() == (new SolidColorBrush(Colors.Green)).ToString())
            {
                tooltip4 = GetTooltipText();
                btn60.ToolTip = tooltip4;
            }
            else if (btn60.Background.ToString() == (new SolidColorBrush(Colors.Orange)).ToString())
            {
                tooltip4 = GetTooltipText(true);
                btn60.ToolTip = tooltip4;

            }
            else if (btn60.Background.ToString() == (new SolidColorBrush(Colors.Red)).ToString())
            {
                tooltip4 = GetTooltipText(true);
                btn60.ToolTip = tooltip4;

            }
            else if (btn60.Background.ToString() == (new SolidColorBrush(Colors.Yellow)).ToString())
            {
                tooltip4 = GetTooltipText(false, true);
                btn60.ToolTip = tooltip4;
            }


            ToolTipService.SetShowDuration(btn15, 20000);
            ToolTipService.SetShowDuration(btn30, 20000);
            ToolTipService.SetShowDuration(btn45, 20000);
            ToolTipService.SetShowDuration(btn60, 20000);

        }

        /// <summary>
        /// 
        /// </summary>
        private void MoveCursor()
        {
            // Set the Current cursor, move the cursor's Position,
            // and set its clipping rectangle to the form. 

            Point absoluteScreenPos = PointToScreen(Mouse.GetPosition(this));
            SetPosition(Convert.ToInt32(absoluteScreenPos.X), intPosY);

            //((VakmanDagView)((StackPanel)((StackPanel)((ScrollViewer)((WrapPanel)((WrapPanel)this.Parent).Parent).Parent).Parent).Parent).Parent).lblCorcor.Content = Mouse.GetPosition((WrapPanel)this.Parent).X.ToString();

            //if (Mouse.GetPosition((WrapPanel)this.Parent).X > ((ScrollViewer)((WrapPanel)((WrapPanel)this.Parent).Parent).Parent).HorizontalOffset + 880)
            if (Mouse.GetPosition((WrapPanel)this.Parent).X > Tools.FindVisualParent<ScrollViewer>(this).HorizontalOffset + Tools.FindVisualParent<ScrollViewer>(this).ActualWidth - 4)
            {
                SetPosition(Convert.ToInt32(absoluteScreenPos.X - 10), intPosY);

                Tools.FindVisualParent<ScrollViewer>(this).ScrollToHorizontalOffset(Tools.FindVisualParent<ScrollViewer>(this).HorizontalOffset + 20);
            }
            else if (Mouse.GetPosition((WrapPanel)this.Parent).X < Tools.FindVisualParent<ScrollViewer>(this).HorizontalOffset + 4)
            {
                SetPosition(Convert.ToInt32(absoluteScreenPos.X + 10), intPosY);

                Tools.FindVisualParent<ScrollViewer>(this).ScrollToHorizontalOffset(Tools.FindVisualParent<ScrollViewer>(this).HorizontalOffset - 20);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private void SetPosition(int a, int b)
        {
            SetCursorPos(a, b);
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);


        public bool IsInRange(ProjectDagViewProjectUur vp, int quarter)
        {
            if (IsExitingRange(vp, quarter))
            {
                return false;
            }
            else
            {

                if ((selectionDirection)ApplicationState.GetValue<int>("selectionDirection") == selectionDirection.Right)
                {
                    return (vp.Uur * 4 + quarter) >= ApplicationState.GetValue<int>("MouseFirstEnterPosition") && (vp.Uur * 4 + quarter) <= ApplicationState.GetValue<int>("MouseLastEnterPosition");
                }
                else if ((selectionDirection)ApplicationState.GetValue<int>("selectionDirection") == selectionDirection.Left)
                {
                    return (vp.Uur * 4 + quarter) >= ApplicationState.GetValue<int>("MouseLastEnterPosition") && (vp.Uur * 4 + quarter) <= ApplicationState.GetValue<int>("MouseFirstEnterPosition");
                }
                else if ((vp.Uur * 4 + quarter) == ApplicationState.GetValue<int>("MouseFirstEnterPosition"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public bool IsExitingRange(ProjectDagViewProjectUur vp, int quarter)
        {
            try
            {
                Point p = new Point();
                switch (quarter)
                {
                    case 1:
                        p = Mouse.GetPosition(((ProjectDagViewProjectUur)vp).btn15);
                        break;
                    case 2:
                        p = Mouse.GetPosition(((ProjectDagViewProjectUur)vp).btn30);
                        break;
                    case 3:
                        p = Mouse.GetPosition(((ProjectDagViewProjectUur)vp).btn45);
                        break;
                    case 4:
                        p = Mouse.GetPosition(((ProjectDagViewProjectUur)vp).btn60);
                        break;
                }

                if ((selectionDirection)ApplicationState.GetValue<int>("selectionDirection") == selectionDirection.Right)
                {
                    return p.X < 0;
                }
                else if ((selectionDirection)ApplicationState.GetValue<int>("selectionDirection") == selectionDirection.Left)
                {
                    return p.X > 20;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }


        public void SetAllColors(ProjectDagViewProjectUur vpSender)
        {
            foreach (ProjectDagViewProjectUur vp in ((WrapPanel)this.Parent).Children)
            {
                if (vp.vakman == vpSender.vakman)
                {

                    if ((selectionMode)ApplicationState.GetValue<int>("selectionMode") == selectionMode.Selecting)
                    {
                        if (IsInRange(vp, 1) && vp.IsEnabledQ1)
                        {
                            vp.IsSelectedQ1 = true;
                            vp.IsDeSelectedQ1 = false;
                        }
                        else if (!vp.IsIngeplandQ1 && vp.IsEnabledQ1)
                        {
                            vp.IsSelectedQ1 = false;
                            vp.IsDeSelectedQ1 = true;
                        }

                        if (IsInRange(vp, 2) && vp.IsEnabledQ2)
                        {
                            vp.IsSelectedQ2 = true;
                            vp.IsDeSelectedQ2 = false;
                        }
                        else if (!vp.IsIngeplandQ2 && vp.IsEnabledQ2)
                        {
                            vp.IsSelectedQ2 = false;
                            vp.IsDeSelectedQ2 = true;
                        }

                        if (IsInRange(vp, 3) && vp.IsEnabledQ3)
                        {
                            vp.IsSelectedQ3 = true;
                            vp.IsDeSelectedQ3 = false;
                        }
                        else if (!vp.IsIngeplandQ3 && vp.IsEnabledQ3)
                        {
                            vp.IsSelectedQ3 = false;
                            vp.IsDeSelectedQ3 = true;
                        }

                        if (IsInRange(vp, 4) && vp.IsEnabledQ4)
                        {
                            vp.IsSelectedQ4 = true;
                            vp.IsDeSelectedQ4 = false;
                        }
                        else if (!vp.IsIngeplandQ4 && vp.IsEnabledQ4)
                        {
                            vp.IsSelectedQ4 = false;
                            vp.IsDeSelectedQ4 = true;
                        }
                    }
                    else if ((selectionMode)ApplicationState.GetValue<int>("selectionMode") == selectionMode.Deselecting)
                    {
                        if (IsInRange(vp, 1) && vp.IsEnabledQ1)
                        {
                            vp.IsSelectedQ1 = false;
                            vp.IsDeSelectedQ1 = true;
                        }
                        else if (vp.IsIngeplandQ1 && vp.IsEnabledQ1)
                        {
                            vp.IsSelectedQ1 = true;
                            vp.IsDeSelectedQ1 = false;
                        }


                        if (IsInRange(vp, 2) && vp.IsEnabledQ2)
                        {
                            vp.IsSelectedQ2 = false;
                            vp.IsDeSelectedQ2 = true;
                        }
                        else if (vp.IsIngeplandQ2 && vp.IsEnabledQ2)
                        {
                            vp.IsSelectedQ2 = true;
                            vp.IsDeSelectedQ2 = false;
                        }

                        if (IsInRange(vp, 3) && vp.IsEnabledQ3)
                        {
                            vp.IsSelectedQ3 = false;
                            vp.IsDeSelectedQ3 = true;
                        }
                        else if (vp.IsIngeplandQ3 && vp.IsEnabledQ3)
                        {
                            vp.IsSelectedQ3 = true;
                            vp.IsDeSelectedQ3 = false;
                        }

                        if (IsInRange(vp, 4) && vp.IsEnabledQ4)
                        {
                            vp.IsSelectedQ4 = false;
                            vp.IsDeSelectedQ4 = true;
                        }
                        else if (vp.IsIngeplandQ4 && vp.IsEnabledQ4)
                        {
                            vp.IsSelectedQ4 = true;
                            vp.IsDeSelectedQ4 = false;
                        }
                    }


                    vp.SetColors();
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="e"></param>
        /// <param name="isIngepland"></param>
        /// <param name="isAangevraagd"></param>
        /// <param name="isSelected"></param>
        /// <param name="quarter"></param>
        private void SetMouseActions(Border button, MouseEventArgs e, bool isIngepland, bool isAangevraagd, bool isSelected, int quarter)
        {
            try
            {
                if (IsOwner)
                {


                    if (e.LeftButton == MouseButtonState.Pressed)
                    {

                        ApplicationState.SetValue(ApplicationVariables.intVakmanIdSelecting, vakman.VakmanId);


                        Point absoluteScreenPos = PointToScreen(Mouse.GetPosition(this));
                        ApplicationState.SetValue("MouseLastEnterPosition", this.Uur * 4 + quarter);


                        if ((selectionDirection)ApplicationState.GetValue<int>("selectionDirection") == selectionDirection.Unknown && ApplicationState.GetValue<int>("MouseFirstEnterPosition") > 0 && ApplicationState.GetValue<int>("MouseLastEnterPosition") > 0)
                        {
                            if (ApplicationState.GetValue<int>("MouseFirstEnterPosition") > ApplicationState.GetValue<int>("MouseLastEnterPosition"))
                            {
                                ApplicationState.SetValue("selectionDirection", selectionDirection.Left);
                            }
                            else if (ApplicationState.GetValue<int>("MouseFirstEnterPosition") < ApplicationState.GetValue<int>("MouseLastEnterPosition"))
                            {
                                ApplicationState.SetValue("selectionDirection", selectionDirection.Right);
                            }

                        }




                        if (!blnIsMouseButtonPressed)
                        {


                            //Mouse.SetCursor(Cursors.SizeWE);


                            // test to lock the mouse
                            System.Drawing.Rectangle r = new System.Drawing.Rectangle(10, (int)absoluteScreenPos.Y, 2000, (int)absoluteScreenPos.Y + 1);
                            ClipCursor(ref r);

                            intPosY = Convert.ToInt32(absoluteScreenPos.Y - Mouse.GetPosition(this).Y + 30);
                            blnIsMouseButtonPressed = true;

                            if (ApplicationState.GetValue<int>("MouseFirstEnterPosition") == 0)
                            {
                                ApplicationState.SetValue("MouseFirstEnterPosition", this.Uur * 4 + quarter);
                                ApplicationState.SetValue("MouseFirstEnterPositionCoordinate", absoluteScreenPos.X);
                            }


                            // bij eerste click, stel de selectionMode in
                            if ((selectionMode)ApplicationState.GetValue<int>("selectionMode") == selectionMode.Unknown)
                            {
                                if (isSelected)
                                {
                                    ApplicationState.SetValue("selectionMode", (int)selectionMode.Deselecting);
                                }
                                else
                                {
                                    ApplicationState.SetValue("selectionMode", (int)selectionMode.Selecting);
                                }
                            }
                        }

                        // are we selecting or deselecting
                        selectionMode smode = (selectionMode)ApplicationState.GetValue<int>("selectionMode");

                        
                        MoveCursor();

                        //if (IsAllowedToChange(quarter, true))
                        //{

                        //if (!IsExitingRange())
                        //{
                        //    ApplicationState.SetValue("MouseExitedRange", false);
                        //}

                        //if (ApplicationState.GetValue<bool>("MouseExitedRange") == false)
                        //{


                            switch (quarter)
                            {
                                case 1:
                                    if (isEnabledQ1)
                                    {
                                        if (smode == selectionMode.Selecting)
                                        {
                                            IsSelectedQ1 = true;
                                            IsDeSelectedQ1 = false;

                                            ProjectDagViewProjectUur vpSender = (ProjectDagViewProjectUur)this;
                                            SetAllColors(vpSender);

                                            //ToggleOthers(vpSender, quarter, 0);
                                        }
                                        else if (smode == selectionMode.Deselecting)
                                        {
                                            IsSelectedQ1 = false;
                                            IsDeSelectedQ1 = true;

                                            ProjectDagViewProjectUur vpSender = (ProjectDagViewProjectUur)this;
                                            SetAllColors(vpSender);
                                        }


                                    }
                                    break;
                                case 2:
                                    if (isEnabledQ2)
                                    {
                                        if (smode == selectionMode.Selecting)
                                        {
                                            IsSelectedQ2 = true;
                                            IsDeSelectedQ2 = false;

                                            ProjectDagViewProjectUur vpSender = (ProjectDagViewProjectUur)this;
                                            SetAllColors(vpSender);

                                            //ToggleOthers(vpSender, quarter, 0);
                                        }
                                        else if (smode == selectionMode.Deselecting)
                                        {
                                            IsSelectedQ2 = false;
                                            IsDeSelectedQ2 = true;

                                            ProjectDagViewProjectUur vpSender = (ProjectDagViewProjectUur)this;
                                            SetAllColors(vpSender);

                                        }
                                    }
                                    break;
                                case 3:
                                    if (isEnabledQ3)
                                    {
                                        if (smode == selectionMode.Selecting)
                                        {
                                            IsSelectedQ3 = true;
                                            IsDeSelectedQ3 = false;

                                            ProjectDagViewProjectUur vpSender = (ProjectDagViewProjectUur)this;
                                            SetAllColors(vpSender);

                                            //ToggleOthers(vpSender, quarter, 0);
                                        }
                                        else if (smode == selectionMode.Deselecting)
                                        {
                                            IsSelectedQ3 = false;
                                            IsDeSelectedQ3 = true;

                                            ProjectDagViewProjectUur vpSender = (ProjectDagViewProjectUur)this;
                                            SetAllColors(vpSender);

                                        }
                                    }
                                    break;
                                case 4:
                                    if (isEnabledQ4)
                                    {
                                        if (smode == selectionMode.Selecting)
                                        {
                                            IsSelectedQ4 = true;
                                            IsDeSelectedQ4 = false;

                                            ProjectDagViewProjectUur vpSender = (ProjectDagViewProjectUur)this;
                                            SetAllColors(vpSender);

                                            //ToggleOthers(vpSender, quarter, 0);
                                        }
                                        else if (smode == selectionMode.Deselecting)
                                        {
                                            IsSelectedQ4 = false;
                                            IsDeSelectedQ4 = true;

                                            ProjectDagViewProjectUur vpSender = (ProjectDagViewProjectUur)this;
                                            SetAllColors(vpSender);

                                        }
                                    }
                                    break;
                            }
                            //}
                        //}

                    }

                    else if (e.LeftButton == MouseButtonState.Released)
                    {
                        // test release mouse again
                        ClipCursor(IntPtr.Zero);

                        if (blnIsMouseButtonPressed)
                        {
                            //ApplicationState.SetValue("selectionDirection", (int)selectionDirection.Unknown);
                            //ApplicationState.SetValue("selectionMode", (int)selectionMode.Unknown);
                            //ApplicationState.SetValue("LastMouseLeavePosition", 0);
                            //ApplicationState.SetValue("MouseFirstEnterPosition", 0);
                            //ApplicationState.SetValue("MouseFirstEnterPositionHold", 0);

                            //ProjectDagView pdvParent = Tools.FindVisualParent<ProjectDagView>(this);
                            //pdvParent.ConfirmUpdate(this.vakman.VakmanId);

                            //RaiseEvent(new RoutedEventArgs(VakmanDagViewEvent, this));

                            blnIsMouseButtonPressed = false;
                        }
                    }
                    else if (e.RightButton == MouseButtonState.Released)
                    {
                        //if (blnIsMouseButtonPressed)
                        //{
                        //    ApplicationState.SetValue("selectionDirection", (int)selectionDirection.Unknown);
                        //    ApplicationState.SetValue("selectionMode", (int)selectionMode.Unknown);
                        //    ApplicationState.SetValue("LastMouseLeavePosition", 0);
                        //    ApplicationState.SetValue("MouseFirstEnterPosition", 0);
                        //    ApplicationState.SetValue("MouseFirstEnterPositionHold", 0);

                        //    RaiseEvent(new RoutedEventArgs(VakmanDagViewEvent, this));

                        //    blnIsMouseButtonPressed = false;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                // do something here
                //MessageBox.Show("Er is een error opgetreden, het systeem was nog niet klaar met het verwerken van de uren van de vorige actie. U kunt nu weer verder gaan.");
            }
        }

        [DllImport("user32.dll")]
        static extern void ClipCursor(ref System.Drawing.Rectangle rect);

        [DllImport("user32.dll")]
        static extern void ClipCursor(IntPtr rect);



        private void btn15_MouseMove(object sender, MouseEventArgs e)
        {
            SetMouseActions(btn15, e, isIngeplandQ1, isAangevraagdQ1, IsSelectedQ1, 1);
        }

        private void btn30_MouseMove(object sender, MouseEventArgs e)
        {
            SetMouseActions(btn30, e, isIngeplandQ2, isAangevraagdQ2, IsSelectedQ2, 2);
        }

        private void btn45_MouseMove(object sender, MouseEventArgs e)
        {
            SetMouseActions(btn45, e, isIngeplandQ3, isAangevraagdQ3, IsSelectedQ3, 3);
        }

        private void btn60_MouseMove(object sender, MouseEventArgs e)
        {
            SetMouseActions(btn60, e, isIngeplandQ4, isAangevraagdQ4, IsSelectedQ4, 4);
        }



        //protected bool IsAllowedToChange(int quarter, bool isEntering)
        //{
        //    // na het togglen van de anderen, checken of we voorbij het beginpunt zijn gegaan, indien dat het geval 
        //    // is, blokkeren voor verdere acties

        //    int firstEnterPosition = ApplicationState.GetValue<int>("MouseFirstEnterPosition");
        //    int lastEnterPosition = ApplicationState.GetValue<int>("MouseLastEnterPosition");
        //    int lastLeavePosition = ApplicationState.GetValue<int>("LastMouseLeavePosition");
        //    selectionDirection originalDirection = (selectionDirection)ApplicationState.GetValue<int>("selectionDirection");


        //    if (isEntering)
        //    {
        //        // normally we start to the right
        //        if (originalDirection == selectionDirection.Right && (lastEnterPosition > firstEnterPosition || lastLeavePosition == 0))
        //        {
        //            return true;
        //        }
        //        else if (originalDirection == selectionDirection.Left && (lastEnterPosition < firstEnterPosition || lastLeavePosition == 0))// we started to the left
        //        {
        //            return true;
        //        }
        //        else if (originalDirection == selectionDirection.Unknown)
        //        {
        //            return true;
        //        }
        //        //else if (CurrentSelectionDirection(quarter) == selectionDirection.Right && (lastLeavePosition) >= firstEnterPosition)
        //        //{
        //        //    //MessageBox.Show("lastLeavePosition: " + lastLeavePosition.ToString() +  " firstEnterPosition: " + firstEnterPosition.ToString());
        //        //    return true;
        //        //}
        //        //else if (CurrentSelectionDirection() == selectionDirection.Left  && lastLeavePosition - 30 <= firstEnterPosition)
        //        //{
        //        //    return true;
        //        //}
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else // is leaving
        //    {
        //        // normally we start to the right
        //        if (originalDirection == selectionDirection.Right && (lastEnterPosition >= firstEnterPosition || lastLeavePosition == 0))
        //        {
        //            return true;
        //        }
        //        else if (originalDirection == selectionDirection.Left && (lastEnterPosition <= firstEnterPosition || lastLeavePosition == 0))// we started to the left
        //        {
        //            return true;
        //        }
        //        else if (originalDirection == selectionDirection.Unknown)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }

        //    }

        //}



        /// <summary>
        /// 
        /// </summary>
        /// <param name="vpSender"></param>
        /// <param name="Q"></param>
        /// <param name="state"></param>
        public void ToggleOthers(ProjectDagViewProjectUur vpSender, int Q, int state)
        {
            foreach (ProjectDagViewProjectUur vp in ((WrapPanel)this.Parent).Children)
            {
                if (vp.Uur == vpSender.Uur && vp != vpSender)
                {
                    ToggleButtons(vp, Q, state, vpSender.IsOwner);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vp"></param>
        /// <param name="Q"></param>
        /// <param name="state"></param>
        public void ToggleButtons(ProjectDagViewProjectUur vp, int Q, int state, bool hoverOverOwner)
        {
            switch (Q)
            {
                case 1:
                    if (state == 0)
                    {
                        vp.IsSelectedQ1 = false;
                        vp.IsDeSelectedQ1 = true;
                        //if (vp.IsOwner && hoverOverOwner)
                        //{
                        //    vp.IsDeSelectedQ1 = true;
                        //}
                        SetColors(vp, 1);
                    }
                    break;
                case 2:
                    if (state == 0)
                    {
                        vp.IsSelectedQ2 = false;
                        vp.IsDeSelectedQ2 = true;
                        //if (vp.IsOwner && hoverOverOwner)
                        //{
                        //    vp.IsDeSelectedQ2 = true;
                        //}
                        SetColors(vp, 2);
                    }
                    break;
                case 3:
                    if (state == 0)
                    {
                        vp.IsSelectedQ3 = false;
                        vp.IsDeSelectedQ3 = true;
                        //if (vp.IsOwner && hoverOverOwner)
                        //{
                        //    vp.IsDeSelectedQ3 = true;
                        //}
                        SetColors(vp, 3);
                    }
                    break;
                case 4:
                    if (state == 0)
                    {
                        vp.IsSelectedQ4 = false;
                        vp.IsDeSelectedQ4 = true;
                        //if (vp.IsOwner && hoverOverOwner)
                        //{
                        //    vp.IsDeSelectedQ4 = true;
                        //}
                        SetColors(vp, 4);
                    }
                    break;

            }
        }

        private void btnApprove_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (canApprove)
            {
                dbRepository dbrep = new dbRepository();

                // activemerge niet van toepassing
                dbrep.ConfirmMandag(mandag, true);


                // refresh alle andere programma's
                MenuControl owner = Tools.FindVisualParent<MenuControl>(this);
//#if DEBUG
//#else

                DateTime dt1 = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
                DateTime dt2 = dt1.AddDays(1);
                List<int> vakmanIds = new List<int>();
                vakmanIds.Add(vakman.VakmanId);

                owner.PageChannelMessage("projectdagview", Tools.CreateChannelMessage(vakmanIds, dt1, dt2));

//#endif
                
                //RaiseEvent(new RoutedEventArgs(VakmanDagViewRefreshEvent, this));
                ProjectDagView pdvParent = Tools.FindVisualParent<ProjectDagView>(this);
                pdvParent.LoadVakmanDagView(false);

            }
        }

        private void btnCancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (canCancel)
            {
                dbRepository dbrep = new dbRepository();
                dbrep.Deletemandag(mandag);


                // refresh alle andere programma's
                MenuControl owner = Tools.FindVisualParent<MenuControl>(this);
//#if DEBUG
//#else

                DateTime dt1 = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
                DateTime dt2 = dt1.AddDays(1);

                List<int> vakmanIds = new List<int>();
                vakmanIds.Add(vakman.VakmanId);

                owner.PageChannelMessage("projectdagview", Tools.CreateChannelMessage(vakmanIds, dt1, dt2));
                
                //#endif

                //RaiseEvent(new RoutedEventArgs(VakmanDagViewRefreshEvent, this));
                ProjectDagView pdvParent = Tools.FindVisualParent<ProjectDagView>(this);
                pdvParent.LoadVakmanDagView(false);

            }
        }

        private void btnReject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (canApprove)
            {
                dbRepository dbrep = new dbRepository();
                dbrep.Rejectmandag(mandag);

                // refresh alle andere programma's
                MenuControl owner = Tools.FindVisualParent<MenuControl>(this);
//#if DEBUG
//#else

                DateTime dt1 = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
                DateTime dt2 = dt1.AddDays(1);
                List<int> vakmanIds = new List<int>();
                vakmanIds.Add(vakman.VakmanId);

                owner.PageChannelMessage("projectdagview", Tools.CreateChannelMessage(vakmanIds, dt1, dt2));
                //#endif

                //RaiseEvent(new RoutedEventArgs(VakmanDagViewRefreshEvent, this));
                ProjectDagView pdvParent = Tools.FindVisualParent<ProjectDagView>(this);
                pdvParent.LoadVakmanDagView(false);

            }
        }

        private void btnHasSeen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (canConfirmHasSeen)
            {
                dbRepository dbrep = new dbRepository();
                dbrep.Deletemandag(mandag);


                // refresh alle andere programma's
                MenuControl owner = Tools.FindVisualParent<MenuControl>(this);
//#if DEBUG
//#else

                DateTime dt1 = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay);
                DateTime dt2 = dt1.AddDays(1);
                List<int> vakmanIds = new List<int>();
                vakmanIds.Add(vakman.VakmanId);

                owner.PageChannelMessage("projectdagview", Tools.CreateChannelMessage(vakmanIds, dt1, dt2));
                //#endif

                //RaiseEvent(new RoutedEventArgs(VakmanDagViewRefreshEvent, this));
                ProjectDagView pdvParent = Tools.FindVisualParent<ProjectDagView>(this);
                pdvParent.LoadVakmanDagView(false);

            }
        }

        private void btn15_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsExitingRange(this, 1))
            {
                SetAllColors(this);
            }

        }
        private void btn30_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsExitingRange(this, 2))
            {
                SetAllColors(this);
            }

        }
        private void btn45_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsExitingRange(this, 3))
            {
                SetAllColors(this);
            }

        }
        private void btn60_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsExitingRange(this, 4))
            {
                SetAllColors(this);
            }

        }


    }
}


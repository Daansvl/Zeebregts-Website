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
using MandagenRegistratieDomain;
using System.Runtime.InteropServices;
using System.ComponentModel;
using ZeebregtsLogic;
using MandagenRegistratie.tools;

namespace MandagenRegistratie.controls.Vakmannen.Weekview
{
    /// <summary>
    /// Interaction logic for VakmanDagViewProjectUur.xaml
    /// </summary>
    public partial class VakmanWeekViewProjectDag : UserControl
    {
        public VakmanWeekViewProjectDag()
        {
            InitializeComponent();
        }

        public void Load()
        {
            if (IsSelected)
            {
                bUren.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC"));
            }
            else
            {
                bUren.Background = new SolidColorBrush(Colors.White);
            }


            if (IsDotted)
            {

                double[] dimensions = new double[] { 2, 2 };
                Rectangle dottedRectangle = new Rectangle();
                dottedRectangle.Stroke = IsOwner ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Gray);
                dottedRectangle.SnapsToDevicePixels = true;
                dottedRectangle.Height = 2;
                dottedRectangle.StrokeThickness = 2;
                dottedRectangle.Height = 24;
                dottedRectangle.Width = 45;
                dottedRectangle.StrokeDashArray = new DoubleCollection(dimensions);

                VisualBrush vb = new VisualBrush();
                vb.Visual = dottedRectangle;

                bUrenBorder.BorderBrush = vb;
            }
            else
            {
                if (IsOwner && !Global.useWeekviewLeesstand)
                {
                    bUrenBorder.BorderBrush = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    bUrenBorder.BorderBrush = new SolidColorBrush(Colors.Gray);
                }
            }

            txtUren.Text = tools.Functies.CalculateUrenExact(listMandagen.Where(m => m.Status).ToList());

            // TODO: tijdelijk uitgezet, weer aanzetten
            //DbTools dbtools = new DbTools();
            //StackPanel tooltip = dbtools.AddTooltipAanvraagAll(vakman, project, datum, true, false);
            //ToolTipService.SetShowDuration(txtUren, 20000);
            //txtUren.ToolTip = tooltip;

        }


        public bool IsSelected;
        public bool IsOwner;
        public bool IsDotted;

        public bool IsDeSelected;

        public Project project;
        public Vakman vakman;
        public List<Mandagen> listMandagen;

        public DateTime datum;

        public int Uur;

        public void SetColors()
        {
            // deze functie gebeurt bij laden pagina
            //SetButtonColor(btn15, isIngeplandQ1, isAangevraagdQ1, IsSelectedQ1, isOwner, isEnabledQ1, isErgensAangevraagdQ1, IsDeSelectedQ1, isAangevraagdQ1AndCancelled, false, IsErgensDoorMijAangevraagdQ1AndCancelled);
            //SetButtonColor(btn30, isIngeplandQ2, isAangevraagdQ2, IsSelectedQ2, isOwner, isEnabledQ2, isErgensAangevraagdQ2, IsDeSelectedQ2, isAangevraagdQ2AndCancelled, false, IsErgensDoorMijAangevraagdQ2AndCancelled);
            //SetButtonColor(btn45, IsIngeplandQ3, isAangevraagdQ3, IsSelectedQ3, isOwner, isEnabledQ3, isErgensAangevraagdQ3, IsDeSelectedQ3, isAangevraagdQ3AndCancelled, false, IsErgensDoorMijAangevraagdQ3AndCancelled);
            //SetButtonColor(btn60, IsIngeplandQ4, isAangevraagdQ4, IsSelectedQ4, isOwner, isEnabledQ4, isErgensAangevraagdQ4, IsDeSelectedQ4, isAangevraagdQ4AndCancelled, false, IsErgensDoorMijAangevraagdQ4AndCancelled);
        }

        //public void SetColors(VakmanWeekViewProjectDag vpu, int quarter)
        //{
        //    // deze functie gebeurt bij mouseover
        //    switch (quarter)
        //    {
        //        case 1:
        //            if (vpu.IsEnabledQ1)
        //            {
        //                SetButtonColor(vpu.btn15, vpu.IsIngeplandQ1, vpu.IsAangevraagdQ1, vpu.IsSelectedQ1, vpu.IsOwner, vpu.IsEnabledQ1, vpu.isErgensAangevraagdQ1, vpu.IsDeSelectedQ1, vpu.IsAangevraagdQ1AndCancelled, true, vpu.IsErgensDoorMijAangevraagdQ1AndCancelled);
        //            }
        //            break;
        //        case 2:
        //            if (vpu.IsEnabledQ2)
        //            {
        //                SetButtonColor(vpu.btn30, vpu.IsIngeplandQ2, vpu.IsAangevraagdQ2, vpu.IsSelectedQ2, vpu.IsOwner, vpu.IsEnabledQ2, vpu.isErgensAangevraagdQ2, vpu.IsDeSelectedQ2, vpu.IsAangevraagdQ2AndCancelled, true, vpu.IsErgensDoorMijAangevraagdQ2AndCancelled);
        //            }
        //            break;
        //        case 3:
        //            if (vpu.IsEnabledQ3)
        //            {
        //                SetButtonColor(vpu.btn45, vpu.IsIngeplandQ3, vpu.IsAangevraagdQ3, vpu.IsSelectedQ3, vpu.IsOwner, vpu.IsEnabledQ3, vpu.isErgensAangevraagdQ3, vpu.IsDeSelectedQ3, vpu.IsAangevraagdQ3AndCancelled, true, vpu.IsErgensDoorMijAangevraagdQ3AndCancelled);
        //            }
        //            break;
        //        case 4:
        //            if (vpu.IsEnabledQ4)
        //            {
        //                SetButtonColor(vpu.btn60, vpu.IsIngeplandQ4, vpu.IsAangevraagdQ4, vpu.IsSelectedQ4, vpu.IsOwner, vpu.IsEnabledQ4, vpu.isErgensAangevraagdQ4, vpu.IsDeSelectedQ4, vpu.IsAangevraagdQ4AndCancelled, true, vpu.IsErgensDoorMijAangevraagdQ4AndCancelled);
        //            }
        //            break;
        //    }
        //}


        //private void SetButtonColor(Border button, bool isIngepland, bool isAangevraagd, bool isSelected, bool isowner, bool isEnabled, bool isErgensAangevraagd, bool isDeselected, bool isAangevraagdAndCancelled, bool IsMouseOver, bool isErgensDoorMijAangevraagdEnGecancelled)
        //{
        //    blockStatus bs = (blockStatus)ApplicationState.GetValue<int>("blockStatus");


        //    if (canApprove && bs == blockStatus.New)
        //    {
        //        btnApprove.Visibility = System.Windows.Visibility.Visible;
        //        btnReject.Visibility = System.Windows.Visibility.Visible;
        //    }
        //    else
        //    {
        //        btnApprove.Visibility = System.Windows.Visibility.Collapsed;
        //        btnReject.Visibility = System.Windows.Visibility.Collapsed;
        //    }

        //    if (canCancel && bs == blockStatus.New)
        //    {
        //        btnCancel.Visibility = System.Windows.Visibility.Visible;
        //    }
        //    else if (canApprove)
        //    {
        //        btnCancel.Visibility = System.Windows.Visibility.Collapsed;

        //    }
        //    else
        //    {
        //        btnCancel.Visibility = System.Windows.Visibility.Hidden;
        //    }

        //    if (canConfirmHasSeen && bs == blockStatus.New)
        //    {
        //        btnHasSeen.Visibility = System.Windows.Visibility.Visible;
        //    }
        //    else if (canApprove)
        //    {
        //        btnHasSeen.Visibility = System.Windows.Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        btnHasSeen.Visibility = System.Windows.Visibility.Hidden;
        //    }

        //    // eigenaar van t project
        //    if (isowner)
        //    {
        //        // 
        //        if (((isIngepland && isErgensAangevraagd) || (isIngepland && !isSelected)) && !isDeselected)
        //        // TODO: TEST: if (((isIngepland && isErgensAangevraagd) || (isIngepland && !isSelected)))
        //        {
        //            // oranje vlak
        //            button.BorderThickness = new System.Windows.Thickness(1);
        //            if (isErgensAangevraagd)
        //            {
        //                button.Background = new SolidColorBrush(Colors.Yellow);
        //            }
        //            else
        //            {
        //                button.Background = new SolidColorBrush(Colors.Green);
        //            }

                    
        //            button.BorderBrush = new SolidColorBrush(Colors.Black); // Green weggehaald // Black weggehaald

        //            ((TextBlock)button.Child).Visibility = System.Windows.Visibility.Visible;
        //        }
        //        else if ((isAangevraagd || isSelected) && !isDeselected)
        //        {
        //            // groen vlak
        //            if (IsMouseOver)
        //            {
        //                if (isIngepland)
        //                {
        //                    button.BorderBrush = new SolidColorBrush(Colors.Black);
        //                    button.Background = new SolidColorBrush(Colors.Green);
        //                }
        //                else
        //                {
        //                    button.BorderBrush = new SolidColorBrush(Colors.Black);
        //                    button.Background = new SolidColorBrush(Colors.Gray);
        //                }
        //            }
        //            else
        //            {
        //                if (isAangevraagd)
        //                {
        //                    //button.BorderBrush = new SolidColorBrush(Colors.Orange);
        //                    button.BorderThickness = new System.Windows.Thickness(1);
        //                    button.BorderBrush = new SolidColorBrush(Colors.Black);

        //                    button.Background = new SolidColorBrush(Colors.Orange);
        //                }
        //                else
        //                {
        //                    button.BorderThickness = new System.Windows.Thickness(1);
        //                    button.BorderBrush = new SolidColorBrush(Colors.Black);
        //                    button.Background = new SolidColorBrush(Colors.Green);
        //                }
        //            }

        //            button.BorderThickness = new System.Windows.Thickness(1);
        //            //button.BorderBrush = new SolidColorBrush(Colors.Gray);
        //            ((TextBlock)button.Child).Visibility = System.Windows.Visibility.Visible;
        //        }
        //        else if (isAangevraagdAndCancelled)
        //        {
        //            // blauw vlak
        //            button.Background = new SolidColorBrush(Colors.Red);
        //            button.BorderThickness = new System.Windows.Thickness(1);
        //            button.BorderBrush = new SolidColorBrush(Colors.Black);
        //            //button.BorderBrush = new SolidColorBrush(Colors.Gray);

        //            // TODO: TEST THIS
        //            ((TextBlock)button.Child).Visibility = System.Windows.Visibility.Visible;

        //        }
        //        else
        //        {
        //            // wit
        //            // oneven uren 'arceren'
        //            //if (!isDeselected || !isIngepland)
        //            //{
        //            if (Uur % 2 == 1)
        //            {
        //                button.BorderThickness = new System.Windows.Thickness(1);
        //                button.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC")); // new SolidColorBrush(Colors.LightSteelBlue); // LightSteelBlue
        //            }
        //            else
        //            {
        //                button.BorderThickness = new System.Windows.Thickness(1);
        //                button.Background = new SolidColorBrush(Colors.White);
        //            }
        //            //}

        //            // alleen randje eromheen als we hier iets kunnen wijzigen
        //            if (isEnabled)
        //            {
        //                button.BorderBrush = new SolidColorBrush(Colors.Black);
        //                //button.BorderBrush = new SolidColorBrush(Colors.Gray);
        //            }

        //            // TODO: voor versie B, deze else gebruiken
        //            // else
        //            else if (!isErgensAangevraagd && !isErgensDoorMijAangevraagdEnGecancelled) // TODO: versie A
        //            {
        //                button.BorderBrush = new SolidColorBrush(Colors.Gray);
        //            }
        //        }
        //    }
        //    // geen eigenaar van t project
        //    else
        //    {
        //        // 
        //        if (((isIngepland && isErgensAangevraagd) || (isIngepland && !isSelected)) && !isDeselected)
        //        {
        //            // oranje vlak
        //            button.BorderThickness = new System.Windows.Thickness(1);
        //            button.BorderBrush = new SolidColorBrush(Colors.Gray); // Gray
        //            //button.BorderBrush = new SolidColorBrush(Colors.Gray);
        //            if (isErgensAangevraagd)
        //            {
        //                button.Background = new SolidColorBrush(Colors.Yellow);
        //            }
        //            else
        //            {
        //                button.Background = new SolidColorBrush(Colors.Green);
        //            }
        //            //button.Background = new SolidColorBrush(Colors.Green);

        //            ((TextBlock)button.Child).Visibility = System.Windows.Visibility.Visible;
        //        }
        //        else if ((isAangevraagd || isSelected) && !isDeselected)
        //        {
        //            // rood vlak
        //            button.BorderThickness = new System.Windows.Thickness(1);
        //            button.BorderBrush = new SolidColorBrush(Colors.Gray);
        //            //button.BorderBrush = new SolidColorBrush(Colors.Gray);

        //            if (isAangevraagd)
        //            {
        //                //button.BorderBrush = new SolidColorBrush(Colors.Orange);
        //                button.BorderThickness = new System.Windows.Thickness(1);
        //                button.BorderBrush = new SolidColorBrush(Colors.Gray);

        //                button.Background = new SolidColorBrush(Colors.Orange);
        //            }
        //            else
        //            {
        //                button.Background = new SolidColorBrush(Colors.Green);
        //            }

        //            ((TextBlock)button.Child).Visibility = System.Windows.Visibility.Visible;
        //        }
        //        // hoeft niks blauws te zien als die geen eigenaar is van t project
        //        else if (isAangevraagdAndCancelled)
        //        {
        //            // rood vlak grijze rand
        //            button.Background = new SolidColorBrush(Colors.Red);
        //            button.BorderThickness = new System.Windows.Thickness(1);
        //            button.BorderBrush = new SolidColorBrush(Colors.Gray);
        //            ((TextBlock)button.Child).Visibility = System.Windows.Visibility.Visible;
        //        }
        //        else
        //        {
        //            // wit
        //            // oneven uren 'arceren'
        //            if (Uur % 2 == 1)
        //            {
        //                button.BorderThickness = new System.Windows.Thickness(1);
        //                button.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD8E4BC")); // new SolidColorBrush(Colors.LightSteelBlue); // LightSteelBlue
        //            }
        //            else
        //            {
        //                button.BorderThickness = new System.Windows.Thickness(1);
        //                button.Background = new SolidColorBrush(Colors.White);
        //            }

        //            // alleen randje eromheen als we hier iets kunnen wijzigen
        //            //if (!isErgensAangevraagd)
        //            //{
        //            button.BorderBrush = new SolidColorBrush(Colors.Gray);
        //            //}
        //        }
        //    }
        //}






    }
}


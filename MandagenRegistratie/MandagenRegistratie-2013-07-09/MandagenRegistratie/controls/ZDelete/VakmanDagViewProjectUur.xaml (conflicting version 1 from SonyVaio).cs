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

namespace MandagenRegistratie.controls
{
    /// <summary>
    /// Interaction logic for VakmanDagViewProjectUur.xaml
    /// </summary>
    public partial class VakmanDagViewProjectUur : UserControl
    {
        public VakmanDagViewProjectUur()
        {
            InitializeComponent();
        }

        public Project project;
        public Vakman vakman;

        public bool Q1Selected;
        public bool Q2Selected;
        public bool Q3Selected;
        public bool Q4Selected;

        public bool blnIsEnabled;
        public bool blnIsOwner;

        public int Uur;

        public bool btnIsPressed = false;
        public int intPosY;

        private void MoveCursor()
        {
            // Set the Current cursor, move the cursor's Position,
            // and set its clipping rectangle to the form. 

            Point absoluteScreenPos = PointToScreen(Mouse.GetPosition(this));
            SetPosition(Convert.ToInt32(absoluteScreenPos.X), intPosY);



            ((VakmanDagView)((StackPanel)((StackPanel)((ScrollViewer)((WrapPanel)this.Parent).Parent).Parent).Parent).Parent).lblCorcor.Content = Mouse.GetPosition((WrapPanel)this.Parent).X.ToString();

            if (Mouse.GetPosition((WrapPanel)this.Parent).X > ((ScrollViewer)((WrapPanel)this.Parent).Parent).HorizontalOffset + 880)
            {
                SetPosition(Convert.ToInt32(absoluteScreenPos.X - 10), intPosY);

                ((ScrollViewer)((WrapPanel)this.Parent).Parent).ScrollToHorizontalOffset(((ScrollViewer)((WrapPanel)this.Parent).Parent).HorizontalOffset + 20);
            }
            else if (Mouse.GetPosition((WrapPanel)this.Parent).X < ((ScrollViewer)((WrapPanel)this.Parent).Parent).HorizontalOffset + 4)
            {
                SetPosition(Convert.ToInt32(absoluteScreenPos.X + 10), intPosY);

                ((ScrollViewer)((WrapPanel)this.Parent).Parent).ScrollToHorizontalOffset(((ScrollViewer)((WrapPanel)this.Parent).Parent).HorizontalOffset - 20);

            }

            //this.Cursor = new Cursor(Cursor.Current.Handle);
            //Cursor.Position = new Point(Cursor.Position.X - 50, Cursor.Position.Y - 50);
            //Cursor.Clip = new Rectangle(this.Location, this.Size);
        }

        private void SetPosition(int a, int b)
        {
            SetCursorPos(a, b);
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        private void btn15_MouseMove(object sender, MouseEventArgs e)
        {
            if (blnIsEnabled)
            {
                Mouse.SetCursor(Cursors.SizeWE);



                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (!btnIsPressed)
                    {
                        Point absoluteScreenPos = PointToScreen(Mouse.GetPosition(this));
                        intPosY = Convert.ToInt32(absoluteScreenPos.Y - Mouse.GetPosition(this).Y + 35);
                        btnIsPressed = true;
                    }

                    MoveCursor();
                    ToggleOthers(1, 0);
                    if (blnIsOwner)
                    {
                        btn15.Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        btn15.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    Q1Selected = true;
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    if (!btnIsPressed)
                    {
                        Point absoluteScreenPos = PointToScreen(Mouse.GetPosition(this));
                        intPosY = Convert.ToInt32(absoluteScreenPos.Y - Mouse.GetPosition(this).Y + 35);
                        btnIsPressed = true;
                    }
                    MoveCursor();
                    btn15.Background = new SolidColorBrush(Colors.White);
                    Q1Selected = false;
                }
                else if (e.LeftButton == MouseButtonState.Released)
                {
                    btnIsPressed = false;
                }
                else if (e.RightButton == MouseButtonState.Released)
                {
                    btnIsPressed = false;
                }
            }

        }

        private void btn30_MouseMove(object sender, MouseEventArgs e)
        {
            if (blnIsEnabled)
            {
                Mouse.SetCursor(Cursors.SizeWE);

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (!btnIsPressed)
                    {
                        Point absoluteScreenPos = PointToScreen(Mouse.GetPosition(this));
                        intPosY = Convert.ToInt32(absoluteScreenPos.Y - Mouse.GetPosition(this).Y + 35);
                        btnIsPressed = true;
                    }
                    MoveCursor();
                    ToggleOthers(2, 0);
                    if (blnIsOwner)
                    {
                        btn30.Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        btn30.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    Q2Selected = true;
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    if (!btnIsPressed)
                    {
                        Point absoluteScreenPos = PointToScreen(Mouse.GetPosition(this));
                        intPosY = Convert.ToInt32(absoluteScreenPos.Y - Mouse.GetPosition(this).Y + 35);
                        btnIsPressed = true;
                    }
                    MoveCursor();
                    btn30.Background = new SolidColorBrush(Colors.White);
                    Q2Selected = false;
                }
                else if (e.LeftButton == MouseButtonState.Released)
                {
                    btnIsPressed = false;
                }
                else if (e.RightButton == MouseButtonState.Released)
                {
                    btnIsPressed = false;
                }
            }
        }

        private void btn45_MouseMove(object sender, MouseEventArgs e)
        {
            if (blnIsEnabled)
            {
                Mouse.SetCursor(Cursors.SizeWE);

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (!btnIsPressed)
                    {
                        Point absoluteScreenPos = PointToScreen(Mouse.GetPosition(this));
                        intPosY = Convert.ToInt32(absoluteScreenPos.Y - Mouse.GetPosition(this).Y + 35);
                        btnIsPressed = true;
                    }
                    MoveCursor();
                    ToggleOthers(3, 0);
                    if (blnIsOwner)
                    {
                        btn45.Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        btn45.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    Q3Selected = true;
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    if (!btnIsPressed)
                    {
                        Point absoluteScreenPos = PointToScreen(Mouse.GetPosition(this));
                        intPosY = Convert.ToInt32(absoluteScreenPos.Y - Mouse.GetPosition(this).Y + 35);
                        btnIsPressed = true;
                    }
                    MoveCursor();
                    btn45.Background = new SolidColorBrush(Colors.White);
                    Q3Selected = false;
                }
                else if (e.LeftButton == MouseButtonState.Released)
                {
                    btnIsPressed = false;
                }
                else if (e.RightButton == MouseButtonState.Released)
                {
                    btnIsPressed = false;
                }
            }
        }

        private void btn60_MouseMove(object sender, MouseEventArgs e)
        {
            if (blnIsEnabled)
            {
                Mouse.SetCursor(Cursors.SizeWE);

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (!btnIsPressed)
                    {
                        Point absoluteScreenPos = PointToScreen(Mouse.GetPosition(this));
                        intPosY = Convert.ToInt32(absoluteScreenPos.Y - Mouse.GetPosition(this).Y + 35);
                        btnIsPressed = true;
                    }
                    MoveCursor();
                    ToggleOthers(4, 0);
                    if (blnIsOwner)
                    {
                        btn60.Background = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        btn60.Background = new SolidColorBrush(Colors.OrangeRed);
                    }
                    Q4Selected = true;
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    if (!btnIsPressed)
                    {
                        Point absoluteScreenPos = PointToScreen(Mouse.GetPosition(this));
                        intPosY = Convert.ToInt32(absoluteScreenPos.Y - Mouse.GetPosition(this).Y + 35);
                        btnIsPressed = true;
                    }
                    MoveCursor();
                    btn60.Background = new SolidColorBrush(Colors.White);
                    Q4Selected = false;
                }
                else if (e.LeftButton == MouseButtonState.Released)
                {
                    btnIsPressed = false;
                }
                else if (e.RightButton == MouseButtonState.Released)
                {
                    btnIsPressed = false;
                }
            }
        }

        public void ToggleOthers(int Q, int state)
        {
            VakmanDagViewProjectUur vpSender = (VakmanDagViewProjectUur)this;

            foreach (VakmanDagViewProjectUur vp in ((WrapPanel)this.Parent).Children)
            {
                if (vp.Uur == vpSender.Uur && vp != vpSender)
                {
                    ToggleButtons(vp, Q, state);
                }
            }
        }

        public void ToggleButtons(VakmanDagViewProjectUur vp, int Q, int state)
        {
            switch (Q){
                case 1:
                    if (state == 0)
                    {
                        vp.Q1Selected = false;
                        vp.btn15.Background = new SolidColorBrush(Colors.White);
                    }
                    break;
                case 2:
                    if (state == 0)
                    {
                        vp.Q2Selected = false;
                        vp.btn30.Background = new SolidColorBrush(Colors.White);
                    }
                    break;
                case 3:
                    if (state == 0)
                    {
                        vp.Q3Selected = false;
                        vp.btn45.Background = new SolidColorBrush(Colors.White);
                    }
                    break;
                case 4:
                    if (state == 0)
                    {
                        vp.Q4Selected = false;
                        vp.btn60.Background = new SolidColorBrush(Colors.White);
                    }
                    break;

            }
        }

        //private void btn15_MouseLeftButtonDown(object sender, EventArgs e)
        //{
        //    btn15.Background = new SolidColorBrush(Colors.OrangeRed);
        //    Q1Selected = true;
        //}

        //private void btn30_MouseLeftButtonDown(object sender, EventArgs e)
        //{
        //    btn30.Background = new SolidColorBrush(Colors.OrangeRed);
        //    Q1Selected = true;
        //}

        //private void btn45_MouseLeftButtonDown(object sender, EventArgs e)
        //{
        //    btn45.Background = new SolidColorBrush(Colors.OrangeRed);
        //    Q1Selected = true;
        //}

        //private void btn60_MouseLeftButtonDown(object sender, EventArgs e)
        //{
        //    btn60.Background = new SolidColorBrush(Colors.OrangeRed);
        //    Q1Selected = true;
        //}

        //void border_MouseMove(object sender, MouseEventArgs e)
        //{

        //    Mouse.SetCursor(Cursors.ScrollWE);

        //    var cBegin = Colors.Green;
        //    var cEnd = Colors.Yellow;
        //    GradientStop gs1, gs2;
        //    var brush = new LinearGradientBrush();
        //    var gs0 = new GradientStop(cBegin, 0);
        //    var gs3 = new GradientStop(cEnd, 1.0);

        //    Point p = e.GetPosition((Border)sender);
        //    var factor = TrackHorizontally ? p.Y / ((Border)sender).Height : p.X / ((Border)sender).Width;

        //    if (TrackHorizontally)
        //    {
        //        brush.StartPoint = new Point(0.5, 0);
        //        brush.EndPoint = new Point(0.5, 1);

        //        gs1 = new GradientStop(cBegin, factor);
        //        gs2 = new GradientStop(cEnd, factor);
        //    }
        //    else
        //    {
        //        brush.StartPoint = new Point(0, 0.5);
        //        brush.EndPoint = new Point(1, 0.5);

        //        gs1 = new GradientStop(cBegin, factor);
        //        gs2 = new GradientStop(cEnd, factor);
        //    }

        //    brush.GradientStops = new GradientStopCollection { gs0, gs1, gs2, gs3 };

        //    ((Border)sender).Background = brush;
        //}
    }



}


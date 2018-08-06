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
using ZeebregtsLogic;

namespace MandagenRegistratie.controls.Periodes
{
    /// <summary>
    /// Interaction logic for ProjectleidersOverzicht.xaml
    /// </summary>
    public partial class Intro : MenuControl
    {

        public Intro()
        {
            InitializeComponent();
            #region Pagina specifieke informatie
            PageTitle = "Instellingen";
            PageSubtitle = "Gebruikers";

            if (Rechten.IsAdmin || ApplicationState.GetValue<bool>(ApplicationVariables.blnControlMDR))
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                PageGereedButtonVisibility = System.Windows.Visibility.Hidden;
            }


            PageBackButtonText = "TERUG";
            PageOKButtonText = "WIJZIGEN";
            #endregion

            this.Reloaded += Load;
            this.OkClick += Wijzigen;

            Load();
        }


        /// 
        /// </summary>
        public void Load()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // intialize
                dbRepository dbrep = new dbRepository();
                dbOriginalRepository dbrepOriginal = new dbOriginalRepository();

                //listGebruikers = dbrep.datacontext.Gebruikers.ToList();
                //listProjectleiders = dbrep.datacontext.Projectleiders.ToList();


            }
            catch { }
            finally { Mouse.OverrideCursor = null; }

        }


        public void Wijzigen()
        {
            Edit edit = new Edit();
            edit.Load();
            PageGoToPage(edit);
        }



    }
}

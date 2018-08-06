using eBrochure_zeebregts.Classes;
using eBrochure_zeebregts.KeuzeControls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.ServiceModel.DomainServices.Client;
namespace eBrochure_zeebregts.ExpertControls
{
    public partial class AfspraakPopup:UserControl
    {
        public AfspraakPopup(AfspraakBlok lb, DetailsWindow parWin, AfspraakMaker parent)
        {
            InitializeComponent();
            linkedBlok = lb;
            DwParent = parWin;
            MakerParent = parent;
        }
        private AfspraakBlok linkedBlok;
        private DetailsWindow DwParent;
        private AfspraakMaker MakerParent;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var obj = (from b in Acumulator.Instance().ctx.Blokkens
                       where b.BlokID == linkedBlok.BlokID
                       select b).FirstOrDefault();
            if(obj != null)
            {
                if (linkedBlok.Status == AfspraakStatus.Beschikbaar)
                {
                    obj.Status = true;
                    obj.EigenaarGebruikerID = Acumulator.Instance().HuidigGebruiker.ID;
                    obj.Eigenaar = Acumulator.Instance().HuidigGebruiker.GebruikersNaam;
                  //  linkedBlok.Status = AfspraakStatus.Gekozen;
                }
                else if(linkedBlok.Status == AfspraakStatus.Gekozen)
                {
                  //  obj.Status = false;
                   // obj.Eigenaar = null;
                   // obj.EigenaarGebruikerID = null;
                    //linkedBlok.Status = AfspraakStatus.Beschikbaar;
                }
               

                var invokeop = Acumulator.Instance().ctx.ReserveerBlok(obj,ProbeerReserveerComplete,null);
                       
            }

           
        }
        private void ProbeerReserveerComplete(InvokeOperation<bool> resOperation)
        {

            var btnText = InvokeBtn.Content.ToString().ToLower();
            var msgText = "";
            if(btnText == "reserveer")
            {
                msgText = "Het reserveren is ";
            }
            else if(btnText == "annuleer")
            {
                msgText = "Het annuleren is ";
            }

            if(resOperation.HasError )//!(!resOperation.Value && btnText == "annuleer") )
            {
                //reserveer failed
                msgText += "mislukt. Dit blok is waarschijnlijk al bezet.";
                Acumulator.Instance().ctx.RejectChanges();
            }
            else if( !resOperation.Value && btnText == "reserveer")
            {
                //reserveer failed
                msgText += "mislukt. Dit blok is waarschijnlijk al bezet.";
                Acumulator.Instance().ctx.RejectChanges();
            }
            else
            {

                msgText += "gelukt!";
            }
            MessageBox.Show(msgText);
            if (DwParent != null)
            {
                DwParent.Close();
                MakerParent.DataContext = null;
                MakerParent.ReloadData();
            }
        }


    }
}

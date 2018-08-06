using eBrochure_zeebregts.ExpertControls.Models;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using eBrochure_zeebregts.Classes;
using System.Collections.Generic;

namespace eBrochure_zeebregts.ExpertControls
{

    public partial class AdvancedTegelSubCatPage : UserControl, INotifyPropertyChanged, IBaseControl
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private bool leesStand;
        
        public bool LeesStand
        {
            get { return leesStand; }
            set { leesStand = value; OnPropertyChanged("LeesStand"); }
        }

        private AdvancedTegelSubCatInfo internData;
        
        public AdvancedTegelSubCatInfo InternData
        {
            get { return internData; }
            set { internData = value; OnPropertyChanged("InternData"); }
        }

        public double RestantBasisMeters
        {
            get { return InternData.Regels.FirstOrDefault(r => r.Status == StatusType.Basis).VervangInfo.GekozenMeters; }
        }

        public AdvancedTegelSubCatPage()
        {
            InitializeComponent();
          
        }

        public void LoadContext(AdvancedTegelSubCatInfo Data)
        {
            InternData = Data;
            DataContext = this;

            var ruimte = Acumulator.Instance().BB.HuidigRuimte;
            if (ruimte.AanvullendeMeters.Where(a => a.LinkedSubCat == InternData.SubCatNR) != null && ruimte.AanvullendeMeters.Where(a => a.LinkedSubCat == InternData.SubCatNR).ToList().Count > 0)
            {
                Acumulator.Instance().BB.ShowWijzigBtn(true);
                LeesStand = true;

                foreach (var exM in ruimte.AanvullendeMeters)
                {
                    if (exM.LinkedSubCat != InternData.SubCatNR)
                    { continue; }

                    if (exM.RegelIndex == 0)
                    {
                        var basisrgl = InternData.Regels.FirstOrDefault(r => r.Status == StatusType.Basis);
                        if (basisrgl == null)
                        { continue; }
                        //basisrgl.Opmerking = exM.Opmerking;
                        basisrgl.VervangInfo.Opmerking = exM.Opmerking;
                        var bsTgl = basisrgl.VervangInfo.Tegels.FirstOrDefault(t =>t.ProdSoort == ProductSoort.Tegel && t.ProductId == exM.LinkedProduct.ID);

                        if (bsTgl != null)
                        {
                            bsTgl.IsGekozen = true;
                            basisrgl.GekozenTegel = basisrgl.VervangInfo.Tegels.FirstOrDefault(t => t.ProductId == exM.LinkedProduct.ID);
                            continue;
                        }
                        bsTgl = basisrgl.VervangInfo.Tegels.FirstOrDefault(h => h.ProdSoort == ProductSoort.HoekProfiel && h.ProductId == exM.LinkedHoekProf.ProfielID);
                        if (bsTgl != null)
                        {
                            bsTgl.IsGekozen = true;
                            basisrgl.GekozenTegel = basisrgl.VervangInfo.Tegels.FirstOrDefault(t => t.ProductId == exM.LinkedHoekProf.ProfielID);
                            continue;
                        }
                        bsTgl = basisrgl.VervangInfo.Tegels.FirstOrDefault(d => d.ProdSoort == ProductSoort.Dorpel && d.ProductId == exM.LinkedProduct.ID);
                        if (bsTgl != null)
                        {
                            bsTgl.IsGekozen = true;
                            basisrgl.GekozenTegel = basisrgl.VervangInfo.Tegels.FirstOrDefault(t => t.ProductId == exM.LinkedProduct.ID);
                            continue;
                        }
                        continue;
                    }

                    AdvancedVervangRegel freshRgl;
                    freshRgl = InternData.Regels.FirstOrDefault(r => r.Status == StatusType.Leeg);
                    if (freshRgl == null)
                    {
                        freshRgl = AdvancedVervangRegel.GetNewTegelRegel(AdvancedTegelSubCatInfo.GetTegelColl(InternData.TegelInput, InternData.BasisTegel.InwisselPrijs,exM.ProdSoort), InternData.GetFirstFreeIndex(),InternData.Regels.FirstOrDefault().Eenheid);
                        InternData.Regels.Add(freshRgl);
                        
                    }
                    freshRgl.Status = StatusType.Toevoeging;
                    freshRgl.VervangInfo.Wijzigstand = false;
                    //freshRgl.Opmerking = exM.Opmerking;
                    freshRgl.VervangInfo.Opmerking = exM.Opmerking;
                    if (exM.LinkedProduct != null)
                    {
                        var tglItem = freshRgl.VervangInfo.Tegels.FirstOrDefault(t => t.ProductId == exM.LinkedProduct.ID);

                        if (tglItem != null)
                        {
                            tglItem.IsGekozen = true;
                            freshRgl.GekozenTegel = tglItem;
                        }
                    }
                    else if(exM.LinkedHoekProf != null)
                    {
                        var tglItem = freshRgl.VervangInfo.Tegels.FirstOrDefault(t => t.ProductId == exM.LinkedHoekProf.ProfielID);
                        if (tglItem != null)
                        {
                            tglItem.IsGekozen = true;
                            freshRgl.GekozenTegel = tglItem;
                        }
                    }
                    freshRgl.VervangInfo.GekozenMeters = exM.Meters;

                    var newEmptyrgl = AdvancedVervangRegel.GetNewTegelRegel(AdvancedTegelSubCatInfo.GetTegelColl(InternData.TegelInput, InternData.BasisTegel.InwisselPrijs,exM.ProdSoort), InternData.GetFirstFreeIndex(),InternData.Regels.FirstOrDefault().Eenheid);
                    InternData.Regels.Add(newEmptyrgl);
                    newEmptyrgl.Status = StatusType.Leeg;
                    newEmptyrgl.VervangInfo.Wijzigstand = false;
                }
            }
            RecalcBasisMeters();
            LeesStand = Acumulator.Instance().BB.IsCurrentComplete();
            Acumulator.Instance().BB.ShowWijzigBtn(LeesStand);

        }
        
        private void AddRemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject tvi= null;
            var button = sender as Button;
            //walk up the tree to find the ListboxItem
            var mic = sender as MetersInvoerControl;
            if (button != null)
            {
                tvi = findParentTreeItem(button, typeof(ListBoxItem));
                
            }
            else
            {
                tvi = findParentTreeItem(mic, typeof(ListBoxItem));
            }
            //if not null cast the Dependancy object to type of Listbox item.
            if (tvi != null)
            {
                ListBoxItem lbi = tvi as ListBoxItem;
                //select it.
                lbi.IsSelected = true;
            }

            var regel = AccentenListBox.SelectedItem as AdvancedVervangRegel;




            if (regel != null && regel.Status == StatusType.Leeg)
            {
                regel.Status = StatusType.Toevoeging;
                
                var nwRegel =AdvancedVervangRegel.GetNewTegelRegel(AdvancedTegelSubCatInfo.GetTegelColl(InternData.TegelInput,InternData.BasisTegel.InwisselPrijs,InternData.BasisTegel.ProdSoort),InternData.GetFirstFreeIndex(),InternData.Regels.FirstOrDefault().Eenheid);
                InternData.Regels.Add(nwRegel);
                regel.VervangInfo.Wijzigstand = true;
            }
            else if (regel != null && regel.Status == StatusType.Toevoeging)
            {
                InternData.Regels.Remove(regel);
                if(InternData.Regels.Count == 1)
                {
                    InternData.Regels.Add(AdvancedVervangRegel.GetNewTegelRegel(AdvancedTegelSubCatInfo.GetTegelColl(InternData.TegelInput, InternData.BasisTegel.InwisselPrijs, InternData.BasisTegel.ProdSoort), InternData.GetFirstFreeIndex(), InternData.Regels.FirstOrDefault().Eenheid));
                }
                RecalcBasisMeters();
            }
            OnPropertyChanged("CanAddDelete");
        }

        private DependencyObject findParentTreeItem(DependencyObject CurrentControl, Type ParentType)
        {
            bool notfound = true;
            while (notfound)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(CurrentControl);
                string ParentTypeName = ParentType.Name;
                //Compare current type name with what we want
                if (parent == null)
                {
                    System.Diagnostics.Debugger.Break();
                    notfound = false;
                    continue;
                }
                if (parent.GetType().Name == ParentTypeName)
                {
                    return parent;
                }
                //we haven't found it so walk up the tree.
                CurrentControl = parent;
            }
            return null;
        }

        private void RecalcBasisMeters()
        {
            var basisRegel = InternData.Regels.FirstOrDefault(r => r.Status == StatusType.Basis);
            if (basisRegel != null)
            {
                //if groter dan 0!!!

                var remainingBasisMeters = InternData.TotaalMeters;
                foreach (var regel in InternData.Regels.Where(r => r.Status != StatusType.Basis && r.VervangInfo.Wijzigstand == false))
                {
                    remainingBasisMeters -= regel.VervangInfo.GekozenMeters;
                }
                remainingBasisMeters = Math.Round(remainingBasisMeters, 2);
                if (remainingBasisMeters > 0)
                {
                    basisRegel.VervangInfo.GekozenMeters = remainingBasisMeters;
                }
            }
            
            OnPropertyChanged("RestantBasisMeters");
        }

        private bool MetersValid()
        {
            var correct = true;

            var basisRegel = InternData.Regels.FirstOrDefault(r => r.Status == StatusType.Basis);
            if (basisRegel != null)
            {
                var remainingBasisMeters = InternData.TotaalMeters;
                foreach (var regel in InternData.Regels.Where(r => r.Status != StatusType.Basis && r.VervangInfo.Wijzigstand == false))
                {
                    remainingBasisMeters -= regel.VervangInfo.GekozenMeters;
                }
                basisRegel.VervangInfo.GekozenMeters = remainingBasisMeters;
                if (basisRegel.VervangInfo.GekozenMeters <= 0)
                {
                    correct = false;
                }
            }
            OnPropertyChanged("RestantBasisMeters");


            return correct;
        }
        private void MetersInvoerControl_SubmitMeters(object sender, SubmitMetersEventArgs e)
        {
            RecalcBasisMeters();
        }

        private void MetersInvoerControl_SubmitWijzig(object sender, SubmitWijzigEventArgs e)
        {



            var mic = sender as MetersInvoerControl;
            //walk up the tree to find the ListboxItem
            DependencyObject tvi = findParentTreeItem(mic, typeof(ListBoxItem));
            //if not null cast the Dependancy object to type of Listbox item.
            if (tvi != null)
            {
                ListBoxItem lbi = tvi as ListBoxItem;
                //select it.
                lbi.IsSelected = true;
                lbi.UpdateLayout();
            }
           
            var regel = AccentenListBox.SelectedItem as AdvancedVervangRegel;

            Button arButton = null;
            foreach (var btn in FindVisualChildren<Button>(tvi))
            {
                if (btn.Name == "AddRemoveBtn")
                {
                    arButton = btn;
                }
            }
            

            if (regel.VervangInfo.Wijzigstand && regel.Status != StatusType.Leeg)
            {
                if (arButton != null)
                {
                    arButton.IsEnabled = false;
                }
            }
            else
            {
                if (arButton != null)
                {
                    arButton.IsEnabled = true;
                }
            }
        }

        public IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
        private bool SaveExtraMeterLine(AdvancedVervangRegel rgl, bool BasisReplaced)
        {
            var result = true;
            if (rgl.VervangInfo.Wijzigstand == true)
            {
                result = false;
            }
            if (result)
            {
                switch (rgl.GekozenTegel.ProdSoort)
                {
                    case ProductSoort.Dorpel:
                        result &= addDorpelLine(rgl, BasisReplaced);
                        break;
                    case ProductSoort.Tegel:
                        result &= addTegelLine(rgl, BasisReplaced);
                        break;
                    case ProductSoort.HoekProfiel:
                        result &= addHoekLine(rgl, BasisReplaced);
                        break;
                }
            }
            return result;
        }

        private bool addDorpelLine(AdvancedVervangRegel rgl,bool BasisReplaced)
        {
            var result = true;
            var ctx = Acumulator.Instance().ctx;
            var curRuimte = Acumulator.Instance().BB.HuidigRuimte;
            var lsc = rgl.Status == StatusType.Basis ? InternData.SubCatNR : "Sub4Accent" + Ruimte.GenerateScbNr4Accent(InternData.SubCatNR).SCB_ID;
           

            var dorpl = (from prdct in ctx.Productens
                         where prdct.PD_ID == rgl.GekozenTegel.ProductId
                         select new Product()
                         {
                             ID = prdct.PD_ID,
                             volgorde = (int)prdct.volgorde,
                             VerpakkingsToeslag = rgl.GekozenTegel.InstapPrijs,//(double)prdct.verpakkingstoeslag,
                             LinkedSubCat = lsc,
                             LinkedMainCat = InternData.HCNR,
                             Omschrijving = prdct.Omschrijving,
                             Kleur = (from kleur in ctx.Kleurens
                                      where kleur.K_ID == prdct.PKC_NR
                                      select kleur.Omschrijving).FirstOrDefault().ToString(),
                             kleurVolgorde = (from kleur in ctx.Kleurens
                                              where kleur.K_ID == prdct.PKC_NR
                                              select (int)kleur.volgorde).FirstOrDefault(),
                             KleurCode = prdct.PKC_NR,
                             Breedte = (int)prdct.breedte,
                             Lengte = (int)prdct.lengte,
                             ImgPath = prdct.ImagePath,
                             productcode = prdct.productcode
                         }).FirstOrDefault();

            if (dorpl != null)
            {
                if (rgl.Status != StatusType.Basis || BasisReplaced)
                {
                    curRuimte.GekozenAccenten.Add(dorpl);
                }

                var extMeters = new ExtraMeters
                {
                    LinkedProduct = dorpl,
                    LinkedSubCat = InternData.SubCatNR,
                    Meters = rgl.VervangInfo.GekozenMeters,
                    RegelIndex = rgl.RegelIndex,
                    NabewerkingSetNR = rgl.GekozenTegel.NSNR,
                    IsBasis = rgl.Status == StatusType.Basis ? !BasisReplaced : false,
                    Meerprijs = rgl.GekozenTegel.PrijsPerMeter,
                    Opmerking = rgl.VervangInfo.Opmerking,
                    ProdSoort = ProductSoort.Dorpel
                };

                curRuimte.AanvullendeMeters.Add(extMeters);
            }

            return result;
        }
        private bool addHoekLine(AdvancedVervangRegel rgl,bool BasisReplaced)
        {
            var result = true;
            var ctx = Acumulator.Instance().ctx;
            var curRuimte = Acumulator.Instance().BB.HuidigRuimte;
            var lsc = rgl.Status == StatusType.Basis ? InternData.SubCatNR : "Sub4Accent" + Ruimte.GenerateScbNr4Accent(InternData.SubCatNR).SCB_ID;

          
            var hoekprof = (from prf in ctx.Productens
                            where prf.PD_ID == rgl.GekozenTegel.ProductId
                            select new HoekProfiel()
                            {
                                ProfielID = prf.PD_ID,
                                volgorde = (int)prf.volgorde,
                                VerpakkingsToeslag = rgl.GekozenTegel.InstapPrijs,
                                LinkedSubCat = lsc,
                                Omschrijving = prf.Omschrijving,
                                Kleur = (from kleur in ctx.Kleurens
                                         where kleur.K_ID == prf.PKC_NR
                                         select kleur.Omschrijving).FirstOrDefault().ToString(),
                                KleurVolgorde = (from kleur in ctx.Kleurens
                                                 where kleur.K_ID == prf.PKC_NR
                                                 select (int)kleur.volgorde).FirstOrDefault(),
                                KleurCode = prf.PKC_NR,
                                Breedte = (int)prf.breedte,
                                Lengte = (int)prf.lengte,
                                ProfielImg = prf.ImagePath,
                                Meters = rgl.VervangInfo.GekozenMeters,
                                IsExpertMode = true,

                            }).FirstOrDefault();
           
            if (hoekprof != null)
            {
                if (rgl.Status != StatusType.Basis || BasisReplaced)
                {
                    curRuimte.GekozenProfielen.Add(hoekprof);
                }

                

                var extMeters = new ExtraMeters
                {
                    LinkedHoekProf = hoekprof,
                    LinkedSubCat = InternData.SubCatNR,
                    Meters = rgl.VervangInfo.GekozenMeters,
                    RegelIndex = rgl.RegelIndex,
                    NabewerkingSetNR = rgl.GekozenTegel.NSNR,
                    IsBasis = rgl.Status == StatusType.Basis ? !BasisReplaced : false,
                    Meerprijs = rgl.GekozenTegel.PrijsPerMeter,
                    Opmerking = rgl.VervangInfo.Opmerking,
                    ProdSoort = ProductSoort.HoekProfiel
                };
                curRuimte.AanvullendeMeters.Add(extMeters);
            }

            return result;
        }
        private bool addTegelLine(AdvancedVervangRegel rgl,bool BasisReplaced)
        {
            var result = true;
            var ctx = Acumulator.Instance().ctx;
            var curRuimte = Acumulator.Instance().BB.HuidigRuimte;
            var lsc = rgl.Status == StatusType.Basis ? InternData.SubCatNR : "Sub4Accent" + Ruimte.GenerateScbNr4Accent(InternData.SubCatNR).SCB_ID;

            var product = (from prdct in ctx.Productens
                           where prdct.PD_ID == rgl.GekozenTegel.ProductId
                           select new Product()
                           {
                               ID = prdct.PD_ID,
                               volgorde = (int)prdct.volgorde,
                               VerpakkingsToeslag = rgl.GekozenTegel.InstapPrijs,//(double)prdct.verpakkingstoeslag,
                               LinkedSubCat = lsc,
                               LinkedMainCat = InternData.HCNR,
                               Omschrijving = prdct.Omschrijving,
                               Kleur = (from kleur in ctx.Kleurens
                                        where kleur.K_ID == prdct.PKC_NR
                                        select kleur.Omschrijving).FirstOrDefault().ToString(),
                               kleurVolgorde = (from kleur in ctx.Kleurens
                                                where kleur.K_ID == prdct.PKC_NR
                                                select (int)kleur.volgorde).FirstOrDefault(),
                               KleurCode = prdct.PKC_NR,
                               Breedte = (int)prdct.breedte,
                               Lengte = (int)prdct.lengte,
                               ImgPath = prdct.ImagePath,
                               productcode = prdct.productcode
                           }).FirstOrDefault();

            if (product != null)
            {
                if (rgl.Status != StatusType.Basis || BasisReplaced)
                {
                    curRuimte.GekozenAccenten.Add(product);
                }

                var extMeters = new ExtraMeters
                {
                    LinkedProduct = product,
                    LinkedSubCat = InternData.SubCatNR,
                    Meters = rgl.VervangInfo.GekozenMeters,
                    RegelIndex = rgl.RegelIndex,
                    NabewerkingSetNR = rgl.GekozenTegel.NSNR,
                    IsBasis = rgl.Status == StatusType.Basis ? !BasisReplaced : false,
                    Meerprijs = rgl.GekozenTegel.PrijsPerMeter,
                    Opmerking = rgl.VervangInfo.Opmerking,
                    ProdSoort = ProductSoort.Tegel
                };

                curRuimte.AanvullendeMeters.Add(extMeters);
            }

            return result;
        }

        public bool SubmitPressed()
        {
            var result = true;
            if (MetersValid())
            {
                var ctx = Acumulator.Instance().ctx;
                
                var BasisReplaced = false;
                if (InternData.Regels.Count(x => x.Status != StatusType.Leeg) > 0)
                {
                    if (InternData.Regels[0].GekozenTegel.ProductId != InternData.BasisTegel.ProductId)
                    {
                        BasisReplaced = true;
                    }
                }
                //v3
                foreach (var rgl in InternData.Regels)
                {
                    if (rgl.Status != StatusType.Leeg)
                    {
                        //Extra meters moeten altijd worden opgeslagen zowel voor basis wijziging als voor gedeeltelijke aanpassing.
                        result &= SaveExtraMeterLine(rgl, BasisReplaced);
                        if (!result)
                        {
                            break;
                        }
                    }
                }
                ////v2
                //if (!BasisReplaced)
                //{
                //    //basis is hetzelfde gebleven. Alle regels die er zijn zijn toevoegingen of gedeeltelijke aanpassingen.
                //    foreach (var rgl in InternData.Regels)
                //    {
                //        if (rgl.Status == StatusType.Toevoeging)
                //        {
                //            result &= SaveExtraMeterLine(rgl, BasisReplaced);
                //        }
                //    }

                //}
                //else
                //{
                //    foreach (var rgl in InternData.Regels)
                //    {
                //        //Extra meters moeten altijd worden opgeslagen zowel voor basis wijziging als voor gedeeltelijke aanpassing.
                //        result &= SaveExtraMeterLine(rgl, BasisReplaced);
                //    }
                //}
                ////

                
            }
            else
            {
                MessageBox.Show("Er is een fout in het totaal aantal meters. Controleer en verbeter de meters om verder te gaan.");
                result = false;
            }
            return result;
        }

        public void Clear4Submit()
        {
            var curRuimte = Acumulator.Instance().BB.HuidigRuimte;
            curRuimte.GekozenAccenten.Clear();
            curRuimte.AanvullendeMeters.Clear();
            curRuimte.GekozenProfielen.RemoveAll(x => x.IsExpertMode == true);
        }

        public void WijzigPressed()
        {
            LeesStand = false;
        }

        private void MetersInvoerControl_SubmitDelete(object sender, SubmitDeleteEventArgs e)
        {
            AddRemoveBtn_Click(sender, new RoutedEventArgs());
        }
    }
    public class StatusVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((StatusType)value)
            {
                case StatusType.Basis:
                    return (string)parameter == "AddRemBtn" ? Visibility.Collapsed : Visibility.Visible;
                case StatusType.Toevoeging:
                 return Visibility.Visible;     
                  //  return (string)parameter == "AddRemBtn" ? Visibility.Collapsed : Visibility.Visible;
                case StatusType.Leeg:
                    return (string)parameter == "AddRemBtn" ? Visibility.Visible : Visibility.Collapsed;
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class StatusButtonConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch((StatusType)value)
            {
                case StatusType.Basis:
                    return null;
                case StatusType.Toevoeging:
                    var reluriT = new Uri("../Images/trashcan_empty_alt.png", UriKind.Relative);
                return new BitmapImage(reluriT);
                case StatusType.Leeg:
                 var reluriL = new Uri("../Images/add.png", UriKind.Relative);
                return new BitmapImage(reluriL);
                default:
                    return new BitmapImage();

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class WijzigDisableConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MetersVisConv : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Visible)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Visible;
            }
            //return (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

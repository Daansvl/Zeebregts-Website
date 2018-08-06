using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using eBrochure_zeebregts.Classes;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
namespace eBrochure_zeebregts.ExpertControls.Models
{
    public enum StatusType
    {
        Basis,
        Toevoeging,
        Leeg
    }

    public enum ProductSoort
    {
        Tegel,
        HoekProfiel,
        Dorpel

    }

    public class AdvancedTegelModel : INotifyPropertyChanged
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

    }
    public class AdvancedTegelSubCatInfo : AdvancedTegelModel
    {
        private double totaalMeters;
        public double TotaalMeters
        {
            get { return totaalMeters; }
            set { totaalMeters = value; OnPropertyChanged("TotaalMeters"); }
        }

        private string subCatTitle;
        public string SubCatTitle
        {
            get { return subCatTitle; }
            set { subCatTitle = value; OnPropertyChanged("SubCatTitle"); }
        }

        private string subcatnr;
        public string SubCatNR 
        {
            get { return subcatnr; }
            set
            {
                subcatnr = value;
                if(subcatnr.StartsWith("SCBP"))
                {
                    LogHelper.SendLog("A PROBLEM IS HERE advancedTegelModel new atsci: " + subcatnr, LogType.error);
                    //MessageBox.Show("OR A PROBLEM IS HERE");
                }
            }
        }
        public string HCNR { get; set; }
        private AdvancedTegelInfo basisTegel;
        public AdvancedTegelInfo BasisTegel
        {
            get { return basisTegel; }
            set { basisTegel = value; OnPropertyChanged("BasisTegel"); }
        }
        private ObservableCollection<AdvancedVervangRegel> regels;
        public ObservableCollection<AdvancedVervangRegel> Regels
        {
            get { return regels; }
            set { regels = value; OnPropertyChanged("Regels"); }
        }

        public IEnumerable<eBrochure_zeebregts.Web.VrijAccentProductInfo> TegelInput;

        public int GetFirstFreeIndex()
        {
            var result = 1;
           
            bool done = false;
            while (! done)
            {
                var miscnt = 0;
                foreach (var rgl in Regels)
                {
                    if (result == rgl.RegelIndex)
                    {
                        result++;
                    }
                    else
                    {
                        miscnt++;
                    }
                }
                if (miscnt == Regels.Count())
                {
                    done = true;
                }
            }
            return result;
        }

        public static ObservableCollection<AdvancedTegelInfo> GetTegelColl(IEnumerable<eBrochure_zeebregts.Web.VrijAccentProductInfo> input, double basisprijspermeter,ProductSoort soort)
        {
            var resultCol = new ObservableCollection<AdvancedTegelInfo>();
            if (input != null)
            {
                foreach (var vpi in input)
                {
                    var prijsholder = (from ip in Acumulator.Instance().ctx.InstapPrijzens
                                               where ip.PR_NR == Acumulator.Instance().Projectnr
                                               && ip.PD_NR == vpi.PD_NR
                                               select ip).FirstOrDefault();
                    double iPrijs = 0.0;
                    if (prijsholder != null)
                    {
                        double.TryParse(prijsholder.InstapPrijs.ToString(), out iPrijs);
                    }
                    var prPM = from or in vpi.AccentSetOpbouw
                               where or.VrijAccentRegel_NR == or.VrijAccentProductInfo.VAPP_ID
                               select or.Prijs;

                                                              
                    resultCol.Add(new AdvancedTegelInfo
                    {
                        ImgPath = System.String.IsNullOrEmpty(vpi.Producten.ImagePath) ? "" : vpi.Producten.ImagePath,
                        TegelOmschrijving = vpi.Producten.Omschrijving,
                        TegelDetails = vpi.Producten.productcode,
                        PrijsPerMeter = calcPrijsPerMeter((double)prPM.FirstOrDefault(), basisprijspermeter),
                       
                        InstapPrijs = iPrijs,
                        IsGekozen = false,
                        ProductId = vpi.Producten.PD_ID,
                        Lengte = vpi.Producten.lengte ?? 0,
                        Breedte = vpi.Producten.breedte ?? 0,
                        NSNR = vpi.NS_NR,
                        ProdSoort = soort
                    });
                }
            }

            return resultCol;
        }
        private static double calcPrijsPerMeter(double accentPrijs, double basisPrijs)
        {
            //=AFRONDEN.BOVEN(([PrijsAccent]-[PrijsBasis])/0,9/0,9*1,05*1,21;0,05)
            var longresult = (accentPrijs - basisPrijs) / 0.9 / 0.9 * 1.05 * 1.21;
            double bar = (longresult / 0.05) + 0.5;
            double foobar = System.Math.Round(bar, 0);
            var result = foobar * 0.05;
            result = System.Math.Round(result, 2);
            result = result < 0 ? 0 : result;
            return result;
        }

        public static AdvancedTegelSubCatInfo GetNewAccentModel(SubCatPerPakket subc, HoekProfiel gekozenProfiel, double totaalMeters)
        {
            // 
            var ctx = Acumulator.Instance().ctx;
            var data = new AdvancedTegelSubCatInfo();
            data.TotaalMeters = totaalMeters;
            var HC = (from cc in ctx.Categorieëns
                      where cc.C_ID == subc.CategorieNR
                      select cc.HC_NR).FirstOrDefault();

            if (subc.SubCatNR == "SCB3" || subc.SubCatNR == "SCB10" || subc.SubCatNR == "SCB12")
            {
                HC = "HC3";
            }
            data.HCNR = HC;
            data.SubCatNR = subc.SubCatNR;
            var sctitle = (from c in ctx.HoofdCategorieëns
                           where c.HC_ID == HC
                           select c.Omschrijving).FirstOrDefault().ToString();
            data.SubCatTitle = sctitle;

            var prodCheck = (from vapp in ctx.VrijAccentProductInfos
                             where vapp.PD_NR == gekozenProfiel.ProfielID
                             && vapp.HC_NR == HC
                             select vapp).FirstOrDefault();
            if (prodCheck != null)
            {

                var ppm = (from acprod in ctx.VrijAccentProductInfos
                           where acprod.PD_NR == gekozenProfiel.ProfielID &&
                           acprod.HC_NR == HC
                           select (from or in acprod.AccentSetOpbouw
                                   where or.VrijAccentRegel_NR == or.VrijAccentProductInfo.VAPP_ID
                                   select or.Prijs).FirstOrDefault()).FirstOrDefault();
                var prodCode = (from p in ctx.Productens
                                where p.PD_ID == gekozenProfiel.ProfielID
                                select p.productcode).FirstOrDefault();
                data.BasisTegel = new AdvancedTegelInfo
                {

                    ImgPath = gekozenProfiel.ProfielImg,
                    TegelOmschrijving = gekozenProfiel.Omschrijving,
                    TegelDetails = prodCode,
                    InwisselPrijs = (double)ppm,
                    PrijsPerMeter = (double)ppm - (double)ppm,
                    InstapPrijs = 0.0,
                    IsGekozen = true,
                    ProductId = gekozenProfiel.ProfielID,
                    Lengte = gekozenProfiel.Lengte,
                    Breedte = gekozenProfiel.Breedte,
                    NSNR = (from acprod in ctx.VrijAccentProductInfos
                            where acprod.PD_NR == gekozenProfiel.ProfielID &&
                           acprod.HC_NR == HC
                            select acprod.NS_NR).FirstOrDefault(),
                    ProdSoort = ProductSoort.HoekProfiel
                };



                if (HC == null)
                {
                    return null;
                }
                if (Acumulator.Instance().BB.HuidigRuimte == null || Acumulator.Instance().BB.HuidigRuimte.GekozenPakket == null)
                {
                    return null;
                }

                var validpdinf = (from prds in ctx.VrijAccentProductInfos
                                  where prds.HC_NR == HC
                                  && prds.PD_NR != data.BasisTegel.ProductId
                                  select prds);
                if (validpdinf == null || validpdinf.Count() <= 0)
                {
                    return null;
                }
                data.TegelInput = validpdinf;

                var atiList = GetTegelColl(validpdinf, (double)ppm, ProductSoort.HoekProfiel);
                atiList.Insert(0,data.BasisTegel);
               // atiList.Reverse();
                var eh = (from x in ctx.SubCats
                          where x.SCB_ID == subc.SubCatNR
                          select x.eenheidMETERS).FirstOrDefault().ToString();
                data.Regels = new ObservableCollection<AdvancedVervangRegel>
                {
                    new AdvancedVervangRegel
                    {
                        GekozenTegel = data.BasisTegel,
                        Status = StatusType.Basis,
                        RegelIndex = 0,
                        VervangInfo = new AdvancedVervangActie
                        {
                            Wijzigstand = false,
                            GekozenMeters = data.TotaalMeters,
                            //Opmerking ="Standaard keuze uit pakket",
                            Tegels = atiList
                        }  ,
                        Eenheid = eh,
                    },
                    AdvancedVervangRegel.GetNewTegelRegel(GetTegelColl(validpdinf,(double)ppm,ProductSoort.HoekProfiel),1,eh)
                };

                //
                return data;
            }
            else
            {
                return null;
            }


        }
        
        public static AdvancedTegelSubCatInfo GetNewAccentModel(SubCatPerPakket subc, Product gekozenTegel, double totaalMeters)
        {
            // 
            var ctx = Acumulator.Instance().ctx;
            var data = new AdvancedTegelSubCatInfo();
            data.TotaalMeters = totaalMeters;
            var HC = (from cc in ctx.Categorieëns
                      where cc.C_ID == subc.CategorieNR
                      select cc.HC_NR).FirstOrDefault();
            data.HCNR = HC;
            data.SubCatNR = subc.SubCatNR;
            var sctitle = (from c in ctx.HoofdCategorieëns
                           where c.HC_ID == HC
                           select c.Omschrijving).FirstOrDefault().ToString();
            data.SubCatTitle = sctitle;

            var prodCheck = (from vapp in ctx.VrijAccentProductInfos
                             where vapp.PD_NR == gekozenTegel.ID
                             && vapp.HC_NR == HC
                             select vapp).FirstOrDefault();
            if (prodCheck != null)
            {

                var ppm = (from acprod in ctx.VrijAccentProductInfos
                           where acprod.PD_NR == gekozenTegel.ID &&
                           acprod.HC_NR == HC
                           select (from or in acprod.AccentSetOpbouw
                                   where or.VrijAccentRegel_NR == or.VrijAccentProductInfo.VAPP_ID
                                   select or.Prijs).FirstOrDefault()).FirstOrDefault();
                data.BasisTegel = new AdvancedTegelInfo
                {
                    ImgPath = gekozenTegel.ImgPath,
                    TegelOmschrijving = gekozenTegel.Omschrijving,
                    TegelDetails = gekozenTegel.Naam,
                    InwisselPrijs = (double)ppm,
                    PrijsPerMeter = (double)ppm - (double)ppm,
                    InstapPrijs = 0.0,
                    IsGekozen = true,
                    ProductId = gekozenTegel.ID,
                    Lengte = gekozenTegel.Lengte,
                    Breedte = gekozenTegel.Breedte,
                    NSNR = (from acprod in ctx.VrijAccentProductInfos
                           where acprod.PD_NR == gekozenTegel.ID &&
                           acprod.HC_NR == HC
                           select acprod.NS_NR).FirstOrDefault(),
                    ProdSoort = HC != "HC6" ? ProductSoort.Tegel : ProductSoort.Dorpel
                };



                if (HC == null)
                {
                    return null;
                }
                if (Acumulator.Instance().BB.HuidigRuimte == null || Acumulator.Instance().BB.HuidigRuimte.GekozenPakket == null)
                {
                    return null;
                }

                var validpdinf = (from prds in ctx.VrijAccentProductInfos
                                  where prds.HC_NR == HC 
                                  && prds.PD_NR != data.BasisTegel.ProductId
                                  select prds);
                if (validpdinf == null || validpdinf.Count() <= 0)
                {
                    return null;
                }
                data.TegelInput = validpdinf;

                var atiList = GetTegelColl(validpdinf, (double)ppm,data.BasisTegel.ProdSoort);
                atiList.Insert(0,data.BasisTegel);
               // atiList.Reverse();
                var eh = (from x in ctx.SubCats
                          where x.SCB_ID == subc.SubCatNR
                          select x.eenheidMETERS).FirstOrDefault().ToString();
                data.Regels = new ObservableCollection<AdvancedVervangRegel>
                {
                    new AdvancedVervangRegel
                    {
                        GekozenTegel = data.BasisTegel,
                        Status = StatusType.Basis,
                        RegelIndex = 0,
                        VervangInfo = new AdvancedVervangActie
                        {
                            Wijzigstand = false,
                            GekozenMeters = data.TotaalMeters,
                            //Opmerking ="Standaard keuze uit pakket",
                            Tegels = atiList
                        },
                        Eenheid = eh
                    },
                    AdvancedVervangRegel.GetNewTegelRegel(GetTegelColl(validpdinf,(double)ppm,ProductSoort.Tegel),1,eh)
                };

                //
                return data;
            }
            else
            {
                return null;
            }
        }


    }
    public class AdvancedVervangRegel : AdvancedTegelModel
    {
        private StatusType status;
        public StatusType Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        private int regelIndex;
        public int RegelIndex
        {
            get { return regelIndex; }
            set { regelIndex = value; OnPropertyChanged("RegelIndex"); }
        }

        private AdvancedVervangActie vervangInfo;
        public AdvancedVervangActie VervangInfo
        {
            get
            {
                return vervangInfo;
            }
            set
            {
                vervangInfo = value;
                OnPropertyChanged("VervangInfo");
            }
        }

        private AdvancedTegelInfo gekozenTegel;
        public AdvancedTegelInfo GekozenTegel
        {
            get { return gekozenTegel; }
            set { gekozenTegel = value; OnPropertyChanged("GekozenTegel"); }
        }

        private AdvancedTegelInfo savedtegel;
        public AdvancedTegelInfo SavedTegel
        {
            get { return savedtegel; }
            set { savedtegel = value; OnPropertyChanged("SavedTegel"); }
        }

        private string eenheid;
        public string Eenheid
        {
            get { return eenheid; }
            set { eenheid = value; OnPropertyChanged("Eenheid"); }
        }
        private string opmerking;
        public string Opmerking
        {
            get { return opmerking; }
            set { opmerking = value; OnPropertyChanged("Opmerking"); }
        }

        private string savedopmerking;
        public string SavedOpmerking
        {
            get { return savedopmerking; }
            set { savedopmerking = value; OnPropertyChanged("SavedOpmerking"); }
        }
        

        public static AdvancedVervangRegel GetNewTegelRegel(ObservableCollection<AdvancedTegelInfo> tegelsInput, int index,string eenheid)
        {
            var nwRegel = new AdvancedVervangRegel
                {
                    Status = StatusType.Leeg,
                    RegelIndex = index,
                    VervangInfo = new AdvancedVervangActie
                    {
                        Wijzigstand = true,
                        GekozenMeters = 0.0,
                        //Opmerking = "Eerste vervang regel",
                        Tegels = tegelsInput
                    },
                    Eenheid = eenheid
                };
            return nwRegel;
        }
    }

    public class AdvancedVervangActie : AdvancedTegelModel
    {
        private bool wijzigstand;
        public bool Wijzigstand
        {
            get { return wijzigstand; }
            set 
            {
                wijzigstand = value;
                OnPropertyChanged("Wijzigstand");
                if (wijzigstand)
                {
                    savedmeters = gekozenMeters;
                    savedtegels = tegels;
                }
            }
        }

        private double gekozenMeters;
        public double GekozenMeters
        {
            get { return gekozenMeters; }
            set { gekozenMeters = value; OnPropertyChanged("GekozenMeters"); }
        }

        private string opmerking;
        public string Opmerking
        {
            get { return opmerking; }
            set { opmerking = value; OnPropertyChanged("Opmerking"); }
        }

        private ObservableCollection<AdvancedTegelInfo> tegels;
        public ObservableCollection<AdvancedTegelInfo> Tegels
        {
            get { return tegels; }
            set { tegels = value; OnPropertyChanged("Tegels"); }
        }

        private double savedmeters;
        public double SavedMeters
        {
            get{ return savedmeters;}
            set { savedmeters = value; OnPropertyChanged("SavedMeters"); }
        }

        private ObservableCollection<AdvancedTegelInfo> savedtegels;
        public ObservableCollection<AdvancedTegelInfo> SavedTegels
        {
            get { return savedtegels; }
            set { savedtegels = value; OnPropertyChanged("SavedTegels"); }
        }

    }

    public class AdvancedTegelInfo : AdvancedTegelModel
    {
        private ProductSoort prodSoort;
        public ProductSoort ProdSoort
        {
            get { return prodSoort; }
            set { prodSoort = value; OnPropertyChanged("ProdSoort"); }
        }
        private string tegelOmschrijving;
        public string TegelOmschrijving
        {
            get { return tegelOmschrijving; }
            set { tegelOmschrijving = value; OnPropertyChanged("TegelOmschrijving"); }
        }

        private string tegelDetails;
        public string TegelDetails
        {
            get { return tegelDetails; }
            set { tegelDetails = value; OnPropertyChanged("TegelDetails"); }
        }

        private int lengte;
        public int Lengte
        {
            get { return lengte; }
            set { 
                    lengte = value;
                    calcScale();
                    OnPropertyChanged("Lengte");
                    OnPropertyChanged("SLengte");
                }
        }

        private int breedte;
        public int Breedte
        {
            get { return breedte; }
            set {
                    breedte = value;
                    calcScale();    
                    OnPropertyChanged("Breedte");
                    OnPropertyChanged("SBreedte");
                }
        }

        public int SBreedte
        {
            get { return (int)(120 * Scale); }
        }
        public int SLengte
        {
            get { return (int)(120 * Scale); }
        }
        private double scale;
        public double Scale
        {
            get { calcScale(); return scale; }
            set { 
                    scale = value;
                    OnPropertyChanged("Scale");
                    OnPropertyChanged("SLengte");
                    OnPropertyChanged("SBreedte");
            }
        }
        private double prijsPerMeter;
        public double PrijsPerMeter
        {
            get { return prijsPerMeter; }
            set { prijsPerMeter = value; OnPropertyChanged("PrijsPerMeter"); }
        }

        private double inwisselprijs;
        public double InwisselPrijs
        {
            get { return inwisselprijs; }
            set { inwisselprijs = value; OnPropertyChanged("InwisselPrijs"); }
        }
        private double instapPrijs;
        public double InstapPrijs
        {
            get { return instapPrijs; }
            set { instapPrijs = value; OnPropertyChanged("InstapPrijs"); }
        }

        private bool isGekozen;
        public bool IsGekozen
        {
            get { return isGekozen; }
            set { isGekozen = value; OnPropertyChanged("IsGekozen"); }
        }

        private string productId;
        public string ProductId
        {
            get { return productId; }
            set { productId = value; OnPropertyChanged("ProductId"); }
        }

        private string imgPath;
        public string ImgPath
        {
            get { return imgPath; }
            set { imgPath = value; OnPropertyChanged("ImgPath"); }
        }

        private string nsNR;
        public string NSNR
        {
            get { return nsNR; }
            set { nsNR = value; OnPropertyChanged("NSNR"); }
        }

        private bool filtered;
        public bool Filtered
        {
            get { return filtered; }
            set { filtered = value; OnPropertyChanged("Filtered"); }
        }
        private ImageSource tegelImage;
        public ImageSource TegelImage
        {
            get { return tegelImage; }
            set { tegelImage = value; OnPropertyChanged("ImageSource"); }
        }

        private void calcScale()
        {
            int _breedte = Breedte / 10;
            int _lengte = Lengte / 10;

            int mfactor = 60;
            if (_lengte >= 60 || _breedte >= 60)
            {
                scale = 1;
            }
            else
            {
                if (_lengte > _breedte)
                {
                    scale = ((double)(_lengte * 2) + mfactor) / (120 + mfactor);
                }
                else
                {
                    scale = ((double)(_breedte * 2) + mfactor) / (120 + mfactor);
                }
            }

        }
    }
}

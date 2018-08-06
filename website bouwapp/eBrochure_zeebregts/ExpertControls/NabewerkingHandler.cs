using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using eBrochure_zeebregts.Web.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using eBrochure_zeebregts.Classes;
namespace eBrochure_zeebregts.ExpertControls
{
    public class NabewerkingHandler
    {


        //vars
        public ObservableCollection<NabewerkingOptieHolder> OptieHoldersCollection { get; set; }
        public ObservableCollection<NabewerkingUiRegel> UiRegelCollection { get; set; }
        //vars

        //hoofd functie voor nabewerking pagina (called per sub cat)
        public void MakeBewerkingMain(List<SubCatPerPakket> nabew, Ruimte r)
        {
            UiRegelCollection = new ObservableCollection<NabewerkingUiRegel>();

            var prodrefs = Acumulator.Instance().ctx.Productens;

            var tegelH = nabew.Where(x => x.SubCatNR == "SCB4").FirstOrDefault();
            if (tegelH != null)
            {
                var plafond = (from b in Acumulator.Instance().ctx.Bron_Ruimtes
                                   where b.R_NR == r.RuimteID
                                   select b.PLAFONDHOOGTE).FirstOrDefault();
                var hoogte = tegelH.TotaalMeters;
                BewerkingTegelHoogteRegel((int)plafond, hoogte);
            }

           

            foreach (var product in r.GekozenTegels)
            {
                var prodcat = (from pr in prodrefs
                               where pr.PD_ID == product.ID
                               select pr.PC_NR).FirstOrDefault().ToString();

                if (!String.IsNullOrEmpty(prodcat))
                {
                    var subcat = (from scb in Acumulator.Instance().ctx.SubCatPerPakkets
                                  where scb.SCBP_ID == product.LinkedSubCat
                                  select scb.SCB_NR).FirstOrDefault();

                    var scpp = nabew.Where(x => x.SubCatNR == subcat).FirstOrDefault();
                    switch (prodcat)
                    {
                        case "1"://tegel
                            var accentMatchT = r.AanvullendeMeters.Where(x => (x as ExtraMeters).ProdSoort == ExpertControls.Models.ProductSoort.Tegel
                                                                        && (x as ExtraMeters).LinkedProduct == product).FirstOrDefault();
                            if (scpp != null || accentMatchT != null)
                            {
                                BewerkingTegelRegel(scpp, product, accentMatchT);
                            }
                            break;
                        case "2"://profiel
                            //geen hoekprofielen hier
                            //BewerkingHoekprofielRegel(nabew, xxxx);
                            break;
                        case "3"://dorpel
                            var accentMatchD = r.AanvullendeMeters.Where(x => (x as ExtraMeters).ProdSoort == ExpertControls.Models.ProductSoort.Dorpel
                                                                       && (x as ExtraMeters).LinkedProduct == product).FirstOrDefault();
                            if (scpp != null || accentMatchD != null)
                            {
                                BewerkingDorpelRegel(scpp, product, accentMatchD);
                            }
                             
                            break;
                    }
                }

            }

            foreach (var accent in r.GekozenAccenten)
            {
                var prodcat = (from pr in prodrefs
                               where pr.PD_ID == accent.ID
                               select pr.PC_NR).FirstOrDefault().ToString();


                if (!String.IsNullOrEmpty(prodcat))
                {
                    switch (prodcat)
                    {
                        case "1"://tegel
                            var accentMatchT = r.AanvullendeMeters.Where(x => (x as ExtraMeters).ProdSoort == ExpertControls.Models.ProductSoort.Tegel
                   && (x as ExtraMeters).LinkedProduct.ID == accent.ID).FirstOrDefault();
                            BewerkingTegelRegelAccent(accent, accentMatchT);
                            break;
                        case "2"://profiel
                            //geen hoekprofiel hier >> profiel != product
                            break;
                        case "3"://dorpel
                            var accentMatchD = r.AanvullendeMeters.Where(x => (x as ExtraMeters).ProdSoort == ExpertControls.Models.ProductSoort.Dorpel
                   && (x as ExtraMeters).LinkedProduct.ID == accent.ID).FirstOrDefault();
                            BewerkingDorpelRegelAccent(accent, accentMatchD);
                            break;
                    }
                }
            }

            foreach (var profiel in r.GekozenProfielen)
            {
                var subcat = (from scb in Acumulator.Instance().ctx.SubCatPerPakkets
                              where scb.SCBP_ID == profiel.LinkedSubCat
                              select scb.SCB_NR).FirstOrDefault();

                var scpp = nabew.Where(x => x.SubCatNR == subcat).FirstOrDefault();

                var accentMatch = r.AanvullendeMeters.Where(x => (x as ExtraMeters).ProdSoort == ExpertControls.Models.ProductSoort.HoekProfiel
                    && (x as ExtraMeters).LinkedHoekProf.ProfielID == profiel.ProfielID).FirstOrDefault();

                if (scpp != null)
                {
                    BewerkingHoekprofielRegel(scpp, profiel, accentMatch);
                }
                else if (accentMatch != null)
                {
                    BewerkingHoekprofielRegelAccent(profiel, accentMatch);
                }
                
                
            }

        }

        public List<NabewerkingUiRegel> GetRegelsPerSub(string subcat)
        {
            return UiRegelCollection.Where(x => x.SubCat == subcat).ToList();
        }

        //product regels
        
        //tegel hoogte regel
        private void BewerkingTegelHoogteRegel(int plafond, double hoogte)
        {
            var eenh = GetEenheid("SCB4");
            var nRegel = new NabewerkingUiRegel
            {
                //Meters = hoogte, 
               // Eenheid = eenh,
                InfoText = plafond <= hoogte ? "Betegelen tot plafond: hoogte " + plafond + eenh : "Betegelen tot " + hoogte + eenh,
                IsBasis = true,
                SubCat = "SCB4",

            };
            UiRegelCollection.Add(nRegel);
            
        }
        //tegel regels
        //normaal
        private void BewerkingTegelRegel(SubCatPerPakket scpp, Product tegel, ExtraMeters accentInfo)
        {
            //not sure if true
           
            var nRegel = new NabewerkingUiRegel
            {
                Meters = accentInfo != null ? accentInfo.Meters :scpp.TotaalMeters,
                Eenheid = GetEenheid(tegel.LinkedSubCat),
                InfoText = tegel.Omschrijving.Split('€')[0],
                IsBasis = accentInfo == null,
                SubCat = accentInfo != null ? accentInfo.LinkedSubCat :scpp.SubCatNR,
                bewerkingenVoeg = scpp.Children.OfType<Nabewerking>().Where(x=> x.BewerkingCat_NR == "NC1").ToList(),
                bewerkingVerwerk = scpp.Children.OfType<Nabewerking>().Where(x => x.BewerkingCat_NR == "NC2").ToList(),
                ProductId = tegel.ID
                
                                                      
            };
            foreach (var nb in nRegel.bewerkingenVoeg)
            {
                nb.Kenmerk = tegel.ID + nRegel.SubCat;
            }
            foreach(var nb in nRegel.bewerkingVerwerk)
            {
                nb.Kenmerk = tegel.ID + nRegel.SubCat;
            }
            UiRegelCollection.Add(nRegel);
        }

        //accent
        private void BewerkingTegelRegelAccent(Product accentTegel, ExtraMeters accentInfo)
        {
            var nRegel = new NabewerkingUiRegel
            {
                Meters = accentInfo.Meters,
                Eenheid = GetEenheid(accentTegel.LinkedSubCat),
                InfoText = accentTegel.Omschrijving.Split('€')[0],
                IsBasis = false,
                SubCat = TranslateSubCat(accentInfo.LinkedSubCat),
                bewerkingenVoeg = GetNabewerkingSet(accentInfo.NabewerkingSetNR,accentInfo.LinkedSubCat).Where(x=>x.BewerkingCat_NR == "NC1").ToList(),
                bewerkingVerwerk = GetNabewerkingSet(accentInfo.NabewerkingSetNR, accentInfo.LinkedSubCat).Where(x => x.BewerkingCat_NR == "NC2").ToList(),
                ProductId = accentTegel.ID

            };
            foreach (var nb in nRegel.bewerkingenVoeg)
            {
                nb.Kenmerk = accentTegel.ID + nRegel.SubCat + "A";
            }
            foreach (var nb in nRegel.bewerkingVerwerk)
            {
                nb.Kenmerk = accentTegel.ID + nRegel.SubCat + "A";
            }
            UiRegelCollection.Add(nRegel);
        }

        //hoekprofiel regels
        //normaal
        private void BewerkingHoekprofielRegel(SubCatPerPakket scpp, HoekProfiel profiel, ExtraMeters accentInfo)
        {
            var nRegel = new NabewerkingUiRegel
            {
                Meters = accentInfo != null ? accentInfo.Meters : scpp.TotaalMeters,
                Eenheid = GetEenheid(profiel.LinkedSubCat),
                InfoText = profiel.Omschrijving.Split('€')[0],
                IsBasis = accentInfo == null,
                SubCat = accentInfo != null ? accentInfo.LinkedSubCat : scpp.SubCatNR
            };

            UiRegelCollection.Add(nRegel);
        }

        //accent
        private void BewerkingHoekprofielRegelAccent(HoekProfiel accentProfiel, ExtraMeters accentInfo)
        {
            var nRegel = new NabewerkingUiRegel
            {
                Meters = accentInfo.Meters,
                Eenheid = GetEenheid(accentProfiel.LinkedSubCat),
                InfoText = accentProfiel.Omschrijving.Split('€')[0],
                IsBasis = false,
                SubCat = TranslateSubCat(accentProfiel.LinkedSubCat)

            };
            UiRegelCollection.Add(nRegel);
        }

        //dorpel regels
        //normaal
        private void BewerkingDorpelRegel(SubCatPerPakket scpp,Product dorpel, ExtraMeters accentInfo)
        {
            var nRegel = new NabewerkingUiRegel
            {
                Meters = accentInfo != null ? accentInfo.Meters : scpp.TotaalMeters,
                Eenheid = GetEenheid(dorpel.LinkedSubCat),
                InfoText = dorpel.Omschrijving.Split('€')[0],
                IsBasis = accentInfo == null,
                SubCat = accentInfo != null ? accentInfo.LinkedSubCat : scpp.SubCatNR

            };
            UiRegelCollection.Add(nRegel);
        }

        //accent
        private void BewerkingDorpelRegelAccent(Product accentDorpel, ExtraMeters accentInfo)
        {

            var nRegel = new NabewerkingUiRegel
            {
                Meters = accentInfo.Meters,
                Eenheid = GetEenheid(accentInfo.LinkedSubCat),
                InfoText = accentDorpel.Omschrijving.Split('€')[0],
                IsBasis = false,
                SubCat = TranslateSubCat(accentInfo.LinkedSubCat)

            };
            UiRegelCollection.Add(nRegel);
        }

        //helper methods
        private static string GetEenheid(string inputLinkedSubCat)
        {
            var linkedSubCat = inputLinkedSubCat.StartsWith("Sub4Accent") ? inputLinkedSubCat.Substring(10) : inputLinkedSubCat;

            var eenheid = (from e in Acumulator.Instance().ctx.SubCats
                           where e.SCB_ID == (from scb in Acumulator.Instance().ctx.SubCatPerPakkets
                                              where scb.SCBP_ID == linkedSubCat
                                              select scb.SCB_NR).FirstOrDefault()
                           select e.eenheidMETERS).FirstOrDefault();

            if (string.IsNullOrEmpty(eenheid))
            {
                eenheid = (from e in Acumulator.Instance().ctx.SubCats
                           where e.SCB_ID == linkedSubCat
                           select e.eenheidMETERS).FirstOrDefault();
            }

            return eenheid;
        }
        
        public static string TranslateSubCat(string inputLinkedSubCat)
        {
            var linkedSubCat = inputLinkedSubCat.StartsWith("Sub4Accent") ? inputLinkedSubCat.Substring(10) : inputLinkedSubCat;
            if(linkedSubCat.StartsWith("SCBP"))
            {
                LogHelper.SendLog("Had to change linkedSubcat in nabewerkinghander orig was: " + linkedSubCat, LogType.error);
                linkedSubCat = "SCB3";
                
            }
            switch (linkedSubCat)
            {
                case "SCB15"://accent wand
                    linkedSubCat = "SCB1";
                    break;
                case "SCB16"://accent vloer
                    linkedSubCat = "SCB2";
                    break;
                case "SCB17"://accent hoekprofiel
                    linkedSubCat = "SCB3";
                    break;
                case "SCB18"://accent dorpel
                    linkedSubCat = "SCB18";
                    break;
            }
            return linkedSubCat;
        }

        private static List<Nabewerking> GetNabewerkingSet(string nabewerkingSetNr,string lsc)
        {
            var ctx = Acumulator.Instance().ctx;
            var bewerking = from bwrk in ctx.Nabewerkingens
                            join nbso in ctx.NabewerkingSetOpbouws on bwrk.N_ID equals nbso.N_NR
                            join nbst in ctx.NabewerkingSets on nbso.NS_NR equals nbst.NS_ID
                            where nbst.NS_ID == nabewerkingSetNr
                            orderby bwrk.volgorde
                            select new Nabewerking()
                            {
                                Nabewerking_ID = bwrk.N_ID,
                                LinkedSubcatNr =lsc,
                                BewerkingCat_NR = bwrk.NC_NR,
                                Omschrijving = bwrk.omschrijving,
                                volgorde = (int)bwrk.volgorde,
                                TextVoorZin = bwrk.TextVoorZin
                            };

            return bewerking.ToList();
        }
        //nabewerking collector
        private void AddNabewerking()
        {

        }

        public void GroupNabewerkingOptiesProducten()
        {
            var GroupIterator = 0;
            foreach (var holder in OptieHoldersCollection)
            {
                holder.GroupNr = GroupIterator;
                foreach (var comp in OptieHoldersCollection.Where(x => x != holder))
                {
                    foreach (var set in holder.NabewerkingSets)
                    {
                        var match = comp.NabewerkingSets.Where(x => x.NabewerkingSetNr == set.NabewerkingSetNr).FirstOrDefault();
                        if (match != null)
                        {
                            comp.GroupNr = holder.GroupNr;
                        }

                    }
                }
                GroupIterator++;
            }
        }
    }
    public class NabewerkingUiRegel
    {
        private double meters;
        public double Meters { get { return meters; } set { meters = value; MetersText = meters.ToString() + Eenheid; } }
        private string eenheid;
        public string Eenheid { get { return eenheid; } set { eenheid = value; MetersText = meters.ToString() + eenheid; } }
        public string MetersText { get; set; }
        public string InfoText { get; set; }
        public bool IsBasis { get; set; }
        public string SubCat { get; set; }

        public string ProductId { get; set; }
        public List<Nabewerking> bewerkingenVoeg { get; set; }
        public List<Nabewerking> bewerkingVerwerk { get; set; }

        //public Nabewerking SelectedVoeg { get; set; }
        //public Nabewerking SelectedVerwerk { get; set; }

    }

    public class NabewerkingOptieHolder
    {
        public ObservableCollection<NabewerkingSetholder> NabewerkingSets { get; set; }
        public string LinkedProdID { get; set; }
        public int GroupNr { get; set; }
    }
    public class NabewerkingSetholder
    {
        public string NabewerkingSetNr { get; set; }
        public List<string> NabewerkingIDs { get; set; }
       
    }
}

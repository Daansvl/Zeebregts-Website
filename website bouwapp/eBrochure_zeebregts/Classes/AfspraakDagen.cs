using eBrochure_zeebregts.ExpertControls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
namespace eBrochure_zeebregts.Classes
{
    public enum AfspraakStatus
    {
        Beschikbaar,
        Gereserveerd,
        Gekozen
    }

    public abstract class AfspraakBase : INotifyPropertyChanged
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
        private static AfspraakStatus StatusHelper(bool? statusbit, int gebruikerid)
        {
            AfspraakStatus resultStatus;

            if (statusbit == true)
            {
                if (gebruikerid == Acumulator.Instance().HuidigGebruiker.ID)
                {
                    resultStatus = AfspraakStatus.Gekozen;
                }
                else
                {
                    resultStatus = AfspraakStatus.Gereserveerd;
                }
            }
            else
            {
                resultStatus = AfspraakStatus.Beschikbaar;
            }

            return resultStatus;
        }
        
        public static AfspraakDagen LoadAfspraken()
        {
            var ctx = Acumulator.Instance().ctx;
            AfspraakDagen AfspraakSet = null;
            if (!String.IsNullOrEmpty(Acumulator.Instance().Projectnr))
            {
                AfspraakSet = new AfspraakDagen();
                var sets = ctx.AfsprakenSets.Where(x => x.ProjectNR == Acumulator.Instance().Projectnr);
                foreach (var set in sets)
                {
                    AfspraakSet.Project = set.ProjectNR;
                    AfspraakSet.Adviseur = set.Adviseur;
                    AfspraakSet.AdresNaam = set.AdresNaam;
                    AfspraakSet.AdresPlaats = set.AdresPlaats;
                    AfspraakSet.AdresStraat = set.AdresStraat;
                    foreach (var dag in set.AfspraakDagen)
                    {
                        if (AfspraakSet.Dagen == null)
                        {
                            AfspraakSet.Dagen = new ObservableCollection<AfspraakDag>();
                        }
                        var blokken = new ObservableCollection<AfspraakBlok>();
                        foreach (var blok in dag.BlokkenSets.Blokken)
                        {
                            blokken.Add(new AfspraakBlok
                            {
                                BlokID = blok.BlokID,
                                Eigenaar = blok.Eigenaar,
                                StartTijd = blok.StartTijd ?? DateTime.Now,
                                EindTijd = blok.EindTijd ?? DateTime.Now,
                                EigenaarNR = blok.EigenaarGebruikerID ?? -1,
                                Status = StatusHelper(blok.Status, blok.EigenaarGebruikerID ?? -1),
                            });

                        }

                        AfspraakSet.Dagen.Add(new AfspraakDag
                        {
                            Datum = dag.Datum ?? DateTime.Now,
                            DagNaam = dag.DagNaam,
                            Blokken = blokken

                        });
                    }
                }
            }
            return AfspraakSet;
        }
    }
    public class AfspraakDagen : AfspraakBase 
    {
        private ObservableCollection<AfspraakDag> _dagen;
        public ObservableCollection<AfspraakDag> Dagen { get { return _dagen; } set { _dagen = value; OnPropertyChanged("Dagen"); } }
        public string Project { get; set; }
        public string Adviseur { get; set; }
        public string AdresNaam { get; set; }
        public string AdresStraat { get;set; }
        public string AdresPlaats {get;set;}
    }
    public class AfspraakDag : AfspraakBase 
    {
        private ObservableCollection<AfspraakBlok> _blokken;
        public ObservableCollection<AfspraakBlok> Blokken { get { return _blokken; } set { _blokken = value; OnPropertyChanged("Blokken"); } }
        private string _dagNaam;
        public string DagNaam { get { return _dagNaam; } set { _dagNaam = value; OnPropertyChanged("DagNaam"); } }
        private DateTime _datum;
        public DateTime Datum { get { return _datum; } set { _datum = value; OnPropertyChanged("Datum"); } }
    }
    public class AfspraakBlok : AfspraakBase 
    {
        public int BlokID { get; set; }
        public DateTime StartTijd { get; set; }
        public DateTime EindTijd { get; set; }
        public string TijdOmschrijving
        {
            get
            {
                return StartTijd.ToShortTimeString() + " - " + EindTijd.ToShortTimeString();
            }
            set { ;}
        }
        private string eigenaar;
        public string Eigenaar { get { return eigenaar; } set { eigenaar = value; OnPropertyChanged("Eigenaar"); } }
        private int eigenaarNR;
        public int EigenaarNR { get { return eigenaarNR; } set { eigenaarNR = value; OnPropertyChanged("EigenaarNR"); } }
        private AfspraakStatus _status;
        public AfspraakStatus Status { get { return _status; } set { _status = value; OnPropertyChanged("StatusOmschrijving"); OnPropertyChanged("Status"); } }
        public string StatusOmschrijving
        {
            get
            {
                var omsch = ""; 
                switch (Status)
                {
                    case AfspraakStatus.Beschikbaar:
                        omsch = "Beschikbaar";
                        break;
                    case AfspraakStatus.Gereserveerd:
                        omsch = "Gereserveerd";
                        if (Acumulator.Instance().HuidigGebruiker.Rol == UserRole.Admin ||
                            Acumulator.Instance().HuidigGebruiker.Rol == UserRole.Adviseur ||
                            Acumulator.Instance().HuidigGebruiker.Rol == UserRole.Demo)
                        {
                            omsch = Eigenaar;
                        }
                        break;
                    case AfspraakStatus.Gekozen:
                        omsch = Acumulator.Instance().HuidigGebruiker.GebruikersNaam;
                        break;
                    default:
                        break;
                }
                return omsch;
            }
            private set { ;}
        }
    }
    public class AfspraakPopupInfo:AfspraakBase
    {
        public string DagNaam { get; set; }
        public DateTime Datum { get; set; }
        public DateTime StartTijd { get; set; }
        public DateTime EindTijd { get; set; }
        public string Adviseur { get; set; }
        public AfspraakStatus Status { get; set; }
        public string KnopText
        {
            get
            {
                var tekst = "";
                switch (Status)
                {
                    case AfspraakStatus.Gekozen:
                        tekst = "Annuleer";
                        break;
                    case AfspraakStatus.Beschikbaar:
                        tekst = "Reserveer";
                        break;
                    case AfspraakStatus.Gereserveerd:
                        tekst = "Geblokeerd";
                        break;
                }
                return tekst;
            }
            set { ;}
        }
        public bool KanKiezen
        {
            get { return Status == AfspraakStatus.Beschikbaar || Status == AfspraakStatus.Gekozen; }
            set {;}
        }
        public string TijdOmschrijving
        {
            get
            {
                return StartTijd.ToShortTimeString() + " - " + EindTijd.ToShortTimeString();
            }
            set { ;}
        }
        public string LocatieNaam { get; set; }
        public string LocatieStraat { get; set; }
        public string LocatiePlaats { get; set; }
        

    }
}

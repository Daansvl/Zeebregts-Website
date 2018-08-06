using eBrochure_zeebregts.Classes;
using eBrochure_zeebregts.KeuzeControls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace eBrochure_zeebregts.ExpertControls
{
    public partial class AfspraakMaker : UserControl
    {
        public AfspraakMaker()
        {
            InitializeComponent();
        }
        public void SetContext(AfspraakDagen dagen)
        {
            DataContext = dagen;
        }
        public DetailsWindow DwParent
        { get; set; }
        private void ListBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var MagKiezen = true;
            var blok = ((sender as ListBox).SelectedItem as AfspraakBlok);
            foreach(var dag in (DataContext as AfspraakDagen).Dagen)
            {
                foreach(var blk in dag.Blokken)
                {
                    if(blk.BlokID != blok.BlokID && blk.EigenaarNR == Acumulator.Instance().HuidigGebruiker.ID && blk.Status == AfspraakStatus.Gekozen)
                    {
                        MagKiezen = false;
                    }
                }
            }
            if (!MagKiezen)
            {
                MessageBox.Show("U kunt maar 1 afspraak tegelijk maken. Annuleer eerst uw andere afspraak.");
            }
            else
            {
               
                if (blok != null)
                {
                    var arche = DataContext as AfspraakDagen;
                    var dag = arche.Dagen.Where(x => x.Blokken.Contains(blok)).FirstOrDefault();

                    var popinf = new AfspraakPopupInfo
                    {
                        Adviseur = arche.Adviseur,
                        DagNaam = dag.DagNaam,
                        Datum = dag.Datum,
                        StartTijd = blok.StartTijd,
                        EindTijd = blok.EindTijd,
                        Status = blok.Status,
                        LocatieNaam = arche.AdresNaam,
                        LocatieStraat = arche.AdresStraat,
                        LocatiePlaats = arche.AdresPlaats,
                    };

                    var pop = new DetailsWindow();


                    var afpop = new AfspraakPopup(blok, pop, this);
                    afpop.DataContext = popinf;
                    pop.LoadContent(afpop);
                    pop.Height = 340;
                    pop.Width = 440;
                    pop.Show();
                }
            }
        }
        public void ReloadData()
        {
            var ctx = Acumulator.Instance().ctx;

            ctx.Load(ctx.GetBlokkenQuery(),System.ServiceModel.DomainServices.Client.LoadBehavior.RefreshCurrent,false).Completed += (sender1, args1) =>
            {
                ctx.Load(ctx.GetAfspraakDagenQuery(),System.ServiceModel.DomainServices.Client.LoadBehavior.RefreshCurrent,false).Completed += (sender2, args2) =>
                {
                    ctx.Load(ctx.GetBlokkenSetsQuery(),System.ServiceModel.DomainServices.Client.LoadBehavior.RefreshCurrent,false).Completed += (sender3, args3) =>
                    {
                        ctx.Load(ctx.GetAfsprakenSetsQuery(),System.ServiceModel.DomainServices.Client.LoadBehavior.RefreshCurrent,false).Completed += (sender4, args4) =>
                        {
                            DataContext = AfspraakBase.LoadAfspraken();
                        };
                    };
                };
            };
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(DwParent != null)
            {
                DwParent.Close();
            }
        }

        
    }
    public class Status2ColorConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value as AfspraakStatus?;
            if(status == null)
            {
                return new SolidColorBrush(Colors.Yellow);
            }
            if(status == AfspraakStatus.Beschikbaar)
            {
                return new SolidColorBrush(Colors.Green);
            }
            else if(status == AfspraakStatus.Gereserveerd)
            {
                return new SolidColorBrush(Colors.Orange);
            }
            else if(status == AfspraakStatus.Gekozen)
            {
                return new SolidColorBrush(Colors.White);
            }
            else
            {
                return new SolidColorBrush(Colors.Yellow);
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

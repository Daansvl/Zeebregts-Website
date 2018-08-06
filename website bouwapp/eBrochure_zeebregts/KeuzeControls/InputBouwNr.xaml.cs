using System;
using System.Linq;
using System.Windows.Controls;
using eBrochure_zeebregts.Web.Services;
using eBrochure_zeebregts.Web;
using System.Collections.Generic;
using System.ComponentModel;

namespace eBrochure_zeebregts
{
	public partial class InputBouwNr : UserControl, IBaseControl
	{
		private bool KiesProject = false;
		private bool KiesBouwnummer = false;
		public InputBouwNr(UserRole ur)
		{
			
			InitializeComponent();
		//	LoadList();
			
			if (ur == UserRole.Adviseur || ur==UserRole.Admin || ur == UserRole.Showroom)
			{
				KiesProject = true;
				KiesBouwnummer = true;
				loadProj(ur);
			}
            else if (ur == UserRole.Demo)
            {
                KiesBouwnummer = true;
                loadProj(ur);
             
            }
			

		}
		private void loadProj(UserRole ur)
		{
			eBrochureDomainContext ctx;
			if (Acumulator.Instance().ctx == null)
			{
				ctx = new eBrochureDomainContext();
				Acumulator.Instance().ctx = ctx;
			}
			else
			{
				ctx = Acumulator.Instance().ctx;
			}

			if (ur == UserRole.Admin)
			{
				ctx.Load(ctx.GetPRojectQuery()).Completed += (sender, args) =>
				{
					ProjectNrCbb.ItemsSource = ctx.PRojects;
					ProjectNrCbb.SelectedValuePath = "PR_ID";
					ProjectNrCbb.DisplayMemberPath = "Omschrijving";
					KiesProject = true;
				};
			}
			else if (ur == UserRole.Adviseur || ur==UserRole.Demo || ur == UserRole.Showroom)
			{
				ctx.Load(ctx.GetPRojectQuery()).Completed += (sender1, args1) =>
				{
					ctx.Load(ctx.GetGebruikersQuery()).Completed += (sender2, args2) =>
					{
						ctx.Load(ctx.GetProjectSetOpbouwQuery()).Completed += (sender, args) =>
						{
                            
							var pros = (from p in ctx.PRojects
										join pso in ctx.ProjectenSetOpbouws on p.PR_ID equals pso.Project_NR
										join g in ctx.Gebruikers on pso.ProjectenSet_NR equals g.ProjectenSet_NR
                                        where g.GebruikersID == Acumulator.Instance().HuidigGebruiker.ID
										select p).ToList();

							ProjectNrCbb.ItemsSource = pros;
							ProjectNrCbb.SelectedValuePath = "PR_ID";
							ProjectNrCbb.DisplayMemberPath = "Omschrijving";
							KiesProject = true;
                            if (ProjectNrCbb.Items.Count == 1)
                            {
                                ProjectNrCbb.SelectedIndex = 0;
                                ProjectNrCbb.IsEnabled = false;
                                //ProjectNrCbb.Visibility = System.Windows.Visibility.Collapsed;
                                //projlabel.Visibility = System.Windows.Visibility.Collapsed;
                            }
						};
					};
				};
			}
		}
		
		public bool SubmitPressed()
		{
			bool retval = false;
			if (KiesProject)
			{
				if (ProjectNrCbb.SelectedIndex > -1)
				{
					ProjErrorLbl.Text = String.Empty;
                    Acumulator.Instance().Projectnr = (string)ProjectNrCbb.SelectedValue;
				}
				else
				{
					ProjErrorLbl.Text = "Geen Project geselecteerd.";
					retval = false;
				}

			}
			if (KiesBouwnummer)
			{
				if (BouwNrCbb.SelectedIndex > -1)
				{
					BouwErrorLbl.Text = String.Empty;
					Acumulator.Instance().Bouwnr = (string)BouwNrCbb.SelectedValue;
                    Acumulator.Instance().oOL = (BouwNrCbb.SelectedItem as BouwnummerItem).oOL;
					retval = true;
				}
				else
				{
					BouwErrorLbl.Text = "Geen bouwnummer geselecteerd.";
					retval = false;
				}
			}
			return retval;

		}
		public void Clear4Submit()
		{ }
		public void WijzigPressed()
		{
		}
        public void LoadBnrListbox(List<OpgeslagenOfferteLean> oOs)
        {
            var ctx = Acumulator.Instance().ctx;
            List<BouwnummerItem> source;
            if (Acumulator.Instance().HuidigGebruiker.Rol == UserRole.Showroom)
            {
                /*var foo = (from g in ctx.Gebruikers
                            where g.GebruikersID == Acumulator.Instance().HuidigGebruiker.ID
                            select g.BouwnummerSet_NR).FirstOrDefault();
                var bar = (from bso in ctx.BouwnummerSetOpbouws
                            where bso.BouwnummerSet_NR == foo
                            select bso.Bouwnummer_NR).ToList();
                var foobar = (from bnr in ctx.Bouwnummers
                                where bar.Contains(bnr.B_ID)
                                select bnr).ToList();*/

                source = (from bnr in ctx.Bouwnummers
                          join bso in ctx.BouwnummerSetOpbouws on bnr.B_ID equals bso.Bouwnummer_NR
                          join g in ctx.Gebruikers on bso.BouwnummerSet_NR equals g.BouwnummerSet_NR
                          join tp in ctx.bouwTypes on bnr.T_NR equals tp.T_ID
                       
                          
                          where g.GebruikersID == Acumulator.Instance().HuidigGebruiker.ID
                          select new BouwnummerItem(bnr.B_ID, bnr.Omschrijving, bnr.VerkoopStatus != null ? bnr.VerkoopStatus.StatusNaam : "", tp.Omschrijving, oOs.Where(x => x.B_ID == bnr.B_ID).FirstOrDefault())).ToList();
            }
            else
            {

                source = (from bnr in ctx.Bouwnummers
                          join tp in ctx.bouwTypes on bnr.T_NR equals tp.T_ID
                         
                        
                          where bnr.PR_NR == ProjectNrCbb.SelectedValue.ToString()
                          select new BouwnummerItem(bnr.B_ID, bnr.Omschrijving,bnr.VerkoopStatus != null ? bnr.VerkoopStatus.StatusNaam : "" , tp.Omschrijving, oOs.Where(x => x.B_ID == bnr.B_ID).FirstOrDefault())).ToList();
            }
            Helpers.CustomComparer<BouwnummerItem> comp = new Helpers.CustomComparer<BouwnummerItem>();
            source.Sort(comp);
            BouwNrCbb.Items.Clear();
            foreach (var x in source)
            {
                BouwNrCbb.Items.Add(x);
            }
            //     BouwNrCbb.ItemsSource = source;
            BouwNrCbb.SelectedValuePath = "B_ID";
            LoadingMsg.Visibility = System.Windows.Visibility.Collapsed;
            if (BouwNrCbb.Items.Count > 0)
            {
                BouwNrCbb.Visibility = System.Windows.Visibility.Visible;
            }
        }
		private void ProjectNrCbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
            LoadingMsg.Visibility = System.Windows.Visibility.Visible;
            BouwNrCbb.Visibility = System.Windows.Visibility.Collapsed;
            if (ProjectNrCbb.Items.Count > 0 && ProjectNrCbb.SelectedItem != null)
            {
                GemaakteKeuze.Text = (ProjectNrCbb.SelectedItem as PRoject).Omschrijving;
            }
			if (ProjectNrCbb.SelectedIndex > -1)
			{
                BouwNrCbb.Items.Clear();
				eBrochureDomainContext ctx;
				if (Acumulator.Instance().ctx != null)
				{
					ctx = Acumulator.Instance().ctx;

					ctx.Load(ctx.GetBouwnummersQuery()).Completed += (sender1, args) =>
					{
                        ctx.Load(ctx.GetBouwnummerSetOpbouwQuery()).Completed += (sender2, args2) =>
                        {
                            ctx.Load(ctx.GetBouwTypesQuery()).Completed += (sender3, args3) =>
                            {
                                ctx.Load(ctx.GetVerkoopStatusQuery()).Completed += (sender01, args01) =>
                                 {
                                    List<OpgeslagenOfferteLean> oOs;
                                    ctx.OpgeslagenOffertes.Clear();
                                    ctx.GetOpgeslagenOfferteLean(operation => 
                                    {
                                        oOs = operation.Value;
                                        if (oOs != null)
                                        {
                                            LoadBnrListbox(oOs);
                                        }
                                    }, null);
                                };
                            };
                        };
                    };
				}
			}
		}

        private void BouwNrCbb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BouwNrCbb.Items.Count > 0 && BouwNrCbb.SelectedItem != null)
            {
                GemaakteKeuze.Text = (ProjectNrCbb.SelectedItem as PRoject).Omschrijving + " : " + (BouwNrCbb.SelectedItem as BouwnummerItem).B_omschrijving;
            }
        }

	}
    public class BouwnummerItem :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _B_ID;
        private string _B_omschrijving;
        private string _verkoopStatus;
        private string _TypeOmschrijving;
        private bool _Opgeslagen;
        private string _gebruiker;
        private bool _Accoord;
        private OpgeslagenOfferteLean _oOL;
        
        public string B_ID
        { get { return _B_ID; } set { _B_ID = value; OnPropertyChanged("B_ID"); } }
        public string B_omschrijving
        { get { return _B_omschrijving; } set { _B_omschrijving = value; OnPropertyChanged("B_omschrijving"); } }
        public string VerkoopStatus
        {
            get { return _verkoopStatus; }
            set { _verkoopStatus = value; OnPropertyChanged("VerkoopStatus"); }
        }
        public string TypeOmschrijving
        { get { return _TypeOmschrijving; } set { _TypeOmschrijving = value; OnPropertyChanged("TypeOmschrijving"); } }
        public bool Opgeslagen
        { get { return _Opgeslagen; } set { _Opgeslagen = value; OnPropertyChanged("Opgeslagen"); } }
        public string Gebruiker
        { get { return _gebruiker; } set { _gebruiker = value; OnPropertyChanged("Gebruiker"); } }
        public bool Accoord
        { get { return _Accoord; } set { _Accoord = value; OnPropertyChanged("Accoord"); } }
        public OpgeslagenOfferteLean oOL
        { get { return _oOL; } set { _oOL = value; } }
        public BouwnummerItem(string Bid, string omsch, string verkoopstatus,string tpe,OpgeslagenOfferteLean oO)
        {
            B_ID = Bid;
            B_omschrijving = omsch;
            TypeOmschrijving = tpe;
            Accoord = false;
           VerkoopStatus = verkoopstatus;
            oOL = oO;
            if (oOL != null)
                    {
                        Gebruiker = oOL.gebruiker;
                        Opgeslagen = true;
                    }
                    else
                    {
                        Opgeslagen = false;
                    }
                

        /*  ctx.Load(ctx.GetOpgeslagenOfferteByIdQuery(B_ID)).Completed += (sender, args) =>
                {
                    if (ctx.OpgeslagenOffertes.Count > 0)
                    {
                        Opgeslagen = true;

                    }
                    else
                    {
                        Opgeslagen = false;
                    }
                };*/
            
        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}

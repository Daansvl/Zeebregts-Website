using eBrochure_zeebregts.Classes;
using eBrochure_zeebregts.ExpertControls;
using eBrochure_zeebregts.ExpertControls.Models;
using eBrochure_zeebregts.KeuzeControls;
using eBrochure_zeebregts.Web.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
namespace eBrochure_zeebregts
{
	public partial class StartPagina : UserControl
	{
		private BrochureBasis BB;
		public StartPagina()
		{
			InitializeComponent();
			ControlHolder.Children.Add(new TextBlock() { Text = "Login om te beginnen met de online tegelbrochure" });
			Acumulator.Instance().InfoBar = infoBar;
            Acumulator.Instance().StartPagina = this;
			infoBar.Visibility = Visibility.Collapsed;
        	
		}
        public void RecoverError()
        {
            SwitchBnrFunc();
        }
		private void HomeBtn_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("http://www.zeebregts.nl"));
		}

		private void LoginBtn_Click(object sender, RoutedEventArgs e)
		{
			LoginWindow loginWnd = new LoginWindow();
			loginWnd.Closed += new EventHandler(loginWnd_Closed);
			loginWnd.Show();
		}

		void loginWnd_Closed(object sender, EventArgs e)
		{
			ControlHolder.Children.Clear();
			LoginWindow lw = (LoginWindow)sender;
			if (lw.DialogResult == true && lw.NaamBox.Text != string.Empty)
			{
				LoginBtn.Visibility = Visibility.Collapsed;
				LogouBtn.Visibility = Visibility.Visible;
				Acumulator.Instance().GebruikersNaam = lw.NaamBox.Text;
				ControlHolder.Children.Add(new TextBlock() { Text = "Welkom " + lw.NaamBox.Text, FontSize = 18 });
				TimeOut.SetTimeout(500, () =>
				{
					startBorchure();
					
				});

			}
			else if (lw.DialogResult == false)
			{
				ControlHolder.Children.Add(new TextBlock() { Text = "Login geannuleerd.", FontSize = 18 });

			}
		}
		private void startBorchure()
		{
            
			ControlHolder.Children.Clear();
			BB = new BrochureBasis(this);
			ControlHolder.Children.Add(BB);
			BB.LoadSubPage(SwitchStart4User());
		}
		private void LogouBtn_Click(object sender, RoutedEventArgs e)
		{
			infoBar.Visibility = Visibility.Collapsed;
			infoBar.Clear4Submit();
			LogouBtn.Visibility = Visibility.Collapsed;
			LoginBtn.Visibility = Visibility.Visible;
			infoBar.Visibility = Visibility.Collapsed;
			NewUserBtn.Visibility = Visibility.Collapsed;
			SwitchBnrBtn.Visibility = Visibility.Collapsed;
            AfspraakBtn.Visibility = Visibility.Collapsed;
			ControlHolder.Children.Clear();
            Acumulator.Instance().uitvoerPlaceHolder = null;
            Acumulator.Instance().SavedXml = null;
			Acumulator.Instance().bluePrintManager.ClearAll();
            //Acumulator.Instance().HuidigRuimteSetKey.Clear();
            //Acumulator.Instance().TekeningBijRuimte.Clear();
            Acumulator.Instance().clearDownloadsActive();
            Acumulator.Instance().oOL = null;
			ControlHolder.Children.Add(new TextBlock(){Text = "U bent nu uitgelogd"});
			WebContext.Current.Authentication.Logout(false);
			LogHelper.SendLog("Log out User: " + Acumulator.Instance().HuidigGebruiker.GebruikersNaam, LogType.activity);
		}
		public void Scroll2Top()
		{
			ScrollViewer.ScrollToVerticalOffset(0);
		}

		private SubPage SwitchStart4User()
		{
			SubPage sp = new SubPage();
			switch (Acumulator.Instance().HuidigGebruiker.Rol)
			{
				case UserRole.Admin:
					sp.Titel = "Kies Project en Bouwnummer";
					sp.Addcontrol(new InputBouwNr(Acumulator.Instance().HuidigGebruiker.Rol));
					NewUserBtn.Visibility = Visibility.Visible;
					SwitchBnrBtn.Visibility = Visibility.Visible;
                    AfspraakBtn.Visibility = Visibility.Visible;
					break;
                case UserRole.Showroom:
                case UserRole.Demo:
				case UserRole.Adviseur:
					sp.Titel = "Kies Bouwnummer";
					sp.Addcontrol(new InputBouwNr(Acumulator.Instance().HuidigGebruiker.Rol));
					SwitchBnrBtn.Visibility = Visibility.Visible;
                    AfspraakBtn.Visibility = Visibility.Visible;
					break;
				
				case UserRole.Bewoner:
					var ctx = new eBrochureDomainContext();
					Acumulator.Instance().ctx = ctx;
					ctx.Load(ctx.GetGebruikersQuery()).Completed += (sender,args)=>
						{
							ctx.Load(ctx.GetBouwnummerSetOpbouwQuery()).Completed += (sender2, args2) =>
								{
                                    ctx.Load(ctx.GetBouwnummersQuery()).Completed += (sender3, args3) =>
                                        {
                                            var bnr = (from b in ctx.BouwnummerSetOpbouws
                                                       join g in ctx.Gebruikers on b.BouwnummerSet_NR equals g.BouwnummerSet_NR
                                                       join baseb in ctx.Bouwnummers on b.Bouwnummer_NR equals baseb.B_ID
                                                       where g.Naam.ToLower() == Acumulator.Instance().GebruikersNaam.ToLower()
                                                       select new { b.Bouwnummer_NR, baseb.PR_NR }).FirstOrDefault();
                                            Acumulator.Instance().Bouwnr = bnr.Bouwnummer_NR;
                                            Acumulator.Instance().Projectnr = bnr.PR_NR;
                                            BB.GeenInputBouwnummer();
                                        };
								};
						};
                    AfspraakBtn.Visibility = Visibility.Visible;
					break;

			}
			return sp;
		}

		private void NewUserBtn_Click(object sender, RoutedEventArgs e)
		{
			DetailsWindow dw = new DetailsWindow();
            dw.Height = 450;
			dw.ContentPanel.Children.Add(new NewUserControl(dw));
			dw.Width = 600; //dw.Height = 770;
			dw.Show();
		}
        private void SwitchBnrFunc()
        {
			Acumulator.Instance().bluePrintManager.ClearAll();
            //Acumulator.Instance().ClearBouwnummer();
            Acumulator.Instance().uitvoerPlaceHolder = null;
            Acumulator.Instance().SavedXml = null;
            //Acumulator.Instance().HuidigRuimteSetKey.Clear();
            //Acumulator.Instance().TekeningBijRuimte.Clear();
            Acumulator.Instance().clearDownloadsActive();
            Acumulator.Instance().oOL = null;
            infoBar.Clear4Submit();
            startBorchure();
        }
		private void SwitchBnrBtn_Click(object sender, RoutedEventArgs e)
		{
            SwitchBnrFunc();
		}
        private void LoadAfspraken()
        {
            var afspMaker = new AfspraakMaker();
            afspMaker.ReloadData();
            //var AfspraakSet = AfspraakBase.LoadAfspraken();

            if (afspMaker != null && !String.IsNullOrEmpty(Acumulator.Instance().Projectnr))
            {
             
                //afspMaker.SetContext(AfspraakSet);
                var holder = new DetailsWindow();
                afspMaker.DwParent = holder;
                holder.Width = 820;
                holder.Height = 500;
                holder.LoadContent(afspMaker);
                holder.Show();
            }
            else
            {
                //text select project first
                MessageBox.Show("Geen Project geselecteerd!");
            }
        }
        private void AfspraakBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadAfspraken();
        }

        private void TestBtn_Click(object sender, RoutedEventArgs e)
        {
            //var testWindow = new DetailsWindow();
            //var page = new AdvancedTegelSubCatPage();
            //var prod = new Product();
            //if(Acumulator.Instance().BB.HuidigRuimte.GekozenTegels.FirstOrDefault() != null)
            //{
            //    prod = Acumulator.Instance().BB.HuidigRuimte.GekozenTegels.FirstOrDefault();
            //}
            //var data = AdvancedTegelSubCatInfo.GetNewAccentModel("C2",prod, 100.0);
            ////var data = new AdvancedTegelSubCatInfo();
            /////////
            ////data.TotaalMeters = 50.02;
            ////data.BasisTegel = new AdvancedTegelInfo
            ////                {
            ////                    TegelOmschrijving = "Dit is een vloertegel Op de Wand",
            ////                    TegelDetails = "60x60 Gebroken Beige",
            ////                    PrijsPerMeter = 19.95,
            ////                    IsGekozen = true
            ////                };
            ////data.Regels = new ObservableCollection<AdvancedVervangRegel>
            ////{
            ////    new AdvancedVervangRegel
            ////    {
            ////        Status = StatusType.Basis,
            ////        GekozenTegel = new AdvancedTegelInfo
            ////        {
            ////            TegelOmschrijving = "Dit is een vloertegel Op de Wand",
            ////            TegelDetails = "60x60 Gebroken Beige",
            ////            PrijsPerMeter = 19.95,
            ////            IsGekozen =true
            ////        },
            ////        VervangInfo = new AdvancedVervangActie
            ////        {
            ////            Wijzigstand = false,
            ////            GekozenMeters= data.TotaalMeters,
            ////            Opmerking = "Dit is de eerste Regel",
            ////            Tegels = new ObservableCollection<AdvancedTegelInfo>
            ////            {
            ////                new AdvancedTegelInfo
            ////                {
            ////                    TegelOmschrijving = "Dit is een vloertegel Op de Wand",
            ////                    TegelDetails = "60x60 Gebroken Beige",
            ////                    PrijsPerMeter = 19.95,
            ////                    IsGekozen =true
            ////                }
            ////            }
            ////        }
            ////    },
            ////    new AdvancedVervangRegel
            ////    {
            ////        Status = StatusType.Leeg,
            ////        VervangInfo = new AdvancedVervangActie
            ////        {
            ////            Wijzigstand=true,
            ////            GekozenMeters = 0.0,
            ////            Opmerking = "Dit is de tweede Regel",
            ////            Tegels = new ObservableCollection<AdvancedTegelInfo>
            ////            {
            ////                new AdvancedTegelInfo
            ////                {
            ////                    TegelOmschrijving = "Dit is een vloertegel Op de Wand",
            ////                    TegelDetails = "60x60 Gebroken Beige",
            ////                    PrijsPerMeter = 19.95,
            ////                    IsGekozen = false
            ////                },
            ////                new AdvancedTegelInfo
            ////                {
            ////                    TegelOmschrijving = "Dit is een andere vloertegel Op de Wand",
            ////                    TegelDetails = "60x60 BLauw Wit",
            ////                    PrijsPerMeter = 24.95,
            ////                    IsGekozen = false
            ////                }
            ////            }
            ////        }
            ////    }
            ////};
            ///////
            //if (data != null)
            //{
            //    page.LoadContext(data);
            //    testWindow.Width = 800;
            //    testWindow.Height = 800;
            //    testWindow.LoadContent(page);
            //    testWindow.Show();
            //}
        }
	}
}

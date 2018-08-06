using eBrochure_zeebregts.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace eBrochure_zeebregts.KeuzeControls {
	public partial class BluePrintPage : UserControl,IBaseControl {
		private string ruimteID = "";
		private bool failed = true;
		public BluePrintPage(string r_id) {

			InitializeComponent();
			ruimteID = r_id;
			init();
		}
		private void init() 
		{
            System.Threading.Thread.Sleep(1000);
            var status = Acumulator.Instance().bluePrintManager.GetBlueprintStatus(ruimteID);

            if (status == Helpers.BluePrintStatus.Complete)
            { 
                initCompleet(); 
            }
            else if (status == Helpers.BluePrintStatus.Unavailable)
            {
                BlueprintUnavailable();
            }
            else
            {
                initFailed();
            }
		}
		private void initCompleet() 
        {
            if (Acumulator.Instance().BB.IsCurrentComplete())
            {
                StatusText.Text = "Deze plattegrond is het resultaat van de door u gekozen opties.";
            }
            else
            {
                StatusText.Text = "Bevestig de plategrond als gevolg van de gekozen opties.";
			}
			failed = false;
			RetryBtn.Visibility = Visibility.Collapsed;
			geenBlueprintCB.Visibility = Visibility.Collapsed;
			BluePrintImg.Visibility = Visibility.Visible;
			LoadImage();
		}
		private void initFailed() {
			BluePrintImg.Visibility = Visibility.Collapsed;
			failed = true;
			StatusText.Text = "Het downloaden van de plattegrond is mislukt.\nKlik op retry om nogmaals te donwloaden/n of bevestig dat u de offerte wilt afmaken zonder plategronden.\n Deze kunnen later naar u worden gemaild.";

		}
		private void LoadImage() {
			var data = Acumulator.Instance().bluePrintManager.getBlueprintData(ruimteID,false);
			if (data != null) {
				BluePrintImg.LoadImg(data);
			}
			else
				initFailed();
		}
		private void RetryBtn_Click_1(object sender, RoutedEventArgs e) {
			var bw = new BackgroundWorker();
			bw.DoWork += bw_DoWork;
			bw.RunWorkerCompleted += bw_RunWorkerCompleted;
			Acumulator.Instance().BusyBee.IsBusy = true;
			Acumulator.Instance().BusyBee.BusyContent = "Plattegrond opnieuw aan het downloaden...";
			bw.RunWorkerAsync();
		}

        private void BlueprintUnavailable()
        {
            failed = false;
            StatusText.Text = "Voor deze configuratie is geen plattegrond beschikbaar.";
            RetryBtn.Visibility = Visibility.Collapsed;
            geenBlueprintCB.Visibility = Visibility.Collapsed;
            BluePrintImg.Visibility = Visibility.Collapsed;
        }
	private	void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			init();
			Acumulator.Instance().BusyBee.IsBusy = false;
		}

	private	void bw_DoWork(object sender, DoWorkEventArgs e) {
			var holder = Acumulator.Instance().bluePrintManager.RetryDownload(ruimteID);
		
			
			while (holder.statusInfo != Helpers.BluePrintStatus.Complete && holder.statusInfo != Helpers.BluePrintStatus.Failed)
				System.Threading.Thread.Sleep(500);

			
			
		}

		private void geenBlueprintCB_Checked_1(object sender, RoutedEventArgs e) {
			RetryBtn.IsEnabled = !(bool)geenBlueprintCB.IsChecked;
		}
		public void WijzigPressed() { }
		public void Clear4Submit() { }
		public bool SubmitPressed() {
			if (!failed)
				return true;
			else if (geenBlueprintCB.IsChecked == true)
				return true;
			else
				return false;
		}
	}
}

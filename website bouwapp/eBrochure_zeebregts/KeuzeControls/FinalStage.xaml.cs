using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using eBrochure_zeebregts.Classes;
using System.IO;
//using ImageTools.IO.Bmp;
using ImageTools.IO.Jpeg;
//using ImageTools.IO.Png;
using eBrochure_zeebregts.Controls;
using eBrochure_zeebregts.Helpers;
using ICSharpCode.SharpZipLib.Zip;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using eBrochure_zeebregts.ExpertControls.UiElements;


namespace eBrochure_zeebregts.KeuzeControls
{
    
	public partial class FinalStage : UserControl
	{
         private List<byte[]> FileStreams = new List<byte[]>();
         private bool SaveNeeded = true;
         private bool Isvolledig;
         private BackgroundWorker BW;
         public DateTime PrintDatum;
		public FinalStage()
		{
            Acumulator.Instance().FSdelegate = this;
			InitializeComponent();
            Acumulator.Instance().ctx.GetServerTime(operation => 
            {
                PrintDatum = operation.Value;
                Acumulator.Instance().BusyBee.BusyContent = "Uitvoer genereren...";
                Acumulator.Instance().BusyBee.IsBusy = true;
                if (Acumulator.Instance().HuidigGebruiker.Rol == UserRole.Admin || Acumulator.Instance().HuidigGebruiker.Rol == UserRole.Adviseur)
                {
                    ResetSave.Visibility = Visibility.Visible;
                }
                //ImageTools.IO.Encoders.AddEncoder<BmpEncoder>();
                ImageTools.IO.Encoders.AddEncoder<JpegEncoder>();
                //CreateZip();
                //ImageTools.IO.Encoders.AddEncoder<PngEncoder>();
                BW = new BackgroundWorker();
                BW.DoWork += new DoWorkEventHandler(BW_DoWork);
                BW.ProgressChanged += new ProgressChangedEventHandler(BW_ProgressChanged);
                BW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BW_RunWorkerCompleted);
				newInitPre();//InitializeFinalStage_Pre();
            }, null);
            
		}
		private PdfDocument _File;
        
		private List<string> InvalidChars = new List<string>(){@"/",@"\",@":",@"*",@"?",@"<",@">",@"|"};
		private void MakePdf(List<ImageTools.ExtendedImage> imgList, List<ImageTools.ExtendedImage> bpImgList)
		{
            var document = new PdfDocument();
           // UitvoerCreator uitvoercreator = new UitvoerCreator();
            foreach (var img in imgList)
            {
                PdfPage page = document.AddPage();
                page.Width = 797;
                page.Height = 1123;
                XGraphics gfx = null;
                UIThread.Invoke(() => { gfx = XGraphics.FromPdfPage(page); });
                while (gfx == null)
                {
                    System.Threading.Thread.Sleep(100);
                }
                using (MemoryStream mstream = new MemoryStream())
                {


                    //var img = UitvoerView.GetImg();
                    if (img != null)
                    {
                        img.DensityX = 300;
                        img.DensityY = 300;
                        JpegEncoder encoder = new JpegEncoder();
                        encoder.Encode(img, mstream);
                        MemoryStream mss = new MemoryStream(mstream.ToArray());
                        mss.Seek(0, SeekOrigin.Begin);
                        //JpegEncoder encoder = new JpegEncoder();
                        //encoder.Encode(img, mstream);
                        //mstream.Seek(0, SeekOrigin.Begin);
                        //XImage.FromStream(ms);
                        XImage pdfImg = null;
                        UIThread.Invoke(() => { pdfImg = XImage.FromStream(mss); });
                        while (pdfImg == null)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                        pdfImg.Interpolate = false;
                        //gfx.DrawImage(pdfImg, 0, 0);
                        gfx.DrawImage(pdfImg, 0, 0, 797, 1123);

                    }
                }
            }
         
			PdfPage page2 = document.AddPage();
            page2.Width = 797;
            page2.Height = 1123;

            XGraphics gfx2 = null;
            UIThread.Invoke(() => {gfx2= XGraphics.FromPdfPage(page2); });
            while (gfx2 == null)
            {
                System.Threading.Thread.Sleep(100);
            }
            using (MemoryStream mstream2 = new MemoryStream())
            {
                ImageTools.ExtendedImage img2 = null;
                UIThread.Invoke(() => { img2 = UitvoerPrijsView.GetImg(SaveNeeded,PrintDatum); });
                while (img2 == null)
                {
                    System.Threading.Thread.Sleep(100);
                }
                
                    img2.DensityX = 900;
                    img2.DensityY = 900;
                    JpegEncoder encoder2 = new JpegEncoder();
                    encoder2.Quality = 100;
                    encoder2.Encode(img2, mstream2);
                    MemoryStream mss2 = new MemoryStream(mstream2.ToArray());
                    mss2.Seek(0, SeekOrigin.Begin);
                    XImage pdfImg2 = null;
                    UIThread.Invoke(() => {pdfImg2 = XImage.FromStream(mss2); });
                    while (pdfImg2 == null)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    pdfImg2.Interpolate = false;
                    gfx2.DrawImage(pdfImg2, 0, 0,797,1123);
                
            }
           
            foreach (var img in bpImgList)
            {
                using (MemoryStream msb = new MemoryStream())
                {
                    PdfPage pageb = document.AddPage();
                    pageb.Width = 797;
                    pageb.Height = 1123;
                    XGraphics gfxb = null;
                    UIThread.Invoke(() => { gfxb = XGraphics.FromPdfPage(pageb); });
                    while (gfxb == null)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    img.DensityX = 300;
                    img.DensityY = 300;
                    JpegEncoder encoderb = new JpegEncoder();
                    encoderb.Encode(img, msb);
                    MemoryStream msb2 = new MemoryStream(msb.ToArray());
                    msb2.Seek(0, SeekOrigin.Begin);
                    XImage pdfImg = null;
                    UIThread.Invoke(() => { pdfImg = XImage.FromStream(msb2); });
                    while (pdfImg == null)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    pdfImg.Interpolate = false;
                    gfxb.DrawImage(pdfImg, 0, 0, 797, 1123);
                }
            }
			_File = document;
            
            /* foreach (var Img in uitvoercreator.MaakPlattegrondUitvoer())
          {
              PdfPage page3 = document.AddPage();
              page3.Width = 797;
              page3.Height = 1123;
              XGraphics gfx3 = XGraphics.FromPdfPage(page3);
              using (MemoryStream mstream3 = new MemoryStream())
              {
                  if (Img != null)
                  {
                      Img.DensityX = 300;
                      Img.DensityY = 300;
                      JpegEncoder encoder = new JpegEncoder();
                      encoder.Encode(Img, mstream3);
                      MemoryStream mss3 = new MemoryStream(mstream3.ToArray());
                      mss3.Seek(0, SeekOrigin.Begin);
                      XImage pdfImg3 = XImage.FromStream(mss3);
                      pdfImg3.Interpolate = false;
                      gfx3.DrawImage(pdfImg3, 0, 0, 797, 1123);

                  }
              }
          }*/
       }
		private void CreateFile()
		{
			//
		
			//string koptekst = "Demo Uitvoer ebrochure";
			
			var kavelinfo = (from k in Acumulator.Instance().ctx.Bouwnummers
							where k.B_ID == Acumulator.Instance().Bouwnr
							select k.Omschrijving).FirstOrDefault().ToString();
			string subkoptekst = kavelinfo;
			
			string hoofdtekst = "";
		
			foreach(Ruimte r in Acumulator.Instance().OTracker.offerteRuimte_.Children)
			{
				hoofdtekst+= r.Omschrijving;
				hoofdtekst+= "Pakket: " + r.GekozenPakket.Omschrijving+" "+ r.GekozenPakket.PrijsHuidig;
				hoofdtekst += "Opties: ";
				foreach(OptieKeuze ok in r.GekozenOpties)
				{
					hoofdtekst+="   "+ok.Omschrijving +" "+ok.BasisPrijs;
				}
				hoofdtekst+= "Producten: "+ Environment.NewLine;
				foreach(Product p in r.GekozenTegels)
				{
					hoofdtekst += "   "+p.Omschrijving + Environment.NewLine;
					var bew = from b in r.GekozenBewerkingen
							where b.LinkedSubcatNr == p.LinkedSubCat
							select b;
					foreach(var b in bew)
					{
						hoofdtekst += " "+b.Omschrijving;
					}
					hoofdtekst+=Environment.NewLine;
				}
			}

		}
		private void Save2File_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog d = new SaveFileDialog();
            var projnaam = (from p in Acumulator.Instance().ctx.PRojects
                            where p.PR_ID == Acumulator.Instance().Projectnr
                            select p.Omschrijving).FirstOrDefault();
            //var projnaam = Acumulator.Instance().ProjFase.Omschrijving;
            var bnr = (from k in Acumulator.Instance().ctx.Bouwnummers
                       where k.B_ID == Acumulator.Instance().Bouwnr
                       select k.Omschrijving).FirstOrDefault().ToString();
            var versienr = "";
            if (SaveNeeded)
            {
                if (Acumulator.Instance().oOL != null) { versienr = "versie_" + (Acumulator.Instance().oOL.VersieFull + 1)+".0"; }
                else
                {
                    versienr = "versie_1.0";
                }
            }
            else
            {
                if (Acumulator.Instance().oOL != null) { versienr = "versie_" + Acumulator.Instance().oOL.VersieFull+".0"; } else { versienr = "versie_1.0"; }
            }
            var filename = projnaam + "_" + bnr + "_" + "Offerte_" + versienr + "_" +PrintDatum.ToString("dd-MM-yyyy_HH_mm_ss") + ".pdf";
            foreach (var letter in InvalidChars)
            {
               filename =  filename.Replace(letter, "_");
            }
            d.DefaultFileName = filename;
			d.Filter = "PDF file format|*.pdf";
			if (d.ShowDialog() == true)
			{
                _File.Save(d.OpenFile());
                SaveOfferte(Isvolledig);   
                //var stream = d.OpenFile();
                //using( ZipOutputStream zipOstream = new ZipOutputStream(stream))
                //{
                //    zipOstream.SetLevel(5);
                //    byte[] buffer;
                //    using (MemoryStream file =new MemoryStream() )
                //    {
                //        _File.Save(file,false);
                        
                //        buffer = new byte[file.Length];
                //        //file.Read(buffer, 0, buffer.Length);
                //        file.Seek(0, SeekOrigin.Begin);
                //        buffer = file.ToArray();
                //    }

                //    ZipEntry entry = new ZipEntry(projnaam + "_" + bnr + "_" + "Offerte_" + versienr + "_" + PrintDatum.ToString("dd-MM-yyyy_HH_mm_ss") + ".pdf");
                    
                //    zipOstream.PutNextEntry(entry);
                //    zipOstream.Write(buffer, 0, buffer.Length);
                //    int counter = 0;

                ///*    foreach (var rp in Acumulator.Instance().HuidigRuimteSetKey)
                //    {
                //        var path = Acumulator.Instance().TekeningBijRuimte[rp.Key][rp.Value];
                //        var data = Acumulator.Instance().Blueprints[path];
                //        var ruimtenaam = (from r in Acumulator.Instance().OTracker.offerteRuimte_.Children
                //                         where r.GetType() == typeof(Ruimte) && (r as Ruimte).RuimteID == rp.Key
                //                         select (r as Ruimte).Omschrijving).FirstOrDefault();
                //        ZipEntry bentry = new ZipEntry(projnaam + "_" + bnr + "_Tekening_" + ruimtenaam + ".jpg");
                //        zipOstream.PutNextEntry(bentry);
                //        buffer = data;
                //        zipOstream.Write(buffer, 0, buffer.Length);
                //    }*/
                //    var slx = new SaveLoadXML();
                //    var curxml = slx.SerializeOfferte(Acumulator.Instance().OTracker.offerteRuimte_);
                //    ZipEntry xentry = new ZipEntry(projnaam + "_" + bnr + "_xmlSave.xml");
                //    zipOstream.PutNextEntry(xentry);
                //    buffer = System.Text.ASCIIEncoding.UTF8.GetBytes(curxml);
                //    zipOstream.Write(buffer, 0, buffer.Length);
                //    /*
                //    foreach (byte[] bs in Acumulator.Instance().Blueprints.Values)
                //    {
                //        counter++;
                //        ZipEntry bentry = new ZipEntry(projnaam + "_" + bnr + "_Tekening_" +counter+ ".jpg");
                //        zipOstream.PutNextEntry(bentry);

                //        buffer = bs;

                //        zipOstream.Write(buffer, 0, buffer.Length);

                //    }*/
                    
                    
                //    zipOstream.Finish();
                //}
                
                //_File.Save(d.OpenFile());
			}        
            
		}
        
       
        /*private void RequestReady(IAsyncResult asyncResult)
        {
            
            var req = asyncResult.AsyncState as HttpWebRequest;
             using(WebResponse wrs = req.EndGetResponse(asyncResult))
             {
                 var path = wrs.ResponseUri.ToString().Replace("https://mybouwapp.nl/Images/Blueprints/","");
                 var foo = wrs.GetResponseStream();
                 var bar = foo.CloneToMemoryStream();
                 var foobar = bar.ToArray();
                FileStreams.Add(foobar);
                 Acumulator.Instance().Blueprints.Add(path,foobar);
             }
        }
      */  
		private void MailFile_Click(object sender, RoutedEventArgs e)
		{

		}

		private void PrintFile_Click(object sender, RoutedEventArgs e)
		{

		}

        private void BW_DoWork(Object sender, DoWorkEventArgs e)
        {
            var lists = e.Argument as List<List<ImageTools.ExtendedImage>>;
            var imgList = lists[0] as List<ImageTools.ExtendedImage>;
           /* foreach(var r in Acumulator.Instance().OTracker.offerteRuimte_.Children)
            {
                int cntr = 0;
                while (Acumulator.Instance().HuidigRuimteSetKey[(r as Ruimte).RuimteID] == "Basis" && cntr < 150)
                {
                    cntr++;
                    System.Threading.Thread.Sleep(100);
                }
            }*/
            List<ImageTools.ExtendedImage> bpImgList;
            if (lists.Count > 1)
            {
               bpImgList = lists[1] as List<ImageTools.ExtendedImage>;
            }
            else
            {
                bpImgList = new List<ImageTools.ExtendedImage>();
            }

             InitializeFinalStage_BW(imgList,bpImgList);
        }
        private void BW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        private void BW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Acumulator.Instance().uitvoerPlaceHolder = new UitvoerPlaceHolder() { uitvoerCreator = _uitvoerCreator, Pdfdoc = _File };
            Acumulator.Instance().BusyBee.IsBusy = false;
        }
        private UitvoerCreator _uitvoerCreator;
        private int ImagesOk()
        {
            var result = 0;
            foreach (Ruimte r in Acumulator.Instance().OTracker.offerteRuimte_.Children)
            {
                r.SwitchBluePrint();
                var waitcntr = 0;
                var status = Acumulator.Instance().bluePrintManager.GetBlueprintStatus(r.RuimteID);
                while (status == BluePrintStatus.Downloading && waitcntr < 30)
                {
                    status = Acumulator.Instance().bluePrintManager.GetBlueprintStatus(r.RuimteID);
                    System.Threading.Thread.Sleep(1000);
                    waitcntr++;
                }
                if (status != BluePrintStatus.Complete && status != BluePrintStatus.Unavailable && status != BluePrintStatus.Failed)
                {
                    result++;
                    var nlist = (List<TvNode>)Acumulator.Instance().BB.treeView1.ItemsSource;

                   var parentnode = nlist[0].Children.FirstOrDefault(x => (x as TvNode).NodeNaam.ToLower() == r.Omschrijving.ToLower());
                   
                   var platnode = parentnode.Children.FirstOrDefault(x => x.NodeNaam.ToLower() == "plategrond");
                   if (platnode != null)
                   {
                       platnode.Status = TvNodeStatus.Incompleet;
                       platnode.ShowCompleet();
                   }
                }
            }
            if (result > 0)
            {
                Dispatcher.BeginInvoke(()=> Acumulator.Instance().BB.treeView1.ExpandAll());
            }
            return result;
        }

		public void newInitPre()
        {
			//check volledig onvolledig
            Acumulator.Instance().BB.ShowHideAllMarks(true, null);
			var aantalIncompleet = (from p in Acumulator.Instance().BB.CheckAllMarks(null)
									where !p.Value
									select p.Key).Count();
            aantalIncompleet += ImagesOk();
			var compleet = Isvolledig = aantalIncompleet == 0;
			var saveXmlAanwezig = Acumulator.Instance().SavedXml != null;
			var memXmlAanwezig = Acumulator.Instance().lastGeneratedXml != null;
			var moetGenereren = true;
			var moetOpslaan = true;
			if (memXmlAanwezig)
				moetGenereren = !XmlMatch(Acumulator.Instance().lastGeneratedXml);
			if (saveXmlAanwezig) {
				moetOpslaan = !XmlMatch(Acumulator.Instance().SavedXml);
				CurrentStatusBox.SetDataSaved();
				CurrentStatusBox.Visibility = System.Windows.Visibility.Visible;
			}
			if (compleet) {
				Save2File.IsEnabled = true;
				ShowPrijsRaport.IsEnabled = true;
				ShowTegelRaport.IsEnabled = true;
			}
			if (moetOpslaan) {
				SaveXml.IsEnabled = true;
				NewStatusBox.SetDataNew(PrintDatum,Isvolledig);
                NewStatusBox.Visibility = Visibility.Visible;
            }
			else {
				SaveXml.IsEnabled = false;
				Acumulator.Instance().BB.ValidateAllMarks(null);
			}

			if ((compleet && moetGenereren) || (compleet && !moetGenereren && Acumulator.Instance().uitvoerPlaceHolder == null) ) {
				_uitvoerCreator = new UitvoerCreator();
				var imgList = _uitvoerCreator.MaakTegelUitvoer(SaveNeeded, PrintDatum);
				List<List<ImageTools.ExtendedImage>> l = new List<List<ImageTools.ExtendedImage>>();
				l.Add(imgList);
				if (Acumulator.Instance().bluePrintManager.GetAllDone()) {
					var bpImgList = _uitvoerCreator.MaakPlattegrondUitvoer(SaveNeeded, PrintDatum);
					l.Add(bpImgList);
				}
				else {
					ImageStaus.Text = "Er moeten nog plaatjes gedownload worden.";
				}
				UitvoerPrijsView.SetPaginaNR(_uitvoerCreator.totalPages,  _uitvoerCreator.pagebreaks + 1);
				BW.RunWorkerAsync(l);
				UitvoerPrijsView.Visibility = Visibility.Collapsed;
				UitvoerView.Visibility = Visibility.Collapsed;
			}
			else if (compleet) {
				_File = Acumulator.Instance().uitvoerPlaceHolder.Pdfdoc;
				_uitvoerCreator = Acumulator.Instance().uitvoerPlaceHolder.uitvoerCreator;
				PrintDatum = Acumulator.Instance().PrevPrintDatum;
				Acumulator.Instance().BusyBee.IsBusy = false;
			}
			else {
				Acumulator.Instance().BusyBee.IsBusy = false;
			}
			if (Acumulator.Instance().HuidigGebruiker.Rol == UserRole.Demo) {
				SaveXml.IsEnabled = false;
			}
		}

		private void InitializeFinalStage_Pre()
        {
            var statuslijst = Acumulator.Instance().BB.CheckAllMarks(null);
            int donecntr = 0;
            foreach (var kvp in statuslijst){
                if (kvp.Value){
                    donecntr++;
                }
            }
            if (donecntr == statuslijst.Count)
            {
                Isvolledig = true;
                Save2File.IsEnabled = true;
                ShowPrijsRaport.IsEnabled = true;
                ShowTegelRaport.IsEnabled = true;
                SaveNeeded = !XmlMatch(null);
                if (SaveNeeded){
                    Acumulator.Instance().uitvoerPlaceHolder = null;
                    SaveXml.IsEnabled = true;
                    NewStatusBox.SetDataNew(PrintDatum,Isvolledig);
                    NewStatusBox.Visibility = Visibility.Visible;
                    if (Acumulator.Instance().oOL != null){
                        CurrentStatusBox.SetDataSaved();
                        CurrentStatusBox.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                else{
                    Acumulator.Instance().BB.ValidateAllMarks(null);
                    CurrentStatusBox.SetDataSaved();
                    CurrentStatusBox.Visibility = System.Windows.Visibility.Visible;
                    SaveXml.IsEnabled = false;
                }
                if (Acumulator.Instance().uitvoerPlaceHolder == null){
                    if (Acumulator.Instance().Donwloadsactive == 0){
                        _uitvoerCreator = new UitvoerCreator();
                        var imgList = _uitvoerCreator.MaakTegelUitvoer(SaveNeeded, PrintDatum);
                        var bpImgList = _uitvoerCreator.MaakPlattegrondUitvoer(SaveNeeded, PrintDatum);
                        List<List<ImageTools.ExtendedImage>> l = new List<List<ImageTools.ExtendedImage>>();
                        l.Add(imgList);
                        l.Add(bpImgList);
                        UitvoerPrijsView.SetPaginaNR(_uitvoerCreator.totalPages, _uitvoerCreator.pagebreaks + 1);
                        BW.RunWorkerAsync(l);
                    }
                    else{
                        _uitvoerCreator = new UitvoerCreator();
                        var imgList = _uitvoerCreator.MaakTegelUitvoer(SaveNeeded, PrintDatum);
                        List<List<ImageTools.ExtendedImage>> l = new List<List<ImageTools.ExtendedImage>>();
                        l.Add(imgList);
                        UitvoerPrijsView.SetPaginaNR(_uitvoerCreator.totalPages, _uitvoerCreator.pagebreaks + 1);
                        BW.RunWorkerAsync(l);
                        ImageStaus.Text = "Er moeten nog plaatjes gedownload worden."; 
                    }
                }
                else if (Acumulator.Instance().uitvoerPlaceHolder != null && Acumulator.Instance().uitvoerPlaceHolder.Pdfdoc != null)
                {
                    _File = Acumulator.Instance().uitvoerPlaceHolder.Pdfdoc;
                    _uitvoerCreator = Acumulator.Instance().uitvoerPlaceHolder.uitvoerCreator;
                    PrintDatum = Acumulator.Instance().PrevPrintDatum;
                    Acumulator.Instance().BusyBee.IsBusy = false;
                }
                UitvoerPrijsView.Visibility = Visibility.Collapsed;
                UitvoerView.Visibility = Visibility.Collapsed;
            }
            else{
                Isvolledig = false;
                NewStatusBox.SetDataNew(PrintDatum, Isvolledig);
                NewStatusBox.Visibility = Visibility.Visible;
                if (Acumulator.Instance().oOL != null){
                    CurrentStatusBox.SetDataSaved();
                    CurrentStatusBox.Visibility = System.Windows.Visibility.Visible;
                }
                Acumulator.Instance().BusyBee.IsBusy = false;
            }
            if (Acumulator.Instance().HuidigGebruiker.Rol == UserRole.Demo){
                SaveXml.IsEnabled = false;
            }
        
        }
        private void InitializeFinalStage_BW(List<ImageTools.ExtendedImage> imgList, List<ImageTools.ExtendedImage> bpImgList)
        {
            MakePdf(imgList, bpImgList);
           
        }
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{

		  
		}

        public void downloadChanged(Dictionary<string, int> dls)
        {
           /* int cntr = 0;
            foreach (var ruimtedls in dls)
            {
                if(ruimtedls.Value > 0)
                {
                    cntr++;
                }
            }
            if (cntr == 0)
            {
                Acumulator.Instance().BusyBee.BusyContent = "Uitvoer genereren";
                Acumulator.Instance().BusyBee.IsBusy = true;
                _uitvoerCreator = new UitvoerCreator();
                var imgList = _uitvoerCreator.MaakTegelUitvoer();
                var bpImgList = _uitvoerCreator.MaakPlattegrondUitvoer();
                List<List<ImageTools.ExtendedImage>> l = new List<List<ImageTools.ExtendedImage>>();
                l.Add(imgList);
                l.Add(bpImgList);

                UitvoerPrijsView.SetPaginaNR(_uitvoerCreator.totalPages, _uitvoerCreator.pagebreaks + 1);
                BW.RunWorkerAsync(l);
                Save2File.IsEnabled = true;
            }
            else if(!BW.IsBusy)
            {
                /*
                Acumulator.Instance().BusyBee.IsBusy = false;
                //Save2File.IsEnabled = false;
                ImageStaus.Text = "Er moeten nog " + cntr + "plaatjes gedownload worden.";
                Acumulator.Instance().BusyBee.BusyContent = "Uitvoer genereren";
                Acumulator.Instance().BusyBee.IsBusy = true;
                _uitvoerCreator = new UitvoerCreator();
                var imgList = _uitvoerCreator.MaakTegelUitvoer();
                var bpImgList = new List<ImageTools.ExtendedImage>();
                List<List<ImageTools.ExtendedImage>> l = new List<List<ImageTools.ExtendedImage>>();
                l.Add(imgList);
                l.Add(bpImgList);

                UitvoerPrijsView.SetPaginaNR(_uitvoerCreator.totalPages, _uitvoerCreator.pagebreaks + 1);
                BW.RunWorkerAsync(l);
                Save2File.IsEnabled = true;
            }*/
        }
       

		private void ShowTegelRaport_Click(object sender, RoutedEventArgs e)
		{
			var dw = new DetailsWindow();
			var uitvoercreator = new UitvoerCreator();
			dw.Width = 0;
			dw.Height = 0;
			foreach (var u in uitvoercreator.MaakCanvasTegelRaport(SaveNeeded,PrintDatum))
			{
				dw.LoadContent(u);
				dw.Width = u.Width + 20;
				dw.Height += 400;
			}
            //var uv = new Uitvoer();
			//uv.GetImg();
			//dw.Width = uv.Width + 20;
			//dw.Height = uv.Height;
			//dw.LoadContent(uv);
			dw.Show();
		}

		private void ShowPrijsRaport_Click(object sender, RoutedEventArgs e)
		{
			var dw = new DetailsWindow();
            var uv = new ExpertUitvoerPrijs();
			uv.GetImg(SaveNeeded,PrintDatum);
			dw.Height = 800;
			dw.Width = uv.Width + 20;
			dw.LoadContent(uv);
			dw.Show();
		}
		private void SaveOfferte(bool volledig)
        {
            if (Acumulator.Instance().HuidigGebruiker.Rol != UserRole.Demo && SaveNeeded)
            {
                //check for changes
                SaveLoadXML slx = new SaveLoadXML();
                slx.SaveOfferte(Acumulator.Instance().OTracker.offerteRuimte_,volledig,this);
                LogHelper.SendLog("Offerte Saved by " + Acumulator.Instance().HuidigGebruiker.GebruikersNaam + " - Bnr: " + Acumulator.Instance().Bouwnr, LogType.activity);
                SaveNeeded = false;
                SaveXml.IsEnabled = false;
                //MessageBox.Show("Offerte Succesvol Opgeslagen");
                //MakePdf();
                NewStatusBox.Visibility = System.Windows.Visibility.Collapsed;
                Acumulator.Instance().BB.ValidateAllMarks(null);
            }
            
		}
		private void TestXML_Click(object sender, RoutedEventArgs e)
		{
			SaveOfferte(Isvolledig);
			
		}

        private void ResetSave_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show("Alle saves van dit bouwnummer verwijderen?", "Alles verwijderen?", MessageBoxButton.OKCancel);
            if (res == MessageBoxResult.OK)
            {
                SaveLoadXML slx = new SaveLoadXML();
                slx.RemoveSave();
                MessageBox.Show("Succesvol verwijderd");
                SaveNeeded = true;
                if (Acumulator.Instance().HuidigGebruiker.Rol != UserRole.Demo)
                {
                    SaveXml.IsEnabled = true;
                }
               LogHelper.SendLog("Save Reset bnr: "+Acumulator.Instance().Bouwnr+ " by user : "+Acumulator.Instance().HuidigGebruiker.GebruikersNaam,LogType.activity);
            }
        }

        private void OffAccoord_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void OffAccoord_Unchecked(object sender, RoutedEventArgs e)
        {

        }
		private bool XmlMatch(string matchwidth)
        {
            var retval = false;
            if(Acumulator.Instance().SavedXml == null)
            {
                retval = false;
            }
            else
            {
				var cntr = 0;
			Start:
				if (cntr < 3) {
					cntr++;
					try {
						SaveLoadXML slx = new SaveLoadXML();
                        foreach (var r in Acumulator.Instance().OTracker.offerteRuimte_.Children.Where(c => c.GetType() == typeof(Ruimte)))
                        {
                            (r as Ruimte).GetMetersAdjusted((r as Ruimte).GetSaldoMeters());
                            (r as Ruimte).GetSaldoMetersAccent();
                            (r as Ruimte).GetSaldoMetersHoek();
                            
                        }
						var curxml = slx.SerializeOfferte(Acumulator.Instance().OTracker.offerteRuimte_);
						curxml = NormalizeXml(curxml);
						Acumulator.Instance().lastGeneratedXml = curxml;
						string savedxml;
						if (matchwidth != null) {
							savedxml = matchwidth;
						}
						else {
							savedxml = Acumulator.Instance().SavedXml;
						}
						savedxml = NormalizeXml(savedxml);
						//   retval = String.Equals(curxml, savedxml, StringComparison.Ordinal);  
						for (int i = 0; i < curxml.Length; i++) {
							if (curxml[i] != savedxml[i]) {
								var totc = curxml.Substring(i);
								var tots = savedxml.Substring(i);
								break;
							}
						}
						var hash_curxml = CalculateMD5Hash(curxml);
						var hash_savedxml = CalculateMD5Hash(savedxml);
						retval = String.Equals(hash_curxml, hash_savedxml, StringComparison.Ordinal);
					}
					catch (Exception e) {
						LogHelper.SendLog(e.Message, LogType.error);
						goto Start;
					}
				}
            }


            return retval;

        }
        private string NormalizeXml(string input)
        {
            if (input.StartsWith(@"<?xml"))
            {
                input = input.Remove(0, 40);

            }
            input = input.Replace("\r", "");
            input = input.Replace("\n", "");
            input = input.Replace(">  <", "><");
            input = input.Replace(">   <", "><");
            input = input.Replace(">    <", "><");
            input = input.Replace(">     <", "><");
            input = input.Replace(">      <", "><");
            input = input.Replace(">       <", "><");
            input = input.Replace(">        <", "><");
            input = input.Replace(">         <", "><");

            return input;
        }
        private string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5Managed md5 = new MD5Managed();
            byte[] inputBytes = System.Text.ASCIIEncoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        private void ShowTekeningBtn_Click_1(object sender, RoutedEventArgs e)
        {
            var dw = new DetailsWindow();
            dw.Height = 450;
            dw.Width = 600;
            foreach (Ruimte r in Acumulator.Instance().OTracker.offerteRuimte_.Children.Where(x => x.GetType() == typeof(Ruimte)))
            {
               /* if (Acumulator.Instance().HuidigRuimteSetKey.ContainsKey(r.RuimteID) && Acumulator.Instance().TekeningBijRuimte.ContainsKey(r.RuimteID) && Acumulator.Instance().TekeningBijRuimte[r.RuimteID].ContainsKey(Acumulator.Instance().HuidigRuimteSetKey[r.RuimteID]))
                {
                    var imgpath = Acumulator.Instance().TekeningBijRuimte[r.RuimteID][Acumulator.Instance().HuidigRuimteSetKey[r.RuimteID]];
                    if(Acumulator.Instance().Blueprints.ContainsKey(imgpath))
                    {
                       
                        bp.LoadImg(Acumulator.Instance().Blueprints[imgpath]);
						
                    }
                    
                }*/
                var data = Acumulator.Instance().bluePrintManager.getBlueprintData(r.RuimteID, false);
				if (data != null) {
					var bp = new BluePrintControl();
					bp.LoadImg(data);
					dw.LoadContent(bp);
				}
            }
            dw.Show();
        }

        private void Adjust_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
           
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                double newtotal;
                var ifbar = Acumulator.Instance().InfoBar;

                newtotal = ifbar.Bon.GetTotalD();//totaalprijs.Text.TrimStart('€'),out newtotal);
                double oldtotal;
                var ool = Acumulator.Instance().oOL;
                double.TryParse(ool.Prijs.TrimStart('€'), out oldtotal);
                double oldkort = 0.0;
                var otrack = Acumulator.Instance().OTracker;
                if (otrack.offerteRuimte_.Korting != null)
                {
                    oldkort = otrack.offerteRuimte_.Korting.KortingBedrag;
                }
                var dw = new DetailsWindow();
                dw.Width = 500;
                dw.Height = 500;
                dw.Closed += dw_Closed;
                dw.LoadContent(new DiscountAuthorizer(newtotal,oldtotal,oldkort,dw));
                dw.Show();
            }
         
        }

        void dw_Closed(object sender, EventArgs e)
        {
            Acumulator.Instance().ctx.GetServerTime(operation =>
            {
                PrintDatum = operation.Value;
                Acumulator.Instance().BusyBee.BusyContent = "Uitvoer Herberekenen...";
                Acumulator.Instance().BusyBee.IsBusy = true;
                if (Acumulator.Instance().HuidigGebruiker.Rol == UserRole.Admin || Acumulator.Instance().HuidigGebruiker.Rol == UserRole.Adviseur)
                {
                    ResetSave.Visibility = Visibility.Visible;
                }
                //ImageTools.IO.Encoders.AddEncoder<BmpEncoder>();
                ImageTools.IO.Encoders.AddEncoder<JpegEncoder>();
                //CreateZip();
                //ImageTools.IO.Encoders.AddEncoder<PngEncoder>();
                BW = new BackgroundWorker();
                BW.DoWork += new DoWorkEventHandler(BW_DoWork);
                BW.ProgressChanged += new ProgressChangedEventHandler(BW_ProgressChanged);
                BW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BW_RunWorkerCompleted);
                newInitPre();//InitializeFinalStage_Pre();
            }, null);
        }

      

       
	}
}

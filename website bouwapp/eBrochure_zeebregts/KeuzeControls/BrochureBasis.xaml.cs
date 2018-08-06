using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using eBrochure_zeebregts.Classes;
using eBrochure_zeebregts.KeuzeControls;
using eBrochure_zeebregts.Helpers;
using System.ComponentModel;
using eBrochure_zeebregts.ExpertControls;
using eBrochure_zeebregts.ExpertControls.UiElements;


namespace eBrochure_zeebregts
{
    enum SkipType { normaal, accent,hoek,bewerking };
	public partial class BrochureBasis : UserControl
	{
		public SubPage SubPagina;
		private StartPagina StartPage;
		private int RuimteTrackNR = -1;
		public BrochureBasis(StartPagina sp)
		{
			InitializeComponent();
			StartPage = sp;
			Acumulator.Instance().BB = this;
		//	Acumulator.Instance().Projectnr = "PR1";
			Acumulator.Instance().BusyBee = this.BusyIndie;
			
		}
		public void LoadSubPage(SubPage Sub)
		{
			KeuzePanel.Children.Clear();
			KeuzePanel.Children.Add(Sub);
			SubPagina = Sub;
		}
		public void load_tree()
		{
			treeView1.Items.Clear();
			/*treeView1.Items.Add*/
			Acumulator.Instance().OTracker.offerteBasis_.MakeTree(null);
			//(treeView1.ItemsSource as List<TvNode>).Add(new TvNode("Tot slot", "FinalStage",null,null));
			treeView1.UpdateLayout();
			var proninf=(from p in Acumulator.Instance().ctx.PRojects
							join b in Acumulator.Instance().ctx.Bouwnummers on p.PR_ID equals b.PR_NR
								where b.B_ID== Acumulator.Instance().Bouwnr
							 select p.Omschrijving).FirstOrDefault();
			TBProjInfo.Text = "Project: " + proninf;
			startTree();
			//this.Dispatcher.BeginInvoke(()=>startTree());
			
		}
		private void startTree()
		{
			var item = (treeView1.ItemsSource as List<TvNode>).First();
			var tvi = treeView1.ContainerFromItem(item);
			treeView1.UpdateLayout();
			tvi.Focus();
			tvi.IsSelected = true;
		//	boom_next(null);
		
		}
		public void GeenInputBouwnummer()
		{
			var OT = new OfferteTracker();
			OT.NieuwOfferte();
		}
		public void boom_back(TvNode _tvi)
		{
			TreeViewItem tvi = (TreeViewItem)treeView1.ContainerFromItem(_tvi);

			var PapaNode = (tvi.Header as TvNode).Parent as TvNode;
			if (PapaNode == null)
			{
				if ((tvi.Header as TvNode).TypeHandle.ToString() != "RuimteOfferte")
				{
					var basis = treeView1.ItemsSource as List<TvNode>;
					var indx = basis.IndexOf(tvi.Header as TvNode) - 1;
					var thenode = treeView1.ContainerFromItem(basis[indx].Children.Last().Children.Last());
					
					if (basis[indx].Children.Last().Children.Last().Status == TvNodeStatus.Incompleet)
					{
						boom_back(basis[indx].Children.Last().Children.Last());
					}
					else
					{
						treeView1.UpdateLayout();
						thenode.Focus();
						thenode.IsSelected = true;
						thenode.IsExpanded = true;
					}
					//van tot slot terug naar laatste ruimte laatste pagina?
					/*var node = (TreeView)tvi.Parent;
					var anode = node.Items[node.Items.IndexOf(tvi) - 1] as TreeViewItem;
					var pnode = (anode.Items[anode.Items.Count - 1] as TreeViewItem);
					if (pnode.Items.Count > 0)
					{
						var cnode = pnode.Items[pnode.Items.Count - 1] as TreeViewItem;
						treeView1.UpdateLayout();
						cnode.Focus();
						cnode.IsSelected = true;
						cnode.IsExpanded = true;
					}*/
				}
			}
			else if (PapaNode.Children.IndexOf(tvi.Header as TvNode) > 0 && (tvi.Header as TvNode).Children == null)
			{
				int indx = PapaNode.Children.IndexOf(tvi.Header as TvNode) - 1;

				var node = treeView1.ContainerFromItem(PapaNode.Children[indx]);
				//var node = (TreeViewItem)PapaNode.Items[PapaNode.Items.IndexOf(tvi) - 1];
					var tv_node = PapaNode.Children[indx];
					if ((tv_node.Children == null || tv_node.Children.Count < 1) && tv_node.Status == TvNodeStatus.Incompleet )
					{
						boom_back(tv_node);
					}
					else
					{
						treeView1.UpdateLayout();
						node.Focus();
						node.IsSelected = true;
						node.IsExpanded = true;
				
					}
			}
			else if (((tvi.Header as TvNode).Parent as TvNode).Children.IndexOf(tvi.Header as TvNode) == 0)//((tvi.Parent as TreeViewItem).Items.IndexOf(tvi) == 0)
			{
				var node = treeView1.ContainerFromItem(PapaNode);//(TreeViewItem)tvi.Parent;
				treeView1.UpdateLayout();
				node.Focus();
				node.IsSelected = true;
				node.IsExpanded = true;
			}
			else
			{
				//var node = (TreeViewItem)tvi.Parent;
				//var pnode = node.Items[node.Items.IndexOf(tvi) - 1] as TreeViewItem;

				var indx = PapaNode.Children.IndexOf(tvi.Header as TvNode) - 1;
				var pnode = treeView1.ContainerFromItem(PapaNode.Children[indx]);
				if ((PapaNode.Children[indx] as TvNode).Children.Count >0)//(pnode.Items.Count > 0)
				{
					var cnode = treeView1.ContainerFromItem(PapaNode.Children[indx].Children.Last());
					//var cnode = pnode.Items[pnode.Items.Count - 1] as TreeViewItem;
					var tv_node = PapaNode.Children[indx].Children.Last();
					if ((tv_node.Children == null || tv_node.Children.Count < 1) && tv_node.Status == TvNodeStatus.Incompleet)
					{
						boom_back(tv_node);
					}
					else
					{
						treeView1.UpdateLayout();
						cnode.Focus();
						cnode.IsSelected = true;
						cnode.IsExpanded = true;
					}

				}
			}
		}
		public void boom_next(TvNode _tvi)
		{
			TreeViewItem tvi = (TreeViewItem)treeView1.ContainerFromItem(_tvi);
			if (tvi == null )
			{
				if (treeView1.Items.Count > 0)
				{
					tvi = (TreeViewItem)treeView1.ItemContainerGenerator.ContainerFromIndex(0);
					
				}
				else
				{
					return;
				}
			}
			tvi.IsExpanded = true;
			if (tvi.Items.Count > 0)
			{
				treeView1.UpdateLayout();
				TreeViewItem tnode = treeView1.ContainerFromItem(tvi.Items[0]);
				
				tnode.Focus();
				tnode.IsSelected = true;
				tnode.IsExpanded = true;
			
				//TreeViewItem tnode = (TreeViewItem)tvi.ItemContainerGenerator.ContainerFromIndex(0);
				
				//check of current node nu ook de nieuwe node is
				
			}
			else if ((tvi.Header as TvNode).Parent.Children.Count > 1 && (tvi.Header as TvNode).Parent.Children.IndexOf(tvi.Header as TvNode) + 1 <= (tvi.Header as TvNode).Parent.Children.Count - 1)//else if ((tvi.Parent as TreeViewItem).Items.Count > 1 && ((tvi.Parent as TreeViewItem).Items.IndexOf(tvi) + 1 <= (tvi.Parent as TreeViewItem).Items.Count - 1))
			{
				var tvi_parent = treeView1.ContainerFromItem((tvi.Header as TvNode).Parent);
				int indx = (tvi_parent.Header as TvNode).Children.IndexOf(tvi.Header as TvNode) +1;
				//int idx = tvi_parent.Items.IndexOf(tvi) + 1;
				if (indx <= tvi_parent.Items.Count -1)
				{
					var node = treeView1.ContainerFromItem((tvi_parent.Header as TvNode).Children[indx]);
					node.Focus();
					node.IsSelected = true;
					node.IsExpanded = true;
					//(tvi_parent.Items[idx] as TreeViewItem).IsSelected = true;
				}

			}
			else if((tvi.Header as TvNode).Parent.Children.Count - 1 == (tvi.Header as TvNode).Parent.Children.IndexOf(tvi.Header as TvNode)) //((tvi.Parent as TreeViewItem).Items.IndexOf(tvi) == (tvi.Parent as TreeViewItem).Items.Count - 1)
			{
				//bool stop = false;
				if(((tvi.Header as TvNode).Parent as TvNode).Parent.Children.IndexOf((tvi.Header as TvNode).Parent) == (((tvi.Header as TvNode).Parent as TvNode).Parent.Children.Count-1))//(((tvi.Parent as TreeViewItem).Parent as TreeViewItem).Items.IndexOf((tvi.Parent as TreeViewItem)) == ((tvi.Parent as TreeViewItem).Parent as TreeViewItem).Items.Count - 1)
				{
					/*Ga naar tot slot node
					var node = treeView1.Items[1] as TreeViewItem;
					;*/
					var basis = treeView1.ItemsSource as List<TvNode>;
					var node = treeView1.ContainerFromItem(basis.Last());
					treeView1.UpdateLayout();
					node.Focus();
					node.IsSelected = true;
					node.IsExpanded = true;
				}
				else
				{
					var tpar = (tvi.Header as TvNode).Parent as TvNode;
					int indx = (tpar.Parent as TvNode).Children.IndexOf(tpar) + 1;
					var _n = treeView1.ContainerFromItem((tpar.Parent as TvNode).Children[indx]) as TreeViewItem;
					treeView1.UpdateLayout();
					_n.Focus();
					_n.IsSelected = true;
					_n.IsExpanded = true;
					/*foreach (TreeViewItem _n in ((tvi.Parent as TreeViewItem).Parent as TreeViewItem).Items)
					{

						if (stop)
						{
							treeView1.UpdateLayout();
							_n.Focus();
							_n.IsSelected = true;
							_n.IsExpanded = true;
							break;
						}
						else
						{
							if (_n == tvi.Parent)
							{
								stop = true;
							}
						}
					}*/
				}
			}
			
		}
		public void InvalidateNaWijzig()
		{
			var node = treeView1.SelectedItem as TvNode;
            if (node.Parent != null)
            {
                int indx = node.Parent.Children.IndexOf(node);
                foreach (TvNode tnode in node.Parent.Children)
                {
                    if (node.Parent.Children.IndexOf(tnode) >= indx)
                    {
                        tnode.Status = TvNodeStatus.Incompleet;
                        tnode.HideCompleet();
                    }
                }
            }
		}
		public bool IsCurrentComplete()
		{
			return internalCompleet((treeView1.SelectedItem as TvNode));
		}
		private void UpdateBon()
		{
			if (Acumulator.Instance().InfoBar != null)
			{
				Acumulator.Instance().InfoBar.UpdateInfo();
			}
		}
		private void SubmitBtn_Click(object sender, RoutedEventArgs e)
		{
			bool AllSubmited = true;
			if (treeView1.Items.Count < 1)
				{
					if ((KeuzePanel.Children[0] as IBaseControl).SubmitPressed())
					{
						var OT = new OfferteTracker();
						/*treeView1.Items.Add(*/
						OT.NieuwOfferte();
						//Current_item = (TreeViewItem)treeView1.Items[0];
					}
					else
					{ AllSubmited = false; }

				}
				else
				{
					var subkc = SubPagina.SubContPanel.Children.Where(c => c.GetType() == typeof(SubCatkControl)).FirstOrDefault() as SubCatkControl;
					if (subkc != null)
					{
						subkc.Clear4Submit();
					}
                    var advancedAccent = SubPagina.SubContPanel.Children.Where(c => c.GetType() == typeof(AdvancedTegelSubCatPage)).FirstOrDefault() as AdvancedTegelSubCatPage;
                    if (advancedAccent != null)
                    {
                        advancedAccent.Clear4Submit();
                    }

                	foreach (var item in SubPagina.SubContPanel.Children)
					{
						try
						{
							if (!(item as IBaseControl).SubmitPressed())
							{
								AllSubmited = false;
							}
						}
						catch (NullReferenceException nre)
						{
							// no fail
						}
					}
					if (AllSubmited)
					{
						(treeView1.SelectedItem as TvNode).Status = TvNodeStatus.InvalidCompleet;
						(treeView1.SelectedItem as TvNode).ShowCompleet();
						boom_next((TvNode)treeView1.SelectedItem);
						SubmitFeedbackTB.Text = "";
						SubmitFeedbackTB.Visibility = Visibility.Collapsed;
					}
					else
					{
						(treeView1.SelectedItem as TvNode).Status = TvNodeStatus.Incompleet;
						(treeView1.SelectedItem as TvNode).HideCompleet();
						if ((treeView1.SelectedItem as TvNode).TypeHandle.ToString() != "RuimteOpties_pre")
						{
							SubmitFeedbackTB.Text = "U heeft niet overal een keuze gemaakt. Controleer de pagina en probeer opnieuw.";
							SubmitFeedbackTB.Visibility = Visibility.Visible;
						}
					}

				}

			UpdateBon();
			
			
		}
		private void getRuimteTrackNr(TvNode tvi)
		{
			int RuimteTnr = RuimteTrackNR;
			string head = "";
            string Rid = "";
			switch(tvi.TypeHandle.ToString())
			{
				case "Ruimte":
					head = tvi.NodeNaam.ToString();
                    Rid = tvi.NodeID;
					break;
				case "PakketKeuze":
				case "RuimteOpties_pre":
				case "SubCategoriën":
				case "SubCategoriën_na":
				case "SubCategoriën_accent":
				case "SubCategoriën_hoek":
				case "BluePrintPage":
					head = (tvi.Parent as TvNode).NodeNaam.ToString();
                    Rid = tvi.NodeID;
					break;
			}
			var offRuimte = Acumulator.Instance().OTracker.offerteRuimte_;
            var ruimte = (from r in offRuimte.Children
					   where r.GetType() == typeof(Ruimte) && (r as Ruimte).RuimteID == Rid
					   select r).FirstOrDefault();
            RuimteTnr = offRuimte.Children.IndexOf(ruimte);
			RuimteTrackNR = RuimteTnr;
			
		}
        public Ruimte HuidigRuimte;
        
        private bool SkipNode(SkipType type)
        {
            var retval = true;
            var checkmeters = false;
            if (HuidigRuimte != null)
            {
                var saldi = new Dictionary<string, double>();
                switch (type)
                {
                    case SkipType.normaal:
                        saldi = HuidigRuimte.GetSaldoMeters();
                        checkmeters = true;
                        break;
                    case SkipType.accent:
                        if (HuidigRuimte.qrySubCatAccent(Acumulator.Instance().ctx,HuidigRuimte.GekozenPakket).Count > 0)
                        {
                            retval = false;
                        }
                        else
                        {
                            retval = true;
                        }
                        break;
                    case SkipType.hoek:
                        saldi = HuidigRuimte.GetSaldoMetersHoek();
                        break;
                    case SkipType.bewerking:
                        if ((HuidigRuimte.GekozenTegels.Count + HuidigRuimte.GekozenAccenten.Count) > 0)
                        {
                            retval = false;
                        }
                        else
                        {
                            retval= true;
                        }
                        break;
                }
                if (checkmeters)
                {
                    var totaalmeters = 0.0;
                    foreach (var saldo in saldi)
                    {
                        if (saldo.Key != "SCB4")
                        {
                            totaalmeters += saldo.Value;
                        }
                    }
                    if (totaalmeters > 0)
                    {
                        retval = false;
                    }
                    else
                    {
                        retval = true;
                    }
                }
            }
            else
            {
                retval = false;
            }
            

            return retval;
        }
        public void ReloadImage()
        {
            try
                {
					var bpctrl = new BluePrintControl();
                    var data = Acumulator.Instance().bluePrintManager.getBlueprintData(HuidigRuimte.RuimteID, false);
					if (data != null)
						bpctrl.LoadImg(data);
                  
				/*var rskey = Acumulator.Instance().HuidigRuimteSetKey[HuidigRuimte.RuimteID];
                    var rsk_path = Acumulator.Instance().TekeningBijRuimte[HuidigRuimte.RuimteID][rskey];
                    
                    if (Acumulator.Instance().Blueprints.ContainsKey(rsk_path))
                    {
                        bpctrl.LoadImg(Acumulator.Instance().Blueprints[rsk_path]);
                    }
                    else
                    {
                        string path = Acumulator.Instance().GetPathGespiegeld(HuidigRuimte.RuimteID);
                        bpctrl.LoadImg(path);
                    }*/
					
				BluePrintControl bcntrol = (from bp in SubPagina.SubContPanel.Children
                                   where bp.GetType() == typeof(BluePrintControl)
                                   select bp).FirstOrDefault() as BluePrintControl;
                              if(bcntrol != null)
                              {
                                  SubPagina.SubContPanel.Children.Clear();
                                  SubPagina.SubContPanel.Children.Add(bpctrl);
                              }
                             
                }
                catch (Exception ex)
                {
                    LogHelper.SendLog(ex.Message, LogType.error);
                }
            
                        
        }
        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
            if ((treeView1.SelectedItem as TvNode).TypeHandle.ToString() != "FinalStage")
            {
                Acumulator.Instance().FSdelegate = null;
            }
			TerugBtn.Visibility = Visibility.Visible;
			VerderBtn.Visibility = Visibility.Visible;
			SubmitFeedbackTB.Text = "";
			StartPage.Scroll2Top();
			TvNode tvItem = treeView1.SelectedItem as TvNode;
			tvItem.Enabled = true;
			WijzigBtn.Visibility = Visibility.Collapsed;
			Acumulator.Instance().SelectedTVItem = tvItem;
			if (tvItem != null)
			{
				getRuimteTrackNr(tvItem);
			}
			HuidigRuimte = new Ruimte("", "", 0);

			if (RuimteTrackNR > -1)
			{
				HuidigRuimte = Acumulator.Instance().OTracker.offerteRuimte_.Children[RuimteTrackNR] as Ruimte;
				var ctx = Acumulator.Instance().ctx;

                string path = Acumulator.Instance().GetPathGespiegeld(HuidigRuimte.RuimteID);
                /*var spiegeld = (from bnr in ctx.Bouwnummers
                                where bnr.B_ID == Acumulator.Instance().Bouwnr
                                select bnr.Gespiegeld).FirstOrDefault();
                if (spiegeld != null && spiegeld == true)
                {
                    path = (from br in ctx.Bron_Ruimtes
                            where br.R_NR == HuidigRuimte.RuimteID
                            select br.tekeningPath_Spiegel).FirstOrDefault();
                }
                else
                {
                    path = (from br in ctx.Bron_Ruimtes
                    where br.R_NR == HuidigRuimte.RuimteID
                    select br.tekeningPath).FirstOrDefault();
                }*/
				/*
				if(Acumulator.Instance().HuidigRuimteSetKey.ContainsKey(HuidigRuimte.RuimteID) && Acumulator.Instance().TekeningBijRuimte.ContainsKey(HuidigRuimte.RuimteID) && Acumulator.Instance().TekeningBijRuimte[HuidigRuimte.RuimteID].ContainsKey(Acumulator.Instance().HuidigRuimteSetKey[HuidigRuimte.RuimteID]))
                {
                    var dpath = Acumulator.Instance().TekeningBijRuimte[HuidigRuimte.RuimteID][Acumulator.Instance().HuidigRuimteSetKey[HuidigRuimte.RuimteID]];
                    if(Acumulator.Instance().Blueprints.ContainsKey(dpath))
                    {
                        Acumulator.Instance().InfoBar.LoadImg(dpath);
                    }
                    else
                    {
                        Acumulator.Instance().InfoBar.LoadImg(path);
                    }
                }
                else
                {
                    Acumulator.Instance().InfoBar.LoadImg(path);
                }
				*/
				Acumulator.Instance().InfoBar.SetRuimteID(HuidigRuimte.RuimteID);
			}
			if (tvItem.Children == null || tvItem.Children.Count < 1)
			{
				if (IsCurrentComplete() == false)
				{
					VerderBtn.Visibility = Visibility.Collapsed;
					SubmitBtn.Visibility = Visibility.Visible;
				}
				else
				{
					SubmitBtn.Visibility = Visibility.Collapsed;
				}
			}
			else
			{
				SubmitBtn.Visibility = Visibility.Collapsed;
			}
			if (treeView1.SelectedItem != null)
			{
				SubPagina.SubContPanel.Children.Clear();
				ShowHideAllMarks(false, null);
				switch ((treeView1.SelectedItem as TvNode).TypeHandle.ToString())
				{

					case "KavelOfferte":
						SubPagina.Titel = "Gegevens";
						SubPagina.SubContPanel.Children.Add(new TextBlock() { Text = "Controleer uw Gegevens \n Naam Koper: \n Bouwnummer: " + Acumulator.Instance().Bouwnr + "\n En klik op Verder." });
						break;
					case "RuimteOfferte":
						
						TerugBtn.Visibility = Visibility.Collapsed;
						SubPagina.Titel = tvItem.NodeNaam;
                        var kg = new KopersGegevens();
                        SubPagina.SubContPanel.Children.Add(kg);
                        /*var subkc = new SubCatkControl(null, MainCatType.normaal);
						subkc.InitSubs(null, "info", HuidigRuimte, SubPagina, null);
						var kavelinfo = (from k in Acumulator.Instance().ctx.Bouwnummers
										 where k.B_ID == Acumulator.Instance().Bouwnr
										 select k.Omschrijving).FirstOrDefault().ToString();
                        var proj = (from p in Acumulator.Instance().ctx.PRojects
                                    join b in Acumulator.Instance().ctx.Bouwnummers on p.PR_ID equals b.PR_NR
                                    where b.B_ID == Acumulator.Instance().Bouwnr
                                    select p.Omschrijving).FirstOrDefault();
                        var koper = (from b in Acumulator.Instance().ctx.Bouwnummers
                                         where b.B_ID == Acumulator.Instance().Bouwnr
                                         select b).FirstOrDefault();
                        subkc.SubCatPanel.Children.Add(new TextBlock() { Text = "Controleer uw Gegevens \n\n", FontWeight= FontWeights.Bold});
                        subkc.SubCatPanel.Children.Add(new TextBlock() { Text = "Naam Koper: " + koper.KlantNaam});
                        subkc.SubCatPanel.Children.Add(new TextBlock() { Text = "Straat: " + koper.adres_straat});
                        subkc.SubCatPanel.Children.Add(new TextBlock() { Text = "Plaats: " + koper.adres_plaats + "\n\n" });
                        subkc.SubCatPanel.Children.Add(new TextBlock() { Text = "Project:" + proj});
                        subkc.SubCatPanel.Children.Add(new TextBlock() { Text = "Bouwnummer: " + kavelinfo + "\n\n" });
                        subkc.SubCatPanel.Children.Add(new TextBlock() { Text = "Aanwezige Ruimtes:", FontWeight = FontWeights.Bold });
                        foreach (var r in Acumulator.Instance().OTracker.offerteRuimte_.Children)
						{
							var ru = new TextBlock();
							ru.Text = "-	" + (r as Ruimte).Omschrijving;
							subkc.SubCatPanel.Children.Add(ru);
						}
                         SubPagina.SubContPanel.Children.Add(subkc);
                         */
						Acumulator.Instance().InfoBar.UpdateInfo();
                       
						break;
					case "Ruimte":
						SubPagina.Titel = tvItem.NodeNaam.ToString();
						treeView1.UpdateLayout();
                        try
                            {
                                var bpctrl = new BluePrintControl();
                                var data = Acumulator.Instance().bluePrintManager.getBlueprintData(HuidigRuimte.RuimteID, true);
                                if (data != null)
                                {
                                    bpctrl.LoadImg(data);
                                    SubPagina.SubContPanel.Children.Add(bpctrl);
                                }
                                else
                                {
                                    SubPagina.SubContPanel.Children.Add(new TextBlock()
                                        {
                                            Text = "Het downloaden van de plattegrond duurt langer dan verwacht. \n Ga door met het invullen van de offerte",
                                            Margin = new Thickness(20, 0, 0, 0),
                                            FontFamily = new System.Windows.Media.FontFamily("Lucida Grande"),
                                            FontSize = 12
                                        });
                                }
                                
                               

                                //if (bpstatus == BluePrintStatus.Complete)
                                //{
                                //    var bpctrl = new BluePrintControl();
                                //    var data = Acumulator.Instance().bluePrintManager.getBlueprintData(HuidigRuimte.RuimteID);
                                //    if (data != null)
                                //        bpctrl.LoadImg(data);
                                //    SubPagina.SubContPanel.Children.Add(bpctrl);
                                //}
                                //else if (bpstatus == BluePrintStatus.Downloading)
                                //{
                                //    var retrycounter = 0;
                                //    while (bpstatus == BluePrintStatus.Downloading && retrycounter < 15)
                                //    {
                                //        System.Threading.Thread.Sleep(1000);
                                //        bpstatus = Acumulator.Instance().bluePrintManager.GetBlueprintStatus(HuidigRuimte.RuimteID);
                                //        retrycounter++;
                                //    }
                                //    if (bpstatus == BluePrintStatus.Complete)
                                //    {
                                //        var bpctrl = new BluePrintControl();
                                //        var data = Acumulator.Instance().bluePrintManager.getBlueprintData(HuidigRuimte.RuimteID);
                                //        if (data != null)
                                //            bpctrl.LoadImg(data);
                                //        SubPagina.SubContPanel.Children.Add(bpctrl);
                                //    }
                                //    else
                                //    {
                                //        SubPagina.SubContPanel.Children.Add(new TextBlock()
                                //        {
                                //            Text = "Het downloaden van de plattegrond duurt langer dan verwacht. \n Ga door met het invullen van de offerte",
                                //            Margin = new Thickness(20, 0, 0, 0),
                                //            FontFamily = new System.Windows.Media.FontFamily("Lucida Grande"),
                                //            FontSize = 12
                                //        });
                                //    }
                                //}
                                //else
                                //{
                                //    SubPagina.SubContPanel.Children.Add(new TextBlock()
                                //    {
                                //        Text = "Voor deze ruimte en opstelling is geen plattegrond beschikbaar.",
                                //        Margin = new Thickness(20, 0, 0, 0),
                                //        FontFamily = new System.Windows.Media.FontFamily("Lucida Grande"),
                                //        FontSize = 12
                                //    });
                                //}
                                /*
                                var rskey = Acumulator.Instance().HuidigRuimteSetKey[HuidigRuimte.RuimteID];
                                var rsk_path = Acumulator.Instance().TekeningBijRuimte[HuidigRuimte.RuimteID][rskey];
                                if (Acumulator.Instance().Blueprints.ContainsKey(rsk_path))
                                {
                                    bpctrl.LoadImg(Acumulator.Instance().Blueprints[rsk_path]);
                                }
                                else
                                {

                                    string path = Acumulator.Instance().GetPathGespiegeld(HuidigRuimte.RuimteID);
                                
                                    bpctrl.LoadImg(path);
                                }


                              
                              
							 */  
                            }
                            catch (Exception ex)
                            {
                                LogHelper.SendLog(ex.Message, LogType.error);
                            }
                        
                        
						//boom_next((TreeViewItem)treeView1.SelectedItem);
						break;
					case "BluePrintPage":
						SubPagina.Titel = "Plattegrond";
                        HuidigRuimte.SwitchBluePrint();
						var bpp = new BluePrintPage(HuidigRuimte.RuimteID);
						SubPagina.SubContPanel.Children.Add(bpp);
						break;
					case "PakketKeuze":
						SubPagina.Titel = "Pakket";

                        if (!SkipNode(SkipType.normaal))
                        {
                            KeuzeList KL = new KeuzeList(HuidigRuimte, "Pakket", null);
                            var paks = HuidigRuimte.Children.Where(x => x.GetType() == typeof(PakketKeuze)).ToList();
                            var compp = new CustomComparer<IOfferte>();
                            paks.Sort(compp);
                            KL.InitList(paks, true);
                            if (HuidigRuimte.PakketOud != null || HuidigRuimte.GekozenPakket != null)
                            {
                                KL.ReloadKeuzes();
                            }
                            SubPagina.SubContPanel.Children.Add(KL);
                        }
                        else
                        {
                            TextBlock tb = new TextBlock()
                            {
                                Margin = new Thickness(20, 0, 0, 0),
                                FontFamily = new System.Windows.Media.FontFamily("Lucida Grande"),
                                FontSize = 12
                            };
                            tb.Text = "Er is voor deze configuratie geen pakket beschikbaar." + Environment.NewLine + (IsCurrentComplete() ? "" : "Klik op bevestigen om door te gaan naar de volgende stap.");
                            SubPagina.SubContPanel.Children.Add(tb);
                        }
                       
                          break;
					case "RuimteOpties_pre":
						SubPagina.Titel = "Opties";
						KeuzeList KL2 = new KeuzeList(HuidigRuimte, "Opties", null);
						var opts1 = HuidigRuimte.Children.Where(x => x.GetType() == typeof(OptieKeuze) &&
                                                               ((x as OptieKeuze).OptieSoort == "OT1"  ||
                                                               (x as OptieKeuze).OptieSoort == "OT2"   ||
                                                               (x as OptieKeuze).OptieSoort == "OT5")  &&
                                                               ((x as OptieKeuze).OptType == OptieType.Determinating ||
                                                               (x as OptieKeuze).OptType == OptieType.Independant)).ToList();
                        if (opts1.Count > 0)
                        {
                            //filter options?
                            var ctx = Acumulator.Instance().ctx;
                            var opsToDel = new List<IOfferte>();
                            foreach (var o in opts1)
                            {
                                var optieKeuze = o as OptieKeuze;
                                if (optieKeuze == null)
                                {continue;                                }
                                var bnrGroepFilterCombos = (from bfc in ctx.BouwnummerFilterCombinaties
                                                           where bfc.R_NR == optieKeuze.OptieID
                                                           select bfc.BOG_NR).ToList();
                                if (bnrGroepFilterCombos == null || bnrGroepFilterCombos.Count < 1)
                                {continue;}
                                
                                //optie moet gefilterd worden!
                                var foo = (from b in ctx.BouwnummerOptieGroepOpbouws
                                           where bnrGroepFilterCombos.Contains(b.BOG_NR)
                                           && b.B_NR == Acumulator.Instance().Bouwnr
                                           select b.BOGO_ID).ToList();
                                if (foo == null || foo.Count < 1)
                                {
                                    //optie hiden
                                    opsToDel.Add(o);
                                }
                            }

                            foreach (var otd in opsToDel)
                            {
                                opts1.Remove(otd);
                            }

                            //poging 1
                            //var filterdOptions = (from r in ctx.BouwnummerFilterCombinaties
                            //                 select r.R_NR).ToList();
                            //var activeFilterdOptions = opts1.Where(x => x.GetType() == typeof(OptieKeuze) && filterdOptions.Contains((x as OptieKeuze).OptieID)).ToList();

                            //if (activeFilterdOptions != null && activeFilterdOptions.Count > 0)
                            //{
                            //    var BogNr = (int)(from bfc in ctx.BouwnummerOptieGroepOpbouws
                            //              where bfc.B_NR == Acumulator.Instance().Bouwnr
                            //              select bfc.BOG_NR).FirstOrDefault();
                            //    var validActiveFilerdOptions = (from o in ctx.BouwnummerFilterCombinaties
                            //                                    where o.BOG_NR == BogNr &&
                            //                                    activeFilterdOptions.Select(x=> (x as OptieKeuze).OptieID).Contains(o.R_NR)
                            //                                    select o.R_NR).ToList();
                            //}
                            //////



                            var comp = new Helpers.CustomComparer<IOfferte>();
                            opts1.Sort(comp);
                            KL2.InitList(opts1, false);
                            if (HuidigRuimte.GekozenOpties.Count > 0 || IsCurrentComplete())
                            {
                                WijzigBtn.Visibility = Visibility.Visible;
                                //HuidigRuimte.LoadOptieSet(KL2);
                                KL2.ReloadKeuzes();
                            }
                            SubPagina.SubContPanel.Children.Add(KL2);
                        }
                        else
                        {
                            TextBlock tb = new TextBlock()
                            {
                                Margin = new Thickness(20, 0, 0, 0),
                                FontFamily = new System.Windows.Media.FontFamily("Lucida Grande"),
                                FontSize = 12
                            };
                            tb.Text = "Er is voor deze configuratie geen optie beschikbaar." + Environment.NewLine + (IsCurrentComplete() ? "" : "Klik op bevestigen om door te gaan naar de volgende stap.");
                            SubPagina.SubContPanel.Children.Add(tb);
                        }
						break;
					case "SubCategoriën":
						SubPagina.Titel = "Tegels";
                         if (HuidigRuimte.GekozenPakket != null && !SkipNode(SkipType.normaal))
                            {
                                var scpp = HuidigRuimte.qrySubCatPakketten(Acumulator.Instance().ctx, HuidigRuimte.GekozenPakket);
                                var comp1 = new Helpers.CustomComparer<IOfferte>();
                                scpp.Sort(comp1);
                                Dictionary<string, List<SubCatPerPakket>> PerMainCat = new Dictionary<string, List<SubCatPerPakket>>();
                                foreach (var subc in scpp)
                                {
                                    var oms = (from sbct in Acumulator.Instance().ctx.SubCats
                                               where sbct.SCB_ID == subc.SubCatNR
                                               select sbct.Omschrijving).FirstOrDefault();
                                    subc.Omschrijving = oms;
                                    if (PerMainCat.Keys.Contains(subc.CategorieNR))
                                    {
                                        PerMainCat[subc.CategorieNR].Add(subc);
                                    }
                                    else
                                    {
                                        var s = new List<SubCatPerPakket>();
                                        s.Add(subc);
                                        PerMainCat.Add(subc.CategorieNR, s);
                                    }
                                }
                                foreach (var cat in PerMainCat)
                                {
                                    var sc = new SubCatkControl(null, MainCatType.normaal);
                                    var cat_oms = (from c in Acumulator.Instance().ctx.Categorieëns
                                                   where c.C_ID == cat.Key
                                                   select c.Omschrijving).FirstOrDefault().ToString();
                                    sc.setTitle(cat_oms);
                                    foreach (var subct in cat.Value)
                                    {
                                        if (subct.SubCatNR != "SCB3" && subct.SubCatNR != "SCB10" && subct.SubCatNR != "SCB12")
                                        {
                                            string ttl = null;
                                            if (cat.Value.Count > 1)
                                            {
                                                ttl = subct.Omschrijving;
                                            }
                                            sc.InitSubs(subct, "product", HuidigRuimte, SubPagina, ttl);
                                        }
                                    }
                                    SubPagina.SubContPanel.Children.Add(sc);
                                }
                            }
                            else
                            {
                                TextBlock tb = new TextBlock()
                                {
                                    Margin = new Thickness(20, 0, 0, 0),
                                    FontFamily = new System.Windows.Media.FontFamily("Lucida Grande"),
                                    FontSize = 12
                                };
                                tb.Text = "Er is voor deze configuratie geen tegel beschikbaar." + Environment.NewLine + (IsCurrentComplete() ? "" : "Klik op bevestigen om door te gaan naar de volgende stap.");
                                SubPagina.SubContPanel.Children.Add(tb);
                            }
                        
						break;
					case "SubCategoriën_accent":
						SubPagina.Titel = "Accent Tegels";
                        if (HuidigRuimte.GekozenPakket != null && (HuidigRuimte.IsAccentNewStyle() || !SkipNode(SkipType.accent)))
                        {
                            if (HuidigRuimte.IsAccentNewStyle())
                            {
                                var scppN = HuidigRuimte.qryNewSubCatAccent(Acumulator.Instance().ctx,HuidigRuimte.GekozenPakket);

                               

                                foreach (var subc in scppN)
                                {
                                    if (subc.Value != null)
                                    {
                                        var page = new AdvancedTegelSubCatPage();
                                        page.LoadContext(subc.Value);
                                        SubPagina.SubContPanel.Children.Add(page);
                                    }
                                //    var oms = (from sbct in Acumulator.Instance().ctx.SubCats
                                //               where sbct.SCB_ID == subc.Key
                                //               select sbct.Omschrijving).FirstOrDefault();
                                //    var hcnr = (from cat in Acumulator.Instance().ctx.Categorieëns
                                //                join scat in Acumulator.Instance().ctx.SubCats on cat.C_ID equals scat.C_NR
                                //                where scat.SCB_ID == subc.Key
                                //                select cat.C_ID).FirstOrDefault();
                                //    var sc = new SubCatkControl(hcnr, MainCatType.accent);
                                //    var cat_oms = (from c in Acumulator.Instance().ctx.Categorieëns
                                //                   where c.C_ID == hcnr
                                //                   select c.Omschrijving).FirstOrDefault();
                                //    sc.setTitle(cat_oms);
                                       
                                }
                                
                            }
                            else
                            {
                                var scpp = HuidigRuimte.qrySubCatAccent(Acumulator.Instance().ctx, HuidigRuimte.GekozenPakket);
                                var comp1 = new Helpers.CustomComparer<IOfferte>();
                                scpp.Sort(comp1);
                                if (scpp.Count > 0)
                                {
                                    Dictionary<string, List<SubCatPerPakket>> PerMainCat = new Dictionary<string, List<SubCatPerPakket>>();
                                    foreach (var subc in scpp)
                                    {
                                        var oms = (from sbct in Acumulator.Instance().ctx.SubCats
                                                   where sbct.SCB_ID == subc.SubCatNR
                                                   select sbct.Omschrijving).FirstOrDefault();
                                        subc.Omschrijving = oms;
                                        if (PerMainCat.Keys.Contains(subc.CategorieNR))
                                        {
                                            PerMainCat[subc.CategorieNR].Add(subc);
                                        }
                                        else
                                        {
                                            var s = new List<SubCatPerPakket>();
                                            s.Add(subc);
                                            PerMainCat.Add(subc.CategorieNR, s);
                                        }
                                    }
                                    foreach (var cat in PerMainCat)
                                    {
                                        var hcnr = (from c in Acumulator.Instance().ctx.Categorieëns
                                                    where c.C_ID == cat.Key
                                                    select c.HC_NR).FirstOrDefault().ToString();
                                        var sc = new SubCatkControl(hcnr, MainCatType.accent);
                                        var cat_oms = (from c in Acumulator.Instance().ctx.Categorieëns
                                                       where c.C_ID == cat.Key
                                                       select c.Omschrijving).FirstOrDefault().ToString();
                                        sc.setTitle(cat_oms);
                                        foreach (var subct in cat.Value)
                                        {
                                            string ttl = null;
                                            if (cat.Value.Count > 1)
                                            {
                                                ttl = subct.Omschrijving;
                                            }
                                            else
                                            {
                                                //      ttl = cat_oms;
                                            }
                                            sc.InitSubs(subct, "product", HuidigRuimte, SubPagina, ttl);
                                        }
                                        //  sc.Visibility = Visibility.Visible;
                                        SubPagina.SubContPanel.Children.Add(sc);
                                    }
                                }
                                else
                                {
                                    TextBlock tb = new TextBlock()
                                    {
                                        Margin = new Thickness(20, 0, 0, 0),
                                        FontFamily = new System.Windows.Media.FontFamily("Lucida Grande"),
                                        FontSize = 12
                                    };
                                    tb.Text = "Er is voor deze configuratie geen accenttegel beschikbaar." + Environment.NewLine + (IsCurrentComplete() ? "" : "Klik op bevestigen om door te gaan naar de volgende stap.");
                                    SubPagina.SubContPanel.Children.Add(tb);
                                }
                            }
                        }
                        else
                        {
                            TextBlock tb = new TextBlock()
                            {
                                Margin = new Thickness(20, 0, 0, 0),
                                FontFamily = new System.Windows.Media.FontFamily("Lucida Grande"),
                                FontSize = 12
                            };
                            tb.Text = "Er is voor deze configuratie geen accenttegel beschikbaar." + Environment.NewLine + (IsCurrentComplete() ? "" : "Klik op bevestigen om door te gaan naar de volgende stap.");
                            SubPagina.SubContPanel.Children.Add(tb);
                        }
                            
                        
						break;
					case "SubCategoriën_hoek":
						SubPagina.Titel = "Hoekprofielen";
                        if (HuidigRuimte.GekozenPakket != null)
                            {
                                SubPagina.firstchoice = null;
                                var scpp = HuidigRuimte.qryHoekprofielen(Acumulator.Instance().ctx, HuidigRuimte.GekozenPakket);
                                if (scpp != null && scpp.Count > 0)
                                {
                                    var comp1 = new Helpers.CustomComparer<IOfferte>();
                                    scpp.Sort(comp1);
                                    Dictionary<string, List<SubCatPerPakket>> PerMainCat = new Dictionary<string, List<SubCatPerPakket>>();
                                    foreach (var subc in scpp)
                                    {
                                        var oms = (from sbct in Acumulator.Instance().ctx.SubCats
                                                   where sbct.SCB_ID == subc.SubCatNR
                                                   select sbct.Omschrijving).FirstOrDefault();
                                        subc.Omschrijving = oms;
                                        if (PerMainCat.Keys.Contains(subc.CategorieNR))
                                        {
                                            PerMainCat[subc.CategorieNR].Add(subc);
                                        }
                                        else
                                        {
                                            var s = new List<SubCatPerPakket>();
                                            s.Add(subc);
                                            PerMainCat.Add(subc.CategorieNR, s);
                                        }
                                    }
                                    foreach (var cat in PerMainCat)
                                    {
                                        var hcnr = (from c in Acumulator.Instance().ctx.Categorieëns
                                                    where c.C_ID == cat.Key
                                                    select c.HC_NR).FirstOrDefault().ToString();
                                        var sc = new SubCatkControl(hcnr, MainCatType.verwerking);
                                        var cat_oms = (from c in Acumulator.Instance().ctx.Categorieëns
                                                       where c.C_ID == cat.Key
                                                       select c.Omschrijving).FirstOrDefault().ToString();
                                        sc.setTitle(cat_oms);
                                        foreach (var subct in cat.Value)
                                        {
                                            string ttl = null;
                                            if (cat.Value.Count > 1)
                                            {
                                                ttl = subct.Omschrijving;
                                            }
                                            sc.InitSubs(subct, "hoekprofiel", HuidigRuimte, SubPagina, ttl);
                                        }
                                        SubPagina.SubContPanel.Children.Add(sc);
                                    }
                                }
                                else
                                {
                                    TextBlock tb = new TextBlock()
                                    {
                                        Margin = new Thickness(20, 0, 0, 0),
                                        FontFamily = new System.Windows.Media.FontFamily("Lucida Grande"),
                                        FontSize = 12
                                    };
                                    tb.Text = "Er is voor deze configuratie geen hoekprofiel beschikbaar." + Environment.NewLine + (IsCurrentComplete() ? "" : "Klik op bevestigen om door te gaan naar de volgende stap.");
                                    SubPagina.SubContPanel.Children.Add(tb);
                                }
                            }
                            else
                            {
                                TextBlock tb = new TextBlock()
                                {
                                    Margin = new Thickness(20, 0, 0, 0),
                                    FontFamily = new System.Windows.Media.FontFamily("Lucida Grande"),
                                    FontSize = 12
                                };
                                tb.Text = "Er is voor deze configuratie geen hoekprofiel beschikbaar." + Environment.NewLine + (IsCurrentComplete() ? "" : "Klik op bevestigen om door te gaan naar de volgende stap.");
                                SubPagina.SubContPanel.Children.Add(tb);
                            }
                        
						break;
					case "SubCategoriën_na":
						SubPagina.Titel = "Verwerking";
                        if (HuidigRuimte.GekozenPakket != null)
                        {
                            var nabew = HuidigRuimte.qryNabewerkingen(Acumulator.Instance().ctx);
                            var comp2 = new Helpers.CustomComparer<IOfferte>();
                            nabew.Sort(comp2);
                            nabew.RemoveAll(x => (x as SubCatPerPakket).SubCatNR == "SCB17");
                            Dictionary<string, List<SubCatPerPakket>> PerMainCat = new Dictionary<string, List<SubCatPerPakket>>();


                            var PerUiCat = new Dictionary<string, List<NabewerkingUiRegel>>();
                            var naHandler = new NabewerkingHandler();
                            naHandler.MakeBewerkingMain(nabew, HuidigRuimte);

                            if (HuidigRuimte.GekozenBewerkingen.Count > 0)
                            {
                                SubmitBtn.Visibility = Visibility.Visible;
                            }
                            foreach (var ur in naHandler.UiRegelCollection)
                            {

                                if (PerUiCat.Keys.Contains(ur.SubCat))
                                {
                                    PerUiCat[ur.SubCat].Add(ur);
                                }
                                else
                                {
                                    var nwList = new List<NabewerkingUiRegel>();
                                    nwList.Add(ur);
                                    PerUiCat.Add(ur.SubCat, nwList);
                                }
                            }

                            foreach (var cat in PerUiCat)
                            {
                                var omschr = (from sbct in Acumulator.Instance().ctx.SubCats
                                              where sbct.SCB_ID == cat.Key
                                              select sbct.Omschrijving).FirstOrDefault();

                                 var Cat = (from sc in Acumulator.Instance().ctx.SubCats where sc.SCB_ID == cat.Key select sc.C_NR).FirstOrDefault();
                                 if (Cat != null)
                                 {
                                     var hcnr = (from c in Acumulator.Instance().ctx.Categorieëns
                                                 where c.C_ID == Cat
                                                 select c.HC_NR).FirstOrDefault();
                                     if (hcnr != null)
                                     {
                                         var scControl = new SubCatkControl(hcnr, MainCatType.verwerking);
                                         scControl.setTitle(omschr);

                                         scControl.InitNabew(naHandler.GetRegelsPerSub(cat.Key), HuidigRuimte, SubPagina, null);

                                         SubPagina.SubContPanel.Children.Add(scControl);
                                     }
                                     else
                                     {
                                         MessageBox.Show("No HCNR");
                                     }
                                 }
                                 else
                                 {
                                     MessageBox.Show("No cat found");
                                 }
                               
                            }
                        }
                        else
                        {
                            TextBlock tb = new TextBlock()
                            {
                                Margin = new Thickness(20, 0, 0, 0),
                                FontFamily = new System.Windows.Media.FontFamily("Lucida Grande"),
                                FontSize = 12
                            };
                            tb.Text = "Er is voor deze configuratie geen verwerking beschikbaar." + Environment.NewLine + (IsCurrentComplete() ? "" : "Klik op bevestigen om door te gaan naar de volgende stap.");
                            SubPagina.SubContPanel.Children.Add(tb);
                        }
                        
						break;
					case "FinalStage":
						SubmitBtn.Visibility = Visibility.Collapsed;
						VerderBtn.Visibility = Visibility.Collapsed;
						SubPagina.Titel = "";
						Acumulator.Instance().BusyBee.BusyContent = "Ingevulde ruimtes controleren...";
                        Acumulator.Instance().BusyBee.IsBusy = true;
                        BackgroundWorker BW = new BackgroundWorker();
                        BW.DoWork += new DoWorkEventHandler(BW_DoWork);
                        BW.RunWorkerAsync();
                       break;
					default:
						SubPagina.Titel = (treeView1.SelectedItem as TvNode).NodeNaam.ToString();
						treeView1.UpdateLayout();
						boom_next((TvNode)treeView1.SelectedItem);
						break;
				}

			}
		}

        void BW_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(500);
            UIThread.Invoke(() => { SubPagina.SubContPanel.Children.Add(new FinalStage()); });
            System.Threading.Thread.Sleep(50);
            UIThread.Invoke(() => { ShowHideAllMarks(true, null); });
        }
		public Dictionary<string,bool> CheckAllMarks(TvNode node)
		{
			Dictionary<string, bool> results = new Dictionary<string, bool>(); 
			List<TvNode> NodeL;
			if(node == null)
			{
				NodeL = treeView1.ItemsSource as List<TvNode>;
			}
			else
			{
				NodeL = node.Children;
			}
			if (NodeL != null)
			{
				foreach (TvNode tn in NodeL)
				{
					var rets = CheckAllMarks(tn);
					foreach (var x in rets)
					{
						results.Add(x.Key, x.Value);
					}
					var tvi = treeView1.ContainerFromItem(tn) as TreeViewItem;
					if (tvi != null)
					{
						tvi.IsExpanded = true;
					}
					if (tn.Children == null && node != null)
					{
						results.Add(node.NodeNaam + " " + tn.NodeNaam + " :"+tn.NodeID, internalCompleet(tn));
					}
				}
			}
			return results;
		}
        public void ValidateAllMarks(TvNode node)
        {
            bool refresh = false;
            List<TvNode> NodeL;
            if (node == null)
            {
                refresh = true;
                NodeL = treeView1.ItemsSource as List<TvNode>;
            }
            else
            {
                NodeL = node.Children;
            }
            if (NodeL != null)
            {
                foreach (TvNode tn in NodeL)
                {
                    ValidateAllMarks(tn);
                    if (tn.Children == null && node != null)
                    {
                        if (internalCompleet(tn))
                        {
                            tn.Status = TvNodeStatus.Compleet;
                        }
                    }
                }
            }
            if (refresh)
            {
                ShowHideAllMarks(true, null);
            }
        }
        private bool internalCompleet(TvNode tn)
        {
            if (tn.Status == TvNodeStatus.Compleet || tn.Status == TvNodeStatus.InvalidCompleet)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
		public void ShowHideAllMarks(bool show, TvNode node)
		{List<TvNode> NodeL;
			if(node == null)
			{
				NodeL = treeView1.ItemsSource as List<TvNode>;
			}
			else
			{
				NodeL = node.Children;
			}
			if (NodeL != null)
			{
				foreach (TvNode tn in NodeL)
				{
					ShowHideAllMarks(show, tn);
					if (show)
						tn.ShowCompleet();
					else
					{
						if (tn.Status == TvNodeStatus.Compleet || tn.Status == TvNodeStatus.InvalidCompleet)
						{
							tn.ShowCompleet();
						}
						else
							tn.HideCompleet();
					}
				}
			}
			
		}
		public List<string> MakeCompleteList()
		{
			return makecomplisRec(null);
		}
		private List<string> makecomplisRec(TvNode node)
		{
			List<string> completes = new List<string>();
			List<TvNode> NodeL;
			if (node == null)
			{
				NodeL = treeView1.ItemsSource as List<TvNode>;
			}
			else
			{
				NodeL = node.Children;
			}
			if (NodeL != null)
			{
				foreach (TvNode tn in NodeL)
				{
					completes.AddRange(makecomplisRec(tn));
					if (tn.Parent != null && (tn.Status == TvNodeStatus.Compleet || tn.Status == TvNodeStatus.InvalidCompleet))
					{
						completes.Add(tn.NodeNaam + "-"+(String.IsNullOrEmpty(tn.Parent.NodeNaam)? "": tn.Parent.NodeNaam));
					}
				}
			}
			return completes;
		}
		public void CompleteAfterLoad(TvNode node)
		{
			List<TvNode> NodeL;
			if (node == null)
			{
				NodeL = treeView1.ItemsSource as List<TvNode>;
			}
			else
			{
				NodeL = node.Children;
			}
			if (NodeL != null)
			{
				foreach (TvNode tn in NodeL)
				{
                    try
                    {
                        if (tn.Parent != null && Acumulator.Instance().OTracker.offerteRuimte_.LoadedCompleet.Contains(tn.NodeNaam + "-" + tn.Parent.NodeNaam))
                        {
                            tn.Status = TvNodeStatus.Compleet;
                        }
                    }
                    catch (Exception exep)
                    {
                        tn.Status = TvNodeStatus.Incompleet;
                    }
					CompleteAfterLoad(tn);
				}
			}
			
		}
        private void BackBtn_Click(object sender, RoutedEventArgs e)
		{
			boom_back((TvNode)treeView1.SelectedItem);
		}

		public void ShowWijzigBtn(bool zichtbaar)
		{
			if (!zichtbaar)
			{
				WijzigBtn.Visibility = Visibility.Collapsed;
			}
			else
			{
				WijzigBtn.Visibility = Visibility.Visible;
			}
		}
		private void WijzigBtn_Click(object sender, RoutedEventArgs e)
		{
            var res = new MessageBoxResult();
             var tmpname = (from b in Acumulator.Instance().ctx.Bouwnummers
					   where b.B_ID == Acumulator.Instance().Bouwnr
					   select b.Omschrijving).FirstOrDefault();
             if ((treeView1.SelectedItem as TvNode).NodeNaam.EndsWith(tmpname))
             {
                 res = MessageBoxResult.OK;
             }
             else
             {
                 res = MessageBox.Show("Hiermee gaan al uw vervolgkeuzes binnen deze ruimte verloren, weet u zeker dat u wilt wijzigen?", "Zeker weten?", MessageBoxButton.OKCancel);
             }
			if (res == MessageBoxResult.OK)
			{
                Acumulator.Instance().OTracker.offerteRuimte_.Korting = null;
				if (SubPagina != null)
				{
					SubPagina.WijzigPressed();
				}
				WijzigBtn.Visibility = Visibility.Collapsed;
				SubmitBtn.Visibility = Visibility.Visible;
			}
		}

		private void TerugBtn_Click(object sender, RoutedEventArgs e)
		{
			boom_back((TvNode)treeView1.SelectedItem);
		}

		private void VerderBtn_Click(object sender, RoutedEventArgs e)
		{
			boom_next((TvNode)treeView1.SelectedItem);
		}
	}
	public static class TreeViewExtensions
	{
		public static TreeViewItem ContainerFromItem(this TreeView treeView, object item)
		{
			TreeViewItem containerThatMightContainItem = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(item);
			if (containerThatMightContainItem != null)
				return containerThatMightContainItem;
			else
				return ContainerFromItem(treeView.ItemContainerGenerator, treeView.Items, item);
		}

		private static TreeViewItem ContainerFromItem(ItemContainerGenerator parentItemContainerGenerator, ItemCollection itemCollection, object item)
		{
			foreach (object curChildItem in itemCollection)
			{
				TreeViewItem parentContainer = (TreeViewItem)parentItemContainerGenerator.ContainerFromItem(curChildItem);
				if (parentContainer == null)
					return null;
				TreeViewItem containerThatMightContainItem = (TreeViewItem)parentContainer.ItemContainerGenerator.ContainerFromItem(item);
				if (containerThatMightContainItem != null)
					return containerThatMightContainItem;
				TreeViewItem recursionResult = ContainerFromItem(parentContainer.ItemContainerGenerator, parentContainer.Items, item);
				if (recursionResult != null)
					return recursionResult;
			}
			return null;
		}

		public static object ItemFromContainer(this TreeView treeView, TreeViewItem container)
		{
			TreeViewItem itemThatMightBelongToContainer = (TreeViewItem)treeView.ItemContainerGenerator.ItemFromContainer(container);
			if (itemThatMightBelongToContainer != null)
				return itemThatMightBelongToContainer;
			else
				return ItemFromContainer(treeView.ItemContainerGenerator, treeView.Items, container);
		}

		private static object ItemFromContainer(ItemContainerGenerator parentItemContainerGenerator, ItemCollection itemCollection, TreeViewItem container)
		{
			foreach (object curChildItem in itemCollection)
			{
				TreeViewItem parentContainer = (TreeViewItem)parentItemContainerGenerator.ContainerFromItem(curChildItem);
				if (parentContainer == null)
					return null;
				TreeViewItem itemThatMightBelongToContainer = (TreeViewItem)parentContainer.ItemContainerGenerator.ItemFromContainer(container);
				if (itemThatMightBelongToContainer != null)
					return itemThatMightBelongToContainer;
				TreeViewItem recursionResult = ItemFromContainer(parentContainer.ItemContainerGenerator, parentContainer.Items, container) as TreeViewItem;
				if (recursionResult != null)
					return recursionResult;
			}
			return null;
		}
		
	}
	
}

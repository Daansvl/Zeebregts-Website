using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.Windows.Input;
using System.Windows.Threading;


namespace GebruikersMonitor
{
    /// <summary>
    /// Interaction logic for Module1Monitor.xaml
    /// </summary>
    public partial class Module1Monitor : Window
    {
        List<int> proj_Hand_nrs = new List<int>() { 1, 2, 26, 18, 20, 21, 2, 5, 6, 13, 14, 15, 16, 17 };
        List<int> bdr_Hand_nrs = new List<int>() {1,2,26,18,22,23,20,19,21,24,25,5,6,13,14,15,16,17,3,4 };
        List<int> bdr_vi_Hand_nrs = new List<int>() { 8,10,12};
        List<int> cont_Hand_nrs = new List<int>() { 1, 2, 26, 18, 22, 23, 19, 21, 24, 25, 3, 7, 9, 11, 3, 2, 5, 6, 13, 14, 15, 16, 17 };
        List<int> cont_vi_Hand_nrs = new List<int>() { 7, 9, 11 };
        List<int> all_Hand_nrs = new List<int>() { 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26};
        DateTime date_forall = new DateTime(2011, 5, 1);
        string stand = String.Empty;
        DateTime date_forFilter = new DateTime();
        private Dictionary<string, bool> _dict;
        private ICollectionView h_log;
        private string conString = String.Empty;
        private Linq2handelingenLogDataContext Handelingen_DataContext;
        private System.Windows.Forms.Form Pa_rent;
        private Stack<bool> expanded = new Stack<bool>();
        
        public Module1Monitor(String connectionString, System.Windows.Forms.Form parent )
        {
            InitializeComponent();
            conString = connectionString;
            Handelingen_DataContext = new Linq2handelingenLogDataContext(conString);
            _dict = new Dictionary<string, bool>();
            Pa_rent = parent;
            stand = "Algemeen";
           
            this.Title = "Monitor";
            this.Height = Pa_rent.Size.Height;
            this.Width = Pa_rent.Width;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            this.Top = Pa_rent.Location.Y;
            this.Left = Pa_rent.Location.X;
            this.WindowState = (System.Windows.WindowState)Pa_rent.WindowState;
            this.Icon = null;
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() =>
                {
                    rb_maand.IsChecked = true;
                }));
            
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Pa_rent.Location = new System.Drawing.Point((int)this.Left, (int)this.Top);
            Pa_rent.Height = (int)this.Height;
            Pa_rent.Width = (int)this.Width;
            Pa_rent.WindowState = (System.Windows.Forms.FormWindowState)this.WindowState;
            Pa_rent.Show();
        }
        
        private ICollectionView make_cv(DataTable dt)
        {
            h_log = CollectionViewSource.GetDefaultView(dt);
            h_log.SortDescriptions.Add(new SortDescription("SortOrder", ListSortDirection.Ascending));
            h_log.GroupDescriptions.Add(new PropertyGroupDescription("Soort"));
           return h_log;
        }
        private ICollectionView convert4grouping(List<HM_item> hmlist, List<int> Hand_nrs)
        {
            
            DataTable tmp_dt = new DataTable();
            List<string> al_een_string_column = new List<string>();
            List<int> al_een_int_column = new List<int>();
            DataColumn dc1 = new DataColumn("Handeling", typeof(string));
            DataColumn dc2 = new DataColumn("SortOrder", typeof(int));
            int col1_cntr = 0;
            dc1.ColumnMapping = MappingType.Hidden;
            tmp_dt.Columns.Add(dc1);
            tmp_dt.Columns.Add(dc2);
           foreach(HM_item item in hmlist)
            {
                if (!al_een_int_column.Contains(item.handeling_nr))
                {
                    al_een_int_column.Add(item.handeling_nr);
                    DataRow dr = tmp_dt.NewRow();
                    dr[0] = item.handeling_nr;
                    dr[1] = item.Order;
                    col1_cntr++;
                    tmp_dt.Rows.Add(dr);
                }
                if (!al_een_string_column.Contains(item.gebruiker.ToString()))
                {
                    DataColumn dc = new DataColumn(item.gebruiker, item.gebruiker.GetType());
                    al_een_string_column.Add(item.gebruiker);
                    tmp_dt.Columns.Add(dc);
                }
            }
           foreach (HM_item itt in hmlist)
           {
               foreach (DataRow d_r in tmp_dt.Rows)
               {
                   if (int.Parse(d_r[0].ToString()) == itt.handeling_nr)
                   {
                       d_r.SetField(itt.gebruiker, itt._count);
                   }
               }
           }
           var nm = from h in Handelingen_DataContext.gebruikers_handelingens
                    select new { nummer = h.handeling_nr, naam = h.handeling_omschrijving, categorie = h.Categorie };
           foreach (var x in nm)
           {
               if (!al_een_int_column.Contains(x.nummer) && Hand_nrs.Contains(x.nummer))
               {
                   DataRow drr = tmp_dt.NewRow();
                   drr[0] = x.nummer;
                   tmp_dt.Rows.Add(drr);
               }
           }
           DataColumn dc_naam = new DataColumn("Handelingen", typeof(string));
           DataColumn dc_categorie = new DataColumn("Soort", typeof(string));
           
           tmp_dt.Columns.Add(dc_naam); tmp_dt.Columns.Add(dc_categorie);
           
           
           foreach(DataRow dtr in tmp_dt.Rows)
            {
                int ix = int.Parse(dtr[0].ToString());
                var handelingnaam = (from h in Handelingen_DataContext.gebruikers_handelingens
                                      where h.handeling_nr == ix
                                      select new { h.handeling_omschrijving });
              var categorienaam = (from c in Handelingen_DataContext.gebruikers_handelingens
                                where c.handeling_nr == ix
                                select new { c.Categorie });
              foreach (var x in handelingnaam)
              {
                  dtr["Handelingen"] = x.handeling_omschrijving;
              }
              foreach (var xc in categorienaam)
              {
                  dtr["Soort"] = xc.Categorie;
              }
                
           }
           dc_naam.SetOrdinal(0);
            return make_cv(tmp_dt);
        }
        private void MenuBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            Pa_rent.Location = new System.Drawing.Point((int)this.Left, (int)this.Top);
            Pa_rent.Height = (int)this.Height;
            Pa_rent.Width = (int)this.Width;
            Pa_rent.WindowState = (System.Windows.Forms.FormWindowState)this.WindowState;
            Pa_rent.Show();
            this.Close();
            this.Cursor = Cursors.Arrow;
        }
        private void load_handelingen_algemeen()
        {
            Category_lbl.Content = "Algemeen";
            stand = "Algemeen";
            var img1 = (Image)this.FindName("alg_img");
            img1.Source = (ImageSource)this.FindResource("algemeen_kleur");
            var img2 = (Image)this.FindName("proj_img");
            img2.Source = (ImageSource)this.FindResource("project_gray");
            var img3 = (Image)this.FindName("bdr_img");
            img3.Source = (ImageSource)this.FindResource("bedrijf_gray");
            var img4 = (Image)this.FindName("cont_img");
            img4.Source = (ImageSource)this.FindResource("contact_gray");
            
            List<HM_item> alglist = (from ah in Handelingen_DataContext.log_handeling_gebruikers
                                     join hg in Handelingen_DataContext.gebruikers_handelingens
                                     on ah.handeling_nr equals hg.handeling_nr
                                     where ah.timestamp >= date_forFilter
                                     group ah by new { ah.handeling_nr, ah.gebruiker, hg.Sort_order, ah.record_type } into g
                                     orderby g.Key.gebruiker, g.Key.handeling_nr, g.Key.Sort_order
                                     select new HM_item { gebruiker = g.Key.gebruiker, handeling_nr = (int)g.Key.handeling_nr, _count = g.Select(x => x.log_id).Count(), Order = (int)g.Key.Sort_order, r_type = (int)g.Key.record_type }).ToList<HM_item>();

            var suplement = from g in Handelingen_DataContext.gebruikers_handelingens
                            select new HM_item {gebruiker = "qxz", handeling_nr = g.handeling_nr, _count = 0, Order = (int)g.Sort_order };
            foreach (var x in suplement)
            {
                alglist.Add(x);
            }
            HM2GL(alglist, 0);
            
        }
        private void load_handelingen_project()
        {
            Category_lbl.Content = "Project";
            stand = "Project";
            var img1 = (Image)this.FindName("alg_img");
            img1.Source = (ImageSource)this.FindResource("algemeen_gray");
            var img2 = (Image)this.FindName("proj_img");
            img2.Source = (ImageSource)this.FindResource("project_kleur");
            var img3 = (Image)this.FindName("bdr_img");
            img3.Source = (ImageSource)this.FindResource("bedrijf_gray");
            var img4 = (Image)this.FindName("cont_img");
            img4.Source = (ImageSource)this.FindResource("contact_gray");
            List<HM_item> projlist = (from ph in Handelingen_DataContext.log_handeling_gebruikers
                                      join hg in Handelingen_DataContext.gebruikers_handelingens
                                      on ph.handeling_nr equals hg.handeling_nr
                                      where ph.record_type == 1 && proj_Hand_nrs.Contains((int)ph.handeling_nr) && ph.timestamp >= date_forFilter
                                      group ph by new { ph.handeling_nr, ph.gebruiker, hg.Sort_order, ph.record_type } into g
                                      orderby g.Key.gebruiker, g.Key.handeling_nr, g.Key.Sort_order
                                      select new HM_item { gebruiker = g.Key.gebruiker, handeling_nr = (int)g.Key.handeling_nr, _count = g.Select(x => x.log_id).Count(), Order = (int)g.Key.Sort_order, r_type = (int)g.Key.record_type }).ToList<HM_item>();

            var suplement = from g in Handelingen_DataContext.gebruikers_handelingens
                            where g.Categorie != "View" && g.handeling_nr < 28
                            select new HM_item { gebruiker = "qxz", handeling_nr = g.handeling_nr, _count = 0, Order = (int)g.Sort_order, r_type = 1 };
            foreach (var x in suplement)
            {
                projlist.Add(x);
            }
            HM2GL(projlist, 1);
        }
        private void load_handelingen_bedrijf()
        {
            Category_lbl.Content = "Bedrijf";
            stand = "Bedrijf";
            var img1 = (Image)this.FindName("alg_img");
            img1.Source = (ImageSource)this.FindResource("algemeen_gray");
            var img2 = (Image)this.FindName("proj_img");
            img2.Source = (ImageSource)this.FindResource("project_gray");
            var img3 = (Image)this.FindName("bdr_img");
            img3.Source = (ImageSource)this.FindResource("bedrijf_kleur");
            var img4 = (Image)this.FindName("cont_img");
            img4.Source = (ImageSource)this.FindResource("contact_gray");
            List<HM_item> bdrlist = (from bh in Handelingen_DataContext.log_handeling_gebruikers
                                     join hg in Handelingen_DataContext.gebruikers_handelingens
                                     on bh.handeling_nr equals hg.handeling_nr
                                     where bh.timestamp >= date_forFilter &&(
                                     (bh.record_type == 2 && bdr_Hand_nrs.Contains((int)bh.handeling_nr)) ||
                                     (bh.record_type == 1 && bdr_vi_Hand_nrs.Contains((int)bh.handeling_nr)) ||
                                     (bh.record_type == 3 && bdr_vi_Hand_nrs.Contains((int)bh.handeling_nr)))
                                     group bh by new { bh.handeling_nr, bh.gebruiker, hg.Sort_order, bh.record_type } into g
                                     orderby g.Key.gebruiker, g.Key.handeling_nr, g.Key.Sort_order
                                     select new HM_item
                                     {
                                         gebruiker = g.Key.gebruiker,
                                         handeling_nr = (int)g.Key.handeling_nr,
                                         _count = g.Select(x => x.log_id).Count(),
                                         Order = (int)g.Key.Sort_order,
                                         r_type = (int)g.Key.record_type
                                     }).ToList<HM_item>();
            List<int> tmpint = new List<int>() { 8,10,12};
            var suplement = from g in Handelingen_DataContext.gebruikers_handelingens
                            where !tmpint.Contains(g.handeling_nr) && g.handeling_nr != 7 && g.handeling_nr != 9 && g.handeling_nr != 11
                            select new HM_item { gebruiker = "qxz", handeling_nr = g.handeling_nr, _count = 0, Order = (int)g.Sort_order, r_type = 2 };
            foreach (var x in suplement)
            {
                bdrlist.Add(x);
            }
            var suplement2 = from g in Handelingen_DataContext.gebruikers_handelingens
                             where tmpint.Contains(g.handeling_nr) && g.handeling_nr != 7 && g.handeling_nr != 9 && g.handeling_nr != 11
                             select new HM_item { gebruiker = "qxz", handeling_nr = g.handeling_nr, _count = 0, Order = (int)g.Sort_order, r_type = 3 };
            foreach (var x in suplement2)
            {
                bdrlist.Add(x);
            }
            var suplement3 = from g in Handelingen_DataContext.gebruikers_handelingens
                             where tmpint.Contains(g.handeling_nr) && g.handeling_nr != 7 && g.handeling_nr != 9 && g.handeling_nr != 11
                             select new HM_item { gebruiker = "qxz", handeling_nr = g.handeling_nr, _count = 0, Order = (int)g.Sort_order, r_type = 1 };
            foreach (var x in suplement3)
            {
                bdrlist.Add(x);
            }
           HM2GL(bdrlist, 2);
        }
        private void load_handelingen_contact()
        {
            Category_lbl.Content = "Contact";
            stand = "Contact";
            var img1 = (Image)this.FindName("alg_img");
            img1.Source = (ImageSource)this.FindResource("algemeen_gray");
            var img2 = (Image)this.FindName("proj_img");
            img2.Source = (ImageSource)this.FindResource("project_gray");
            var img3 = (Image)this.FindName("bdr_img");
            img3.Source = (ImageSource)this.FindResource("bedrijf_gray");
            var img4 = (Image)this.FindName("cont_img");
            img4.Source = (ImageSource)this.FindResource("contact_kleur");
            List<HM_item> contlist = (from ch in Handelingen_DataContext.log_handeling_gebruikers
                                      join hg in Handelingen_DataContext.gebruikers_handelingens
                                      on ch.handeling_nr equals hg.handeling_nr
                                      where  ch.timestamp >= date_forFilter &&
                                      (
                                      (ch.record_type == 3 && cont_Hand_nrs.Contains((int)ch.handeling_nr)) ||
                                      (ch.record_type == 1 && cont_vi_Hand_nrs.Contains((int)ch.handeling_nr))
                                      )
                                      group ch by new { ch.handeling_nr, ch.gebruiker, hg.Sort_order, ch.record_type } into g
                                      orderby g.Key.gebruiker, g.Key.handeling_nr, g.Key.Sort_order
                                      select new HM_item { gebruiker = g.Key.gebruiker, handeling_nr = (int)g.Key.handeling_nr, _count = g.Select(x => x.log_id).Count(), Order = (int)g.Key.Sort_order, r_type = (int)g.Key.record_type }).ToList<HM_item>();
            List<int> tmpint = new List<int>() { 7, 9, 11};
            var suplement = from g in Handelingen_DataContext.gebruikers_handelingens
                            where !tmpint.Contains(g.handeling_nr) && g.handeling_nr != 4 && g.handeling_nr != 8 && g.handeling_nr != 10 && g.handeling_nr != 12 && g.handeling_nr != 28 && g.handeling_nr != 29
                            select new HM_item { gebruiker = "qxz", handeling_nr = g.handeling_nr, _count = 0, Order = (int)g.Sort_order, r_type = 3 };
            foreach (var x in suplement)
            {
                contlist.Add(x);
            }
            var suplement2 = from g in Handelingen_DataContext.gebruikers_handelingens
                             where tmpint.Contains(g.handeling_nr) && g.handeling_nr != 4 && g.handeling_nr != 8 && g.handeling_nr != 10 && g.handeling_nr != 12
                             select new HM_item { gebruiker = "qxz", handeling_nr = g.handeling_nr, _count = 0, Order = (int)g.Sort_order, r_type = 1 };
            foreach (var x in suplement2)
            {
                contlist.Add(x);
            }
            HM2GL(contlist, 3);
        }
        private void Project_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            load_handelingen_project();
            this.Cursor = Cursors.Arrow;
        }
        private void Bedrijf_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            load_handelingen_bedrijf();
            this.Cursor = Cursors.Arrow;
        }
        private void Contact_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            load_handelingen_contact();
            this.Cursor = Cursors.Arrow;
        }
        private void Algemeen_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            load_handelingen_algemeen();
            this.Cursor = Cursors.Arrow;
        }
        private void HM2GL(List<HM_item> h_list, int Type)
        {
             List<int> nwn = new List<int>(){2,3,4};
             var aangemaakt = new List<HM_item>();
             if (Type > 0)
             {
                 aangemaakt = (from q in Handelingen_DataContext.log_handeling_gebruikers
                                   where nwn.Contains((int)q.handeling_nr) && q.timestamp >= date_forFilter && q.record_type == Type
                                   group q by new { q.gebruiker, q.log_id } into g
                                   select new HM_item { gebruiker = g.Key.gebruiker, handeling_nr = 27, Order = 0, r_type = Type, _count = g.Select(x => x.log_id).Count() }).ToList<HM_item>();
             }
             else
             {
                 aangemaakt = (from q in Handelingen_DataContext.log_handeling_gebruikers
                                   where nwn.Contains((int)q.handeling_nr) && q.timestamp >= date_forFilter
                                   group q by new { q.gebruiker, q.log_id } into g
                                   select new HM_item { gebruiker = g.Key.gebruiker, handeling_nr = 27, Order = 0, r_type = Type, _count = g.Select(x => x.log_id).Count() }).ToList<HM_item>();
             }
            foreach (var aanm_HMI in aangemaakt)
            {
                 h_list.Add(aanm_HMI);
                 
            }
            List<GL_mon_item> G_list = new List<GL_mon_item>();
            string handelingnaam = String.Empty;
            string categorienaam = String.Empty;
            int volgorde_nr = 0;
            foreach (HM_item HI in h_list)
            {
                if (G_list.Exists(GL_mon_item => GL_mon_item.HandelingNummer == HI.handeling_nr && (GL_mon_item.H_Type == HI.r_type || Type == 0)))
                    {
                        GL_mon_item GI = G_list.Find(GL_mon_item => GL_mon_item.HandelingNummer == HI.handeling_nr);
                        bool bestaat = false;
                        Stack<KeyValuePair<string, int>> kvp_kill_stack = new Stack<KeyValuePair<string, int>>();
                        Stack<KeyValuePair<string, int>> kvp_new_stack = new Stack<KeyValuePair<string, int>>();
                        foreach (KeyValuePair<string, int> kvp in GI.GebruikersCntr)
                        {
                            if (kvp.Key == HI.gebruiker)
                            {
                                bestaat = true;
                                int neem_mee = kvp.Value;
                                kvp_kill_stack.Push(kvp);
                                kvp_new_stack.Push(new KeyValuePair<string, int>(HI.gebruiker, HI._count + neem_mee));
                            }
                        }
                        if (!bestaat)
                        {
                            GI.GebruikersCntr.Add(new KeyValuePair<string, int>(HI.gebruiker, HI._count));
                        }
                        else
                        {
                            while (kvp_kill_stack.Count > 0)
                            {
                                GI.GebruikersCntr.Remove(kvp_kill_stack.Pop());
                            }
                            while (kvp_new_stack.Count > 0)
                            {
                                GI.GebruikersCntr.Add(kvp_new_stack.Pop());
                            }
                        }
                    }
                    else
                    {
                        var x_handelingnaam = (from h in Handelingen_DataContext.gebruikers_handelingens
                                               where h.handeling_nr == HI.handeling_nr
                                               select new { h.handeling_omschrijving });
                        foreach (var x in x_handelingnaam)
                        {
                            handelingnaam = x.handeling_omschrijving;
                        }
                        var x_categorienaam = (from c in Handelingen_DataContext.gebruikers_handelingens
                                               where c.handeling_nr == HI.handeling_nr
                                               select new { c.Categorie });
                        foreach (var x in x_categorienaam)
                        {
                            categorienaam = x.Categorie;
                        }
                        if (categorienaam.ToLower() == "view")
                        {
                            if (HI.r_type == 1 || HI.handeling_nr == 3)
                            {
                                categorienaam += " Project";
                            }
                            else if (HI.r_type == 3 || HI.handeling_nr == 4)
                            {
                                categorienaam += " Contact";
                            }
                        }
                        var x_volgorde_nr = (from s in Handelingen_DataContext.gebruikers_handelingens
                                             where s.handeling_nr == HI.handeling_nr
                                             select new { s.Sort_order });
                        foreach (var x in x_volgorde_nr)
                        {
                            volgorde_nr = (int)x.Sort_order;
                        }
                        G_list.Add(new GL_mon_item()
                        {
                            HandelingNummer = HI.handeling_nr,
                            Naam = handelingnaam,
                            GroepNaam = categorienaam,
                            SortOrder = volgorde_nr,
                            GebruikersCntr = new List<KeyValuePair<string, int>>(),
                            H_Type = HI.r_type
                        });
                        GL_mon_item GI = G_list.Find(GL_mon_item => GL_mon_item.HandelingNummer == HI.handeling_nr);
                        bool bestaat = false;
                        Stack<KeyValuePair<string, int>> kvp_kill_stack = new Stack<KeyValuePair<string, int>>();
                        Stack<KeyValuePair<string, int>> kvp_new_stack = new Stack<KeyValuePair<string, int>>();
                        foreach (KeyValuePair<string, int> kvp in GI.GebruikersCntr)
                        {
                            if (kvp.Key == HI.gebruiker)
                            {
                                bestaat = true;
                                int neem_mee = kvp.Value;
                                kvp_kill_stack.Push(kvp);
                                kvp_new_stack.Push(new KeyValuePair<string, int>(HI.gebruiker, HI._count + neem_mee));
                            }
                        }
                        if (!bestaat)
                        {
                            GI.GebruikersCntr.Add(new KeyValuePair<string, int>(HI.gebruiker, HI._count));
                        }
                        else
                        {
                            while (kvp_kill_stack.Count > 0)
                            {
                                GI.GebruikersCntr.Remove(kvp_kill_stack.Pop());
                            }
                            while (kvp_new_stack.Count > 0)
                            {
                                GI.GebruikersCntr.Add(kvp_new_stack.Pop());
                            }
                        }
                    }
                }
           
            makecv_GLmonlist(G_list, Type);
        }
        private void makecv_GLmonlist(List<GL_mon_item> GL_list, int Type)
        {
            List<KeyValuePair<string, int>> kvplist_aanmaak = new List<KeyValuePair<string, int>>();
            List<KeyValuePair<string, int>> kvplist_dub1 = new List<KeyValuePair<string, int>>();
            foreach (GL_mon_item subGL in GL_list)
            {
                if (subGL.HandelingNummer == 27)
                {
                    foreach (KeyValuePair<string, int> kvp in subGL.GebruikersCntr)
                    {
                        kvplist_aanmaak.Add(kvp);
                    }
                }
                else if (subGL.HandelingNummer == 6)
                {
                    foreach (KeyValuePair<string, int> kvp in subGL.GebruikersCntr)
                    {
                        kvplist_dub1.Add(kvp);
                    }
                }
               
            }
            List<KeyValuePair<string, int>> nwnkvplist = new List<KeyValuePair<string, int>>();
            foreach (KeyValuePair<string, int> kvvp in kvplist_aanmaak)
            {
                foreach (KeyValuePair<string, int> kkvp in kvplist_dub1)
                {
                    if (kvvp.Key == kkvp.Key)
                    {
                        nwnkvplist.Add(new KeyValuePair<string, int>(kvvp.Key, kvvp.Value - kkvp.Value));
                    }
                }
            }
            //GL_list.Add(new GL_mon_item(){ HandelingNummer = 30, GebruikersCntr = nwnkvplist, H_Type = Type, GroepNaam = "Nieuwe invoer", Naam = "Dubbele invoerlijst niet getoont", SortOrder = 1});
             List<GL_mon_item> GL_sorted_list = new List<GL_mon_item>();
            var x = from g in GL_list
                    orderby g.SortOrder ascending, g.GroepNaam ascending
                    select g;
            foreach (var a in x)
            {
                GL_sorted_list.Add(a);
            }
            
            DataTable DT = new DataTable();
            DataColumn C_hnaam = new DataColumn("HandelingNaam", typeof(string));
            
            DT.Columns.Add(C_hnaam);
            DataColumn C_hnr = new DataColumn("HandelingNr", typeof(int));
            DT.Columns.Add(C_hnr);
            int max_g_col = GL_sorted_list.Max(GL_mon_item => GL_mon_item.GebruikersCntr.Count);
            GL_mon_item GL_I = GL_sorted_list.Find(GL_mon_item => GL_mon_item.GebruikersCntr.Count == max_g_col);
            for (int i = 0; i < max_g_col; i++)
            {
                if (!DT.Columns.Contains(GL_I.GebruikersCntr.ElementAt(i).Key))
                {
                    DataColumn dc = new DataColumn(GL_I.GebruikersCntr.ElementAt(i).Key, typeof(int));
                    DT.Columns.Add(dc);
                }
            }
            DataColumn C_cat = new DataColumn("Groep", typeof(string));
            DT.Columns.Add(C_cat);
            DataColumn C_volgorde = new DataColumn("Volgorde", typeof(int));
            DT.Columns.Add(C_volgorde);
            foreach (GL_mon_item GLMI in GL_sorted_list)
            {
                DataRow DR = DT.NewRow();
                DR[0] = GLMI.Naam;
                DR["HandelingNr"] = GLMI.HandelingNummer;
                foreach (KeyValuePair<string, int> KVP in GLMI.GebruikersCntr)
                {
                    if (DT.Columns[KVP.Key] != null)
                    {
                        DR[KVP.Key] = KVP.Value;
                    }
                }
                DR["Groep"] = GLMI.GroepNaam;
                DR["Volgorde"] = GLMI.SortOrder;
                DT.Rows.Add(DR);
            }
            //// voeg samen
            if (Type == 0)
            {
                DT = voegsamen_algemeen(DT);

            }
            DataContext = DT;
            ICollectionView view = CollectionViewSource.GetDefaultView(DT);
            view.GroupDescriptions.Clear();
            view.GroupDescriptions.Add(new PropertyGroupDescription("Groep"));
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription("Groep", ListSortDirection.Ascending));
            view.Refresh();
        }
        private DataTable voegsamen_algemeen(DataTable DT)
        {
            List<int> c2c_hnrs = new List<int>() { 18, 22, 23 };
            List<int> Vws = new List<int>() { 3, 4, 7, 8, 9, 10, 11, 12 };
            int row_indx_mother = 0;
            List<int> row_indx_child = new List<int>();
            foreach (DataRow DDR in DT.Rows)
            {
                if (c2c_hnrs.Contains((int)DDR["HandelingNr"]))
                {
                    if ((int)DDR["HandelingNr"] != 18)
                    {
                        row_indx_child.Add(DT.Rows.IndexOf(DDR));
                    }
                    else
                    {
                        row_indx_mother = DT.Rows.IndexOf(DDR);
                    }
                }
            }
            foreach (int p in row_indx_child)
            {
                for (int q = 2; q < DT.Columns.Count - 2; q++)
                {
                    if (DT.Rows[row_indx_mother][q].GetType() == typeof(int))
                    {
                        int outp = 0;
                        int outp_b = 0;
                        int.TryParse(DT.Rows[p][q].ToString(), out outp);
                        int.TryParse(DT.Rows[row_indx_mother][q].ToString(), out outp_b);
                        DT.Rows[row_indx_mother][q] = outp + outp_b;
                        DT.Rows[row_indx_mother][0] = "Copy2Clipboard";
                    }
                }
               
            }
            row_indx_child.Reverse();
            foreach (int rp in row_indx_child)
            {
                DT.Rows.RemoveAt(rp);
            }

            //// views
            List<int> row_indx_child_views = new List<int>();
            List<int> row_indx_mother_views = new List<int>();
            foreach (DataRow DDR2 in DT.Rows)
            {
                if (Vws.Contains((int)DDR2["HandelingNr"]))
                {
                    if ((int)DDR2["HandelingNr"] % 2 == 0)//even
                    {
                        row_indx_child_views.Add(DT.Rows.IndexOf(DDR2));
                    }
                    else
                    {
                        row_indx_mother_views.Add(DT.Rows.IndexOf(DDR2));
                    }
                }
            }
            foreach (int l in row_indx_child_views)
            {
                bool hit = false;
                foreach (int m_l in row_indx_mother_views)
                {
                    int o_a = 0;
                    int o_b = 0;
                    int.TryParse(DT.Rows[m_l]["HandelingNr"].ToString(), out o_a);
                    int.TryParse(DT.Rows[l]["HandelingNr"].ToString(), out o_b);
                    if (o_a == (o_b -1))
                    {
                        hit = true;
                        for (int z = 2; z < DT.Columns.Count - 2; z++)
                        {
                            if (DT.Rows[m_l][z].GetType() == typeof(int))
                            {
                                int outp_a = 0;
                                int outp_b = 0;
                                int.TryParse(DT.Rows[l][z].ToString(), out outp_a);
                                int.TryParse(DT.Rows[m_l][z].ToString(), out outp_b);
                                DT.Rows[m_l][z] = outp_a + outp_b;
                                DT.Rows[m_l]["Groep"] = "Alle Views";
                            }
                            else if(DT.Rows[l][z].GetType() == typeof(int))
                            {
                                DT.Rows[m_l][z] = DT.Rows[l][z];
                                DT.Rows[m_l]["Groep"] = "Alle Views";
                                
                            }
                        }
                    }
                }
                if(!hit)
                {
                    DT.Rows[l]["Groep"] = "Alle Views";
                }
            }
            Stack<DataRow> todel = new Stack<DataRow>();
            foreach (DataRow DDR3 in DT.Rows)
            {
                if (DDR3["Groep"].ToString().StartsWith("View"))
                {
                    todel.Push(DDR3);
                }
            }
            while (todel.Count > 0)
            {
                DataRow dr = todel.Pop();
                DT.Rows.Remove(dr);
            }
            ////
            return DT;
        }
        private void log_viewer_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.Column.Header.ToString())
            {
                case "Handeling": case "Soort":case "SortOrder": case "Groep": case "Volgorde": case "HandelingNr": case "qxz":
                    e.Cancel = true;
                    break;
                case "Handelingen": 
                    e.Column.Header = "";
                    break;
                case "HandelingNaam":
                    e.Column.Header = "";
                    e.Column.MinWidth = 115;
                    break;
            }
        }

        private void rb_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            switch (rb.Name)
            {
                case "rb_week":
                    date_forFilter = DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0));
                    break;
                case "rb_maand":
                    int DiM = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                    date_forFilter = DateTime.Now.Subtract(new TimeSpan(DiM,0,0,0));
                    break;
                case "rb_jaar":
                    DateTime tmp = DateTime.Now.Subtract(new TimeSpan(365, 0, 0, 0));
                    date_forFilter = tmp;
                    break;
                case "rb_all":
                    date_forFilter = date_forall;
                    break;

            }
            switch (stand)
            {
                case "Algemeen":
                    load_handelingen_algemeen();
                    break;
                case "Project":
                    load_handelingen_project();
                    break;
                case "Bedrijf":
                    load_handelingen_bedrijf();
                    break;
                case "Contact":
                    load_handelingen_contact();
                    break;
            }
        }
        private void Expander_Initialized(object sender, EventArgs e)
        {
            var exp = sender as Expander;
            var dc = exp.DataContext as CollectionViewGroup;
            if (_dict != null && _dict.ContainsKey(dc.Name.ToString()) && _dict[dc.Name.ToString()])
            {
                exp.IsExpanded = true;
            }
        }
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            var exp = sender as Expander;
            var dc = exp.DataContext as CollectionViewGroup;
            _dict[dc.Name.ToString()] = exp.IsExpanded;
        }

}
   public class HM_item
   {
       public int handeling_nr
       { get; set; }
       public int _count
       { get; set; }
       public string gebruiker
       { get; set; }
       public DateTime timestamp
       { get; set; }
       public int Order
       { get; set; }
       public int r_type
       { get; set; }
    }
   public class GL_mon_item
   {
       public int HandelingNummer
       { get; set; }
       public string Naam
       { get; set; }
       public string GroepNaam
       { get; set; }
       public List<KeyValuePair<string, int>> GebruikersCntr
       { get; set; }
       public int SortOrder
       { get; set; }
       public int H_Type
       { get; set; }
   }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeebregtsCs
{
    class taak_bepaler
    {
        private int _record_id = 0;
        private int _type = 0;
        bedrijfformdatasetTableAdapters.Get_afgl_functTableAdapter TaAdpt = new bedrijfformdatasetTableAdapters.Get_afgl_functTableAdapter();
        bedrijfformdatasetTableAdapters.bedrijfTableAdapter adapter = new bedrijfformdatasetTableAdapters.bedrijfTableAdapter();
        private void Calc_function(bool exist)
        {
            TaAdpt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            adapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            switch (_type)
            {
                case 2:
                    Calc_bdr(!exist);
                    break;
                case 3:
                    Calc_pers(!exist);
                    break;
            }
        }
        private void Calc_bdr(bool nieuw)
        {
            TaAdpt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            adapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            List<zeebregtsCs.RefItem> tmplst = new List<zeebregtsCs.RefItem>();
            ///////////////////
            int bid = _record_id;
            int result = 0;
            string column = "";
            string table = "";
            // zeebregtsCs.bedrijfformdatasetTableAdapters.QueriesTableAdapter qtadapt = new zeebregtsCs.bedrijfformdatasetTableAdapters.QueriesTableAdapter();
            zeebregtsCs.bedrijfformdatasetTableAdapters.bedrijf_nr_locatiesTableAdapter locaties_adapt = new zeebregtsCs.bedrijfformdatasetTableAdapters.bedrijf_nr_locatiesTableAdapter();
            locaties_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            zeebregtsCs.bedrijfformdataset.bedrijf_nr_locatiesDataTable locaties_table = locaties_adapt.GetData();
            zeebregtsCs.bedrijfformdatasetTableAdapters.verwijder_get_varbedrijf_functnaamTableAdapter vrgfadapt = new zeebregtsCs.bedrijfformdatasetTableAdapters.verwijder_get_varbedrijf_functnaamTableAdapter();
            vrgfadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            zeebregtsCs.bedrijfformdataset.verwijder_get_varbedrijf_functnaamDataTable varfuntdt = vrgfadapt.GetData(bid);
            foreach (zeebregtsCs.bedrijfformdataset.bedrijf_nr_locatiesRow row in locaties_table.Rows)
            {
                if (row.tabel == "bedrijvensetvariabel")
                {
                    foreach (zeebregtsCs.bedrijfformdataset.verwijder_get_varbedrijf_functnaamRow dr in varfuntdt.Rows)
                    {
                        if (dr.functie_ID != 11 && dr.functie_ID != 1)
                        {
                            tmplst.Add(new zeebregtsCs.RefItem { funct_naam = dr.naam, funct_count = dr.aantal, functie_id = dr.functie_ID, uit_var = true });
                        }
                    }
                }
                else
                {
                    column = row.column.ToString();
                    table = row.tabel.ToString();
                    result = (int)adapter.Check_del_bdr(bid, column, table);
                    if (result > 0)
                    {
                        bool voegtoe = true;
                        foreach (zeebregtsCs.RefItem RI in tmplst)
                        {
                            if (RI.funct_naam == row.weergavenaam)
                            {
                                RI.funct_count += result;
                                RI.functie_id = row.regel_nr;
                                RI.uit_var = false;
                                voegtoe = false;
                            }
                        }
                        if (voegtoe)
                        {
                            if (row.regel_nr != 7 && row.regel_nr != 6 && row.regel_nr != 3 && row.regel_nr != 2)
                            {
                                tmplst.Add(new zeebregtsCs.RefItem
                                {
                                    funct_naam = row.weergavenaam,
                                    funct_count = result,
                                    functie_id = row.regel_nr,
                                    uit_var = false
                                });
                            }
                        }
                    }
                }
            }
            // qtadapt.Dispose();
            locaties_adapt.Dispose();
            locaties_table.Dispose();
            //////////////////
            tmplst = tmplst.OrderByDescending(RefItem => RefItem.funct_count).ToList<zeebregtsCs.RefItem>();
            //////////////
            if (tmplst.Count == 1)
            {
                if (!nieuw)
                {
                    TaAdpt.del_agfl_funct(_record_id, 2);
                }
            }
            else if (tmplst.Count > 1)
            {
                if (nieuw)
                {
                    TaAdpt.new_agfl_funct(_record_id, 2, tmplst[0].functie_id, tmplst[0].uit_var);
                }
                else
                {
                    TaAdpt.update_agfl_funct(_record_id, 2, tmplst[0].functie_id, tmplst[0].uit_var);
                }
            }


        }
        private void Calc_pers(bool nieuw)
        {
            TaAdpt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            adapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            List<zeebregtsCs.RefItem> tmplst = new List<zeebregtsCs.RefItem>();
            ////////////////////
            int pid = _record_id;
            int result = 0;
            string column = "";
            string table = "";
            //zeebregtsCs.persoonsformdatasetTableAdapters.QueriesTableAdapter qtadapt = new zeebregtsCs.persoonsformdatasetTableAdapters.QueriesTableAdapter();
            zeebregtsCs.persoonsformdatasetTableAdapters.persoon_nr_locatiesTableAdapter locaties_adapt = new zeebregtsCs.persoonsformdatasetTableAdapters.persoon_nr_locatiesTableAdapter();
            locaties_adapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            zeebregtsCs.persoonsformdataset.persoon_nr_locatiesDataTable locaties_table = locaties_adapt.GetData();
            zeebregtsCs.persoonsformdatasetTableAdapters.var_pers_funct_countTableAdapter varcnttadapt = new zeebregtsCs.persoonsformdatasetTableAdapters.var_pers_funct_countTableAdapter();
            varcnttadapt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            zeebregtsCs.persoonsformdataset.var_pers_funct_countDataTable varcntdt = varcnttadapt.GetData(pid);
            foreach (zeebregtsCs.persoonsformdataset.persoon_nr_locatiesRow row in locaties_table.Rows)
            {
                if (row.column == "persoon_nr")
                {
                    foreach (zeebregtsCs.persoonsformdataset.var_pers_funct_countRow dr in varcntdt.Rows)
                    {
                        tmplst.Add(new zeebregtsCs.RefItem { funct_count = dr.aantal, funct_naam = dr.naam, functie_id = dr.functie_id, uit_var = true });
                    }
                }
                else
                {
                    column = row.column.ToString();
                    table = row.tabel.ToString();
                    result = (int)adapter.Check_del_pers(pid, column, table);

                    if (result > 0)
                    {
                        bool voegtoe = true;
                        foreach (zeebregtsCs.RefItem RI in tmplst)
                        {
                            if (RI.funct_naam == row.weergavenaam)
                            {
                                RI.funct_count += result;
                                RI.uit_var = false;
                                RI.functie_id = row.regel_nr;
                                voegtoe = false;
                            }
                        }
                        if (voegtoe)
                        {
                            tmplst.Add(new zeebregtsCs.RefItem
                            {
                                funct_naam = row.weergavenaam,
                                funct_count = result,
                                functie_id = row.regel_nr,
                                uit_var = false
                            });
                        }
                    }
                }
            }
            //qtadapt.Dispose();
            locaties_adapt.Dispose();
            locaties_table.Dispose();
            ///////////////////
            tmplst = tmplst.OrderByDescending(RefItem => RefItem.funct_count).ToList<zeebregtsCs.RefItem>();
			if (tmplst.Count == 1)
			{
				if (!nieuw)
				{
					TaAdpt.del_agfl_funct(_record_id, 3);
				}
			}
			else if (tmplst.Count > 0)
            {
                if (nieuw)
                {
                    TaAdpt.new_agfl_funct(_record_id, 3, tmplst[0].functie_id, tmplst[0].uit_var);
                }
                else
                {
                    TaAdpt.update_agfl_funct(_record_id, 3, tmplst[0].functie_id, tmplst[0].uit_var);
                }

            }
        }
        public void Recalc_function(int id, int tp)
        {
            TaAdpt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            adapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            _record_id = id;
            _type = tp;
            bool exists = false;
            int rslt = (int)TaAdpt.Check_afgl_exists(_record_id, _type);
            if (rslt > 0)
            {
                exists = true;
            }
            Calc_function(exists);
           
        }
        public String[] Get_function()
        {
            TaAdpt.Connection.ConnectionString = Global.ConnectionString_fileserver;
            adapter.Connection.ConnectionString = Global.ConnectionString_fileserver;
            string omsch_vrij = String.Empty;
            string omsch_afgl = String.Empty;

            bedrijfformdataset.Get_afgl_functDataTable afgl_dt = TaAdpt.GetData(_record_id, _type, ref omsch_vrij, ref omsch_afgl);
            if (afgl_dt.Rows.Count > 0)
            {
                bedrijfformdataset.Get_afgl_functRow afgl_row = (bedrijfformdataset.Get_afgl_functRow)afgl_dt.Rows[0];

                if (!afgl_row.Isomschrijving_vrijNull())
                {
                    omsch_vrij = afgl_row.omschrijving_vrij;
                }
                if (!afgl_row.Isafgeleide_omschrijvingNull())
                {
                    omsch_afgl = afgl_row.afgeleide_omschrijving;
                }
            }
            string[] results = { omsch_vrij, omsch_afgl };
            return results;
        }
    }
}

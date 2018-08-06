using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace zeebregtsCs
{
    public static class dubblinv_linq
    {
        private static List<string> ignores = new List<string>{ "woningen", "huur", "appartementen", "project", "kantoor", "koop", "tilburg", "won.", "app.","huurwoning","koopwoning","zorgwoning", "huurwoningen","koopwoningen", "zorgwoningen" };
        public static void Get_doublicates_dgv(int type, DataGridView dataGridView1 )
        {
             linq2sqlDataContext DB_data_context = new linq2sqlDataContext(Global.ConnectionString_fileserver);
            switch(type)
            {
            
                case 1:
                string proj_nm_tmp = Regex.Replace(Global.dub_proj_naamproj, @"\d", "");
                string[] proj_nm_delen = Regex.Split(proj_nm_tmp, @"\s");
                string proj_zoek_naam_project = String.Empty;
                string proj_zoek_naam_project2nd = String.Empty;
                int proj_max_length = 0;
                foreach (string str in proj_nm_delen)
                {
                    if (!ignores.Contains(str.ToLower()) && !Global.dub_proj_plaats.ToLower().Contains(str.ToLower()))
                    {
                        if (str.Length > proj_max_length && str.Length > 2)
                        {
                            proj_zoek_naam_project = str;
                            proj_max_length = str.Length;
                        }
                    }
                }
                proj_max_length = 0;
                foreach (string str in proj_nm_delen)
                {
                    if (!ignores.Contains(str.ToLower()) && !Global.dub_proj_plaats.ToLower().Contains(str.ToLower()))
                    {
                        if (str.Length > proj_max_length && str != proj_zoek_naam_project && str.Length>3)
                        {
                            proj_zoek_naam_project2nd = str;
                            proj_max_length = str.Length;
                        }
                    }
                }
                if (proj_zoek_naam_project2nd == String.Empty)
                {
                    proj_zoek_naam_project2nd = proj_zoek_naam_project;
                }
                int scheidslijn = 0;
                if ((proj_zoek_naam_project.Length % 2) == 0)
                {
                    scheidslijn = proj_zoek_naam_project.Length / 2;
                }
                else
                {
                    scheidslijn = (proj_zoek_naam_project.Length +1 )/2;
                }
                string proj_naam_part1 = proj_zoek_naam_project;
                string proj_naam_part2 = proj_zoek_naam_project;
                if (proj_zoek_naam_project.Length > 5)
                {
                    proj_naam_part1 = proj_zoek_naam_project.Substring(0, scheidslijn);
                    proj_naam_part2 = proj_zoek_naam_project.Substring(scheidslijn, proj_zoek_naam_project.Length - scheidslijn);
                }
                

                int scheidslijn2nd = 0;
                if ((proj_zoek_naam_project2nd.Length % 2) == 0)
                {
                    scheidslijn2nd = proj_zoek_naam_project2nd.Length / 2;
                }
                else
                {
                    scheidslijn2nd = (proj_zoek_naam_project2nd.Length + 1) / 2;
                }
                string proj_naam_part1_2nd = proj_zoek_naam_project2nd;
                string proj_naam_part2_2nd = proj_zoek_naam_project2nd;
                if (proj_zoek_naam_project2nd.Length > 5)
                {
                    proj_naam_part1_2nd = proj_zoek_naam_project2nd.Substring(0, scheidslijn2nd);
                    proj_naam_part2_2nd = proj_zoek_naam_project2nd.Substring(scheidslijn2nd, proj_zoek_naam_project2nd.Length - scheidslijn2nd);
                }
               var dub_projecten = from p in DB_data_context.projects
                                    join s in DB_data_context.status on p.status equals s.omschrijving_nr
                                    join b in DB_data_context.bedrijfs on p.opdrachtgeverZEEBREGTS_nr equals b.bedrijf_nr
                                    where
                                    (
                                       ( p.naam_project.Contains(proj_naam_part1)    ||  p.naam_project.Contains(proj_naam_part2) )
                                    || ( p.naam_project.Contains(proj_naam_part1_2nd)||  p.naam_project.Contains(proj_naam_part2_2nd))
                                    )
                                    &&
                                    (p.plaats == Global.dub_proj_plaats || p.opdrachtgeverZEEBREGTS_nr == Global.dub_proj_opdrachtgever)
                                    && (s.omschrijving_nr == 1 || s.omschrijving_nr == 3 || s.omschrijving_nr == 4 || s.omschrijving_nr == 6 || s.omschrijving_nr == 7 || s.omschrijving_nr == 8 || s.omschrijving_nr == 9 || s.omschrijving_nr == 11)
                                    orderby s.planning descending, s.volgorde ascending, b.naam ascending, p.plaats ascending
                                    select new DU_Item{ID =  int.Parse(p.project_NR.ToString()), zoek_col1 = p.naam_project,zoek_col2 = p.plaats, zoek_col3 = b.naam };



                dataGridView1.DataSource = dub_projecten;
                break;
                case 2:
                string bdr_naam_tmp = Regex.Replace(Global.dub_bdr_naam, @"\d", "");
                string[] bdr_naam_delen = Regex.Split(bdr_naam_tmp, @"\s");
                string zoek_bdr_naam = String.Empty;
                string zoek_bdr_naam2nd = String.Empty;
                int bdr_max_length = 0;
                foreach (string str in bdr_naam_delen)
                {
                    if (str.Length > bdr_max_length)
                    {
                        zoek_bdr_naam = str;
                        bdr_max_length = str.Length;
                    }
                }
                bdr_max_length = 0;
                foreach (string str in bdr_naam_delen)
                {
                    if (str.Length > bdr_max_length && str != zoek_bdr_naam)
                    {
                        zoek_bdr_naam2nd = str;
                        bdr_max_length = str.Length;
                    }
                }
                string bdr_zoeknaam_tmp = Regex.Replace(Global.dub_bdr_zoeknaam, @"\d", "");
                string[] bdr_zoeknaam_delen = Regex.Split(bdr_zoeknaam_tmp, @"\s");
                string zoek_bdr_zoeknaam = String.Empty;
                string zoek_bdr_zoeknaam2nd = String.Empty;
                bdr_max_length = 0;
                foreach (string str in bdr_zoeknaam_delen)
                {
                    if (str.Length > bdr_max_length)
                    {
                        zoek_bdr_zoeknaam = str;
                        bdr_max_length = str.Length;
                    }
                }
                bdr_max_length = 0;
                foreach (string str in bdr_zoeknaam_delen)
                {
                    if (str.Length > bdr_max_length && str != zoek_bdr_zoeknaam)
                    {
                        zoek_bdr_zoeknaam2nd = str;
                        bdr_max_length = str.Length;
                    }
                }
                var dub_bedrijven = from b in DB_data_context.bedrijfs
                                    where (b.postcode == Global.dub_bdr_postcode || b.postPOSTCODE == Global.dub_bdr_postcode)
                                    && ((b.naam.Contains(zoek_bdr_naam) || b.naam.Contains(zoek_bdr_naam2nd) )||( b.zoeknaam.Contains(zoek_bdr_zoeknaam) || b.zoeknaam.Contains(zoek_bdr_zoeknaam2nd)))
                                    orderby b.zoeknaam ascending, b.plaats ascending
                                    select new { b.bedrijf_nr,b.zoeknaam,b.plaats};
                dataGridView1.DataSource = dub_bedrijven;
                break;
                case 3:
                var dub_personen = DB_data_context.Dubbel_inv_list_pers(Global.dub_pers_voornaam, Global.dub_pers_voorletters, Global.dub_pers_achternaam, Global.dub_pers_bedrijf_nr, Global.dub_pers_man);
                dataGridView1.DataSource = dub_personen;    
                break;
            }
        }
        public static List<DU_Item> Get_doublicates_hxzr_proj(string projnaam, string plaats, int opdgeef)
        {
            List<DU_Item> retlist = new List<DU_Item>();
            linq2sqlDataContext DB_data_context = new linq2sqlDataContext(Global.ConnectionString_fileserver);
           
                string proj_nm_tmp = Regex.Replace(projnaam, @"\d", "");
                string[] proj_nm_delen = Regex.Split(proj_nm_tmp, @"\s");
                string proj_zoek_naam_project = String.Empty;
                string proj_zoek_naam_project2nd = String.Empty;
                int proj_max_length = 0;
                foreach (string str in proj_nm_delen)
                {
                    if (!ignores.Contains(str.ToLower()) && !plaats.ToLower().Contains(str.ToLower()))
                    {
                        if (str.Length > proj_max_length && str.Length > 2)
                        {
                            proj_zoek_naam_project = str;
                            proj_max_length = str.Length;
                        }
                    }
                }
                proj_max_length = 0;
                foreach (string str in proj_nm_delen)
                {
                    if (!ignores.Contains(str.ToLower()) && !plaats.ToLower().Contains(str.ToLower()))
                    {
                        if (str.Length > proj_max_length && str != proj_zoek_naam_project && str.Length>3)
                        {
                            proj_zoek_naam_project2nd = str;
                            proj_max_length = str.Length;
                        }
                    }
                }
                if (proj_zoek_naam_project2nd == String.Empty)
                {
                    proj_zoek_naam_project2nd = proj_zoek_naam_project;
                }
                int scheidslijn = 0;
                if ((proj_zoek_naam_project.Length % 2) == 0)
                {
                    scheidslijn = proj_zoek_naam_project.Length / 2;
                }
                else
                {
                    scheidslijn = (proj_zoek_naam_project.Length +1 )/2;
                }
                string proj_naam_part1 = proj_zoek_naam_project;
                string proj_naam_part2 = proj_zoek_naam_project;
                if (proj_zoek_naam_project.Length > 5)
                {
                    proj_naam_part1 = proj_zoek_naam_project.Substring(0, scheidslijn);
                    proj_naam_part2 = proj_zoek_naam_project.Substring(scheidslijn, proj_zoek_naam_project.Length - scheidslijn);
                }
                

                int scheidslijn2nd = 0;
                if ((proj_zoek_naam_project2nd.Length % 2) == 0)
                {
                    scheidslijn2nd = proj_zoek_naam_project2nd.Length / 2;
                }
                else
                {
                    scheidslijn2nd = (proj_zoek_naam_project2nd.Length + 1) / 2;
                }
                string proj_naam_part1_2nd = proj_zoek_naam_project2nd;
                string proj_naam_part2_2nd = proj_zoek_naam_project2nd;
                if (proj_zoek_naam_project2nd.Length > 5)
                {
                    proj_naam_part1_2nd = proj_zoek_naam_project2nd.Substring(0, scheidslijn2nd);
                    proj_naam_part2_2nd = proj_zoek_naam_project2nd.Substring(scheidslijn2nd, proj_zoek_naam_project2nd.Length - scheidslijn2nd);
                }
               var dub_projecten = from p in DB_data_context.projects
                                    join s in DB_data_context.status on p.status equals s.omschrijving_nr
                                    join b in DB_data_context.bedrijfs on p.opdrachtgeverZEEBREGTS_nr equals b.bedrijf_nr
                                    where
                                    (
                                       ( p.naam_project.Contains(proj_naam_part1)    ||  p.naam_project.Contains(proj_naam_part2) )
                                    || ( p.naam_project.Contains(proj_naam_part1_2nd)||  p.naam_project.Contains(proj_naam_part2_2nd))
                                    )
                                    &&
                                    (p.plaats == plaats || p.opdrachtgeverZEEBREGTS_nr == opdgeef)
                                    && (s.omschrijving_nr == 1 || s.omschrijving_nr == 3 || s.omschrijving_nr == 4 || s.omschrijving_nr == 6 || s.omschrijving_nr == 7 || s.omschrijving_nr == 8 || s.omschrijving_nr == 9 || s.omschrijving_nr == 11)
                                    orderby s.planning descending, s.volgorde ascending, b.naam ascending, p.plaats ascending
                                    select new DU_Item{ID =  int.Parse(p.project_NR.ToString()), zoek_col1 = p.naam_project,zoek_col2 = p.plaats, zoek_col3 = b.naam };


                  return  retlist = dub_projecten.ToList<DU_Item>();
        }
        public static List<DU_Item> Get_doublicates_hxzr_bdr(string bdrnaam,string bdrzoeknaam, string postcode)
        {
            List<DU_Item> retlist = new List<DU_Item>();
            linq2sqlDataContext DB_data_context = new linq2sqlDataContext(Global.ConnectionString_fileserver);
           
            string bdr_naam_tmp = Regex.Replace(bdrnaam, @"\d", "");
            string[] bdr_naam_delen = Regex.Split(bdr_naam_tmp, @"\s");
            string zoek_bdr_naam = String.Empty;
            string zoek_bdr_naam2nd = String.Empty;
            int bdr_max_length = 0;
            foreach (string str in bdr_naam_delen)
            {
                if (str.Length > bdr_max_length)
                {
                    zoek_bdr_naam = str;
                    bdr_max_length = str.Length;
                }
            }
            bdr_max_length = 0;
            foreach (string str in bdr_naam_delen)
            {
                if (str.Length > bdr_max_length && str != zoek_bdr_naam)
                {
                    zoek_bdr_naam2nd = str;
                    bdr_max_length = str.Length;
                }
            }
            string bdr_zoeknaam_tmp = Regex.Replace(bdrzoeknaam, @"\d", "");
            string[] bdr_zoeknaam_delen = Regex.Split(bdr_zoeknaam_tmp, @"\s");
            string zoek_bdr_zoeknaam = String.Empty;
            string zoek_bdr_zoeknaam2nd = String.Empty;
            bdr_max_length = 0;
            foreach (string str in bdr_zoeknaam_delen)
            {
                if (str.Length > bdr_max_length)
                {
                    zoek_bdr_zoeknaam = str;
                    bdr_max_length = str.Length;
                }
            }
            bdr_max_length = 0;
            foreach (string str in bdr_zoeknaam_delen)
            {
                if (str.Length > bdr_max_length && str != zoek_bdr_zoeknaam)
                {
                    zoek_bdr_zoeknaam2nd = str;
                    bdr_max_length = str.Length;
                }
            }
            var dub_bedrijven = from b in DB_data_context.bedrijfs
                                where (b.postcode == postcode || b.postPOSTCODE == postcode)
                                && ((b.naam.Contains(zoek_bdr_naam) || b.naam.Contains(zoek_bdr_naam2nd)) || (b.zoeknaam.Contains(zoek_bdr_zoeknaam) || b.zoeknaam.Contains(zoek_bdr_zoeknaam2nd)))
                                orderby b.zoeknaam ascending, b.plaats ascending
                                select new DU_Item { ID = int.Parse(b.bedrijf_nr.ToString()), zoek_col1 = b.zoeknaam, zoek_col2 = b.plaats };
         retlist = dub_bedrijven.ToList<DU_Item>();
         return retlist;
        }
    }//
}

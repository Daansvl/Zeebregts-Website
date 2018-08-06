using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace zeebregtsCs
{
    class Form_helper
    {
        int m_type = -1;
        int m_route= -1; 
        int m_Parent_ID = -1;
        base_form m_start_scherm;
        base_form m_pa_scherm;
       String m_veldnaam;
        String m_ownernaam;
        
        public void Start_route(int type,int route, base_form start_scherm, base_form pa_scherm ,int Parent_ID, String veldnaam, String ownernaam)
        {
            
            m_type = type;
            m_route = route;
            m_start_scherm = start_scherm;
            m_pa_scherm = pa_scherm;
            m_Parent_ID = Parent_ID;
            m_veldnaam = veldnaam;
            m_ownernaam = ownernaam;

            switch(route)
            {
                case 0:// normaal
                    switch (m_type)
                    {
                        case 1://project
                            Norm_proj();
                            break;
                        case 2://bedrijf
                            Norm_bdr();
                            break;
                        case 3://persoon
                            Norm_pers();
                            break;
                    }
                    break; 
                case 1:// rood
                    switch (m_type)
                    {
                        case 1://project
                            Rood_proj();
                            break;
                        case 2://bedrijf
                            Rood_bdr();
                            break;
                        case 3://persoon
                            Rood_pers();
                            break;
                    }
                    break; 
                case 2:// groen
                    switch (m_type)
                    {
                        case 1://project
                            Groen_proj();
                            break;
                        case 2://bedrijf
                            Groen_bdr();
                            break;
                        case 3://persoon
                            Groen_pers();
                            break;
                    }
                    break; 
                case 3://menu>listview
                    Norm_ov();
                    break;
                case 4://route *, nieuw-aanmaken
                    Open_record();
                    break;
                case 5://dubbelinvoer listv
                    Dubbel_invoer();
                    break;
                case 6://dubbel_inv dublicate found
                    Dubbel_found();
                    break;

            }
        }
        public void Norm_ov()//type * route 3
        {
            if (m_start_scherm is persoon_form)
            {
                overview1 mn_ov = new overview1(m_type, m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 3);
                mn_ov.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                mn_ov.WindowState = Global.windowstate;
                if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                {
                    mn_ov.Location = Global.position;
                    mn_ov.Size = Global.size;
                }
                mn_ov.Show();
            }
            else if (m_start_scherm is project_form)
            {
                overview1 mn_ov = new overview1(m_type, m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 2);
                mn_ov.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                mn_ov.WindowState = Global.windowstate;
                if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                {
                    mn_ov.Location = Global.position;
                    mn_ov.Size = Global.size;
                }
                mn_ov.Show();
            }
            else if (m_start_scherm is newrecord)
            {
                overview1 mn_ov = new overview1(m_type, m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 1);
                mn_ov.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                mn_ov.WindowState = Global.windowstate;
                if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                {
                    mn_ov.Location = Global.position;
                    mn_ov.Size = Global.size;
                }
                mn_ov.Show();
            }
            else if (m_start_scherm is Menu)
            {
                overview1 mn_ov = new overview1(m_type, m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 0);
                mn_ov.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                mn_ov.WindowState = Global.windowstate;
                if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                {
                    mn_ov.Location = Global.position;
                    mn_ov.Size = Global.size;
                }
                mn_ov.Show();
            }
            else if (m_start_scherm is bedrijf_form)
            {
                overview1 mn_ov = new overview1(m_type, m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 6);
                mn_ov.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                mn_ov.WindowState = Global.windowstate;
                if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                {
                    mn_ov.Location = Global.position;
                    mn_ov.Size = Global.size;
                }
                mn_ov.Show();
            }
            
        }
        public void Norm_proj()//(1,0)
        {
            try
            {
                project_form project_form1 = new project_form(m_start_scherm, m_pa_scherm, 0);
                project_form1.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                project_form1.WindowState = Global.windowstate;
                if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                {
                    project_form1.Location = Global.position;
                    project_form1.Size = Global.size;
                }
                
                project_form1.Show();
            }
            catch (ObjectDisposedException ODE)
            {
                String log_line = "Exception occurred @ " + DateTime.Now.ToString() + " error: " + ODE;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
            }
        }
        public void Norm_bdr()//(2,0)
        {
            try
            {
                bedrijf_form bedrijf_form1 = new bedrijf_form(m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 0);
                bedrijf_form1.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                bedrijf_form1.WindowState = Global.windowstate;
                if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                {
                    bedrijf_form1.Location = Global.position;
                    bedrijf_form1.Size = Global.size;
                }
                bedrijf_form1.Show();
            }
            catch (System.ObjectDisposedException ODE)
            {
                String log_line = "Exception occurred @ " + DateTime.Now.ToString() + " error: " + ODE;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
            }
        }
        public void Norm_pers()//(3,0)
        {
            try
            {
                persoon_form persoon_form1 = new persoon_form(m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 0);
                persoon_form1.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                persoon_form1.WindowState = Global.windowstate;
                if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                {
                    persoon_form1.Location = Global.position;
                    persoon_form1.Size = Global.size;
                }
                persoon_form1.Show();
            }
            catch (System.ObjectDisposedException ODE)
            {
                String log_line = "crash program @ " + DateTime.Now.ToString() + " error: " + ODE;
                System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                file.WriteLine(log_line);
                file.Close();
            }
        }

        public void Rood_proj()//(1,1)
        {
         newrecord nwr = new newrecord(1, m_start_scherm, m_pa_scherm,m_Parent_ID, m_veldnaam, m_ownernaam);
         nwr.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            nwr.WindowState = Global.windowstate;
         if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
         {
             nwr.Location = Global.position;
             nwr.Size = Global.size;
         }
            nwr.Show();
        }
        public void Rood_bdr()
        {
            newrecord nwr = new newrecord(2, m_start_scherm, m_pa_scherm,m_Parent_ID, m_veldnaam, m_ownernaam);
            nwr.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            nwr.WindowState = Global.windowstate;
            if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
            {
                nwr.Location = Global.position;
                nwr.Size = Global.size;
            }
            nwr.Show();
        }
        public void Rood_pers()
        {
            newrecord nwr = new newrecord(3, m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam);
            nwr.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            nwr.WindowState = Global.windowstate;
            if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
            {
                nwr.Location = Global.position;
                nwr.Size = Global.size;
            }
            nwr.Show();
        }

        public void Groen_proj()
        {

        }
        public void Groen_bdr()
        {
            bedrijf_form bdrf = new bedrijf_form(m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 3);
            bdrf.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            bdrf.WindowState = Global.windowstate;
            if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
            {
                bdrf.Location = Global.position;
                bdrf.Size = Global.size;
            }
            bdrf.Show();
        }
        public void Groen_pers()
        {
            persoon_form prsf = new persoon_form(m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 3);
            prsf.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            prsf.WindowState = Global.windowstate;
            if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
            {
                prsf.Location = Global.position;
                prsf.Size = Global.size;
            }
            prsf.Show();
        }
        public void Open_record()
        {
            switch (m_type)
            {
                case 1:
                    project_form prf = new project_form(m_start_scherm, m_pa_scherm, 1);
                    prf.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                    prf.WindowState = Global.windowstate;
                    if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                    {
                        prf.Location = Global.position;
                        prf.Size = Global.size;
                    }
                    prf.Show();
                    break;
                case 2:
                    if (m_ownernaam != "")
                    {
                        bedrijf_form bdrf = new bedrijf_form(m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 2);
                        bdrf.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                        bdrf.WindowState = Global.windowstate;
                        if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                        {
                            bdrf.Location = Global.position;
                            bdrf.Size = Global.size;
                        }
                        bdrf.Show();
                    }
                    else
                    {
                        bedrijf_form bdrf = new bedrijf_form(m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 1);
                        bdrf.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                        bdrf.WindowState = Global.windowstate;
                        if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                        {
                            bdrf.Location = Global.position;
                            bdrf.Size = Global.size;
                        }
                        bdrf.Show();
                    }
                    
                    break;
                case 3:
                    if (m_ownernaam != "")
                    { 
                        persoon_form persf = new persoon_form(m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 2);
                        persf.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                        persf.WindowState = Global.windowstate;
                        if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                        {
                            persf.Location = Global.position;
                            persf.Size = Global.size;
                        }
                        persf.Show();
                    }
                    else
                    {
                        persoon_form persf = new persoon_form(m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 1);
                        persf.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                        persf.WindowState = Global.windowstate;
                        if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                        {
                            persf.Location = Global.position;
                            persf.Size = Global.size;
                        }
                        persf.Show();
                    }
                    
                    break;
            }
        }
        private void Dubbel_invoer()
        {
            switch (m_type)
            {
                case 1:
                    overview1 overv1 = new overview1(1, m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 4);
                    overv1.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                    overv1.WindowState = Global.windowstate;
                        if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                        {
                            overv1.Location = Global.position;
                            overv1.Size = Global.size;
                        }
                    overv1.Show();
                    break;
                case 2:
                    overview1 overv2 = new overview1(2, m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 4);
                    overv2.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                    overv2.WindowState = Global.windowstate;
                        if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                        {
                            overv2.Location = Global.position;
                            overv2.Size = Global.size;
                        }
                    overv2.Show();
                    break;
                case 3:
                    overview1 overv3 = new overview1(3, m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 4);
                    overv3.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                    overv3.WindowState = Global.windowstate;
                        if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                        {
                            overv3.Location = Global.position;
                            overv3.Size = Global.size;
                        }
                    overv3.Show();
                    break;
            }
        }
        private void Dubbel_found()
        {
            switch (m_type)
            {
                case 1://project
                    project_form pfrm = new project_form(m_start_scherm, m_pa_scherm, 4);
                    pfrm.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                    pfrm.WindowState = Global.windowstate;
                        if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                        {
                            pfrm.Location = Global.position;
                            pfrm.Size = Global.size;
                        }
                    pfrm.Show();
                    break;
                case 2://bedrijf
                    bedrijf_form bfrm = new bedrijf_form(m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 4);
                    bfrm.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                    bfrm.WindowState = Global.windowstate;
                        if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                        {
                            bfrm.Location = Global.position;
                            bfrm.Size = Global.size;
                        }
                    bfrm.Show();
                    break;
                case 3://contact
                    persoon_form prfrm = new persoon_form(m_start_scherm, m_pa_scherm, m_Parent_ID, m_veldnaam, m_ownernaam, 4);
                    prfrm.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                    prfrm.WindowState = Global.windowstate;
                        if (Global.windowstate != System.Windows.Forms.FormWindowState.Maximized)
                        {
                            prfrm.Location = Global.position;
                            prfrm.Size = Global.size;
                        }
                    prfrm.Show();
                    break;
            }
        }
    
    }
   
}

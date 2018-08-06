using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Linq;
using System.Linq.Expressions;
namespace zeebregtsCs
{
 public static class FormManager
    {
     private static OrderedDictionary Forms_OD = new OrderedDictionary();
     private static Stack<base_form> KillStack = new Stack<base_form>();
     private static IDictionaryEnumerator DE;



        public static Menu GetMenu()
        {
            return (Menu)Forms_OD[0];
        }
        public static void VoegToe(base_form huidig, base_form sluit_naar)
        {
            Forms_OD.Add(huidig, sluit_naar);
            String log_line = "open venster @ " + DateTime.Now.ToString() + ": huidig: " + huidig.ToString() + ": close_naar: " + sluit_naar.ToString();
            System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
            file.WriteLine(log_line);
            file.Close();
        }
        public static void Sluit_form(base_form enkel_sluit)
        {
            base_form tmpfrm;
            if (Forms_OD.Contains(enkel_sluit))
            {
              tmpfrm = (base_form)Forms_OD[enkel_sluit];
                enkel_sluit.program_closes = true;
                enkel_sluit.Close();
                Forms_OD.Remove(enkel_sluit);
               
                String log_line = "sluit venster @ " + DateTime.Now.ToString() + ": huidig: " + enkel_sluit.ToString() + ": close_naar: " + tmpfrm.ToString();
            System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
            file.WriteLine(log_line);
            file.Close();
            }
           
        }
        public static void Sluit_forms(base_form sluit_vanaf)
        {
            DE = Forms_OD.GetEnumerator();
            int index = 0;
            bool gevonden = false;
            base_form herlaad_scherm = new base_form();
            while (DE.MoveNext())
            {
                if (DE.Key == sluit_vanaf && !gevonden)
                {

                    herlaad_scherm = (base_form)Forms_OD[index];
                    KillStack.Push((base_form)DE.Key);
                    gevonden = true;
                }
                else if (gevonden)
                {
                    KillStack.Push((base_form)DE.Key);
                }
                
                ++index;
            }
            int KillCount = KillStack.Count;
            for(int j = 0 ; j < KillCount; j++)
            {
                base_form tmp = KillStack.Pop();
               
                if (Sluitbaar(tmp))
                {
                    Forms_OD.Remove(tmp);
                    tmp.program_closes = true;
                    tmp.Close();
                    String log_line = "sluit venster @ " + DateTime.Now.ToString() + ": huidig: " + tmp.ToString() + ": close_naar: " + herlaad_scherm.ToString();
                    System.IO.StreamWriter file = new System.IO.StreamWriter(Global.log_file_path, true);
                    file.WriteLine(log_line);
                    file.Close();
                }
                else
                {
                    herlaad_scherm = tmp;
                    break;
                }
            }
        
            if (gevonden)
            {
                herlaad_scherm.herlaad();
            }
            KillStack.Clear();
            
        }
        private static bool Sluitbaar(base_form bf)
        {
            if (bf.kan_sluiten())
            {
                return true;
            }
            else
            {
                 DialogResult dr = MessageBox.Show("Uw wijzigingen gaan verloren. Doorgaan?", "sluiten", MessageBoxButtons.OKCancel);
                 if (dr == DialogResult.Cancel)
                 {
                     return false;
                 }
                 else
                 {
                     return true ;
                     
                 }
            }
        }
       
    }
}

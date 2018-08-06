using eBrochure_zeebregts.Classes;
using System;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Collections.Generic;
namespace eBrochure_zeebregts.Helpers
{
    public class XmlChecker
    {
        public void CheckAllSaves()
        {
            var ctx = Acumulator.Instance().ctx;
            
            ctx.Load(ctx.GetOpgeslagenOfferteByIdQuery(Acumulator.Instance().Bouwnr)).Completed += (sender, arg) =>
                {
                    string result = "resultaat vergelijking : " + Environment.NewLine;
                    var saves = (from oo in ctx.OpgeslagenOffertes
                                 where oo.Bouwnummer_NR==Acumulator.Instance().Bouwnr
                                 orderby oo.timestamp
                                 select oo.Xml_Value).ToList();
                    List<KeyValuePair<int, int>> done = new List<KeyValuePair<int, int>>();
                    for (int i = 0; i <=saves.Count()-1; i++)
                    {
                        for (int j = 0; j <= saves.Count() - 1; j++)
                        {
                            if (j != i && !done.Contains(new KeyValuePair<int,int>(i,j))&& !done.Contains(new KeyValuePair<int,int>(j,i)))
                            {
                                if (XmlMatch(saves[i], saves[j]))
                                {
                                    result += "xml van " + (i + 1) + " en " + (j + 1) + " zijn gelijk" + Environment.NewLine;
                                }
                                else
                                {
                                    result += "xml van " + (i + 1) + " en " + (j + 1) + " zijn ongelijk" + Environment.NewLine;
                                }
                                done.Add(new KeyValuePair<int,int>(i,j));
                            }
                        }
                    }
                    MessageBox.Show(result);
                };
        }
        private bool XmlMatch(string xml1, string xml2)
        {
            var retval = false;
            var curxml = xml1;
            curxml = NormalizeXml(curxml);
            var savedxml = xml2;
            savedxml = NormalizeXml(savedxml);
           /* for (int i = 0; i < curxml.Length; i++)
            {
                if (curxml[i] != savedxml[i])
                {
                    var totc = curxml.Substring(i);
                    var tots = savedxml.Substring(i);
                    break;
                }
            }*/
            var hash_curxml = CalculateMD5Hash(curxml);
            var hash_savedxml = CalculateMD5Hash(savedxml);
            retval = String.Equals(hash_curxml, hash_savedxml, StringComparison.Ordinal);
           


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
    }
}

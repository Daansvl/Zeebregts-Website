using System.Collections.Generic;
using eBrochure_zeebregts.Classes;
using System.Linq;
using eBrochure_zeebregts.Web;
using System;

namespace eBrochure_zeebregts.Helpers
{
	public class CustomComparer<T>:IComparer<T>
	{
		int IComparer<T>.Compare(T a, T b)
		{
			OptieKeuze opta = a as OptieKeuze;
			OptieKeuze optb = b as OptieKeuze;
			Classes.SubCatPerPakket scppA = a as Classes.SubCatPerPakket;
			Classes.SubCatPerPakket scppB = b as Classes.SubCatPerPakket;
			Product prodA = a as Product;
			Product prodB = b as Product;
            BouwnummerItem bnrA = a as BouwnummerItem;
            BouwnummerItem bnrB = b as BouwnummerItem;
			HoekProfiel hoekA = a as HoekProfiel;
			HoekProfiel hoekB = b as HoekProfiel;
            PakketKeuze pakA = a as PakketKeuze;
            PakketKeuze pakB = b as PakketKeuze;
            Bouwnummers BnrsA = a as Bouwnummers;
            Bouwnummers BnrsB = b as Bouwnummers;
			if (opta != null && optb != null)
			{
                
				var ctx = Acumulator.Instance().ctx;

                var rdVolgA = (from rd in ctx.RuimteDelens
                               where rd.R_ID == opta.OptieID
                               select rd.volgorde).FirstOrDefault();

                var rdVolgB = (from rd in ctx.RuimteDelens
                               where rd.R_ID == optb.OptieID
                               select rd.volgorde).FirstOrDefault();
                if (rdVolgA != null && rdVolgB != null)
                {
                    int ia = (int)rdVolgA;
                    var ib = (int)rdVolgB;
                    if (ia > ib)
                        return 1;
                    if (ia < ib)
                        return -1;
                    else
                        return 0;
                }
                else
                {
                    var maxmeters = (from spr in ctx.SubCatPerRuimteDeels
                                     where spr.R_NR == opta.OptieID && spr.SCB_NR != "SCB4"
                                     select new KeyValuePair<string, double>(spr.SCB_NR, (double)spr.meters)).ToList();
                    var max = 0.0;
                    var scbA = "";
                    foreach (var x in maxmeters)
                    {
                        var getal = Math.Abs(x.Value);
                        if (getal > max)
                        {
                            max = getal;
                            scbA = x.Key;
                        }
                    }

                    /*var subcpnrA = (from spr in ctx.SubCatPerRuimteDeels
                                    where spr.R_NR == opta.OptieID
                                    select spr.SCB_NR).FirstOrDefault();*/
                    int volgnrA;
                    if (scbA != null)
                    {
                        volgnrA = (int)(from sc in ctx.SubCats
                                        where sc.SCB_ID == scbA
                                        select sc.volgorde).FirstOrDefault();
                    }
                    else
                    {
                        volgnrA = 9999;
                    }

                    var maxmetersB = (from spr in ctx.SubCatPerRuimteDeels
                                      where spr.R_NR == opta.OptieID && spr.SCB_NR != "SCB4"
                                      select new KeyValuePair<string, double>(spr.SCB_NR, (double)spr.meters)).ToList();
                    var maxB = 0.0;
                    var scbB = "";
                    foreach (var x in maxmetersB)
                    {
                        var getal = Math.Abs(x.Value);

                        if (getal > maxB)
                        {
                            maxB = getal;
                            scbB = x.Key;
                        }
                    }

                    /*var subcpnrB = (from spr in ctx.SubCatPerRuimteDeels
                                    where spr.R_NR == optb.OptieID
                                    select spr.SCB_NR).FirstOrDefault();*/
                    int volgnrB;
                    if (scbB != null)
                    {
                        volgnrB = (int)(from sc in ctx.SubCats
                                        where sc.SCB_ID == scbB
                                        select sc.volgorde).FirstOrDefault();
                    }
                    else
                    {
                        volgnrB = 9999;
                    }
                    int ia = volgnrA;
                    var ib = volgnrB;
                    if (ia > ib)
                        return 1;
                    if (ia < ib)
                        return -1;
                    else
                        return 0;
                }

			}
			else if(scppA != null && scppB != null)
			{
				if (scppA != null && scppB != null)
				{
					var ctx = Acumulator.Instance().ctx;
					var subcnrA = (from sc in ctx.SubCats
								   where sc.SCB_ID == scppA.SubCatNR
								   select sc.volgorde).FirstOrDefault();
					int volgA = (int)subcnrA;

					var subcnrB = (from sc in ctx.SubCats
								   where sc.SCB_ID == scppB.SubCatNR
								   select sc.volgorde).FirstOrDefault();
					int volgB = (int)subcnrB;
					if (volgA > volgB)
						return 1;
					if (volgA < volgB)
						return -1;
					else
						return 0;
				}
				else
				{
					return 0;
				}
			}
			else if(prodA != null && prodB != null)
			{
				int maatA = prodA.Lengte + prodA.Breedte;
				int maatB = prodB.Lengte + prodB.Breedte;
				int volgA = prodA.kleurVolgorde;
				int volgB = prodB.kleurVolgorde;
				if (maatA > maatB)
					return 1;
				if (maatA < maatB)
					return -1;
				else
				{
					if (volgA > volgB)
						return 1;
					if (volgA < volgB)
						return -1;
					else
						return 0;
				}
				
			}
			else if (bnrA != null && bnrB != null)
			{
				
				int ia = int.Parse(bnrA.B_ID.TrimStart('B'));
				int ib = int.Parse(bnrB.B_ID.TrimStart('B'));
				if (ia > ib)
					return 1;
				if (ia < ib)
					return -1;
				else
					return 0;
			}
			else if (hoekA != null && hoekB != null)
			{
				int volgA = hoekA.KleurVolgorde;
				int volgB = hoekB.KleurVolgorde;
				if (hoekA.ProfielType == hoekB.ProfielType)
				{
					if (volgA > volgB)
						return 1;
					if (volgA < volgB)
						return -1;
					else
						return 0;
				}
				else if (hoekA.ProfielType == "Recht")
				{
					return -1;
				}
				else
				{
					return 1;
				}
			}
            else if(BnrsA != null && BnrsB != null)
            {
                var ia = int.Parse(BnrsA.B_ID.TrimStart('B'));
                var ib = int.Parse(BnrsB.B_ID.TrimStart('B'));
                if(ia>ib)
                { return 1; }
                if(ia < ib)
                { return -1; }
                else { return 0; }
            }
            else if (pakA != null && pakB != null)
            {
                var ctx = Acumulator.Instance().ctx;
                var pgo_a = (from pgo in ctx.PakketGroepOpbouws
                             join br in ctx.Bron_Ruimtes on pgo.PG_NR equals br.PG_NR
                             where br.R_NR == Acumulator.Instance().BB.HuidigRuimte.RuimteID && pgo.P_NR == pakA.Pakket_ID
                             select pgo.PGO_ID).FirstOrDefault();
                var pgo_b = (from pgo in ctx.PakketGroepOpbouws
                             join br in ctx.Bron_Ruimtes on pgo.PG_NR equals br.PG_NR
                             where br.R_NR == Acumulator.Instance().BB.HuidigRuimte.RuimteID && pgo.P_NR == pakB.Pakket_ID
                             select pgo.PGO_ID).FirstOrDefault();

                var SA = pgo_a.Split('O');
                var IA = int.Parse(SA[1]);
                var SB = pgo_b.Split('O');
                var IB = int.Parse(SB[1]);

                if (IA > IB)
                    return 1;
                else if (IA < IB)
                    return -1;
                else
                    return 0;

            }
            else
            {
                return 0;
            }
				
			}
			
	}
	public class UitvoerComparer<T> : IComparer<T>
	{
		int IComparer<T>.Compare(T a, T b)
		{
			var prodA = a as Product;
			var prodB = b as Product;
			var ctx = Acumulator.Instance().ctx;
			if (prodA != null && prodB != null)
			{
				var scnrA = (from scp in ctx.SubCatPerPakkets
							 where scp.SCBP_ID == prodA.LinkedSubCat
							 select scp.SCB_NR).FirstOrDefault();
				var scnrB = (from scp in ctx.SubCatPerPakkets
							 where scp.SCBP_ID == prodB.LinkedSubCat
							 select scp.SCB_NR).FirstOrDefault();
				var volgA = (int)(from sc in ctx.SubCats
							 where sc.SCB_ID == scnrA
							 select sc.volgorde).FirstOrDefault();
				var volgB = (int)(from sc in ctx.SubCats
							 where sc.SCB_ID == scnrB
							 select sc.volgorde).FirstOrDefault();

				if (volgA > volgB)
					return 1;
				if (volgA < volgB)
					return -1;
				else
					return 0;

			}
			else
				return 0;
		}
	}
	
}

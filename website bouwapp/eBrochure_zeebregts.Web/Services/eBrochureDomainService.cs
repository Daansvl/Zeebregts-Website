
namespace eBrochure_zeebregts.Web.Services
{
	using System;
	using System.Linq;
	using System.ServiceModel.DomainServices.EntityFramework;
	using System.ServiceModel.DomainServices.Hosting;
	using System.ServiceModel.DomainServices.Server;
	using eBrochure_zeebregts.Web;
	using System.Web.Services;
    using System.Data.Objects.DataClasses;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using System.ServiceModel;


	// Implements application logic using the eBrochureDBEntities context.
	// TODO: Add your application logic to these methods or in additional methods.
	// TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
	// Also consider adding roles to restrict access as appropriate.
	// [RequiresAuthentication]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, AddressFilterMode = AddressFilterMode.Any)]
	[EnableClientAccess()]
	public class eBrochureDomainService : LinqToEntitiesDomainService<eBrochureDBEntities>
	{
        [WebMethod]
        public DateTime GetServerTime()
        {
            return DateTime.Now;
        }
		
        public bool ValidLoginCombo(string name, string password)
		{
            try
            {
                var gebruiker = this.ObjectContext.Gebruikers.Where(g => g.Naam == name && g.Wachtwoord == password).FirstOrDefault();
                if (gebruiker != null)
                {
                    if (gebruiker.GeldigTot == null)
                    {
                        return true;
                    }
                    else if (DateTime.Compare(DateTime.Now.Subtract(DateTime.Now.TimeOfDay), (DateTime)gebruiker.GeldigVan) >= 0 && DateTime.Compare(DateTime.Now.Subtract(DateTime.Now.TimeOfDay), (DateTime)gebruiker.GeldigTot) <= 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                //var p = new Exception("First test exception");
                throw ex;
            }

		}
		
        [WebMethod]
		public void LogEndSession(string user)
		{
			var sl = new ServerLog();
			var MSG = @"[ACTIVITY]-";
			MSG += @"-[MSG]- " + "User Logout - "+user + " -[END]";
			sl.logmsg = MSG;
			sl.TimeStamp = DateTime.Now;
			InsertLog(sl);
		}
		
        [WebMethod]
		public void LogUserIP(string uip, string user)
		{
			var sl = new ServerLog();
			var MSG = @"[Status]-";
			MSG += @"-[MSG]- " + user +"using IP:"+uip+ " -[END]";
			sl.logmsg = MSG;
			sl.TimeStamp = DateTime.Now;
			InsertLog(sl);
		}
		
        [Insert]
		public void InsertLog(ServerLog sl)
		{
			this.ObjectContext.AddToServerLog(sl);
		}
		
        public bool DoesUserExist(string name)
		{
			return this.ObjectContext.Gebruikers.Where(g => g.Naam == name).First() != null;
		}
		
        public Gebruikers GetGebruiker(string name)
		{
			return this.ObjectContext.Gebruikers.Where(g => g.Naam == name).First();
		}
		
        [Invoke]
		public string GetClientAddres()
		{
			return System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
		}
		
        [Insert]
		public void InsertGebruiker(Gebruikers g)
		{
			this.ObjectContext.AddToGebruikers(g);
		}
		
        [Insert]
		public void InsertProjectenSets(ProjectenSets ps)
		{
			ObjectContext.AddToProjectenSets(ps);
		}
		
        [Insert]
		public void InsertProjectenSetOpbouw(ProjectenSetOpbouw pso)
		{
			ObjectContext.AddToProjectenSetOpbouw(pso);
		}
		
        [Insert]
		public void InsertBouwnummerSets(BouwnummerSets bs)
		{
			ObjectContext.AddToBouwnummerSets(bs);
		}
		
        [Insert]
		public void InsertBouwnummerSetOpbouw(BouwnummerSetOpbouw bso)
		{
			ObjectContext.AddToBouwnummerSetOpbouw(bso);
		}
		
        [Insert]
		public void InsertOpgeslagenOfferte(OpgeslagenOffertes oo)
		{
			ObjectContext.AddToOpgeslagenOffertes(oo);
		}
        
        [Insert]
        public void InsertArchiefOffertes(ArchiefOffertes ao)
        {
            ObjectContext.AddToArchiefOffertes(ao);
        }
		
        public IQueryable<ServerLog> GetServerLogs()
		{
			return ObjectContext.ServerLog;
		}
        
        [Delete]
        public void DeleteBouwnummer(Bouwnummers b)
        {
            ObjectContext.DeleteObject(b);
        }
        
        [Insert]
        public void InsertBouwnummer(Bouwnummers b)
        {
            ObjectContext.AddToBouwnummers(b);
        }
        
        [Update]
        public void UpdateBouwnummer(Bouwnummers b)
        {
            ObjectContext.Bouwnummers.AttachAsModified(b);
            ObjectContext.SaveChanges();
        }
        
        public void DeleteOpgeslagenOfferte(OpgeslagenOffertes oo)
        {
            var obj = (from o in ObjectContext.OpgeslagenOffertes
                       where o.OpgeslagenOfferte_ID == oo.OpgeslagenOfferte_ID
                       select o).First();
            ObjectContext.DeleteObject(obj);
            ObjectContext.SaveChanges();
        }
		
        //
        public IQueryable<OpgeslagenOffertes> GetOpgeslagenOfferteById(string Bouwnummer)
        {
            return ObjectContext.OpgeslagenOffertes.Where(x => x.Bouwnummer_NR == Bouwnummer);
        }
        
        [Invoke]
        public List<OpgeslagenOfferteLean> GetOpgeslagenOfferteLean()
        {
            var saveID = (from save in ObjectContext.OpgeslagenOffertes
                          group save by save.Bouwnummer_NR into g
                          select new { BNR = g.Key, datum = (DateTime)g.Max(s => s.timestamp) });
            var res = (from oo in ObjectContext.OpgeslagenOffertes
                       select new OpgeslagenOfferteLean()
                       {
                           B_ID = oo.Bouwnummer_NR,
                           gebruiker = oo.Gebruiker,
                           Datum = (DateTime)oo.timestamp,
                           VersieFull = (from oo1 in ObjectContext.OpgeslagenOffertes
                                         where oo1.Bouwnummer_NR == oo.Bouwnummer_NR && oo1.Volledig != false
                                         select oo1.OpgeslagenOfferte_ID).Count(),
                           VersiePartial = (from oo2 in ObjectContext.OpgeslagenOffertes
                                            where oo2.Bouwnummer_NR == oo.Bouwnummer_NR && oo2.Volledig == false
                                            select oo2.OpgeslagenOfferte_ID).Count(),
                            Prijs = oo.Prijs
                       }).ToList();
                
            foreach (var x in saveID)
            {
                var todel = new Stack<OpgeslagenOfferteLean>();
                foreach (var y in res)
                {
                    if (y.B_ID == x.BNR && y.Datum != x.datum)
                    {
                        todel.Push(y);
                    }

                }
                while (todel.Count > 0)
                {
                    res.Remove(todel.Pop());
                }
            }
           /*var query = (from oo in ObjectContext.OpgeslagenOffertes
                         group oo by oo.Bouwnummer_NR into g
                         select new OpgeslagenOfferteLean() { B_ID = g.FirstOrDefault().Bouwnummer_NR, gebruiker = g.FirstOrDefault().Gebruiker, Datum = (DateTime)g.FirstOrDefault().timestamp, Versie = (from c in ObjectContext.OpgeslagenOffertes where c.Bouwnummer_NR == g.FirstOrDefault().Bouwnummer_NR select c).Count() }).Distinct().ToList();
            */
             return res;
        }
        
        public IQueryable<ArchiefOffertes> GetArchiefOffertes()
        {
            return ObjectContext.ArchiefOffertes;
        }
        
        public IQueryable<OpgeslagenOffertes> GetOpgeslagenOfferte(string Bouwnummer)
		{
			var max = ObjectContext.OpgeslagenOffertes.Where(x => x.Bouwnummer_NR == Bouwnummer).Max(x => x.timestamp);
			return ObjectContext.OpgeslagenOffertes.Where(x => x.Bouwnummer_NR == Bouwnummer && x.timestamp == max);
		}
        
        public IQueryable<Merknamen> GetMerknamen()
       {
           return ObjectContext.Merknamen;
       }
       
        public IQueryable<InstapPrijzen> GetInstapPrijzen(string project_NR)
        {
            return ObjectContext.InstapPrijzen.Where(ip => ip.PR_NR == project_NR);
        }

        public IQueryable<AccentSet> GetAccentSet(string ProjectID)
        {
            return ObjectContext.AccentSet;
        }

        public IQueryable<AccentSetOpbouw> GetAccentSetOpbouw()
        {
            return ObjectContext.AccentSetOpbouw;
        }
        
        public IQueryable<AccentSetProjectCombinatie> GetAccentProjectCombinaties()
        {
            return ObjectContext.AccentSetProjectCombinatie;
        }

        public IQueryable<VrijAccentProductInfo> GetVrijAccentData(string project_nr)
        {
            var accSetNr = ObjectContext.AccentSetProjectCombinatie.Where(x => x.Project_NR == project_nr).Select(ax => ax.AccentSet_NR);
            var validAccRegels = ObjectContext.AccentSetOpbouw.Where(x => accSetNr.Contains(x.AccentSet_NR)).Select(vARR => vARR.VrijAccentRegel_NR);
            return ObjectContext.VrijAccentProductInfo.Where(x => validAccRegels.Contains(x.VAPP_ID));
        }

        public IQueryable<VerkoopStatus> GetVerkoopStatus()
       {
           return ObjectContext.VerkoopStatus;
       }
        
        public IQueryable<VerplichteCombinatieOpbouw> GetVerplichteCombinatieOpbouw()
        {
            return ObjectContext.VerplichteCombinatieOpbouw;
        }
		
        public IQueryable<BouwnummerSetOpbouw> GetBouwnummerSetOpbouw()
		{
			return ObjectContext.BouwnummerSetOpbouw;
		}
		
        public IQueryable<BouwnummerSets> GetBouwnummerSets()
		{
			return ObjectContext.BouwnummerSets;
		}
		
        public IQueryable<ProjectenSetOpbouw> GetProjectSetOpbouw()
		{
			return ObjectContext.ProjectenSetOpbouw;
		}
		
        public IQueryable<ProjectenSets> GetProjectenSets()
		{
			return ObjectContext.ProjectenSets;
		}
		
        public IQueryable<KleurHoekKleurCombinaties> GetKleurHoekKleurCombinaties()
		{
			return this.ObjectContext.KleurHoekKleurCombinaties;
		}
		
        public IQueryable<KleurCombinatieSetOpbouw> GetKleurCombinatieSetOpbouw()
		{
			return this.ObjectContext.KleurCombinatieSetOpbouw;
		}
	
        public IQueryable<HoekProfielType> GetHoekProfielTypes()
		{
			return this.ObjectContext.HoekProfielType;
		}
	
        public IQueryable<GebruikersRollen> GetGebruikersRollen()
		{
			return this.ObjectContext.GebruikersRollen;
		}
        
        public IQueryable<MeterPrijsPerProject> GetMeterPrijsPerProjects()
        {
            return this.ObjectContext.MeterPrijsPerProject;
        }
        
        public IQueryable<MeterPrijsPerProject> GetMeterPrijsPerProject(string projectNr)
        {
            return this.ObjectContext.MeterPrijsPerProject.Where(x=>x.PR_NR == projectNr);
        }
        
        public IQueryable<AfspraakDagen> GetAfspraakDagen()
        {
            return this.ObjectContext.AfspraakDagen;
        }
        
        public IQueryable<BlokkenSets> GetBlokkenSets()
        {
            return this.ObjectContext.BlokkenSets;
        }

        public IQueryable<Blokken> GetBlokken()
        {
            return this.ObjectContext.Blokken;
        }

        public IQueryable<AfsprakenSets> GetAfsprakenSets()
        {
            return this.ObjectContext.AfsprakenSets;
        }

        public IQueryable<BouwnummerOptieGroep> GetBouwnummerOptieGroepen()
        {
            return ObjectContext.BouwnummerOptieGroep;
        }

        public IQueryable<BouwnummerOptieGroepOpbouw> GetBouwnummerOptieGroepOpbouw()
        {
            return ObjectContext.BouwnummerOptieGroepOpbouw;
        }

        public IQueryable<BouwnummerFilterCombinaties> GetBouwnummerOptieFilterCombinateies()
        {
            return ObjectContext.BouwnummerFilterCombinaties;
        }

        [Invoke]
        public bool ReserveerBlok(Blokken b)
        {
            var success = false;
            var blokInDb = ObjectContext.Blokken.Where(x => x.BlokID == b.BlokID).FirstOrDefault();

            if (blokInDb != null)
            {
                if (blokInDb.Status == true && blokInDb.EigenaarGebruikerID != b.EigenaarGebruikerID)
                {
                    
                    success = false;
                }
                else if(blokInDb.Status == true)
                {
                    b.Eigenaar = null;
                    b.EigenaarGebruikerID = null;
                    b.Status = false;
                    ObjectContext.Detach(blokInDb);
                    ObjectContext.Blokken.AttachAsModified(b, blokInDb);
                    ObjectContext.SaveChanges();
                    success = true;
                }
                else
                {
                   
                    ObjectContext.Detach(blokInDb);
                    ObjectContext.Blokken.AttachAsModified(b, blokInDb);
                    ObjectContext.SaveChanges();
                    success = true;
                }
            }

            return success;
        }

        [Update]
        public void UpdateBlokken(Blokken b)
        {
            ObjectContext.Blokken.AttachAsModified(b);
            ObjectContext.SaveChanges();
        }

        //[Update]
        //public void UpdateBlokken(Blokken b)
        //{
        //    var blokInDb = ObjectContext.Blokken.Where(x => x.BlokID == b.BlokID).FirstOrDefault();

        //    if(blokInDb != null)
        //    {
        //        if(blokInDb.Status == true && blokInDb.EigenaarGebruikerID != b.EigenaarGebruikerID)
        //        {
        //            //do nothing
        //        }
        //        else
        //        {
        //            ObjectContext.Detach(blokInDb);
        //            ObjectContext.Blokken.AttachAsModified(b,blokInDb);
        //            ObjectContext.SaveChanges();
        //        }
        //    }
        //}

        
        // TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'Bouwnummers' query.
		public IQueryable<Bouwnummers> GetBouwnummers()
		{
			return this.ObjectContext.Bouwnummers;
		}

        public IQueryable<DorpelOptieCombo> GetDorpelOptieCombos()
        {
            return this.ObjectContext.DorpelOptieCombo;
        }

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'bouwTypes' query.
		public IQueryable<bouwTypes> GetBouwTypes()
		{
			return this.ObjectContext.bouwTypes;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'Bron_Ruimtes' query.
		public IQueryable<Bron_Ruimtes> GetBron_Ruimtes()
		{
			return this.ObjectContext.Bron_Ruimtes;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'Categorieën' query.
		public IQueryable<Categorieën> GetCategorieën()
		{
			return this.ObjectContext.Categorieën;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'Fase' query.
		public IQueryable<Fase> GetFase()
		{
			return this.ObjectContext.Fase;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'Gebruikers' query.
		public IQueryable<Gebruikers> GetGebruikers()
		{
			return this.ObjectContext.Gebruikers;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'HoofdCategorieën' query.
		public IQueryable<HoofdCategorieën> GetHoofdCategorieën()
		{
			return this.ObjectContext.HoofdCategorieën;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'Kleuren' query.
		public IQueryable<Kleuren> GetKleuren()
		{
			return this.ObjectContext.Kleuren;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'KleurenSetOpbouw' query.
		public IQueryable<KleurenSetOpbouw> GetKleurenSetOpbouw()
		{
			return this.ObjectContext.KleurenSetOpbouw;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'KleurenSets' query.
		public IQueryable<KleurenSets> GetKleurenSets()
		{
			return this.ObjectContext.KleurenSets;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'MeerPrijzenRuimteOptie' query.
		public IQueryable<MeerPrijzenRuimteOptie> GetMeerPrijzenRuimteOptie()
		{
			return this.ObjectContext.MeerPrijzenRuimteOptie;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'MeervoudigeCombinaties' query.
		public IQueryable<MeervoudigeCombinaties> GetMeervoudigeCombinaties()
		{
			return this.ObjectContext.MeervoudigeCombinaties;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'MeervoudigeCombinatiesOpbouw' query.
		public IQueryable<MeervoudigeCombinatiesOpbouw> GetMeervoudigeCombinatiesOpbouw()
		{
			return this.ObjectContext.MeervoudigeCombinatiesOpbouw;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'NabewerkingCategoriën' query.
		public IQueryable<NabewerkingCategoriën> GetNabewerkingCategoriën()
		{
			return this.ObjectContext.NabewerkingCategoriën;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'Nabewerkingen' query.
		public IQueryable<Nabewerkingen> GetNabewerkingen()
		{
			return this.ObjectContext.Nabewerkingen;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'NabewerkingSetCombinaties' query.
		public IQueryable<NabewerkingSetCombinaties> GetNabewerkingSetCombinaties()
		{
			return this.ObjectContext.NabewerkingSetCombinaties;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'NabewerkingSetOpbouw' query.
		public IQueryable<NabewerkingSetOpbouw> GetNabewerkingSetOpbouw()
		{
			return this.ObjectContext.NabewerkingSetOpbouw;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'NabewerkingSets' query.
		public IQueryable<NabewerkingSets> GetNabewerkingSets()
		{
			return this.ObjectContext.NabewerkingSets;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'OngeldigeCombinatieOpbouw' query.
		public IQueryable<OngeldigeCombinatieOpbouw> GetOngeldigeCombinatieOpbouw()
		{
			return this.ObjectContext.OngeldigeCombinatieOpbouw;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'OngeldigeCombinaties' query.
		public IQueryable<OngeldigeCombinaties> GetOngeldigeCombinaties()
		{
			return this.ObjectContext.OngeldigeCombinaties;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'OptieProductCombos' query.
		public IQueryable<OptieProductCombos> GetOptieProductCombos()
		{
			return this.ObjectContext.OptieProductCombos;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'OptieTypes' query.
		public IQueryable<OptieTypes> GetOptieTypes()
		{
			return this.ObjectContext.OptieTypes;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'PakketGroep' query.
		public IQueryable<PakketGroep> GetPakketGroep()
		{
			return this.ObjectContext.PakketGroep;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'PakketGroepOpbouw' query.
		public IQueryable<PakketGroepOpbouw> GetPakketGroepOpbouw()
		{
			return this.ObjectContext.PakketGroepOpbouw;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'PakketPrijsgroep' query.
		public IQueryable<PakketPrijsgroep> GetPakketPrijsgroep()
		{
			return this.ObjectContext.PakketPrijsgroep;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'Pakketten' query.
		public IQueryable<Pakketten> GetPakketten()
		{
			/*var pks = (from pg in ObjectContext.PakketGroepOpbouw
					  where pg.PG_NR == PrijsGroepNR
					  select pg.P_NR).ToList();*/
			return this.ObjectContext.Pakketten;//.Where(x=>pks.Contains(x.P_ID));
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'ProductCategoriën' query.
		public IQueryable<ProductCategoriën> GetProductCategoriën()
		{
			return this.ObjectContext.ProductCategoriën;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'Producten' query.
		public IQueryable<Producten> GetProducten()
		{
			return this.ObjectContext.Producten;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'ProductSetOpbouw' query.
		public IQueryable<ProductSetOpbouw> GetProductSetOpbouw()
		{
			return this.ObjectContext.ProductSetOpbouw;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'ProductSets' query.
		public IQueryable<ProductSets> GetProductSets()
		{
			return this.ObjectContext.ProductSets;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'PRoject' query.
		public IQueryable<PRoject> GetPRoject()
		{
			return this.ObjectContext.PRoject;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'RuimteDelen' query.
		public IQueryable<RuimteDelen> GetRuimteDelen()
		{
			return this.ObjectContext.RuimteDelen;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'RuimteOpbouw' query.
		public IQueryable<RuimteOpbouw> GetRuimteOpbouw()
		{
			return this.ObjectContext.RuimteOpbouw;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'RuimteS' query.
		public IQueryable<RuimteS> GetRuimteS()
		{
			return this.ObjectContext.RuimteS;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'RuimtesPerType' query.
		public IQueryable<RuimtesPerType> GetRuimtesPerType()
		{
			return this.ObjectContext.RuimtesPerType;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'StandaardOnafhandelijkeRuimteOpbouw' query.
		public IQueryable<StandaardOnafhandelijkeRuimteOpbouw> GetStandaardOnafhandelijkeRuimteOpbouw()
		{
			return this.ObjectContext.StandaardOnafhandelijkeRuimteOpbouw;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'StandaardRuimteOpbouw' query.
		public IQueryable<StandaardRuimteOpbouw> GetStandaardRuimteOpbouw()
		{
			return this.ObjectContext.StandaardRuimteOpbouw;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'SubCatPerPakket' query.
		public IQueryable<SubCatPerPakket> GetSubCatPerPakket()
		{
			return this.ObjectContext.SubCatPerPakket;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'SubCatPerRuimteDeel' query.
		public IQueryable<SubCatPerRuimteDeel> GetSubCatPerRuimteDeel()
		{
			return this.ObjectContext.SubCatPerRuimteDeel;
		}

		// TODO:
		// Consider constraining the results of your query method.  If you need additional input you can
		// add parameters to this method or create additional query methods with different names.
		// To support paging you will need to add ordering to the 'SubCats' query.
		public IQueryable<SubCats> GetSubCats()
		{
			return this.ObjectContext.SubCats;
		}
	}

    public class OpgeslagenOfferteLean
    {
        public string B_ID { get; set; }
        public string gebruiker{get;set;}
        public DateTime Datum { get; set; }
        public int VersieFull { get; set; }
        public int VersiePartial { get; set; }
        public string Prijs { get; set; }
    }
}



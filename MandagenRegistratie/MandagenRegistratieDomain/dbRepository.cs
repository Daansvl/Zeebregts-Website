using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using ZeebregtsLogic;
using System.Configuration;
using System.Web;

namespace MandagenRegistratieDomain
{
    public class dbRepository
    {
        //public dbRepository()
        //{

        //}

        public bool IsStandalone()
        {
            if (datacontext.Configurations.FirstOrDefault(c => c.ConfigurationName == "ForeignDatabase").ConfigurationValue == "MDR")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string connectionString = ConfigurationManager.ConnectionStrings["MandagenRegistratieConnectionString"].ConnectionString;
        public string connectionStringForeignDatabase()
        {
            if (IsStandalone())
            {
                return ConfigurationManager.ConnectionStrings["MandagenRegistratieConnectionString"].ConnectionString;
            }
            else
            {
                return ConfigurationManager.ConnectionStrings["ZeebregtsDBConnectionString"].ConnectionString;
            }
        }

        public dbDataContext datacontext = new dbDataContext(connectionString);

        //public List<Mandagen> GetMandagen(int vakmanId, DateTime datum)
        //{

        //    CacheManager mandagenCache = (CacheManager)CacheFactory.GetCacheManager();
        //    List<Mandagen> resultaat = (List<Mandagen>)mandagenCache.GetData("GetMandagen_" + vakmanId.ToString());

        //    if (resultaat == null)
        //    {

        //        //string id = "ProductOneId";
        //        //string name = "ProductXYName";
        //        //int price = 50;

        //        //Product product = new Product(id, name, price);

        //        //productsCache.Add(product.ProductID, product, CacheItemPriority.Normal, null, new SlidingTime(TimeSpan.FromMinutes(5)));

        //        //// Retrieve the item.
        //        //product = (Product)productsCache.GetData(id);


        //        DateTime startDatum = new DateTime(datum.Year, datum.Month, datum.Day);
        //        DateTime eindDatumTemp = datum.AddDays(1);
        //        DateTime eindDatum = new DateTime(eindDatumTemp.Year, eindDatumTemp.Month, eindDatumTemp.Day);

        //        resultaat = datacontext.Mandagens.Where(p => p.VakmanId == vakmanId && p.Begintijd > startDatum && p.Begintijd < eindDatum).ToList();
        //        mandagenCache.Add("GetMandagen_" + vakmanId.ToString(), resultaat, CacheItemPriority.Normal, null, new SlidingTime(TimeSpan.FromMinutes(5)));
        //    }

        //   return resultaat;
        //}

        #region Insert statements


        /// <summary>
        ///  Inserts a new Vakman, returns the new Vakman, inclusive of its key
        /// </summary>
        /// <param name="bedrijf"></param>
        /// <returns></returns>
        public Vakman InsertVakman(Vakman vakman)
        {
            // 
            datacontext.Vakmans.InsertOnSubmit(vakman);

            datacontext.SubmitChanges();

            return vakman;
        }

        /// <summary>
        ///  Inserts a new Project, returns the new Project, inclusive of its key
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public int InsertProject(Project project)
        {
            // 
            datacontext.Projects.InsertOnSubmit(project);

            datacontext.SubmitChanges();

            return project.ProjectId;
        }



        #endregion

        #region Update statements

        //public void SaveBedrijf(Bedrijf bedrijf)
        //{
        //    Bedrijf bedrijfToSave = datacontext.Bedrijfs.Where(b => b.BedrijfId == bedrijf.BedrijfId).FirstOrDefault();

        //    bedrijfToSave.BedrijfIdOrigineel = bedrijf.BedrijfIdOrigineel;
            
        //    datacontext.SubmitChanges();
        //}

        //public void SaveProject(Project project)
        //{
        //    Project projectToSave = datacontext.Projects.Where(b => b.ProjectId == project.ProjectId).FirstOrDefault();

        //    projectToSave.Adres = project.Adres;
        //    projectToSave.Huisnummer = project.Huisnummer;
        //    projectToSave.Mutatiedatum = project.Mutatiedatum;
        //    projectToSave.Naam = project.Naam;
        //    projectToSave.Postcode = project.Postcode;

        //    datacontext.SubmitChanges();
        //}


        //public void SaveVakman(Vakman vakman)
        //{
        //    Vakman vakmanToSave = datacontext.Vakmans.Where(v => v.VakmanId == vakman.VakmanId).FirstOrDefault();

        //    vakmanToSave.Adres = vakman.Adres;
        //    vakmanToSave.BedrijfId = vakman.BedrijfId;
        //    vakmanToSave.Bsn = vakman.Bsn;
        //    vakmanToSave.ContactIdOrigineel = vakman.ContactIdOrigineel;
        //    vakmanToSave.Di = vakman.Di;
        //    vakmanToSave.Do = vakman.Do;
        //    vakmanToSave.Huisnummer = vakman.Huisnummer;
        //    vakmanToSave.Intern = vakman.Intern;
        //    vakmanToSave.IsChauffeur = vakman.IsChauffeur;
        //    vakmanToSave.Kenteken = vakman.Kenteken;
        //    vakmanToSave.Kvk = vakman.Kvk;
        //    vakmanToSave.Loonkosten = vakman.Loonkosten;
        //    vakmanToSave.Ma = vakman.Ma;
        //    vakmanToSave.Ophaaladres = vakman.Ophaaladres;
        //    vakmanToSave.Ophaalhuisnummer = vakman.Ophaalhuisnummer;
        //    vakmanToSave.Ophaalpostcode = vakman.Ophaalpostcode;
        //    vakmanToSave.Postcode = vakman.Postcode;
        //    vakmanToSave.ProjectleiderId = vakman.ProjectleiderId;
        //    vakmanToSave.Var = vakman.Var;
        //    vakmanToSave.Vr = vakman.Vr;
        //    vakmanToSave.Werkweek = vakman.Werkweek;
        //    vakmanToSave.Wo = vakman.Wo;
        //    vakmanToSave.Za = vakman.Za;
        //    vakmanToSave.Zo = vakman.Zo;
        //    vakmanToSave.ZZP = vakman.ZZP;

        //    datacontext.SubmitChanges();
        //}




        #endregion



        public List<Mandagen> GetMandagen(int vakmanId, DateTime dtBegintijd)
        {

            //List<Mandagen> resultaat = ApplicationState.GetValue<List<Mandagen>>("GetMandagen_" + vakmanId.ToString());

            ////CacheManager mandagenCache = (CacheManager)CacheFactory.GetCacheManager();
            ////List<Mandagen> resultaat = (List<Mandagen>)mandagenCache.GetData("GetMandagen_" + vakmanId.ToString());

            //if (resultaat == null)
            //{
            List<Mandagen> resultaat = datacontext.Mandagens.Where(m => m.VakmanId == vakmanId && m.Begintijd >= dtBegintijd && m.Begintijd < dtBegintijd.AddDays(1)).ToList();
                //mandagenCache.Add("GetMandagen_" + vakmanId.ToString(), resultaat, CacheItemPriority.Normal, null, new SlidingTime(TimeSpan.FromMinutes(5)));
                //ApplicationState.SetValue("GetMandagen_" + vakmanId.ToString(), resultaat);
            //}

            return resultaat;

        }

        public List<Mandagen> GetMandagenWeekview(int vakmanId, DateTime dtBegintijd)
        {

            //List<Mandagen> resultaat = ApplicationState.GetValue<List<Mandagen>>("GetMandagen_" + vakmanId.ToString());

            ////CacheManager mandagenCache = (CacheManager)CacheFactory.GetCacheManager();
            ////List<Mandagen> resultaat = (List<Mandagen>)mandagenCache.GetData("GetMandagen_" + vakmanId.ToString());

            //if (resultaat == null)
            //{
            List<Mandagen> resultaat = datacontext.Mandagens.Where(m => m.VakmanId == vakmanId && m.Begintijd >= dtBegintijd && m.Begintijd < dtBegintijd.AddDays(7)).ToList();
            //mandagenCache.Add("GetMandagen_" + vakmanId.ToString(), resultaat, CacheItemPriority.Normal, null, new SlidingTime(TimeSpan.FromMinutes(5)));
            //ApplicationState.SetValue("GetMandagen_" + vakmanId.ToString(), resultaat);
            //}

            return resultaat;

        }

        public List<Mandagen> GetMandagenByProjectWeekview(int projectId, DateTime dtBegintijd)
        {

            //List<Mandagen> resultaat = ApplicationState.GetValue<List<Mandagen>>("GetMandagen_" + vakmanId.ToString());

            ////CacheManager mandagenCache = (CacheManager)CacheFactory.GetCacheManager();
            ////List<Mandagen> resultaat = (List<Mandagen>)mandagenCache.GetData("GetMandagen_" + vakmanId.ToString());

            //if (resultaat == null)
            //{

            List<Mandagen> resultaat = datacontext.Mandagens.Where(m => m.ProjectId == projectId && m.Begintijd >= dtBegintijd && m.Begintijd < dtBegintijd.AddDays(7)).ToList();
            
            //mandagenCache.Add("GetMandagen_" + vakmanId.ToString(), resultaat, CacheItemPriority.Normal, null, new SlidingTime(TimeSpan.FromMinutes(5)));
            //ApplicationState.SetValue("GetMandagen_" + vakmanId.ToString(), resultaat);
            //}

            return resultaat;

        }

        public List<Mandagen> GetMandagenByProjectWeekviewAll(DateTime dtBegintijd)
        {

            //List<Mandagen> resultaat = ApplicationState.GetValue<List<Mandagen>>("GetMandagen_" + vakmanId.ToString());

            ////CacheManager mandagenCache = (CacheManager)CacheFactory.GetCacheManager();
            ////List<Mandagen> resultaat = (List<Mandagen>)mandagenCache.GetData("GetMandagen_" + vakmanId.ToString());

            //if (resultaat == null)
            //{

            List<Mandagen> resultaat = datacontext.Mandagens.Where(m => m.Begintijd >= dtBegintijd && m.Begintijd < dtBegintijd.AddDays(7)).ToList();

            //mandagenCache.Add("GetMandagen_" + vakmanId.ToString(), resultaat, CacheItemPriority.Normal, null, new SlidingTime(TimeSpan.FromMinutes(5)));
            //ApplicationState.SetValue("GetMandagen_" + vakmanId.ToString(), resultaat);
            //}

            return resultaat;

        }


        public List<Mandagen> GetMandagen(DateTime dtBegintijd)
        {
            //// use cache for 20 seconds
            //bool useCache = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtDatabaseCache) != null &&
            //    ApplicationState.GetValue<DateTime>(ApplicationVariables.dtDatabaseCache).AddSeconds(20) > DateTime.Now &&
            //    ApplicationState.GetValue<List<Mandagen>>(ApplicationVariables.objDatabaseGetMandagen + dtBegintijd.ToString("yyyyMMdd")) != null;

            List<Mandagen> resultaat = new List<Mandagen>();

            //if (useCache)
            //{
            //    resultaat = ApplicationState.GetValue<List<Mandagen>>(ApplicationVariables.objDatabaseGetMandagen + dtBegintijd.ToString("yyyyMMdd"));
            //}
            //else
            //{
            //    ApplicationState.SetValue(ApplicationVariables.dtDatabaseCache, DateTime.Now);
            //    ApplicationState.SetValue(ApplicationVariables.objDatabaseGetMandagen + dtBegintijd.ToString("yyyyMMdd"), resultaat);
            //}

            resultaat = datacontext.Mandagens.Where(m => m.Begintijd >= dtBegintijd && m.Begintijd < dtBegintijd.AddDays(1)).ToList();

            return resultaat;

        }

        public List<Mandagen> GetMandagenCached(DateTime dtBegintijd)
        {
            //// use cache for 20 seconds
            //bool useCache = ApplicationState.GetValue<DateTime>(ApplicationVariables.dtDatabaseCache) != null &&
            //    ApplicationState.GetValue<DateTime>(ApplicationVariables.dtDatabaseCache).AddSeconds(20) > DateTime.Now &&
            //    ApplicationState.GetValue<List<Mandagen>>(ApplicationVariables.objDatabaseGetMandagen + dtBegintijd.ToString("yyyyMMdd")) != null;

            List<Mandagen> resultaat = new List<Mandagen>();

            //if (useCache)
            //{
            //    resultaat = ApplicationState.GetValue<List<Mandagen>>(ApplicationVariables.objDatabaseGetMandagen + dtBegintijd.ToString("yyyyMMdd"));
            //}
            //else
            //{
            //    ApplicationState.SetValue(ApplicationVariables.dtDatabaseCache, DateTime.Now);
            //    ApplicationState.SetValue(ApplicationVariables.objDatabaseGetMandagen + dtBegintijd.ToString("yyyyMMdd"), resultaat);
            //}

            dbOriginalRepository dbo = new dbOriginalRepository();

            resultaat = datacontext.Mandagens.LinqCache().Where(m => m.Begintijd >= dtBegintijd && m.Begintijd < dtBegintijd.AddDays(1)).OrderBy(m => dbo.GetContact(m.Vakman.ContactIdOrigineel, true) == null ? "" : dbo.GetContact(m.Vakman.ContactIdOrigineel, true).voornaam).ToList();

            return resultaat;

        }


        public List<Mandagen> GetMandagenByProject(int projectId, DateTime dtBegintijd)
        {

            //List<Mandagen> resultaat = ApplicationState.GetValue<List<Mandagen>>("GetMandagen_" + vakmanId.ToString());

            ////CacheManager mandagenCache = (CacheManager)CacheFactory.GetCacheManager();
            ////List<Mandagen> resultaat = (List<Mandagen>)mandagenCache.GetData("GetMandagen_" + vakmanId.ToString());

            //if (resultaat == null)
            //{
            List<Mandagen> resultaat = datacontext.Mandagens.Where(m => m.ProjectId == projectId && m.Begintijd >= dtBegintijd && m.Begintijd < dtBegintijd.AddDays(1)).ToList();
            //mandagenCache.Add("GetMandagen_" + vakmanId.ToString(), resultaat, CacheItemPriority.Normal, null, new SlidingTime(TimeSpan.FromMinutes(5)));
            //ApplicationState.SetValue("GetMandagen_" + vakmanId.ToString(), resultaat);
            //}

            return resultaat;


        }


        public List<Mandagen> GetMandagenByProject(int projectId)
        {

            //List<Mandagen> resultaat = ApplicationState.GetValue<List<Mandagen>>("GetMandagenByProject_" + projectId.ToString());

            //if (resultaat == null)
            //{
            //    resultaat = datacontext.Mandagens.Where(p => p.ProjectId == projectId).ToList();

            //    ApplicationState.SetValue("GetMandagenByProject_" + projectId.ToString(), resultaat);
            //}

            return datacontext.Mandagens.Where(p => p.ProjectId == projectId).ToList();

        }


        public List<Project> GetProjects()
        {
            //if (ApplicationState.GetValue<List<Project>>("Projects") == null)
            //{
            //    ApplicationState.SetValue("Projects", datacontext.Projects.Where(p => p.ProjectId > 0).ToList());
            //    return ApplicationState.GetValue<List<Project>>("Projects");

            //}
            //else
            //{
            //    return ApplicationState.GetValue<List<Project>>("Projects");
            //}

            return datacontext.Projects.Where(p => p.ProjectId > 0).ToList();
        }

        public List<vwProject> GetViewProjects()
        {
            return datacontext.vwProjects.Where(p => p.ProjectId > 0).ToList();
        }

        public List<Gebruiker> GetProjectleiders()
        {
            //dbOriginalRepository dbrepOriginal = new dbOriginalRepository();

            //List<Projectleider> projectleiders = (from pl in datacontext.Projectleiders
            // join p in dbrepOriginal.datacontext.persoons on pl.ContactIdOrigineel equals p.persoon_nr
            // orderby p.achternaam
            // select pl).Distinct().ToList();

            return datacontext.Gebruikers.ToList();
        }



        public Project GetProject(int projectId)
        {
            //Project project = ApplicationState.GetValue<Project>("GetProject_" + projectId.ToString());

            //if (project == null)
            //{
            //    project = datacontext.Projects.Where(p => p.ProjectId == projectId).FirstOrDefault();
            //    ApplicationState.SetValue("GetProject_" + projectId.ToString(), project);
            //}

            return datacontext.Projects.Where(p => p.ProjectId == projectId).FirstOrDefault();
        }

        public Project GetProjectByProjectNrOrigineel(int projectNr)
        {
            Project project = datacontext.Projects.Where(p => p.ProjectNr == projectNr).FirstOrDefault();
            
            return project;
        }

        public List<vwProjectAll> GetVwProjectsAll()
        {
            
            //if (ApplicationState.GetValue<List<project>>("projects") == null)
            //{
            //    ApplicationState.SetValue("projects", datacontext.Projects.ToList());
            //    return ApplicationState.GetValue<List<project>>("projects");
            //}
            //else
            //{
            //    return ApplicationState.GetValue<List<project>>("projects");
            //}

            return datacontext.vwProjectAlls.ToList();
        }

        public List<vwProjectAll> GetVwProjectsAllRemainder()
        {

            //if (ApplicationState.GetValue<List<project>>("projects") == null)
            //{
            //    ApplicationState.SetValue("projects", datacontext.Projects.ToList());
            //    return ApplicationState.GetValue<List<project>>("projects");
            //}
            //else
            //{
            //    return ApplicationState.GetValue<List<project>>("projects");
            //}

            return datacontext.vwProjectAlls.Where(pp => !datacontext.Projects.Any(p => p.ProjectNr == pp.ProjectNrOrigineel)).ToList();
        }


        /// <summary>
        /// returns only projects containing all arguments in Naam or ProjectId
        /// </summary>
        /// <param name="strArguments"></param>
        /// <returns></returns>
        public List<vwProjectAll> GetVwProjectsAll(string[] strArguments)
        {
            List<vwProjectAll> listProjects = GetVwProjectsAllRemainder();

            foreach (string argument in strArguments)
            {
                listProjects = listProjects.Where(p => p.Bedrijfsnaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.voornaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.achternaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.naam_project.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.ProjectNrOrigineel.ToString().Contains(argument, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return listProjects;
        }

        public Gebruiker GetGebruiker(string username)
        {
            return datacontext.Gebruikers.Where(g => g.Gebruikersnaam == username).FirstOrDefault();
        }

        public Gebruiker GetGebruiker(int gebruikerId)
        {
            return datacontext.Gebruikers.Where(g => g.GebruikerId == gebruikerId).FirstOrDefault();
        }

        public Gebruiker GetProjectleider(int projectleiderId)
        {
            //Gebruiker projectLeider = ApplicationState.GetValue<Gebruiker>("GetProjectleider_" + projectleiderId.ToString());

            //if (projectLeider == null)
            //{
            //    projectLeider = datacontext.Gebruikers.Where(p => p.ProjectleiderId == projectleiderId).FirstOrDefault();
            //    ApplicationState.SetValue("GetProjectleider_" + projectleiderId.ToString(), projectLeider);
            //}

            return datacontext.Gebruikers.Where(p => p.ProjectleiderId == projectleiderId).FirstOrDefault();
        }

        /// <summary>
        /// Get projectleider op basis van het bestaan van een mandagenrecord
        /// Als deze niet bestaat, dan gewoon de projecteigenaar
        /// </summary>
        /// <param name="projectleiderId"></param>
        /// <param name="projectId"></param>
        /// <param name="datum"></param>
        /// <returns></returns>
        public Gebruiker GetProjectleider(int projectleiderId, int projectId, DateTime datum)
        {
            //Gebruiker projectLeider = ApplicationState.GetValue<Gebruiker>("GetProjectleider_" + projectleiderId.ToString());

            //if (projectLeider == null)
            //{
            //    projectLeider = datacontext.Gebruikers.Where(p => p.ProjectleiderId == projectleiderId).FirstOrDefault();
            //    ApplicationState.SetValue("GetProjectleider_" + projectleiderId.ToString(), projectLeider);
            //}

            Mandagen mandag = datacontext.Mandagens.FirstOrDefault(m => m.ProjectId == projectId && m.Begintijd == datum && m.Begintijd != m.Eindtijd);

            if (mandag == null)
            {
                return datacontext.Gebruikers.Where(p => p.ProjectleiderId == projectleiderId).FirstOrDefault();
            }
            else
            {
                return datacontext.Gebruikers.Where(p => p.ProjectleiderId == mandag.ProjectleiderId).FirstOrDefault();
            }
        }


        public List<Project> GetProjectsByVakmanId(int vakmanId, DateTime weekstart)
        {
            var query = (from m in datacontext.Mandagens.ToList()
                         where m.ProjectId != 0 && m.Begintijd < weekstart.AddDays(21) && m.Begintijd > weekstart.AddDays(-21)
                         && m.VakmanId == vakmanId
                         join p in datacontext.Projects.ToList() on m.ProjectId equals p.ProjectId
                         select p).Distinct().OrderBy(pp => pp.ProjectNr).ToList();

            return query;
        }

        public List<Project> GetProjectsByVakmanId(int vakmanId, DateTime daystart, int days)
        {
            var query = (from m in datacontext.Mandagens.OrderBy(m => m.Begintijd).ToList()
                         where m.ProjectId != 0 && m.Begintijd >= daystart && m.Begintijd < daystart.AddDays(days)
                         && m.VakmanId == vakmanId
                         join p in datacontext.Projects.ToList() on m.ProjectId equals p.ProjectId
                         select p).Distinct().ToList();

            return query;
        }

        /// <summary>
        /// Get projects ingepland of aangevraagd voor een vakman op een bepaald tijdsinterval
        /// </summary>
        /// <param name="vakmanId"></param>
        /// <param name="intervalstart"></param>
        /// <param name="intervalend"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<Project> GetProjectsByVakmanId(int vakmanId, DateTime intervalstart, DateTime intervalend, bool status)
        {
            var query = (from m in datacontext.Mandagens.OrderBy(m => m.Begintijd).ToList()
                         where m.ProjectId != 0 && m.Begintijd >= intervalstart && m.Begintijd < intervalend
                         && m.VakmanId == vakmanId
                         join p in datacontext.Projects.ToList() on m.ProjectId equals p.ProjectId
                         select p).Distinct().ToList();

            return query;
        }


        public List<Project> GetProjectsIngeplandByVakmanId(int vakmanId, DateTime weekstart)
        {
            var query = (from m in datacontext.Mandagens.ToList()
                         where m.ProjectId != 0 && m.Begintijd < weekstart.AddDays(21) && m.Begintijd > weekstart.AddDays(-21)
                         && m.VakmanId == vakmanId && m.Begintijd != m.Eindtijd
                         join p in datacontext.Projects.ToList() on m.ProjectId equals p.ProjectId
                         select p).Distinct().ToList();

            return query;
        }


        public List<Project> GetProjectsByVakmanId1WeekExactly(int vakmanId, DateTime weekstart)
        {
            var query = (from m in datacontext.Mandagens.ToList()
                         where m.ProjectId != 0 && m.Begintijd < weekstart.AddDays(7) && m.Eindtijd >= weekstart
                         && m.VakmanId == vakmanId
                         join p in datacontext.Projects.ToList() on m.ProjectId equals p.ProjectId
                         select p).Distinct().ToList();

            return query;
        }

        public List<Project> GetProjectsByProjectleiderId(int projectleiderId, DateTime begintijd, DateTime eindtijd)
        {
            var query = (from m in datacontext.Mandagens.ToList()
                         where m.ProjectId != 0 && m.Begintijd >= begintijd && m.Eindtijd <= eindtijd
                         && m.ProjectleiderId == projectleiderId && m.Begintijd != m.Eindtijd
                         join p in datacontext.Projects.ToList() on m.ProjectId equals p.ProjectId
                         select p).Distinct().OrderBy(pp => pp.ProjectNr).ToList();

            return query;
        }

        public List<Project> GetProjectsByInterval(DateTime begintijd, DateTime eindtijd)
        {
            var query = (from m in datacontext.Mandagens.ToList()
                         where m.ProjectId != 0 && m.Begintijd >= begintijd && m.Eindtijd <= eindtijd
                         && m.Begintijd != m.Eindtijd
                         join p in datacontext.Projects.ToList() on m.ProjectId equals p.ProjectId
                         select p).Distinct().OrderBy(pp => pp.ProjectNr).ToList();

            return query;
        }


        public List<Project> GetProjectsByProjectleiderId(int projectleiderId)
        {
            var query = (from m in datacontext.Mandagens.ToList()
                         where m.ProjectId != 0
                         && m.ProjectleiderId == projectleiderId
                         join p in datacontext.Projects.ToList() on m.ProjectId equals p.ProjectId
                         select p).Distinct().ToList();

            return query;
        }


        public List<Project> GetProjectsToAddByVakmanId(int vakmanId, DateTime weekstart)
        {
            var query = (from p in datacontext.Projects.ToList()
                         where p.ProjectId != 0 && !datacontext.Mandagens.Any(m => m.ProjectId == p.ProjectId && m.ProjectId != 0 && m.Begintijd < weekstart.AddDays(14) && m.Begintijd > weekstart.AddDays(-7))
                         select p).Distinct().ToList();

            return query;
        }

        //public List<Vakman> GetVakmannenByProjectId(int projectId, DateTime weekstart)
        //{

        //    var query = (from m in datacontext.Mandagens.ToList()
        //                 where m.ProjectId == projectId && m.Begintijd < weekstart.AddDays(14) && m.Begintijd > weekstart.AddDays(-7)
        //                 join p in datacontext.Vakmans.ToList() on m.VakmanId equals p.VakmanId
        //                 select p).Distinct().ToList();
        //    return query;

        //}


        /// <summary>
        /// TODO: checken of deze sneller is dan de query hierboven, EN of het wel kan met een vwVakman ipv een Vakman
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="weekstart"></param>
        /// <returns></returns>
        public List<vwVakman> GetVakmannenIngeplandByProjectId(int projectId, DateTime weekstart, int intDayRange)
        {
            // return datacontext.Vakmans.Where(v => v.Mandagens.Any(m => m.VakmanId == v.VakmanId && m.ProjectId == projectId && m.Begintijd < weekstart.AddDays(14) && m.Begintijd > weekstart.AddDays(-7))).ToList();

            var query = (from m in datacontext.Mandagens.ToList()
                         where m.ProjectId == projectId && m.Begintijd < weekstart.AddDays(intDayRange) && m.Begintijd >= weekstart.AddDays(0)
                         && m.Begintijd != m.Eindtijd
                         join p in datacontext.vwVakmans.ToList() on m.VakmanId equals p.VakmanId 
                         select p).Distinct().ToList();
            return query;

        }

        /// <summary>
        /// TODO: checken of deze sneller is dan de query hierboven, EN of het wel kan met een vwVakman ipv een Vakman
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="weekstart"></param>
        /// <returns></returns>
        public List<vwVakman> GetVakmannenByProjectId(int projectId, DateTime weekstart)
        {
            // return datacontext.Vakmans.Where(v => v.Mandagens.Any(m => m.VakmanId == v.VakmanId && m.ProjectId == projectId && m.Begintijd < weekstart.AddDays(14) && m.Begintijd > weekstart.AddDays(-7))).ToList();

            var query = (from m in datacontext.Mandagens.ToList()
                         where m.ProjectId == projectId && m.Begintijd < weekstart.AddDays(21) && m.Begintijd > weekstart.AddDays(-21)
                         join p in datacontext.vwVakmans.ToList() on m.VakmanId equals p.VakmanId
                         select p).Distinct().ToList();
            return query;

        }



        public List<Vakman> GetVakmannenToAddByProjectId(int projectId)
        {
            var query = (from v in datacontext.Vakmans.ToList()
                         where !datacontext.Mandagens.Any(m => m.VakmanId == v.VakmanId)
                         select v).Distinct().ToList();

            return query;
        }


        public List<Vakman> GetVakmannenAll()
        {
            //if (ApplicationState.GetValue<List<Vakman>>("Vakmannen") == null)
            //{
            //    ApplicationState.SetValue("Vakmannen", datacontext.Vakmans.ToList());
            //    return ApplicationState.GetValue<List<Vakman>>("Vakmannen");

            //}
            //else
            //{
            //    return ApplicationState.GetValue<List<Vakman>>("Vakmannen");
            //}
            return datacontext.Vakmans.ToList();
        }

        public List<vwVakman> GetVakmannenAll(DateTime dtBegin, DateTime dtEind)
        {
            var query = (from m in datacontext.Mandagens.ToList()
                         where m.Begintijd <= dtEind && m.Begintijd >= dtBegin
                         join p in datacontext.vwVakmans.ToList() on m.VakmanId equals p.VakmanId
                         select p).Distinct().ToList();
            return query;
        }

        public List<vwVakman> GetViewVakmannenAll()
        {
            return datacontext.vwVakmans.ToList();
        }



        public Vakman GetVakman(int vakmanId)
        {
            return datacontext.Vakmans.Where(p => p.VakmanId == vakmanId).FirstOrDefault();
        }

        /// <summary>
        /// Get Vakman by original Id
        /// </summary>
        /// <param name="vakmanId"></param>
        /// <returns></returns>
        public Vakman GetVakmanByOriginalId(int vakmanOriginalContactId)
        {
            return datacontext.Vakmans.Where(p => p.ContactIdOrigineel == vakmanOriginalContactId).FirstOrDefault();
        }

        /// <summary>
        /// Delete alle mandagen die ik heb aangevraagd (per definitie dus ook owner)
        /// </summary>
        /// <param name="vakmanId"></param>
        /// <param name="datum"></param>
        /// <param name="projectleiderId"></param>
        /// <returns></returns>
        public void ResetMandagenVoorVakmanId(int vakmanId, DateTime datum, int projectleiderId)
        {
            // reset mandagen
            ApplicationState.SetValue("GetMandagen_" + vakmanId.ToString(), null);

            // status false verwijderen
            // voor alle mandagen van deze vakman van alle projecten
            foreach (Mandagen mandag in datacontext.Mandagens.Where(m => m.VakmanId == vakmanId && m.Begintijd.Year == datum.Year && m.Begintijd.Month == datum.Month && m.Begintijd.Day == datum.Day && m.Status == false && m.MutatieDoorProjectleiderId == projectleiderId && !m.Geannulleerd))
            {
                datacontext.Mandagens.DeleteOnSubmit(mandag);
                ApplicationState.SetValue("GetMandagenByProject_" + mandag.ProjectId.ToString(), null);
            }

            datacontext.SubmitChanges();
        }

        ///// <summary>
        ///// Delete alle mandagen waarvan ik de owner ben
        ///// </summary>
        ///// <param name="vakmanId"></param>
        ///// <param name="datum"></param>
        ///// <param name="projectleiderId"></param>
        ///// <returns></returns>
        //public int DeleteMandagenVoorVakmanId(int vakmanId, DateTime datum, int projectleiderId, DateTime dtCheckDatum)
        //{
        //    int changeStatus = 1;
            
        //    // reset mandagen
        //    ApplicationState.SetValue("GetMandagen_" + vakmanId.ToString(), null);

        //    // status true verwijderen
        //    // voor alle mandagen van deze vakman van alle projecten
        //    foreach (Mandagen mandag in datacontext.Mandagens.Where(m => m.VakmanId == vakmanId && m.Begintijd.Year == datum.Year && m.Begintijd.Month == datum.Month && m.Begintijd.Day == datum.Day && m.Status && (m.Eindtijd.Hour > 0 || m.Eindtijd.Minute > 0) && m.Project.ProjectleiderId == projectleiderId))
        //    {
        //        if (mandag.Mutatiedatum > dtCheckDatum)
        //        {
        //            // we mogen niks wijzigen want nadat we dit scherm openden heeft iemand anders iets gewijzigd
        //            if (mandag.MutatieDoorProjectleiderId == projectleiderId)
        //            {
        //                changeStatus = 0;
        //            }
        //            else
        //            {
        //                changeStatus = -1;
        //            }
        //        }
        //        else
        //        {
        //            // we mogen gewoon wijzigen en deleten
        //            datacontext.Mandagens.DeleteOnSubmit(mandag);
        //            ApplicationState.SetValue("GetMandagenByProject_" + mandag.ProjectId.ToString(), null);
        //        }
        //    }

        //    // alleen doorvoeeren als het mag
        //    if (changeStatus == 1)
        //    {
        //        datacontext.SubmitChanges();
        //    }
        //    else
        //    {
        //        // cancel changes
        //        // niks wijzigen
        //    }

        //    return changeStatus;

        //}


        /// <summary>
        /// Delete alle mandagen waarvan ik de owner ben
        /// </summary>
        /// <param name="vakmanId"></param>
        /// <param name="datum"></param>
        /// <param name="projectleiderId"></param>
        /// <returns></returns>
        public int DeleteMandagenVoorVakmanId(int vakmanId, DateTime datum, int projectleiderId, DateTime dtCheckDatum)
        {
            int changeStatus = 1;

            SqlConnection sqlCon = null;

            try
            {
                sqlCon = new SqlConnection(connectionString);

                SqlCommand sqlCmd = new SqlCommand("p_DeleteMandagenVoorVakmanId", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@VakmanId", vakmanId);
                sqlCmd.Parameters.AddWithValue("@Datum", datum);
                sqlCmd.Parameters.AddWithValue("@DatumEind", datum.AddDays(1));
                sqlCmd.Parameters.AddWithValue("@ProjectleiderId", projectleiderId);
                sqlCmd.Parameters.AddWithValue("@CheckDatum", dtCheckDatum);

                SqlParameter param = new SqlParameter("@ReturnId", SqlDbType.Int);
                param.Direction = ParameterDirection.ReturnValue;
                sqlCmd.Parameters.Add(param);

                sqlCon.Open();
                sqlCmd.ExecuteNonQuery();

                changeStatus = Convert.ToInt32(param.Value);

                sqlCon.Close();

            }
            catch
            {
                //catch
                return -1;
            }
            finally
            {
                if (sqlCon != null) sqlCon.Close();
            }

            return changeStatus;

        }

        /// <summary>
        /// Voeg de nieuwe persoon toe aan de MDRpersoon tabel
        /// </summary>
        /// <param name="vakmanId"></param>
        /// <param name="datum"></param>
        /// <param name="projectleiderId"></param>
        /// <returns></returns>
        public int InsertMDRPersoon(string voornaam)
        {
            int intNewId = -1;

            SqlConnection sqlCon = null;

            try
            {
                sqlCon = new SqlConnection(connectionString);

                SqlCommand sqlCmd = new SqlCommand("p_InsertMDRpersoon", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@voornaam", voornaam);

                SqlParameter param = new SqlParameter("@ReturnId", SqlDbType.Int);
                param.Direction = ParameterDirection.ReturnValue;
                sqlCmd.Parameters.Add(param);

                sqlCon.Open();
                sqlCmd.ExecuteNonQuery();

                intNewId = Convert.ToInt32(param.Value);

                sqlCon.Close();

            }
            catch
            {
                //catch
                return -1;
            }
            finally
            {
                if (sqlCon != null) sqlCon.Close();
            }

            return intNewId;

        }


        /// <summary>
        /// Delete alle mandagen waarvan ik de owner ben in dit projectId
        /// </summary>
        /// <param name="vakmanId"></param>
        /// <param name="datum"></param>
        /// <param name="projectleiderId"></param>
        /// <returns></returns>
        public int DeleteMandagenVoorVakmanId(int vakmanId, int projectId, DateTime datum, int projectleiderId, DateTime dtCheckDatum)
        {

            int changeStatus = 1;

            SqlConnection sqlCon = null;

            try
            {
                sqlCon = new SqlConnection(connectionString);

                SqlCommand sqlCmd = new SqlCommand("p_DeleteMandagenVoorVakmanIdByProject", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@VakmanId", vakmanId);
                sqlCmd.Parameters.AddWithValue("@ProjectId", projectId);
                sqlCmd.Parameters.AddWithValue("@Datum", datum);
                sqlCmd.Parameters.AddWithValue("@DatumEind", datum.AddDays(1));
                sqlCmd.Parameters.AddWithValue("@ProjectleiderId", projectleiderId);
                sqlCmd.Parameters.AddWithValue("@CheckDatum", dtCheckDatum);

                SqlParameter param = new SqlParameter("@ReturnId", SqlDbType.Int);
                param.Direction = ParameterDirection.ReturnValue;
                sqlCmd.Parameters.Add(param);

                sqlCon.Open();
                sqlCmd.ExecuteNonQuery();

                changeStatus = Convert.ToInt32(param.Value);

                sqlCon.Close();

            }
            catch
            {
                //catch
                return -1;
            }
            finally
            {
                if (sqlCon != null) sqlCon.Close();
            }

            return changeStatus;

        }



        /// <summary>
        /// Get alle mandagen waarvan ik NIET de owner ben
        /// </summary>
        /// <param name="vakmanId"></param>
        /// <param name="datum"></param>
        /// <param name="projectleiderId"></param>
        /// <returns></returns>
        public List<Mandagen> GetMandagenVoorVakmanIdNoOwner(int vakmanId, DateTime datum, int projectleiderId)
        {
            // TODO: CHECK THIS TEST
            //return datacontext.Mandagens.Where(m => m.VakmanId == vakmanId && m.Begintijd.Year == datum.Year && m.Begintijd.Month == datum.Month && m.Begintijd.Day == datum.Day && m.Status && (m.Eindtijd.Hour > 0 || m.Eindtijd.Minute > 0) && m.Project.ProjectleiderId != projectleiderId).ToList();
            return datacontext.Mandagens.Where(m => m.VakmanId == vakmanId && m.Begintijd.Year == datum.Year && m.Begintijd.Month == datum.Month && m.Begintijd.Day == datum.Day && (m.Eindtijd.Hour > 0 || m.Eindtijd.Minute > 0) && m.ProjectleiderId != projectleiderId).ToList();

        }



        public Mandagen CreateMandag(Mandagen originalMandag)
        {

            Mandagen mandag = new Mandagen();
            // info die je bij adden al weet:

            // unieke key
            mandag.VakmanId = originalMandag.VakmanId;
            mandag.ProjectId = originalMandag.ProjectId;
            // begintijd
            mandag.Begintijd = originalMandag.Begintijd;
            mandag.Eindtijd = originalMandag.Eindtijd;

           // dit is een wijziging, status true is alleen voor solid kleur
            mandag.Status = originalMandag.Status;
            // gedaan door
            mandag.MutatieDoorProjectleiderId = originalMandag.MutatieDoorProjectleiderId;
            // op
            mandag.Mutatiedatum = DateTime.Now;
            // einde key
            // niet geannulleerd
            mandag.Geannulleerd = originalMandag.Geannulleerd;

            // niet definitief
            mandag.Definitief = originalMandag.Definitief;

            // niet bevestigd
            mandag.Bevestigd = originalMandag.Bevestigd;


            mandag.IsChauffeurHeen = originalMandag.IsChauffeurHeen;
            mandag.IsChauffeurTerug = originalMandag.IsChauffeurTerug;
            mandag.KentekenHeen = originalMandag.KentekenHeen;
            mandag.KentekenTerug = originalMandag.KentekenTerug;
            mandag.Bevestigingsdatum = DateTime.Now;


            // projectleider op dit moment, voor later in t archief na een eventuele projectleider mutatie
            mandag.ProjectleiderId = originalMandag.ProjectleiderId;


            // obsolete, vervangen door status (composite key)
            mandag.Gewijzigd = originalMandag.Gewijzigd;

            // obsolete
            mandag.Uren = originalMandag.Uren;
            mandag.Minuten = originalMandag.Minuten;

            return mandag;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mandagToDelete"></param>
        public void Deletemandag(Mandagen mandagToDelete)
        {
            if (mandagToDelete != null)
            {
                Mandagen mandag = datacontext.Mandagens.Where(m => m.Begintijd == mandagToDelete.Begintijd && m.Eindtijd == mandagToDelete.Eindtijd && m.Status == mandagToDelete.Status && m.VakmanId == mandagToDelete.VakmanId && m.ProjectId == mandagToDelete.ProjectId).FirstOrDefault();
                if (mandag != null)
                {
                    datacontext.Mandagens.DeleteOnSubmit(mandag);
                    datacontext.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mandagToDelete"></param>
        public void Rejectmandag(Mandagen mandagToDelete)
        {
            if (mandagToDelete != null)
            {
                Mandagen mandag = datacontext.Mandagens.Where(m => m.Begintijd == mandagToDelete.Begintijd && m.Eindtijd == mandagToDelete.Eindtijd && m.Status == mandagToDelete.Status && m.VakmanId == mandagToDelete.VakmanId && m.ProjectId == mandagToDelete.ProjectId).FirstOrDefault();
                if (mandag != null)
                {
                    mandag.Geannulleerd = true;
                    datacontext.SubmitChanges();
                }
            }
        }


        /// <summary>
        /// returns list with the new situation
        /// </summary>
        /// <param name="origineleMandag"></param>
        /// <param name="nieuweMandag"></param>
        /// <returns></returns>
        public List<Mandagen> MergeMandagen(Mandagen origineleMandag, Mandagen nieuweMandag)
        {
            List<Mandagen> adjustedMandagen = new List<Mandagen>();

            // als ze van dezelfde vakman en project combinatie zijn
            if (origineleMandag.VakmanId == nieuweMandag.VakmanId && origineleMandag.ProjectId == nieuweMandag.ProjectId)
            {
                //if (activeMerge)
                //{
                 //TODO testen of dit em is gewoon ;-)
                adjustedMandagen.Add(nieuweMandag);
                //}
                //else
                //{
                // als de mandag eindigt voordat de originele begint of begint nadat de originele eindigt
                //if (nieuweMandag.Eindtijd < origineleMandag.Begintijd || nieuweMandag.Begintijd > origineleMandag.Eindtijd)
                //{
                //    // niks veranderen, beide apart toevoegen
                //    adjustedMandagen.Add(origineleMandag);
                //    adjustedMandagen.Add(nieuweMandag);
                //}
                //else
                //{
                //    // een grote mandag van maken
                //    Mandagen newMandag = new datacontext.Mandagens;
                //    newMandag = CreateMandag(origineleMandag);

                //    // begintijd is eerder nu               
                //    newMandag.Begintijd = origineleMandag.Begintijd < nieuweMandag.Begintijd ? origineleMandag.Begintijd : nieuweMandag.Begintijd;

                //    // eindtijd is later nu               
                //    newMandag.Eindtijd = origineleMandag.Eindtijd < nieuweMandag.Eindtijd ? nieuweMandag.Eindtijd : origineleMandag.Eindtijd;

                //    adjustedMandagen.Add(newMandag);
                //}
                ////}

            }
            else
            {
                // ervanuitgaande dat ze NIET van dezelfde vakman en project combinatie zijn

                // als de mandag eindigt voordat de originele begint of begint nadat de originele eindigt
                if (nieuweMandag.Eindtijd <= origineleMandag.Begintijd || nieuweMandag.Begintijd >= origineleMandag.Eindtijd)
                {
                    // niks veranderen, beide apart toevoegen
                    adjustedMandagen.Add(origineleMandag);
                    adjustedMandagen.Add(nieuweMandag);
                }
                // als de nieuwe eerder of gelijktijdig begint en later of gelijktijdig eindigt, dus alles overlapt
                else if (nieuweMandag.Begintijd <= origineleMandag.Begintijd && nieuweMandag.Eindtijd >= origineleMandag.Eindtijd)
                {
                    // alleen de nieuwe toevoegen
                    adjustedMandagen.Add(nieuweMandag);
                }
                // als de nieuwe eerder of gelijktijdig begint en eerder eindigt
                else if (nieuweMandag.Begintijd <= origineleMandag.Begintijd && nieuweMandag.Eindtijd < origineleMandag.Eindtijd)
                {
                    // alleen begintijd originele aanpassen
                    Mandagen mandag1 = CreateMandag(origineleMandag);
                    mandag1.Begintijd = nieuweMandag.Eindtijd;

                    // beiden toevoegen
                    adjustedMandagen.Add(mandag1);
                    adjustedMandagen.Add(nieuweMandag);


                }
                // als de nieuwe later begint en later of gelijktijdig eindigt
                else if (nieuweMandag.Begintijd > origineleMandag.Begintijd && nieuweMandag.Eindtijd >= origineleMandag.Eindtijd)
                {
                    // alleen eindtijd originele aanpassen
                    Mandagen mandag1 = CreateMandag(origineleMandag);
                    mandag1.Eindtijd = nieuweMandag.Begintijd;

                    // beiden toevoegen
                    adjustedMandagen.Add(mandag1);
                    adjustedMandagen.Add(nieuweMandag);
      
                }
                // als de nieuwe later begint en eerder eindigt
                else if (nieuweMandag.Begintijd > origineleMandag.Begintijd && nieuweMandag.Eindtijd < origineleMandag.Eindtijd)
                {
                    // 3 mandagen

                    // 
                    Mandagen mandag1 = CreateMandag(origineleMandag);
                    Mandagen mandag2 = CreateMandag(origineleMandag);
                    mandag1.Eindtijd = nieuweMandag.Begintijd;
                    mandag2.Begintijd = nieuweMandag.Eindtijd;

                    // beiden toevoegen
                    adjustedMandagen.Add(mandag1);
                    adjustedMandagen.Add(mandag2);
                    adjustedMandagen.Add(nieuweMandag);


                }

            }

            return adjustedMandagen;

        }

        /// <summary>
        /// returns list with the new situation
        /// </summary>
        /// <param name="mandag1"></param>
        /// <param name="nieuweMandag"></param>
        /// <returns></returns>
        public Mandagen ConcatenateMandagen(Mandagen mandag1, Mandagen mandag2)
        {
            Mandagen concatenatedMandag = new Mandagen();
            concatenatedMandag = CreateMandag(mandag1);

            // kleinste begintijd nemen
            if (mandag1.Begintijd < mandag2.Begintijd)
            {
                concatenatedMandag.Begintijd = mandag1.Begintijd;
            }
            else
            {
                concatenatedMandag.Begintijd = mandag2.Begintijd;
            }

            // grootste eindtijd nemen
            if (mandag1.Eindtijd > mandag2.Eindtijd)
            {
                concatenatedMandag.Eindtijd = mandag1.Eindtijd;
            }
            else
            {
                concatenatedMandag.Eindtijd = mandag2.Eindtijd;
            }

            return concatenatedMandag;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="vakmanId"></param>
        /// <param name="datum"></param>
        /// <param name="projectleiderId">mutatie door projectleiderId</param>
        public void ConfirmMandag(Mandagen mandagToConfirm, bool isApproval)
        {
            // reset mandagen
            ApplicationState.SetValue("GetMandagen_" + mandagToConfirm.VakmanId.ToString(), null);



            // voor alle mandagen die deze mandag beinvloed
            // bereken de nieuwe mandagen (merge)
            // verwijder de oude met status true
            // voeg de nieuwe toe met status true
            // verwijder de oude met status false

            List<Mandagen> definitieveLijst = new List<Mandagen>();
            List<Mandagen> definitieveLijst2 = new List<Mandagen>();
            List<Mandagen> definitieveLijst3 = new List<Mandagen>();
            List<Mandagen> addedLijst = new List<Mandagen>();

            // status true verwijderen
            int count = 0;
            // voor alle mandagen van deze vakman van alle projecten
            foreach (Mandagen mandag in datacontext.Mandagens.Where(m => m.VakmanId == mandagToConfirm.VakmanId && m.Begintijd < mandagToConfirm.Eindtijd && m.Eindtijd > mandagToConfirm.Begintijd && m.Status))
            //foreach (Mandagen mandag in datacontext.Mandagens.Where(m => m.VakmanId == mandagToConfirm.VakmanId && m.Begintijd <= mandagToConfirm.Eindtijd && m.Eindtijd > mandagToConfirm.Begintijd))
            {
                definitieveLijst.AddRange(MergeMandagen(mandag, CreateMandag(mandagToConfirm)));

                datacontext.Mandagens.DeleteOnSubmit(mandag);
                count++;
            }

            if (count == 0)
            {
                definitieveLijst.Add(CreateMandag(mandagToConfirm));
                count++;
            }


            //if (isApproval)
            //{

            //    // voor alle mandagen in de definitieve lijst
            //    foreach (Mandagen mandag in definitieveLijst)
            //    {
            //        // alle aangrenzende tijdsblokken toevoegen aan tussenlijstje 3
            //        foreach (Mandagen mandag2 in datacontext.Mandagens.Where(m => m.VakmanId == mandag.VakmanId && m.Eindtijd > m.Begintijd && m.ProjectId == mandag.ProjectId && (m.Eindtijd == mandag.Begintijd || m.Begintijd == mandag.Eindtijd) && m.Status))
            //        {
            //        definitieveLijst3.Add(mandag2);

            //        datacontext.Mandagens.DeleteOnSubmit(mandag2);

            //        }
            //    }

            //    // de definitieve lijst zelf hier ook nog aan toevoegen
            //    definitieveLijst3.AddRange(definitieveLijst);

            //    // nu nog de definitieveLijst3 sorteren
            //    definitieveLijst3 = definitieveLijst3.OrderBy(d => d.ProjectId).OrderBy(d => d.Begintijd).OrderBy(d => d.Eindtijd).ToList();

            //    bool skipFirst = true;
            //    // en nu alles achter elkaar plakken waar nodig
            //    for (int i = 0; i < definitieveLijst3.Count; i++) // datacontext.Mandagens.Where(m => m.VakmanId == mandagToConfirm.VakmanId && m.Eindtijd > m.Begintijd && m.ProjectId == mandagToConfirm.ProjectId  && (m.Eindtijd == mandagToConfirm.Begintijd || m.Begintijd == mandagToConfirm.Eindtijd) && m.Status)
            //    //foreach (Mandagen mandag in datacontext.Mandagens.Where(m => m.VakmanId == mandagToConfirm.VakmanId && m.Begintijd <= mandagToConfirm.Eindtijd && m.Eindtijd > mandagToConfirm.Begintijd))
            //    {
            //        // alvast toevoegen voor het geval we niet gaan concatenaten
            //        // en ook voor de eerste die wordt overgeslagen, anders waren we die kwijt in geval van geen concatenation
            //        definitieveLijst2.Add(definitieveLijst3[i]);

            //        // eerste altijd overslaan
            //        if (!skipFirst)
            //        {
            //            // als deze grenst aan de vorige, plakken aan vorige
            //            if (definitieveLijst3[i - 1].Eindtijd == definitieveLijst3[i].Begintijd && definitieveLijst3[i - 1].ProjectId == definitieveLijst3[i].ProjectId)
            //            {
            //                // vorige weghalen (hadden we net toegevoegd)
            //                definitieveLijst2.RemoveAt(definitieveLijst2.Count - 1);

            //                // nieuwe laatste (samengestelde) toevoegen
            //                definitieveLijst2.Add(ConcatenateMandagen(definitieveLijst3[i - 1], definitieveLijst3[i]));
            //            }
            //        }
            //        else
            //        {
            //            skipFirst = false;
            //        }
            //    }



            //    if (count == 0)
            //    {
            //        definitieveLijst.Add(CreateMandag(mandagToConfirm));
            //    }

            //    // in dit geval zat het ge-approvede tijdsblok precies tussen 2 andere blokken in
            //    if (definitieveLijst.Count == 2)
            //    {
            //        definitieveLijst2.Add(ConcatenateMandagen(definitieveLijst[0], definitieveLijst[1]));
            //        definitieveLijst.Clear();

            //        definitieveLijst.AddRange(definitieveLijst2);
            //    }


            //}



            ////if (definitieveLijst.Count > 0)
            ////{
            ////    int count2 = 0;
            ////    // kijken of er nog merges kunnen plaatsvinden
            ////    foreach (Mandagen mandag in definitieveLijst)
            ////    {
            ////        // probeer te mergen met de opvolgende, als die er is
            ////        if (count2 < definitieveLijst.Count - 1)
            ////        {
            ////            definitieveLijst2.AddRange(MergeMandagen(mandag, CreateMandag(mandag)));


            ////        }
            ////        else // bij de laatste
            ////        {


            ////        }



            ////        count2++;
            ////    }



            ////}




            // TODO testen, misschien heb ik deze al weggehaald....
            dbRepository dbrep = new dbRepository();
            dbrep.Deletemandag(mandagToConfirm);

            foreach (Mandagen mandag in definitieveLijst)
            {
                Mandagen createdMandag = CreateMandag(mandag);
                createdMandag.Status = true;
                createdMandag.Geannulleerd = false;
                //mandag.Status = true;
                createdMandag.MutatieDoorProjectleiderId = mandagToConfirm.MutatieDoorProjectleiderId;
                createdMandag.Mutatiedatum = DateTime.Now;

                ApplicationState.SetValue("GetMandagenByProject_" + createdMandag.ProjectId.ToString(), null);

                // obsolete
                createdMandag.Bevestigd = true;
                createdMandag.Gewijzigd = false;

                // voorkom duplicate entries
                if (!addedLijst.Any(m => m.Begintijd == createdMandag.Begintijd && m.Eindtijd == createdMandag.Eindtijd && m.Status == createdMandag.Status))
                {
                    datacontext.Mandagens.InsertOnSubmit(createdMandag);
                    addedLijst.Add(createdMandag);
                }
            }

            datacontext.SubmitChanges();


            if (isApproval)
            {
                // tussenlijstje
                List<Mandagen> opnieuwToevoegen = new List<Mandagen>();

                Mandagen mandagVorige = null;
                bool sequence = false;
                // voor alle mandagen van dit project
                foreach (Mandagen mandag in datacontext.Mandagens.Where(m => m.VakmanId == mandagToConfirm.VakmanId && m.Eindtijd > m.Begintijd && m.ProjectId == mandagToConfirm.ProjectId && m.Status).OrderBy(o => o.Begintijd).OrderBy(o => o.Eindtijd))
                {
                    // als de vorige leeg is niks doen
                    if (mandagVorige != null)
                    {
                        // als deze grenst aan de vorige, nieuwe gemergde aanmaken
                        if (mandag.Begintijd == mandagVorige.Eindtijd)
                        {
                            // mergen
                            if (sequence)
                            {
                                opnieuwToevoegen.Add(ConcatenateMandagen(opnieuwToevoegen[opnieuwToevoegen.Count-1], mandag));
                                opnieuwToevoegen.RemoveAt(opnieuwToevoegen.Count - 2);
                            }
                            else
                            {
                                opnieuwToevoegen.Add(ConcatenateMandagen(mandagVorige, mandag));

                            }
                            // vorige verwijderen
                            datacontext.Mandagens.DeleteOnSubmit(mandagVorige);
                            // deze verwijderen
                            datacontext.Mandagens.DeleteOnSubmit(mandag);
                            // nieuwe toevoegen

                            sequence = true;
                        }
                        else
                        {
                            sequence = false;
                        }
                    }
                    // doorgaan met de loop
                    mandagVorige = mandag;
                }

                // submitchanges
                datacontext.SubmitChanges();


                mandagVorige = null;

                foreach (Mandagen mandag in opnieuwToevoegen)
                {
                    // als de vorige leeg is niks doen
                    if (mandagVorige != null)
                    {
                        // als deze grenst aan de vorige, nieuwe gemergde aanmaken
                        if (mandag.Begintijd == mandagVorige.Eindtijd)
                        {
                            // vorige verwijderen
                            opnieuwToevoegen.Remove(mandagVorige);

                            // mergen
                            opnieuwToevoegen.Add(ConcatenateMandagen(mandagVorige, mandag));
                        }
                    }
                    // doorgaan met de loop
                    mandagVorige = mandag;
                }


                foreach (Mandagen mandag in opnieuwToevoegen)
                {
                    datacontext.Mandagens.InsertOnSubmit(mandag);
                }

                // submitchanges
                datacontext.SubmitChanges();
            }

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="vakmanId"></param>
        /// <param name="datum"></param>
        /// <param name="projectleiderId">mutatie door projectleiderId</param>
        public void ConfirmMandagenVoorVakmanId(int vakmanId, DateTime datum, int projectleiderId)
        {
            // reset mandagen
            ApplicationState.SetValue("GetMandagen_" + vakmanId.ToString(), null);

            // status true verwijderen
            // voor alle mandagen van deze vakman van alle projecten
            foreach (Mandagen mandag in datacontext.Mandagens.Where(m => m.VakmanId == vakmanId && m.Begintijd.Year == datum.Year && m.Begintijd.Month == datum.Month && m.Begintijd.Day == datum.Day && m.Status))
            {
                datacontext.Mandagens.DeleteOnSubmit(mandag);
            }

            datacontext.SubmitChanges();

            // status false op true zetten
            // voor alle mandagen van deze vakman van alle projecten, gewijzigd door projectleiderId
            foreach (Mandagen mandag in datacontext.Mandagens.Where(m => m.VakmanId == vakmanId && m.Begintijd.Year == datum.Year && m.Begintijd.Month == datum.Month && m.Begintijd.Day == datum.Day && m.MutatieDoorProjectleiderId == projectleiderId))
            {
                mandag.Geannulleerd = false;
                //mandag.Status = true;
                mandag.MutatieDoorProjectleiderId = projectleiderId;
                mandag.Mutatiedatum = DateTime.Now;

                ApplicationState.SetValue("GetMandagenByProject_" + mandag.ProjectId.ToString(), null);

                // obsolete
                mandag.Bevestigd = true;
                mandag.Gewijzigd = false;

                datacontext.Mandagens.DeleteOnSubmit(mandag);


                Mandagen newMandag = new Mandagen();
                newMandag.Begintijd = mandag.Begintijd;
                newMandag.Bevestigd = mandag.Bevestigd;
                newMandag.Bevestigingsdatum = mandag.Bevestigingsdatum;
                newMandag.Definitief = mandag.Definitief;
                newMandag.Eindtijd = mandag.Eindtijd;
                newMandag.Geannulleerd = mandag.Geannulleerd;
                newMandag.Gewijzigd = mandag.Gewijzigd;
                newMandag.IsChauffeurHeen = mandag.IsChauffeurHeen;
                newMandag.IsChauffeurTerug = mandag.IsChauffeurTerug;
                newMandag.KentekenHeen = mandag.KentekenHeen;
                newMandag.KentekenTerug = mandag.KentekenTerug;
                newMandag.Minuten = mandag.Minuten;
                newMandag.Mutatiedatum = mandag.Mutatiedatum;
                newMandag.MutatieDoorProjectleiderId = mandag.MutatieDoorProjectleiderId;
                newMandag.ProjectId = mandag.ProjectId;
                newMandag.ProjectleiderId = mandag.ProjectleiderId;
                //newMandag.Status = mandag.Status;
                newMandag.Uren = mandag.Uren;
                newMandag.VakmanId = mandag.VakmanId;

                newMandag.Status = true;

                datacontext.Mandagens.InsertOnSubmit(newMandag);
            }

            datacontext.SubmitChanges();

        }


        public Mandagen GetMandag(int vakmanId, int projectId, DateTime datum, bool status)
        {
            return datacontext.Mandagens.Where(m => m.ProjectId == projectId && m.VakmanId == vakmanId && m.Begintijd == datum && m.Status == status).FirstOrDefault();
        }

        public Mandagen GetMandagOngeveer(int vakmanId, int projectId, DateTime datum, int vakmanStatus, bool status)
        {
            return datacontext.Mandagens.Where(m => m.ProjectId == projectId && m.VakmanId == vakmanId && m.Begintijd <= datum && m.Eindtijd >= datum && m.Status == status).FirstOrDefault();
        }


        public void InsertMandag(Mandagen mandag)
        {
            Mandagen oldMandag = GetMandag(mandag.VakmanId, mandag.ProjectId, mandag.Begintijd, mandag.Status);

            ApplicationState.SetValue("GetMandagen_" + mandag.VakmanId.ToString(), null);

            ApplicationState.SetValue("GetMandagenByProject_" + mandag.ProjectId.ToString(), null);

            if (oldMandag != null && oldMandag.Eindtijd > oldMandag.Begintijd)
            {
                oldMandag.Bevestigd = mandag.Bevestigd;
                oldMandag.Gewijzigd = mandag.Gewijzigd;
                oldMandag.ProjectleiderId = mandag.ProjectleiderId;
                oldMandag.Eindtijd = mandag.Eindtijd;
                datacontext.SubmitChanges();
            }
            else
            {
                datacontext.Mandagens.InsertOnSubmit(mandag);
                datacontext.SubmitChanges();
            }


        }

        /// <summary>
        /// returns only projects containing all arguments in Naam or ProjectId
        /// </summary>
        /// <param name="strArguments"></param>
        /// <returns></returns>
        public List<vwProject> GetViewProjects(string[] strArguments)
        {
            List<vwProject> listProjects = GetViewProjects();

            foreach (string argument in strArguments)
            {
                listProjects = listProjects.Where(p => p.Projectplaats.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.voornaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.achternaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.Bedrijfsnaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.Naam.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.ProjectNrOrigineel.ToString().Contains(argument, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return listProjects;
        }

        

        /// <summary>
        /// returns only projects containing all vakmannen in Naam or ProjectId
        /// </summary>
        /// <param name="strArguments"></param>
        /// <returns></returns>
        public List<vwVakman> GetViewVakmannen(string[] strArguments)
        {
            List<vwVakman> listVakmannen = GetViewVakmannenAll();

            foreach (string argument in strArguments)
            {
                listVakmannen = listVakmannen.Where(v => v.bedrijfnaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || v.bedrijfzoeknaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || v.bedrijfplaats.Contains(argument, StringComparison.OrdinalIgnoreCase) || v.achternaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || v.voornaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || v.Adres.Contains(argument, StringComparison.OrdinalIgnoreCase) || v.persoon_nr.ToString().Contains(argument, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return listVakmannen;
        }


        public List<vwContactAll> GetContacten()
        {
            return datacontext.vwContactAlls.Where(cc => !datacontext.Vakmans.Any(c => c.ContactIdOrigineel == cc.persoon_ID)).ToList();

            // caching weggehaald
            //if (ApplicationState.GetValue<List<persoon>>("persoons") == null)
            //{
            //    ApplicationState.SetValue("persoons", datacontext.persoons.ToList());
            //    return ApplicationState.GetValue<List<persoon>>("persoons");
            //}
            //else
            //{
            //    return ApplicationState.GetValue<List<persoon>>("persoons");
            //}

        }

        /// <summary>
        /// returns only projects containing all vakmannen in Naam or ProjectId
        /// </summary>
        /// <param name="strArguments"></param>
        /// <returns></returns>
        public List<vwContactAll> GetContacten(string[] strArguments)
        {
            List<vwContactAll> listContacten = GetContacten();

            foreach (string argument in strArguments)
            {
                listContacten = listContacten.Where(c => c.persoon_nr.ToString().Contains(argument) || c.achternaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || c.voornaam.Contains(argument, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return listContacten;
        }



    }
}

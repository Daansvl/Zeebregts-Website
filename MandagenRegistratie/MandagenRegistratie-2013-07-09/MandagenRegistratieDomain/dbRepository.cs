using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ZeebregtsLogic;
using System.Configuration;
using System.Web;
using System.Data.SqlClient;

namespace MandagenRegistratieDomain
{
    public class dbRepository
    {

        public dbDataContext datacontext = new dbDataContext(ConfigurationManager.ConnectionStrings["MandagenRegistratieDomain.Properties.Settings.MandagenRegistratieConnectionString"].ConnectionString);
        public string connectionString = ConfigurationManager.ConnectionStrings["MandagenRegistratieDomain.Properties.Settings.MandagenRegistratieConnectionString"].ConnectionString;

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
        ///  Inserts a new Bedrijf, returns the new bedrijf, inclusive of its key
        /// </summary>
        /// <param name="bedrijf"></param>
        /// <returns></returns>
        public Bedrijf InsertBedrijf(Bedrijf bedrijf)
        {
            // 
            datacontext.Bedrijfs.InsertOnSubmit(bedrijf);
            
            datacontext.SubmitChanges();

            return bedrijf;
        }

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

        //    projectToSave.Actief = project.Actief;
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

        //    vakmanToSave.Actief = vakman.Actief;
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

        public Bedrijf GetBedrijf(int bedrijfId)
        {
            return datacontext.Bedrijfs.Where(b => b.BedrijfId == bedrijfId).FirstOrDefault();
        }



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


        public List<Mandagen> GetMandagen(DateTime dtBegintijd)
        {

            List<Mandagen> resultaat = datacontext.Mandagens.Where(m => m.Begintijd >= dtBegintijd && m.Begintijd < dtBegintijd.AddDays(1)).ToList();

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

            List<Mandagen> resultaat = ApplicationState.GetValue<List<Mandagen>>("GetMandagenByProject_" + projectId.ToString());

            if (resultaat == null)
            {
                resultaat = datacontext.Mandagens.Where(p => p.ProjectId == projectId).ToList();

                ApplicationState.SetValue("GetMandagenByProject_" + projectId.ToString(), resultaat);
            }

            return resultaat;

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

        public List<Projectleider> GetProjectleiders()
        {
            return datacontext.Projectleiders.ToList();
        }



        public Project GetProject(int projectId)
        {
            Project project = ApplicationState.GetValue<Project>("GetProject_" + projectId.ToString());

            if (project == null)
            {
                project = datacontext.Projects.Where(p => p.ProjectId == projectId).FirstOrDefault();
                ApplicationState.SetValue("GetProject_" + projectId.ToString(), project);
            }

            return project;
        }

        public Project GetProjectByProjectIdOrigineel(int projectId)
        {
            Project project = datacontext.Projects.Where(p => p.ProjectIdOrigineel == projectId).FirstOrDefault();
            
            return project;
        }

        public List<vwProjectAll> GetVwProjectsAll()
        {
            //if (ApplicationState.GetValue<List<project>>("projects") == null)
            //{
            //    ApplicationState.SetValue("projects", datacontext.projects.ToList());
            //    return ApplicationState.GetValue<List<project>>("projects");
            //}
            //else
            //{
            //    return ApplicationState.GetValue<List<project>>("projects");
            //}

            return datacontext.vwProjectAlls.ToList();
        }

        /// <summary>
        /// returns only projects containing all arguments in Naam or ProjectId
        /// </summary>
        /// <param name="strArguments"></param>
        /// <returns></returns>
        public List<vwProjectAll> GetVwProjectsAll(string[] strArguments)
        {
            List<vwProjectAll> listProjects = GetVwProjectsAll();

            foreach (string argument in strArguments)
            {
                listProjects = listProjects.Where(p => p.Bedrijfsnaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.voornaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.achternaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.naam_project.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.ProjectNrOrigineel.ToString().Contains(argument, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return listProjects;
        }




        public Projectleider GetProjectleider(int projectleiderId)
        {
            Projectleider projectLeider = ApplicationState.GetValue<Projectleider>("GetProjectleider_" + projectleiderId.ToString());

            if (projectLeider == null)
            {
                projectLeider = datacontext.Projectleiders.Where(p => p.ProjectleiderId == projectleiderId).FirstOrDefault();
                ApplicationState.SetValue("GetProjectleider_" + projectleiderId.ToString(), projectLeider);
            }

            return projectLeider;
        }

        public List<Project> GetProjectsByVakmanId(int vakmanId, DateTime weekstart)
        {
            var query = (from m in datacontext.Mandagens.ToList()
                         where m.ProjectId != 0 && m.Begintijd < weekstart.AddDays(14) && m.Begintijd > weekstart.AddDays(-7)
                         && m.VakmanId == vakmanId
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
        public List<vwVakman> GetVakmannenByProjectId(int projectId, DateTime weekstart)
        {
            // return datacontext.Vakmans.Where(v => v.Mandagens.Any(m => m.VakmanId == v.VakmanId && m.ProjectId == projectId && m.Begintijd < weekstart.AddDays(14) && m.Begintijd > weekstart.AddDays(-7))).ToList();

            var query = (from m in datacontext.Mandagens.ToList()
                         where m.ProjectId == projectId && m.Begintijd < weekstart.AddDays(14) && m.Begintijd > weekstart.AddDays(-7)
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
            return datacontext.Mandagens.Where(m => m.VakmanId == vakmanId && m.Begintijd.Year == datum.Year && m.Begintijd.Month == datum.Month && m.Begintijd.Day == datum.Day && (m.Eindtijd.Hour > 0 || m.Eindtijd.Minute > 0) && m.Project.ProjectleiderId != projectleiderId).ToList();

        }


        /// <summary>
        /// Resets all current mandagen, and returns if user is allowed to perform change independently
        /// </summary>
        /// <param name="vakmanId"></param>
        /// <param name="datum"></param>
        /// <param name="projectleiderId"></param>
        /// <returns></returns>
        public void ResetMandagenNietIngevuldVoorVakmanId(int vakmanId, DateTime datum, int projectleiderId)
        {
            // reset mandagen
            ApplicationState.SetValue("GetMandagen_" + vakmanId.ToString(), null);

            foreach (Mandagen mandag in datacontext.Mandagens.Where(m => m.VakmanId == vakmanId && m.Begintijd.Year == datum.Year && m.Begintijd.Month == datum.Month && m.Begintijd.Day == datum.Day))
            {
                mandag.Bevestigd = true;
                mandag.Gewijzigd = false;
                mandag.Geannulleerd = false;
                mandag.Uren = 0;
                mandag.Minuten = 0;
                mandag.UrenGewijzigd = 0;
                mandag.MinutenGewijzigd = 0;
                mandag.MutatieDoorProjectleiderId = projectleiderId;
                mandag.Mutatiedatum = DateTime.Now;
                mandag.Eindtijd = mandag.Begintijd.AddHours(mandag.Uren);

                // TODO: CHECK OF DIT KLOPT!!!
                datacontext.Mandagens.DeleteOnSubmit(mandag);

                ApplicationState.SetValue("GetMandagenByProject_" + mandag.ProjectId.ToString(), null);

            }
            datacontext.SubmitChanges();
        }

        public void ResetMandagenGeannulleerdVoorVakmanId(int vakmanId, DateTime datum, int projectleiderId)
        {
            // reset mandagen
            ApplicationState.SetValue("GetMandagen_" + vakmanId.ToString(), null);

            // for each mandag van deze projectleiderId
            foreach (Mandagen mandag in datacontext.Mandagens.Where(m => m.VakmanId == vakmanId && m.Begintijd.Year == datum.Year && m.Begintijd.Month == datum.Month && m.Begintijd.Day == datum.Day && m.ProjectleiderId == projectleiderId))
            {
                //mandag.UrenGewijzigd = 0;
                //mandag.MinutenGewijzigd = 0;
                mandag.Geannulleerd = false;
            }

            datacontext.SubmitChanges();
        }

        public void ResetMandagenGeannulleerdVoorVakmanId(int vakmanId)
        {
            // reset mandagen
            ApplicationState.SetValue("GetMandagen_" + vakmanId.ToString(), null);

            foreach (Mandagen mandag in datacontext.Mandagens.Where(m => m.VakmanId == vakmanId))
            {
                mandag.Geannulleerd = false;
            }

            datacontext.SubmitChanges();
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


            mandag.VakmansoortId = originalMandag.VakmansoortId;
            mandag.VakmanstatusId = originalMandag.VakmanstatusId;
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


        public void CancelMandagenVoorVakmanId(int vakmanId, DateTime datum, int projectleiderId)
        {
            // reset mandagen
            ApplicationState.SetValue("GetMandagen_" + vakmanId.ToString(), null);

            foreach (Mandagen mandag in datacontext.Mandagens.Where(m => m.VakmanId == vakmanId && m.Begintijd.Year == datum.Year && m.Begintijd.Month == datum.Month && m.Begintijd.Day == datum.Day))
            {
                // alleen mandagen wijzigen waarvan ik de initiator ben OF eigenaar ben
                if (mandag.ProjectleiderId == projectleiderId || mandag.Project.ProjectleiderId == projectleiderId)
                {
                    if (!mandag.Bevestigd && mandag.MutatieDoorProjectleiderId != projectleiderId)
                    {
                        mandag.Geannulleerd = true;
                    }

                    mandag.Bevestigd = true;
                    mandag.Gewijzigd = false;
                    mandag.MutatieDoorProjectleiderId = projectleiderId;
                    mandag.Mutatiedatum = DateTime.Now;
                    mandag.Eindtijd = mandag.Begintijd.AddHours(mandag.Uren);

                    ApplicationState.SetValue("GetMandagenByProject_" + mandag.ProjectId.ToString(), null);
                }

            }

            datacontext.SubmitChanges();
        }

        public void CancelMandagenVoorVakmanId(int vakmanId, int projectleiderId)
        {
            // reset mandagen
            ApplicationState.SetValue("GetMandagen_" + vakmanId.ToString(), null);

            foreach (Mandagen mandag in datacontext.Mandagens.Where(m => m.VakmanId == vakmanId))
            {
                // alleen mandagen wijzigen waarvan ik de initiator ben OF eigenaar ben
                if (mandag.ProjectleiderId == projectleiderId || mandag.Project.ProjectleiderId == projectleiderId)
                {
                    if (!mandag.Bevestigd && mandag.MutatieDoorProjectleiderId != projectleiderId)
                    {
                        mandag.Geannulleerd = true;
                    }

                    mandag.Bevestigd = true;
                    mandag.Gewijzigd = false;
                    mandag.MutatieDoorProjectleiderId = projectleiderId;
                    mandag.Mutatiedatum = DateTime.Now;
                    mandag.Eindtijd = mandag.Begintijd.AddHours(mandag.Uren).AddMinutes(mandag.Minuten);

                    ApplicationState.SetValue("GetMandagenByProject_" + mandag.ProjectId.ToString(), null);
                }
            }

            datacontext.SubmitChanges();
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
                //    Mandagen newMandag = new Mandagen();
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
                foreach (Mandagen mandag in datacontext.Mandagens.Where(m => m.VakmanId == mandagToConfirm.VakmanId && m.Eindtijd > m.Begintijd && m.ProjectId == mandagToConfirm.ProjectId && m.Status).OrderBy(o=>o.Begintijd).OrderBy(o=>o.Eindtijd))
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
                newMandag.MinutenGewijzigd = mandag.MinutenGewijzigd;
                newMandag.Mutatiedatum = mandag.Mutatiedatum;
                newMandag.MutatieDoorProjectleiderId = mandag.MutatieDoorProjectleiderId;
                newMandag.ProjectId = mandag.ProjectId;
                newMandag.ProjectleiderId = mandag.ProjectleiderId;
                //newMandag.Status = mandag.Status;
                newMandag.Uren = mandag.Uren;
                newMandag.VakmanId = mandag.VakmanId;
                newMandag.VakmansoortId = mandag.VakmansoortId;
                newMandag.VakmanstatusId = mandag.VakmanstatusId;

                newMandag.Status = true;

                datacontext.Mandagens.InsertOnSubmit(newMandag);
            }

            datacontext.SubmitChanges();

        }

        public void ConfirmMandagenVoorVakmanId(int vakmanId, int projectleiderId)
        {
            // reset mandagen
            ApplicationState.SetValue("GetMandagen_" + vakmanId.ToString(), null);

            foreach (Mandagen mandag in datacontext.Mandagens.Where(m => m.VakmanId == vakmanId))
            {
                                // alleen mandagen wijzigen waarvan ik de eigenaar ben
                if (mandag.Project.ProjectleiderId == projectleiderId)
                {

                    mandag.Geannulleerd = false;
                    mandag.Bevestigd = true;
                    mandag.Gewijzigd = false;

                    int tempUren = mandag.UrenGewijzigd;
                    mandag.Uren = tempUren;

                    int tempMinuten = mandag.MinutenGewijzigd;
                    mandag.Minuten = tempMinuten;

                    mandag.MutatieDoorProjectleiderId = projectleiderId;
                    mandag.Mutatiedatum = DateTime.Now;
                    mandag.Eindtijd = mandag.Begintijd.AddHours(tempUren);

                    ApplicationState.SetValue("GetMandagenByProject_" + mandag.ProjectId.ToString(), null);

                }
            }

            datacontext.SubmitChanges();
        }

        /// <summary>
        /// Bewaar alle wijzigingen die de gebruiker zojuist heeft gedaan
        /// </summary>
        /// <param name="vakmanId"></param>
        /// <param name="projectleiderId"></param>
        public void AutoConfirmMandagenForWeekView(int vakmanId, int projectleiderId)
        {

            foreach (Mandagen mandag in datacontext.Mandagens.Where(m => m.VakmanId == vakmanId && m.Gewijzigd == true && m.MutatieDoorProjectleiderId == projectleiderId && m.Project.ProjectleiderId == projectleiderId))
            {
                if (!datacontext.Mandagens.Any(m => m.VakmanId == vakmanId && m.Gewijzigd == true && m.MutatieDoorProjectleiderId != projectleiderId && m.Begintijd == mandag.Begintijd))
                {
                    //mandag.Bevestigd = true;
                    mandag.Gewijzigd = false;

                    int tempUren = mandag.UrenGewijzigd;
                    mandag.Uren = tempUren;

                    int tempMinuten = mandag.MinutenGewijzigd;
                    mandag.Minuten = tempMinuten;

                    mandag.MutatieDoorProjectleiderId = projectleiderId;
                    mandag.Mutatiedatum = DateTime.Now;
                    mandag.Eindtijd = mandag.Begintijd;
                }
            }

            datacontext.SubmitChanges();
        }

        public Mandagen GetMandag(int vakmanId, int projectId, DateTime datum, int vakmanStatus, bool status)
        {
            return datacontext.Mandagens.Where(m => m.ProjectId == projectId && m.VakmanId == vakmanId && m.Begintijd == datum && m.VakmanstatusId == vakmanStatus && m.Status == status).FirstOrDefault();
        }

        public Mandagen GetMandagOngeveer(int vakmanId, int projectId, DateTime datum, int vakmanStatus, bool status)
        {
            return datacontext.Mandagens.Where(m => m.ProjectId == projectId && m.VakmanId == vakmanId && m.Begintijd <= datum && m.Eindtijd >= datum && m.VakmanstatusId == vakmanStatus && m.Status == status).FirstOrDefault();
        }


        public void InsertMandag(Mandagen mandag)
        {
            Mandagen oldMandag = GetMandag(mandag.VakmanId, mandag.ProjectId, mandag.Begintijd, mandag.VakmanstatusId, mandag.Status);

            ApplicationState.SetValue("GetMandagen_" + mandag.VakmanId.ToString(), null);

            ApplicationState.SetValue("GetMandagenByProject_" + mandag.ProjectId.ToString(), null);

            if (oldMandag != null && oldMandag.Eindtijd > oldMandag.Begintijd)
            {
                oldMandag.UrenGewijzigd = mandag.UrenGewijzigd;
                oldMandag.MinutenGewijzigd = mandag.MinutenGewijzigd;
                oldMandag.Bevestigd = mandag.Bevestigd;
                oldMandag.Gewijzigd = mandag.Gewijzigd;
                oldMandag.VakmanstatusId = mandag.VakmanstatusId;
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
                listProjects = listProjects.Where(p => p.voornaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.achternaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.Bedrijfsnaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.Naam.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.ProjectNrOrigineel.ToString().Contains(argument, StringComparison.OrdinalIgnoreCase)).ToList();
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
                listVakmannen = listVakmannen.Where(v => v.bedrijfnaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || v.bedrijfzoeknaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || v.bedrijfplaats.Contains(argument, StringComparison.OrdinalIgnoreCase) || v.achternaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || v.voornaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || v.Adres.Contains(argument, StringComparison.OrdinalIgnoreCase) || v.ContactIdOrigineel.ToString().Contains(argument, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return listVakmannen;
        }

    }
}

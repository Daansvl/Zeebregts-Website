using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeebregtsLogic;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace MandagenRegistratieDomain
{
    public class dbOriginalRepository
    {
        public dbOriginalRepository()
        {
            dbRepository checkDbName = new dbRepository();
            datacontext = new dbOriginalDataContext(checkDbName.connectionStringForeignDatabase());
        }

        //public dbOriginalDataContext datacontext = new dbOriginalDataContext(ConfigurationManager.ConnectionStrings["ZeebregtsDBConnectionString"].ConnectionString);
        public dbOriginalDataContext datacontext;

        public MDRproject GetProject(int projectNr)
        {
            return GetProject(projectNr, false);
        }

        private int intUseCacheSeconds = 8;

        public bool UseCache(string listName)
        {
            DateTime lastUsedCache = ApplicationState.GetValue<DateTime>(listName + "Cache");

            if (lastUsedCache == null)
            {
                // make sure we DO use the cache the next time
                ApplicationState.SetValue(listName + "Cache", DateTime.Now);
                return false;
            }
            else if (lastUsedCache != null && lastUsedCache.AddSeconds(intUseCacheSeconds) < DateTime.Now)
            {
                // make sure we DO use the cache the next time
                ApplicationState.SetValue(listName + "Cache", DateTime.Now);
                return false;
            }
            else
            {
                return true;
            }

        }


        public MDRproject GetProject(int projectNr, bool useCache)
        {
            if (!useCache)
            {
                ApplicationState.SetValue("listProjects", null);
            }



            return GetProjects(UseCache("listProjects")).Where(p => p.project_NR == projectNr).FirstOrDefault();

            //if (ApplicationState.GetValue<dbo_project>("dbo_project_" + projectId.ToString()) == null)
            //{
            //    ApplicationState.SetValue("dbo_project_" + projectId.ToString(), datacontext.dbo_projects.Where(p => p.project_ID == projectId).FirstOrDefault());
            //    return ApplicationState.GetValue<dbo_project>("dbo_project_" + projectId.ToString());
            //}
            //else
            //{
            //    return ApplicationState.GetValue<dbo_project>("dbo_project_" + projectId.ToString());
            //}

        }

        public MDRbedrijf GetBedrijf(int bedrijfNr)
        {
            return datacontext.MDRbedrijfs.Where(b => b.bedrijf_nr == bedrijfNr).FirstOrDefault();
        }

        public MDRbedrijf GetBedrijfByID(int bedrijfId)
        {
            return datacontext.MDRbedrijfs.Where(b => b.bedrijf_ID == bedrijfId).FirstOrDefault();
        }

        public MDRpersoon GetContact(int contactId)
        {
            return GetContact(contactId, false);
        }

        public MDRpersoon GetContact(int contactId, bool useCache)
        {

            if (!useCache)
            {
                ApplicationState.SetValue("listPersoons", null);
            }

            return Persoons(useCache).Where(p => p.persoon_ID == contactId).FirstOrDefault();

            //if (ApplicationState.GetValue<dbo_project>("dbo_project_" + projectId.ToString()) == null)
            //{
            //    ApplicationState.SetValue("dbo_project_" + projectId.ToString(), datacontext.dbo_projects.Where(p => p.project_ID == projectId).FirstOrDefault());
            //    return ApplicationState.GetValue<dbo_project>("dbo_project_" + projectId.ToString());
            //}
            //else
            //{
            //    return ApplicationState.GetValue<dbo_project>("dbo_project_" + projectId.ToString());
            //}

        }

        /// <summary>
        /// ge-cachede lijst met alle personen uit daans database
        /// </summary>
        /// <returns></returns>
        public List<MDRpersoon> Persoons(bool useCache)
        {
            //if (useCache)
            //{
            //    List<persoon> listPersoons = ApplicationState.GetValue<List<persoon>>("listPersoons");
            //    if (listPersoons == null)
            //    {
            //        listPersoons = datacontext.persoons.ToList();
            //        ApplicationState.SetValue("listPersoons", listPersoons);
            //    }

            //    return listPersoons;
            //}
            //else
            //{
            return datacontext.MDRpersoons.LinqCache().ToList();

            //}
        }

        public List<MDRproject> GetProjects(bool useCache)
        {
            ////if (ApplicationState.GetValue<List<project>>("projects") == null)
            ////{
            ////    ApplicationState.SetValue("projects", datacontext.projects.ToList());
            ////    return ApplicationState.GetValue<List<project>>("projects");
            ////}
            ////else
            ////{
            ////    return ApplicationState.GetValue<List<project>>("projects");
            ////}

            //if (useCache)
            //{
            //    List<project> listProjects = ApplicationState.GetValue<List<project>>("listProjects");
            //    if (listProjects == null)
            //    {
            //        listProjects = datacontext.projects.ToList();
            //        ApplicationState.SetValue("listProjects", listProjects);
            //    }

            //    return listProjects;

            //}
            //else
            //{
            return datacontext.MDRprojects.LinqCache().ToList();
            //}

       }



        public List<MDRpersoon> GetContacten()
        {
            return datacontext.MDRpersoons.ToList();

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
        /// returns only projects containing all arguments in Naam or ProjectId
        /// </summary>
        /// <param name="strArguments"></param>
        /// <returns></returns>
        public List<MDRproject> GetProjects(string[] strArguments)
        {
            List<MDRproject> listProjects = GetProjects(false);

            foreach (string argument in strArguments)
            {
                listProjects = listProjects.Where(p => p.naam_project.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.project_NR.ToString().Contains(argument, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return listProjects;
        }

        ///// <summary>
        ///// returns only projects containing all arguments in Naam or ProjectId
        ///// </summary>
        ///// <param name="strArguments"></param>
        ///// <returns></returns>
        //public List<MDRproject> GetProjectsIn(List<int> listProjectIdOrigineels)
        //{
        //    List<MDRproject> listProjects = datacontext.MDRprojects.Where(p => listProjectIdOrigineels.Contains(p.project_ID)).ToList();

        //    return listProjects;
        //}


        /// <summary>
        /// returns only projects containing all vakmannen in Naam or ProjectId
        /// </summary>
        /// <param name="strArguments"></param>
        /// <returns></returns>
        public List<MDRpersoon> GetContacten(string[] strArguments)
        {
            List<MDRpersoon> listContacten = GetContacten();

            foreach (string argument in strArguments)
            {
                listContacten = listContacten.Where(c => c.persoon_nr.ToString().Contains(argument) || c.persoon_ID.ToString().Contains(argument) || c.achternaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || c.voornaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || c.voorletters.Contains(argument, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return listContacten;
        }

        /// <summary>
        /// returns only projects containing all vakmannen in Naam or ProjectId
        /// </summary>
        /// <param name="strArguments"></param>
        /// <returns></returns>
        public List<MDRpersoon> GetContactenIn(List<int> listVwVakmannen)
        {
            List<MDRpersoon> listpersoons = datacontext.MDRpersoons.Where(p => listVwVakmannen.Contains(p.persoon_ID)).ToList();

            return listpersoons;
        }

    }
}

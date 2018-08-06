using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeebregtsLogic;
using System.Data;
using System.Configuration;

namespace MandagenRegistratieDomain
{
    public class dbOriginalRepository
    {

        public dbOriginalDataContext datacontext = new dbOriginalDataContext(ConfigurationManager.ConnectionStrings["MandagenRegistratieDomain.Properties.Settings.ZeebregtsTestConnectionString"].ConnectionString);


        public project GetProject(int projectId)
        {
            return datacontext.projects.Where(p => p.project_ID == projectId).FirstOrDefault();

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

        public bedrijf GetBedrijf(int bedrijfNr)
        {
            return datacontext.bedrijfs.Where(b => b.bedrijf_nr == bedrijfNr).FirstOrDefault();
        }

        public persoon GetContact(int contactId)
        {
            return datacontext.persoons.Where(p => p.persoon_ID == contactId).FirstOrDefault();

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

        public List<project> GetProjects()
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

            return datacontext.projects.ToList();
        }



        public List<persoon> GetContacten()
        {
            return datacontext.persoons.ToList();

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
        public List<project> GetProjects(string[] strArguments)
        {
            List<project> listProjects = GetProjects();

            foreach (string argument in strArguments)
            {
                listProjects = listProjects.Where(p => p.naam_project.Contains(argument, StringComparison.OrdinalIgnoreCase) || p.project_NR.ToString().Contains(argument, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return listProjects;
        }


        /// <summary>
        /// returns only projects containing all vakmannen in Naam or ProjectId
        /// </summary>
        /// <param name="strArguments"></param>
        /// <returns></returns>
        public List<persoon> GetContacten(string[] strArguments)
        {
            List<persoon> listContacten = GetContacten();

            foreach (string argument in strArguments)
            {
                listContacten = listContacten.Where(c => c.achternaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || c.voornaam.Contains(argument, StringComparison.OrdinalIgnoreCase) || c.voorletters.Contains(argument, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return listContacten;
        }

    }
}

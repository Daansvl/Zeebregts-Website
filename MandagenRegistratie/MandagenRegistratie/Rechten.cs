using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie
{
    public static class Rechten
    {
        public static bool IsAdmin
        {
            get 
            {
                return ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).IsAdministrator || ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruikerFirst).IsAdministrator;
            }
        }


        public static bool IsProjectleider
        {
            get
            {
                return ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).IsProjectleider || ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruikerFirst).IsProjectleider;
            }
        }

        public static bool CanLoginAsProjectleider
        {
            get
            {
                return ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).CanLoginAsProjectleider || ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruikerFirst).CanLoginAsProjectleider;
            }
        }

        public static bool CanPrint
        {
            get
            {
                return ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruiker).CanPrint || ApplicationState.GetValue<Gebruiker>(ApplicationVariables.objGebruikerFirst).CanPrint;
            }
        }



    }
}

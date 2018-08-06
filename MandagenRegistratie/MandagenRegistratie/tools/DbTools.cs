using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MandagenRegistratieDomain;
using ZeebregtsLogic;

namespace MandagenRegistratie.tools
{
    public static class DbToolsExtensions
    {

        public class MandagShow {

            public Project project { get; set; }
            public Vakman vakman { get; set; }
            public DateTime begintijd { get; set; }
            public DateTime eindtijd { get; set; }
            public bool status { get; set; }
            public TimeSpan timespan { get; set; }
            public Mandagen mandag { get; set; }

            public int projectId { get; set; }
        }

        public class AanvraagShowRecord
        {
            public AanvraagShowRecord()
            {
                isTime = false;
            }

            public AanvraagShowRecord(string _text)
            {
                text = _text;
                isTime = false;
            }
            public int aanvraagId { get; set; }
            public bool isTime { get; set; }
            public string text { get; set; }
            public TimeSpan tijd1 { get; set; }
            public TimeSpan tijd2 { get; set; }
            public string scheidingsteken { get; set; }
            public int projectId { get; set; }
        }


        //public static void AddAanvragen(this StackPanel strTooltipAanvraag, List<Mandagen> listMandagenTotal, List<Mandagen> listAanvragen)
        //{
        //    //dbRepository dbrep = new dbRepository();
        //    dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
        //    DbTools dbtools = new DbTools();

        //    // get alle aanvragen voor dit project
        //    // List<Mandagen> listAanvragen = listMandagenTotal.Where(m => !m.Status).ToList();

        //    int previousVakmanId = -1;
        //    int previousProjectId = -1;

        //    List<AanvraagShowRecord> aanvraagTexten = new List<AanvraagShowRecord>();

        //        //            List<Mandagen> listAangevraagdeMandagen = dbrep.datacontext.Mandagens.Where(m => m.ProjectId == project.ProjectId && m.VakmanId == vakman.VakmanId && m.Begintijd >= datum && m.Eindtijd < datum.AddDays(1) && !m.Status).ToList();
        //        //List<Mandagen> listIngeplandeMandagen = dbrep.GetMandagen(vakman.VakmanId, datum).Where(m => m.Status).ToList();

        //    int aanvraagId = 1;

        //    foreach (Mandagen mandag in listAanvragen.OrderBy(p => p.ProjectId).ThenBy(v => v.VakmanId))
        //    {
        //        List<Mandagen> listOriginelen = listMandagenTotal.Where(m => m.VakmanId == mandag.VakmanId && DbTools.HasOverlap(m, mandag) && m.Status).OrderBy(o => o.ProjectId).ToList();

        //        // alleen nogmaals tonen als het niet meer om dezelfde vakman gaat en dus een nieuwe aanvraag is.
        //        if (previousVakmanId != mandag.VakmanId || previousProjectId != mandag.ProjectId)
        //        {
        //            aanvraagId += 1;

        //            // re-initialise objVakman
        //            MDRpersoon objVakman = dbrepOriginal.GetContact(mandag.Vakman.ContactIdOrigineel);
        //            AanvraagShowRecord asr = new AanvraagShowRecord("Aanvraag " + (objVakman.voornaam + " " + objVakman.tussenvoegsel + " " + objVakman.achternaam).ToStringTrimmed() + (mandag.Geannulleerd ? " AFGEWEZEN" : ""));
        //            asr.aanvraagId = aanvraagId;
        //            asr.projectId = mandag.ProjectId;
        //            aanvraagTexten.Add(asr);
        //            aanvraagId += 1;

        //            // plus text is cumulatief
        //            MDRproject objProjectNaar = dbrepOriginal.GetProject((int)mandag.Project.ProjectNr, true);

        //            AanvraagShowRecord asr2 = new AanvraagShowRecord();
        //            asr2.tijd1 = dbtools.GetIngeplandeTijd(mandag.Project, mandag.Vakman, dbtools.GetHeleDag(mandag.Begintijd), listMandagenTotal);
        //            asr2.tijd2 = dbtools.GetAangevraagdeTijd(mandag.Project, mandag.Vakman, dbtools.GetHeleDag(mandag.Begintijd), listOriginelen.Select(m => m.ProjectId).ToList(), listMandagenTotal);
        //            asr2.text = objProjectNaar.naam_project + " (Project " + objProjectNaar.project_NR.ToString() + ")";
        //            asr2.isTime = true;
        //            asr2.projectId = mandag.ProjectId;
        //            asr2.scheidingsteken = "+";
        //            asr2.aanvraagId = aanvraagId;
        //            aanvraagTexten.Add(asr2);
        //            aanvraagId += 1;

        //            // GetAangevraagdeTijd(mandag.Project, mandag.Vakman, GetHeleDag(mandag.Begintijd)) WEGGEHAALD
        //            //strTooltipAanvraag.AddText("(" + dbtools.GetIngeplandeTijd(mandag.Project, mandag.Vakman, dbtools.GetHeleDag(mandag.Begintijd), listMandagenTotal) + ") + (" + dbtools.GetAangevraagdeTijd(mandag.Project, mandag.Vakman, dbtools.GetHeleDag(mandag.Begintijd), listOriginelen.Select(m => m.ProjectId).ToList(), listMandagenTotal) + ") " + objProjectNaar.naam_project + " (Project " + objProjectNaar.project_NR.ToString() + ")");
        //        }

        //        // minus text is een opsomming van alle mandagen die beinvloed worden door de aanvraag
        //        foreach (MandagShow origineel in listOriginelen.GroupBy(g => g.ProjectId).Select(m => new MandagShow() { begintijd = m.First().Begintijd, eindtijd = m.First().Eindtijd, project = m.First().Project, timespan = new TimeSpan(m.Sum(c=> DbTools.GetOverlapTicks(mandag, c))), mandag = m.First(), projectId = m.First().ProjectId }))
        //        {
        //            Project objProjectVan = origineel.project;

        //            AanvraagShowRecord asr3 = new AanvraagShowRecord();
        //            asr3.tijd1 = DbTools.GetTijd(origineel.mandag);
        //            asr3.aanvraagId = aanvraagId;
        //            asr3.projectId = origineel.projectId;
        //            asr3.tijd2 = origineel.timespan;
        //            asr3.isTime = true;
        //            asr3.scheidingsteken = "\u2212";
        //            asr3.text = objProjectVan.Naam + " (Project " + objProjectVan.ProjectNr.ToString() + ")";
        //            aanvraagTexten.Add(asr3);

        //            // GetIngeplandeTijd(objProjectVan, mandag.Vakman, GetHeleDag(mandag.Begintijd))
        //            //strTooltipAanvraag.AddText("(" + DbTools.ToonTijd(origineel.mandag) + ") \u2212 (" + DbTools.ToonTijd(origineel.timespan) + ") " + objProjectVan.Naam + " (Project " + objProjectVan.ProjectNr.ToString() + ")");
        //        }






        //        previousVakmanId = mandag.VakmanId;
        //        previousProjectId = mandag.ProjectId;
        //    }


        //    foreach (AanvraagShowRecord asr in aanvraagTexten.GroupBy(a => new { a.aanvraagId, a.projectId }).Select(a => new AanvraagShowRecord() { isTime = a.First().isTime, scheidingsteken = a.First().scheidingsteken, text = a.First().text, tijd1 = a.First().tijd1, tijd2 = new TimeSpan(a.Sum(t => t.tijd2.Ticks)) }))
        //    {
        //        if (asr.isTime)
        //        {
        //            strTooltipAanvraag.AddText("(" + DbTools.ToonTijd(asr.tijd1) + ") " + asr.scheidingsteken + " (" + DbTools.ToonTijd(asr.tijd2) + ") " + asr.text);
        //        }
        //        else
        //        {
        //            strTooltipAanvraag.AddText(asr.text);
        //        }
        //    }

        //}

        public static void AddAanvragen(this StackPanel strTooltipAanvraag, List<Mandagen> listMandagenTotal, List<Mandagen> listAanvragen)
        {
            //dbRepository dbrep = new dbRepository();
            //dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
            DbTools dbtools = new DbTools();

            // get alle aanvragen voor dit project
            // List<Mandagen> listAanvragen = listMandagenTotal.Where(m => !m.Status).ToList();

            int previousVakmanId = -1;
            int previousProjectId = -1;
            int previousVakmanIdOrigineel = -1;
            int previousProjectIdOrigineel = -1;
            DateTime previousBegintijdOrigineel = DateTime.MinValue;

            //List<Mandagen> listOriginelen = new List<Mandagen>();

                int aanvraagProjectId = 0;
            // voor elke vakman in de lijst
            foreach (MandagShow ms in listAanvragen.GroupBy(a => a.VakmanId).Select(a => new MandagShow() { vakman = a.First().Vakman, projectId = a.First().ProjectId }))
            {
            List<AanvraagShowRecord> aanvraagTexten = new List<AanvraagShowRecord>();
                // voor elke aanvraag, per 
                foreach (Mandagen mandag in listAanvragen.Where(a=>a.VakmanId == ms.vakman.VakmanId).OrderBy(p => p.ProjectId))
                {
                    List<Mandagen> listOriginelen = listMandagenTotal.Where(m => m.VakmanId == mandag.VakmanId && DbTools.HasOverlap(m, mandag) && m.Status).OrderBy(o => o.ProjectId).ToList();
                    //listOriginelen.AddRange(listMandagenTotal.Where(m => m.VakmanId == mandag.VakmanId && DbTools.HasOverlap(m, mandag) && m.Status).OrderBy(o => o.ProjectId).ToList());


                    // alleen nogmaals tonen als het niet meer om dezelfde vakman gaat en dus een nieuwe aanvraag is.
                    if (previousVakmanId != mandag.VakmanId)
                    {
                        // re-initialise objVakman
                        MDRpersoon objVakman = ApplicationState.GetValue<List<MDRpersoon>>(ApplicationVariables.listMDRPersoons).FirstOrDefault(a => a.persoon_ID == mandag.Vakman.ContactIdOrigineel);

                        strTooltipAanvraag.AddText("Aanvraag " + (objVakman.voornaam + " " + objVakman.tussenvoegsel + " " + objVakman.achternaam).ToStringTrimmed() + (mandag.Geannulleerd ? " AFGEWEZEN" : ""), true);

                        // plus text is cumulatief
                        //MDRproject objProjectNaar = dbrepOriginal.GetProject((int)mandag.Project.ProjectNr, true);
                        // // GetAangevraagdeTijd(mandag.Project, mandag.Vakman, GetHeleDag(mandag.Begintijd)) WEGGEHAALD
                        //strTooltipAanvraag.AddText("(" + DbTools.ToonTijd(dbtools.GetIngeplandeTijd(mandag.Project, mandag.Vakman, dbtools.GetHeleDag(mandag.Begintijd), listMandagenTotal)) + ") + (" + DbTools.ToonTijd(dbtools.GetAangevraagdeTijd(mandag.Project, mandag.Vakman, dbtools.GetHeleDag(mandag.Begintijd), listOriginelen.Select(m => m.ProjectId).ToList(), listMandagenTotal)) + ") " + objProjectNaar.naam_project + " (Project " + objProjectNaar.project_NR.ToString() + ")", true);
                    }

                    MDRproject objProjectNaar = ApplicationState.GetValue<List<MDRproject>>(ApplicationVariables.listMDRProjecten).FirstOrDefault(a => a.project_NR == (int)mandag.Project.ProjectNr);

                    strTooltipAanvraag.AddText("(" + DbTools.ToonTijd(dbtools.GetIngeplandeTijd(mandag.Project, mandag.Vakman, dbtools.GetHeleDag(mandag.Begintijd), listMandagenTotal)) + ") + (" + DbTools.ToonTijd(mandag) + ") " + objProjectNaar.naam_project + " (Project " + objProjectNaar.project_NR.ToString() + ")", true);

                    if (previousProjectId != mandag.ProjectId)
                    {
                    }
                        aanvraagProjectId++;

                    //// minus text is een opsomming van alle mandagen die beinvloed worden door de aanvraag
                    //foreach (Mandagen origineel in listOriginelen.OrderBy(a=>a.ProjectId).ThenBy(a=>a.Begintijd))
                    //{
                    //    //if (previousVakmanIdOrigineel != origineel.VakmanId || previousProjectIdOrigineel != origineel.ProjectId)
                    //    //{
                    //        Project objProjectVan = origineel.Project;
                    //        // GetIngeplandeTijd(objProjectVan, mandag.Vakman, GetHeleDag(mandag.Begintijd))
                    //        //strTooltipAanvraag.AddText("(" + DbTools.ToonTijd(origineel) + ") \u2212 (" + DbTools.ToonOverlap(mandag, origineel) + ") " + objProjectVan.Naam + " (Project " + objProjectVan.ProjectNr.ToString() + ")");
                    //        strTooltipAanvraag.AddText("(" + DbTools.ToonTijd(origineel) + ") \u2212 (" + DbTools.ToonTijd(origineel) + ") " + objProjectVan.Naam + " (Project " + objProjectVan.ProjectNr.ToString() + ")");

                    //    //}

                    //    previousVakmanIdOrigineel = origineel.VakmanId;
                    //    previousProjectIdOrigineel = origineel.ProjectId;

                    //}

                    previousVakmanId = mandag.VakmanId;
                    previousProjectId = mandag.ProjectId;

                    // minus text is een opsomming van alle mandagen die beinvloed worden door de aanvraag
                    foreach (Mandagen origineel in listOriginelen.OrderBy(a => a.ProjectId).ThenBy(a => a.Begintijd))
                    //foreach (MandagShow origineel in listOriginelen.OrderBy(a => a.ProjectId).GroupBy(g => g.ProjectId).Select(m => new MandagShow() { begintijd = m.First().Begintijd, vakman = m.First().Vakman, eindtijd = m.First().Eindtijd, project = m.First().Project, timespan = new TimeSpan(m.Sum(c => DbTools.GetOverlapTicks(mandag, c))), mandag = m.First(), projectId = m.First().ProjectId }))
                    {
                        Project objProjectVan = origineel.Project;

                        AanvraagShowRecord asr3 = new AanvraagShowRecord();
                        asr3.tijd1 = DbTools.GetTijd(origineel);
                        asr3.aanvraagId = aanvraagProjectId;
                        asr3.projectId = origineel.ProjectId;
                        asr3.tijd2 = DbTools.GetOverlap(origineel, mandag); // origineel.timespan;
                        asr3.isTime = true;
                        asr3.scheidingsteken = "\u2212";
                        asr3.text = objProjectVan.Naam + " (Project " + objProjectVan.ProjectNr.ToString() + ")";
                        aanvraagTexten.Add(asr3);

                        previousVakmanIdOrigineel = origineel.VakmanId;
                        previousProjectIdOrigineel = origineel.ProjectId;
                        previousBegintijdOrigineel = origineel.Begintijd;
                    }

                }


                foreach (AanvraagShowRecord asr in aanvraagTexten.OrderBy(a=>a.projectId).GroupBy(a => new { a.aanvraagId, a.projectId }).Select(a => new AanvraagShowRecord() { isTime = a.First().isTime, scheidingsteken = a.First().scheidingsteken, text = a.First().text, tijd1 = a.First().tijd1, tijd2 = new TimeSpan(a.Sum(t => t.tijd2.Ticks)) }))
                {
                    if (asr.isTime)
                    {
                        strTooltipAanvraag.AddText("(" + DbTools.ToonTijd(asr.tijd1) + ") " + asr.scheidingsteken + " (" + DbTools.ToonTijd(asr.tijd2) + ") " + asr.text);
                    }
                    else
                    {
                        strTooltipAanvraag.AddText(asr.text);
                    }
                }

            }

        }
    }

    public class DbTools
    {


        public Vakman GetVakman(vwVakman v)
        {

            Vakman vakman = new Vakman();
            vakman.Actief = v.Actief;
            vakman.Adres = v.Adres;
            vakman.BedrijfIdOrigineel = v.BedrijfIdOrigineel;
            vakman.Bsn = v.Bsn;
            vakman.ContactIdOrigineel = v.ContactIdOrigineel;
            vakman.DefaultBeginminuut = v.DefaultBeginminuut;
            vakman.DefaultBeginuur = v.DefaultBeginuur;
            vakman.Di = v.Di;
            vakman.Do = v.Do;
            vakman.Huisnummer = v.Huisnummer;
            vakman.Intern = v.Intern;
            vakman.IsBijrijder = false;
            vakman.IsChauffeur = v.IsChauffeur;
            vakman.Kenteken = v.Kenteken;
            vakman.Kvk = v.Kvk;
            vakman.Land = v.Land;
            vakman.Loonkosten = v.Loonkosten;
            vakman.Ma = v.Ma;
            vakman.Ophaaladres = v.Ophaaladres;
            vakman.Ophaalhuisnummer = v.Ophaalhuisnummer;
            vakman.Ophaalland = v.Ophaalland;
            vakman.Ophaalplaats = v.Ophaalplaats;
            vakman.Ophaalpostcode = v.Ophaalpostcode;
            vakman.Plaats = v.Plaats;
            vakman.Postcode = v.Postcode;
            vakman.ProjectleiderId = v.ProjectleiderId;
            vakman.VakmanId = v.VakmanId;
            vakman.Var = v.Var;
            vakman.Vr = v.Vr;
            vakman.Werkweek = v.Werkweek;
            vakman.Wo = v.Wo;
            vakman.Za = v.Za;
            vakman.Zo = v.Zo;
            vakman.ZZP = v.ZZP;

            return vakman;

        }







        public string ToonNaam(MDRpersoon objPersoon)
        {
            return objPersoon.voornaam + " " + (string.IsNullOrWhiteSpace(objPersoon.tussenvoegsel) ? "" : objPersoon.tussenvoegsel + " ") + objPersoon.achternaam;
        }


        public class CompositeMandag
        {
            public TimeSpan tijdsduur { get; set; }
            public int ContactIdOrigineel { get; set; }
            public bool Status { get; set; }
        }


        public StackPanel AddTooltipAanvraagAll(Vakman vakman, Project project, DateTime datum, bool isProjectOriented, bool isOverzicht)
        {
            // haal alle mandagen op voor deze deze dag (volgorde van vakmanNaam)
            dbRepository dbrep = new dbRepository();
            List<Mandagen> listMandagenTotal = dbrep.GetMandagenCached(datum).Where(m => m.Begintijd != m.Eindtijd).ToList();

            return AddTooltipAanvraagAll(vakman, project, datum, isProjectOriented, isOverzicht, listMandagenTotal);
        }

        /// <summary>
        /// Show toolip gebaseerd op de volgende parameters
        /// </summary>
        /// <param name="vakman"></param>
        /// <param name="project"></param>
        /// <param name="datum"></param>
        /// <param name="isProjectOriented"></param>
        /// <param name="isOverzicht"></param>
        /// <returns></returns>
        public StackPanel AddTooltipAanvraagAll(Vakman vakman, Project project, DateTime datum, bool isProjectOriented, bool isOverzicht, List<Mandagen> listMandagenTotal, bool hasAanvraag = true, bool hasUren = true)
        {

            try
            {
                //Stopwatch sw = new Stopwatch();
                //sw.Start();

                //dbRepository dbrep = new dbRepository();
                //dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
                MDRpersoon objVakman = ApplicationState.GetValue<List<MDRpersoon>>(ApplicationVariables.listMDRPersoons).FirstOrDefault(a=>a.persoon_ID == vakman.ContactIdOrigineel);
                
                StackPanel strTooltipAanvraag = new StackPanel();

                // haal alle mandagen op voor deze deze dag (volgorde van vakmanNaam)
                //List<Mandagen> listMandagenTotal = dbrep.GetMandagenCached(datum).Where(m => m.Begintijd != m.Eindtijd).ToList();

                // haal alle mandagen op voor deze deze dag voor dit project
                List<Mandagen> listMandagenProjectTotal = new List<Mandagen>();
                // haal alle mandagen op voor deze deze dag voor deze vakman
                List<Mandagen> listMandagenVakmanTotal = new List<Mandagen>();

                if (hasUren)
                {
                    if (isProjectOriented)
                    {
                        // haal alle mandagen op voor deze deze dag voor dit project
                        listMandagenProjectTotal = listMandagenTotal.Where(m => m.ProjectId == project.ProjectId).ToList();
                    }
                    else
                    {
                        // haal alle mandagen op voor deze deze dag voor deze vakman
                        listMandagenVakmanTotal = listMandagenTotal.Where(m => m.VakmanId == vakman.VakmanId).OrderBy(m => m.Begintijd).ToList();
                    }
                }

                // #1 STAP 1
                // toon alle vakmannen op het project
                // #1 STAP 1
                // 
                //
                if (isProjectOriented)
                {

                    // altijd eerst project naam tonen
                    strTooltipAanvraag.AddText(project.Naam + " (Project " + project.ProjectNr.ToString() + ")");

                    // toon alle vakmannen op het project

                    //List<CompositeMandag> test = (from m in listMandagenProjectTotal
                    //                              group m by new { m.Vakman.ContactIdOrigineel, m.Status } into grp
                    //                              select new CompositeMandag() { Status = grp.Key.Status, ContactIdOrigineel = grp.Key.ContactIdOrigineel, tijdsduur = new TimeSpan(grp.Sum(m => m.Eindtijd.Ticks - m.Begintijd.Ticks)) }).ToList();

                    if (hasUren)
                    {
                        foreach (Mandagen mandag in listMandagenProjectTotal)
                        {

                            // bereken de som der uren 
                            //ALLEEN ALS HET EEN BEVESTIGD UUR IS
                            if (mandag.Status)
                            {
                                MDRpersoon persoon = ApplicationState.GetValue<List<MDRpersoon>>(ApplicationVariables.listMDRPersoons).FirstOrDefault(a => a.persoon_ID == mandag.Vakman.ContactIdOrigineel);
                                if (persoon != null)
                                {
                                    strTooltipAanvraag.AddText("(" + ToonTijd(new TimeSpan(mandag.Eindtijd.Ticks - mandag.Begintijd.Ticks)) + ") " + (persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam).ToStringTrimmed());
                                }
                            }
                        }
                    }
                }
                //if (isProjectOriented)
                //{

                //    // altijd eerst project naam tonen
                //    strTooltipAanvraag.AddText(project.Naam + " (Project " + project.ProjectNr.ToString() + ")");

                //    // toon alle vakmannen op het project

                //    List<CompositeMandag> test = (from m in listMandagenProjectTotal
                //            group m by new { m.Vakman.ContactIdOrigineel, m.Status } into grp
                //            select new CompositeMandag() { Status = grp.Key.Status, ContactIdOrigineel = grp.Key.ContactIdOrigineel, tijdsduur = new TimeSpan(grp.Sum(m => m.Eindtijd.Ticks - m.Begintijd.Ticks)) }).ToList();

                //    foreach (CompositeMandag mandag in test)
                //    {

                //        // bereken de som der uren 
                //        //ALLEEN ALS HET EEN BEVESTIGD UUR IS
                //        if (mandag.Status)
                //        {
                //            MDRpersoon persoon = dbrepOriginal.GetContact(mandag.ContactIdOrigineel, true);
                //            if (persoon != null)
                //            {
                //                strTooltipAanvraag.AddText("(" + ToonTijd(mandag.tijdsduur) + ") " + (persoon.voornaam + " " + persoon.tussenvoegsel + " " + persoon.achternaam).ToStringTrimmed());
                //            }
                //        }
                //    }
                //}
                // #2 STAP 2
                // toon alle projecten voor deze vakman
                // #2 STAP 2
                // 
                // altijd eerst vakman naam tonen
                //
                else
                {
                    if (hasUren)
                    {
                        strTooltipAanvraag.AddText(ToonNaam(objVakman), true);

                        TimeSpan tijd = new TimeSpan();

                        List<Mandagen> listMandagenVakmanTotalStatus = listMandagenVakmanTotal.Where(m => m.Status).ToList();
                        // toon alle projecten van deze vakman
                        for (int i = 0; i < listMandagenVakmanTotalStatus.Count; i++)
                        {
                            Mandagen mm = listMandagenVakmanTotalStatus[i];
                            bool isLast = i + 1 == listMandagenVakmanTotalStatus.Count;

                            bool isBold = project == new Project() || mm.ProjectId == project.ProjectId;

                            //if (!isLast && mm.ProjectId == listMandagenVakmanTotalStatus[i + 1].ProjectId)
                            //{
                            //    // cumulate tijd
                            //    tijd = tijd.Add(GetTijd(mm));
                            //}
                            //else
                            //{
                            // cumulate tijd
                            tijd = tijd.Add(GetTijd(mm));

                            MDRproject mdrProject = ApplicationState.GetValue<List<MDRproject>>(ApplicationVariables.listMDRProjecten).FirstOrDefault(a => a.project_NR == (int)mm.Project.ProjectNr);

                            strTooltipAanvraag.AddText("(" + ToonTijd(tijd) + ")" + " " + mdrProject.naam_project + " (Project " + mm.Project.ProjectNr.ToString() + ")", isBold);
                            // reset tijd
                            tijd = new TimeSpan();
                            //}
                        }
                    }
                }

                // dit hele gedoe alleen uitvoeren als er sprake is van een aanvraag, wat we hier al weten
                if (hasAanvraag)
                {
                    // overzicht projecten

                    if (isProjectOriented && isOverzicht)
                    {
                        // selecteer mandagen die of zelf een aanvraag zijn van dit project OF
                        // OF een overlap hebben met een aanvraag van een extern project
                        List<Mandagen> listAanvragenProjectOverzicht = listMandagenTotal.Where(m => !m.Status && ((m.ProjectId == project.ProjectId) || (m.ProjectId != project.ProjectId && HasOverlap(m, listMandagenProjectTotal.Where(mpt => mpt.Status && mpt.VakmanId == m.VakmanId).ToList())))).ToList();
                        strTooltipAanvraag.AddAanvragen(listMandagenTotal, listAanvragenProjectOverzicht);
                    }
                    else if (!isProjectOriented && isOverzicht)
                    {
                        // selecteer mandagen die of zelf een aanvraag zijn van deze vakman OF
                        // OF (geen OF)
                        List<Mandagen> listAanvragenVakmanOverzicht = listMandagenTotal.Where(m => !m.Status && (m.VakmanId == vakman.VakmanId)).ToList();
                        strTooltipAanvraag.AddAanvragen(listMandagenTotal, listAanvragenVakmanOverzicht);
                    }
                    else // bij alle andere gevallen is er spraken van een intersection tussen vakman en project
                    {
                        // deze toch alsnog ophalen indien het projectOriented is
                        if (isProjectOriented)
                        {
                            // haal alle mandagen op voor deze deze dag voor deze vakman
                            listMandagenVakmanTotal = listMandagenTotal.Where(m => m.VakmanId == vakman.VakmanId).OrderBy(m => m.Begintijd).ToList();
                        }

                        // selecteer mandagen die of zelf een aanvraag zijn van dit project icm deze vakman OF
                        // OF een overlap hebben met een aanvraag van een extern project icm deze vakman
                        List<Mandagen> listAanvragenProjectVakman = listMandagenTotal.Where(m => !m.Status && ((m.ProjectId == project.ProjectId && m.VakmanId == vakman.VakmanId) || (m.ProjectId != project.ProjectId && m.VakmanId == vakman.VakmanId && HasOverlap(m, listMandagenVakmanTotal.Where(mpt => mpt.Status).ToList())))).ToList();
                        strTooltipAanvraag.AddAanvragen(listMandagenTotal, listAanvragenProjectVakman);
                    }

                }

                //// #3 STAP 3
                //// toon alle aanvragen voor deze vakman
                //// #3 STAP 3
                //if (!isProjectOriented && isOverzicht)
                //{
                //    bool isAanvraagVakman;
                //    bool showOnceVakman = true;
                //    // get alle aanvragen voor deze vakman
                //    List<Mandagen> listAanvragenVakman = listMandagenVakmanTotal.Where(m => !m.Status).ToList();

                //    foreach (Mandagen mandag in listAanvragenVakman)
                //    {
                //        List<Mandagen> listOriginelen = listMandagenVakmanTotal.Where(m => HasOverlap(m, mandag) && m.Status).OrderByDescending(o => OrderByTest(project, o.ProjectId)).ThenBy(o => o.ProjectId).ToList();

                //        if (showOnceVakman)
                //        {
                //            showOnceVakman = false;
                //            strTooltipAanvraag.AddText("Aanvraag " + (objVakman.voornaam + " " + objVakman.tussenvoegsel + " " + objVakman.achternaam).ToStringTrimmed() + (mandag.Geannulleerd ? " AFGEWEZEN" : ""), true);

                //            // plus text is cumulatief
                //            MDRproject objProjectNaar = dbrepOriginal.GetProject((int)mandag.Project.ProjectNr, true);
                //            // GetAangevraagdeTijd(mandag.Project, mandag.Vakman, GetHeleDag(mandag.Begintijd)) WEGGEHAALD
                //            strTooltipAanvraag.AddText("(" + GetIngeplandeTijd(mandag.Project, mandag.Vakman, GetHeleDag(mandag.Begintijd)) + ") + (" + GetAangevraagdeTijd(mandag.Project, mandag.Vakman, GetHeleDag(mandag.Begintijd), listOriginelen.Select(m => m.ProjectId).ToList()) + ") " + objProjectNaar.naam_project + " (Project " + objProjectNaar.project_NR.ToString() + ")", true);
                //        }

                //        isAanvraagVakman = project == new Project() || mandag.ProjectId == project.ProjectId;

                //        // minus text is een opsomming van alle mandagen die beinvloed worden door de aanvraag
                //        foreach (Mandagen origineel in listOriginelen)
                //        {
                //            Project objProjectVan = origineel.Project;
                //            // GetIngeplandeTijd(objProjectVan, mandag.Vakman, GetHeleDag(mandag.Begintijd))
                //            strTooltipAanvraag.AddText("(" + ToonTijd(origineel) + ") \u2212 (" + ToonOverlap(mandag, origineel) + ") " + objProjectVan.Naam + " (Project " + objProjectVan.ProjectNr.ToString() + ")");
                //        }
                //    }
                //}


                //// #4 STAP 4
                //// toon alle aanvragen voor dit project
                //// #4 STAP 4
                //if (isProjectOriented && isOverzicht)
                //{
                //    bool isAanvraagProject;
                //    bool showOnceProject = true;
                //    // get alle aanvragen voor dit project
                //    List<Mandagen> listAanvragenProject = listMandagenProjectTotal.Where(m => !m.Status).ToList();


                //    foreach (Mandagen mandag in listAanvragenProject)
                //    {
                //        List<Mandagen> listOriginelen = listMandagenProjectTotal.Where(m => HasOverlap(m, mandag) && m.Status).OrderByDescending(o => OrderByTest(project, o.ProjectId)).ThenBy(o => o.ProjectId).ToList();

                //        if (showOnceProject)
                //        {
                //            // re-initialise objVakman
                //            objVakman = dbrepOriginal.GetContact(mandag.Vakman.ContactIdOrigineel);

                //            showOnceProject = false;
                //            strTooltipAanvraag.AddText("Aanvraag " + (objVakman.voornaam + " " + objVakman.tussenvoegsel + " " + objVakman.achternaam).ToStringTrimmed() + (mandag.Geannulleerd ? " AFGEWEZEN" : ""), true);

                //            // plus text is cumulatief
                //            MDRproject objProjectNaar = dbrepOriginal.GetProject((int)mandag.Project.ProjectNr, true);
                //            // GetAangevraagdeTijd(mandag.Project, mandag.Vakman, GetHeleDag(mandag.Begintijd)) WEGGEHAALD
                //            strTooltipAanvraag.AddText("(" + GetIngeplandeTijd(mandag.Project, mandag.Vakman, GetHeleDag(mandag.Begintijd)) + ") + (" + GetAangevraagdeTijd(mandag.Project, mandag.Vakman, GetHeleDag(mandag.Begintijd), listOriginelen.Select(m => m.ProjectId).ToList()) + ") " + objProjectNaar.naam_project + " (Project " + objProjectNaar.project_NR.ToString() + ")", true);
                //        }

                //        isAanvraagProject = project == new Project() || mandag.ProjectId == project.ProjectId;

                //        // minus text is een opsomming van alle mandagen die beinvloed worden door de aanvraag
                //        foreach (Mandagen origineel in listOriginelen)
                //        {
                //            Project objProjectVan = origineel.Project;
                //            // GetIngeplandeTijd(objProjectVan, mandag.Vakman, GetHeleDag(mandag.Begintijd))
                //            strTooltipAanvraag.AddText("(" + ToonTijd(origineel) + ") \u2212 (" + ToonOverlap(mandag, origineel) + ") " + objProjectVan.Naam + " (Project " + objProjectVan.ProjectNr.ToString() + ")");
                //        }
                //    }
                //}



                //// #5 STAP 5
                //// toon alle aanvragen voor dit project OF de vakman
                //// #5 STAP 5
                //if (!isOverzicht)
                //{
                //    bool isAanvraag;
                //    bool showOnce = true;
                //    // get alle aanvragen voor dit project
                //    List<Mandagen> listAanvragen = listMandagenTotal.Where(m => !m.Status).ToList();

                //    foreach (Mandagen mandag in listAanvragen)
                //    {
                //        List<Mandagen> listOriginelen = listMandagenTotal.Where(m => HasOverlap(m, mandag) && m.Status).OrderByDescending(o => OrderByTest(project, o.ProjectId)).ThenBy(o => o.ProjectId).ToList();

                //        if (showOnce)
                //        {
                //            showOnce = false;

                //            // re-initialise objVakman
                //            objVakman = dbrepOriginal.GetContact(mandag.Vakman.ContactIdOrigineel);

                //            strTooltipAanvraag.AddText("Aanvraag " + (objVakman.voornaam + " " + objVakman.tussenvoegsel + " " + objVakman.achternaam).ToStringTrimmed() + (mandag.Geannulleerd ? " AFGEWEZEN" : ""), true);

                //            // plus text is cumulatief
                //            MDRproject objProjectNaar = dbrepOriginal.GetProject((int)mandag.Project.ProjectNr, true);
                //            // GetAangevraagdeTijd(mandag.Project, mandag.Vakman, GetHeleDag(mandag.Begintijd)) WEGGEHAALD
                //            strTooltipAanvraag.AddText("(" + GetIngeplandeTijd(mandag.Project, mandag.Vakman, GetHeleDag(mandag.Begintijd)) + ") + (" + GetAangevraagdeTijd(mandag.Project, mandag.Vakman, GetHeleDag(mandag.Begintijd), listOriginelen.Select(m => m.ProjectId).ToList()) + ") " + objProjectNaar.naam_project + " (Project " + objProjectNaar.project_NR.ToString() + ")", true);
                //        }

                //        isAanvraag = project == new Project() || mandag.ProjectId == project.ProjectId;

                //        // minus text is een opsomming van alle mandagen die beinvloed worden door de aanvraag
                //        foreach (Mandagen origineel in listOriginelen)
                //        {
                //            Project objProjectVan = origineel.Project;
                //            // GetIngeplandeTijd(objProjectVan, mandag.Vakman, GetHeleDag(mandag.Begintijd))
                //            strTooltipAanvraag.AddText("(" + ToonTijd(origineel) + ") \u2212 (" + ToonOverlap(mandag, origineel) + ") " + objProjectVan.Naam + " (Project " + objProjectVan.ProjectNr.ToString() + ")");
                //        }
                //    }
                //}

                //sw.Stop();

                //strTooltipAanvraag.AddText(sw.ElapsedMilliseconds.ToString() + "ms elapsed");

                return strTooltipAanvraag;

            }
            catch (Exception ex)
            {


                MessageBox.Show(ex.Message);

                return null;

            }
        }






        public bool OrderByTest(Project project, int projectId)
        {
            return project == new Project() || project.ProjectId == projectId;
        }

        //public StackPanel AddTooltipAanvraag(List<Mandagen> listMandagenWaarDitDeAanvraagVanIs, Mandagen mandag, bool reverse, bool includeProjects)
        //{
        //    dbRepository dbrep = new dbRepository();
        //    dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
        //    MDRpersoon objVakman = dbrepOriginal.GetContact(mandag.Vakman.ContactIdOrigineel);

        //    List<Mandagen> listMandagenTotal = dbrep.GetMandagen(mandag.VakmanId, ApplicationState.GetValue<DateTime>(ApplicationVariables.dtSelectedDay)).Where(m => m.Begintijd != m.Eindtijd).OrderBy(m=>m.Begintijd).ToList();


        //    StackPanel strTooltipAanvraag = new StackPanel();

        //    if (includeProjects)
        //    {
        //        //List<Project> listProjectsBold = dbrep.GetProjectsByVakmanId(mandag.VakmanId, mandag.Begintijd, mandag.Eindtijd, true);

        //        //foreach (Project project in dbrep.GetProjectsByVakmanId(mandag.VakmanId, GetHeleDag(mandag.Begintijd), 1))
        //        //{
        //        //    bool isBold = listProjectsBold.Contains(project);

        //        //    strTooltipAanvraag.AddText("(0:tt" + ")" + " " + dbrepOriginal.GetProject((int)project.ProjectNr).naam_project, isBold);



        //        //}


        //        foreach (Mandagen mm in listMandagenTotal)
        //        {

        //            if (mm.Status)
        //            {
        //                List<Mandagen> mandagenToCompare = new List<Mandagen>();
        //                bool isBold = false;
        //                if (!reverse)
        //                {
        //                    mandagenToCompare.Add(mandag);
        //                }
        //                else
        //                {
        //                    mandagenToCompare = listMandagenWaarDitDeAanvraagVanIs.Where(m => m.Begintijd < mandag.Eindtijd && m.Eindtijd > mandag.Begintijd && m.VakmanId == mandag.VakmanId).ToList();
        //                }

        //                foreach (Mandagen mmm in mandagenToCompare)
        //                {
        //                    if(HasOverlap(mmm, mm))
        //                    {
        //                        isBold = true;
        //                    }
        //                }

        //                strTooltipAanvraag.AddText("(" + ToonTijd(mm) + ")" + " " + dbrepOriginal.GetProject((int)mm.Project.ProjectNr).naam_project + " (Project " + mm.Project.ProjectNr.ToString() + ")", isBold);
                        
                        
        //                //if (!isBold)
        //                //{
        //                //    Mandagen test = listMandagenWaarDitDeAanvraagVanIs.FirstOrDefault(m => m.Begintijd <= mandag.Eindtijd && m.Eindtijd >= mandag.Begintijd && m.VakmanId == mandag.VakmanId);
        //                //    isBold = (mm.ProjectId == test.ProjectId && reverse);
        //                //}
        //            }
        //        }
        //    }

        //    StackPanel txtPlus = new StackPanel();
        //    StackPanel txtMinus = new StackPanel();

        //    if (listMandagenWaarDitDeAanvraagVanIs.Count > 0)
        //    {
        //        strTooltipAanvraag.AddText("Aanvraag " + (objVakman.voornaam + " " + objVakman.tussenvoegsel + " " + objVakman.achternaam).ToStringTrimmed() + (mandag.Geannulleerd ? " AFGEWEZEN" : ""), true);


        //        //Mandagen test = listMandagenWaarDitDeAanvraagVanIs.FirstOrDefault(m => m.Begintijd <= mandag.Eindtijd && m.Eindtijd >= mandag.Begintijd && m.VakmanId == mandag.VakmanId);
                
        //        listMandagenWaarDitDeAanvraagVanIs = listMandagenWaarDitDeAanvraagVanIs.Where(m => m.Begintijd < mandag.Eindtijd && m.Eindtijd > mandag.Begintijd && m.VakmanId == mandag.VakmanId).ToList();
                
        //        //MessageBox.Show("Count:" + listMandagenWaarDitDeAanvraagVanIs.Count.ToString() + test.Begintijd.ToString() + " <= " + mandag.Eindtijd.ToString() + " en " + test.Eindtijd.ToString() + " >= " + mandag.Begintijd.ToString());
                
        //        // plus text is cumulatief
        //        MDRproject objProjectNaar = dbrepOriginal.GetProject((int)mandag.Project.ProjectNr, true);
        //        // GetAangevraagdeTijd(mandag.Project, mandag.Vakman, GetHeleDag(mandag.Begintijd)) WEGGEHAALD
        //        txtPlus.AddText("(" + GetIngeplandeTijd(mandag.Project, mandag.Vakman, GetHeleDag(mandag.Begintijd)) + ") + (" + GetAangevraagdeTijd(mandag.Project, mandag.Vakman, GetHeleDag(mandag.Begintijd), listMandagenWaarDitDeAanvraagVanIs.Select(m => m.ProjectId).ToList()) + ") " + objProjectNaar.naam_project + " (Project " + objProjectNaar.project_NR.ToString() + ")", true);


        //        // minus text is een opsomming van alle mandagen die beinvloed worden door de aanvraag
        //        foreach (Mandagen origineel in listMandagenWaarDitDeAanvraagVanIs)
        //        {
        //            Project objProjectVan = origineel.Project;
        //            // GetIngeplandeTijd(objProjectVan, mandag.Vakman, GetHeleDag(mandag.Begintijd))
        //            txtMinus.AddText("(" + ToonTijd(origineel) + ") \u2212 (" + ToonOverlap(mandag, origineel) + ") " + objProjectVan.Naam + " (Project " + objProjectVan.ProjectNr.ToString() + ")");
        //        }

        //        // plaats de tooltip in de gewenste volgorde
        //        if (reverse)
        //        {
        //            strTooltipAanvraag.Children.Add(txtPlus);
        //            strTooltipAanvraag.Children.Add(txtMinus);
        //        }
        //        else
        //        {
        //            strTooltipAanvraag.Children.Add(txtMinus);
        //            strTooltipAanvraag.Children.Add(txtPlus);
        //        }

        //    }

        //    return strTooltipAanvraag;
        //}

        //public StackPanel AddTooltipAanvraagMinOnly(List<Mandagen> listMandagenWaarDitDeAanvraagVanIs, Mandagen mandag, bool reverse)
        //{
        //    dbOriginalRepository dbrepOriginal = new dbOriginalRepository();
        //    MDRpersoon objVakman = dbrepOriginal.GetContact(mandag.Vakman.ContactIdOrigineel);

        //    StackPanel strTooltipAanvraag = new StackPanel();
        //    string txtPlus = string.Empty;
        //    string txtMinus = string.Empty;

        //    if (listMandagenWaarDitDeAanvraagVanIs.Count > 0)
        //    {
        //        //strTooltipAanvraag += Environment.NewLine;
        //        //strTooltipAanvraag += "Aanvraag " + (objVakman.voornaam + " " + objVakman.tussenvoegsel + " " + objVakman.achternaam).ToStringTrimmed();

        //        //if (mandag.Geannulleerd)
        //        //{
        //        //    strTooltipAanvraag += " AFGEWEZEN";
        //        //}

        //        listMandagenWaarDitDeAanvraagVanIs = listMandagenWaarDitDeAanvraagVanIs.Where(m => m.Begintijd < mandag.Eindtijd && m.Eindtijd > mandag.Begintijd && m.VakmanId == mandag.VakmanId).ToList();

        //        // plus text is cumulatief
        //        MDRproject objProjectNaar = dbrepOriginal.GetProject((int)mandag.Project.ProjectNr, true);
        //        //txtPlus += Environment.NewLine;
        //        //txtPlus += GetIngeplandeTijd(mandag.Project, mandag.Vakman, GetHeleDag(mandag.Begintijd)) + " + (" + GetAangevraagdeTijd(mandag.Project, mandag.Vakman, GetHeleDag(mandag.Begintijd)) + ") " + objProjectNaar.naam_project + " (Project " + objProjectNaar.project_NR.ToString() + ")";

        //        // minus text is een opsomming van alle mandagen die beinvloed worden door de aanvraag
        //        foreach (Mandagen origineel in listMandagenWaarDitDeAanvraagVanIs)
        //        {
        //            Project objProjectVan = origineel.Project;
        //            // GetIngeplandeTijd(objProjectVan, mandag.Vakman, GetHeleDag(mandag.Begintijd))
        //            txtMinus += "(" + ToonTijd(origineel) + ") \u2212 (" + ToonOverlap(mandag, origineel) + ") " + objProjectVan.Naam + " (Project " + objProjectVan.ProjectNr.ToString() + ")";
        //        }

        //        // plaats de tooltip in de gewenste volgorde
        //        if (reverse)
        //        {
        //            strTooltipAanvraag.AddText(txtPlus);
        //            strTooltipAanvraag.AddText(txtMinus);
        //        }
        //        else
        //        {
        //            strTooltipAanvraag.AddText(txtMinus);
        //            strTooltipAanvraag.AddText(txtPlus);
        //        }

        //    }

        //    return strTooltipAanvraag;
        //}


        public static string ToonTijd(Mandagen mandag)
        {
            return mandag.Uren.ToString() + ":" + (mandag.Minuten < 10 ? "0" : "") + mandag.Minuten.ToString();
        }

        public static TimeSpan GetTijd(Mandagen mandag)
        {
            return new TimeSpan(mandag.Uren, mandag.Minuten, 0);
        }

        public static long GetMax(long value1, long value2)
        {
            if(value1 > value2)
            {
                return value1;
            }
            else
            {
                return value2;
            }
        }

        public static long GetMin(long value1, long value2)
        {
            if(value1 > value2)
            {
                return value2;
            }
            else
            {
                return value1;
            }
        }

        public static string ToonOverlap(Mandagen mandag1, Mandagen mandag2)
        {
            TimeSpan tsOverlap = GetOverlap(mandag1, mandag2); // new TimeSpan(GetMin(mandag1.Eindtijd.Ticks, mandag2.Eindtijd.Ticks) - GetMax(mandag1.Begintijd.Ticks, mandag2.Begintijd.Ticks));

            string tijd = tsOverlap.Hours.ToString();
            tijd += ":";
            tijd += tsOverlap.Minutes > 9 ? tsOverlap.Minutes.ToString() : "0" + tsOverlap.Minutes.ToString();

            return tijd;
        }

        public static TimeSpan GetOverlap(Mandagen mandag1, Mandagen mandag2)
        {
            TimeSpan tsOverlap = new TimeSpan(GetMin(mandag1.Eindtijd.Ticks, mandag2.Eindtijd.Ticks) - GetMax(mandag1.Begintijd.Ticks, mandag2.Begintijd.Ticks));

            return tsOverlap;
        }

        public static long GetOverlapTicks(Mandagen mandag1, Mandagen mandag2)
        {
            TimeSpan tsOverlap = new TimeSpan(GetMin(mandag1.Eindtijd.Ticks, mandag2.Eindtijd.Ticks) - GetMax(mandag1.Begintijd.Ticks, mandag2.Begintijd.Ticks));

            return tsOverlap.Ticks;
        }

        public static TimeSpan GetOverlap(List<Mandagen> listMandagen, Mandagen mandag, Project projectTOV)
        {
            TimeSpan tsOverlap = new TimeSpan();
            foreach (Mandagen m in listMandagen)
            {
                if (HasOverlap(m, mandag) && m.ProjectId == projectTOV.ProjectId)
                {
                    tsOverlap = tsOverlap.Add(GetOverlap(m, mandag));
                }

            }

            return tsOverlap;
        }


        public static bool HasOverlap(Mandagen mandag1, Mandagen mandag2)
        {
            bool hasOverlap = false;

            if (mandag1.Begintijd < mandag2.Eindtijd && mandag1.Eindtijd > mandag2.Begintijd)
            {
                hasOverlap = true;
            }

            return hasOverlap;
        }

        public static bool HasOverlap(Mandagen mandag1, List<Mandagen> mandagenlist)
        {
            bool hasOverlap = false;

            foreach (Mandagen mandag2 in mandagenlist)
            {
                if (HasOverlap(mandag1, mandag2))
                {
                    hasOverlap = true;
                    break;
                }
            }

            return hasOverlap;
        }


        public string ToonIngeplandeTijd(Project project, Vakman vakman, DateTime datum, List<Mandagen> listMandagenTotal)
        {
            try
            {
                TimeSpan tsIngeplandeTijd = GetIngeplandeTijd(project, vakman, datum, listMandagenTotal);

                string tijd = tsIngeplandeTijd.Hours.ToString();
                tijd += ":";
                tijd += tsIngeplandeTijd.Minutes > 9 ? tsIngeplandeTijd.Minutes.ToString() : "0" + tsIngeplandeTijd.Minutes.ToString();

                return tijd;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }

        public TimeSpan GetIngeplandeTijd(Project project, Vakman vakman, DateTime datum, List<Mandagen> listMandagenTotal)
        {
            try
            {
                List<Mandagen> listIngeplandeMandagen = listMandagenTotal.Where(m => m.ProjectId == project.ProjectId && m.VakmanId == vakman.VakmanId && m.Begintijd >= datum && m.Eindtijd < datum.AddDays(1) && m.Status).ToList();

                TimeSpan tsIngeplandeTijd = new TimeSpan(listIngeplandeMandagen.Sum(m => m.Eindtijd.Ticks - m.Begintijd.Ticks));

                return tsIngeplandeTijd;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new TimeSpan();
            }
        }

        //public string GetIngeplandeTijd(Project project, Vakman vakman, DateTime datum, Project projectTOV)
        //{

        //    dbRepository dbrep = new dbRepository();
        //    List<Mandagen> listIngeplandeMandagen = dbrep.datacontext.Mandagens.Where(m => m.ProjectId == project.ProjectId && m.VakmanId == vakman.VakmanId && m.Begintijd >= datum && m.Eindtijd < datum.AddDays(1) && m.Status).ToList();
        //    List<Mandagen> listMandagenTotal = dbrep.GetMandagen(vakman.VakmanId, datum).Where(m => m.Begintijd != m.Eindtijd).OrderBy(m => m.Begintijd).ToList();

        //    TimeSpan tsIngeplandeTijdTOV = new TimeSpan();

        //    foreach (Mandagen mandag in listIngeplandeMandagen)
        //    {
        //        tsIngeplandeTijdTOV = tsIngeplandeTijdTOV.Add(GetOverlap(listMandagenTotal, mandag, projectTOV));


        //    }

        //    string tijd = tsIngeplandeTijdTOV.Hours.ToString();
        //    tijd += ":";
        //    tijd += tsIngeplandeTijdTOV.Minutes > 9 ? tsIngeplandeTijdTOV.Minutes.ToString() : "0" + tsIngeplandeTijdTOV.Minutes.ToString();

        //    return tijd;
        //}

        //public string GetAangevraagdeTijd(Project project, Vakman vakman, DateTime datum)
        //{
        //    try
        //    {

        //        dbRepository dbrep = new dbRepository();
        //        List<Mandagen> listAangevraagdeMandagen = dbrep.datacontext.Mandagens.Where(m => m.ProjectId == project.ProjectId && m.VakmanId == vakman.VakmanId && m.Begintijd >= datum && m.Eindtijd < datum.AddDays(1) && !m.Status).ToList();
        //        List<Mandagen> listIngeplandeMandagen = dbrep.GetMandagen(vakman.VakmanId, datum).Where(m => m.Status).ToList();

        //        TimeSpan tsOverlappendeTijd = new TimeSpan();

        //        foreach (Mandagen mandag in listAangevraagdeMandagen)
        //        {
        //            foreach (Mandagen mandag2 in listIngeplandeMandagen)
        //            {
        //                if (HasOverlap(mandag, mandag2))
        //                {
        //                    tsOverlappendeTijd = tsOverlappendeTijd.Add(GetOverlap(mandag, mandag2));
        //                }
        //            }
        //        }

        //        string tijd = tsOverlappendeTijd.Hours.ToString();
        //        tijd += ":";
        //        tijd += tsOverlappendeTijd.Minutes > 9 ? tsOverlappendeTijd.Minutes.ToString() : "0" + tsOverlappendeTijd.Minutes.ToString();

        //        return tijd;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        return string.Empty;
        //    }

        //}

        public string ToonAangevraagdeTijd(Project project, Vakman vakman, DateTime datum, List<int> projectIdsTOV, List<Mandagen> listMandagenTotal)
        {

            try
            {
                TimeSpan tsOverlappendeTijd = GetAangevraagdeTijd(project, vakman, datum, projectIdsTOV, listMandagenTotal);
                return ToonTijd(tsOverlappendeTijd);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return string.Empty;
            }

        }

        public TimeSpan GetAangevraagdeTijd(Project project, Vakman vakman, DateTime datum, List<int> projectIdsTOV, List<Mandagen> listMandagenTotal)
        {

            try
            {
                List<Mandagen> listAangevraagdeMandagen = listMandagenTotal.Where(m => m.ProjectId == project.ProjectId && m.VakmanId == vakman.VakmanId && m.Begintijd >= datum && m.Eindtijd < datum.AddDays(1) && !m.Status).ToList();
                List<Mandagen> listIngeplandeMandagen = listMandagenTotal.Where(m => m.VakmanId == vakman.VakmanId && m.Begintijd >= datum && m.Begintijd < datum.AddDays(1) && m.Status).ToList();
                //dbrep.GetMandagen(vakman.VakmanId, datum).Where(m => m.Status).ToList();

                TimeSpan tsOverlappendeTijd = new TimeSpan();

                foreach (Mandagen mandag in listAangevraagdeMandagen)
                {
                    foreach (Mandagen mandag2 in listIngeplandeMandagen)
                    {
                        //if (HasOverlap(mandag, mandag2) && projectIdsTOV.Contains(mandag2.ProjectId))
                        if (HasOverlap(mandag, mandag2))
                        {
                            tsOverlappendeTijd = tsOverlappendeTijd.Add(GetOverlap(mandag, mandag2));
                        }
                    }
                }


                return tsOverlappendeTijd;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new TimeSpan();
            }

        }


        public static string ToonTijd(TimeSpan ts)
        {
            string tijd = ts.Hours.ToString();
            tijd += ":";
            tijd += ts.Minutes > 9 ? ts.Minutes.ToString() : "0" + ts.Minutes.ToString();

            return tijd;
        }

        public DateTime GetHeleDag(DateTime dag)
        {
            return new DateTime(dag.Year, dag.Month, dag.Day);
        }

        public string DisplayTijd(int hours, int minutes)
        {
            return hours.ToString() + ":" + (minutes > 9 ? minutes.ToString() : "0" + minutes.ToString());
        }



    }
}

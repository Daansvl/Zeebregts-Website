using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using EvoPdf;
using System.Configuration;

namespace MDR2PDF
{
    public partial class _1b_WeeklijstExternen : UserControl
    {
        static string xHtmlFolder = ConfigurationManager.AppSettings["htmlFolder"].ToString();
        public EvoTools ET = new EvoTools(xHtmlFolder);

        public int Aantal_Regels_per_Pagina = -1; // Pagina Totaalregel uit indien '-1'
        public int[] PaginaTotalen = new int[8];
        public int BTotaal = 0;
        public int GTotaal = 0;


        // EVO HTML Variabelen
        //static string pathcontainer = System.IO.Path.Combine(xHtmlFolder, "html/Lijst1b/Lijst1b.html");
        static string pathcontainer = System.IO.Path.Combine(xHtmlFolder, "html/Lijst1/Lijst1_bc.html");
        static string pathfooter = System.IO.Path.Combine(xHtmlFolder, "html/Lijst1/footer_1bc.html");
        //static string pathhandtekening = System.IO.Path.Combine(xHtmlFolder, "html/Lijst1/handtekening.png").Replace("\\","/");
        static string pathhandtekening = "http://www.essed.com/40df86d0-a86d-11e4-bcd8-0800200c9a66/PL000000/handtekening0.png";

        public string htmlcontainer = File.ReadAllText(pathcontainer); // Public ipv static zodat het opnieuw wordt ingelezen
        public string htmlfooter = File.ReadAllText(pathfooter); // Public ipv static zodat het opnieuw wordt ingelezen

        // Werkelijke waarde pas toekennen in initialize(), kan hier niet
        public string HTMLTotaal    ;
        public string TotaalRegel   ;
        public string TotaalRegelPlusLijntje;
        public string DetailRegel   ;
        public string VakmanRegel   ;
        public string NietGewerkRegel;
        public string TabelProject  ;
        public string TabelVakman   ;
        public int Detailregels = 0;
        public int VakmanNr = 0;
        public int DetailregelsLeft = 0; // Aantal regels dat er nog afgedrukt zullen worden
        public bool TussenLijntjeNodig = false;

        // Zodat we een melding kunnen geven als gegevens van een vakman ontbreken
        string VakmannenWithNoBedrijf = string.Empty;
        string VakmannenWithNoBsn = string.Empty;
        string VakmannenWithNoGeboortedatum = string.Empty;
        string VakmannenWithNoID = string.Empty;
        string VakmannenWithVerlopenID = string.Empty;
        string ProjectenWithVakmanNulls = string.Empty;


        public _1b_WeeklijstExternen()
        {
            InitializeComponent();

            HTMLTotaal = VerwijderLabel(htmlcontainer, "VakmanTabel");
            HTMLTotaal = VerwijderLabel(HTMLTotaal, "Projecttabel");
            TabelProject = VerwijderLabel(GetLabel(htmlcontainer, "Projecttabel"), "Tabelregels");
            TabelVakman = GetLabel(htmlcontainer, "Vakmantabel");
            TotaalRegel = GetLabel(htmlcontainer, "Totaalregel");
            TotaalRegelPlusLijntje = GetLabel(htmlcontainer, "TREGELBOVENLIJN");
            DetailRegel = GetLabel(htmlcontainer, "Detailregel");
            VakmanRegel = GetLabel(htmlcontainer, "VakmanRegel");
            NietGewerkRegel = GetLabel(htmlcontainer, "nietgewerktregel");
        }

        private static string VerwijderLabel(string htmlcontainer, string Label)
        {
            int P1 = htmlcontainer.ToLower().IndexOf("<!-- "+ Label.ToLower());
            int P2 = htmlcontainer.ToLower().IndexOf("<!-- /"+ Label.ToLower());
            if (P1<0 || P2 < 0)
            {
                MessageBox.Show("Label '" + Label + "' niet gevonden");
                return "";
            }
            string s1 = htmlcontainer.Substring(0, P1);
            string S2 = htmlcontainer.Substring(P2);
            return s1 + S2;
        }

        private static string GetLabel(string htmlcontainer, string Label)
        {
            int P1 = htmlcontainer.ToLower().IndexOf("<!-- " + Label.ToLower());
            int P2 = htmlcontainer.ToLower().IndexOf("<!-- /" + Label.ToLower());
            if (P1<0 || P2 < 0)
            {
                MessageBox.Show("Label '" + Label + "' niet gevonden");
                return "";
            }
            P1 += Label.Length + "<!--__-->".Length;
            return htmlcontainer.Substring(P1, P2 - P1);
        }

        public string records = string.Empty;
        public string recordsVakman = string.Empty;

        public int RegelsOpDezePagina = 0;

        public List<DateTime> WeekStart = new List<DateTime>();
        public List<DateTime> WeekEinde = new List<DateTime>();

        /// <summary>
        /// Deel het project in in perioden van weken waarbij aaneengesloten lege weken bij elkaar samengevoegd worden.
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="Maandag"></param>
        /// <param name="selxMANDAGEN"></param>
        /// <param name="pEindDatum"></param>
        internal void Bepaal_Projectweken(int ProjectId, DateTime Maandag, List<PoortViewMandagen> selxMANDAGEN, DateTime pEindDatum )
        {
            WeekStart.Clear();
            WeekEinde.Clear();
            do
            {
                WeekStart.Add(Maandag);
                DateTime NextMaandag = Maandag.AddDays(7);
                while (selxMANDAGEN.Where(x => 
                                        x.Begintijd >= Maandag && x.Begintijd < NextMaandag.AddDays(7) &&
                                        x.ProjectId == ProjectId).Count() == 0 && 
                                        NextMaandag <= pEindDatum)
                    NextMaandag = NextMaandag.AddDays(7);
                WeekEinde.Add(NextMaandag);
                Maandag = NextMaandag;
            } while (Maandag <= pEindDatum);// pAllMaxDatum);// pEindDatum);
        }

        internal void Maak_Lijst(DateTime pStartDatum, DateTime pEindDatum, List<PoortViewMandagen> selxMANDAGEN, List<allArbeidsRelaty> _AllArbeidsrelaties, List<PoortViewRegistratiebevoegd> _AllProjectleiders, List<PoortViewProject> _AllProjecten, List<PoortViewBedrijf> _AllBedrijven, List<PoortViewVakman> _AllVakmannen, EvoTools.HeaderFooter LijstGegevens, bool _PlusInfo)
        {
            string tabellen = string.Empty;
            string tabellenPlus = string.Empty;
            // Bepaal start-maandag
            // Let op: Pak de HELE periode, niet slechts de keuze in de UI (Met showall wordt de periode automatisch uitgebreid!)
            DateTime pAllMinDatum = selxMANDAGEN.Select(x => x.Begintijd.Date).Min(); if (pStartDatum < pAllMinDatum) pAllMinDatum = pStartDatum;
            DateTime pAllMaxDatum = selxMANDAGEN.Select(x => x.Begintijd.Date).Max(); if (pEindDatum > pAllMaxDatum) pAllMaxDatum = pEindDatum;

            // Bepaal de maandag van de startweek
            //DateTime Maandag = pAllMinDatum;
            // We willen gewoon alle weken zien, ook de weken voorafgaande aan de eerste week met echte data dus pak pStartdatum
            DateTime Maandag = pAllMinDatum;
            while (Maandag.DayOfWeek != DayOfWeek.Monday)
                Maandag = Maandag.AddDays(-1);

            /*
             * Verzamel nu per project, per week de nodige data
             * */

            // Per Project
            string tabel = string.Empty;
            string tabelPlus = string.Empty;
            records = string.Empty;
            var ProjectIDs = selxMANDAGEN
                .OrderBy(x => x.project_NR)
                .Select(x => x.project_NR)
                .Distinct().ToList();
            bool FirstPage = true;
            foreach (var DitProject in _AllProjecten.Where(y => ProjectIDs.Contains(y.ProjectNR)))
            try
            {
                // Bepaal weken voor dit project
                Bepaal_Projectweken(DitProject.ProjectId, Maandag, selxMANDAGEN, pAllMaxDatum);

                // Bereken nu alvast totaal aantal detailregels voor dit project
                DetailregelsLeft = selxMANDAGEN.Where(x =>
                    x.ProjectId == DitProject.ProjectId)
                    .Select(x => new { VK = x.VakmanId, Week = ET.weeknummerNEW(x.Begintijd,false)}).Distinct().ToList().Count();

                // Algemene Projectgegevens (Nieuwe tabel)
                tabel = TabelProject;
                tabelPlus = TabelVakman; VakmanNr = 0;
                if (FirstPage)
                {
                    tabel = tabel.Replace("TableOnNewPage", "TableOnSamePage");
                    FirstPage = false;
                }
                tabel = tabel.Replace("{Rapportnaam}", LijstGegevens.LijstNaam);
                tabel = tabel.Replace("{Bestandscode}", LijstGegevens.LijstCode);
                string WeekS = ET.weeknummerNEW(pAllMinDatum,true);
                string WeekE = ET.weeknummerNEW(pAllMaxDatum, true);
                tabel = tabel.Replace("{Week}", WeekS + (WeekS == WeekE ? "" : " tot en met " + WeekE));
                tabel = tabel.Replace("{Periode}", string.Format("{0} - {1}",pAllMinDatum.ToLongDateString(), pAllMaxDatum.ToLongDateString()));
                tabel = tabel.Replace("{Projectnummer}",DitProject.ProjectNR.ToString());
                string __Opdrachtgever = (from og in _AllBedrijven
                                            where og.bedrijf_nr == DitProject.BedrijfVW
                                            select og.naam).FirstOrDefault() ?? "{Onbekend}";
                // Opdrachtger / Aannemer (aannemer = 'Select AannwmerVW, anders gelijk aan opdrachtgever)
                tabel = tabel.Replace("{Opdrachtgever}", __Opdrachtgever);
                if (DitProject.AannemerVW == DitProject.BedrijfVW || DitProject.AannemerVW == null)
                    tabel = tabel.Replace("{Aannemer}", __Opdrachtgever);
                else
                    tabel = tabel.Replace("{Aannemer}", (from og in _AllBedrijven
                                                         where og.bedrijf_nr == DitProject.AannemerVW
                                                         select og.naam).FirstOrDefault() ?? "{Onbekend}");
                tabel = tabel.Replace("{Onderaannemer}", "Tegelzettersbedrijf J.H.J Zeebregts BV"); // Voorlopig gewoon altijd 'Zeebregts', tzt nemen we dit in de database op en wordt et variabel

                tabel = tabel.Replace("{Projectnaam}", DitProject.Naam);
                tabel = tabel.Replace("{Projectlocatie}", DitProject.plaats);
                tabel = tabel.Replace("{Projectstraat}", DitProject.bouw_straat);

               // Voeg handtekening toe (Als ie niet bestaat vervang 'm dan door standaard vakman 0-handtekening
                string pathhandtekeningPL = ("0000000000" + DitProject.ProjectleiderId.ToString());
                pathhandtekeningPL = pathhandtekeningPL.Substring(pathhandtekeningPL.Length - 6);
                pathhandtekeningPL = pathhandtekening.Replace("000000", pathhandtekeningPL).Replace("handtekening0", "handtekening" + DitProject.ProjectleiderId.ToString());

                // Het checken van het handtekeningenbestand blijft steeds hangen en  levert niks op.
                // Laat maar zitten dus die check, ik plaats wel voor alle mogelijk vakmannen standaard een leeg handtekeningenbestand

                //System.Net.HttpWebRequest request = null;
                //System.Net.HttpWebResponse response = null;
                //request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(pathhandtekeningPL);
                //request.Timeout = 30000;
                //try
                //{
                //    response = (System.Net.HttpWebResponse)request.GetResponse();
                tabel = tabel.Replace("{handtekening}", pathhandtekeningPL);
                //}
                //catch
                //{
                //    tabel = tabel.Replace("{handtekening}", pathhandtekening);
                //}

                var ss = _AllProjectleiders.Where(x => x.ProjectleiderId == DitProject.ProjectleiderId).Select(x => x.Gebruikersnaam).FirstOrDefault();
                tabel = tabel.Replace("{ondertekenaar}", ss);
                tabel = tabel.Replace("{datum}", DateTime.Now.Date.ToString("d-M-yyyy"));

                var UitvoerdersIDsOpDitPorject = selxMANDAGEN.Where(x =>
                    x.project_NR == DitProject.ProjectNR)
                    .Select(x => x.ProjectleiderId).Distinct().ToList();
                var UitvoerdersOpDitPorject = selxMANDAGEN.Where(x =>
                    x.ProjectId == DitProject.ProjectNR)
                    .Select(x => x.ProjectleiderId).Distinct().ToList();
                string Uitvoerders = string.Empty;
                foreach (var uitv in _AllProjectleiders.Where(x => UitvoerdersIDsOpDitPorject.Contains(x.ProjectleiderId ?? -1)))
                {
                    Uitvoerders += (Uitvoerders == string.Empty ? "" : " / ");
                    Uitvoerders += uitv.Gebruikersnaam;
                }
                //tabel = tabel.Replace("{Projectleider}", Uitvoerders);
                var Opdrachtgever = _AllBedrijven.Where(x => x.bedrijf_nr == DitProject.BedrijfVW).FirstOrDefault();
                tabel = tabel.Replace("{Opdrachtgever}", Opdrachtgever.naam); ;


                // ProjectTotaal hele periode (en gedetailleerd per vakman)
                records = string.Empty;
                recordsVakman = string.Empty;

                if (_PlusInfo)
                {
                    BTotaal = 0;
                    GTotaal = 0;
                    foreach (var DezeVakman in selxMANDAGEN.Where(x =>
                        x.ProjectId == DitProject.ProjectId)
                        .Select(x => new
                        // Let op: 
                        // Neem Arbeidsrelatie mee in de distinct zodat vakman als aparte vakman wordt gezien 
                        // indien wijziging van arbeidsrelatie gedurende deze periode
                        {
                            x.VakmanId,
                            _Vakman = _AllVakmannen.Where(y => y.VakmanId == x.VakmanId).FirstOrDefault(),
                            _Relatie = x.ArbeidsrelatieVW ,
                            _RelatieNaam = (x.KetenpartnerVW == 1 ? _AllArbeidsrelaties.Where(z => z.ArbeidsRelatieID == x.ArbeidsrelatieVW).Select(z => z.ArbeidsRelatieNaam).FirstOrDefault() : "Extern")
                        }).Distinct().ToList()
                        .OrderBy(x => x._Vakman.Naam))
                    {
                        int VakmanFouten = VakmannenWithNoBedrijf.Length + VakmannenWithNoBsn.Length + VakmannenWithNoGeboortedatum.Length + VakmannenWithNoID.Length + VakmannenWithVerlopenID.Length;
                        recordsVakman += Vul_VakmanRegel(DezeVakman._RelatieNaam, DezeVakman._Vakman, DezeVakman._Relatie, _AllBedrijven, selxMANDAGEN.Where(x => x.ProjectId == DitProject.ProjectId && x.VakmanId == DezeVakman.VakmanId ).ToList());
                        // Als een gegeven van een vakman niet bekend was is die bij vul_vakmanRegel() toegevoegd aan "VakmannenWithNo..., 
                        // dus dan is de totale lengte van dat alles groter dan daarvoor
                        if (VakmanFouten < VakmannenWithNoBedrijf.Length + VakmannenWithNoBsn.Length + VakmannenWithNoGeboortedatum.Length + VakmannenWithNoID.Length + VakmannenWithVerlopenID.Length
                            && !ProjectenWithVakmanNulls.Contains(DitProject.Naam))
                            ProjectenWithVakmanNulls += (ProjectenWithVakmanNulls.Length > 1 ? ", " : "") + DitProject.Naam + " (" + DitProject.ProjectNR + ")" ;
                    }
                    tabel = VerwijderLabel(tabel, "vakmanregel");
                    tabel = tabel.Replace("<!-- /VAKMANREGEL -->", recordsVakman);
                    tabel = tabel.Replace("{T}", Uurformaat(BTotaal, true));
                    tabel = tabel.Replace("{G}", Uurformaat(GTotaal, true));
                } else
                {
                    tabel = VerwijderLabel(tabel, "VakmanTabel");
                    tabel = VerwijderLabel(tabel, "Verklaring");
                }


                // NIEUW voer totaalperiode-overzicht alleen uit als de periode meer dan twee weken beslaat. 
                // Anders heeft totaaltelling zo weing nut!
                
                DateTime mindag = selxMANDAGEN.Where(x => x.ProjectId == DitProject.ProjectId).Select(x => x.Begintijd).Min();
                DateTime maxdag = selxMANDAGEN.Where(x => x.ProjectId == DitProject.ProjectId).Select(x => x.Begintijd).Max();
                if (ET.weeknummerNEW(mindag, true) != ET.weeknummerNEW(maxdag, true))
                {
                    records += vul_TotaalRegel(selxMANDAGEN, DitProject);
                    // En nu uitgesplitst per vakman
                    foreach (var DezeVakman in selxMANDAGEN.Where(x =>
                        x.ProjectId == DitProject.ProjectId)
                        .Select(x => new
                        {
                            x.VakmanId,
                            _Vakman = _AllVakmannen.Where(y => y.VakmanId == x.VakmanId).FirstOrDefault(),
                        }).Distinct().ToList()
                        .OrderBy(x => x._Vakman.Naam))
                    {
                        records += Vul_DetailRegel(selxMANDAGEN, DitProject, DezeVakman._Vakman, ref tabel );
                    }
                    tabel = tabel.Replace("{T}", Uurformaat(BTotaal + GTotaal));
                    //tabel = tabel.Replace("{B}", Uurformaat(BTotaal));
                    if (GTotaal == 0)
                        tabel = tabel.Replace("{G}", "0:00"); 
                    else
                        tabel = tabel.Replace("{G}", Uurformaat(GTotaal));
                }


                // ProjectTotaal per week (en detailregels uitgesplitst per Vakman)
                for (int i = 0; i < WeekStart.Count(); i++)
                {
                    DateTime _StartTijd = WeekStart[i]; DateTime _EindeTijd = WeekEinde[i];
                    records += Vul_TotaalRegel(selxMANDAGEN, DitProject, _StartTijd, _EindeTijd);

                    // En nu uitgesplitst per vakman
                    foreach (var DezeVakman in selxMANDAGEN.Where(x =>
                        x.Begintijd >= _StartTijd &&
                        x.Begintijd < _EindeTijd &&
                        x.ProjectId == DitProject.ProjectId)
                        .Select(x => new
                        {
                            x.VakmanId,
                            _Vakman = _AllVakmannen.Where(y => y.VakmanId == x.VakmanId).FirstOrDefault(),
                        }).Distinct().ToList()
                        .OrderBy(x => x._Vakman.Naam))
                    {
                        records += Vul_DetailRegel(selxMANDAGEN, DitProject, DezeVakman._Vakman, _StartTijd, _EindeTijd, ref tabel);
                    }
                } // Week
                tabel = tabel.Replace("<!-- /TABELREGELS -->", records);
                tabellen += tabel;
            } // Project
            catch (Exception e)
            {
                string foutmelding = string.Format("Fout geconstateerd! Programma was bezig met verwerken van project={0}. Foutmelding:{1}", DitProject.Naam, e);
                MessageBox.Show(foutmelding);
                return;
            }


            // Geef eerste een melding als er vakmangegevens ontbraken
            string melding = string.Empty;
            if (VakmannenWithNoBedrijf.Length > 1)
                melding += "\nBedrijfsnaam van " + VakmannenWithNoBedrijf + " niet bekend";
            if (VakmannenWithNoBsn.Length > 1)
                melding += "\nBsn nr van " + VakmannenWithNoBsn + " niet bekend";
            if (VakmannenWithNoGeboortedatum.Length > 1)
                melding += "\nGeboortedatum " + VakmannenWithNoGeboortedatum + " niet bekend";
            if (VakmannenWithNoID.Length > 1)
                melding += "\nPaspoort nr van " + VakmannenWithNoID + " niet bekend of niet volledig";
            if (VakmannenWithVerlopenID.Length > 1)
                melding += string.Format("\nGeldigheidsdatum identiteitsbewijs is verlopen van {0}", VakmannenWithVerlopenID);
            if (melding.Length > 1)
                MessageBox.Show("Sommige vakmangegevens zijn niet bekend: \n\n" + melding + "\n\n Zie project(en): " + ProjectenWithVakmanNulls);

             // Maak PDF aan volgens NIEUWE METHODE
            string test = HTMLTotaal.Replace("<!-- /PROJECTTABEL -->", tabellen);
            ET.Log(test);
            ET.MakePdf(test, LijstGegevens, htmlfooter, false, 0);

        }

        private string Vul_VakmanRegel(string _RelatieNaam, PoortViewVakman DezeVakman, int _ArbeidsRelatie, List<PoortViewBedrijf> Bedrijven, List<PoortViewMandagen> Mandagen)
        {
            var _ZijnBedrijf = Bedrijven.Where(x => x.bedrijf_nr == DezeVakman.bedrijf_nr).FirstOrDefault();
            int _ZijnMinuten = Mandagen.Where(x => x.ArbeidsrelatieVW == _ArbeidsRelatie).Select(x => x.Uren * 60 + x.Minuten ).Sum();
            BTotaal += _ZijnMinuten;

            // Sla naam vd vakman op als gegevens ontbreken
            if ((_ZijnBedrijf == null || _ZijnBedrijf.naam.Length < 1) && !VakmannenWithNoBedrijf.Contains(DezeVakman.Naam))
                VakmannenWithNoBedrijf += (VakmannenWithNoBedrijf.Length > 0 ? ", " : "") + DezeVakman.Naam;
            if ((DezeVakman.Bsn == null || DezeVakman.Bsn.Length < 1) && !VakmannenWithNoBsn.Contains(DezeVakman.Naam))
                VakmannenWithNoBsn += (VakmannenWithNoBsn.Length > 0 ? ", " : "") + DezeVakman.Naam;
            if (DezeVakman.geboortedatum == null && !VakmannenWithNoGeboortedatum.Contains(DezeVakman.Naam))
                VakmannenWithNoGeboortedatum += (VakmannenWithNoGeboortedatum.Length > 0 ? ", " : "") + DezeVakman.Naam;

            // Check if ID ongeldig of verlopen
            if (DezeVakman.SoortID == null || DezeVakman.IDstring == null || DezeVakman.GeldigTot == null || DezeVakman.IDstring.Length < 1)
            {
                if (!VakmannenWithNoID.Contains(DezeVakman.Naam))
                    VakmannenWithNoID += (VakmannenWithNoID.Length > 0 ? ", " : "") + DezeVakman.Naam;
                DezeVakman.IDstring = string.Empty;
            }
            else
            {
                if (DezeVakman.GeldigTot < DezeVakman.LastDate)
                {
                    if (!VakmannenWithVerlopenID.Contains(DezeVakman.Naam))
                        VakmannenWithVerlopenID += (VakmannenWithVerlopenID.Length > 0 ? ", " : "") + DezeVakman.Naam;
                    //DezeVakman.IDstring += string.Format("{0} \n(geldig tot {1:d MMMM yyy})",DezeVakman.IDstring,DezeVakman.GeldigTot);
                    DezeVakman.IDstring = string.Empty;
                }
            }
                
            string regel = VakmanRegel;
            regel = regel.Replace("{Nr}", (++VakmanNr).ToString());
            regel = regel.Replace("{Vakman}", DezeVakman.Naam);
            regel = regel.Replace("{Bsn}",  DezeVakman.BSNLandcode + " " + DezeVakman.Bsn ?? "");
            regel = regel.Replace("{GebDatum}", string.Format("{0:dd-MM-yyyy}",DezeVakman.geboortedatum ));
            if (DezeVakman.IDstring.Length < 2)
                regel = regel.Replace("{IDNummer}", "");
            else
                regel = regel.Replace("{IDNummer}", string.Format("{0} {1} {2}", DezeVakman.SoortID ?? "", DezeVakman.IDstring == null ? "" : "nr:", DezeVakman.IDstring ?? ""));
            regel = regel.Replace("{BedrijfsAdres}", _ZijnBedrijf == null ? "" : _ZijnBedrijf.straat);// DezeVakman.Adres);
            string pc = _ZijnBedrijf == null || _ZijnBedrijf.postcode == null ? "" : _ZijnBedrijf.postcode.Trim().ToUpper();
            regel = regel.Replace("{BedrijfsAdresPostcode}", pc.Length == 6 ? pc.Substring(0, 4) + " " + pc.Substring(4, 2) : pc);
            regel = regel.Replace("{BedrijfsAdresPlaats}", _ZijnBedrijf == null ? "" : _ZijnBedrijf.plaats);
            regel = regel.Replace("{BedrijfsLand}", _ZijnBedrijf == null ? "" : _ZijnBedrijf.land);
            string bedrijfsnaam = _ZijnBedrijf == null || _ZijnBedrijf.naam == null ? "<Geen Bedrijf geregistreerd>" : _ZijnBedrijf.naam;
            regel = regel.Replace("{T}", Uurformaat(_ZijnMinuten,true ) );
            switch (_ArbeidsRelatie)
            {
                case 1: // Intern
                    regel = regel.Replace("{Dienstverband}", "In loondienst bij:");
                    regel = regel.Replace("{Bedrijf}", bedrijfsnaam );
                    regel = regel.Replace("{G}", Uurformaat( _ZijnMinuten,true ));
                    GTotaal += _ZijnMinuten;
                    break;
                case 2: //ZZP
                    regel = regel.Replace("{Dienstverband}", "ZZP'er");
                    regel = regel.Replace("{Bedrijf}", bedrijfsnaam );
                    regel = regel.Replace("{G}", "0:00");
                    break;
                //case 3: // Uitzendkracht
                //    regel = regel.Replace("{Dienstverband}", "Ingeleende uitzendkrecht of zo");
                //    regel = regel.Replace("{Bedrijf}", bedrijfsnaam);
                //    regel = regel.Replace("{G}", Uurformaat( _ZijnMinuten,true ));
                //    GTotaal += _ZijnMinuten;
                //    break;
                default:
                    regel = regel.Replace("{Dienstverband}", "Arbeidsrelatie onbekend");
                    regel = regel.Replace("{Bedrijf}", bedrijfsnaam);
                    regel = regel.Replace("{G}", "??:??");
                    break;
            }
            return regel;
        }


        /// <summary>
        /// Vul detailregel (Alle 'maandagen' bij elkaar opgeteld voor de hele periode)
        /// </summary>
        /// <param name="selxMANDAGEN"></param>
        /// <param name="DitProject"></param>
        /// <param name="_ArbeidsRelatieVW"></param>
        /// <param name="poortViewVakman"></param>
        /// <param name="_AllArbeidsrelaties"></param>
        private string Vul_DetailRegel(List<PoortViewMandagen> selxMANDAGEN, PoortViewProject DitProject, PoortViewVakman poortViewVakman, ref string tabel)
        {
            // Om de 3 regels de kleur van de kleur van de detailregel omwisselen
            string regel = DetailRegel.Replace("Detailregel", "DetailregelColor" + ((Detailregels++ % 6) / 3).ToString());

            //regel = regel.Replace("{Relatie}", _Ketenpartner);
            regel = regel.Replace("{BSN}", poortViewVakman.BSNLandcode + " " + poortViewVakman.Bsn);
            regel = regel.Replace("{Naam}", poortViewVakman.Naam);
            DateTime EenWillekeurigeMaandag = new DateTime(2013, 10, 21).Date;

            int TOTAAL = 0;
            for (int Weekdag = 0; Weekdag <= 6; Weekdag++)
            {
                var DagNR = EenWillekeurigeMaandag.AddDays(Weekdag).DayOfWeek;
                int totaal = selxMANDAGEN
                    .Where(x =>
                        x.ProjectId == DitProject.ProjectId &&
                        x.VakmanId == poortViewVakman.VakmanId &&
                        x.Begintijd.DayOfWeek == DagNR)
                    .Select(x => x.Uren * 60 + x.Minuten).Sum();
                TOTAAL += totaal;
                regel = regel.Replace("{" + DagNR.ToString() + "}",Uurformaat(totaal));
            }
            return (TOTAAL > 0 ? regel.Replace("{Totaal}",Uurformaat(TOTAAL)) : "");
        }


        /// <summary>
        /// Vul Detailregel (Losse dagen binnen één week)
        /// </summary>
        /// <param name="selxMANDAGEN"></param>
        /// <param name="DitProject"></param>
        /// <param name="_ArbeidsRelatieVW"></param>
        /// <param name="poortViewVakman"></param>
        /// <param name="_AllArbeidsrelaties"></param>
        /// <param name="pStartDatum"></param>
        /// <param name="pEindDatum"></param>
        private string Vul_DetailRegel(List<PoortViewMandagen> selxMANDAGEN, PoortViewProject DitProject,PoortViewVakman poortViewVakman, DateTime pStartDatum,DateTime pEindDatum, ref string tabel)
        {
            // Om de 3 regels de kleur van de kleur van de detailregel omwisselen
            string regel = DetailRegel.Replace("Detailregel", "DetailregelColor" + ((Detailregels++ % 6) / 3).ToString());
            if (DetailregelsLeft-- == 3) // Neem nu extra grote rowspan om vorige rowspans waar deze rij in zit soort van te 'overrulen'
                regel = regel.Replace("</tr>", "<td rowspan=\"100\" style=\"page-break-inside:avoid\" > </td></tr>");

            //regel = regel.Replace("{Relatie}", _Ketenpartner );
            regel = regel.Replace("{BSN}", poortViewVakman.BSNLandcode + " " + poortViewVakman.Bsn);
            regel = regel.Replace("{Naam}", poortViewVakman.Naam);

            int TOTAAL = 0;
            for (DateTime dag = pStartDatum ; dag < pEindDatum; dag = dag.AddDays(1))
            {
                int totaal = selxMANDAGEN
                    .Where(x =>
                        x.ProjectId == DitProject.ProjectId &&
                        x.VakmanId == poortViewVakman.VakmanId &&
                        x.Begintijd.Date == dag.Date)
                    .Select(x => x.Uren * 60 + x.Minuten).Sum();
                TOTAAL += totaal;
                regel = regel.Replace("{" + dag.DayOfWeek + "}", Uurformaat(totaal));
            }
            return TOTAAL > 0 ? (regel.Replace("{Totaal}", Uurformaat(TOTAAL))) : "";
        }


        /// <summary>
        /// Vul Totaalregel (Alle 'maandagen' bij elkaar opgeteld voor de hele periode)
        /// </summary>
        /// <param name="selxMANDAGEN"></param>
        /// <param name="DitProject"></param>
        private string vul_TotaalRegel(List<PoortViewMandagen> selxMANDAGEN, PoortViewProject DitProject)
        {
            string regel = TotaalRegel.Replace("{Periode}","Totaal");
            int TOTAAL = 0;
            Detailregels = 0;

            // Indien je begint met een totaalgebeuren, dan wil je dat daarna de eerste week-totaalregel begint met een tussenlijntje
            // om zo iets duidelijker onderscheid te maken tussen het totaaloverzicht en de weektotalen
            TussenLijntjeNodig = true;

            DateTime EenWillekeurigeMaandag = new DateTime(2013, 10, 21).Date;

            for (int Weekdag = 0; Weekdag <= 6; Weekdag++)
            {
                var DagNR = EenWillekeurigeMaandag.AddDays(Weekdag).DayOfWeek;
                int totaal = selxMANDAGEN
                    .Where(x => x.Begintijd.DayOfWeek == DagNR && x.ProjectId == DitProject.ProjectId)
                    .Select(x => x.Uren * 60 + x.Minuten).Sum();
                TOTAAL += totaal;
                regel = regel.Replace("{" + DagNR.ToString() + "}", Uurformaat(totaal));
            }

            if (TOTAAL > 0)
                return regel.Replace("{Totaal}",Uurformaat(TOTAAL));
            else
                return "";
        }

        /// <summary>
        /// Vul Totaalregel (alle afzonderlijke dagen binnen de gegeven periode van een week)
        /// </summary>
        /// <param name="selxMANDAGEN"></param>
        /// <param name="DitProject"></param>
        /// <param name="_StartTijd"></param>
        /// <param name="_EindeTijd"></param>
        private string Vul_TotaalRegel(List<PoortViewMandagen> selxMANDAGEN, PoortViewProject DitProject, DateTime _StartTijd, DateTime _EindeTijd)
        {

            //RegelsOpDezePagina = TotaalRegelPlusLijntje;

            string regel = TotaalRegel;
            if (TussenLijntjeNodig)
            {
                TussenLijntjeNodig = false;
                regel = TotaalRegelPlusLijntje;
            }
            if (_StartTijd == _EindeTijd.AddDays(-7))
                regel = regel.Replace("{Periode}", "Week "+ ET.weeknummerNEW(_StartTijd,true) + "");
            else
                if (_StartTijd.AddDays(14) == _EindeTijd)
                    regel = regel.Replace("{Periode}", "Week " + ET.weeknummerNEW(_StartTijd, true) + " en week " + ET.weeknummerNEW(_EindeTijd.AddDays(-7), true));
                else
                    regel = regel.Replace("{Periode}", "Week " + ET.weeknummerNEW(_StartTijd, true) + " t/m week " + ET.weeknummerNEW(_EindeTijd.AddDays(-7), true));

            int TOTAAL = 0;
            Detailregels = 0; 
            
            for (DateTime dag = _StartTijd; dag < _EindeTijd; dag = dag.AddDays(1))
            {
                var TEST = selxMANDAGEN
                    .Where(x => x.Begintijd.Date == dag.Date && x.ProjectId == DitProject.ProjectId)
                    .ToList();
                int totaal = selxMANDAGEN
                    .Where(x => x.Begintijd.Date  == dag.Date && x.ProjectId == DitProject.ProjectId)
                    .Select(x => x.Uren * 60 + x.Minuten).Sum();
                TOTAAL += totaal;
                regel = regel.Replace("{" + dag.DayOfWeek + "}", Uurformaat(totaal));
            }

            // Indien geen uren deze periode, voeg dan detailregel toe met tekst "Niet gewerkt'
            if (TOTAAL == 0)
                regel += NietGewerkRegel;
            return regel.Replace("{Totaal}", Uurformaat(TOTAAL));
        }

        private string Uurformaat(int totaal, bool Toon0)
        {
            int UrenT = totaal / 60;
            int MinutenT = totaal - (UrenT * 60);

            if (UrenT == 0 && MinutenT == 0)
                return Toon0 ? "00:00" : "";

            string min = MinutenT.ToString();
            return UrenT.ToString() + ":" + (min.Length == 1 ? "0" + min : min);
        }
        private string Uurformaat(int totaal)
        {
            return Uurformaat(totaal, false);
        }



    } //
} //

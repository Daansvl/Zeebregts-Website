using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDR2PDF
{
    public partial class LijstTools : UserControl
    {
        public class GedeeldeMandagen
        {
            public DateTime Datum;
            public int VakmanID;
            public int CodeDienstbetrekkine = new int();
            public int PLID = new int();
            public int ProjectID = new int();
        }

        public enum Datumkeuze
        {
            Week = 10,
            Jaar = 11,
            Dag = 21
        }
        public class FriendlyUISettings
        {
            public int DatumKeuze = (int) Datumkeuze.Week;
        }

        public LijstTools()
        {
            InitializeComponent();
        }

        internal List<GedeeldeMandagen> DubbeleProjecten_Per_Vakman(List<USMario.xMANDAG> SMandagen)
        {
            // NIEUW: lijst met vakmannen die op één dag gedeeld worden over verschillende projectleiders
            // mandagen per dag persoon per projectleider
            var _MandagenPerDagPerPersoonPerProjectleider = SMandagen
                .Select(x => new
                {
                    Datum = x.Mandag.Begintijd.Date,
                    VakmanID = x.Mandag.VakmanId,
                    CodeDienstbetrekking = x.Dienstbetrekking,
                    PLID = x.Mandag.ProjectleiderId
                        //NIEUW
                    ,
                    ProjectID = x.Mandag.ProjectId
                }
                    )
                .Distinct()
                .ToList();
            // Idem, maar nu group by { vakman, count(aantal verschillende projectleiders)"} 
            var _MandagenPerDag_PerVakman_AantalProjectleiders = _MandagenPerDagPerPersoonPerProjectleider.Select(x => x)
                .GroupBy(x => new { x.Datum, x.VakmanID, x.CodeDienstbetrekking })
                .Select(x => new { x.Key, Aantal = x.Count() })
                .ToList();
            // Pak alleen die mandagen die op één over meer dan 1 project lopen
            _MandagenPerDag_PerVakman_AantalProjectleiders = _MandagenPerDag_PerVakman_AantalProjectleiders.Where(x => x.Aantal > 1).ToList();

            // En dan als laatst: alle losse mandaggegevens waarvan je de key hierboven bepaalt hebt. is het nog enigszinds duidelijk?

            var temp = 
                (from x in _MandagenPerDagPerPersoonPerProjectleider
                 join y in _MandagenPerDag_PerVakman_AantalProjectleiders
                 on new { x.Datum, x.VakmanID } equals new { y.Key.Datum, y.Key.VakmanID }
                 select x).ToList();

            GedeeldeMandagen tempie;
            List<GedeeldeMandagen> _MeerDan1ProjectenPerVakmanPerDag = new List<GedeeldeMandagen>();
            for (int i = 0; i < temp.Count; i++)
            {
                tempie = new GedeeldeMandagen();
                tempie.Datum = temp[i].Datum;
                tempie.VakmanID = temp[i].VakmanID;
                tempie.CodeDienstbetrekkine = temp[i].CodeDienstbetrekking;
                tempie.PLID = temp[i].PLID;
                tempie.ProjectID = temp[i].ProjectID;
                _MeerDan1ProjectenPerVakmanPerDag.Add(tempie);
            }

            return _MeerDan1ProjectenPerVakmanPerDag;

        }


    }
}

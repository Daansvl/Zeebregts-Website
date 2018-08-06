using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using eBrochure_zeebregts.Web.Services;
using System.Linq;

namespace eBrochure_zeebregts.Classes
{
	public class ProjectFase
	{
		private string _fid;
		public string FID { get { return _fid; } }
		private string _omschrijving;
		public string Omschrijving { get { return _omschrijving; } }
		private double _afrondingboven;
		public double AfrondingBoven { get { return _afrondingboven; } }
		private double _opslag;
		public double Opslag { get { return _opslag; } }
		private double _indexering;
		public double Indexering { get { return _indexering; } }
		private double _btw;
		public double BTW { get { return _btw; } }
		private double _korting;
		public double Korting { get { return _korting; } }
        private bool _toonnegatieveprijzen;
        public bool ToonNegatievePrijzen
        { get { return _toonnegatieveprijzen; } }
        private bool _filterdorpels;
        public bool FilterDorpels
        { get { return _filterdorpels; } }
		public ProjectFase(string Bouwnr, eBrochureDomainContext ctx)
		{
    		var fase = (from f in ctx.Fases
						join bn in ctx.Bouwnummers on f.F_ID equals bn.F_NR
						where bn.B_ID == Bouwnr
						select f).FirstOrDefault();
			
			_fid = fase.F_ID;
			_omschrijving = fase.Omschrijving;
			_afrondingboven = (double)fase.AfrondingBoven;
			_opslag = (double)fase.Opslag;
			_indexering = (double)fase.Indexering;
			_btw = (double)fase.BTW;
            _toonnegatieveprijzen = (bool)fase.ToonNegatievePrijzen;
            _filterdorpels = (bool)fase.FilterDorpels;
            if (Acumulator.Instance().HuidigGebruiker.Rol != UserRole.Showroom)
            {
                _korting = (double)fase.Korting;
            }
			
		}
	}
}

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using eBrochure_zeebregts.Web.Services;
using eBrochure_zeebregts.Web;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace eBrochure_zeebregts.KeuzeControls
{
	public partial class NewUserControl : UserControl,INotifyPropertyChanged
	{
		eBrochureDomainContext ctx;


		public event PropertyChangedEventHandler PropertyChanged;
		private bool _kproject;
		private bool _kbouwnummer;
		private bool _kperiode;
        private bool _kcolbouwnummer;
		public bool KiesProject
		{ get { return _kproject; } set { _kproject = value; OnPropertyChanged("KiesProject"); } }
		public bool KiesBouwnummer
		{ get { return _kbouwnummer; } set { _kbouwnummer = value; OnPropertyChanged("KiesBouwnummer"); } }
		public bool KiesPeriode
		{ get { return _kperiode; } set { _kperiode = value; OnPropertyChanged("KiesPeriode"); } }
        public bool KiesColBouwnummer
        { get { return _kcolbouwnummer; } set { _kcolbouwnummer = value; OnPropertyChanged("KiesColBouwnummer"); } }
		private DetailsWindow ParentWindow;
		public NewUserControl(DetailsWindow parentwindow)
		{
			InitializeComponent();
			ctx = Acumulator.Instance().ctx;
			ParentWindow = parentwindow;
            ParentWindow.MaxHeight = 450;
			LoadRolCBB();
			LoadProjLB();
			LoadBouwNRCBB();
            LoadColBouwnrLB();
			
		}

		private void LoadRolCBB()
		{
			ctx.Load(ctx.GetGebruikersRollenQuery()).Completed += (sender0, args0) =>
			{
				var source = ctx.GebruikersRollens.ToList();
				RolCBB.ItemsSource = source;
				RolCBB.SelectedValuePath = "Rol_ID";
				RolCBB.DisplayMemberPath = "Rolnaam";
			};
		}
		private void LoadProjLB()
		{
			ctx.Load(ctx.GetPRojectQuery()).Completed += (sender1, args1) =>
			{
				var source = (from p in ctx.PRojects
							  select new ProjectListItem(this) { ID = p.PR_ID, ProjectNummer = (int)p.projectnummer, Omschrijving = p.Omschrijving }).ToList();

				ProjectLB.ItemsSource = source;
				

			};
		}
        private void LoadColBouwnrLB()
        {
            ctx.Load(ctx.GetBouwnummersQuery()).Completed += (sender, args) =>
            {
               /* var source = (from b in ctx.Bouwnummers
                              select new BouwnummerListItem() { B_ID = b.B_ID, ProjectNummer = b.PR_NR, Omschrijving = b.Omschrijving }).ToList();
                BouwNummerLB.ItemsSource = source;*/
            };
        }
		private void LoadBouwNRCBB()
		{
			ctx.Load(ctx.GetBouwnummersQuery()).Completed += (sender, args) =>
			{
				var source = ctx.Bouwnummers.ToList();
				Helpers.CustomComparer<Bouwnummers> comp = new Helpers.CustomComparer<Bouwnummers>();
				source.Sort(comp);
				BouwNrCBB.ItemsSource = source;
				BouwNrCBB.SelectedValuePath = "B_ID";
				BouwNrCBB.DisplayMemberPath = "Omschrijving";
			};
		}
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			PassBox.Text = GenereerPassWord(); 
			
		}

        private void SetDate(int dag, int maand, int jaar, Calendar cal)
		{
			DateTime dt = new DateTime(jaar, maand, dag);
			cal.SelectedDate = dt;
			cal.DisplayDate = dt;
		}

		private void NumericBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (((e.Key > Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Tab))
			{ 
				e.Handled = false; 
			}
			else
			{
				e.Handled = true;
			}
		}

		private string[] Lowers = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
		private string[] Upers = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
		private string[] Tekens = new string[] { "!", "@", "#", "$", "%", "&", "*", "(", ")", "{", "}", "[", "]", "<", ">", "?", "+", "-" };
		private string GenereerPassWord()
		{
			string pass = "";
			Random R1 = new Random(DateTime.Now.Millisecond);
			Random R2 = new Random(DateTime.Now.Millisecond + DateTime.Now.Second);
			bool[] variatie = new bool[4] { false, false, false, false };
			while (pass.Length < 8)
			{
				int sw = R1.Next(1, 5);
				int cnt = 0;
				foreach (bool b in variatie)
				{
					if (!b)
					{
						cnt++;
					}
				}
				if (cnt > 0 && 8 - pass.Length <= cnt)
				{
					if (!variatie[0])
					{
						sw = 1;
					}
					else if(!variatie[1])
					{
						sw = 2;
					}
					else if (!variatie[2])
					{
						sw = 3;
					}
					else if (!variatie[3])
					{
						sw = 4;
					}
				}
				switch (sw)
				{
					case 1://upper
						int u = R2.Next(0, 26);
						pass += Lowers[u].ToUpper();
						variatie[0] = true;
						break;
					case 2://lower
						int l = R2.Next(0, 26);
						pass += Lowers[l];
						variatie[1] = true;
						break;
					case 3://number
						pass += R2.Next(0, 10);
						variatie[2] = true;
						break;
					case 4://signs
						int s = R2.Next(0, 18);
						pass += Tekens[s];
						variatie[3] = true;
						break;
				}
			}
			return pass;
		}

		private void RolCBB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
           // ProjectLB.SelectedItems.Clear();
       //     BouwNummerLB.SelectedItems.Clear();
			switch ((int)RolCBB.SelectedValue)
			{
				case 1://admin
					KiesProject = false;
					KiesBouwnummer = false;
					KiesPeriode = false;
					break;
				case 2://advie
					KiesProject = true;
					KiesPeriode = true;
					KiesBouwnummer = false;
					break;
				case 3://demo
					KiesPeriode = true;
					KiesProject = true;
					KiesBouwnummer = false;
					break;
				case 4://bewoner
					KiesProject = true;
					KiesPeriode = true;
					KiesBouwnummer = true;
					break;
                case 5://showroom
                    KiesProject = true;
                    KiesColBouwnummer = true;
                    KiesPeriode = true;
                    break;
			}
		}
		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
		private bool ValidateInput()
		{
			//reset all error msgs
			ValGebruikersnaam.Visibility = Visibility.Collapsed;
			ValEmail.Visibility = Visibility.Collapsed;
			ValRol.Visibility = Visibility.Collapsed;
			ValBouwnummer.Visibility = Visibility.Collapsed;
			ValPeriode.Visibility = Visibility.Collapsed;
			ValProject.Visibility = Visibility.Collapsed;
			ValWachtwoord.Visibility = Visibility.Collapsed;
			bool retval = true;
			//validate gebruikersnaam
			if (TBGebruikersnaam.Text.Length > 1)
			{
				if (ctx.Gebruikers.Where(x => x.Naam == TBGebruikersnaam.Text).FirstOrDefault() != null)
				{
					ValGebruikersnaam.Text = "Naam bestaat al";
					ValGebruikersnaam.Visibility = Visibility.Visible;
					retval = false;
				}
			
			}
			else
			{
				ValGebruikersnaam.Text = "Vul naam in";
				ValGebruikersnaam.Visibility = Visibility.Visible;
				retval = false;
			}
			//validate email
            //if (TBEmail.Text.Length > 4)
            //{
            //    if (!Regex.IsMatch(TBEmail.Text, @"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$"))
            //    {
            //        retval = false;
            //        ValEmail.Text = "Ongeldige email";
            //        ValEmail.Visibility = Visibility.Visible;
            //    }
            //}
            //else
            //{
            //    ValEmail.Text = "Vul email in";
            //    ValEmail.Visibility = Visibility.Visible;
            //    retval = false;
            //}
			//validate rol
			if (RolCBB.SelectedIndex < 0)
			{
				ValRol.Text = "Selecteer rol";
				ValRol.Visibility = Visibility.Visible;
				retval = false;
			}
			//validate wachtwoord
			if(PassBox.Text.Length > 7)
			{
				var ltrz = PassBox.Text.ToCharArray();
				var checks = new bool[4] { false, false, false, false };
				for (int i = 0; i < PassBox.Text.Length; i++)
				{
					if (Lowers.ToList().Contains(ltrz[i].ToString()))
					{ checks[0] = true; }
					if (Upers.ToList().Contains(ltrz[i].ToString()))
					{ checks[1] = true; }
					if (Tekens.ToList().Contains(ltrz[i].ToString()))
					{ checks[2] = true; }
					int num;
					if (int.TryParse(ltrz[i].ToString(), out num))
					{ checks[3] = true; }

				}
				foreach (bool b in checks)
				{
					if (!b)
					{
						ValWachtwoord.Text= "Wachtwoord moet een hoofdletter, kleine letter, leesteken en cijfer bevatten";
						ValWachtwoord.Visibility = Visibility.Visible;
						retval = false;
					}
				}

			}
			else
			{
				ValWachtwoord.Text = "Wachtwoord te kort";
				ValWachtwoord.Visibility = Visibility.Visible;
				retval = false;
			}
			//validate project
			var projcount = 0;
			foreach (ProjectListItem pli in ProjectLB.Items)
			{
				if (pli.IsGekozen)
				{
					projcount++;
				}
			}
			if (KiesProject && projcount < 1)
			{
				ValProject.Text = "Kies minstens 1 project";
				ValProject.Visibility = Visibility.Visible;
				retval = false;
			}
            //validate bouwnummer verzameling
            if (KiesColBouwnummer && BouwNummerLB.Items.Where(i => (i as BouwnummerListItem).IsGekozen == true).Count() <= 0)
            {
                ValBouwnummer.Text = "Kies minstens 1 bouwnummer";
                ValBouwnummer.Visibility = Visibility.Visible;
                retval = false;
            }
			//validate bouwnummer
			if (KiesBouwnummer && BouwNrCBB.SelectedIndex < 0)
			{
				ValBouwnummer.Text = "Kies een Bouwnummer";
				ValBouwnummer.Visibility = Visibility.Visible;
				retval = false;
			}
			//validate periode
			if (KiesPeriode)
			{
                int comp_verleden = DateTime.Compare((DateTime.Now + Acumulator.Instance().serverTimeDiff).Subtract((DateTime.Now + Acumulator.Instance().serverTimeDiff).TimeOfDay)
                                                     ,ToDateCal.SelectedDate.Value);
				int comp_periode = DateTime.Compare(FromDateCal.SelectedDate.Value,
                                                    ToDateCal.SelectedDate.Value);
                if (comp_verleden < 0 && comp_periode < 0)
				{

				}
				else
				{
					ValPeriode.Text = "Kies een geldige periode";
					ValPeriode.Visibility = Visibility.Visible;
					retval = false;
				}
			}
			//punt bereik alles valid
			return retval;
		}
		private void SaveNewUser()
		{
			Gebruikers g = new Gebruikers();
            g.Rol_NR = RolCBB.SelectedValue != null ? int.Parse(RolCBB.SelectedValue.ToString()) : -1;
            if (g.Rol_NR == -1)
            {
                MessageBox.Show("vergeten rol te kiezen");
                return;
            }

			g.Naam = TBGebruikersnaam.Text.ToString();
			g.Wachtwoord =GetSHA256(PassBox.Text);
			g.Email = String.IsNullOrEmpty(TBEmail.Text) ? "" : TBEmail.Text;
			//projecten koppelen als nodig
			if (KiesProject)
			{
				ProjectenSets ps = new ProjectenSets();
				foreach (ProjectListItem x in ProjectLB.Items.Where(i=>(i as ProjectListItem).IsGekozen == true))
				{
					ProjectenSetOpbouw pso = new ProjectenSetOpbouw();
					pso.Project_NR = x.ID;
					ps.ProjectenSetOpbouw.Add(pso);
				}
				g.ProjectenSets = ps;
			}
            if (KiesColBouwnummer)
            {
                BouwnummerSets bs = new BouwnummerSets();
                foreach (BouwnummerListItem bli in BouwNummerLB.Items.Where(i => (i as BouwnummerListItem).IsGekozen == true))
                {
                    BouwnummerSetOpbouw bso = new BouwnummerSetOpbouw();
                    bso.Bouwnummer_NR = bli.B_ID;
                    bs.BouwnummerSetOpbouw.Add(bso);
                }
                g.BouwnummerSets = bs;
                
            }
			if (KiesBouwnummer)
			{
				BouwnummerSets bs = new BouwnummerSets();
				BouwnummerSetOpbouw bso = new BouwnummerSetOpbouw();
				bso.Bouwnummer_NR = BouwNrCBB.SelectedValue != null ? BouwNrCBB.SelectedValue.ToString() : "";
                if (bso.Bouwnummer_NR == "")
                {
                    MessageBox.Show("Vergeten bouwnummer te kiezen");
                    return;
                }
				bs.BouwnummerSetOpbouw.Add(bso);
				g.BouwnummerSets = bs;
			}
			if (KiesPeriode)
			{
                if (FromDateCal.SelectedDate.HasValue)
                {
                    g.GeldigVan = FromDateCal.SelectedDate;
                }
                else
                {
                    MessageBox.Show("Kies ingangs datum login");
                    return;
                }
                if (ToDateCal.SelectedDate.HasValue)
                {
                    g.GeldigTot = ToDateCal.SelectedDate;
                }
                else
                {
                    MessageBox.Show("Kies verval datum login");
                    return;
                }
			}
			ctx.Gebruikers.Add(g);
			ctx.SubmitChanges();
		}
		private static string GetSHA256(string text)
		{
			SHA256Managed sha256 = new SHA256Managed();
			byte[] sha256Bytes = System.Text.Encoding.UTF8.GetBytes(text);
			byte[] cryString = sha256.ComputeHash(sha256Bytes);
			string sha256Str = string.Empty;
			for (int i = 0; i < cryString.Length; i++)
			{
				sha256Str += cryString[i].ToString("X");
			}
			return sha256Str;



		}
		private void OpslaanClick(object sender, RoutedEventArgs e)
		{
			if (ValidateInput())
			{
				SaveNewUser();
				ParentWindow.Close();
				
			}
			else
			{

			}
		}

		private void FromDateCal_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
		{
			FromDateTB.Text ="Van: "+ FromDateCal.SelectedDate.Value.ToShortDateString();
		}

		private void ToDateCal_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
		{
			ToDateTB.Text = "Tot en met: "+ ToDateCal.SelectedDate.Value.ToShortDateString();
		}
        public void ProjectChanged()
        {
            if (KiesBouwnummer)
            {
                BouwNrCBB.ItemsSource = null;
                BouwNrCBB.UpdateLayout();
                var source = new List<Bouwnummers>();
                foreach (ProjectListItem item in ProjectLB.Items.Where(i => (i as ProjectListItem).IsGekozen == true))
                {
                    var bnrs = (from b in ctx.Bouwnummers
                                join p in ctx.PRojects on b.PR_NR equals p.PR_ID
                                where p.projectnummer == item.ProjectNummer
                                select b).ToList();
                    source.AddRange(bnrs);
                }
                Helpers.CustomComparer<Bouwnummers> comp = new Helpers.CustomComparer<Bouwnummers>();
                source.Sort(comp);
                BouwNrCBB.ItemsSource = source;
                BouwNrCBB.SelectedValuePath = "B_ID";
                BouwNrCBB.DisplayMemberPath = "Omschrijving";
                BouwNrCBB.UpdateLayout();
            }
            else if (KiesColBouwnummer)
            {
                BouwNummerLB.ItemsSource = null;
                BouwNummerLB.UpdateLayout();
                var source = new List<BouwnummerListItem>();
                foreach (ProjectListItem item in ProjectLB.Items)
                {
                    if (item.IsGekozen)
                    {
                        var bnrs = (from b in ctx.Bouwnummers
                                    join p in ctx.PRojects on b.PR_NR equals p.PR_ID
                                    where p.projectnummer == item.ProjectNummer
                                    select new BouwnummerListItem() { B_ID = b.B_ID, ProjectNummer = b.PR_NR, Omschrijving = b.Omschrijving }).ToList();
                        source.AddRange(bnrs);
                    }
                }
                BouwNummerLB.ItemsSource = source;
                BouwNummerLB.UpdateLayout();
            }
        }
        

	}
    public class BouwnummerListItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _bid;
        public string B_ID
        { get { return _bid; } set { _bid = value; OnPropertyChanged("B_ID"); } }
        private string _omschrijving;
        public string Omschrijving
        { get { return _omschrijving; } set { _omschrijving = value; OnPropertyChanged("Omschrijving"); } }
        private string _projnr;
        public string ProjectNummer
        { get { return _projnr; } set { _projnr = value; OnPropertyChanged("ProjectNummer"); } }
        private bool _gekozen;
        public bool IsGekozen
        { get { return _gekozen; } set { _gekozen = value; OnPropertyChanged("IsGekozen"); } }
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler!=null)
            {
                handler(this,new PropertyChangedEventArgs(name));
            }
        }
    }
    public delegate void Gekozendelegate();
	public class ProjectListItem :INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private int _projnr;
		public int ProjectNummer
		{ get { return _projnr; } set { _projnr = value; OnPropertyChanged("ProjectNummer"); } }
		private string _omsch;
		public string Omschrijving
		{ get { return _omsch; } set { _omsch = value; OnPropertyChanged("Omschrijving"); } }
		private string _id;
		public string ID
		{ get { return _id; } set { _id = value; OnPropertyChanged("ID"); } }
		private bool _gekozen;
        private NewUserControl NuC;
        public ProjectListItem(NewUserControl nuc)
        {
            NuC = nuc;
        }
        public bool IsGekozen
        { get { return _gekozen; } set { _gekozen = value; OnPropertyChanged("IsGekozen"); NuC.ProjectChanged(); } }

			protected void OnPropertyChanged(string name)
			{
				PropertyChangedEventHandler handler = PropertyChanged;
				if (handler != null)
				{
					handler(this, new PropertyChangedEventArgs(name));
				}
			}
	}
}

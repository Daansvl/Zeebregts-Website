using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using System.Security.Cryptography;
using eBrochure_zeebregts.Web.Services;
using eBrochure_zeebregts.Classes;

namespace eBrochure_zeebregts
{
	public partial class LoginWindow : ChildWindow
	{
		private AuthenticationService _service = WebContext.Current.Authentication;
		private AuthenticationOperation _op;
		public LoginWindow()
		{
			InitializeComponent();
        }
		private void OkBtn()
		{
            try
            {
                BusyIndie.BusyContent = "Identiteit controleren...";
                BusyIndie.IsBusy = true;

                //hash hier
                string pswd = GetSHA256(passwordBox.Password);
                //string pswd = passwordBox.Password;
                this._op = this._service.Login(new LoginParameters(NaamBox.Text, pswd, false, null));

                this._op.Completed += this.HandleCompletionEvent;
            }
            catch (Exception e)
            {
                MessageBox.Show("Ok Btn Catched: " + e.Message);
            }
		}
		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			OkBtn();
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
		private void HandleCompletionEvent(object sender, EventArgs e)
		{
            try
            {
                if (_op.HasError)
                {
                    MessageBox.Show(_op.Error.Message.ToString());
                    this.DialogResult = false;
                    LogHelper.SendLog("Login Failed: " + _op.Error.Message.ToString(), LogType.error);
                }
                else if (_op.User != null && _op.User.Identity.IsAuthenticated)
                {
                    eBrochureDomainContext ctx = new eBrochureDomainContext();
                    ctx.Load(ctx.GetGebruikersQuery()).Completed += (args, sender0) =>
                        {
                            var gebr = (from g in ctx.Gebruikers
                                        where g.Naam.ToLower() == NaamBox.Text.ToLower()
                                        select new LoggedInUser() { GebruikersNaam = g.Naam, Rol = (UserRole)g.Rol_NR, ID = g.GebruikersID }).FirstOrDefault();
                            Acumulator.Instance().HuidigGebruiker = gebr;
                            this.DialogResult = true;
                            LogHelper.SendLog("Login by User: " + gebr.GebruikersNaam + " - " + gebr.Rol.ToString(), LogType.activity);
                            LogHelper.SendLog("Site accessed from: " + Application.Current.Resources["ClientIP"] + " by user: " + gebr.GebruikersNaam, LogType.activity);
                        };

                }
                else
                {
                    LogHelper.SendLog("Login Failed", LogType.error);
                    this.DialogResult = false;
                }
                this._op = null;
                BusyIndie.IsBusy = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Handle Complete Catched: " + ex.Message);
            }
		}
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}
		private void ChildWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.DialogResult == true &&
			(this.NaamBox.Text == string.Empty || this.passwordBox.Password == string.Empty))
			{
				e.Cancel = true;
				ChildWindow cw = new ChildWindow();
				cw.Content = "Geef uw login naam en wachtwoord in, of druk op annuleer.";
				cw.Show();
			}
		}

		private void passwordBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				OkBtn();
			}
		}

	}
}


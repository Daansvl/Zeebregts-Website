namespace eBrochure_zeebregts.Web
{
	using System.ServiceModel.DomainServices.Hosting;
	using System.ServiceModel.DomainServices.Server;
	using System.ServiceModel.DomainServices.Server.ApplicationServices;
	using System.Security.Principal;
	using eBrochure_zeebregts.Web.Services;
	using System.Web.Security;
	using System.ServiceModel.DomainServices.EntityFramework;
	using System.Collections.Generic;
	using System;
	using System.Security;
    using System.ServiceModel;
    using System.Reflection;

	// TODO: Switch to a secure endpoint when deploying the application.
	//       The user's name and password should only be passed using https.
	//       To do this, set the RequiresSecureEndpoint property on EnableClientAccessAttribute to true.
	//   
	//       [EnableClientAccess(RequiresSecureEndpoint = true)]
	//
	//       More information on using https with a Domain Service can be found on MSDN.

	/// <summary>
	/// Domain Service responsible for authenticating users when they log on to the application.
	///
	/// Most of the functionality is already provided by the AuthenticationBase class.
	/// </summary>
    /// 
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, AddressFilterMode = AddressFilterMode.Any)]
	[EnableClientAccess]
	public class AuthenticationService : LinqToEntitiesDomainService<eBrochureDBEntities>, IAuthentication<User> 
	{
		private static User DefaultUser = new User()
		{
			Name = string.Empty,
			 FriendlyName = string.Empty,
			 Roles = new List<string>()
		};

        private eBrochureDomainService _service = new eBrochureDomainService();
		public User Login(string userName, string password, bool isPersistent, string customData)
		{
            try
            {
                if (this.ValidateUser(userName, password))
                {
                    FormsAuthentication.SetAuthCookie(userName.ToString(), isPersistent);
                    return this.GetUser(userName.ToString());
                }
                return null;
            }
            catch
            {
                throw new Exception("Login exception!!");
            }
		}
        protected override void OnError(DomainServiceErrorInfo errorInfo)
        {
            base.OnError(errorInfo);
            throw new ApplicationException(errorInfo.Error.Message);
        }
		private User GetUser(string userName)
		{
			User user = null;
			if (_service.DoesUserExist(userName))
			{
				Gebruikers gebr = _service.GetGebruiker(userName);
				user = new User();
				user.Name = gebr.Naam;

			}
			return user;
		}
		public User GetUser()
		{
			if (this.ServiceContext != null && this.ServiceContext.User != null && this.ServiceContext.User.Identity.IsAuthenticated)
			{
				return this.GetUser(this.ServiceContext.User.Identity.Name);
			}
			return AuthenticationService.DefaultUser;
		}
		private bool ValidateUser(string userName, string password)
		{
			return _service.ValidLoginCombo(userName,password);
		}
		private User GetAuthenticatedUser(IPrincipal principal)
		{
			User user = null;
			if (_service.DoesUserExist(principal.Identity.Name))
			{
				user = new User();
				Gebruikers gebr = _service.GetGebruiker(principal.Identity.Name);
				user.Name = gebr.Naam;
				
			}
			return user;
		}
		public User Logout()
		{
			FormsAuthentication.SignOut();
			return AuthenticationService.DefaultUser;
		}
		public void UpdateUser(User user)
		{
			if((this.ServiceContext.User == null)||
				(this.ServiceContext.User.Identity == null)||
				!string.Equals(this.ServiceContext.User.Identity.Name, user.Name, System.StringComparison.Ordinal))
			{
				throw new UnauthorizedAccessException("Je mag alleen je eigen profiel wijzigen");
			}
			//this.ObjectContext.Gebruikers.AttachAsModified<user,this.ChangeSet.GetOriginal(user));
		}
		public override void Initialize(DomainServiceContext context)
		{
			_service.Initialize(context);
			base.Initialize(context);
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_service.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}

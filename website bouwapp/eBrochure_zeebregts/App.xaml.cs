namespace eBrochure_zeebregts
{
	using System;
	using System.Runtime.Serialization;
	using System.ServiceModel.DomainServices.Client.ApplicationServices;
	using System.Windows;
	using System.Windows.Controls;
	using eBrochure_zeebregts.Classes;
	using System.Windows.Browser;
    using System.ServiceModel.DomainServices.Client;
    using System.Net;
    using System.Net.Browser;

	/// <summary>
	/// Main <see cref="Application"/> class.
	/// </summary>
    public partial class App : Application
	{
		/// <summary>
		/// Creates a new <see cref="App"/> instance.
		/// </summary>
		public App()
		{
			InitializeComponent();

			// Create a WebContext and add it to the ApplicationLifetimeObjects collection.
			// This will then be available as WebContext.Current.
			//WebContext webContext = new WebContext();
			
			//webContext.Authentication = new FormsAuthentication();
			//webContext.Authentication = new WindowsAuthentication();
			//this.ApplicationLifetimeObjects.Add(webContext);
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
            HttpWebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);
            HttpWebRequest.RegisterPrefix("https://", WebRequestCreator.ClientHttp);
			// This will enable you to bind controls in XAML to WebContext.Current properties.
			this.Resources.Add("WebContext", WebContext.Current);
			JavaScriptMethods JSM = new JavaScriptMethods();
            HtmlPage.RegisterScriptableObject("JSM", JSM);
						// This will automatically authenticate a user when using Windows authentication or when the user chose "Keep me signed in" on a previous login attempt.
			//WebContext.Current.Authentication.LoadUser(this.Application_UserLoaded, null);
            eBrochure_zeebregts.Web.Services.eBrochureDomainContext ctx = new Web.Services.eBrochureDomainContext();
            
            
          ((WebDomainClient<eBrochure_zeebregts.Web.Services.eBrochureDomainContext.IeBrochureDomainServiceContract>)ctx.DomainClient).ChannelFactory.Endpoint.Binding.OpenTimeout = new TimeSpan(0, 10, 0);
          Acumulator.Instance().ctx = ctx;
          this.Resources.Add("ClientIP",e.InitParams["ClientIP"]); 
       //   var ipaddress = e.InitParams["ClientIP"];
      //    MessageBox.Show(ipaddress + "::" + hostipaddres);
         // Acumulator.Instance().ClientIP = ipaddress;

            // Show some UI to the user while LoadUser is in progress
			this.InitializeRootVisual();
		}

		/// <summary>
		/// Invoked when the <see cref="LoadUserOperation"/> completes.
		/// Use this event handler to switch from the "loading UI" you created in <see cref="InitializeRootVisual"/> to the "application UI".
		/// </summary>
		private void Application_UserLoaded(LoadUserOperation operation)
		{
			if (operation.HasError)
			{
				ErrorWindow.CreateNew(operation.Error);
				operation.MarkErrorAsHandled();
			}
		}

		/// <summary>
		/// Initializes the <see cref="Application.RootVisual"/> property.
		/// The initial UI will be displayed before the LoadUser operation has completed.
		/// The LoadUser operation will cause user to be logged in automatically if using Windows authentication or if the user had selected the "Keep me signed in" option on a previous login.
		/// </summary>
		protected virtual void InitializeRootVisual()
		{
			eBrochure_zeebregts.Controls.BusyIndicator busyIndicator = new eBrochure_zeebregts.Controls.BusyIndicator();
			busyIndicator.Content = new StartPagina();
			busyIndicator.HorizontalContentAlignment = HorizontalAlignment.Stretch;
			busyIndicator.VerticalContentAlignment = VerticalAlignment.Stretch;

			this.RootVisual = busyIndicator;
		}

		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			// If the app is running outside of the debugger then report the exception using a ChildWindow control.
			if (!System.Diagnostics.Debugger.IsAttached)
			{
				// NOTE: This will allow the application to continue running after an exception has been thrown but not handled. 
				// For production applications this error handling should be replaced with something that will report the error to the website and stop the application.
				e.Handled = true;
				LogHelper.SendLog("Unhandled Exception: " +e.ExceptionObject.Message, LogType.error);
				ErrorWindow.CreateNew(e.ExceptionObject);
			}
		}

		private void Application_Exit(object sender, EventArgs e)
		{ 
		}
	}
}
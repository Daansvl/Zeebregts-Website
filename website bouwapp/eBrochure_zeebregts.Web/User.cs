using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.DomainServices.Server.ApplicationServices;

namespace eBrochure_zeebregts.Web
{
	public partial class User:UserBase
	{
		public string FriendlyName { get; set; }
	}
}
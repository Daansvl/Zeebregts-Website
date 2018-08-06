using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace eBrochure_zeebregts.Web.Services
{
    public static class DomainServiceExtensions
    {
        public static void ChangeTimeout(this eBrochureDomainContext domainContext,
                                         TimeSpan newTimeout)
        {
            // Try to get the channel factory property from the domain client 
            // of the domain context. In case that this property does not exist
            // we throw an invalid operation exception.
            var channelFactoryProperty = domainContext.DomainClient.GetType().GetProperty("ChannelFactory");
            if (channelFactoryProperty == null)
            {
                throw new InvalidOperationException("The 'ChannelFactory' property on the DomainClient does not exist.");
            }

            // Now get the channel factory from the domain client and set the
            // new timeout to the binding of the service endpoint.
            var factory = (ChannelFactory)channelFactoryProperty.GetValue(domainContext.DomainClient, null);
            factory.Endpoint.Binding.SendTimeout = newTimeout;
        }
    }
    public partial class eBrochureDomainContext
    {
        partial void OnCreated()
        {
            this.ChangeTimeout(new TimeSpan(1, 0, 0));
        }
    }
}
namespace eBrochure_zeebregts.Web
{
    public static class AuthServiceExtensions
    {
        public static void ChangeTimeout(this AuthenticationContext domainContext,
                                         TimeSpan newTimeout)
        {
            // Try to get the channel factory property from the domain client 
            // of the domain context. In case that this property does not exist
            // we throw an invalid operation exception.
            var channelFactoryProperty = domainContext.DomainClient.GetType().GetProperty("ChannelFactory");
            if (channelFactoryProperty == null)
            {
                throw new InvalidOperationException("The 'ChannelFactory' property on the DomainClient does not exist.");
            }

            // Now get the channel factory from the domain client and set the
            // new timeout to the binding of the service endpoint.
            var factory = (ChannelFactory)channelFactoryProperty.GetValue(domainContext.DomainClient, null);
            factory.Endpoint.Binding.SendTimeout = newTimeout;
        }
    }

    public partial class AuthenticationContext
    {
        partial void OnCreated()
        {
            this.ChangeTimeout(new TimeSpan(1, 0, 0));
        }
    }
}

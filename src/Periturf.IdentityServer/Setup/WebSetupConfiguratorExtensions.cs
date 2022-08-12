using Microsoft.AspNetCore.Http;
using Periturf.IdentityServer.Setup;
using Periturf.Web.Setup;

namespace Periturf
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebSetupConfiguratorExtensions
    {
        /// <summary>
        /// Setup IdentityServer Web Component.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="name">Component name (defaults to <c>IdentityServer</c>)</param>
        /// <param name="path">Web application base path (defaults to <c>/IdentityServer</c>)</param>
        public static void IdentityServer(this IWebSetupConfigurator configurator, string name = "IdentityServer", PathString? path = null)
        {
            configurator.AddWebComponentSpecification(new IdentityServerComponentSetupSpecification(name, path ?? "/IdentityServer"));
        }
    }
}

using Microsoft.AspNetCore.Http;
using Periturf.Configuration;
using Periturf.IdentityServer.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Periturf
{
    /// <summary>
    /// 
    /// </summary>
    public static class IdentityServerConfigurationExtensions
    {
        /// <summary>
        /// Configure an IdentityServer Component
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name">The name of the component</param>
        /// <param name="config"></param>
        public static void IdentityServer(this IConfigurationContext context, Action<IIdentityServerConfigurationConfigurator> config)
        {
            context.IdentityServer("IdentityServer", config);
        }

        /// <summary>
        /// Configure an IdentityServer Component
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name">The name of the component</param>
        /// <param name="config"></param>
        public static void IdentityServer(this IConfigurationContext context, string name, Action<IIdentityServerConfigurationConfigurator> config)
        {
            var spec = context.CreateComponentConfigSpecification<IdentityServerConfigurationSpecification>(name);
            config(spec);
            context.AddSpecification(spec);
        }
    }
}

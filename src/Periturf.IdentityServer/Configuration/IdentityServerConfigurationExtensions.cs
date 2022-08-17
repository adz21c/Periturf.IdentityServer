using Microsoft.AspNetCore.Http;
using Periturf.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.IdentityServer.Configuration
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
        public static void IdentityServer(this IConfigurationContext context, string name)
        {
            var spec = context.CreateComponentConfigSpecification<IdentityServerConfigurationSpecification>(name);
            context.AddSpecification(spec);
        }
    }
}

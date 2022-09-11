//
//   Copyright 2021 Adam Burton (adz21c@gmail.com)
//   
//   Licensed under the Apache License, Version 2.0 (the "License")
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//  
//

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

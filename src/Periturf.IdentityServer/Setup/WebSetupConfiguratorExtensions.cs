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
using Periturf.IdentityServer.Setup;
using Periturf.Web.Setup;
using System;

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
        /// <param name="config"></param>
        public static void IdentityServer(this IWebSetupConfigurator configurator, string name = "IdentityServer", PathString? path = null, Action<IIdentityServerSetupConfigurator>? config = null)
        {
            configurator.AddWebComponentSpecification(new IdentityServerComponentSetupSpecification(name, path ?? "/IdentityServer"));
        }
    }
}

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

using Duende.IdentityServer.Models;

namespace Periturf.IdentityServer.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public interface IIdentityServerConfigurationConfigurator
    {
        /// <summary>
        /// Add an <see cref="Duende.IdentityServer.Models.Client"/>
        /// </summary>
        /// <param name="client"></param>
        void Client(Client client);

        /// <summary>
        /// Add an <see cref="Duende.IdentityServer.Models.ApiResource"/>
        /// </summary>
        /// <param name="resource"></param>
        void ApiResource(ApiResource resource);

        /// <summary>
        /// Add an <see cref="Duende.IdentityServer.Models.IdentityResource"/>
        /// </summary>
        /// <param name="resource"></param>
        void IdentityResource(IdentityResource resource);

        /// <summary>
        /// Add an <see cref="Duende.IdentityServer.Models.ApiScope"/>
        /// </summary>
        /// <param name="scope"></param>
        void Scope(ApiScope scope);
    }
}
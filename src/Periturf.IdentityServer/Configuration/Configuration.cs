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
using System.Collections.Generic;

namespace Periturf.IdentityServer.Configuration
{
    class Configuration
    {
        public Configuration(List<Client>? clients, List<ApiResource>? apiResources, List<IdentityResource> identityResources, List<ApiScope> _apiScopes)
        {
            Clients = clients;
            ApiResources = apiResources;
            IdentityResources = identityResources;
            ApiScopes = _apiScopes;
        }

        public List<Client>? Clients { get; }

        public List<ApiResource>? ApiResources { get; }

        public List<IdentityResource>? IdentityResources { get; }
        
        public List<ApiScope>? ApiScopes { get; }
    }
}
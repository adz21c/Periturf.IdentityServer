﻿//
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
using Duende.IdentityServer.Stores;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Periturf.IdentityServer.Configuration
{
    [ExcludeFromCodeCoverage]
    class ClientStore : IClientStore
    {
        private readonly ConfigurationStore _store;

        public ClientStore(ConfigurationStore store)
        {
            _store = store;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            return _store.FindClientByIdAsync(clientId);
        }
    }
}
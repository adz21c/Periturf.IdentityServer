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

using Duende.IdentityServer.Configuration;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Periturf.IdentityServer.Configuration;
using Periturf.IdentityServer.Verification;
using Periturf.Web.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.IdentityServer.Setup
{
    class IdentityServerComponentSetupSpecification : IWebComponentSetupSpecification, IIdentityServerSetupConfigurator
    {
        private Action<IdentityServerOptions>? _options;

        public IdentityServerComponentSetupSpecification(string name, PathString path)
        {
            Name = name;
            Path = path;
        }

        public string Name { get; }

        public PathString Path { get; }

        public void Options(Action<IdentityServerOptions> options)
        {
            _options = options;
        }

        public ConfigureWebAppDto Configure()
        {
            // Ensure required config is overriden
            var options = (IdentityServerOptions o) =>
            {
                _options?.Invoke(o);
                o.Events.RaiseSuccessEvents = true;
                o.Events.RaiseFailureEvents = true;
                o.Events.RaiseInformationEvents = true;
                o.Events.RaiseErrorEvents = true;
            };

            var eventVerificationManager = new EventVerificationManager();
            var configurationStore = new ConfigurationStore();
            
            return new ConfigureWebAppDto(
                new IdentityServerComponent(configurationStore, Name, eventVerificationManager),
                app => app.UseIdentityServer(),
                services =>
                {
                    services.AddSingleton(eventVerificationManager);
                    services.AddSingleton(configurationStore);
                    services.AddIdentityServer(options)
                        .AddClientStore<ClientStore>()
                        .AddResourceStore<ResourceStore>();
                });
        }
    }
}

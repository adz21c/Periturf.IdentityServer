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

using Periturf.Clients;
using Periturf.Components;
using Periturf.Configuration;
using Periturf.Events;
using Periturf.IdentityServer.Configuration;
using Periturf.IdentityServer.Verification;
using Periturf.Verify;

namespace Periturf.IdentityServer
{
    class IdentityServerComponent : IComponent
    {
        private readonly string _componentName;
        private readonly ConfigurationStore _configStore;
        private readonly IEventVerificationManager _eventVerificationManager;

        public IdentityServerComponent(ConfigurationStore configStore, string componentName, IEventVerificationManager eventVerificationManager)
        {
            _configStore = configStore;
            _componentName = componentName;
            _eventVerificationManager = eventVerificationManager;
        }

        public IComponentClient CreateClient()
        {
            throw new System.NotImplementedException();
        }

        public IConditionBuilder CreateConditionBuilder()
        {
            return new IdentityServerConditionBuilder(_componentName, _eventVerificationManager);
        }

        public TSpecification CreateConfigurationSpecification<TSpecification>(IEventHandlerFactory eventHandlerFactory) where TSpecification : IConfigurationSpecification
        {
            return (TSpecification)(object)new IdentityServerConfigurationSpecification(_configStore);
        }
    }
}

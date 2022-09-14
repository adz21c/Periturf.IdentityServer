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

using Duende.IdentityServer.Events;
using Periturf.Values;
using Periturf.Verify;
using System;

namespace Periturf.IdentityServer.Verification
{
    class IdentityServerConditionBuilder : IIdentityServerConditionBuilder
    {
        private readonly string _componentName;
        private readonly IEventVerificationManager _eventVerificationManager;

        public IdentityServerConditionBuilder(string componentName, IEventVerificationManager eventVerificationManager)
        {
            _componentName = componentName;
            _eventVerificationManager = eventVerificationManager;
        }

        public IConditionSpecification OnApiAuthenticationSuccess(Func<IValueContext<ApiAuthenticationSuccessEvent>, IValueProviderSpecification<ApiAuthenticationSuccessEvent, bool>> config)
        {
            return new ApiAuthenticationSuccessConditionSpecification(_componentName, _eventVerificationManager, config(new ValueContext()));
        }

        class ValueContext : IValueContext<ApiAuthenticationSuccessEvent> { }
    }
}

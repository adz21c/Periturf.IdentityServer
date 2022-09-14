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

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Duende.IdentityServer.Events;
using Periturf.Values;
using Periturf.Verify;
using Periturf.Web;

namespace Periturf.IdentityServer.Verification
{
    class ApiAuthenticationSuccessConditionSpecification : IConditionSpecification
    {
        private readonly IEventVerificationManager _eventVerificationManager;
        private readonly IValueProviderSpecification<ApiAuthenticationSuccessEvent, bool> _evaluatorSpecification;

        public ApiAuthenticationSuccessConditionSpecification(
            string componentName,
            IEventVerificationManager eventVerificationManager,
            IValueProviderSpecification<ApiAuthenticationSuccessEvent, bool> evaluatorSpecification)
        {
            ComponentName = componentName;
            _eventVerificationManager = eventVerificationManager;
            _evaluatorSpecification = evaluatorSpecification;
        }

        public string ComponentName { get; }

        public string ConditionDescription => "";

        public Task<IConditionFeed> BuildAsync(IConditionInstanceFactory conditionInstanceFactory, CancellationToken ct)
        {
            var evaluator = _evaluatorSpecification.Build();
            var feed = _eventVerificationManager.CreateFeed(conditionInstanceFactory, evaluator);
            return Task.FromResult(feed);
        }
    }
}

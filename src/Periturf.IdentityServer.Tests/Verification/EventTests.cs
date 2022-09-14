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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Duende.IdentityServer.Events;
using FakeItEasy;
using Periturf.IdentityServer.Verification;
using Periturf.Values;
using Periturf.Verify;
using Periturf.Web.Verification;

namespace Periturf.IdentityServer.Tests.Verification
{
    class EventTests
    {
        [Test]
        public async Task Given_EvaluatorAndEventType_When_Build_Then_CreateFeed()
        {
            var feed = A.Fake<IConditionFeed>();
            var eventVerificationManager = A.Fake<IEventVerificationManager>();
            A.CallTo(() => eventVerificationManager.CreateFeed(A<IConditionInstanceFactory>._, A<Func<ApiAuthenticationSuccessEvent, ValueTask<bool>>>._)).Returns(feed);

            var conditionInstanceFactory = A.Fake<IConditionInstanceFactory>();

            var evaluator = A.Fake<Func<ApiAuthenticationSuccessEvent, ValueTask<bool>>>();
            var evaluatorSpec = A.Fake<IValueProviderSpecification<ApiAuthenticationSuccessEvent, bool>>();
            A.CallTo(() => evaluatorSpec.Build()).Returns(evaluator);

            const string componentName = "Component";

            var conditionBuilder = new IdentityServerConditionBuilder(componentName, eventVerificationManager);
            var spec = conditionBuilder.OnEvent<ApiAuthenticationSuccessEvent>(v => evaluatorSpec);

            var instanceFeed = await spec.BuildAsync(conditionInstanceFactory, CancellationToken.None);

            Assert.That(spec?.ComponentName, Is.Not.Null.And.EqualTo(componentName));
            Assert.That(instanceFeed, Is.Not.Null.And.SameAs(feed));
            A.CallTo(() => evaluatorSpec.Build()).MustHaveHappened().Then(
                A.CallTo(() => eventVerificationManager.CreateFeed(conditionInstanceFactory, evaluator)).MustHaveHappenedOnceExactly());
        }
    }
}

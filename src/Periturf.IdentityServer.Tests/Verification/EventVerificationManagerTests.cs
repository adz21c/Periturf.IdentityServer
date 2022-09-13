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
using Periturf.Verify;

namespace Periturf.IdentityServer.Tests.Verification
{
    class EventVerificationManagerTests
    {
        private Func<ApiAuthenticationSuccessEvent, ValueTask<bool>>? _evaluator;
        private IConditionInstanceFactory _conditionInstanceFactory;
        private EventVerificationManager _sut;
        private IConditionFeed _feed;

        [SetUp]
        public void SetUp()
        {
            _evaluator = A.Fake<Func<ApiAuthenticationSuccessEvent, ValueTask<bool>>>();
            A.CallTo(() => _evaluator.Invoke(A<ApiAuthenticationSuccessEvent>._)).Returns(true);

            _conditionInstanceFactory = A.Fake<IConditionInstanceFactory>();

            _sut = new EventVerificationManager();

            _feed = _sut.CreateFeed(_conditionInstanceFactory, _evaluator);
        }

        [TearDown]
        public async Task TearDown()
        {
            await _feed.DisposeAsync();
        }

        [Test]
        public void Given_Nothing_When_CreateFeed_Then_FeedCreated()
        {
            Assert.That(_feed, Is.Not.Null);
        }


        [Test]
        public async Task Given_Event_When_PersistWithEvaluateTrue_Then_EventReturnedByFeed()
        {
            Assume.That(_feed, Is.Not.Null);

            var conditionInstance = new ConditionInstance(TimeSpan.Zero, "ID");
            A.CallTo(() => _conditionInstanceFactory.Create(A<string>._)).Returns(conditionInstance);

            await _sut.PersistAsync(new ApiAuthenticationSuccessEvent("", ""));

            var instances = await _feed.WaitForInstancesAsync(CancellationToken.None);
            Assert.That(instances, Is.Not.Null.And.Count.EqualTo(1).And.Contain(conditionInstance));
            A.CallTo(() => _conditionInstanceFactory.Create(A<string>._)).MustHaveHappened();
        }

        [Test]
        public async Task Given_Event_When_PersistWithEvaluateFalse_Then_FeedReturnsNothing()
        {
            Assume.That(_feed, Is.Not.Null);

            A.CallTo(() => _evaluator.Invoke(A<ApiAuthenticationSuccessEvent>._)).Returns(false);

            var conditionInstance = new ConditionInstance(TimeSpan.Zero, "ID");
            A.CallTo(() => _conditionInstanceFactory.Create(A<string>._)).Returns(conditionInstance);

            using var ctSource = new CancellationTokenSource(500);
            var waitForInstances = Task.Run(async () => await _feed.WaitForInstancesAsync(ctSource.Token), ctSource.Token);

            await _sut.PersistAsync(new ApiAuthenticationSuccessEvent("", ""));
            await Task.Delay(100);

            await _feed.DisposeAsync();
            Assert.That(async () => await waitForInstances, Throws.TypeOf<OperationCanceledException>());
            A.CallTo(() => _conditionInstanceFactory.Create(A<string>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Given_MultipleEventsAndFeedsWithDifferentEvaluators_When_PersistAndWaitForInstances_Then_DifferentResultsForEachFeed()
        {
            Assume.That(_feed, Is.Not.Null);

            A.CallTo(() => _conditionInstanceFactory.Create(A<string>._)).Invokes((string id) => new ConditionInstance(TimeSpan.Zero, "ID"));
            
            A.CallTo(() => _evaluator.Invoke(A<ApiAuthenticationSuccessEvent>._)).Returns(true);

            var evaluator2 = A.Fake<Func<ApiAuthenticationSuccessEvent, ValueTask<bool>>>();
            A.CallTo(() => evaluator2.Invoke(A<ApiAuthenticationSuccessEvent>._)).ReturnsNextFromSequence(true, false);
            var feed2 = _sut.CreateFeed(_conditionInstanceFactory, evaluator2);

            await _sut.PersistAsync(new ApiAuthenticationSuccessEvent("", ""));
            await _sut.PersistAsync(new ApiAuthenticationSuccessEvent("", ""));

            var instances1 = await _feed.WaitForInstancesAsync(new CancellationTokenSource(100).Token);
            var instances2 = await feed2.WaitForInstancesAsync(new CancellationTokenSource(100).Token);

            Assert.That(instances1, Is.Not.Null.And.Count.EqualTo(2));
            Assert.That(instances2, Is.Not.Null.And.Count.EqualTo(1));
            A.CallTo(() => _conditionInstanceFactory.Create(A<string>._)).MustHaveHappened();

            // tear down
            await feed2.DisposeAsync();
        }

        [Test]
        public async Task Given_Feed_When_Dispose_Then_FeedRemoved()
        {
            Assume.That(_feed, Is.Not.Null);

            await _feed.DisposeAsync();
            await _feed.DisposeAsync(); // Repeat calls ok
        }
    }
}

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
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Periturf.Verify;

namespace Periturf.IdentityServer.Verification
{
    class EventVerificationManager : IEventSink, IEventVerificationManager
    {
        private List<EventConditionFeed> _feeds = new List<EventConditionFeed>();

        public IConditionFeed CreateFeed(IConditionInstanceFactory conditionInstanceFactory, Func<ApiAuthenticationSuccessEvent, ValueTask<bool>> evaluator)
        {
            var feed = new EventConditionFeed(this, conditionInstanceFactory, evaluator);
            _feeds.Add(feed);
            return feed;
        }

        private void Remove(EventConditionFeed feed)
        {
            _feeds.Remove(feed);
        }

        public async Task PersistAsync(Event evt)
        {
            foreach (var feed in _feeds)
                await feed.EvaluateInstanceAsync((ApiAuthenticationSuccessEvent)evt, CancellationToken.None);
        }

        class EventConditionFeed : IConditionFeed
        {
            private bool _disposed = false;
            private readonly Channel<int> _channel = Channel.CreateUnbounded<int>();
            private readonly EventVerificationManager _eventVerificationManager;
            private readonly IConditionInstanceFactory _conditionInstanceFactory;
            private readonly Func<ApiAuthenticationSuccessEvent, ValueTask<bool>> _evaluator;

            public EventConditionFeed(EventVerificationManager eventVerificationManager, IConditionInstanceFactory conditionInstanceFactory, Func<ApiAuthenticationSuccessEvent, ValueTask<bool>> evaluator)
            {
                _eventVerificationManager = eventVerificationManager;
                _conditionInstanceFactory = conditionInstanceFactory;
                _evaluator = evaluator;
            }

            public async ValueTask EvaluateInstanceAsync(ApiAuthenticationSuccessEvent @event, CancellationToken ct)
            {
                var result = await _evaluator(@event);
                if (result)
                    await _channel.Writer.WriteAsync(0, ct);
            }

            public async Task<List<ConditionInstance>> WaitForInstancesAsync(CancellationToken ct)
            {
                await _channel.Reader.WaitToReadAsync(ct);
                var instances = new List<ConditionInstance>();

                while (_channel.Reader.TryRead(out _))
                {
                    instances.Add(_conditionInstanceFactory.Create("ID"));
                }

                return instances;
            }

            public ValueTask DisposeAsync()
            {
                if (_disposed)
                    return ValueTask.CompletedTask;

                _eventVerificationManager.Remove(this);
                _disposed = true;
                return ValueTask.CompletedTask;
            }
        }
    }
}

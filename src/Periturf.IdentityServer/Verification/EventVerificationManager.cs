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
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly List<IEventConditionFeed> _feeds = new List<IEventConditionFeed>();

        public IConditionFeed CreateFeed<TEvent>(IConditionInstanceFactory conditionInstanceFactory, Func<TEvent, ValueTask<bool>> evaluator)
            where TEvent : Event
        {
            var feed = new EventConditionFeed<TEvent>(this, conditionInstanceFactory, evaluator);

            _lock.EnterWriteLock();
            try
            {
                _feeds.Add(feed);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            return feed;
        }

        private void Remove(IEventConditionFeed feed)
        {
            _lock.EnterWriteLock();
            try
            {
                _feeds.Remove(feed);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public async Task PersistAsync(Event evt)
        {
            _lock.EnterReadLock();
            try
            {
                await Task.WhenAll(_feeds.Select(x => x.EvaluateInstanceAsync(evt, CancellationToken.None).AsTask()));
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        interface IEventConditionFeed : IConditionFeed
        {
            ValueTask EvaluateInstanceAsync(Event @event, CancellationToken ct);
        }

        class EventConditionFeed<TEvent> : IConditionFeed, IEventConditionFeed where TEvent : Event
        {
            private bool _disposed = false;
            private readonly Channel<int> _channel = Channel.CreateUnbounded<int>();
            private readonly EventVerificationManager _eventVerificationManager;
            private readonly IConditionInstanceFactory _conditionInstanceFactory;
            private readonly Func<TEvent, ValueTask<bool>> _evaluator;

            public EventConditionFeed(EventVerificationManager eventVerificationManager, IConditionInstanceFactory conditionInstanceFactory, Func<TEvent, ValueTask<bool>> evaluator)
            {
                _eventVerificationManager = eventVerificationManager;
                _conditionInstanceFactory = conditionInstanceFactory;
                _evaluator = evaluator;
            }

            public async ValueTask EvaluateInstanceAsync(Event @event, CancellationToken ct)
            {
                if (@event is TEvent concreteEvent)
                {
                    var result = await _evaluator(concreteEvent);
                    if (result)
                        await _channel.Writer.WriteAsync(0, ct);
                }
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

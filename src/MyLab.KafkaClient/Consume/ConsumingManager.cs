using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.LogDsl;

namespace MyLab.KafkaClient.Consume
{
    class ConsumingManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DslLogger _log;

        public IKafkaLog KafkaLog { get; set; }

        public ConsumingManager( 
            IServiceProvider serviceProvider,
            ILogger<ConsumingManager> logger = null)
        {
            _serviceProvider = serviceProvider;
            _log = logger?.Dsl();
        }

        public async Task ConsumeLoopAsync(
            IConsumer<string, string> nativeConsumer,
            IKafkaConsumer[] consumers, 
            CancellationToken cancellationToken)
        {
            var topicsForSubscribe = consumers
                .Select(c => c.TopicName)
                .Distinct();

            nativeConsumer.Subscribe(topicsForSubscribe);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var incomingEvent = nativeConsumer.Consume(cancellationToken);

                    if (incomingEvent == null || incomingEvent.Message == null)
                        continue;

                    var hitConsumers = consumers
                        .Where(c => c.TopicName == incomingEvent.Topic)
                        .ToArray();

                    await ProcessEvent(nativeConsumer, hitConsumers, incomingEvent, cancellationToken);

                    KafkaLog?.ReportConsuming(incomingEvent);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    KafkaLog?.ReportConsumingError(e);

                    if (e.Error.IsFatal)
                    {
                        _log.Error("Fatal error when event consuming", e).Write();
                    }
                    else
                    {
                        _log.Error(e).Write();
                    }
                }
                catch (Exception e)
                {
                    _log.Error("Unhandled error when event consuming", e).Write();
                }
            }
        }

        private async Task ProcessEvent(
            IConsumer<string, string> nativeConsumer, 
            IKafkaConsumer[] hitConsumers, 
            ConsumeResult<string, string> incomingEvent, 
            CancellationToken cancellationToken)
        {
            if (hitConsumers.Length > 1)
            {
                _log.Warning("Too many consumers")
                    .AndFactIs("Topic", incomingEvent.Topic)
                    .AndFactIs("Consumer count", hitConsumers.Length)
                    .Write();
            }
            else if (hitConsumers.Length == 0)
            {
                _log.Warning("No consumer found")
                    .AndFactIs("Topic", incomingEvent.Topic)
                    .Write();

                return;
            }

            var c = hitConsumers.First();

            using var scope = _serviceProvider.CreateScope();
            var scopedServiceProvider = scope.ServiceProvider;

            SetScopedEvent(incomingEvent, scopedServiceProvider);

            var ctx = new ConsumingContext(scopedServiceProvider, incomingEvent);

            await c.ConsumeAsync(ctx, cancellationToken);

            nativeConsumer.Commit(incomingEvent);
        }

        private static void SetScopedEvent(ConsumeResult<string, string> incomingEvent, IServiceProvider scopedServiceProvider)
        {
            var eventAccessor = (KafkaEventAccessor)scopedServiceProvider.GetService(typeof(KafkaEventAccessor));
            eventAccessor?.SetScopedIncomingEvent(incomingEvent);
        }
    }
}

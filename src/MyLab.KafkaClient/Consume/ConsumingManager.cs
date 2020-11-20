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
        private readonly IKafkaLog _kafkaLog;
        private readonly DslLogger _log;
        private readonly ConsumerConfig _config;

        public ConsumingManager( 
            IServiceProvider serviceProvider,
            IConsumerConfigProvider consumerConfigProvider,
            IKafkaLog kafkaLog = null,
            ILogger<ConsumingManager> logger = null)
        {
            _serviceProvider = serviceProvider;
            _config = consumerConfigProvider.ProvideConsumerConfig();
            _kafkaLog = kafkaLog;
            _log = logger?.Dsl();
        }

        public async Task ConsumeLoopAsync(
            IKafkaConsumer[] consumers, 
            CancellationToken cancellationToken)
        {
            var topicsForSubscribe = consumers
                .Select(c => c.TopicName)
                .Distinct();

            var nativeConsumer = CreateConsumer();
            nativeConsumer.Subscribe(topicsForSubscribe);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var incomingEvent = nativeConsumer.Consume(cancellationToken);

                        if (incomingEvent?.Message == null)
                        {
                            throw new InvalidOperationException("Incoming message is empty");
                        }

                        var hitConsumers = consumers
                            .Where(c => c.TopicName == incomingEvent.Topic)
                            .ToArray();

                        await ProcessEvent(nativeConsumer, hitConsumers, incomingEvent, cancellationToken);

                        _kafkaLog?.ReportConsuming(incomingEvent);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (ConsumeException e)
                    {
                        _kafkaLog?.ReportConsumingError(e);

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
            finally
            {
                nativeConsumer.Dispose();
            }
        }

        private IConsumer<string, string> CreateConsumer()
        {
            return new ConsumerBuilder<string, string>(_config).Build();
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

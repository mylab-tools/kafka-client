using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyLab.KafkaClient.Consume
{
    class KafkaConsumersHost : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IKafkaConsumerRegistry _consumerRegistry;
        private readonly IKafkaLog _kafkaLog;
        private readonly IConsumer<string, string> _consumer;

        public KafkaConsumersHost(
            IServiceProvider serviceProvider,
            IKafkaConsumerRegistry consumerRegistry, 
            IConsumerConfigProvider consumerConfigProvider,
            IKafkaLog kafkaLog = null)
        {
            _serviceProvider = serviceProvider;
            _consumerRegistry = consumerRegistry;
            _kafkaLog = kafkaLog;

            var consumeConfig = consumerConfigProvider.ProvideConsumerConfig();
            var consumerBuilder = new ConsumerBuilder<string, string>(consumeConfig);
            _consumer = consumerBuilder.Build();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => Consume(stoppingToken), stoppingToken);
        }

        private async Task Consume(CancellationToken stoppingToken)
        {
            var consumers = _consumerRegistry
                .ProvideConsumers()
                .ToArray();

            var topicsForSubscribe = consumers
                .Select(c => c.TopicName)
                .Distinct();

            _consumer.Subscribe(topicsForSubscribe);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var incomingEvent = _consumer.Consume(stoppingToken);

                    if (incomingEvent == null || incomingEvent.Message == null)
                        continue;
                    
                    var hitConsumers = consumers
                        .Where(c => c.TopicName == incomingEvent.Topic)
                        .ToArray();

                    await ProcessEvent(hitConsumers, incomingEvent, stoppingToken);

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
                        //todo: log error
                        break;
                    }
                    else
                    {
                        //todo: log warning
                    }
                }
                catch (Exception)
                {
                    //todo: log error
                    break;
                }
            }
        }

        private async Task ProcessEvent(IKafkaConsumer[] hitConsumers, ConsumeResult<string, string> incomingEvent, CancellationToken cancellationToken)
        {
            if (hitConsumers.Length > 1)
            {
                //todo: log warning
            }
            else if (hitConsumers.Length == 0)
            {
                //todo: log warning

                return;
            }

            var c = hitConsumers.First();

            using var scope = _serviceProvider.CreateScope();
            var scopedServiceProvider = scope.ServiceProvider;

            SetScopedEvent(incomingEvent, scopedServiceProvider);

            var ctx = new ConsumingContext(scopedServiceProvider, incomingEvent);

            try
            {
                await c.ConsumeAsync(ctx, cancellationToken);

                _consumer.Commit(incomingEvent);
            }
            catch (Exception e)
            {
                //todo: log error
            }
        }

        private static void SetScopedEvent(ConsumeResult<string, string> incomingEvent, IServiceProvider scopedServiceProvider)
        {
            var eventAccessor = (KafkaEventAccessor) scopedServiceProvider.GetService(typeof(KafkaEventAccessor));
            eventAccessor?.SetScopedIncomingEvent(incomingEvent);
        }

        public override void Dispose()
        {
            _consumer.Dispose();

            base.Dispose();
        }
    }
}

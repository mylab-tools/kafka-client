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
        private readonly IConsumer<string, string> _nativeConsumer;

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
            _nativeConsumer = consumerBuilder.Build();
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

            var consumeManager = ActivatorUtilities.CreateInstance<ConsumingManager>(_serviceProvider);
            consumeManager.KafkaLog = _kafkaLog;

            await consumeManager.ConsumeLoopAsync(_nativeConsumer, consumers, stoppingToken);
        }

        public override void Dispose()
        {
            _nativeConsumer.Dispose();

            base.Dispose();
        }
    }
}

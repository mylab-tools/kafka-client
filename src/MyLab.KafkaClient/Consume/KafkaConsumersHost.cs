using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyLab.KafkaClient.Consume
{
    class KafkaConsumersHost : BackgroundService
    {
        private readonly IKafkaConsumerRegistry _consumerRegistry;
        private readonly ConsumingManager _consumingManager;

        public KafkaConsumersHost(
            IServiceProvider serviceProvider,
            IKafkaConsumerRegistry consumerRegistry)
        {
            _consumerRegistry = consumerRegistry;

            _consumingManager = ActivatorUtilities.CreateInstance<ConsumingManager>(serviceProvider);
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

            await _consumingManager.ConsumeLoopAsync(consumers, stoppingToken);
        }
    }
}

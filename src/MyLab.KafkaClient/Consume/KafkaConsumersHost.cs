﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;

namespace MyLab.KafkaClient.Consume
{
    class KafkaConsumersHost : BackgroundService
    {
        private readonly IKafkaConsumerRegistry _consumerRegistry;

        public KafkaConsumersHost(IKafkaConsumerRegistry consumerRegistry)
        {
            _consumerRegistry = consumerRegistry;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => Consume(stoppingToken), stoppingToken);
        }

        private void Consume(in CancellationToken stoppingToken)
        {
            var consumers = _consumerRegistry.ProvideConsumers();

            var topicsForSubscribe = consumers
                .Select(c => c.TopicName)
                .Distinct();

            var consumeConfig = new ConsumerConfig();
            var consumerBuilder = new ConsumerBuilder<string,string>(consumeConfig);
            var consumer = consumerBuilder.Build();

            consumer.Subscribe(topicsForSubscribe);

            
        }
    }
}

using System;
using Microsoft.Extensions.DependencyInjection;

namespace MyLab.KafkaClient.Consume
{
    class DiConsumerFactory<TConsumer> : IKafkaConsumerFactory
        where TConsumer : KafkaConsumer
    {
        public KafkaConsumer Create(IServiceProvider serviceProvider)
        {
            return ActivatorUtilities.CreateInstance<TConsumer>(serviceProvider);
        }
    }
}
using System;
using Microsoft.Extensions.DependencyInjection;

namespace MyLab.KafkaClient.Consume
{
    class DiConsumerFactory<TConsumer> : IKafkaConsumerFactory
        where TConsumer : IKafkaConsumer
    {
        public IKafkaConsumer Create(IServiceProvider serviceProvider)
        {
            return ActivatorUtilities.CreateInstance<TConsumer>(serviceProvider);
        }
    }
}
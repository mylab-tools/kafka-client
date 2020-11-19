using System;

namespace MyLab.KafkaClient.Consume
{
    /// <summary>
    /// Creates Kafak consumers
    /// </summary>
    public interface IKafkaConsumerFactory
    {
        /// <summary>
        /// Creates Kafka consumer uses registered services
        /// </summary>
        KafkaConsumer Create(IServiceProvider serviceProvider);
    }
}
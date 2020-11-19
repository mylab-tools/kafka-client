using System;

namespace MyLab.KafkaClient.Consume
{
    /// <summary>
    /// Creates Kafka consumers
    /// </summary>
    public interface IKafkaConsumerFactory
    {
        /// <summary>
        /// Creates Kafka consumer uses registered services
        /// </summary>
        IKafkaConsumer Create(IServiceProvider serviceProvider);
    }
}
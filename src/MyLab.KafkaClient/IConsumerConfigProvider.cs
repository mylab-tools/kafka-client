using Confluent.Kafka;

namespace MyLab.KafkaClient
{
    /// <summary>
    /// Provides Kafka consumer config
    /// </summary>
    public interface IConsumerConfigProvider
    {
        /// <summary>
        /// Provides consumer config
        /// </summary>
        ConsumerConfig ProvideConsumerConfig();
    }
}
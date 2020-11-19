using Confluent.Kafka;

namespace MyLab.KafkaClient
{
    /// <summary>
    /// Provides Kafka producer config
    /// </summary>
    public interface IProducerConfigProvider
    {
        /// <summary>
        /// Provides producer config
        /// </summary>
        ProducerConfig ProvideProducerConfig();
    }
}
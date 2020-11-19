using Confluent.Kafka;

namespace MyLab.KafkaClient
{
    /// <summary>
    /// Provides Kafka admin config
    /// </summary>
    public interface IAdminConfigProvider
    {
        /// <summary>
        /// Provides admin config
        /// </summary>
        AdminClientConfig ProvideAdminConfig();
    }
}
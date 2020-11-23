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
        ConsumerConfigEx ProvideConsumerConfig();
    }
}
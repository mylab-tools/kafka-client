namespace MyLab.KafkaClient
{
    /// <summary>
    /// Defines Kafka event model which provides event key
    /// </summary>
    public interface IKafkaEventKayProvider
    {
        /// <summary>
        /// Provides Kafka event key
        /// </summary>
        string ProvideKey();
    }
}
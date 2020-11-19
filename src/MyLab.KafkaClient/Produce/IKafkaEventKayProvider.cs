namespace MyLab.KafkaClient.Produce
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
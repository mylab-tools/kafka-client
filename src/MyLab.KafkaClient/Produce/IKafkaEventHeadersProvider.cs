using Confluent.Kafka;

namespace MyLab.KafkaClient.Produce
{
    /// <summary>
    /// Defines Kafka event model which provides event headers
    /// </summary>
    public interface IKafkaEventHeadersProvider
    {
        /// <summary>
        /// Adds headers into specified collection
        /// </summary>
        void ProvideHeaders(Headers headers);
    }
}
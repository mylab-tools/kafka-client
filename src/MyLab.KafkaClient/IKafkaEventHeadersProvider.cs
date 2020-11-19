using Confluent.Kafka;

namespace MyLab.KafkaClient
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
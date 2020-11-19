using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace MyLab.KafkaClient.Produce
{
    /// <summary>
    /// Publish events to Kafka
    /// </summary>
    public interface IKafkaProducer
    {
        /// <summary>
        /// Publish event to Kafka
        /// </summary>
        /// <exception cref="KafkaProduceException">Producing error</exception>
        Task<TopicPartitionOffset> ProduceAsync(OutgoingKafkaEvent eventObj, CancellationToken cancellationToken = default);
    }
}

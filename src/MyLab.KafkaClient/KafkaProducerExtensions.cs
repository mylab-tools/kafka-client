using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace MyLab.KafkaClient
{
    /// <summary>
    /// Extensions for <see cref="IKafkaProducer"/>
    /// </summary>
    public static class KafkaProducerExtensions
    {
        /// <summary>
        /// Publish event to Kafka
        /// </summary>
        /// <exception cref="KafkaProduceException">Producing error</exception>
        public static Task<TopicPartitionOffset> ProduceAsync(this IKafkaProducer producer, object eventContent,
            CancellationToken cancellationToken = default)
        {
            if (producer == null) throw new ArgumentNullException(nameof(producer));

            return producer.ProduceAsync(new OutgoingKafkaEvent(eventContent), cancellationToken);
        }
    }
}
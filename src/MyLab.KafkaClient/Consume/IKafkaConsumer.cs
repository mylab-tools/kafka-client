using System.Threading;
using System.Threading.Tasks;

namespace MyLab.KafkaClient.Consume
{
    /// <summary>
    /// Defines Kafka consumer
    /// </summary>
    public interface IKafkaConsumer
    {
        /// <summary>
        /// Target topic 
        /// </summary>
        public string TopicName { get; }

        /// <summary>
        /// Consumes incoming event 
        /// </summary>
        Task ConsumeAsync(IConsumingContext ctx, CancellationToken cancellationToken);
    }
}
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.KafkaClient.Consume
{
    /// <summary>
    /// Consumes Kafka events
    /// </summary>
    /// <typeparam name="TLogic">logic type</typeparam>
    public class KafkaConsumer<TLogic> : IKafkaConsumer
        where TLogic : IKafkaConsumerLogic
    {
        /// <summary>
        /// Target topic 
        /// </summary>
        public string TopicName { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="KafkaConsumer{T}"/>
        /// </summary>
        public KafkaConsumer(string topicName)
        {
            TopicName = topicName;
        }

        /// <summary>
        /// Consumes incoming event 
        /// </summary>
        public Task ConsumeAsync(IConsumingContext ctx, CancellationToken cancellationToken)
        {
            return null;
        }
    }

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

    public interface IKafkaConsumerLogic
    {

    }

    /// <summary>
    /// Contains tools for event consuming
    /// </summary>
    public interface IConsumingContext
    {
        /// <summary>
        /// Creates logic
        /// </summary>
        IKafkaConsumerFactory CreateLogic<T>()
            where T : IKafkaConsumerLogic;
    }
}
using System.Threading;
using System.Threading.Tasks;

namespace MyLab.KafkaClient.Consume
{
    /// <summary>
    /// Consumes Kafka events
    /// </summary>
    /// <typeparam name="TLogic">logic type</typeparam>
    /// <typeparam name="TEventContent">event content type</typeparam>
    public class KafkaConsumer<TLogic, TEventContent> : IKafkaConsumer
        where TLogic : IKafkaConsumerLogic<TEventContent>
    {
        /// <summary>
        /// Target topic 
        /// </summary>
        public string TopicName { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="KafkaConsumer{TLogic, TEventContent}"/>
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
            var logic = ctx.CreateLogic<TLogic, TEventContent>();
            return logic.ConsumeAsync(ctx.ProvideEvent<TEventContent>());
        }
    }
}
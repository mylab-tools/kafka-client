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
        private readonly TLogic _logic;

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
        /// Initializes a new instance of <see cref="KafkaConsumer{TLogic, TEventContent}"/>
        /// </summary>
        public KafkaConsumer(string topicName, TLogic logic)
        {
            _logic = logic;
            TopicName = topicName;
        }

        /// <summary>
        /// Consumes incoming event 
        /// </summary>
        public Task ConsumeAsync(IConsumingContext ctx, CancellationToken cancellationToken)
        {
            var logic = _logic ?? ctx.CreateLogic<TLogic, TEventContent>();
            return logic.ConsumeAsync(ctx.ProvideEvent<TEventContent>());
        }
    }
}
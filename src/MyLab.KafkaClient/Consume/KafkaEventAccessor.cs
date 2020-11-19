using Confluent.Kafka;

namespace MyLab.KafkaClient.Consume
{
    /// <summary>
    /// Gets access to scoped incoming Kafka event
    /// </summary>
    public interface IKafkaEventAccessor
    {
        /// <summary>
        /// Gets scoped event consuming result
        /// </summary>
        IncomingKafkaEvent<TContent> GetScopedIncomingEvent<TContent>();
    }

    interface IKafkaEventAccessorCore
    {
        void SetScopedIncomingEvent(ConsumeResult<string, string> consumeResult);
    }

    class KafkaEventAccessor : IKafkaEventAccessor, IKafkaEventAccessorCore
    {
        private ConsumeResult<string, string> _event;

        public IncomingKafkaEvent<TContent> GetScopedIncomingEvent<TContent>()
        {
            return new IncomingKafkaEvent<TContent>(_event);
        }

        public void SetScopedIncomingEvent(ConsumeResult<string, string> consumeResult)
        {
            _event = consumeResult;
        }
    }
}

namespace MyLab.KafkaClient.Consume
{
    /// <summary>
    /// Contains tools for event consuming
    /// </summary>
    public interface IConsumingContext
    {
        /// <summary>
        /// Creates logic
        /// </summary>
        IKafkaConsumerLogic<TEventContent> CreateLogic<TLogic, TEventContent>()
            where TLogic : IKafkaConsumerLogic<TEventContent>;

        /// <summary>
        /// Provides incoming Kafka event
        /// </summary>
        IncomingKafkaEvent<TContent> ProvideEvent<TContent>();
    }
}
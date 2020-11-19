using System.Threading.Tasks;

namespace MyLab.KafkaClient.Consume
{
    /// <summary>
    /// Defines consumer logic
    /// </summary>
    /// <typeparam name="TEventContent">incoming message content type</typeparam>
    public interface IKafkaConsumerLogic<TEventContent>
    {
        /// <summary>
        /// Consumes incoming event
        /// </summary>
        Task ConsumeAsync(IncomingKafkaEvent<TEventContent> incomingKafkaEvent);
    }
}
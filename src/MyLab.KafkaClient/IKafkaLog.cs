using Confluent.Kafka;

namespace MyLab.KafkaClient
{
    /// <summary>
    /// Collect kafka communications
    /// </summary>
    public interface IKafkaLog
    {
        /// <summary>
        /// Reports about success consuming
        /// </summary>
        void ReportConsuming(ConsumeResult<string, string> consumeResult);
        /// <summary>
        /// Report about success producing
        /// </summary>
        void ReportProducing(DeliveryResult<string, string> deliveryResult);
        /// <summary>
        /// Report about producing error
        /// </summary>
        void ReportProducingError(ProduceException<string, string> e);

        /// <summary>
        /// Report about consuming error
        /// </summary>
        void ReportConsumingError(ConsumeException e);
    }
}

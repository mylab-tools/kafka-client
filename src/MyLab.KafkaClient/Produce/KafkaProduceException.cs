using Confluent.Kafka;

namespace MyLab.KafkaClient.Produce
{
    /// <summary>
    /// Throws when producing exception
    /// </summary>
    public class KafkaProduceException : ProduceException<string, string>
    {
        public KafkaProduceException(ProduceException<string, string> origin) 
            : base(origin.Error, origin.DeliveryResult, origin)
        {
        }
    }
}
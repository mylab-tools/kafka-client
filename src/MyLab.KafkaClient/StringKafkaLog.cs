using System;
using Confluent.Kafka;

namespace MyLab.KafkaClient
{
    class StringKafkaLog : IKafkaLog
    {
        private readonly IKafkaLogWriter _writer;

        /// <summary>
        /// Initializes a new instance of <see cref="StringKafkaLog"/>
        /// </summary>
        public StringKafkaLog(IKafkaLogWriter writer)
        {
            _writer = writer;
        }

        public void ReportConsuming(ConsumeResult<string, string> consumeResult)
        {
            _writer.WriteLine($"Consumed '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'.");
        }

        public void ReportProducing(DeliveryResult<string, string> deliveryResult)
        {
            _writer.WriteLine($"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
        }
        
        public void ReportProducingError(ProduceException<string, string> e)
        {
            _writer.WriteLine("Delivering error:");
            _writer.WriteLine(e.ToString());
        }

        public void ReportConsumingError(ConsumeException e)
        {
            _writer.WriteLine("Consuming error:");
            _writer.WriteLine(e.ToString());
        }
    }
}
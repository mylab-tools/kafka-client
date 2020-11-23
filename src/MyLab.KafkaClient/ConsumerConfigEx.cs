using Confluent.Kafka;

namespace MyLab.KafkaClient
{
    /// <summary>
    /// Extended base consumer configuration based on <see cref="ConsumerConfig"/>
    /// </summary>
    public class ConsumerConfigEx : ConsumerConfig
    {
        /// <summary>
        /// An idle interval (before next try) after unhandled exception
        /// </summary>
        public int? ErrorBasedRetryIntervalMs { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ConsumerConfigEx"/>
        /// </summary>
        public ConsumerConfigEx(ClientConfig clientConfig)
            :base(clientConfig)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ConsumerConfigEx"/>
        /// </summary>
        public ConsumerConfigEx(ConsumerConfig consumerConfig)
            : base(consumerConfig)
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="ConsumerConfigEx"/>
        /// </summary>
        public ConsumerConfigEx()
        {
            
        }
    }
}
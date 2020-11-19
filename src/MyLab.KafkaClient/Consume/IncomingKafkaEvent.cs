using System;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace MyLab.KafkaClient.Consume
{
    /// <summary>
    /// Defines consumed event
    /// </summary>
    public class IncomingKafkaEvent<TContent>
    {
        /// <summary>
        /// The collection of message headers (or null). Specifying null or an empty list are equivalent. The order of headers is maintained, and duplicate header keys are allowed.
        /// </summary>
        public Headers Headers { get; }

        /// <summary>
        /// True if this instance represents an end of partition event, false if it represents a message in kafka.
        /// </summary>
        public bool IsPartitionEof { get; }

        /// <summary>
        /// The TopicPartitionOffset associated with the message.
        /// </summary>
        public TopicPartitionOffset TopicPartitionOffset { get; }

        /// <summary>
        /// Event content
        /// </summary>
        public TContent Content { get; }

        /// <summary>
        /// The Kafka message Key
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="IncomingKafkaEvent{T}"/>
        /// </summary>
        public IncomingKafkaEvent(ConsumeResult<string, string> incomingEvent)
        {
            if (incomingEvent == null) throw new ArgumentNullException(nameof(incomingEvent));

            Headers = incomingEvent.Message.Headers;
            Key = incomingEvent.Message.Key;
            Content = JsonConvert.DeserializeObject<TContent>(incomingEvent.Message.Value);
            TopicPartitionOffset = incomingEvent.TopicPartitionOffset;
            IsPartitionEof = incomingEvent.IsPartitionEOF;
        }
    }
}

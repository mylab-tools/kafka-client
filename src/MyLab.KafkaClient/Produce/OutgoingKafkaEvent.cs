using System;
using System.Reflection;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace MyLab.KafkaClient.Produce
{
    /// <summary>
    /// Defines produced event
    /// </summary>
    public class OutgoingKafkaEvent
    {
        /// <summary>
        /// Specifies topic and partition for publishing
        /// </summary>
        public TopicPartition Target { get; set; }

        /// <summary>
        /// Specifies event headers
        /// </summary>
        public Headers Headers { get; set; }

        /// <summary>
        /// Specifies event key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Content
        /// </summary>
        public object Content { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="OutgoingKafkaEvent"/>
        /// </summary>
        public OutgoingKafkaEvent(object content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        /// <summary>
        /// Gets evaluated <see cref="TopicPartition"/>
        /// </summary>
        public TopicPartition GetTarget()
        {
            string topic = Target?.Topic;

            if (topic == null)
            {
                var contentType = Content.GetType();
                var topicAttr = contentType.GetCustomAttribute<KafkaTopicAttribute>();
                if(topicAttr == null)
                    throw new InvalidOperationException($"Topic is not defined. Event type: '{contentType.FullName}'");

                topic = topicAttr.TopicName;
            }

            return new TopicPartition(topic, Target?.Partition ?? Partition.Any);
        }

        /// <summary>
        /// Creates <see cref="Message{TKey,TValue}"/> from contained properties
        /// </summary>
        public Message<string,string> ToNativeMessage()
        {
            var key = Key;

            if(key == null && Content is IKafkaEventKayProvider keyProvider)
            {
                key = keyProvider.ProvideKey();
            }

            var headers = Headers ?? new Headers();

            if (Content is IKafkaEventHeadersProvider headersProvider)
            {
                headersProvider.ProvideHeaders(headers);
            }

            var strContent = Content is string sc ? sc : JsonConvert.SerializeObject(Content);

            return new Message<string, string>
            {
                Key = key,
                Headers = headers,
                Value = strContent
            };
        }
    }
}
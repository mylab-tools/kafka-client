using System;

namespace MyLab.KafkaClient
{
    /// <summary>
    /// Defines topic name for kafka event model
    /// </summary>
    public class KafkaTopicAttribute : Attribute
    {
        /// <summary>
        /// Gets target topic name
        /// </summary>
        public string TopicName { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="KafkaTopicAttribute"/>
        /// </summary>
        public KafkaTopicAttribute(string topicName)
        {
            TopicName = topicName;
        }
    }
}
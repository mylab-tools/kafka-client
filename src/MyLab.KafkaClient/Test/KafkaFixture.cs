using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace MyLab.KafkaClient.Test
{
    /// <summary>
    /// Provides abilities to work with kafka for testing
    /// </summary>
    public sealed class KafkaFixture : IDisposable
    {
        private readonly List<KafkaTopicFactory> _factories = new List<KafkaTopicFactory>();

        /// <summary>
        /// Creates new topic factory for specified configuration
        /// </summary>
        public KafkaTopicFactory CreateTopicFactory(ClientConfig clientConfig, string topicNamePrefix = null)
        {
            var config = new AdminClientConfig(clientConfig);

            var adminClient = new AdminClientBuilder(config).Build();

            var factory = new KafkaTopicFactory(adminClient, clientConfig)
            {
                TopicNamePrefix = topicNamePrefix
            };

            _factories.Add(factory);

            return factory;
        }

        public void Dispose()
        {
            Task.WaitAll(_factories.Select(f => f.DisposeAsync().AsTask()).ToArray());
            _factories.Clear();
        }
    }
}

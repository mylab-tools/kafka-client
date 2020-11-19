using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace MyLab.KafkaClient.Test
{
    /// <summary>
    /// Creates Kafka topics
    /// </summary>
    public sealed class KafkaTopicFactory : IAsyncDisposable
    {
        private readonly IAdminClient _adminClient;
        private readonly ClientConfig _clientConfig;
        private readonly List<KafkaTopic> _createdTopics = new List<KafkaTopic>();

        /// <summary>
        /// Adds prefix for topic name if create with Id and random Id
        /// </summary>
        public string TopicNamePrefix { get; set; }

        /// <summary>
        /// Gets or sets Kafka communications log
        /// </summary>
        public IKafkaLog Log { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="KafkaTopicFactory"/>
        /// </summary>
        public KafkaTopicFactory(IAdminClient adminClient, ClientConfig clientConfig)
        {
            _adminClient = adminClient ?? throw new ArgumentNullException(nameof(adminClient));
            _clientConfig = clientConfig;
        }

        public async Task<KafkaTopic> CreateWithNameAsync(string fullName)
        {
            await _adminClient.CreateTopicsAsync(Enumerable.Repeat(new TopicSpecification
            {
                Name = fullName,
                NumPartitions = 1,
            }, 1));

            var topic = new KafkaTopic(fullName, _adminClient, _clientConfig)
            {
                Log = Log
            };

            _createdTopics.Add(topic);
            return topic;
        }

        public Task<KafkaTopic> CreateWithIdAsync(string id)
        {
            return CreateWithNameAsync(TopicNamePrefix + id);
        }

        public Task<KafkaTopic> CreateWithRandomIdAsync()
        {
            return CreateWithIdAsync(Guid.NewGuid().ToString("N"));
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var createdTopic in _createdTopics)
            {
                await createdTopic.DisposeAsync();
            }

            _createdTopics.Clear();

            _adminClient?.Dispose();
        }
    }
}
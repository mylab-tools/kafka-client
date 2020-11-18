using System;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace MyLab.KafkaClient.Test
{
    /// <summary>
    /// Represent Kafka topic
    /// </summary>
    public class KafkaTopic : IAsyncDisposable
    {
        private readonly IAdminClient _adminClient;
        private readonly ClientConfig _clientConfig;


        /// <summary>
        /// Topic name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="KafkaTopic"/>
        /// </summary>
        public KafkaTopic(string name, IAdminClient adminClient, ClientConfig clientConfig)
        {
            _adminClient = adminClient;
            _clientConfig = clientConfig;
            Name = name;
        }

        public bool Exists()
        {
            var md = _adminClient.GetMetadata(Name,TimeSpan.FromMinutes(1));

            var found = md?.Topics?.FirstOrDefault(t => t.Topic == Name);

            if (found == null) return false;

            return found.Partitions != null && found.Partitions.Count != 0;
        }

        public async ValueTask DisposeAsync()
        {
            await _adminClient.DeleteTopicsAsync(Enumerable.Repeat(Name, 1));
        }
    }
}
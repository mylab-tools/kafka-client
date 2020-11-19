using System;
using System.Linq;
using System.Runtime.CompilerServices;
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

        readonly Lazy<IProducer<string, string>> _producer;
        readonly Lazy<IConsumer<string, string>> _consumer;

        /// <summary>
        /// Topic name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets Kafka communications log
        /// </summary>
        public IKafkaLog Log { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="KafkaTopic"/>
        /// </summary>
        public KafkaTopic(string name, IAdminClient adminClient, ClientConfig clientConfig)
        {
            _adminClient = adminClient;
            Name = name;

            _producer = new Lazy<IProducer<string, string>>(() =>
                new ProducerBuilder<string, string>(clientConfig).Build());

            var consumerConfig = new ConsumerConfig(clientConfig)
            {
                GroupId = Guid.NewGuid().ToString("N"),
                AutoOffsetReset = AutoOffsetReset.Earliest
            }; 

            _consumer = new Lazy<IConsumer<string, string>>(() =>
                new ConsumerBuilder<string, string>(consumerConfig).Build());
        }

        public bool Exists()
        {
            var md = _adminClient.GetMetadata(Name,TimeSpan.FromMinutes(1));

            var found = md?.Topics?.FirstOrDefault(t => t.Topic == Name);

            if (found == null) return false;

            return found.Partitions != null && found.Partitions.Count != 0;
        }

        public Task<TopicPartitionOffset> Produce(string content)
        {
            return Produce(new Message<string, string>
            {
                Value = content
            });
        }

        public async Task<TopicPartitionOffset> Produce(Message<string, string> message)
        {
            DeliveryResult<string, string> res;

            try
            {
                res = await _producer.Value.ProduceAsync(Name, message);

                Log?.ReportProducing(res);
            }
            catch (ProduceException<string,string> e)
            {
                Log?.ReportProducingError(e);
                throw;
            }

            return res.TopicPartitionOffset;
        }

        public string ConsumeOne(TimeSpan timeout = default)
        {
            _consumer.Value.Subscribe(Name);

            var realTimeout = timeout == default ? TimeSpan.FromSeconds(1) : timeout;

            ConsumeResult<string, string> incomingEvent;

            try
            {
                incomingEvent = _consumer.Value.Consume(realTimeout);

                Log?.ReportConsuming(incomingEvent);
            }
            catch (ConsumeException e)
            {
                Log?.ReportConsumingError(e);
                throw;
            }

            if(incomingEvent == null)
                throw new TimeoutException();

            return incomingEvent.Message.Value;
        }

        public async ValueTask DisposeAsync()
        {
            await _adminClient.DeleteTopicsAsync(Enumerable.Repeat(Name, 1));

            if (_producer.IsValueCreated)
                _producer.Value.Dispose();

            if (_consumer.IsValueCreated)
                _consumer.Value.Dispose();
        }
    }
}
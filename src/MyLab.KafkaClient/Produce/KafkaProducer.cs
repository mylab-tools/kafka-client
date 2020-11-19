using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace MyLab.KafkaClient.Produce
{
    class KafkaProducer : IKafkaProducer
    {
        private readonly IKafkaLog _log;
        private readonly Lazy<IProducer<string, string>> _producer;

        public KafkaProducer(IOptions<ProducerConfig> options, IKafkaLog log = null)
            :this(options.Value)
        {
            
        }

        public KafkaProducer(ProducerConfig options, IKafkaLog log = null)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _log = log;

            _producer = new Lazy<IProducer<string, string>>(() =>
                new ProducerBuilder<string, string>(options).Build());
        }

        public Task<TopicPartitionOffset> ProduceAsync(OutgoingKafkaEvent eventObj, CancellationToken cancellationToken = default)
        {
            if (eventObj == null) throw new ArgumentNullException(nameof(eventObj));

            return ProduceCoreAsync(eventObj, cancellationToken);
        }

        async Task<TopicPartitionOffset> ProduceCoreAsync(OutgoingKafkaEvent eventObj, CancellationToken cancellationToken)
        {
            var producer = _producer.Value;

            DeliveryResult<string, string> res;

            try
            {
                res = await producer.ProduceAsync(
                    eventObj.GetTarget(),
                    eventObj.ToNativeMessage(),
                    cancellationToken);

                _log?.ReportProducing(res);
            }
            catch (ProduceException<string, string> e)
            {
                _log?.ReportProducingError(e);

                throw new KafkaProduceException(e);
            }

            return res.TopicPartitionOffset;
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace MyLab.KafkaClient.Produce
{
    class KafkaProducer : IKafkaProducer
    {
        private readonly IKafkaLog _log;
        private readonly Lazy<IProducer<string, string>> _producer;
        
        public KafkaProducer(IProducerConfigProvider configProvider, IKafkaLog log = null)
        {
            _log = log;

            var config = configProvider.ProvideProducerConfig();
            _producer = new Lazy<IProducer<string, string>>(() =>
                new ProducerBuilder<string, string>(config).Build());
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
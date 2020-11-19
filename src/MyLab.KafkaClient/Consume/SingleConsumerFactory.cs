using System;

namespace MyLab.KafkaClient.Consume
{
    class SingleConsumerFactory : IKafkaConsumerFactory
    {
        private readonly KafkaConsumer _singleConsumer;

        public SingleConsumerFactory(KafkaConsumer singleConsumer)
        {
            _singleConsumer = singleConsumer ?? throw new ArgumentNullException(nameof(singleConsumer));
        }
        public KafkaConsumer Create(IServiceProvider serviceProvider)
        {
            return _singleConsumer;
        }
    }
}
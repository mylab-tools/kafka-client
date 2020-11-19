using System;

namespace MyLab.KafkaClient.Consume
{
    class SingleConsumerFactory : IKafkaConsumerFactory
    {
        private readonly IKafkaConsumer _singleConsumer;

        public SingleConsumerFactory(IKafkaConsumer singleConsumer)
        {
            _singleConsumer = singleConsumer ?? throw new ArgumentNullException(nameof(singleConsumer));
        }
        public IKafkaConsumer Create(IServiceProvider serviceProvider)
        {
            return _singleConsumer;
        }
    }
}
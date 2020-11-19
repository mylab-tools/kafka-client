using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLab.KafkaClient.Consume
{
    class KafkaConsumerRegistry : IKafkaConsumerRegistry
    {
        readonly ICollection<IKafkaConsumerFactory> _consumerFactories = new List<IKafkaConsumerFactory>();

        private readonly IServiceProvider _serviceProvider;

        public KafkaConsumerRegistry(IKafkaConsumerRegistrar consumerRegistrar, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            consumerRegistrar.Register(_consumerFactories);
        }

        public IEnumerable<KafkaConsumer> ProvideConsumers()
        {
            return _consumerFactories
                .Select(cf => cf.Create(_serviceProvider))
                .Where(consumer => consumer != null);
        }
    }
}
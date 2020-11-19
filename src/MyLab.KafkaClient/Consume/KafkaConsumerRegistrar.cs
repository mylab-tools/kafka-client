using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MyLab.KafkaClient.Consume
{
    /// <summary>
    /// Registers Kafka consumer factories
    /// </summary>
    public class KafkaConsumerRegistrar : IKafkaConsumerRegistrar
    {
        private readonly Collection<IKafkaConsumerFactory> _factories = new Collection<IKafkaConsumerFactory>();

        /// <summary>
        /// Registers new consumer factory
        /// </summary>
        public void Add(IKafkaConsumerFactory consumerFactory)
        {
            if (consumerFactory == null) throw new ArgumentNullException(nameof(consumerFactory));
            _factories.Add(consumerFactory);
        }

        void IKafkaConsumerRegistrar.Register(ICollection<IKafkaConsumerFactory> targetRegistry)
        {
            foreach (var kafkaConsumerFactory in _factories)
            {
                targetRegistry.Add(kafkaConsumerFactory);
            }
        }
    }
}
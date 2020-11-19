using System.Collections.Generic;

namespace MyLab.KafkaClient.Consume
{
    interface IKafkaConsumerRegistrar
    {
        void Register(ICollection<IKafkaConsumerFactory> targetRegistry);
    }
}
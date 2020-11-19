using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace MyLab.KafkaClient.Consume
{
    interface IKafkaConsumerRegistry
    {
        IEnumerable<KafkaConsumer> ProvideConsumers();
    }
}

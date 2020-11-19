using System;
using Microsoft.Extensions.Options;

namespace MyLab.KafkaClient.Consume
{
    class OptionsBasedConsumerFactory<TOptions> : IKafkaConsumerFactory
        where TOptions : class, new()
    {
        private readonly Func<TOptions, KafkaConsumer> _consumerFactory;

        public OptionsBasedConsumerFactory(Func<TOptions, KafkaConsumer> consumerFactoryMethod)
        {
            _consumerFactory = consumerFactoryMethod;
        }

        public KafkaConsumer Create(IServiceProvider serviceProvider)
        {
            var options = (IOptions<TOptions>)serviceProvider.GetService(typeof(IOptions<TOptions>));
            return options == null ? null : _consumerFactory(options.Value);
        }
    }
}
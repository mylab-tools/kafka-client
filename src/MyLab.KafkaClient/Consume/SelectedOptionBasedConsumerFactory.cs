using System;
using Microsoft.Extensions.Options;

namespace MyLab.KafkaClient.Consume
{
    class SelectedOptionBasedConsumerFactory<TOptions, TOption> : IKafkaConsumerFactory
        where TOptions : class, new()
    {
        private readonly Func<TOptions, TOption> _optionSelector;
        private readonly Func<TOption, KafkaConsumer> _consumerFactoryMethod;

        public SelectedOptionBasedConsumerFactory(
            Func<TOptions, TOption> optionSelector,
            Func<TOption, KafkaConsumer> consumerFactoryMethodMethod)
        {
            _optionSelector = optionSelector;
            _consumerFactoryMethod = consumerFactoryMethodMethod;
        }

        public KafkaConsumer Create(IServiceProvider serviceProvider)
        {
            var options = (IOptions<TOptions>)serviceProvider.GetService(typeof(IOptions<TOptions>));

            if (options == null)
                return null;

            var option = _optionSelector(options.Value);

            if (option == null)
                return null;

            return _consumerFactoryMethod(option);
        }
    }
}
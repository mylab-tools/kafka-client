using System;

namespace MyLab.KafkaClient.Consume
{
    /// <summary>
    /// Extensions for <see cref="KafkaConsumerRegistrar"/>
    /// </summary>
    public static class KafkaConsumerRegistrarExtensions
    {
        /// <summary>
        /// Registers consumer factory based on options 
        /// </summary>
        public static void Add<TOptions>(this KafkaConsumerRegistrar registrar, Func<TOptions, IKafkaConsumer> consumerFactoryProvider)
            where TOptions : class, new()
        {
            if (registrar == null) throw new ArgumentNullException(nameof(registrar));
            if (consumerFactoryProvider == null) throw new ArgumentNullException(nameof(consumerFactoryProvider));

            registrar.Add(new OptionsBasedConsumerFactory<TOptions>(consumerFactoryProvider));
        }

        /// <summary>
        /// Registers consumer factory based on options field
        /// </summary>
        public static void Add<TOptions, TOption>(this KafkaConsumerRegistrar registrar, Func<TOptions, TOption> optionSelector, Func<TOption, IKafkaConsumer> consumerFactoryProvider)
            where TOptions : class, new()
        {
            if (registrar == null) throw new ArgumentNullException(nameof(registrar));
            if (optionSelector == null) throw new ArgumentNullException(nameof(optionSelector));
            if (consumerFactoryProvider == null) throw new ArgumentNullException(nameof(consumerFactoryProvider));

            registrar.Add(new SelectedOptionBasedConsumerFactory<TOptions, TOption>(optionSelector, consumerFactoryProvider));
        }

        /// <summary>
        /// Registers consumer
        /// </summary>
        public static void Add(this KafkaConsumerRegistrar registrar, IKafkaConsumer consumer)
        {
            if (registrar == null) throw new ArgumentNullException(nameof(registrar));
            if (consumer == null) throw new ArgumentNullException(nameof(consumer));

            registrar.Add(new SingleConsumerFactory(consumer));
        }

        /// <summary>
        /// Registers consumer type
        /// </summary>
        public static void Add<TConsumer>(this KafkaConsumerRegistrar registrar)
            where TConsumer : IKafkaConsumer
        {
            if (registrar == null) throw new ArgumentNullException(nameof(registrar));

            registrar.Add(new DiConsumerFactory<TConsumer>());
        }
    }
}
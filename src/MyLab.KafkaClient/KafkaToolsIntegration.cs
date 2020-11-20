using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLab.KafkaClient.Consume;
using MyLab.KafkaClient.Produce;

namespace MyLab.KafkaClient
{
    /// <summary>
    /// Extensions for <see cref="IServiceCollection"/> to integrate Kafka tools
    /// </summary>
    public static class KafkaToolsIntegration
    {
        /// <summary>
        /// Adds Kafka producing tools into application
        /// </summary>
        public static IServiceCollection AddKafkaProducer(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.AddSingleton<IKafkaProducer, KafkaProducer>();

            return serviceCollection;
        }

        /// <summary>
        /// Adds Kafka consuming tools into application 
        /// </summary>
        public static IServiceCollection AddKafkaConsuming(this IServiceCollection serviceCollection, Action<KafkaConsumerRegistrar> consumerRegistration)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
            if (consumerRegistration == null) throw new ArgumentNullException(nameof(consumerRegistration));

            var consumerRegistrar = new KafkaConsumerRegistrar();

            consumerRegistration(consumerRegistrar);

            serviceCollection.AddSingleton<IKafkaConsumerRegistrar>(consumerRegistrar);
            serviceCollection.AddSingleton<IKafkaConsumerRegistry, KafkaConsumerRegistry>();
            serviceCollection.AddHostedService<KafkaConsumersHost>();

            return serviceCollection;
        }

        /// <summary>
        /// Configure Kafka tools
        /// </summary>
        public static IServiceCollection ConfigureKafkaTools(this IServiceCollection sc,
            IConfiguration config, string configSectionName = "Kafka")
        {
            if (sc == null) throw new ArgumentNullException(nameof(sc));
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (configSectionName == null) throw new ArgumentNullException(nameof(configSectionName));

            var configProvider = new KafkaConfigProvider(configSectionName, config);

            return ConfigureKafkaTools(sc, configProvider);
        }

        /// <summary>
        /// Configure Kafka tools
        /// </summary>
        public static IServiceCollection ConfigureKafkaTools(this IServiceCollection sc,
            KafkaConfigProvider configProvider)
        {
            if (sc == null) throw new ArgumentNullException(nameof(sc));
            if (configProvider == null) throw new ArgumentNullException(nameof(configProvider));

            sc.AddSingleton<IAdminConfigProvider>(configProvider);
            sc.AddSingleton<IConsumerConfigProvider>(configProvider);
            sc.AddSingleton<IProducerConfigProvider>(configProvider);

            return sc;
        }

        /// <summary>
        /// Configure Kafka tools
        /// </summary>
        public static IServiceCollection ConfigureKafkaTools(this IServiceCollection sc,
            IProducerConfigProvider producerConfigProvider = null, 
            IConsumerConfigProvider consumerConfigProvider = null, 
            IAdminConfigProvider adminConfigProvider = null)
        {
            if (sc == null) throw new ArgumentNullException(nameof(sc));
            if (producerConfigProvider == null && consumerConfigProvider == null && adminConfigProvider == null) 
                throw new InvalidOperationException("At least one of config providers should be specified");

            if(adminConfigProvider != null)
                sc.AddSingleton(adminConfigProvider);
            if(consumerConfigProvider != null)
                sc.AddSingleton(consumerConfigProvider);
            if(producerConfigProvider != null)
                sc.AddSingleton(producerConfigProvider);

            return sc;
        }
    }
}

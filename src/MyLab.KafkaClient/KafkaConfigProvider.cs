using System;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace MyLab.KafkaClient
{
    /// <summary>
    /// Provides Kafka configurations
    /// </summary>
    public class KafkaConfigProvider : IAdminConfigProvider, IProducerConfigProvider, IConsumerConfigProvider
    {
        private readonly string _baseSectionName;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Default section name for base properties
        /// </summary>
        public const string DefaultCommonSectionName = "";
        /// <summary>
        /// Default section name for admin properties
        /// </summary>
        public const string DefaultAdminSectionName = "Admin";
        /// <summary>
        /// Default section name for producer properties
        /// </summary>
        public const string DefaultProduceSectionName = "Produce";
        /// <summary>
        /// Default section name for consumer properties
        /// </summary>
        public const string DefaultConsumeSectionName = "Consume";

        /// <summary>
        /// Section name for base properties
        /// </summary>
        public string CommonSectionName { get; set; } = DefaultCommonSectionName;
        /// <summary>
        /// Section name for admin properties
        /// </summary>
        public string AdminSectionName { get; set; } = DefaultAdminSectionName;
        /// <summary>
        /// Section name for producer properties
        /// </summary>
        public string ProduceSectionName { get; set; } = DefaultProduceSectionName;
        /// <summary>
        /// Section name for consumer properties
        /// </summary>
        public string ConsumeSectionName { get; set; } = DefaultConsumeSectionName;

        /// <summary>
        /// Initializes a new instance of <see cref="KafkaConfigProvider"/>
        /// </summary>
        public KafkaConfigProvider(string baseSectionName, IConfiguration configuration)
        {
            _baseSectionName = baseSectionName ?? throw new ArgumentNullException(nameof(baseSectionName));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Provides admin config
        /// </summary>
        public AdminClientConfig ProvideAdminConfig()
        {
            var baseSection = _configuration.GetSection(_baseSectionName);
            var config = new AdminClientConfig();

            BindSectionToModel(baseSection, CommonSectionName, config);
            BindSectionToModel(baseSection, AdminSectionName, config);
            
            return config;
        }

        /// <summary>
        /// Provides producer config
        /// </summary>
        public ProducerConfig ProvideProducerConfig()
        {
            var baseSection = _configuration.GetSection(_baseSectionName);
            var config = new ProducerConfig();

            BindSectionToModel(baseSection, CommonSectionName, config);
            BindSectionToModel(baseSection, ProduceSectionName, config);

            return config;
        }

        /// <summary>
        /// Provides consumer config
        /// </summary>
        public ConsumerConfigEx ProvideConsumerConfig()
        {
            var baseSection = _configuration.GetSection(_baseSectionName);
            var config = new ConsumerConfigEx();

            BindSectionToModel(baseSection, CommonSectionName, config);
            BindSectionToModel(baseSection, ConsumeSectionName, config);

            return config;
        }

        void BindSectionToModel(IConfigurationSection baseSection, string subsectionName, object model)
        {
            var section = string.IsNullOrWhiteSpace(subsectionName)
                ? baseSection
                : baseSection.GetSection(subsectionName);

            section.Bind(model);
        }
    }
}

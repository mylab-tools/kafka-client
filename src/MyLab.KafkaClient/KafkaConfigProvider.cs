using System;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace MyLab.KafkaClient
{
    class KafkaConfigProvider : IAdminConfigProvider, IProducerConfigProvider, IConsumerConfigProvider
    {
        private readonly string _baseSectionName;
        private readonly IConfiguration _configuration;

        public const string DefaultCommonSectionName = "";
        public const string DefaultAdminSectionName = "Admin";
        public const string DefaultProduceSectionName = "ProduceAsync";
        public const string DefaultConsumeSectionName = "Consume";

        public string CommonSectionName { get; set; } = DefaultCommonSectionName;
        public string AdminSectionName { get; set; } = DefaultAdminSectionName;
        public string ProduceSectionName { get; set; } = DefaultProduceSectionName;
        public string ConsumeSectionName { get; set; } = DefaultConsumeSectionName;

        /// <summary>
        /// Initializes a new instance of <see cref="KafkaConfigProvider"/>
        /// </summary>
        public KafkaConfigProvider(string baseSectionName, IConfiguration configuration)
        {
            _baseSectionName = baseSectionName ?? throw new ArgumentNullException(nameof(baseSectionName));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public AdminClientConfig ProvideAdminConfig()
        {
            var baseSection = _configuration.GetSection(_baseSectionName);
            var config = new AdminClientConfig();

            BindSectionToModel(baseSection, CommonSectionName, config);
            BindSectionToModel(baseSection, AdminSectionName, config);

            return config;
        }

        public ProducerConfig ProvideProducerConfig()
        {
            var baseSection = _configuration.GetSection(_baseSectionName);
            var config = new ProducerConfig();

            BindSectionToModel(baseSection, CommonSectionName, config);
            BindSectionToModel(baseSection, ProduceSectionName, config);

            return config;
        }

        public ConsumerConfig ProvideConsumerConfig()
        {
            var baseSection = _configuration.GetSection(_baseSectionName);
            var config = new ConsumerConfig();

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

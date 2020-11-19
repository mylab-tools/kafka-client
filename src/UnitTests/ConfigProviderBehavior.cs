using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using MyLab.KafkaClient;
using Xunit;

namespace UnitTests
{
    public partial class ConfigProviderBehavior
    {
        [Fact]
        public void ShouldLoadCommonConfig()
        {
            //Arrange
            ConfigurationBuilder cb = new ConfigurationBuilder();
            AddJsonConfig(cb, CommonConfigJson);

            var config = cb.Build();

            LogConfig(config);

            var configProvider = new ConfigProvider("Kafka", config);

            //Act
            var cfg = configProvider.GetAdminConfig();

            LogConfig(cfg);

            //Assert
            Assert.Equal("foo-user", cfg.SaslUsername);
        }

        [Fact]
        public void ShouldLoadConsumerConfig()
        {
            //Arrange
            ConfigurationBuilder cb = new ConfigurationBuilder();
            AddJsonConfig(cb, ConsumeConfigJson);

            var config = cb.Build();

            LogConfig(config);

            var configProvider = new ConfigProvider("Kafka", config);

            //Act
            var cfg = configProvider.GetConsumeConfig();

            LogConfig(cfg);

            //Assert
            Assert.Equal("foo-group", cfg.GroupId);
        }

        [Fact]
        public void ShouldLoadProducerConfig()
        {
            //Arrange
            ConfigurationBuilder cb = new ConfigurationBuilder();
            AddJsonConfig(cb, ProduceConfigJson);

            var config = cb.Build();

            LogConfig(config);

            var configProvider = new ConfigProvider("Kafka", config);

            //Act
            var cfg = configProvider.GetProduceConfig();

            LogConfig(cfg);

            //Assert
            Assert.Equal(Partitioner.Murmur2, cfg.Partitioner);
        }

        [Fact]
        public void ShouldOverrideBaseConfigParameters()
        {
            //Arrange
            ConfigurationBuilder cb = new ConfigurationBuilder();
            AddJsonConfig(cb, CommonConfigJson);
            AddJsonConfig(cb, ConsumeConfigJson);

            var config = cb.Build();

            LogConfig(config);

            var configProvider = new ConfigProvider("Kafka", config);

            //Act
            var cfg = configProvider.GetConsumeConfig();

            LogConfig(cfg);

            //Assert
            Assert.Equal("bar-user", cfg.SaslUsername);
        }
    }
}

using System;
using Microsoft.Extensions.DependencyInjection;
using MyLab.KafkaClient.Consume;
using Xunit;

namespace UnitTests
{
    public class SelectedOptionBasedConsumerFactoryBehavior
    {
        [Fact]
        public void ShouldCreateConsumerIfOptionsFieldSpecified()
        {
            //Arrange
            var initialOptions = new TestOptions
            {
                Value = "foo"
            };

            var factory = new SelectedOptionBasedConsumerFactory<TestOptions, string>(
                o => o.Value,
                optVal => new TestConsumer{Value = optVal});

            var serviceProvider = InitServicesWithOptions(initialOptions);

            //Act
            var consumer = factory.Create(serviceProvider) as TestConsumer;

            //Assert
            Assert.NotNull(consumer);
            Assert.Equal("foo", consumer.Value);
        }

        [Fact]
        public void ShouldNotCreateConsumerIfOptionsFieldNotSpecified()
        {
            //Arrange
            var initialOptions = new TestOptions
            {
                Value = null
            };

            var factory = new SelectedOptionBasedConsumerFactory<TestOptions, string>(
                o => o.Value,
                optVal => new TestConsumer { Value = optVal });

            var serviceProvider = InitServicesWithOptions(initialOptions);

            //Act
            var consumer = factory.Create(serviceProvider) as TestConsumer;

            //Assert
            Assert.Null(consumer);
        }

        [Fact]
        public void ShouldNotCreateConsumerIfOptionsNotSpecified()
        {
            //Arrange
            var factory = new SelectedOptionBasedConsumerFactory<TestOptions, string>(
                o => o.Value,
                optVal => new TestConsumer { Value = optVal });

            var serviceProvider = InitServicesWithOptions(null);

            //Act
            var consumer = factory.Create(serviceProvider) as TestConsumer;

            //Assert
            Assert.Null(consumer);
        }

        IServiceProvider InitServicesWithOptions(TestOptions initialOptions)
        {
            var serviceCollection = new ServiceCollection();

            if (initialOptions != null)
                serviceCollection.Configure<TestOptions>(options => options.Value = initialOptions.Value);

            return serviceCollection.BuildServiceProvider();
        }

        class TestOptions
        {
            public string Value { get; set; }
        }

        class TestConsumer : KafkaConsumer
        {
            public string Value { get; set; }
        }
    }
}
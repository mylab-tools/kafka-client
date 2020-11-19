using System;
using System.Threading;
using System.Threading.Tasks;
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
                Topic = "foo"
            };

            var factory = new SelectedOptionBasedConsumerFactory<TestOptions, string>(
                o => o.Topic,
                optVal => new TestConsumer{ TopicName = optVal});

            var serviceProvider = InitServicesWithOptions(initialOptions);

            //Act
            var consumer = factory.Create(serviceProvider) as TestConsumer;

            //Assert
            Assert.NotNull(consumer);
            Assert.Equal("foo", consumer.TopicName);
        }

        [Fact]
        public void ShouldNotCreateConsumerIfOptionsFieldNotSpecified()
        {
            //Arrange
            var initialOptions = new TestOptions
            {
                Topic = null
            };

            var factory = new SelectedOptionBasedConsumerFactory<TestOptions, string>(
                o => o.Topic,
                optVal => new TestConsumer { TopicName = optVal });

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
                o => o.Topic,
                optVal => new TestConsumer { TopicName = optVal });

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
                serviceCollection.Configure<TestOptions>(options => options.Topic = initialOptions.Topic);

            return serviceCollection.BuildServiceProvider();
        }

        class TestOptions
        {
            public string Topic { get; set; }
        }

        class TestConsumer : IKafkaConsumer
        {
            public string TopicName { get; set; }
            public Task ConsumeAsync(IConsumingContext ctx, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
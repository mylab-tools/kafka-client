using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MyLab.KafkaClient.Consume;
using Xunit;

namespace UnitTests
{
    public class KafkaConsumerRegistryBehavior
    {
        [Fact]
        public void ShouldProvideRegisteredConsumer()
        {
            //Arrange
            var initialConsumer = new TestConsumer();

            var registrar = new KafkaConsumerRegistrar();
            registrar.Add(initialConsumer);

            var cRegistry = InitRegistry(registrar);
            
            //Act
            var actualConsumer = cRegistry.ProvideConsumers().FirstOrDefault();

            //Assert
            Assert.Equal(initialConsumer, actualConsumer);
        }

        [Fact]
        public void ShouldNotProvideOptionsDependentConsumerIfNotDefined()
        {
            //Arrange
            var registrar = new KafkaConsumerRegistrar();
            registrar.Add<TestOptions>(
                opt => new TestConsumer
                {
                    TopicName = opt.Topic
                });

            var cRegistry = InitRegistry(registrar);

            //Act
            var actualConsumer = cRegistry.ProvideConsumers().FirstOrDefault();

            //Assert
            Assert.Null(actualConsumer);
        }

        [Fact]
        public void ShouldProvideOptionsDependentConsumerIfDefined()
        {
            //Arrange
            var registrar = new KafkaConsumerRegistrar();
            registrar.Add<TestOptions>(
                opt => new TestConsumer
                {
                    TopicName = opt.Topic
                });

            var cRegistry = InitRegistry(registrar, new TestOptions{Topic = "foo"});

            //Act
            var actualConsumer = cRegistry.ProvideConsumers().FirstOrDefault() as TestConsumer;

            //Assert
            Assert.NotNull(actualConsumer);
            Assert.Equal("foo", actualConsumer.TopicName);
        }

        [Fact]
        public void ShouldNotProvideOptionsFieldDependentConsumerIfOptionsFieldNotDefined()
        {
            //Arrange
            var registrar = new KafkaConsumerRegistrar();
            registrar.Add<TestOptions, string>(
                opts => opts.Topic,
                opt => new TestConsumer
                {
                    TopicName = opt
                });

            var cRegistry = InitRegistry(registrar, new TestOptions{ Topic = null});

            //Act
            var actualConsumer = cRegistry.ProvideConsumers().FirstOrDefault();

            //Assert
            Assert.Null(actualConsumer);
        }

        [Fact]
        public void ShouldNotProvideOptionsFieldDependentConsumerIfOptionsNotDefined()
        {
            //Arrange
            var registrar = new KafkaConsumerRegistrar();
            registrar.Add<TestOptions, string>(
                opts => opts.Topic,
                opt => new TestConsumer
                {
                    TopicName = opt
                });

            var cRegistry = InitRegistry(registrar);

            //Act
            var actualConsumer = cRegistry.ProvideConsumers().FirstOrDefault();

            //Assert
            Assert.Null(actualConsumer);
        }

        [Fact]
        public void ShouldProvideOptionsFieldDependentConsumerIfDefined()
        {
            //Arrange
            var registrar = new KafkaConsumerRegistrar();
            registrar.Add<TestOptions, string>(
                opts => opts.Topic,
                opt => new TestConsumer
                {
                    TopicName = opt
                });

            var cRegistry = InitRegistry(registrar, new TestOptions() { Topic = "foo" });

            //Act
            var actualConsumer = cRegistry.ProvideConsumers().FirstOrDefault() as TestConsumer;

            //Assert
            Assert.NotNull(actualConsumer);
            Assert.Equal("foo", actualConsumer.TopicName);
        }

        IKafkaConsumerRegistry InitRegistry(KafkaConsumerRegistrar registrar, TestOptions options = null)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IKafkaConsumerRegistry, KafkaConsumerRegistry>();
            serviceCollection.AddSingleton<IKafkaConsumerRegistrar>(registrar);

            if (options != null)
                serviceCollection.Configure<TestOptions>(opt => opt.Topic = options.Topic);

            var sp = serviceCollection.BuildServiceProvider();

            return (IKafkaConsumerRegistry)sp.GetService(typeof(IKafkaConsumerRegistry));
        }

        class TestConsumer : IKafkaConsumer
        {
            public string TopicName { get; set; }
            public Task ConsumeAsync(IConsumingContext ctx, CancellationToken cancellationToken)
            {
                throw new System.NotImplementedException();
            }
        }

        class TestOptions
        {
            public string Topic { get; set; }
        }
    }
}

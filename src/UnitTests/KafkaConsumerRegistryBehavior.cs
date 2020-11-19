using System.Linq;
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
                    Value = opt.Value
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
                    Value = opt.Value
                });

            var cRegistry = InitRegistry(registrar, new TestOptions{Value = "foo"});

            //Act
            var actualConsumer = cRegistry.ProvideConsumers().FirstOrDefault() as TestConsumer;

            //Assert
            Assert.NotNull(actualConsumer);
            Assert.Equal("foo", actualConsumer.Value);
        }

        [Fact]
        public void ShouldNotProvideOptionsFieldDependentConsumerIfOptionsFieldNotDefined()
        {
            //Arrange
            var registrar = new KafkaConsumerRegistrar();
            registrar.Add<TestOptions, string>(
                opts => opts.Value,
                opt => new TestConsumer
                {
                    Value = opt
                });

            var cRegistry = InitRegistry(registrar, new TestOptions{ Value = null});

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
                opts => opts.Value,
                opt => new TestConsumer
                {
                    Value = opt
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
                opts => opts.Value,
                opt => new TestConsumer
                {
                    Value = opt
                });

            var cRegistry = InitRegistry(registrar, new TestOptions() { Value = "foo" });

            //Act
            var actualConsumer = cRegistry.ProvideConsumers().FirstOrDefault() as TestConsumer;

            //Assert
            Assert.NotNull(actualConsumer);
            Assert.Equal("foo", actualConsumer.Value);
        }

        IKafkaConsumerRegistry InitRegistry(KafkaConsumerRegistrar registrar, TestOptions options = null)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IKafkaConsumerRegistry, KafkaConsumerRegistry>();
            serviceCollection.AddSingleton<IKafkaConsumerRegistrar>(registrar);

            if (options != null)
                serviceCollection.Configure<TestOptions>(opt => opt.Value = options.Value);

            var sp = serviceCollection.BuildServiceProvider();

            return (IKafkaConsumerRegistry)sp.GetService(typeof(IKafkaConsumerRegistry));
        }

        class TestConsumer : KafkaConsumer
        {
            public string Value { get; set; }
        }

        class TestOptions
        {
            public string Value { get; set; }
        }
    }
}

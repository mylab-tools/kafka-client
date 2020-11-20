using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MyLab.KafkaClient;
using MyLab.KafkaClient.Consume;
using MyLab.KafkaClient.Test;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class ConsumingManagerBehavior : IClassFixture<KafkaFixture>
    {
        private readonly ITestOutputHelper _output;
        private readonly KafkaTopicFactory _tFactory;
        private readonly IKafkaLog _kafkaLog;

        public ConsumingManagerBehavior(KafkaFixture fxt, ITestOutputHelper output)
        {
            _output = output;
            _kafkaLog = TestStuff.CreateKafkaLog(output);
            _tFactory = fxt.CreateTopicFactory(TestStuff.Config, nameof(ConsumingManagerBehavior) + "_", _kafkaLog);
        }

        [Fact]
        public async Task ShouldConsumeMessage()
        {
            //Arrange
            var testValueHolder = new TestResultHolder();
            var topic = await _tFactory.CreateWithRandomIdAsync();

            var cMgr = CreateConsumerManager(testValueHolder);

            var logicConsumer = new KafkaConsumer<TestConsumerLogic, string>(topic.Name);

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));

            await topic.ProduceAsync("foo");

            //Act

            await cMgr.ConsumeLoopAsync(new[] {logicConsumer}, cancellationTokenSource.Token);

            //Assert
            Assert.Equal("foo", testValueHolder.Values.FirstOrDefault());
        }

        [Fact]
        public async Task ShouldConsumeFromSeveralTopics()
        {
            //Arrange
            var testValueHolder = new TestResultHolder();

            var topic1 = await _tFactory.CreateWithRandomIdAsync();
            var topic2 = await _tFactory.CreateWithRandomIdAsync();

            var cMgr = CreateConsumerManager(testValueHolder);

            var logicConsumer1 = new KafkaConsumer<TestConsumerLogic, string>(topic1.Name);
            var logicConsumer2 = new KafkaConsumer<TestConsumerLogic, string>(topic2.Name);

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));

            await topic1.ProduceAsync("foo");
            await topic2.ProduceAsync("bar");

            //Act

            await cMgr.ConsumeLoopAsync(new[] { logicConsumer1, logicConsumer2 }, cancellationTokenSource.Token);

            //Assert
            Assert.Equal(2, testValueHolder.Values.Count);
            Assert.Contains("foo", testValueHolder.Values);
            Assert.Contains("bar", testValueHolder.Values);
        }

        ConsumingManager CreateConsumerManager(TestResultHolder testValueHolder)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(testValueHolder);
            serviceCollection.AddLogging(lb => lb.AddXUnit(_output));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var consumerConfig = new ConsumerConfig(TestStuff.Config)
            {
                GroupId = Guid.NewGuid().ToString("N"),
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var logger = (ILogger<ConsumingManager>)serviceProvider.GetService(typeof(ILogger<ConsumingManager>));

            var configProviderMock = new Mock<IConsumerConfigProvider>();
            configProviderMock
                .Setup(p => p.ProvideConsumerConfig())
                .Returns(new ConsumerConfig(consumerConfig));

            return new ConsumingManager(serviceProvider, configProviderMock.Object, _kafkaLog, logger);
        }

        class TestConsumerLogic : IKafkaConsumerLogic<string>
        {
            private readonly TestResultHolder _resultHolder;

            public TestConsumerLogic(TestResultHolder resultHolder)
            {
                _resultHolder = resultHolder;
            }

            public Task ConsumeAsync(IncomingKafkaEvent<string> incomingKafkaEvent)
            {
                _resultHolder.Values.Add(incomingKafkaEvent.Content);

                return Task.CompletedTask;
            }
        }

        class TestResultHolder
        {
            public List<string> Values { get; } = new List<string>();
        }
    }
}

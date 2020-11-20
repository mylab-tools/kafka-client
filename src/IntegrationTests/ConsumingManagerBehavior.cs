using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(testValueHolder);
            serviceCollection.AddLogging(lb => lb.AddXUnit(_output));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var topic = await _tFactory.CreateWithRandomIdAsync();

            var consumerConfig = new ConsumerConfig(TestStuff.Config)
            {
                GroupId = Guid.NewGuid().ToString("N"),
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            
            var nativeConsumer = new ConsumerBuilder<string,string>(consumerConfig).Build();
            var logger = (ILogger<ConsumingManager>)serviceProvider.GetService(typeof(ILogger<ConsumingManager>));

            var cMgr = new ConsumingManager(serviceProvider, logger)
            {
                KafkaLog = _kafkaLog
            };

            var logicConsumer = new KafkaConsumer<TestConsumerLogic, string>(topic.Name);

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));

            await topic.ProduceAsync("foo");

            //Act

            await cMgr.ConsumeLoopAsync(nativeConsumer, new[] {logicConsumer}, cancellationTokenSource.Token);

            //Assert
            Assert.Equal("foo", testValueHolder.Values.FirstOrDefault());
        }

        [Fact]
        public async Task ShouldConsumeFromSeveralTopics()
        {
            //Arrange
            var testValueHolder = new TestResultHolder();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(testValueHolder);
            serviceCollection.AddLogging(lb => lb.AddXUnit(_output));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var topic1 = await _tFactory.CreateWithRandomIdAsync();
            var topic2 = await _tFactory.CreateWithRandomIdAsync();

            var consumerConfig = new ConsumerConfig(TestStuff.Config)
            {
                GroupId = Guid.NewGuid().ToString("N"),
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var nativeConsumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            var logger = (ILogger<ConsumingManager>)serviceProvider.GetService(typeof(ILogger<ConsumingManager>));

            var cMgr = new ConsumingManager(serviceProvider, logger)
            {
                KafkaLog = _kafkaLog
            };

            var logicConsumer1 = new KafkaConsumer<TestConsumerLogic, string>(topic1.Name);
            var logicConsumer2 = new KafkaConsumer<TestConsumerLogic, string>(topic2.Name);

            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));

            await topic1.ProduceAsync("foo");
            await topic2.ProduceAsync("bar");

            //Act

            await cMgr.ConsumeLoopAsync(nativeConsumer, new[] { logicConsumer1, logicConsumer2 }, cancellationTokenSource.Token);

            //Assert
            Assert.Equal(2, testValueHolder.Values.Count);
            Assert.Contains("foo", testValueHolder.Values);
            Assert.Contains("bar", testValueHolder.Values);
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

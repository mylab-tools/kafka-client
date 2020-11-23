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

            
            var cancellationTokenSource = CreateCancellationTokenSource();
            var testInterrupter = new TestInterrupter(cancellationTokenSource);

            var cMgr = CreateConsumerManager(testValueHolder, testInterrupter);

            var logicConsumer = new KafkaConsumer<TestConsumerLogic, string>(topic.Name);

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

            var cancellationTokenSource = CreateCancellationTokenSource();
            var testInterrupter = new TestInterrupter(cancellationTokenSource);
            var cMgr = CreateConsumerManager(testValueHolder, testInterrupter);

            var consumerLogic = new TestConsumerLogic(testValueHolder, testInterrupter)
            {
                ExpectedMsgNumber = 2
            };

            var logicConsumer1 = new KafkaConsumer<TestConsumerLogic, string>(topic1.Name, consumerLogic);
            var logicConsumer2 = new KafkaConsumer<TestConsumerLogic, string>(topic2.Name, consumerLogic);

            await topic1.ProduceAsync("foo");
            await topic2.ProduceAsync("bar");

            //Act

            await cMgr.ConsumeLoopAsync(new[] { logicConsumer1, logicConsumer2 }, cancellationTokenSource.Token);

            //Assert
            Assert.Equal(2, testValueHolder.Values.Count);
            Assert.Contains("foo", testValueHolder.Values);
            Assert.Contains("bar", testValueHolder.Values);
        }

        [Fact]
        public async Task ShouldRetryIfUnhandledError()
        {
            var testValueHolder = new TestResultHolder();
            var topic = await _tFactory.CreateWithRandomIdAsync();

            var cancellationTokenSource = CreateCancellationTokenSource();
            var testInterrupter = new TestInterrupter(cancellationTokenSource);
            var cMgr = CreateConsumerManager(testValueHolder, testInterrupter);

            var logicConsumer = new KafkaConsumer<TestConsumerWithSingleRetryLogic, string>(topic.Name);
            
            await topic.ProduceAsync("foo");

            //Act

            await cMgr.ConsumeLoopAsync(new[] { logicConsumer }, cancellationTokenSource.Token);

            //Assert
            Assert.Equal("foo", testValueHolder.Values.FirstOrDefault());
        }

        private const int MaxTestTimeoutS = 10;

        CancellationTokenSource CreateCancellationTokenSource()
        {
            return new CancellationTokenSource(TimeSpan.FromSeconds(MaxTestTimeoutS));
        }

        ConsumingManager CreateConsumerManager(TestResultHolder testValueHolder, TestInterrupter testInterrupter)
        {
            var serviceCollection = new ServiceCollection();

            var configProviderMock = new Mock<IConsumerConfigProvider>();

            var consumerConfig = new ConsumerConfigEx(TestStuff.Config)
            {
                GroupId = Guid.NewGuid().ToString("N"),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                ErrorBasedRetryIntervalMs = 100
            };

            configProviderMock
                .Setup(p => p.ProvideConsumerConfig())
                .Returns(new ConsumerConfigEx(consumerConfig));

            serviceCollection.AddSingleton(testValueHolder)
                .AddLogging(lb => lb.AddXUnit(_output))
                .AddSingleton(_kafkaLog)
                .AddSingleton(configProviderMock.Object)
                .AddSingleton(testInterrupter)
                .AddSingleton<ConsumingManager>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider.GetService<ConsumingManager>();
        }

        class TestInterrupter
        {
            private readonly CancellationTokenSource _cancellationTokenSource;

            public TestInterrupter(CancellationTokenSource cancellationTokenSource)
            {
                _cancellationTokenSource = cancellationTokenSource;
            }

            public void Cancel()
            {
                _cancellationTokenSource.Cancel();
            }
        }

        class TestConsumerLogic : IKafkaConsumerLogic<string>
        {
            private readonly TestResultHolder _resultHolder;
            private readonly TestInterrupter _testInterrupter;
            private int _msgCount = 0;

            public int ExpectedMsgNumber { get; set; } = 1;

            public TestConsumerLogic(TestResultHolder resultHolder, TestInterrupter testInterrupter)
            {
                _resultHolder = resultHolder;
                _testInterrupter = testInterrupter;
            }

            public Task ConsumeAsync(IncomingKafkaEvent<string> incomingKafkaEvent)
            {
                _resultHolder.Values.Add(incomingKafkaEvent.Content);

                _msgCount++;

                if(_msgCount >= ExpectedMsgNumber)
                    _testInterrupter.Cancel();
                
                return Task.CompletedTask;
            }
        }

        class TestConsumerWithSingleRetryLogic : IKafkaConsumerLogic<string>
        {
            private readonly TestResultHolder _resultHolder;
            private readonly TestInterrupter _testInterrupter;

            public TestConsumerWithSingleRetryLogic(TestResultHolder resultHolder, TestInterrupter testInterrupter)
            {
                _resultHolder = resultHolder;
                _testInterrupter = testInterrupter;
            }

            public Task ConsumeAsync(IncomingKafkaEvent<string> incomingKafkaEvent)
            {
                if (!_resultHolder.HasError)
                {
                    _resultHolder.HasError = true;
                    throw new Exception();
                }

                _resultHolder.Values.Add(incomingKafkaEvent.Content);
                _testInterrupter.Cancel();

                return Task.CompletedTask;
            }
        }

        class TestResultHolder
        {
            public bool HasError { get; set; }

            public List<string> Values { get; } = new List<string>();
        }
    }
}

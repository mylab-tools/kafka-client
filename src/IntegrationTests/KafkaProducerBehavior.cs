using System.Threading.Tasks;
using Confluent.Kafka;
using Moq;
using MyLab.KafkaClient;
using MyLab.KafkaClient.Produce;
using MyLab.KafkaClient.Test;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class KafkaProducerBehavior : IClassFixture<KafkaFixture>
    {
        private readonly KafkaTopicFactory _tFactory;
        private readonly IKafkaLog _log;

        public KafkaProducerBehavior(KafkaFixture fxt, ITestOutputHelper output)
        {
            _log = TestStuff.CreateKafkaLog(output);
            _tFactory = fxt.CreateTopicFactory(TestStuff.Config, nameof(KafkaProducerBehavior)+"_", _log);
        }

        [Fact]
        public async Task ShouldProduceEvent()
        {
            //Arrange
            var topic = await _tFactory.CreateWithRandomIdAsync();
            var produceTarget = new TopicPartition(topic.Name, Partition.Any);

            var configProviderMock = new  Mock<IProducerConfigProvider>();
            configProviderMock
                .Setup(p => p.ProvideProducerConfig())
                .Returns(new ProducerConfig(TestStuff.Config));
            
            var producer = new KafkaProducer(configProviderMock.Object, _log);

            //Act
            await producer.ProduceAsync(new OutgoingKafkaEvent("foo"){Target = produceTarget });

            var incoming = topic.ConsumeOne();

            //Assert
            Assert.Equal("foo", incoming);
        }
    }
}

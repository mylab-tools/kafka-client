using System;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using MyLab.KafkaClient.Test;
using Xunit;

namespace IntegrationTests
{
    public class KafkaTopicBehavior
    {
        [Fact]
        public void ShouldCheckIfNotExists()
        {
            //Arrange
            using var adminClient = new AdminClientBuilder(TestStuff.Config).Build();
            var topic = new KafkaTopic(Guid.NewGuid().ToString("N"), adminClient, TestStuff.Config);

            //Act
            var exists = topic.Exists();

            //Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldCheckIfExists()
        {
            //Arrange
            using var adminClient = new AdminClientBuilder(TestStuff.Config).Build();
            var topicName = Guid.NewGuid().ToString("N");
            var topic = new KafkaTopic(topicName, adminClient, TestStuff.Config);
            bool exists = false;

            try
            {
                await adminClient.CreateTopicsAsync(
                    Enumerable.Repeat(
                        new TopicSpecification
                        {
                            Name = topicName,
                            NumPartitions = 1
                        }, 1));
                await Task.Delay(500);
                //Act
                exists = topic.Exists();
            }
            finally
            {
                await adminClient.DeleteTopicsAsync(Enumerable.Repeat(topicName, 1));
            }

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ShouldRemoveWhenDispose()
        {
            //Arrange
            using var adminClient = new AdminClientBuilder(TestStuff.Config).Build();
            var topicName = Guid.NewGuid().ToString("N");
            var topic = new KafkaTopic(topicName, adminClient, TestStuff.Config);
            bool exists = false;

            await adminClient.CreateTopicsAsync(
                Enumerable.Repeat(
                    new TopicSpecification
                    {
                        Name = topicName,
                        NumPartitions = 1
                    }, 1));
            await Task.Delay(500);
            //Act

            await topic.DisposeAsync();

            exists = topic.Exists();

            //Assert
            Assert.False(exists);
        }
    }
}

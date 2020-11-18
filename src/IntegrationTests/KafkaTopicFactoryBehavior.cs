using System;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using MyLab.KafkaClient.Test;
using Xunit;

namespace IntegrationTests
{
    public class KafkaTopicFactoryBehavior
    {
        [Fact]
        public async Task ShouldCreateTopic()
        {
            //Arrange
            using var adminClient = new AdminClientBuilder(TestStuff.Config).Build();
            var factory = new KafkaTopicFactory(adminClient, TestStuff.Config);
            var topicName = Guid.NewGuid().ToString("N");
            KafkaTopic topic = null;
            bool exists;

            //Act
            try
            {
                topic = await factory.CreateWithNameAsync(topicName);
                await Task.Delay(500);
                exists = topic.Exists();
            }
            finally
            {
                if(topic != null)
                    await topic.DisposeAsync();
            }

            //Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ShouldDisposeTopicsWhenDisposed()
        {
            //Arrange
            var adminClient = new AdminClientBuilder(TestStuff.Config).Build();
            var factory = new KafkaTopicFactory(adminClient, TestStuff.Config);
            var topicName = Guid.NewGuid().ToString("N");
            KafkaTopic topic = null;
            bool hasError = false;
            bool exists = false;

            //Act
            try
            {
                topic = await factory.CreateWithNameAsync(topicName);
                await Task.Delay(500);

                await factory.DisposeAsync();

                using var adminClient2 = new AdminClientBuilder(TestStuff.Config).Build();
                var metadata = adminClient2.GetMetadata(topicName, TimeSpan.FromSeconds(1));
                var foundTopicRecord = metadata?.Topics?.FirstOrDefault(t => t.Topic == topicName);

                exists = foundTopicRecord?.Partitions != null && foundTopicRecord.Partitions.Count > 0;

            }
            catch
            {
                hasError = true;
                if (topic != null)
                    await topic.DisposeAsync();
            }

            //Assert
            Assert.False(hasError);
            Assert.False(exists);
        }

        [Fact]
        public async Task ShouldDisposeAdminClientWhenDisposed()
        {
            //Arrange
            var adminClient = new AdminClientBuilder(TestStuff.Config).Build();
            var factory = new KafkaTopicFactory(adminClient, TestStuff.Config);

            //Act
            await factory.DisposeAsync();

            //Assert
            Assert.Throws<ObjectDisposedException>(() => adminClient.GetMetadata(TimeSpan.FromSeconds(1)));
        }
    }
}

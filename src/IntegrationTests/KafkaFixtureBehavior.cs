using System;
using System.Threading.Tasks;
using MyLab.KafkaClient.Test;
using Xunit;

namespace IntegrationTests
{
    public class KafkaFixtureBehavior
    {
        [Fact]
        public async Task ShouldDisposeFactoriesWhenDisposed()
        {
            //Arrange
            var fixture = new KafkaFixture();
            var factory = fixture.CreateTopicFactory(TestStuff.Config);

            //Act
            fixture.Dispose();

            //Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => factory.CreateWithRandomIdAsync());
        }
    }
}

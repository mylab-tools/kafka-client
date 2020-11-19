using System;
using System.Text;
using Confluent.Kafka;
using MyLab.KafkaClient;
using MyLab.KafkaClient.Produce;
using Newtonsoft.Json;
using Xunit;

namespace UnitTests
{
    public class OutgoingKafkaEventBehavior
    {
        [Fact]
        public void ShouldProvideSpecifiedTopicAndPartition()
        {
            //Arrange
            var e = new OutgoingKafkaEvent(new object())
            {
                Target = new TopicPartition("foo", new Partition(10))
            };

            //Act
            var target = e.GetTarget();

            //Assert
            Assert.Equal("foo", target.Topic);
            Assert.Equal(10, target.Partition.Value);
        }

        [Fact]
        public void ShouldProvideTopicFromModelByDefault()
        {
            //Arrange
            var e = new OutgoingKafkaEvent(new EventModel());

            //Act
            var target = e.GetTarget();

            //Assert
            Assert.Equal("foo", target.Topic);
            Assert.Equal(Partition.Any, target.Partition);
        }

        [Fact]
        public void ShouldNotPassWhenCantDetermineTopicName()
        {
            //Arrange
            var e = new OutgoingKafkaEvent(new object());

            //Act & Assert
            Assert.Throws<InvalidOperationException>(() => e.GetTarget());
        }

        [Fact]
        public void ShouldNotPassWhenEventContentNotSpecified()
        {
            //Act & Assert
            Assert.Throws<ArgumentNullException>(() => new OutgoingKafkaEvent(null));
        }

        [Fact]
        public void ShouldProvideModelKey()
        {
            //Arrange
            var e = new OutgoingKafkaEvent(
                new EventModel
            {
                Key = "foo"
            });

            //Act
            var nMsg = e.ToNativeMessage();

            //Assert
            Assert.Equal("foo", nMsg.Key);
        }

        [Fact]
        public void ShouldProvideSpecifiedKeyInsteadModel()
        {
            //Arrange
            var e = new OutgoingKafkaEvent(
                new EventModel
                {
                    Key = "foo"
                })
            {
                Key = "bar"
            };

            //Act
            var nMsg = e.ToNativeMessage();

            //Assert
            Assert.Equal("bar", nMsg.Key);
        }

        [Fact]
        public void ShouldProvideNullKeyIfNowhereSpecified()
        {
            //Arrange
            var e = new OutgoingKafkaEvent(
                new EventModel
                {
                    Key = null
                })
            {
                Key = null
            };

            //Act
            var nMsg = e.ToNativeMessage();

            //Assert
            Assert.Null(nMsg.Key);
        }

        [Fact]
        public void ShouldProvideModelHeaders()
        {
            //Arrange
            var e = new OutgoingKafkaEvent(new EventModel
            {
                HeaderKey = "foo",
                HeaderValue = "bar"
            });

            //Act
            var nMsg = e.ToNativeMessage();

            var strHeaderVal = Encoding.UTF8.GetString(nMsg.Headers.GetLastBytes("foo"));
            var headerVal = JsonConvert.DeserializeObject<string>(strHeaderVal);

            //Assert
            Assert.Equal("bar", headerVal);
        }

        [Fact]
        public void ShouldProvideModelHeadersWithSpecifiedHeaders()
        {
            //Arrange

            var specifiedHeaders = new Headers
            {
                { "baz", "quux" }
            };
            var e = new OutgoingKafkaEvent(new EventModel
            {
                HeaderKey = "foo",
                HeaderValue = "bar"
            })
            {
                Headers = specifiedHeaders
            };

            //Act
            var nMsg = e.ToNativeMessage();

            var fooStrHeaderVal = Encoding.UTF8.GetString(nMsg.Headers.GetLastBytes("foo"));
            var fooHeaderVal = JsonConvert.DeserializeObject<string>(fooStrHeaderVal);

            var bazStrHeaderVal = Encoding.UTF8.GetString(nMsg.Headers.GetLastBytes("baz"));
            var bazHeaderVal = JsonConvert.DeserializeObject<string>(bazStrHeaderVal);

            //Assert
            Assert.Equal("bar", fooHeaderVal);
            Assert.Equal("quux", bazHeaderVal);
        }

        [KafkaTopic("foo")]
        class EventModel : IKafkaEventKayProvider, IKafkaEventHeadersProvider
        {
            public string Key { get; set; }
            public string HeaderKey { get; set; }
            public object HeaderValue { get; set; }

            public string ProvideKey()
            {
                return Key;
            }

            public void ProvideHeaders(Headers headers)
            {
                if(HeaderKey != null)
                    headers.Add(HeaderKey, HeaderValue);
            }
        }
    }
}
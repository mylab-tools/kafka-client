using Confluent.Kafka;
using MyLab.KafkaClient;
using Xunit.Abstractions;

namespace IntegrationTests
{
    static class TestStuff
    {
        public static ClientConfig Config = new ClientConfig
        {
            BootstrapServers = "localhost:1192"
        };

        public static IKafkaLog CreateKafkaLog(ITestOutputHelper output)
        {
            return new StringKafkaLog(new TestLogWriter(output));
        }
    }
}
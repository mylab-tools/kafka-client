using Confluent.Kafka;

namespace IntegrationTests
{
    static class TestStuff
    {
        public static ClientConfig Config = new ClientConfig
        {
            BootstrapServers = "localhost:1192"
        };
    }
}
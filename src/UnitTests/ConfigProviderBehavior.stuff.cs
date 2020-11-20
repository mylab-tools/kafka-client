using System.IO;
using System.Linq;
using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace UnitTests
{
    public partial class ConfigProviderBehavior
    {
        private readonly ITestOutputHelper _output;
        private const string CommonConfigJson = "{\"Kafka\":{\"SaslUsername\":\"foo-user\"}}";

        private const string ConsumeConfigJson =
            "{\"Kafka\":{\"Consume\":{\"SaslUsername\":\"bar-user\", \"GroupId\":\"foo-group\"}}}";

        private const string ProduceConfigJson = "{\"Kafka\":{\"ProduceAsync\":{\"Partitioner\":\"MurMur2\"}}}";

        /// <summary>
        /// Initializes a new instance of <see cref="ConfigProviderBehavior"/>
        /// </summary>
        public ConfigProviderBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        private void AddJsonConfig(ConfigurationBuilder cb, string json)
        {
            cb.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json)));
        }

        private void LogConfig(IConfigurationRoot config)
        {
            _output.WriteLine("");
            _output.WriteLine("Config:");

            var keys = config.AsEnumerable().ToArray();

            if (keys.Length == 0)
            {
                _output.WriteLine("\t[empty]");
            }
            else
            {
                foreach (var key in keys)
                {
                    _output.WriteLine($"\t{key.Key}: {key.Value}");
                }
            }
        }

        private void LogConfig(ClientConfig configObj)
        {
            var str = JsonConvert.SerializeObject(configObj, Formatting.Indented);

            _output.WriteLine("");
            _output.WriteLine("Config object:");
            _output.WriteLine(str);
        }
    }
}
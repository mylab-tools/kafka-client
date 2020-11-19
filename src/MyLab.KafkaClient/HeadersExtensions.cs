using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace MyLab.KafkaClient
{
    /// <summary>
    /// Extensions for <see cref="Headers"/>
    /// </summary>
    public static class HeadersExtensions
    {
        /// <summary>
        /// Append a new header to the collection
        /// </summary>
        public static void Add(this Headers headers, string key, object value)
        {
            var strVal = JsonConvert.SerializeObject(value);

            headers.Add(key, Encoding.UTF8.GetBytes(strVal));
        }

        /// <summary>
        /// Append a new header to the collection
        /// </summary>
        public static void Add(this Headers headers, string key, string value)
        {
            headers.Add(key, Encoding.UTF8.GetBytes(value));
        }
    }
}

using System;
using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace MyLab.KafkaClient
{
    /// <summary>
    /// Extensions for <see cref="Header"/>
    /// </summary>
    public static class HeaderExtensions
    {
        /// <summary>
        /// Converts header value to string
        /// </summary>
        public static string ToStringValue(this Header header)
        {
            if (header == null) throw new ArgumentNullException(nameof(header));
            return Encoding.UTF8.GetString(header.GetValueBytes());
        }

        /// <summary>
        /// Converts header value to object
        /// </summary>
        public static T ToObject<T>(this Header header)
        {
            var str = Encoding.UTF8.GetString(header.GetValueBytes());
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
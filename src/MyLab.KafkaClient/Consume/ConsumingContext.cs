using System;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace MyLab.KafkaClient.Consume
{
    class ConsumingContext: IConsumingContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConsumeResult<string, string> _consumeResult;

        /// <summary>
        /// Initializes a new instance of <see cref="ConsumingContext"/>
        /// </summary>
        public ConsumingContext(IServiceProvider serviceProvider, ConsumeResult<string,string> consumeResult)
        {
            _serviceProvider = serviceProvider;
            _consumeResult = consumeResult;
        }

        public IKafkaConsumerLogic CreateLogic<T>() 
            where T : IKafkaConsumerLogic
        {
            return ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        }

        public IncomingKafkaEvent<TContent> ProvideEvent<TContent>()
        {
            return new IncomingKafkaEvent<TContent>(_consumeResult);
        }
    }
}
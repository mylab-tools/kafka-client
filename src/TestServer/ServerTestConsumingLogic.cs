using System.Threading.Tasks;
using MyLab.KafkaClient.Consume;

namespace TestServer
{
    public class ServerTestConsumingLogic : IKafkaConsumerLogic<string>
    {
        public Task ConsumeAsync(IncomingKafkaEvent<string> incomingKafkaEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}
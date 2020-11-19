using MyLab.KafkaClient;
using Xunit.Abstractions;

namespace IntegrationTests
{
    class TestLogWriter : IKafkaLogWriter
    {
        private readonly ITestOutputHelper _output;

        public TestLogWriter(ITestOutputHelper output)
        {
            _output = output;
        }
        public void WriteLine(string str)
        {
            _output.WriteLine(str);
        }
    }
}
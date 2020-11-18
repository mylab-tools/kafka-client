//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using Confluent.Kafka;
//using Confluent.Kafka.Admin;
//using Xunit;
//using Xunit.Abstractions;

//namespace IntegrationTests
//{
//    public class UnitTest1
//    {
//        private readonly ITestOutputHelper _output;

//        public UnitTest1(ITestOutputHelper output)
//        {
//            _output = output;
//        }

//        [Fact]
//        public async Task Test1()
//        {
//            var config = new ProducerConfig { BootstrapServers = "localhost:1192"};
//            //var config = new ProducerConfig { BootstrapServers = "dlg-kafka-dev-01.notariat.corp:9092,dlg-kafka-dev-02.notariat.corp:9092,dlg-kafka-dev-03.notariat.corp:9092" };

//            // If serializers are not specified, default serializers from
//            // `Confluent.Kafka.Serializers` will be automatically used where
//            // available. Note: by default strings are encoded as UTF8.
//            using (var p = new ProducerBuilder<Null, string>(config).Build())
//            {
//                try
//                {
//                    //var dr = await p.ProduceAsync(new TopicPartition("dev-mark-1", Partition.Any), new Message<Null, string> { Value = "test" });\\

//                    for (int i = 0; i < 10; i++)
//                    {
//                        var dr = await p.ProduceAsync(new TopicPartition("test-topic", Partition.Any), new Message<Null, string> { Value = "test" + i });
//                        _output.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
//                    }
//                }
//                catch (ProduceException<Null, string> e)
//                {
//                    _output.WriteLine(e.ToString());
//                }
//            }
//        }

//        [Fact]
//        public async Task ShouldNAME()
//        {
//            var conf = new ConsumerConfig
//            {
                
//                GroupId = "test-consumer-group3",
//                BootstrapServers = "localhost:1192",
//                // Note: The AutoOffsetReset property determines the start offset in the event
//                // there are not yet any committed offsets for the consumer group for the
//                // topic/partitions of interest. By default, offsets are committed
//                // automatically, so in this example, consumption will only start from the
//                // earliest message in the topic 'my-topic' the first time you run the program.
//                AutoOffsetReset = AutoOffsetReset.Earliest,
//                EnableAutoCommit = false
//            };

//            using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
//            {
//                c.Subscribe("test-topic");

//                CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));



//                try
//                {
//                    while (true)
//                    {
//                        try
//                        {
//                            var cr = c.Consume(cts.Token);
//                            _output.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
//                        }
//                        catch (ConsumeException e)
//                        {
//                            _output.WriteLine($"Error occured: {e.Error.Reason}");
//                        }
//                    }
//                }
//                catch (OperationCanceledException e)
//                {
//                    _output.WriteLine(e.ToString());
//                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
//                    c.Close();
//                }
//            }
//        }

//        [Fact]
//        public void ShouldNAME2()
//        {
//            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = "localhost:1192" }).Build())
//            {
//                adminClient.CreateTopicsAsync(, new CreateTopicsOptions());

//                // Warning: The API for this functionality is subject to change.
//                var meta = adminClient.GetMetadata(TimeSpan.FromSeconds(20));
//                _output.WriteLine($"{meta.OriginatingBrokerId} {meta.OriginatingBrokerName}");
//                meta.Brokers.ForEach(broker =>
//                    _output.WriteLine($"Broker: {broker.BrokerId} {broker.Host}:{broker.Port}"));

//                meta.Topics.ForEach(topic =>
//                {
//                    _output.WriteLine($"Topic: {topic.Topic} {topic.Error}");
//                    topic.Partitions.ForEach(partition =>
//                    {
//                        _output.WriteLine($"  Partition: {partition.PartitionId}");
//                        _output.WriteLine($"    Replicas: {ToString(partition.Replicas)}");
//                        _output.WriteLine($"    InSyncReplicas: {ToString(partition.InSyncReplicas)}");
//                    });
//                });
//            }

//            string ToString(int[] array) => $"[{string.Join(", ", array)}]";
//        }
//    }
//}

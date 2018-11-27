using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace KafkaInterface
{
    public class ContentProducer
    {
        private Dictionary<string, object> config;
        private string topicName;

        public ContentProducer(string topicName)
        {
            this.topicName = topicName;

            config = new Dictionary<string, object> {
                { "bootstrap.servers", "localhost:9092" }
            };
        }

        public async void SendMessage(string message)
        {
            using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
            {
                try
                {
                    // Awaiting the asynchronous produce request below prevents flow of execution
                    // from proceeding until the acknowledgement from the broker is received.
                    var deliveryReport = await producer.ProduceAsync(topicName, null, message);
                    Console.WriteLine($"delivered to: {deliveryReport.TopicPartitionOffset}");
                }
                catch (KafkaException e)
                {
                    Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                }
            }
        }
    }
}

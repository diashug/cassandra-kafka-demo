using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace KafkaInterface
{
    class ContentConsumer
    {
        private Dictionary<string, object> config;
        private string topicName;

        public ContentConsumer(string topicName)
        {
            config = new Dictionary<string, object>
            {
                { "group.id", "my-consumer" },
                { "bootstrap.servers", "localhost:9092" },
                { "enable.auto.commit", "false"}
            };

            this.topicName = topicName;
        }

        public void ConsumeMessages()
        {
            using (var consumer = new Consumer<Null, string>(config, null, new StringDeserializer(Encoding.UTF8)))
            {
                try
                {
                    consumer.Subscribe(new string[] { topicName });

                    consumer.OnMessage += (_, msg) =>
                    {
                        Console.WriteLine($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {msg.Value}");
                        consumer.CommitAsync(msg);
                    };

                    while (true)
                    {
                        consumer.Poll(100);
                    }
                } catch (KafkaException e)
                {
                    Console.WriteLine($"failed to read message: {e.Message} [{e.Error.Code}]");
                }
            }
        }
    }
}

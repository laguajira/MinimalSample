using Confluent.Kafka;
using Newtonsoft.Json;

namespace MinimalSample.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> kafkaProducer;

    public KafkaProducer(string bootstrapServers)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
        };

        this.kafkaProducer = new ProducerBuilder<string, string>(producerConfig).Build();
    }

    public void SendMessage<T>(string topic, string key, T message)
    {
        Console.WriteLine($"Producing Message: {message}\n");

        var serialized_message = JsonConvert.SerializeObject(message);
        kafkaProducer.Produce(topic, new Message<string, string> { Key = key, Value = serialized_message });
    }

    public void Dispose()
    {
        kafkaProducer?.Dispose();
    }
}
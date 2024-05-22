using Confluent.Kafka;

namespace MinimalSample.Kafka;

public class KafkaConsumer : IKafkaConsumer
{
    private readonly IConsumer<string, string> kafkaConsumer;

    public KafkaConsumer(string bootstrapServers, string groupId, string autoOffsetRest, string enableAutoOffsetStore)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = ConvertToAutoOffsetReset(autoOffsetRest),// AutoOffsetReset.Earliest,
            EnableAutoOffsetStore = Convert.ToBoolean(enableAutoOffsetStore),
        };

        this.kafkaConsumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
    }

    static AutoOffsetReset ConvertToAutoOffsetReset(string value)
    {
        return value.ToLower() switch
        {
            "earliest" => AutoOffsetReset.Earliest,
            "latest" => AutoOffsetReset.Latest,
            "error" => AutoOffsetReset.Error,
            _ => throw new ArgumentException($"Invalid AutoOffsetReset value: {value}")
        };
    }

    public void StartConsuming(string topic)
    {
        Console.WriteLine("Starting to Consume *******");
        
        // Subscribe to the Kafka topic
        kafkaConsumer.Subscribe(topic);

        // Start an infinite loop to consume messages
        while (true)
        {
            try
            {
                // Consume messages from the Kafka topic
                var consumeResult = kafkaConsumer.Consume(CancellationToken.None);

                                
                // System.Threading.Thread.Sleep(5000);
                
                // Process the consumed message (implement your logic here)
                Console.WriteLine($"Received message: {consumeResult.Offset} \n");
                Console.WriteLine($"Received message: {consumeResult.Message.Key} \n") ;
                Console.WriteLine($"Received message: {consumeResult.Message.Value} \n\n\n");

            }
            catch (ConsumeException ex)
            {
                // Handle any errors that occur during message consumption
                Console.WriteLine($"Error consuming message: {ex.Error.Reason}");
            }
        }
    }

    public void Dispose()
    {
        // Properly dispose of the Kafka consumer
        kafkaConsumer?.Dispose();
    }
}
namespace MinimalSample.Kafka;

public interface IKafkaConsumer
{
    public void StartConsuming(string topic);
}
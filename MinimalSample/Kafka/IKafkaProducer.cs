namespace MinimalSample.Kafka;

public interface IKafkaProducer
{
    public void SendMessage<T>(string topic, string key, T message);
}
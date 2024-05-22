using Microsoft.AspNetCore.Mvc;
using MinimalSample.Kafka;

var builder = WebApplication.CreateBuilder(args);

var objBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true);
//IConfiguration configuration = objBuilder.Build();

//kafka
builder.Services.AddSingleton<IKafkaProducer>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var bootstrapServers = configuration["KafkaProducerConfig:bootstrapServers"];

    return new KafkaProducer(bootstrapServers);
});

builder.Services.AddSingleton<IKafkaConsumer>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var bootstrapServers = configuration["KafkaConsumerConfig:BootstrapServers"];
    var groupId = configuration["KafkaConsumerConfig:GroupId"];
    var autoOffsetRest = configuration["KafkaConsumerConfig:AutoOffsetRest"];
    var enableAutoOffsetStore = configuration["KafkaConsumerConfig:EnableAutoOffsetStore"];

    return new KafkaConsumer(bootstrapServers, groupId, autoOffsetRest, enableAutoOffsetStore);
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/produce", async([FromServices]IKafkaProducer kafkaProducer) =>
    {
        var topic = "message_kafkatest";
        var key = "Key_1";
        kafkaProducer.SendMessage(topic, key, "I sent this!");

        return "String here";
    })
    .WithName("Kafkatest");

app.MapGet("/consume", async ([FromServices] IKafkaConsumer kafkaConsumer) =>
    {
        var topic = "message_kafkatest";

        kafkaConsumer.StartConsuming(topic);
    })
    .WithName("Consume");
app.Run();
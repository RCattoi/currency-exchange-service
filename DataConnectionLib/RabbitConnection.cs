using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace DataConnectionLib;

public class RabbitConnection
{
  public IModel? _channel;
  private IConnection? _connection;
  // private List<string>? _queueList;

  public void Connect()
  {
    var builder = new ConfigurationBuilder().AddJsonFile(".\\appsettings.json", optional: false, reloadOnChange: true);
    var config = builder.Build();

    ConnectionFactory factory = new();

    var uri = config["rabbitConnection"];
    if (string.IsNullOrEmpty(uri))
    {
      Console.WriteLine("RabbitMQ connection string is missing in appsettings.json");
      return;
    }
    System.Console.WriteLine(uri);
    factory.Uri = new Uri(uriString: uri);
    factory.ClientProvidedName = "Rabbit Sender App";
    _connection = factory.CreateConnection();
    _channel = _connection.CreateModel();
  }

  public void ConfigureQueue(List<string> queues)
  {
    string exchangeName = "httpVerbs";
    string routingKey = "routing-key";

    if (_channel == null)
    {
      Console.WriteLine("Canal não configurado");
      return;
    }

    _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true);

    foreach (string i in queues)
    {
      _channel.QueueDeclare(queue: i, durable: true, exclusive: false, autoDelete: false, arguments: null);
      _channel.QueueBind(queue: i, exchange: exchangeName, routingKey: routingKey, arguments: null);
    }
    System.Console.WriteLine("Filas configuradas");
  }



}
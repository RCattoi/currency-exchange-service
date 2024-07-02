
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;




namespace UpdateDB.Infra
{
  public class RabbitMqHandler
  {
    private IConnection? connection;
    private IModel? _channel;
    private string exchangeName = "httpVerbs";
    private string routingKey = "routing-key";


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
      connection = factory.CreateConnection();
      _channel = connection.CreateModel();

      string redisQueue = "redis";
      string sqlQueue = "sql";

      _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true);
      _channel.QueueDeclare(queue: redisQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
      _channel.QueueDeclare(queue: sqlQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);

      _channel.QueueBind(queue: redisQueue, exchange: exchangeName, routingKey: routingKey, arguments: null);
      _channel.QueueBind(queue: sqlQueue, exchange: exchangeName, routingKey: routingKey, arguments: null);
      System.Console.WriteLine("Filas configuradas");

    }

    public void SendToQueue(CurrencyListModel currencyList)
    {
      if (_channel == null)
      {
        Console.WriteLine("Canal não configurado");
        return;
      }
      try
      {
        string jsonString = System.Text.Json.JsonSerializer.Serialize(currencyList.Currencies, new JsonSerializerOptions { WriteIndented = true });
        System.Console.WriteLine(jsonString);
        byte[] messageBody = Encoding.UTF8.GetBytes(jsonString);
        _channel.BasicPublish(exchangeName, routingKey, basicProperties: null, messageBody);
      }
      catch (Exception e)
      {
        System.Console.WriteLine(e.Message);
      }
    }


  }

}

using RabbitMQ.Client;
using System.Text.Json;
using System.Text;




namespace UpdateDB.Infra
{
  public class RabbitMqHandler
  {
    private IModel? _channel;
    private string exchangeName = "httpVerbs";
    private string routingKey = "routing-key";

    public RabbitMqHandler(IModel channel)
    {
      _channel = channel;
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
        string jsonString = JsonSerializer.Serialize(currencyList.Currencies, new JsonSerializerOptions { WriteIndented = true });
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
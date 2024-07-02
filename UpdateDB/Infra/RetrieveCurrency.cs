using Newtonsoft.Json.Linq;


namespace UpdateDB.Infra
{
  public class RetrieveCurrency
  {

    public async Task<CurrencyListModel> GetCurrencyList()
    {
      await Console.Out.WriteLineAsync("inicio updateTeste");
      HttpClient client = new HttpClient();
      var builder = new ConfigurationBuilder().AddJsonFile(".\\appsettings.json", optional: false, reloadOnChange: true);
      var config = builder.Build();
      HttpResponseMessage response = await client.GetAsync(config["exchangeRateApiUrl"]);
      if (response.IsSuccessStatusCode)
      {
        CurrencyListModel currencyList = new();
        string responseBody = await response.Content.ReadAsStringAsync();
        currencyList.Currencies = new();
        JObject data = JObject.Parse(responseBody);
        var dataList = data["conversion_rates"];
        if (dataList != null)
        {
          foreach (JProperty rate in dataList.Cast<JProperty>())
          {
            CurrencyModel newCurrency = new(rate.Name, (decimal)rate.Value);
            currencyList.Currencies.Add(newCurrency);
          }
          return currencyList;
        }

      }
      await Console.Out.WriteLineAsync("Erro");
      throw new Exception();
    }
  }
}
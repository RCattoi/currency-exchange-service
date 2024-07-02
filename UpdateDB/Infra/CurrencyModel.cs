namespace UpdateDB.Infra
{
    public class CurrencyModel
    {
        public string Currency { get; set; }
        public decimal Rate { get; set; }

        public CurrencyModel(string _currency, decimal _rate)
        {
            Currency = _currency;
            Rate = _rate;
        }
    }
}
namespace App.Services.Traders
{
    public class TradeErrorResult
    {
        public TradeErrorResult(string name, string error) 
        {
            Name = name;
            Error = error;
        }
        public string Name { get; set; }
        public string Error { get; set; }
    }
}

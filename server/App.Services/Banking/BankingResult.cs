namespace App.Services.Banking
{
    public class BankingResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Value { get; set; }

        public BankingResult() { }

        public BankingResult(bool success, string message, T value)
        {
            Success = success;
            Message = message;
            Value = value;
        }

        // Helper methods for convenience
        public static BankingResult<T> Ok(T value, string message = "")
            => new BankingResult<T>(true, message, value);

        public static BankingResult<T> Fail(string message)
            => new BankingResult<T>(false, message, default!);
    }
}
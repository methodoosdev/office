namespace App.Services.Helpers
{
    public partial interface INumberHelper 
    {
        decimal CalculatePercentage(decimal a, decimal b);
    }
    public partial class NumberHelper : INumberHelper
    {
        public decimal CalculatePercentage(decimal a, decimal b)
        {
            if (a == 0) // Prevent division by zero
            {
                return 0;
            }
            return (b / a) * 100;
        }
    }
}
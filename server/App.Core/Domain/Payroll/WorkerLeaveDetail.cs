namespace App.Core.Domain.Payroll
{
    public partial class WorkerLeaveDetail : BaseEntity // Άδειες
    {
        public int TraderId { get; set; }
        public int DaysLeft { get; set; }
        public int WorkerId { get; set; }
        public string WorkerName { get; set; }
        public int Year { get; set; }

    }
}

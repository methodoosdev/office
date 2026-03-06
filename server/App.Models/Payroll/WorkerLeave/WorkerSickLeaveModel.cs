using App.Framework.Models;
using System;

namespace App.Models.Payroll
{
    public partial record WorkerSickLeaveSearchModel : BaseNopModel
    {
        public int TraderId { get; set; }
        public DateTime To { get; set; } // Επιθυμητή ημερομηνία
        public string QuickSearch { get; set; }
    }

    public partial record WorkerSickLeaveModel : BaseNopModel
    {
        public string LastName { get; set; } 
        public string FirstName { get; set; }
        public string Vat { get; set; }
        public DateTime HireDate { get; set; }
        public int Deserved { get; set; }
        public int DaysTaken { get; set; }
        public int DaysLeft { get; set; }
    }
    public partial record WorkerSickLeaveTableModel : BaseNopModel
    {
    }
}

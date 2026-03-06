using System.Collections.Generic;
using System.Linq;

namespace App.Services.Traders
{
    public class TraderConnectionResult
    {
        private readonly IList<string> _errors;

        public TraderConnectionResult()
        {
            _errors = new List<string>();
        }

        public int TraderId { get; set; }
        public int CompanyId { get; set; }
        public int CustomerTypeId { get; set; }
        public int LogistikiProgramTypeId { get; set; }
        public int CategoryBookTypeId { get; set; }
        public int AccountingSchema { get; set; }
        public int HyperPayrollId { get; set; }
        public int TraderPayment { get; set; }
        public int TraderExpense { get; set; }
        public decimal TaxesFee { get; set; }
        public string Vat { get; set; }
        public string TraderName { get; set; }
        public string Connection { get; set; }
        public string DiscountPredictions { get; set; }
        public string DeductibleCredits { get; set; }
        public string TaxisUserName { get; set; }
        public string TaxisPassword { get; set; }
        public string MydataUserName { get; set; }
        public string MydataPaswword { get; set; }

        public bool Success => !_errors.Any();

        public void AddError(string error)
        {
            _errors.Add(error);
        }

        public string Error => _errors.FirstOrDefault();
    }
}
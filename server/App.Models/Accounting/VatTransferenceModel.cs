using App.Core.Domain.Traders;
using App.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.Accounting
{
    public partial record VatTransferenceSearchModel : BaseNopModel
    {
        public int TraderId { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
    }
    public partial record VatTransferenceModel : BaseNopEntityModel
    {
        public string Vat { get; set; }
        public decimal Sales { get; set; }
        public decimal Expenses { get; set; }
        public decimal Difference { get; set; }
        public decimal Percentage {  get; set; }
    }
    public partial record VatTransferenceTableModel : BaseNopModel
    {

    }
    public partial record VatTransferenceResult
    {
        public int Code { get; set; }
        public decimal REMAINVAL { get; set; }
        public string Vat { get; set; }
        public int Type {  get; set; }
       
    }

}

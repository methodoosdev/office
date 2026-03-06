using App.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.Accounting
{
    public class CompanyPeriodUsesModel
    {
        public int Year { get; set; }
        public int Period { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    public class CompanyYearUsesModel
    {
        public int Year { get; set; }
        public int Schema { get; set; }
        public string Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    public class AccountingCodePerSchemaPerGradeModel
    {
        public string AccountingCode { get; set; }
        public string Description { get; set; }
        //public string ProcessedName { get; set; }
        public int Schema { get; set; }
        public int Grade { get; set; }
    }
    public class AccountingCodePerSchemaModel
    {
        public string AccountingCode { get; set; }
        public string Description { get; set; }
        //public string ProcessedName { get; set; }
        public int Schema { get; set; }
        public int Grade { get; set; }
    }
    public class FiscalPeriodPerYearModel
    {
        public int Year { get; set; }
        public int Period { get; set; }
        public List<SelectionItemList> Years { get; set; }
        public List<SelectionItemList> Periods { get; set; }
    }
    public class FiscalYearModel
    {
        public int Year { get; set; }
        public List<SelectionItemList> Years { get; set; }
    }
}

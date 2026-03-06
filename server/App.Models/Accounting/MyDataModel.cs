using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Models.Accounting
{
    public class MyDataInfoModel
    {
        public int TraderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsIssuer { get; set; }
    }
    public partial record MyDataInfoFormModel : BaseNopModel
    {
    }
    public class MyDataModel
    {
        public int Id { get; set; }
        public string InvoiceType { get; set; } // Κωδ.Παραστατικού 
        public DateTime IssueDate { get; set; } // Ημερομηνία
        public string Series { get; set; } // Σειρά
        public string Aa { get; set; } // Αριθμός
        public string VatNumber { get; set; } // ΑΦΜ
        public string TraderCode { get; set; }
        public int Branch { get; set; } // Υποκατάστημα
        public long Mark { get; set; } // Μαρκ
        public int PaymentMethodId { get; set; } // Τρόπος Πληρωμής
        public decimal TotalNetValue { get; set; } // Αξία
        public decimal TotalVatAmount { get; set; } // ΦΠΑ
        public decimal TotalGrossValue { get; set; } // Σύνολο
        public int VatProvisionId { get; set; } // Δ.απαλλαγής
        public int SeriesId { get; set; } // Σειρά
        public int DocTypeId { get; set; }
        public bool Register { get; set; }
        public bool IsIssuer { get; set; }
        public bool ExportToExcel { get; set; }

        public IList<MyDataDetailModel> Details { get; set; } = new List<MyDataDetailModel>();

        public string TraderName { get; set; } // Συναλλασσόμενος
        public string InvoiceTypeName { get; set; } // Παραστατικό
        public string PaymentMethodName { get; set; } // Τρόπος Πληρωμής
        public string VatProvisionName { get; set; } // Δ.απαλλαγής
        public string SeriesName { get; set; } // Σειρά
        public string DocTypeName { get; set; } // Τύπος
    }
    public partial record MyDataTableModel : BaseNopModel
    {
    }
    public partial record MyDataDialogFormModel : BaseNopModel
    {
    }
    public class MyDataDetailModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int DocTypeId { get; set; } // Πωλήσεις, Ειδικές πωλήσεων, Αγορές, Δαπάνες
        public int VatCategoryId { get; set; } // Κατηγορία ΦΠΑ
        public decimal NetValue { get; set; } // Αξία είδους
        public decimal VatAmount { get; set; } // ΦΠΑ είδους
        //public int TaxType { get; set; }
        public int TaxCategoryId { get; set; }
        public string ProductCode { get; set; } // Κωδ.Είδους
        public int VatId { get; set; } // ΦΠΑ
        public int CurrencyId { get; set; } // Νόμισμα

        public string ProductCodeName { get; set; } // Περ.Είδους
        public string VatCategoryName { get; set; } // Κατηγορία ΦΠΑ
        public string TaxCategoryName { get; set; } // Κατηγορία εξόδου
        public string VatName { get; set; } // ΦΠΑ
        public string CurrencyName { get; set; } // Νόμισμα
    }
    public partial record MyDataDetailTableModel : BaseNopModel
    {
    }
    public partial record MyDataDetailDialogFormModel : BaseNopModel
    {
    }
    public class MyDataProduct
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public int VatId { get; set; }
        public string VatName { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
    }
    public class MyDataVatProvision
    {
        public int Code { get; set; }
        public string Description { get; set; }
    }
    public class MyDataSeries
    {
        public int Code { get; set; }
        public string Description { get; set; }
    }
    public class MyDataTrader
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Vat { get; set; }
    }
    public class MyDataInvoice
    {
        public DateTime InvoiceDate { get; set; }
        public string Invoice { get; set; }
        public decimal NetAmount { get; set; }
        public string Vat { get; set; }
        public string Mark { get; set; }
    }
    public enum MyDataDocType
    {
        Sales = 1, // Πωλήσεις
        //SpecialSales = 2, // Ειδικές πωλήσεων
        Purchases = 4, // Αγορές
        Expenses = 8 // Δαπάνες
    }
}
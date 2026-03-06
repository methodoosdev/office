using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Models.Accounting
{
    public partial record MonthlyBCategoryBulletinSearchModel : BaseNopModel
    {
        public int TraderId { get; set; }
        public DateTime Period { get; set; }
        public decimal ExpirationInventory { get; set; }
        public decimal ExpirationDepreciate { get; set; }
    }

    public partial record MonthlyBCategoryBulletinModel : BaseNopModel
    {
        public string Id { get; set; }              // Κωδικός
        public string Description { get; set; }     // Περιγραφή
        public decimal January { get; set; }
        public decimal February { get; set; }
        public decimal March { get; set; }
        public decimal April { get; set; }
        public decimal May { get; set; }
        public decimal June { get; set; }
        public decimal July { get; set; }
        public decimal August { get; set; }
        public decimal September { get; set; }
        public decimal October { get; set; }
        public decimal November { get; set; }
        public decimal December { get; set; }
        public int DisplayOrder { get; set; }
        public string Type { get; set; }
        public decimal Total { get; set; }
    }

    public partial record MonthlyBCategoryBulletinTableModel : BaseNopModel
    {
    }

    public class MonthlyBCategoryBulletinQuery
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public int Periodos { get; set; }
        public decimal Value { get; set; }
        public string Type { get; set; }
        public int Year { get; set; }
        public decimal Total { get; set; }
    }

    public class PrepareMonthlyBCategoryBulletinModel
    {
        public IList<MonthlyBCategoryBulletinModel> CodeList { get; set; }
        public IList<MonthlyBCategoryBulletinRemodelingCostsQueryModel> RemodelingCostsList { get; set; }
        public MonthlyBCategoryBulletinResultFormModel ResultModel { get; set; }

    }

    public partial record MonthlyBCategoryBulletinResultFormModel : BaseNopModel
    {
        public decimal ExpirationInventory { get; set; }
        public decimal NetProfitPeriod { get; set; }
        public decimal RemodelingCosts { get; set; }
        public decimal PreviousYearDamage { get; set; }
        public decimal TaxProfitPeriod { get; set; }
        public decimal TaxIncome { get; set; }
        public decimal HoldingTaxAdvance { get; set; }
        public decimal TaxAdvance { get; set; }
        public decimal PaymentPreviousYear { get; set; }
        public decimal TaxesFee { get; set; }
        public decimal AmountPayable { get; set; }
        public decimal TaxReturn { get; set; }
    }
    public partial record MonthlyBCategoryBulletinRemodelingCostsQueryModel : BaseNopModel
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
        public decimal Total { get; set; }
        public decimal Result { get; set; }
    }
    public class MonthlyBCategoryBulletinCodes
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
    }
    public class MonthlyBCategoryBulletinPdfForm
    {
        public string Description { get; set; }
        public decimal Value { get; set; }
    }
    public class MonthlyBCategoryBulletinPdfResult
    {
        public int Periodos { get; set; }
        public decimal Value { get; set; }
        public string Type { get; set; }
    }
    public partial record MonthlyBCategoryBulletinPdfModel : BaseNopModel
    {
        public string Description { get; set; }     // Περιγραφή
        public decimal January { get; set; }
        public decimal February { get; set; }
        public decimal March { get; set; }
        public decimal April { get; set; }
        public decimal May { get; set; }
        public decimal June { get; set; }
        public decimal July { get; set; }
        public decimal August { get; set; }
        public decimal September { get; set; }
        public decimal October { get; set; }
        public decimal November { get; set; }
        public decimal December { get; set; }
        public decimal Total { get; set; }
    }

    // Αποθέματα έναρξης περιόδου
    public partial record MonthlyBCategoryExpirationPdfModel : BaseNopModel
    {
        public string Type { get; set; }     // Περιγραφή
        public decimal Goods { get; set; }  // Εμπορευμάτων
        public decimal Materials { get; set; }     // Α-Β υλών
        public decimal Consumables { get; set; }   // Αναλώσιμα
        public decimal SpareParts { get; set; }   // Ανταλλακτικά
        public decimal WarehouseOther { get; set; }  // Λοιπών αποθ.
        public decimal Total { get; set; }           // Σύνολα
    }

    // Αποθέματα λήξης περιόδου
    public partial record MonthlyBCategoryEndingPdfModel : BaseNopModel
    {
        public string Type { get; set; }     // Περιγραφή
        public decimal Goods { get; set; }  // Εμπορευμάτων
        public decimal Materials { get; set; }     // Α-Β υλών
        public decimal Consumables { get; set; }   // Αναλώσιμα
        public decimal SpareParts { get; set; }   // Ανταλλακτικά
        public decimal WarehouseOther { get; set; }  // Λοιπών αποθ.
        public decimal Total { get; set; }           // Σύνολα
    }
    public class MonthlyBCategoryPrintPdfResult
    {
        public string Type { get; set; }     // Περιγραφή
        public decimal Value { get; set; }
    }
    public class MonthlyBCategoryPdfResult
    {
        public MonthlyBCategoryBulletinSearchModel SearchModel { get; set; }     
        public MonthlyBCategoryBulletinResultFormModel ResultModel { get; set; }
    }

}
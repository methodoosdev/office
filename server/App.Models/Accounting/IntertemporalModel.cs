using App.Framework.Models;

namespace App.Models.Accounting
{
    public partial record IntertemporalSearchModel : BaseNopEntityModel
    {
        public int TraderId { get; set; }
        public decimal ExpirationInventory { get; set; }
        public decimal ExpirationDepreciate { get; set; }
    }

    public class IntertemporalModel
    {
        public decimal Sales { get; set; } // Πωλήσεις
        public decimal Income { get; set; } // Άλλα έσοδα
        public decimal OpeningInventory { get; set; } // Αποθέματα έναρξης
        public decimal Purchases { get; set; } // Αγορές
        public decimal ClosingInventory { get; set; } // Αποθέματα λήξης

        public decimal GrossProfit { get; set; } // Μεικτό (Πωλήσεις - αξία υλικών)
        public decimal GrossProfitRate { get; set; } // Μεικτό % (Πωλήσεις - αξία υλικών)
        public decimal CostOfGoodsSold { get; set; } // Κόστος πωληθέντων (υλικά)
        public decimal CostOfGoodsSoldRate { get; set; } // Κόστος πωληθέντων % (υλικά)
        public decimal Expenses { get; set; } // Έξοδα
        public decimal ExpensesRate { get; set; } // Έξοδα
        public decimal NetProfits { get; set; } // Καθαρά κέρδη
        public decimal NetProfitsRate { get; set; } // Καθαρά κέρδη %
        public decimal SalesClosingInventoryRate { get; set; } // Αποθ.Λήξης % των πωλήσεων
        public decimal PurchasesClosingInventoryRate { get; set; } // Αποθ.Λήξης % των αγορών

        public int Year { get; set; } // Έτος
    }

    public class IntertemporalResult
    {
        public string Id { get; set; }
        public int Year { get; set; }
        public decimal Total { get; set; }
    }

    public class IntertemporalData
    {
        public string Description { get; set; }
        public string Year { get; set; }
        public decimal Value { get; set; }
    }

}

using App.Framework.Models;
using System;

namespace App.Models.Accounting
{
    public partial record AggregateAnalysisSearchModel : BaseNopModel
    {
        public int TraderId { get; set; }
        public int Year { get; set; }
        public int Period { get; set; }
    }
    public partial record AggregateAnalysisModel : BaseNopEntityModel
    {
        public string Date { get; set; } // Ημερομηνία
        public decimal Sales { get; set; } //Πωλήσεις
        public decimal SalesExports { get; set; } //Εξαγωγές
        public decimal SalesReturns { get; set; } //Επιστροφές
        public decimal SalesTotal { get; set; } //Σύνολο
        public decimal Orders { get; set; } //Παραγγελίες
        public decimal Purchases { get; set; } //Αγορές
        public decimal PurchasesImports { get; set; } //Εισαγωγές
        public decimal PurchasesReturns { get; set; } //Επιστροφές Αγ.
        public decimal PurchasesTotal { get; set; } //Σύνολο Αγ.
        public decimal CustomerBalance { get; set; } //Υπόλοιπο πελατών
        public decimal SuppliersBalance { get; set; } //Υπόλοιπο προμηθευτών 
        public decimal WarehouseBalance { get; set; } //Υπόλοιπο αποθήκης 
        public decimal Receipts { get; set; } //Εισπράξεις
        public decimal Payments { get; set; } //Πληρωμές
        public decimal Expenses { get; set; } //Δαπάνες
    }
    public partial record AggregateAnalysisTableModel : BaseNopModel
    {
    }
    public class AggregateAnalysisItem
    {
        public DateTime Date { get; set; }
        public string Id { get; set; }
        public decimal Total { get; set; }
    }

    public class AggregateAnalysisTotalModel
    {
        public decimal Cash { get; set; } //Ταμείο
        public decimal Bank { get; set; } //Λογ.Όψεως
        public decimal Term { get; set; }
        public decimal Else { get; set; }
        public decimal Vat { get; set; } //ΦΠΑ
    }
    public partial record AggregateAnalysisTotalTableModel : BaseNopModel
    {
    }
}
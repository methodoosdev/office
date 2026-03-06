using App.Framework.Models;
using System;

namespace App.Models.Scripts
{
    public partial record ScriptFieldSearchModel : BaseSearchModel
    {
        public ScriptFieldSearchModel() : base("order") { }
    }
    public partial record ScriptFieldListModel : BasePagedListModel<ScriptFieldModel>
    {
    }
    public partial record ScriptFieldModel : BaseNopEntityModel
    {
        // Table
        public int? ScriptTableId { get; set; } // Πίνακας λογαριασμού
        public int StartingFiscalYear { get; set; } // Αρχική οικον.χρήση
        public int FiscalYear { get; set; } // Οικονομική χρήση
        public int PeriodFrom { get; set; } // Περίοδος από
        public int PeriodTo { get; set; } // Περίοδος εώς
        public bool Inventory { get; set; } // Απογραφή
        public bool BalanceSheet { get; set; } // Ισολογισμός
        public bool Locked { get; set; } // Κλειδωμένο
        // Query
        public int? ScriptQueryTypeId { get; set; } // Ερώτημα στη βάση
        // Function
        public int? ScriptFunctionTypeId { get; set; } // Σταθερές διαδικασίες
        [Obsolete]
        public int ScriptFunctionId { get; set; } // Σταθερές διαδικασίες
        // Value
        public decimal FixedValue { get; set; } // Σταθερή τιμή

        public int TraderId { get; set; } // Συναλλασσόμενος
        public string FieldName { get; set; } // Όνομα πεδίου
        [Obsolete]
        public string Group { get; set; } // Ομαδοποίηση
        public int ScriptGroupId { get; set; } // Ομαδοποίηση
        public string Description { get; set; } // Περιγραφή
        public int ScriptAggregateTypeId { get; set; } // Τύπος υπολογισμού
        public int ScriptFieldTypeId { get; set; } // Τύπος πεδίου
        public int Order { get; set; } // Κατάταξη

        // Model
        public string ScriptTableName { get; set; } // Πίνακας λογαριασμού
        public string ScriptFunctionName { get; set; } // Διαδικασία
        public string ScriptAggregateTypeName { get; set; } // Τύπος υπολογισμού
        public string ScriptFieldTypeName { get; set; } // Τύπος πεδίου
        public string ScriptDetailName { get; set; } // Διασάφηση
        public string ParentGroupName { get; set; } // Ομαδ.Πατρ.Στ.
        public string ScriptGroupName { get; set; } // Ομαδοποίηση
    }
    public partial record ScriptFieldFormModel : BaseNopModel
    {
    }
}
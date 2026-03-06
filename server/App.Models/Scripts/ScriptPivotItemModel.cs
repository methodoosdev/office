using App.Framework.Models;

namespace App.Models.Scripts
{
    public partial record ScriptPivotItemSearchModel : BaseSearchModel
    {
        public ScriptPivotItemSearchModel() : base("order") { }
    }
    public partial record ScriptPivotItemListModel : BasePagedListModel<ScriptPivotItemModel>
    {
    }
    public partial record ScriptPivotItemModel : BaseNopEntityModel
    {
        public int ScriptPivotId { get; set; } // Σενάριο υπολογισμού
        public int? ScriptFieldId { get; set; } // Υπολογιζόμενο πεδίο
        public int ScriptOperationTypeId { get; set; } // Αριθμητική πράξη 
        public bool Printed { get; set; } // Εκτυπώνεται 
        public int Order { get; set; } // Κατάταξη

        public string ScriptPivotName { get; set; } // Σενάριο υπολογισμού
        public string ScriptFieldName { get; set; } // Υπολογιζόμενο πεδίο
        public string ScriptOperationTypeName { get; set; } // Αριθμητική πράξη 
        public string ScriptFieldTypeName { get; set; } // Τύπος πεδίου
        public string ScriptDetailName { get; set; } // Διασάφηση
        public string ParentGroupName { get; set; } // Ομαδ.Πατρ.Στ.

    }
    public partial record ScriptPivotItemFormModel : BaseNopModel
    {
    }
}
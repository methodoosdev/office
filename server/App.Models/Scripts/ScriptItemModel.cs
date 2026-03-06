using App.Framework.Models;

namespace App.Models.Scripts
{
    public partial record ScriptItemSearchModel : BaseSearchModel
    {
        public ScriptItemSearchModel() : base("order") { }
    }
    public partial record ScriptItemListModel : BasePagedListModel<ScriptItemModel>
    {
    }
    public partial record ScriptItemModel : BaseNopEntityModel
    {
        public int ScriptId { get; set; } // Σενάριο υπολογισμού
        public int ScriptTypeId { get; set; } // Τύπος σεναρίου
        public int? ScriptFieldId { get; set; } // Υπολογιζόμενο πεδίο
        public int ParentId { get; set; } // Σενάριο υπολογισμού
        public int ScriptOperationTypeId { get; set; } // Αριθμητική πράξη 
        public int Order { get; set; } // Κατάταξη

        public string ScriptName { get; set; } // Σενάριο υπολογισμού
        public string ParentName { get; set; } // Σενάριο υπολογισμού
        public string ScriptTypeName { get; set; } // Τύπος σεναρίου
        public string ScriptOperationTypeName { get; set; } // Αριθμητική πράξη 
        public string ParentGroupName { get; set; } // Ομαδ.Πατρ.Στ.

    }
    public partial record ScriptItemFormModel : BaseNopModel
    {
    }
}
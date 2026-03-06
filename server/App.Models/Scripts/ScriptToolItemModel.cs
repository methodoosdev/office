using App.Framework.Models;

namespace App.Models.Scripts
{
    public partial record ScriptToolItemSearchModel : BaseSearchModel
    {
        public ScriptToolItemSearchModel() : base("order") { }
    }
    public partial record ScriptToolItemListModel : BasePagedListModel<ScriptToolItemModel>
    {
    }
    public partial record ScriptToolItemModel : BaseNopEntityModel
    {
        public int ScriptToolId { get; set; } // Εργαλεία
        public int ScriptId { get; set; } // Σενάρια υπολογισμού λεπτομέρειες

        public string ScriptName { get; set; } // Σενάρια υπολογισμού λεπτομέρειες
        public string ScriptGroupName { get; set; } // Ομαδοποίηση
        public int Order { get; set; } // Κατάταξη
    }
    public partial record ScriptToolItemFormModel : BaseNopModel
    {
    }
}
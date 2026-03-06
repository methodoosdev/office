using App.Framework.Models;

namespace App.Models.Scripts
{
    public partial record ScriptTableSearchModel : BaseSearchModel
    {
        public ScriptTableSearchModel() : base("order") { }
    }
    public partial record ScriptTableListModel : BasePagedListModel<ScriptTableModel>
    {
    }
    public partial record ScriptTableModel : BaseNopEntityModel
    {
        public int TraderId { get; set; } // Συναλλασσόμενος
        public string TableName { get; set; } // Όνομα πίνακα
        public string Group { get; set; } // Ομαδοποίηση
        public int ScriptGroupId { get; set; } // Ομαδοποίηση
        public int Order { get; set; } // Κατάταξη

        //public bool HasChildren { get; set; }

        public string ScriptGroupName { get; set; } // Ομαδοποίηση
    }
    public partial record ScriptTableFormModel : BaseNopModel
    {
    }
}
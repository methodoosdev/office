using App.Framework.Models;
using System;

namespace App.Models.Scripts
{
    public partial record ScriptGroupSearchModel : BaseSearchModel
    {
        public ScriptGroupSearchModel() : base("order") { }
    }
    public partial record ScriptGroupListModel : BasePagedListModel<ScriptGroupModel>
    {
    }
    public partial record ScriptGroupModel : BaseNopEntityModel
    {
        public int TraderId { get; set; } // Συναλλασσόμενος
        public string GroupName { get; set; } // Όνομα ομαδοποίησης
        //public int ScriptAlignTypeId { get; set; } // Τοποθέτηση
        public int Order { get; set; } // Κατάταξη
        //public string ScriptAlignTypeName { get; set; } // Τοποθέτηση
    }
    public partial record ScriptGroupFormModel : BaseNopModel
    {
    }
}
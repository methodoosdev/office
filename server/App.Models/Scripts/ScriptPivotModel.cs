using App.Framework.Models;
using System;

namespace App.Models.Scripts
{
    public partial record ScriptPivotConfigModel : BaseNopModel
    {
        public int Year { get; set; }
        public int Period { get; set; }
        public int ShowTypeId { get; set; }
        public bool Inventory { get; set; }
    }
    public partial record ScriptPivotSearchModel : BaseSearchModel
    {
        public ScriptPivotSearchModel() : base("order") { }
    }
    public partial record ScriptPivotListModel : BasePagedListModel<ScriptPivotModel>
    {
    }
    public partial record ScriptPivotModel : BaseNopEntityModel
    {
        public int TraderId { get; set; } // Συναλλασσόμενος
        public string ScriptPivotName { get; set; } // Όνoμα σεναρίου
        [Obsolete]
        public string Group { get; set; } // Ομαδοποίηση
        public int ScriptGroupId { get; set; } // Ομαδοποίηση
        public string Description { get; set; } // Περιγραφή
        public int Order { get; set; } // Κατάταξη

        public string ScriptGroupName { get; set; } // Ομαδοποίηση
    }
    public partial record ScriptPivotFormModel : BaseNopModel
    {
    }
}
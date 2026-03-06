using App.Framework.Models;

namespace App.Models.Scripts
{
    public partial record ScriptTableItemSearchModel : BaseSearchModel
    {
        public ScriptTableItemSearchModel() : base("accountingCode") { }
    }
    public partial record ScriptTableItemListModel : BasePagedListModel<ScriptTableItemModel>
    {
    }
    public partial record ScriptTableItemModel : BaseNopEntityModel
    {
        public int ScriptTableId { get; set; } // Πίνακας λογαριασού
        public string AccountingCode { get; set; } // Λογαριασμός
        public int ScriptBehaviorTypeId { get; set; } // Συμπεριφορά

        public string ScriptTableName { get; set; } // Πίνακας λογαριασού
        public string ScriptBehaviorTypeName { get; set; } // Συμπεριφορά

    }
    public partial record ScriptTableItemFormModel : BaseNopModel
    {
    }
}
using App.Framework.Models;

namespace App.Models.Scripts
{
    public partial record ScriptTableNameSearchModel : BaseSearchModel
    {
        public ScriptTableNameSearchModel() : base("order") { }
    }
    public partial record ScriptTableNameListModel : BasePagedListModel<ScriptTableNameModel>
    {
    }
    public partial record ScriptTableNameModel : BaseNopEntityModel
    {
        public string Name { get; set; } // Όνομα πίνακα
        public int Order { get; set; } // Κατάταξη
    }
    public partial record ScriptTableNameFormModel : BaseNopModel
    {
    }
}
using App.Framework.Models;

namespace App.Models.Common
{
    public partial record PersistStateSearchModel : BaseSearchModel
    {
        public PersistStateSearchModel() : base("customerEmail") { }
    }
    public partial record PersistStateListModel : BasePagedListModel<PersistStateModel>
    {
    }
    public partial record PersistStateModel : BaseNopEntityModel
    {
        public string ModelType { get; set; }
        public string JsonValue { get; set; }
        public int CustomerId { get; set; }

        //
        public string CustomerEmail { get; set; }

    }
}
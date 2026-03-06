using App.Framework.Models;

namespace App.Models.Customers
{
    public partial record CustomerRoleSearchModel : BaseSearchModel
    {
        public CustomerRoleSearchModel() : base("name") { }
        public string Name { get; set; }
        public string SystemName { get; set; }
    }
    public partial record CustomerRoleListModel : BasePagedListModel<CustomerRoleModel>
    {
    }
    public partial record CustomerRoleModel : BaseNopEntityModel
    {
        public CustomerRoleModel()
        {
        }

        public string Name { get; set; }
        public bool Active { get; set; }
        public bool IsSystemRole { get; set; }
        public string SystemName { get; set; }
        public bool EnablePasswordLifetime { get; set; }
    }
    public partial record CustomerRoleFormModel : BaseNopModel
    {
    }
}
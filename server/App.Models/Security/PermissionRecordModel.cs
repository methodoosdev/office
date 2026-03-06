using App.Framework.Models;

namespace App.Models.Security
{
    public partial record PermissionRecordModel : BaseNopModel
    {
        public string Name { get; set; }

        public string SystemName { get; set; }
    }
}
using App.Core.Domain.Localization;

namespace App.Core.Domain.Employees
{
    public partial class Education : BaseEntity, ILocalizedEntity
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}

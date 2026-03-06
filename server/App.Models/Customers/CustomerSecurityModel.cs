using App.Framework.Models;

namespace App.Models.Customers
{
    public partial record CustomerSecurityModel : BaseNopModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string SystemName { get; set; }
    }
}
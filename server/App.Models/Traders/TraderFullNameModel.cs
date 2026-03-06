using App.Core.Domain.Common;
using App.Framework.Models;

namespace App.Models.Traders
{
    public partial record TraderFullNameModel : BaseNopEntityModel, IFullName
    {
        public string Vat { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

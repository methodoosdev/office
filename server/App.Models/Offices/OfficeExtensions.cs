using App.Core.Domain.Offices;
using App.Framework.Infrastructure.Mapper.Extensions;

namespace App.Models.Offices
{
    public static class OfficeExtensions
    {
        public static AccountingOfficeModel ToAccountingOfficeDecrypt(this AccountingOffice accountingOffice)
        {
            return accountingOffice.ToModel<AccountingOfficeModel>();
        }
    }
}
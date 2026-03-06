using App.Core.Configuration;
using App.Core.Domain.Offices;
using App.Data;
using App.Models.Offices;
using System.Threading.Tasks;

namespace App.Services.Offices
{
    public partial interface IAccountingOfficeService
    {
        Task<AccountingOfficeModel> GetAccountingOfficeModelAsync();
        Task<AccountingOffice> GetAccountingOfficeByIdAsync(int accountingOfficeId);
        Task UpdateAccountingOfficeAsync(AccountingOffice accountingOffice);

    }
    public partial class AccountingOfficeService : IAccountingOfficeService
    {
        private readonly AppSettings _appSettings;
        private readonly IRepository<AccountingOffice> _accountingOfficeRepository;

        public AccountingOfficeService(AppSettings appSettings, IRepository<AccountingOffice> accountingOfficeRepository)
        {
            _appSettings = appSettings;
            _accountingOfficeRepository = accountingOfficeRepository;
        }

        public virtual async Task<AccountingOfficeModel> GetAccountingOfficeModelAsync()
        {
            var config = _appSettings.Get<CommonConfig>();
            var office = await _accountingOfficeRepository.GetByIdAsync(config.AccountingOfficeId);

            return office.ToAccountingOfficeDecrypt();
        }

        public virtual async Task<AccountingOffice> GetAccountingOfficeByIdAsync(int accountingOfficeId)
        {
            return await _accountingOfficeRepository.GetByIdAsync(accountingOfficeId);
        }

        public virtual async Task UpdateAccountingOfficeAsync(AccountingOffice accountingOffice)
        {
            await _accountingOfficeRepository.UpdateAsync(accountingOffice);
        }
    }
}
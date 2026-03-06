using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Data;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Traders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Traders
{
    public partial interface IAccountingWorkService
    {
        IQueryable<AccountingWork> Table { get; }
        Task<AccountingWork> GetAccountingWorkByIdAsync(int accountingWorkId);
        Task<IList<AccountingWork>> GetAccountingWorksByIdsAsync(int[] accountingWorkIds);
        Task<IList<AccountingWork>> GetAllAccountingWorksAsync();
        Task<IPagedList<AccountingWorkModel>> GetPagedListAsync(AccountingWorkSearchModel searchModel);
        Task DeleteAccountingWorkAsync(AccountingWork accountingWork);
        Task DeleteAccountingWorkAsync(IList<AccountingWork> accountingWorks);
        Task InsertAccountingWorkAsync(AccountingWork accountingWork);
        Task UpdateAccountingWorkAsync(AccountingWork accountingWork);
    }
    public partial class AccountingWorkService : IAccountingWorkService
    {
        private readonly IRepository<AccountingWork> _accountingWorkRepository;

        public AccountingWorkService(
            IRepository<AccountingWork> accountingWorkRepository)
        {
            _accountingWorkRepository = accountingWorkRepository;
        }

        public virtual IQueryable<AccountingWork> Table => _accountingWorkRepository.Table;

        public virtual async Task<AccountingWork> GetAccountingWorkByIdAsync(int accountingWorkId)
        {
            return await _accountingWorkRepository.GetByIdAsync(accountingWorkId);
        }

        public virtual async Task<IList<AccountingWork>> GetAccountingWorksByIdsAsync(int[] accountingWorkIds)
        {
            return await _accountingWorkRepository.GetByIdsAsync(accountingWorkIds);
        }

        public virtual async Task<IList<AccountingWork>> GetAllAccountingWorksAsync()
        {
            var entities = await _accountingWorkRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public async Task<IPagedList<AccountingWorkModel>> GetPagedListAsync(AccountingWorkSearchModel searchModel)
        {
            var query = _accountingWorkRepository.Table.AsEnumerable()
                .Select(x => x.ToModel<AccountingWorkModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => 
                    c.SortDescription.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task DeleteAccountingWorkAsync(AccountingWork accountingWork)
        {
            await _accountingWorkRepository.DeleteAsync(accountingWork);
        }

        public virtual async Task DeleteAccountingWorkAsync(IList<AccountingWork> accountingWorks)
        {
            await _accountingWorkRepository.DeleteAsync(accountingWorks);
        }

        public virtual async Task InsertAccountingWorkAsync(AccountingWork accountingWork)
        {
            await _accountingWorkRepository.InsertAsync(accountingWork);
        }

        public virtual async Task UpdateAccountingWorkAsync(AccountingWork accountingWork)
        {
            await _accountingWorkRepository.UpdateAsync(accountingWork);
        }
    }
}
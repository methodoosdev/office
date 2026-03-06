using App.Core;
using App.Core.Domain.Customers;
using App.Core.Domain.Traders;
using App.Core.Infrastructure.Mapper;
using App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Traders
{
    public partial interface ITraderService
    {
        IQueryable<Trader> Table { get; }
        Task<Trader> GetTraderByIdAsync(int traderId);
        //Task<TraderDecryptModel> GetTraderDecryptModelAsync(int traderId);
        Task<Trader> GetTraderByVatAsync(string vat);
        Task<Trader> GetTraderByTaxSystemIdAsync(int taxSystemId);
        Task<Trader> GetTraderByHyperPayrollIdAsync(int hyperPayrollId);
        //Task<Trader> GetTraderByNameAsync(string name);
        Task<IList<Trader>> GetTradersByIdsAsync(int[] traderIds);
        Task<IList<Trader>> GetTradersByVatsAsync(string[] vats);
        Task<IList<Trader>> GetTradersByCustomerIdsAsync(int[] customerIds);
        Task DeleteTraderAsync(Trader trader);
        Task DeleteTraderAsync(IList<Trader> traders);
        Task<IList<Trader>> GetAllTradersAsync(bool active = true, bool deleted = false);
        Task<IList<Trader>> GetAllTradersAsync(FieldConfigType type, bool nonRepresentationOfNaturalPerson = false);
        Task InsertTraderAsync(Trader trader);
        Task UpdateTraderAsync(Trader trader);
        Task UpdateTraderAsync(IList<Trader> traders);
        Task<IList<AccountingWork>> GetAccountingWorksByTraderIdAsync(int traderId);
        Task RemoveTraderAccountingWorkMappingAsync(Trader trader, AccountingWork accountingWork);
        Task InsertTraderAccountingWorkMappingAsync(Trader trader, AccountingWork accountingWork);

    }
    public partial class TraderService : ITraderService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Trader> _traderRepository;
        private readonly IRepository<AccountingWork> _accountingWorkRepository;
        private readonly IRepository<TraderAccountingWorkMapping> _traderAccountingWorkMappingRepository;

        public TraderService(
            IRepository<Customer> customerRepository,
            IRepository<Trader> traderRepository,
            IRepository<AccountingWork> accountingWorkRepository,
            IRepository<TraderAccountingWorkMapping> traderAccountingWorkMappingRepository)
        {
            _customerRepository = customerRepository;
            _traderRepository = traderRepository;
            _accountingWorkRepository = accountingWorkRepository;
            _traderAccountingWorkMappingRepository = traderAccountingWorkMappingRepository;
        }

        public virtual IQueryable<Trader> Table => _traderRepository.Table;

        public virtual async Task<Trader> GetTraderByIdAsync(int traderId)
        {
            return await _traderRepository.GetByIdAsync(traderId);
        }

        //public virtual async Task<TraderDecryptModel> GetTraderDecryptModelAsync(int traderId)
        //{
        //    var trader = await _traderRepository.GetByIdAsync(traderId);

        //    return trader?.ToModel<TraderDecryptModel>();
        //}

        public virtual async Task<Trader> GetTraderByVatAsync(string vat)
        {
            return await _traderRepository.Table.ToAsyncEnumerable().FirstOrDefaultAsync(x => AesEncryption.Decrypt(x.Vat) == vat);
        }

        public virtual async Task<Trader> GetTraderByTaxSystemIdAsync(int taxSystemId)
        {
            return await _traderRepository.Table.FirstOrDefaultAsync(x => x.TaxSystemId == taxSystemId);
        }

        public virtual async Task<Trader> GetTraderByHyperPayrollIdAsync(int hyperPayrollId)
        {
            return await _traderRepository.Table.FirstOrDefaultAsync(x => x.HyperPayrollId == hyperPayrollId);
        }

        //public virtual async Task<Trader> GetTraderByNameAsync(string name)
        //{
        //    name = name?.Trim();

        //    if (string.IsNullOrEmpty(name)) return null;

        //    var entities = await _traderRepository.GetAllAsync(query =>
        //    {
        //        query = query
        //            .AsEnumerable()
        //            .Where(x => x.FullName().Equals(name, StringComparison.OrdinalIgnoreCase))
        //            .AsQueryable();
        //        return query;
        //    });

        //    return entities.FirstOrDefault();
        //}

        public virtual async Task<IList<Trader>> GetTradersByIdsAsync(int[] traderIds)
        {
            return await _traderRepository.GetByIdsAsync(traderIds);
        }

        public virtual async Task<IList<Trader>> GetTradersByVatsAsync(string[] vats)
        {
            if (vats is null)
                throw new ArgumentNullException(nameof(vats));

            return await (from v in _traderRepository.Table
                          where vats.Contains(v.Vat) && !v.Deleted && v.Active
                          select v).ToListAsync();
        }

        public virtual async Task<IList<Trader>> GetTradersByCustomerIdsAsync(int[] customerIds)
        {
            if (customerIds is null)
                throw new ArgumentNullException(nameof(customerIds));

            return await (from v in _traderRepository.Table
                          join c in _customerRepository.Table on v.Id equals c.TraderId
                          where customerIds.Contains(c.Id) && !v.Deleted && v.Active
                          select v).Distinct().ToListAsync();
        }

        public virtual async Task DeleteTraderAsync(Trader trader)
        {
            await _traderRepository.DeleteAsync(trader);
        }

        public virtual async Task DeleteTraderAsync(IList<Trader> traders)
        {
            await _traderRepository.DeleteAsync(traders);
        }

        public virtual async Task InsertTraderAsync(Trader trader)
        {
            await _traderRepository.InsertAsync(trader);
        }

        public virtual async Task UpdateTraderAsync(Trader trader)
        {
            await _traderRepository.UpdateAsync(trader);
        }

        public virtual async Task UpdateTraderAsync(IList<Trader> traders)
        {
            await _traderRepository.UpdateAsync(traders);
        }

        public virtual async Task<IList<Trader>> GetAllTradersAsync(bool active = true, bool deleted = false)
        {
            var entities = await _traderRepository.GetAllAsync(query =>
            {
                if (active)
                    query = query.Where(e => e.Active);

                if (!deleted)
                    query = query.Where(e => !e.Deleted);

                return query;
            });

            return entities;
        }

        public async Task<IList<Trader>> GetAllTradersAsync(FieldConfigType type, bool nonRepresentationOfNaturalPersonEnable = false)
        {
            var entities = await _traderRepository.GetAllAsync(query =>
            {
                query = query.Where(e => !e.Deleted && e.Active);

                switch (type)
                {
                    case FieldConfigType.Payroll:
                        query = query.Where(x => x.HyperPayrollId > 0);
                        break;
                    case FieldConfigType.IndividualLegal:
                        query = query.Where(x => x.CustomerTypeId == (int)CustomerType.PersonalCompany || x.CustomerTypeId == (int)CustomerType.LegalPerson || x.CustomerTypeId == (int)CustomerType.IndividualCompany || x.CustomerTypeId == (int)CustomerType.Consortium || x.CustomerTypeId == (int)CustomerType.Society);
                        break;
                    case FieldConfigType.IndividualNatural:
                        query = query.Where(x => x.CustomerTypeId == (int)CustomerType.NaturalPerson || x.CustomerTypeId == (int)CustomerType.IndividualCompany);
                        break;
                    case FieldConfigType.IndividualLegalNatural:
                        query = query.Where(x => x.CustomerTypeId == (int)CustomerType.PersonalCompany || x.CustomerTypeId == (int)CustomerType.LegalPerson || x.CustomerTypeId == (int)CustomerType.IndividualCompany || x.CustomerTypeId == (int)CustomerType.Consortium || x.CustomerTypeId == (int)CustomerType.Society || x.CustomerTypeId == (int)CustomerType.NaturalPerson);
                        break;
                    case FieldConfigType.WithCategoryBooks:
                        query = query.Where(x => x.CategoryBookTypeId == (int)CategoryBookType.B || x.CategoryBookTypeId == (int)CategoryBookType.C);
                        break;
                    case FieldConfigType.WithCategoryBookB:
                        query = query.Where(x => x.CategoryBookTypeId == (int)CategoryBookType.B);
                        break;
                    case FieldConfigType.WithCategoryBookC:
                        query = query.Where(x => x.CategoryBookTypeId == (int)CategoryBookType.C);
                        break;
                    case FieldConfigType.OnlySoftone:
                        query = query.Where(x => x.LogistikiProgramTypeId == (int)LogistikiProgramType.SoftOne);
                        break;
                }

                if (nonRepresentationOfNaturalPersonEnable)
                    query = query.Where(x => x.NonRepresentationOfNaturalPerson == false);

                return query;
            });

            return entities;
        }

        //TraderAccountingWorkMapping
        public virtual async Task<IList<AccountingWork>> GetAccountingWorksByTraderIdAsync(int traderId)
        {
            if (traderId == 0)
                return new List<AccountingWork>();

            return await _accountingWorkRepository.GetAllAsync(query =>
            {
                return from a in query
                       join taw in _traderAccountingWorkMappingRepository.Table on a.Id equals taw.AccountingWorkId
                       where taw.TraderId == traderId
                       select a;
            });
        }

        public virtual async Task RemoveTraderAccountingWorkMappingAsync(Trader trader, AccountingWork accountingWork)
        {
            if (trader == null)
                throw new ArgumentNullException(nameof(trader));

            if (accountingWork is null)
                throw new ArgumentNullException(nameof(accountingWork));

            if (await _traderAccountingWorkMappingRepository.Table
                .FirstOrDefaultAsync(m => m.AccountingWorkId == accountingWork.Id && m.TraderId == trader.Id)
                is TraderAccountingWorkMapping mapping)
            {
                await _traderAccountingWorkMappingRepository.DeleteAsync(mapping);
            }
        }

        public virtual async Task InsertTraderAccountingWorkMappingAsync(Trader trader, AccountingWork accountingWork)
        {
            if (trader is null)
                throw new ArgumentNullException(nameof(trader));

            if (accountingWork is null)
                throw new ArgumentNullException(nameof(accountingWork));

            if (await _traderAccountingWorkMappingRepository.Table
                .FirstOrDefaultAsync(m => m.AccountingWorkId == accountingWork.Id && m.TraderId == trader.Id)
                is null)
            {
                var mapping = new TraderAccountingWorkMapping
                {
                    AccountingWorkId = accountingWork.Id,
                    TraderId = trader.Id
                };

                await _traderAccountingWorkMappingRepository.InsertAsync(mapping);
            }
        }
    }
}
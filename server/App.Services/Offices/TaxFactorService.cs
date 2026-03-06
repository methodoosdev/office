using App.Core;
using App.Core.Domain.Offices;
using App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace App.Services.Offices
{
    public partial interface ITaxFactorService
    {
        IQueryable<TaxFactor> Table { get; }
        Task<TaxFactor> GetTaxFactorByIdAsync(int taxFactorId);
        Task<IList<TaxFactor>> GetTaxFactorsByIdsAsync(int[] taxFactorIds);
        Task<IList<TaxFactor>> GetAllTaxFactorsAsync();
        Task DeleteTaxFactorAsync(TaxFactor taxFactor);
        Task DeleteTaxFactorAsync(IList<TaxFactor> taxFactors);
        Task InsertTaxFactorAsync(TaxFactor taxFactor);
        Task UpdateTaxFactorAsync(TaxFactor taxFactor);
    }
    public partial class TaxFactorService : ITaxFactorService
    {
        private readonly IRepository<TaxFactor> _taxFactorRepository;

        public TaxFactorService(IRepository<TaxFactor> taxFactorRepository)
        {
            _taxFactorRepository = taxFactorRepository;
        }

        public virtual IQueryable<TaxFactor> Table => _taxFactorRepository.Table;

        public virtual async Task<TaxFactor> GetTaxFactorByIdAsync(int taxFactorId)
        {
            return await _taxFactorRepository.GetByIdAsync(taxFactorId);
        }

        public virtual async Task<IList<TaxFactor>> GetTaxFactorsByIdsAsync(int[] taxFactorIds)
        {
            return await _taxFactorRepository.GetByIdsAsync(taxFactorIds);
        }

        public virtual async Task<IList<TaxFactor>> GetAllTaxFactorsAsync()
        {
            var taxFactors = await _taxFactorRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(x => x.Year);

                return query;
            });

            return taxFactors;
        }

        public virtual async Task DeleteTaxFactorAsync(TaxFactor taxFactor)
        {
            if (taxFactor == null)
                throw new ArgumentNullException(nameof(taxFactor));

            await _taxFactorRepository.DeleteAsync(taxFactor);
        }

        public virtual async Task DeleteTaxFactorAsync(IList<TaxFactor> taxFactors)
        {
            await _taxFactorRepository.DeleteAsync(taxFactors);
        }

        public virtual async Task InsertTaxFactorAsync(TaxFactor taxFactor)
        {
            if (taxFactor == null)
                throw new ArgumentNullException(nameof(taxFactor));

            await _taxFactorRepository.InsertAsync(taxFactor);
        }

        public virtual async Task UpdateTaxFactorAsync(TaxFactor taxFactor)
        {
            if (taxFactor == null)
                throw new ArgumentNullException(nameof(taxFactor));

            await _taxFactorRepository.UpdateAsync(taxFactor);
        }

    }
}
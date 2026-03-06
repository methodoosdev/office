using App.Core.Domain.VatExemption;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.VatExemption
{
    public partial interface IVatExemptionDocService
    {
        IQueryable<VatExemptionDoc> Table { get; }
        Task<VatExemptionDoc> GetVatExemptionDocByIdAsync(int vatExemptionDocId);
        Task<IList<VatExemptionDoc>> GetVatExemptionDocsByIdsAsync(int[] vatExemptionDocIds);
        Task<IList<VatExemptionDoc>> GetAllVatExemptionDocsAsync(int traderId = 0);
        Task DeleteVatExemptionDocAsync(VatExemptionDoc vatExemptionDoc);
        Task DeleteVatExemptionDocAsync(IList<VatExemptionDoc> vatExemptionDocs);
        Task InsertVatExemptionDocAsync(VatExemptionDoc vatExemptionDoc);
        Task UpdateVatExemptionDocAsync(VatExemptionDoc vatExemptionDoc);
        Task<VatExemptionDoc> GetLastVatExemptionDocAsync(int traderId);
    }
    public partial class VatExemptionDocService : IVatExemptionDocService
    {
        private readonly IRepository<VatExemptionDoc> _vatExemptionDocRepository;

        public VatExemptionDocService(
            IRepository<VatExemptionDoc> vatExemptionDocRepository)
        {
            _vatExemptionDocRepository = vatExemptionDocRepository;
        }

        public virtual IQueryable<VatExemptionDoc> Table => _vatExemptionDocRepository.Table;

        public virtual async Task<VatExemptionDoc> GetVatExemptionDocByIdAsync(int vatExemptionDocId)
        {
            return await _vatExemptionDocRepository.GetByIdAsync(vatExemptionDocId);
        }

        public virtual async Task<IList<VatExemptionDoc>> GetVatExemptionDocsByIdsAsync(int[] vatExemptionDocIds)
        {
            return await _vatExemptionDocRepository.GetByIdsAsync(vatExemptionDocIds);
        }

        public virtual async Task<IList<VatExemptionDoc>> GetAllVatExemptionDocsAsync(int traderId = 0)
        {
            var entities = await _vatExemptionDocRepository.GetAllAsync(query =>
            {
                if (traderId > 0)
                    query = query.Where(x => x.TraderId == traderId);

                query = query.OrderBy(l => l.Id);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteVatExemptionDocAsync(VatExemptionDoc vatExemptionDoc)
        {
            await _vatExemptionDocRepository.DeleteAsync(vatExemptionDoc);
        }

        public virtual async Task DeleteVatExemptionDocAsync(IList<VatExemptionDoc> vatExemptionDocs)
        {
            await _vatExemptionDocRepository.DeleteAsync(vatExemptionDocs);
        }

        public virtual async Task InsertVatExemptionDocAsync(VatExemptionDoc vatExemptionDoc)
        {
            await _vatExemptionDocRepository.InsertAsync(vatExemptionDoc);
        }

        public virtual async Task UpdateVatExemptionDocAsync(VatExemptionDoc vatExemptionDoc)
        {
            await _vatExemptionDocRepository.UpdateAsync(vatExemptionDoc);
        }
        public virtual async Task<VatExemptionDoc> GetLastVatExemptionDocAsync(int traderId)
        {
            return await _vatExemptionDocRepository.Table
                .Where(x => x.TraderId == traderId)
                .OrderByDescending(o => o.CreatedDate)
                .FirstOrDefaultAsync();
        }

    }
}
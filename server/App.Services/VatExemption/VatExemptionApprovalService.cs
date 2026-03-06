using App.Core.Domain.VatExemption;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.VatExemption
{
    public partial interface IVatExemptionApprovalService
    {
        IQueryable<VatExemptionApproval> Table { get; }
        Task<VatExemptionApproval> GetVatExemptionApprovalByIdAsync(int vatExemptionApprovalId);
        Task<IList<VatExemptionApproval>> GetVatExemptionApprovalsByIdsAsync(int[] vatExemptionApprovalIds);
        Task<IList<VatExemptionApproval>> GetAllVatExemptionApprovalsAsync(int traderId = 0);
        Task DeleteVatExemptionApprovalAsync(VatExemptionApproval vatExemptionApproval);
        Task DeleteVatExemptionApprovalAsync(IList<VatExemptionApproval> vatExemptionApprovals);
        Task InsertVatExemptionApprovalAsync(VatExemptionApproval vatExemptionApproval);
        Task UpdateVatExemptionApprovalAsync(VatExemptionApproval vatExemptionApproval);
        Task UpdateVatExemptionApprovalAsync(IList<VatExemptionApproval> vatExemptionApprovals);
    }
    public partial class VatExemptionApprovalService : IVatExemptionApprovalService
    {
        private readonly IRepository<VatExemptionApproval> _vatExemptionApprovalRepository;

        public VatExemptionApprovalService(
            IRepository<VatExemptionApproval> vatExemptionApprovalRepository)
        {
            _vatExemptionApprovalRepository = vatExemptionApprovalRepository;
        }

        public virtual IQueryable<VatExemptionApproval> Table => _vatExemptionApprovalRepository.Table;

        public virtual async Task<VatExemptionApproval> GetVatExemptionApprovalByIdAsync(int vatExemptionApprovalId)
        {
            return await _vatExemptionApprovalRepository.GetByIdAsync(vatExemptionApprovalId);
        }

        public virtual async Task<IList<VatExemptionApproval>> GetVatExemptionApprovalsByIdsAsync(int[] vatExemptionApprovalIds)
        {
            return await _vatExemptionApprovalRepository.GetByIdsAsync(vatExemptionApprovalIds);
        }

        public virtual async Task<IList<VatExemptionApproval>> GetAllVatExemptionApprovalsAsync(int traderId = 0)
        {
            var entities = await _vatExemptionApprovalRepository.GetAllAsync(query =>
            {
                if (traderId > 0)
                    query = query.Where(x => x.TraderId == traderId);

                query = query.OrderBy(l => l.Id);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteVatExemptionApprovalAsync(VatExemptionApproval vatExemptionApproval)
        {
            await _vatExemptionApprovalRepository.DeleteAsync(vatExemptionApproval);
        }

        public virtual async Task DeleteVatExemptionApprovalAsync(IList<VatExemptionApproval> vatExemptionApprovals)
        {
            await _vatExemptionApprovalRepository.DeleteAsync(vatExemptionApprovals);
        }

        public virtual async Task InsertVatExemptionApprovalAsync(VatExemptionApproval vatExemptionApproval)
        {
            await _vatExemptionApprovalRepository.InsertAsync(vatExemptionApproval);
        }

        public virtual async Task UpdateVatExemptionApprovalAsync(VatExemptionApproval vatExemptionApproval)
        {
            await _vatExemptionApprovalRepository.UpdateAsync(vatExemptionApproval);
        }
        public virtual async Task UpdateVatExemptionApprovalAsync(IList<VatExemptionApproval> vatExemptionApprovals)
        {
            await _vatExemptionApprovalRepository.UpdateAsync(vatExemptionApprovals);
        }

    }
}
using App.Core.Domain.Assignment;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Assignment
{
    public partial interface IAssignmentReasonService
    {
        IQueryable<AssignmentReason> Table { get; }
        Task<AssignmentReason> GetAssignmentReasonByIdAsync(int assignmentReasonId);
        Task<IList<AssignmentReason>> GetAssignmentReasonsByIdsAsync(int[] assignmentReasonIds);
        Task<IList<AssignmentReason>> GetAllAssignmentReasonsAsync();
        Task DeleteAssignmentReasonAsync(AssignmentReason assignmentReason);
        Task DeleteAssignmentReasonAsync(IList<AssignmentReason> assignmentReasons);
        Task InsertAssignmentReasonAsync(AssignmentReason assignmentReason);
        Task UpdateAssignmentReasonAsync(AssignmentReason assignmentReason);
    }
    public partial class AssignmentReasonService : IAssignmentReasonService
    {
        private readonly IRepository<AssignmentReason> _assignmentReasonRepository;

        public AssignmentReasonService(
            IRepository<AssignmentReason> assignmentReasonRepository)
        {
            _assignmentReasonRepository = assignmentReasonRepository;
        }

        public virtual IQueryable<AssignmentReason> Table => _assignmentReasonRepository.Table;

        public virtual async Task<AssignmentReason> GetAssignmentReasonByIdAsync(int assignmentReasonId)
        {
            return await _assignmentReasonRepository.GetByIdAsync(assignmentReasonId);
        }

        public virtual async Task<IList<AssignmentReason>> GetAssignmentReasonsByIdsAsync(int[] assignmentReasonIds)
        {
            return await _assignmentReasonRepository.GetByIdsAsync(assignmentReasonIds);
        }

        public virtual async Task<IList<AssignmentReason>> GetAllAssignmentReasonsAsync()
        {
            var entities = await _assignmentReasonRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteAssignmentReasonAsync(AssignmentReason assignmentReason)
        {
            await _assignmentReasonRepository.DeleteAsync(assignmentReason);
        }

        public virtual async Task DeleteAssignmentReasonAsync(IList<AssignmentReason> assignmentReasons)
        {
            await _assignmentReasonRepository.DeleteAsync(assignmentReasons);
        }

        public virtual async Task InsertAssignmentReasonAsync(AssignmentReason assignmentReason)
        {
            await _assignmentReasonRepository.InsertAsync(assignmentReason);
        }

        public virtual async Task UpdateAssignmentReasonAsync(AssignmentReason assignmentReason)
        {
            await _assignmentReasonRepository.UpdateAsync(assignmentReason);
        }
    }
}
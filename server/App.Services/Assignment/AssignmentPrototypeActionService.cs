using App.Core.Domain.Assignment;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Assignment
{
    public partial interface IAssignmentPrototypeActionService
    {
        IQueryable<AssignmentPrototypeAction> Table { get; }
        Task<AssignmentPrototypeAction> GetAssignmentPrototypeActionByIdAsync(int assignmentPrototypeActionId);
        Task<IList<AssignmentPrototypeAction>> GetAssignmentPrototypeActionsByIdsAsync(int[] assignmentPrototypeActionIds);
        Task<IList<AssignmentPrototypeAction>> GetAllAssignmentPrototypeActionsAsync();
        Task DeleteAssignmentPrototypeActionAsync(AssignmentPrototypeAction assignmentPrototypeAction);
        Task DeleteAssignmentPrototypeActionAsync(IList<AssignmentPrototypeAction> assignmentPrototypeActions);
        Task InsertAssignmentPrototypeActionAsync(AssignmentPrototypeAction assignmentPrototypeAction);
        Task UpdateAssignmentPrototypeActionAsync(AssignmentPrototypeAction assignmentPrototypeAction);
    }
    public partial class AssignmentPrototypeActionService : IAssignmentPrototypeActionService
    {
        private readonly IRepository<AssignmentPrototypeAction> _assignmentPrototypeActionRepository;

        public AssignmentPrototypeActionService(
            IRepository<AssignmentPrototypeAction> assignmentPrototypeActionRepository)
        {
            _assignmentPrototypeActionRepository = assignmentPrototypeActionRepository;
        }

        public virtual IQueryable<AssignmentPrototypeAction> Table => _assignmentPrototypeActionRepository.Table;

        public virtual async Task<AssignmentPrototypeAction> GetAssignmentPrototypeActionByIdAsync(int assignmentPrototypeActionId)
        {
            return await _assignmentPrototypeActionRepository.GetByIdAsync(assignmentPrototypeActionId);
        }

        public virtual async Task<IList<AssignmentPrototypeAction>> GetAssignmentPrototypeActionsByIdsAsync(int[] assignmentPrototypeActionIds)
        {
            return await _assignmentPrototypeActionRepository.GetByIdsAsync(assignmentPrototypeActionIds);
        }

        public virtual async Task<IList<AssignmentPrototypeAction>> GetAllAssignmentPrototypeActionsAsync()
        {
            var entities = await _assignmentPrototypeActionRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteAssignmentPrototypeActionAsync(AssignmentPrototypeAction assignmentPrototypeAction)
        {
            await _assignmentPrototypeActionRepository.DeleteAsync(assignmentPrototypeAction);
        }

        public virtual async Task DeleteAssignmentPrototypeActionAsync(IList<AssignmentPrototypeAction> assignmentPrototypeActions)
        {
            await _assignmentPrototypeActionRepository.DeleteAsync(assignmentPrototypeActions);
        }

        public virtual async Task InsertAssignmentPrototypeActionAsync(AssignmentPrototypeAction assignmentPrototypeAction)
        {
            await _assignmentPrototypeActionRepository.InsertAsync(assignmentPrototypeAction);
        }

        public virtual async Task UpdateAssignmentPrototypeActionAsync(AssignmentPrototypeAction assignmentPrototypeAction)
        {
            await _assignmentPrototypeActionRepository.UpdateAsync(assignmentPrototypeAction);
        }
    }
}
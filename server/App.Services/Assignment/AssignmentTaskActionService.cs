using App.Core.Domain.Assignment;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Assignment
{
    public partial interface IAssignmentTaskActionService
    {
        IQueryable<AssignmentTaskAction> Table { get; }
        Task<AssignmentTaskAction> GetAssignmentTaskActionByIdAsync(int assignmentTaskActionId);
        Task<IList<AssignmentTaskAction>> GetAssignmentTaskActionsByIdsAsync(int[] assignmentTaskActionIds);
        Task<IList<AssignmentTaskAction>> GetAllAssignmentTaskActionsAsync(int assignmentTaskId = 0);
        Task DeleteAssignmentTaskActionAsync(AssignmentTaskAction assignmentTaskAction);
        Task DeleteAssignmentTaskActionAsync(IList<AssignmentTaskAction> assignmentTaskActions);
        Task InsertAssignmentTaskActionAsync(AssignmentTaskAction assignmentTaskAction);
        Task InsertAssignmentTaskActionAsync(IList<AssignmentTaskAction> assignmentTaskActions);
        Task UpdateAssignmentTaskActionAsync(AssignmentTaskAction assignmentTaskAction);
    }
    public partial class AssignmentTaskActionService : IAssignmentTaskActionService
    {
        private readonly IRepository<AssignmentTaskAction> _assignmentTaskActionRepository;

        public AssignmentTaskActionService(
            IRepository<AssignmentTaskAction> assignmentTaskActionRepository)
        {
            _assignmentTaskActionRepository = assignmentTaskActionRepository;
        }

        public virtual IQueryable<AssignmentTaskAction> Table => _assignmentTaskActionRepository.Table;

        public virtual async Task<AssignmentTaskAction> GetAssignmentTaskActionByIdAsync(int assignmentTaskActionId)
        {
            return await _assignmentTaskActionRepository.GetByIdAsync(assignmentTaskActionId);
        }

        public virtual async Task<IList<AssignmentTaskAction>> GetAssignmentTaskActionsByIdsAsync(int[] assignmentTaskActionIds)
        {
            return await _assignmentTaskActionRepository.GetByIdsAsync(assignmentTaskActionIds);
        }

        public virtual async Task<IList<AssignmentTaskAction>> GetAllAssignmentTaskActionsAsync(int assignmentTaskId = 0)
        {
            var entities = await _assignmentTaskActionRepository.GetAllAsync(query =>
            {
                if (assignmentTaskId > 0)
                    query = query.Where(x => x.AssignmentTaskId == assignmentTaskId);

                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteAssignmentTaskActionAsync(AssignmentTaskAction assignmentTaskAction)
        {
            await _assignmentTaskActionRepository.DeleteAsync(assignmentTaskAction);
        }

        public virtual async Task DeleteAssignmentTaskActionAsync(IList<AssignmentTaskAction> assignmentTaskActions)
        {
            await _assignmentTaskActionRepository.DeleteAsync(assignmentTaskActions);
        }

        public virtual async Task InsertAssignmentTaskActionAsync(AssignmentTaskAction assignmentTaskAction)
        {
            await _assignmentTaskActionRepository.InsertAsync(assignmentTaskAction);
        }

        public virtual async Task InsertAssignmentTaskActionAsync(IList<AssignmentTaskAction> assignmentTaskActions)
        {
            await _assignmentTaskActionRepository.InsertAsync(assignmentTaskActions);
        }

        public virtual async Task UpdateAssignmentTaskActionAsync(AssignmentTaskAction assignmentTaskAction)
        {
            await _assignmentTaskActionRepository.UpdateAsync(assignmentTaskAction);
        }
    }
}
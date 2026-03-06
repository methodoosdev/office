using App.Core;
using App.Core.Domain.Assignment;
using App.Core.Domain.Employees;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Assignment
{
    public partial interface IAssignmentTaskService
    {
        IQueryable<AssignmentTask> Table { get; }
        Task<AssignmentTask> GetAssignmentTaskByIdAsync(int assignmentTaskId);
        Task<IList<AssignmentTask>> GetAssignmentTasksByIdsAsync(int[] assignmentTaskIds);
        Task<IList<AssignmentTask>> GetAllAssignmentTasksAsync();
        Task DeleteAssignmentTaskAsync(AssignmentTask assignmentTask);
        Task DeleteAssignmentTaskAsync(IList<AssignmentTask> assignmentTasks);
        Task InsertAssignmentTaskAsync(AssignmentTask assignmentTask);
        Task UpdateAssignmentTaskAsync(AssignmentTask assignmentTask);
        Task<IList<SelectionItemList>> GetAllAssignorsAsync();
    }
    public partial class AssignmentTaskService : IAssignmentTaskService
    {
        private readonly IRepository<AssignmentTask> _assignmentTaskRepository;
        private readonly IRepository<Employee> _employeeRepository;

        public AssignmentTaskService(
            IRepository<AssignmentTask> assignmentTaskRepository,
            IRepository<Employee> employeeRepository)
        {
            _assignmentTaskRepository = assignmentTaskRepository;
            _employeeRepository = employeeRepository;
        }

        public virtual IQueryable<AssignmentTask> Table => _assignmentTaskRepository.Table;

        public virtual async Task<AssignmentTask> GetAssignmentTaskByIdAsync(int assignmentTaskId)
        {
            return await _assignmentTaskRepository.GetByIdAsync(assignmentTaskId);
        }

        public virtual async Task<IList<AssignmentTask>> GetAssignmentTasksByIdsAsync(int[] assignmentTaskIds)
        {
            return await _assignmentTaskRepository.GetByIdsAsync(assignmentTaskIds);
        }

        public virtual async Task<IList<AssignmentTask>> GetAllAssignmentTasksAsync()
        {
            var entities = await _assignmentTaskRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.Id);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteAssignmentTaskAsync(AssignmentTask assignmentTask)
        {
            await _assignmentTaskRepository.DeleteAsync(assignmentTask);
        }

        public virtual async Task DeleteAssignmentTaskAsync(IList<AssignmentTask> assignmentTasks)
        {
            await _assignmentTaskRepository.DeleteAsync(assignmentTasks);
        }

        public virtual async Task InsertAssignmentTaskAsync(AssignmentTask assignmentTask)
        {
            await _assignmentTaskRepository.InsertAsync(assignmentTask);
        }

        public virtual async Task UpdateAssignmentTaskAsync(AssignmentTask assignmentTask)
        {
            await _assignmentTaskRepository.UpdateAsync(assignmentTask);
        }

        public virtual async Task<IList<SelectionItemList>> GetAllAssignorsAsync()
        {
            var assignorIds = _assignmentTaskRepository.Table.Select(x => x.AssignorId).Distinct().ToArray();
            var employees = await _employeeRepository.GetByIdsAsync(assignorIds);

            var list = employees.Select(x => new SelectionItemList
            { Value = x.Id, Label = x.FullName() }).ToList();

            return list;
        }
    }
}
using App.Core.Domain.SimpleTask;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.SimpleTask
{
    public partial interface ISimpleTaskDepartmentService
    {
        IQueryable<SimpleTaskDepartment> Table { get; }
        Task<SimpleTaskDepartment> GetSimpleTaskDepartmentByIdAsync(int simpleTaskDepartmentId);
        Task<IList<SimpleTaskDepartment>> GetSimpleTaskDepartmentsByIdsAsync(int[] simpleTaskDepartmentIds);
        Task<IList<SimpleTaskDepartment>> GetAllSimpleTaskDepartmentsAsync();
        Task DeleteSimpleTaskDepartmentAsync(SimpleTaskDepartment simpleTaskDepartment);
        Task DeleteSimpleTaskDepartmentAsync(IList<SimpleTaskDepartment> simpleTaskDepartments);
        Task InsertSimpleTaskDepartmentAsync(SimpleTaskDepartment simpleTaskDepartment);
        Task UpdateSimpleTaskDepartmentAsync(SimpleTaskDepartment simpleTaskDepartment);
    }
    public partial class SimpleTaskDepartmentService : ISimpleTaskDepartmentService
    {
        private readonly IRepository<SimpleTaskDepartment> _simpleTaskDepartmentRepository;

        public SimpleTaskDepartmentService(
            IRepository<SimpleTaskDepartment> simpleTaskDepartmentRepository)
        {
            _simpleTaskDepartmentRepository = simpleTaskDepartmentRepository;
        }

        public virtual IQueryable<SimpleTaskDepartment> Table => _simpleTaskDepartmentRepository.Table;

        public virtual async Task<SimpleTaskDepartment> GetSimpleTaskDepartmentByIdAsync(int simpleTaskDepartmentId)
        {
            return await _simpleTaskDepartmentRepository.GetByIdAsync(simpleTaskDepartmentId);
        }

        public virtual async Task<IList<SimpleTaskDepartment>> GetSimpleTaskDepartmentsByIdsAsync(int[] simpleTaskDepartmentIds)
        {
            return await _simpleTaskDepartmentRepository.GetByIdsAsync(simpleTaskDepartmentIds);
        }

        public virtual async Task<IList<SimpleTaskDepartment>> GetAllSimpleTaskDepartmentsAsync()
        {
            var entities = await _simpleTaskDepartmentRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteSimpleTaskDepartmentAsync(SimpleTaskDepartment simpleTaskDepartment)
        {
            await _simpleTaskDepartmentRepository.DeleteAsync(simpleTaskDepartment);
        }

        public virtual async Task DeleteSimpleTaskDepartmentAsync(IList<SimpleTaskDepartment> simpleTaskDepartments)
        {
            await _simpleTaskDepartmentRepository.DeleteAsync(simpleTaskDepartments);
        }

        public virtual async Task InsertSimpleTaskDepartmentAsync(SimpleTaskDepartment simpleTaskDepartment)
        {
            await _simpleTaskDepartmentRepository.InsertAsync(simpleTaskDepartment);
        }

        public virtual async Task UpdateSimpleTaskDepartmentAsync(SimpleTaskDepartment simpleTaskDepartment)
        {
            await _simpleTaskDepartmentRepository.UpdateAsync(simpleTaskDepartment);
        }
    }
}
using App.Core.Domain.Employees;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Employees
{
    public partial interface IDepartmentService
    {
        IQueryable<Department> Table { get; }
        Task<Department> GetDepartmentBySystemNameAsync(string systemName);
        Task<Department> GetDepartmentByIdAsync(int departmentId);
        Task<IList<Department>> GetDepartmentsByIdsAsync(int[] departmentIds);
        Task<IList<Department>> GetAllDepartmentsAsync();
        Task DeleteDepartmentAsync(Department department);
        Task DeleteDepartmentAsync(IList<Department> departments);
        Task InsertDepartmentAsync(Department department);
        Task UpdateDepartmentAsync(Department department);
    }
    public partial class DepartmentService : IDepartmentService
    {
        private readonly IRepository<Department> _departmentRepository;

        public DepartmentService(
            IRepository<Department> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public virtual IQueryable<Department> Table => _departmentRepository.Table;

        public virtual async Task<Department> GetDepartmentByIdAsync(int departmentId)
        {
            return await _departmentRepository.GetByIdAsync(departmentId);
        }

        public virtual async Task<Department> GetDepartmentBySystemNameAsync(string systemName)
        {
            return await _departmentRepository.Table.FirstOrDefaultAsync(x => x.SystemName == systemName);
        }

        public virtual async Task<IList<Department>> GetDepartmentsByIdsAsync(int[] departmentIds)
        {
            return await _departmentRepository.GetByIdsAsync(departmentIds);
        }

        public virtual async Task<IList<Department>> GetAllDepartmentsAsync()
        {
            var entities = await _departmentRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.DisplayOrder);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteDepartmentAsync(Department department)
        {
            await _departmentRepository.DeleteAsync(department);
        }

        public virtual async Task DeleteDepartmentAsync(IList<Department> departments)
        {
            await _departmentRepository.DeleteAsync(departments);
        }

        public virtual async Task InsertDepartmentAsync(Department department)
        {
            await _departmentRepository.InsertAsync(department);
        }

        public virtual async Task UpdateDepartmentAsync(Department department)
        {
            await _departmentRepository.UpdateAsync(department);
        }
    }
}
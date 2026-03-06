using App.Core.Domain.Employees;
using App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Employees
{
    public partial interface IEmployeeService
    {
        IQueryable<Employee> Table { get; }
        Task<Employee> GetEmployeeByIdAsync(int employeeId);
        Task<IList<Employee>> GetEmployeesByIdsAsync(int[] employeeIds);
        Task<IList<Employee>> GetAllEmployeesAsync(bool showHidden = false);
        Task<IList<Employee>> GetEmployeesByDepartmentSystemNameAsync(string systemName, bool active = true);
        Task DeleteEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(IList<Employee> employees);
        Task InsertEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
    }
    public partial class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Department> _departmentRepository;

        public EmployeeService(
            IRepository<Employee> employeeRepository, 
            IRepository<Department> departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
        }

        public virtual IQueryable<Employee> Table => _employeeRepository.Table;

        public virtual async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            return await _employeeRepository.GetByIdAsync(employeeId);
        }

        public virtual async Task<IList<Employee>> GetEmployeesByIdsAsync(int[] employeeIds)
        {
            return await _employeeRepository.GetByIdsAsync(employeeIds);
        }

        public virtual async Task<IList<Employee>> GetAllEmployeesAsync(bool showHidden = false)
        {
            var entities = await _employeeRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.LastName);

                if (!showHidden)
                    query = query.Where(v => v.Active);

                return query;
            });

            return entities;
        }

        public virtual async Task<IList<Employee>> GetEmployeesByDepartmentSystemNameAsync(string systemName, bool active = true)
        {
            var department = await _departmentRepository.Table.FirstOrDefaultAsync(x => x.SystemName == systemName);
            if (department == null)
                throw new ArgumentNullException(nameof(systemName));

            var entities = await _employeeRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.DepartmentId == department.Id);

                if (active)
                    query = query.Where(v => v.Active);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteEmployeeAsync(Employee employee)
        {
            await _employeeRepository.DeleteAsync(employee);
        }

        public virtual async Task DeleteEmployeeAsync(IList<Employee> employees)
        {
            await _employeeRepository.DeleteAsync(employees);
        }

        public virtual async Task InsertEmployeeAsync(Employee employee)
        {
            await _employeeRepository.InsertAsync(employee);
        }

        public virtual async Task UpdateEmployeeAsync(Employee employee)
        {
            await _employeeRepository.UpdateAsync(employee);
        }
    }
}
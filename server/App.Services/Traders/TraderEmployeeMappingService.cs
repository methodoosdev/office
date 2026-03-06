using App.Core;
using App.Core.Domain.Employees;
using App.Core.Domain.Traders;
using App.Data;
using App.Models.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Traders
{
    public partial interface ITraderEmployeeMappingService
    {
        IQueryable<TraderEmployeeMapping> Table { get; }
        Task<IList<TraderEmployeeMapping>> GetAllTraderEmployeeMappingAsync();
        Task<IList<Trader>> GetTradersByEmployeeIdAsync(int employeeId);
        Task<string[]> GetEmployeesFullnameByTraderIdAsync(int traderId);
        Task<IList<TraderEmployeesNames>> GetEmployeesNamesAsync();
        Task<IList<Employee>> GetEmployeesByTraderIdAsync(int traderId);
        Task<IList<Employee>> GetEmployeesByTradersIdsAsync(int[] traderIds);
        Task RemoveTraderEmployeeAsync(Trader trader, Employee employee);
        Task InsertTraderEmployeeAsync(Trader trader, Employee employee);
        string GetEmployeeByTraderByDepartment(int traderId, int departmentId);
        Task<List<TraderEmployees>> GetPayrollTraderEmployeesAsync();
    }
    public partial class TraderEmployeeMappingService : ITraderEmployeeMappingService
    {
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Trader> _traderRepository;
        private readonly IRepository<TraderEmployeeMapping> _traderEmployeeMappingRepository;

        public TraderEmployeeMappingService(
            IRepository<Department> departmentRepository,
            IRepository<Employee> employeeRepository,
            IRepository<Trader> traderRepository,
            IRepository<TraderEmployeeMapping> traderEmployeeMappingRepository)
        {
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;
            _traderRepository = traderRepository;
            _traderEmployeeMappingRepository = traderEmployeeMappingRepository;
        }

        public virtual IQueryable<TraderEmployeeMapping> Table => _traderEmployeeMappingRepository.Table;

        public virtual async Task<IList<TraderEmployeeMapping>> GetAllTraderEmployeeMappingAsync()
        {
            var entities = await _traderEmployeeMappingRepository.GetAllAsync(query =>
            {
                return query;
            });

            return entities;
        }
        
        // Checked for decrypt
        public virtual async Task<IList<Trader>> GetTradersByEmployeeIdAsync(int employeeId)
        {
            return await _traderRepository.GetAllAsync(query =>
            {
                return from t in query
                       join tre in _traderEmployeeMappingRepository.Table on t.Id equals tre.TraderId
                       where tre.EmployeeId == employeeId
                       select t;
            });
        }

        public virtual async Task<IList<Employee>> GetEmployeesByTraderIdAsync(int traderId)
        {
            return await _employeeRepository.GetAllAsync(query =>
            {
                return from e in query
                       join tre in _traderEmployeeMappingRepository.Table on e.Id equals tre.EmployeeId
                       where tre.TraderId == traderId
                       select e;
            });
        }

        public virtual async Task<IList<Employee>> GetEmployeesByTradersIdsAsync(int[] traderIds)
        {
            return await _employeeRepository.GetAllAsync(query =>
            {
                return from e in query
                       join tre in _traderEmployeeMappingRepository.Table on e.Id equals tre.EmployeeId
                       where traderIds.Contains(tre.TraderId)
                       select e;
            });
        }

        public virtual async Task<string[]> GetEmployeesFullnameByTraderIdAsync(int traderId)
        {
            var employees = await _employeeRepository.GetAllAsync(query =>
            {
                return from e in query
                       join tre in _traderEmployeeMappingRepository.Table on e.Id equals tre.EmployeeId
                       where tre.TraderId == traderId
                       select e;
            });

            return employees.Select(x => x.FullName()).ToArray();
        }

        public virtual async Task<IList<TraderEmployeesNames>> _GetEmployeesNamesAsync()
        {
            var query =
                from t in _traderRepository.Table
                join trt in _traderEmployeeMappingRepository.Table
                    on t.Id equals trt.TraderId into traderMapping
                select new TraderEmployeesNames
                {
                    TraderId = t.Id,
                    //CompanyId = t.HyperPayrollId,
                    Employees = string.Join(", ",
                        from map in traderMapping
                        join e in _employeeRepository.Table
                            on map.EmployeeId equals e.Id
                        select e.FullName()
                    )
                };

            return await query.ToListAsync();
        }

        public virtual async Task<IList<TraderEmployeesNames>> __GetEmployeesNamesAsync()
        {
            var table = from t in _traderRepository.Table
                        join trt in _traderEmployeeMappingRepository.Table on t.Id equals trt.TraderId into traderMapping
                        select new
                        {
                            TraderId = t.Id,
                            Employees = from map in traderMapping
                                        join e in _employeeRepository.Table on map.EmployeeId equals e.Id
                                        select e
                        };

            var list = from t in table
                        select new TraderEmployeesNames
                        {
                            TraderId = t.TraderId,
                            //Employees = string.Join(", ", t.Employees.OrderBy(o => o.LastName).Select(x => x.FullName()).ToArray())
                        };

            return await list.ToListAsync();
        }

        public virtual async Task<IList<TraderEmployeesNames>> GetEmployeesNamesAsync()
        {
            var query = from te in _traderEmployeeMappingRepository.Table
                        join t in _traderRepository.Table on te.TraderId equals t.Id
                        join e in _employeeRepository.Table on te.EmployeeId equals e.Id
                        group e by new { t.Id, t.LastName } into g
                        select new TraderEmployeesNames
                        {
                            TraderId = g.Key.Id,
                            Employees = string.Join(", ", g.Select(x => x.LastName))
                        };

            return await query.ToListAsync();
        }

        public virtual async Task RemoveTraderEmployeeAsync(Trader trader, Employee employee)
        {
            if (trader == null)
                throw new ArgumentNullException(nameof(trader));

            if (employee is null)
                throw new ArgumentNullException(nameof(employee));

            if (await _traderEmployeeMappingRepository.Table
                .FirstOrDefaultAsync(m => m.EmployeeId == employee.Id && m.TraderId == trader.Id) is TraderEmployeeMapping mapping)
            {
                await _traderEmployeeMappingRepository.DeleteAsync(mapping);
            }
        }

        public virtual async Task InsertTraderEmployeeAsync(Trader trader, Employee employee)
        {
            if (trader is null)
                throw new ArgumentNullException(nameof(trader));

            if (employee is null)
                throw new ArgumentNullException(nameof(employee));

            if (await _traderEmployeeMappingRepository.Table.FirstOrDefaultAsync(m => m.EmployeeId == employee.Id && m.TraderId == trader.Id) is null)
            {
                var mapping = new TraderEmployeeMapping
                {
                    EmployeeId = employee.Id,
                    TraderId = trader.Id
                };

                await _traderEmployeeMappingRepository.InsertAsync(mapping);
            }
        }

        public virtual string GetEmployeeByTraderByDepartment(int traderId, int departmentId)
        {
            var query = from dept in _departmentRepository.Table
                        join emp in _employeeRepository.Table on dept.Id equals emp.DepartmentId
                        join mapping in _traderEmployeeMappingRepository.Table on emp.Id equals mapping.EmployeeId
                        join trader in _traderRepository.Table on mapping.TraderId equals trader.Id
                        where dept.Id == departmentId && trader.Id == traderId
                        select emp.LastName;

            return query.FirstOrDefault();
        }

        public virtual async Task<List<TraderEmployees>> GetPayrollTraderEmployeesAsync()
        {
            // Base query: all the rows we care about
            var baseQuery =
                from te in _traderEmployeeMappingRepository.Table
                join t in _traderRepository.Table on te.TraderId equals t.Id
                join e in _employeeRepository.Table on te.EmployeeId equals e.Id
                where t.HyperPayrollId > 0
                   && e.DepartmentId == 3
                select new
                {
                    TraderId = t.Id,
                    CompanyId = t.HyperPayrollId,
                    EmployeeId = e.Id,
                    EmployeeName = e.LastName
                };

            // Per-trader: find the *minimum* EmployeeId
            var minPerTrader =
                from x in baseQuery
                group x by new { x.TraderId, x.CompanyId } into g
                select new
                {
                    g.Key.TraderId,
                    g.Key.CompanyId,
                    MinEmployeeId = g.Min(p => p.EmployeeId)
                };

            // Join back to get the full employee info for that MinEmployeeId
            var query =
                from x in baseQuery
                join m in minPerTrader
                    on new { x.TraderId, x.CompanyId, x.EmployeeId }
                    equals new { m.TraderId, m.CompanyId, EmployeeId = m.MinEmployeeId }
                select new TraderEmployees
                {
                    TraderId = x.TraderId,
                    CompanyId = x.CompanyId,
                    EmployeeId = x.EmployeeId,
                    EmployeeName = x.EmployeeName
                };

            return await query.ToListAsync();
        }

        public virtual async Task<List<TraderEmployees>> _GetPayrollTraderEmployeesAsync()
        {
            // 1. Build the DB query (no Dictionary yet)
            var query =
                from t in _traderRepository.Table
                where t.HyperPayrollId > 0
                join m in _traderEmployeeMappingRepository.Table on t.Id equals m.TraderId
                join e in _employeeRepository.Table on m.EmployeeId equals e.Id
                where e.DepartmentId == 3
                group new { e.Id, e.FirstName, e.LastName } by new { t.Id, t.HyperPayrollId } into g
                select new
                {
                    TraderId = g.Key.Id,
                    CompanyId = g.Key.HyperPayrollId,
                    Employees = g
                        .OrderBy(x => x.LastName)
                        .Select(x => new
                        {
                            x.Id,
                            Name = x.FirstName + " " + x.LastName
                        })
                };

            // 2. Materialize and build the Dictionary on the client
            return await query
                .AsEnumerable()
                .Select(x => new TraderEmployees
                {
                    TraderId = x.TraderId,
                    CompanyId = x.CompanyId,
                    Employees = x.Employees.ToDictionary(
                        e => e.Id,
                        e => e.Name)
                })
                .ToListAsync();
        }
    }
}
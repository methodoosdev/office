using App.Core;
using App.Core.Domain.Employees;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Core.Infrastructure.Mapper;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Traders;
using App.Services;
using App.Services.Employees;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Infra.Queries.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Traders
{
    public partial interface ITraderRatingReportModelFactory
    {
        IList<TraderRatingByEmployeeModel> GetByEmployeeList();
        IList<ColumnConfig> GetByEmployeeColumnsConfig();
        IList<TraderRatingByDepartmentModel> GetByDepartmentList();
        IList<ColumnConfig> GetByDepartmentColumnsConfig(bool hidden = false);
        Task<IList<TraderRatingByTraderModel>> GetByTraderListAsync(string connection);
        IList<ColumnConfig> GetByTraderColumnsConfig();
        IList<ColumnConfig> SummaryTableModelColumnsConfig();
        Task<IList<SummaryTableModel>> SummaryTableModelListAsync(string connection);
        IList<ColumnConfig> ValuationTraderResultColumnsConfig();
        Task<IList<ValuationTraderResult>> ValuationTraderResultListAsync(string connection);
        IList<ColumnConfig> ValuationTableModelColumnsConfig();
        Task<IList<ValuationTableResult>> ValuationTableModelListAsync(string connection);
    }
    public partial class TraderRatingReportModelFactory : ITraderRatingReportModelFactory
    {
        private readonly ITraderService _traderService;
        private readonly IEmployeeService _employeeService;
        private readonly ITraderEmployeeMappingService _traderEmployeeMappingService;
        private readonly ITraderRatingCategoryService _traderRatingCategoryService;
        private readonly ITraderRatingService _traderRatingService;
        private readonly ITraderRatingTraderMappingService _traderRatingTraderMappingService;
        private readonly IDepartmentService _departmentService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public TraderRatingReportModelFactory(
            ITraderService traderService,
            IEmployeeService employeeService,
            ITraderEmployeeMappingService traderEmployeeMappingService,
            ITraderRatingCategoryService traderRatingCategoryService,
            ITraderRatingService traderRatingService,
            ITraderRatingTraderMappingService traderRatingTraderMappingService,
            IDepartmentService departmentService,
            IAppDataProvider dataProvider,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _employeeService = employeeService;
            _traderEmployeeMappingService = traderEmployeeMappingService;
            _traderRatingCategoryService = traderRatingCategoryService;
            _traderRatingService = traderRatingService;
            _traderRatingTraderMappingService = traderRatingTraderMappingService;
            _departmentService = departmentService;
            _dataProvider = dataProvider;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public IList<TraderRatingByEmployeeModel> GetByEmployeeList()
        {
            var list =
                (from g in _traderRatingService.Table
                 join tm in _traderRatingTraderMappingService.Table on g.Id equals tm.TraderRatingId
                 join t in _traderService.Table on tm.TraderId equals t.Id
                 join c in _traderRatingCategoryService.Table on g.TraderRatingCategoryId equals c.Id
                 join e in _employeeService.Table on g.DepartmentId equals e.DepartmentId
                 join em in _traderEmployeeMappingService.Table on new { eId = e.Id, tId = tm.TraderId } equals new { eId = em.EmployeeId, tId = em.TraderId }
                 join d in _departmentService.Table on e.DepartmentId equals d.Id
                 group new { g, t, e, t.CategoryBookTypeId, e.DepartmentId } by e.LastName into grouped
                 select new TraderRatingByEmployeeModel
                 {
                     Employee = grouped.Key,
                     Value = grouped.Sum(x => x.g.Gravity),
                     TraderCount = grouped.Select(x => x.t.Id).Distinct().Count(),
                     CategoryBookTypeId = grouped.Select(x => x.CategoryBookTypeId).FirstOrDefault(),
                     DepartmentId = grouped.Select(x => x.DepartmentId.HasValue ? x.DepartmentId.Value : 0).FirstOrDefault()

                 }).ToList();

            return list;
        }

        public IList<ColumnConfig> GetByEmployeeColumnsConfig()
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderRatingByEmployeeModel>(1, nameof(TraderRatingByEmployeeModel.Employee)),
                ColumnConfig.Create<TraderRatingByEmployeeModel>(2, nameof(TraderRatingByEmployeeModel.Value), ColumnType.Numeric, style: rightAlign),
                ColumnConfig.Create<TraderRatingByEmployeeModel>(3, nameof(TraderRatingByEmployeeModel.TraderCount), ColumnType.Numeric, style: rightAlign)
            };

            return columns;
        }

        public IList<TraderRatingByDepartmentModel> GetByDepartmentList()
        {
            var list =
                (from g in _traderRatingService.Table
                 join tm in _traderRatingTraderMappingService.Table on g.Id equals tm.TraderRatingId
                 join t in _traderService.Table on tm.TraderId equals t.Id
                 join c in _traderRatingCategoryService.Table on g.TraderRatingCategoryId equals c.Id
                 join e in _employeeService.Table on g.DepartmentId equals e.DepartmentId
                 join em in _traderEmployeeMappingService.Table on new { eId = e.Id, tId = tm.TraderId } equals new { eId = em.EmployeeId, tId = em.TraderId }
                 join d in _departmentService.Table on e.DepartmentId equals d.Id
                 select new TraderRatingByDepartmentModel
                 {
                     Trader = ($"{AesEncryption.Decrypt(t.LastName)} {AesEncryption.Decrypt(t.FirstName)}").ToSubstring(39),
                     Department = d.Description,
                     Category = c.Description,
                     Gravity = g.Description,
                     Employee = e.LastName,
                     Value = g.Gravity
                 }).ToList();

            var i = 1;
            list.ForEach(x => x.Id = i++);

            return list;
        }

        public IList<ColumnConfig> GetByDepartmentColumnsConfig(bool hidden = false)
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderRatingByDepartmentModel>(1, nameof(TraderRatingByDepartmentModel.Department), hidden: hidden),
                ColumnConfig.Create<TraderRatingByDepartmentModel>(2, nameof(TraderRatingByDepartmentModel.Category), hidden: hidden),
                ColumnConfig.Create<TraderRatingByDepartmentModel>(3, nameof(TraderRatingByDepartmentModel.Gravity), hidden: hidden),
                ColumnConfig.Create<TraderRatingByDepartmentModel>(4, nameof(TraderRatingByDepartmentModel.Trader)),
                ColumnConfig.Create<TraderRatingByDepartmentModel>(5, nameof(TraderRatingByDepartmentModel.Employee), hidden : hidden),
                ColumnConfig.Create<TraderRatingByDepartmentModel>(6, nameof(TraderRatingByDepartmentModel.Value), ColumnType.Numeric, style: rightAlign)
            };

            return columns;
        }

        public async Task<IList<TraderRatingByTraderModel>> GetByTraderListAsync(string connection)
        {
            //var list =
            //    (from g in _traderRatingService.Table
            //     join tm in _traderRatingTraderMappingService.Table on g.Id equals tm.TraderRatingId
            //     join t in _traderService.Table on tm.TraderId equals t.Id
            //     join c in _traderRatingCategoryService.Table on g.TraderRatingCategoryId equals c.Id
            //     join e in _employeeService.Table on g.DepartmentId equals e.DepartmentId
            //     join em in _traderEmployeeMappingService.Table on new { eId = e.Id, tId = tm.TraderId } equals new { eId = em.EmployeeId, tId = em.TraderId }
            //     join d in _departmentService.Table on e.DepartmentId equals d.Id
            //     group new { e, t, g } by new { EmployeeName = e.LastName + " " + e.FirstName, TraderName = t.LastName + " " + t.FirstName } into grouped
            //     select new TraderRatingByTraderModel
            //     {
            //         Employee = grouped.Key.EmployeeName,
            //         Trader = grouped.Key.TraderName,
            //         Value = grouped.Sum(item => item.g.Gravity)
            //     }).ToList();

            var list = await _dataProvider.QueryAsync<TraderRatingByTraderModel>(connection, OfficeDBQuery.TraderRatingByTrader);

            foreach (var item in list)
            {
                var name = item.Trader.Split(' ');
                item.Trader = $"{AesEncryption.Decrypt(name[0])} {AesEncryption.Decrypt(name[1])}";
            }

            return list;
        }

        public IList<ColumnConfig> GetByTraderColumnsConfig()
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderRatingByTraderModel>(1, nameof(TraderRatingByTraderModel.Employee)),
                ColumnConfig.Create<TraderRatingByTraderModel>(2, nameof(TraderRatingByTraderModel.Trader), hidden: true),
                ColumnConfig.Create<TraderRatingByTraderModel>(3, nameof(TraderRatingByTraderModel.Value), ColumnType.Numeric, style: rightAlign)
            };

            return columns;
        }

        public IList<ColumnConfig> SummaryTableModelColumnsConfig()
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<SummaryTableModel>(1, nameof(SummaryTableModel.Vat)),
                ColumnConfig.Create<SummaryTableModel>(2, nameof(SummaryTableModel.Trader)),
                ColumnConfig.Create<SummaryTableModel>(2, nameof(SummaryTableModel.CategoryBook)),
                ColumnConfig.Create<SummaryTableModel>(3, nameof(SummaryTableModel.Employee_Dep2), filterable: true),
                ColumnConfig.Create<SummaryTableModel>(3, nameof(SummaryTableModel.Gravity_Dep2), ColumnType.Numeric, style: rightAlign),
                ColumnConfig.Create<SummaryTableModel>(3, nameof(SummaryTableModel.TotalGravity_Dep2), ColumnType.Numeric, style: rightAlign),
                ColumnConfig.Create<SummaryTableModel>(3, nameof(SummaryTableModel.Employee_Dep3)),
                ColumnConfig.Create<SummaryTableModel>(3, nameof(SummaryTableModel.Gravity_Dep3), ColumnType.Numeric, style: rightAlign),
                ColumnConfig.Create<SummaryTableModel>(3, nameof(SummaryTableModel.TotalGravity_Dep3), ColumnType.Numeric, style: rightAlign)
            };

            return columns;
        }

        public async Task<IList<SummaryTableModel>> SummaryTableModelListAsync(string connection)
        {
            var list = await _dataProvider.QueryAsync<SummaryTableModel>(connection, OfficeDBQuery.SummaryTableQuery);
            var categoryBookTypes = await CategoryBookType.None.ToSelectionItemListAsync();

            foreach (var item in list)
            {
                var name = item.Trader.Split(' ');
                item.Vat = AesEncryption.Decrypt(item.Vat);
                item.Trader = $"{AesEncryption.Decrypt(name[0])} {AesEncryption.Decrypt(name[1])}";
                item.CategoryBook = categoryBookTypes.FirstOrDefault(a => a.Value == item.CategoryBookTypeId)?.Label ?? "";
            }

            return list.OrderBy(x => x.Trader).ToList();
        }

        public IList<ColumnConfig> ValuationTraderResultColumnsConfig()
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ValuationTraderResult>(1, nameof(ValuationTraderResult.Trader), filterable: true),
                ColumnConfig.Create<ValuationTraderResult>(1, nameof(ValuationTraderResult.Income), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<ValuationTraderResult>(1, nameof(ValuationTraderResult.Expences), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<ValuationTraderResult>(1, nameof(ValuationTraderResult.Total), ColumnType.Decimal, style : rightAlign),
                ColumnConfig.Create<ValuationTraderResult>(1, nameof(ValuationTraderResult.TotalRate), ColumnType.Percent, style : rightAlign),
            };

            return columns;
        }

        public async Task<IList<ValuationTraderResult>> ValuationTraderResultListAsync(string connection)
        {
            var list = await _dataProvider.QueryAsync<ValuationTableModel>(connection, OfficeDBQuery.ValuationTableQuery);

            var result = new List<ValuationTraderResult>();

            foreach (var item1 in list)
            {
                var name = item1.Trader.Split(' ');
                item1.Vat = AesEncryption.Decrypt(item1.Vat);
                item1.Trader = $"{AesEncryption.Decrypt(name[0])} {AesEncryption.Decrypt(name[1])}";
            }

            foreach (var item in list)
            {
                var income = item.TraderPayment;

                var cost2 = SafeDivide((item.EmployeeSalary_Dep2 * item.Gravity_Dep2), item.TotalGravity_Dep2);
                var cost3 = SafeDivide((item.EmployeeSalary_Dep3 * item.Gravity_Dep3), item.TotalGravity_Dep3);
                var cost7 = SafeDivide((item.EmployeeSalary_Dep7 * item.Gravity_Dep7), item.TotalGravity_Dep7);
                var cost8 = SafeDivide((item.EmployeeSalary_Dep8 * item.Gravity_Dep8), item.TotalGravity_Dep8);

                var cost = cost2 + cost3 + cost7 + cost8;
                var total = income - cost;

                result.Add(new ValuationTraderResult 
                {
                    Trader = item.Trader,
                    Income = income,
                    Expences = cost,
                    Total = total,
                    TotalRate = SafeDivide(total * 100, income)
                });
            }

            return result;
        }

        public IList<ColumnConfig> ValuationTableModelColumnsConfig()
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ValuationTableResult>(2, nameof(ValuationTableResult.Employee), filterable: true),
                ColumnConfig.Create<ValuationTableResult>(1, nameof(ValuationTableResult.Turnover), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<ValuationTableResult>(1, nameof(ValuationTableResult.TurnoverRate), ColumnType.Percent, style: rightAlign),
                ColumnConfig.Create<ValuationTableResult>(2, nameof(ValuationTableResult.SpecialtyName), hidden: true),
                ColumnConfig.Create<ValuationTableResult>(1, nameof(ValuationTableResult.SalaryDep2), ColumnType.Decimal, style : rightAlign),
                ColumnConfig.Create<ValuationTableResult>(1, nameof(ValuationTableResult.RateDep2), ColumnType.Percent, style : rightAlign),
                ColumnConfig.Create<ValuationTableResult>(1, nameof(ValuationTableResult.SalaryDep3), ColumnType.Decimal, style : rightAlign),
                ColumnConfig.Create<ValuationTableResult>(1, nameof(ValuationTableResult.RateDep3), ColumnType.Percent, style : rightAlign),
                ColumnConfig.Create<ValuationTableResult>(1, nameof(ValuationTableResult.SalaryDep7), ColumnType.Decimal, style : rightAlign),
                ColumnConfig.Create<ValuationTableResult>(1, nameof(ValuationTableResult.RateDep7), ColumnType.Percent, style : rightAlign),
                ColumnConfig.Create<ValuationTableResult>(1, nameof(ValuationTableResult.SalaryDep8), ColumnType.Decimal, style : rightAlign),
                ColumnConfig.Create<ValuationTableResult>(1, nameof(ValuationTableResult.RateDep8), ColumnType.Percent, style : rightAlign),
                ColumnConfig.Create<ValuationTableResult>(1, nameof(ValuationTableResult.Total), ColumnType.Decimal, style : rightAlign),
                ColumnConfig.Create<ValuationTableResult>(1, nameof(ValuationTableResult.TotalRate), ColumnType.Percent, style : rightAlign),
            };

            return columns;
        }

        public async Task<IList<ValuationTableResult>> ValuationTableModelListAsync(string connection)
        {
            var list = await _dataProvider.QueryAsync<ValuationTableModel>(connection, OfficeDBQuery.ValuationTableQuery);

            foreach (var item1 in list)
            {
                var name = item1.Trader.Split(' ');
                item1.Vat = AesEncryption.Decrypt(item1.Vat);
                item1.Trader = $"{AesEncryption.Decrypt(name[0])} {AesEncryption.Decrypt(name[1])}";
            }

            var employees = list.Select(x => x.Employee_Dep2).Distinct().ToList();
            var specialties = list.Select(x => x.SpecialtyName).Distinct().ToList();
            var totalTurnover = list.Sum(x => x.TraderPayment);

            var costCenterList = new List<ValuationTableResult>();

            foreach (var _employee in employees)
            {
                var accountantList = list.Where(x => x.Employee_Dep2.Contains(_employee)).ToList();

                foreach (var accountant in accountantList)
                {
                    var partialSalary2 = SafeDivide((accountant.Gravity_Dep2 * accountant.EmployeeSalary_Dep2), accountant.TotalGravity_Dep2);
                    //var rateSalary2 = SafeDivide((partialSalary2 * 100), accountant.EmployeeSalary_Dep2);

                    var partialSalary3 = SafeDivide((accountant.Gravity_Dep3 * accountant.EmployeeSalary_Dep3), accountant.TotalGravity_Dep3);
                    //var rateSalary3 = SafeDivide((partialSalary3 * 100), accountant.EmployeeSalary_Dep3);

                    var partialSalary7 = SafeDivide((accountant.Gravity_Dep7 * accountant.EmployeeSalary_Dep7), accountant.TotalGravity_Dep7);
                    //var rateSalary7 = SafeDivide((partialSalary7 * 100), accountant.EmployeeSalary_Dep7);

                    var partialSalary8 = SafeDivide((accountant.Gravity_Dep8 * accountant.EmployeeSalary_Dep8), accountant.TotalGravity_Dep8);
                    //var rateSalary8 = SafeDivide((partialSalary8 * 100), accountant.EmployeeSalary_Dep8);

                    costCenterList.Add(new ValuationTableResult
                    {
                        TraderPayment = accountant.TraderPayment,
                        Employee = accountant.Employee_Dep2,
                        SpecialtyName = accountant.SpecialtyName,
                        SalaryDep2 = partialSalary2,
                        //RateDep2 = rateSalary2,
                        SalaryDep3 = partialSalary3,
                        //RateDep3 = rateSalary3,
                        SalaryDep7 = partialSalary7,
                        //RateDep7 = rateSalary7,
                        SalaryDep8 = partialSalary8,
                        //RateDep8 = rateSalary8
                    });
                }
            }

            var valuations = new List<ValuationTableResult>();

            foreach (var specialty in specialties)
            {
                var turnoverSpecialty = costCenterList.Where(x => x.SpecialtyName == specialty).Sum(x => x.TraderPayment);


                foreach (var employee in employees)
                {
                    var accountantList = costCenterList.Where(x => x.Employee.Contains(employee) && x.SpecialtyName == specialty).ToList();

                    if (accountantList.Count == 0)
                        continue;

                    var turnover = accountantList.Sum(x => x.TraderPayment);
                    var traderPayment = accountantList.Max(x => x.TraderPayment);

                    var partialSalary2 = accountantList.Sum(x => x.SalaryDep2);
                    var rateSalary2 = SafeDivide(partialSalary2 * 100, turnover);

                    var partialSalary3 = accountantList.Sum(x => x.SalaryDep3);
                    var rateSalary3 = SafeDivide(partialSalary3 * 100, turnover);

                    var partialSalary7 = accountantList.Sum(x => x.SalaryDep7);
                    var rateSalary7 = SafeDivide(partialSalary7 * 100, turnover);

                    var partialSalary8 = accountantList.Sum(x => x.SalaryDep8);
                    var rateSalary8 = SafeDivide(partialSalary8 * 100, turnover);

                    var totalSalary = partialSalary2 + partialSalary3 + partialSalary7 + partialSalary8;
                    //var totalSalaryRate = rateSalary2 + rateSalary3 + rateSalary7 + rateSalary8;

                    var total = turnover - totalSalary;
                    var totalRate = SafeDivide(total * 100, turnover);

                    valuations.Add(new ValuationTableResult
                    {
                        Turnover = turnover,
                        TurnoverRate = SafeDivide(turnover * 100, turnoverSpecialty, 2),
                        Employee = accountantList.FirstOrDefault()?.Employee,
                        SpecialtyName = accountantList.FirstOrDefault()?.SpecialtyName,
                        SalaryDep2 = Round(partialSalary2, 2),
                        RateDep2 = Round(rateSalary2, 2),
                        SalaryDep3 = Round(partialSalary3, 2),
                        RateDep3 = Round(rateSalary3, 2),
                        SalaryDep7 = Round(partialSalary7, 2),
                        RateDep7 = Round(rateSalary7, 2),
                        SalaryDep8 = Round(partialSalary8, 2),
                        RateDep8 = Round(rateSalary8, 2),
                        Total = Round(total, 2),
                        TotalRate = Round(totalRate, 2)
                    });
                }
            }

            return valuations;
        }
        /*
        public async Task<IList<ValuationTableResult>> ValuationTableModelList2Async(string connection)
        {
            var list = await _dataProvider.QueryAsync<ValuationTableModel>(connection, OfficeDBQuery.ValuationTableQuery);
            var categoryBookTypes = await CategoryBookType.None.ToSelectionItemListAsync();

            foreach (var item1 in list)
            {
                var name = item1.Trader.Split(' ');
                item1.Vat = AesEncryption.Decrypt(item1.Vat);
                item1.Trader = $"{AesEncryption.Decrypt(name[0])} {AesEncryption.Decrypt(name[1])}";
            }

            var employees = list.Select(x => x.Employee_Dep2).Distinct().ToList();
            var categoryBookTypeIds = list.Select(x => x.CategoryBookTypeId).Distinct().ToList();
            var totalTurnover = list.Sum(x => x.TraderPayment);

            var costCenterList = new List<ValuationTableResult>();

            foreach (var _employee in employees)
            {
                var accountantList = list.Where(x => x.Employee_Dep2.Contains(_employee)).ToList();

                foreach (var accountant in accountantList)
                {
                    var partialSalary2 = SafeDivide((accountant.Gravity_Dep2 * accountant.EmployeeSalary_Dep2), accountant.TotalGravity_Dep2);
                    var rateSalary2 = SafeDivide((partialSalary2 * 100), accountant.EmployeeSalary_Dep2);

                    var partialSalary3 = SafeDivide((accountant.Gravity_Dep3 * accountant.EmployeeSalary_Dep3), accountant.TotalGravity_Dep3);
                    var rateSalary3 = SafeDivide((partialSalary3 * 100), accountant.EmployeeSalary_Dep3);

                    var partialSalary7 = SafeDivide((accountant.Gravity_Dep7 * accountant.EmployeeSalary_Dep7), accountant.TotalGravity_Dep7);
                    var rateSalary7 = SafeDivide((partialSalary7 * 100), accountant.EmployeeSalary_Dep7);

                    var partialSalary8 = SafeDivide((accountant.Gravity_Dep8 * accountant.EmployeeSalary_Dep8), accountant.TotalGravity_Dep8);
                    var rateSalary8 = SafeDivide((partialSalary8 * 100), accountant.EmployeeSalary_Dep8);

                    costCenterList.Add(new ValuationTableResult
                    {
                        TraderPayment = accountant.TraderPayment,
                        Employee = accountant.Employee_Dep2,
                        CategoryBookTypeId = accountant.CategoryBookTypeId,
                        SalaryDep2 = partialSalary2,
                        RateDep2 = rateSalary2,
                        SalaryDep3 = partialSalary3,
                        RateDep3 = rateSalary3,
                        SalaryDep7 = partialSalary7,
                        RateDep7 = rateSalary7,
                        SalaryDep8 = partialSalary8,
                        RateDep8 = rateSalary8
                    });
                }
            }

            var valuations = new List<ValuationTableResult>();

            foreach (var categoryBookTypeId in categoryBookTypeIds)
            {
                var turnoverByCategoryBook = costCenterList.Where(x => x.CategoryBookTypeId == categoryBookTypeId).Sum(x => x.TraderPayment);
                foreach (var employee in employees)
                {
                    var accountantList = costCenterList.Where(x => x.Employee.Contains(employee) && x.CategoryBookTypeId == categoryBookTypeId).ToList();

                    if (accountantList.Count == 0)
                        continue;

                    var traderPayment = accountantList.Max(x => x.TraderPayment);

                    var partialSalary2 = accountantList.Sum(x => x.SalaryDep2);
                    var rateSalary2 = accountantList.Sum(x => x.RateDep2);

                    var partialSalary3 = accountantList.Sum(x => x.SalaryDep3);
                    var rateSalary3 = accountantList.Sum(x => x.RateDep3);

                    var partialSalary7 = accountantList.Sum(x => x.SalaryDep7);
                    var rateSalary7 = accountantList.Sum(x => x.RateDep7);

                    var partialSalary8 = accountantList.Sum(x => x.SalaryDep8);
                    var rateSalary8 = accountantList.Sum(x => x.RateDep8);

                    var totalSalary = partialSalary2 + partialSalary3 + partialSalary7 + partialSalary8;  //var totalSalaryRate = rateSalary2 + rateSalary3 + rateSalary7 + rateSalary8;

                    var turnover = accountantList.Sum(x => x.TraderPayment);
                    var total = turnover - totalSalary;
                    var totalRate = SafeDivide(total * 100, turnover);

                    valuations.Add(new ValuationTableResult
                    {
                        Turnover = turnover,
                        TurnoverRate = SafeDivide(turnover * 100, turnoverByCategoryBook, 2),
                        TotalTurnoverRate = SafeDivide(turnover * 100, totalTurnover, 2),
                        Employee = accountantList.FirstOrDefault()?.Employee,
                        CategoryBookTypeId = categoryBookTypeId,
                        CategoryBook = categoryBookTypes.FirstOrDefault(a => a.Value == categoryBookTypeId)?.Label ?? "Κενό",
                        SalaryDep2 = Round(partialSalary2, 2),
                        RateDep2 = Round(rateSalary2, 2),
                        SalaryDep3 = Round(partialSalary3, 2),
                        RateDep3 = Round(rateSalary3, 2),
                        SalaryDep7 = Round(partialSalary7, 2),
                        RateDep7 = Round(rateSalary7, 2),
                        SalaryDep8 = Round(partialSalary8, 2),
                        RateDep8 = Round(rateSalary8, 2),
                        Total = Round(total, 2),
                        TotalRate = Round(totalRate, 2)
                    });
                }
            }

            return valuations;
        }
        */
        private decimal Round(decimal value, int decimals = 18, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return Math.Round(value, decimals, mode);
        }
        private decimal SafeDivide(decimal numerator, decimal denominator, int decimals = 2, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            if (denominator == 0)
                return 0;

            return Round((numerator / denominator), decimals, mode);
        }

    }
}
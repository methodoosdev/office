using App.Core;
using App.Core.Domain.Employees;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Employees;
using App.Services;
using App.Services.Customers;
using App.Services.Employees;
using App.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Employees
{
    public partial interface IEmployeeModelFactory
    {
        Task<EmployeeSearchModel> PrepareEmployeeSearchModelAsync(EmployeeSearchModel searchModel);
        Task<EmployeeListModel> PrepareEmployeeListModelAsync(EmployeeSearchModel searchModel);
        Task<EmployeeModel> PrepareEmployeeModelAsync(EmployeeModel model, Employee employee);
        Task<EmployeeFormModel> PrepareEmployeeFormModelAsync(EmployeeFormModel formModel, EmployeeModel model);
    }
    public partial class EmployeeModelFactory : IEmployeeModelFactory
    {
        private readonly IModelFactoryService _modelFactoryService;
        private readonly IEmployeeService _employeeService;
        private readonly ICustomerService _customerService;
        private readonly IEducationService _educationService;
        private readonly IDepartmentService _departmentService;
        private readonly ISpecialtyService _specialtyService;
        private readonly IJobTitleService _jobTitleService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public EmployeeModelFactory(
            IModelFactoryService modelFactoryService,
            IEmployeeService employeeService,
            ICustomerService customerService,
            IEducationService educationService,
            IDepartmentService departmentService,
            ISpecialtyService specialtyService,
            IJobTitleService jobTitleService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _modelFactoryService = modelFactoryService;
            _employeeService = employeeService;
            _customerService = customerService;
            _educationService = educationService;
            _departmentService = departmentService;
            _specialtyService = specialtyService;
            _jobTitleService = jobTitleService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<EmployeeModel>> GetPagedListAsync(EmployeeSearchModel searchModel)
        {
            var query = (from e in _employeeService.Table.AsEnumerable()
                         from er in _educationService.Table.AsEnumerable().Where(x => x.Id == e.EducationId).DefaultIfEmpty()
                         from dr in _departmentService.Table.AsEnumerable().Where(x => x.Id == e.DepartmentId).DefaultIfEmpty()
                         from sr in _specialtyService.Table.AsEnumerable().Where(x => x.Id == e.SpecialtyId).DefaultIfEmpty()
                         from jr in _jobTitleService.Table.AsEnumerable().Where(x => x.Id == e.JobTitleId).DefaultIfEmpty()
                         from sv in _employeeService.Table.AsEnumerable().Where(x => x.Id == e.SupervisorId).DefaultIfEmpty()
                         select new EmployeeModel
                         {
                             Id = e.Id,
                             EmployeeSalary = e.EmployeeSalary,
                             FullName = e.FullName() ?? "",
                             FatherName = e.FatherName ?? "",
                             EmailContact = e.EmailContact ?? "",
                             Mobile = e.Mobile ?? "",
                             InternalPhoneNumber = e.InternalPhoneNumber ?? "",
                             EducationName = er?.Description ?? "",
                             DepartmentName = dr?.Description ?? "",
                             SpecialtyName = sr?.Description ?? "",
                             JobTitleName = jr?.Description ?? "",
                             SupervisorName = sv?.FullName() ?? ""
                         }).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.FullName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.EmailContact.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Mobile.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.EducationName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.DepartmentName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.SpecialtyName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.JobTitleName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.SupervisorName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<EmployeeSearchModel> PrepareEmployeeSearchModelAsync(EmployeeSearchModel searchModel)
        {
            //prepare page parameters
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.EmployeeModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<EmployeeListModel> PrepareEmployeeListModelAsync(EmployeeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var employees = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new EmployeeListModel().PrepareToGrid(searchModel, employees);

            return model;
        }

        public virtual Task<EmployeeModel> PrepareEmployeeModelAsync(EmployeeModel model, Employee employee)
        {
            if (employee != null)
            {
                //fill in model values from the entity
                model ??= employee.ToModel<EmployeeModel>();
            }

            //set default values for the new model
            if (employee == null)
                model.Active = true;

            return Task.FromResult(model);
        }

        private async Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<EmployeeModel>(1, nameof(EmployeeModel.FullName), ColumnType.Link, link: "/office/employee"),
                ColumnConfig.Create<EmployeeModel>(2, nameof(EmployeeModel.FatherName), hidden: true),
                ColumnConfig.Create<EmployeeModel>(3, nameof(EmployeeModel.EmailContact)),
                ColumnConfig.Create<EmployeeModel>(4, nameof(EmployeeModel.Mobile)),
                ColumnConfig.Create<EmployeeModel>(5, nameof(EmployeeModel.SupervisorName)),
                ColumnConfig.Create<EmployeeModel>(6, nameof(EmployeeModel.DepartmentName)),
                ColumnConfig.Create<EmployeeModel>(7, nameof(EmployeeModel.EducationName), hidden: true),
                ColumnConfig.Create<EmployeeModel>(8, nameof(EmployeeModel.JobTitleName), hidden: true),
                ColumnConfig.Create<EmployeeModel>(9, nameof(EmployeeModel.SpecialtyName), hidden: true)
            };

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsAdminAsync(customer) || await _customerService.IsOfficeAsync(customer))
                columns.Add(ColumnConfig.Create<EmployeeModel>(10, nameof(EmployeeModel.EmployeeSalary), ColumnType.Numeric));

            return columns;
        }

        public virtual async Task<EmployeeFormModel> PrepareEmployeeFormModelAsync(EmployeeFormModel formModel, EmployeeModel model)
        {
            var departments = await _modelFactoryService.GetAllDepartmentsAsync();
            var educations = await _modelFactoryService.GetAllEducationsAsync();
            var specialties = await _modelFactoryService.GetAllSpecialtiesAsync();
            var jobTitles = await _modelFactoryService.GetAllJobTitlesAsync();
            var supervisors = await _modelFactoryService.GetAllSupervisorsAsync();

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmployeeModel>(nameof(EmployeeModel.LastName), FieldType.Text),
                FieldConfig.Create<EmployeeModel>(nameof(EmployeeModel.FirstName), FieldType.Text),
                FieldConfig.Create<EmployeeModel>(nameof(EmployeeModel.FatherName), FieldType.Text),
                FieldConfig.Create<EmployeeModel>(nameof(EmployeeModel.EmailContact), FieldType.Text),
                FieldConfig.Create<EmployeeModel>(nameof(EmployeeModel.Mobile), FieldType.Text),
                FieldConfig.Create<EmployeeModel>(nameof(EmployeeModel.InternalPhoneNumber), FieldType.Text),
                FieldConfig.Create<EmployeeModel>(nameof(EmployeeModel.PayrollInfoEmail), FieldType.Checkbox),
                FieldConfig.Create<EmployeeModel>(nameof(EmployeeModel.Active), FieldType.Checkbox),
                FieldConfig.Create<EmployeeModel>(nameof(EmployeeModel.DepartmentId), FieldType.Select, options: departments),
                FieldConfig.Create<EmployeeModel>(nameof(EmployeeModel.EducationId), FieldType.Select, options: educations),
                FieldConfig.Create<EmployeeModel>(nameof(EmployeeModel.SpecialtyId), FieldType.Select, options: specialties),
                FieldConfig.Create<EmployeeModel>(nameof(EmployeeModel.JobTitleId), FieldType.Select, options: jobTitles),
                FieldConfig.Create<EmployeeModel>(nameof(EmployeeModel.SupervisorId), FieldType.Select, options: supervisors)
            };

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsAdminAsync(customer) || await _customerService.IsOfficeAsync(customer))
                fields.Insert(2, FieldConfig.Create<EmployeeModel>(nameof(EmployeeModel.EmployeeSalary), FieldType.Numeric));

            var panels = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Common.About"), true, "col-12 md:col-6", fields)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.EmployeeModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", panels);

            return formModel;
        }
    }
}
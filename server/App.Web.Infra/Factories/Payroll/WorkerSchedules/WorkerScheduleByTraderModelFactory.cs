using App.Core;
using App.Core.Domain.Payroll;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Payroll;
using App.Services;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Payroll;
using App.Services.Traders;
using App.Web.Infra.Queries.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Payroll.WorkerSchedules
{
    public partial interface IWorkerScheduleByTraderModelFactory
    {
        Task<WorkerScheduleSearchModel> PrepareWorkerScheduleSearchModelAsync(WorkerScheduleSearchModel searchModel);
        Task<WorkerScheduleListModel> PrepareWorkerScheduleListModelAsync(WorkerScheduleSearchModel searchModel);
        Task<WorkerScheduleModel> PrepareWorkerScheduleModelAsync(WorkerScheduleModel model, WorkerSchedule workerSchedule, Trader trader, string connection);
        Task<WorkerScheduleFormModel> PrepareWorkerScheduleFormModelAsync(WorkerScheduleFormModel formModel, bool editMode, string connection);
        Task<int> CreateWorkerSchedule(WorkerScheduleModel model, string connection);
    }
    public partial class WorkerScheduleByTraderModelFactory : IWorkerScheduleByTraderModelFactory
    {
        private readonly ITraderService _traderService;
        private readonly IWorkerScheduleDateService _workerScheduleDateService;
        private readonly IWorkerScheduleService _workerScheduleService;
        private readonly IWorkerScheduleWorkerService _workerScheduleWorkerService;
        private readonly ILocalizationService _localizationService;
        private readonly IAppDataProvider _dataProvider;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        public WorkerScheduleByTraderModelFactory(ITraderService traderService,
            IWorkerScheduleDateService workerScheduleDateService,
            IWorkerScheduleService workerScheduleService,
            IWorkerScheduleWorkerService workerScheduleWorkerService,
            ILocalizationService localizationService,
            IAppDataProvider appDataProvider,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _workerScheduleDateService = workerScheduleDateService;
            _workerScheduleService = workerScheduleService;
            _workerScheduleWorkerService = workerScheduleWorkerService;
            _localizationService = localizationService;
            _dataProvider = appDataProvider;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        private async Task<IPagedList<WorkerScheduleModel>> GetPagedListAsync(WorkerScheduleSearchModel searchModel)
        {
            var scheduleTypes = await WorkerScheduleType.Weekly.ToSelectionItemListAsync();
            var scheduleModeTypes = await WorkerScheduleModeType.Waiting.ToSelectionItemListAsync();

            var query = _workerScheduleService.Table.AsEnumerable()
                .Select(x =>
                {
                    var model = x.ToModel<WorkerScheduleModel>();

                    model.Description = $"{model.PeriodFromDate.ToString("dd/MM/yyyy")} - {model.PeriodToDate.ToString("dd/MM/yyyy")}";
                    model.WorkerScheduleTypeName = scheduleTypes.FirstOrDefault(a => a.Value == x.WorkerScheduleTypeId)?.Label ?? "";
                    model.WorkerScheduleModeTypeName = scheduleModeTypes.FirstOrDefault(a => a.Value == x.WorkerScheduleModeTypeId)?.Label ?? "";

                    return model;
                }).AsQueryable();

            //Hack
            var trader = await _workContext.GetCurrentTraderAsync();
            //Todo delete if
            if (trader != null)
                query = query.Where(a => a.TraderId == trader.Id);

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.Description.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.WorkerScheduleTypeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.WorkerScheduleModeTypeName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<WorkerScheduleSearchModel> PrepareWorkerScheduleSearchModelAsync(WorkerScheduleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize(); searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.WorkerScheduleModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<WorkerScheduleListModel> PrepareWorkerScheduleListModelAsync(WorkerScheduleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var workerSchedules = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new WorkerScheduleListModel().PrepareToGrid(searchModel, workerSchedules);

            return model;
        }

        public virtual async Task<WorkerScheduleModel> PrepareWorkerScheduleModelAsync(WorkerScheduleModel model, WorkerSchedule workerSchedule, Trader trader, string connection)
        {
            if (workerSchedule != null)
            {
                var scheduleModeTypes = await WorkerScheduleModeType.Waiting.ToSelectionItemListAsync();
                //fill in model values from the entity
                model ??= workerSchedule.ToModel<WorkerScheduleModel>();

                var workers = await _workerScheduleWorkerService.GetAllWorkerScheduleWorkersAsync(workerSchedule.Id);
                model.Workers = workers.Select(x => x.WorkerId).ToList();
                model.WorkerCardNames = string.Join(", ", workers.Select(x => x.WorkerCardName).ToArray());
                model.WorkerScheduleModeTypeName = scheduleModeTypes.FirstOrDefault(a => a.Value == model.WorkerScheduleModeTypeId)?.Label ?? "";
            }

            if (workerSchedule == null)
            {
                var date = DateTime.UtcNow;
                model.TraderId = trader.Id;
                model.PeriodFromDate = date.Date;
                model.PeriodToDate = date.Date;
                model.DeliveryDate = date;
                model.SubmitDate = date;
                model.WorkerScheduleTypeId = (int)WorkerScheduleType.Weekly;
                model.WorkerScheduleModeTypeId = (int)WorkerScheduleModeType.Waiting;
                model.Workers = (await GetWorkersListItemAsync(connection, trader.HyperPayrollId)).Select(x => x.Value).ToList();
            }

            return model;
        }

        private async Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<WorkerScheduleModel>(1, nameof(WorkerScheduleModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<WorkerScheduleModel>(2, nameof(WorkerScheduleModel.PeriodFromDate), ColumnType.Date, hidden: true),
                ColumnConfig.Create<WorkerScheduleModel>(3, nameof(WorkerScheduleModel.PeriodToDate), ColumnType.Date, hidden: true),
                ColumnConfig.Create<WorkerScheduleModel>(4, nameof(WorkerScheduleModel.DeliveryDate), ColumnType.Date),
                ColumnConfig.Create<WorkerScheduleModel>(5, nameof(WorkerScheduleModel.SubmitDate), ColumnType.Date),
                ColumnConfig.Create<WorkerScheduleModel>(6, nameof(WorkerScheduleModel.Protocol), hidden: true),
                ColumnConfig.Create<WorkerScheduleModel>(7, nameof(WorkerScheduleModel.WorkerScheduleTypeName)),
                ColumnConfig.Create<WorkerScheduleModel>(8, nameof(WorkerScheduleModel.WorkerScheduleModeTypeName)),
                ColumnConfig.CreateButton<WorkerScheduleModel>(11, ColumnType.RowButton, "submit", "info",
                    await _localizationService.GetResourceAsync("App.Common.SchedulesSubmit"), textAlign, textAlign),
                ColumnConfig.CreateButton<WorkerScheduleModel>(11, ColumnType.RowButton, "check", "success",
                    await _localizationService.GetResourceAsync("App.Common.SchedulesCheck"), textAlign, textAlign)
            };

            return columns;
        }

        public virtual async Task<WorkerScheduleFormModel> PrepareWorkerScheduleFormModelAsync(WorkerScheduleFormModel formModel, bool editMode, string connection)
        {
            var scheduleTypes = await WorkerScheduleType.Weekly.ToSelectionItemListAsync();

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.PeriodFromDate), FieldType.Date, markAsRequired: true, _readonly: editMode),
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.PeriodToDate), FieldType.Date, markAsRequired: true, _readonly: editMode),
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.DeliveryDate), FieldType.Date, _readonly: true),
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.Protocol), FieldType.Text, _readonly: true),
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.Notes), FieldType.Textarea, _readonly: true),
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.WorkerScheduleTypeId), FieldType.Select, options: scheduleTypes, _readonly: editMode),
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.WorkerScheduleModeTypeName), FieldType.Text, _readonly: true)
            };

            if (editMode)
            {
                fields.Insert(0, FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.WorkerCardNames), FieldType.Textarea, _readonly: true, rows: 5));
            }
            else
            {
                var trader = await _workContext.GetCurrentTraderAsync();
                var workers = await GetWorkersListItemAsync(connection, trader.HyperPayrollId);
                var workersField = FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.Workers), FieldType.MultiSelect, markAsRequired: true, options: workers);
                fields.Insert(0, workersField);
            }

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.WorkerScheduleModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }

        private async Task<IList<SelectionItemList>> GetWorkersListItemAsync(string connection, int companyId)
        {
            var results = await GetWorkersAsync(connection, companyId);
            return results.Select(x => new SelectionItemList { Value = x.WorkerId, Label = x.WorkerCardName }).ToList();
        }

        private async Task<IList<WorkerScheduleQueryResult>> GetWorkersAsync(string connection, int companyId)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
            var workers = await _dataProvider.QueryAsync<WorkerScheduleQueryResult>(connection, WorkerScheduleQuery.Workers, pCompanyId);

            return workers.Where(x => !string.IsNullOrEmpty(x.WorkerCardName)).ToList();
        }

        public async Task<int> CreateWorkerSchedule(WorkerScheduleModel model, string connection)
        {
            var workerSchedule = model.ToEntity<WorkerSchedule>();
            await _workerScheduleService.InsertWorkerScheduleAsync(workerSchedule);

            //create WorkerScheduleWorkers
            var trader = await _traderService.GetTraderByIdAsync(model.TraderId);

            var workers = await GetWorkersAsync(connection, trader.HyperPayrollId);
            workers = workers.Where(x => model.Workers.Contains(x.WorkerId)).ToList();

            var workerScheduleWorkers = new List<WorkerScheduleWorker>();
            foreach (var x in workers)
            {
                var item = new WorkerScheduleWorker
                {
                    WorkerScheduleId = workerSchedule.Id,
                    TraderId = trader.Id,
                    ActiveCard = x.ActiveCard,
                    WorkerCardName = x.WorkerCardName,
                    WorkerId = x.WorkerId,
                    WorkerName = x.WorkerName,
                    WorkerVat = x.WorkerVat,
                    WorkingHours = x.WorkingHours

                };
                workerScheduleWorkers.Add(item);

            }
            await _workerScheduleWorkerService.InsertWorkerScheduleWorkerAsync(workerScheduleWorkers);

            //create WorkerScheduleDates
            var dates = new List<DateTime>();
            for (var dt = workerSchedule.PeriodFromDate.Date; dt <= workerSchedule.PeriodToDate.Date; dt = dt.AddDays(1))
                dates.Add(dt);

            var workerScheduleDates = new List<WorkerScheduleDate>();
            var initialDate = new DateTime(new DateTime(2000, 2, 2, 0, 0, 0, 0).Ticks, DateTimeKind.Utc);

            foreach (var date in dates)
            {
                foreach (var worker in workerScheduleWorkers)
                {
                    var item = new WorkerScheduleDate
                    {
                        TraderId = worker.TraderId,
                        WorkerScheduleId = worker.WorkerScheduleId,
                        WorkerId = worker.WorkerId,
                        WorkerVat = worker.WorkerVat,
                        WorkingDate = date,
                        NonstopFromDate = initialDate,
                        NonstopToDate = initialDate,
                        SplitFromDate = initialDate,
                        SplitToDate = initialDate,
                        BreakNonstopFromDate = initialDate,
                        BreakNonstopToDate = initialDate,
                        BreakNonstop2FromDate = initialDate,
                        BreakNonstop2ToDate = initialDate,
                        BreakSplitFromDate = initialDate,
                        BreakSplitToDate = initialDate,
                        OvertimeFromDate = initialDate,
                        OvertimeToDate = initialDate,
                        WeekOfYear = await _dateTimeHelper.GetWeekOfYear(date),
                        IsSaturday = date.DayOfWeek == DayOfWeek.Saturday,
                        IsSunday = date.DayOfWeek == DayOfWeek.Sunday,
                        Active = true
                    };
                    workerScheduleDates.Add(item);
                }
            }

            var _dates = workerScheduleDates.Select(x => x.WorkingDate).Distinct().ToList();
            var _workers = workerScheduleDates.Select(x => x.WorkerId).Distinct().ToList();

            //Get workerScheduleDates belong trader
            var list = await _workerScheduleDateService.Table
                .Where(x =>
                    x.TraderId == trader.Id &&
                    _dates.Contains(x.WorkingDate) &&
                    _workers.Contains(x.WorkerId) &&
                    x.Active)
                .ToListAsync();

            var updatedList = new List<WorkerScheduleDate>();
            list.ForEach(x =>
            {
                x.Active = false;
                updatedList.Add(x);
            });

            // inActive same dates
            //var list = await _workerScheduleDateService.Table.Where(x => x.TraderId == trader.Id && x.Active).ToListAsync();
            //var updatedList = new List<WorkerScheduleDate>();

            //foreach (var x in list)
            //{
            //    foreach (var w in workerScheduleDates)
            //    {
            //        if (x.WorkerId == w.WorkerId && x.WorkingDate == w.WorkingDate && x.Active)
            //        {
            //            x.Active = false;
            //            updatedList.Add(x);
            //        }
            //    }
            //}
            await _workerScheduleDateService.UpdateWorkerScheduleDateAsync(updatedList);

            //insert workerScheduleDates
            await _workerScheduleDateService.InsertWorkerScheduleDateAsync(workerScheduleDates);


            return workerSchedule.Id;
        }
    }
}
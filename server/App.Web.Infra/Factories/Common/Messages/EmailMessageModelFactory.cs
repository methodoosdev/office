using App.Core;
using App.Core.Domain.Common;
using App.Core.Domain.Messages;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Messages;
using App.Models.Traders;
using App.Services;
using App.Services.Customers;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Messages;
using App.Services.Offices;
using App.Services.Traders;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Messages
{
    public partial interface IEmailMessageModelFactory
    {
        Task<EmailMessageSearchModel> PrepareEmailMessageSearchModelAsync(EmailMessageSearchModel searchModel);
        Task<EmailMessageListModel> PrepareEmailMessageListModelAsync(EmailMessageSearchModel searchModel);
        Task<EmailMessageModel> PrepareEmailMessageModelAsync(EmailMessageModel model, EmailMessage emailMessage);
        Task<EmailMessageFormModel> PrepareEmailMessageFormModelAsync(EmailMessageFormModel formModel);
        Task<EmailMessageFilterFormModel> PrepareEmailMessageFilterFormModelAsync(EmailMessageFilterFormModel emailMessageFilterFormModel);
    }
    public partial class EmailMessageModelFactory : IEmailMessageModelFactory
    {
        private readonly IEmailMessageService _emailMessageService;
        private readonly ITraderService _traderService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly IPersistStateService _persistStateService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        public EmailMessageModelFactory(
            IEmailMessageService emailMessageService,
            ITraderService traderService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IModelFactoryService modelFactoryService,
            IPersistStateService persistStateService,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _emailMessageService = emailMessageService;
            _traderService = traderService;
            _customerService = customerService;
            _localizationService = localizationService;
            _modelFactoryService = modelFactoryService;
            _persistStateService = persistStateService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        private async Task<IPagedList<EmailMessageModel>> GetPagedListAsync(EmailMessageSearchModel searchModel, EmailMessageFilterModel filterModel, bool filterExist)
        {
            var emailAccounts = await _modelFactoryService.GetAllEmailAccountsAsync(false);
            var emailMessageTypes = await EmailMessageType.Other.ToSelectionItemListAsync();
            var customerTypes = await CustomerType.Other.ToSelectionItemListAsync();
            var legalFormTypes = await LegalFormType.None.ToSelectionItemListAsync();
            var categoryBookTypes = await CategoryBookType.None.ToSelectionItemListAsync();
            var months = await _modelFactoryService.GetSelectionItemListAsync(DateLocaleResources.LocaleMonthResourceDict);

            var customers = await _customerService.GetAllCustomersAsync();
            var traderList = await _traderService.GetAllTradersAsync();
            var traders = traderList.Select(x => new TraderHelperResult
            {
                TraderId = x.Id,
                FullName = x.ToTraderFullName(),
                CustomerTypeId = x.CustomerTypeId,
                LegalFormTypeId = x.LegalFormTypeId,
                CategoryBookTypeId = x.CategoryBookTypeId
            }).ToList();

            var query = _emailMessageService.Table.
                Select(x => new EmailMessageModel
                {
                    Id = x.Id,
                    CustomerId = x.CustomerId,
                    EmailMessageTypeId = x.EmailMessageTypeId,
                    Description = x.Description,
                    CreatedDate = x.CreatedDate,
                    ShippingDate = x.ShippingDate,
                    Subject = x.Subject,
                    FromAddress = x.FromAddress,
                    FromName = x.FromName,
                    ToAddress = x.ToAddress,
                    ToName = x.ToName,
                    ReplyTo = x.ReplyTo,
                    ReplyToName = x.ReplyToName,
                    AttachmentFilePath = x.AttachmentFilePath,
                    AttachmentFileName = x.AttachmentFileName,
                    AttachedDownloadId = x.AttachedDownloadId,
                    Headers = x.Headers,
                    Bcc = x.Bcc,
                    Cc = x.Cc,
                    SenderId = x.SenderId,
                    TraderId = x.TraderId,
                    Period = x.Period
                });

            query = query.AsEnumerable()
                .Select(x =>
                {
                    var sender = customers.FirstOrDefault(k => k.Id == x.SenderId);
                    var customer = customers.FirstOrDefault(k => k.Id == x.CustomerId);
                    var trader = traders.FirstOrDefault(k => k.TraderId == x.TraderId);

                    var date = _dateTimeHelper.ConvertToUserTimeAsync(x.CreatedDate, DateTimeKind.Utc).Result;

                    x.EmailMessageTypeName = emailMessageTypes.FirstOrDefault(e => e.Value == x.EmailMessageTypeId)?.Label ?? "";
                    x.IsSent = x.ShippingDate.HasValue;

                    x.TraderName = trader?.FullName ?? string.Empty;
                    x.CustomerTypeId = customerTypes.FirstOrDefault(a => a.Value == trader?.CustomerTypeId)?.Value ?? -1;
                    x.CustomerTypeName = customerTypes.FirstOrDefault(a => a.Value == trader?.CustomerTypeId)?.Label ?? "";

                    x.LegalFormTypeId = legalFormTypes.FirstOrDefault(a => a.Value == trader?.LegalFormTypeId)?.Value ?? -1;
                    x.LegalFormTypeName = legalFormTypes.FirstOrDefault(a => a.Value == trader?.LegalFormTypeId)?.Label ?? "";

                    x.CategoryBookTypeId = categoryBookTypes.FirstOrDefault(a => a.Value == trader?.CategoryBookTypeId)?.Value ?? -1;
                    x.CategoryBookTypeName = categoryBookTypes.FirstOrDefault(a => a.Value == trader?.CategoryBookTypeId)?.Label ?? "";

                    x.SenderName = sender?.FullName() ?? string.Empty;
                    x.CustomerName = customer?.FullName() ?? string.Empty;
                    x.CreatedOn = date;
                    x.PeriodName = $"{months.First(k => k.Value == x.Period).Label} {date.Year}";

                    return x;

                }).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }


            if (filterExist)
            {
                if (filterModel.EmailMessageTypeId > 0)
                    query = query.Where(x => x.EmailMessageTypeId.Equals(filterModel.EmailMessageTypeId));

                if (!string.IsNullOrEmpty(filterModel.Description))
                    query = query.Where(c =>
                        c.Description.ContainsIgnoreCase(filterModel.Description));

                if (filterModel.ShippingDate.HasValue)
                    query = query.Where(x =>
                        x.ShippingDate.HasValue ?
                        x.ShippingDate.Value.Year == filterModel.ShippingDate.Value.Year &&
                         x.ShippingDate.Value.Month == filterModel.ShippingDate.Value.Month : false);

                if (filterModel.PeriodDate.HasValue)
                {
                    query = query.Where(x =>
                        filterModel.PeriodDate.Value.Year == x.CreatedOn.Year &&
                        filterModel.PeriodDate.Value.Month == x.Period);
                }

                if (filterModel.CustomerId > 0)
                    query = query.Where(c => c.CustomerId == filterModel.CustomerId);

                if (!string.IsNullOrEmpty(filterModel.Subject))
                    query = query.Where(c =>
                        c.Subject.ContainsIgnoreCase(filterModel.Subject));

                if (!string.IsNullOrEmpty(filterModel.ToAddress))
                    query = query.Where(c =>
                        c.ToAddress.ContainsIgnoreCase(filterModel.ToAddress));

                if (!string.IsNullOrEmpty(filterModel.ToName))
                    query = query.Where(c =>
                        c.ToName.ContainsIgnoreCase(filterModel.ToName));

                if (filterModel.IsSent > 0)
                    query = query.Where(c => c.IsSent == (filterModel.IsSent == 1));

                if (filterModel.CustomerTypeId.HasValue && filterModel.CustomerTypeId.Value > -1)
                    query = query.Where(c => c.CustomerTypeId == filterModel.CustomerTypeId.Value);

                if (filterModel.LegalFormTypeId.HasValue && filterModel.LegalFormTypeId.Value > -1)
                    query = query.Where(c => c.LegalFormTypeId == filterModel.LegalFormTypeId.Value);

                if (filterModel.CategoryBookTypeId.HasValue && filterModel.CategoryBookTypeId.Value > -1)
                    query = query.Where(c => c.CategoryBookTypeId == filterModel.CategoryBookTypeId.Value);
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.AsAsyncEnumerable().ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<EmailMessageSearchModel> PrepareEmailMessageSearchModelAsync(EmailMessageSearchModel searchModel)
        {
            var persistState = await _persistStateService.GetModelInstance<EmailMessageSearchModel>();

            if (persistState.Exist)
                return persistState.Model;

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.EmailMessageModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<EmailMessageListModel> PrepareEmailMessageListModelAsync(EmailMessageSearchModel searchModel)
        {
            var filterState = await _persistStateService.GetModelInstance<EmailMessageFilterModel>();

            //get customer roles
            var emailMessages = await GetPagedListAsync(searchModel, filterState.Model, filterState.Exist);

            //prepare grid model
            var model = new EmailMessageListModel().PrepareToGrid(searchModel, emailMessages);
            model.FilterExist = filterState.Exist;

            return model;
        }

        public virtual Task<EmailMessageModel> PrepareEmailMessageModelAsync(EmailMessageModel model, EmailMessage emailMessage)
        {
            if (emailMessage != null)
            {
                //fill in model values from the entity
                model ??= emailMessage.ToModel<EmailMessageModel>();
            }

            if (emailMessage == null)
            {
                model.CreatedDate = DateTime.UtcNow.ToUtcRelative();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<EmailMessageModel>(2, nameof(EmailMessageModel.EmailMessageTypeName), ColumnType.RouterLink),
                ColumnConfig.Create<EmailMessageModel>(1, nameof(EmailMessageModel.Description), hidden: true),
                ColumnConfig.Create<EmailMessageModel>(3, nameof(EmailMessageModel.Subject)),
                ColumnConfig.Create<EmailMessageModel>(4, nameof(EmailMessageModel.ToName)),
                ColumnConfig.Create<EmailMessageModel>(4, nameof(EmailMessageModel.ToAddress)),
                ColumnConfig.Create<EmailMessageModel>(5, nameof(EmailMessageModel.CreatedDate), ColumnType.DateTime, style: centerAlign),
                ColumnConfig.Create<EmailMessageModel>(6, nameof(EmailMessageModel.ShippingDate), ColumnType.DateTime, style: centerAlign),
                ColumnConfig.Create<EmailMessageModel>(4, nameof(EmailMessageModel.PeriodName)),
                ColumnConfig.Create<EmailMessageModel>(4, nameof(EmailMessageModel.SenderName)),
                ColumnConfig.Create<EmailMessageModel>(4, nameof(EmailMessageModel.CustomerName)),
                ColumnConfig.Create<EmailMessageModel>(4, nameof(EmailMessageModel.CreatedOn), ColumnType.DateTime),
                //ColumnConfig.Create<EmailMessageModel>(4, nameof(EmailMessageModel.TraderName), hidden: true),
                ColumnConfig.Create<EmailMessageModel>(4, nameof(EmailMessageModel.CustomerTypeName), hidden: true),
                ColumnConfig.Create<EmailMessageModel>(4, nameof(EmailMessageModel.LegalFormTypeName), hidden: true),
                ColumnConfig.Create<EmailMessageModel>(4, nameof(EmailMessageModel.CategoryBookTypeName), hidden : true),
            };

            return columns;
        }

        public virtual async Task<EmailMessageFormModel> PrepareEmailMessageFormModelAsync(EmailMessageFormModel formModel)
        {
            var traders = await _modelFactoryService.GetAllTradersAsync(false);
            var emailMessageTypes = await EmailMessageType.Other.ToSelectionItemListAsync();

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.EmailMessageTypeId), FieldType.Select, options: emailMessageTypes),
                FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.Description), FieldType.Text, markAsRequired: true),
                FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.CreatedDate), FieldType.Date),
                //FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.ShippingDate), FieldType.Date, hideExpression: "!!model.shippingDate"),
                FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.Subject), FieldType.Text, markAsRequired : true),
                //FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.FromAddress), FieldType.Text),
                //FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.FromName), FieldType.Text),
                FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.TraderId), FieldType.GridSelect, options: traders),
                FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.ToAddress), FieldType.Text),
                FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.ToName), FieldType.Text),
                FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.Cc), FieldType.TextareaButton, rows: 3),
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.ReplyTo), FieldType.Text),
                FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.ReplyToName), FieldType.Text),
                FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.AttachmentFilePath), FieldType.Text),
                FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.AttachmentFileName), FieldType.Text),
                FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.AttachedDownloadId), FieldType.Numeric),
                FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.Headers), FieldType.Text),
                FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.Bcc), FieldType.TextareaButton, rows: 3),
            };

            var editor = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmailMessageModel>(nameof(EmailMessageModel.Body), FieldType.Editor, markAsRequired : true),
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6", "col-12" }, left, right, editor);

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.EmailMessageModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", fields);

            return formModel;
        }

        public virtual async Task<EmailMessageFilterFormModel> PrepareEmailMessageFilterFormModelAsync(EmailMessageFilterFormModel filterFormModel)
        {
            var tristateTypes = await TristateType.Null.ToSelectionItemListAsync();
            var emailMessageTypes = await EmailMessageType.Welcome.ToSelectionItemListAsync(true, true);

            var customerTypes = await CustomerType.Other.ToSelectionItemListAsync(true, true, -1);
            var legalFormTypes = await LegalFormType.None.ToSelectionItemListAsync(true, true, -1);
            var categoryBookTypes = await CategoryBookType.None.ToSelectionItemListAsync(true, true, -1);

            var array = await _emailMessageService.Table
                .Where(x => x.CustomerId > 0).Select(k => k.CustomerId)
                .Distinct()
                .ToArrayAsync();
            var customerIds = await _customerService.GetCustomersByIdsAsync(array);

            var creators = customerIds.Select(x => new SelectionItemList(x.Id, x.FullName())).ToList();
            creators.Insert(0, new SelectionItemList { Label = "", Value = 0 });

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmailMessageFilterModel>(nameof(EmailMessageFilterModel.EmailMessageTypeId), FieldType.Select, options: emailMessageTypes),
                FieldConfig.Create<EmailMessageFilterModel>(nameof(EmailMessageFilterModel.ShippingDate), FieldType.MonthDate),
                FieldConfig.Create<EmailMessageFilterModel>(nameof(EmailMessageFilterModel.Description), FieldType.Text),
            };

            var center = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmailMessageFilterModel>(nameof(EmailMessageFilterModel.Subject), FieldType.Text),
                FieldConfig.Create<EmailMessageFilterModel>(nameof(EmailMessageFilterModel.ToAddress), FieldType.Text),
                FieldConfig.Create<EmailMessageFilterModel>(nameof(EmailMessageFilterModel.ToName), FieldType.Text),
                FieldConfig.Create<EmailMessageFilterModel>(nameof(EmailMessageFilterModel.CustomerId), FieldType.GridSelect, options: creators),
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmailMessageFilterModel>(nameof(EmailMessageFilterModel.IsSent), FieldType.Select, options: tristateTypes),
                FieldConfig.Create<EmailMessageFilterModel>(nameof(EmailMessageFilterModel.CustomerTypeId), FieldType.Select, options: customerTypes),
                FieldConfig.Create<EmailMessageFilterModel>(nameof(EmailMessageFilterModel.LegalFormTypeId), FieldType.Select, options: legalFormTypes),
                FieldConfig.Create<EmailMessageFilterModel>(nameof(EmailMessageFilterModel.CategoryBookTypeId), FieldType.Select, options: categoryBookTypes)
            };

            var saveState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmailMessageFilterModel>("SaveState", FieldType.Button, themeColor: "primary",
                label: await _localizationService.GetResourceAsync("App.Common.SaveState"))
            };

            var removeState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmailMessageFilterModel>("RemoveState", FieldType.Button, themeColor: "warning",
                label: await _localizationService.GetResourceAsync("App.Common.RemoveState"))
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-4", "col-12 md:col-4", "col-12 md:col-4", "col-12 md:col-2", "col-12 md:col-2" }, left, center, right, saveState, removeState);

            filterFormModel.CustomProperties.Add("fields", fields);

            return filterFormModel;
        }
    }
}
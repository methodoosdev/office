using App.Core.Domain.Messages;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Messages;
using App.Services;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Messages;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LinqToDB.Sql;

namespace App.Web.Admin.Factories.Messages
{
    public partial interface IQueuedEmailModelFactory
    {
        Task<QueuedEmailSearchModel> PrepareQueuedEmailSearchModelAsync(QueuedEmailSearchModel searchModel);

        Task<QueuedEmailListModel> PrepareQueuedEmailListModelAsync(QueuedEmailSearchModel searchModel);

        Task<QueuedEmailModel> PrepareQueuedEmailModelAsync(QueuedEmailModel model, QueuedEmail queuedEmail, bool excludeProperties = false);
        Task<QueuedEmailFormModel> PrepareQueuedEmailFormModelAsync(QueuedEmailFormModel formModel);
    }
    public partial class QueuedEmailModelFactory : IQueuedEmailModelFactory
    {
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILocalizationService _localizationService;
        private readonly IQueuedEmailService _queuedEmailService;

        public QueuedEmailModelFactory(IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            ILocalizationService localizationService,
            IQueuedEmailService queuedEmailService)
        {
            _dateTimeHelper = dateTimeHelper;
            _emailAccountService = emailAccountService;
            _localizationService = localizationService;
            _queuedEmailService = queuedEmailService;
        }

        protected virtual string GetEmailAccountName(EmailAccount emailAccount)
        {
            if (emailAccount == null)
                return string.Empty;

            if (!string.IsNullOrWhiteSpace(emailAccount.DisplayName))
                return emailAccount.Email + " (" + emailAccount.DisplayName + ")";

            return emailAccount.Email;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<QueuedEmailModel>(1, nameof(QueuedEmailModel.PriorityName)),
                ColumnConfig.Create<QueuedEmailModel>(2, nameof(QueuedEmailModel.To), ColumnType.RouterLink),
                ColumnConfig.Create<QueuedEmailModel>(3, nameof(QueuedEmailModel.Subject)),
                ColumnConfig.Create<QueuedEmailModel>(4, nameof(QueuedEmailModel.CreatedOn), ColumnType.Date),
                ColumnConfig.Create<QueuedEmailModel>(5, nameof(QueuedEmailModel.SentOn), ColumnType.Date)
            };

            return columns;
        }

        public virtual async Task<QueuedEmailSearchModel> PrepareQueuedEmailSearchModelAsync(QueuedEmailSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.QueuedEmailModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<QueuedEmailListModel> PrepareQueuedEmailListModelAsync(QueuedEmailSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter emails
            var startDateValue = !searchModel.SearchStartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchStartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.SearchEndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.SearchEndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            //get queued emails
            var queuedEmails = await _queuedEmailService.SearchEmailsAsync(
                createdFromUtc: startDateValue, createdToUtc: endDateValue, unsendOnly: false,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new QueuedEmailListModel().PrepareToGridAsync(searchModel, queuedEmails, () =>
            {
                return queuedEmails.SelectAwait(async queuedEmail =>
                {
                    //fill in model values from the entity
                    var queuedEmailModel = queuedEmail.ToModel<QueuedEmailModel>();

                    //little performance optimization: ensure that "Body" is not returned
                    queuedEmailModel.Body = string.Empty;

                    //convert dates to the user time
                    queuedEmailModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(queuedEmail.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(queuedEmail.EmailAccountId);
                    queuedEmailModel.EmailAccountName = GetEmailAccountName(emailAccount);
                    queuedEmailModel.PriorityName = await _localizationService.GetLocalizedEnumAsync(queuedEmail.Priority);

                    //if (queuedEmail.DontSendBeforeDateUtc.HasValue)
                    //{
                    //    queuedEmailModel.DontSendBeforeDate = await _dateTimeHelper
                    //        .ConvertToUserTimeAsync(queuedEmail.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
                    //}

                    if (queuedEmail.SentOnUtc.HasValue)
                        queuedEmailModel.SentOn = await _dateTimeHelper.ConvertToUserTimeAsync(queuedEmail.SentOnUtc.Value, DateTimeKind.Utc);

                    return queuedEmailModel;
                });
            });

            return model;
        }

        public virtual async Task<QueuedEmailModel> PrepareQueuedEmailModelAsync(QueuedEmailModel model, QueuedEmail queuedEmail, bool excludeProperties = false)
        {
            if (queuedEmail == null)
                return model;

            var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(queuedEmail.EmailAccountId);

            //fill in model values from the entity
            model ??= queuedEmail.ToModel<QueuedEmailModel>();

            model.EmailAccountName = GetEmailAccountName(emailAccount);
            model.FromName = emailAccount.DisplayName;
            model.From = emailAccount.Email;
            model.PriorityName = await _localizationService.GetLocalizedEnumAsync(queuedEmail.Priority);
            model.CreatedOnUtc = await _dateTimeHelper.ConvertToUserTimeAsync(queuedEmail.CreatedOnUtc, DateTimeKind.Utc);

            //if (queuedEmail.SentOnUtc.HasValue)
            //    model.SentOn = await _dateTimeHelper.ConvertToUserTimeAsync(queuedEmail.SentOnUtc.Value, DateTimeKind.Utc);
            //if (queuedEmail.DontSendBeforeDateUtc.HasValue)
            //    model.DontSendBeforeDate = await _dateTimeHelper.ConvertToUserTimeAsync(queuedEmail.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
            //else
            //    model.SendImmediately = true;

            return model;
        }

        public virtual async Task<QueuedEmailFormModel> PrepareQueuedEmailFormModelAsync(QueuedEmailFormModel formModel)
        {
            var priorities = await QueuedEmailPriority.Low.ToSelectionItemListAsync();

            var top = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<QueuedEmailModel>(nameof(QueuedEmailModel.PriorityId), FieldType.Select, options: priorities),
                FieldConfig.Create<QueuedEmailModel>(nameof(QueuedEmailModel.To), FieldType.Text),
                FieldConfig.Create<QueuedEmailModel>(nameof(QueuedEmailModel.Subject), FieldType.Text),
                FieldConfig.Create<QueuedEmailModel>(nameof(QueuedEmailModel.CreatedOnUtc), FieldType.Date),
                //FieldConfig.Create<QueuedEmailModel>(nameof(QueuedEmailModel.SentOnUtc), FieldType.Date),
                FieldConfig.Create<QueuedEmailModel>(nameof(QueuedEmailModel.EmailAccountName), FieldType.Text, _readonly: true)
            };

            var bottom = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<QueuedEmailModel>(nameof(QueuedEmailModel.Body), FieldType.Editor)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12" }, top, bottom);

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.QueuedEmailModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }
    }
}
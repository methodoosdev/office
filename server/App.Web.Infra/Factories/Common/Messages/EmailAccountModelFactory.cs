using App.Core;
using App.Core.Domain.Messages;
using App.Core.Infrastructure;
using App.Data;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Messages;
using App.Services.Localization;
using App.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Messages
{
    /// <summary>
    /// Represents the email account model factory implementation
    /// </summary>
    public partial class EmailAccountModelFactory : IEmailAccountModelFactory
    {
        #region Fields

        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public EmailAccountModelFactory(IRepository<EmailAccount> emailAccountRepository,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _emailAccountRepository = emailAccountRepository;
            _emailAccountSettings = emailAccountSettings;
            _emailAccountService = emailAccountService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        private async Task<IPagedList<EmailAccountModel>> GetPagedListAsync(EmailAccountSearchModel searchModel)
        {
            var query = _emailAccountService.Table
                .AsEnumerable()
                .Select(emailAccount =>
                {
                    EmailAccountModel emailAccountModel = emailAccount.ToModel<EmailAccountModel>();

                    //fill in additional values (not existing in the entity)
                    emailAccountModel.IsDefaultEmailAccount = emailAccount.Id == _emailAccountSettings.DefaultEmailAccountId;

                    return emailAccountModel;

                }).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => 
                    c.DisplayName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Email.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Username.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        /// <summary>
        /// Prepare email account search model
        /// </summary>
        /// <param name="searchModel">Email account search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the email account search model
        /// </returns>
        public virtual async Task<EmailAccountSearchModel> PrepareEmailAccountSearchModelAsync(EmailAccountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.EmailAccountModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        /// <summary>
        /// Prepare paged email account list model
        /// </summary>
        /// <param name="searchModel">Email account search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the email account list model
        /// </returns>
        public virtual async Task<EmailAccountListModel> PrepareEmailAccountListModelAsync(EmailAccountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get email accounts
            var emailAccounts = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new EmailAccountListModel().PrepareToGrid(searchModel, emailAccounts);

            return model;
        }

        /// <summary>
        /// Prepare email account model
        /// </summary>
        /// <param name="model">Email account model</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the email account model
        /// </returns>
        public virtual Task<EmailAccountModel> PrepareEmailAccountModelAsync(EmailAccountModel model,
            EmailAccount emailAccount, bool excludeProperties = false)
        {
            //fill in model values from the entity
            if (emailAccount != null)
            {
                model ??= emailAccount.ToModel<EmailAccountModel>();
                model.InfoMessage = "Νέα έκδοση σε 10 λεπτά, η εφαρμογή θα τερματιστεί.";
            }

            //set default values for the new model
            if (emailAccount == null)
                model.Port = 25;

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<EmailAccountModel>(1, nameof(EmailAccountModel.Email), ColumnType.RouterLink),
                ColumnConfig.Create<EmailAccountModel>(2, nameof(EmailAccountModel.DisplayName)),
                ColumnConfig.Create<EmailAccountModel>(3, nameof(EmailAccountModel.Host)),
                ColumnConfig.Create<EmailAccountModel>(4, nameof(EmailAccountModel.Port)),
                ColumnConfig.Create<EmailAccountModel>(5, nameof(EmailAccountModel.Username), media: "(min-width: 680px)"),
                ColumnConfig.Create<EmailAccountModel>(6, nameof(EmailAccountModel.Password)),
                ColumnConfig.Create<EmailAccountModel>(7, nameof(EmailAccountModel.EnableSsl), ColumnType.Checkbox),
                ColumnConfig.Create<EmailAccountModel>(8, nameof(EmailAccountModel.UseDefaultCredentials), ColumnType.Checkbox, hidden: true),
                ColumnConfig.Create<EmailAccountModel>(9, nameof(EmailAccountModel.IsDefaultEmailAccount), ColumnType.Checkbox)
            };

            return columns;
        }

        public virtual async Task<EmailAccountFormModel> PrepareEmailAccountFormModelAsync(EmailAccountFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>
            {
                FieldConfig.Create<EmailAccountModel>(nameof(EmailAccountModel.Email), FieldType.Text),
                FieldConfig.Create<EmailAccountModel>(nameof(EmailAccountModel.DisplayName), FieldType.Text),
                FieldConfig.Create<EmailAccountModel>(nameof(EmailAccountModel.Host), FieldType.Text),
                FieldConfig.Create<EmailAccountModel>(nameof(EmailAccountModel.Port), FieldType.Numeric),
                FieldConfig.Create<EmailAccountModel>(nameof(EmailAccountModel.Username), FieldType.Text),
                FieldConfig.Create<EmailAccountModel>(nameof(EmailAccountModel.Password), FieldType.Text),
                FieldConfig.Create<EmailAccountModel>(nameof(EmailAccountModel.EnableSsl), FieldType.Checkbox),
                FieldConfig.Create<EmailAccountModel>(nameof(EmailAccountModel.UseDefaultCredentials), FieldType.Checkbox),
                FieldConfig.CreateDivider(),
                FieldConfig.Create<EmailAccountModel>(nameof(EmailAccountModel.SendTestEmailTo), FieldType.TextButton),
                FieldConfig.Create<EmailAccountModel>(nameof(EmailAccountModel.InfoMessage), FieldType.TextButton)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.EmailAccountModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }

        #endregion
    }
}
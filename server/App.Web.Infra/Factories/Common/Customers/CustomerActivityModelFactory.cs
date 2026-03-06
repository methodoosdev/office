using App.Core;
using App.Core.Domain.Customers;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Customers;
using App.Services.Helpers;
using App.Services.Html;
using App.Services.Localization;
using App.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Customers
{
    public partial interface ICustomerActivityModelFactory
    {
        Task<CustomerActivityLogSearchModel> PrepareActivityLogCustomerSearchModelAsync(CustomerActivityLogSearchModel searchModel);
        Task<CustomerActivityLogListModel> PrepareCustomerActivityLogListModelAsync(CustomerActivityLogSearchModel searchModel);
        Task<string>  PrepareLastActivityLogAsync(int customerId);
    }
    public partial class CustomerActivityModelFactory : ICustomerActivityModelFactory
    {
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        public CustomerActivityModelFactory(
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IHtmlFormatter htmlFormatter,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _htmlFormatter = htmlFormatter;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        public virtual async Task<CustomerActivityLogSearchModel> PrepareActivityLogCustomerSearchModelAsync(CustomerActivityLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.CustomerActivityLogModel.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<CustomerActivityLogModel>(1, nameof(CustomerActivityLogModel.ActivityLogTypeName)),
                ColumnConfig.Create<CustomerActivityLogModel>(2, nameof(CustomerActivityLogModel.CreatedOn), ColumnType.DateTime),
                ColumnConfig.Create<CustomerActivityLogModel>(3, nameof(CustomerActivityLogModel.CustomerEmail)),
                ColumnConfig.Create<CustomerActivityLogModel>(4, nameof(CustomerActivityLogModel.EntityId), hidden: true),
                ColumnConfig.Create<CustomerActivityLogModel>(5, nameof(CustomerActivityLogModel.EntityName), hidden: true),
                ColumnConfig.Create<CustomerActivityLogModel>(6, nameof(CustomerActivityLogModel.IpAddress), hidden: true)
            };

            return columns;
        }

        public virtual async Task<CustomerActivityLogListModel> PrepareCustomerActivityLogListModelAsync(CustomerActivityLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var customer = await _workContext.GetCurrentCustomerAsync();

            //get customer activity log
            var activityLog = await _customerActivityService.GetAllActivitiesAsync(customerId: customer.Id,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new CustomerActivityLogListModel().PrepareToGridAsync(searchModel, activityLog, () =>
            {
                return activityLog.SelectAwait(async logItem =>
                {
                    //fill in model values from the entity
                    var model = logItem.ToModel<CustomerActivityLogModel>();

                    //fill in additional values (not existing in the entity)
                    model.ActivityLogTypeName = (await _customerActivityService.GetActivityTypeByIdAsync(logItem.ActivityLogTypeId))?.Name;

                    //convert dates to the user time
                    model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);

                    model.CustomerEmail = customer.Email;

                    model.Comment = _htmlFormatter.ConvertPlainTextToHtml(logItem.Comment);

                    return model;
                });
            });

            return model;
        }

        public virtual Task<string> PrepareLastActivityLogAsync(int customerId)
        {
            var logItem = _customerActivityService.Table.AsEnumerable()
                .LastOrDefault(x => x.CustomerId == customerId);

            var comment = _htmlFormatter.ConvertPlainTextToHtml(logItem?.Comment);

            return Task.FromResult(comment);
        }

    }
}
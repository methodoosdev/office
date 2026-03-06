using App.Core;
using App.Core.Domain.Messages;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Traders;
using App.Services.Configuration;
using App.Services.Localization;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Traders
{
    public partial interface ITraderInfoModelFactory
    {
        Task<TraderInfoSearchModel> PrepareTraderInfoSearchModelAsync(TraderInfoSearchModel searchModel);
        Task<TraderInfoListModel> PrepareTraderInfoListModelAsync(TraderInfoSearchModel searchModel, int traderId);
        Task<TraderInfoModel> PrepareTraderInfoModelAsync(TraderInfoModel model, TraderInfo traderInfo);
        Task<TraderInfoFormModel> PrepareTraderInfoFormModelAsync(TraderInfoFormModel formModel);
    }
    public partial class TraderInfoModelFactory : ITraderInfoModelFactory
    {
        private readonly ITraderInfoService _traderInfoService;
        private readonly ITraderBoardMemberTypeService _traderBoardMemberTypeService;
        private readonly IFieldConfigService _fieldConfigService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public TraderInfoModelFactory(ITraderInfoService traderInfoService,
            ITraderBoardMemberTypeService traderBoardMemberTypeService,
            IFieldConfigService fieldConfigService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _traderInfoService = traderInfoService;
            _traderBoardMemberTypeService = traderBoardMemberTypeService;
            _fieldConfigService = fieldConfigService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<TraderInfoSearchModel> PrepareTraderInfoSearchModelAsync(TraderInfoSearchModel searchModel)
        {
            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.TraderInfoModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<TraderInfoListModel> PrepareTraderInfoListModelAsync(TraderInfoSearchModel searchModel, int traderId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var pagedList = await _traderInfoService.GetPagedListAsync(searchModel, traderId);

            //prepare grid model
            var model = new TraderInfoListModel().PrepareToGrid(searchModel, pagedList);

            return model;
        }

        public virtual Task<TraderInfoModel> PrepareTraderInfoModelAsync(TraderInfoModel model, TraderInfo traderInfo)
        {
            if (traderInfo != null)
            {
                //fill in model values from the entity
                model ??= traderInfo.ToModel<TraderInfoModel>();
            }

            if (traderInfo == null)
            {
                model.CreatedDate = DateTime.UtcNow;
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderInfoModel>(1, nameof(TraderInfoModel.Gravity)),
                ColumnConfig.Create<TraderInfoModel>(2, nameof(TraderInfoModel.SortDescription), ColumnType.Link, link: "/office/trader-info", target: "_blank"),
                ColumnConfig.Create<TraderInfoModel>(3, nameof(TraderInfoModel.CreatedDate), ColumnType.Date)
            };

            return columns;
        }

        public virtual async Task<TraderInfoFormModel> PrepareTraderInfoFormModelAsync(TraderInfoFormModel formModel)
        {
            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderInfoModel>(nameof(TraderInfoModel.SortDescription), FieldType.Text)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderInfoModel>(nameof(TraderInfoModel.Gravity), FieldType.Numeric)
            };

            var center = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderInfoModel>(nameof(TraderInfoModel.Description), FieldType.Textarea)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6", "col-12" }, left, right, center);

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.TraderInfoModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }
    }
}
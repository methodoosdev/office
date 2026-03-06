using App.Core;
using App.Core.Domain.Traders;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Accounting;
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
    public partial interface ITraderMembershipModelFactory
    {
        Task<TraderMembershipSearchModel> PrepareTraderMembershipSearchModelAsync(TraderMembershipSearchModel searchModel);
        Task<TraderMembershipListModel> PrepareTraderMembershipListModelAsync(TraderMembershipSearchModel searchModel, int traderId);
        Task<TraderMembershipModel> PrepareTraderMembershipModelAsync(TraderMembershipModel model, TraderMembership traderMembership);
        Task<TraderMembershipFormModel> PrepareTraderMembershipFormModelAsync(TraderMembershipFormModel formModel);
    }
    public partial class TraderMembershipModelFactory : ITraderMembershipModelFactory
    {
        private readonly ITraderMembershipService _traderMembershipService;
        private readonly ITraderBoardMemberTypeService _traderBoardMemberTypeService;
        private readonly IFieldConfigService _fieldConfigService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public TraderMembershipModelFactory(ITraderMembershipService traderMembershipService,
            ITraderBoardMemberTypeService traderBoardMemberTypeService,
            IFieldConfigService fieldConfigService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _traderMembershipService = traderMembershipService;
            _traderBoardMemberTypeService = traderBoardMemberTypeService;
            _fieldConfigService = fieldConfigService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<TraderMembershipSearchModel> PrepareTraderMembershipSearchModelAsync(TraderMembershipSearchModel searchModel)
        {
            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.TraderMembershipModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<TraderMembershipListModel> PrepareTraderMembershipListModelAsync(TraderMembershipSearchModel searchModel, int traderId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var pagedList = await _traderMembershipService.GetPagedListAsync(searchModel, traderId);

            //prepare grid model
            var model = new TraderMembershipListModel().PrepareToGrid(searchModel, pagedList);

            return model;
        }

        public virtual Task<TraderMembershipModel> PrepareTraderMembershipModelAsync(TraderMembershipModel model, TraderMembership traderMembership)
        {
            if (traderMembership != null)
            {
                //fill in model values from the entity
                model ??= traderMembership.ToModel<TraderMembershipModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderMembershipModel>(1, nameof(TraderMembershipModel.Vat)),
                ColumnConfig.Create<TraderMembershipModel>(2, nameof(TraderMembershipModel.SurnameFatherName), ColumnType.Link, link: "/office/trader-membership", target: "_blank"),
                ColumnConfig.Create<TraderMembershipModel>(3, nameof(TraderMembershipModel.StartDateOn), ColumnType.Date),
                //ColumnConfig.Create<TraderMembershipModel>(4, nameof(TraderMembershipModel.ExpireDateOn), ColumnType.Date),
                ColumnConfig.Create<TraderMembershipModel>(5, nameof(TraderMembershipModel.ParticipationName)),
                ColumnConfig.Create<TraderMembershipModel>(6, nameof(TraderMembershipModel.TraderBoardMemberTypeName)),
                ColumnConfig.Create<TraderMembershipModel>(7, nameof(TraderMembershipModel.ParticipationRate), ColumnType.Percent),
                ColumnConfig.Create<TraderMembershipModel>(8, nameof(TraderMembershipModel.Notes)),
            };

            return columns;
        }

        public virtual async Task<TraderMembershipFormModel> PrepareTraderMembershipFormModelAsync(TraderMembershipFormModel formModel)
        {
            var traderBoardMemberTypesList = await _traderBoardMemberTypeService.GetAllTraderBoardMemberTypesAsync();
            var traderBoardMemberTypes = traderBoardMemberTypesList.Select(x => new SelectionItemList { Value = x.Id, Label = x.Name }).ToList();

            var fields = new List<Dictionary<string, object>>()
            {                
                await _fieldConfigService.GetTradersMultiColumnComboBox<TraderMembershipModel>(nameof(TraderMembershipModel.ParentId), FieldConfigType.IndividualNatural),
                FieldConfig.Create<TraderMembershipModel>(nameof(TraderMembershipModel.Vat), FieldType.Text),
                FieldConfig.Create<TraderMembershipModel>(nameof(TraderMembershipModel.SurnameFatherName), FieldType.Text),
                FieldConfig.Create<TraderMembershipModel>(nameof(TraderMembershipModel.StartDateOnUtc), FieldType.Date),
                //FieldConfig.Create<TraderMembershipModel>(nameof(TraderMembershipModel.ExpireDateOnUtc), FieldType.Date),
                FieldConfig.Create<TraderMembershipModel>(nameof(TraderMembershipModel.ParticipationName), FieldType.Text, _readonly: false),
                FieldConfig.Create<TraderMembershipModel>(nameof(TraderMembershipModel.TraderBoardMemberTypeId), FieldType.GridSelect, options: traderBoardMemberTypes),
                FieldConfig.Create<TraderMembershipModel>(nameof(TraderMembershipModel.ParticipationRate), FieldType.Decimals),
                FieldConfig.Create<TraderMembershipModel>(nameof(TraderMembershipModel.ParticipatingFraction), FieldType.Text),
                FieldConfig.Create<TraderMembershipModel>(nameof(TraderMembershipModel.Notes), FieldType.Textarea)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.TraderMembershipModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}
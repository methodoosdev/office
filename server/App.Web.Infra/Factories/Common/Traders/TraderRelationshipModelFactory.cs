using App.Core;
using App.Core.Domain.Traders;
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
    public partial interface ITraderRelationshipModelFactory
    {
        Task<TraderRelationshipSearchModel> PrepareTraderRelationshipSearchModelAsync(TraderRelationshipSearchModel searchModel);
        Task<TraderRelationshipListModel> PrepareTraderRelationshipListModelAsync(TraderRelationshipSearchModel searchModel, int parentId);
        Task<TraderRelationshipModel> PrepareTraderRelationshipModelAsync(TraderRelationshipModel model, TraderRelationship traderRelationship);
        Task<TraderRelationshipFormModel> PrepareTraderRelationshipFormModelAsync(TraderRelationshipFormModel formModel);
    }
    public partial class TraderRelationshipModelFactory : ITraderRelationshipModelFactory
    {
        private readonly ITraderRelationshipService _traderRelationshipService;
        private readonly ITraderBoardMemberTypeService _traderBoardMemberTypeService;
        private readonly IFieldConfigService _fieldConfigService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public TraderRelationshipModelFactory(ITraderRelationshipService traderRelationshipService,
            ITraderBoardMemberTypeService traderBoardMemberTypeService,
            IFieldConfigService fieldConfigService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _traderRelationshipService = traderRelationshipService;
            _traderBoardMemberTypeService = traderBoardMemberTypeService;
            _fieldConfigService = fieldConfigService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<TraderRelationshipSearchModel> PrepareTraderRelationshipSearchModelAsync(TraderRelationshipSearchModel searchModel)
        {
            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.TraderRelationshipModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<TraderRelationshipListModel> PrepareTraderRelationshipListModelAsync(TraderRelationshipSearchModel searchModel, int parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var pagedList = await _traderRelationshipService.GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new TraderRelationshipListModel().PrepareToGrid(searchModel, pagedList);

            return model;
        }

        public virtual Task<TraderRelationshipModel> PrepareTraderRelationshipModelAsync(TraderRelationshipModel model, TraderRelationship traderRelationship)
        {
            if (traderRelationship != null)
            {
                //fill in model values from the entity
                model ??= traderRelationship.ToModel<TraderRelationshipModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderRelationshipModel>(1, nameof(TraderRelationshipModel.Vat)),
                ColumnConfig.Create<TraderRelationshipModel>(2, nameof(TraderRelationshipModel.SurnameFatherName), ColumnType.Link, link: "/office/trader-relationship", target: "_blank"),
                ColumnConfig.Create<TraderRelationshipModel>(3, nameof(TraderRelationshipModel.StartDateOn), ColumnType.Date),
                //ColumnConfig.Create<TraderRelationshipModel>(4, nameof(TraderRelationshipModel.ExpireDateOn), ColumnType.Date),
                ColumnConfig.Create<TraderRelationshipModel>(5, nameof(TraderRelationshipModel.RelationshipName)),
                ColumnConfig.Create<TraderRelationshipModel>(6, nameof(TraderRelationshipModel.TraderBoardMemberTypeName)),
                ColumnConfig.Create<TraderRelationshipModel>(7, nameof(TraderRelationshipModel.RelationshipRate), ColumnType.Percent),
                ColumnConfig.Create<TraderRelationshipModel>(8, nameof(TraderRelationshipModel.Notes)),
            };

            return columns;
        }

        public virtual async Task<TraderRelationshipFormModel> PrepareTraderRelationshipFormModelAsync(TraderRelationshipFormModel formModel)
        {
            var traderBoardMemberTypesList = await _traderBoardMemberTypeService.GetAllTraderBoardMemberTypesAsync();
            var traderBoardMemberTypes = traderBoardMemberTypesList.Select(x => new SelectionItemList { Value = x.Id, Label = x.Name }).ToList();

            var fields = new List<Dictionary<string, object>>()
            {                
                await _fieldConfigService.GetTradersMultiColumnComboBox<TraderRelationshipModel>(nameof(TraderRelationshipModel.ParentId), FieldConfigType.IndividualNatural),
                FieldConfig.Create<TraderRelationshipModel>(nameof(TraderRelationshipModel.Vat), FieldType.Text),
                FieldConfig.Create<TraderRelationshipModel>(nameof(TraderRelationshipModel.SurnameFatherName), FieldType.Text),
                FieldConfig.Create<TraderRelationshipModel>(nameof(TraderRelationshipModel.StartDateOnUtc), FieldType.Date),
                //FieldConfig.Create<TraderRelationshipModel>(nameof(TraderRelationshipModel.ExpireDateOnUtc), FieldType.Date),
                FieldConfig.Create<TraderRelationshipModel>(nameof(TraderRelationshipModel.RelationshipName), FieldType.Text, _readonly: false),
                FieldConfig.Create<TraderRelationshipModel>(nameof(TraderRelationshipModel.TraderBoardMemberTypeId), FieldType.GridSelect, options: traderBoardMemberTypes),
                FieldConfig.Create<TraderRelationshipModel>(nameof(TraderRelationshipModel.RelationshipRate), FieldType.Decimals),
                FieldConfig.Create<TraderRelationshipModel>(nameof(TraderRelationshipModel.Notes), FieldType.Textarea)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.TraderRelationshipModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}
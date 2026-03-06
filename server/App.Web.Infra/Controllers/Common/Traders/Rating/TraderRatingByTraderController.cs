using App.Core;
using App.Core.Infrastructure;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Traders;
using App.Services;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using App.Web.Infra.Factories.Common.Traders;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Traders
{
    [CheckCustomerPermission(true)]
    public partial class TraderRatingByTraderController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly ITraderRatingService _traderRatingService;
        private readonly ITraderRatingModelFactory _traderRatingModelFactory;
        private readonly ITraderRatingTraderMappingService _traderRatingTraderMappingService;
        private readonly IModelFactoryService _modelFactoryService;

        public TraderRatingByTraderController(
            ITraderService traderService,
            ITraderRatingService traderRatingService,
            ITraderRatingModelFactory traderRatingModelFactory,
            ITraderRatingTraderMappingService traderRatingTraderMappingService,
            IModelFactoryService modelFactoryService)
        {
            _traderService = traderService;
            _traderRatingService = traderRatingService;
            _traderRatingModelFactory = traderRatingModelFactory;
            _traderRatingTraderMappingService = traderRatingTraderMappingService;
            _modelFactoryService = modelFactoryService;
        }

        private async Task<IPagedList<TraderRatingModel>> GetTraderRatingByTraderIdAsync(TraderRatingSearchModel searchModel, int traderId)
        {
            var traderRatings = await _traderRatingTraderMappingService.GetTraderRatingByTraderIdAsync(traderId);

            var categories = await _modelFactoryService.GetAllTraderRatingCategoriesAsync(false);
            var departments = await _modelFactoryService.GetAllDepartmentsAsync(false);

            var query = traderRatings.AsEnumerable().Select(x =>
            {
                var model = x.ToModel<TraderRatingModel>();
                model.CategoryName = categories.FirstOrDefault(c => c.Value == x.TraderRatingCategoryId).Label;
                model.DepartmentName = departments.FirstOrDefault(c => c.Value == x.DepartmentId).Label;

                return model;
            }).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.Description.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.CategoryName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.DepartmentName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _traderRatingModelFactory.PrepareTraderRatingSearchModelAsync(new TraderRatingSearchModel(), true);

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TraderRatingSearchModel searchModel, int parentId)
        {
            //try to get entity with the specified id
            var trader = await _traderService.GetTraderByIdAsync(parentId);
            if (trader == null)
                return await AccessDenied();

            var traders = await GetTraderRatingByTraderIdAsync(searchModel, trader.Id);

            //prepare grid model
            var model = new TraderRatingListModel().PrepareToGrid(searchModel, traders);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportMapping([FromBody] ICollection<int> selectedIds, int parentId)
        {
            //try to get entity with the specified id
            var trader = await _traderService.GetTraderByIdAsync(parentId);
            if (trader == null)
                return await AccessDenied();

            var traderRatings = await _traderRatingService.GetTraderRatingByIdsAsync(selectedIds.ToArray());

            foreach (var selected in traderRatings)
            {
                await _traderRatingTraderMappingService.InsertTraderRatingTraderAsync(selected, trader);
            }

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> RemoveMapping([FromBody] ICollection<int> selectedIds, int parentId)
        {
            var trader = await _traderService.GetTraderByIdAsync(parentId);
            if (trader == null)
                return await AccessDenied();

            var traderRatings = await _traderRatingService.GetTraderRatingByIdsAsync(selectedIds.ToArray());

            foreach (var selected in traderRatings)
            {
                await _traderRatingTraderMappingService.RemoveTraderRatingTraderAsync(selected, trader);
            }

            return Ok();
        }

    }
}
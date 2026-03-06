using App.Core;
using App.Core.Domain.Directory;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Directory;
using App.Services.Localization;
using App.Services.Offices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Offices
{
    public partial interface IBookmarkModelFactory
    {
        Task<BookmarkSearchModel> PrepareBookmarkSearchModelAsync(BookmarkSearchModel searchModel);
        Task<BookmarkListModel> PrepareBookmarkListModelAsync(BookmarkSearchModel searchModel);
        Task<BookmarkModel> PrepareBookmarkModelAsync(BookmarkModel model, Bookmark bookmark);
        Task<BookmarkFormModel> PrepareBookmarkFormModelAsync(BookmarkFormModel formModel);
        Task<List<BookmarkModel>> GetLoadListAsync(int customerId);
    }
    public partial class BookmarkModelFactory : IBookmarkModelFactory
    {
        private readonly IBookmarkService _bookmarkService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public BookmarkModelFactory(
            IBookmarkService bookmarkService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _bookmarkService = bookmarkService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<BookmarkModel>> GetPagedListAsync(BookmarkSearchModel searchModel)
        {
            var customerId = (await _workContext.GetCurrentCustomerAsync()).Id;

            var query = _bookmarkService.Table.AsEnumerable()
                .Where(b => b.CustomerId == customerId)
                .Select(x => x.ToModel<BookmarkModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.UrlPath.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<BookmarkSearchModel> PrepareBookmarkSearchModelAsync(BookmarkSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.BookmarkModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<BookmarkListModel> PrepareBookmarkListModelAsync(BookmarkSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var bookmarks = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new BookmarkListModel().PrepareToGrid(searchModel, bookmarks);

            return model;
        }

        public virtual async Task<BookmarkModel> PrepareBookmarkModelAsync(BookmarkModel model, Bookmark bookmark)
        {
            if (bookmark != null)
            {
                //fill in model values from the entity
                model ??= bookmark.ToModel<BookmarkModel>();
            }

            if (bookmark == null)
            {
                model.CustomerId = (await _workContext.GetCurrentCustomerAsync()).Id;
            }

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<BookmarkModel>(1, nameof(BookmarkModel.UrlPath), ColumnType.RouterLink),
                ColumnConfig.Create<BookmarkModel>(2, nameof(BookmarkModel.Description)),
                ColumnConfig.Create<BookmarkModel>(3, nameof(BookmarkModel.DisplayOrder))
            };

            return columns;
        }

        public virtual async Task<BookmarkFormModel> PrepareBookmarkFormModelAsync(BookmarkFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<BookmarkModel>(nameof(BookmarkModel.UrlPath), FieldType.Text),
                FieldConfig.Create<BookmarkModel>(nameof(BookmarkModel.Description), FieldType.Text),
                FieldConfig.Create<BookmarkModel>(nameof(BookmarkModel.DisplayOrder), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.BookmarkModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }

        public async Task<List<BookmarkModel>> GetLoadListAsync(int customerId)
        {
            var query = _bookmarkService.Table.AsEnumerable()
                .Where(b => b.CustomerId == customerId)
                .Select(x => x.ToModel<BookmarkModel>())
                .AsQueryable();

            query = query.OrderBy(x => x.DisplayOrder);

            return await query.ToListAsync();
        }
    }
}
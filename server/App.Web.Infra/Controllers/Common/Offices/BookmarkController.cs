using App.Core;
using App.Core.Domain.Directory;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Directory;
using App.Services.Localization;
using App.Services.Offices;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using App.Web.Infra.Factories.Common.Offices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Offices
{
    [CheckCustomerPermission(true)]
    public partial class BookmarkController : BaseProtectController
    {
        private readonly IBookmarkService _bookmarkService;
        private readonly ILocalizationService _localizationService;
        private readonly IBookmarkModelFactory _bookmarkModelFactory;
        private readonly IWorkContext _workContext;

        public BookmarkController(
            IBookmarkService bookmarkService,
            ILocalizationService localizationService,
            IBookmarkModelFactory bookmarkModelFactory,
            IWorkContext workContext)
        {
            _bookmarkService = bookmarkService;
            _localizationService = localizationService;
            _bookmarkModelFactory = bookmarkModelFactory;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _bookmarkModelFactory.PrepareBookmarkSearchModelAsync(new BookmarkSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] BookmarkSearchModel searchModel)
        {
            //prepare model
            var model = await _bookmarkModelFactory.PrepareBookmarkListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _bookmarkModelFactory.PrepareBookmarkModelAsync(new BookmarkModel(), null);

            //prepare form
            var formModel = await _bookmarkModelFactory.PrepareBookmarkFormModelAsync(new BookmarkFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] BookmarkModel model)
        {
            if (ModelState.IsValid)
            {
                var bookmark = model.ToEntity<Bookmark>();
                await _bookmarkService.InsertBookmarkAsync(bookmark);

                return Json(bookmark.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var bookmark = await _bookmarkService.GetBookmarkByIdAsync(id);
            if (bookmark == null)
                return await AccessDenied();

            //prepare model
            var model = await _bookmarkModelFactory.PrepareBookmarkModelAsync(null, bookmark);

            //prepare form
            var formModel = await _bookmarkModelFactory.PrepareBookmarkFormModelAsync(new BookmarkFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] BookmarkModel model)
        {
            //try to get entity with the specified id
            var bookmark = await _bookmarkService.GetBookmarkByIdAsync(model.Id);
            if (bookmark == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    bookmark = model.ToEntity(bookmark);
                    await _bookmarkService.UpdateBookmarkAsync(bookmark);

                    return Json(bookmark.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Bookmarks.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var bookmark = await _bookmarkService.GetBookmarkByIdAsync(id);
            if (bookmark == null)
                return await AccessDenied();

            try
            {
                await _bookmarkService.DeleteBookmarkAsync(bookmark);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Bookmarks.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _bookmarkService.DeleteBookmarkAsync((await _bookmarkService.GetBookmarksByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Bookmarks.Errors.TryToDelete");
            }
        }

        [CheckCustomerPermission(true)]
        public virtual async Task<IActionResult> LoadList(int parentId)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            //prepare model
            var model = await _bookmarkModelFactory.GetLoadListAsync(customer == null ? 0 : customer.Id);

            return Json(model);
        }
    }
}
using App.Models.Logging;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Offices;
using App.Web.Admin.Factories.Logging;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Admin.Controllers
{
    public partial class LogController : BaseProtectController
    {
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly ILogModelFactory _logModelFactory;
        private readonly IPersistStateService _persistStateService;

        public LogController(
            ILocalizationService localizationService,
            ILogger logger,
            ILogModelFactory logModelFactory,
            IPersistStateService persistStateService)
        {
            _localizationService = localizationService;
            _logger = logger;
            _logModelFactory = logModelFactory;
            _persistStateService = persistStateService;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _logModelFactory.PrepareLogSearchModelAsync(new LogSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] LogSearchModel searchModel)
        {
            //prepare model
            var model = await _logModelFactory.PrepareLogListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get a log with the specified id
            var log = await _logger.GetLogByIdAsync(id);
            if (log == null)
                return await AccessDenied();

            //prepare model
            var model = await _logModelFactory.PrepareLogModelAsync(null, log);

            //prepare form
            var formModel = await _logModelFactory.PrepareLogFormModelAsync(new LogFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a log with the specified id
            var log = await _logger.GetLogByIdAsync(id);
            if (log == null)
                return await AccessDenied();

            await _logger.DeleteLogAsync(log);

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            await _logger.DeleteLogsAsync((await _logger.GetLogByIdsAsync(selectedIds.ToArray())).ToList());

            return Json(new { Result = true });
        }

        public virtual async Task<IActionResult> ClearAll()
        {
            await _logger.ClearLogAsync();

            return Ok();
        }

    }
}
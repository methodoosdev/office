using App.Core;
using App.Core.Domain.Common;
using App.Models.Common;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Offices;
using App.Services.Security;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using App.Web.Infra.Factories.Common.Offices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Offices
{
    public partial class PersistStateController : BaseProtectController
    {
        private readonly IPersistStateService _persistStateService;
        private readonly ILocalizationService _localizationService;
        private readonly IPersistStateModelFactory _persistStateModelFactory;
        private readonly IWorkContext _workContext;

        public PersistStateController(
            IPersistStateService persistStateService,
            ILocalizationService localizationService,
            IPersistStateModelFactory persistStateModelFactory,
            IWorkContext workContext)
        {
            _persistStateService = persistStateService;
            _localizationService = localizationService;
            _persistStateModelFactory = persistStateModelFactory;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _persistStateModelFactory.PreparePersistStateSearchModelAsync(new PersistStateSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] PersistStateSearchModel searchModel)
        {
            //prepare model
            var model = await _persistStateModelFactory.PreparePersistStateListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var persistState = await _persistStateService.GetPersistStateByIdAsync(id);
            if (persistState == null)
                return await AccessDenied();

            try
            {
                await _persistStateService.DeletePersistStateAsync(persistState);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.PersistStates.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _persistStateService.DeletePersistStateAsync((await _persistStateService.GetPersistStatesByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.PersistStates.Errors.TryToDelete");
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [CheckCustomerPermission(true)]
        public virtual async Task<IActionResult> SaveState([FromBody] JObject obj, string modelType)
        {
            //if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            //    return await AccessDenied();

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer == null)
                return await AccessDenied();

            //var entityType = obj.GetValue("__entityType").ToString();

            var type = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("App.")).ToList()
                .Select(a => a.GetTypes().FirstOrDefault(x => x.Name == modelType))
                .FirstOrDefault(t => t != null);

            if (type == null)
                return await BadRequestMessageAsync($"ModelType ({modelType}) cannot by instantiated");

            var persistState = await _persistStateService.GetPersistStateJsonValueAsync(type);

            if (persistState == null)
            {
                persistState = new PersistState
                {
                    CustomerId = customer.Id,
                    ModelType = modelType,
                    JsonValue = JsonConvert.SerializeObject(obj)
                };
                await _persistStateService.InsertPersistStateAsync(persistState);
            }
            else
            {
                persistState.JsonValue = JsonConvert.SerializeObject(obj);

                await _persistStateService.UpdatePersistStateAsync(persistState);
            }

            return Ok();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [CheckCustomerPermission(true)]
        public virtual async Task<IActionResult> RemoveState(string modelType)
        {
            //if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageMaintenance))
            //    return await AccessDenied();

            var type = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("App.")).ToList()
                .Select(a => a.GetTypes().FirstOrDefault(x => x.Name == modelType))
                .FirstOrDefault(t => t != null);

            if (type == null)
                return await BadRequestMessageAsync($"ModelType ({modelType}) cannot by instantiated");

            var persistState = await _persistStateService.GetPersistStateJsonValueAsync(type);

            if (persistState != null)
                await _persistStateService.DeletePersistStateAsync(persistState);

            return Ok(persistState != null);
        }

    }
}
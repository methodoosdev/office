using App.Core.Caching;
using App.Core.Domain.Localization;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Localization;
using App.Services.Installation;
using App.Services.Localization;
using App.Web.Admin.Factories;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Admin.Controllers
{
    public partial class LanguageController : BaseProtectController
    {
        private readonly ILanguageModelFactory _languageModelFactory;
        private readonly ILanguageService _languageService;
        private readonly IInstallStringResourcesService _installStringResourcesService;
        private readonly IStaticCacheManager _staticCacheManager;

        public LanguageController(
            ILanguageModelFactory languageModelFactory,
            ILanguageService languageService,
            IInstallStringResourcesService installStringResourcesService,
            IStaticCacheManager staticCacheManager)
        {
            _languageModelFactory = languageModelFactory;
            _languageService = languageService;
            _installStringResourcesService = installStringResourcesService;
            _staticCacheManager = staticCacheManager;

        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _languageModelFactory.PrepareLanguageSearchModelAsync(new LanguageSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] LanguageSearchModel searchModel)
        {
            //prepare model
            var model = await _languageModelFactory.PrepareLanguageListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _languageModelFactory.PrepareLanguageModelAsync(new LanguageModel(), null);

            //prepare form
            var formModel = await _languageModelFactory.PrepareLanguageFormModelAsync(new LanguageFormModel(), model);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] LanguageModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var language = model.ToEntity<Language>();
                await _languageService.InsertLanguageAsync(language);

                return Json(language.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get a language with the specified id
            var language = await _languageService.GetLanguageByIdAsync(id);
            if (language == null)
                return await AccessDenied();

            //prepare model
            var model = await _languageModelFactory.PrepareLanguageModelAsync(null, language);

            //prepare form
            var formModel = await _languageModelFactory.PrepareLanguageFormModelAsync(new LanguageFormModel(), model);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody]LanguageModel model, bool continueEditing)
        {
            //try to get a language with the specified id
            var language = await _languageService.GetLanguageByIdAsync(model.Id);
            if (language == null)
                return await AccessDenied();

            if (ModelState.IsValid)
            {
                //ensure we have at least one published language
                var allLanguages = await _languageService.GetAllLanguagesAsync();
                if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id && !model.Published)
                    return await AccessDenied();

                //update
                language = model.ToEntity(language);
                await _languageService.UpdateLanguageAsync(language);

                return Json(language.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a language with the specified id
            var language = await _languageService.GetLanguageByIdAsync(id);
            if (language == null)
                return await AccessDenied();

            //ensure we have at least one published language
            var allLanguages = await _languageService.GetAllLanguagesAsync();
            if (allLanguages.Count == 1 && allLanguages[0].Id == language.Id)
                return BadRequest("Must have at least one published language.");

            //delete
            await _languageService.DeleteLanguageAsync(language);

            return Ok();
        }

        public virtual async Task<IActionResult> ImportResources()
        {
            try
            {
                await _installStringResourcesService.ImportResourcesFromJsonAsync();
                await _staticCacheManager.ClearAsync();
                return Ok();
            }
            catch
            {
                return BadRequest("Failed to import StringResources from json file.");
            }
        }
    }
}
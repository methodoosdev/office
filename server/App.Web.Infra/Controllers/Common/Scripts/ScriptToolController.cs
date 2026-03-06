using App.Core.Domain.Scripts;
using App.Core.Infrastructure;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Scripts;
using App.Models.Traders;
using App.Services.ExportImport;
using App.Services.Localization;
using App.Services.Scripts;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Scripts;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Scripts
{
    public partial class ScriptToolController : BaseProtectController
    {
        protected readonly ITraderService _traderService;
        protected readonly IScriptToolService _scriptToolService;
        protected readonly ILocalizationService _localizationService;
        protected readonly IScriptToolModelFactory _scriptToolModelFactory;
        protected readonly IExcelTemplateService _excelTemplateService;
        protected readonly INopFileProvider _fileProvider;

        public ScriptToolController(
            ITraderService traderService,
            IScriptToolService scriptToolService,
            ILocalizationService localizationService,
            IScriptToolModelFactory scriptToolModelFactory,
            IExcelTemplateService excelTemplateService,
            INopFileProvider fileProvider)
        {
            _traderService = traderService;
            _scriptToolService = scriptToolService;
            _localizationService = localizationService;
            _scriptToolModelFactory = scriptToolModelFactory;
            _excelTemplateService = excelTemplateService;
            _fileProvider = fileProvider;
        }

        public virtual async Task<IActionResult> Config(int parentId)
        {
            //prepare model
            var configModel = await _scriptToolModelFactory.PrepareScriptToolConfigModelAsync(new ScriptToolConfigModel(), parentId);

            return Json(new { configModel });
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _scriptToolModelFactory.PrepareScriptToolSearchModelAsync(new ScriptToolSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ScriptToolSearchModel searchModel, int parentId)
        {
            //prepare model
            var model = await _scriptToolModelFactory.PrepareScriptToolListModelAsync(searchModel, parentId);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create(int parentId)
        {
            //prepare model
            var model = await _scriptToolModelFactory.PrepareScriptToolModelAsync(new ScriptToolModel(), null);
            model.TraderId = parentId;

            //prepare form
            var formModel = await _scriptToolModelFactory.PrepareScriptToolFormModelAsync(new ScriptToolFormModel(), parentId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] ScriptToolModel model)
        {
            if (ModelState.IsValid)
            {
                var scriptTool = model.ToEntity<ScriptTool>();
                await _scriptToolService.InsertScriptToolAsync(scriptTool);

                return Json(scriptTool.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var scriptTool = await _scriptToolService.GetScriptToolByIdAsync(id);
            if (scriptTool == null)
                return await AccessDenied();

            //prepare model
            var model = await _scriptToolModelFactory.PrepareScriptToolModelAsync(null, scriptTool);

            //prepare form
            var formModel = await _scriptToolModelFactory.PrepareScriptToolFormModelAsync(new ScriptToolFormModel(), scriptTool.TraderId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] ScriptToolModel model)
        {
            //try to get entity with the specified id
            var scriptTool = await _scriptToolService.GetScriptToolByIdAsync(model.Id);
            if (scriptTool == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    scriptTool = model.ToEntity(scriptTool);
                    await _scriptToolService.UpdateScriptToolAsync(scriptTool);

                    return Json(scriptTool.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptTools.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var scriptTool = await _scriptToolService.GetScriptToolByIdAsync(id);
            if (scriptTool == null)
                return await AccessDenied();

            try
            {
                await _scriptToolService.DeleteScriptToolAsync(scriptTool);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptTools.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _scriptToolService.DeleteScriptToolAsync((await _scriptToolService.GetScriptToolsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptTools.Errors.TryToDelete");
            }
        }
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 268435456)]
        [RequestSizeLimit(268435456)]
        public async Task<IActionResult> Upload1(int id, IFormFile file)
        {
            if (id == 0)
                return BadRequest("No scriptTool found with zero id");

            var scriptTool = await _scriptToolService.GetScriptToolByIdAsync(id);
            if (scriptTool is null)
                return BadRequest("No scriptTool found with the specified id");

            try
            {
                if (file == null || file.Length == 0)
                    return Json(new { success = false, message = "No file uploaded." });

                var fileName = _fileProvider.GetFileName(file.FileName); // strips any path
                var ext = _fileProvider.GetFileExtension(fileName)?.ToLowerInvariant();

                var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    ".xlsx", ".xlsm"
                };
                if (string.IsNullOrEmpty(ext) || !allowed.Contains(ext))
                    return BadRequest("Only .xlsx/.xlsm are supported.");

                // Read into memory once
                byte[] bytes;
                await using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    bytes = stream.ToArray();
                }

                // Content type fallback
                var contentType = string.IsNullOrWhiteSpace(file.ContentType)
                    ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    : file.ContentType;

                // Assign
                scriptTool.FileName = fileName;
                scriptTool.Extension = ext;
                scriptTool.ContentType = contentType;
                scriptTool.SizeBytes = bytes.Length;
                scriptTool.Bytes = bytes;
                scriptTool.CreatedOnUtc = DateTime.UtcNow;

                await _scriptToolService.UpdateScriptToolAsync(scriptTool);

                return Json(new { success = true });
            }
            catch (Exception exc)
            {
                // Log exc for details (inner exceptions often reveal size/mapping issues)
                return Json(new
                {
                    success = false,
                    message = $"Cannot insert Excel file. {exc.Message}"
                });
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Upload(int id, IFormCollection form)
        {
            if (id == 0)
                throw new ArgumentException("No scriptTool found with zero id");

            //try to get a scriptTool with the specified id
            var scriptTool = await _scriptToolService.GetScriptToolByIdAsync(id)
                ?? throw new ArgumentException("No scriptTool found with the specified id");

            try
            {
                var files = form.Files.ToList();
                if (!files.Any())
                    return Json(new { success = false });

                var formFile = files.First();

                //await using var fileStream = formFile.OpenReadStream();
                //await using var ms = new MemoryStream();
                //await fileStream.CopyToAsync(ms);
                //var fileBytes = ms.ToArray();

                // Read into memory once
                byte[] fileBytes;
                await using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream);
                    fileBytes = stream.ToArray();
                }

                var fileName = formFile.FileName;

                //remove path (passed in IE)
                fileName = _fileProvider.GetFileName(fileName);

                var contentType = string.IsNullOrWhiteSpace(formFile.ContentType)
                                  ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                                  : formFile.ContentType;

                var fileExtension = _fileProvider.GetFileExtension(fileName);
                if (!string.IsNullOrEmpty(fileExtension))
                    fileExtension = fileExtension.ToLowerInvariant();

                var imgExt = new List<string> { ".xlsx", ".xlsm" } as IReadOnlyCollection<string>;

                if (imgExt.All(ext => !ext.Equals(fileExtension, StringComparison.CurrentCultureIgnoreCase)))
                    return BadRequest("Only .xlsx/.xlsm are supported.");

                scriptTool.FileName = fileName;
                scriptTool.Extension = fileExtension;
                scriptTool.ContentType = contentType;
                scriptTool.SizeBytes = fileBytes.Length;
                scriptTool.Bytes = fileBytes;
                scriptTool.CreatedOnUtc = DateTime.UtcNow;

                await _scriptToolService.UpdateScriptToolAsync(scriptTool);
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = $"Cannot insert Excel file. {exc.Message}",
                });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DownloadPrototype(int id)
        {
            try
            {
                //try to get a customer role with the specified id
                var scriptTool = await _scriptToolService.GetScriptToolByIdAsync(id);
                if (scriptTool == null)
                    return await AccessDenied();

                var request = new ScriptToolFillRequest();

                return File(scriptTool.Bytes, scriptTool.ContentType, scriptTool.FileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DownloadExcel([FromBody] ScriptToolConfigModel config, int id)
        {
            try 
            {
                //try to get a customer role with the specified id
                var scriptTool = await _scriptToolService.GetScriptToolByIdAsync(id);
                if (scriptTool == null)
                    return await AccessDenied();

                var request = new ScriptToolFillRequest();

                // Discover tokens first (from original bytes)
                using (var inMs = new MemoryStream(scriptTool.Bytes, writable: false))
                using (var inWb = new XLWorkbook(inMs))
                {
                    var tokens = _excelTemplateService.DiscoverTokens(inWb);

                    // If no mapping was provided, build a demo/default one (replace with your resolver)
                    if (request.Mapping == null)
                        request = request with { Mapping = await BuildMapping(scriptTool.TraderId, tokens, config) };
                }

                // Apply mapping to a fresh copy of the stored template
                using var fresh = new MemoryStream(scriptTool.Bytes);
                using var outWb = new XLWorkbook(fresh);

                _excelTemplateService.ApplyMapping(outWb, request.Mapping);

                using var outMs = new MemoryStream();
                outWb.SaveAs(outMs);
                outMs.Position = 0;

                var downloadName = string.IsNullOrWhiteSpace(request.DownloadFileName)
                    ? _fileProvider.GetFileNameWithoutExtension(scriptTool.FileName) + "_filled" + scriptTool.Extension
                    : EnsureExtension(request.DownloadFileName!, scriptTool.Extension);

                return File(outMs.ToArray(), App.Core.MimeTypes.TextXlsx, downloadName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(ex.Message);
            }

            //return File(
            //    fileContents: outMs.ToArray(),
            //    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            //    fileDownloadName: downloadName
            //);
        }

        private async Task<Dictionary<string, object>> BuildMapping(int traderId, IEnumerable<string> tokens, ScriptToolConfigModel config)
        {
            var dict = await _scriptToolModelFactory.PrepareScriptReplacementAsync(traderId, tokens.ToArray(), config);

            var _trader = await _traderService.GetTraderByIdAsync(traderId);
            var trader = _trader.ToTraderModel();

            dict.Add("#Address", trader.JobAddress);
            dict.Add("#City", trader.JobCity);
            dict.Add("#Phone", trader.JobPhoneNumber1);
            dict.Add("#Postcode", trader.JobPostcode);
            dict.Add("#Email", trader.Email);
            dict.Add("#TraderName", _trader.ToTraderFullName());

            return dict;
        }

        private static string EnsureExtension(string name, string ext)
            => Path.GetExtension(name).Equals(ext, StringComparison.OrdinalIgnoreCase) ? name : name + ext;

        private static string FormatBytes(long bytes, int decimals = 1)
        {
            if (bytes <= 0) return "0 KB";
            string[] sizes = { "Bytes", "KB", "MB", "GB", "TB" };
            var order = (int)Math.Floor(Math.Log(bytes, 1024));
            var value = bytes / Math.Pow(1024, order);
            var dp = order == 0 ? 0 : decimals;
            return $"{value}:F{dp} {sizes[order]}";
        }
    }
}
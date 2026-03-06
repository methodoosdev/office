using App.Core;
using App.Core.Domain.Accounting;
using App.Core.Domain.Logging;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Accounting;
using App.Core.Infrastructure.Mapper;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Accounting;
using App.Models.Traders;
using App.Services.Accounting;
using App.Services.Common;
using App.Services.Customers;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Traders;
using App.Web.Accounting.Factories;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace App.Web.Accounting.Controllers
{
    public partial class PeriodicF2Controller : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly IPeriodicF2Service _periodicF2Service;
        private readonly ILocalizationService _localizationService;
        private readonly IPeriodicF2ModelFactory _periodicF2ModelFactory;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly PlaywrightHttpClient _httpClient;

        public PeriodicF2Controller(
            ITraderService traderService,
            IPeriodicF2Service periodicF2Service,
            ILocalizationService localizationService,
            IPeriodicF2ModelFactory periodicF2ModelFactory,
            ITraderConnectionService traderConnectionService,
            ICustomerActivityService customerActivityService,
            PlaywrightHttpClient httpClient)
        {
            _traderService = traderService;
            _periodicF2Service = periodicF2Service;
            _localizationService = localizationService;
            _periodicF2ModelFactory = periodicF2ModelFactory;
            _traderConnectionService = traderConnectionService;
            _customerActivityService = customerActivityService;
            _httpClient = httpClient;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _periodicF2ModelFactory.PreparePeriodicF2SearchModelAsync(new PeriodicF2SearchModel());

            //prepare model
            var dialogModel = await _periodicF2ModelFactory.PreparePeriodicF2DialogFormModelAsync(new PeriodicF2DialogFormModel());

            return Json(new { searchModel, dialogModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] PeriodicF2SearchModel searchModel)
        {
            //prepare model
            var model = await _periodicF2ModelFactory.PreparePeriodicF2ListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _periodicF2ModelFactory.PreparePeriodicF2ModelAsync(new PeriodicF2Model(), null);

            //prepare form
            var formModel = await _periodicF2ModelFactory.PreparePeriodicF2FormModelAsync(new PeriodicF2FormModel());

            //prepare model
            var dialogModel = await _periodicF2ModelFactory.PreparePeriodicF2DialogFormModelAsync(new PeriodicF2DialogFormModel());

            return Json(new { model, formModel, dialogModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] PeriodicF2Model model)
        {
            if (ModelState.IsValid)
            {
                var periodicF2 = model.ToEntity<PeriodicF2>();
                await _periodicF2Service.InsertPeriodicF2Async(periodicF2);

                return Json(periodicF2.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var periodicF2 = await _periodicF2Service.GetPeriodicF2ByIdAsync(id);
            if (periodicF2 == null)
                return await AccessDenied();

            //prepare model
            var model = await _periodicF2ModelFactory.PreparePeriodicF2ModelAsync(null, periodicF2);

            //prepare form
            var formModel = await _periodicF2ModelFactory.PreparePeriodicF2FormModelAsync(new PeriodicF2FormModel());

            //prepare model
            var dialogModel = await _periodicF2ModelFactory.PreparePeriodicF2DialogFormModelAsync(new PeriodicF2DialogFormModel());

            return Json(new { model, formModel, dialogModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] PeriodicF2Model model)
        {
            //try to get entity with the specified id
            var periodicF2 = await _periodicF2Service.GetPeriodicF2ByIdAsync(model.Id);
            if (periodicF2 == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    periodicF2 = model.ToEntity(periodicF2);
                    await _periodicF2Service.UpdatePeriodicF2Async(periodicF2);

                    return Json(periodicF2.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.PeriodicF2s.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var periodicF2 = await _periodicF2Service.GetPeriodicF2ByIdAsync(id);
            if (periodicF2 == null)
                return await AccessDenied();

            try
            {
                await _periodicF2Service.DeletePeriodicF2Async(periodicF2);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.PeriodicF2s.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _periodicF2Service.DeletePeriodicF2Async((await _periodicF2Service.GetPeriodicF2ByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.PeriodicF2s.Errors.TryToDelete");
            }
        }

        public virtual async Task<IActionResult> Generate([FromBody] PeriodicF2DialogModel dialogModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(dialogModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            //prepare model
            var model = await _periodicF2ModelFactory.PreparePeriodicF2GenerateAsync(new PeriodicF2Model(), dialogModel, connectionResult.Connection);

            var periodicF2 = model.ToEntity<PeriodicF2>();
            await _periodicF2Service.InsertPeriodicF2Async(periodicF2);

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> Retrieve([FromBody] PeriodicF2DialogModel dialogModel, string connectionId)
        {
            var trader = await _traderService.GetTraderByIdAsync(dialogModel.TraderId);
            if (trader == null)
                return await AccessDenied();

            var custActivity = new CustomerActivityResult();

            var traderModel = trader.ToTraderModel();
            var traderName = traderModel.FullName();
            var traderVat = traderModel.Vat.Trim();

            var taxPeriod = _periodicF2ModelFactory.TaxPeriodByCategoryBookType(traderModel.CategoryBookTypeId, dialogModel.Period);
            var pageCredential = await _periodicF2ModelFactory.GetPageCredentialAsync(traderModel, dialogModel.Representative);
            var year = dialogModel.Date.Year;

            var format = "{0}?traderName={1}&vat={2}&pageKindTypeId={3}&year={4}&from={5}&to={6}&userName={7}&password={8}&connectionId={9}";
            var url = string.Format(format,
                "api/periodicF2/retrieve",
                WebUtility.UrlEncode(traderName),
                traderVat,
                (int)pageCredential.PageCredentialType,
                year,
                taxPeriod.First(),
                taxPeriod.Last(),
                WebUtility.UrlEncode(pageCredential.UserName),
                WebUtility.UrlEncode(pageCredential.Password),
                connectionId == "undefined" ? null : connectionId);

            var result = await _httpClient.SendAsync(HttpMethod.Post, url);
            if (result.Success)
            {
                var response = JsonConvert.DeserializeObject<DtoListResponse<PeriodicF2Result>>(result.Content);
                if (response.Success)
                {
                    IList<PeriodicF2Model> list = new List<PeriodicF2Model>();
                    foreach (var item in response.List)
                    {
                        var model = AutoMapperConfiguration.Mapper.Map<PeriodicF2Model>(item);
                        model.TraderId = traderModel.Id;
                        model.F001b = year;
                        model.F004 = year;
                        model.F101 = traderName;
                        model.F104 = traderVat;
                        model.F005a = new DateTime(year, taxPeriod.First(), 1).Date;
                        model.F005b = new DateTime(year, taxPeriod.Last(), 1).AddMonths(1).AddDays(-1).Date;
                        model.F006 = dialogModel.Period;
                        model.SubmitModeTypeId = (int)SubmitModeType.Submited; // Υποβλήθηκε

                        list.Add(model);
                    }

                    var ids = _periodicF2Service.Table.Where(x => x.TraderId == traderModel.Id).Select(s => s.RegistrationNumber).ToList();
                    var periodicF2List = list.Select(x => x.ToEntity<PeriodicF2>()).ToList();

                    foreach (var entity in periodicF2List)
                        if (!ids.Contains(entity.RegistrationNumber))
                            await _periodicF2Service.InsertPeriodicF2Async(entity);

                    custActivity.AddSuccess(response.Message);
                }
                else
                    custActivity.AddError(response.Error);
            }
            else
                custActivity.AddError(result.Error);

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.PeriodicF2Retrieve, custActivity.ToString());

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> Submit(int id, bool representative, string connectionId)
        {
            //try to get a customer role with the specified id
            var periodicF2 = await _periodicF2Service.GetPeriodicF2ByIdAsync(id);
            if (periodicF2 == null)
                return await AccessDenied();

            var dto = AutoMapperConfiguration.Mapper.Map<PeriodicF2Result>(periodicF2);

            var trader = await _traderService.GetTraderByIdAsync(periodicF2.TraderId);
            if (trader == null)
                return await AccessDenied();

            var custActivity = new CustomerActivityResult();

            var traderModel = trader.ToTraderModel();
            var traderName = traderModel.FullName();
            var traderVat = traderModel.Vat.Trim();

            var taxPeriod = _periodicF2ModelFactory.TaxPeriodByCategoryBookType(traderModel.CategoryBookTypeId, periodicF2.F006);
            var pageCredential = await _periodicF2ModelFactory.GetPageCredentialAsync(traderModel, representative);

            var format = "{0}?traderName={1}&vat={2}&pageKindTypeId={3}&f007={4}&year={5}&from={6}&to={7}&userName={8}&password={9}&connectionId={10}";
            var url = string.Format(format,
                "api/periodicF2/submit",
                WebUtility.UrlEncode(traderName),
                traderVat,
                (int)pageCredential.PageCredentialType,
                periodicF2.F007,
                periodicF2.F004,
                taxPeriod.First(),
                taxPeriod.Last(),
                WebUtility.UrlEncode(pageCredential.UserName),
                WebUtility.UrlEncode(pageCredential.Password),
                connectionId == "undefined" ? null : connectionId);

            var result = await _httpClient.SendAsync(HttpMethod.Post, url, dto);
            if (result.Success)
            {
                var response = JsonConvert.DeserializeObject<DtoListResponse<string>>(result.Content);
                if (response.Success)
                {
                    periodicF2.RegistrationNumber = response.List.FirstOrDefault();
                    periodicF2.SubmitModeTypeId = (int)SubmitModeType.Submited;
                    await _periodicF2Service.UpdatePeriodicF2Async(periodicF2);

                    custActivity.AddSuccess(response.Message);
                }
                else
                    custActivity.AddError(response.Error);
            }
            else
                custActivity.AddError(result.Error);

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.PeriodicF2, custActivity.ToString());

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> IdentityPayment([FromBody] PeriodicF2DialogModel dialogModel, string connectionId)
        {
            var _trader = await _traderService.GetTraderByIdAsync(dialogModel.TraderId);
            if (_trader == null)
                return await AccessDenied();

            var custActivity = new CustomerActivityResult();

            var traderModel = _trader.ToTraderModel();
            var traderName = traderModel.FullName();
            var traderVat = traderModel.Vat.Trim();

            var taxPeriod = _periodicF2ModelFactory.TaxPeriodByCategoryBookType(traderModel.CategoryBookTypeId, dialogModel.Period);
            var pageCredential = await _periodicF2ModelFactory.GetPageCredentialAsync(traderModel, dialogModel.Representative);
            var year = dialogModel.Date.Year;

            var format = "{0}?traderName={1}&vat={2}&pageKindTypeId={3}&f007={4}&year={5}&from={6}&to={7}&userName={8}&password={9}&connectionId={10}";
            var url = string.Format(format,
                "api/periodicF2/identityPayment",
                WebUtility.UrlEncode(traderName),
                traderVat,
                (int)pageCredential.PageCredentialType,
                dialogModel.F007,
                year,
                taxPeriod.First(),
                taxPeriod.Last(),
                WebUtility.UrlEncode(pageCredential.UserName),
                WebUtility.UrlEncode(pageCredential.Password),
                connectionId == "undefined" ? null : connectionId);

            var result = await _httpClient.SendAsync(HttpMethod.Post, url);
            if (result.Success)
            {
                var response = JsonConvert.DeserializeObject<DtoListResponse<PeriodicF2Result>>(result.Content);
                if (response.Success)
                {
                    custActivity.AddSuccess(response.Message);
                }
                else
                    custActivity.AddError(response.Error);
            }
            else
                custActivity.AddError(result.Error);

            //activity log
            await _customerActivityService.InsertActivityAsync(ActivityLogTypeType.PeriodicF2IdentityPayment, custActivity.ToString());

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> Calc(int id)
        {
            //try to get entity with the specified id
            var periodicF2 = await _periodicF2Service.GetPeriodicF2ByIdAsync(id);
            if (periodicF2 == null)
                return await AccessDenied();

            //prepare model
            var model = await _periodicF2ModelFactory.PreparePeriodicF2ModelAsync(null, periodicF2);
            _periodicF2ModelFactory.PrepareCalcPeriodicF2ModelAsync(model);

            return Json(new { model });
        }

    }
}
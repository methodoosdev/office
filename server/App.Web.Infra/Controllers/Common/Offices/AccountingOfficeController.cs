using App.Core.Infrastructure.Mapper;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Offices;
using App.Services.Localization;
using App.Services.Offices;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Offices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Offices
{
    public partial class AccountingOfficeController : BaseProtectController
    {
        private readonly IAccountingOfficeService _accountingOfficeService;
        private readonly ILocalizationService _localizationService;
        private readonly IAccountingOfficeModelFactory _accountingOfficeModelFactory;

        public AccountingOfficeController(
            IAccountingOfficeService accountingOfficeService,
            ILocalizationService localizationService,
            IAccountingOfficeModelFactory accountingOfficeModelFactory)
        {
            _accountingOfficeService = accountingOfficeService;
            _localizationService = localizationService;
            _accountingOfficeModelFactory = accountingOfficeModelFactory;
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var accountingOffice = await _accountingOfficeService.GetAccountingOfficeByIdAsync(id);
            if (accountingOffice == null)
                return await AccessDenied();

            //prepare model
            var model = await _accountingOfficeModelFactory.PrepareAccountingOfficeModelAsync(null, accountingOffice);

            //prepare form
            var formModel = await _accountingOfficeModelFactory.PrepareAccountingOfficeFormModelAsync(new AccountingOfficeFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] AccountingOfficeModel model)
        {
            //try to get entity with the specified id
            var accountingOffice = await _accountingOfficeService.GetAccountingOfficeByIdAsync(model.Id);
            if (accountingOffice == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    accountingOffice = model.ToEntity(accountingOffice);
                    await _accountingOfficeService.UpdateAccountingOfficeAsync(accountingOffice);

                    return Json(accountingOffice.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AccountingOffices.Errors.TryToEdit");
            }
        }
    }
}
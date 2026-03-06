using App.Core.Domain.Traders;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Traders;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Traders;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Traders
{
    public partial class AccountingWorkController : BaseProtectController
    {
        private readonly IAccountingWorkService _accountingWorkService;
        private readonly IAccountingWorkModelFactory _accountingWorkModelFactory;

        public AccountingWorkController(
            IAccountingWorkService accountingWorkService,
            IAccountingWorkModelFactory accountingWorkModelFactory)
        {
            _accountingWorkService = accountingWorkService;
            _accountingWorkModelFactory = accountingWorkModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _accountingWorkModelFactory.PrepareAccountingWorkSearchModelAsync(new AccountingWorkSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] AccountingWorkSearchModel searchModel)
        {
            //prepare model
            var model = await _accountingWorkModelFactory.PrepareAccountingWorkListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _accountingWorkModelFactory.PrepareAccountingWorkModelAsync(new AccountingWorkModel(), null);

            //prepare form
            var formModel = await _accountingWorkModelFactory.PrepareAccountingWorkFormModelAsync(new AccountingWorkFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] AccountingWorkModel model)
        {
            if (ModelState.IsValid)
            {
                var accountingWork = model.ToEntity<AccountingWork>();
                await _accountingWorkService.InsertAccountingWorkAsync(accountingWork);

                return Json(accountingWork.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var accountingWork = await _accountingWorkService.GetAccountingWorkByIdAsync(id);
            if (accountingWork == null)
                return await AccessDenied();

            //prepare model
            var model = await _accountingWorkModelFactory.PrepareAccountingWorkModelAsync(null, accountingWork);

            //prepare form
            var formModel = await _accountingWorkModelFactory.PrepareAccountingWorkFormModelAsync(new AccountingWorkFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] AccountingWorkModel model)
        {
            //try to get entity with the specified id
            var accountingWork = await _accountingWorkService.GetAccountingWorkByIdAsync(model.Id);
            if (accountingWork == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    accountingWork = model.ToEntity(accountingWork);
                    await _accountingWorkService.UpdateAccountingWorkAsync(accountingWork);

                    return Json(accountingWork.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AccountingWorks.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var accountingWork = await _accountingWorkService.GetAccountingWorkByIdAsync(id);
            if (accountingWork == null)
                return await AccessDenied();

            try
            {
                await _accountingWorkService.DeleteAccountingWorkAsync(accountingWork);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AccountingWorks.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _accountingWorkService.DeleteAccountingWorkAsync((await _accountingWorkService.GetAccountingWorksByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch (Exception exc)
            {
                return await BadRequestMessageAsync("App.Models.AccountingWorks.Errors.TryToDelete");
            }
        }
    }
}
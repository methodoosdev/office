using App.Core;
using App.Core.Domain.Accounting;
using App.Core.Domain.Messages;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Accounting;
using App.Models.Employees;
using App.Services.Accounting;
using App.Services.Hubs;
using App.Services.Logging;
using App.Services.Messages;
using App.Services.Offices;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Accounting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Accounting
{
    public partial class MyDataItemController : BaseProtectController
    {
        private readonly IHubContext<ChatHub> _hub;
        private readonly ITraderService _traderService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly IMyDataItemService _myDataItemService;
        private readonly IMyDataItemModelFactory _myDataItemModelFactory;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IPersistStateService _persistStateService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEmailSender _emailSender;
        private readonly IWorkContext _workContext;

        public MyDataItemController(
            IHubContext<ChatHub> hub,
            ITraderService traderService,
            ITraderConnectionService traderConnectionService,
            IMyDataItemService myDataItemService,
            IMyDataItemModelFactory myDataItemModelFactory,
            EmailAccountSettings emailAccountSettings,
            IEmailAccountService emailAccountService,
            IPersistStateService persistStateService,
            ICustomerActivityService customerActivityService,
            IEmailSender emailSender,
            IWorkContext workContext)
        {
            _hub = hub;
            _traderService = traderService;
            _traderConnectionService = traderConnectionService;
            _myDataItemService = myDataItemService;
            _myDataItemModelFactory = myDataItemModelFactory;
            _emailAccountSettings = emailAccountSettings;
            _emailAccountService = emailAccountService;
            _persistStateService = persistStateService;
            _customerActivityService = customerActivityService;
            _emailSender = emailSender;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _myDataItemModelFactory.PrepareMyDataItemSearchModelAsync(new MyDataItemSearchModel());

            //prepare model
            var infoModel = new MyDataItemInfoModel();
            var infoFormModel = await _myDataItemModelFactory.PrepareMyDataItemInfoFormModelAsync(new MyDataItemInfoFormModel());

            return Json(new { searchModel, infoModel, infoFormModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] MyDataItemSearchModel searchModel, int traderId, bool isIssuer, int docTypeId)
        {
            var trader = await _traderService.GetTraderByIdAsync(traderId);
            if (trader == null)
                return await AccessDenied();

            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(trader.Id);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            //prepare model
            var model = await _myDataItemModelFactory.PrepareMyDataItemListModelAsync(searchModel, traderId, isIssuer, docTypeId, connectionResult.Connection);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create(int traderId, bool isIssuer, int docTypeId)
        {
            var trader = await _traderService.GetTraderByIdAsync(traderId);
            if (trader == null)
                return await AccessDenied();

            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(trader.Id);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var infoModel = new MyDataItemInfoModel { TraderId = trader.Id, IsIssuer = isIssuer, DocTypeId = docTypeId };

            //prepare model
            var model = await _myDataItemModelFactory.PrepareMyDataItemModelAsync(new MyDataItemModel(), null, infoModel, connectionResult.TraderName, connectionResult.Vat);

            //prepare form
            var formModel = await _myDataItemModelFactory.PrepareMyDataItemFormModelAsync(new MyDataItemFormModel(), infoModel, connectionResult.Connection, connectionResult.CompanyId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] MyDataItemModel model)
        {
            if (ModelState.IsValid)
            {
                var myDataItem = model.ToEntity<MyDataItem>();
                await _myDataItemService.InsertMyDataItemAsync(myDataItem);

                return Json(myDataItem.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var myDataItem = await _myDataItemService.GetMyDataItemByIdAsync(id);
            if (myDataItem == null)
                return await AccessDenied();

            var trader = await _traderService.GetTraderByVatAsync(myDataItem.TraderVat);
            if (trader == null)
                return await AccessDenied();

            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(trader.Id);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var infoModel = new MyDataItemInfoModel { TraderId = trader.Id, IsIssuer = myDataItem.IsIssuer, DocTypeId = myDataItem.DocTypeId };

            //prepare model
            var model = await _myDataItemModelFactory.PrepareMyDataItemModelAsync(null, myDataItem, infoModel, connectionResult.TraderName, connectionResult.Vat);

            //prepare form
            var formModel = await _myDataItemModelFactory.PrepareMyDataItemFormModelAsync(new MyDataItemFormModel(), infoModel, connectionResult.Connection, connectionResult.CompanyId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] MyDataItemModel model)
        {
            //try to get entity with the specified id
            var myDataItem = await _myDataItemService.GetMyDataItemByIdAsync(model.Id);
            if (myDataItem == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    myDataItem = model.ToEntity(myDataItem);
                    await _myDataItemService.UpdateMyDataItemAsync(myDataItem);

                    return Json(myDataItem.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.MyDataItems.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var myDataItem = await _myDataItemService.GetMyDataItemByIdAsync(id);
            if (myDataItem == null)
                return await AccessDenied();

            try
            {
                await _myDataItemService.DeleteMyDataItemAsync(myDataItem);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.MyDataItems.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                {
                    var entities = await _myDataItemService.GetMyDataItemsByIdsAsync(selectedIds.ToArray());
                    await _myDataItemService.DeleteMyDataItemAsync(entities);
                }

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.MyDataItems.Errors.TryToDelete");
            }
        }

    }
}
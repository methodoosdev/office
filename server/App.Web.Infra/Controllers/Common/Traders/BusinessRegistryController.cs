using App.Services.Offices;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Traders;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Traders
{
    public partial class BusinessRegistryController : BaseProtectController
    {
        private readonly IAccountingOfficeService _accountingOfficeService;
        private readonly IBusinessRegistryModelFactory _businessRegistryModelFactory;

        public BusinessRegistryController(
            IAccountingOfficeService accountingOfficeService,
            IBusinessRegistryModelFactory businessRegistryModelFactory)
        {
            _accountingOfficeService = accountingOfficeService;
            _businessRegistryModelFactory = businessRegistryModelFactory;
        }

        public virtual async Task<IActionResult> Import(string afmCalledFor)
        {
            var office = await _accountingOfficeService.GetAccountingOfficeModelAsync();

            var model = _businessRegistryModelFactory.GetDocumentModel(office.AadeRegistryUsername, office.AadeRegistryPassword, afmCalledFor);

            return Json(model);
        }
    }
}
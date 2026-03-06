using App.Data;
using App.Models.VatExemption;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class VatExemptionSerialValidator : BaseNopValidator<VatExemptionSerialModel>
    {
        public VatExemptionSerialValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.SerialNo).GreaterThan(-1).WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionSerialModel.Validation.SerialNo"));
            RuleFor(x => x.SerialName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionSerialModel.Validation.SerialName"));
            RuleFor(x => x.Limit).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionSerialModel.Validation.Limit"));
            RuleFor(x => x.VatExemptionApprovalId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionSerialModel.Validation.VatExemptionApprovalId"));

        }
    }
}
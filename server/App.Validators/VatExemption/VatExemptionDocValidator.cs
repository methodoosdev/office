using App.Data;
using App.Models.VatExemption;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class VatExemptionDocValidator : BaseNopValidator<VatExemptionDocModel>
    {
        public VatExemptionDocValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.VatExemptionSerialId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionDocModel.Validation.VatExemptionSerialId"));
            RuleFor(x => x.CreatedDate).NotNull().WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionDocModel.Validation.CreatedDate"));
        }
    }
}
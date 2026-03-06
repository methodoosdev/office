using App.Data;
using App.Models.VatExemption;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class VatExemptionApprovalValidator : BaseNopValidator<VatExemptionApprovalModel>
    {
        public VatExemptionApprovalValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.ApprovalNumber).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionApprovalModel.Validation.ApprovalNumber"));
            RuleFor(x => x.ApprovalProtocol).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionApprovalModel.Validation.ApprovalProtocol"));
            RuleFor(x => x.Limit).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionApprovalModel.Validation.Limit"));
            RuleFor(x => x.Doy).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionApprovalModel.Validation.Doy"));
            RuleFor(x => x.CreatedDate).NotNull().WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionApprovalModel.Validation.CreatedDate"));
            RuleFor(x => x.StartingDate).NotNull().WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionApprovalModel.Validation.StartingDate"));
            RuleFor(x => x.ExpiryDate).NotNull().WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionApprovalModel.Validation.ExpiryDate"));
        }
    }
}
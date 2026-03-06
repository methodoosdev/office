using App.Data;
using App.Models.VatExemption;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class VatExemptionReportValidator : BaseNopValidator<VatExemptionReportModel>
    {
        public VatExemptionReportValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.VatExemptionApprovalId).GreaterThan(0).WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionReportModel.Validation.VatExemptionApprovalId"));
            RuleFor(x => x.Subject).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionReportModel.Validation.Subject"));
            RuleFor(x => x.CreatedDate).NotNull().WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionReportModel.Validation.CreatedDate"));
            RuleFor(x => x.Description).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.VatExemptionReportModel.Validation.Description"));

        }
    }
}
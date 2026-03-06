using App.Data;
using App.Models.Offices;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class ChamberValidator : BaseNopValidator<ChamberModel>
    {
        public ChamberValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.ChamberName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.ChamberModel.Validation.ChamberName"));
        }
    }
}
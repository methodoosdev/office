using App.Data;
using App.Models.Offices;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class PeriodicityItemValidator : BaseNopValidator<PeriodicityItemModel>
    {
        public PeriodicityItemValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Paragraph)
                .NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("App.Models.PeriodicityItemModel.Validation.Paragraph.NotEmpty"))
                .MaximumLength(40).WithMessageAwait(localizationService.GetResourceAsync("App.Models.PeriodicityItemModel.Validation.Paragraph.MaximumLength"));
        }
    }
}

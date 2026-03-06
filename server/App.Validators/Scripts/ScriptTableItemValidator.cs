using App.Data;
using App.Models.Scripts;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class ScriptTableItemValidator : BaseNopValidator<ScriptTableItemModel>
    {
        public ScriptTableItemValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.AccountingCode).NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("App.Models.ScriptTableItemModel.Validation.AccountingCode"));            
        }
    }
}
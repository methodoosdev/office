using App.Data;
using App.Models.Scripts;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class ScriptTableNameValidator : BaseNopValidator<ScriptTableNameModel>
    {
        public ScriptTableNameValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("App.Models.ScriptTableNameModel.Validation.Name"));            
        }
    }
}
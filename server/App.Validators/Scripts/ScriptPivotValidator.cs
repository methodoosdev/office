using App.Data;
using App.Models.Scripts;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class ScriptPivotValidator : BaseNopValidator<ScriptPivotModel>
    {
        public ScriptPivotValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.ScriptPivotName).NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("App.Models.ScriptModel.Validation.ScriptPivotName"));            
        }
    }
}
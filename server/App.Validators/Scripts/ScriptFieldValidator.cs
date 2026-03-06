using App.Data;
using App.Models.Scripts;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class ScriptFieldValidator : BaseNopValidator<ScriptFieldModel>
    {
        public ScriptFieldValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.FieldName).NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("App.Models.ScriptFieldModel.Validation.FieldName"));            
        }
    }
}
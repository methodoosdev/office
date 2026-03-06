using App.Data;
using App.Models.Scripts;
using App.Services.Localization;
using FluentValidation;

namespace App.Validators
{
    public partial class ScriptTableValidator : BaseNopValidator<ScriptTableModel>
    {
        public ScriptTableValidator(ILocalizationService localizationService, INopDataProvider dataProvider)
        {
            RuleFor(x => x.TableName).NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("App.Models.ScriptTableModel.Validation.TableName"));            
        }
    }
}
using App.Framework.Models;
using System.Collections.Generic;

namespace App.Models.Employees
{
    public partial record EducationSearchModel : BaseSearchModel
    {
        public EducationSearchModel() : base("displayOrder") { }
    }
    public partial record EducationListModel : BasePagedListModel<EducationModel>
    {
    }
    public partial record EducationModel : BaseNopEntityModel, ILocalizedModel<EducationLocalizedModel>
    {
        public EducationModel()
        {
            Locales = new List<EducationLocalizedModel>();
        }

        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public IList<EducationLocalizedModel> Locales { get; set; }
    }
    public partial record EducationFormModel : BaseNopModel
    {
    }
    public partial class EducationLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }
        public string Description { get; set; }
    }
}
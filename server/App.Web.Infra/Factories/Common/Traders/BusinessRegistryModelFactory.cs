using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Models.Traders;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Traders
{
    public partial interface IBusinessRegistryModelFactory
    {
        bool CheckIfProfessionTypeActive(string userName, string password, string afmCalledFor);
        BusinessRegistryModel GetDocumentModel(string userName, string password, string afmCalledFor);
        Task<List<TablePropertiesModel>> PrepareTablePropertiesModelAsync(BusinessRegistryModel model, BusinessRegistryModel docModel);
    }

    public partial class BusinessRegistryModelFactory : IBusinessRegistryModelFactory
    {
        private readonly IBusinessRegistryService _businessRegistryService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;

        public BusinessRegistryModelFactory(
            IBusinessRegistryService businessRegistryService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService)
        {
            _businessRegistryService = businessRegistryService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
        }

        protected async Task<string> ConvertToStringAsync<T>(T value)
        {
            var yes = await _localizationService.GetResourceAsync("App.Common.Yes");
            var no = await _localizationService.GetResourceAsync("App.Common.No");

            var result = "";
            switch (value)
            {
                case DateTime dt:
                    result = dt.ToString("s");
                    break;
                case int i:
                    result = Convert.ToString(i);
                    break;
                case bool b:
                    result = b == true ? yes : no;
                    break;
                case string s:
                    result = s;
                    break;
                default:
                    break;
            }
            return result;
        }

        public virtual async Task<List<TablePropertiesModel>> PrepareTablePropertiesModelAsync(
            BusinessRegistryModel model, BusinessRegistryModel docModel)
        {
            model.StartingDate = model.StartingDate.HasValue
                ? await _dateTimeHelper.ConvertToUserTimeAsync(model.StartingDate.Value, DateTimeKind.Utc) : null;
            model.ExpiryDate = model.ExpiryDate.HasValue
                ? await _dateTimeHelper.ConvertToUserTimeAsync(model.ExpiryDate.Value, DateTimeKind.Utc) : null;

            docModel.StartingDate = docModel.StartingDate.HasValue
                ? await _dateTimeHelper.ConvertToUserTimeAsync(docModel.StartingDate.Value, DateTimeKind.Utc) : null;
            docModel.ExpiryDate = docModel.ExpiryDate.HasValue
                ? await _dateTimeHelper.ConvertToUserTimeAsync(docModel.ExpiryDate.Value, DateTimeKind.Utc) : null;

            var modelProperties = model.GetType().GetProperties();
            var docModelProperties = docModel.GetType().GetProperties();
            var list = new List<TablePropertiesModel>();
            var idx = 1;

            foreach (var modelProperty in modelProperties)
            {
                var name = modelProperty.Name;

                var docModelProperty = docModelProperties.FirstOrDefault(p => p.Name == name);

                var modelPropValue = modelProperty.GetValue(model);
                var docModelPropValue = docModelProperty.GetValue(docModel);

                var modelPropertyValue = await ConvertToStringAsync(modelPropValue);
                var docModelPropertyValue = await ConvertToStringAsync(docModelPropValue);

                var isEquals = !modelPropertyValue.Equals(docModelPropertyValue);

                var propertyType = Nullable.GetUnderlyingType(docModelProperty.PropertyType);
                var registryType = propertyType == null ? docModelProperty.PropertyType.Name : propertyType.Name;

                var prop = new TablePropertiesModel
                {
                    Id = idx++,
                    FieldLabel = await _localizationService.GetResourceAsync("App.Models.BusinessRegistryModel.Fields." + name),
                    FieldName = name.ToCamelCase(),
                    FieldValue = modelPropertyValue,
                    RegistryValue = docModelPropertyValue,
                    RegistryType = registryType.ToCamelCase(),
                    IsEquals = isEquals
                };

                if (name == nameof(BusinessRegistryModel.ActivatedTypeId))
                {
                    prop.FieldValue = await _localizationService.GetLocalizedEnumAsync((ActivatedType)(int)modelPropValue);
                    prop.RegistryValue = await _localizationService.GetLocalizedEnumAsync((ActivatedType)(int)docModelPropValue);
                }

                if (name == nameof(BusinessRegistryModel.ProfessionTypeId))
                {
                    prop.FieldValue = await _localizationService.GetLocalizedEnumAsync((ProfessionType)(int)modelPropValue);
                    prop.RegistryValue = await _localizationService.GetLocalizedEnumAsync((ProfessionType)(int)docModelPropValue);
                }

                list.Add(prop);
            }

            return list;
        }

        public bool CheckIfProfessionTypeActive(string userName, string password, string afmCalledFor)
        {
            var dict = _businessRegistryService.GetDocument(userName.Trim(), password.Trim(), afmCalledFor.Trim());
            var type = dict.TryGetValue("firm_flag_descr", out string firm_flag_descr) ? GetProfessionType(firm_flag_descr) : ProfessionType.None;

            return type == ProfessionType.Active;
        }

        public BusinessRegistryModel GetDocumentModel(string userName, string password, string afmCalledFor)
        {
            var dict = _businessRegistryService.GetDocument(userName, password, afmCalledFor);
            var docModel = new BusinessRegistryModel();
            dict.TryGetValue("doy", out string doy);

            if (dict.TryGetValue("afm", out string afm)) docModel.Vat = afm;
            if (dict.TryGetValue("doy_descr", out string doy_desc)) docModel.Doy = doy + " - " + doy_desc;
            if (dict.TryGetValue("deactivation_flag", out string deactivation_flag)) docModel.ActivatedTypeId = int.TryParse(deactivation_flag, out var outValue) ? 0 : outValue == 1 ? 0 : 1;
            if (dict.TryGetValue("firm_flag_descr", out string firm_flag_descr)) docModel.ProfessionTypeId = (int)GetProfessionType(firm_flag_descr);
            if (dict.TryGetValue("onomasia", out string onomasia)) docModel.LastName = onomasia;
            if (dict.TryGetValue("commer_title", out string commer_title)) docModel.TradeName = commer_title;
            if (dict.TryGetValue("postal_address", out string postal_address)) docModel.JobAddress = postal_address;
            if (dict.TryGetValue("postal_address_no", out string postal_address_no)) docModel.JobStreetNumber = postal_address_no;
            if (dict.TryGetValue("postal_zip_code", out string postal_zip_code)) docModel.JobPostcode = postal_zip_code;
            if (dict.TryGetValue("postal_area_description", out string postal_area_description)) docModel.JobCity = postal_area_description;
            if (dict.TryGetValue("regist_date", out string regist_date)) docModel.StartingDate = GetCorrectDate(regist_date);
            if (dict.TryGetValue("stop_date", out string stop_date)) docModel.ExpiryDate = GetCorrectDate(stop_date);
            if (dict.TryGetValue("firm_act_descr", out string firm_act_descr)) docModel.ProfessionalActivity = firm_act_descr;

            return docModel;
        }
        private ProfessionType GetProfessionType(string profession)
        {
            var result = ProfessionType.None;
            switch (profession)
            {
                case "ΕΠΙΤΗΔΕΥΜΑΤΙΑΣ":
                    result = ProfessionType.Active;
                    break;
                case "ΜΗ ΕΠΙΤΗΔΕΥΜΑΤΙΑΣ":
                    break;
                case "ΠΡΩΗΝ ΕΠΙΤΗΔΕΥΜΑΤΙΑΣ":
                    result = ProfessionType.Deactive;
                    break;
                default:
                    break;
            }

            return result;
        }
        private DateTime? GetCorrectDate(string date)
        {
            if (DateTime.TryParseExact(date, "yyyy-MM-dd", null, DateTimeStyles.None, out var _date) ||
                DateTime.TryParseExact(date, "yyyy-M-d", null, DateTimeStyles.None, out _date))
                return _date;

            return null;
        }
    }
}

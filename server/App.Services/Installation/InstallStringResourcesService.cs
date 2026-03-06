using App.Core.Domain.Localization;
using App.Core.Infrastructure;
using App.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Installation
{
    public partial interface IInstallStringResourcesService
    {
        Task ImportResourcesFromJsonAsync();
        Task ImportResourcesFromLanguageAsync(Language language, HashSet<(string name, string value)> flattenList);
    }
    public partial class InstallStringResourcesService : IInstallStringResourcesService
    {
        private const string LocalizationJsonPath = "~/App_Assets/I18n/";

        private readonly INopFileProvider _fileProvider;
        private readonly IRepository<LocaleStringResource> _lsrRepository;
        private readonly IRepository<Language> _languageRepository;

        public InstallStringResourcesService(INopFileProvider fileProvider,
            IRepository<LocaleStringResource> lsrRepository,
            IRepository<Language> languageRepository)
        {
            _fileProvider = fileProvider;
            _lsrRepository = lsrRepository;
            _languageRepository = languageRepository;
        }

        public HashSet<(string name, string value)> LoadLocaleResourcesFromJson(string json)
        {
            JObject jobject = JObject.Parse(json);

            return jobject.Descendants()
                .Where(j => j.Children().Count() == 0)
                .Aggregate(new HashSet<(string name, string value)>(), (props, jtoken) =>
                {
                    props.Add((jtoken.Path.ToLowerInvariant(), jtoken.ToString()));
                    return props;
                });
        }

        public virtual async Task ImportResourcesFromJsonAsync()
        {
            //save resources
            var directoryPath = _fileProvider.MapPath(LocalizationJsonPath);
            foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, "*.json"))
            {
                var languageCulture = Path.GetFileName(filePath).Split('.').FirstOrDefault();
                var language = _languageRepository.Table.Single(l => l.LanguageCulture == languageCulture);

                var json = _fileProvider.ReadAllText(filePath, Encoding.UTF8);
                var flattenList = LoadLocaleResourcesFromJson(json);

                await ImportResourcesFromLanguageAsync(language, flattenList);
            }
        }

        public virtual async Task ImportResourcesFromLanguageAsync(Language language, HashSet<(string name, string value)> flattenList)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            var lsNamesList = new Dictionary<string, LocaleStringResource>();

            foreach (var localeStringResource in _lsrRepository.Table.Where(lsr => lsr.LanguageId == language.Id)
                .OrderBy(lsr => lsr.Id))
                lsNamesList[localeStringResource.ResourceName.ToLowerInvariant()] = localeStringResource;

            var lrsToUpdateList = new List<LocaleStringResource>();
            var lrsToInsertList = new Dictionary<string, LocaleStringResource>();

            foreach (var (name, value) in flattenList)
            {
                if (lsNamesList.ContainsKey(name))
                {
                    var lsr = lsNamesList[name];
                    lsr.ResourceValue = value;
                    lrsToUpdateList.Add(lsr);
                }
                else
                {
                    var lsr = new LocaleStringResource { LanguageId = language.Id, ResourceName = name, ResourceValue = value };
                    lrsToInsertList[name] = lsr;
                }
            }

            await _lsrRepository.UpdateAsync(lrsToUpdateList, false);
            await _lsrRepository.InsertAsync(lrsToInsertList.Values.ToList(), false);
        }
    }
}

using App.Core.Infrastructure;
using App.Services.Localization;
using System.Collections.Generic;

namespace App.Framework.Components
{
    public class ColumnConfig
    {
        private const string _columnFormat = "App.Models.{0}.Columns.{1}";
        private const string _titleTooltipFormat = "App.Models.{0}.Hints.{1}";
        public ColumnConfig(int order, string field, string title, bool hidden, string format, string filterType,
            string media, string fieldType, string theme, int? width,
            Dictionary<string, string> style, Dictionary<string, string> headerStyle, string disabledField,
            string backgroundField, string link, string target, string titleTooltip, string _class, string headerClass,
            bool filterable, bool sticky)
        {
            Order = order;
            Field = field;
            Title = title;
            TitleTooltip = titleTooltip;
            Format = format;
            FilterType = filterType;
            Media = media;
            Theme = theme;
            Hidden = hidden;
            FieldType = fieldType;
            Style = style == null ? new Dictionary<string, string>() : style;
            HeaderStyle = headerStyle == null ? new Dictionary<string, string>() : headerStyle;
            DisabledField = disabledField;
            BackgroundField = backgroundField;
            Link = link;
            Target = target;
            Class = _class;
            HeaderClass = headerClass;
            Filterable = filterable;
            Sticky = sticky;

            if (width.HasValue)
            {
                Width = width.Value;
            }
        }

        public int Order { get; set; }
        public string Field { get; set; }
        public string FieldType { get; set; }
        public string Title { get; set; }
        public string TitleTooltip { get; set; }
        public string Format { get; set; }
        public string FilterType { get; set; } //: "text" | "numeric" | "boolean" | "date"
        public bool Hidden { get; set; }
        public string Media { get; set; }
        public string Theme { get; set; }
        public int? Width { get; set; }
        public Dictionary<string, string> Style { get; set; }
        public Dictionary<string, string> HeaderStyle { get; set; }
        public string DisabledField { get; set; }
        public string BackgroundField { get; set; }
        public string Link { get; set; }
        public string Target { get; set; }
        public string Class { get; set; }
        public string HeaderClass { get; set; }
        public bool Filterable { get; set; }
        public bool Sticky { get; set; }

        public static ColumnConfig Create<T>(int order, string property, string fieldType = null, bool hidden = false,
            string format = null, string filterType = "text", string theme = null, string media = null, int? width = null,
            Dictionary<string, string> style = null, Dictionary<string, string> headerStyle = null, string disabledField = null,
            string backgroundField = null, string link = "", string target = "_self", string _class = null, string headerClass = null,
            bool filterable = false, bool sticky = false)
        {
            var _localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var entityType = typeof(T).Name;
            var resourceName = string.Format(_columnFormat, entityType, property);
            var title = _localizationService.GetResourceAsync(resourceName).Result;

            var titleTooltipName = string.Format(_titleTooltipFormat, entityType, property);
            var titleTooltip = _localizationService.GetResourceAsync(titleTooltipName).Result;

            var propertyName = property.ToCamelCase();

            return new ColumnConfig(order, propertyName, title, hidden, format, filterType, media, fieldType,
                theme, width, style, headerStyle, disabledField, backgroundField, link, target, titleTooltip, _class, headerClass, filterable, sticky);
        }

        public static ColumnConfig CreateButton<T>(int order, string fieldType, string field, string theme, string title,
            Dictionary<string, string> style = null, Dictionary<string, string> headerStyle = null, string disabledField = null,
            string backgroundField = null, string link = "", string target = "_self", string titleTooltip = "", string _class = null, string headerClass = null,
            bool hidden = false)
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };

            style = style ?? centerAlign;
            headerStyle = headerStyle ?? centerAlign;

            return new ColumnConfig(order, field, title, hidden, null, null, null, fieldType,
                theme, null, style, headerStyle, disabledField, backgroundField, link, target, titleTooltip, _class, headerClass, false, false);
        }
    }
}
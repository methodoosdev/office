using App.Core;
using App.Core.Infrastructure;
using App.Services.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace App.Framework.Components
{
    public class FieldConfig
    {
        protected const string _fieldFormat = "App.Models.{0}.Fields.{1}";
        protected const string _placeholderFormat = "App.Models.{0}.Placeholders.{1}";
        protected const string _hintsFormat = "App.Models.{0}.Hints.{1}";

        protected static ILocalizationService _localizationService => EngineContext.Current.Resolve<ILocalizationService>();
        protected static IWorkContext _workContext => EngineContext.Current.Resolve<IWorkContext>();

        public static Dictionary<string, object> Create<T>(string key, string type,
            object defaultValue = null, string template = null, bool? hide = null, int? debounce = null,
            string hideExpression = null, string disableExpression = null, string className = "col-12",
            string fieldGroupClassName = null, bool? focus = null, bool? hideRequiredMarker = null,
            bool? hideLabel = null, string orientation = "horizontal", bool? markAsRequired = null,
            bool? expanded = null, int? listHeight = null, bool? groupable = null,
            IEnumerable columns = null, string groupProp = null, string labelProp = null,
            string valueProp = null, string disabledProp = null, string inputType = null,
            string label = null, string placeholder = null, bool? disabled = null,
            IEnumerable options = null, IEnumerable sourceOptions = null, int? rows = null, int? cols = null, 
            string description = null, bool? hidden = null, int? max = null, int? min = null, int? maxLength = null,
            int? minLength = null, string pattern = null, bool? required = null, int? tabindex = null,
            bool? _readonly = null, Dictionary<string, object> attributes = null, int? step = null,
            string checkAll = null, string itemsSelected = null, int? decimals = null,
            string format = null, string size = null, string rounded = null, bool? spinners = null,
            int? value = null, DateTime? minDate = null, DateTime? maxDate = null, bool? nullable = null,
            string themeColor = null, string style = null, string onLabel = null, string offLabel = null)
        {
            var properties = new Dictionary<string, object>
            {
                ["type"] = type
            };

            if (key != null)
                properties.Add("key", key.ToCamelCase());

            if (defaultValue != null)
                properties.Add("defaultValue", defaultValue);

            if (!string.IsNullOrEmpty(template))
                properties.Add("template", template);

            if (hide.HasValue)
                properties.Add("hide", hide.Value);

            if (debounce.HasValue)
            {
                var _default = new Dictionary<string, object> { ["default"] = debounce.Value };
                var _debounce = new Dictionary<string, object> { ["debounce"] = _default };
                var modelOptions = new Dictionary<string, object> { ["modelOptions"] = _debounce };
                properties.Add("modelOptions", modelOptions);
            }

            if (!string.IsNullOrEmpty(hideExpression))
            {
                var _hide = new Dictionary<string, object> { ["hide"] = hideExpression };
                properties.Add("expressions", _hide);
            }

            if (!string.IsNullOrEmpty(disableExpression) && string.IsNullOrEmpty(hideExpression))
            {
                var _disabled = new Dictionary<string, object> { ["props.disabled"] = disableExpression };
                properties.Add("expressions", _disabled);
            }

            if (!string.IsNullOrEmpty(className))
                properties.Add("className", className);

            if (!string.IsNullOrEmpty(fieldGroupClassName))
                properties.Add("fieldGroupClassName", fieldGroupClassName);

            if (focus.HasValue)
                properties.Add("focus", focus.Value);

            var props = new Dictionary<string, object>();

            if (hideRequiredMarker.HasValue)
                props.Add("hideRequiredMarker", hideRequiredMarker.Value);

            if (hideLabel.HasValue)
                props.Add("hideLabel", hideLabel.Value);

            if (!string.IsNullOrEmpty(orientation)) // 'vertical' | 'horizontal'
                props.Add("orientation", orientation);

            if (markAsRequired.HasValue)
                props.Add("markAsRequired", markAsRequired.Value);

            if (expanded.HasValue)
                props.Add("expanded", expanded.Value);

            if (listHeight.HasValue)
                props.Add("listHeight", listHeight.Value);

            if (columns != null)
                props.Add("columns", columns);

            if (!string.IsNullOrEmpty(groupProp))
                props.Add("groupProp", groupProp);

            if (!string.IsNullOrEmpty(labelProp))
                props.Add("labelProp", labelProp);

            if (!string.IsNullOrEmpty(valueProp))
                props.Add("valueProp", valueProp);

            if (!string.IsNullOrEmpty(disabledProp))
                props.Add("disabledProp", disabledProp);

            if (!string.IsNullOrEmpty(inputType))
                props.Add("type", inputType);

            if (!string.IsNullOrEmpty(label))
                props.Add("label", label);
            else
            {
                var entityType = typeof(T).Name;
                var resourceName = string.Format(_fieldFormat, entityType, key ?? "");
                var _label = _localizationService.GetResourceAsync(resourceName).Result;

                props.Add("label", _label);
            }

            if (!string.IsNullOrEmpty(placeholder))
                props.Add("placeholder", placeholder);
            else
            {
                var entityType = typeof(T).Name;
                var placeholderName = string.Format(_hintsFormat, entityType, key ?? "");
                var _placeholder = _localizationService.GetResourceAsync(placeholderName, false).Result;

                props.Add("placeholder", _placeholder);
            }

            if (!string.IsNullOrEmpty(description))
                props.Add("description", description);

            if (!string.IsNullOrEmpty(pattern))
                props.Add("pattern", pattern);

            if (disabled.HasValue)
                props.Add("disabled", disabled.Value);

            if (groupable.HasValue)
                props.Add("groupable", groupable.Value);

            if (hidden.HasValue)
                props.Add("hidden", hidden.Value);

            if (required.HasValue)
                props.Add("required", required.Value);

            if (_readonly.HasValue)
                props.Add("readonly", _readonly.Value);

            if (tabindex.HasValue)
                props.Add("tabindex", tabindex.Value);

            if (max.HasValue)
                props.Add("max", max.Value);

            if (min.HasValue)
                props.Add("min", min.Value);

            if (maxLength.HasValue)
                props.Add("maxLength", maxLength.Value);

            if (minLength.HasValue)
                props.Add("minLength", minLength.Value);

            if (rows.HasValue)
                props.Add("rows", rows.Value);

            if (cols.HasValue)
                props.Add("cols", cols.Value);

            if (step.HasValue)
                props.Add("step", step.Value);

            if (options != null)
                props.Add("options", options);

            if (sourceOptions != null)
                props.Add("sourceOptions", sourceOptions);

            if (attributes != null)
                props.Add("attributes", attributes);

            if (!string.IsNullOrEmpty(checkAll))
                props.Add("checkAll", checkAll);

            if (!string.IsNullOrEmpty(itemsSelected))
                props.Add("itemsSelected", itemsSelected);

            if (decimals.HasValue)
                props.Add("decimals", decimals.Value);

            if (!string.IsNullOrEmpty(format))
                props.Add("format", format);

            if (!string.IsNullOrEmpty(size))
                props.Add("size", size);

            if (!string.IsNullOrEmpty(rounded))
                props.Add("rounded", rounded);

            if (spinners.HasValue)
                props.Add("spinners", spinners.Value);

            if (value.HasValue)
                props.Add("value", value.Value);

            if (minDate.HasValue)
                props.Add("minDate", minDate.Value);

            if (maxDate.HasValue)
                props.Add("maxDate", maxDate.Value);

            if (nullable.HasValue)
                props.Add("nullable", nullable.Value);

            if (!string.IsNullOrEmpty(themeColor))
                props.Add("themeColor", themeColor);

            var lang = _workContext.GetWorkingLanguageAsync().Result?.LanguageCulture;
            if (type == FieldType.Date)
            {
                props.Add("format", lang == "en-US" ? "MM/dd/yy" : "dd/MM/yy");
                props.Add("activeView", "month");
                props.Add("bottomView", "month");
            }
            if (type == FieldType.DateTime)
            {
                props.Add("format", lang == "en-US" ? "MM/dd/yy HH:mm:ss" : "dd/MM/yy HH:mm:ss");
                props.Add("activeView", "month");
                props.Add("bottomView", "month");
            }
            if (type == FieldType.MonthDate)
            {
                props.Add("format", "MMMM yyyy");
                props.Add("activeView", "year");
                props.Add("bottomView", "year");
            }

            if (type == FieldType.YearDate)
            {
                props.Add("format", "yyyy");
                props.Add("activeView", "decade");
                props.Add("bottomView", "decade");
            }

            if (!string.IsNullOrEmpty(style))
                props.Add("style", style);

            if (!string.IsNullOrEmpty(onLabel))
                props.Add("onLabel", onLabel);

            if (!string.IsNullOrEmpty(offLabel))
                props.Add("offLabel", offLabel);

            properties.Add("props", props);

            return properties;
        }

        public static Dictionary<string, object> CreateDivider(string label = null, string className = "col-12")
        {
            return new Dictionary<string, object>
            {
                ["template"] = label == null ? "<hr />" : $"<div class='k-expander-title'>{label}</div><hr />",
                ["className"] = className
            };
        }

        public static IList<Dictionary<string, object>> CreateFields(IList<Dictionary<string, object>> fieldGroup, string className = "col-12 md:col-6")
        {
            return new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    ["fieldGroupClassName"] = "grid",
                    ["fieldGroup"] = new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            ["fieldGroupClassName"] = "grid",
                            ["className"] = className,
                            ["fieldGroup"] = fieldGroup
                        }
                    }
                }
            };
        }

        public static IList<Dictionary<string, object>> CreateFields(string[] classes, params IList<Dictionary<string, object>>[] fieldGroups)
        {
            var classNames = classes.ToArray();
            var index = 0;
            var fieldGroup = new List<Dictionary<string, object>>();
            foreach (var group in fieldGroups.ToArray())
            {
                fieldGroup.Add(
                    new Dictionary<string, object>
                    {
                        ["fieldGroupClassName"] = "grid",
                        ["className"] = classNames[index++],
                        ["fieldGroup"] = group
                    });
            }

            return new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    ["fieldGroupClassName"] = "grid",
                    ["className"] = "col-12",
                    ["fieldGroup"] = fieldGroup
                }
            };
        }

        public static Dictionary<string, object> CreatePanel(string label, bool expanded, string className,
            params IList<Dictionary<string, object>>[] fieldGroups)
        {
            var fieldGroup = new List<Dictionary<string, object>>();
            foreach (var group in fieldGroups.ToArray())
            {
                fieldGroup.Add(
                    new Dictionary<string, object>
                    {
                        ["fieldGroupClassName"] = "grid",
                        ["className"] = className,
                        ["fieldGroup"] = group
                    });
            }

            return new Dictionary<string, object>
            {
                ["fieldGroupClassName"] = "grid",
                ["wrappers"] = new List<string> { "expansion-panel" },
                ["props"] = new Dictionary<string, object>
                {
                    ["expanded"] = expanded,
                    ["label"] = label
                },
                ["fieldGroup"] = fieldGroup
            };
        }
        public static Dictionary<string, object> CreateSection(string label, string[] classses,
            params List<Dictionary<string, object>>[] fieldGroups)
        {
            var classNames = classses.ToArray();
            var index = 0;
            var fieldGroup = new List<Dictionary<string, object>>();
            foreach (var group in fieldGroups.ToArray())
            {
                fieldGroup.Add(
                    new Dictionary<string, object>
                    {
                        ["fieldGroupClassName"] = "grid",
                        ["className"] = classNames[index++],
                        ["fieldGroup"] = group
                    });
            }

            return new Dictionary<string, object>
            {
                ["fieldGroupClassName"] = "grid",
                ["wrappers"] = new List<string> { "simple-section" },
                ["props"] = new Dictionary<string, object>
                {
                    ["label"] = label
                },
                ["fieldGroup"] = fieldGroup
            };
        }

    }
}
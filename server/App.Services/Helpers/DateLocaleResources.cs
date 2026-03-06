using App.Core;
using App.Services.Localization;
using System.Collections.Generic;

namespace App.Services.Helpers
{
    public static partial class DateLocaleResources
    {
        public static Dictionary<int, string> LocaleMonthResourceDict => new()
        {
            [1] = "App.Locale.Month.January",
            [2] = "App.Locale.Month.February",
            [3] = "App.Locale.Month.March",
            [4] = "App.Locale.Month.April",
            [5] = "App.Locale.Month.May",
            [6] = "App.Locale.Month.June",
            [7] = "App.Locale.Month.July",
            [8] = "App.Locale.Month.August",
            [9] = "App.Locale.Month.September",
            [10] = "App.Locale.Month.October",
            [11] = "App.Locale.Month.November",
            [12] = "App.Locale.Month.December"
        };
        public static Dictionary<int, string> LocaleSortMonthResourceDict => new()
        {
            [1] = "App.Locale.SortMonth.January",
            [2] = "App.Locale.SortMonth.February",
            [3] = "App.Locale.SortMonth.March",
            [4] = "App.Locale.SortMonth.April",
            [5] = "App.Locale.SortMonth.May",
            [6] = "App.Locale.SortMonth.June",
            [7] = "App.Locale.SortMonth.July",
            [8] = "App.Locale.SortMonth.August",
            [9] = "App.Locale.SortMonth.September",
            [10] = "App.Locale.SortMonth.October",
            [11] = "App.Locale.SortMonth.November",
            [12] = "App.Locale.SortMonth.December"
        };
        public static Dictionary<int, string> LocaleDayResourceDict => new()
        {
            [1] = "App.Locale.Day.Monday",
            [2] = "App.Locale.Day.Tuesday",
            [3] = "App.Locale.Day.Wednesday",
            [4] = "App.Locale.Day.Thursday",
            [5] = "App.Locale.Day.Friday",
            [6] = "App.Locale.Day.Saturday",
            [7] = "App.Locale.Day.Sunday"
        };
        public static Dictionary<int, string> LocaleSortDayResourceDict => new()
        {
            [1] = "App.Locale.SortDay.Monday",
            [2] = "App.Locale.SortDay.Tuesday",
            [3] = "App.Locale.SortDay.Wednesday",
            [4] = "App.Locale.SortDay.Thursday",
            [5] = "App.Locale.SortDay.Friday",
            [6] = "App.Locale.SortDay.Saturday",
            [7] = "App.Locale.SortDay.Sunday"
        };

        public static Dictionary<int, string> LocalePeriodResourceDict => new()
        {
            [1] = "App.Locale.Period.First",
            [2] = "App.Locale.Period.Second",
            [3] = "App.Locale.Period.Third",
            [4] = "App.Locale.Period.Forth"
        };

        public static string GetLocaleMonth(int month)
        {
            if (LocaleMonthResourceDict.TryGetValue(month, out string resource)) return resource;

            return "App.Errors.WrongValue";
        }

        public static string GetLocalePeriod(int period)
        {
            if (LocalePeriodResourceDict.TryGetValue(period, out string resource)) return resource;

            return "App.Errors.WrongValue";
        }
        public static IList<SelectionItemList> GetLocalePeriods(ILocalizationService localizationService)
        {
            var list = new List<SelectionItemList>();
            foreach (var item in LocaleMonthResourceDict) 
            {
                var description = localizationService.GetResourceAsync(item.Value).Result;
                list.Add(new SelectionItemList(item.Key, description));
            }

            return list;
        }
    }
}
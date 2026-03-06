using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;

namespace App.Services.Scripts
{
    public class DynamicModel
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public decimal Value { get; set; }
        public bool Printed { get; set; }
    }
    public class GridColumn
    {
        public string Field { get; set; }
        public string Title { get; set; }
        public bool Hidden { get; set; }
        public int? Width { get; set; }
        public int? MinResizableWidth { get; set; }
        public string Filter { get; set; }   // "text", "numeric"
        public string Format { get; set; }   // "#,##0.00"
        public string TextAlign { get; set; } // "right", "left", "center"
        public string HeaderAlign { get; set; }
    }

    public class GridResponse<T>
    {
        public string Title { get; set; }
        public List<T> Data { get; set; }
        public List<GridColumn> Columns { get; set; }
    }

    //public class ScriptDynamicModelResponse
    //{
    //    public string Title { get; set; }
    //    public List<DynamicModel> Data { get; set; }
    //    public Dictionary<string, string> Columns { get; set; }
    //}

    public class PivotDynamicModel
    {
        public string FieldName { get; set; }
        public string TitleName { get; set; }
        public decimal Value { get; set; }
        public string Month { get; set; }
    }
    public class PivotResponse
    {
        public string Title { get; set; }
        public List<ExpandoObject> Data { get; set; }
        public List<GridColumn> Columns { get; set; }
        public List<Dictionary<string, string>> Aggregates { get; set; }
    }
    //public class ScriptPivotResponse
    //{
    //    public string Title { get; set; }
    //    public List<ExpandoObject> Data { get; set; }
    //    public Dictionary<string, string> Columns { get; set; }  // English -> Greek
    //    public List<string> AggregateColumns { get; set; }       // Which numeric columns to sum
    //}

    public static class ScriptPivotHelper
    {
        public static List<ExpandoObject> PivotForGrid(List<PivotDynamicModel> list)
        {
            var months = list.Select(x => x.Month).Distinct().ToList();
            var fieldNames = list.Select(x => x.FieldName).Distinct().ToList();

            var result = new List<ExpandoObject>();

            foreach (var month in months)
            {
                dynamic row = new ExpandoObject();
                var dict = (IDictionary<string, object>)row;

                dict["period"] = month;

                foreach (var fieldName in fieldNames)
                {
                    //string eng = TransliterateGreekToLatin(greekName);
                    var value = list
                        .Where(x => x.Month == month && x.FieldName == fieldName)
                        .Select(x => x.Value)
                        .FirstOrDefault();

                    dict[fieldName] = value == 0 ? null : value;
                }

                result.Add(row);
            }

            return result;
        }
        public static string MakeValidPropertyName(string input, HashSet<string> existing)
        {
            // Transliterate Greek → Latin
            string name = TransliterateGreekToLatin(input);

            // Remove invalid chars (keep letters, digits, underscore)
            name = Regex.Replace(name, @"[^A-Za-z0-9_]", "");

            // Must start with letter or underscore
            if (!Regex.IsMatch(name, @"^[A-Za-z_]"))
                name = "_" + name;

            // Ensure uniqueness
            string baseName = name;
            int counter = 1;
            while (existing.Contains(name))
            {
                name = $"{baseName}_{counter++}";
            }

            existing.Add(name);
            return name;
        }

        public static string TransliterateGreekToLatin(string greek)
        {
            Dictionary<char, string> map = new()
            {
                ['Α'] = "A",
                ['Β'] = "V",
                ['Γ'] = "G",
                ['Δ'] = "D",
                ['Ε'] = "E",
                ['Ζ'] = "Z",
                ['Η'] = "I",
                ['Θ'] = "Th",
                ['Ι'] = "I",
                ['Κ'] = "K",
                ['Λ'] = "L",
                ['Μ'] = "M",
                ['Ν'] = "N",
                ['Ξ'] = "X",
                ['Ο'] = "O",
                ['Π'] = "P",
                ['Ρ'] = "R",
                ['Σ'] = "S",
                ['Τ'] = "T",
                ['Υ'] = "Y",
                ['Φ'] = "F",
                ['Χ'] = "Ch",
                ['Ψ'] = "Ps",
                ['Ω'] = "O",
                ['α'] = "a",
                ['β'] = "v",
                ['γ'] = "g",
                ['δ'] = "d",
                ['ε'] = "e",
                ['ζ'] = "z",
                ['η'] = "i",
                ['θ'] = "th",
                ['ι'] = "i",
                ['κ'] = "k",
                ['λ'] = "l",
                ['μ'] = "m",
                ['ν'] = "n",
                ['ξ'] = "x",
                ['ο'] = "o",
                ['π'] = "p",
                ['ρ'] = "r",
                ['σ'] = "s",
                ['ς'] = "s",
                ['τ'] = "t",
                ['υ'] = "y",
                ['φ'] = "f",
                ['χ'] = "ch",
                ['ψ'] = "ps",
                ['ω'] = "o",
                ['ά'] = "a",
                ['έ'] = "e",
                ['ί'] = "i",
                ['ό'] = "o",
                ['ύ'] = "y",
                ['ή'] = "i",
                ['ώ'] = "o"
            };
            return string.Concat(greek.Select(c => map.ContainsKey(c) ? map[c] : c.ToString()));
        }
    }

}

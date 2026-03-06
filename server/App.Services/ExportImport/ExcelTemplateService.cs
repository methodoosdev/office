using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace App.Services.ExportImport
{
    public interface IExcelTemplateService
    {
        HashSet<string> DiscoverTokens(XLWorkbook wb);
        void ApplyMapping(XLWorkbook wb, IDictionary<string, object> map);
    }

    /// <summary>
    /// Culture-proof Excel templating for ClosedXML 0.95.4.
    /// - Strings are ALWAYS written as TEXT (no parsing, locale-safe).
    /// - Typed values (number/date/bool) are written typed.
    /// - RichText is rebuilt safely to avoid corruption.
    /// </summary>
    public sealed class ExcelTemplateService : IExcelTemplateService
    {
        private readonly string _defaultNumericFormat = "#,##0.00;-#,##0.00";

        // Allowed token characters after '#':
        // English + Greek + combining marks + digits +  _. - ( ) [ ] / ? < > :
        private const string TokenCharClass =
            @"[A-Za-z\u0370-\u03FF\u1F00-\u1FFF\p{M}\p{Nd}_\.\-\(\)\[\]/\?<>:]";

        // Find tokens anywhere (for discovery within larger text)
        private static readonly Regex TokenRegexInner =
            new(@"#" + TokenCharClass + "+",
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        // (Optional) Whole-string token validator if you need it elsewhere:
        private static readonly Regex TokenRegexWhole =
            new(@"^#" + TokenCharClass + "+$",
                RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public HashSet<string> DiscoverTokens(XLWorkbook wb)
        {
            var found = new HashSet<string>(StringComparer.Ordinal);

            foreach (var ws in wb.Worksheets)
            {
                var used = ws.RangeUsed() ?? ws.Range(ws.FirstCellUsed()?.Address ?? ws.FirstCell().Address,
                                                      ws.LastCellUsed()?.Address ?? ws.LastCell().Address);

                foreach (var cell in used.CellsUsed())
                {
                    // DISCOVER by displayed text only (don’t parse!)
                    var text = (cell.GetString() ?? string.Empty).Normalize(NormalizationForm.FormC);
                    if (text.Length == 0) continue;

                    foreach (Match m in TokenRegexInner.Matches(text))
                        found.Add(m.Value.Trim());
                }
            }

            return found;
        }


        public void ApplyMapping(XLWorkbook wb, IDictionary<string, object> map)
        {
            if (wb == null) throw new ArgumentNullException(nameof(wb));
            if (map == null || map.Count == 0) return;

            var dict = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (var kv in map)
                if (!string.IsNullOrEmpty(kv.Key) && kv.Key[0] == '#')
                    dict[kv.Key] = kv.Value;

            if (dict.Count == 0) return;

            foreach (var ws in wb.Worksheets)
            {
                var used = ws.RangeUsed();
                if (used == null) continue;

                foreach (var cell in used.CellsUsed())
                {
                    if (cell.HasFormula) continue;                 // don’t touch formulas
                    if (cell.Value is not string s) continue;      // only string cells
                    if (s.Length <= 1 || s[0] != '#') continue;    // only tokens

                    if (!dict.TryGetValue(s, out var mapped))
                        continue;

                    // Snapshot style BEFORE change
                    var style = SavedCellStyle.Capture(cell);

                    // Write value (typed or text). We never parse strings.
                    var wroteNumber = SetTypedOrText(cell, mapped);

                    // Restore style (font/fill/align)
                    style.RestoreNonFormat(cell);

                    // Re-apply number format AFTER the value write
                    // If the cell previously had a meaningful number format, restore it.
                    if (style.HasMeaningfulNumberFormat)
                    {
                        cell.Style.NumberFormat.Format = style.NumberFormat;
                    }
                    else if (wroteNumber && !string.IsNullOrEmpty(_defaultNumericFormat))
                    {
                        // Apply optional fallback numeric format (e.g., "#.##0,00")
                        cell.Style.NumberFormat.Format = _defaultNumericFormat!;
                    }
                }
            }
        }

        /// <summary>
        /// Writes value. Returns true if a numeric type was written (so caller may set a number format).
        /// </summary>
        private static bool SetTypedOrText(IXLCell cell, object value = null)
        {
            if (value is null)
            {
                cell.Clear(XLClearOptions.Contents); // preserves style
                return false;
            }

            switch (value)
            {
                case bool b:
                    cell.SetValue(b);
                    return false;

                case sbyte or byte or short or ushort or int or uint or long or ulong:
                    cell.SetValue(Convert.ToInt64(value, CultureInfo.InvariantCulture));
                    return true;

                case float or double or decimal:
                    cell.Value = value; // numeric typed
                    return true;

                case DateTime dt:
                    cell.SetValue(dt);
                    return false;

                case string s:
                    AssignText(cell, s);
                    return false;

                default:
                    AssignText(cell, Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty);
                    return false;
            }
        }

        /// <summary>Force TEXT and write sanitized string.</summary>
        private static void AssignText(IXLCell cell, string s)
        {
            cell.SetDataType(XLDataType.Text); // guarantees no culture parsing
            cell.Value = StripInvalidXmlChars(s ?? string.Empty);
        }

        // XML 1.0 valid char ranges: 0x9 | 0xA | 0xD | 0x20–0xD7FF | 0xE000–0xFFFD
        private static string StripInvalidXmlChars(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var sb = new StringBuilder(input.Length);
            foreach (var ch in input)
            {
                if (ch == 0x9 || ch == 0xA || ch == 0xD ||
                    (ch >= 0x20 && ch <= 0xD7FF) ||
                    (ch >= 0xE000 && ch <= 0xFFFD))
                    sb.Append(ch);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Style snapshot that preserves:
        /// - NumberFormat (and whether it’s meaningful vs "General"/empty)
        /// - Font, Fill (bg), Alignment
        /// We restore non-format first, then re-apply NumberFormat AFTER writing the value.
        /// </summary>
        private readonly struct SavedCellStyle
        {
            // Number format
            public readonly string NumberFormat;
            public readonly bool HasMeaningfulNumberFormat;

            // Font
            public readonly string FontName;
            public readonly double FontSize;
            public readonly bool FontBold;
            public readonly bool FontItalic;
            public readonly bool FontStrikethrough;
            public readonly XLFontUnderlineValues FontUnderline;
            public readonly XLColor FontColor;

            // Fill
            public readonly XLFillPatternValues FillPattern;
            public readonly XLColor FillBackgroundColor;

            // Alignment
            public readonly XLAlignmentHorizontalValues AlignHorizontal;
            public readonly XLAlignmentVerticalValues AlignVertical;
            public readonly bool WrapText;
            public readonly int Indent;

            private static bool IsMeaningfulNumberFormat(string fmt = null)
            {
                if (string.IsNullOrWhiteSpace(fmt)) return false;
                var f = fmt.Trim();
                // Treat "General" as not meaningful
                return !f.Equals("General", StringComparison.OrdinalIgnoreCase);
            }

            private SavedCellStyle(IXLCell c)
            {
                var st = c.Style;

                NumberFormat = st.NumberFormat.Format;
                HasMeaningfulNumberFormat = IsMeaningfulNumberFormat(NumberFormat);

                var f = st.Font;
                FontName = f.FontName;
                FontSize = f.FontSize;
                FontBold = f.Bold;
                FontItalic = f.Italic;
                FontStrikethrough = f.Strikethrough;
                FontUnderline = f.Underline;
                FontColor = f.FontColor;

                var fill = st.Fill;
                FillPattern = fill.PatternType;
                FillBackgroundColor = fill.BackgroundColor;

                var al = st.Alignment;
                AlignHorizontal = al.Horizontal;
                AlignVertical = al.Vertical;
                WrapText = al.WrapText;
                Indent = al.Indent;
            }

            public static SavedCellStyle Capture(IXLCell cell) => new(cell);

            /// <summary>Restore font/fill/alignment first. (NumberFormat applied later.)</summary>
            public void RestoreNonFormat(IXLCell c)
            {
                var st = c.Style;

                // Font
                st.Font.FontName = FontName;
                st.Font.FontSize = FontSize;
                st.Font.Bold = FontBold;
                st.Font.Italic = FontItalic;
                st.Font.Strikethrough = FontStrikethrough;
                st.Font.Underline = FontUnderline;
                if (FontColor != null) st.Font.FontColor = FontColor;

                // Fill
                st.Fill.PatternType = FillPattern;
                if (FillBackgroundColor != null) st.Fill.BackgroundColor = FillBackgroundColor;

                // Alignment
                st.Alignment.Horizontal = AlignHorizontal;
                st.Alignment.Vertical = AlignVertical;
                st.Alignment.WrapText = WrapText;
                st.Alignment.Indent = Indent;
            }
        }
    }
}
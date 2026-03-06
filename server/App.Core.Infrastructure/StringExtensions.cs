using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace App.Core.Infrastructure
{
    public static class StringExtensions
    {
        public static string ToSubstring(this string input, int lebgth = 19)
        {
            return input.Length > lebgth ? input.Substring(0, lebgth - 2) + "..." : input + ".";
        }
        public static bool ToContains(this string source, string destination)
        {
            return source.ToUpperInvariant().Contains(destination.ToUpperInvariant());
        }
        public static string ToCamelCase(this string value)
        {
            return Char.ToLowerInvariant(value[0]) + value.Substring(1);
        }
        public static string ToPascalCase(this string value)
        {
            return Char.ToUpperInvariant(value[0]) + value.Substring(1);
        }
        public static string ToSuggestion(this string camelCase)
        {
            List<char> chars = new List<char>();
            chars.Add(camelCase[0]);
            foreach (char c in camelCase.Skip(1))
            {
                if (char.IsUpper(c))
                {
                    chars.Add(' ');
                    chars.Add(char.ToLower(c));
                }
                else
                    chars.Add(c);
            }

            return new string(chars.ToArray());
        }

        public static bool Include(this string value, string search)
        {
            value = value?.ToLower() ?? "";
            search = search?.ToLower() ?? "";
            return value.Contains(search);
        }

        public static string ToStringGR(this decimal value)
        {
            return value.ToString("N2", new CultureInfo("el-GR"));
        }

        public static decimal ToDecimal(this string value)
        {
            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.CreateSpecificCulture("el-GR"), out decimal number))
            {
                return number;
            }

            return 0m;
        }

        public static decimal ToEuroDecimal(this string value)
        {
            try
            {
                return value.Split('\u00A0')[0].ToDecimal();
            }
            catch
            {
                return value.Split('\u0020')[0].ToDecimal();     // character regular whitespace (\u0020)
            }
        }

        public static DateTime ToUtcRelative(this DateTime source, bool dateWithTime = true)
        {
            if (!dateWithTime)
                return new DateTime(source.Year, source.Month, source.Day, 0, 0, 0, 0, DateTimeKind.Utc);

            return new DateTime(source.Ticks, DateTimeKind.Utc);
        }
        //static void Main()
        //{
        //    // Convert DateTime to Guid
        //    DateTime dateTimeValue = new DateTime(2023, 12, 17, 22, 55, 59);
        //    Guid guidFromDateTime = DateTimeToGuid(dateTimeValue);
        //    var ticks = dateTimeValue.Ticks;

        //    Console.WriteLine($"Original DateTime: {ticks}");
        //    Console.WriteLine($"Original DateTime: {new DateTime(ticks)}");
        //    Console.WriteLine($"Original DateTime: {dateTimeValue}");
        //    Console.WriteLine($"Converted Guid: {guidFromDateTime}");

        //    // Convert Guid back to DateTime
        //    DateTime dateTimeFromGuid = GuidToDateTime(guidFromDateTime);

        //    Console.WriteLine($"Converted DateTime: {dateTimeFromGuid}");
        //}

        public static Guid DateTimeToGuid(this DateTime dateTime)
        {
            // Get bytes from DateTime ticks
            byte[] ticksBytes = BitConverter.GetBytes(dateTime.Ticks);

            // Get bytes from a randomly generated identifier
            byte[] randomBytes = Guid.NewGuid().ToByteArray();

            // Combine the bytes
            byte[] combinedBytes = new byte[16];
            Array.Copy(ticksBytes, combinedBytes, 8);
            Array.Copy(randomBytes, 0, combinedBytes, 8, 8);

            // Create a Guid from the combined bytes
            return new Guid(combinedBytes);
        }

        public static DateTime GuidToDateTime(this Guid guid)
        {
            // Get the bytes from the Guid
            byte[] combinedBytes = guid.ToByteArray();

            // Extract the first 8 bytes (DateTime ticks)
            long ticks = BitConverter.ToInt64(combinedBytes, 0);

            // Create a DateTime from ticks
            return new DateTime(ticks);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;

namespace App.Core.Infrastructure
{
    public static class DateTimeExtensions
    {
        private static DateTime GetOrthodoxEaster(int year)
        {
            var a = year % 19;
            var b = year % 7;
            var c = year % 4;

            var d = (19 * a + 16) % 30;
            var e = (2 * c + 4 * b + 6 * d) % 7;
            var f = (19 * a + 16) % 30;

            var key = f + e + 3;
            var month = (key > 30) ? 5 : 4;
            var day = (key > 30) ? key - 30 : key;

            return new DateTime(year, month, day);
        }

        private static List<DateTime> GetPublicHolidaysDates(int year)
        {
            var firstDayOfYear = new DateTime(year, 1, 1);
            var lightsHoliday = new DateTime(year, 1, 6);
            var march25 = new DateTime(year, 3, 25);
            var easter = GetOrthodoxEaster(year);
            var cleanMonday = easter.AddDays(-48);
            var easterSecondDay = easter.AddDays(1);
            var saintLight = easter.AddDays(50);
            var fisrtMay = new DateTime(year, 5, 1);
            var assumptionOfMary = new DateTime(year, 8, 15);
            var october26 = new DateTime(year, 10, 26);
            var october28 = new DateTime(year, 10, 28);
            var christmas = new DateTime(year, 12, 25);

            var publicHolidays = new List<DateTime> {firstDayOfYear,lightsHoliday,march25,cleanMonday,easterSecondDay,saintLight,fisrtMay,
            assumptionOfMary,october26,october28,christmas};

            return publicHolidays;
        }

        // ***Επιστρέφει την πρώτη εργάσιμη μετά το τέλος του μήνα αν η τελευταια μέρα είναι αργία *** 
        public static DateTime CheckPaymentDate2(this DateTime paymentDate)
        {
            var year = paymentDate.Year;
            var dayOfWeek = paymentDate.DayOfWeek;
            var publicHolidays = GetPublicHolidaysDates(year);

            if (dayOfWeek.Equals(DayOfWeek.Saturday))
                paymentDate = paymentDate.AddDays(2);

            else if (dayOfWeek.Equals(DayOfWeek.Sunday))
                paymentDate = paymentDate.AddDays(1);

            if (publicHolidays.Contains(paymentDate) && dayOfWeek.Equals(DayOfWeek.Friday))
                paymentDate = paymentDate.AddDays(3);

            else if (publicHolidays.Contains(paymentDate))
                paymentDate.AddDays(1);

            return paymentDate;
        }

        // *** Επιστρέφει την τελευταία εργάσιμη του τρέχοντος μήνα ***
        public static DateTime CheckPaymentDate(this DateTime lastDayOfMonth)
        {
            var year = lastDayOfMonth.Year;
            var dayOfWeek = lastDayOfMonth.DayOfWeek;
            var publicHolidays = GetPublicHolidaysDates(year);

            if (dayOfWeek.Equals(DayOfWeek.Saturday))
                lastDayOfMonth = lastDayOfMonth.AddDays(-1);

            else if (dayOfWeek.Equals(DayOfWeek.Sunday))
                lastDayOfMonth = lastDayOfMonth.AddDays(-2);

            if (publicHolidays.Contains(lastDayOfMonth) && dayOfWeek.Equals(DayOfWeek.Monday))
                lastDayOfMonth = lastDayOfMonth.AddDays(-3);

            else if (publicHolidays.Contains(lastDayOfMonth))
                lastDayOfMonth.AddDays(-1);

            return lastDayOfMonth;
        }

        public static DateTime ToDateGR(this string value)
        {
            // First way 
            //return Convert.ToDateTime(value.Trim(), new CultureInfo("el-GR"));

            //Second way
            return DateTime.ParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

    }
}
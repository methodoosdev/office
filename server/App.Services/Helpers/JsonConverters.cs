using App.Services.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Newtonsoft.Json
{
    public class DateTimeNullableConverter : JsonConverter<DateTime?>
    {
        private readonly IDateTimeHelper _dateTimeHelper;

        public DateTimeNullableConverter(IDateTimeHelper dateTimeHelper)
        {
            _dateTimeHelper = dateTimeHelper;
        }

        public override DateTime? ReadJson(JsonReader reader, Type objectType, DateTime? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var success = DateTime.TryParse(reader.Value?.ToString(), out var value);

            if (!success)
                return null;

            return _dateTimeHelper.ConvertToUserTimeAsync(value).Result;
        }

        public override void WriteJson(JsonWriter writer, DateTime? value, JsonSerializer serializer)
        {
            writer.WriteValue(value.HasValue ? value.Value.ToString("s") : null);
            //var date = new DateTime(value.Ticks, DateTimeKind.Utc);
            //var date = _dateTimeHelper.ConvertToUserTimeAsync(value).Result;
            //writer.WriteValue(value.ToString());
        }
    }

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private readonly IDateTimeHelper _dateTimeHelper;

        public DateTimeConverter(IDateTimeHelper dateTimeHelper)
        {
            _dateTimeHelper = dateTimeHelper;
        }

        public override bool CanWrite => base.CanWrite;

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var date = DateTime.TryParse(reader.Value.ToString(), out var value) ? value : DateTime.UtcNow;
            return _dateTimeHelper.ConvertToUserTimeAsync(date).Result;
        }

        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString("s"));
            //var date = new DateTime(value.Ticks, DateTimeKind.Utc);
            //var date = _dateTimeHelper.ConvertToUserTimeAsync(value).Result;
            //writer.WriteValue(value.ToString());
        }
    }
}
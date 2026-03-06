using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Newtonsoft.Json
{
    public class DynamicPropertyConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<T>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<List<T>>();
            }
            else if (token.Type == JTokenType.Object)
            {
                return new List<T> { token.ToObject<T>() };
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is List<T> list)
            {
                writer.WriteStartArray();

                foreach (var item in list)
                {
                    serializer.Serialize(writer, item);
                }

                writer.WriteEndArray();
            }
        }
    }
}
using System;
using Newtonsoft.Json;

namespace LMS.Services.Helpers
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string DateFormat = "yyyy-MM-dd";

        public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(DateFormat));
        }

        public override DateOnly ReadJson(JsonReader reader, Type objectType, DateOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Date)
            {
                var dateString = reader.Value.ToString();
                if (DateTime.TryParse(dateString, out DateTime dateTime))
                {
                    return DateOnly.FromDateTime(dateTime);
                }
                else if (DateOnly.TryParseExact(dateString, DateFormat, null, System.Globalization.DateTimeStyles.None, out DateOnly date))
                {
                    return date;
                }
            }

            throw new JsonException($"Unable to convert \"{reader.Value}\" to DateOnly.");
        }
    }
}

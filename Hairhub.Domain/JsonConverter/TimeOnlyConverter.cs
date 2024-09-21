using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hairhub.Domain.JsonConverter
{
    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
    {
        private readonly string _timeFormat = "HH:mm";

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? timeString = reader.GetString();
            if (TimeOnly.TryParseExact(timeString, _timeFormat, out TimeOnly result))
            {
                return result;
            }
            throw new JsonException("Invalid time format.");
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_timeFormat));
        }
    }
}

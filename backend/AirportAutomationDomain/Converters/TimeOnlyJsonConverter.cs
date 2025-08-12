using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirportAutomation.Core.Converters
{
	public sealed class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
	{
		public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return TimeOnly.ParseExact(reader.GetString(), "HH:mm:ss", CultureInfo.InvariantCulture);
		}

		public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
		{
			var timeString = value.ToString("HH:mm:ss");
			writer.WriteStringValue(timeString);
		}
	}
}

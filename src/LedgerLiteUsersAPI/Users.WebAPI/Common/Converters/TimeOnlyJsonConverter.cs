using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Users.WebAPI.Common.Converters
{
    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
    {
        private const string DefaultTimeFormat = "HH:mm:ss";

        // Methods:
        /// <summary>
        /// Reads and converts the JSON representation of a <see cref="TimeOnly"/> object.
        /// </summary>
        /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
        /// <param name="typeToConvert">The type of the object to convert, which is <see cref="TimeOnly"/>.</param>
        /// <param name="options">Options to use for deserialization.</param>
        /// <returns>The <see cref="TimeOnly"/> value parsed from the JSON.</returns>
        /// <exception cref="JsonException">Thrown when the JSON value cannot be parsed into a valid <see cref="TimeOnly"/> object using the expected format.</exception>
        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();

            return (TimeOnly.TryParseExact(value, DefaultTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly timeOnly))
                ? timeOnly
                : throw new JsonException(
                    $"##### [Users.WebAPI.Common.Converters.TimeOnlyJsonConverters.cs] [Read()] Invalid time format! Expect {DefaultTimeFormat}, got \"{value}\". #####"
                );
        }

        /// <summary>
        /// Writes a <see cref="TimeOnly"/> value as a JSON string using the specified format.
        /// </summary>
        /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
        /// <param name="value">The <see cref="TimeOnly"/> value to convert to JSON.</param>
        /// <param name="options">Options to use for serialization.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="writer"/> is <c>null</c>.</exception>
        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString(DefaultTimeFormat));
    }
}
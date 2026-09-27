using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Users.WebAPI.Common.Converters
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string DefaultDateFormat = "yyyy-MM-dd";

        // Methods:
        /// <summary>
        /// Reads and converts the JSON string representation of a date to a <see cref="DateOnly"/> object.
        /// </summary>
        /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
        /// <param name="typeToConvert">The type of the object to convert to. This parameter is ignored in this implementation.</param>
        /// <param name="options">Options to control the behavior during deserialization. This parameter is ignored in this implementation.</param>
        /// <returns>The <see cref="DateOnly"/> object parsed from the JSON string.</returns>
        /// <exception cref="JsonException">Thrown when the JSON string does not match the expected date format.</exception>
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();

            return (DateOnly.TryParseExact(value, DefaultDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly dateOnly))
                ? dateOnly
                : throw new JsonException(
                    $"##### [Users.WebAPI.Common.Converters.DateOnlyJsonConverter.cs] [Read()] Invalid date format: expected \"{DefaultDateFormat}\", got\"{value}\". #####"
                );
        }

        /// <summary>
        /// Writes the <see cref="DateOnly"/> value as a JSON string representation.
        /// </summary>
        /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
        /// <param name="value">The <see cref="DateOnly"/> value to write.</param>
        /// <param name="options">Options to control the behavior during serialization. This parameter is ignored in this implementation.</param>
        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
    }
}
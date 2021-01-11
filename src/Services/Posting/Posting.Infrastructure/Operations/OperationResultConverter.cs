#nullable enable
using System;
using Newtonsoft.Json;
using Posting.Core.Interfaces.Asp;

namespace Posting.Infrastructure.Operations
{
    public class OperationResultConverter : JsonConverter
    {
        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is IOperationResult<object> r)
            {
                if (r.IsSuccess)
                {
                    serializer.Serialize(writer, r.Result);
                }
                else if (!r.IsSuccess)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("error");
                    serializer.Serialize(writer, r.Error);
                    writer.WriteEndObject();
                }
            }
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IOperationResult<object>).IsAssignableFrom(objectType);
        }
    }
}
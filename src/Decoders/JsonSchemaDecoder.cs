using System;
using System.Text.Json;

namespace RestSchema.Decoders
{
    /// <summary>
    /// JSON Schema Decoder
    /// </summary>
    internal sealed class JsonSchemaDecoder : ISchemaDecoder
    {
        private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public Schema Decode( string json ) =>
            JsonSerializer.Deserialize<Schema>( json, serializerOptions );
    }
}

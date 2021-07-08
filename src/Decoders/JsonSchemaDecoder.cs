using System;
using System.Text.Json;

namespace RestSchema.Decoders
{
    internal sealed class JsonSchemaDecoder : ISchemaDecoder
    {
        public Schema Decode( string json ) =>
            JsonSerializer.Deserialize<Schema>( json );
    }
}

using System;

namespace RestSchema.Decoders
{
    internal interface ISchemaDecoder
    {
        Schema Decode( string raw );
    }
}

using System;
using System.Text;
using Faactory.Types;

namespace RestSchema.Decoders
{
    internal static class SchemaDecoder
    {
        private static readonly ISchemaDecoder jsonDecoder = new JsonSchemaDecoder();
        private static readonly ISchemaDecoder textDecoder = new TextSchemaDecoder();

        public static Schema Decode( string raw )
        {
            // plain-text format (doesn't use encoding)
            if ( raw.StartsWith( "?" ) || raw.StartsWith( "#" ) )
            {
                return textDecoder.Decode( raw );
            }

            var content = Encoding.UTF8.GetString( Base64Encoder.FromBase64String( raw ) );

            // json format
            if ( content.StartsWith( "{" ) )
            {
                return jsonDecoder.Decode( content );
            }

            // assuming everything else is yaml format
            //return yamlDecoder.Decode( content );

            throw new FormatException( "Unknown schema format!" );
        }
    }
}

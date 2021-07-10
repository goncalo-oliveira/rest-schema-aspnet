using System;
using System.Text;

namespace RestSchema.Decoders
{
    internal static class SchemaDecoder
    {
        private static readonly ISchemaDecoder jsonDecoder = new JsonSchemaDecoder();
        private static readonly ISchemaDecoder textDecoder = new TextSchemaDecoder();

        public static Schema Decode( string raw )
        {
            // plain-text format (doesn't use encoding)
            if ( raw.Contains( ',' ) || raw.Contains( ';' ) )
            {
                return textDecoder.Decode( raw );
            }

            var base64 = FromBase64Url( raw ); // ensure we have a base64 and not a base64url
            var buffer = new Span<byte>( new byte[raw.Length] );
            if ( !Convert.TryFromBase64String( raw, buffer, out int bytesWritten ) )
            {
                // not a valid base64 value, it's most likely plain-text format
                return textDecoder.Decode( base64 );
            }

            var content = Encoding.UTF8.GetString( buffer ).TrimEnd( '\0' );

            // json format
            if ( content.StartsWith( "{" ) )
            {
                return jsonDecoder.Decode( content );
            }

            // assuming everything else is yaml format
            //return yamlDecoder.Decode( content );

            throw new FormatException( "Unknown schema format!" );
        }

        /// <summary>
        /// Converts from a possible base64url string to base64 string
        /// </summary>
        /// <param name="value">The possible base64url</param>
        /// <returns>A base64 string</returns>
        private static string FromBase64Url( string value )
        {
            string base64Value = value.Replace( '_', '/' )
                .Replace( '-', '+' );

            switch( value.Length % 4 )
            {
                case 2: base64Value += "=="; break;
                case 3: base64Value += "="; break;
            }
            
            return ( base64Value );
        }
    }
}

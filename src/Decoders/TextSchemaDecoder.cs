using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RestSchema.Decoders
{
    /// <summary>
    /// Plain-Text Schema decoder
    /// </summary>
    public sealed class TextSchemaDecoder : ISchemaDecoder
    {
        private const string regex = @"^([a-zA-Z0-9_.]+)+\[([a-zA-Z0-9_]+)(\,[a-zA-Z0-9_]+)*\](\,*([a-zA-Z0-9_.]+)+\[([a-zA-Z0-9_]+)(\,[a-zA-Z0-9_]+)*\])*$";
        

        public Schema Decode( string text )
        {
            text = text.Replace( '(', '[' )
                .Replace( ')', ']' );

            // validate expression
            if ( !Regex.IsMatch( text, regex ) )
            {
                return Schema.Empty;
            }

            var schema = new Schema
            {
                Version = SchemaVersion.Value.ToString(),
                Spec = new SchemaSpec()
            };

            // parse expression
            var idx = 0;
            var schemaStartIdx = 0;
            var schemaEndIdx = 0;

            var name = string.Empty;
            var values = Array.Empty<string>();

            while ( idx < text.Length )
            {
                schemaStartIdx = text.IndexOf( '[', idx );
                schemaEndIdx = text.IndexOf( ']', schemaStartIdx );

                name = text.Substring( idx, schemaStartIdx - idx );
                values = text.Substring( schemaStartIdx + 1, schemaEndIdx - schemaStartIdx - 1 )
                    .Split( ',' );

                idx = schemaEndIdx + 2;

                schema.Spec.Add( name, values );
            }

            //

            return ( schema );
        }
    }
}

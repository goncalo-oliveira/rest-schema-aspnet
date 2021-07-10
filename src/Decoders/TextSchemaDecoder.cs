using System;
using System.Linq;

namespace RestSchema.Decoders
{
    /// <summary>
    /// Plain-Text Schema decoder
    /// </summary>
    internal sealed class TextSchemaDecoder : ISchemaDecoder
    {
        public Schema Decode( string text )
        {
            var schema = new Schema
            {
                Spec = new SchemaSpec()
            };

            // split schemas
            var items = text.Split( ';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );

            foreach ( var item in items )
            {
                var values = item;
                var name = "_";

                if ( values.Contains( '=' ) )
                {
                    var idx = values.IndexOf( '=' );
                    name = values.Substring( 0, idx );
                    values = values.Substring( idx + 1 );
                }
                else if ( schema.Spec.Any() )
                {
                    //throw new ArgumentException( "Invalid schema spec!" );
                    // TODO: log error
                    return Schema.Empty;
                }

                var properties = values.TrimEnd( ';' )
                    .Split( ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );

                schema.Spec.Add( name, properties );
            }
            //

            return ( schema );
        }
    }
}

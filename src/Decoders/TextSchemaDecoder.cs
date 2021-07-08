using System;

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

            // do we have a version?
            if ( text.StartsWith( '#' ) )
            {
                schema.Version = text.Substring( 1, text.IndexOf( '?' ) - 1 );
                text = text.Substring( text.IndexOf( '?' ) );

                if ( Version.Parse( schema.Version ) > SchemaVersion.Value )
                {
                    //throw new NotImplementedException( $"The schema spec version '{schema.Version}' is not yet implemented." );
                    // TODO: log error
                    return Schema.Empty;
                }
            }

            // split schemas
            var items = text.Split( ';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries );

            foreach ( var item in items )
            {
                if ( !item.StartsWith( '?' ) )
                {
                    //throw new ArgumentException( "Invalid schema spec!" );
                    // TODO: log error
                    return Schema.Empty;
                }

                var values = item.Substring( 1 );
                var name = "_";

                if ( values.Contains( ':' ) )
                {
                    var idx = values.IndexOf( ':' );
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

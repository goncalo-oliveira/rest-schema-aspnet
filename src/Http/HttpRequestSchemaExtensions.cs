using Microsoft.AspNetCore.Http;
using RestSchema;
using RestSchema.Decoders;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Schema extensions for HttpRequest
    /// </summary>
    public static class HttpRequestSchemaExtensions
    {
        /// <summary>
        /// Gets if a given property is included in the schema data
        /// </summary>
        /// <param name="propertyName">The property name to verify</param>
        /// <returns>True if the given property is included in the schema, false otherwise</returns>
        public static bool SchemaIncludes( this HttpRequest request, string propertyName )
        {
            return SchemaIncludes( request, null, propertyName );
        }

        /// <summary>
        /// Gets if a given property is included in the given schema data
        /// </summary>
        /// <param name="specName">The name of the schema to verify</param>
        /// <param name="propertyName">The property name to verify</param>
        /// <returns>True if the given property is included in the schema, false otherwise</returns>
        public static bool SchemaIncludes( this HttpRequest request, string specName, string propertyName )
        {
            // If there's no Schema-Mapping or Schema-Include this method returns false
            if ( !request.Headers.ContainsKey( SchemaHeaders.SchemaMapping ) 
              && !request.Headers.ContainsKey( SchemaHeaders.SchemaInclude ) )
            {
                return ( false );
            }

            // extract Schema-Mapping
            if ( request.Headers.TryGetValue( SchemaHeaders.SchemaMapping, out var mapping ) )
            {
                var schema = SchemaDecoder.Decode( mapping );

                return schema.Spec.ContainsProperty( specName, propertyName );
            }

            // extract Schema-Include
            if ( request.Headers.TryGetValue( SchemaHeaders.SchemaInclude, out var include ) )
            {
                var schema = SchemaDecoder.Decode( include );

                return schema.Spec.ContainsProperty( specName, propertyName );
            }

            return ( false );
        }

        public static Schema GetSchema( this HttpRequest request, string schemaHeaderKey )
        {
            if ( !request.Headers.TryGetValue( schemaHeaderKey, out var encodedSchema ) )
            {
                return ( null );
            }

            var schema = SchemaDecoder.Decode( encodedSchema );

            return ( schema );

        }
    }
}

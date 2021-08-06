using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using RestSchema;
using RestSchema.Decoders;

namespace RestSchema.Mvc
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

            if ( schema.Spec.Count == 0 )
            {
                return ( null );
            }

            // add natural arguments from query string as filters
            var additionalFilters = GetQueryFilters( request );

            if ( additionalFilters?.Any() == true )
            {
                if ( schema.Filters == null )
                {
                    // assign filters
                    schema.Filters = additionalFilters;
                }
                else
                {
                    // append additional filters
                    foreach ( var kvp in additionalFilters )
                    {
                        schema.Filters.Add( kvp.Key, kvp.Value );
                    }
                }
            }

            return ( schema );

        }

        public static Dictionary<string, string> GetQueryFilters( this HttpRequest request )
        {
            // TODO: refactor this

            // add natural arguments from query string as filters
            var naturalArgs = request.Query?.Where( x => !x.Key.StartsWith( '_' ) )
                .ToArray();

            if ( !( naturalArgs?.Any() == true ) )
            {
                return null;
            }

            var dictionary = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase );

            foreach ( var arg in naturalArgs )
            {
                var key = arg.Key;
                var op = "==";

                if ( key.EndsWith( "_ne" ) )
                {
                    key = key.Substring( 0, key.Length - "_ne".Length );
                    op = "!="; 
                }
                else if ( key.EndsWith( "_gt" ) )
                {
                    key = key.Substring( 0, key.Length - "_gt".Length );
                    op = ">"; 
                }
                else if ( key.EndsWith( "_gte" ) )
                {
                    key = key.Substring( 0, key.Length - "_gte".Length );
                    op = ">="; 
                }
                else if ( key.EndsWith( "_lt" ) )
                {
                    key = key.Substring( 0, key.Length - "_lt".Length );
                    op = "<"; 
                }
                else if ( key.EndsWith( "_lte" ) )
                {
                    key = key.Substring( 0, key.Length - "_lte".Length );
                    op = "<=";
                }

                dictionary.Add( key, string.Concat( op, arg.Value ) );
            }
            //

            return dictionary;
        }
    }
}

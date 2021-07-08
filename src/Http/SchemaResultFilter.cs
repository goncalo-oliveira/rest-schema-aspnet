using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace RestSchema.Http
{
    internal sealed class AsyncSchemaResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync( ResultExecutingContext context, ResultExecutionDelegate next )
        {
            if ( !( context.Result is ObjectResult result ) )
            {
                await next();

                return;
            }

            var value = result.Value;

            if ( value == null )
            {
                await next();

                return;
            }

            var schemaMapping = context.HttpContext.Request.GetSchema( SchemaHeaders.SchemaMapping );

            if ( schemaMapping == null )
            {
                // no available schema... do nothing...
                await next();

                return;
            }

            var chrono = System.Diagnostics.Stopwatch.StartNew();

            // single object or enumerable?
            if ( typeof( IEnumerable ).IsAssignableFrom( value.GetType() ) )
            {
                // an enumerable...
                var list = new System.Collections.Generic.List<object>();

                foreach ( var item in (IEnumerable)value )
                {
                    var json = System.Text.Json.JsonSerializer.Serialize( item );
                    var dict = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, object>>( json );

                    dict = ApplySchemaMapping( schemaMapping, dict );

                    list.Add( dict );
                }

                result.Value = list.ToArray();
            }
            else
            {
                // single object
                var json = System.Text.Json.JsonSerializer.Serialize( value );
                var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>( json );

                dict = ApplySchemaMapping( schemaMapping, dict );

                result.Value = dict;
            }

            chrono.Stop();

            // add Schema headers
            context.HttpContext.Response.Headers.Add( "X-Schema-Map-Duration", chrono.Elapsed.TotalMilliseconds.ToString() );
            context.HttpContext.Response.Headers.Add( "X-Schema-Version", SchemaVersion.Value.ToString() );

            await next();
        }

        private Dictionary<string, object> ApplySchemaMapping( Schema schemaMapping, Dictionary<string, object> item )
        {
            return ApplySchemaMapping( schemaMapping, schemaMapping.Spec.Root().Key, item );
        }

        private Dictionary<string, object> ApplySchemaMapping( Schema schemaMapping, string schemaName, Dictionary<string, object> item )
        {
            // TODO: apply schema mapping for property values

            // select only properties that are in the schema
            var marked = item.Where( x => schemaMapping.Spec.ContainsProperty( schemaName, x.Key ) )
                .ToDictionary( x => x.Key, x => x.Value );

            // look for schema mappings for property value
            var unmapped = marked.Where( x => schemaMapping.Spec.ContainsSpec( x.Key ) 
                               || schemaMapping.Spec.ContainsSpec( $"{schemaName}.{x.Key}" ) )
                            .ToArray();

            foreach ( var kvp in unmapped )
            {
                // find explicit mapping
                var valueSchemaName = $"{schemaName}.{kvp.Key}";
                if ( !schemaMapping.Spec.ContainsSpec( valueSchemaName ) )
                {
                    // we already know a mapping exists, so if not the full name
                    // then it's the simplified name
                    valueSchemaName = kvp.Key;
                }

                var json = System.Text.Json.JsonSerializer.Serialize( kvp.Value );
                var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>( json );

                marked[kvp.Key] = ApplySchemaMapping( schemaMapping, valueSchemaName, dict );
            }

            return marked;
        }

    }
}

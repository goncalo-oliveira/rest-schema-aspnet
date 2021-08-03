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

namespace RestSchema.Mvc.Filters
{
    internal sealed class SchemaMapResultFilter : IAsyncResultFilter
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

            // lookup for ignore attribute
            var attr = context.Controller.GetType()
                .GetCustomAttributes( typeof( SchemaIgnoreAttribute ), true )
                .SingleOrDefault();

            // TODO: lookup method ignore attribute
            // TODO: improve custom attributes
            // https://stackoverflow.com/questions/31874733/how-to-read-action-methods-attributes-in-asp-net-core-mvc

            if ( attr != null )
            {
                // explicit ignore schema
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
                    var dict = ApplySchemaMapping( schemaMapping, item );

                    list.Add( dict );
                }

                result.Value = list.ToArray();
            }
            else
            {
                // single object
                var dict = ApplySchemaMapping( schemaMapping, value );

                result.Value = dict;
            }

            chrono.Stop();

            // add Schema headers
            context.HttpContext.Response.Headers.Add( SchemaHeaders.SchemaMapping + "-Duration"
                , chrono.Elapsed.TotalMilliseconds.ToString() + "ms" );

            context.HttpContext.Response.Headers.Add( SchemaHeaders.SchemaVersion
                , SchemaVersion.Value.ToString() );

            await next();
        }

        private Dictionary<string, object> ApplySchemaMapping( Schema schemaMapping, object item )
        {
            return ApplySchemaMapping( schemaMapping, schemaMapping.Spec.Root().Key, item );
        }

        private Dictionary<string, object> ApplySchemaMapping( Schema schemaMapping, string schemaName, object item )
        {
            var dict = new Microsoft.AspNetCore.Routing.RouteValueDictionary( item );

            // select only properties that are in the schema
            var selected = dict.Where( x => schemaMapping.Spec.ContainsProperty( schemaName, x.Key ) )
                .ToDictionary( x => x.Key, x => x.Value );

            // look for additional schema mappings for property values
            var additionalMappings = selected.Where( x => schemaMapping.Spec.ContainsKey( x.Key ) 
                               || schemaMapping.Spec.ContainsKey( $"{schemaName}.{x.Key}" ) )
                            .ToArray();

            foreach ( var kvp in additionalMappings )
            {
                // find explicit mapping
                var valueSchemaName = $"{schemaName}.{kvp.Key}";
                if ( !schemaMapping.Spec.ContainsKey( valueSchemaName ) )
                {
                    // we already know a mapping exists, so if not the full name
                    // then it's the simplified name
                    valueSchemaName = kvp.Key;
                }

                selected[kvp.Key] = ApplySchemaMapping( schemaMapping, valueSchemaName, kvp.Value );
            }

            return ( selected );
        }

    }
}

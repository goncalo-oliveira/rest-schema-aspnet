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
        public Task OnResultExecutionAsync( ResultExecutingContext context, ResultExecutionDelegate next )
        {
            if ( !( context.Result is ObjectResult result ) )
            {
                return next();
            }

            var value = result.Value;

            if ( value == null )
            {
                return next();
            }

            var schemaMapping = context.HttpContext.Request.GetSchema( SchemaHeaders.SchemaMapping );

            if ( schemaMapping == null )
            {
                // no available schema...execute the filtering separately
                return OnFilterExecutionAsync( context, next );
            }

            // lookup for ignore attribute
            var ignoreSchema = context.ActionDescriptor.EndpointMetadata.OfType<SchemaIgnoreAttribute>()
                .Any();

            if ( ignoreSchema )
            {
                // explicit ignore schema
                return next();
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

                var filtered = ApplySchemaFilters( schemaMapping, list );

                result.Value = filtered.ToArray();
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

            return next();
        }

        private Task OnFilterExecutionAsync( ResultExecutingContext context, ResultExecutionDelegate next )
        {
            var queryFilters = context.HttpContext.Request.GetQueryFilters();

            if ( !( queryFilters?.Any() == true ) )
            {
                return next();
            }

            // lookup for ignore attribute
            var ignoreSchema = context.ActionDescriptor.EndpointMetadata.OfType<SchemaIgnoreAttribute>()
                .Any();

            if ( ignoreSchema )
            {
                // explicit ignore schema
                return next();
            }

            var result = (ObjectResult)context.Result;
            var value = result.Value;

            // single object or enumerable?
            if ( typeof( IEnumerable ).IsAssignableFrom( value.GetType() ) )
            {
                var chrono = System.Diagnostics.Stopwatch.StartNew();

                // an enumerable...
                var list = new System.Collections.Generic.List<object>();

                foreach ( var item in (IEnumerable)value )
                {
                    var dict = new Microsoft.AspNetCore.Routing.RouteValueDictionary( item )
                        .ToDictionary( x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase );

                    list.Add( dict );
                }

                // we need to build a schema based on the result
                var schemaMapping = new Schema
                {
                    Spec = new SchemaSpec(),
                    Filters = queryFilters
                };

                GenerateSchemaSpec( schemaMapping.Spec, "_", ((IEnumerable)value).GetFirstElement() );

                var filtered = ApplySchemaFilters( schemaMapping, list );

                result.Value = filtered.ToArray();

                chrono.Stop();

                // add Schema headers
                context.HttpContext.Response.Headers.Add( SchemaHeaders.SchemaMapping + "-Duration"
                    , chrono.Elapsed.TotalMilliseconds.ToString() + "ms" );

                context.HttpContext.Response.Headers.Add( SchemaHeaders.SchemaVersion
                    , SchemaVersion.Value.ToString() );
            }

            return next();
        }

        private Dictionary<string, object> ApplySchemaMapping( Schema schemaMapping, object item )
            => ApplySchemaMapping( schemaMapping, schemaMapping.Spec.Root().Key, item );

        private Dictionary<string, object> ApplySchemaMapping( Schema schemaMapping, string schemaName, object item )
        {
            var dict = new Microsoft.AspNetCore.Routing.RouteValueDictionary( item );

            // select only properties that are in the schema
            var selected = dict.Where( x => schemaMapping.Spec.ContainsProperty( schemaName, x.Key ) )
                .ToDictionary( x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase );

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

        private IEnumerable<object> ApplySchemaFilters( Schema schemaMapping, IEnumerable<object> items )
            => ApplySchemaFilters( schemaMapping, schemaMapping.Spec.Root().Key, items );

        private IEnumerable<object> ApplySchemaFilters( Schema schemaMapping, string schemaName, IEnumerable<object> items )
        {
            var filters = SchemaFilterCollection.Create( schemaMapping.Filters );

            if ( !filters.Any() )
            {
                // no filters
                return ( items );
            }


            var list = items.Cast<Dictionary<string, object>>();

            if ( list == null )
            {
                return ( items );
            }

            var filtered = list.Where( item =>
            {
                var selectedFilters = item.Keys.Select( x => filters.GetFilter( x ) )
                    .Where( x => x != null )
                    .ToArray();

                return selectedFilters.Select( x => x.IsMatch( item[x.Key] ) )
                    .All( x => x == true );
            } );

            return ( filtered );
        }

        private void GenerateSchemaSpec( SchemaSpec spec, string schemaName, object item )
        {
            var type = item.GetType();

            var schema = type.GetProperties()
                .Select( x => x.Name )
                .ToArray();

            spec.Add( schemaName, schema );

            // TODO: dig deeper
        }
    }
}

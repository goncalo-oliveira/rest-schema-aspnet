using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RestSchema.Mvc.Filters
{
    internal sealed class SchemaResourceFilter : IResourceFilter
    {
        public void OnResourceExecuting( ResourceExecutingContext context )
        {
            if ( !EnsureSchemaVersionIsValid( context ) )
            {
                return;
            }

            // TODO: this section requires some refactoring
            //       to organize the code and making it
            //       more efficient

            if ( !context.HttpContext.Request.Headers.ContainsKey( SchemaHeaders.SchemaMapping ) &&
                 !context.HttpContext.Request.Headers.ContainsKey( SchemaHeaders.SchemaInclude ) )
            {
                // no schema headers...
                // look for schema in query string
                // if found, we add it to the headers

                var queryKey = SchemaQueryKeys.SchemaMapping;
                if ( !context.HttpContext.Request.Query.ContainsKey( queryKey ) )
                {
                    // Schema-Mapping takes precedence, but if not found...
                    queryKey = SchemaQueryKeys.SchemaInclude;
                }

                if ( !context.HttpContext.Request.Query.ContainsKey( queryKey  ) )
                {
                    // no schema query arguments either... move on
                    return;
                }

                context.HttpContext.Request.Headers.Add( 
                    queryKey.Equals( SchemaQueryKeys.SchemaMapping ) 
                        ? SchemaHeaders.SchemaMapping
                        : SchemaHeaders.SchemaInclude
                        , context.HttpContext.Request.Query[queryKey]  );
            }

            // validate schema data on X-Schema-Map and X-Schema-Include
            var headerName = SchemaHeaders.SchemaMapping;
            if ( !context.HttpContext.Request.Headers.ContainsKey( headerName ) )
            {
                // Schema-Mapping takes precedence, but if not found...
                headerName = SchemaHeaders.SchemaInclude;
            }

            var headerValue = context.HttpContext.Request.Headers[headerName];

            try
            {
                var schema = Decoders.SchemaDecoder.Decode( headerValue );

                // explicit schema version needs to be verified as well
                if ( !string.IsNullOrEmpty( schema.Version ) &&
                   ( Version.Parse( schema.Version ) > SchemaVersion.Value ) )
                {
                    throw new NotSupportedException( "Version not supported." );
                }
            }
            catch ( Exception ex )
            {
                ReplyWithBadRequest( context, headerName, "Invalid format! " + ex.Message );

                return;
            }
        }

        public void OnResourceExecuted( ResourceExecutedContext context )
        {}

        private bool EnsureSchemaVersionIsValid( ResourceExecutingContext context )
        {
            // look for X-Schema-Version and see if it matches the server spec
            if ( !context.HttpContext.Request.Headers.TryGetValue( SchemaHeaders.SchemaVersion, out var versionValue ) )
            {
                // header not present... move on...
                return ( true );
            }

            if ( !Version.TryParse( versionValue, out var version ) )
            {
                // invalid version format!
                ReplyWithBadRequest( context, SchemaHeaders.SchemaVersion, "Invalid format!" );

                return ( false );
            }

            if ( version > SchemaVersion.Value )
            {
                // schema data spec version is higher than the server's spec version
                ReplyWithBadRequest( context, SchemaHeaders.SchemaVersion, "Version not supported." );

                return ( false );
            }

            return ( true );
        }

        private void ReplyWithBadRequest( ResourceExecutingContext context, string header, string errorMessage )
        {
            context.ModelState.AddModelError( header, errorMessage );

            context.Result = new BadRequestObjectResult( context.ModelState );

            context.HttpContext.Response.Headers.Add( SchemaHeaders.SchemaVersion
                , SchemaVersion.Value.ToString() );
        }
    }
}

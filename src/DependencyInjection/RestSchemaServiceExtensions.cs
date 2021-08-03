using System;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up MVC services with RestSchema middleware in an IServiceCollection
    /// </summary>
    public static class RestSchemaServiceExtensions
    {
        /// <summary>
        /// Adds services for controllers to the specified IServiceCollection, including RestSchema middleware. This method will not register services used for views or pages.
        /// </summary>
        /// <returns>An IMvcBuilder that can be used to further configure the MVC services</returns>
        public static IMvcBuilder AddSchemaControllers( this IServiceCollection services )
        {
            return AddSchemaControllers( services, null );
        }

        /// <summary>
        /// Adds services for controllers to the specified IServiceCollection, including RestSchema middleware. This method will not register services used for views or pages.
        /// </summary>
        /// <param name="configure">An Action to configure the provided MvcOptions</param>
        /// <returns>An IMvcBuilder that can be used to further configure the MVC services</returns>
        public static IMvcBuilder AddSchemaControllers( this IServiceCollection services, Action<MvcOptions> configure )
        {
            var builder = services.AddControllers( options =>
            {
                options.Filters.AddRestSchemaFilters();

                configure?.Invoke( options );
            } );


            builder.AddJsonOptions( options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            } );

            return ( builder );
        }
    }
}

using System;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection
{
    // TODO: Not yet implemented. This will replace the filter extensions and manually
    //       setting json options (if using default json serializer) in the startup

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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds services for controllers to the specified IServiceCollection, including RestSchema middleware. This method will not register services used for views or pages.
        /// </summary>
        /// <param name="configure">An Action to configure the provided MvcOptions</param>
        /// <returns>An IMvcBuilder that can be used to further configure the MVC services</returns>
        public static IMvcBuilder AddSchemaControllers( this IServiceCollection services, Action<MvcOptions> configure )
        {
            throw new NotImplementedException();
        }
    }
}

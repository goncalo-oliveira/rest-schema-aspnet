using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FilterServiceExtensions
    {
        /// <summary>
        /// Adds RestSchema filters
        /// </summary>
        /// <returns>An IEnumerable<Microsoft.AspNetCore.Mvc.Filters.IFilterMetadata> representing the added types</returns>
        public static IEnumerable<IFilterMetadata> AddRestSchemaFilters( this FilterCollection filters )
        {
            var resourceMetadata = filters.Add<RestSchema.Http.SchemaResourceFilter>();
            var resultMetadata = filters.Add<RestSchema.Http.AsyncSchemaResultFilter>();

            return ( new IFilterMetadata[]
            {
                resourceMetadata,
                resultMetadata
            } );
        }
    }
}

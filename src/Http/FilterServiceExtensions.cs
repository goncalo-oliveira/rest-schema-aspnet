using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FilterServiceExtensions
    {
        public static IFilterMetadata AddSchemaResultFilter( this FilterCollection filters ) =>
            filters.Add<RestSchema.Http.AsyncSchemaResultFilter>();
    }
}

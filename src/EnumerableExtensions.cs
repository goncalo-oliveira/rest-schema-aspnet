using System;
using System.Collections;

namespace RestSchema
{
    internal static class EnumerableExtensions
    {
        public static object GetFirstElement( this IEnumerable source )
        {
            var enumerator = source.GetEnumerator();

            if ( !enumerator.MoveNext() )
            {
                return ( null );
            }

            return enumerator.Current;
        }
    }
}

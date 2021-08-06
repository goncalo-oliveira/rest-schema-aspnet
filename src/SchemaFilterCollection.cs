using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RestSchema
{
    public sealed class SchemaFilterCollection : IEnumerable<SchemaFilter>
    {
        private static readonly Dictionary<string, SchemaFilterOperator> operatorMapping = new Dictionary<string, SchemaFilterOperator>
        {
            { "==", SchemaFilterOperator.Equal },
            { "~=", SchemaFilterOperator.Contains },
            { "!=", SchemaFilterOperator.NotEqual },
            { ">=", SchemaFilterOperator.GreaterOrEqual },
            { "<=", SchemaFilterOperator.LesserOrEqual },
            { ">", SchemaFilterOperator.Greater },
            { "<", SchemaFilterOperator.Lesser }
        };

        private readonly Dictionary<string, SchemaFilter> filters;

        private SchemaFilterCollection( IEnumerable<SchemaFilter> items )
        {
            filters = new Dictionary<string, SchemaFilter>(
                items.ToDictionary( x => x.Key, x => x ),
                StringComparer.OrdinalIgnoreCase );

        }

        public SchemaFilter GetFilter( string key )
        {
            if ( !filters.TryGetValue( key, out var filter ) )
            {
                return ( null );
            }

            return ( filter );
        }

        internal static SchemaFilterCollection Create( Dictionary<string, string> dictionary )
        {
            if ( !( dictionary?.Any() == true ) )
            {
                return new SchemaFilterCollection( Enumerable.Empty<SchemaFilter>() );
            }

            var items = dictionary.Select( x => Parse( x.Key, x.Value ) )
                .ToArray();

            return new SchemaFilterCollection( items );
        }

        private static SchemaFilter Parse( string key, string value )
        {
            foreach ( var mapping in operatorMapping )
            {
                if ( value.StartsWith( mapping.Key ) )
                {
                    return new SchemaFilter
                    {
                        Key = key,
                        Operator = mapping.Value,
                        Value = value.Substring( mapping.Key.Length )
                    };
                }
            }

            // use "equal" as default operator
            return new SchemaFilter
            {
                Key = key,
                Operator = SchemaFilterOperator.Equal,
                Value = value
            };
        }

        #region IEnumerable

        public IEnumerator<SchemaFilter> GetEnumerator()
        {
            return filters.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return filters.Values.GetEnumerator();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RestSchema
{
    internal static class ObjectExtensions
    {
        /// <summary>
        /// Converts an object to a Dictionary<string, object>
        /// </summary>
        /// <returns>A Dictionary<string, object> containing the object properties</returns>
        public static IDictionary<string, object> ToDictionary( this object source )
        {
            if ( source == null )
            {
                throw new NullReferenceException( "Can't convert a null object!" );
            }

            var properties = TypeDescriptor.GetProperties( source );
            var dictionary = new Dictionary<string, object>( properties.Count );

            foreach ( PropertyDescriptor property in TypeDescriptor.GetProperties( source ) )
            {
                dictionary.Add( property.Name, property.GetValue( source ) );
            }

            return ( dictionary );
        }
    }
}

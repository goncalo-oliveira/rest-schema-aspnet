using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RestSchema
{
    public sealed class SchemaSpec : Dictionary<string, string[]>
    {
        public SchemaSpec()
        : base( StringComparer.OrdinalIgnoreCase )
        {}

        public KeyValuePair<string, string[]> Root()
        {
            return this.First();
        }

        /// <summary>
        /// Gets if the root spec contains a property with the given name
        /// </summary>
        /// <param name="propertyName">The property name to validate</param>
        /// <returns>True if the root spec contains a property with the given name, false otherwise</returns>
        public bool ContainsProperty( string propertyName )
        {
            return ( Values.FirstOrDefault()?.Contains( propertyName, StringComparer.OrdinalIgnoreCase ) == true );
        }

        /// <summary>
        /// Gets if the spec for the given schema contains a property with the given name
        /// </summary>
        /// <param name="specName">The name of the schema to validate</param>
        /// <param name="propertyName">The property name to validate</param>
        /// <returns>True if the spec contains a property with the given name, false otherwise</returns>
        public bool ContainsProperty( string specName, string propertyName )
        {
            if ( string.IsNullOrEmpty( specName ) )
            {
                return ContainsProperty( propertyName );
            }

            if ( !TryGetValue( specName, out var properties ) )
            {
                // schema not found
                return ( false );
            }

            return properties.Contains( propertyName, StringComparer.OrdinalIgnoreCase );
        }

        /// <summary>
        /// Gets all properties in the root schema
        /// </summary>
        /// <returns>An array containing the schema properties</returns>
        public IEnumerable<string> GetProperties()
        {
            return Values.FirstOrDefault() ?? Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets all properties in the schema with the given name
        /// </summary>
        /// <param name="specName">The name of the schema to retrieve the properties from</param>
        /// <returns>An array containing the schema properties</returns>
        public IEnumerable<string> GetProperties( string specName )
        {
            if ( string.IsNullOrEmpty( specName ) )
            {
                return GetProperties();
            }

            if ( TryGetValue( specName, out var properties ) )
            {
                return ( properties );
            }

            return Enumerable.Empty<string>();
        }
    }
}

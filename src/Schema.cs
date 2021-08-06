using System;
using System.Collections.Generic;

namespace RestSchema
{
    /// <summary>
    /// The Schema spec data
    /// </summary>
    public sealed class Schema
    {
        public static Schema Empty = new Schema();

        public Schema()
        {
            Spec = new SchemaSpec();
        }

        /// <summary>
        /// Gets the schema spec version
        /// </summary>
        /// <value></value>
        public string Version { get; set; }

        /// <summary>
        /// Gets the schema spec
        /// </summary>
        /// <value></value>
        public SchemaSpec Spec { get; set; }

        public Dictionary<string, string> Filters { get; set; }

        // FUTURE spec
        //public Dictionary Refs { get; set; }
    }
}

using System;

namespace RestSchema.Mvc
{
    /// <summary>
    /// Indicates that a type or method won't execute REST-SCHEMA extensions
    /// </summary>
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Method )]
    public class SchemaIgnoreAttribute : Attribute
    { }
}

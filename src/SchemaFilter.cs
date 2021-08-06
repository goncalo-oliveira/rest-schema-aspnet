using System;
using System.Globalization;

namespace RestSchema
{
    public enum SchemaFilterOperator
    {
        Equal,
        NotEqual,
        Contains,
        Greater,
        GreaterOrEqual,
        Lesser,
        LesserOrEqual
    }

    public sealed class SchemaFilter
    {
        public string Key { get; set; }
        public SchemaFilterOperator Operator { get; set; }
        public string Value { get; set; }

        public bool IsMatch( object value )
        {
            if ( value == null )
            {
                return ( true );
            }

            var type = value.GetType();

            switch ( Operator )
            {
                case SchemaFilterOperator.Equal:
                    return value.ToString().Equals( Value, StringComparison.OrdinalIgnoreCase );

                case SchemaFilterOperator.NotEqual:
                    return !value.ToString().Equals( Value, StringComparison.OrdinalIgnoreCase );

                case SchemaFilterOperator.Contains:
                    return value.ToString().Contains( Value.ToString(), StringComparison.OrdinalIgnoreCase );

                case SchemaFilterOperator.Greater:
                case SchemaFilterOperator.GreaterOrEqual:
                case SchemaFilterOperator.Lesser:
                case SchemaFilterOperator.LesserOrEqual:
                    return IsMatchNumeric( value, type );

                default:
                    return true; // ignore unknown operators
            }

            // TODO: this needs to be refactored
        }

        private bool IsMatchNumeric( object value, Type type )
        {
            switch ( Type.GetTypeCode( type ) )
            {
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return IsMatchUInt( Operator, UInt64.Parse( value.ToString() ), UInt64.Parse( Value ) );

                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return IsMatchInt( Operator, Int64.Parse( value.ToString() ), Int64.Parse( Value ) );

                case TypeCode.Single:
                case TypeCode.Double:
                    return IsMatchDouble( Operator, Double.Parse( value.ToString() ), Double.Parse( Value ) );

                case TypeCode.Decimal:
                    return IsMatchDecimal( Operator, Decimal.Parse( value.ToString() ), Decimal.Parse( Value ) );

                default:
                    return false;
            }
        }

        private bool IsMatchUInt( SchemaFilterOperator filterOperator, ulong value1, ulong value2 )
        {
            switch ( filterOperator )
            {
                case SchemaFilterOperator.Greater:
                    return ( value1 > value2 );

                case SchemaFilterOperator.GreaterOrEqual:
                    return ( value1 >= value2 );

                case SchemaFilterOperator.Lesser:
                    return ( value1 < value2 );

                case SchemaFilterOperator.LesserOrEqual:
                    return ( value1 <= value2 );

                default:
                    return false;
            }
        }

        private bool IsMatchInt( SchemaFilterOperator filterOperator, long value1, long value2 )
        {
            switch ( filterOperator )
            {
                case SchemaFilterOperator.Greater:
                    return ( value1 > value2 );

                case SchemaFilterOperator.GreaterOrEqual:
                    return ( value1 >= value2 );

                case SchemaFilterOperator.Lesser:
                    return ( value1 < value2 );

                case SchemaFilterOperator.LesserOrEqual:
                    return ( value1 <= value2 );

                default:
                    return false;
            }
        }

        private bool IsMatchDouble( SchemaFilterOperator filterOperator, double value1, double value2 )
        {
            switch ( filterOperator )
            {
                case SchemaFilterOperator.Greater:
                    return ( value1 > value2 );

                case SchemaFilterOperator.GreaterOrEqual:
                    return ( value1 >= value2 );

                case SchemaFilterOperator.Lesser:
                    return ( value1 < value2 );

                case SchemaFilterOperator.LesserOrEqual:
                    return ( value1 <= value2 );

                default:
                    return false;
            }
        }

        private bool IsMatchDecimal( SchemaFilterOperator filterOperator, decimal value1, decimal value2 )
        {
            switch ( filterOperator )
            {
                case SchemaFilterOperator.Greater:
                    return ( value1 > value2 );

                case SchemaFilterOperator.GreaterOrEqual:
                    return ( value1 >= value2 );

                case SchemaFilterOperator.Lesser:
                    return ( value1 < value2 );

                case SchemaFilterOperator.LesserOrEqual:
                    return ( value1 <= value2 );

                default:
                    return false;
            }
        }

    }
}

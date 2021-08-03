using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using RestSchema;
using Xunit;

namespace schema_tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test()
        {
            var regex = @"^([a-zA-Z0-9_])+\[([a-zA-Z0-9_])(\,[a-zA-Z0-9_])*\](\,*([a-zA-Z0-9_])+\[([a-zA-Z0-9_])(\,[a-zA-Z0-9_])*\])*$";

            var matches = Regex.Matches( "_[name,email,teams],teams[id,attributes],teams.attributes[id,name]", regex );
        }
    }
}

using DBMS;
using Errors;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Test_EDB
{
    public class ColumnTest
    {
        [Theory]
        [InlineData("testName int")]
        [InlineData("testName inT")]
        [InlineData("testName INT")]
        [InlineData("testName IN")]
        [InlineData("testName int ")]
        [InlineData("testName  inT")]
        [InlineData("testName INT   ")]
        [InlineData("testName   IN  ")]
        [InlineData("testName")]
        [InlineData("testName   ")]
        public void ParseTypeOfColumnTest(string arguments)
        {
            try
            {
                var column = new Column();
                column.ParseType(arguments);
                Assert.Equal("testName", column.Name);
                Assert.Equal(1, column.Type);
            }
            catch(Exception ex)
            {
                Assert.Equal(typeof(ColumnError), ex.GetType());
            }
        }
    }
}

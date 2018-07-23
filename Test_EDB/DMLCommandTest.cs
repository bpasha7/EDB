using DML.Commands;
using Errors;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Test_EDB
{
    public class DMLCommandTest
    {
        [Theory]
        [InlineData("INSERT INTO test (Id, Name) VALUES (1, 'test strinG wiTH UP and down case.')")]
        [InlineData("INSERT INTO test (ID, name) VALUES (2, 'test strinG wiTH UP and down case.', '2345')")]
        [InlineData("INSERT INTO test (ID, Name) VALUES (2)")]
        public void ParsePatternedInsertCommandTest(string sqlQuery)
        {
            try
            {
                var cmd = new InsertCommand(sqlQuery);

                Assert.Equal("test", cmd.TableName);
                Assert.True(cmd.HasPattern);
                Assert.NotNull(cmd.Values);
                Assert.NotEmpty(cmd.Values);
            }
            catch (Exception ex)
            {
                Assert.Equal(typeof(InsertCommandParse), ex.GetType());
            }
        }

        [Theory]
        [InlineData("INSERT INTO test VALUES (1, 'test strinG wiTH UP and down case.')")]
        [InlineData("INSERT INTO test VALUES (2, 'test strinG wiTH UP and down case.', '2345')")]
        [InlineData("INSERT INTO test VALUES (2)")]
        public void ParseNotPatternedInsertCommandTest(string sqlQuery)
        {
            try
            {
                var cmd = new InsertCommand(sqlQuery);

                Assert.Equal("test", cmd.TableName);
                Assert.False(cmd.HasPattern);
                Assert.NotNull(cmd.Values);
                Assert.NotEmpty(cmd.Values);
            }
            catch (Exception ex)
            {
                Assert.Equal(typeof(InsertCommandParse), ex.GetType());
            }
        }
    }
}

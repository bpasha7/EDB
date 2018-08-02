using DML.Commands;
using Errors;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Theory]
        [InlineData("select * from test where a > c and c == 0 or c > 1")]
        [InlineData("select id from test where a > c and c == 0 or c > 1")]
        [InlineData("select id FROM test where a > c and c == 0 or c > 1")]
        [InlineData("select id FROM test where a > c or c == 0 or c > 1")]
        public void ParseConditionsSelectCommandTest(string sqlQuery)
        {
            var words = sqlQuery
                .ToLower()
                .Trim()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var cmd = new SelectCommand(words);
            Assert.Equal(3, cmd.Conditions.Count);
            Assert.Equal(2, cmd.ConditionsOperators.Count);
        }

        [Theory]
        [InlineData("a > 0", 100)]
        [InlineData("a >= 0", 0)]
        [InlineData("a = 0", 0)]
        [InlineData("a <= 0", -100)]
        [InlineData("a <> 0", -100)]
        public void OperateIntConditionsTest(string sqlQuery, int val)
        {
            var words = parseBySpaces(sqlQuery).ToList();
            var condition = new Condition(words);
            var res = condition.Operate("a", val);
            Assert.True(res);
            res = condition.Operate("A", val);
            Assert.True(res);
        }

        [Theory]
        [InlineData("a = 'test'", "test")]
        [InlineData("a <> 'test'", "test1")]
        public void OperateStringConditionsTest(string sqlQuery, string val)
        {
            var words = parseBySpaces(sqlQuery).ToList();
            var condition = new Condition(words);
            var res = condition.Operate("a", val);
            Assert.True(res);
            res = condition.Operate("A", val);
            Assert.True(res);
        }

        private string[] parseBySpaces(string sentence)
        {
            return
                 sentence
                .ToLower()
                .Trim()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        #region GetingWordIndexFromArray
        [Theory]
        [InlineData("select * from test")]
        [InlineData("select id from test")]
        [InlineData("select id FROM test")]
        [InlineData("select id From test")]
        [InlineData("SELECT id From test")]
        [InlineData("select id, text from test")]
        public void GetingWordIndexFromArray(string sqlQuery)
        {
            var words = sqlQuery.Split(new char[] { ' ' });
            var res = getIndexWord(words, "from");
            Assert.NotEqual(-1, res);
            res = getIndexWord(words, "extraword");
            Assert.Equal(-1, res);
        }

        private int getIndexWord(string[] words, string word)
        {
            var i = words
                .Select((text, index) => new { Text = text, Index = index })
                .Where(w => w.Text.ToLower() == word)
                .SingleOrDefault();
            return i == null ? -1 : i.Index;
        }
        #endregion
    }
}

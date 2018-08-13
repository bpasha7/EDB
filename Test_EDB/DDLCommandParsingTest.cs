using DDL.Commands;
using Errors;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Test_EDB
{
    public class DDLCommandParsingTest
    {
        [Fact]
        public void ParseCreateDenseIndexCommandTest()
        {
            var names = new string[] { "NUM_IND", "num_ind", "numInd", "noTableName", "", "", "test", "onCol" };
            try
            {
                for (int i = 0; i < names.Length; i++)
                {
                    var rnd = new Random();
                    var col = names[rnd.Next(0, names.Length - 1)];
                    var table = names[rnd.Next(0, names.Length - 1)];
                    var index = names[rnd.Next(0, names.Length - 1)];
                    var query = $"CREATE INDEX {index} ON {table} ({col})";
                    var cmd = new CreateIndexCommands(query);

                    Assert.Equal(table, cmd.TableName);
                    Assert.Equal(index, cmd.IndexName);
                    Assert.Equal(col, cmd.ColumnName);
                    Assert.Equal(IndexType.DenseIndex, cmd.IndexType);
                }
               
            }
            catch (Exception ex)
            {
                Assert.Equal(typeof(CreateIndexParse), ex.GetType());
            }

        }
    }
}

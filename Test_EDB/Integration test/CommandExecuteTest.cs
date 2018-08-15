using DBMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Test_EDB.Integration_test
{
    public class CommandExecuteTest
    {
        [Fact]
        public void InsertTest()
        {
            string path = "C:\\temp\\", name = "test";
            var db = new Database(path, name);
            for (int i = 0; i < 1000; i++)
            {
                var rnd = new Random();
                var query = $"insert into stud Values ({rnd.Next(-1000, 1000)}, '{randomString(10, rnd)}', {rnd.Next(0, 1)})";
                db.InsertIntoTable(query);
            }
        }

        [Fact]
        public void InsertIntoIndexTableTest()
        {
            string path = "C:\\temp\\", name = "test";
            var db = new Database(path, name);
            for (int i = 0; i < 1000; i++)
            {
                var rnd = new Random();
                var query = $"insert into test Values ({rnd.Next(-1000, 1000)}, '{randomString(10, rnd)}', {rnd.Next(0, 1)})";
                db.InsertIntoTable(query);
            }
        }

        private static string randomString(int length, Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}

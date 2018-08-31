using BinaryFileStream;
using DBMS;
using DBMS.Indexes;
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
        public void CreateAndInsertTest()
        {
            string path = "C:\\temp\\", name = "test";
            var db = new Database(path, name);
            //var createQuery = $"create table stud (Id int, Name varchar(100), Flag bit, Date datetime)";
            //db.CreateTable(createQuery);
            for (int i = 0; i < 1000; i++)
            {
                var rnd = new Random();
                var query = $"insert into stud Values ({rnd.Next(-1000, 1000)}, '{randomString(10, rnd)}', {rnd.Next(0, 1)}, '{RandomDay(rnd):G}')";
                db.InsertIntoTable(query);
            }
        }

        [Fact]
        public void CreateAndInsertWithIndexTest()
        {
            string path = "C:\\temp\\", name = "test";
            var db = new Database(path, name);
            //var createQuery = $"create table studi (Id int index IND_1, Name varchar(100), Flag bit, Date datetime)";
            //db.CreateTable(createQuery);
            for (int i = 0; i < 1000; i++)
            {
                var rnd = new Random();
                var query = $"insert into studi Values ({rnd.Next(-1000, 1000)}, '{randomString(10, rnd)}', {rnd.Next(0, 1)}, '{RandomDay(rnd):G}')";
                db.InsertIntoTable(query);
            }
        }

        DateTime RandomDay(Random rnd)
        {
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(rnd.Next(range));
        }

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
            //var db2 = new Database(path, name+"1");
            for (int i = 0; i < 1000; i++)
            {
                var rnd = new Random();
                var query = $"insert into test1 Values ({rnd.Next(-1000, 1000)}, '{randomString(10, rnd)}', {rnd.Next(0, 1)})";
                db.InsertIntoTable(query);

            }
            //for (int i = 0; i < 1000; i++)
            //{
            //    var rnd = new Random();
            //    var query = $"insert into test1 Values ({rnd.Next(-1000, 1000)}, '{randomString(10, rnd)}', {rnd.Next(0, 1)})";
            //    db2.InsertIntoTable(query);
            //}
        }

        [Fact]
        public void CheckSortingIndexTableTest()
        {
            string path = "C:\\temp\\", name = "test";

            var fileStream = new FileStream(path, "test", $"tt-IND_1", FileType.Index);
            var indexes = new List<int>();
            fileStream?.Create();
            fileStream?.Open();
            fileStream?.SetPosition(0);
            fileStream?.WriteInt(0);
            var rnd = new Random();
            var t = new int[] { 5, 10, 1, 6, 4, 1, 1, 5, 5, 5 };
            for (int i = 0; i < t.Length; i++)
            {
                //var n = rnd.Next(0, 50);

                var p = findPositiontoInsertIndex(fileStream, t[i]);
                var temp = fileStream.CutToEnd(p);
                fileStream.SetPosition(p);
                fileStream?.WriteInt(t[i]);

                if (temp != null)
                    fileStream.WriteBytes(temp);

                fileStream?.SetPosition(0);
                var cnt = fileStream.ReadInt();
                fileStream?.SetPosition(0);
                fileStream?.WriteInt(++cnt);
            }

            fileStream?.SetPosition(0);

            var tt = findPositiontoInsertIndex(fileStream, 1);

            //var count = fileStream?.ReadInt();
            //for (int i = 0; i < count; i++)
            //{
            //    var val = fileStream.ReadInt();
            //    //var pos = fileStream.ReadInt();
            //    //var idx = new DenseIndex(val, pos);
            //    indexes.Add(val);
            //}
            fileStream?.Close();
            fileStream?.Delete();
        }

        private int findPositiontoInsertIndex(FileStream fileStream, int value)
        {
            fileStream.Open();
            // read indexes count
            var count = fileStream.ReadInt();
            int first = 0, last = count;
            int intValue = 0, curPos = 4;
            while (first < last)
            {
                var mid = first + (last - first) / 2;
                curPos = 4 + mid * 4;
                fileStream.SetPosition(curPos);
                // read value
                intValue = fileStream.ReadInt();
                // save pos and min diference value
                if (value <= intValue)
                {
                    last = mid;
                }
                else
                {
                    first = mid + 1;
                }
            }
            // * 4 size of data int
            return last * 4 + 4;
        }


        private static string randomString(int length, Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}

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
            var t = new int[] { 5, 10, 1, 6, 4 };
            for (int i = 0; i < 20; i++)
            {
                var n = rnd.Next(0, 50);

                var p = findPositiontoInsertIndex(fileStream, n);
                var temp = fileStream.CutToEnd(p);
                fileStream.SetPosition(p);
                fileStream?.WriteInt(n);

                if (temp != null)
                    fileStream.WriteBytes(temp);

                fileStream?.SetPosition(0);
                var cnt = fileStream.ReadInt();
                fileStream?.SetPosition(0);
                fileStream?.WriteInt(++cnt);
            }

            fileStream?.SetPosition(0);

            var count = fileStream?.ReadInt();
            for (int i = 0; i < count; i++)
            {
                var val = fileStream.ReadInt();
                //var pos = fileStream.ReadInt();
                //var idx = new DenseIndex(val, pos);
                indexes.Add(val);
            }
            fileStream?.Close();
            fileStream?.Delete();
        }

        private int findPositiontoInsertIndex(FileStream fileStream, int value)
        {
            fileStream.Open();
            // read indexes count
            var count = fileStream.ReadInt();
            // return first position if no indexes

            int first = 0, last = count;

            //bool right = false, left = false;

            //int min = Int32.MaxValue, minPos = count, 
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
            //curPos = 4 + last * 4;
            //intValue = fileStream.ReadInt();
            //if (intValue == value)
            //{
            //    minPos = curPos;
            //}
            //return minPos;
        }


        private static string randomString(int length, Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}

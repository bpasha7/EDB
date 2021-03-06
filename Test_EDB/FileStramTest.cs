﻿using BinaryFileStream;
using Errors;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Test_EDB
{
    public class FileStramTest
    {
        [Fact]
        public void WritingReadingFileTest()
        {
            //var path = "D:\\workspaces\\EDB\\EDB\\bin\\Debug\\netcoreapp2.0\\";
            var path = "C:\\temp\\";
            var db = "test";
            var table = "test";
            var fs = new FileStream(path, db, table);
            fs.Create();

            fs.Open();
            fs.SetPosition(0);

            byte b = 4;
            int i = 10001;
            string s = "12345";

            fs.WriteByte(b);
            fs.WriteInt(i);
            fs.WriteText(s);
            fs.Close();

            fs.Open();
            fs.SetPosition(0);

            var res_b = fs.ReadByte();
            var res_i = fs.ReadInt();
            var res_s = fs.ReadText(s.Length);
            Assert.Equal(b, res_b);
            Assert.Equal(i, res_i);
            Assert.Equal(s, res_s);

            fs.Close();

            fs.Delete();
        }

        [Fact]
        public void RewriteDataIntoFileTest()
        {
            var path = "C:\\temp\\";
            var db = "test";
            var table = "testR";
            var fs = new FileStream(path, db, table);
            fs.Create();
            fs.Open();
            fs.SetPosition(0);
            //fs.WriteInt(10002);
            fs.WriteText("|123456780|");

            fs.SetPosition(0);
            //var res1 = fs.rea();

            //Assert.Equal(10002, res1);

            fs.Close();
            fs.Open();
            fs.SetPosition(0);
            //fs.WriteInt(10003);
            fs.WriteText("|0987654321|");
            fs.WriteInt(1000);
            fs.SetPosition(0);
            //res1 = fs.ReadInt();
            //Assert.Equal(10003, res1);

            fs.Close();
            //fs.Delete();

        }

        [Fact]
        public void AccessBlockedFilesTest()
        {
            //var path = "D:\\workspaces\\EDB\\EDB\\bin\\Debug\\netcoreapp2.0\\";
            var path = "C:\\temp\\";
            var db = "test";
            var table = "test";
            var fs = new FileStream(path, db, table);
            fs.Create();

            fs.Open();

            

            try
            {
                var fs3 = new FileStream(path, db, table);
                fs3.Open();
                Assert.True(false);
            }
            catch(Exception ex)
            {
                Assert.Equal(typeof(FileSystemError), ex.GetType());
            }

            fs.Close();

            var fs2 = new FileStream(path, db, table);
            fs2.Open();
            fs2.Close();
            fs2.Delete();
        }
    }
}

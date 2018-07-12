using Program;
using System;
using Xunit;


namespace Test_EDB
{
    public class UnitTestSettings
    {
        [Fact]
        public void Test1()
        {
           var dbms = Program.Program.Domain;

            string authors = dbms.GetAuthors();
            Assert.NotEmpty(authors);
        }
    }
}

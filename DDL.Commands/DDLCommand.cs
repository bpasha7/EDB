using System;
using System.Collections.Generic;

namespace DDL.Commands
{
    public class DDLCommand
    {
        public string CommandText { get; set; }
        public string[] Words { get; set; }

        public DDLCommand(string comandLine)
        {
            Words = comandLine
                .Trim()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
        protected void ParseWords(string query)
        {

        }
    }
}

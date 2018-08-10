using Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDL.Commands
{
    public class CreateDatabaseCommand : DDLCommand
    {
        /// <summary>
        /// Database name to create
        /// </summary>
        public string DatebaseName { get; private set; }

        public CreateDatabaseCommand(string[] words) : base(words)
        {
            if (words.Length != 3)
                throw new CreateDatabaseParse($"Command has incorrect signature.");
            DatebaseName = words[2];
        }
        protected override void ParseQuery()
        {
            throw new Error("ParseQuery method is not implemented.");
        }
    }

    public class DropDatabaseCommand : DDLCommand
    {
        /// <summary>
        /// Database name to drop
        /// </summary>
        public string DatebaseName { get; private set; }

        public DropDatabaseCommand(string[] words) : base(words)
        {
            if (words.Length != 3)
                throw new CreateDatabaseParse($"Command has incorrect signature.");
            DatebaseName = words[2];
        }
        protected override void ParseQuery()
        {
            throw new Error("ParseQuery method is not implemented.");
        }
    }
}

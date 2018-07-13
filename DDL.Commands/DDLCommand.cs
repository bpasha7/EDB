using System;
using System.Collections.Generic;

namespace DDL.Commands
{
    public abstract class DDLCommand
    {
        protected string CommandText { get; set; }

        public DDLCommand(string query)
        {
            CommandText = query;
        }
        protected abstract void ParseQuery();
    }
}

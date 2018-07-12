using System;

namespace DDL.Commands
{
    public abstract class DDLCommand
    {
        public string CommandText { get; set; }
        protected abstract void ParseWords();
    }
}

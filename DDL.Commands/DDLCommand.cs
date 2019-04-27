using BinaryFileStream;
using System;
using System.Collections.Generic;

namespace DDL.Commands
{
    public abstract class DDLCommand
    {
        /// <summary>
        /// SQL command text
        /// </summary>
        protected string CommandText { get; set; }

        /// <summary>
        /// DatabaseName
        /// </summary>
        public string DatabaseName { get; set; }

        public DDLCommand(string query)
        {
            CommandText = query;
        }

        public DDLCommand(string[] words)
        {
        }
        protected abstract void ParseQuery();
        //public abstract void Excecute(FileStream fileStream);
    }
}

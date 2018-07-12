using System;
using System.Collections.Generic;
using System.Text;

namespace DDL.Commands
{
    public sealed class CreateTableCommand //: DDLCommand
    {
        /// <summary>
        /// Table name
        /// </summary>
        public string TableName { get; set; }

        protected void ParseScheme(string query)
        {

        }

    }
}

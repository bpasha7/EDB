using System;
using System.Collections.Generic;
using System.Text;

namespace DBMS
{
    public class Table
    {
        /// <summary>
        /// Table Name
        /// </summary>
        public readonly string Name;
        public Table(string dataBaseName, string tableName)
        {
            Name = tableName;
        }

        public void Create()
        {

        }
    }
}

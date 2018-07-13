using BinaryFileStream;
using DDL.Commands;
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
        public readonly string Database;
        public Table(string dataBaseName, string tableName)
        {
            Name = tableName;
            Database = dataBaseName;
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="cmd"></param>
        //public void Create(CreateTableCommand cmd, FileStream fileStream)
        //{

        //}
    }
}

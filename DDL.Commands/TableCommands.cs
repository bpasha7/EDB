using BinaryFileStream;
using Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDL.Commands
{
    public sealed class CreateTableCommand : DDLCommand
    {
        /// <summary>
        /// 
        /// </summary>
        public string[] Columns { get; private set; }
        /// <summary>
        /// Table name
        /// </summary>
        public string TableName { get; private set; }

        public CreateTableCommand(string query) : base(query)
        {
            ParseQuery();
        }

        protected override void ParseQuery()
        {
            // find index of open bracket in create query, where are descriptions of columns
            var openBacket = CommandText.IndexOf('(');
            // find index of close bracket in create query, where are descriptions of columns
            var closeBacket = CommandText.LastIndexOf(')');
            if(openBacket == -1 || closeBacket == -1 || openBacket == closeBacket)
                throw new CreateTableParse($"Command has incorrect signature. Check brackets.");
            // get command words, words befor descriptions of columns
            var commandWords = CommandText?
                .Substring(0, openBacket)
                .Trim()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (commandWords.Length != 3)
                throw new CreateTableParse($"Command has incorrect signature. Check table name.");
            // set table name and database name if format [dbname].[tableName]
            var tableAndDatabaseNames = commandWords[2].Split('.');
            if (tableAndDatabaseNames.Length == 2)
            {
                DatabaseName = tableAndDatabaseNames[0];
                TableName = tableAndDatabaseNames[1];
            }
            else
            {
                TableName = tableAndDatabaseNames[0];
            }
            // 
            Columns = CommandText?
                .Substring(openBacket + 1, closeBacket - openBacket - 1)
                .Trim()
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if(Columns.Length == 0)
                throw new CreateTableParse($"Command does not have new columns description.");
        }
    }
}

using Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDL.Commands
{
    public enum IndexType { DenseIndex, BTreeIndex };
    public class CreateIndexCommands : DDLCommand
    {
        //private readonly string[] _words;
        public CreateIndexCommands(string query) : base(query)
        {
            //_words = words;
            ParseQuery();
        }

        /// <summary>
        /// Column name
        /// </summary>
        public string ColumnName { get; private set; }
        /// <summary>
        /// Index name
        /// </summary>
        public string IndexName { get; private set; }
        /// <summary>
        /// Table name
        /// </summary>
        public string TableName { get; private set; }
        /// <summary>
        /// Index Type
        /// </summary>
        public IndexType IndexType { get; private set; }


        /// <summary>
        /// EXAMPLE QUERY
        /// CREATE INDEX idx_lastname
        /// ON Persons(LastName);
        /// </summary>
        protected override void ParseQuery()
        {
            // find index of open bracket in create query, where are descriptions of columns
            var openBacket = CommandText.IndexOf('(');
            // find index of close bracket in create query, where are descriptions of columns
            var closeBacket = CommandText.LastIndexOf(')');
            if (openBacket == -1 || closeBacket == -1 || openBacket == closeBacket)
                throw new CreateIndexParse($"Command has incorrect signature. Check brackets.");
            // get columns name
            var commandWords = CommandText?
                .Substring(0, openBacket)
                .Trim()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if(commandWords[0].ToLower() != "create")
                throw new CreateIndexParse($"Command has incorrect signature.");
            if (commandWords.Length == 5)
            {
                if (commandWords[2].ToLower() != "index")
                    throw new CreateIndexParse($"Command has incorrect signature.");
                // find 'ON' into query
                if (commandWords[3].ToLower() != "on")
                    throw new CreateIndexParse($"Command has incorrect signature.");
                TableName = commandWords[4];
                IndexName = commandWords[2];
                IndexType = IndexType.DenseIndex;
            }
            else if(commandWords.Length == 6)
            {
                if (commandWords[3].ToLower() != "index")
                    throw new CreateIndexParse($"Command has incorrect signature.");
                throw new CreateIndexParse($"Command has not released.");
            }
            else
                throw new CreateIndexParse($"Command has incorrect signature.");

            var columnName = CommandText?
                .Substring(openBacket + 1, closeBacket - 1 - openBacket);
            if (columnName.Length == 0)
                throw new CreateIndexParse($"Command has incorrect signature. Can not parse columns name.");

            TableName = commandWords[2].Trim();
        }
    }
}

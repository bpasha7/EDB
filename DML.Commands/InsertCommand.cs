using Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace DML.Commands
{
    /// <summary>
    /// Insert command class
    /// </summary>
    public class InsertCommand : DMLCommand
    {
        /// <summary>
        /// Dictionary values
        /// </summary>
        public Dictionary<string, string> Values { get; set; }
        /// <summary>
        /// True - has pattern for insert
        /// </summary>
        public bool HasPattern { get; set; }
        public InsertCommand(string query)
        {
            parse(query);
        }
        /// <summary>
        /// Parse table name and values from sql script
        /// </summary>
        /// <param name="query"></param>
        private void parse(string query)
        {
            // cut arguments from string query
            var args = cutLastBracketsArguments(ref query);
            // cut pattern from query if exist
            var pattern = cutLastBracketsArguments(ref query);
            // parse values and pattern
            Values = new Dictionary<string, string>();
            // use pattern
            if (pattern != null)
            {
                if (pattern.Length != args.Length)
                    throw new InsertCommandParse($"Number of arguments does no match the pattern into query.");
                HasPattern = true;
                for (int i = 0; i < pattern.Length; i++)
                {
                    var fieldName = pattern[i].Trim();
                    if (Values.ContainsKey(fieldName))
                        throw new InsertCommandParse($"Field [{fieldName}] contains more than once into query.");
                    Values.Add(fieldName, args[i]);
                }
            }
            // just values
            else
            {
                HasPattern = false;
                for (int i = 0; i < args.Length; i++)
                {
                    var value = args[i].Trim();
                    Values.Add($"p{i + 1}", value);
                }
            }
            var splitedWords = query
                .Trim()
                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            #region Syntax error into query signature
            if (splitedWords.Length != 4)
            {
                if(splitedWords[splitedWords.Length - 1].ToLower() == "values" && splitedWords.Length == 3)
                {
                    throw new InsertCommandParse($"Table name not found into query.");
                }
                else
                    throw new InsertCommandParse($"Syntaxis error into query.");
            }
            #endregion
            // get table name
            //TableName = splitedWords[2];
            // set table name and database name if format [dbname].[tableName]
            var tableAndDatabaseNames = splitedWords[2].Split('.');
            if (tableAndDatabaseNames.Length == 2)
            {
                DatabaseName = tableAndDatabaseNames[0];
                TableName = tableAndDatabaseNames[1];
            }
            else
            {
                TableName = tableAndDatabaseNames[0];
            }
        }

    }
}

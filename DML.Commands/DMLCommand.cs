using Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace DML.Commands
{
    public class DMLCommand
    {
        /// <summary>
        /// Table Name
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// DatabaseName
        /// </summary>
        public string DatabaseName { get; set; }

        protected string[] cutLastBracketsArguments(ref string query)
        {
            // find index of open bracket in query
            var openBacket = query.LastIndexOf('(');
            // find index of close bracket in query
            var closeBacket = query.LastIndexOf(')');
            if (openBacket == -1 || closeBacket == -1 || openBacket == closeBacket)
                return null;
                //throw new DMLCommandError($"Command has incorrect signature. Check brackets.");
            // get command words, words befor descriptions of columns
            var agrs = query?
                .Substring(openBacket + 1, closeBacket - openBacket - 1)
                .Trim()
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            query = query.Remove(openBacket, closeBacket - openBacket + 1);
            return agrs;

        }
    }
}

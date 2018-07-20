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
            var args = cutLastBracketsArguments(ref query);
            var pattern = cutLastBracketsArguments(ref query);
            if(pattern == null)
            {
                HasPattern = false;

            }
        }
        
    }
}

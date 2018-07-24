using Errors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DML.Commands
{
    public class SelectCommand : DMLCommand
    {
        public bool AllColumns { get; set; }
        public IList<string> ColumnsName { get; set; }

        public SelectCommand(string[] words)
        {
            ColumnsName = new List<string>();  
            parse(words);
        }
        private void parse(string[] words)
        {
            var fromIndex = getIndexWord(words, "from");
            if(fromIndex + 1 >= words.Length)
                throw new SelectCommandParse($"Not found table name into query.");
            // set table name
            TableName = words[fromIndex + 1];
            // set flag if all columns
            if (words[1] == "*" && fromIndex == 2)
                AllColumns = true;
            // if there are columns name by commas
            else if(fromIndex > 2)
            {
                for (int i = 1; i < fromIndex; i++)
                {
                    ColumnsName.Add(words[i].Replace(",", "").Trim());
                }
            }
            else
            {
                throw new SelectCommandParse($"Not found columns into query.");
            }
        }
        /// <summary>
        /// Get index of word in query
        /// </summary>
        /// <param name="words">Words</param>
        /// <param name="word">word to find</param>
        /// <returns>index of word or -1 if not exist</returns>
        private int getIndexWord(string[] words, string word)
        {
            var i = words
                .Select((text, index) => new { Text = text, Index = index })
                .Where(w => w.Text.ToLower() == word)
                .SingleOrDefault();
            if (i == null)
                throw new SelectCommandParse($"Word {word} not found in query.");
            return i.Index;
        }
    }
}

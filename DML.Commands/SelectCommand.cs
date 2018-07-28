using Errors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DML.Commands
{
    public class SelectCommand : DMLCommand
    {
        /// <summary>
        /// Flag if all columns
        /// </summary>
        public bool AllColumns { get; set; }
        /// <summary>
        /// Columns to return
        /// </summary>
        public IList<string> ColumnsName { get; }
        /// <summary>
        /// Conditions into SELECT query
        /// </summary>
        public IList<Condition> Conditions { get; }
        /// <summary>
        /// Conditions operators
        /// </summary>
        public IList<string> ConditionsOperators { get; }
        /// Fields ordering dictionary
        /// </summary>
        public Dictionary<string, bool> OrderBy { get; }

        public SelectCommand(string[] words)
        {
            ColumnsName = new List<string>();
            ConditionsOperators = new List<string>();
            OrderBy = new Dictionary<string, bool>();
            Conditions = new List<Condition>();
            parse(words);
        }
        private void parse(string[] words)
        {
            var fromIndex = getIndexWord(words, "from");
            if (fromIndex + 1 >= words.Length)
                throw new SelectCommandParse($"Not found table name into query.");
            // set table name
            TableName = words[fromIndex + 1];
            // set flag if all columns
            if (words[1] == "*" && fromIndex == 2)
                AllColumns = true;
            // if there are columns name by commas
            else if (fromIndex >= 2)
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


            var orderIndex = -1;
            var whereIndex = -1;
            try
            {
                orderIndex = getIndexWord(words, "order");
            }
            catch (SelectCommandParse error)
            {
                // skip if no order by clause
            }
            try
            {
                whereIndex = getIndexWord(words, "where");
            }
            catch (SelectCommandParse error)
            {
                // skip if no order by clause
            }
            // parse conditions
            if (whereIndex != -1)
            {
                var indexTo = orderIndex == -1 ? words.Length : orderIndex;
                var conditionParts = new List<string>();
                for (int i = whereIndex + 1; i < indexTo; i++)
                {
                    if (words[i].ToLower() == "and" || words[i].ToLower() == "or")
                    {
                        ConditionsOperators.Add(words[i].ToLower());
                        Conditions.Add(new Condition(conditionParts));
                        conditionParts = new List<string>();
                    }
                    else
                    {
                        conditionParts.Add(words[i]);
                    }
                }
                Conditions.Add(new Condition(conditionParts));
            }
            // do not have order by clause into query
            if (orderIndex == -1)
            {
                return;
            }
            // if ... no continue for order by clause
            if (words.Length - orderIndex < 2)
            {
                throw new SelectCommandParse($"Not found ORDER BY columns into query.");
            }
            // check BY after ORDER
            if (words[orderIndex + 1].ToLower() != "by")
            {
                throw new SelectCommandParse($"Not found 'BY' after ORDER clause into query.");
            }
            // parse order by clause
            for (int i = orderIndex + 2; i < words.Length; i++)
            {
                string item = words[i]
                    .Replace(",", "")
                    .Trim();
                // checking double column
                if (OrderBy.ContainsKey(item))
                {
                    throw new SelectCommandParse($"Double column [{item}] for ORDER clause into query.");
                }
                bool asc = true;
                if (i + 1 != words.Length)
                {
                    string type = words[i + 1]
                        .Replace(",", "")
                        .ToLower()
                        .Trim();
                    if (type == "desc")
                        asc = false;
                    i++;
                }
                OrderBy.Add(item, asc);
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

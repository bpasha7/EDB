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
            var openBacket = query.IndexOf('(');
            // find index of close bracket in query
            var closeBacket = query.LastIndexOf(')');
            if (openBacket == -1 || closeBacket == -1 || openBacket == closeBacket)
                return null;
            //throw new DMLCommandError($"Command has incorrect signature. Check brackets.");
            // get command words, words befor descriptions of columns
            var agrsString = query?
                .Substring(openBacket + 1, closeBacket - openBacket - 1)
                .Trim();
            var args = new List<string>();
            var sb = new StringBuilder();
            var inString = false;
            //var nextIsRequired = false;
            for (int i = 0; i < agrsString.Length; i++)
            {
                // add argument and next iterration
                if (agrsString[i] == ',' && !inString)
                {
                    args.Add(sb.ToString());
                    sb.Clear();
                    continue;
                }
                //if single quote
                if (agrsString[i] == '\'')
                {
                    if (inString == false)
                        inString = true;
                    if (i + 1 == agrsString.Length || agrsString[i + 1] == ',')
                    {
                        inString = !inString;
                    }
                }
                sb.Append(agrsString[i]);
            }
            //foreach (var item in agrsString)
            //{
            //    if (nextIsRequired)
            //    {
            //        sb.Append(item);
            //        nextIsRequired = !nextIsRequired;
            //        continue;
            //    }
            //    //ignore 
            //    if (item == '\\' )
            //    {
            //        nextIsRequired = true;
            //        continue;
            //    }
            //    // add argument and next iterration
            //    if (item == ',' && !inString)
            //    {
            //        args.Add(sb.ToString());
            //        sb.Clear();
            //        continue;
            //    }
            //    //if single quote
            //    if(item == '\'')
            //    {
            //        inString = !inString;
            //    }
            //    sb.Append(item);
            //}

            //check string builder

            if (sb.Length > 0)
                args.Add(sb.ToString());
            sb.Clear();

            //.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            query = query.Remove(openBacket, closeBacket - openBacket + 1);
            return args.ToArray();

        }
    }
}

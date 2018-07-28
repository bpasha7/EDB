using System;
using System.Collections.Generic;
using System.Text;

namespace DML.Commands
{
    public class Condition
    {
        public IList<string> Operands { get; }
        public string Operator { get; }
        public Condition(string[] condition)
        {
            Operands = new List<string>();

        }
    }
}

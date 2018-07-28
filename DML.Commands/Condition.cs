using Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace DML.Commands
{
    public class Condition
    {
        public IList<string> Operands { get; }
        public string Operator { get; }
        public Condition(IList<string> conditionParts)
        {
            Operands = new List<string>();
            if(conditionParts.Count != 3)
            {
                throw new ConditionParse($"Conditions {conditionParts} has not correct signature.");
            }
            Operands.Add(conditionParts[0]);
            Operands.Add(conditionParts[2]);
            Operator = conditionParts[1];
        }
    }
}

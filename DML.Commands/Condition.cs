using Errors;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (conditionParts.Count != 3)
            {
                throw new ConditionParse($"Conditions {conditionParts} has not correct signature.");
            }
            Operands.Add(TruncQuetes(conditionParts[0]));
            Operands.Add(TruncQuetes(conditionParts[2]));
            Operator = conditionParts[1];
        }
        /// <summary>
        /// Compare two numbers
        /// </summary>
        /// <param name="operand1">First number</param>
        /// <param name="operand2">Second number</param>
        /// <returns>Result of comparison</returns>
        private bool operate(int operand1, int operand2)
        {
            switch (Operator)
            {
                case ">":
                    return operand1 > operand2;
                case ">=":
                    return operand1 >= operand2;
                case "<":
                    return operand1 < operand2;
                case "<=":
                    return operand1 <= operand2;
                case "=":
                    return operand1 == operand2;
                case "<>":
                    return operand1 != operand2;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Compare two string
        /// </summary>
        /// <param name="operand1">First string</param>
        /// <param name="operand2">Second string</param>
        /// <returns>Result of comparison</returns>
        private bool operate(string operand1, string operand2)
        {
            switch (Operator)
            {
                case "=":
                    return operand1 == operand2;
                case "<>":
                    return operand1 != operand2;
                default:
                    return false;
            }
        }
        private int? getOperandIndex(string field)
        {
            var fieldOperand = Operands.SingleOrDefault(op => op.ToLower() == field.ToLower());
            if (fieldOperand == null)
                return null;
            else
                return Operands.IndexOf(fieldOperand);
        }
        /// <summary>
        /// Truncate quetes for field name or string value
        /// </summary>
        /// <param name="val">Field name or string</param>
        private string TruncQuetes(string val)
        {
            if (val.Length > 2 && (val[0] != val[val.Length - 1]))
                return val.ToLower();
            var res = val;
            if (res[res.Length - 1] == '\'' || res[res.Length - 1] == '`')
                res = res.Remove(res.Length - 1, 1);
            if (res[0] == '\'' || res[0] == '`')
                res = res.Remove(0, 1);
            return res;
        }

        /// <summary>
        /// Operate condition for integer field
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="val">Field value</param>
        /// <returns>Result of operation</returns>
        public bool Operate(string fieldName, int val)
        {
            // find index of field name into operands list if exist
            var fieldOperandIndex = getOperandIndex(fieldName);
            if (fieldOperandIndex == null)
                return true;
            // set index of value operator
            var secondOperandIndex = fieldOperandIndex == 0 ? 1 : 0;
            // convert value
            var nextOperandValue = Convert.ToInt32(Operands[secondOperandIndex]);
            // compare
            if ((int)fieldOperandIndex < secondOperandIndex)
            {
                return operate(val, nextOperandValue);
            }
            else
            {
                return operate(nextOperandValue, val);
            }
        }
        public bool Operate(string fieldName, string val)
        {
            // throw new Exception("String compare is not released!");
            // find index of field name into operands list if exist
            var fieldOperandIndex = getOperandIndex(fieldName);
            if (fieldOperandIndex == null)
                return true;
            // set index of value operator
            var secondOperandIndex = fieldOperandIndex == 0 ? 1 : 0;
            // convert value
            var nextOperandValue = Operands[secondOperandIndex];
            // compare
            if ((int)fieldOperandIndex < secondOperandIndex)
                return operate(val, nextOperandValue);
            else
                return operate(nextOperandValue, val);
        }
    }
}

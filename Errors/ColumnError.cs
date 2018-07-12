using System;
using System.Collections.Generic;
using System.Text;

namespace Errors
{
    /// <summary>
    /// Errors depended with column of table
    /// </summary>
    public sealed class ColumnError : Error
    {
        public ColumnError(string message, int code = 0) : base(message, code)
        {
        }
    }
}

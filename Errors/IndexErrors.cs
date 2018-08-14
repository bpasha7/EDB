using System;
using System.Collections.Generic;
using System.Text;

namespace Errors
{
    /// <summary>
    /// Errors depended with Index of table
    /// </summary>
    public sealed class IndexError : Error
    {
        public IndexError(string message, int code = 0) : base(message, code)
        {
        }
    }
}

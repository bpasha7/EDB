using System;
using System.Collections.Generic;
using System.Text;

namespace Errors
{
    public abstract class DLLCommandError : Error
    {
        public DLLCommandError(string message, int code = 0) : base(message, code)
        {
        }
    }

    public class CreateTableParse : Error
    {
        public CreateTableParse(string message, int code = 0) : base(message, code)
        {
        }
    }
}

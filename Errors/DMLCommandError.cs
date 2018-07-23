using System;
using System.Collections.Generic;
using System.Text;

namespace Errors
{
    public class DMLCommandError : Error
    {
        public DMLCommandError(string message, int code = 0) : base(message, code)
        {
        }
    }

    public class InsertCommandParse : DMLCommandError
    {
        public InsertCommandParse(string message, int code = 0) : base(message, code)
        {
        }
    }

    public class InsertCommandExcecute : DMLCommandError
    {
        public InsertCommandExcecute(string message, int code = 0) : base(message, code)
        {
        }
    }
}

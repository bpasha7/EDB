using System;
using System.Collections.Generic;
using System.Text;

namespace Errors
{
    public abstract class DDLCommandError : Error
    {
        public DDLCommandError(string message, int code = 0) : base(message, code)
        {
        }
    }

    public class CreateTableParse : Error
    {
        public CreateTableParse(string message, int code = 0) : base(message, code)
        {
        }
    }

    public class CreateIndexParse : Error
    {
        public CreateIndexParse(string message, int code = 0) : base(message, code)
        {
        }
    }

    public class CreateDatabaseParse : Error
    {
        public CreateDatabaseParse(string message, int code = 0) : base(message, code)
        {
        }
    }
}

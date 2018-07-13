using System;
using System.Collections.Generic;
using System.Text;

namespace Errors
{
    public sealed class FileSystemError : Error
    {
        public FileSystemError(string message, int code = 0) : base(message, code)
        {
        }
    }
}

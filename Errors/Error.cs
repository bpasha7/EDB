using System;

namespace Errors
{
    /// <summary>
    /// Inner error class
    /// </summary>
    public class Error : Exception
    {
        private string _message;
        private int _code;
        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get => _message; set => _message = value; }
        /// <summary>
        /// Error code
        /// </summary>
        public int Code { get => _code; set => _code = value; }
        /// <summary>
        /// Constructor of error
        /// </summary>
        /// <param name="message">Error text</param>
        /// <param name="code"> Error code</param>
        public Error(string message, int code = 0)
        {
            _message = message;
            _code = code;
        }
        public override string ToString()
        {
            var code = _code != 0 ? $"{_code}: " : "";
            return $"{code}{_message}";
        }
    }
}

using System;

namespace Errors
{
    /// <summary>
    /// Inner error class
    /// </summary>
    public class Error : Exception
    {
        private readonly string _message;
        private readonly int _code;
        private readonly DateTime _date;
        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get => _message; }
        /// <summary>
        /// Error code
        /// </summary>
        public int Code { get => _code; }
        /// <summary>
        /// Constructor of error
        /// </summary>
        /// <param name="message">Error text</param>
        /// <param name="code"> Error code</param>
        public Error(string message, int code = 0)
        {
            _message = message;
            _code = code;
            _date = DateTime.Now;
        }
        public override string ToString()
        {
            var code = _code != 0 ? $"{_code}: " : "";
            return $"[{_date}] {code}{_message}";
        }
    }
}

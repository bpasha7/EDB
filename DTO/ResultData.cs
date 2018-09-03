using System;
using System.Collections.Generic;

namespace DTO
{
    public enum ResultDataType { Message = 1, DataSet = 2 }
    public class ResultData
    {
        public ResultDataType DataType { get; set; }
        public string Message { get; set; }
        public IList<string> Headers { get; set; }
        public IList<string> Types { get; set; }
        public IList<object[]> Values { get; set; }
        public int Count { get; set; }
        public ResultData()
        {
            Headers = new List<string>();
            Types = new List<string>();
            Values = new List<object[]>();
        }
    }
}

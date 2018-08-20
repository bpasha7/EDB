using System;
using System.Collections.Generic;

namespace DTO
{
    public class ResultData
    {
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

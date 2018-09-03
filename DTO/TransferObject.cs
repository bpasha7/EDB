using System;
using System.Collections.Generic;
using System.Text;

namespace DTO
{
    [Serializable]
    public class TransferObject
    {
        public ErrorData Error { get; set; }
        public ResultData Data { get; set; }
        public string Time { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DBMS.Indexes
{
    /// <summary>
    /// Positions of headers into Dense Index File
    /// </summary>
    public enum DenseIndexFileScheme : int
    {
        /// <summary>
        /// Index count into file
        /// </summary>
        Count = 0,
    }
    /// <summary>
    /// Size of headers into Dense Index File
    /// </summary>
    public enum DenseIndexFileSchemeSize : int
    {
        /// <summary>
        /// Index count into file
        /// </summary>
        Count = 4,
    }
    /// <summary>
    /// Dense index
    /// </summary>
    public class DenseIndex
    {
        /// <summary>
        /// Flag of removed index. 1 - removed, 0 - ok
        /// </summary>
        public byte Removed { get; set; }
        /// <summary>
        /// Index vaule size
        /// </summary>
        public int Size
        {
            get
            {
                return Value.Length;
            }
        }
        /// <summary>
        /// Index vaule
        /// </summary>
        public byte[] Value { get; set; }
        /// <summary>
        /// Position of record into table file
        /// </summary>
        public int RecordPosition { get; set; }
        ///// <summary>
        ///// Position of next index into index file
        ///// if root - 0
        ///// </summary>
        //public int NextIndex { get; set; }
        ///// <summary>
        ///// Position of previous index into index file
        ///// if tail - 0
        ///// </summary>
        //public int PreviousIndex { get; set; }

        public DenseIndex(int value, int recordPosition)
        {
            Value = BitConverter.GetBytes(value);
            RecordPosition = recordPosition;
        }
        public DenseIndex(string value, int recordPosition)
        {
            Value = Encoding.ASCII.GetBytes(value);
            RecordPosition = recordPosition;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DBMS.Indexes
{
    /// <summary>
    /// Dense index
    /// </summary>
    public class DenseIndex
    {
        /// <summary>
        /// Flag of removed index
        /// </summary>
        public bool Removed { get; set; }
        /// <summary>
        /// Index vaule
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// Position of record into table file
        /// </summary>
        public int RecordPosition { get; set; }
        /// <summary>
        /// Position of next index into index file
        /// if root - 0
        /// </summary>
        public int NextIndex { get; set; }
        /// <summary>
        /// Position of previous index into index file
        /// if tail - 0
        /// </summary>
        public int PreviousIndex { get; set; }
    }
}

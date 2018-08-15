using Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBMS.Indexes
{
    /// <summary>
    /// Builder pattern for DenseIndex
    /// </summary>
    public sealed class DenseIndexBuilder
    {
        private readonly DenseIndex _index;// = new DenseIndex();
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">Default null</param>
        public DenseIndexBuilder(DenseIndex index = null)
        {
            //if (index == null)
            //    //_index = new DenseIndex();
            //else
            //    _index = index;
        }
        /// <summary>
        /// Set string value
        /// </summary>
        /// <param name="value">String value</param>
        /// <returns>Builder</returns>
        public DenseIndexBuilder Value(string value)
        {
            throw new Error("Not Released");
            _index.Value = Encoding.ASCII.GetBytes(value);
            return this;
        }
        /// <summary>
        /// Set int value
        /// </summary>
        /// <param name="value">Integer value</param>
        /// <returns>Builder</returns>
        public DenseIndexBuilder Value(int value)
        {
            _index.Value = BitConverter.GetBytes(value);
            return this;
        }
        /// <summary>
        /// Set record position
        /// </summary>
        /// <param name="position">Record position into data file</param>
        /// <returns>Builder</returns>
        public DenseIndexBuilder RecordPosition(int position)
        {
            _index.RecordPosition = position;
            return this;
        }
        ///// <summary>
        ///// Set next index address
        ///// </summary>
        ///// <param name="position"></param>
        ///// <returns>Builder</returns>
        //public DenseIndexBuilder NextIndexAddress(int position)
        //{
        //    _index.NextIndex = position;
        //    return this;
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="position"></param>
        ///// <returns>Builder</returns>
        //public DenseIndexBuilder PreviousIndexAddress(int position)
        //{
        //    _index.PreviousIndex = position;
        //    return this;
        //}
        /// <summary>
        /// Build
        /// </summary>
        /// <returns></returns>
        public DenseIndex Build()
        {
            //if (_index.PreviousIndex < 0 || _index.NextIndex < 0)
            //    throw new IndexError($"Adress of Index can not be less then 0.");
            //if (_index.PreviousIndex == _index.NextIndex)
            //    throw new IndexError($"Adress of Next and Priveous Index can not be the same.");
            if(_index.Size == 0)
                throw new IndexError($"Index can not be null.");
            return _index;
        }
    }
}

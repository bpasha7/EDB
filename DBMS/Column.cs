using Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBMS
{
    /// <summary>
    /// Column of table
    /// </summary>
    public class Column
    {
        private string _name;
        private uint _size;
        private byte _type;
        private ulong _offset;

        /// <summary>
        /// Name of column
        /// </summary>
        public string Name { get => _name; set => _name = value; }
        /// <summary>
        /// Type of column:
        /// INT - 1
        /// BIT - 2
        /// VARCHAR(*) - 3
        /// </summary>
        public byte Type { get => _type; set => _type = value; }
        /// <summary>
        /// Size of colums in bytes
        /// </summary>
        public UInt32 Size { get => _size; set => _size = value; }
        /// <summary>
        /// Offset from record beginning
        /// </summary>
        public ulong Offset { get => _offset; set => _offset = value; }
        /// <summary>
        /// Parse column defenition from query
        /// </summary>
        /// <param name="arguments">part of sql query</param>
        public void ParseType(string arguments)
        {
            // split part of query by brackets and spaces
            var splited = arguments
                .Trim()
                .Split(new char[] { '(', ')', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            // check length of splited query
            if (splited.Length < 2)
                throw new ColumnError($"Can not parse ['{arguments}'].");
            _name = splited[0];
            // parse type and size by splited values
            switch (splited[1].ToLower())
            {
                // INTEGER type
                case "int":
                    _type = 1;
                    _size = sizeof(int);
                    break;
                // BIT type
                case "bit":
                    _type = 2;
                    _size = sizeof(bool);
                    break;
                // VARCHAR(*) type
                case "varchar":
                    if (splited.Length != 3)
                        throw new ColumnError($"Can not parse size of VARCHAR from ['{arguments}']. ");
                    _type = 3;
                    _size = Convert.ToUInt32(splited[2]);
                    break;
                default:
                    throw new ColumnError($"'{splited[1]}' is not supported type. ");
            }
        }
    }
}

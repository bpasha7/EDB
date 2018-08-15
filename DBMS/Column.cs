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
        /// name|type|size|primaryKey|indexName
        private string _name;
        private int _size;
        private byte _type;
        private int _offset;

        private bool _visible;

        private byte _primaryKey;
        private string _indexName;

        /// <summary>
        /// Id rimary key
        /// </summary>
        public byte PrimaryKey { get => _primaryKey; set => _primaryKey = value; }
        /// <summary>
        /// Name of index
        /// </summary>
        public string IndexName { get => _indexName; set => _indexName = value; }
        /// <summary>
        /// Name of column
        /// </summary>
        public bool Visible { get => _visible; set => _visible = value; }
        /// <summary>
        /// Name of column
        /// </summary>
        public string Name { get => _name; set => _name = value; }
        /// <summary>
        /// Type of column:
        /// INT - 1
        /// BIT - 2
        /// VARCHAR(*) - 3
        /// DATETIME - 4
        /// </summary>
        public byte Type { get => _type; set => _type = value; }
        /// <summary>
        /// Size of colums in bytes
        /// </summary>
        public int Size { get => _size; set => _size = value; }
        /// <summary>
        /// Offset from record beginning
        /// </summary>
        public int Offset { get => _offset; set => _offset = value; }
        /// <summary>
        /// Parse column defenition from query
        /// </summary>
        /// <param name="arguments">part of sql query</param>
        public void ParseType(string arguments)
        {
            var str = arguments.ToLower();
            #region define if primary key columns
            var PK_ATIBUTE = "primary key";
            var isPrimary = str.IndexOf(PK_ATIBUTE);
            if (isPrimary > 0)
            {
                _primaryKey = 1;
                arguments = arguments.Remove(isPrimary, PK_ATIBUTE.Length);
            }
            #endregion

            #region define if indexcolumns
            var INDEX_ATIBUTE = "index";
            var isIndex = str.IndexOf(INDEX_ATIBUTE);
            if (isIndex > 0)
            {
                var indexParts = arguments
                    .Substring(isIndex)
                    .Trim()
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (indexParts.Length == 2)
                {
                    _indexName = indexParts[1];
                    arguments = arguments.Substring(0, isIndex);
                }
                else
                    throw new ColumnError($"Can not parse index name  from ['{arguments}'].");
            }
            #endregion
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
                    _size = Convert.ToInt32(splited[2]);
                    break;
                // DATETIME type
                case "datetime":
                    _type = 4;
                    _size = sizeof(long);
                    break;
                default:
                    throw new ColumnError($"'{splited[1]}' is not supported type. ");
            }
        }
    }
}

using Errors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryFileStream
{
    /// <summary>
    /// File stream for work with data table
    /// </summary>
    public class FileStream : IDisposable
    {
        private static List<string> _blockedDatabaseTables = new List<string>();
        private readonly string _path;
        private System.IO.FileStream _stream;
        //private BinaryWriter _streamWriter;
        //private BinaryReader _streamReader;
        private string Table;
        public readonly string Database;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dataBaseName"></param>
        /// <param name="head">true - head file, false - default</param>
        /// 
        public FileStream(string path, string dataBaseName, string tableName, bool head = false)
        {
            // set root path
            _path = path;
            // set database name
            Database = dataBaseName;
            // set table name
            Table = tableName;
            _path = GetPath(head);
            // set current path to database table
            //_path = GetPath(head);
            // check blocked or not
            if (_blockedDatabaseTables.Contains(_path))
                throw new FileSystemError($"Table {dataBaseName} is blocked.");
            //else
            //    // insert database table into block list
            //    _blockedDatabaseTables.Add(_path);

        }
        #region File Actions
        /// <summary>
        /// Create file for new table
        /// </summary>
        public void Create()
        {
            //_stream?.Close();
            //_stream = new System.IO.FileStream(_path, FileMode.Create);
            //_stream.Close();
            var createStream = File.Create(_path);
            createStream.Close();
        }
        public void Delete()
        {
            _stream?.Close();
            File.Delete(_path);
        }
        /// <summary>
        /// Open database table file by name
        /// </summary>
        public void Open()
        {
            // check exists or not database table file
            if (!File.Exists(_path))
                throw new FileSystemError($"Table [{Table}] is not exist.");
            // insert database table into block list
                _blockedDatabaseTables.Add(_path);
            _stream?.Close();
            _stream = new System.IO.FileStream(_path, FileMode.Open);
            // lock file
            //_stream.Lock();
        }
        /// <summary>
        /// Close opened file stream and remove it from block database table
        /// </summary>
        public void Close()
        {
            _stream?.Close();
            _blockedDatabaseTables.Remove(_path);
        }
        /// <summary>
        /// Generate path depends on extention
        /// </summary>
        /// <param name="head">true - head file</param>
        /// <returns>Path to file</returns>
        private string GetPath(bool head = false)
        {
            var extention = head ? ".df" : ".dt";
            return $"{_path}{Database}\\{Table}{extention}";
        }
        public void SetPosition(long position)
        {
            _stream?.Seek(position, SeekOrigin.Begin);
        }
        public long? GetPosition()
        {
            return _stream?.Position;
        }
        #endregion
        #region Writing Values
        public void WriteByte(byte value)
        {
            _stream?.WriteByte(value);
        }
        public void WriteInt(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            _stream?.Write(bytes, 0, bytes.Length);

        }
        public void WriteText(string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            _stream?.Write(bytes, 0, bytes.Length);
        }
        #endregion
        #region Reading Vlaues
        public byte ReadByte()
        {
            return Convert.ToByte(_stream?.ReadByte());
        }
        public int ReadInt()
        {
            byte[] intValue = new byte[4];
            _stream?.Read(intValue, 0, 4);
            BitConverter.ToInt32(intValue, 0);
            return BitConverter.ToInt32(intValue, 0);// Convert.ToInt32(intValue);
        }
        public string ReadText(int length)
        {
            byte[] textValue = new byte[length];
            _stream?.Read(textValue, 0, length);
            return Encoding.ASCII.GetString(textValue);
        }
        #endregion

        public void Dispose()
        {
            Close();
        }

    }
}

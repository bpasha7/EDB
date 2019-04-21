using Errors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BinaryFileStream
{
    public enum FileType { Data, Scheme, Index };
    /// <summary>
    /// File stream for work with data table
    /// </summary>
    public class FileStream : IDisposable
    {
        private static List<string> _blockedDatabaseTables = new List<string>();
        private readonly string _path;
        private System.IO.FileStream _stream;
        private string FileName;
        public readonly string Database;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dataBaseName"></param>
        /// <param name="head">true - head file, false - default</param>
        /// 
        public FileStream(string path, string dataBaseName, string fileName, FileType type = FileType.Data)
        {
            // set root path
            _path = path;
            // set database name
            Database = dataBaseName;
            // set table name
            FileName = fileName;
            // set path to file
            _path = GetPath(type);
            // check blocked or not
            if (_blockedDatabaseTables.Contains(_path))
                throw new FileSystemError($"Database [{dataBaseName}] is blocked.");
        }
        #region File Actions
        /// <summary>
        /// Create file for new table
        /// </summary>
        public void Create()
        {
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
                throw new FileSystemError($"Table [{FileName}] is not exist.");
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
        
        public byte[] CutToEnd(int position)
        {
            var size = Convert.ToInt32(_stream.Length - position);
            if (size == 0)
                return null;
            var bytes = new byte[size];
            SetPosition(position);
            _stream?.Read(bytes, 0, size);
            //_stream?.SetLength(position);
            return bytes;
        }

        public void SetLength(int length)
        {
            _stream?.SetLength(length);
        }

        /// <summary>
        /// Generate path depends on extention
        /// </summary>
        /// <param name="head">true - head file</param>
        /// <returns>Path to file</returns>
        private string GetPath(FileType type)
        {
            var extention = ".dt";
            if (type == FileType.Scheme)
                extention = ".df";
            if (type == FileType.Index)
                extention = ".idx";
            return $"{_path}{Database}\\{FileName}{extention}";
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
        public void WriteDate(long ticks)
        {
            byte[] bytes = BitConverter.GetBytes(ticks);
            _stream?.Write(bytes, 0, bytes.Length);
        }
        public void WriteBytes(byte[] bytes)
        {
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
            return BitConverter.ToInt32(intValue, 0);
        }
        public string ReadText(int length)
        {
            byte[] textValue = new byte[length];
            _stream?.Read(textValue, 0, length);
            return Encoding.ASCII.GetString(textValue);
        }
        public byte[] ReadBytes(int length)
        {
            byte[] Value = new byte[length];
            _stream?.Read(Value, 0, length);
            return Value;
        }
        public long ReadDate()
        {
            byte[] longValue = new byte[sizeof(long)];
            _stream?.Read(longValue, 0, sizeof(long));
            return BitConverter.ToInt64(longValue, 0);
        }
        #endregion

        public void Dispose()
        {
            Close();
        }

    }
}

using System;

namespace BinaryFileStream
{
    public class FileStream
    {
        public FileStream(string dataBaseName, string tableName, bool head = false)
        {

        }
        #region File Actions
        private void Create(string dataBaseName, string tableName, bool head = false)
        {

        }
        private void Open(string dataBaseName, string tableName, bool head = false)
        {

        }
        public void SetPosition(long position)
        {

        }
        #endregion
        #region Writing Values
        public char ReadChar()
        {
            return '0';
        }
        public int ReadInt()
        {
            return 0;
        }
        public string ReadText(int length)
        {
            return "0";
        }

        #endregion
    }
}

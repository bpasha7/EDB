using BinaryFileStream;
using DDL.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBMS
{
    public class Table
    {
        /// <summary>
        /// Table Name
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Database name
        /// </summary>
        public readonly string Database;
        private Column[] _columns;
        //public Table()
        //{

        //}
        public Table(string database, string tableName)
        {
            Name = tableName;
            Database = database;
        }

        private void getScheme()
        {
            
            string path = "";
#if DEBUG
            path = "C:\\Users\\bpash\\Documents\\workspace\\EDB\\EDB\\bin\\Debug\\netcoreapp2.0\\";
#endif
            // open stream to read table scheme
            var fileStream = new FileStream(path, Database, Name, true);
            fileStream.Open();
            fileStream.SetPosition(0);
            // read count of table columns
            var columnsCount = fileStream.ReadInt();
            // create array to read columns properties
            _columns = new Column[columnsCount];
            // read all columns properties
            for (int i = 0; i < columnsCount; i++)
            {
                // read lenght of columna name
                var nameLength = fileStream.ReadInt();
                // read column name
                _columns[i].Name = fileStream.ReadText(nameLength);
                // read column type
                _columns[i].Type = fileStream.ReadByte();
                // read column size
                _columns[i].Size = Convert.ToUInt32(fileStream.ReadInt());
            }
        }

        public int Insert()
        {
            getScheme();

            return 0;
        }


        public void Create(CreateTableCommand cmd)
        {
            string path = "";
#if DEBUG
            path = "C:\\Users\\bpash\\Documents\\workspace\\EDB\\EDB\\bin\\Debug\\netcoreapp2.0\\";
#endif
            // create new data file for new table
            var fileStream = new FileStream(path, Database, Name);
            fileStream.Create();
            fileStream.Close();
            // create new head file for new table
            fileStream = new FileStream(path, Database, Name, true);
            fileStream.Create();
            fileStream.Open();
            fileStream.SetPosition(0);
            // write count of columns in the begining of header file
            fileStream.WriteInt(cmd.Columns.Length);
            var column = new Column();
            // parse and write columns
            foreach (var columnInfo in cmd.Columns)
            {
                column.ParseType(columnInfo);
                // write length of column name
                fileStream.WriteInt(column.Name.Length);
                // write column name
                fileStream.WriteText(column.Name);
                //  write column Type
                fileStream.WriteByte(column.Type);
                //  write column Size
                fileStream.WriteInt(Convert.ToInt32(column.Size));
            }
            fileStream.Close();
        }
    }
}

using BinaryFileStream;
using DDL.Commands;
using DML.Commands;
using DTO;
using Errors;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public readonly string Path;
        private Column[] _columns;
        private FileStream fileStream;
        private readonly NLog.Logger _logger;
        //public Table()
        //{

        //}
        public Table(string database, string tableName)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _logger.Info("test");
            Name = tableName;
            Database = database;
#if DEBUG
            Path = "C:\\Users\\bpash\\Documents\\workspace\\EDB\\EDB\\bin\\Debug\\netcoreapp2.0\\";
#endif
        }
        /// <summary>
        /// Get table scheme from head file
        /// </summary>
        private void getScheme()
        {
            try
            {
                // open stream to read table scheme
                fileStream = new FileStream(Path, Database, Name, true);
                fileStream.Open();
                fileStream.SetPosition(4);
                // read count of table columns
                var columnsCount = fileStream.ReadInt();
                // create array to read columns properties
                _columns = new Column[columnsCount];
                // read all columns properties
                for (int i = 0; i < columnsCount; i++)
                {
                    _columns[i] = new Column();
                    // read lenght of columna name
                    var nameLength = fileStream.ReadInt();
                    // read column name
                    _columns[i].Name = fileStream.ReadText(nameLength);
                    // read column type
                    _columns[i].Type = fileStream.ReadByte();
                    // read column size
                    _columns[i].Size = Convert.ToUInt32(fileStream.ReadInt());
                    // set offset in record
                    _columns[i].Offset = i == 0 ? 0 : _columns[i - 1].Size;
                }

            }
            catch(Exception ex)
            {
                throw new Errors.Error(ex.Message);
            }
            finally
            {
                // close
                fileStream?.Close();
            }
        }
        /// <summary>
        /// Check data type from scheme and value from query
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value from query</param>
        /// <returns></returns>
        private bool checkDataToInsert(string fieldName, string value)
        {
            // find column by name
            var col = _columns.SingleOrDefault(c => c.Name == fieldName);
            // try parse value by type
            switch (col?.Type)
            {
                // parse int
                case 1:
                    int resInt = 0;
                    return Int32.TryParse(value, out resInt);
                // parse byte
                case 2:
                    byte resByte = 0;
                    return Byte.TryParse(value, out resByte);
                // parse string
                case 3:
                    // string value must have single quotes at start and end of it-self
                    return (value[0] == '\'' && value[value.Length - 1] == '\'') ? true : false;
                default:
                    return false;
            };
        }

        private bool insert(IDictionary<string, string> values, bool hasPattern)
        {
            try
            {
                fileStream = new FileStream(Path, Database, Name);
                fileStream.Open();
                // read last position and go there to insert new one
                fileStream.SetPosition(0);
                var lastPosition = fileStream.ReadInt();
                fileStream.SetPosition(lastPosition);
                // if patterned query
#warning not released
                if(hasPattern)
                {
                    throw new Exception("Method not released");
                   // for
                }
                else
                {
                    var dataToInsert = values.ToList();
                    // compare count iserted data and column count
                    if (dataToInsert.Count != _columns.Length)
                        throw new InsertCommandExcecute($"Inserted data does not match fild count in table.");
                    for (int i = 0; i < _columns.Length; i++)
                    {
                        switch (_columns[i].Type)
                        {
                            case 1:
                                int intValue = Convert.ToInt32(dataToInsert[i].Value);
                                fileStream.WriteInt(intValue);
                                break;
                            case 2:
                                byte byteValue = Convert.ToByte(dataToInsert[i].Value[0]);
                                fileStream.WriteByte(byteValue);
                                break;
                            case 3:
                                // cut text without single quotes
                                string text = dataToInsert[i].Value
                                    .Substring(1, dataToInsert[i].Value.Length - 2);
                                // check available size
                                if (text.Length > _columns[i].Size)
                                    throw new InsertCommandExcecute($"Column [{_columns[i].Name}] has maximum size {_columns[i].Size}.");
                                // pad text if length is less then column size
                                text = text.PadLeft((int)_columns[i].Size);
                                fileStream.WriteText(text);
                                break;
                            default:
                                break;
                        }
                    }
                }
                // write last posistion
                var pos = (int)fileStream.GetPosition();
                fileStream.SetPosition(0);
                fileStream.WriteInt(pos);
                fileStream.Close();
                fileStream = new FileStream(Path, Database, Name, true);
                fileStream.Open();
                fileStream.SetPosition(0);
                pos = fileStream.ReadInt();
                fileStream.SetPosition(pos);
                fileStream.WriteInt(lastPosition);
                fileStream.SetPosition(0);
                fileStream.WriteInt(pos + 4);
                fileStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                //if(typeof(InsertCommandExcecute) == ex.GetType())
                //{
                //    throw ex as InsertCommandExcecute;
                //}
                //throw new Errors.Error(ex.Message);
                return false;
            }
            finally
            {
                // close
                fileStream?.Close();
            }
        }

        public void Insert(InsertCommand cmd)
        {
            getScheme();
            var res = insert(cmd.Values, cmd.HasPattern);
            if (!res)
                throw new InsertCommandExcecute($"The row was not inserted.");
        }

        public ResultData Select(SelectCommand cmd)
        {
            var resultData = new ResultData();

            return resultData;
        }

        public void Create(CreateTableCommand cmd)
        {
            // create new data file for new table
            fileStream = new FileStream(Path, Database, Name);
            fileStream.Create();
            fileStream.Open();
            // write position for next insert data
            fileStream.WriteInt(4);
            fileStream.Close();
            // create new head file for new table
            fileStream = new FileStream(Path, Database, Name, true);
            fileStream.Create();
            fileStream.Open();
            fileStream.SetPosition(0);
            // write fake position where rows positions will be
            fileStream.WriteInt(100);
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
            // get current position
            var pos = fileStream.GetPosition();
            // write current position
            fileStream.SetPosition(0);
            // rewrite fake position
            fileStream.WriteInt((int)pos);
            fileStream.Close();
        }
    }
}

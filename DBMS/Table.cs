using BinaryFileStream;
using DBMS.Indexes;
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
    public class Table : IDisposable
    {
        /// <summary>
        /// Table Name
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Database name
        /// </summary>
        private readonly string Database;
        private readonly string Path;
        private Column[] _columns;
        private FileStream _fileStream;
        private readonly NLog.Logger _logger;

        public Table(string path, string database, string tableName)
        {
            //if (!System.IO.File.Exists($"{path}{database}\\{tableName}.df"))
            //    throw new Error($"Table [{tableName}] is not exist.");
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _logger.Info($"Database: [{database}]. Table: [{tableName}].");
            Name = tableName;
            Database = database;
            Path = path;
        }
        /// <summary>
        /// Get table scheme from head file
        /// </summary>
        private void getScheme()
        {
            try
            {
                // open stream to read table scheme
                _fileStream = new FileStream(Path, Database, Name, FileType.Scheme);
                _fileStream.Open();
                _fileStream.SetPosition(4);
                // read count of table columns
                var columnsCount = _fileStream.ReadInt();
                // create array to read columns properties
                _columns = new Column[columnsCount];
                // read all columns properties
                for (int i = 0; i < columnsCount; i++)
                {
                    _columns[i] = new Column();
                    // read lenght of columna name
                    var nameLength = _fileStream.ReadInt();
                    // read column name
                    _columns[i].Name = _fileStream.ReadText(nameLength);
                    // read column type
                    _columns[i].Type = _fileStream.ReadByte();
                    // read column size
                    _columns[i].Size = _fileStream.ReadInt();
                    // read pk flag
                    _columns[i].PrimaryKey = _fileStream.ReadByte();
                    // read index name
                    var indexNameLength = _fileStream.ReadInt();
                    if(indexNameLength > 0)
                        _columns[i].IndexName = _fileStream.ReadText(indexNameLength);
                    // set offset in record
                    _columns[i].Offset = i == 0 ? 0 : _columns[i - 1].Size;
                }

            }
            catch (Exception ex)
            {
                throw new Errors.Error(ex.Message);
            }
            finally
            {
                // close
                _fileStream?.Close();
            }
        }
        /// <summary>
        /// Get Columns if exist by name
        /// </summary>
        /// <param name="name"Columns name</param>
        /// <returns>column or null</returns>
        private Column getColumnByName(string name)
        {
            return _columns.SingleOrDefault(c => c.Name.ToLower() == name.ToLower());
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
            var col = getColumnByName(fieldName);
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
        /// <summary>
        /// Get one record size
        /// </summary>
        /// <returns></returns>
        private int getRecordSize()
        {
            var size = 0;
            // sum all columns size
            foreach (var item in _columns)
            {
                size += item.Size;
            }
            return size;
        }
        /// <summary>
        /// Insert values into table
        /// </summary>
        /// <param name="values"> Vlaues</param>
        /// <param name="hasPattern">flag for pattern</param>
        private void insert(IDictionary<string, string> values, bool hasPattern)
        {
            try
            {
                _fileStream = new FileStream(Path, Database, Name);
                _fileStream.Open();
                // read last position and go there to insert new one
                _fileStream.SetPosition(0);
                var lastPosition = _fileStream.ReadInt();
                _fileStream.SetPosition(lastPosition);
                // if patterned query
#warning not released
                if (hasPattern)
                {
                    throw new Exception("Method not released");
                    // for
                }
                else
                {
                    var dataToInsert = values.ToList();
                    // compare count iserted data and column count
                    if (dataToInsert.Count != _columns.Length)
                        throw new InsertCommandExcecute($"Inserted data does not match field count in table.");

                    // check data to insert
                    for (int i = 0; i < _columns.Length; i++)
                    {
                        var isValid = checkDataToInsert(_columns[i].Name, dataToInsert[i].Value);
                        if (!isValid)
                            throw new InsertCommandExcecute($"Column [{_columns[i].Name}] does not match for [{dataToInsert[i].Value}] format.");
                    }

                    for (int i = 0; i < _columns.Length; i++)
                    {
                        switch (_columns[i].Type)
                        {
                            case 1:
                                int intValue = Convert.ToInt32(dataToInsert[i].Value);
                                _fileStream.WriteInt(intValue);
                                // if has index name write index
                                if (_columns[i].IndexName?.Length > 0)
                                {
                                    var newIndex = new DenseIndex(intValue, lastPosition);
                                    _logger.Trace($"Записываем индекс для колонки {_columns[i].Name} для значения {intValue}.");
                                    var posToInsertIndex = findPositiontoInsertIndex(_columns[i], intValue);

                                    writeDenseIndex(_columns[i], newIndex, posToInsertIndex);
                                }
                                break;
                            case 2:
                                byte byteValue = Convert.ToByte(dataToInsert[i].Value[0]);
                                _fileStream.WriteByte(byteValue);
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
                                _fileStream.WriteText(text);
                                break;
                            case 4:
                                DateTime date;
                                var dateString = dataToInsert[i].Value
                                    .Substring(1, dataToInsert[i].Value.Length - 2);
                                var convertResult = DateTime.TryParse(dateString, out date);
                                if(!convertResult)
                                    throw new InsertCommandExcecute($"Value for column [{_columns[i].Name}] has not correct datetime format.");
                                _fileStream.WriteDate(date.Ticks);
                                break;
                            default:
                                break;
                        }
                    }
                }
                // write last posistion
                var pos = (int)_fileStream.GetPosition();
                _fileStream.SetPosition(0);
                _fileStream.WriteInt(pos);
                _fileStream.Close();
                _fileStream = new FileStream(Path, Database, Name, FileType.Scheme);
                _fileStream.Open();
                _fileStream.SetPosition(0);
                pos = _fileStream.ReadInt();
                _fileStream.SetPosition(pos);
                _fileStream.WriteInt(lastPosition);
                _fileStream.SetPosition(0);
                _fileStream.WriteInt(pos + 4);
                _fileStream.Close();



            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                if (typeof(InsertCommandExcecute) == ex.GetType())
                {
                    throw ex as InsertCommandExcecute;
                }
                throw new Errors.Error(msg);
            }
            finally
            {
                // close
                _fileStream?.Close();
            }
        }
        /// <summary>
        /// Execute Insert command
        /// </summary>
        /// <param name="cmd">Insert command</param>
        public void Insert(InsertCommand cmd)
        {
            getScheme();
            insert(cmd.Values, cmd.HasPattern);
            //if (!res)
            //    throw new InsertCommandExcecute($"The row was not inserted.");
        }
        /// <summary>
        /// Read record from table. File steam must be opened and position set.
        /// </summary>
        /// <returns>Array of read objects from current position</returns>
        private object[] readRecord()
        {
            var values = new object[_columns.Length];
            //var current = 0;
            // cycle by colmns to read
            for (int j = 0; j < _columns.Length; j++)
            {
                switch (_columns[j].Type)
                {
                    case 1:
                        {
                            int val = _fileStream.ReadInt();
                            _logger.Trace($"Read number {val}. Columns {_columns[j].Name}.");
                            values[j] = val;
                        }
                        break;
                    case 2:
                        {
                            byte val = Convert.ToByte(_fileStream.ReadByte() - 48);
                            _logger.Trace($"Read byte {val}. Columns {_columns[j].Name}.");
                            values[j] = Convert.ToBoolean(val);
                        }
                        break;
                    case 3:
                        {
                            int length = Convert.ToInt32(_columns[j].Size);//_fileStream.ReadInt();
                            string val = _fileStream.ReadText(length).TrimStart();
                            _logger.Trace($"Read text {val}. Columns {_columns[j].Name}.");
                            values[j] = val;
                        }
                        break;
                    case 4:
                        {
                            var ticks = _fileStream.ReadDate();
                            DateTime val = new DateTime(ticks);
                            _logger.Trace($"Read datetime {val}. Columns {_columns[j].Name}.");
                            values[j] = val;
                        }
                        break;
                    default:
                        //throw new SelectCommandExcecute($"{}");
                        break;
                }
            }
            return values;
        }

        private bool checkConditions(IList<Condition> conditions, IList<string> operators, Dictionary<string, int> columnDictionary, object[] recodValues)
        {
            bool res = true;
            // if single condition
            if (conditions.Count == 1 && operators.Count == 0 && columnDictionary.Count == 1)
            {
                var kV = columnDictionary.ElementAt(0);
                var col = _columns[kV.Value];
                switch (col.Type)
                {
                    case 1:
                        {
                            var val = Convert.ToInt32(recodValues[kV.Value]);
                            res = conditions[0].Operate(col.Name, val);
                            return res;
                        }
                    case 2:
                        {
                            var val = Convert.ToInt32(recodValues[kV.Value]);
                            res = conditions[0].Operate(col.Name, val);
                            return res;
                        }
                    case 3:
                        {
                            var val = recodValues[kV.Value] as string;
                            res = conditions[0].Operate(col.Name, val);
                            return res;
                        }
                    default:
                        return false;
                }
            }
            else
            {

            }
            return res;
        }

        private void select(SelectCommand cmd, ResultData resultData, IList<int> positions = null)
        {
            // get columns that visible
            _logger.Trace($"Get columns to input data.");
            var columns = _columns.Where(col => col.Visible).ToList();
            _logger.Trace($"Got {columns.Count} columns to input data.");
            // if filtered column
            var columnDictionary = new Dictionary<string, int>();
            // if have conditions create dictionary with fields
            if (cmd.Conditions.Count != 0)
            {
                _logger.Trace($"Try to catch fields into condition.");
                // find fields into conditions
                for (int i = 0; i < _columns.Length; i++)
                {
                    var colName = _columns[i].Name.ToLower();
                    var fieldCondition = cmd.Conditions
                        .Where(condition => condition.Operands.Contains(colName))
                        .SingleOrDefault();
                    if (fieldCondition != null && !columnDictionary.ContainsKey(colName))
                    {
                        // add into dictionary
                        _logger.Info($"Detected field [{_columns[i].Name}] into conditions.");
                        columnDictionary.Add(colName, i);
                        if(_columns[i].IndexName?.Length > 0)
                        {
                            positions = readPosition(_columns[i], fieldCondition);
                        }
                    }
                }
            }
            _fileStream = new FileStream(Path, Database, Name);
            _fileStream.Open();
            if (positions == null)
            {
                _fileStream.SetPosition(0);
                // read table records count
                var recordsCount = _fileStream.ReadInt() / getRecordSize();
                _logger?.Trace($"Получение числа записей в таблице {Name}: {recordsCount}.");

                // cycle by records to read
                for (int i = 0; i < recordsCount; i++)
                {
                    // flag to not read 
                    //var skip = false;
                    _logger.Trace($"Read record {i + 1}.");
                    var record = readRecord();
                    // check condtions
                    if (checkConditions(cmd.Conditions, cmd.ConditionsOperators, columnDictionary, record))
                    {
                        resultData.Values.Add(record);
                        _logger.Trace($"Record {i + 1} was inserted into result data.");
                    }
                    else
                    {
                        _logger.Trace($"Record {i + 1} was not inserted into result data.");

                    }
                    _logger.Trace($"------------------------------------.");

                }
            }
            else
            {
                // cycle by records to read
                for (int i = 0; i < positions.Count; i++)
                {
                    // flag to not read 
                    //var skip = false;
                    _fileStream.SetPosition(positions[i]);

                    _logger.Trace($"Read record {i + 1}.");
                    var record = readRecord();
                    resultData.Values.Add(record);
                    // check condtions
                    //if (checkConditions(cmd.Conditions, cmd.ConditionsOperators, columnDictionary, record))
                    //{
                    //    resultData.Values.Add(record);
                    //    _logger.Trace($"Record {i + 1} was inserted into result data.");
                    //}
                    //else
                    //{
                    //    _logger.Trace($"Record {i + 1} was not inserted into result data.");

                    //}
                    _logger.Trace($"------------------------------------.");

                }
            }
            _fileStream.Close();
        }
        /// <summary>
        /// Select data from table
        /// </summary>
        /// <param name="cmd">Select query</param>
        /// <returns>ResultData</returns>
        public ResultData Select(SelectCommand cmd)
        {
            var resultData = new ResultData();
            _logger?.Trace($"Получение схемы таблицы {Name}.");
            getScheme();
            // if all columns
            if (cmd.AllColumns)
            {
                // add namne of columns to result data
                foreach (var column in _columns)
                {
                    resultData.Headers.Add(column.Name);
                }
                _logger?.Trace($"Добавление названия полей в структуру ответа.");
            }
            // has pattern
            else
            {
                foreach (var item in cmd.ColumnsName)
                {
                    var col = getColumnByName(item);
                    if (col == null)
                        throw new SelectCommandExcecute($"Columns [{item}] not exist into table [{Name}].");
#warning 'Check setting flag or delete it'
                    col.Visible = true;
                    resultData.Headers.Add(col.Name);
                }
            }
            select(cmd, resultData);


            return resultData;
        }
        /// <summary>
        /// Create table into data base
        /// </summary>
        /// <param name="cmd">Create table query</param>
        public void Create(CreateTableCommand cmd)
        {
            try
            {
                // create new data file for new table
                _fileStream = new FileStream(Path, Database, Name);
                _fileStream.Create();
                _fileStream.Open();
                // write position for next insert data
                _fileStream.WriteInt(4);
                _fileStream.Close();
                // create new head file for new table
                _fileStream = new FileStream(Path, Database, Name, FileType.Scheme);
                _fileStream.Create();
                _fileStream.Open();
                _fileStream.SetPosition(0);
                // write fake position where rows positions will be
                _fileStream.WriteInt(100);
                // write count of columns in the begining of header file
                _fileStream.WriteInt(cmd.Columns.Length);
                // parse and write columns
                foreach (var columnInfo in cmd.Columns)
                {
                    var column = new Column();
                    column.ParseType(columnInfo);
                    // write length of column name
                    _fileStream.WriteInt(column.Name.Length);
                    // write column name
                    _fileStream.WriteText(column.Name);
                    //  write column Type
                    _fileStream.WriteByte(column.Type);
                    //  write column Size
                    _fileStream.WriteInt(Convert.ToInt32(column.Size));
                    // write primary key
                    _fileStream.WriteByte(column.PrimaryKey);
                    // write index name if exist
                    _fileStream.WriteInt(Convert.ToInt32(column.IndexName?.Length));
                    // write column name
                    _fileStream.WriteText(column.IndexName == null ? "" : column.IndexName);
                    // create index file for column
                    if (column.IndexName?.Length > 0)
                    {
                        _logger.Trace($"Создаем индекс файл [{Name}-{column.IndexName}] для колонки {column.Name}");
                        var fs = new FileStream(Path, Database, $"{Name}-{column.IndexName}", FileType.Index);
                        fs.Create();
                        fs.Open();
                        fs.WriteInt(0);
                        fs.Close();
                    }
                }
                // get current position
                var pos = _fileStream.GetPosition();

                // write current position
                _fileStream.SetPosition(0);
                // rewrite fake position
                _fileStream.WriteInt((int)pos);
                _fileStream.Close();
            }
            catch (Exception ex)
            {
                throw new Errors.Error(ex.Message);
            }
            finally
            {
                // close
                _fileStream?.Close();
            }
        }

        private int findPositiontoInsertIndex(Column column, int value)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(Path, Database, $"{Name}-{column.IndexName}", FileType.Index);
                fileStream.Open();
                // read indexes count
                var count = fileStream.ReadInt();
                // return first position if no indexes
                if (count == 0)
                    return 4;
                // calculate index weight
                var indexWeight = sizeof(int) + column.Size + sizeof(byte);

                int first = 0, last = count;

                int min = Int32.MaxValue, minPos = count, intValue = 0, curPos = 0;
                while(first < last)
                {
                    var mid = first + (last - first) / 2;
                    curPos = 4 + mid * indexWeight;
                    fileStream.SetPosition(curPos);
#warning not depends for deleted indexes
                    // skip flag
                    fileStream.ReadByte();
                    // read value
                    intValue = fileStream.ReadInt();
                    // save pos and min diference value
                    var abs = Math.Abs(value - intValue);
                    if (abs < min)
                    {
                        min = abs;
                        minPos = curPos;
                    }
                    if (value <= intValue)
                    {
                        last = mid;
                    }
                    else
                    {
                        first = mid + 1;
                    }
                }
                curPos = 4 + last * indexWeight;
                fileStream.ReadByte();
                intValue = fileStream.ReadInt();
                if (Math.Abs(value - intValue) < min)
                {
                    minPos = curPos;
                }
                return minPos;
            }
            catch (Exception ex)
            {
                throw new Error($"{ex}");
            }
            finally
            {
                // close
                fileStream?.Close();
            }
        }

        private IList<int> readPosition(Column column, Condition condition)
        {
            var positions = new List<int>();
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(Path, Database, $"{Name}-{column.IndexName}", FileType.Index);
                fileStream.Open();
                // read indexes count
                var count = fileStream.ReadInt();
                // calculate index weight
                var indexWeight = sizeof(int) + column.Size + sizeof(byte);

                for (int i = 0; i < count; i++)
                {
                    // set position into index file
                    fileStream.SetPosition(4 + i * indexWeight);
                    // write flag
                    var isDeleted = fileStream.ReadByte();
                    if (isDeleted == 1)
                        continue;
                    bool res = false;
                    switch (column.Type)
                    {
                        case 1:
                            {
                                var val = fileStream.ReadInt();
                                res = condition.Operate(column.Name, val);
                                break;
                            }
#warning not release filter by bit index
                        //case 2:
                        //    {
                        //        var val = Convert.ToInt32(recodValues[kV.Value]);
                        //        res = conditions[0].Operate(col.Name, val);
                        //    }
                        case 3:
                            {
                                var val = fileStream.ReadText(column.Size).TrimStart();
                                res = condition.Operate(column.Name, val);
                                break;
                            }
                        default:
                            res = false;
                            break;
                    }
                    // if no result
                    if (!res)
                        continue;
                    // read position
                    var position = fileStream.ReadInt();
                    positions.Add(position);
                }
                fileStream.Close();
                return positions;
            }
            catch (Exception ex)
            {
                throw new Error($"{ex}");
            }
            finally
            {
                // close
                fileStream?.Close();
            }
        }

        private void writeDenseIndex(Column column, DenseIndex index, int position)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(Path, Database, $"{Name}-{column.IndexName}", FileType.Index);
                fileStream.Open();
                // set position into index file
                fileStream.SetPosition(position);
                var temp = fileStream.CutToEnd(position);
                var indexWeight = sizeof(int) + index.Size + sizeof(byte);

                //fileStream.SetLength(temp.Length + indexWeight);

                //if (count == 0)
                //{
                //    fileStream.SetPosition(4);
                //}
                //else
                //{
                //    // calculate index weight
                //    var indexWeight = sizeof(int) + index.Size + sizeof(byte);
                //    fileStream.SetPosition(4 + count * indexWeight);
                //}
                // write flag
                fileStream.WriteByte(index.Removed);
                // write value
                fileStream.WriteBytes(index.Value);
                // write position into data file
                fileStream.WriteInt(index.RecordPosition);
                if (temp != null)
                    fileStream.WriteBytes(temp);
                _logger.Trace($"Индекс записан.");
                // update index count in the begining of file
                fileStream.SetPosition(0);
                // read indexes count
                var count = fileStream.ReadInt();
                fileStream.SetPosition(0);
                fileStream.WriteInt(++count);
                fileStream.Close();
            }
            catch (Exception ex)
            {
                throw new Error($"{ex}");
            }
            finally
            {
                // close
                fileStream?.Close();
            }

        }

        public void Dispose()
        {
            _fileStream?.Close();
        }
    }
}

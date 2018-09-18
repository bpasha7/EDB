using BinaryFileStream;
using Console;
using DDL.Commands;
using DML.Commands;
using DTO;
using Errors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DBMS
{
    public class DatabaseManagmentSystem
    {
        /// <summary>
        /// System setting from json file
        /// </summary>
        private readonly SystemSettings _settings;
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<DatabaseManagmentSystem> _logger;
        /// <summary>
        /// Current database
        /// </summary>
        private Database _currentDatabase;
        /// <summary>
        /// Inner system timer
        /// </summary>
        private Stopwatch _stopwatch;
        /// <summary>
        /// Constructor for DI
        /// </summary>
        /// <param name="settings">Settings factory</param>
        /// <param name="logger">Logger Factory</param>
        public DatabaseManagmentSystem(IOptions<SystemSettings> settings, ILogger<DatabaseManagmentSystem> logger)
        {
            _logger = logger;
            _logger.LogInformation("Set system settings.");
            _settings = settings.Value;
            _stopwatch = new Stopwatch();
        }
        /// <summary>
        /// Run system
        /// </summary>
        public void Run()
        {
            _logger.LogInformation("Run system.");
            CommandLine.Location = "EDB";
            //CommandLine.WriteError(_settings.Authors);
            _stopwatch.Start();
            var t = new Task(() => ReadCommands());
            t.Start();
            Listen();
            //ReadCommands();
        }
        /// <summary>
        /// Change data base
        /// </summary>
        /// <param name="databaseName">Database name to move</param>
        private void ChangeDatabase(string databaseName = null)
        {
            try
            {
                if (databaseName == null)
                {
                    _currentDatabase = null;
                    return;
                }
                _currentDatabase = new Database(_settings.RootPath, databaseName);
                CommandLine.Location = _currentDatabase.Name;
                _logger.LogInformation($"Change database to {databaseName}.");
            }
            catch (Error error)
            {
                CommandLine.WriteError($"{error}");
                _logger.LogError($"{error}");
            }
            catch (Exception ex)
            {
                CommandLine.WriteError($"System exeption, see log files.");
#if DEBUG
                CommandLine.WriteError($"{ex}");
#endif
                _logger.LogCritical($"{ex}");
            }
            finally
            {
                if (_currentDatabase == null)
                    CommandLine.Location = "EDB";
            }
        }
        #region TCP
        private TcpListener _listener;
        public void Listen()
        {
            _listener = new TcpListener(IPAddress.Any, _settings.Port);
            //_semaphore 
            var task = new Task(() =>
            {
                _listener.Start();
                while (true)
                {
                    TcpClient client = _listener.AcceptTcpClient();
                    NetworkStream ns = client.GetStream();
                    try
                    {
                        var clientTask = new Task(() =>
                        {

                            byte[] bytes = null;
                            if (ns.CanRead)
                            {
                                //получаем длину строки int
                                bytes = new byte[4];
                                ns.Read(bytes, 0, 4);
                            }
                            var length = BitConverter.ToInt32(bytes, 0);
                            if (ns.CanRead)
                            {
                                //поулчаем строку
                                bytes = new byte[length];
                                ns.Read(bytes, 0, length);
                            }
                            var line = System.Text.ASCIIEncoding.UTF8.GetString(bytes, 0, length);

                            var res = ReadCommand(line);
                            var json = JsonConvert.SerializeObject(res, Formatting.Indented);
                            bytes = System.Text.ASCIIEncoding.UTF8.GetBytes(json);
                            byte[] bytesLength = BitConverter.GetBytes(bytes.Length);

                            ////Передаем длину
                            //if (ns.CanWrite)
                            //{
                            //    ns.Write(bytesLength, 0, bytesLength.Length);
                            //}
                            //Передаем строку
                            if (ns.CanWrite)
                            {
                                ns.Write(bytes, 0, bytes.Length);
                            }
                            ns.Close();
                            client.Close();
                        });
                        clientTask.Start();
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex.Message);
                    }
                }
            });
            task.Start();
            task.Wait();
            _listener.Stop();
        }
        #endregion

        TransferObject ReadCommand(string line)
        {
            var to = new TransferObject();
            try
            {
                // split line to words
                var words = line
                    .Trim()
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                // navigate through databases
                if (words[0] == "#")
                {
                    ChangeDatabase(words[1]);
                }
                // show info
                if (words[0].ToLower() == "/info")
                {
                    var res = $"Uptime: {_stopwatch.Elapsed}. Version: {_settings.Version}.";
                    var resData = new ResultData
                    {
                        DataType = ResultDataType.Message,
                        Message = res
                    };
                    CommandLine.WriteInfo(res);
                    to.Data = resData;
                    return to;
                }
                //show databases tree
                if (line.ToLower() == "/show databases")
                {
                    var res = GetSchemes();
                    var resData = new ResultData
                    {
                        DataType = ResultDataType.Message,
                        Message = res
                    };
                    CommandLine.WriteInfo(res);
                    to.Data = resData;
                    return to;
                }
                //show databases sizes
                if (line.ToLower() == "/show databases size")
                {
                    var res = GetDBSizes();
                    var resData = new ResultData
                    {
                        DataType = ResultDataType.Message,
                        Message = res
                    };
                    CommandLine.WriteInfo(res);
                    to.Data = resData;
                    return to;
                }
                //if create database
                if (words[0].ToLower() == "create" && words[1].ToLower() == "database")
                {
                    _logger.LogInformation($"Read command: [{line}].");
                    var cmd = new CreateDatabaseCommand(words);
                    var res = CreateDatabase(cmd.DatebaseName);
                    _logger.LogInformation($"{res}.");
                    CommandLine.WriteInfo(res);
                }
                //if drop database
                if (words[0].ToLower() == "drop" && words[1].ToLower() == "database")
                {
                    _logger.LogInformation($"Read command: [{line}].");
                    var cmd = new DropDatabaseCommand(words);
                    var res = DropDatabase(cmd.DatebaseName);
                    _logger.LogInformation($"{res}.");
                    CommandLine.WriteInfo(res);
                }
                // if create table command
                if (words[0].ToLower() == "create" && words[1].ToLower() == "table")
                {
                    _logger.LogInformation($"Read command: [{line}].");
                    var res = _currentDatabase?.CreateTable(line);
                    _logger.LogInformation($"{res}.");
                    CommandLine.WriteInfo(res);
                }
                // if insert into
                if (words[0].ToLower() == "insert" && words[1].ToLower() == "into")
                {
                    _logger.LogInformation($"Read command: [{line}].");
                    var res = _currentDatabase?.InsertIntoTable(line);
                    _logger.LogInformation($"{res}.");
                    CommandLine.WriteInfo(res);
                    var resData = new ResultData
                    {
                        Message = JsonConvert.SerializeObject(res, Formatting.Indented),
                        DataType = ResultDataType.Message
                    };
                    to.Data = resData;
                    return to;
                }
                // if select from
#warning 'Rewrite condition'
                if (words[0].ToLower() == "select")
                {
//#if Debug
//                    ChangeDatabase("DBtest");
//#endif
                    string info = "";
                    _logger.LogInformation($"Read command: [{line}].");
                    var res = _currentDatabase?.SelectFromTable(words, out info);
                    CommandLine.ShowData(res);
                    _logger.LogInformation($"{info}.");
                    CommandLine.WriteInfo(info);
                    res.DataType = ResultDataType.DataSet;
                    var resData = new ResultData
                    {
                        DataType = ResultDataType.DataSet,
                        Headers = res.Headers,
                        Values = res.Values
                        //Message = JsonConvert.SerializeObject(res, Formatting.Indented)
                    };
                    to.Data = resData;
                    to.Time = info;
                    return to;
                }
                return to;
            }
            catch (Error error)
            {
                CommandLine.WriteError($"{error}");
                _logger.LogError($"{error}");
                var err = new ErrorData
                {
                    Message = $"{error}"
                };
                to.Error = err;
                return to;
            }
            catch (Exception ex)
            {
                CommandLine.WriteError($"System exeption, see log files.");
#if DEBUG
                CommandLine.WriteError($"{ex}");
#endif
                _logger.LogCritical($"{ex}");
                var err = new ErrorData
                {
                    Message = $"{ex}"
                };
                to.Error = err;
                return to;
            }
        }

        public void ReadCommands()
        {
            while (true)
            {
                var line = CommandLine.Wait();
                //if (line == "@")
                //    break;
                ReadCommand(line);
            }
        }

        public string CreateDatabase(string databaseName)
        {
            var dirPath = $"{_settings.RootPath}{databaseName}";
            if (Directory.Exists(dirPath))
                throw new Error($"Database [{databaseName}] is already exist.");
            Directory.CreateDirectory(dirPath);
            return $"Database [{databaseName}] was created.";
        }

        public string DropDatabase(string databaseName)
        {
            var dirPath = $"{_settings.RootPath}{databaseName}";
            if (!Directory.Exists(dirPath))
                throw new Error($"Database [{databaseName}] is not exist.");
            DirectoryInfo di = new DirectoryInfo(dirPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            //foreach (DirectoryInfo dir in di.GetDirectories())
            //{
            //    dir.Delete(true);
            //}
            Directory.Delete(dirPath);
            if (_currentDatabase.Name == databaseName)
                ChangeDatabase();
            return $"Database [{databaseName}] was deleted.";
        }

        public string GetSchemes()
        {
            var dirPath = $"{_settings.RootPath}";
            DirectoryInfo di = new DirectoryInfo(dirPath);
            var dbs = new List<Database>();
            //DataSet dataSet = new DataSet("dataSet");
            //dataSet.Namespace = "NetFrameWork";
            var hashTable = new Hashtable();
            var arr = new List<object[]>();

            foreach (var dir in di.GetDirectories())
            {
                var db = new Database(_settings.RootPath, dir.Name);
                db.LoadTablesInfo();
                arr.Add(new object[] { db.Name, db.Tables.Select(t => t.Name).ToArray() });
            }
            var json = JsonConvert.SerializeObject(arr.ToArray(), Formatting.Indented);
            return json.ToString();
        }

        public string GetDBSizes()
        {
            var dirPath = $"{_settings.RootPath}";
            DirectoryInfo di = new DirectoryInfo(dirPath);

            //var dic = new Dictionary<string, long>();

            var list = new List<DatabaseSizeObject>();

            foreach (var dir in di.GetDirectories())
            {
                string[] a = Directory.GetFiles(dir.FullName, "*.*");
                long b = 0;
                foreach (string name in a)
                {
                    // 3.
                    // Use FileInfo to get length of each file.
                    FileInfo info = new FileInfo(name);
                    b += info.Length;
                }
                // 4.
                // Return total size
                list.Add(new DatabaseSizeObject
                {
                    Name = dir.Name,
                    Size = b / 1024
                });
            }
            var json = JsonConvert.SerializeObject(list, Formatting.Indented);
            return json.ToString();
        }

        public string GetAuthors()
        {
            return _settings.Authors;
        }
    }
}

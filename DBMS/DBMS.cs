using BinaryFileStream;
using Console;
using DDL.Commands;
using DML.Commands;
using DTO;
using Errors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Settings;
using System;
using System.Diagnostics;
using System.IO;

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
            ReadCommands();
        }
        /// <summary>
        /// Change data base
        /// </summary>
        /// <param name="databaseName">Database name to move</param>
        private void ChangeDatabase(string databaseName = null)
        {
            try
            {
                if(databaseName == null)
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
                if(_currentDatabase == null)
                    CommandLine.Location = "EDB";
            }
        }

        public void ReadCommands()
        {
            while(true)
            {
                try
                {
                    var line = CommandLine.Wait();
                    if (line == "@")
                        break;
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
                        CommandLine.WriteInfo($"Uptime: {_stopwatch.Elapsed}. Version: {_settings.Version}.");
                    }
                    //if create database
                    if(words[0].ToLower() == "create" && words[1].ToLower() == "database")
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
                    }
                    // if select from
#warning 'Rewrite condition'
                    if (words[0].ToLower() == "select")
                    {
                        string info = "";
                        _logger.LogInformation($"Read command: [{line}].");
                        var res = _currentDatabase?.SelectFromTable(words, out info);
                        _logger.LogInformation($"{info}.");
                        CommandLine.WriteInfo(info);
                    }
                }
                catch(Error error)
                {
                    CommandLine.WriteError($"{error}");
                    _logger.LogError($"{error}");
                }
                catch(Exception ex)
                {
                    CommandLine.WriteError($"System exeption, see log files.");
#if DEBUG
                    CommandLine.WriteError($"{ex}");
#endif
                    _logger.LogCritical($"{ex}");
                }
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

        public string GetAuthors()
        {
            return _settings.Authors;
        }
    }
}

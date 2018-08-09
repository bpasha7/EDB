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

namespace DBMS
{
    public class DatabaseManagmentSystem
    {
        private readonly SystemSettings _settings;
        private readonly ILogger<DatabaseManagmentSystem> _logger;
        /// <summary>
        /// Current database name
        /// </summary>
        private string CurrentDatabase;

        private Stopwatch _stopwatch;

        public DatabaseManagmentSystem(IOptions<SystemSettings> settings, ILogger<DatabaseManagmentSystem> logger)
        {
            _logger = logger;
            _logger.LogInformation("Set system settings.");
            _settings = settings.Value;
            _stopwatch = new Stopwatch();
        }
        public void Run()
        {
            _logger.LogInformation("Run system.");
            CommandLine.Location = "EDB";
            //CommandLine.WriteError(_settings.Authors);
            _stopwatch.Start();
            ReadCommands();
        }
        private void ChangeLocation(string databaseName)
        {
            // check if database exist
            //if()
            CommandLine.Location = CurrentDatabase = databaseName;
            _logger.LogInformation($"Change database to {databaseName}.");
            //CommandLine.Location = "EDB";
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
                        ChangeLocation(words[1]);
                        //CommandLine.WriteInfo($"");
                    }
                    // show info
                    if (words[0].ToLower() == "/info")
                    {
                        CommandLine.WriteInfo($"Uptime: {_stopwatch.Elapsed}");
                    }
                    // if create table command
                    if (words[0].ToLower() == "create" && words[1].ToLower() == "table")
                    {
                        _logger.LogInformation($"Read command: [{line}].");
                        var res = CreateTable(line);
                        _logger.LogInformation($"{res}.");
                        CommandLine.WriteInfo(res);
                    }
                    // if insert into
                    if (words[0].ToLower() == "insert" && words[1].ToLower() == "into")
                    {
                        _logger.LogInformation($"Read command: [{line}].");
                        var res = InsertIntoTable(line);
                        _logger.LogInformation($"{res}.");
                        CommandLine.WriteInfo(res);
                    }
                    // if select from
#warning 'Rewrite condition'
                    if (words[0].ToLower() == "select")
                    {
                        string info = "";
                        _logger.LogInformation($"Read command: [{line}].");
                        var res = SelectFromTable(words, out info);
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
        public string CreateTable(string query)
        {
            var cmd = new CreateTableCommand(query);
            var table = new Table( CurrentDatabase, cmd.TableName);
            var start =_stopwatch.Elapsed;
            table.Create(cmd);
            var executeTime = _stopwatch.Elapsed - start;
            return $"Table [{cmd.TableName}] was created. Execution time: {executeTime}.";
        }
        public string InsertIntoTable(string query)
        {
            var cmd = new InsertCommand(query);
            var table = new Table(CurrentDatabase, cmd.TableName);
            var start = _stopwatch.Elapsed;
            table.Insert(cmd);
            var executeTime = _stopwatch.Elapsed - start;
            return $"New record was inserted. Execution time: {executeTime}.";
        }
        public ResultData SelectFromTable(string[] words, out string executeInfo)
        {
            var cmd = new SelectCommand(words);
            var table = new Table(CurrentDatabase, cmd.TableName);
            var start = _stopwatch.Elapsed;
            var res = table.Select(cmd);
            var executeTime = _stopwatch.Elapsed - start;
            executeInfo = $"New record was inserted. Execution time: {executeTime}.";
            return res;
        }
        public string GetAuthors()
        {
            return _settings.Authors;
        }
    }
}

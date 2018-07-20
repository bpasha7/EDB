using BinaryFileStream;
using Console;
using DDL.Commands;
using Errors;
using Microsoft.Extensions.Options;
using Settings;
using System;

namespace DBMS
{
    public class DatabaseManagmentSystem
    {
        private readonly SystemSettings _settings;
        /// <summary>
        /// Current database name
        /// </summary>
        private string CurrentDatabase;

        public DatabaseManagmentSystem(IOptions<SystemSettings> settings)
        {
            _settings = settings.Value;
        }
        public void Run()
        {
            CommandLine.Location = "EDB";
            CommandLine.WriteError(_settings.Authors);
            ReadCommands();
        }
        private void ChangeLocation(string databaseName)
        {
            // check if database exist
            //if()
            CommandLine.Location = CurrentDatabase = databaseName;
            //CommandLine.Location = "EDB";

        }
        private void ReadCommands()
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
                        .ToLower()
                        .Trim()
                        .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    // navigate through databases
                    if (words[0] == "#")
                    {
                        ChangeLocation(words[1]);
                        //CommandLine.WriteInfo($"");
                    }
                    // if create table command
                    if (words[0] == "create" && words[1] == "table")
                    {
                        var res = CreateTable(line);
                        CommandLine.WriteInfo(res);
                    }
                }
                catch(Error error)
                {
                    CommandLine.WriteError($"{error}");
                }
                catch(Exception ex)
                {
                    CommandLine.WriteError($"System exeption, see log files.");
#if DEBUG
                    CommandLine.WriteError($"{ex}");
#endif
                }
            }
        }
        public string CreateTable(string query)
        {
            var cmd = new CreateTableCommand(query);
            var table = new Table( CurrentDatabase, cmd.TableName);
            table.Create(cmd);
            return $"Table [{cmd.TableName}] was created.";
        }
        public string GetAuthors()
        {
            return _settings.Authors;
        }
    }
}

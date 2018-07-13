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
        private string CurrentDatabase;

        public DatabaseManagmentSystem(IOptions<SystemSettings> settings)
        {
            _settings = settings.Value;
        }
        public void Run()
        {
            CommandLine.Location = "EDB";
            CommandLine.WriteError(_settings.Authors);
        }
        private void ChangeLocation(string databaseName)
        {
            // check if database exist
            //if()
            CurrentDatabase = databaseName;
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

                    }
                    // if create table
                    if (words[0] == "create" && words[1] == "table")
                    {
                        var cmd = new CreateTableCommand(line);
                        var table = new Table(CurrentDatabase, cmd.TableName);
                    }
                }
                catch(Error error)
                {
                    CommandLine.WriteError($"{error}");
                }
                catch(Exception ex)
                {
                    CommandLine.WriteError($"System exeption, see log files.");

                }
            }
        }
        public string GetAuthors()
        {
            return _settings.Authors;
        }
    }
}

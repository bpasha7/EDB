using Console;
using DDL.Commands;
using Microsoft.Extensions.Options;
using Settings;
using System;

namespace DBMS
{
    public class DatabaseManagmentSystem
    {
        private readonly SystemSettings _settings;

        public DatabaseManagmentSystem(IOptions<SystemSettings> settings)
        {
            _settings = settings.Value;
        }
        public void Run()
        {
            CommandLine.Location = "EDB";
            CommandLine.WriteError(_settings.Authors);

        }
        private void ReadCommands()
        {
            while(true)
            {
                var line = CommandLine.Wait();
                if (line == "@")
                    break;
                // split line to words
                var words = line
                    .ToLower()
                    .Trim()
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                // if create table
                if(words[0] == "create" && words[1] == "table")
                {
                    var cmd = new CreateTableCommand();
                }
            }
        }
        public string GetAuthors()
        {
            return _settings.Authors;
        }
    }
}

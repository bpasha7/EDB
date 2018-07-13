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
                    // if create table
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
                }
            }
        }
        public string CreateTable(string query)
        {
            var cmd = new CreateTableCommand(query);
            // create new data file for new table
            var fileStream = new FileStream(_settings.RootPath, CurrentDatabase, cmd.TableName);
            fileStream.Create();
            fileStream.Close();
            // create new head file for new table
            fileStream = new FileStream(_settings.RootPath, CurrentDatabase, cmd.TableName, true);
            fileStream.Create();
            fileStream.Close();
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
            return $"Table [{cmd.TableName}] was created.";
        }
        public string GetAuthors()
        {
            return _settings.Authors;
        }
    }
}

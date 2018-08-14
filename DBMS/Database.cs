﻿using DDL.Commands;
using DML.Commands;
using DTO;
using Errors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DBMS
{
    /// <summary>
    /// Database
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Database name
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// System root path
        /// </summary>
        private readonly string _path;
        /// <summary>
        /// timer
        /// </summary>
        private Stopwatch _stopwatch;
        /// <summary>
        /// Database class constructor
        /// </summary>
        /// <param name="path">Path to system storage from settings</param>
        /// <param name="name">Database name</param>
        public Database(string path, string name)
        {
            var dirPath = $"{path}{name}";
            if (!Directory.Exists(dirPath))
                throw new Error($"Database [{name}] is not exist.");
            _path = path;
            Name = name;
            _stopwatch = new Stopwatch();
        }
        /// <summary>
        /// Create table into current database
        /// </summary>
        /// <param name="query">SQL-query</param>
        /// <returns></returns>
        public string CreateTable(string query)
        {
            var cmd = new CreateTableCommand(query);
            var table = new Table(_path, Name, cmd.TableName);
            var start = _stopwatch.Elapsed;
            table.Create(cmd);
            var executeTime = _stopwatch.Elapsed - start;
            return $"Table [{cmd.TableName}] was created. Execution time: {executeTime}.";
        }
        /// <summary>
        /// InsertValues into current database table
        /// </summary>
        /// <param name="query">SQL-query</param>
        /// <returns></returns>
        public string InsertIntoTable(string query)
        {
            var cmd = new InsertCommand(query);
            var table = new Table(_path, Name, cmd.TableName);
            var start = _stopwatch.Elapsed;
            table.Insert(cmd);
            var executeTime = _stopwatch.Elapsed - start;
            return $"New record was inserted. Execution time: {executeTime}.";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="words"></param>
        /// <param name="executeInfo"></param>
        /// <returns></returns>
        public ResultData SelectFromTable(string[] words, out string executeInfo)
        {
            var cmd = new SelectCommand(words);
            var table = new Table(_path, Name, cmd.TableName);
            var start = _stopwatch.Elapsed;
            var res = table.Select(cmd);
            var executeTime = _stopwatch.Elapsed - start;
            executeInfo = $"New record was inserted. Execution time: {executeTime}.";
            return res;
        }
    }
}
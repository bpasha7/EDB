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
            Console.WriteLine(_settings.Authors);
        }
        public string GetAuthors()
        {
            return _settings.Authors;
        }
    }
}

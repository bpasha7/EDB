using System;

namespace Console
{
    /// <summary>
    /// Handling system console events
    /// </summary>
    public static class CommandLine
    {
        /// <summary>
        /// Current Location. Using for showing in command line
        /// </summary>
        public static string Location;
        /// <summary>
        /// Write simple text
        /// </summary>
        /// <param name="text"></param>
        static public void Write(string text)
        {
            System.Console.WriteLine(text);
        }
        /// <summary>
        /// Waiting new line
        /// </summary>
        /// <returns>Query</returns>
        static public string Wait()
        {
            System.Console.Write($"{Location}: ");
            return System.Console.ReadLine();
        }
        /// <summary>
        /// Writing error text with red color
        /// </summary>
        /// <param name="text"></param>
        static public void WriteError(string text)
        {
            System.Console.BackgroundColor = ConsoleColor.Red;
            System.Console.WriteLine(text);
            System.Console.ResetColor();
        }
    }
}

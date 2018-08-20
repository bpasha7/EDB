using DTO;
using System;
using System.Linq;
using System.Text;

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
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine(text);
            System.Console.ResetColor();
        }
        /// <summary>
        /// Writing info text with green color
        /// </summary>
        /// <param name="text"></param>
        static public void WriteInfo(string text)
        {
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine(text);
            System.Console.ResetColor();
        }

        static public void ShowData(ResultData resultData)
        {
            var with = (System.Console.WindowWidth - 10 - resultData.Headers.Count) / resultData.Headers.Count;
            printLine();
            var sb = new StringBuilder("|");
            // print each columne name
            for(var i = 0; i < resultData.Headers.Count; i++)
            {
                sb.Append(alignCentre($"{ resultData.Headers[i]} [{ resultData.Types[i]}]", with));
                sb.Append("|");
            }
            WriteInfo(sb.ToString());
            printLine();
            // print each row
            foreach (var row in resultData.Values)
            {
                sb.Clear();
                sb.Append("|");
                foreach (var item in row)
                {
                    sb.Append(alignCentre(item.ToString(), with));
                    sb.Append("|");
                }
                WriteInfo(sb.ToString());
            }
            printLine();
        }

        static private string alignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

        static private void printLine()
        {
            WriteInfo(new string('-', System.Console.WindowWidth - 10));
        }
    }
}

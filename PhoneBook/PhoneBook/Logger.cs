using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBook
{
    internal class Logger : IDisposable
    {
        // Fields and Properties
        // ----------------------------------------------------------------------------------------------
        /// <summary>
        /// Directory where the log is placed
        /// </summary>
        public string FileDirectory { get; }
        
        /// <summary>
        /// Name of the log file
        /// </summary>
        public string FileName { get; }
        
        /// <summary>
        /// Time of log's creation
        /// </summary>
        public DateTime DateCreated { get; }
        
        /// <summary>
        /// StreamWriter used in logging
        /// </summary>
        private StreamWriter _writer;

        
        // Methods
        // ----------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor of the class.
        /// </summary>
        /// <param name="directory">Directory of the logs. Default: <c>logs</c> catalogue in current working directory</param>
        /// <param name="fileName">Name of the log. Default: current date and time</param>
        /// <exception cref="ClosingException">Fatal exception; the app should be closed</exception>
        public Logger (string? directory = null, string? fileName = null)
        {
            try
            {
                DateCreated = DateTime.Now;

                if (directory == null) FileDirectory = Path.Combine(Environment.CurrentDirectory, "logs");
                else FileDirectory = directory;

                if (fileName == null) FileName = DateCreated.ToString("yyyy-MM-dd hhMMss") + ".log";
                else FileName = fileName + ".log";

                // Create a directory if it does not exist
                if (!Directory.Exists(FileDirectory)) Directory.CreateDirectory(FileDirectory);

                _writer = new StreamWriter(Path.Combine(FileDirectory, FileName), new FileStreamOptions() { Access = FileAccess.Write, Mode = FileMode.Create});

                Log("Logger instantiated.", "Logger");
            }
            catch (Exception ex)
            {
                throw new ClosingException("Failed to initaite Logger", ex);
            }
        }

        /// <summary>
        /// Disposing the Logger
        /// </summary>
        public void Dispose()
        {
            Log("Logger closing.", "Logger");
            _writer.Dispose();
            _writer.Close();
        }

        /// <summary>
        /// Log a message
        /// </summary>
        /// <param name="message">Message to write in the log</param>
        /// <param name="source">Source of the message</param>
        public void Log(string message, string source)
        {
            _writer.WriteLine($"Log entry {DateTime.Now.ToString("s")}");
            _writer.WriteLine($"\t> Source: {source}");
            _writer.WriteLine($"\t> Message {message}");
            _writer.WriteLine();
        }
    }
}

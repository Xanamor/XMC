#region Header

//    XMC, a Minecraft SMP server.
//    Copyright (C) 2011 XMC. All rights reserved.
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion Header

namespace XMC.Logger
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;

    class LogWorker : IDisposable
    {
        #region Fields

        bool _disposed;
        string _errorFile;
        Queue<string> _errorQueue = new Queue<string>();
        object _lockObject = new object();
        string _logFile;
        Queue<string> _messageQueue = new Queue<string>();
        Queue<string> _warrningQueue = new Queue<string>();
        Thread _workingThread;

        #endregion Fields

        #region Constructors

        public LogWorker()
        {
            //TODO: Move to Configing classes
            if (!Directory.Exists("log"))
                Directory.CreateDirectory("log");
            _logFile = "log/" + DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt";
            _errorFile = "log/StackTraces" + DateTime.Now.ToShortDateString().Replace("/", "-") + ".txt";
            _workingThread = new Thread(new ThreadStart(WorkerThread));
            _workingThread.IsBackground = true;
            _workingThread.Start();
        }

        #endregion Constructors

        #region Methods

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                lock (_lockObject)
                {
                    if (_errorQueue.Count > 0)
                    {
                        WriteQueue(_logFile, _errorQueue);
                    }

                    _messageQueue.Clear();
                    Monitor.Pulse(_lockObject);
                }
            }
        }

        public void LogError(Exception ex)
        {
            StringBuilder SBuild = new StringBuilder();
            Exception e = ex;

            SBuild.AppendLine("----" + DateTime.Now + "----");

            while (e != null)
            {
                SBuild.AppendLine(getErrorText(e));
                e = e.InnerException;
            }

            SBuild.AppendLine(new string('-', 25));
            lock(_lockObject)
            {
                _errorQueue.Enqueue(SBuild.ToString());
                Monitor.Pulse(_lockObject);
            }
        }

        public void LogMessage(string message)
        {
            try
            {
                if (message != null && message.Length > 0)
                    lock (_lockObject)
                    {
                    _messageQueue.Enqueue(message);
                    Monitor.Pulse(_lockObject);
                    }
                Console.Write(message);
            }
            catch { }
        }

        public void LogWarrning(string message)
        {
            try
            {
                if (message != null && message.Length > 0)
                    lock (_lockObject)
                    {
                        _warrningQueue.Enqueue(message);
                        Monitor.Pulse(_lockObject);
                    }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(message);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            catch { }
        }

        string getErrorText(Exception e)
        {
            StringBuilder SBuild = new StringBuilder();
            if (e != null)
            {
                SBuild.AppendLine("Type: " + e.GetType().Name);
                SBuild.AppendLine("Source: " + e.Source);
                SBuild.AppendLine("Message: " + e.Message);
                SBuild.AppendLine("Target: " + e.TargetSite.Name);
                SBuild.AppendLine("Trace: " + e.StackTrace);
            }
            return SBuild.ToString();
        }

        void WorkerThread()
        {
            while (!_disposed)
            {
                lock (_lockObject)
                {
                    if (_errorQueue.Count > 0)
                        WriteQueue(_errorFile, _errorQueue);
                    if (_warrningQueue.Count > 0)
                        WriteQueue(_logFile, _warrningQueue);
                    if (_messageQueue.Count > 0)
                        WriteQueue(_logFile, _messageQueue);
                    Monitor.Wait(_lockObject, 500);
                }
            }
        }

        void WriteQueue(string path, Queue<string> cache)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                while (cache.Count > 0)
                {
                    byte[] tmp = Encoding.Default.GetBytes(cache.Dequeue());
                    fs.Write(tmp, 0, tmp.Length);
                }
                fs.Close();
            } //catch {   }
            finally
            {
                if (fs != null)
                    fs.Dispose();
            }
        }

        #endregion Methods
    }
}

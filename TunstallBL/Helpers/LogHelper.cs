using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TunstallBL.Helpers
{
    public enum LogMessageType
    {
        INFO,
        TRACE,
        DEBUG,
        ERROR
    }

    public class LogHelper
    {
        #region Private Variables

        string _logFile = string.Empty;

        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();
        #endregion

        #region Public Variables
        #endregion

        #region Constructor
        public LogHelper(string logfile)
        {
            var fileName = Path.GetFileName(logfile);
            var split = fileName.Split('.');
            var newFileName = string.Format("{0}_{1}.{2}", split[0], DateTime.Now.ToString("yyyyMMdd"), split[1]);
            _logFile = newFileName;
        }


        #endregion

        #region Public Functions

        public void LogMessage(LogMessageType type, string message)
        {
            WriteMessageToTextFile(type, message);
        }

        public void LogException(Exception ex)
        {
            WriteMessageToTextFile(LogMessageType.ERROR, ex.Message);
        }

        #endregion

        #region Private Functions

        private void WriteMessageToTextFile(LogMessageType type, string message)
        {
            _readWriteLock.EnterWriteLock();

            try
            {
                string dir = Path.GetDirectoryName(_logFile);
                if (!Directory.Exists(dir) & !string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                if (!File.Exists(_logFile))
                {
                    File.Create(_logFile).Close();
                }


                using (StreamWriter sw = File.AppendText(_logFile))
                {
                    //050118KF Added date 
                    string msg = string.Format("{0} {1}: {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), type.ToString(), message);
                    sw.WriteLine(msg);
                    Console.WriteLine(msg);
                }
            }
            catch (Exception ex) { }
            finally
            {
                _readWriteLock.ExitWriteLock();
            }
        }

        #endregion
    }
}

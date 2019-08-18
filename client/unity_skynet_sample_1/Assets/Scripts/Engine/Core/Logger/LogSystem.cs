
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using gtmInterface;
using UnityEngine;

namespace gtmEngine
{
    public class LogSystem : ILogSystem
    {
        #region 变量

        private bool m_enableSave = false;

        private string m_logFileDir = "";

        private string m_logFileName = "output.log";

        private StreamWriter m_logFileWriter = null;

        #endregion

        #region 接口

        public override void DoInit()
        {
            
        }

        public override void DoUpdate()
        {
           
        }

        public override void DoClose()
        {
            
        }

        protected override void Log(LogCategory category, LogType type, string message)
        {
            DateTime now = DateTime.Now;
            //string msg = "[" + now.ToString("yyyy-MM-dd hh:mm:ss") + "]";
            string msg = "[" + LogCategoryName[(int)category] + "]" + message;

            if (type == LogType.Log)
            {
                UnityEngine.Debug.Log(msg);
            }
            else if (type == LogType.Warning)
            {
                UnityEngine.Debug.LogWarning(msg);
            }
            else if (type == LogType.Error)
            {
                UnityEngine.Debug.LogError(msg);
            }

            LogToFile(msg);
        }

        public override void EnableSave(string logFileDir = null)
        {
            m_enableSave = true;
            m_logFileDir = logFileDir;

            if (string.IsNullOrEmpty(m_logFileDir))
            {
                m_logFileDir = Application.persistentDataPath + "/";
            }
        }

        #endregion

        #region 函数
        
        private string GenLogFileName()
        {
            DateTime now = DateTime.Now;
            string filename = now.GetDateTimeFormats('s')[0].ToString();//2005-11-05T14:06:25
            filename = filename.Replace("-", "_");
            filename = filename.Replace(":", "_");
            filename = filename.Replace(" ", "");
            filename = filename.Replace("T", "_");
            filename += ".log";

            return filename;
        }

        private void LogToFile(string message)
        {
            if (!m_enableSave)
                return;

            if (m_logFileWriter == null)
            {
                if (string.IsNullOrEmpty(m_logFileDir))
                    return;

                string fullpath = m_logFileDir + m_logFileName;
                try
                {
                    m_logFileWriter = File.CreateText(fullpath);
                    m_logFileWriter.AutoFlush = true;
                }
                catch (Exception e)
                {
                    Log(LogCategory.GameEngine, LogType.Error, e.Message);
                    m_logFileWriter = null;
                    return;
                }
            }

            if (m_logFileWriter != null)
            {
                try
                {
                    m_logFileWriter.WriteLine(message);

                    StackTrace st = new StackTrace(true);
                    m_logFileWriter.WriteLine(st.ToString());
                    m_logFileWriter.WriteLine("\n");
                }
                catch (Exception e)
                {
                    Log(LogCategory.GameEngine, LogType.Error, e.Message);
                    return;
                }
            }
        }

        #endregion
    }
}

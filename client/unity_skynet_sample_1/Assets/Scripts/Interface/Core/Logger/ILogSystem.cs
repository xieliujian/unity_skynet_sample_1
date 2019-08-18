using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace gtmInterface
{
    public enum LogCategory
    {
        GameLogic,
        GameEngine,
        Plugin_Xlua,
    }

    public abstract class ILogSystem : IManager
    {
        protected string[] LogCategoryName = {
            "GameLogic",
            "GameEngine",
            "Plugin_Xlua"
        };

        public ILogSystem()
        {
            m_sInstance = this;
        }

        protected static ILogSystem m_sInstance = null;

        public static ILogSystem instance
        {
            get { return m_sInstance; }
        }

        [Conditional("DEBUG")]
        public void Log(LogCategory category, string message)
        {
            Log(category, LogType.Log, message);
        }

        [Conditional("DEBUG")]
        public void LogWarning(LogCategory category, string message)
        {
            Log(category, LogType.Warning, message);
        }

        [Conditional("DEBUG")]
        public void LogError(LogCategory category, string message)
        {
            Log(category, LogType.Error, message);
        }

        public abstract void EnableSave(string logFileDir = null);

        protected abstract void Log(LogCategory category, LogType type, string message);
    }
}

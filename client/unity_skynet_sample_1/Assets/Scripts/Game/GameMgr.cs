using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gtmEngine;
using gtmInterface;

namespace gtmGame
{
    public class GameMgr : ClientSingleton<GameMgr>
    {
        /// <summary>
        /// 网络
        /// </summary>
        private INetManager m_netMgr = new NetManager();

        /// <summary>
        /// 日志
        /// </summary>
        private ILogSystem m_logSystem = new LogSystem();

        /// <summary>
        /// 消息分发
        /// </summary>
        private IMsgDispatcher m_msgDispatcher = new MsgDispatcher();

        public void DoInit()
        {
            m_netMgr.DoInit();

            m_logSystem.EnableSave();
            m_logSystem.DoInit();
            m_msgDispatcher.DoInit();
        }

        public void DoUpdate()
        {
            m_netMgr.DoUpdate();
            m_logSystem.DoUpdate();
            m_msgDispatcher.DoUpdate();
        }

        public void DoClose()
        {
            m_netMgr.DoClose();
            m_logSystem.DoClose();
            m_msgDispatcher.DoClose();
        }
    }
}


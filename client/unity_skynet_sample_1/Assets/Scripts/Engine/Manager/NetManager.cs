using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using gtmEngine.Net;
using gtmInterface;

namespace gtmEngine
{  
    public class NetManager : INetManager
    {
        #region 变量

        /// <summary>
        /// Socket
        /// </summary>
        private SocketClient m_SocketClient = new SocketClient();

        /// <summary>
        /// 事件队列
        /// </summary>
        private static Queue<KeyValuePair<ulong, byte[]>> m_EventQueue = new Queue<KeyValuePair<ulong, byte[]>>();
        
        #endregion

        #region 接口函数

        /// <summary>
        /// 初始化
        /// </summary>
        public override void DoInit()
        {
            if (m_SocketClient == null)
                return;

            m_SocketClient.OnRegister();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public override void DoUpdate()
        {
            UpdateEventQueue();
        }

        public override void DoClose()
        {
            if (m_SocketClient == null)
                return;

            m_SocketClient.OnRemove();
        }

        /// <summary>
        /// 发送链接请求
        /// </summary>
        public override void SendConnect(string address, int port)
        {
            m_SocketClient.SendConnect(address, port);
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public override void CloseSocket()
        {
            m_SocketClient.Close();
        }

        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        public override bool IsConnected()
        {
            if (m_SocketClient == null)
                return false;

            return m_SocketClient.IsConnected();
        }

        /// <summary>
        /// 发送SOCKET消息
        /// </summary>
        public override void SendMessage(ByteBuffer buffer)
        {
            m_SocketClient.SendMessage(buffer);
        }

        /// <summary>
        /// 增加事件
        /// </summary>
        /// <param name="bytearray"></param>
        public override void AddEvent(ulong msgid, byte[] bytearray)
        {
            lock (m_EventQueue)
            {
                m_EventQueue.Enqueue(new KeyValuePair<ulong, byte[]>(msgid, bytearray));
            }
        }

        #endregion

        #region 函数

        /// <summary>
        /// 刷新事件队列
        /// </summary>
        private void UpdateEventQueue()
        {
            if (m_EventQueue.Count <= 0)
                return;

            while (m_EventQueue.Count > 0)
            {
                KeyValuePair<ulong, byte[]> keyvaleupair = m_EventQueue.Dequeue();

                if (MsgDispatcher.instance != null)
                {
                    MsgDispatcher.instance.Dispatcher(keyvaleupair.Key, keyvaleupair.Value);
                }                     
            }
        }

        #endregion
    }
}



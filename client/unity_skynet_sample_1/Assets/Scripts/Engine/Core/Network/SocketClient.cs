using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using gtmInterface;

namespace gtmEngine.Net
{
    public class SocketClient
    {
        #region 常量和枚举

        public enum DisType
        {
            Exception,
            Disconnect,
        }

        private const int MAX_READ = 8192;

        #endregion

        #region 变量

        /// <summary>
        /// tcp client
        /// </summary>
        private TcpClient m_Client = null;

        /// <summary>
        /// network stream
        /// </summary>
        private NetworkStream m_NetStream = null;

        /// <summary>
        /// memory stream
        /// </summary>
        private MemoryStream m_MemStream = null;

        /// <summary>
        /// reader
        /// </summary>
        private BinaryReader m_Reader = null;

        /// <summary>
        /// 网络接收的数据
        /// </summary>
        private byte[] m_ByteBuffer = new byte[MAX_READ];

        #endregion

        #region 函数

        // Use this for initialization
        public SocketClient()
        {
        }

        /// <summary>
        /// 注册代理
        /// </summary>
        public void OnRegister()
        {
            m_MemStream = new MemoryStream();
            m_Reader = new BinaryReader(m_MemStream);
        }

        /// <summary>
        /// 移除代理
        /// </summary>
        public void OnRemove()
        {
            Close();

            if (m_Reader != null)
                m_Reader.Close();

            if (m_MemStream != null)
                m_MemStream.Close();

        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        void ConnectServer(string host, int port)
        {
            m_Client = null;
            m_Client = new TcpClient();
            m_Client.SendTimeout = 1000;
            m_Client.ReceiveTimeout = 1000;
            m_Client.NoDelay = true;

            try
            {
                m_Client.BeginConnect(host, port, new AsyncCallback(OnConnect), null);
            }
            catch (Exception e)
            {
                Close();
                LogSystem.instance.LogError(LogCategory.GameEngine, e.Message);
            }
        }

        /// <summary>
        /// 连接上服务器
        /// </summary>
        void OnConnect(IAsyncResult asr)
        {
            m_NetStream = m_Client.GetStream();
            m_NetStream.BeginRead(m_ByteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
            LogSystem.instance.Log(LogCategory.GameEngine, "======连接========");
        }

        /// <summary>
        /// 写数据
        /// </summary>
        void WriteMessage(byte[] message)
        {
            if (IsConnected())
            {
                m_NetStream.BeginWrite(message, 0, message.Length, new AsyncCallback(OnWrite), null);
            }
            else
            {
                LogSystem.instance.LogError(LogCategory.GameEngine, "client.connected----->>false");
            }
        }

        /// <summary>
        /// 读取消息
        /// </summary>
        void OnRead(IAsyncResult asr)
        {
            int bytesRead = 0;
            try
            {
                if (!IsConnected())
                    return;

                lock (m_Client.GetStream())
                {
                    //读取字节流到缓冲区
                    bytesRead = m_Client.GetStream().EndRead(asr);
                }

                if (bytesRead < 1)
                {
                    //包尺寸有问题，断线处理
                    OnDisconnected(DisType.Disconnect, "bytesRead < 1");
                    return;
                }
                
                //分析数据包内容，抛给逻辑层
                OnReceive(m_ByteBuffer, bytesRead);

                lock (m_Client.GetStream())
                {
                    //分析完，再次监听服务器发过来的新消息
                    Array.Clear(m_ByteBuffer, 0, m_ByteBuffer.Length);   //清空数组
                    m_Client.GetStream().BeginRead(m_ByteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
                }
            }
            catch (Exception ex)
            {
                //PrintBytes();
                OnDisconnected(DisType.Exception, ex.Message);
            }
        }

        /// <summary>
        /// 丢失链接
        /// </summary>
        void OnDisconnected(DisType dis, string msg)
        {
            LogSystem.instance.Log(LogCategory.GameEngine, "OnDisconnected" + msg);
            LogSystem.instance.Log(LogCategory.GameEngine, "======断开连接========");
            Close();   //关掉客户端链接
        }

        /// <summary>
        /// 打印字节
        /// </summary>
        /// <param name="bytes"></param>
        void PrintBytes()
        {
            string returnStr = string.Empty;
            for (int i = 0; i < m_ByteBuffer.Length; i++)
            {
                returnStr += m_ByteBuffer[i].ToString("X2");
            }

            LogSystem.instance.LogError(LogCategory.GameEngine, returnStr);
        }

        /// <summary>
        /// 向链接写入数据流
        /// </summary>
        void OnWrite(IAsyncResult r)
        {
            try
            {
                m_NetStream.EndWrite(r);
            }
            catch (Exception ex)
            {
                LogSystem.instance.LogError(LogCategory.GameEngine, "OnWrite--->>>" + ex.Message);
            }
        }

        /// <summary>
        /// 接收到消息
        /// </summary>
        void OnReceive(byte[] bytes, int length)
        {
            m_MemStream.Seek(0, SeekOrigin.End);
            m_MemStream.Write(bytes, 0, length);

            //Reset to beginning
            m_MemStream.Seek(0, SeekOrigin.Begin);

            while (RemainingBytes() > 2)
            {
                ushort msglen = m_Reader.ReadUInt16();
                if (RemainingBytes() >= msglen)
                {
                    ulong msgid = m_Reader.ReadUInt64();
                    int protocollen = msglen - sizeof(ulong);
                    byte[] bytearray = m_Reader.ReadBytes(protocollen);
                    OnReceivedMessage(msgid, bytearray);
                }
                else
                {
                    m_MemStream.Position = m_MemStream.Position - 2;
                    break;
                }
            }

            byte[] leftover = m_Reader.ReadBytes((int)RemainingBytes());
            m_MemStream.SetLength(0);
            m_MemStream.Write(leftover, 0, leftover.Length);
        }

        /// <summary>
        /// 剩余的字节
        /// </summary>
        private long RemainingBytes()
        {
            return m_MemStream.Length - m_MemStream.Position;
        }

        /// <summary>
        /// 接收到消息
        /// </summary>
        /// <param name="ms"></param>
        void OnReceivedMessage(ulong msgid, byte[] bytearray)
        {
            NetManager.instance.AddEvent(msgid, bytearray);
        }

        /// <summary>
        /// 会话发送
        /// </summary>
        void SessionSend(byte[] bytes)
        {
            WriteMessage(bytes);
        }

        /// <summary>
        /// 关闭链接
        /// </summary>
        public void Close()
        {
            if (m_Client != null)
            {
                if (m_Client.Connected)
                    m_Client.Close();

                m_Client = null;

                LogSystem.instance.Log(LogCategory.GameEngine, "======关闭连接========");
            }
        }

        /// <summary>
        /// 发送连接请求
        /// </summary>
        public void SendConnect(string address, int port)
        {
            ConnectServer(address, port);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        public void SendMessage(ByteBuffer buffer)
        {
            if (!IsConnected())
                return;

            SessionSend(buffer.ToBytes());
            buffer.Close();
        }

        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return (m_Client != null && m_Client.Connected);
        }

        #endregion
    }

}

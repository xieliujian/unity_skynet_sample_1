using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gtmEngine;

namespace gtmInterface
{
    class IFlatBufferProcFun
    {
        public virtual void Invoke(byte[] buf)
        {

        }
    }

    public delegate void MsgProcDelegate<T>(T msg);

    public abstract class IMsgDispatcher : IManager
    {
        public IMsgDispatcher()
        {
            m_sInstance = this;
        }

        protected static IMsgDispatcher m_sInstance = null;

        public static IMsgDispatcher instance
        {
            get { return m_sInstance; }
        }

        public delegate void MsgProcFunc(byte[] bytearray);

        public abstract void Dispatcher(ulong msgid, byte[] bytearray);

        public abstract void RegisterMsg(ulong msgid, MsgProcFunc func);

        public abstract void UnRegisterMsg(ulong msgid);

        public abstract void SendMsg(ulong msgid, byte[] bytearray);
    }
}

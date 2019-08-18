using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gtmInterface;
using System;
using gtmEngine.Net;
using System.Reflection;

namespace gtmEngine
{
    public class MsgDispatcher : IMsgDispatcher
    {
        #region 变量

        private Dictionary<ulong, MsgProcFunc> m_msgProcFuncDict = new Dictionary<ulong, MsgProcFunc>(100);

        #endregion

        #region 继承

        public override void Dispatcher(ulong msgid, byte[] bytearray)
        {
            MsgProcFunc outfunc;
            if (m_msgProcFuncDict.TryGetValue(msgid, out outfunc))
            {
                if (outfunc != null)
                {
                    outfunc(bytearray);
                }
            }
        }

        public override void DoClose()
        {
            
        }

        public override void DoInit()
        {
            
        }

        public override void DoUpdate()
        {
            
        }

        public override void RegisterMsg(ulong msgid, MsgProcFunc func)
        {
            MsgProcFunc outfunc;
            if (!m_msgProcFuncDict.TryGetValue(msgid, out outfunc))
            {
                m_msgProcFuncDict[msgid] = func;
            }
        }

        public override void UnRegisterMsg(ulong msgid)
        {
            m_msgProcFuncDict.Remove(msgid);
        }

        public override void SendMsg(ulong msgid, byte[] bytearray)
        {
            gtmInterface.ByteBuffer buff = new gtmInterface.ByteBuffer();
            UInt16 lengh = (UInt16)(bytearray.Length + sizeof(ulong));
            buff.WriteShort(lengh);
            buff.WriteUlong(msgid);
            buff.WriteBytes(bytearray);

            if (NetManager.instance != null)
            {
                NetManager.instance.SendMessage(buff);
            }
        }

        #endregion
    }
}

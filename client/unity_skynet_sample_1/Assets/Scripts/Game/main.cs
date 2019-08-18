using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gtmInterface;
using gtmEngine;
using gtmEngine.Net;

namespace gtmGame
{
    public class main : MonoBehaviour
    {
        private GameMgr m_gameMgr = new GameMgr();

        public string ipaddress = "192.168.0.102";

        private ulong reqmsgid = 10000;

        private ulong rspmsgid = 10001;

        // Start is called before the first frame update
        void Start()
        {
            m_gameMgr.DoInit();

            IMsgDispatcher.instance.RegisterMsg(rspmsgid, TestMsgProc);

            NetManager.instance.SendConnect(ipaddress, 8888);
        }

        // Update is called once per frame
        void Update()
        {
            m_gameMgr.DoUpdate();
        }

        private void OnDestroy()
        {
            IMsgDispatcher.instance.UnRegisterMsg(rspmsgid);

            m_gameMgr.DoClose();
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 300, 200), "SendTestMsg"))
            {
                SendTestMsg();
            }
        }

        private void SendTestMsg()
        {
            byte[] bytearray = System.Text.Encoding.UTF8.GetBytes("unity_skynet_sample_1 你好");
            IMsgDispatcher.instance.SendMsg(reqmsgid, bytearray);
        }

        private void TestMsgProc(byte[] bytearray)
        {
            string str = System.Text.Encoding.UTF8.GetString(bytearray);
            ILogSystem.instance.Log(LogCategory.GameLogic, str);
        }
    }
}

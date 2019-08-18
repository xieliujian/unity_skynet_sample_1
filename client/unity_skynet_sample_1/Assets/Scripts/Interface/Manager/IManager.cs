using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gtmInterface
{
    public abstract class IManager
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public abstract void DoInit();

        /// <summary>
        /// 刷新
        /// </summary>
        public abstract void DoUpdate();

        /// <summary>
        /// 关闭
        /// </summary>
        public abstract void DoClose();
    }
}

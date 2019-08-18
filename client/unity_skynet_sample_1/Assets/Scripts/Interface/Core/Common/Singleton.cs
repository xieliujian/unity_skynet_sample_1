using System;
using System.Collections;
using System.Diagnostics;

namespace gtmInterface
{
    public class Singleton<T> where T : new()
    {
        protected static T m_sInstance = default(T);

        protected Singleton()
        {
            
        }

        public static T instance
        {
            get
            {
                if (m_sInstance == null)
                {
                    if (m_sInstance == null)
                    {
                        m_sInstance = new T();
                    }
                }

                return m_sInstance;
            }
        }
    }
}
using System;
using System.Collections;
using System.Diagnostics;

namespace gtmGame
{
    public class ClientSingleton<T> where T : new()
    {
        protected static T m_sInstance = default(T);

        protected ClientSingleton()
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
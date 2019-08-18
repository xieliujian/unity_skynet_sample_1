using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace gtmInterface
{
    public class GameEvent : UnityEvent
    {

    }

    public class GameEvent<T0> : UnityEvent<T0>
    {

    }

    public class GameEvent<T0, T1> : UnityEvent<T0, T1>
    {

    }

    public class GameEvent<T0, T1, T2> : UnityEvent<T0, T1, T2>
    {

    }

    public class GameEvent<T0, T1, T2, T3> : UnityEvent<T0, T1, T2, T3>
    {

    }
}


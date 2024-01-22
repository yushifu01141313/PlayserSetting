using System;
using System.Collections.Generic;

public static class EventCenter
{
    //管理所有监听事件
    #region Private Fields
    private static Dictionary<Type, List<Delegate>> eventList = new Dictionary<Type, List<Delegate>>();
    #endregion

    #region Public Methods

    //注册监听事件
    public static void StartListenToEvent<T>(Action<T> addlisten)
    {
        if (!eventList.ContainsKey(typeof(T)))
        {
            List<Delegate> list = new List<Delegate>();
            list.Add(addlisten);
            eventList.Add(typeof(T), list);
        }
        else
        {
            eventList[typeof(T)].Add(addlisten);
        }
    }

    //取消监听
    public static void StopListenToEvent<T>(Action<T> addlisten)
    {
        if (!eventList.ContainsKey(typeof(T)))
        {
            return;
        }
        eventList[typeof(T)].Remove(addlisten);
        if (eventList[typeof(T)].Count == 0)
        {
            eventList.Remove(typeof(T));
        }
    }

    //触发方式一：通过发送指定的类触发委托链
    public static void TriggerEvent(Type eventName)
    {
        List<Delegate> list = new List<Delegate>();
        if (eventList.TryGetValue(eventName, out list))
        {
            foreach (Delegate action in list.ToArray())
            {
                action.DynamicInvoke();
            }
        }
    }
    //触发方式一：发送一个带参数的类的实例
    public static void TriggerEvent<T>(T eventName)
    {
        List<Delegate> list = new List<Delegate>();
        if (eventList.TryGetValue(typeof(T), out list))
        {
            foreach (Delegate action in list.ToArray())
            {
                (action as Action<T>).Invoke(eventName);
            }
        }
    }
    #endregion

    #region Dispose
    public static void ClearEventCenter()
    {
        eventList.Clear();
    }
    #endregion
}

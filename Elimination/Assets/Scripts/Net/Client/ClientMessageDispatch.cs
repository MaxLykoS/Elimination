using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientMessageDispatch 
{
    //  每帧处理消息的数量
    public const int NUM_MSG = 15;

    //  消息队列
    private Queue<Protocol> MsgQueue = new Queue<Protocol>();

    //委托类型
    public delegate void ListenDelegate(Protocol proto);

    //  反复执行的监听表
    private Dictionary<string, ListenDelegate> eventDic = new Dictionary<string, ListenDelegate>();
    //  只执行一次的监听表
    private Dictionary<string, ListenDelegate> onceDic = new Dictionary<string, ListenDelegate>();

    public void Update()
    {
        for (int i = 0; i < NUM_MSG; i++)
        {
            if (MsgQueue.Count > 0)
            {
                DispatchMsgEvent(MsgQueue.Peek());
                lock (MsgQueue)
                {
                    MsgQueue.Dequeue();
                }
            }
            else
                break;
        }
    }

    public Queue<Protocol> GetMsgQ()
    {
        return MsgQueue;
    }

    public void DispatchMsgEvent(Protocol protocol)
    {
        string className = protocol.ClassName;
        Debug.Log("分发消息" + className);

        ListenDelegate method;
        if (eventDic.TryGetValue(className, out method))
        {
            Debug.Log("执行多次消息" + className);
            method(protocol);
        }
        if (onceDic.TryGetValue(className, out method))
        {
            Debug.Log("执行单次消息" + className);
            method(protocol);
            onceDic[className] = null;
            onceDic.Remove(className);
        }
    }

    //添加监听事件
    public void AddListener(string className, ListenDelegate cb)
    {
        Debug.Log("增加方法" + className + ":" + cb.ToString());
        ListenDelegate method;
        if (eventDic.TryGetValue(className, out method))
            method += cb;
        else
            eventDic[className] = cb;
    }

    //添加单词监听事件
    public void AddOnceListener(string className, ListenDelegate cb)
    {
        ListenDelegate method;
        if (onceDic.TryGetValue(className, out method))
            method += cb;
        else
            onceDic[className] = cb;
    }

    //删除监听事件
    public void DelListener(string className, ListenDelegate cb)
    {
        Debug.Log("删除方法" + className + ":" + cb.ToString());
        ListenDelegate method;
        if (eventDic.ContainsKey(className))
        {
            method = eventDic[className];

            method -= cb;

            if (method == null)
            {
                eventDic.Remove(className);
            }
        }
    }

    //删除单次监听事件
    public void DelOnceListener(string className, ListenDelegate cb)
    {
        Debug.Log("删除方法" + className + ":" + cb.ToString());
        ListenDelegate method;
        if (onceDic.ContainsKey(className))
        {
            method = eventDic[className];

            method -= cb;

            if (method == null)
            {
                onceDic.Remove(className);
            }
        }
    }

    public void ClearEventDic()
    {
        eventDic.Clear();
        Debug.Log("清除监听事件");
    }

    public string ShowEventDicElement()
    {
        string str = "";
        foreach (ListenDelegate d in eventDic.Values)
        {
            str += d.ToString();
        }
        return str;
    }
}

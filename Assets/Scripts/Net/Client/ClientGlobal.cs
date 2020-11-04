using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ClientGlobal
{
    private static ClientGlobal instance;
    private List<Action> list_action = new List<Action>();
    private Mutex mutex_actionList = new Mutex();

    public static int UID;
    public static int UdpSendPort;
    public static string ServerIP = "127.0.0.1";
    public static readonly int TcpServerPort = 13001;
    public static readonly int FrameTime = 66;

    public static ClientGlobal Instance
    {
        get
        {
            if (instance == null)
                instance = new ClientGlobal();
            return instance;
        }
    }

    private ClientGlobal()
    { 
        
    }

    public void Destroy()
    {
        instance = null;
    }

    public void AddAction(Action _action)
    {
        mutex_actionList.WaitOne();
        list_action.Add(_action);
        mutex_actionList.ReleaseMutex();
    }

    public void DoForAction()
    {
        mutex_actionList.WaitOne();
        for (int i = 0; i < list_action.Count; i++)
            list_action[i]();
        list_action.Clear();
        mutex_actionList.ReleaseMutex();
    }
}

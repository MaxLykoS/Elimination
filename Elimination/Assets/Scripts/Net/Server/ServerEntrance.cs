using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerEntrance : MonoBehaviour
{
    public ServerTcp ServerTcp = new ServerTcp();
    void Start()
    {
        ServerTcp.StartServer();
    }

    void OnApplicationQuit()
    {
        ServerTcp.ShutDown();
        MatchMgr.Instance.Close();
    }

    void Update()
    {
        MatchMgr.Instance.DoForAction();
    }
}

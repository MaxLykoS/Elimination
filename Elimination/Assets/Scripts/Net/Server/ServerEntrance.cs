using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerEntrance : MonoBehaviour
{
    public ServerTcp ServerTcp;
    void Start()
    {
        ServerTcp = new ServerTcp();
        ServerTcp.StartServer();
    }
}

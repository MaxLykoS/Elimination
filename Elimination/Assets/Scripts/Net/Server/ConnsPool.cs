using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ConnsPool
{
    public static readonly int MAX_CONN_SIZE = 10;
    private Conn[] conns = new Conn[MAX_CONN_SIZE];

    private Queue<int> freeIndexs = new Queue<int>();

    public ConnsPool()
    {
        for (int i = 0; i < conns.Length; i++)
            conns[i] = new Conn(i);

        for (int i = 0; i < MAX_CONN_SIZE; i++)
            freeIndexs.Enqueue(i);
    }

    public void Recycle(Conn conn)
    {
        freeIndexs.Enqueue(conn.index);
    }

    public Conn YieldConn(Socket socket)
    {
        if (freeIndexs.Count == 0)
            return null;
        Conn yielded = conns[freeIndexs.Dequeue()];
        yielded.Init(socket);
        return yielded;
    }

    public void CloseAll()
    {
        foreach (Conn conn in conns)
        {
            if (conn.isUse)
            {
                freeIndexs.Enqueue(conn.index);
                conn.Close();
            }
        }
    }

    public void HeartBeat()
    {
        //Debug.Log("[主定时器执行]");
        long timeNow = Utility.GetTimeStamp();

        for (int i = 0; i < conns.Length; i++)
        {
            Conn conn = conns[i];
            if (!conn.isUse) continue;

            if (conn.lastTickTime < timeNow - ServerConfig.HEART_BEAT_TIME)
            {
                Debug.Log("[心跳引起断开连接]" + conn.GetAdress());
                lock (conn)
                    conn.Close();
            }
        }
    }
}

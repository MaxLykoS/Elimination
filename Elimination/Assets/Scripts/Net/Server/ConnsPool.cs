using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ConnsPool
{
    public static readonly int MAX_CONN_SIZE = 1024;
    private Conn[] conns;

    private Queue<int> freeIndexs;

    public ConnsPool()
    {
        conns = new Conn[MAX_CONN_SIZE];
        for (int i = 0; i < conns.Length; i++)
            conns[i] = new Conn(i);

        freeIndexs = new Queue<int>();
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
        int freeIndex = freeIndexs.Dequeue();
        conns[freeIndex].Init(socket);
        return conns[freeIndex];
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
            if (conn == null) continue;
            if (!conn.isUse) continue;

            if (conn.lastTickTime < timeNow - ServerTcp.hearBeatTime)
            {
                Debug.Log("[心跳引起断开连接]" + conn.GetAdress());
                lock (conn)
                    conn.Close();
            }
        }
    }
}

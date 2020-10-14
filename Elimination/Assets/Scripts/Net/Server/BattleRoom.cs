using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BattleRoom
{
    private List<Conn> group;
    public BattleRoom(List<Conn> group)
    {
        this.group = group;
    }

    public void Begin()
    {
        int randSeed = UnityEngine.Random.Range(0, 100);

        ThreadPool.QueueUserWorkItem((obj) =>
        {
            MatchMessage matchMsg = new MatchMessage(randSeed);
            foreach (Conn conn in group)
            {
                string ip = conn.GetAdress();
                ServerClientUdp clientUdp = new ServerClientUdp(ip);
                Debug.Log("启动UDP客户端");
            }
        });
    }
}
